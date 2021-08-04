    using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;

namespace BP.Sys
{
    public class ToolbarExcelSlnAttr : ToolbarExcelAttr
    {
        public const string  FK_Flow="FK_Flow";
        public const string  FK_Node="FK_Node";
        public const string FK_Frm = "FK_Frm";

    }
    /// <summary>
    ///  ToolbarExcel 控制器
    /// </summary>
    public class ToolbarExcelSln : EntityMyPK
    {
        #region 界面上的访问控制
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsView = false;
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsUpdate = true;
                    uac.IsView = true;
                }
                return uac;
            }
        }
        #endregion

        #region 功能按钮.
        /// <summary>
        /// 打开本地标签.
        /// </summary>
        public string  OfficeOpenLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeOpenLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenLab, value);
            }
        }
        /// <summary>
        /// 是否打开本地模版文件.
        /// </summary>
        public bool OfficeOpenEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeOpenEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenEnable, value);
            }
        }
        /// <summary>
        /// 打开模板 标签.
        /// </summary>
        public string OfficeOpenTemplateLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeOpenTemplateLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenTemplateLab, value);
            }
        }
        /// <summary>
        /// 打开模板.
        /// </summary>
        public bool OfficeOpenTemplateEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeOpenTemplateEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenTemplateEnable, value);
            }
        }
        /// <summary>
        /// 保存 标签.
        /// </summary>
        public string OfficeSaveLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeSaveLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSaveLab, value);
            }
        }
        /// <summary>
        /// 保存.是否启用.
        /// </summary>
        public bool OfficeSaveEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeSaveEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSaveEnable, value);
            }
        }
        /// <summary>
        /// 接受修订 标签.
        /// </summary>
        public string OfficeAcceptLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeAcceptLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeAcceptLab, value);
            }
        }
        /// <summary>
        /// 接受修订.
        /// </summary>
        public bool OfficeAcceptEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeAcceptEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeAcceptEnable, value);
            }
        }
        /// <summary>
        /// 拒绝修订 标签.
        /// </summary>
        public string OfficeRefuseLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeRefuseLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeRefuseLab, value);
            }
        }
        /// <summary>
        /// 拒绝修订.
        /// </summary>
        public bool OfficeRefuseEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeRefuseEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeRefuseEnable, value);
            }
        }
        /// <summary>
        /// 套红按钮 标签.
        /// </summary>
        public string OfficeOverLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeOverLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOverLab, value);
            }
        }
        /// <summary>
        /// 套红按钮.
        /// </summary>
        public bool OfficeOverEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeOverEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOverEnable, value);
            }
        }
        /// <summary>
        /// 查看用户留痕
        /// </summary>
        public bool OfficeMarksEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeMarksEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeMarksEnable, value);
            }
        }
        /// <summary>
        /// 打印按钮-标签
        /// </summary>
        public string OfficePrintLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficePrintLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficePrintLab, value);
            }
        }
        /// <summary>
        /// 打印
        /// </summary>
        public bool OfficePrintEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficePrintEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficePrintEnable, value);
            }
        }
        /// <summary>
        /// 签章-标签
        /// </summary>
        public string OfficeSealLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeSealLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSealLab, value);
            }
        }
        /// <summary>
        /// 签章
        /// </summary>
        public bool OfficeSealEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeSealEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSealEnable, value);
            }
        }



        /// <summary>
        /// 插入流程-标签
        /// </summary>
        public string OfficeInsertFlowLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeInsertFlowLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeInsertFlowLab, value);
            }
        }
        /// <summary>
        /// 插入流程
        /// </summary>
        public bool OfficeInsertFlowEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeInsertFlowEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeInsertFlowEnable, value);
            }
        }

        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeNodeInfo
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeNodeInfo);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeNodeInfo, value);
            }
        }
        /// <summary>
        /// 是否该节点保存为PDF
        /// </summary>
        public bool OfficeReSavePDF
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeReSavePDF);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeReSavePDF, value);
            }
        }
        /// <summary>
        /// 是否进入留痕模式
        /// </summary>
        public bool OfficeIsMarks
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeIsMarks);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeIsMarks, value);
            }
        }
        /// <summary>
        /// 指定文档模板
        /// </summary>
        public string OfficeTemplate
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeTemplate);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeTemplate, value);
            }
        }
        /// <summary>
        /// 是否使用父流程的文档
        /// </summary>
        public bool OfficeIsParent
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeIsParent);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeIsParent, value);
            }
        }
        /// <summary>
        /// 是否启用标签
        /// </summary>
        public string OfficeDownLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeDownLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeDownLab, value);
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        public bool OfficeIsDown
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeIsDown);
            }
        }
        /// <summary>
        /// 是否自动套红
        /// </summary>
        public bool OfficeTHEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeTHEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeTHEnable, value);
            }
        }
        /// <summary>
        /// 套红模板
        /// </summary>
        public string OfficeTHTemplate
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeTHTemplate);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeTHTemplate, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// ToolbarExcel功能控制区域
        /// </summary>
        public ToolbarExcelSln() { }
        /// <summary>
        /// ToolbarExcel功能控制
        /// </summary>
        /// <param name="no">表单ID</param>
        public ToolbarExcelSln(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        public ToolbarExcelSln(string fk_flow, int fk_node, string fk_frm)
        {
            int i = this.Retrieve(ToolbarExcelSlnAttr.FK_Flow, fk_flow, 
                ToolbarExcelSlnAttr.FK_Node, fk_node, ToolbarExcelSlnAttr.FK_Frm, fk_frm);
            if (i == 0)
            {
                return;
                throw new Exception("@フォーム関連情報を削除しました。");
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

                Map map = new Map("WF_FrmNode","ToolbarExcelSln機能制御");

                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddMyPK();
                map.AddTBString(ToolbarExcelSlnAttr.FK_Frm, null, "フォームID", true, true, 1, 32, 32);
                map.AddTBInt(ToolbarExcelSlnAttr.FK_Node, 0, "ノード番号", true, true);
                map.AddTBString(ToolbarExcelSlnAttr.FK_Flow, null, "フロー番号", true, true, 1, 20, 20);


                #region 公文按钮
                map.AddTBString(ToolbarExcelAttr.OfficeOpenLab, "ローカルを開く", "ローカルラベルを開く", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeOpenEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeOpenTemplateLab, "テンプレートを開く", "テンプレートラベルを開く", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeOpenTemplateEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeSaveLab, "保存", "保存ラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeSaveEnable, true, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeAcceptLab, "改訂を承認", "改訂を承認ラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeAcceptEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeRefuseLab, "改訂を拒否", "改訂ラベルを拒否", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeRefuseEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeOverLab, "赤いボタン", "赤いボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeOverEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeMarksEnable, true, "ユーザートレースをチェックするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficePrintLab, "印刷ボタン", "印刷ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficePrintEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeSealLab, "サインボタン", "サインボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeSealEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeInsertFlowLab, "挿入フロー", "挿入フローラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeInsertFlowEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeNodeInfo, false, "ノード情報を記録するかどうか", true, true);
                map.AddBoolean(ToolbarExcelAttr.OfficeReSavePDF, false, "PDFとして自動的に保存する必要があります", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeDownLab, "ダウンロード", "ダウンロードボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeDownEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeIsMarks, true, "マークモードに入るかどうか", true, true);
                map.AddTBString(ToolbarExcelAttr.OfficeTemplate, "", "ドキュメントテンプレートを指定", true, false, 0, 100, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeIsParent, true, "親フローのドキュメントを使用するかどうか", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeTHEnable, false, "自動的に裁定取引を行うかどうか", true, true);
                map.AddTBString(ToolbarExcelAttr.OfficeTHTemplate, "", "自動赤テンプレート", true, false, 0, 200, 10);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// ToolbarExcel表单.
    /// </summary>
    public class ToolbarExcelSlns : EntitiesMyPK
    {
        /// <summary>
        /// 功能控制
        /// </summary>
        public ToolbarExcelSlns()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ToolbarExcelSln();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。

        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ToolbarExcelSln> ToJavaList()
        {
            return (System.Collections.Generic.IList<ToolbarExcelSln>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ToolbarExcelSln> Tolist()
        {
            System.Collections.Generic.List<ToolbarExcelSln> list = new System.Collections.Generic.List<ToolbarExcelSln>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ToolbarExcelSln)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
