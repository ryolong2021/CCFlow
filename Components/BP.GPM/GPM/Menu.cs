using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{

    /// <summary>
    /// 控制方式
    /// </summary>
    public enum CtrlWay
    {
        /// <summary>
        /// 游客
        /// </summary>
        Guest,
        /// <summary>
        /// 任何人
        /// </summary>
        AnyOne,
        /// <summary>
        /// 按岗位
        /// </summary>
        ByStation,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按人员
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按sql
        /// </summary>
        BySQL
    }
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 系统根目录
        /// </summary>
        Root,
        /// <summary>
        /// 系统类别
        /// </summary>
        AppSort,
        /// <summary>
        /// 系统
        /// </summary>
        App,
        /// <summary>
        /// 目录
        /// </summary>
        Dir,
        /// <summary>
        /// 菜单
        /// </summary>
        Menu,
        /// <summary>
        /// 功能控制点
        /// </summary>
        Function
    }
    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuAttr : EntityTreeAttr
    {
        /// <summary>
        /// 控制方法
        /// </summary>
        public const string MenuCtrlWay = "MenuCtrlWay";
        /// <summary>
        /// 系统
        /// </summary>
        public const string FK_App = "FK_App";
        /// <summary>
        /// 图片
        /// </summary>
        public const string Img = "Img";
        /// <summary>
        /// 连接
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// 控制内容
        /// </summary>
        public const string CtrlObjs = "CtrlObjs";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 打开方式
        /// </summary>
        public const string OpenWay = "OpenWay";
        /// <summary>
        /// 标记
        /// </summary>
        public const string Flag = "Flag";
        /// <summary>
        /// 扩展1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 扩展2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// Tag3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// 图标
        /// </summary>
        public const string Icon = "Icon";
    }
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : EntityTree
    {
        #region 属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    return uac;
                }
                else
                {
                    uac.Readonly();
                }
                return uac;
            }
        }
        public string CtrlObjs
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.CtrlObjs);
            }
            set
            {
                this.SetValByKey(MenuAttr.CtrlObjs, value);
            }
        }
        //public CtrlWay HisCtrlWay
        //{
        //    get
        //    {
        //        return (CtrlWay)this.GetValIntByKey(MenuAttr.CtrlWay);
        //    }
        //    set
        //    {
        //        this.SetValByKey(MenuAttr.CtrlWay, (int)value);
        //    }
        //}
        /// <summary>
        /// 功能
        /// </summary>
        public MenuType HisMenuType
        {
            get
            {
                return (MenuType)this.GetValIntByKey(MenuAttr.MenuType);
            }
            set
            {
                this.SetValByKey(MenuAttr.MenuType, (int)value);
            }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MenuAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(MenuAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// 打开方式
        /// </summary>
        public string OpenWay
        {
            get
            {
                int openWay = 0;

                switch (openWay)
                {
                    case 0:
                        return "_blank";
                    case 1:
                        return this.No;
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 是否是ccSytem
        /// </summary>
        public MenuType MenuType
        {
            get
            {
                return (MenuType)this.GetValIntByKey(MenuAttr.MenuType);
            }
            set
            {
                this.SetValByKey(MenuAttr.MenuType, (int)value);
            }
        }
        public string FK_App
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.FK_App);
            }
            set
            {
                this.SetValByKey(MenuAttr.FK_App, value);
            }
        }

        public string Flag
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Flag);
            }
            set
            {
                this.SetValByKey(MenuAttr.Flag, value);
            }
        }

        public string Img
        {
            get
            {
                string s = this.GetValStringByKey("WebPath");
                if (DataType.IsNullOrEmpty(s))
                {
                    if (this.HisMenuType == GPM.MenuType.Dir)
                        return "/Images/Btn/View.gif";
                    else
                        return "/Images/Btn/Go.gif";
                }
                else
                {
                    return s;
                }
            }
            set
            {
                this.SetValByKey("WebPath", value);
            }
        }
        public string Url
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Url);
            }
            set
            {
                this.SetValByKey(MenuAttr.Url, value);
            }
        }
        public bool IsCheck = false;
        /// <summary>
        /// 标记
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Tag1);
            }
            set
            {
                this.SetValByKey(MenuAttr.Tag1, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 菜单
        /// </summary>
        public Menu()
        {
        }
        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="mypk"></param>
        public Menu(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            if (this.Flag.Contains("FlowSort") || this.Flag.Contains("Flow"))
            {
                throw new Exception("@削除できませんでした。このアイテムはワークフローメニューであり、削除できません。");
            }

            return base.beforeDelete();
        }
        protected override void afterDelete()
        {
            //删除他的子项目.
            Menus ens = new Menus();
            ens.Retrieve(MenuAttr.ParentNo, this.No);
            foreach (Menu item in ens)
                item.Delete();

            base.afterDelete();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_Menu");  // 类的基本属性.
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "システムメニュー";
                map.EnType = EnType.Sys;
                map.CodeStruct = "4";

                #region 与树有关的必备属性.
                map.AddTBStringPK(MenuAttr.No, null, "機能番号", true, true, 1, 90, 50);
                map.AddDDLEntities(MenuAttr.ParentNo, null, DataType.AppString, "親ノード", new Menus(), "No", "Name", false);
                map.AddTBString(MenuAttr.Name, null, "名前", true, false, 0, 300, 200, true);
                map.AddTBInt(MenuAttr.Idx, 0, "シーケンス番号", true, false);
                #endregion 与树有关的必备属性.
                // 类的字段属性. 
                map.AddDDLSysEnum(MenuAttr.MenuType, 0, "メニュータイプ", true, true, MenuAttr.MenuType,
                    "@0=システムルートディレクトリ@1=システムカテゴリ@2=システム@3=ディレクトリ@4=機能/インターフェース@5=機能制御点");

                // @0=系统根目录@1=系统类别@2=系统.
                map.AddDDLEntities(MenuAttr.FK_App, null, "システム", new Apps(), false);
                map.AddDDLSysEnum(MenuAttr.OpenWay, 1, "開く方式", true, true, MenuAttr.OpenWay, "@0=新しいウィンドウ @1 =このウィンドウ @2=新しいウィンドウを上書き");

                map.AddTBString(MenuAttr.Url, null, "接続", true, false, 0, 3900, 200, true);
                map.AddBoolean(MenuAttr.IsEnable, true, "有効要否?", true, true);
                map.AddTBString(MenuAttr.Icon, null, "Icon", true, false, 0, 500, 50, true);
                map.AddDDLSysEnum(MenuAttr.MenuCtrlWay, 0, "制御する方法", true, true, MenuAttr.MenuCtrlWay,
                    "@0=セットコントロールによると、@1=誰でも使用できます@2= adminユーザーが使用できます");

                map.AddTBString(MenuAttr.Flag, null, "マーク", true, false, 0, 500, 20, false);
                map.AddTBString(MenuAttr.Tag1, null, "Tag1", true, false, 0, 500, 20, true);
                map.AddTBString(MenuAttr.Tag2, null, "Tag2", true, false, 0, 500, 20, true);
                map.AddTBString(MenuAttr.Tag3, null, "Tag3", true, false, 0, 500, 20, true);

                map.AddTBInt(MenuAttr.Idx, 0, "シーケンス番号", true, false);

                //map.AddTBString(EntityNoMyFileAttr.WebPath, "/WF/Img/FileType/IE.gif", "图标", true, false, 0, 200, 20, true);

                map.AddMyFile("アイコン");  //附件.

                map.AddSearchAttr(MenuAttr.FK_App);
                map.AddSearchAttr(MenuAttr.MenuType);
                map.AddSearchAttr(MenuAttr.OpenWay);


                //map.AddDDLSysEnum(AppAttr.CtrlWay, 1, "控制方式", true, true, AppAttr.CtrlWay,
                //    "@0=游客@1=所有人员@2=按岗位@3=按部门@4=按人员@5=按SQL");
                // map.AddTBString(MenuAttr.CtrlObjs, null, "控制内容", false, false, 0, 4000, 20);
                //// 一对多的关系.
                //map.AttrsOfOneVSM.Add(new ByStations(), new Stations(), ByStationAttr.RefObj, ByStationAttr.FK_Station,
                //    StationAttr.Name, StationAttr.No, "可访问的岗位");
                //map.AttrsOfOneVSM.Add(new ByDepts(), new Depts(), ByStationAttr.RefObj, ByDeptAttr.FK_Dept,
                //    DeptAttr.Name, DeptAttr.No, "可访问的部门");
                //map.AttrsOfOneVSM.Add(new ByEmps(), new Emps(), ByStationAttr.RefObj, ByEmpAttr.FK_Emp,
                //    EmpAttr.Name, EmpAttr.No, "可访问的人员");

                #region 基本功能.
                //可以访问的权限组.
                map.AttrsOfOneVSM.Add(new GroupMenus(), new Groups(),
                    GroupMenuAttr.FK_Menu, GroupMenuAttr.FK_Group, EmpAttr.Name, EmpAttr.No, "権限グループにバインド");


                //可以访问的权限组.
                map.AttrsOfOneVSM.Add(new StationMenus(), new BP.GPM.Stations(),
                    StationMenuAttr.FK_Menu, StationMenuAttr.FK_Station, EmpAttr.Name, EmpAttr.No, "ポストリストモードにバインドする");

                //可以访问的权限组.
                map.AttrsOfOneVSM.AddGroupListModel(new StationMenus(), new BP.GPM.Stations(),
                    StationMenuAttr.FK_Menu, StationMenuAttr.FK_Station, "ポストグループモードにバインドする", StationAttr.FK_StationType, "Name", EmpAttr.No);

                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new EmpMenus(), new BP.Port.Emps(),
                   EmpMenuAttr.FK_Menu,
                   EmpMenuAttr.FK_Emp, "人員-ツリー構造をバインドする", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //不带有参数的方法.
                RefMethod rm = new RefMethod();
                rm.Title = "機能権限の増加（追加、削除、変更、チェック）";
                rm.Warning = "増やしてもよろしいですか？";
                rm.ClassMethodName = this.ToString() + ".DoAddRight3";
                rm.IsForEns = true;
                rm.IsCanBatch = true; //是否可以批处理？
                map.AddRefMethod(rm);

                #endregion 基本功能.


                #region 创建菜单.
                rm = new RefMethod();
                rm.GroupName = "作成メニュー（ディレクトリに有効）";
                rm.Title = "伝票を作成する";
                rm.Warning = "本当に作成しますか？";

                rm.HisAttrs.AddTBString("No", null, "伝票番号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("Name", null, "伝票名", true, false, 0, 100, 400);
                rm.HisAttrs.AddTBString("PTable", null, "ストレージテーブル（空は同じ番号を意味します）", true, false, 0, 100, 100);
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "伝票モード", true, true, "BillFrmType", "@0=簡易フォーム@1=カスタマイズフォーム");
                rm.HisAttrs.AddDDLSQL("Sys_FormTree", null, "フォームツリーを選択", "SELECT No,Name FROM Sys_FormTree WHERE ParentNo='1'");

                rm.ClassMethodName = this.ToString() + ".DoAddCCBill";
                map.AddRefMethod(rm);
                #endregion 创建菜单.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 增加单据
        /// </summary>
        /// <param name="no">编号</param>
        /// <param name="name">名称</param>
        /// <param name="ptable">物理表</param>
        /// <param name="frmType">表单类型</param>
        /// <returns></returns>
        public string DoAddCCBill(string no, string name, string ptable, int frmType, string formTree)
        {
            if (this.MenuType != GPM.MenuType.Dir)
                return "err@メニューツリーのノードはディレクトリではないと作成できません。";

            try
            {
                //创建表单.
                if (frmType == 0)
                    BP.Sys.CCFormAPI.CreateFrm(no, name, formTree, Sys.FrmType.FoolForm);
                else
                    BP.Sys.CCFormAPI.CreateFrm(no, name, formTree, Sys.FrmType.FreeFrm);

                //更改单据属性.
                //BP.Frm.FrmBill fb = new BP.Frm.FrmBill(no);
                //fb.No = no;
                //fb.Name = name;
                //fb.PTable = ptable;
                //fb.Update();

                //执行绑定.
               // fb.DoBindMenu(this.No,name);

                return "<a href='../Comm/En.htm?EnName=BP.Frm.FrmBill&No=" + no + "' target=_blank>伝票のプロパティを開く</a>.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 增加增删改查功能权限
        /// </summary>
        /// <returns></returns>
        public string DoAddRight3()
        {
            if (this.Url.Contains("Search.htm") == false && this.Url.Contains("SearchBS.htm") == false)
                return "この機能は検索コンポーネントではないため、機能権限を追加できません。";

            Menu en = this.DoCreateSubNode() as Menu;
            en.Name = "権限を追加";
            en.MenuType = GPM.MenuType.Function; //功能权限.
            en.Url = this.Url;
            en.Tag1 = "Insert";
            en.Update();

            en = this.DoCreateSubNode() as Menu;
            en.Name = "権限を変更";
            en.MenuType = GPM.MenuType.Function; //功能权限.
            en.Url = this.Url;
            en.Tag1 = "Update";
            en.Update();

            en = this.DoCreateSubNode() as Menu;
            en.Name = "権限を削除";
            en.MenuType = GPM.MenuType.Function; //功能权限.
            en.Url = this.Url;
            en.Tag1 = "Delete";
            en.Update();

            return "追加が成功しました。ノードをリフレッシュしてください。";
        }
        /// <summary>
        /// 路径
        /// </summary>
        public string WebPath
        {
            get
            {
                return this.GetValStrByKey(EntityNoMyFileAttr.WebPath);
            }
            set
            {
                this.SetValByKey(EntityNoMyFileAttr.WebPath, value);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            this.WebPath = this.WebPath.Replace("//", "/");
            return base.beforeUpdateInsertAction();
        }
        /// <summary>
        /// 创建下级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSubNode()
        {
            Entity en = this.DoCreateSubNode();
            en.SetValByKey(MenuAttr.FK_App, this.GetValByKey(MenuAttr.FK_App));
            en.Update();

            return en.ToJson();
        }
        /// <summary>
        /// 创建同级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSameLevelNode()
        {
            Entity en = this.DoCreateSameLevelNode();
            en.SetValByKey(MenuAttr.FK_App, this.GetValByKey(MenuAttr.FK_App));
            en.Update();
            return en.ToJson();
        }
    }
    /// <summary>
    /// 菜单s
    /// </summary>
    public class Menus : EntitiesTree
    {
        #region 构造
        /// <summary>
        /// 菜单s
        /// </summary>
        public Menus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Menu();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Menu> ToJavaList()
        {
            return (System.Collections.Generic.IList<Menu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Menu> Tolist()
        {
            System.Collections.Generic.List<Menu> list = new System.Collections.Generic.List<Menu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Menu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
