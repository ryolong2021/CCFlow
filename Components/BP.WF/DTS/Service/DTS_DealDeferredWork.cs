using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.Web;

namespace BP.WF.DTS
{
    /// <summary>
    /// 处理延期的任务 的摘要说明
    /// </summary>
    public class DTS_DealDeferredWork : Method
    {
        /// <summary>
        /// 处理延期的任务
        /// </summary>
        public DTS_DealDeferredWork()
        {
            this.Title = "期限切れのタスクに対処する";
            this.Help = "1日1回実行する必要があり、期限超過作業は期限超過ルールに従って処理されます。";
            this.GroupName = "タイミングタスクの自動実行を処理する";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            //string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <='" + DataType.CurrentData + "' ORDER BY FK_Emp";
            //改成小于号SDT <'" + DataType.CurrentData
            string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <'" + DataType.CurrentData + "' ORDER BY FK_Emp";
            //string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <='2013-12-30' ORDER BY FK_Emp";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            string msg = "";
            string dealWorkIDs = "";
            foreach (DataRow dr in dt.Rows)
            {

                string FK_Emp = dr["FK_Emp"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();
                int fk_node = int.Parse(dr["FK_Node"].ToString());
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                Int64 fid = Int64.Parse(dr["FID"].ToString());

                // 方式两个人同时处理一件工作, 一个人处理后，另外一个人还可以处理的情况.
                if (dealWorkIDs.Contains("," + workid + ","))
                    continue;
                dealWorkIDs += "," + workid + ",";

                if (WebUser.No != FK_Emp)
                {
                    Emp emp = new Emp(FK_Emp);
                    BP.Web.WebUser.SignInOfGener(emp);
                }

                BP.WF.Template.NodeExt nd = new BP.WF.Template.NodeExt();
                nd.NodeID = fk_node;
                nd.Retrieve();

                // 首先判断是否有启动的表达式, 它是是否自动执行的总阀门。
                if (DataType.IsNullOrEmpty(nd.DoOutTimeCond) == false)
                {
                    Node nodeN = new Node(nd.NodeID);
                    Work wk = nodeN.HisWork;
                    wk.OID = workid;
                    wk.Retrieve();
                    string exp = nd.DoOutTimeCond.Clone() as string;
                    if (Glo.ExeExp(exp, wk) == false)
                        continue; // 不能通过条件的设置.
                }

                switch (nd.HisOutTimeDeal)
                {
                    case OutTimeDeal.None:
                        break;
                    case OutTimeDeal.AutoTurntoNextStep: //自动转到下一步骤.
                        if (DataType.IsNullOrEmpty(nd.DoOutTime))
                        {
                            /*如果是空的,没有特定的点允许，就让其它向下执行。*/
                            msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                        }
                        else
                        {
                            int nextNode = Dev2Interface.Node_GetNextStepNode(fk_flow, workid);
                            if (nd.DoOutTime.Contains(nextNode.ToString())) /*如果包含了当前点的ID,就让它执行下去.*/
                                msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                        }
                        break;
                    case OutTimeDeal.AutoJumpToSpecNode: //自动的跳转下一个节点.
                        if (DataType.IsNullOrEmpty(nd.DoOutTime))
                            throw new Exception("@設定が間違っています。次に遷移するノードが設定されていません.");
                        int nextNodeID = int.Parse(nd.DoOutTime);
                        msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, null, null, nextNodeID, null).ToMsgOfText();
                        break;
                    case OutTimeDeal.AutoShiftToSpecUser: //移交给指定的人员.
                        msg += BP.WF.Dev2Interface.Node_Shift( workid,nd.DoOutTime, "ccflowからの自動メッセージ：(" + BP.Web.WebUser.Name + ")仕事は時間どおりに処理されません(" + nd.Name + "),今あなたに引き渡します。");
                        break;
                    case OutTimeDeal.SendMsgToSpecUser: //向指定的人员发消息.
                        BP.WF.Dev2Interface.Port_SendMsg(nd.DoOutTime,
                            "ccflowからの自動メッセージ：(" + BP.Web.WebUser.Name + ")仕事は時間どおりに処理されません(" + nd.Name + ")",
                            "ccflowをお選びいただきありがとうございます.", "SpecEmp" + workid);
                        break;
                    case OutTimeDeal.DeleteFlow: //删除流程.
                        msg += BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, true);
                        break;
                    case OutTimeDeal.RunSQL:
                        msg += BP.DA.DBAccess.RunSQL(nd.DoOutTime);
                        break;
                    default:
                        throw new Exception("@エラー判定されないタイムアウト処理方法。" + nd.HisOutTimeDeal);
                }
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);
            return msg;
             
        }
    }
}
