using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Port
{
	/// <summary>
	/// 工作人员属性
	/// </summary>
	public class EmpAttr: BP.En.EntityNoNameAttr
	{
		#region 基本属性
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";
        ///// <summary>
        ///// 单位
        ///// </summary>
        //public const string FK_Unit = "FK_Unit";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// SID
        /// </summary>
        public const string SID = "SID";
        /// <summary>
        /// 手机号码
        /// </summary>
        public const string Tel = "Tel";
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
                    throw new Exception("@オペレータを取得" + this.No + "部門[" + this.FK_Dept + "]エラーが発生しました、システム管理者がメンテナンス部門に彼を与えなかった可能性があります。@" + ex.Message);
                }
            }
        }
       
        /// <summary>
        /// 部门
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
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpAttr.Tel, value);
            }
        }
        #endregion

        public bool CheckPass(string pass)
        {
            if (this.Pass == pass)
                return true;
            return false;
        }
        /// <summary>
        /// 工作人员
        /// </summary>
        public Emp()
        {
        }
        /// <summary>
        /// 工作人员编号
        /// </summary>
        /// <param name="_No">No</param>
        public Emp(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@照会するオペレーター番号が空です。");
            try
            {
                this.Retrieve();
            }
            catch (Exception ex1)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("@ユーザーまたはパスワードのエラー：[" + no + "]、またはアカウントが無効になっています。 @技術情報（メモリからのクエリのエラー）：ex1" + ex1.Message);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
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
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Emp", "ユーザー");

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "ナンバリング", true, false, 1, 20, 100);
                map.AddTBString(EmpAttr.Name, null, "名前", true, false, 0, 100, 100);
                map.AddTBString(EmpAttr.Pass, "123", "パスワード", false, false, 0, 20, 10);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部門", new BP.Port.Depts(), true);
                map.AddTBString(EmpAttr.SID, null, "SID", false, false, 0, 20, 10);
                map.AddTBString(EmpAttr.Tel, null, "携帯電話番号", false, false, 0, 11, 30);
                #endregion 字段

                map.AddSearchAttr(EmpAttr.FK_Dept); //查询条件.

                ////增加点对多属性 一个操作员的部门查询权限与岗位权限.
                //map.AttrsOfOneVSM.Add(new EmpStations(), new Stations(), 
                //    EmpStationAttr.FK_Emp, EmpStationAttr.FK_Station, DeptAttr.Name, DeptAttr.No, "岗位权限");

                RefMethod rm = new RefMethod();
                rm.Title = "無効にする";
                rm.Warning = "実行してもよろしいですか？";
                rm.ClassMethodName = this.ToString() + ".DoDisableIt";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "有効にする";
                rm.Warning = "実行してもよろしいですか？";
                rm.ClassMethodName = this.ToString() + ".DoEnableIt";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "ログインアカウントを変更";
                rm.Warning = "本当に処理しますか？決定すると、ユーザーの現在のTo-Do情報とその他のフロー情報が新しい番号にリセットされます。";
                rm.HisAttrs.AddTBString("FieldNew", null, "新しいアカウント", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeUserNo";
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 重置当前用户编号
        /// </summary>
        /// <param name="userNo">当前用户编号</param>
        /// <returns>返回重置信息</returns>
        public string DoChangeUserNo(string userNo)
        {
            if (BP.Web.WebUser.No != "admin")
                return "スーパー管理者以外は実行できません。";

            string msg = "";
            int i = 0;
            //更新待办.
            string sql = "update wf_generworkerlist set fk_emp='"+userNo+"' where fk_emp='"+this.No+"'";
            i= BP.DA.DBAccess.RunSQL(sql);
            if (i != 0)
                msg += "@保留中の更新[" + i + "]件。";

            sql = "UPDATE WF_GENERWORKFLOW  SET STARTER='"+userNo+"'  WHERE STARTER='"+this.No+"'";
            i = BP.DA.DBAccess.RunSQL(sql);
            if (i != 0)
                msg += "@フロー登録の更新[" + i + "]件。";


            //更换流程信息的数据表
            BP.WF.Flows fls = new Flows();
            fls.RetrieveAll();
            foreach (Flow fl in fls)
            {
                sql = "UPDATE " + fl.PTable + " SET FlowEnder='" + userNo + "' WHERE FlowEnder='" + this.No + "'";
                i = DBAccess.RunSQL(sql);

                if (i != 0)
                    msg += "@フロー登録の更新[" + i + "]件。";

                sql = "UPDATE  " + fl.PTable + "  SET FlowStarter='" + userNo + "' WHERE FlowStarter='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@フロービジネステーブルの開始者、更新済み[" + i + "]件。";


                sql = "UPDATE  " + fl.PTable + "  SET Rec='" + userNo + "' WHERE Rec='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@フロービジネステーブルレコーダー、更新済み[" + i + "]件。";

                string trackTable = "ND" + int.Parse(fl.No) + "Track";
                sql = "UPDATE  " + trackTable + "  SET EmpFrom='" + userNo + "' WHERE EmpFrom='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@トラックテーブルEmpFrom、更新[" + i + "]件。";


                sql = "UPDATE  " + trackTable + "  SET EmpTo='" + userNo + "' WHERE EmpTo='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@トラックテーブルEmpTo、更新[" + i + "]件。";


                sql = "UPDATE  " + trackTable + "  SET Exer='" + userNo + "' WHERE Exer='" + this.No + "'";
                i = DBAccess.RunSQL(sql);
                if (i != 0)
                    msg += "@トラックテーブルExer、更新[" + i + "]件。";
            }


            //更新其他字段.
            BP.Sys.MapAttrs attrs = new Sys.MapAttrs();
            attrs.RetrieveAll();
            foreach (BP.Sys.MapAttr attr in attrs)
            {
                if (attr.DefValReal.Contains("@WebUser.No") == true)
                {
                    try
                    {
                        BP.Sys.MapData md = new Sys.MapData(attr.FK_MapData);
                        sql = "UPDATE " + md.PTable + " SET " + attr.KeyOfEn + "='" + userNo + "' WHERE " + attr.KeyOfEn + "='" + this.No + "'";
                        i = DBAccess.RunSQL(sql);
                        if (i != 0)
                            msg += "@テーブル[" + md.Name + "],[" + md.PTable + "] [" + attr.KeyOfEn + "]、更新しました[" + i + "]件。";
                    }
                    catch
                    {

                    }
                }
            }
            //人员主表信息-手动修改

            return msg;
        }
        /// <summary>
        /// 执行禁用
        /// </summary>
        public string DoDisableIt()
        {
            WFEmp emp = new WFEmp(this.No);
            emp.UseSta = 0;
            emp.Update();
            return "正常に実行（無効化）されました";
        }
        /// <summary>
        /// 执行启用
        /// </summary>
        public string DoEnableIt()
        {
            WFEmp emp = new WFEmp(this.No);
            emp.UseSta = 1;
            emp.Update();
            return "正常に実行（有効化）されました";
        }

        protected override bool beforeDelete()
        {
            //if (BP.Web.WebUser.IsAdmin == false)
            //    throw new Exception("err@非管理员不能删除.");

            return base.beforeDelete();
        }
        protected override bool beforeUpdate()
        {
            WFEmp emp = new WFEmp(this.No);
            emp.Update();
            return base.beforeUpdate();
        }
        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
    }
	/// <summary>
	/// 工作人员
	/// </summary>
    public class Emps : EntitiesNoName
    {
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
        /// 工作人员s
        /// </summary>
        public Emps()
        {
        }

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
 
