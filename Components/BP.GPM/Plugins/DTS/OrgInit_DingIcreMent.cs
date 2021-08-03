using System;
using System.Collections;
using System.Reflection;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.EAI.Plugins.DTS
{
    /// <summary>
    /// 钉钉组织结构增量同步
    /// </summary>
    public class OrgInit_DingIcreMent : Method
    {
        /// <summary>
        /// 钉钉组织结构增量同步
        /// </summary>
        public OrgInit_DingIcreMent()
        {
            this.Title = "増分ピン留めアドレス帳をCCGPMに同期する";
            this.Help = "DingTalkアドレス帳の増分同期には時間がかかります。しばらくお待ちください。<br> Dingding関連の構成はWeb.configに書き込まれ、構成が正しい場合にのみ実行できます";
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
                if (BP.GPM.Glo.IsEnable_DingDing == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            GPM.Emp emp = new GPM.Emp();
            emp.CheckPhysicsTable();

            DingDing ding = new DingDing();
            string result = ding.AnsyIncrementOrgToGPM();
            if (DataType.IsNullOrEmpty(result))
            {
                return "実行に成功しました。変更はありませんでした。";
            }
            else if (result.Contains("部門を取得する時エラーが発生しました"))
            {
                return result;
            }
            else if (result.Length > 0)
            {
                string webPath = "Log/Ding_GPM" + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
                string savePath = BP.Sys.SystemConfig.PathOfDataUser + webPath;

                BP.DA.Log log = new Log(savePath);
                log.WriteLine(result);
                return "正常に実行されました<a href =\"/DataUser/" + webPath + "\" target='_blank'>ログをダウンロード</a>";
            }
            else
                return "実行に失敗しました。";
        }
    }
}
