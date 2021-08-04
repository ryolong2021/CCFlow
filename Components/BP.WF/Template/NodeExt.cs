using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;
using System.IO;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点属性.
    /// </summary>
    public class NodeExt : Entity
    {


        #region 常量
        /// <summary>
        /// CCFlow流程引擎
        /// </summary>
        private const string SYS_CCFLOW = "001";
        /// <summary>
        /// CCForm表单引擎
        /// </summary>
        private const string SYS_CCFORM = "002";
        #endregion

        #region 属性.
        /// <summary>
        /// 会签规则
        /// </summary>
        public HuiQianRole HuiQianRole
        {
            get
            {
                return (HuiQianRole)this.GetValIntByKey(BtnAttr.HuiQianRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.HuiQianRole, (int)value);
            }
        }

        public HuiQianLeaderRole HuiQianLeaderRole
        {
            get
            {
                return (HuiQianLeaderRole)this.GetValIntByKey(BtnAttr.HuiQianLeaderRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.HuiQianLeaderRole, (int)value);
            }
        }
        /// <summary>
        /// 超时处理方式
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        /// <summary>
        /// 步骤
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 审核组件状态
        /// </summary>
        public FrmWorkCheckSta HisFrmWorkCheckSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeAttr.FWCSta);
            }
        }

        public FWCAth FWCAth
        {
            get
            {
                return (FWCAth)this.GetValIntByKey(FrmWorkCheckAttr.FWCAth);
            }
            set
            {
                this.SetValByKey(FrmWorkCheckAttr.FWCAth, (int)value);
            }
        }

        /// <summary>
        /// 超时处理内容
        /// </summary>
        public string DoOutTime
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTime);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTime, value);
            }
        }
        /// <summary>
        /// 超时处理条件
        /// </summary>
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FlowName);
            }
            set
            {
                this.SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 接受人sql
        /// </summary>
        public string DeliveryParas11
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// 是否可以退回
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        public bool IsYouLiTai
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsYouLiTai);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }

        #endregion 属性.

        #region 初试化全局的 Node
        /// <summary>
        /// 访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (this.FK_Flow == "")
                {
                    uac.OpenForAppAdmin();
                    return uac;
                }

                Flow fl = new Flow(this.FK_Flow);
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                else
                    uac.IsUpdate = true; //权宜之计.
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeExt() { }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public NodeExt(int nodeid)
        {
            this.NodeID = nodeid;
            this.Retrieve();
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

                Map map = new Map("WF_Node", "ノード");
                //map 的基 础信息.
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);

                map.IndexField = NodeAttr.FK_Flow;

                #region  基础属性
                map.AddTBIntPK(NodeAttr.NodeID, 0, "ノードID", true, true);
                map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");

                map.AddTBInt(NodeAttr.Step, 0, "ステップ（計算の意味なし）", true, false);
                map.SetHelperUrl(NodeAttr.Step, "http://ccbpm.mydoc.io/?v=5404&t=17902");

                //map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "フロー番号", false, false, 3, 3, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17023");
                map.AddTBString(NodeAttr.FlowName, null, "フロー名", false, true, 0, 200, 10);

                map.AddTBString(NodeAttr.Name, null, "名前", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17903");
                map.SetHelperAlert(NodeAttr.Name, "ノード名を変更するとき、ノードフォーム名が空白の場合、ノードフォーム名とノード名は同じです。それ以外の場合、ノード名とノードフォーム名は異なる場合があります。");
                map.AddTBString(NodeAttr.Tip, null, "操作のヒント", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=18084");

                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "誰が実行するか", true, true, NodeAttr.WhoExeIt,
                    "@0=オペレーター実行@1=マシン実行@2=混合実行");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "http://ccbpm.mydoc.io/?v=5404&t=17913");


                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "領収書を読む", true, true, NodeAttr.ReadReceipts,
                "@0=領収書なし@1=自動領収書@2=前のノードのフォームフィールドによって決定されました@3=SDK開発者パラメーターによって決定されました");
                map.SetHelperUrl(NodeAttr.ReadReceipts, "http://ccbpm.mydoc.io/?v=5404&t=17915");

                
                //map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 300, 10);
                
                //map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                  //  "@0=由连接线条件控制@1=按照用户选择计算@2=发送按钮旁下拉框选择");
                //map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //增加帮助


                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole, (int)CancelRole.OnlyNextStep, "引戻ルール", true, true,
                    NodeAttr.CancelRole, "@0=前のステップを引戻ことができます@1=引戻ことはできません@2=前のステップと開始ノードを引戻ことができます@3=指定したノードを引戻ことができます");
                map.SetHelperUrl(NodeAttr.CancelRole, "http://ccbpm.mydoc.io/?v=5404&t=17919");

                map.AddBoolean(NodeAttr.CancelDisWhenRead, false, "相手が開いており、元に戻すことはできません", true, true);

                map.AddBoolean(NodeAttr.IsTask, true, "割り当ては許可されていますか？", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                map.AddBoolean(NodeAttr.IsExpSender, true, "このノードの受信者は、前のステップの送信者を含めることはできません", true, true, false);
                map.AddBoolean(NodeAttr.IsRM, true, "配信経路の自動メモリー機能は有効になっていますか？", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17905");
                map.AddBoolean(NodeAttr.IsOpenOver, false, "読み終わった？", true, true, true);
                map.SetHelperAlert(NodeAttr.IsOpenOver, "受信者がジョブを開くと、送信ボタンをクリックしてジョブに完了のマークを付けるのではなく、ジョブに完了のマークを付けます"); //增加帮助


                map.AddBoolean(NodeAttr.IsToParentNextNode, false, "子フローがこのノードで実行されると、親フローが次のステップまで自動的に実行されるようにします", true, true);
                map.AddBoolean(NodeAttr.IsYouLiTai, false, "ノードが空いているかどうか", true, true);
                map.SetHelperUrl(NodeAttr.IsYouLiTai, "ノードがフリー状態の場合、接続されたノードのみが固定ノードであり、それがダウンしても実行できます。それ以外の場合、フローは終了します");

                map.AddTBDateTime("DTFrom", "ライフサイクルから", true, true);
                map.AddTBDateTime("DTTo", "ライフサイクルまで", true, true);

                map.AddBoolean(NodeAttr.IsBUnit, false, "ノードテンプレート（ビジネスユニット）ですか？", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");

                map.AddTBString(NodeAttr.FocusField, null, "重点分野", true, false, 0, 50, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17932");

                map.AddBoolean(NodeAttr.IsGuestNode, false, "外部ユーザーが実行するノード（非組織者が処理に参加するノード）ですか？", true, true, true);

                //节点业务类型.
                map.AddTBInt("NodeAppType", 0, "ノードビジネスタイプ", false, false);
                map.AddTBInt("FWCSta", 0, "ノードのステータス", false, false);
                map.AddTBInt("FWCAth", 0, "添付ファイルの監査が有効になっています", false, false);


                map.AddTBString(NodeAttr.SelfParas, null, "カスタム属性", true, false, 0, 500, 10, true);

                #endregion  基础属性

                #region 分合流子线程属性
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "ノードタイプ",
                    true, false, NodeAttr.RunModel, "@0=通常@1=合流@2=分流@3=分合流@4=サブスレッド");

                map.SetHelperUrl(NodeAttr.RunModel, "http://ccbpm.mydoc.io/?v=5404&t=17940"); //增加帮助.

                //子线程类型.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "子スレッドタイプ", true, false, NodeAttr.SubThreadType, "@0=同フォーム@1=異なるフォーム");
                map.SetHelperUrl(NodeAttr.SubThreadType, "http://ccbpm.mydoc.io/?v=5404&t=17944"); //增加帮助

                map.AddTBDecimal(NodeAttr.PassRate, 100, "完全合格率", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, "http://ccbpm.mydoc.io/?v=5404&t=17945"); //增加帮助.

                // 启动子线程参数 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "子スレッド開始モード", true, true,
                    NodeAttr.SubFlowStartWay, "@0=アクティブ化しない@1=指定されたフィールドをアクティブ化する@2=スケジュールに従ってアクティブ化する");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "起動パラメーター", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, "http://ccbpm.mydoc.io/?v=5404&t=17946"); //增加帮助


                //增加对退回到合流节点的 子线城的处理控制.
                map.AddBoolean(BtnAttr.ThreadIsCanDel, false, "子スレッドを削除できますか（現在のノードが送信したスレッドで、現在のノードが分割されているか、分割とマージが有効で、子スレッドが戻った後の操作）？", true, true, true);
                map.AddBoolean(BtnAttr.ThreadIsCanShift, false, "子スレッドを引き渡すことができますか（現在のノードが送信したスレッドで、現在のノードが分割されているか、分割とマージが有効で、子スレッドが戻った後の操作）？", true, true, true);

                ////待办处理模式.
                //map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "多人待办处理模式", true, true, NodeAttr.TodolistModel,
                //    "@0=抢办模式@1=协作模式@2=队列模式@3=共享模式@4=协作组长模式");
                //map.SetHelperUrl(NodeAttr.TodolistModel, "http://ccbpm.mydoc.io/?v=5404&t=17947"); //增加帮助.

                //发送阻塞模式.
                //map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "发送阻塞模式", true, true, NodeAttr.BlockModel,
                //    "@0=不阻塞@1=当前节点有未完成的子流程时@2=按约定格式阻塞未完成子流程@3=按照SQL阻塞@4=按照表达式阻塞");
                //map.SetHelperUrl(NodeAttr.BlockModel, "http://ccbpm.mydoc.io/?v=5404&t=17948"); //增加帮助.

                //map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 700, 10,true);
                //map.SetHelperUrl(NodeAttr.BlockExp, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                //map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞时提示信息", true, false, 0, 700, 10, true);
                //map.SetHelperUrl(NodeAttr.BlockAlert, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "子スレッドはスタッフの繰り返しを受け入れることができますか（転換点が子スレッドに送信された場合にのみ有効）？", true, true, true);

                map.AddBoolean(NodeAttr.AutoRunEnable, false, "自動実行は有効になっていますか？ （シャントポイントが子スレッドに送信された場合のみ有効）", true, true, true);
                map.AddTBString(NodeAttr.AutoRunParas, null, "SQLを自動的に実行する", true, false, 0, 100, 10, true);
                #endregion 分合流子线程属性

                #region 自动跳转规则
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "操作者は本人です", true, true, true);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, "http://ccbpm.mydoc.io/?v=5404&t=17949"); //增加帮助

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "操作者は既に処理済です", true, true, true);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "操作者は前のスデップと同じです", true, true, true);
                map.AddBoolean(NodeAttr.WhenNoWorker, false, "（はい）見つからない場合は遷移させる→（はい）見つからない場合は飛び越えさせる", true, true, true);
                //         map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "找不到处理人处理规则",
                //true, true, NodeAttr.WhenNoWorker, "@0=提示错误@1=自动转到下一步");
                #endregion

                #region  功能按钮状态
                map.AddTBString(BtnAttr.SendLab, "送る", "送るボタンのラベル", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SendLab, "http://ccbpm.mydoc.io/?v=5404&t=16219");
                map.AddTBString(BtnAttr.SendJS, "", "ボタンJS機能", true, false, 0, 999, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow 发送前数据完整性判断"); //增加帮助.
                map.SetHelperUrl(BtnAttr.SendJS, "http://ccbpm.mydoc.io/?v=5404&t=17967");

                map.AddTBString(BtnAttr.SaveLab, "保存する", "保存するボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "有効にするかどうか", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, "http://ccbpm.mydoc.io/?v=5404&t=24366"); //增加帮助

                map.AddTBString(BtnAttr.ThreadLab, "子スレッド", "子スレッドボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "有効にするかどうか", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, "http://ccbpm.mydoc.io/?v=5404&t=16263"); //增加帮助

                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "子スレッドの削除方法", true, true,
           NodeAttr.ThreadKillRole, "@0=削除できません@1=手動で削除できません@2=自動的に削除できません", true);

                //map.SetHelperUrl(NodeAttr.ThreadKillRole, ""); //增加帮助

                //功能和子流程组件重复，屏蔽 hzm
                //  map.AddTBString(BtnAttr.SubFlowLab, "子流程", "子流程按钮标签", true, false, 0, 50, 10);
                //  map.SetHelperUrl(BtnAttr.SubFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=16262");
                //   map.AddBoolean(BtnAttr.SubFlowEnable, false, "是否启用", true, true);
                //map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "控制规则", true, true, BtnAttr.SubFlowCtrlRole, "@0=无@1=不可以删除子流程@2=可以删除子流程");

                map.AddTBString(BtnAttr.JumpWayLab, "遷移", "遷移ボタンのラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "遷移ルール", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "遷移可能なノード", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, "http://ccbpm.mydoc.io/?v=5404&t=16261"); //增加帮助.

                map.AddTBString(BtnAttr.ReturnLab, "差戻", "差戻ボタンのラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0, "差戻ルール", true, true, NodeAttr.ReturnRole);
                map.SetHelperUrl(NodeAttr.ReturnRole, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.
                map.AddTBString(NodeAttr.ReturnAlert, null, "差戻後の情報プロンプト", true, false, 0, 999, 10, true);
                map.AddBoolean(NodeAttr.IsBackTracking, false, "同じように戻ることができるか（戻り機能が有効な場合にのみ有効）", true, true, false);
                //map.AddTBString(NodeAttr.RetunFieldsLable, "退回扩展字段", "退回扩展字段", true, false, 0, 50, 20);


                map.AddTBString(BtnAttr.ReturnField, "", "フィールドに入力する情報を差戻", true, false, 0, 50, 10);
                map.SetHelperUrl(NodeAttr.IsBackTracking, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.

                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "差戻理由", true, false, 0, 999, 10, true);

                map.AddBoolean(NodeAttr.ReturnCHEnable, false, "差戻評価ルールを有効にするかどうか", true, true);

                map.AddDDLSysEnum(NodeAttr.ReturnOneNodeRole, 0, "単一ノードの差戻ルール", true, true, NodeAttr.ReturnOneNodeRole,
                   "@@0=無効にする@1= [コメントを入力フィールドに入力]に従って直接コメントを差戻@2= [レビューコンポーネント]に入力された情報に従ってコメントを直接差戻", true);


                map.AddTBString(BtnAttr.CCLab, "CC", "CCボタンのラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "CCルール", true, true, NodeAttr.CCRole,
                    "@@0=CCできない@1=手動CC@2=自動CC@3=手動および自動@4=フォームのSysCCEmpsフィールドに従って計算@5=送信する前にCCウィンドウを開く");
                map.SetHelperUrl(BtnAttr.CCLab, "http://ccbpm.mydoc.io/?v=5404&t=16259"); //增加帮助.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "Ccルールをコピー",
             true, true, NodeAttr.CCWriteTo, "@0=Ccリストに書き込み@1=to-do@2=書き込みto-doとCcリストに書き込み", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, "http://ccbpm.mydoc.io/?v=5404&t=17976"); //增加帮助.
                map.AddTBString(NodeAttr.DoOutTime, null, "残業コンテンツ", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "実行タイムアウトの条件", false, false, 0, 200, 100);

                map.AddTBString(BtnAttr.ShiftLab, "転送", "転送ボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "有効にするかどうか", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, "http://ccbpm.mydoc.io/?v=5404&t=16257"); //增加帮助.note:none

                map.AddTBString(BtnAttr.DelLab, "削除する", "削除するボタンのラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "ルールを削除", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, "http://ccbpm.mydoc.io/?v=5404&t=17992"); //增加帮助.

                map.AddTBString(BtnAttr.EndFlowLab, "フローを終了する", "フローを終了するボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "有効にするかどうか", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17989"); //增加帮助

                map.AddTBString(BtnAttr.ShowParentFormLab, "親フローを表示", "親フローを表示ボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShowParentFormEnable, false, "有効にするかどうか", true, true);

                // add 2019.1.9 for 东孚.
                map.AddTBString(BtnAttr.OfficeBtnLab, "公式ドキュメントを開く", "くもんボタンラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.OfficeBtnEnable, 0, "ファイルの状態", true, true, BtnAttr.OfficeBtnEnable,
                "@0=利用不可@1=編集可能@2=編集不可", false);

                //map.AddTBString(BtnAttr.OfficeBtnLab, "公文主文件", "公文按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OfficeBtnEnable, false, "是否启用", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintHtmlLab, "HTMLを印刷", "HTMLを印刷ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintHtmlEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.PrintPDFLab, "PDFを印刷", "PDFを印刷ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "有効にするかどうか", true, true);
                map.AddDDLSysEnum(BtnAttr.PrintPDFModle, 0, "PDF印刷ルール", true, true, BtnAttr.PrintPDFModle, "@0=全部印刷@1=単一フォームの印刷（ツリーフォームの場合）", true);
                map.AddTBString(BtnAttr.ShuiYinModle, null, "透かしルールの印刷", true, false, 20, 100, 100, true);

                map.AddTBString(BtnAttr.PrintZipLab, "パッケージをダウンロード", "パッケージをダウンロードzipボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.PrintDocLab, "領収書を印刷する", "領収書を印刷するボタンのラベル", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "打印方式", true,
                //    true, BtnAttr.PrintDocEnable, "@0=不打印@1=打印网页@2=打印RTF模板@3=打印Word模版");
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "有効にするかどうか", true, true);
                //map.SetHelperUrl(BtnAttr.PrintDocEnable, "http://ccbpm.mydoc.io/?v=5404&t=17979"); //增加帮助

                // map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "附件权限"), true, true, 
                //    NodeAttr.FJOpen, "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                map.AddTBString(BtnAttr.TrackLab, "追跡", "追跡ボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "有効にするかどうか", true, true);
                //map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "轨迹"]); //增加帮助
                map.SetHelperUrl(BtnAttr.TrackLab, "http://ccbpm.mydoc.io/?v=5404&t=24369");

                map.AddTBString(BtnAttr.HungLab, "ハング", "ハングボタンのラベル", false, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "有効にするかどうか", false, false);
                map.SetHelperUrl(BtnAttr.HungLab, "http://ccbpm.mydoc.io/?v=5404&t=16267"); //增加帮助.

                //      map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                //      map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "工作方式",
                //true, true, BtnAttr.SelectAccepterEnable);
                //      map.SetHelperUrl(BtnAttr.SelectAccepterLab, "http://ccbpm.mydoc.io/?v=5404&t=16256"); //增加帮助


                map.AddTBString(BtnAttr.SearchLab, "問い合わせる", "問い合わせるボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "有効にするかどうか", true, true);
                //map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "查询"]); //增加帮助
                map.SetHelperUrl(BtnAttr.SearchLab, "http://ccbpm.mydoc.io/?v=5404&t=24373");

                //map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.BatchLab, "批处理", "批处理按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.BatchLab, "http://ccbpm.mydoc.io/?v=5404&t=17920"); //增加帮助

                //功能暂时取消
                //map.AddTBString(BtnAttr.AskforLab, "加签", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.AskforLab, "http://ccbpm.mydoc.io/?v=5404&t=16258");


                map.AddTBString(BtnAttr.HuiQianLab, "副署", "副署ラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "副署", true, true, BtnAttr.HuiQianRole, "@0=無効@1=コラボレーション（同僚）モード@4=グループリーダー（リーダーシップ）モード");

                //map.AddDDLSysEnum(BtnAttr.IsCanAddHuiQianer, 0, "协作模式被加签的人处理规则", true, true, BtnAttr.IsCanAddHuiQianer,
                //   "0=不允许增加其他协作人@1=允许增加协作人", false);

                map.AddDDLSysEnum(BtnAttr.HuiQianLeaderRole, 0, "グループリーダーの副署名規則", true, true, BtnAttr.HuiQianLeaderRole, "0=グループリーダーは1人だけです@1=最後のグループリーダーが送信します@2=すべてのグループリーダーが送信できます", true);

                map.AddTBString(BtnAttr.AddLeaderLab, "ホストを追加", "ホストを追加", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AddLeaderEnable, false, "有効にするかどうか", true, true);

                // add by 周朋 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "循環カスタム", "循環カスタム", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "有効にするかどうか", true, true);
                map.SetHelperUrl(BtnAttr.TCEnable, "http://ccbpm.mydoc.io/?v=5404&t=17978");

                //map.AddTBString(BtnAttr.AskforLabRe, "执行", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);

                // map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "加签"]); //增加帮助


                // 删除了这个模式,让表单方案进行控制了,保留这两个字段以兼容.
                map.AddTBString(BtnAttr.WebOfficeLab, "公式文書", "ドキュメントボタンのラベル", false, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.WebOfficeEnable, 0, "ドキュメントのアクティベーション方法", false, false);

                //cut bye zhoupeng.
                //map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                //  "@0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式");//edited by liuxc,2016-01-18,from xc
                //map.SetHelperUrl(BtnAttr.WebOfficeLab, "http://ccbpm.mydoc.io/?v=5404&t=17993");

                // add by 周朋 2015-08-06. 重要性.
                map.AddTBString(BtnAttr.PRILab, "重要性", "重要性", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PRIEnable, false, "有効にするかどうか", true, true);

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "ノードの時間制限", "ノードの時間制限", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CHRole, 0, "制限時間", true, true, BtnAttr.CHRole, @"0=無効@1=有効@2=読み取り専用@3=有効にし、フローが完了する必要がある時間を調整します");

                // add 2017.5.4  邀请其他人参与当前的工作.
                map.AddTBString(BtnAttr.AllotLab, "分布", "分布ボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AllotEnable, false, "有効にするかどうか", true, true);

                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "注意", "注意", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, false, "有効にするかどうか", true, true);

                // add 2017.5.4 确认就是告诉发送人，我接受这件工作了.
                map.AddTBString(BtnAttr.ConfirmLab, "確認", "確認ボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ConfirmEnable, false, "有効にするかどうか", true, true);

                // add 2019.3.10 增加List.
                map.AddTBString(BtnAttr.ListLab, "リスト", "リストボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ListEnable, true, "有効にするかどうか", true, true);

                // 批量审核
                map.AddTBString(BtnAttr.BatchLab, "バッチレビュー", "バッチレビューラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "有効にするかどうか", true, true);

                //备注 流程不流转，设置备注信息提醒已处理人员当前流程运行情况
                map.AddTBString(BtnAttr.NoteLab, "備考", "備考ラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NoteEnable, 0, "ルールを有効にする", true, true, BtnAttr.NoteEnable, @"0=無効@1=有効@2=読み取り専用");

                //for 周大福.
                map.AddTBString(BtnAttr.HelpLab, "助けて", "助けてラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HelpRole, 0, "ルールの表示に役立つ", true, true, BtnAttr.HelpRole, @"0=無効@1=有効@2=強制プロンプト@3=選択プロンプト");
                #endregion  功能按钮状态

                //节点工具栏,主从表映射.
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                #region 基础功能.
                RefMethod rm = null;

                rm = new RefMethod();
                rm.Title = "受信者のルール";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Sender.png";
                rm.ClassMethodName = this.ToString() + ".DoAccepterRoleNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "ノードイベント"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ノードメッセージ"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = "../../WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "フロー完了条件"; // "フロー完了条件";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Menu/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "送信後に回転"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoTurnToDeal";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Turnto.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "送信ブロックルール";
                rm.ClassMethodName = this.ToString() + ".DoBlockModel";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/BlockModel.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "マルチプレイヤールール";
                rm.ClassMethodName = this.ToString() + ".DoTodolistModel";
                rm.Icon = "../../WF/Img/Multiplayer.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                #endregion 基础功能.


                #region 字段相关功能（不显示在菜单里）
                rm = new RefMethod();
                rm.Title = "公式文書テンプレートをアップロード";
                rm.ClassMethodName = this.ToString() + ".DocTemp";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = BtnAttr.OfficeBtnEnable;
                rm.RefAttrLinkLabel = "公式ドキュメントテンプレートのメンテナンス";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "差戻可能なノード（指定したノードに差戻するように差戻ルールが設定されている場合、設定は有効です。）"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.ReturnRole;
                rm.RefAttrLinkLabel = "リターナブルノードを設定する";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "引戻可能なノード"; // "可撤销发送的节点";
                rm.ClassMethodName = this.ToString() + ".DoCanCancelNodes";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.RefAttrKey = NodeAttr.CancelRole; //在该节点下显示连接.
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "绑定打印格式模版(当打印方式为打印RTF格式模版时,该设置有效)";
                //rm.ClassMethodName = this.ToString() + ".DoBill";
                //rm.Icon = "../../WF/Img/FileType/doc.gif";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //rm = new RefMethod();
                //rm.Title = "打印设置"; // "可撤销发送的节点";
                ////设置相关字段.
                //rm.RefAttrKey = NodeAttr.PrintDocEnable;
                //rm.Target = "_blank";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //map.AddRefMethod(rm);
                //if (BP.Sys.SystemConfig.CustomerNo == "HCBD")
                //{
                //    /* 为海成邦达设置的个性化需求. */
                //    rm = new RefMethod();
                //    rm.Title = "DXReport设置";
                //    rm.ClassMethodName = this.ToString() + ".DXReport";
                //    rm.Icon = "../../WF/Img/FileType/doc.gif";
                //    map.AddRefMethod(rm);
                //}

                rm = new RefMethod();
                rm.Title = "自動カーボンコピールールを設定します（ノードが自動カーボンコピーの場合、この設定は有効です。）"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "自動CC設定";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 字段相关功能（不显示在菜单里）

                #region 表单设置.
                rm = new RefMethod();
                rm.Title = "フォームスキーム";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/Form.png";
                rm.ClassMethodName = this.ToString() + ".DoSheet";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フォーム設定";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "携帯電話フォームのフィールドオーダー";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フォーム設定";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ノードコンポーネント";
                rm.Icon = "../../WF/Img/Components.png";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmNodeComponent";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フォーム設定";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "特別な制御特別なユーザー権限";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/SpecUserSpecFields.png";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "フォーム設定";
                map.AddRefMethod(rm);
                #endregion 表单设置.

                #region 考核属性.

                map.AddTBInt(NodeAttr.TAlertRole, 0, "期限切れのリマインダールール", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.TAlertWay, 0, "期限切れのリマインダー", false, false); //"限期(天)
                map.AddTBInt(NodeAttr.WAlertRole, 0, "早期警告ルール", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.WAlertWay, 0, "早期警戒方法", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.TCent, 2, "ポイントの減額（1時間延長ごと）", false, false);
                map.AddTBInt(NodeAttr.CHWay, 0, "評価方法", false, false); //"限期(天)"

                //考核相关.
                map.AddTBInt(NodeAttr.IsEval, 0, "仕事の質の評価かどうか", false, false);
                map.AddTBInt(NodeAttr.OutTimeDeal, 0, "残業対応", false, false);

                #endregion 考核属性.

                #region 父子流程.
                rm = new RefMethod();
                rm.Title = "基本的なサブフロー設定";
                rm.Icon = "../../WF/Admin/AttrNode/Img/SubFlows.png";
                rm.ClassMethodName = this.ToString() + ".DoSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子フロー";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "サブフローを手動で開始する"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowHand";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子フロー";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "サブフローを自動的にトリガーする"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowAuto";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子フロー";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "継続サブフロー"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowYanXu";
                //  rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子フロー";
                map.AddRefMethod(rm);
                #endregion 父子流程.

                #region 考核.

                rm = new RefMethod();
                rm.Title = "評価ルールを設定する";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "評価ルール";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "タイムアウト処理ルール";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/OvertimeRole.png";
                rm.ClassMethodName = this.ToString() + ".DoCHOvertimeRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "評価ルール";
                map.AddRefMethod(rm);
                #endregion 考核.

                #region 实验中的功能
                rm = new RefMethod();
                rm.Title = "カスタム属性（一般）";
                rm.ClassMethodName = this.ToString() + ".DoSelfParas()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                rm.Visable = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "カスタム属性（カスタム）";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrExt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                rm.Visable = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ノードタイプを設定する";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAppType()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                //rm.Visable = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "ノード属性をバッチで設定する";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                rm.Visable = false;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置独立表单树权限";
                //rm.Icon = ../../Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "实验中的功能";
                //map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "作業バッチルール";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "実験の特徴";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "CCルール";
                rm.GroupName = "実験の特徴";
                rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoCCer";  //要执行的方法名.
                rm.RefMethodType = RefMethodType.RightFrameOpen; // 功能类型
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "リマインダーを設定";
                rm.GroupName = "実験の特徴";
                //     rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoHelpRole";  //要执行的方法名.
                rm.RefAttrKey = BtnAttr.HelpRole; //帮助信息.
                rm.RefMethodType = RefMethodType.LinkeWinOpen; // 功能类型
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "拡張列に戻る";
                rm.ClassMethodName = this.ToString() + ".DtlOfReturn";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = NodeAttr.ReturnCHEnable;
                map.AddRefMethod(rm);
              

                #endregion 实验中的功能

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 考核规则.
        /// <summary>
        /// 考核规则
        /// </summary>
        /// <returns></returns>
        public string DoCHRole()
        {
            return "../../Admin/AttrNode/CHRole.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 超时处理规则
        /// </summary>
        /// <returns></returns>
        public string DoCHOvertimeRole()
        {
            return "../../Admin/AttrNode/CHOvertimeRole.htm?FK_Node=" + this.NodeID;
        }
        #endregion 考核规则.

        #region 基础设置.
        /// <summary>
        /// 多人处理规则.
        /// </summary>
        /// <returns></returns>
        public string DoTodolistModel()
        {
            return "../../Admin/AttrNode/TodolistModel.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 批处理规则
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrNode/BatchStartFields.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return "../../Admin/AttrFlow/NodeAttrs.htm?NodeID=0&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 表单方案
        /// </summary>
        /// <returns></returns>
        public string DoSheet()
        {
            return "../../Admin/AttrNode/FrmSln/Default.htm?FK_Node=" + this.NodeID;
        }
        public string DoSheetOld()
        {
            return "../../Admin/AttrNode/NodeFromWorkModel.htm?FK_Node=" + this.NodeID;
        }


        /// <summary>
        /// 接受人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRoleNew()
        {
            return "../../Admin/AttrNode/AccepterRole/Default.htm?FK_Node=" + this.NodeID;
        }

        /// <summary>
        /// 发送阻塞规则
        /// </summary>
        /// <returns></returns>
        public string DoBlockModel()
        {
            return "../../Admin/AttrNode/BlockModel.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 发送后转向规则
        /// </summary>
        /// <returns></returns>
        public string DoTurnToDeal()
        {
            return "../../Admin/AttrNode/TurnToDeal.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 抄送人规则
        /// </summary>
        /// <returns></returns>
        public string DoCCer()
        {
            return "../../Admin/AttrNode/CCRole.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 加载提示信息
        /// </summary>
        /// <returns></returns>
        public string DoHelpRole()
        {
            return "../../Admin/FoolFormDesigner/HelpRole.htm?NodeID=" + this.NodeID;
        }
        #endregion

        #region 表单相关.
        /// <summary>
        /// 节点组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmNodeComponent()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 特别用户特殊字段权限.
        /// </summary>
        /// <returns></returns>
        public string DoSpecFieldsSpecUsers()
        {
            return "../../Admin/AttrNode/SepcFiledsSepcUsers.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node=" + this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return "../../Admin/AttrNode/SortingMapAttrs.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        #endregion 表单相关.

        #region 实验中的功能.
        /// <summary>
        /// 自定义参数（通用）
        /// </summary>
        /// <returns></returns>
        public string DoSelfParas()
        {
            return "../../Admin/AttrNode/SelfParas.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 自定义参数（自定义）
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrExt()
        {
            return "../../../DataUser/OverrideFiles/NodeAttrExt.htm?FK_Flow=" + this.NodeID;
        }

        /// <summary>
        /// 设置节点类型
        /// </summary>
        /// <returns></returns>
        public string DoNodeAppType()
        {
            return "../../Admin/AttrNode/NodeAppType.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        #endregion

        #region 子流程。
        /// <summary>
        /// 父子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlow()
        {
            return "../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmSubFlow&PK=" + this.NodeID;
        }
        /// <summary>
        /// 自动触发
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowAuto()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowAuto.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 手动启动子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowHand()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowHand.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 延续子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowYanXu()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowYanXu.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        #endregion 子流程。


        public string DoTurn()
        {
            return "../../Admin/AttrNode/TurnTo.htm?FK_Node=" + this.NodeID;
            //, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// 公文模板
        /// </summary>
        /// <returns></returns>
        public string DocTemp()
        {
            return "../../Admin/AttrNode/DocTemp.htm?PKVal=" + this.NodeID;
        } 
        /// <summary>
        /// 抄送规则
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.CC&PKVal=" + this.NodeID;
        }
        /// <summary>
        /// 个性化接受人窗口
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
        }
        /// <summary>
        /// 可触发的子流程
        /// </summary>
        /// <returns></returns>
        public string DoActiveFlows()
        {
            return "../../Admin/ConditionSubFlow.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 退回节点
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return "../../Admin/AttrNode/CanReturnNodes.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 撤销发送的节点
        /// </summary>
        /// <returns></returns>
        public string DoCanCancelNodes()
        {
            return "../../Admin/AttrNode/CanCancelNodes.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// DXReport
        /// </summary>
        /// <returns></returns>
        public string DXReport()
        {
            return "../../Admin/DXReport.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        public string DoCond()
        {
            return "../../Admin/Cond/List.htm?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// 设计自由表单
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return "../../Admin/Sln/BindFrms.htm?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }

        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // 类型.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
                    break;
                default:
                case NodeFormType.FoolForm:
                    return "../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID;
                    break;
            }
            return null;
        }

        /// <summary>
        /// 消息
        /// </summary>
        /// <returns></returns>
        public string DoMessage()
        {
            return "../../Admin/AttrNode/PushMessage.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrNode/Action.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/AttrNode/Bill.htm?FK_Node=" + this.NodeID + "&NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 保存提示信息
        /// </summary>
        /// <returns></returns>
        public string SaveHelpAlert(string text)
        {
            string file = SystemConfig.PathOfDataUser + Path.DirectorySeparatorChar + "CCForm" + Path.DirectorySeparatorChar + "HelpAlert" + Path.DirectorySeparatorChar + this.NodeID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            //若文件夹不存在，则创建
            if (System.IO.Directory.Exists(folder) == false)
                System.IO.Directory.CreateDirectory(folder);

            BP.DA.DataType.WriteFile(file, text);
            return "正常に保存！";
        }
        /// <summary>
        /// 读取提示信息
        /// </summary>
        /// <returns></returns>
        public string ReadHelpAlert()
        {
            string doc = "";
            string file = SystemConfig.PathOfDataUser + Path.DirectorySeparatorChar + "CCForm" + Path.DirectorySeparatorChar + "HelpAlert" + Path.DirectorySeparatorChar + this.NodeID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            if (System.IO.Directory.Exists(folder) != false)
            {
                if (File.Exists(file))
                {
                    doc = BP.DA.DataType.ReadTextFile(file);

                }
            }
            return doc;
        }
        public string DtlOfReturn()
        {
            string url = "../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapDtl=BP.WF.ReturnWorks"+ "&For=BP.WF.ReturnWorks&FK_Flow="+this.FK_Flow;
            return url;
        }
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.FK_Flow);

            #region 处理节点数据.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                //开始节点不能设置游离状态
                if (this.IsYouLiTai == true)
                    throw new Exception("現在のノードは開始ノードであり、解放することはできません");
                /*处理按钮的问题*/
                //不能退回, 加签，移交，退回, 子线程.
                //this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn); //开始节点可以退回。
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*如果是合流点*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            //如果启动了会签,并且是抢办模式,强制设置为队列模式.或者组长模式.
            if (this.HuiQianRole != WF.HuiQianRole.None)
            {
                if (this.HuiQianRole == WF.HuiQianRole.Teamup)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.Teamup + "  WHERE NodeID=" + this.NodeID);

                if (this.HuiQianRole == WF.HuiQianRole.TeamupGroupLeader)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.TeamupGroupLeader + ", TeamLeaderConfirmRole=" + (int)TeamLeaderConfirmRole.HuiQianLeader + " WHERE NodeID=" + this.NodeID);
            }

            // @杜. 翻译&测试.
            if (nd.CondModel == CondModel.ByLineCond)
            {
                /* 如果当前节点方向条件控制规则是按照连接线决定的, 
                 * 那就判断到达的节点的接受人规则，是否是按照上一步来选择，如果是就抛出异常.*/

                //获得到达的节点.
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        string errInfo = "矛盾を設定する:";
                        errInfo += "@現在のノードに設定したアクセスルールは、方向条件に従って制御されます";
                        errInfo += "しかし、ノードは到達しました[" + mynd.Name + "]受信者ルールは前のステップに従って選択され、矛盾を設定します。";
                        // throw new Exception(errInfo);
                    }
                }
            }

            //如果启用了在发送前打开, 当前节点的方向条件控制模式，是否是在下拉框边选择.?
            if (1 == 2 && nd.CondModel != CondModel.SendButtonSileSelect)
            {
                /*如果是启用了按钮，就检查当前节点到达的节点是否有【按照选择接受人】的方式确定接收人的范围. */
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        //强制设置安装人员选择器来选择.
                        this.SetValByKey(NodeAttr.CondModel, (int)CondModel.SendButtonSileSelect);
                        break;
                    }
                }
            }
            #endregion 处理节点数据.


            #region 创建审核组件附件
            if (this.FWCAth == FWCAth.MinAth)
            {
                FrmAttachment workCheckAth = new FrmAttachment();
                workCheckAth.MyPK = "ND" + this.NodeID + "_FrmWorkCheck";
                //不包含审核组件
                if (workCheckAth.RetrieveFromDBSources() == 0)
                {
                    workCheckAth = new FrmAttachment();
                    /*如果没有查询到它,就有可能是没有创建.*/
                    workCheckAth.MyPK = "ND" + this.NodeID + "_FrmWorkCheck";
                    workCheckAth.FK_MapData = "ND" + this.NodeID.ToString();
                    workCheckAth.NoOfObj = "FrmWorkCheck";
                    workCheckAth.Exts = "*.*";

                    //存储路径.
                    workCheckAth.SaveTo = "/DataUser/UploadFile/";
                    workCheckAth.IsNote = false; //不显示note字段.
                    workCheckAth.IsVisable = false; // 让其在form 上不可见.

                    //位置.
                    workCheckAth.X = (float)94.09;
                    workCheckAth.Y = (float)333.18;
                    workCheckAth.W = (float)626.36;
                    workCheckAth.H = (float)150;

                    //多附件.
                    workCheckAth.UploadType = AttachmentUploadType.Multi;
                    workCheckAth.Name = "監査コンポーネント";
                    workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=");
                    workCheckAth.Insert();
                }
            }
            #endregion 创建审核组件附件

            #region 审核组件.
            GroupField gf = new GroupField();
            if (this.HisFrmWorkCheckSta == FrmWorkCheckSta.Disable)
            {
                gf.Delete(GroupFieldAttr.FrmID, "ND" + this.NodeID, GroupFieldAttr.CtrlType, GroupCtrlType.FWC);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlType, GroupCtrlType.FWC, GroupFieldAttr.FrmID, "ND" + this.NodeID) == false)
                {
                    gf = new GroupField();
                    gf.EnName = "ND" + this.NodeID;
                    gf.CtrlType = GroupCtrlType.FWC;
                    gf.Lab = "情報を確認する";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 审核组件.

            BtnLab btnLab = new BtnLab(this.NodeID);
            btnLab.RetrieveFromDBSources();
            //清除所有的缓存.
            BP.DA.CashEntity.DCash.Clear();

            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            if (this.IsYouLiTai == true)
                fl.SetPara("IsYouLiTai", 1);
            else
                fl.SetPara("IsYouLiTai", 0);
            fl.Update();

            BtnLab btnLab = new BtnLab();
            btnLab.NodeID = this.NodeID;
            btnLab.RetrieveFromDBSources();
            Cash2019.UpdateRow(btnLab.ToString(), this.NodeID.ToString(), btnLab.Row);

            BtnLabExtWebOffice btnLabExtWebOffice = new BtnLabExtWebOffice();
            btnLabExtWebOffice.NodeID = this.NodeID;
            btnLabExtWebOffice.RetrieveFromDBSources();
            Cash2019.UpdateRow(btnLabExtWebOffice.ToString(), this.NodeID.ToString(), btnLabExtWebOffice.Row);

            CC cc = new CC();
            cc.NodeID = this.NodeID;
            cc.RetrieveFromDBSources();
            Cash2019.UpdateRow(cc.ToString(), this.NodeID.ToString(), cc.Row);

            FrmNodeComponent frmNodeComponent = new FrmNodeComponent();
            frmNodeComponent.NodeID = this.NodeID;
            frmNodeComponent.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmNodeComponent.ToString(), this.NodeID.ToString(), frmNodeComponent.Row);

            FrmThread frmThread = new FrmThread();
            frmThread.NodeID = this.NodeID;
            frmThread.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmThread.ToString(), this.NodeID.ToString(), frmThread.Row);

            FrmTrack frmTrack = new FrmTrack();
            frmTrack.NodeID = this.NodeID;
            frmTrack.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmTrack.ToString(), this.NodeID.ToString(), frmTrack.Row);

            FrmTransferCustom frmTransferCustom = new FrmTransferCustom();
            frmTransferCustom.NodeID = this.NodeID;
            frmTransferCustom.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmTransferCustom.ToString(), this.NodeID.ToString(), frmTransferCustom.Row);

            FrmWorkCheck frmWorkCheck = new FrmWorkCheck();
            frmWorkCheck.NodeID = this.NodeID;
            frmWorkCheck.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmWorkCheck.ToString(), this.NodeID.ToString(), frmWorkCheck.Row);

            NodeSheet nodeSheet = new NodeSheet();
            nodeSheet.NodeID = this.NodeID;
            nodeSheet.RetrieveFromDBSources();
            Cash2019.UpdateRow(nodeSheet.ToString(), this.NodeID.ToString(), nodeSheet.Row);

            NodeSimple nodeSimple = new NodeSimple();
            nodeSimple.NodeID = this.NodeID;
            nodeSimple.RetrieveFromDBSources();
            Cash2019.UpdateRow(nodeSimple.ToString(), this.NodeID.ToString(), nodeSimple.Row);

            FrmSubFlow frmSubFlow = new FrmSubFlow();
            frmSubFlow.NodeID = this.NodeID;
            frmSubFlow.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmSubFlow.ToString(), this.NodeID.ToString(), frmSubFlow.Row);

            GetTask getTask = new GetTask();
            getTask.NodeID = this.NodeID;
            getTask.RetrieveFromDBSources();
            Cash2019.UpdateRow(getTask.ToString(), this.NodeID.ToString(), getTask.Row);

            //如果是组长会签模式，通用选择器只能单项选择
            if (this.HuiQianRole == HuiQianRole.TeamupGroupLeader && this.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne)
            {
                Selector selector = new Selector();
                selector.NodeID = this.NodeID;
                selector.RetrieveFromDBSources();
                selector.IsSimpleSelector = true;
                selector.Update();

            }

            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class NodeExts : Entities
    {
        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public NodeExts()
        {
        }

        public NodeExts(string fk_flow)
        {
            this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
            return;
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeExt(); }
        }
    }
}
