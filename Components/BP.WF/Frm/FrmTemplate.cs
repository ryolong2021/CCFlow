using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.GPM;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Sys;
using System.Collections.Generic;

namespace BP.Frm
{
    /// <summary>
    /// 单据模版 - Attr
    /// </summary>
    public class FrmTemplateAttr : FrmAttr
    {
    }
    /// <summary>
    /// 单据模版
    /// </summary>
    public class FrmTemplate : EntityNoName
    {
        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = true;
                    return uac;
                }
                uac.Readonly();
                return uac;
            }
        }
        #endregion 权限控制.

        #region 属性
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// 实体类型：@0=单据@1=编号名称实体@2=树结构实体
        /// </summary>
        public EntityType EntityType
        {
            get
            {
                return (EntityType)this.GetValIntByKey(FrmTemplateAttr.EntityType);
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.EntityType, (int)value);
            }
        }
        /// <summary>
        /// 表单类型 (0=傻瓜，2=自由 ...)
        /// </summary>
        public FrmType FrmType
        {
            get
            {
                return (FrmType)this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, (int)value);
            }
        }
        /// <summary>
        /// 表单树
        /// </summary>
        public string FK_FormTree
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FormTree);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FormTree, value);
            }
        }
        /// <summary>
        /// 新建模式 @0=表格模式@1=卡片模式@2=不可用
        /// </summary>
        public int BtnNewModel
        {
            get
            {
                return this.GetValIntByKey(FrmTemplateAttr.BtnNewModel);
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.BtnNewModel, value);
            }
        }
        
        /// <summary>
        /// 单据格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                string str = this.GetValStrByKey(FrmTemplateAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "{LSH4}";
                return str;
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                string str = this.GetValStrByKey(FrmTemplateAttr.TitleRole);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "@WebUser.FK_DeptName @WebUser.Name @RDT";
                return str;
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.BillNoFormat, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 单据模版
        /// </summary>
        public FrmTemplate()
        {
        }
        /// <summary>
        /// 单据模版
        /// </summary>
        /// <param name="no">映射编号</param>
        public FrmTemplate(string no)
            : base(no)
        {
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
                Map map = new Map("Sys_MapData", "帳票テンプレート");
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "フォーム番号", true, true, 1, 190, 20);
                map.SetHelperAlert(MapDataAttr.No, "フォームIDとも呼ばれ、システムには一つしかない");

                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "フォームタイプ", true, true, "BillFrmType", "@0=簡易フォーム @ 1 = 自由フォーム");
                map.AddTBString(MapDataAttr.PTable, null, "ストレージテーブル", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapDataAttr.PTable, "存在しないストレージテーブルを変更すると、システムは自動的にテーブルが作成されます。");

                map.AddTBString(MapDataAttr.Name, null, "フォーム名", true, false, 0, 200, 20, true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "フォームのカテゴリ", new SysFormTrees(), false);

                map.AddDDLSysEnum(FrmAttr.RowOpenModel, 0, "行ロギングオープンモード", true, true,
                    FrmAttr.RowOpenModel, "@0=新しいウィンドウが開く@1=ポップアップウィンドウが開き、リストは閉じた後に更新される@2=ポップアップウィンドウが開き、リストは閉じた後に更新されない");
                #endregion 基本属性.

                #region 单据模版.
                map.AddDDLSysEnum(FrmTemplateAttr.EntityType, 0, "事業の種類", true, false, FrmTemplateAttr.EntityType,
                   "@0=独立形式@1=帳票@2=番号名エンティティ@3=ツリー構造エンティティ");
                map.SetHelperAlert(FrmTemplateAttr.EntityType, "エンティティのタイプ、@0=帳票@1=番号名エンティティ@2=ツリー構造エンティティ.");

                map.AddDDLSysEnum(FrmAttr.EntityShowModel, 0, "ディスプレイモード", true, true, FrmAttr.EntityShowModel, "@0=フォーム @1=トランクモード");

                map.AddTBString(FrmTemplateAttr.BillNoFormat, null, "エンティティの番号付け規則", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmTemplateAttr.BillNoFormat, "\t\nエンティティの番号付け規則: \t\n 2ロゴ:01,02,03など, 3ロゴ:001,002,003,など...");
                #endregion 单据模版.

                #region 实体属性
                map.AddTBInt(FrmTemplateAttr.EntityEditModel, 0, "編集モード", true, false);
                //map.AddDDLSysEnum(FrmAttr.EntityEditModel, 0, "编辑模式", true, true, FrmAttr.EntityEditModel, "@0=只读列表模式@1=Table编辑模式");
                #endregion 实体属性.

                #region 可以创建的权限.
                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new StationCreates(), new BP.WF.Port.Stations(),
                    StationCreateAttr.FrmID,
                    StationCreateAttr.FK_Station, "作成可能な部署", StationAttr.FK_StationType);

                map.AttrsOfOneVSM.AddGroupListModel(new StationCreates(), new BP.WF.Port.Stations(),
                  StationCreateAttr.FrmID,
                  StationCreateAttr.FK_Station, "作成可能な部署Add GroupListModel", StationAttr.FK_StationType);

                //节点绑定部门. 节点绑定部门.
                map.AttrsOfOneVSM.AddBranches(new FrmDeptCreates(), new BP.Port.Depts(),
                   FrmDeptCreateAttr.FrmID,
                   FrmDeptCreateAttr.FK_Dept, "作成できる部門AddBranches", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new EmpCreates(), new BP.Port.Emps(),
                   EmpCreateAttr.FrmID,
                   EmpCreateAttr.FK_Emp, "作成可能な人員", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");
                #endregion 可以创建的权限

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
            
    }
    /// <summary>
    /// 单据模版s
    /// </summary>
    public class FrmTemplates : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 单据模版s
        /// </summary>
        public FrmTemplates()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmTemplate();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmTemplate> Tolist()
        {
            System.Collections.Generic.List<FrmTemplate> list = new System.Collections.Generic.List<FrmTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
