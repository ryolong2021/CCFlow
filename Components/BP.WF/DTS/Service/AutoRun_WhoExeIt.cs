using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.WF.Data;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class AutoRun_WhoExeIt : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public AutoRun_WhoExeIt()
        {
            this.Title = "ノードの自動タスクを実行する.";
            this.Help = "ノードのプロパティで構成された自動実行またはマシンによって実行されたノード上のタスクは自動的に送信されます。";
            this.GroupName = "タイミングタスクの自動実行を処理する";
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
            string info = "";
            string sql = "SELECT WorkID,FID,FK_Emp,FK_Node,FK_Flow FROM WF_GenerWorkerList WHERE WhoExeIt!=0 AND IsPass=0 AND IsEnable=1 ORDER BY FK_Emp";
            DataTable dt = null;

            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return "タスクなし";

            #region 自动启动流程B
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                Int64 fid = Int64.Parse(dr["FID"].ToString());
                string FK_Emp = dr["FK_Emp"].ToString();
                int paras = int.Parse(dr["FK_Node"].ToString());
                string fk_flow = dr["FK_Flow"].ToString();

                if (BP.Web.WebUser.No.Equals(FK_Emp) == false)
                    BP.WF.Dev2Interface.Port_Login(FK_Emp);

                try
                {
                    info += "正常に送信されました:" + BP.Web.WebUser.No + BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                }
                catch (Exception ex)
                {
                    info += "err@送信する時エラー:" + ex.Message.ToString();
                }
            }
            #endregion 自动启动流程

            if (BP.Web.WebUser.No.Equals("admin") == false)
                BP.WF.Dev2Interface.Port_Login("admin");

            return info;
        }
    }
}
