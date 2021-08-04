using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Sys;
using System.Collections.Generic;

namespace BP.Frm
{
    /// <summary>
    /// 实体表单 - Attr
    /// </summary>
    public class FrmDictAttr : FrmAttr
    {
    }
    /// <summary>
    /// 实体表单
    /// </summary>
    public class FrmDict : EntityNoName
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
                return (EntityType)this.GetValIntByKey(FrmDictAttr.EntityType);
            }
            set
            {
                this.SetValByKey(FrmDictAttr.EntityType, (int)value);
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
                return this.GetValIntByKey(FrmDictAttr.BtnNewModel);
            }
            set
            {
                this.SetValByKey(FrmDictAttr.BtnNewModel, value);
            }
        }
        
        /// <summary>
        /// 单据格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                string str = this.GetValStrByKey(FrmDictAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "{LSH4}";
                return str;
            }
            set
            {
                this.SetValByKey(FrmDictAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                string str = this.GetValStrByKey(FrmDictAttr.TitleRole);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "@WebUser.FK_DeptName @WebUser.Name @RDT";
                return str;
            }
            set
            {
                this.SetValByKey(FrmDictAttr.BillNoFormat, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 实体表单
        /// </summary>
        public FrmDict()
        {
        }
        /// <summary>
        /// 实体表单
        /// </summary>
        /// <param name="no">映射编号</param>
        public FrmDict(string no)
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
                Map map = new Map("Sys_MapData", "エンティティフォーム");
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "フォーム番号", true, true, 1, 190, 20);
                map.SetHelperAlert(MapDataAttr.No, "フォームIDとも呼ばれ、システムには一つしかない.");

                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "フォームタイプ", true, true, "BillFrmType", "@0=簡易フォーム @1 = 自由フォーム @ 8 =開発者フォーム");
                map.AddTBString(MapDataAttr.PTable, null, "ストレージテーブル", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapDataAttr.PTable, "存在しないストレージテーブルを変更すると、はシステム自動的にテーブルが作成されます。");

                map.AddTBString(MapDataAttr.Name, null, "フォーム名", true, false, 0, 200, 20, true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "フォームのカテゴリ", new SysFormTrees(), false);
                #endregion 基本属性.

                #region 外观.
                map.AddDDLSysEnum(FrmAttr.RowOpenModel, 0, "行ロギングオープンモード", true, true,
                  "RowOpenMode", "@0=新しいウィンドウを開く@1=このウィンドウで開く@2=ポップアップウィンドウを開く、閉じた後にリストを更新しない@3=ポップアップウィンドウを開く、閉じた後にリストを更新する");
                map.AddTBInt(FrmAttr.PopHeight, 500, "ポップアップの高さ", true, false);
                map.AddTBInt(FrmAttr.PopWidth, 760, "ポップアップウィンドウの幅", true, false);

                map.AddDDLSysEnum(MapDataAttr.TableCol, 0, "フォームに表示される列の数", true, true, "簡易フォームディスプレイ",
                  "@0=4カラム@1= 6カラム@2= 3カラム（上下モード）");

                map.AddDDLSysEnum(FrmAttr.EntityEditModel, 0, "編集モード", true, true, FrmAttr.EntityEditModel, "@0=フォーム @1=行編集");
                map.SetHelperAlert(FrmAttr.EntityEditModel,"エンティティリストを開いて編集する方法0 =読み取り専用クエリモードSearchDict.htm、1 =行編集モードSearchEditer.htm");
                #endregion 外观.

                #region 实体表单.
                map.AddDDLSysEnum(FrmDictAttr.EntityType, 0, "事業の種類", true, false, FrmDictAttr.EntityType,
                   "@0=独立形式@1=帳票@2=番号名エンティティ@3=ツリー構造エンティティ");
                map.SetHelperAlert(FrmDictAttr.EntityType, "エンティティのタイプ、@0=帳票@1=番号名エンティティ@2=ツリー構造エンティティ.");

                map.AddTBString(FrmDictAttr.BillNoFormat, null, "エンティティの番号付け規則", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmDictAttr.BillNoFormat, "\t\nエンティティの番号付け規則: \t\n 2ロゴ:01,02,03など, 3ロゴ:001,002,003,など...");

                #endregion 实体表单.

                #region MyBill - 按钮权限.
                map.AddTBString(FrmDictAttr.BtnNewLable, "新規", "新規", true, false, 0, 50, 20);
                map.AddDDLSysEnum(FrmDictAttr.BtnNewModel, 0, "新規モード", true, true, FrmDictAttr.BtnNewModel,
                   "@0=フォームモード@1=カードモード@2=使用不可",true);


                map.AddTBString(FrmDictAttr.BtnSaveLable, "保存", "保存", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnSaveEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmDictAttr.BtnSubmitLable, "コミット", "コミット", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnSubmitEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmDictAttr.BtnDelLable, "削除", "削除", true, false, 0, 50, 20);
               // map.AddBoolean(FrmDictAttr.BtnDelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmDictAttr.BtnSearchLabel, "リスト", "リスト", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnSearchEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmDictAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnGroupEnable, false, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnPrintHtml, "HTMLを印刷", "HTMLを印刷", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnPrintHtmlEnable, false, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnPrintPDF, "PDFを印刷", "PDFを印刷", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnPrintPDFEnable, false, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnPrintRTF, "RTFを印刷", "RTFを印刷", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnPrintRTFEnable, false, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnPrintCCWord, "CCWordを印刷", "CCWordを印刷", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnPrintCCWordEnable, false, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnExpZip, "zipファイルをエクスポート", "zipファイルをエクスポート", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnExpZipEnable, false, "使えるかどうか？", true, true);
                #endregion 按钮权限.

                #region 查询按钮权限.
                map.AddTBString(FrmDictAttr.BtnImpExcel, "Excelファイルをインポート", "Excelファイルをインポート", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnImpExcelEnable, true, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnExpExcel, "Excelファイルをエクスポート", "Excelファイルをエクスポート", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnExpExcelEnable, true, "使えるかどうか？", true, true);

                map.AddTBString(FrmDictAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                map.AddBoolean(FrmDictAttr.BtnGroupEnable, true, "使えるかどうか？", true, true);

                #endregion 查询按钮权限.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "デザイナー", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "連絡先", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "会社", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "バージョンナンバー", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapDataAttr.Note, null, "備考", true, false, true);
                map.AddTBInt(MapDataAttr.Idx, 100, "シーケンス番号", false, false);
                #endregion 设计者信息.

                #region 扩展参数.
                map.AddTBAtParas(3000); //参数属性.
                map.AddTBString(FrmDictAttr.Tag0, null, "Tag0", false, false, 0, 500, 20);
                map.AddTBString(FrmDictAttr.Tag1, null, "Tag1", false, false, 0, 4000, 20);
                map.AddTBString(FrmDictAttr.Tag2, null, "Tag2", false, false, 0, 500, 20);
                #endregion 扩展参数.

                #region 基本功能.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "デザインフォーム"; // "デザインフォーム";
                rm.ClassMethodName = this.ToString() + ".DoDesigner";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
              //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "帳票URLのAPI"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAPI";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "データ(フォーム)を開く"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoOpenBillDict";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "データ(行編集)を開く"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoOpenBillEditer";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "メニューディレクトリにバインドするく"; // "设计表单";
                rm.HisAttrs.AddDDLSQL("MENUNo", null, "メニューディレクトリを選択", "SELECT No,Name FROM GPM_Menu WHERE MenuType=3");
                rm.HisAttrs.AddTBString("Name", "@Name", "メニュー名", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoBindMenu";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フォームイベント"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoEvent";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "実行方法"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoMethod";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);             
                #endregion 基本功能.

                #region 权限规则.
                rm = new RefMethod();
                rm.Title = "作成ルール"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCreateRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnNewLable;
                rm.GroupName = "許可ルール";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "保存ルール"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSaveRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnSaveLable;
                rm.GroupName = "許可ルール";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "コミットルール"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSubmitRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnSubmitLable;
                rm.GroupName = "許可ルール";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "削除ルール"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDeleteRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnDelLable;
                rm.GroupName = "許可ルール";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "クエリ権限"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearchRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnSearchLabel;
                rm.GroupName = "許可ルール";
                map.AddRefMethod(rm);
                #endregion

                #region 报表定义.
                rm = new RefMethod();
                rm.GroupName = "帳票定義";
                rm.Title = "表示される列を設定する"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_ColsChose";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "帳票定義";
                rm.Title = "列の順序"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_ColsIdxAndLabel";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //   map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "帳票定義";
                rm.Title = "クエリ条件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_SearchCond";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 报表定义.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override void afterInsertUpdateAction()
        {
            //保存权限表
            CtrlModel ctrl = new CtrlModel();

            ctrl.FrmID = this.No;
            ctrl.CtrlObj = "BtnNew";
            ctrl.IsEnableAll = true;
            ctrl.MyPK = ctrl.FrmID + "_" + ctrl.CtrlObj;
            ctrl.Save();

            ctrl = new CtrlModel();
            ctrl.FrmID = this.No;
            ctrl.CtrlObj = "BtnSave";
            ctrl.IsEnableAll = true;
            ctrl.MyPK = ctrl.FrmID + "_" + ctrl.CtrlObj;
            ctrl.Save();

            ctrl = new CtrlModel();
            ctrl.FrmID = this.No;
            ctrl.CtrlObj = "BtnSubmit";
            ctrl.IsEnableAll = true;
            ctrl.MyPK = ctrl.FrmID + "_" + ctrl.CtrlObj;
            ctrl.Save();

            ctrl = new CtrlModel();
            ctrl.FrmID = this.No;
            ctrl.CtrlObj = "BtnDelete";
            ctrl.IsEnableAll = true;
            ctrl.MyPK = ctrl.FrmID + "_" + ctrl.CtrlObj;
            ctrl.Save();

            ctrl = new CtrlModel();
            ctrl.FrmID = this.No;
            ctrl.CtrlObj = "BtnSearch";
            ctrl.IsEnableAll = true;
            ctrl.MyPK = ctrl.FrmID + "_" + ctrl.CtrlObj;
            ctrl.Save();

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 检查enittyNoName类型的实体
        /// </summary>
        public void CheckEnityTypeAttrsFor_EntityNoName()
        {
            //取出来全部的属性.
            MapAttrs attrs = new MapAttrs(this.No);

            #region 补充上流程字段到 NDxxxRpt.
            if (attrs.Contains(this.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.KeyOfEn = "OID";
                attr.Name = "主キーID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.BillNo;
                attr.Name = "ナンバリング"; //  单据ナンバリング
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 名称 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.Title; // "FlowEmps";
                attr.Name = "名前"; //   单据模式， ccform的模式.
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 400;
                attr.Idx = -90;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_BillState") == false)
            {
                /* 单据状态 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "BillState"; // "FlowEmps";
                attr.Name = "帳票のステータス"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 10;
                attr.Idx = -98;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_Starter") == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "Starter";
                attr.Name = "作成者"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_StarterName") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "StarterName";
                attr.Name = "作成者名前"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }


            if (attrs.Contains(this.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 参数 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.AtPara;
                attr.Name = "パラメータ"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 4000;
                attr.Idx = -99;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_RDT") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "RDT"; // "FlowStartRDT";
                attr.Name = "作成時間";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -97;
                attr.Insert();
            }
            #endregion 补充上流程字段。

            #region 注册到外键表.
            SFTable sf = new SFTable();
            sf.No = this.No;
            if (sf.RetrieveFromDBSources() == 0)
            {
                sf.Name = this.Name;
                sf.SrcType = SrcType.SQL;
                sf.SrcTable = this.PTable;
                sf.ColumnValue = "BillNo";
                sf.ColumnText = "Title";
                sf.SelectStatement = "SELECT BillNo AS No, Title as Name FROM " + this.PTable;
                sf.Insert();
            }

            #endregion 注册到外键表
        }

        #region 报表定义
        /// <summary>
        /// 选择显示的列
        /// </summary>
        /// <returns></returns>
        public string DoRpt_ColsChose()
        {
            return "../../CCBill/Admin/ColsChose.htm?FrmID=" + this.No;
        }
        /// <summary>
        /// 列的顺序
        /// </summary>
        /// <returns></returns>
        public string DoRpt_ColsIdxAndLabel()
        {
            return "../../CCBill/Admin/ColsIdxAndLabel.htm?FrmID=" + this.No;
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoRpt_SearchCond()
        {
            return "../../CCBill/Admin/SearchCond.htm?FrmID=" + this.No;
        }
        #endregion 报表定义.

        #region 权限控制.
        /// <summary>
        /// 保存权限规则
        /// </summary>
        /// <returns></returns>
        public string DoSaveRole()
        {
            return "../../CCBill/Admin/CreateRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSave";
        }
        /// <summary>
        /// 提交权限规则
        /// </summary>
        /// <returns></returns>
        public string DoSubmitRole()
        {
            return "../../CCBill/Admin/CreateRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSubmit";
        }

        /// <summary>
        /// 新增权限规则
        /// </summary>
        /// <returns></returns>
        public string DoCreateRole()
        {
            return "../../CCBill/Admin/CreateRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnNew";
        }
        /// <summary>
        /// 删除权限规则
        /// </summary>
        /// <returns></returns>
        public string DoDeleteRole()
        {
            return "../../CCBill/Admin/CreateRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnDelete";
        }

        /// <summary>
        /// 查询权限
        /// </summary>
        /// <returns></returns>
        public string DoSearchRole()
        {
            return "../../CCBill/Admin/CreateRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSearch";
        }
       
        #endregion 权限控制.

        public string DoMethod()
        {
            return "../../CCBill/Admin/Method.htm?s=34&FrmID=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        public string DoPageLoadFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PageLoadFull.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        /// <summary>
        /// 表单事件
        /// </summary>
        /// <returns></returns>
        public string DoEvent()
        {
            return "../../Admin/CCFormDesigner/Action.htm?FK_MapData=" + this.No + "&T=sd&FK_Node=0";
        }
        /// <summary>
        /// 绑定菜单树
        /// </summary>
        /// <returns>返回执行结果.</returns>
        public string DoBindMenu(string menumDirNo, string menuName)
        {
            string sql = "SELECT FK_App FROM GPM_Menu WHERE No='" + menumDirNo + "'";
            string app = DBAccess.RunSQLReturnString(sql);

            string guid = DBAccess.GenerGUID();

            string url = "../WF/CCBill/Search.htm?FrmID=" + this.No;
            sql = "INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('" + guid + "', '" + menuName + "', '" + menumDirNo + "', 1, 4, '" + app + "', '" + url + "',  0,'',1)";
            DBAccess.RunSQL(sql);
            return "正常に参加しました。<a href='En.htm?EnName=BP.GPM.Menu&No=" + guid + "'>制御権限については、GPMに転送してください。</a>";
        }

        #region 业务逻辑.
        public string CreateBlankWorkID()
        {
            return BP.Frm.Dev2Interface.CreateBlankDictID(this.No, BP.Web.WebUser.No, null).ToString();
        }
        #endregion 业务逻辑.

        #region 方法操作.
        /// <summary>
        /// 打开单据
        /// </summary>
        /// <returns></returns>
        public string DoOpenBillDict()
        {
            return "../../CCBill/SearchDict.htm?FrmID=" +
              this.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }
        public string DoOpenBillEditer()
        {
            return "../../CCBill/SearchEditer.htm?FrmID=" +
              this.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }
        public string DoAPI()
        {
            return "../../Admin/FoolFormDesigner/Bill/API.htm?FrmID=" +
              this.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }
        #endregion 方法操作.

    }
    /// <summary>
    /// 实体表单s
    /// </summary>
    public class FrmDicts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 实体表单s
        /// </summary>
        public FrmDicts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmDict();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmDict> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmDict>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmDict> Tolist()
        {
            System.Collections.Generic.List<FrmDict> list = new System.Collections.Generic.List<FrmDict>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmDict)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
