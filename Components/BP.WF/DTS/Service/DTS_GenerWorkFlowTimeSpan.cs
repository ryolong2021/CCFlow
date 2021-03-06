using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 同步待办时间戳 的摘要说明
    /// </summary>
    public class DTS_GenerWorkFlowTimeSpan : Method
    {
        /// <summary>
        /// 同步待办时间戳
        /// </summary>
        public DTS_GenerWorkFlowTimeSpan()
        {
            this.Title = "処理待ちタイムスタンプ、ステータス、フロー登録テーブルの期間を同期します（今週、先週、2週間前、その他3つ）。";
            this.Help = "このメソッドは毎週月曜日に自動的に実行されます。自動で実行できない場合は、手動で実行してください。";
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

            //只能在周1执行.
            DateTime dtNow = DateTime.Now;

            //设置为开始的日期为周1.
            DateTime dtBegin = DateTime.Now;

            dtBegin = dtBegin.AddDays(-7);
            for (int i = 0; i < 8; i++)
            {
                if (dtBegin.DayOfWeek == DayOfWeek.Monday)
                    break;
                dtBegin = dtBegin.AddDays(-1);
            }

            //结束日期为当前.
            DateTime dtEnd = dtBegin.AddDays(7);

            //默认都设置为本周
            string sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.ThisWeek;
            BP.DA.DBAccess.RunSQL(sql);

            //设置为上周.
            sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.NextWeek + " WHERE RDT >= '" + dtBegin.ToString(DataType.SysDataFormat) + " 00:00' AND RDT <= '" + dtEnd.ToString(DataType.SysDataFormat) + " 00:00'";
            BP.DA.DBAccess.RunSQL(sql);

            dtBegin = dtBegin.AddDays(-7);
            dtEnd = dtEnd.AddDays(-7);

            //把上周的，设置为两个周以前.
            sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.TowWeekAgo + " WHERE RDT >= '" + dtBegin.ToString(DataType.SysDataFormat) + " 00:00' AND RDT <= '" + dtEnd.ToString(DataType.SysDataFormat) + " 00:00' ";
            BP.DA.DBAccess.RunSQL(sql);

            //把上周的，设置为更早.
            sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.More + " WHERE RDT <= '" + dtBegin.ToString(DataType.SysDataFormat) + " 00:00' ";
            BP.DA.DBAccess.RunSQL(sql);

            return "正常に実行しました。";
        }
    }
}
