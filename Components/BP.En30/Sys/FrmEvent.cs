using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Web;

namespace BP.Sys
{
    /// <summary>
    /// 消息控制方式
    /// </summary>
    public enum MsgCtrl
    {
        /// <summary>
        /// bufasong 
        /// </summary>
        None,
        /// <summary>
        /// 按照设置计算
        /// </summary>
        BySet,
        /// <summary>
        /// 按照表单的是否发送字段计算，字段:IsSendMsg
        /// </summary>
        ByFrmIsSendMsg,
        /// <summary>
        /// 按照SDK参数计算.
        /// </summary>
        BySDK
    }
    public enum EventDoType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 执行存储过程
        /// </summary>
        SP = 1,
        /// <summary>
        /// 运行SQL
        /// </summary>
        SQL = 2,
        /// <summary>
        /// 自定义URL
        /// </summary>
        URLOfSelf = 3,
        /// <summary>
        /// 自定义WS
        /// </summary>
        WSOfSelf = 4,
        /// <summary>
        /// 执行ddl文件的类与方法
        /// </summary>
        SpecClass = 5,
        /// <summary>
        /// 基类
        /// </summary>
        EventBase = 6,
        /// <summary>
        /// 执行的业务单元
        /// </summary>
        BuessUnit = 7
    }
    public class FrmEventList
    {
        /// <summary>
        /// 表单载入前
        /// </summary>
        public const string FrmLoadBefore = "FrmLoadBefore";
        /// <summary>
        /// 表单载入后
        /// </summary>
        public const string FrmLoadAfter = "FrmLoadAfter";
        /// <summary>
        /// 表单保存前
        /// </summary>
        public const string SaveBefore = "SaveBefore";
        /// <summary>
        /// 表单保存后
        /// </summary>
        public const string SaveAfter = "SaveAfter";
        /// <summary>
        /// 创建OID
        /// </summary>
        public const string CreateOID = "CreateOID";
        /// <summary>
        /// 附件上传前
        /// </summary>
        public const string AthUploadeBefore = "AthUploadeBefore";
        /// <summary>
        /// 上传后.
        /// </summary>
        public const string AthUploadeAfter = "AthUploadeAfter";
        /// <summary>
        /// 从表保存前
        /// </summary>
        public const string DtlRowSaveBefore = "DtlRowSaveBefore";
        /// <summary>
        /// 从表保存后
        /// </summary>
        public const string DtlRowSaveAfter = "DtlRowSaveAfter";
    }
    public class FrmEventListDtl
    {
        /// <summary>
        /// 从表保存前
        /// </summary>
        public const string RowSaveBefore = "DtlRowSaveBefore";
        /// <summary>
        /// 从表保存后
        /// </summary>
        public const string RowSaveAfter = "DtlRowSaveAfter";

        /// <summary>
        /// 从表保存前
        /// </summary>
        public const string DtlRowDelBefore = "DtlRowDelBefore";
        /// <summary>
        /// 从表保存后
        /// </summary>
        public const string DtlRowDelAfter = "DtlRowDelAfter";

    }
    /// <summary>
    /// 事件标记列表
    /// </summary>
    public class EventListOfNode : FrmEventList
    {
        #region 节点事件
        /// <summary>
        /// 创建工作ID
        /// </summary>
        public const string CreateWorkID = "CreateWorkID";
        /// <summary>
        /// 节点发送前
        /// </summary>
        public const string SendWhen = "SendWhen";
        /// <summary>
        /// 工作到达
        /// </summary>
        public const string WorkArrive = "WorkArrive";
        /// <summary>
        /// 节点发送成功后
        /// </summary>
        public const string SendSuccess = "SendSuccess";
        /// <summary>
        /// 节点发送失败后
        /// </summary>
        public const string SendError = "SendError";
        /// <summary>
        /// 当节点退回前
        /// </summary>
        public const string ReturnBefore = "ReturnBefore";
        /// <summary>
        /// 当节点退后
        /// </summary>
        public const string ReturnAfter = "ReturnAfter";
        /// <summary>
        /// 当节点撤销发送前
        /// </summary>
        public const string UndoneBefore = "UndoneBefore";
        /// <summary>
        /// 当节点撤销发送后
        /// </summary>
        public const string UndoneAfter = "UndoneAfter";
        /// <summary>
        /// 当前节点移交后
        /// </summary>
        public const string ShitAfter = "ShitAfter";
        /// <summary>
        /// 当节点加签后
        /// </summary>
        public const string AskerAfter = "AskerAfter";
        /// <summary>
        /// 当节点加签答复后
        /// </summary>
        public const string AskerReAfter = "AskerReAfter";
        /// <summary>
        /// 队列节点发送后
        /// </summary>
        public const string QueueSendAfter = "QueueSendAfter";
        /// <summary>
        /// 节点打开后.
        /// </summary>
        public const string WhenReadWork = "WhenReadWork";
        /// <summary>
        /// 节点预警
        /// </summary>
        public const string NodeWarning = "NodeWarning";
        /// <summary>
        /// 节点逾期
        /// </summary>
        public const string NodeOverDue = "NodeOverDue";
        /// <summary>
        /// 流程预警
        /// </summary>
        public const string FlowWarning = "FlowWarning";
        /// <summary>
        /// 流程逾期
        /// </summary>
        public const string FlowOverDue = "FlowOverDue";


        #endregion 节点事件

        #region 流程事件
        /// <summary>
        /// 当创建workid的时候.
        /// </summary>
        public const string FlowOnCreateWorkID = "FlowOnCreateWorkID";
        /// <summary>
        /// 流程完成时.
        /// </summary>
        public const string FlowOverBefore = "FlowOverBefore";
        /// <summary>
        /// 结束后.
        /// </summary>
        public const string FlowOverAfter = "FlowOverAfter";
        /// <summary>
        /// 流程删除前
        /// </summary>
        public const string BeforeFlowDel = "BeforeFlowDel";
        /// <summary>
        /// 流程删除后
        /// </summary>
        public const string AfterFlowDel = "AfterFlowDel";
        #endregion 流程事件
    }
    /// <summary>
    /// 事件属性
    /// </summary>
    public class FrmEventAttr
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public const string FK_Event = "FK_Event";
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 执行类型
        /// </summary>
        public const string EventDoType = "EventDoType";
        /// <summary>
        /// 执行内容
        /// </summary>
        public const string DoDoc = "DoDoc";
        /// <summary>
        /// 标签
        /// </summary>
        public const string MsgOK = "MsgOK";
        /// <summary>
        /// 执行错误提示
        /// </summary>
        public const string MsgError = "MsgError";

        #region 消息设置.
        /// <summary>
        /// 控制方式
        /// </summary>
        public const string MsgCtrl = "MsgCtrl";
        /// <summary>
        /// 邮件是否启用
        /// </summary>
        public const string MailEnable = "MailEnable";
        /// <summary>
        /// 消息标题
        /// </summary>
        public const string MailTitle = "MailTitle";
        /// <summary>
        /// 消息内容模版
        /// </summary>
        public const string MailDoc = "MailDoc";
        /// <summary>
        /// 是否启用短信
        /// </summary>
        public const string SMSEnable = "SMSEnable";
        /// <summary>
        /// 短信内容模版
        /// </summary>
        public const string SMSDoc = "SMSDoc";
        /// <summary>
        /// 是否推送？
        /// </summary>
        public const string MobilePushEnable = "MobilePushEnable";
        /// <summary>
        /// DLL路径
        /// </summary>
        public const string MonthedDLL = "MonthedDLL";
        /// <summary>
        /// DLL中所选的类名
        /// </summary>
        public const string MonthedClass = "MonthedClass";
        /// <summary>
        /// DLL中所选类中的方法字符串
        /// </summary>
        public const string MonthedName = "MonthedName";
        /// <summary>
        /// DLL中所选类所选方法的参数表达式
        /// </summary>
        public const string MonthedParas = "MonthedParas";
        #endregion 消息设置.
    }
    /// <summary>
    /// 事件
    /// 节点的节点保存事件有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FrmEvent : EntityMyPK
    {
        #region 参数属性.
        /// <summary>
        /// 名称
        /// </summary>
        public string MonthedDLL
        {
            get
            {
                return this.GetParaString(FrmEventAttr.MonthedDLL);

            }
            set
            {
                this.SetPara(FrmEventAttr.MonthedDLL, value);
            }
        }
        /// <summary>
        /// 类名
        /// </summary>
        public string MonthedClass
        {
            get
            {
                return this.GetParaString(FrmEventAttr.MonthedClass);

            }
            set
            {
                this.SetPara(FrmEventAttr.MonthedClass, value);
            }
        }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MonthedName
        {
            get
            {
                return this.GetParaString(FrmEventAttr.MonthedName);

            }
            set
            {
                this.SetPara(FrmEventAttr.MonthedName, value);
            }
        }
        /// <summary>
        /// 方法参数.
        /// </summary>
        public string MonthedParas
        {
            get
            {
                return this.GetParaString(FrmEventAttr.MonthedParas);

            }
            set
            {
                this.SetPara(FrmEventAttr.MonthedParas, value);
            }
        }
        #endregion 参数属性.

        #region 基本属性
        public override En.UAC HisUAC
        {
            get
            {
                UAC uac = new En.UAC();
                uac.IsAdjunct = false;
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmEventAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_MapData, value);
            }
        }
        public string DoDoc
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.DoDoc).Replace("~", "'");
            }
            set
            {
                string doc = value.Replace("'", "~");
                this.SetValByKey(FrmEventAttr.DoDoc, doc);
            }
        }
        /// <summary>
        /// 执行成功提示
        /// </summary>
        public string MsgOK(Entity en)
        {
            string val = this.GetValStringByKey(FrmEventAttr.MsgOK);
            if (val.Trim() == "")
                return "";

            if (val.IndexOf('@') == -1)
                return val;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                val = val.Replace("@" + attr.Key, en.GetValStringByKey(attr.Key));
            }
            return val;
        }
        public string MsgOKString
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.MsgOK);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgOK, value);
            }
        }
        public string MsgErrorString
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.MsgError);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgError, value);
            }
        }
        /// <summary>
        /// 错误或异常提示
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public string MsgError(Entity en)
        {
            string val = this.GetValStringByKey(FrmEventAttr.MsgError);
            if (val.Trim() == "")
                return null;

            if (val.IndexOf('@') == -1)
                return val;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                val = val.Replace("@" + attr.Key, en.GetValStringByKey(attr.Key));
            }
            return val;
        }

        public string FK_Event
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.FK_Event);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_Event, value);
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public EventDoType HisDoType
        {
            get
            {
                return (EventDoType)this.GetValIntByKey(FrmEventAttr.EventDoType);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.EventDoType, (int)value);
            }
        }
        public int HisDoTypeInt
        {
            get
            {
                return this.GetValIntByKey(FrmEventAttr.EventDoType);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.EventDoType, value);
            }
        }
        #endregion

        #region 事件消息.
        /// <summary>
        /// 消息控制类型.
        /// </summary>
        public MsgCtrl MsgCtrl
        {
            get
            {
                return (MsgCtrl)this.GetValIntByKey(FrmEventAttr.MsgCtrl);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgCtrl, (int)value);
            }
        }
        /// <summary>
        /// 是否手机推送？
        /// </summary>
        public bool MobilePushEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEventAttr.MobilePushEnable);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MobilePushEnable, value);
            }
        }
        public bool MailEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEventAttr.MailEnable);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MailEnable, value);
            }
        }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string MailTitle
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.MailTitle);
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;
                switch (this.FK_Event)
                {
                    case EventListOfNode.SendSuccess:
                        return "新しい仕事@Title、送信者@WebUser.No,@WebUser.Name";
                    case EventListOfNode.ShitAfter:
                        return "@Title、@WebUser.No、@WebUser.Nameを引き継いだ新しい仕事";
                    case EventListOfNode.ReturnAfter:
                        return "@Titleに戻り、@WebUser.No、@WebUser.Nameに戻ります";
                    case EventListOfNode.UndoneAfter:
                        return "ジョブがキャンセルされました@Title、送信者@WebUser.No、@WebUser.Name";
                    case EventListOfNode.AskerReAfter:
                        return "新しいジョブ@Title、送信者@WebUser.No、@WebUser.Nameを追加します";
                    case EventListOfNode.AfterFlowDel:
                        return "ワークフローが削除されました@タイトル、送信者@WebUser.No、@WebUser.Name";
                    case EventListOfNode.FlowOverAfter:
                        return "フローの終了@Title、送信者@WebUser.No、@WebUser.Name";
                    default:
                        throw new Exception("@イベントタイプはデフォルトのメッセージテンプレートを定義していません:" + this.FK_Event);
                        break;
                }
                return str;
            }
        }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string MailTitle_Real
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.MailTitle);
                return str;
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MailTitle, value);
            }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailDoc_Real
        {
            get
            {
                return this.GetValStrByKey(FrmEventAttr.MailDoc);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MailDoc, value);
            }
        }
        /// <summary>
        /// 邮件内容模版
        /// </summary>
        public string MailDoc
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.MailDoc);
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;
                switch (this.FK_Event)
                {
                    case EventListOfNode.SendSuccess:
                        str += "\t\nこんにちは:";
                        str += "\t\n    @Titleが処理する必要のある新しいジョブがあります。ここをクリックしてジョブ{Url}を開いてください。";
                        str += "\t\n以上です。";
                        str += "\t\n    @WebUser.No, @WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.ReturnAfter:
                        str += "\t\nこんにちは:";
                        str += "\t\n    ジョブ@Titleが返されました。ここをクリックしてジョブを開いてください{Url}。";
                        str += "\t\n 以上です。";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.ShitAfter:
                        str += "\t\nこんにちは:";
                        str += "\t\n    @Titleに引き渡されたジョブ、ここをクリックしてジョブを開く{Url}。";
                        str += "\t\n 以上です。";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.UndoneAfter:
                        str += "\t\nこんにちは:";
                        str += "\t\n    @Titleに引き渡されたジョブ、ここをクリックしてジョブを開く{Url}。";
                        str += "\t\n 以上です。";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.AskerReAfter: //加签.
                        str += "\t\nこんにちは:";
                        str += "\t\n    @Titleに引き渡されたジョブ、ここをクリックしてジョブを開く{Url}。";
                        str += "\t\n 以上です。";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.AfterFlowDel: //流程删除
                        str += "\t\nこんにちは:";
                        str += "\t\n    削除されたジョブ@Title.";
                        str += "\t\n 以上です。";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    case EventListOfNode.FlowOverAfter: //流程结束
                        str += "\t\nこんにちは:";
                        str += "\t\n    ジョブ@Titleが終了しました。ここをクリックしてジョブを表示してください{Url}。";
                        str += "\t\n 以上です。";
                        str += "\t\n    @WebUser.No,@WebUser.Name";
                        str += "\t\n    @RDT";
                        break;
                    default:
                        throw new Exception("@イベントタイプはデフォルトのメッセージテンプレートを定義していません:" + this.FK_Event);
                        break;
                }
                return str;
            }
        }
        /// <summary>
        /// 是否启用短信发送
        /// </summary>
        public bool SMSEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEventAttr.SMSEnable);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.SMSEnable, value);
            }
        }
        /// <summary>
        /// 短信模版内容
        /// </summary>
        public string SMSDoc_Real
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.SMSDoc);
                return str;
            }
            set
            {
                this.SetValByKey(FrmEventAttr.SMSDoc, value);
            }
        }
        /// <summary>
        /// 短信模版内容
        /// </summary>
        public string SMSDoc
        {
            get
            {
                string str = this.GetValStrByKey(FrmEventAttr.SMSDoc);
                if (DataType.IsNullOrEmpty(str) == false)
                    return str;

                switch (this.FK_Event)
                {
                    case EventListOfNode.SendSuccess:
                        str = "処理する必要がある新しいジョブ@Title、送信者があります：@WebUser.No、@WebUser.Name、{Url}を開きます。";
                        break;
                    case EventListOfNode.ReturnAfter:
                        str = "ジョブ@Titleが返されました：@WebUser.No、@WebUser.Name、{Url}を開く。";
                        break;
                    case EventListOfNode.ShitAfter:
                        str = "仕事を転送@タイトル、人を転送：@WebUser.No、@WebUser.Name、{Url}を開く。";
                        break;
                    case EventListOfNode.UndoneAfter:
                        str = "ジョブのキャンセル@タイトル、キャンセル担当者：@WebUser.No、@WebUser.Name、{Url}を開く。";
                        break;
                    case EventListOfNode.AskerReAfter: //加签.
                        str = "仕事の推奨@Title、支持者：@WebUser.No、@WebUser.Name、{Url}を開く。";
                        break;
                    default:
                        throw new Exception("@イベントタイプはデフォルトのメッセージテンプレートを定義していません:" + this.FK_Event);
                        break;
                }
                return str;
            }
            set
            {
                this.SetValByKey(FrmEventAttr.SMSDoc, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvent()
        {
        }
        public FrmEvent(string mypk)
        {
            this.MyPK = mypk;
            this.RetrieveFromDBSources();
        }
        public FrmEvent(string fk_mapdata, string fk_Event)
        {
            this.FK_Event = fk_Event;
            this.FK_MapData = fk_mapdata;
            this.MyPK = this.FK_MapData + "_" + this.FK_Event;
            this.RetrieveFromDBSources();
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

                Map map = new Map("Sys_FrmEvent", "イベント");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.IndexField = FrmEventAttr.FK_MapData; 


                map.AddMyPK();

                map.AddTBString(FrmEventAttr.FK_Event, null, "イベント名", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.FK_MapData, null, "フォームID", true, true, 0, 100, 10);
                map.AddTBString(FrmEventAttr.FK_Flow, null, "フロー番号", true, true, 0, 100, 10);
                map.AddTBInt(FrmEventAttr.FK_Node, 0, "ノードID", true, true);

                map.AddTBInt(FrmEventAttr.EventDoType, 0, "イベントタイプ", true, true);
              //  map.AddTBInt(FrmEventAttr.DoType, 0, "事件类型", true, true);

                map.AddTBString(FrmEventAttr.DoDoc, null, "実行内容", true, true, 0, 400, 10);

                map.AddTBString(FrmEventAttr.MsgOK, null, "実行成功の提示", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.MsgError, null, "例外情報プロンプト", true, true, 0, 400, 10);

                #region 消息设置. 如下属性放入了节点参数信息了.
                map.AddDDLSysEnum(FrmEventAttr.MsgCtrl, 0, "メッセージ送信制御", true, true, FrmEventAttr.MsgCtrl,
                    "@0 =送信しない@1 =受信者が設定した次のステップに従って自動的に送信する（デフォルト）@2 =ノードフォームのシステムフィールド（IsSendEmail、IsSendSMS）によって決定される@ 3 = SDK開発者パラメーター（IsSendEmail、IsSendSMS）によって決定される", true);


                map.AddBoolean(FrmEventAttr.MailEnable, true, "メール送信を有効しますか？（有効にする場合は、メールテンプレートを設定し、ccflow式をサポートする必要があります。）", true, true, true);
                map.AddTBString(FrmEventAttr.MailTitle, null, "メールヘッダーテンプレート", true, false, 0, 200, 20, true);
                map.AddTBStringDoc(FrmEventAttr.MailDoc, null, "メールコンテンツテンプレート", true, false, true);

                //是否启用手机短信？
                map.AddBoolean(FrmEventAttr.SMSEnable, false, "SMS送信は有効ですか？ （これを有効にする場合は、ccflow式をサポートするSMSテンプレートを設定する必要があります。）", true, true, true);
                map.AddTBStringDoc(FrmEventAttr.SMSDoc, null, "SMSコンテンツテンプレート", true, false, true);
                map.AddBoolean(FrmEventAttr.MobilePushEnable, true, "携帯電話、パッドにプッシュするかどうか。", true, true, true);
                #endregion 消息设置.

                //参数属性
                map.AddTBAtParas(4000);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            //this.MyPK = this.FK_MapData + "_" + this.FK_Event;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 事件
    /// </summary>
    public class FrmEvents : EntitiesOID
    {
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <returns>null 没有事件，其他为执行了事件。</returns>
        public string DoEventNode(string dotype, Entity en)
        {
           // return null; // 2019-08-27 取消节点事件 zl 
            return DoEventNode(dotype, en, null);
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <param name="atPara">参数</param>
        /// <returns>null 没有事件，其他为执行了事件。</returns>
        /// <remarks>
        /// 不再使用节点事件 2019-08-27 zl
        /// 原调用点有两处：（1）FrmEvent.cs 中的DoEventNode()； （2）Flow.cs中的DoFlowEventEntity()方法中，3973行 fes.DoEventNode(doType, en, atPara)。
        /// 现在都已经取消调用
        /// </remarks>
        public string DoEventNode(string dotype, Entity en, string atPara)
        {
            if (this.Count == 0)
                return null;
            string val = _DoEventNode(dotype, en, atPara);
            if (val != null)
                val = val.Trim();

            if (DataType.IsNullOrEmpty(val))
                return ""; // 说明有事件，执行成功了。
            else
                return val; // 没有事件. 
        }

        /// <summary>
        /// 执行事件，事件标记是 EventList.
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <param name="atPara">特殊的参数格式@key=value 方式.</param>
        /// <returns></returns>
        private string _DoEventNode(string dotype, Entity en, string atPara)
        {
            if (this.Count == 0)
                return null;

            FrmEvent nev = this.GetEntityByKey(FrmEventAttr.FK_Event, dotype) as FrmEvent;

            if (nev == null || nev.HisDoType == EventDoType.Disable)
                return null;

            #region 执行的是业务单元.
            if (nev.HisDoType == EventDoType.BuessUnit)
            {
                /* 获得业务单元，开始执行他 */
                BuessUnitBase enBuesss = BP.Sys.Glo.GetBuessUnitEntityByEnName(nev.DoDoc) ;
                enBuesss.WorkID = Int64.Parse(en.PKVal.ToString());
                return enBuesss.DoIt();
            }
            #endregion 执行的是业务单元.


            string doc = nev.DoDoc.Trim();
            if ((doc == null || doc == "") && nev.HisDoType != EventDoType.SpecClass)   //edited by liuxc,2016-01-16,执行DLL文件不需要判断doc为空
                return null;

            #region 处理执行内容
            Attrs attrs = en.EnMap.Attrs;
            string MsgOK = "";
            string MsgErr = "";
            foreach (Attr attr in attrs)
            {
                if (doc.Contains("@" + attr.Key) == false)
                    continue ;
                if (attr.MyDataType == DataType.AppString
                    || attr.MyDataType == DataType.AppDateTime
                    || attr.MyDataType == DataType.AppDate)
                    doc = doc.Replace("@" + attr.Key, "'" + en.GetValStrByKey(attr.Key) + "'");
                else
                    doc = doc.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
            }

            doc = doc.Replace("~", "'");
            doc = doc.Replace("@WebUser.No", BP.Web.WebUser.No);
            doc = doc.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            doc = doc.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            doc = doc.Replace("@FK_Node", nev.FK_MapData.Replace("ND", ""));
            doc = doc.Replace("@FK_MapData", nev.FK_MapData);
            doc = doc.Replace("@WorkID", en.GetValStrByKey("OID", "@WorkID"));

            //SDK表单上服务器地址,应用到使用ccflow的时候使用的是sdk表单,该表单会存储在其他的服务器上. 
            doc = doc.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);

            if (doc.Contains("@") == true)
            {
                if (HttpContextHelper.Current != null)
                {
                    /*如果是 bs 系统, 有可能参数来自于url ,就用url的参数替换它们 .*/
                    //string url = BP.Sys.Glo.Request.RawUrl;
                    //2019-07-25 zyt改造
                    string url = HttpContextHelper.RequestRawUrl ;
                    if (url.IndexOf('?') != -1)
                        url = url.Substring(url.IndexOf('?')).TrimStart('?');

                    string[] paras = url.Split('&');
                    foreach (string s in paras)
                    {
                        string[] mys = s.Split('=');

                        if (doc.Contains("@" + mys[0]) == false)
                            continue;

                        doc = doc.Replace("@" + mys[0], mys[1]);
                    }
                }
            }

            if (nev.HisDoType == EventDoType.URLOfSelf)
            {
                if (doc.Contains("?") == false)
                    doc += "?1=2";

                doc += "&UserNo=" + WebUser.No;
                doc += "&SID=" + WebUser.SID;
                doc += "&FK_Dept=" + WebUser.FK_Dept;
                // doc += "&FK_Unit=" + WebUser.FK_Unit;
                doc += "&OID=" + en.PKVal;

                if (SystemConfig.IsBSsystem)
                {
                    /*是bs系统，并且是url参数执行类型.*/
                    //string url = BP.Sys.Glo.Request.RawUrl;
                    //2019-07-25 zyt改造
                    string url = HttpContextHelper.RequestRawUrl;
                    if (url.IndexOf('?') != -1)
                        url = url.Substring(url.IndexOf('?')).TrimStart('?');

                    string[] paras = url.Split('&');
                    foreach (string s in paras)
                    {
                        string[] mys = s.Split('=');

                        if (doc.Contains(mys[0] + "="))
                            continue;

                        doc += "&" + s;
                    }

                    doc = doc.Replace("&?", "&");
                }

                if (SystemConfig.IsBSsystem == false)
                {
                    /*非bs模式下调用,比如在cs模式下调用它,它就取不到参数. */
                }

                if (doc.StartsWith("http") == false)
                {
                    /*如果没有绝对路径 */
                    if (SystemConfig.IsBSsystem)
                    {
                        /*在cs模式下自动获取*/
                        //string host = BP.Sys.Glo.Request.Url.Host;
                        //2019-07-25 zyt改造
                        string host = HttpContextHelper.RequestUrlHost;
                        if (doc.Contains("@AppPath"))
                            doc = doc.Replace("@AppPath", "http://" + host + HttpContextHelper.RequestApplicationPath);
                        else
                            doc = "http://" + HttpContextHelper.RequestUrlAuthority + doc;
                    }

                    if (SystemConfig.IsBSsystem == false)
                    {
                        /*在cs模式下它的baseurl 从web.config中获取.*/
                        string cfgBaseUrl = SystemConfig.HostURL;
                        if (DataType.IsNullOrEmpty(cfgBaseUrl))
                        {
                            string err = "URLの呼び出しに失敗しました：BaseUrlがweb.configで構成されていないため、実行できないURLイベントが発生します。";
                            Log.DefaultLogWriteLineError(err);
                            throw new Exception(err);
                        }
                        doc = cfgBaseUrl + doc;
                    }
                }

                //增加上系统约定的参数.
                doc += "&EntityName=" + en.ToString() + "&EntityPK=" + en.PK + "&EntityPKVal=" + en.PKVal + "&FK_Event=" + nev.MyPK;
            }
            #endregion 处理执行内容

            if (atPara != null && doc.Contains("@") == true)
            {
                AtPara ap = new AtPara(atPara);
                foreach (string s in ap.HisHT.Keys)
                    doc = doc.Replace("@" + s, ap.GetValStrByKey(s));
            }

            if (dotype == FrmEventList.FrmLoadBefore)
                en.Retrieve(); /*如果不执行，就会造成实体的数据与查询的数据不一致.*/

            switch (nev.HisDoType)
            {
                case EventDoType.SP:
                case EventDoType.SQL:
                    try
                    {
                        // 允许执行带有GO的sql.
                        DBAccess.RunSQLs(doc);
                        return nev.MsgOK(en);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;

                case EventDoType.URLOfSelf:
                    string myURL = doc.Clone() as string;
                    if (myURL.Contains("http") == false)
                    {
                        if (SystemConfig.IsBSsystem)
                        {
                            //string host = BP.Sys.Glo.Request.Url.Host;
                            //2019-07-25 zyt改造
                            string host = HttpContextHelper.RequestUrlHost;
                            if (myURL.Contains("@AppPath"))
                                myURL = myURL.Replace("@AppPath", "http://" + host + HttpContextHelper.RequestApplicationPath);
                            else
                                myURL = "http://" + HttpContextHelper.RequestUrlAuthority + myURL;
                        }
                        else
                        {
                            string cfgBaseUrl =  SystemConfig.HostURL;
                            if (DataType.IsNullOrEmpty(cfgBaseUrl))
                            {
                                string err = "URLの呼び出しに失敗しました：BaseUrlがweb.configで構成されていないため、実行できないURLイベントが発生します。";
                                Log.DefaultLogWriteLineError(err);
                                throw new Exception(err);
                            }
                            myURL = cfgBaseUrl + myURL;
                        }
                    }
                    myURL = myURL.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);

                    if (myURL.Contains("&FID=") == false && en.Row.ContainsKey("FID") == true)
                    {
                        string str = en.Row["FID"].ToString();
                        myURL = myURL + "&FID=" + str;
                    }

                    if (myURL.Contains("&FK_Flow=") == false && en.Row.ContainsKey("FK_Flow") == true)
                    {
                        string str = en.Row["FK_Flow"].ToString();
                        myURL = myURL + "&FK_Flow=" + str;
                    }

                    if (myURL.Contains("&WorkID=") == false && en.Row.ContainsKey("WorkID") == true)
                    {
                        string str = en.Row["WorkID"].ToString();
                        myURL = myURL + "&WorkID=" + str;
                    }

                    try
                    {
                        Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                        string text = DataType.ReadURLContext(myURL, 600000, encode);
                        if (text == null)
                            throw new Exception("@フロー設計エラー、間違ったURLが実行されました:" + myURL + ", 戻り値がnullです。URL設定が正しいかどうか確認してください。ヒント：このURLをコピーしてブラウザーに配置し、正しく実行されるかどうかを確認できます。");

                        if (text != null
                            && text.Length > 7
                            && text.Substring(0, 7).ToLower().Contains("err"))
                            throw new Exception(text);

                        if (text == null || text.Trim() == "")
                            return null;
                        return text;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@" + nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                case EventDoType.EventBase: //执行事件类.
                    // 获取事件类.
                    string evName = doc.Clone() as string;
                    BP.Sys.EventBase ev = null;
                    try
                    {
                        ev = BP.En.ClassFactory.GetEventBase(evName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@イベント名:" + evName + "スペルが間違っているか、システムが存在しません。注：イベントが配置されているクラスライブラリは、BP。で始まり、クラスライブラリのBP.xxx.dllである必要があります。");
                    }

                    //开始执行.
                    try
                    {
                        #region 处理整理参数.
                        Row r = en.Row;
                        try
                        {
                            //系统参数.
                            r.Add("FK_MapData", en.ClassID);
                        }
                        catch
                        {
                            r["FK_MapData"] = en.ClassID;
                        }

                        try
                        {
                            r.Add("EventType", nev.FK_Event);
                        }
                        catch
                        {
                            r["EventType"] = nev.FK_Event;
                        }

                        if (atPara != null)
                        {
                            AtPara ap = new AtPara(atPara);
                            foreach (string s in ap.HisHT.Keys)
                            {
                                try
                                {
                                    r.Add(s, ap.GetValStrByKey(s));
                                }
                                catch
                                {
                                    r[s] = ap.GetValStrByKey(s);
                                }
                            }
                        }

                        if (SystemConfig.IsBSsystem == true)
                        {
                            /*如果是bs系统, 就加入外部url的变量.*/
                            //2019 - 07 - 25 zyt改造
                            foreach (string key in HttpContextHelper.RequestParamKeys)
                            {
                                string val = HttpContextHelper.RequestParams(key);
                                try
                                {
                                    r.Add(key, val);
                                }
                                catch
                                {
                                    r[key] = val;
                                }
                            }
                        }
                        #endregion 处理整理参数.

                        ev.SysPara = r;
                        ev.HisEn = en;
                        ev.Do();
                        return ev.SucessInfo;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@実行イベント(" + ev.Title + ")中にエラーが発生しました:" + ex.Message);
                    }
                    break;
                case EventDoType.WSOfSelf: //执行webservices.. 为石油修改.
                    string[] strs = doc.Split('@');
                    string url = "";
                    string method = "";
                    Hashtable paras = new Hashtable();
                    foreach (string str in strs)
                    {
                        if (str.Contains("=") && str.Contains("Url"))
                        {
                            url = str.Split('=')[2];
                            continue;
                        }

                        if (str.Contains("=") && str.Contains("Method"))
                        {
                            method = str.Split('=')[2];
                            continue;
                        }

                        //处理参数.
                        string[] paraKeys = str.Split(',');

                        if (paraKeys[3] == "Int")
                            paras.Add(paraKeys[0], int.Parse(paraKeys[1]));

                        if (paraKeys[3] == "String")
                            paras.Add(paraKeys[0], paraKeys[1]);

                        if (paraKeys[3] == "Float")
                            paras.Add(paraKeys[0], float.Parse(paraKeys[1]));

                        if (paraKeys[3] == "Double")
                            paras.Add(paraKeys[0], double.Parse(paraKeys[1]));
                    }
                    return null;
                    //开始执行webserives.
                    break;
                case EventDoType.SpecClass:
                    #region //执行dll文件中指定类的指定方法，added by liuxc,2016-01-16
                    string evdll = nev.MonthedDLL;
                    string evclass = nev.MonthedClass;
                    string evmethod = nev.MonthedName;
                    string evparams = nev.MonthedParas;

                    if (string.IsNullOrWhiteSpace(evdll) || !System.IO.File.Exists(evdll))
                        throw new Exception("@DLLファイル[MonthedDLL]」“" + (evdll ?? string.Empty) + "設定が正しくありません。リセットしてください！");

                    Assembly abl = Assembly.LoadFrom(evdll);

                    //判断类是否是静态类
                    Type type = abl.GetType(evclass, false);

                    if (type == null)
                        throw new Exception(@"@DLLファイル[MonthedDLL]」“" + evdll + "”クラス名[MonthedClass]“" +
                                            (evclass ?? string.Empty) + "”設定が正しくありません。このタイプは取得されません。リセットしてください！");

                    //方法
                    if (string.IsNullOrWhiteSpace(evmethod))
                        throw new Exception(@"@DLLファイル[MonthedDLL]」“" + evdll + "”クラス[MonthedClass]“" +
                                            evclass + "”メソッド名[MonthedName]は空にできません。リセットしてください！");

                    MethodInfo md = null;   //当前方法
                    ParameterInfo[] pis = null; //方法的参数集合
                    Dictionary<string, string> pss = new Dictionary<string, string>();  //参数名，参数值类型名称字典，如：Name,String
                    string mdName = evmethod.Split('(')[0]; //方法名称

                    //获取method对象
                    if (mdName.Length == evmethod.Length - 2)
                    {
                        md = type.GetMethod(mdName);
                    }
                    else
                    {
                        string[] pssArr = null;

                        //获取设置里的参数信息
                        foreach (string pstr in evmethod.Substring(mdName.Length + 1, evmethod.Length - mdName.Length - 2).Split(','))
                        {
                            pssArr = pstr.Split(' ');
                            pss.Add(pssArr[1], pssArr[0]);
                        }

                        //与设置里的参数信息对比，取得MethodInfo对象
                        foreach (MethodInfo m in type.GetMethods())
                        {
                            if (m.Name != mdName) continue;

                            pis = m.GetParameters();
                            bool isOK = true;
                            int idx = 0;

                            foreach (KeyValuePair<string, string> ps in pss)
                            {
                                if (pis[idx].Name != ps.Key || pis[idx].ParameterType.ToString()
                                                                   .Replace("System.IO.", "")
                                                                   .Replace("System.", "")
                                                                   .Replace("System.Collections.Generic.", "")
                                                                   .Replace("System.Collections.", "") != ps.Value)
                                {
                                    isOK = false;
                                    break;
                                }

                                idx++;
                            }

                            if (isOK)
                            {
                                md = m;
                                break;
                            }
                        }
                    }

                    if (md == null)
                        throw new Exception(@"@DLLファイル[MonthedDLL]」“" + evdll + "”クラス[MonthedClass]“" +
                                            evclass + "”メソッド名[MonthedName]“" + evmethod + "設定が正しくありません。このメソッドは取得されませんでした。リセットしてください！");

                    //处理参数
                    object[] pvs = new object[pss.Count];   //invoke，传递的paramaters参数，数组中的项顺序与方法参数顺序一致

                    if (pss.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(evparams))
                            throw new Exception(@"@DLLファイル[MonthedDLL]」“" + evdll + "”クラス[MonthedClass]“" +
                                                evclass + "”メソッド[MonthedName]“" + evmethod + "パラメーター[MonthedParas]は空にできません。リセットしてください！");

                        Dictionary<string, string> pds = new Dictionary<string, string>();  //MonthedParas中保存的参数信息集合，格式如：title,@Title
                        int idx = 0;
                        int pidx = -1;
                        string[] pdsArr = evparams.Split(';');
                        string val;

                        //将参数中的名称与值分开
                        foreach(string p in pdsArr)
                        {
                            pidx = p.IndexOf('=');
                            if(pidx==-1) continue;

                            pds.Add(p.Substring(0, pidx), p.Substring(pidx + 1));
                        }

                        foreach (KeyValuePair<string, string> ps in pss)
                        {
                            if (!pds.ContainsKey(ps.Key))
                            {
                                //设置中没有此参数的值信息，则将值赋为null
                                pvs[idx] = null;
                            }
                            else
                            {
                                val = pds[ps.Key];

                                foreach (BP.En.Attr attr in en.EnMap.Attrs)
                                {
                                    if (pds[ps.Key] == "`" + attr.Key + "`")
                                    {
                                        //表示此参数与该attr的值一致，类型也一致
                                        pvs[idx] = en.Row[attr.Key];
                                        break;
                                    }

                                    //替换@属性
                                    val = val.Replace("`" + attr.Key + "`", (en.Row[attr.Key]??string.Empty).ToString());
                                }

                                //转换参数类型，从字符串转换到参数的实际类型，NOTE:此处只列出了简单类型的转换，其他类型暂未考虑
                                switch (ps.Value)
                                {
                                    case "String":
                                        pvs[idx] = val;
                                        break;
                                    case "Int32":
                                        pvs[idx] = int.Parse(val);
                                        break;
                                    case "Int64":
                                        pvs[idx] = long.Parse(val);
                                        break;
                                    case "Double":
                                        pvs[idx] = double.Parse(val);
                                        break;
                                    case "Single":
                                        pvs[idx] = float.Parse(val);
                                        break;
                                    case "Decimal":
                                        pvs[idx] = decimal.Parse(val);
                                        break;
                                    case "DateTime":
                                        pvs[idx] = DateTime.Parse(val);
                                        break;
                                    default:
                                        pvs[idx] = val;
                                        break;
                                }
                            }

                            idx++;
                        }
                    }

                    if (type.IsSealed && type.IsAbstract)
                    {
                        //静态类
                        return (md.Invoke(null, pvs) ?? string.Empty).ToString();
                    }

                    //非静态类
                    //虚类必须被重写，不能直接使用
                    if (type.IsAbstract)
                        return null;

                    //静态方法
                    if (md.IsStatic)
                        return (md.Invoke(null, pvs) ?? string.Empty).ToString();

                    //非静态方法
                    return (md.Invoke(abl.CreateInstance(evclass), pvs) ?? string.Empty).ToString();
                    #endregion
                default:
                    throw new Exception("@no such way." + nev.HisDoType.ToString());
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents()
        {
        }
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="FK_MapData">FK_MapData</param>
        public FrmEvents(string fk_MapData)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmEventAttr.FK_MapData, fk_MapData);
            qo.DoQuery();
        }
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="FK_MapData">按节点ID查询</param>
        public FrmEvents(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmEventAttr.FK_Node, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmEvent();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmEvent> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmEvent>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmEvent> Tolist()
        {
            System.Collections.Generic.List<FrmEvent> list = new System.Collections.Generic.List<FrmEvent>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmEvent)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}