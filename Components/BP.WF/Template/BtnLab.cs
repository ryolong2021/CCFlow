using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 公文工作模式
    /// </summary>
    public enum WebOfficeWorkModel
    {
        /// <summary>
        /// 不启用
        /// </summary>
        None,
        /// <summary>
        /// 按钮方式启用
        /// </summary>
        Button,
        /// <summary>
        /// 表单在前
        /// </summary>
        FrmFirst,
        /// <summary>
        /// 文件在前
        /// </summary>
        WordFirst
    }
    /// <summary>
    /// 节点按钮权限
    /// </summary>
    public class BtnLab : Entity
    {
        /// <summary>
        /// 访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                uac.IsDelete = false;
                return uac;
            }
        }

        #region 基本属性
        /// <summary>
        /// but
        /// </summary>
        public static string Btns
        {
            get
            {
                return "Send,Save,Thread,Return,CC,Shift,Del,Rpt,Ath,Track,Opt,EndFlow,SubFlow";
            }
        }
        /// <summary>
        /// PK
        /// </summary>
        public override string PK
        {
            get
            {
                return NodeAttr.NodeID;
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.NodeID);
            }
            set
            {
                this.SetValByKey(BtnAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.Name);
            }
            set
            {
                this.SetValByKey(BtnAttr.Name, value);
            }
        }
        /// <summary>
        /// 查询标签
        /// </summary>
        public string SearchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SearchLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchLab, value);
            }
        }
        /// <summary>
        /// 查询是否可用
        /// </summary>
        public bool SearchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SearchEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchEnable, value);
            }
        }
        /// <summary>
        /// 移交
        /// </summary>
        public string ShiftLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShiftLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftLab, value);
            }
        }
        /// <summary>
        /// 是否启用移交
        /// </summary>
        public bool ShiftEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ShiftEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftEnable, value);
            }
        }
        /// <summary>
        /// 选择接受人
        /// </summary>
        public string SelectAccepterLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SelectAccepterLab);
            }
        }
        /// <summary>
        /// 选择接受人类型
        /// </summary>
        public int SelectAccepterEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.SelectAccepterEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.SelectAccepterEnable, value);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        public string SaveLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SaveLab);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool SaveEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SaveEnable);
            }
        }
        /// <summary>
        /// 子线程按钮标签
        /// </summary>
        public string ThreadLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ThreadLab);
            }
        }
        /// <summary>
        /// 子线程按钮是否启用
        /// </summary>
        public bool ThreadEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadEnable);
            }
        }
        /// <summary>
        /// 是否可以删除（当前分流，分合流节点发送出去的）子线程.
        /// </summary>
        public bool ThreadIsCanDel
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadIsCanDel);
            }
        }
        /// <summary>
        /// 是否可以移交.
        /// </summary>
        public bool ThreadIsCanShift
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadIsCanShift);
            }
        }
        /// <summary>
        /// 子流程按钮标签
        /// </summary>
        public string SubFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SubFlowLab);
            }
        }
        /// <summary>
        /// 跳转标签
        /// </summary>
        public string JumpWayLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.JumpWayLab);
            }
        }
        public JumpWay JumpWayEnum
        {
            get
            {
                return (JumpWay)this.GetValIntByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 是否启用跳转
        /// </summary>
        public bool JumpWayEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 退回标签
        /// </summary>
        public string ReturnLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnLab);
            }
        }
        /// <summary>
        /// 退回字段
        /// </summary>
        public string ReturnField
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnField);
            }
        }
        /// <summary>
        /// 退回是否启用
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        /// <summary>
        /// 挂起标签
        /// </summary>
        public string HungLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HungLab);
            }
        }
        /// <summary>
        /// 是否启用挂起
        /// </summary>
        public bool HungEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.HungEnable);
            }
        }
        /// <summary>
        /// 打印标签
        /// </summary>
        public string PrintDocLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintDocLab);
            }
        }
        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool PrintDocEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintDocEnable);
            }
        }
        /// <summary>
        /// 发送标签
        /// </summary>
        public string SendLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SendLab);
            }
        }
        /// <summary>
        /// 是否启用发送?
        /// </summary>
        public bool SendEnable
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 发送的Js代码
        /// </summary>
        public string SendJS
        {
            get
            {
                string str = this.GetValStringByKey(BtnAttr.SendJS).Replace("~", "'");
                if (this.CCRole == BP.WF.CCRole.WhenSend)
                    str = str + "  if ( OpenCC()==false) return false;";
                return str;
            }
        }
        /// <summary>
        /// 轨迹标签
        /// </summary>
        public string TrackLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TrackLab);
            }
        }
        /// <summary>
        /// 是否启用轨迹
        /// </summary>
        public bool TrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TrackEnable);
            }
        }
        /// <summary>
        /// 查看父流程标签
        /// </summary>
        public string ShowParentFormLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShowParentFormLab);
            }
        }

        /// <summary>
        /// 是否启用查看父流程
        /// </summary>
        public bool ShowParentFormEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ShowParentFormEnable);
            }
        }


        /// <summary>
        /// 抄送标签
        /// </summary>
        public string CCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.CCLab);
            }
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        public CCRole CCRole
        {
            get
            {
                return (CCRole)this.GetValIntByKey(BtnAttr.CCRole);
            }
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        public string DeleteLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.DelLab);
            }
        }
        /// <summary>
        /// 删除类型
        /// </summary>
        public int DeleteEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.DelEnable);
            }
        }
        /// <summary>
        /// 结束流程
        /// </summary>
        public string EndFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.EndFlowLab);
            }
        }
        /// <summary>
        /// 是否启用结束流程
        /// </summary>
        public bool EndFlowEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.EndFlowEnable);
            }
        }
        /// <summary>
        /// 是否启用流转自定义
        /// </summary>
        public string TCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TCLab);
            }
        }
        /// <summary>
        /// 是否启用流转自定义
        /// </summary>
        public bool TCEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TCEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.TCEnable, value);
            }
        }

        public int HelpRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.HelpRole);
            }
        }

        public string HelpLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HelpLab);
            }
        }

        /// <summary>
        /// 审核标签
        /// </summary>
        public string WorkCheckLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.WorkCheckLab);
            }
        }
        /// <summary>
        /// 标签是否启用？
        /// </summary>
        //public bool SubFlowEnable111
        //{
        //    get
        //    {
        //        return this.GetValBooleanByKey(BtnAttr.SubFlowEnable);
        //    }
        //    set
        //    {
        //        this.SetValByKey(BtnAttr.SubFlowEnable, value);
        //    }
        //}
        /// <summary>
        /// 审核是否可用
        /// </summary>
        public bool WorkCheckEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.WorkCheckEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.WorkCheckEnable, value);
            }
        }
        /// <summary>
        /// 考核 是否可用
        /// </summary>
        public int CHRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.CHRole);
            }
        }
        /// <summary>
        /// 考核 标签
        /// </summary>
        public string CHLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.CHLab);
            }
        }
        /// <summary>
        /// 重要性 是否可用
        /// </summary>
        public bool PRIEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PRIEnable);
            }
        }
        /// <summary>
        /// 重要性 标签
        /// </summary>
        public string PRILab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PRILab);
            }
        }
        /// <summary>
        /// 关注 是否可用
        /// </summary>
        public bool FocusEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.FocusEnable);
            }
        }
        /// <summary>
        /// 关注 标签
        /// </summary>
        public string FocusLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.FocusLab);
            }
        }

        /// <summary>
        /// 分配 是否可用
        /// </summary>
        public bool AllotEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AllotEnable);
            }
        }
        /// <summary>
        /// 分配 标签
        /// </summary>
        public string AllotLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AllotLab);
            }
        }

        /// <summary>
        /// 确认 是否可用
        /// </summary>
        public bool ConfirmEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ConfirmEnable);
            }
        }
        /// <summary>
        /// 确认标签
        /// </summary>
        public string ConfirmLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ConfirmLab);
            }
        }

        /// <summary>
        /// 打包下载 是否可用
        /// </summary>
        public bool PrintZipEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintZipEnable);
            }
        }
        /// <summary>
        /// 打包下载 标签
        /// </summary>
        public string PrintZipLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintZipLab);
            }
        }
        /// <summary>
        /// pdf 是否可用
        /// </summary>
        public bool PrintPDFEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintPDFEnable);
            }
        }
        /// <summary>
        /// 打包下载 标签
        /// </summary>
        public string PrintPDFLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintPDFLab);
            }
        }

        /// <summary>
        /// html 是否可用
        /// </summary>
        public bool PrintHtmlEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintHtmlEnable);
            }
        }
        /// <summary>
        /// html 标签
        /// </summary>
        public string PrintHtmlLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintHtmlLab);
            }
        }


        /// <summary>
        /// 批量处理是否可用
        /// </summary>
        public bool BatchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.BatchEnable);
            }
        }
        /// <summary>
        /// 批处理标签
        /// </summary>
        public string BatchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.BatchLab);
            }
        }
        /// <summary>
        /// 加签
        /// </summary>
        public bool AskforEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AskforEnable);
            }
        }
        /// <summary>
        /// 加签
        /// </summary>
        public string AskforLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AskforLab);
            }
        }
        /// <summary>
        /// 会签规则
        /// </summary>
        public HuiQianRole HuiQianRole
        {
            get
            {
                return (HuiQianRole)this.GetValIntByKey(BtnAttr.HuiQianRole);
            }
        }
        /// <summary>
        /// 会签标签
        /// </summary>
        public string HuiQianLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HuiQianLab);
            }
        }
        
        // public int IsCanAddHuiQianer
        //{
        //    get
        //    {
        //        return this.GetValIntByKey(BtnAttr.IsCanAddHuiQianer);
        //    }
        //}
        public HuiQianLeaderRole HuiQianLeaderRole
        {
            get
            {
                return (HuiQianLeaderRole)this.GetValIntByKey(BtnAttr.HuiQianLeaderRole);
            }
        }

        public string AddLeaderLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.AddLeaderLab);
            }
        }

        public bool AddLeaderEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AddLeaderEnable);
            }
        }

        /// <summary>
        ///是否启用文档,@0=不启用@1=按钮方式@2=公文在前@3=表单在前
        /// </summary>
        private int WebOfficeEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.WebOfficeEnable);
            }
        }
        /// <summary>
        /// 公文的工作模式 @0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式
        /// </summary>
        public WebOfficeWorkModel WebOfficeWorkModel
        {
            get
            {
                return (WebOfficeWorkModel)this.WebOfficeEnable;
            }
            set
            {
                this.SetValByKey(BtnAttr.WebOfficeEnable, (int)value);
            }
        }
        /// <summary>
        /// 文档按钮标签
        /// </summary>
        public string WebOfficeLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.WebOfficeLab);
            }
        }
        /// <summary>
        /// 打开本地文件
        /// </summary>
        public bool OfficeOpenEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOpenEnable); }
        }
        /// <summary>
        /// 打开本地标签      
        /// </summary>
        public string OfficeOpenLab
        {
            get { return this.GetValStrByKey(BtnAttr.OfficeOpenLab); }
        }
        /// <summary>
        /// 打开模板
        /// </summary>
        public bool OfficeOpenTemplateEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOpenTemplateEnable); }
        }
        /// <summary>
        /// 打开模板标签
        /// </summary>
        public string OfficeOpenTemplateLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOpenTemplateLab); }
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        public bool OfficeSaveEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeSaveEnable); }
        }
        /// <summary>
        /// 保存标签
        /// </summary>
        public string OfficeSaveLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeSaveLab); }
        }
        /// <summary>
        /// 接受修订
        /// </summary>
        public bool OfficeAcceptEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeAcceptEnable); }
        }
        /// <summary>
        /// 接受修订标签
        /// </summary>
        public string OfficeAcceptLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeAcceptLab); }
        }
        /// <summary>
        /// 拒绝修订
        /// </summary>
        public bool OfficeRefuseEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeRefuseEnable); }
        }
        /// <summary>
        /// 拒绝修订标签
        /// </summary>
        public string OfficeRefuseLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeRefuseLab); }
        }
        public string OfficeOVerLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOverLab); }
        }
        /// <summary>
        /// 是否套红
        /// </summary>
        public bool OfficeOverEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeOverEnable); }
        }
        /// <summary>
        /// 套红按钮标签
        /// </summary>
        public string OfficeOverLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeOverLab); }
        }
        /// <summary>
        /// 是否打印
        /// </summary>
        public bool OfficePrintEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficePrintEnable); }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool OfficeMarksEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeMarksEnable); }
        }
        /// <summary>
        /// 打印按钮标签
        /// </summary>
        public string OfficePrintLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficePrintLab); }
        }
        /// <summary>
        /// 签章按钮
        /// </summary>
        public bool OfficeSealEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeSealEnable); }
        }
        /// <summary>
        /// 签章标签
        /// </summary>
        public string OfficeSealLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeSealLab); }
        }

        /// <summary>
        ///插入流程
        /// </summary>
        public bool OfficeInsertFlowEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeInsertFlowEnable); }
        }
        /// <summary>
        /// 流程标签
        /// </summary>
        public string OfficeInsertFlowLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeInsertFlowLab); }
        }


        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeNodeInfo
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeNodeInfo); }
        }

        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeReSavePDF
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeReSavePDF); }
        }

        /// <summary>
        /// 是否进入留痕模式
        /// </summary>
        public bool OfficeIsMarks
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeIsMarks); }
        }

        /// <summary>
        /// 风险点模板
        /// </summary>
        public String OfficeFengXianTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeFengXianTemplate); }
        }

        public bool OfficeReadOnly
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeReadOnly); }
        }

        /// <summary>
        /// 下载按钮标签
        /// </summary>
        public String OfficeDownLab
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeDownLab); }
        }
        /// <summary>
        /// 下载按钮标签
        /// </summary>
        public bool OfficeIsDown
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeDownEnable); }
        }

        /// <summary>
        /// 是否启用下载
        /// </summary>
        public bool OfficeDownEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeDownEnable); }
        }

        /// <summary>
        /// 指定文档模板
        /// </summary>
        public String OfficeTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeTemplate); }
        }


        /// <summary>
        /// 是否使用父流程的文档
        /// </summary>
        public bool OfficeIsParent
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeIsParent); }
        }

        /// <summary>
        /// 是否自动套红
        /// </summary>
        public bool OfficeTHEnable
        {
            get { return this.GetValBooleanByKey(BtnAttr.OfficeTHEnable); }
        }
        /// <summary>
        /// 自动套红模板
        /// </summary>
        public string OfficeTHTemplate
        {
            get { return this.GetValStringByKey(BtnAttr.OfficeTHTemplate); }
        }


        /// <summary>
        /// 公文标签
        /// </summary>
        public string OfficeBtnLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.OfficeBtnLab);
            }
        }
        /// <summary>
        /// 公文标签
        /// </summary>
        public bool OfficeBtnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.OfficeBtnEnable);
            }
        }


        /// <summary>
        /// 备注标签
        /// </summary>
        public string NoteLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.NoteLab);
            }
        }
        /// <summary>
        ///备注标签
        /// </summary>
        public int NoteEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.NoteEnable);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLab() { }
        /// <summary>
        /// 节点按钮权限
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public BtnLab(int nodeid)
        {
            this.NodeID = nodeid;
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

                Map map = new Map("WF_Node", "ノードラベル");


                map.Java_SetDepositaryOfEntity(Depositary.Application);

                map.AddTBIntPK(BtnAttr.NodeID, 0, "ノードID", true, true);
                map.AddTBString(BtnAttr.Name, null, "ノード名", true, true, 0, 200, 10);

                map.AddTBString(BtnAttr.SendLab, "送る", "送信ボタンのラベル", true, false, 0, 50, 10);
                map.AddTBString(BtnAttr.SendJS, "", "送信ボタンJS機能", true, false, 0, 50, 10, true);

                //map.AddBoolean(BtnAttr.SendEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.JumpWayLab, "遷移", "遷移ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(NodeAttr.JumpWay, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.SaveLab, "保存する", "保存ボタンのラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "有効にするかどうか", true, true);


                map.AddTBString(BtnAttr.ThreadLab, "子スレッド", "子スレッドボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(BtnAttr.ThreadIsCanDel, false, "子スレッドを削除できますか（現在のノードが送信したスレッドで、現在のノードが分割されているか、分割とマージが有効で、子スレッドが戻った後の操作）？", true, true, true);
                map.AddBoolean(BtnAttr.ThreadIsCanShift, false, "子スレッドを引き渡すことができますか（現在のノードが送信したスレッドで、現在のノードが分割されているか、分割とマージが有効で、子スレッドが戻った後の操作）？", true, true, true);

              
                // add 2019.1.9 for 东孚.
                map.AddTBString(BtnAttr.OfficeBtnLab, "公式ドキュメントを開く", "くもんボタンラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.OfficeBtnEnable, 0, "ファイルの状態", true, true, BtnAttr.OfficeBtnEnable,
                "@@0=利用不可@1=編集可能@2=編集不可", false);


                map.AddTBString(BtnAttr.ReturnLab, "返す", "返すボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ReturnRole, true, "有効にするかどうか", true, true);
                map.AddTBString(BtnAttr.ReturnField, "", "フィールドに入力する情報を返す", true, false, 0, 50, 10, true);

                map.AddDDLSysEnum(NodeAttr.ReturnOneNodeRole, 0, "単一ノードの返品ルール", true, true, NodeAttr.ReturnOneNodeRole,
                "@@0=無効にする@1=[コメントを入力フィールドに入力]に従って直接コメントを返す@2=[レビューコンポーネント]に入力された情報に従ってコメントを直接返す", true);


                map.AddTBString(BtnAttr.CCLab, "CC", "CCボタンラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CCRole, 0, "CCルール", true, true, BtnAttr.CCRole);

                //  map.AddBoolean(BtnAttr, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.ShiftLab, "転送", "転送ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, true, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.DelLab, "フローを削除", "フローを削除ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.DelEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.EndFlowLab, "フローを終了する", "フローを終了するボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.HungLab, "ハング", "ハングボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.ShowParentFormLab, "親フローを表示", "親フローを表示ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShowParentFormEnable, false, "有効にするかどうか", true, true);
                map.SetHelperAlert(BtnAttr.ShowParentFormLab,"現在のフローインスタンスがサブフローでない場合、ボタンを表示せずにすぐに有効になります。");

                map.AddTBString(BtnAttr.PrintDocLab, "領収書を印刷する", "領収書を印刷するボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.TrackLab, "追跡", "追跡ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.SelectAccepterLab, "受取人", "受取人ボタンラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "方式",
          true, true, BtnAttr.SelectAccepterEnable);

                // map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.OptLab, "选项", "选项按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OptEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SearchLab, "問い合わせる", "問い合わせるボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, true, "有効にするかどうか", true, true);

                // 
                map.AddTBString(BtnAttr.WorkCheckLab, "レビュー", "レビューボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "有効にするかどうか", true, true);

                // 
                map.AddTBString(BtnAttr.BatchLab, "バッチレビュー", "バッチレビューボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.BatchEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.AskforLab, "裏書", "裏書ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.HuiQianLab, "副署", "副署ボタンラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "副署モード", true, true, BtnAttr.HuiQianRole,
                    "@0=無効@1=コラボレーションモード@4=グループリーダーモードを無効にする");
                //map.AddDDLSysEnum(BtnAttr.IsCanAddHuiQianer, 0, "协作模式被加签的人处理规则", true, true, BtnAttr.IsCanAddHuiQianer,
                 //    "0=不允许增加其他协作人@1=允许增加协作人", false);


                map.AddDDLSysEnum(BtnAttr.HuiQianLeaderRole, 0, "副署リーダーのルール", true, true, BtnAttr.HuiQianLeaderRole,
                     "0=グループリーダーは1つだけ@1=最後のグループリーダーが送信@2=グループリーダーが送信",true);

                map.AddTBString(BtnAttr.AddLeaderLab, "ホストを追加", "ホストを追加", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AddLeaderEnable, false, "有効にするかどうか", true, true);

                //map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "会签模式", true, true, BtnAttr.HuiQianRole, "@0=不启用@1=组长模式@2=协作模式");

                // map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
                //  map.AddBoolean(BtnAttr.HuiQianRole, false, "是否启用", true, true);

                // add by stone 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "循環カスタム", "循環カスタム", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.WebOfficeLab, "公式文書", "公式文書ボタンラベル", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.WebOfficeEnable, false, "是否启用", true, true);
                map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "ドキュメントのアクティベーション方法", true, true, BtnAttr.WebOfficeEnable,
                 "@0=無効@1=ボタンメソッド@2=ラベルバックモード@3=ラベルフロントモード");  //edited by liuxc,2016-01-18,from xc.

                // add by 周朋 2015-08-06. 重要性.
                map.AddTBString(BtnAttr.PRILab, "重要性", "重要性", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PRIEnable, false, "有効にするかどうか", true, true);

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "ノードの時間制限", "ノードの時間制限", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CHRole, 0, "制限時間", true, true, BtnAttr.CHRole, @"0=無効@1=有効@2=読み取り専用@3=有効にし、フローが完了する必要がある時間を調整します");

                // add 2017.5.4  邀请其他人参与当前的工作.
                map.AddTBString(BtnAttr.AllotLab, "分布", "分布ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AllotEnable, false, "有効にするかどうか", true, true);


                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "注意", "注意", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, true, "有効にするかどうか", true, true);

                // add 2017.5.4 确认就是告诉发送人，我接受这件工作了.
                map.AddTBString(BtnAttr.ConfirmLab, "確認", "確認ボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ConfirmEnable, false, "有効にするかどうか", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintHtmlLab, "HTMLを印刷", "HTMLを印刷ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintHtmlEnable, false, "有効にするかどうか", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintPDFLab, "PDFを印刷", "PDFを印刷ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "有効にするかどうか", true, true);

                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintZipLab, "パッケージをダウンロード", "パッケージをダウンロードzipボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "有効にするかどうか", true, true);

                // add 2019.3.10 增加List.
                map.AddTBString(BtnAttr.ListLab, "リスト", "リストボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ListEnable, true, "有効にするかどうか", true, true);

                //备注 流程不流转，设置备注信息提醒已处理人员当前流程运行情况
                map.AddTBString(BtnAttr.NoteLab, "備考", "備考ラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.NoteEnable, 0, "ルールを有効にする", true, true, BtnAttr.NoteEnable, @"0=無効@1=有効@2=読み取り専用");

                //for 周大福.
                map.AddTBString(BtnAttr.HelpLab, "助けて", "助けてラベル", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HelpRole, 0, "ルールの表示に役立つ", true, true, BtnAttr.HelpRole, @"0=無効@1=有効@2=強制プロンプト@3=選択プロンプト");


                #region 公文按钮
                map.AddTBString(BtnAttr.OfficeOpenLab, "ローカルを開く", "ローカルを開くラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeOpenTemplateLab, "テンプレートを開く", "テンプレートを開くラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOpenTemplateEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeSaveLab, "保存する", "保存するラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSaveEnable, true, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeAcceptLab, "改訂を承認", "改訂を承認ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeAcceptEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeRefuseLab, "改訂を拒否", "改訂を拒否ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeRefuseEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeOverLab, "赤", "赤ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeOverEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(BtnAttr.OfficeMarksEnable, true, "ユーザートレースをチェックするかどうか", true, true, true);

                map.AddTBString(BtnAttr.OfficePrintLab, "印刷する", "印刷するラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficePrintEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeSealLab, "署名", "署名ラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeSealEnable, false, "有効にするかどうか", true, true);

                map.AddTBString(BtnAttr.OfficeInsertFlowLab, "挿入フロー", "挿入フローラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeInsertFlowEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(BtnAttr.OfficeNodeInfo, false, "ノード情報を記録するかどうか", true, true);
                map.AddBoolean(BtnAttr.OfficeReSavePDF, false, "PDFとして自動的に保存する必要があります", true, true);


                map.AddTBString(BtnAttr.OfficeDownLab, "ダウンロード", "ダウンロードボタンラベル", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeDownEnable, false, "有効にするかどうか", true, true);

                map.AddBoolean(BtnAttr.OfficeIsMarks, true, "マークモードに入るかどうか", true, true);
                map.AddTBString(BtnAttr.OfficeTemplate, "", "ドキュメントテンプレートを指定", true, false, 0, 100, 10);
                map.AddBoolean(BtnAttr.OfficeIsParent, true, "親フローのドキュメントを使用するかどうか", true, true);

                map.AddBoolean(BtnAttr.OfficeTHEnable, false, "自動的に裁定取引を行うかどうか", true, true);
                map.AddTBString(BtnAttr.OfficeTHTemplate, "", "自動赤テンプレート", true, false, 0, 200, 10);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            fl.Update();

            BtnLab btnLab = new BtnLab();
            btnLab.NodeID = this.NodeID;
            btnLab.RetrieveFromDBSources();
            btnLab.Update();

            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 节点按钮权限s
    /// </summary>
    public class BtnLabs : Entities
    {
        /// <summary>
        /// 节点按钮权限s
        /// </summary>
        public BtnLabs()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BtnLab();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BtnLab> ToJavaList()
        {
            return (System.Collections.Generic.IList<BtnLab>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BtnLab> Tolist()
        {
            System.Collections.Generic.List<BtnLab> list = new System.Collections.Generic.List<BtnLab>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BtnLab)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
