using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port; 
using BP.Port; 
using BP.En;
using BP.Web;
using System.Drawing;
using System.Text;
using System.IO;

namespace BP.WF.Port.SubInc
{
	/// <summary>
	/// 管理员
	/// </summary>
    public class AdminEmpAttr
    {
        #region 基本属性
        /// <summary>
        /// No
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 申请人
        /// </summary>
        public const string Name = "Name";
        public const string LoginData = "LoginData";
        public const string Tel = "Tel";
        /// <summary>
        /// 授权人
        /// </summary>
        public const string Author = "Author";
        /// <summary>
        /// 授权日期
        /// </summary>
        public const string AuthorDate = "AuthorDate";
        /// <summary>
        /// 是否处于授权状态
        /// </summary>
        public const string AuthorWay = "AuthorWay";
        /// <summary>
        /// 授权自动收回日期
        /// </summary>
        public const string AuthorToDate = "AuthorToDate";
        public const string Email = "Email";
        public const string AlertWay = "AlertWay";
        public const string Stas = "Stas";
        public const string Depts = "Depts";
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 所在组织
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string Idx = "Idx";
        public const string FtpUrl = "FtpUrl";
        public const string Style = "Style";
        public const string Msg = "Msg";
        public const string TM = "TM";
        public const string UseSta = "UseSta";
        /// <summary>
        /// 授权的人员
        /// </summary>
        public const string AuthorFlows = "AuthorFlows";
        /// <summary>
        /// 用户状态
        /// </summary>
        public const string UserType = "UserType";
        /// <summary>
        /// 流程根目录
        /// </summary>
        public const string RootOfFlow = "RootOfFlow";
        /// <summary>
        /// 表单根目录
        /// </summary>
        public const string RootOfForm = "RootOfForm";
        /// <summary>
        /// 部门根目录
        /// </summary>
        public const string RootOfDept = "RootOfDept";
        #endregion
    }
	/// <summary>
	/// 管理员
	/// </summary>
    public class AdminEmp : EntityNoName
    {
        #region 基本属性
        public bool IsAdmin
        {
            get
            {
                if (this.No == "admin")
                    return true;

                if (this.UserType == 1 && this.UseSta == 1)
                    return true;

                return false;
            }
        }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        public int UseSta
        {
            get
            {
                return this.GetValIntByKey(AdminEmpAttr.UseSta);
            }
            set
            {
                SetValByKey(AdminEmpAttr.UseSta, value);
            }
        }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType
        {
            get
            {
                return this.GetValIntByKey(AdminEmpAttr.UserType);
            }
            set
            {
                SetValByKey(AdminEmpAttr.UserType, value);
            }
        }
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(AdminEmpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(AdminEmpAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 组织结构
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(AdminEmpAttr.OrgNo);
            }
            set
            {
                SetValByKey(AdminEmpAttr.OrgNo, value);
            }
        }
        public string RootOfDept
        {
            get
            {
                if (this.No == "admin")
                    return "0";

                return this.GetValStringByKey(AdminEmpAttr.RootOfDept);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfDept, value);
            }
        }
        public string RootOfFlow
        {
            get
            {
                if (this.No == "admin")
                    return "0";

                return this.GetValStrByKey(AdminEmpAttr.RootOfFlow);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfFlow, value);
            }
        }
        public string RootOfForm
        {
            get
            {
                if (this.No == "admin")
                    return "0";

                return this.GetValStringByKey(AdminEmpAttr.RootOfForm);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfForm, value);
            }
        }
        #endregion

        #region 构造函数
        public override En.UAC HisUAC
        {
            get
            {
                UAC uac = new En.UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 管理员
        /// </summary>
        public AdminEmp() { }
        /// <summary>
        /// 管理员
        /// </summary>
        /// <param name="no"></param>
        public AdminEmp(string no)
        {
            this.No = no;
            try
            {
                if (this.RetrieveFromDBSources() == 0)
                {
                    Emp emp = new Emp(no);
                    this.Copy(emp);
                    this.Insert();
                }
            }
            catch
            {
                this.CheckPhysicsTable();
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

                Map map = new Map("WF_Emp", "管理者");

                map.AddTBStringPK(AdminEmpAttr.No, null, "口座番号", true, true, 1, 50, 110);
                map.AddTBString(AdminEmpAttr.Name, null, "名前", true, false, 0, 50, 110);
                map.AddDDLEntities(AdminEmpAttr.FK_Dept, null, "本部", new BP.Port.Depts(), false);
                map.AddDDLEntities(AdminEmpAttr.OrgNo, null, "組織", new BP.WF.Port.Incs(), true);

                map.AddDDLSysEnum(AdminEmpAttr.UseSta, 3, "ユーザーステータス", true, true, AdminEmpAttr.UseSta, "@0=無効@1=有効");
                map.AddDDLSysEnum(AdminEmpAttr.UserType, 3, "ユーザータイプ", true, true, AdminEmpAttr.UserType, "@0=通常のユーザー@1=管理者ユーザー");

                map.AddDDLEntities(AdminEmpAttr.RootOfFlow, null, "フロー権限ノード", new BP.WF.Template.FlowSorts(), false);
                map.AddDDLEntities(AdminEmpAttr.RootOfForm, null, "フォーム許可ノード", new BP.WF.Template.SysFormTrees(), false);
                map.AddDDLEntities(AdminEmpAttr.RootOfDept, null, "組織構造権限ノード", new BP.WF.Port.Incs(), false);
                 

                //查询条件.
                map.AddSearchAttr(AdminEmpAttr.UseSta);
                map.AddSearchAttr(AdminEmpAttr.UserType);


                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "暗号化パスワードを設定する";
                rm.HisAttrs.AddTBString("FrmID", null, "パスワードを入力する", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("FrmwID", null, "ewww", true, false, 0, 100, 100);
                rm.Warning = "パスワードを変更してもよろしいですか？";
                rm.ClassMethodName = this.ToString() + ".DoSetPassword";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "管理者を追加";
                rm.HisAttrs.AddTBString("emp", null, "管理者アカウント", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("OrgNo", null, "管理可能な組織構造コード", true, false, 0, 100, 100);
                rm.RefMethodType = RefMethodType.Func;
                rm.ClassMethodName = this.ToString() + ".DoAdd";
                map.AddRefMethod(rm);


             

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法
        protected override bool beforeUpdateInsertAction()
        {
            if (this.No == "admin")
            {
                this.RootOfDept = "0";
                this.RootOfFlow = "0";
                this.RootOfForm = "0";
            }
           
            return base.beforeUpdateInsertAction();
        }
        #endregion

        public string DoAdd(string empNo, string orgNo)
        {

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = empNo;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@管理者が追加できませんでした、ID=" + empNo + "ユーザーテーブルはありません。追加する管理者はPort_Empユーザーテーブルに存在している必要があります。";

            BP.Port.Dept dept = new BP.Port.Dept();
            dept.No = orgNo;
            if (dept.RetrieveFromDBSources() == 0)
                return "err@orgNoエラー、Port_Deptに存在しません。";

            BP.WF.Port.Inc inc = new BP.WF.Port.Inc();
            inc.No = orgNo;
            if (inc.RetrieveFromDBSources() == 0)
                return "err@orgNoエラー、Port_Incに存在しません。";

            //求根目录流程树.
            BP.WF.Template.FlowSort fsRoot = new WF.Template.FlowSort();
            fsRoot.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo,"0");


            BP.WF.Template.FlowSort fs = new WF.Template.FlowSort();
            fs.No = "Inc" + orgNo;
            if (fs.RetrieveFromDBSources() == 1)
                return "err@組織はフローツリーディレクトリを初期化しました。";

            fs.Name = dept.Name+"-フローツリー";
            fs.ParentNo = fsRoot.No;
            fs.OrgNo = dept.No;
            fs.Insert();


            //求根目录流程树.
            BP.Sys.FrmTree frmRoot = new BP.Sys.FrmTree();
            frmRoot.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");

            BP.Sys.FrmTree frmTree = new BP.Sys.FrmTree();
            frmTree.No = "Inc" + orgNo;
            if (frmTree.RetrieveFromDBSources() == 1)
                return "err@組織はフォームツリーディレクトリを初期化しました。";

            frmTree.ParentNo = frmRoot.No;
            frmTree.Name = dept.Name + "-フォームツリー";
            frmTree.OrgNo = dept.No;
            frmTree.Insert();
          

            AdminEmp ae = new AdminEmp();
            ae.No = empNo;
            if (ae.RetrieveFromDBSources() == 1)
            {
                if (ae.IsAdmin == true)
                    return "err@管理者は既に存在します。管理者を削除して、再追加してください。delete from wf_emp where no='" + empNo + "'";
                ae.Delete();
            }

           

            ae.Copy(emp);

            ae.UserType = 1;
            ae.UseSta = 1;
            ae.RootOfDept = orgNo;
            ae.RootOfFlow = "Inc" + orgNo;
            ae.RootOfForm = "Inc" + orgNo;
            ae.Insert();

            return "info@管理者が正常に追加されました。";
        }

        /// <summary>
        /// 设置加密密码存储
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string DoSetPassword(string password)
        {
            string str = BP.Tools.Cryptography.EncryptString(password);

            BP.Port.Emp en = new BP.Port.Emp(this.No);
            en.Pass = str;
            en.Update();

            DBAccess.RunSQLReturnVal("UPDATE Port_Emp SET Pass='" + str + "' WHERE No='" + this.No + "'");
            return "設定は成功しました。.";
        }
        
    }
	/// <summary>
	/// 管理员s 
	/// </summary>
	public class AdminEmps : EntitiesNoName
	{	 
		#region 构造
		/// <summary>
		/// 管理员s
		/// </summary>
		public AdminEmps()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new AdminEmp();
			}
		}

        public override int RetrieveAll()
        {
            return base.RetrieveAll("FK_Dept","Idx");
        }
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AdminEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<AdminEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AdminEmp> Tolist()
        {
            System.Collections.Generic.List<AdminEmp> list = new System.Collections.Generic.List<AdminEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AdminEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

	}
	
}
