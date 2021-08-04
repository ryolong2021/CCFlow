using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowExt : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FK_FlowSort);
            }
            set
            {
                this.SetValByKey(FlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 系统类别（第2级流程树节点编号）
        /// </summary>
        public string SysType
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.SysType);
            }
            set
            {
                this.SetValByKey(FlowAttr.SysType, value);
            }
        }

        /// <summary>
        /// 是否可以独立启动
        /// </summary>
        public bool IsCanStart
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsCanStart);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsCanStart, value);
            }
        }
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        /// <summary>
        /// 流程标记
        /// </summary>
        public string FlowMark
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.FlowMark);
                if (DataType.IsNullOrEmpty(str) == true)
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }

        #region   前置导航
        /// <summary>
        /// 前置导航方式
        /// </summary>
        public StartGuideWay StartGuideWay
        {
            get
            {
                return (StartGuideWay)this.GetValIntByKey(FlowAttr.StartGuideWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideWay, (int)value);
            }

        }
        /// <summary>
        /// 前置导航参数1
        /// </summary>
        public string StartGuidePara1
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara1);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara1, value);
            }

        }
        /// <summary>
        /// 前置导航参数2
        /// </summary>
        public string StartGuidePara2
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara2);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara2, value);
            }

        }
        /// <summary>
        /// 前置导航参数3
        /// </summary>
        public string StartGuidePara3
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara3);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara3, value);
            }

        }

        /// <summary>
        /// 启动方式
        /// </summary>
        public FlowRunWay FlowRunWay
        {
            get
            {
                return (FlowRunWay)this.GetValIntByKey(FlowAttr.FlowRunWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.FlowRunWay, (int)value);
            }

        }

        /// <summary>
        /// 运行内容
        /// </summary>
        public string RunObj
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.RunObj);
            }
            set
            {
                this.SetValByKey(FlowAttr.RunObj, value);
            }

        }

        /// <summary>
        /// 是否启用开始节点数据重置按钮
        /// </summary>
        public bool IsResetData
        {

            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsResetData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsResetData, value);
            }
        }

        /// <summary>
        /// 是否自动装载上一笔数据
        /// </summary>
        public bool IsLoadPriData
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsLoadPriData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsLoadPriData, value);
            }
        }
        /// <summary>
        /// 是否可以在手机里启用
        /// </summary>
        public bool IsStartInMobile
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsStartInMobile);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsStartInMobile, value);
            }
        }
        #endregion
        /// <summary>
        /// 设计者编号
        /// </summary>
        public string DesignerNo
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerNo, value);
            }
        }
        /// <summary>
        /// 设计者名称
        /// </summary>
        public string DesignerName
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerName);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerName, value);
            }
        }
        /// <summary>
        /// 设计时间
        /// </summary>
        public string DesignTime
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignTime);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignTime, value);
            }
        }
        /// <summary>
        /// 编号生成格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BillNoFormat);
            }
            set
            {
                this.SetValByKey(FlowAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 测试人员
        /// </summary>
        public string Tester
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.Tester);
            }
            set
            {
                this.SetValByKey(FlowAttr.Tester, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No == "admin" || this.DesignerNo == WebUser.No)
                    uac.IsUpdate = true;


                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowExt()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public FlowExt(string _No)
        {
            this.No = _No;
            if (SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("フローIDは存在しません");
            }
            else
            {
                this.Retrieve();
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

                Map map = new Map("WF_Flow", "処理する");

                #region 基本属性。
                map.AddTBStringPK(FlowAttr.No, null, "ナンバリング", true, true, 1, 10, 3);
                map.SetHelperUrl(FlowAttr.No, "http://ccbpm.mydoc.io/?v=5404&t=17023"); //使用alert的方式显示帮助信息.

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "フローカテゴリ", new FlowSorts(), true);
                map.SetHelperUrl(FlowAttr.FK_FlowSort, "http://ccbpm.mydoc.io/?v=5404&t=17024");
                map.AddTBString(FlowAttr.Name, null, "名前", true, false, 0, 50, 10, true);

                // add 2013-02-14 唯一确定此流程的标记
                map.AddTBString(FlowAttr.FlowMark, null, "フローマーク", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "フローイベントエンティティ", true, true, 0, 150, 10);
                map.SetHelperUrl(FlowAttr.FlowMark, "http://ccbpm.mydoc.io/?v=5404&t=16847");
                map.SetHelperUrl(FlowAttr.FlowEventEntity, "http://ccbpm.mydoc.io/?v=5404&t=17026");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "タイトル生成ルール", true, false, 0, 150, 10, true);
                map.SetHelperUrl(FlowAttr.TitleRole, "http://ccbpm.mydoc.io/?v=5404&t=17040");


                map.AddBoolean(FlowAttr.IsCanStart, true, "独立して開始できますか？ （独立して開始されたフローは、開始されたフローのリストに表示できます）", true, true, true);
                map.SetHelperUrl(FlowAttr.IsCanStart, "http://ccbpm.mydoc.io/?v=5404&t=17027");

                map.AddBoolean(FlowAttr.IsFullSA, false, "将来のプロセッサは自動的に計算されますか？", true, true, true);
                map.SetHelperUrl(FlowAttr.IsFullSA, "http://ccbpm.mydoc.io/?v=5404&t=17034");

                //map.AddDDLSysEnum(FlowAttr.IsAutoSendSubFlowOver, 0, "为子流程时结束规则", true, true,
                // FlowAttr.IsAutoSendSubFlowOver, "@0=不处理@1=让父流程自动运行下一步@2=结束父流程");

                map.AddBoolean(FlowAttr.IsGuestFlow, false, "外部ユーザーがフローに参加するかどうか（非組織構造の担当者が関係するフロー）", true, true, false);
                map.SetHelperUrl(FlowAttr.IsGuestFlow, "http://ccbpm.mydoc.io/?v=5404&t=17039");

                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "フローアプリケーションタイプ",
                  true, true, "FlowAppType", "@0=ビジネスフロー@1=エンジニアリング（プロジェクトチームフロー）@2=公式ドキュメントフロー（VSTO）");
                map.SetHelperUrl(FlowAttr.FlowAppType, "http://ccbpm.mydoc.io/?v=5404&t=17035");


                //map.AddDDLSysEnum(FlowAttr.SDTOfFlow, (int)TimelineRole.ByNodeSet, "时效性规则",
                // true, true, FlowAttr.SDTOfFlow, "@0=按节点(由节点属性来定义)@1=按发起人(开始节点SysSDTOfFlow字段计算)");
                //map.SetHelperUrl(FlowAttr.TimelineRole, "http://ccbpm.mydoc.io/?v=5404&t=17036");

                // 草稿
                map.AddDDLSysEnum(FlowAttr.Draft, (int)DraftRole.None, "ドラフトルール",
               true, true, FlowAttr.Draft, "@0=なし（下書きなし）@1=保存して実行@2=下書きボックスに保存");
                map.SetHelperUrl(FlowAttr.Draft, "http://ccbpm.mydoc.io/?v=5404&t=17037");

                // add for 华夏银行.
                map.AddDDLSysEnum(FlowAttr.FlowDeleteRole, (int)FlowDeleteRole.AdminOnly, "フローインスタンスの削除ルール",
            true, true, FlowAttr.FlowDeleteRole,
            "@0=スーパー管理者は削除できます@1=階層管理者は削除できます@2=開始者は削除できます@3=ノードは削除ボタンオペレーターを開始します");

                //子流程结束时，让父流程自动运行到下一步
                map.AddBoolean(FlowAttr.IsToParentNextNode, false, "サブフローが終了したら、親フローを次のステップに自動的に実行させます", true, true);

                map.AddDDLSysEnum(FlowAttr.FlowAppType, (int)FlowAppType.Normal, "フローアプリケーションタイプ", true, true, "FlowAppType", "@0=ビジネスフロー@1=エンジニアリング（プロジェクトチームフロー）@2=公式ドキュメントフロー（VSTO）");
                map.AddTBString(FlowAttr.HelpUrl, null, "ヘルプドキュメント", true, false, 0, 300, 10, true);


                //为 莲荷科技增加一个系统类型, 用于存储当前所在流程树的第2级流程树编号.
                map.AddTBString(FlowAttr.SysType, null, "システムタイプ", false, false, 0, 100, 10, false);
                map.AddTBString(FlowAttr.Tester, null, "テスター", true, false, 0, 300, 10, true);

                String sql = "SELECT No,Name FROM Sys_EnumMain WHERE No LIKE 'Flow_%' ";
                map.AddDDLSQL("NodeAppType", null, "ビジネスタイプの列挙（Nullの場合もあります）", sql, true);

                // add 2014-10-19.
                map.AddDDLSysEnum(FlowAttr.ChartType, (int)FlowChartType.Icon, "ノードグラフタイプ", true, true,
                    "ChartType", "@0=ジオメトリ@1=肖像画");

                map.AddTBString(FlowAttr.HostRun, null, "実行中のホスト（IP+ポート）", true, false, 0, 40, 10, true);
                #endregion 基本属性。

                #region 表单数据.

                //批量发起 add 2013-12-27. 
                map.AddBoolean(FlowAttr.IsBatchStart, false, "フローをバッチで開始することは可能ですか？ （入力する必要があるフィールドを設定する必要がある場合、複数はカンマで区切られます）", true, true, true);
                map.AddTBString(FlowAttr.BatchStartFields, null, "発信フィールドs", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.IsBatchStart, "http://ccbpm.mydoc.io/?v=5404&t=17047");

                //add 2013-05-22.
                map.AddTBString(FlowAttr.HistoryFields, null, "履歴ビューフィールド", true, false, 0, 500, 10, true);

                //移动到这里 by zhoupeng 2016.04.08.
                map.AddBoolean(FlowAttr.IsResetData, false, "開始ノードデータリセットボタンは有効になっていますか？キャンセルされました）", false, true, true);
                map.AddBoolean(FlowAttr.IsLoadPriData, false, "最後のデータを自動的にロードするかどうか。", true, true, true);
                map.AddBoolean(FlowAttr.IsDBTemplate, true, "データテンプレートを有効にするかどうか", true, true, true);
                map.AddBoolean(FlowAttr.IsStartInMobile, true, "電話で有効化できますか？ （開始フォームが特に複雑な場合は、電話で有効にしないでください）", true, true, true);
                map.SetHelperAlert(FlowAttr.IsStartInMobile, "携帯電話フロー開始リストを制御するために使用されます。");

                map.AddBoolean(FlowAttr.IsMD5, false, "データ暗号化フローですか（MD5データ暗号化は改ざん防止）", true, true, true);
                map.SetHelperUrl(FlowAttr.IsMD5, "http://ccbpm.mydoc.io/?v=5404&t=17028");

                // 数据存储.
                map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow, "データストレージ", true, true, FlowAttr.DataStoreModel, "@0=データトラックモード@1=データマージモード");
                map.SetHelperUrl(FlowAttr.DataStoreModel, "http://ccbpm.mydoc.io/?v=5404&t=17038");

                map.AddTBString(FlowAttr.PTable, null, "フローデータ保存表", true, false, 0, 30, 10);
                map.SetHelperUrl(FlowAttr.PTable, "http://ccbpm.mydoc.io/?v=5404&t=17897");


                //map.SetHelperBaidu(FlowAttr.HistoryFields, "ccflow 历史查看字段");
                map.AddTBString(FlowAttr.FlowNoteExp, null, "注目の表現", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.FlowNoteExp, "http://ccbpm.mydoc.io/?v=5404&t=17043");

                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "文書番号の形式", true, false, 0, 50, 10, false);
                map.SetHelperUrl(FlowAttr.BillNoFormat, "http://ccbpm.mydoc.io/?v=5404&t=17041");

                // add 2019-09-25 by zhoupeng
                map.AddTBString(FlowAttr.BuessFields, null, "主な事業分野", true, false, 0, 100, 10, false);
                string msg = "To-Doのビジネスフィールド情報を表示するために使用されます";
                msg += "\t\n 1. ユーザーがタスクを見ると、フローインスタンスの重要な情報を見ることができます。";
                msg += "\t\n 2. やることリスト情報表示に使用";
                msg += "\t\n 3. 構成形式は次のとおりです。Tel、Addr、Emailこれらのフィールドは大文字と小文字が区別され、ノードフォームフィールドです";
                msg += "\t\n 4. データはWF_GenerWorkFlow.AtParaに保存されます";
                msg += "\t\n 5. 保存形式は次のとおりです：@BuessFields = phone ^ Tel ^ 18992323232; address ^ Addr ^ Jinan、Shandong";
                map.SetHelperAlert(FlowAttr.BuessFields, msg);

                //表单URL. //@liuqiang 把他翻译到java里面去.
                map.AddDDLSysEnum(FlowAttr.FlowFrmType, 0, "グローバルフォームタイプの処理", true, false, FlowAttr.FlowFrmType,
                    "@0=2019年のフルバージョン以前のバージョン@1=開発者フォーム@2=フールフォーム@3=カスタム（埋め込み）フォーム@4=SDKフォーム");
                map.AddTBString(FlowAttr.FrmUrl, null, "フォームのURL", true, false, 0, 150, 10, true);
                map.SetHelperAlert(FlowAttr.FrmUrl, "埋め込みフォーム、SDKフォームのURLフォーム、埋め込みフォームに有効であり、フロー全体を設定するために使用されます。");
                #endregion 表单数据.

                #region 开发者信息.

                map.AddTBString(FlowAttr.DesignerNo, null, "デザイナー番号", true, true, 0, 50, 10, false);
                map.AddTBString(FlowAttr.DesignerName, null, "デザイナー名", true, true, 0, 50, 10, false);
                map.AddTBString(FlowAttr.DesignTime, null, "作成時間", true, true, 0, 50, 20, false);
                map.AddTBStringDoc(FlowAttr.Note, null, "過程説明", true, false, true);
                #endregion 开发者信息.

                #region 基本功能.
                //map.AddRefMethod(rm);
                RefMethod rm = new RefMethod();

              

                rm = new RefMethod();
                rm.Title = "自動開始";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/AutoStart.png";
                rm.ClassMethodName = this.ToString() + ".DoSetStartFlowDataSources()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "制限ルールを開始する";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Limit.png";
                rm.ClassMethodName = this.ToString() + ".DoLimit()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フロントナビゲーションを開始する";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/StartGuide.png";
                rm.ClassMethodName = this.ToString() + ".DoStartGuide()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フロントナビゲーションを開始する（実験中）";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/StartGuide.png";
                rm.ClassMethodName = this.ToString() + ".DoStartGuideV2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フローイベント"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "メッセージの処理"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = "../../WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フロー計画時間計算ルール"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSDTOfFlow";
                //rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "アイコンを変更"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoNodesICON";
                //  rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "バージョン管理";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoVer()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "アクセス制御";
                // rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoPowerModel()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);



                //rm = new RefMethod();
                //rm.Title = "与业务表数据同步"; // "抄送规则";
                //rm.ClassMethodName = this.ToString() + ".DoBTable";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/DTS.png";
                //rm.RefAttrLinkLabel = "业务表字段同步配置";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "独立表单树";
                //rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoFlowFormTree()";
                //map.AddRefMethod(rm);
                #endregion 流程设置.

                #region 时限规则
                rm = new RefMethod();
                rm.GroupName = "制限時間";
                rm.Title = "制限時間";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoDeadLineRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "制限時間";
                rm.Title = "早期警告、期限切れのニュースイベント";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/OvertimeRole.png";
                rm.ClassMethodName = this.ToString() + ".DoOverDeadLineRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);

                #endregion 时限规则

                #region 模拟测试.
                rm = new RefMethod();
                rm.GroupName = "模擬試験";
                rm.Title = "試運転"; // "设计检查报告";
                //rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = "../../WF/Img/EntityFunc/Flow/Run.png";
                rm.ClassMethodName = this.ToString() + ".DoRunIt";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "模擬試験";
                rm.Title = "検査報告"; // "设计検査報告";
                rm.Icon = "../../WF/Img/EntityFunc/Flow/CheckRpt.png";
                rm.ClassMethodName = this.ToString() + ".DoCheck2018Url";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.GroupName = "模擬試験";
                rm.Title = "検査報告書（旧）"; // "设计检查报告";
                rm.Icon = "../../WF/Img/EntityFunc/Flow/CheckRpt.png";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                #endregion 模拟测试.

                #region 流程模版管理.
                rm = new RefMethod();
                rm.Title = "テンプレートのインポート";
                rm.Icon = "../../WF/Img/redo.png";
                rm.ClassMethodName = this.ToString() + ".DoImp()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フローテンプレート";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "テンプレートのエクスポート";
                rm.Icon = "../../WF/Img/undo.png";
                rm.ClassMethodName = this.ToString() + ".DoExps()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フローテンプレート";
                map.AddRefMethod(rm);
                #endregion 流程模版管理.

                #region 开发接口.
                rm = new RefMethod();
                rm.Title = "ビジネステーブルデータと同期する";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/DTS.png";

                rm.ClassMethodName = this.ToString() + ".DoDTSBTable()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "URL呼び出しインターフェース";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/URL.png";
                rm.ClassMethodName = this.ToString() + ".DoAPI()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "SDK開発インターフェース";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/API.png";
                rm.ClassMethodName = this.ToString() + ".DoAPICode()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "コードイベント開発インターフェース";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/API.png";
                rm.ClassMethodName = this.ToString() + ".DoAPICodeFEE()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フロー属性のカスタマイズ";
                rm.ClassMethodName = this.ToString() + ".DoFlowAttrExt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "開発インターフェース";
                map.AddRefMethod(rm);

                #endregion 开发接口.

                #region 流程运行维护.
                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Title = "レポートデータを再生成する"; // "删除数据";
                rm.Warning = "実行してもよろしいですか？注：このメソッドはリソースを消費します。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoReloadRptData";
                rm.GroupName = "フローのメンテナンス";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フローのタイトルを再生成";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                //设置相关字段.
                //rm.RefAttrKey = FlowAttr.TitleRole;
                rm.RefAttrLinkLabel = "フローのタイトルを再生成";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "新しいルールに従ってタイトルを再生成してもよろしいですか？";
                rm.GroupName = "フローのメンテナンス";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "FlowEmpsフィールドを再生成します";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerFlowEmps()";
                rm.RefAttrLinkLabel = "wf_generworkflowおよびNDxxxRptフィールドを含むempsフィールドを補足的に修正";
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                rm.Warning = "再生してもよろしいですか？";
                rm.GroupName = "フローのメンテナンス";
                map.AddRefMethod(rm);

                //带有参数的方法.
                rm = new RefMethod();
                rm.GroupName = "フローのメンテナンス";
                rm.Title = "ノードフォームフィールドの名前を変更する";
                //  rm.Warning = "您确定要处理吗？";
                rm.HisAttrs.AddTBString("FieldOld", null, "古いフィールドの英語名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNew", null, "新しいフィールドの英語名", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("FieldNewName", null, "新しいフィールドの中国名", true, false, 0, 100, 100);
                rm.HisAttrs.AddBoolen("thisFlowOnly", true, "現在のフローのみ");
                rm.ClassMethodName = this.ToString() + ".DoChangeFieldName";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "ノードフォームフィールドビュー";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Field.png";
                rm.ClassMethodName = this.ToString() + ".DoFlowFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フローのメンテナンス";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "フローのすべてのデータを削除します"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.Warning = "フローデータを削除してもよろしいですか？ \t\nフローデータが削除された後は復元できません。削除されたコンテンツに注意してください。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoDelData";
                rm.GroupName = "フローのメンテナンス";
                map.AddRefMethod(rm);


                //带有参数的方法.
                rm = new RefMethod();
                rm.GroupName = "フローのメンテナンス";
                rm.Title = "指定した日付範囲内のフローを削除します";
                rm.Warning = "削除してもよろしいですか？";
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.HisAttrs.AddTBDateTime("DTFrom", null, "時間から", true, true);
                rm.HisAttrs.AddTBDateTime("DTTo", null, "時間まで", true, true);
                rm.HisAttrs.AddBoolen("thisFlowOnly", true, "現在のフローのみ");
                rm.ClassMethodName = this.ToString() + ".DoDelFlows";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "ジョブIDで削除"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.GroupName = "フローのメンテナンス";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID", 0, "ジョブIDを入力してください", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "メモを削除", true, false, 0, 100, 100);
                map.AddRefMethod(rm);


                //带有参数的方法.
                rm = new RefMethod();
                rm.GroupName = "フローのメンテナンス";
                rm.Title = "受信者の必須設定";
                rm.HisAttrs.AddTBInt("WorkID", 0, "ジョブIDを入力してください", true, false);
                rm.HisAttrs.AddTBInt("NodeID", 0, "ノードID", true, false);
                rm.HisAttrs.AddTBString("Worker", null, "受取人番号", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoSetTodoEmps";
                map.AddRefMethod(rm);





                rm = new RefMethod();
                //   rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Title = "ジョブIDによる強制終了"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.GroupName = "フローのメンテナンス";
                rm.ClassMethodName = this.ToString() + ".DoStopFlow";
                rm.HisAttrs.AddTBInt("WorkID", 0, "ジョブIDを入力してください", true, false);
                rm.HisAttrs.AddTBString("beizhu", null, "備考", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ロールバックフロー";
                rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
                // rm.Warning = "您确定要回滚它吗？";
                rm.HisAttrs.AddTBInt("workid", 0, "ロールするWorkIDを入力してください", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "ロールバックするノードID", true, false);
                rm.HisAttrs.AddTBString("note", null, "ロールバックの理由", true, false, 0, 600, 200);
                rm.GroupName = "フローのメンテナンス";
                map.AddRefMethod(rm);
                #endregion 流程运行维护.

                #region 流程监控.

                rm = new RefMethod();
                rm.Title = "設計レポート"; // "报表运行";
                rm.Icon = "../../WF/Img/Btn/Rpt.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "监控面板";
                //rm.Icon = ../../Admin/CCBPMDesigner/Img/Monitor.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_Welcome()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "流程监控";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "グラフィカル分析";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_DataCharts()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.GroupName = "フロー監視";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "統合クエリ";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Search.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Search()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.GroupName = "フロー監視";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "包括的な分析";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Group.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_Group()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.GroupName = "フロー監視";
                map.AddRefMethod(rm);

             

                

                //rm = new RefMethod();
                //rm.Title = "实例增长分析";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Grow.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceGrowOneFlow()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "流程监控";
                //rm.Visable = false;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "逾期未完成实例";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Warning.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceWarning()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "流程监控";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "逾期已完成实例";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/overtime.png";
                //rm.ClassMethodName = this.ToString() + ".DoDataManger_InstanceOverTimeOneFlow()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Visable = false;
                //rm.GroupName = "流程监控";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ログを削除";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/log.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_DeleteLog()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フロー監視";
                map.AddRefMethod(rm);


                #endregion 流程监控.

                #region 实验中的功能
                rm = new RefMethod();
                rm.Title = "データサブスクリプション実験";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/RptOrder.png";
                rm.ClassMethodName = this.ToString() + ".DoDataManger_RptOrder()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                rm.Visable = false;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "流程轨迹表单";
                //rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoBindFlowExt()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "实验中的功能";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ノードをバッチでセットアップする";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "トラック閲覧許可";
                rm.Icon = "../../WF/Img/Setting.png";
                rm.ClassMethodName = this.ToString() + ".DoTruckRight()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "データソース管理（新しいデータソースを追加した後に閉じて再度開く必要がある場合）";
                rm.ClassMethodName = this.ToString() + ".DoDBSrc";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = FlowAttr.DTSDBSrc;
                rm.RefAttrLinkLabel = "データソース管理";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.GroupName = "実験の特徴";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "監査コンポーネントの作業モードのワンクリック設定";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "監査コンポーネントモードを設定してもよろしいですか？ \t\n 1、2番目のノードの後のノードフォームはすべて2番目のノードフォームを指します。\t\n 2、終了ノードはすべて読み取り専用モードに設定されます。";
                rm.GroupName = "実験の特徴";
                rm.ClassMethodName = this.ToString() + ".DoSetFWCModel()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "NDxxxRptテーブル、冗長フィールドを削除します。";
                rm.ClassMethodName = this.ToString() + ".DoDeleteFields()";
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "監査コンポーネントモードを設定してもよろしいですか？ \t\n 1、テーブルNDxxxRptが自動的に作成されます。\t\n 2、設定フロー中に、NDxxRptテーブルにいくつかの冗長フィールドが生成されます。\t\n 3、ここでデータフィールドを削除すると、nullであり、追加フィールド";
                rm.GroupName = "実験の特徴";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "NDxxxRptテーブル、nullデータを持つ冗長フィールドを削除します";
                rm.ClassMethodName = this.ToString() + ".DoDeleteFieldsIsNull()";
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "監査コンポーネントモードを設定してもよろしいですか？ \t\n 1、テーブルNDxxxRptが自動的に作成されます。\t\n 2、セットアップフロー中に、NDxxxRptテーブルにいくつかの追加フィールドが生成されます。\t\n 3、ここでデータフィールドを削除するとnullになります。追加フィールド";
                rm.GroupName = "実験の特徴";
                map.AddRefMethod(rm);

                #endregion 实验中的功能

                //rm = new RefMethod();
                //rm.Title = "执行流程数据表与业务表数据手工同步"; 
                //rm.ClassMethodName = this.ToString() + ".DoBTableDTS";
                //rm.Icon = ../../Img/Btn/DTS.gif";
                //rm.Warning = "您确定要执行吗？如果执行了可能会对业务表数据造成错误。";
                ////设置相关字段.
                //rm.RefAttrKey = FlowAttr.DTSSpecNodes;
                //rm.RefAttrLinkLabel = "业务表字段同步配置";
                //rm.RefMethodType = RefMethodType.Func;
                //rm.Target = "_blank";
                ////map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置自动发起"; // "报表运行";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif"; 
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("Event", "事件"); // "报表运行";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowExtDataOut", "数据转出定义");  //"数据转出定义";
                // rm.Icon = "/WF/Img/Btn/Table.gif";
                //rm.ToolTip = "在流程完成时间，流程数据转储存到其它系统中应用。";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 流程监控.

        public string DoDataManger_DataCharts()
        {
            return "../../Admin/AttrFlow/DataCharts.htm?FK_Flow=" + this.No;
           // return "../../Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }
        

        public string DoDataManger_Search()
        {
            return "../../Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }
        public string DoDataManger_Group()
        {
            return "../../Comm/Group.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&WFSta=all";
        }


        public string DoDataManger_DeleteLog()
        {
            return "../../Comm/Search.htm?EnsName=BP.WF.WorkFlowDeleteLogs&FK_Flow=" + this.No + "&WFSta=all";
        }

        /// <summary>
        /// 数据订阅
        /// </summary>
        /// <returns></returns>
        public string DoDataManger_RptOrder()
        {
            return "../../Admin/CCBPMDesigner/App/RptOrder.aspx?anaTime=mouth&FK_Flow=" + this.No;
        }
        #endregion 流程监控.

        #region 开发接口.
        /// <summary>
        /// 执行删除指定日期范围内的流程
        /// </summary>
        /// <param name="dtFrom">日期从</param>
        /// <param name="dtTo">日期到</param>
        /// <param name="isOk">仅仅删除当前流程？1=删除当前流程, 0=删除全部流程.</param>
        /// <returns></returns>
        public string DoDelFlows(string dtFrom, string dtTo, string isDelCurrFlow)
        {
            if (BP.Web.WebUser.No != "admin")
                return "管理者以外のユーザーは削除できません。";

            string sql = "";
            if (isDelCurrFlow == "1")
                sql = "SELECT WorkID, FK_Flow FROM WF_GenerWorkFlow  WHERE RDT >= '" + dtFrom + "' AND RDT <= '" + dtTo + "'  AND FK_Flow='" + this.No + "' ";
            else
                sql = "SELECT WorkID, FK_Flow FROM WF_GenerWorkFlow  WHERE RDT >= '" + dtFrom + "' AND RDT <= '" + dtTo + "' ";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            string msg = "以下のフローIDが削除されます:";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string fk_flow = dr["FK_Flow"].ToString();
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, false);
                msg += " " + workid;
            }
            return msg;
        }
        /// <summary>
        /// 批量重命名字段.
        /// </summary>
        /// <param name="FieldOld"></param>
        /// <param name="FieldNew"></param>
        /// <param name="FieldNewName"></param>
        /// <returns></returns>
        public string DoChangeFieldName(string fieldOld, string fieldNew, string FieldNewName, string thisFlowOnly)
        {

            if (thisFlowOnly == "1")
                return DoChangeFieldNameOne(this, fieldOld, fieldNew, FieldNewName);

            FlowExts fls = new FlowExts();
            fls.RetrieveAll();

            string resu = "";
            foreach (FlowExt item in fls)
            {
                resu += "   ====   " + DoChangeFieldNameOne(item, fieldOld, fieldNew, FieldNewName);

            }
            return resu;
        }
        public string DoChangeFieldNameOne(FlowExt flow, string fieldOld, string fieldNew, string FieldNewName)
        {
            string result = "フィールドで実行を開始:" + fieldOld + "、名前を変更する。";
            result += "<br> ===============================================================   ";
            Nodes nds = new Nodes(flow.No);
            foreach (Node nd in nds)
            {
                result += " @実行ノード:" + nd.Name + " 結果は次のとおりです。<br>";
                result += "<br> ------------------------------------------------------------------------ ";
                MapDataExt md = new MapDataExt("ND" + nd.NodeID);
                result += "\t\n @" + md.DoChangeFieldName(fieldOld, fieldNew, FieldNewName);
            }

            result += "@ Rptの実行結果は次のとおりです。<br>";
            MapDataExt rptMD = new MapDataExt("ND" + int.Parse(flow.No) + "Rpt");
            result += "\t\n@ " + rptMD.DoChangeFieldName(fieldOld, fieldNew, FieldNewName);

            result += "@ MyRptの実行結果は次のとおりです。<br>";
            rptMD = new MapDataExt("ND" + int.Parse(flow.No) + "MyRpt");

            if (rptMD.Retrieve() > 0)
                result += "\t\n@ " + rptMD.DoChangeFieldName(fieldOld, fieldNew, FieldNewName);

            return result;
        }
        /// <summary>
        /// 字段视图
        /// </summary>
        /// <returns></returns>
        public string DoFlowFields()
        {
            return "../../Admin/AttrFlow/FlowFields.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 与业务表数据同步
        /// </summary>
        /// <returns></returns>
        public string DoDTSBTable()
        {
            return "../../Admin/AttrFlow/DTSBTable.htm?FK_Flow=" + this.No;
        }
        public string DoAPI()
        {
            return "../../Admin/AttrFlow/API.htm?FK_Flow=" + this.No;
        }
        public string DoAPICode()
        {
            return "../../Admin/AttrFlow/APICode.htm?FK_Flow=" + this.No;
        }
        public string DoAPICodeFEE()
        {
            return "../../Admin/AttrFlow/APICodeFEE.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 流程属性自定义
        /// </summary>
        /// <returns></returns>
        public string DoFlowAttrExt()
        {
            return "../../../DataUser/OverrideFiles/FlowAttrExts.html?FK_Flow=" + this.No;
        }
        public string DoVer()
        {
            return "../../Admin/AttrFlow/Ver.htm?FK_Flow=" + this.No;
        }
        public string DoPowerModel()
        {
            return "../../Admin/AttrFlow/PowerModel.htm?FK_Flow=" + this.No;
        }

        /// <summary>
        /// 时限规则
        /// </summary>
        /// <returns></returns>
        public string DoDeadLineRole()
        {
            return "../../Admin/AttrFlow/DeadLineRole.htm?FK_Flow=" + this.No;
        }

        /// <summary>
        /// 预警、超期规则
        /// </summary>
        /// <returns></returns>
        public string DoOverDeadLineRole()
        {
            return "../../Admin/AttrFlow/PushMessage.htm?FK_Flow=" + this.No;
        }


        #endregion 开发接口

        #region  基本功能
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrFlow/Action.htm?FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 流程事件
        /// </summary>
        /// <returns></returns>
        public string DoMessage()
        {
            return "../../Admin/AttrFlow/PushMessage.htm?FK_Node=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 计划玩成
        /// </summary>
        /// <returns></returns>
        public string DoSDTOfFlow()
        {
            return "../../Admin/AttrFlow/SDTOfFlow.htm?FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 节点标签
        /// </summary>
        /// <returns></returns>
        public string DoNodesICON()
        {
            return "../../Admin/AttrFlow/NodesIcon.htm?FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoDBSrc()
        {
            return "../../Comm/Sys/SFDBSrcNewGuide.htm";
        }
        public string DoBTable()
        {
            return "../../Admin/AttrFlow/DTSBTable.aspx?s=d34&ShowType=FlowFrms&FK_Node=" + int.Parse(this.No) + "01&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }

        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return "../../Admin/AttrFlow/NodeAttrs.htm?NodeID=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoBindFlowExt()
        {
            return "../../Admin/Sln/BindFrms.htm?s=d34&ShowType=FlowFrms&FK_Node=0&FK_Flow=" + this.No + "&ExtType=StartFlow";
        }
        /// <summary>
        /// 轨迹查看权限
        /// </summary>
        /// <returns></returns>
        public string DoTruckRight()
        {
            return "../../Admin/AttrFlow/TruckViewPower.htm?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 批量发起字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrNode/BatchStartFields.htm?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow";
        }
        /// <summary>
        /// 执行流程数据表与业务表数据手工同步
        /// </summary>
        /// <returns></returns>
        public string DoBTableDTS()
        {
            Flow fl = new Flow(this.No);
            return fl.DoBTableDTS();

        }
        /// <summary>
        /// 恢复已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// </summary>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoRebackFlowData(Int64 workid, int backToNodeID, string note)
        {
            if (note.Length <= 2)
                return "完了したフローを復元する理由を記入してください。";

            Flow fl = new Flow(this.No);
            BP.WF.Data.GERpt rpt = new BP.WF.Data.GERpt("ND" + int.Parse(this.No) + "Rpt");
            rpt.OID = workid;
            int i = rpt.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@エラー、失われたフローデータ。");
            if (backToNodeID == 0)
                backToNodeID = rpt.FlowEndNode;

            Emp empStarter = new Emp(rpt.FlowStarter);

            // 最后一个节点.
            Node endN = new Node(backToNodeID);
            GenerWorkFlow gwf = null;
            bool isHaveGener = false;
            try
            {
                #region 创建流程引擎主表数据.
                gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                if (gwf.RetrieveFromDBSources() == 1)
                {
                    isHaveGener = true;
                    //判断状态
                    if (gwf.WFState != WFState.Complete)
                        throw new Exception("@現在のジョブIDは:" + workid + "フローは終了しておらず、この方法では回復できません。");
                }

                gwf.FK_Flow = this.No;
                gwf.FlowName = this.Name;
                gwf.WorkID = workid;
                gwf.PWorkID = rpt.PWorkID;
                gwf.PFlowNo = rpt.PFlowNo;
                gwf.PNodeID = rpt.PNodeID;
                gwf.PEmp = rpt.PEmp;


                gwf.FK_Node = backToNodeID;
                gwf.NodeName = endN.Name;

                gwf.Starter = rpt.FlowStarter;
                gwf.StarterName = empStarter.Name;
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.SysType = fl.SysType;

                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                gwf.FK_Dept = rpt.FK_Dept;

                Dept dept = new Dept(empStarter.FK_Dept);

                gwf.DeptName = dept.Name;
                gwf.PRI = 1;

                DateTime dttime = DateTime.Now;
                dttime = dttime.AddDays(3);

                gwf.SDTOfNode = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                gwf.SDTOfFlow = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                if (isHaveGener)
                    gwf.Update();
                else
                    gwf.Insert(); /*插入流程引擎数据.*/

                #endregion 创建流程引擎主表数据
                string ndTrack = "ND" + int.Parse(this.No) + "Track";
                string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
                string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                System.Data.DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@ジョブIDは:" + workid + "データは存在しません。");

                string starter = "";
                bool isMeetSpecNode = false;
                GenerWorkerList currWl = new GenerWorkerList();
                foreach (DataRow dr in dt.Rows)
                {
                    int ndFrom = int.Parse(dr["NDFrom"].ToString());
                    Node nd = new Node(ndFrom);

                    string ndFromT = dr["NDFromT"].ToString();
                    string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                    string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                    // 增加上 工作人员的信息.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    gwl.FK_Flow = this.No;

                    gwl.FK_Node = ndFrom;
                    gwl.FK_NodeText = ndFromT;

                    if (gwl.FK_Node == backToNodeID)
                    {
                        gwl.IsPass = false;
                        currWl = gwl;
                    }
                    else
                        gwl.IsPass = true;

                    gwl.FK_Emp = EmpFrom;
                    gwl.FK_EmpText = EmpFromT;
                    if (gwl.IsExits)
                        continue; /*有可能是反复退回的情况.*/

                    Emp emp = new Emp(gwl.FK_Emp);
                    gwl.FK_Dept = emp.FK_Dept;

                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;

                    gwl.IsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                #region 加入退回信息, 让接受人能够看到退回原因.
                ReturnWork rw = new ReturnWork();
                rw.Delete(ReturnWorkAttr.WorkID, workid); //先删除历史的信息.

                rw.WorkID = workid;
                rw.ReturnNode = backToNodeID;
                rw.ReturnNodeName = endN.Name;
                rw.Returner = WebUser.No;
                rw.ReturnerName = WebUser.Name;

                rw.ReturnToNode = currWl.FK_Node;
                rw.ReturnToEmp = currWl.FK_Emp;
                rw.BeiZhu = note;
                rw.RDT = DataType.CurrentDataTime;
                rw.IsBackTracking = false;
                rw.MyPK = BP.DA.DBAccess.GenerGUID();
                rw.Insert();
                #endregion   加入退回信息, 让接受人能够看到退回原因.

                //更新流程表的状态.
                rpt.FlowEnder = currWl.FK_Emp;
                rpt.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                rpt.FlowEndNode = currWl.FK_Node;
                rpt.Update();

                // 向接受人发送一条消息.
                BP.WF.Dev2Interface.Port_SendMsg(currWl.FK_Emp, "作業履歴書:" + gwf.Title, "仕事は:" + WebUser.No + " 回復。" + note, "ReBack" + workid, BP.WF.SMSMsgType.SendSuccess, this.No, int.Parse(this.No + "01"), workid, 0);

                //写入该日志.
                WorkNode wn = new WorkNode(workid, currWl.FK_Node);
                wn.AddToTrack(ActionType.RebackOverFlow, currWl.FK_Emp, currWl.FK_EmpText, currWl.FK_Node, currWl.FK_NodeText, note);

                return "@正常に復元され、現在のフローがに復元されました(" + currWl.FK_NodeText + ")。@人工的な現在のジョブ処理(" + currWl.FK_Emp + " , " + currWl.FK_EmpText + ")  @仕事をするように彼に通知してください。";
            }
            catch (Exception ex)
            {
                //此表的记录删除已取消
                //gwf.Delete();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Delete(GenerWorkerListAttr.WorkID, workid);

                string sqls = "";
                sqls += "@UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Complete + " WHERE OID=" + workid;
                DBAccess.RunSQLs(sqls);
                return "<font color=red>ロールオーバー中にエラーが発生しました</font><hr>" + ex.Message;
            }
        }
        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerFlowEmps()
        {
            if (WebUser.No != "admin")
                return "管理者以外のユーザーは実行できません。";

            Flow fl = new Flow(this.No);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.No);

            foreach (GenerWorkFlow gwf in gwfs)
            {
                string emps = "@";
                string sql = "SELECT EmpFrom,EmpFromT FROM ND" + int.Parse(this.No) + "Track  WHERE WorkID=" + gwf.WorkID;

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (emps.Contains("@" + dr[0].ToString() + "@") || emps.Contains("@" + dr[0].ToString() + ","+dr[1].ToString()+"@"))
                        continue;
                    emps += dr[0].ToString() + "," + dr[1].ToString() + "@";
                }

                sql = "UPDATE " + fl.PTable + " SET FlowEmps='" + emps + "' WHERE OID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);

                sql = "UPDATE WF_GenerWorkFlow SET Emps='" + emps + "' WHERE WorkID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);
            }

            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {
                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);


            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "すべてが正常に生成され、データに影響します(" + wks.Count + ")件";
        }

        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerTitle()
        {
            if (WebUser.No != "admin")
                return "管理者以外のユーザーは実行できません。";
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "すべてが正常に生成され、データに影響します(" + wks.Count + ")件";
        }

        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFlowFormTree()
        {
            return "../../Admin/FlowFormTree.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoAutoStartIt();
        }

        /// <summary>
        /// 强制设置接受人
        /// </summary>
        /// <param name="workid">工作人员ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="worker">工作人员</param>
        /// <returns>执行结果.</returns>
        public string DoSetTodoEmps(int workid, int nodeID, string worker)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
                return "workid=" + workid + "不正解。";

            BP.Port.Emp emp = new Emp();
            emp.No = worker;
            if (emp.RetrieveFromDBSources() == 0)
                return "不正な従業員番号" + worker + ".";

            BP.WF.Node nd = new Node();
            nd.NodeID = nodeID;
            if (nd.RetrieveFromDBSources() == 0)
                return "err@ノード番号[" + nodeID + "]不正解。";

            gwf.FK_Node = nodeID;
            gwf.NodeName = nd.Name;
            gwf.TodoEmps = emp.No + "," + emp.Name + ";";
            gwf.TodoEmpsNum = 1;
            gwf.HuiQianTaskSta = HuiQianTaskSta.None;
            gwf.Update();

            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=1 WHERE WorkID=" + workid);

            GenerWorkerList gwl = new GenerWorkerList();
            gwl.FK_Node = nodeID;
            gwl.WorkID = workid;
            gwl.FK_Emp = emp.No;
            if (gwl.RetrieveFromDBSources() == 0)
            {
                DateTime dt = DateTime.Now;
                gwl.FK_EmpText = emp.Name;

                if (nd.HisCHWay == CHWay.None)
                    gwl.SDT = "なし";
                else
                    gwl.SDT = dt.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

                gwl.RDT = dt.ToString("yyyy-MM-dd HH:mm:ss");
                gwl.IsRead = false;
                gwl.Insert();
            }
            else
            {
                gwl.IsRead = false;
                gwl.IsPassInt = 0;
                gwl.Update();
            }
            return "実行成功。";
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string DoDelDataOne(int workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.No, workid, true);
                return "正常に削除されました workid=" + workid + " 理由:" + note;
            }
            catch (Exception ex)
            {
                return "削除できませんでした:" + ex.Message;
            }
        }
        public string DoStopFlow(Int64 workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoFlowOver(this.No, workid, note);
                return "フローは作業IDを強制的に終了させます workid=" + workid + " 理由:" + note;
            }
            catch (Exception ex)
            {
                return "削除できませんでした:" + ex.Message;
            }
        }
        /// <summary>
        /// 设置发起数据源
        /// </summary>
        /// <returns></returns>
        public string DoSetStartFlowDataSources()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                throw new Exception("着信フロー番号が空です。フローを確認してください");
            string flowID = int.Parse(this.No).ToString() + "01";
            return "../../Admin/AttrFlow/AutoStart.htm?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=";
        }
        public string DoCCNode()
        {
            return "../../Admin/CCNode.aspx?FK_Flow=" + this.No;
        }
        /// <summary>
        /// 执行运行
        /// </summary>
        /// <returns></returns>
        public string DoRunIt()
        {
            return "../../Admin/TestFlow.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            return "../../Admin/AttrFlow/CheckFlow.htm?FK_Flow=" + this.No + "&Lang=CH";
        }

        public string DoCheck2018Url()
        {
            return "../../Admin/Testing/FlowCheckError.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 启动限制规则
        /// </summary>
        /// <returns>返回URL</returns>
        public string DoLimit()
        {
            return "../../Admin/AttrFlow/Limit.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 设置发起前置导航
        /// </summary>
        /// <returns></returns>
        public string DoStartGuide()
        {
            return "../../Admin/AttrFlow/StartGuide.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 设置发起前置导航
        /// </summary>
        /// <returns></returns>
        public string DoStartGuideV2019()
        {
            return "../../Admin/AttrFlow/StartGuide/Default.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行数据同步
        /// </summary>
        /// <returns></returns>
        public string DoDTS()
        {
            return "../../Admin/AttrFlow/DTSBTable.aspx?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public string DoImp()
        {
            return "../../Admin/AttrFlow/Imp.htm?FK_Flow=" + this.No + "&Lang=CH";
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string DoExps()
        {
            return "../../Admin/AttrFlow/Exp.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行重新装载数据
        /// </summary>
        /// <returns></returns>
        public string DoReloadRptData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoReloadRptData();
        }
        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <returns></returns>
        public string DoDelData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDelData();
        }
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                throw new Exception("着信フロー番号が空です。フローを確認してください");
            return "../../Admin/RptDfine/Default.htm?FK_Flow=" + this.No + "&DoType=Edit&FK_MapData=ND" + int.Parse(this.No) + "Rpt";
        }
        /// <summary>
        /// 更新之后的事情，也要更新缓存。
        /// </summary>
        protected override void afterUpdate()
        {
            // Flow fl = new Flow();
            // fl.No = this.No;
            // fl.RetrieveFromDBSources();
            //fl.Update();

            if (BP.WF.Glo.OSModel == OSModel.OneMore)
            {
                // DBAccess.RunSQL("UPDATE  GPM_Menu SET Name='" + this.Name + "' WHERE Flag='Flow" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            }
        }
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.No);

            #region 同步事件实体.
            try
            {
                string flowMark = this.FlowMark;
                if (DataType.IsNullOrEmpty(flowMark) == true)
                    flowMark = this.No;

                this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityStringByFlowMark(flowMark);
            }
            catch
            {
                this.FlowEventEntity = "";
            }
            #endregion 同步事件实体.

            //更新缓存数据。
            Flow fl = new Flow(this.No);
            fl.RetrieveFromDBSources();

            #region StartFlows的清缓存
            if (fl.IsStartInMobile != this.IsStartInMobile || fl.IsCanStart != this.IsCanStart)
            {
                //清空WF_Emp 的StartFlows
                DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");
            }
            #endregion StartFlows的清缓存

            fl.Copy(this);

            //2019-09-25 -by zhoupeng 为周大福增加 关键业务字段.
            fl.BuessFields = this.GetValStrByKey(FlowAttr.BuessFields);
            fl.DirectUpdate();

            #region 检查数据完整性 - 同步业务表数据。
            // 检查业务是否存在.
            if (fl.DTSWay != FlowDTSWay.None)
            {
                /*检查业务表填写的是否正确.*/
                string sql = "select count(*) as Num from  " + fl.DTSBTable + " where 1=2";
                try
                {
                    DBAccess.RunSQLReturnValInt(sql, 0);
                }
                catch (Exception)
                {
                    throw new Exception("@ビジネステーブルの構成が無効です。ビジネスデータテーブルを構成します[" + fl.DTSBTable + "]データに存在しない場合は、スペルエラーを確認してください。クロスデータベースの場合は、次のようなユーザー名を追加してください：for sqlserver：HR.dbo.Emps、For oracle：HR.Emps");
                }

                sql = "select " + fl.DTSBTablePK + " from " + fl.DTSBTable + " where 1=2";
                try
                {
                    DBAccess.RunSQLReturnValInt(sql, 0);
                }
                catch (Exception)
                {
                    throw new Exception("@ビジネステーブルの構成が無効です。ビジネスデータテーブルを構成します[" + fl.DTSBTablePK + "]主キーが存在しません。");
                }


                //检查节点配置是否符合要求.
                if (fl.DTSTime == FlowDTSTime.SpecNodeSend)
                {
                    // 检查节点ID，是否符合格式.
                    string nodes = fl.DTSSpecNodes;
                    nodes = nodes.Replace("，", ",");
                    this.SetValByKey(FlowAttr.DTSSpecNodes, nodes);

                    if (DataType.IsNullOrEmpty(nodes) == true)
                        throw new Exception("@業務データ同期データ構成エラー、指定したとおりにノード構成を設定しましたが、ノードを設定していません。ノードの設定形式は以下のとおりです：101,102,103");

                    string[] strs = nodes.Split(',');
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        if (BP.DA.DataType.IsNumStr(str) == false)
                            throw new Exception("@ビジネスデータ同期データ構成エラー、指定したとおりにノード構成を設定しましたが、ノードの形式が間違っています[" + nodes + "]。正しい形式は次のとおりです：101、102、103");

                        Node nd = new Node();
                        nd.NodeID = int.Parse(str);
                        if (nd.IsExits == false)
                            throw new Exception("@ビジネスデータ同期データ構成エラー、設定したノード形式が正しくありません、ノード[" + str + "]有効なノードではありません。");

                        nd.RetrieveFromDBSources();
                        if (nd.FK_Flow != this.No)
                            throw new Exception("@ビジネスデータ同期データ構成エラー、設定したノード[" + str + "]このフローではもうありません。");
                    }
                }

                //检查流程数据存储表是否正确
                if (!DataType.IsNullOrEmpty(fl.PTable))
                {
                    /*检查流程数据存储表填写的是否正确.*/
                    sql = "select count(*) as Num from  " + fl.PTable + " where 1=2";
                    try
                    {
                        DBAccess.RunSQLReturnValInt(sql, 0);
                    }
                    catch (Exception)
                    {
                        throw new Exception("@フローデータストレージテーブルの構成が無効です。フローデータストレージテーブルを構成します[" + fl.PTable + "]データに存在しない場合は、スペルエラーを確認してください。クロスデータベースの場合は、次のようなユーザー名を追加してください：for sqlserver：HR.dbo.Emps、For oracle：HR.Emps");
                    }
                }
            }
            #endregion 检查数据完整性. - 同步业务表数据。



            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            //同步流程数据表.
            string ndxxRpt = "ND" + int.Parse(this.No) + "Rpt";
            Flow fl = new Flow(this.No);
            if (fl.PTable != "ND" + int.Parse(this.No) + "Rpt")
            {
                BP.Sys.MapData md = new Sys.MapData(ndxxRpt);
                if (md.PTable != fl.PTable)
                    md.Update();
            }

            #region 为systype设置，当前所在节点的第2级别目录。
            FlowSort fs = new FlowSort(fl.FK_FlowSort);
            if (fs.ParentNo == "99" || fs.ParentNo == "0")
            {
                this.SysType = fl.FK_FlowSort;
            }
            else
            {
                FlowSort fsP = new FlowSort(fs.ParentNo);
                if (fsP.ParentNo == "99" || fsP.ParentNo == "0")
                {
                    this.SysType = fsP.No;
                }
                else
                {
                    FlowSort fsPP = new FlowSort(fsP.ParentNo);
                    this.SysType = fsPP.No;
                }
            }
            #endregion 为systype设置，当前所在节点的第2级别目录。

            fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            fl.Update();



            base.afterInsertUpdateAction();
        }
        #endregion

        #region 实验中的功能.
        /// <summary>
        /// 删除多余的字段.
        /// </summary>
        /// <returns></returns>
        public string DoDeleteFields()
        {
            return "まだ終わってない。";
        }
        /// <summary>
        /// 删除多余的字段.
        /// </summary>
        /// <returns></returns>
        public string DoDeleteFieldsIsNull()
        {
            return "まだ終わってない。";
        }
        /// <summary>
        /// 一件设置审核模式.
        /// </summary>
        /// <returns></returns>
        public string DoSetFWCModel()
        {
            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                if (nd.IsStartNode)
                {
                    nd.HisFormType = NodeFormType.FoolForm;
                    nd.Update();
                    continue;
                }

                BP.WF.Template.FrmNodeComponent fnd = new FrmNodeComponent(nd.NodeID);

                if (nd.IsEndNode == true || nd.HisToNodes.Count == 0)
                {
                    nd.FrmWorkCheckSta = FrmWorkCheckSta.Readonly;
                    nd.NodeFrmID = "ND" + int.Parse(this.No) + "02";
                    nd.HisFormType = NodeFormType.FoolForm;
                    nd.Update();


                    fnd.SetValByKey(NodeAttr.NodeFrmID, nd.NodeFrmID);
                    fnd.SetValByKey(NodeAttr.FWCSta, (int)nd.FrmWorkCheckSta);

                    fnd.Update();
                    continue;
                }

                //  fnd.HisFormType = NodeFormType.FoolForm;

                fnd.Update(); //不执行更新，会导致部分字段错误.

                nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                nd.NodeFrmID = "ND" + int.Parse(this.No) + "02";
                nd.HisFormType = NodeFormType.FoolForm;
                nd.Update();
            }

            return "正常に設定されました…";
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class FlowExts : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.Template.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.Template.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public FlowExts() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowExts(string fk_sort)
        {
            this.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowExt> Tolist()
        {
            System.Collections.Generic.List<FlowExt> list = new System.Collections.Generic.List<FlowExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

