using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Sys;
using System.Collections.Generic;

namespace BP.WF.Template
{
    /// <summary>
    /// 自由表单属性 attr
    /// </summary>
    public class MapFrmFreeAttr : MapDataAttr
    {
        /// <summary>
        /// 保存标签
        /// </summary>
        public const string BtnSaveLab = "BtnSaveLab";
        /// <summary>
        /// 保存是否启用
        /// </summary>
        public const string BtnSaveEnable = "BtnSaveEnable";

        /// <summary>
        /// 删除标签
        /// </summary>
        public const string BtnDelLab = "BtnDelLab";
        /// <summary>
        /// 删除是否启用
        /// </summary>
        public const string BtnDelEnable = "BtnDelEnable";

        /// <summary>
        /// 打印标签
        /// </summary>
        public const string BtnPrintLab = "BtnPrintLab";
        /// <summary>
        /// 打印是否启用
        /// </summary>
        public const string BtnPrintEnable = "BtnPrintEnable";
    }
    /// <summary>
    /// 自由表单属性
    /// </summary>
    public class MapFrmFree : EntityNoName
    {
        #region 文件模版属性.
       
        #endregion 文件模版属性.

        #region 属性
        /// <summary>
        /// 物理存储表
        /// </summary>
        public string PTable
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.PTable);
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// 表单事件实体
        /// </summary>
        public string FromEventEntity
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FormEventEntity);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FormEventEntity,value);
            }
        }
        /// <summary>
        /// 是否是节点表单?
        /// </summary>
        public bool IsNodeFrm
        {
            get
            {
                if (this.No.Contains("ND") == false)
                    return false;

                if (this.No.Contains("Rpt") == true)
                    return false;

                if (this.No.Substring(0, 2) == "ND")
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 节点ID.
        /// </summary>
        public int NodeID
        {
            get
            {
                if (this.No.IndexOf("ND") != 0)
                    return 0;
                return int.Parse(this.No.Replace("ND", ""));
            }
        }
      
        /// <summary>
        /// 表格显示的列
        /// </summary>
        public int TableCol
        {
            get
            {
                return 4;
                int i = this.GetValIntByKey(MapFrmFreeAttr.TableCol);
                if (i == 0 || i == 1)
                    return 4;
                return i;
            }
            set
            {
                this.SetValByKey(MapFrmFreeAttr.TableCol, value);
            }
        }
        #endregion

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

        #region 构造方法
        /// <summary>
        /// 自由表单属性
        /// </summary>
        public MapFrmFree()
        {
        }
        /// <summary>
        /// 自由表单属性
        /// </summary>
        /// <param name="no">表单ID</param>
        public MapFrmFree(string no)  : base(no)
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

                Map map = new Map("Sys_MapData", "自由形式の属性");
                map.Java_SetEnType(EnType.Sys);

                #region 基本属性.
                map.AddTBStringPK(MapFrmFreeAttr.No, null, "フォーム番号", true, true, 1, 190, 20);
                map.AddTBString(MapFrmFreeAttr.PTable, null, "収納テーブル", true, false, 0, 100, 20);
                map.AddTBString(MapFrmFreeAttr.Name, null, "フォーム名", true, false, 0, 200, 20,true);
                map.AddTBString(MapDataAttr.FormEventEntity, null, "イベントエンティティ", true, true, 0, 100, 20, true);
                 
                //数据源.
                map.AddDDLEntities(MapFrmFreeAttr.DBSrc, "local", "情報源", new BP.Sys.SFDBSrcs(), true);
                map.AddDDLEntities(MapFrmFreeAttr.FK_FormTree, "01", "フォームのカテゴリ", new SysFormTrees(), true);

                //宽度高度.
                map.AddTBInt(MapFrmFreeAttr.FrmW, 900, "フリーフォーム-幅", true, false);
                map.AddTBInt(MapFrmFreeAttr.FrmH, 1200, "フリーフォーム-高", true, false);

                //表单的运行类型.
                map.AddDDLSysEnum(MapFrmFreeAttr.FrmType, (int)BP.Sys.FrmType.FreeFrm, "フォームタイプ", true, false, MapFrmFreeAttr.FrmType);
                #endregion 基本属性.
 

                #region 设计者信息.
                map.AddTBString(MapFrmFreeAttr.Designer, null, "デザイナー", true, false, 0, 500, 20);
                map.AddTBString(MapFrmFreeAttr.DesignerContact, null, "連絡先", true, false, 0, 500, 20);
                map.AddTBString(MapFrmFreeAttr.DesignerUnit, null, "単位", true, false, 0, 500, 20, false);
                map.AddTBString(MapFrmFreeAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapFrmFreeAttr.Ver, null, "バージョンナンバー", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapFrmFreeAttr.Note, null, "備考", true, false, true);

                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapFrmFreeAttr.Idx, 100, "シーケンス番号", false, false);
                #endregion 设计者信息.

                map.AddMyFile("フォームテンプレート");

                //查询条件.
                map.AddSearchAttr(MapFrmFreeAttr.DBSrc);

                #region 方法 - 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "フールフォームデザイナーを起動する";
                rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
                rm.Icon = "../../WF/Img/FileType/xlsx.gif";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フィールドメンテナンス";
                rm.ClassMethodName = this.ToString() + ".DoEditFiledsList";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フィールドを一括編集する"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBatchEditAttr";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
               // map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ロードと充填"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoPageLoadFull";
                rm.Icon = "../../WF/Img/FullData.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
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
                rm.Title = "検証ルールをバッチで設定する";
                rm.Icon = "../../WF/Img/RegularExpression.png";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpressionBatch";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "プログラミング"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoInitScript";
                rm.Icon = "../../WF/Img/Script.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フォームボディ属性"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBodyAttr";
                rm.Icon = "../../WF/Img/Script.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "XMLフォームテンプレートのエクスポート"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoExp";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "../../WF/Img/Export.png";
                rm.Visable = true;
                rm.RefAttrLinkLabel = "xmlにエクスポート";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                           
                rm = new RefMethod();
                rm.Title = "シーケンスキー"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoTabIdx";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "テンプレート印刷";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;

                map.AddRefMethod(rm);
             


                //rm = new RefMethod();
                //rm.Title = "节点表单组件"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoNodeFrmCompent";
                //rm.Visable = true;
                //rm.RefAttrLinkLabel = "节点表单组件";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //rm.Icon = ../../Img/Components.png";
                //map.AddRefMethod(rm);
                #endregion 方法 - 基本功能.

                #region 高级设置.
                //带有参数的方法.
                rm = new RefMethod();
                rm.Title = "フィールドの名前を変更";
             //   rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("FieldOld", null, "古いフィールドの英語名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新しいフィールドの英語名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新しいフィールドの中国名", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                rm.Icon = "../../WF/Img/ReName.png";
                rm.GroupName = "高度な設定";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フォームIDの名前を変更する";
              //  rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("NewFrmID1", null, "新しいフォームID名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("NewFrmID2", null, "フォームID名を確認", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmID";
                rm.Icon = "../../WF/Img/ReName.png";
                rm.GroupName = "高度な設定";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フォームをコピー";
                //  rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("FrmID", null, "新しいフォームIDをコピーするには", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FrmName", null, "フォーム名", true, false, 0, 100, 100);
                rm.HisAttrs.AddDDLEntities("FrmTree", null, "フォームディレクトリにコピー", new FrmTrees(),true);

                rm.ClassMethodName = this.ToString() + ".DoCopyFrm";
                rm.Icon = "../../WF/Img/Btn/Copy.GIF";
                rm.GroupName = "高度な設定";
                map.AddRefMethod(rm);
                
                rm = new RefMethod();
                rm.Title = "モバイルフォーム";
                rm.GroupName = "高度な設定";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //@李国文.
                rm = new RefMethod();
                rm.Title = "フォームの種類を変更する";
                rm.GroupName = "高度な機能";
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmType()";
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "フォームの種類を変更する", true, true);
                map.AddRefMethod(rm);

                #endregion 高级设置.

                #region 方法 - 开发接口.
                rm = new RefMethod();
                rm.Title = "クエリAPIを呼び出す"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearch";
                rm.Icon = "../../WF/Img/Table.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "分析APIを呼び出す"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoGroup";
                rm.Icon ="../../Img/Table.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);
                #endregion 方法 - 开发接口.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdate()
        {
            //注册事件表单实体.
            BP.Sys.FormEventBase feb = BP.Sys.Glo.GetFormEventBaseByEnName(this.No);
            if (feb == null)
                this.FromEventEntity = "";
            else

                this.FromEventEntity = feb.ToString();

            return base.beforeUpdate();
        }

        protected override void afterUpdate()
        {
            //修改关联明细表
            MapDtl dtl = new MapDtl();
            dtl.No = this.No;
            if (dtl.RetrieveFromDBSources() == 1)
            {
                dtl.Name = this.Name;
                dtl.PTable = this.PTable;
                dtl.DirectUpdate();

                MapData map = new MapData(this.No);
                //避免显示在表单库中
                map.FK_FrmSort = "";
                map.FK_FormTree = "";
                map.DirectUpdate();
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.No);

            base.afterUpdate();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.No);
            base.afterDelete();
        }

        #region 高级设置.
        /// <summary>
        /// 改变表单类型 @李国文 ，需要搬到jflow.
        /// </summary>
        /// <param name="val">要改变的类型</param>
        /// <returns></returns>
        public string DoChangeFrmType(int val)
        {
            MapData md = new MapData(this.No);
            string str = "オリジナルは:" + md.HisFrmTypeText + "種類、";
            md.HisFrmTypeInt = val;
            str += "今に修正" + md.HisFrmTypeText + "の種類";
            md.Update();

            return str;
        }
        public string DoTabIdx()
        {
            return SystemConfig.CCFlowWebPath +"WF/Admin/FoolFormDesigner/TabIdx.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 复制表单
        /// </summary>
        /// <returns></returns>
        public string DoCopyFrm(string frmID, string frmName, string fk_frmTree)
        {
            return BP.Sys.CCFormAPI.CopyFrm(this.No, frmID, frmName, fk_frmTree);
        }
        #endregion 高级设置.


        #region 节点表单方法.
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/AttrNode/Bill.htm?FK_MapData=" + this.No + "&NodeID=" + this.NodeID + "&FK_Node=" + this.NodeID;
        }

        /// <summary>
        /// 启动自由表单设计器(SL)
        /// </summary>
        /// <returns></returns>
        public string DoDesignerSL()
        {
            return "../../Admin/CCFormDesigner/CCFormDesignerSL.htm?FK_MapData="+this.No+"&UserNo="+Web.WebUser.No+"&SID="+BP.Web.WebUser.SID;
        }
        /// <summary>
        /// 启动自由表单设计器(h5)
        /// </summary>
        /// <returns></returns>
        public string DoDesignerH5()
        {
            // WF/Admin/CCFormDesigner/FormDesigner.htm?FK_MapData=ND102&UserNo=admin&SID=44a42h5gcbxnwjof2hv2pw5e
            return "../../Admin/CCFormDesigner/FormDesigner.htm?FK_MapData=" + this.No + "&UserNo=" + Web.WebUser.No + "&SID=" + BP.Web.WebUser.SID;
        }
        /// <summary>
        /// 傻瓜表单设计器
        /// </summary>
        /// <returns></returns>
        public string DoDesignerFool()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&MyPK=" + this.No + "&IsEditMapData=True&DoDesignerFool&IsFirst=1";
        }
        /// <summary>
        /// 编辑excel模版.
        /// </summary>
        /// <returns></returns>
        public string DoEditExcelTemplate()
        {
            return "../../Admin/CCFormDesigner/ExcelFrmDesigner/Designer.htm?FK_MapData=" + this.No;
        }
        /// <summary>
        /// 表单字段.
        /// </summary>
        /// <returns></returns>
        public string DoEditFiledsList()
        {
            return "../../Admin/FoolFormDesigner/BatchEdit.htm?FK_MapData=" + this.No;
        }
        
        #endregion

        #region 通用方法.
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="frmID1"></param>
        /// <param name="frmID2"></param>
        /// <returns></returns>
        public string DoChangeFrmID(string frmID1, string frmID2)
        {
            MapData md = new MapData();
            md.No = frmID1;
            if (md.IsExits == true)
                return "フォームID【" + frmID1 + "】もう存在している";

            if (frmID1 != frmID2)
                return "入力された2つのIDが一致していません";


            string frmIDOld = this.No;

            string sqls = "";
            sqls += "@UPDATE Sys_MapData SET No='" + frmID1 + "' WHERE No='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmLine SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmLab SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmBtn SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapAttr SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapExt SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmImg SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmImgAth SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmRB SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapDtl SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_MapFrame SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmEle SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            sqls += "UPDATE Sys_FrmEvent SET FK_MapData='" + frmID1 + "' WHERE FK_MapData='" + frmIDOld + "'";
            BP.DA.DBAccess.RunSQLs(sqls);

            return "名前の変更に成功しました。ウィンドウを閉じて再度更新する必要があります。";
        }
        /// <summary>
        /// 替换名称
        /// </summary>
        /// <param name="fieldOldName">旧名称</param>
        /// <param name="newField">新字段</param>
        /// <param name="newFieldName">新字段名称(可以为空)</param>
        /// <returns></returns>
        public string DoChangeFieldName(string fieldOld, string newField, string newFieldName)
        {
            MapAttr attrOld = new MapAttr();
            attrOld.KeyOfEn = fieldOld;
            attrOld.FK_MapData = this.No;
            attrOld.MyPK = attrOld.FK_MapData + "_" + attrOld.KeyOfEn;
            if (attrOld.RetrieveFromDBSources() == 0)
                return "@古いフィールド入力エラー[" + attrOld.KeyOfEn + "].";

            //检查是否存在该字段？
            MapAttr attrNew = new MapAttr();
            attrNew.KeyOfEn = newField;
            attrNew.FK_MapData = this.No;
            attrNew.MyPK = attrNew.FK_MapData + "_" + attrNew.KeyOfEn;
            if (attrNew.RetrieveFromDBSources() == 1)
                return "@このフィールド[" + attrNew.KeyOfEn + "]もう存在している。";

            //删除旧数据.
            attrOld.Delete();

            //copy这个数据,增加上它.
            attrNew.Copy(attrOld);
            attrNew.KeyOfEn = newField;
            attrNew.FK_MapData = this.No;

            if (newFieldName != "")
                attrNew.Name = newFieldName;

            attrNew.Insert();

            //更新处理他的相关业务逻辑.
            MapExts exts = new MapExts(this.No);
            foreach (MapExt item in exts)
            {
                item.MyPK = item.MyPK.Replace("_" + fieldOld, "_" + newField);

                if (item.AttrOfOper == fieldOld)
                    item.AttrOfOper = newField;

                if (item.AttrsOfActive == fieldOld)
                    item.AttrsOfActive = newField;

                item.Tag = item.Tag.Replace(fieldOld, newField);
                item.Tag1 = item.Tag1.Replace(fieldOld, newField);
                item.Tag2 = item.Tag2.Replace(fieldOld, newField);
                item.Tag3 = item.Tag3.Replace(fieldOld, newField);

                item.AtPara = item.AtPara.Replace(fieldOld, newField);
                item.Doc = item.Doc.Replace(fieldOld, newField);
                item.Save();
            }
            return "実行成功";
        }
        /// <summary>
        /// 批量设置正则表达式规则.
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpressionBatch()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpressionBatch.htm?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 批量修改字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchEditAttr()
        {
            return "../../Admin/FoolFormDesigner/BatchEdit.htm?FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return "../../Admin/AttrNode/SortingMapAttrs.htm?FK_Flow=&FK_MapData=" +
                   this.No + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 设计表单
        /// </summary>
        /// <returns></returns>
        public string DoDFrom()
        {
            return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;

        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoDFromCol4()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&SID=" + Web.WebUser.SID + "&IsFirst=1&AppCenterDBType=" + BP.DA.DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Sys.SystemConfig.CustomerNo;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string DoSearch()
        {
            return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 调用分析API
        /// </summary>
        /// <returns></returns>
        public string DoGroup()
        {
            return "../../Comm/Group.htm?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
        }
        /// <summary>
        /// 数据源管理
        /// </summary>
        /// <returns></returns>
        public string DoDBSrc()
        {
            return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=BP.Sys.SFDBSrcs";
        }
   
        public string DoPageLoadFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PageLoadFull.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        public string DoInitScript()
        {
            return "../../Admin/FoolFormDesigner/MapExt/InitScript.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        /// <summary>
        /// 自由表单属性.
        /// </summary>
        /// <returns></returns>
        public string DoBodyAttr()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BodyAttr.htm?s=34&FK_MapData=" + this.No + "&ExtType=BodyAttr&RefNo=";
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
        /// 导出表单
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            return "../../Admin/FoolFormDesigner/ImpExp/Exp.htm?FK_MapData=" + this.No;
        }
        #endregion 方法.
    }
    /// <summary>
    /// 自由表单属性s
    /// </summary>
    public class MapFrmFrees : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 自由表单属性s
        /// </summary>
        public MapFrmFrees()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrmFree();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrmFree> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrmFree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrmFree> Tolist()
        {
            System.Collections.Generic.List<MapFrmFree> list = new System.Collections.Generic.List<MapFrmFree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrmFree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
