using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AutoRunOverTimeFlow : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AutoRunOverTimeFlow()
        {
            this.Title = "期限切れのタスクに対処する";
            this.Help = "ノード構成の予想されるルールに従って、期限切れのタスクをスキャンして処理する";
            this.GroupName = "タイミングタスクの自動実行のフロー";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
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
            #region 找到要逾期的数据.
            DataTable generTab = null;
            string sql = "SELECT a.FK_Flow,a.WorkID,a.Title,a.FK_Node,a.SDTOfNode,a.Starter,a.TodoEmps ";
            sql += "FROM WF_GenerWorkFlow a, WF_Node b";
            sql += " WHERE a.SDTOfNode<='" + DataType.CurrentDataTime + "' ";
            sql += " AND WFState=2 and b.OutTimeDeal!=0";
            sql += " AND a.FK_Node=b.NodeID";
            generTab = DBAccess.RunSQLReturnTable(sql);
            #endregion 找到要逾期的数据.

            // 遍历循环,逾期表进行处理.
            string msg = "";
            string info = "";
            foreach (DataRow row in generTab.Rows)
            {
                string fk_flow = row["FK_Flow"] + "";
                int fk_node = int.Parse(row["FK_Node"] + "");
                long workid = long.Parse(row["WorkID"] + "");
                string title = row["Title"] + "";
                string compleateTime = row["SDTOfNode"] + "";
                string starter = row["Starter"] + "";

                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, fk_node);

                bool isLogin = false;
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.IsEnable == false)
                        continue;

                    BP.Port.Emp emp = new Emp(item.FK_Emp);
                    BP.Web.WebUser.SignInOfGener(emp);
                    isLogin = true;
                }

                if (isLogin == false)
                {
                    BP.Port.Emp emp = new Emp("admin");
                    BP.Web.WebUser.SignInOfGener(emp);
                }


                try
                {
                    Node node = new Node(fk_node);
                    if (node.IsStartNode)
                        continue;

                    //获得该节点的处理内容.
                    string doOutTime = node.GetValStrByKey(NodeAttr.DoOutTime);
                    switch (node.HisOutTimeDeal)
                    {
                        case OutTimeDeal.None: //逾期不处理.
                            continue;
                        case OutTimeDeal.AutoJumpToSpecNode: //跳转到指定的节点.
                            try
                            {
                                //if (doOutTime.Contains(",") == false)
                                //    throw new Exception("@系统设置错误，不符合设置规范,格式为: NodeID,EmpNo  现在设置的为:"+doOutTime);

                                int jumpNode = int.Parse(doOutTime);
                                Node jumpToNode = new Node(jumpNode);

                                //设置默认同意.
                                BP.WF.Dev2Interface.WriteTrackWorkCheck(jumpToNode.FK_Flow, node.NodeID, workid, 0,
                                    "同意（期限切れの自動承認）", null);

                                //执行发送.
                                info = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, null, null, jumpToNode.NodeID, null).ToMsgOfText();

                                // info = BP.WF.Dev2Interface.Flow_Schedule(workid, jumpToNode.NodeID, emp.No);
                                msg = "フロー '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは'自動遷移'," + info;


                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);

                            }
                            catch (Exception ex)
                            {
                                msg = "フロー  '" + node.FlowName + "',WorkID=" + workid + ",タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは'自動遷移'で、遷移が異常:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.AutoShiftToSpecUser: //走动移交给.
                            // 判断当前的处理人是否是.
                            Emp empShift = new Emp(doOutTime);
                            try
                            {
                                BP.WF.Dev2Interface.Node_Shift(workid, empShift.No,
                                    "フローノードが期限切れになり、システムが自動的に引き渡されました");

                                msg = "フロー  '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは「指定者に引き継がれ」、自動的に引き継がれている" + empShift.Name + ".";
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "フロー  '" + node.FlowName + "' ,タイトル：'" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは「指定人物への転送」であり、転送が異常：" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.AutoTurntoNextStep:
                            try
                            {
                                GenerWorkerList workerList = new GenerWorkerList();
                                workerList.RetrieveByAttrAnd(GenerWorkerListAttr.WorkID, workid,
                                    GenerWorkFlowAttr.FK_Node, fk_node);

                                BP.Web.WebUser.SignInOfGener(workerList.HisEmp);

                                WorkNode firstwn = new WorkNode(workid, fk_node);
                                string sendIfo = firstwn.NodeSend().ToMsgOfText();
                                msg = "フロー  '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは「次のノードに自動的に送信」され、送信メッセージは:" + sendIfo;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "フロー  '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは「次のノードに自動的に送信」、発送する異常:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.DeleteFlow:
                            info = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, true);
                            msg = "フロー  '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                  "'タイムアウト処理ルールは「フローを削除」です," + info;
                            SetText(msg);
                            BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            break;
                        case OutTimeDeal.RunSQL:
                            try
                            {
                                BP.WF.Work wk = node.HisWork;
                                wk.OID = workid;
                                wk.Retrieve();

                                doOutTime = BP.WF.Glo.DealExp(doOutTime, wk, null);

                                //替换字符串
                                doOutTime.Replace("@OID", workid + "");
                                doOutTime.Replace("@FK_Flow", fk_flow);
                                doOutTime.Replace("@FK_Node", fk_node.ToString());
                                doOutTime.Replace("@Starter", starter);
                                if (doOutTime.Contains("@"))
                                {
                                    msg = "フロー '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード'" + node.Name +
                                          "'タイムアウト処理ルールは「SQL実行」です。置き換えられていないSQL変数があります。'";
                                    SetText(msg);
                                    BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                                    break;
                                }

                                //执行sql.
                                DBAccess.RunSQL(doOutTime);
                            }
                            catch (Exception ex)
                            {
                                msg = "フロー  '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは「SQL実行」です。SQLの実行中に例外が発生します:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        case OutTimeDeal.SendMsgToSpecUser:
                            try
                            {
                                Emp myemp = new Emp(doOutTime);

                                bool boo = BP.WF.Dev2Interface.WriteToSMS(myemp.No, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "システムが期限切れのメッセージを送信する",
                                    "あなたのフロー： '" + title + "'完了時間は '" + compleateTime + "',フローの期限が切れました。時間内に処理してください！", "システムインフォメーション");
                                if (boo)
                                    msg = "'" + title + "'期限切れのメッセージが送信されました： '" + myemp.Name + "'";
                                else
                                    msg = "'" + title + "'期限切れのメッセージは正常に送信されず、送信者は次のとおりでした。" + myemp.Name + "'";
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Info, msg);
                            }
                            catch (Exception ex)
                            {
                                msg = "フロー  '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                      "'タイムアウト処理ルールは「SQL実行」です。SQLの実行中に例外が発生します:" + ex.Message;
                                SetText(msg);
                                BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            }
                            break;
                        default:
                            msg = "フロー '" + node.FlowName + "',タイトル： '" + title + "'で完了する時間は'" + compleateTime + "',現在のノード '" + node.Name +
                                  "'対応するタイムアウト処理ルールが見つかりませんでした.";
                            SetText(msg);
                            BP.DA.Log.DefaultLogWriteLine(LogType.Error, msg);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    SetText("フローが期限を過ぎて異常が発生しました:" + ex.Message);
                    BP.DA.Log.DefaultLogWriteLine(LogType.Error, ex.ToString());

                }
            }
            return generInfo;
        }

        public string generInfo="";
        public void SetText(string msg)
        {
            generInfo += "\t\n" + msg;
        }
    }
}
