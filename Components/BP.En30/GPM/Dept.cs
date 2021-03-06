using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using System.IO;

namespace BP.GPM
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityTreeAttr
    {
        /// <summary>
        /// 单位全名
        /// </summary>
        public const string NameOfPath = "NameOfPath";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
        /// <summary>
        /// 全名
        /// </summary>
        public string NameOfPath
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.NameOfPath);
            }
            set
            {
                this.SetValByKey(DeptAttr.NameOfPath, value);
            }
        }
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public new string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        private Depts _HisSubDepts = null;
        /// <summary>
        /// 它的子节点
        /// </summary>
        public Depts HisSubDepts
        {
            get
            {
                if (_HisSubDepts == null)
                    _HisSubDepts = new Depts(this.No);
                return _HisSubDepts;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Port_Dept";
                map.Java_SetEnType(EnType.Admin);

                map.EnDesc = "部門"; //  实体的描述.
                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap(Depositary.Application);    // Map 的存放位置.

                map.AddTBStringPK(DeptAttr.No, null, "ナンバリング", true, true, 1, 50, 20);

                //比如xx分公司财务部
                map.AddTBString(DeptAttr.Name, null, "名前", true, false, 0, 100, 30);

                //比如:\\驰骋集团\\南方分公司\\财务部
                map.AddTBString(DeptAttr.NameOfPath, null, "部門のパス", true, true, 0, 300, 30, true);

                map.AddTBString(DeptAttr.ParentNo, null, "親ノード番号", true, false, 0, 100, 30);
              
                //顺序号.
                map.AddTBInt(DeptAttr.Idx, 0, "シーケンス番号", true, false);

                RefMethod rm = new RefMethod();
                rm.Title = "該部門のパスをリセットします";
                rm.ClassMethodName = this.ToString() + ".DoResetPathName";
                rm.RefMethodType = RefMethodType.Func;

                string msg = "部門の名前が変更されると、部門の名前パス（Port_Dept.NameOfPath）と部門のサブ部門が変更されます";
                msg += "\t\n 部門と部門のサブ部門の間の人員パスも変更しますPort_Emp列DeptDesc.StaDesc。";
                msg += "\t\n 実行してもよろしいですか？?";
                rm.Warning = msg;

                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "同じレベルで部門を追加する";
                rm.ClassMethodName = this.ToString() + ".DoSameLevelDept";
                rm.HisAttrs.AddTBString("No", null, "同じレベルの部門番号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("Name", null, "部署名", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "下位部門を追加する";
                rm.ClassMethodName = this.ToString() + ".DoSubDept";
                rm.HisAttrs.AddTBString("No", null, "同じレベルの部門番号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("Name", null, "部署名", true, false, 0, 100, 100);
                map.AddRefMethod(rm);


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new DeptEmps(), new BP.Port.Emps(),
                   DeptEmpAttr.FK_Dept,
                   DeptEmpAttr.FK_Emp, "対応要員", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");


                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new DeptStations(), new Stations(),
                    DeptStationAttr.FK_Dept,
                    DeptStationAttr.FK_Station, "対応するポスト（タイル）", StationAttr.FK_StationType);

                map.AttrsOfOneVSM.AddGroupListModel(new DeptStations(), new Stations(),
                  DeptStationAttr.FK_Dept,
                  DeptStationAttr.FK_Station, "対応するポスト（ツリー）", StationAttr.FK_StationType);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 创建下级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSubNode()
        {
            Entity en = this.DoCreateSubNode();
            return en.ToJson();
        }

        /// <summary>
        /// 创建同级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSameLevelNode()
        {
            Entity en = this.DoCreateSameLevelNode();
            return en.ToJson();
        }

        public string DoSameLevelDept(string no, string name)
        {
            Dept en = new Dept();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@IDはすでに存在します";

            en.Name = name;
            en.ParentNo = this.ParentNo;
            en.Insert();

            return "成功を増やします。.";
        }
        public string DoSubDept(string no, string name)
        {
            Dept en = new Dept();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@IDはすでに存在します";

            en.Name = name;
            en.ParentNo = this.No;
            en.Insert();

            return "成功を増やします。";
        }
        /// <summary>
        /// 重置部门
        /// </summary>
        /// <returns></returns>
        public string DoResetPathName()
        {
            this.GenerNameOfPath();
            return "正常にリセットしました。";
        }

        /// <summary>
        /// 生成部门全名称.
        /// </summary>
        public void GenerNameOfPath()
        {
            string name = this.Name;

            //根目录不再处理
            if (this.IsRoot == true)
            {
                this.NameOfPath = name;
                this.DirectUpdate();
                this.GenerChildNameOfPath(this.No);
                return;
            }

            Dept dept = new Dept();
            dept.No = this.ParentNo;
            if (dept.RetrieveFromDBSources() == 0)
                return;

            while (true)
            {
                if (dept.IsRoot)
                    break;

                name = dept.Name + Path.DirectorySeparatorChar + name;
                dept = new Dept(dept.ParentNo);
            }
            //根目录
            name = dept.Name + Path.DirectorySeparatorChar + name;
            this.NameOfPath = name;
            this.DirectUpdate();

            this.GenerChildNameOfPath(this.No);

            //更新人员路径信息.
            BP.GPM.Emps emps = new Emps();
            emps.Retrieve(EmpAttr.FK_Dept, this.No);
            foreach (BP.GPM.Emp emp in emps)
                emp.Update();
        }

        /// <summary>
        /// 处理子部门全名称
        /// </summary>
        /// <param name="FK_Dept"></param>
        public void GenerChildNameOfPath(string deptNo)
        {
            Depts depts = new Depts(deptNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (Dept dept in depts)
                {
                    dept.GenerNameOfPath();
                    GenerChildNameOfPath(dept.No);


                    //更新人员路径信息.
                    BP.GPM.Emps emps = new Emps();
                    emps.Retrieve(EmpAttr.FK_Dept, this.No);
                    foreach (BP.GPM.Emp emp in emps)
                        emp.Update();
                }
            }
        }
    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class Depts : EntitiesTree
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }
        public override int RetrieveAll()
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderBy(GPM.DeptAttr.Idx);
            return qo.DoQuery();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dept> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dept> Tolist()
        {
            System.Collections.Generic.List<Dept> list = new System.Collections.Generic.List<Dept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
    }
}
