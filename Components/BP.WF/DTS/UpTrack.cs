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
    public class UpTrack : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public UpTrack()
        {
            this.Title = "ccflow6が実行するスケジュールをアップグレードします（wf_track部分のみが処理され、繰り返し実行することはできません）。";
            this.Help = "このフローを実行してccflow4をccflow6にアップグレードします。このフローはwf_trackテーブルの問題を解決するだけです";
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
            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();

            string info = "";
            foreach (Flow fl in fls)
            {
                // 检查报表.
                Track.CreateOrRepairTrackTable(fl.No);

                // 查询.
                string sql = "SELECT * FROM WF_Track WHERE FK_Flow='" + fl.No + "'";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Track tk = new Track();
                    tk.FK_Flow = fl.No;
                    tk.Row.LoadDataTable(dt, dt.Rows[0]);
                    tk.DoInsert(0); // 执行insert.
                }
            }
            return  "実行は完了しました。再実行できません。そうすると、トレースデータが繰り返されます。";
        }
    }
}
