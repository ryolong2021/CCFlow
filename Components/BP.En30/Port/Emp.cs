using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 操作员属性
    /// </summary>
    public class EmpAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// sid
        /// </summary>
        public const string SID = "SID";
        #endregion
    }
    /// <summary>
    /// Emp 的摘要说明。
    /// </summary>
    public class Emp : EntityNoName
    {
        #region 扩展属性
        /// <summary>
        /// 主要的部门。
        /// </summary>
        public Dept HisDept
        {
            get
            {
                try
                {
                    return new Dept(this.FK_Dept);
                }
                catch (Exception ex)
                {
                    throw new Exception("@オペレーターを取得" + this.No + "部門[" + this.FK_Dept + "]エラーが発生しました。システム管理者が彼の保守部門にあげていないかもしれません.@" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(EmpAttr.FK_Dept);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Pass);
            }
            set
            {
                this.SetValByKey(EmpAttr.Pass, value);
            }
        }
        public string SID
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.SID);
            }
            set
            {
                this.SetValByKey(EmpAttr.SID, value);
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 权限管理.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 检查密码(可以重写此方法)
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>是否匹配成功</returns>
        public bool CheckPass(string pass)
        {
            //检查是否与通用密码相符.
            //string gePass = SystemConfig.AppSettings["GenerPass"];
            //if (gePass == pass && DataType.IsNullOrEmpty(gePass) == false)
            //    return true;

            if (SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                //如果是使用webservices校验.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                int i = v.CheckUserNoPassWord(this.No, pass);
                if (i == 1)
                    return true;
                return false;
            }
            else
            {
                //启用加密
                if (SystemConfig.IsEnablePasswordEncryption == true)
                    pass = BP.Tools.Cryptography.EncryptString(pass);

                /*使用数据库校验.*/
                if (this.Pass == pass)
                    return true;

            }
            return false;
        }

        private static byte[] Keys = { 0x12, 0xCD, 0x3F, 0x34, 0x78, 0x90, 0x56, 0x7B };

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string EncryptPass(string pass)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

            byte[] data = Encoding.Unicode.GetBytes(pass);//定义字节数组，用来存储要加密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(Keys, Keys), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      
            CStream.FlushFinalBlock();              //释放加密流      
            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string DecryptPass(string pass)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    
            byte[] data = Convert.FromBase64String(pass);//定义字节数组，用来存储要解密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(Keys, Keys), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     
            CStream.FlushFinalBlock();               //释放解密流      
            return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串 
        }
        #endregion 公共方法

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public Emp()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public Emp(string no)
        {
            if (no == null || no.Length == 0)
                throw new Exception("@照会するオペレーター番号が空です。");

            this.No = no.Trim();
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
                {
                    //登陆帐号查询不到用户，使用职工编号查询。
                    QueryObject obj = new QueryObject(this);
                    obj.AddWhere(EmpAttr.No, no);
                    int i = obj.DoQuery();
                    if (i == 0)
                        i = this.RetrieveFromDBSources();
                    if (i == 0)
                        throw new Exception("@ユーザーまたはパスワードのエラー：[" + no + "]，あるいはアカウントが停止されます。@技術情報（メモリから確認してエラーが発生しました)：ex1=" + ex.Message);
                }
                else
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.PhysicsTable = "Port_Emp"; // 要物理表。
                map.Java_SetDepositaryOfMap(Depositary.Application);    //实体map的存放位置.
                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体存放位置
                map.EnDesc = "ユーザー"; // "ユーザー";
                map.Java_SetEnType(EnType.App);   //实体类型。
                map.IndexField = EmpAttr.FK_Dept;
                #endregion

                #region 字段
                /* 关于字段属性的增加 .. */
                //map.IsEnableVer = true;

                map.AddTBStringPK(EmpAttr.No, null, "ナンバリング", true, false, 1, 20, 30);
                map.AddTBString(EmpAttr.Name, null, "名前", true, false, 0, 200, 30);
                map.AddTBString(EmpAttr.Pass, "123", "パスワード", false, false, 0, 20, 10);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部門", new Port.Depts(), true);
                map.AddTBString(EmpAttr.SID, null, "セキュリティチェックコード", false, false, 0, 36, 36);

                // map.AddTBString("docs", null, "安全校33验码", false, false, 0, 4000, 36);

                #endregion 字段

                map.AddSearchAttr(EmpAttr.FK_Dept);



                this._enMap = map;
                return this._enMap;
            }

        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
        #endregion 构造函数

        #region 重写方法
        protected override bool beforeDelete()
        {

            return base.beforeDelete();
        }
        #endregion 重写方法

        #region 重写查询.
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public override int Retrieve()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetEmp(this.No);
                if (dt.Rows.Count == 0)
                    throw new Exception("@番号(" + this.No + ")の人は存在しません。");
                this.Row.LoadDataTable(dt, dt.Rows[0]);
                return 1;
            }
            else
            {
                return base.Retrieve();
            }
        }
        /// <summary>
        /// 查询.
        /// </summary>
        /// <returns></returns>
        public override int RetrieveFromDBSources()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetEmp(this.No);
                if (dt.Rows.Count == 0)
                    return 0;
                this.Row.LoadDataTable(dt, dt.Rows[0]);
                return 1;
            }
            else
            {
                return base.RetrieveFromDBSources();
            }
        }
        #endregion


        #region 方法测试代码.
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public string ResetPass()
        {
            return "実行成功。";
        }
        /// <summary>
        /// ChangePass
        /// </summary>
        /// <param name="oldpass"></param>
        /// <param name="pass1"></param>
        /// <param name="pass2"></param>
        /// <returns></returns>
        public string ChangePass(string oldpass, string pass1, string pass2)
        {
            if (BP.Web.WebUser.No != this.No)
                return "err@sss";
            return "実行成功。";
        }
        #endregion 方法测试代码.

    }
    /// <summary>
    /// 操作员
    // </summary>
    public class Emps : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Emp();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public Emps()
        {
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public Emps(string deptNo)
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                this.Clear(); //清除缓存数据.
                //获得数据.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetEmpsByDeptNo(deptNo);
                if (dt.Rows.Count != 0)
                    //设置查询.
                    QueryObject.InitEntitiesByDataTable(this, dt, null);
            }
            else
            {
                this.Retrieve(EmpAttr.FK_Dept, deptNo);
            }
        }
        #endregion 构造方法

        public string reseet()
        {
            return "ceshi";
        }

        #region 重写查询,add by stone 2015.09.30 为了适应能够从 webservice 数据源查询数据.
        /// <summary>
        /// 重写查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            //if (BP.Web.WebUser.No != "admin")
            //    throw new Exception("@您没有查询的权限.");

            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                this.Clear(); //清除缓存数据.
                //获得数据.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetEmps();
                if (dt.Rows.Count == 0)
                    return 0;

                //设置查询.
                QueryObject.InitEntitiesByDataTable(this, dt, null);
                return dt.Rows.Count;
            }
            else
            {
                return base.RetrieveAll();
            }
        }
        /// <summary>
        /// 重写重数据源查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                this.Clear(); //清除缓存数据.
                //获得数据.
                var v = DataType.GetPortalInterfaceSoapClientInstance();
                DataTable dt = v.GetEmps();
                if (dt.Rows.Count == 0)
                    return 0;

                //设置查询.
                QueryObject.InitEntitiesByDataTable(this, dt, null);
                return dt.Rows.Count;
            }
            else
            {
                return base.RetrieveAllFromDBSource();
            }
        }
        #endregion 重写查询.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Emp> ToJavaList()
        {
            return (System.Collections.Generic.IList<Emp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Emp> Tolist()
        {
            System.Collections.Generic.List<Emp> list = new System.Collections.Generic.List<Emp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Emp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
