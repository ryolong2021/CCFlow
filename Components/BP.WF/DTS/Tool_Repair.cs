using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 升级ccflow6 要执行的调度
    /// </summary>
    public class Tool_Repair : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public Tool_Repair()
        {
            this.Title = "表示できないノードのドロップダウンボックスが原因で、送信できないバグによって引き起こされたジャンクデータを修正します。";
            this.Help = "このバグは修正されました。同様の問題が引き続き発生する場合は、他の問題が原因である可能性があります。";
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
                if (BP.Web.WebUser.No == "admin")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string sql = "SELECT * FROM WF_GENERWORKFLOW WHERE WFState=2 ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string todoEmps = dr["TODOEMPS"].ToString();
                string nodeID = dr["FK_NODE"].ToString();

                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.IsPass, 0);
                foreach (GenerWorkerList gwl in gwls)
                {
                    if (todoEmps.Contains(gwl.FK_Emp + ",") == false)
                    {
                        if (nodeID.ToString().EndsWith("01") == true)
                            continue;

                        GenerWorkFlow gwf = new GenerWorkFlow(workid);
                        msg += "<br>@フロー:" + gwf.FlowName + "ノード:" + gwf.FK_Node + "," + gwf.NodeName + " workid: " + workid + "title:" + gwf.Title + " todoEmps:" + gwf.TodoEmps;
                        msg += "含まない:" + gwl.FK_Emp + "," + gwl.FK_EmpText;

                        gwf.TodoEmps += gwl.FK_Emp + "," + gwl.FK_EmpText + ";";
                        gwf.Update();
                    }
                }
            }
            return msg;
        }
    }
}
