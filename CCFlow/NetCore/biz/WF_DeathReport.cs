using BP.DA;
using System;
using System.Data;

namespace BP.WF.HttpHandler
{
    public class WF_DeathReport: BP.WF.HttpHandler.DirectoryPageBase
    {

        WF_AppForm wf_appfrom = new WF_AppForm();
        /// <summary>
        /// 家族情報_死亡届トランザクションテーブルデータの取得
        /// </summary>
        /// <returns>取得したデータリスト</returns>
        public string GetDeathInfo()
        {
            try
            {
                // Sql文と条件設定の取得
                string sql = "SELECT * FROM TT_WF_DEATH WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("WorkID"));

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }


        /// <summary>
        /// 家族情報の取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetFamilyInfo()
        {
            try
            {
                string shainbango = this.GetRequestVal("UserNo");

                // Sql文と条件設定の取得
                string sql = "SELECT * FROM MT_EBS_EMPLOYEE_FAMILY_INFO WHERE EBS_SHAINBANGO = @SHAINBANGO AND FAMILY_RELATIONSHIP <> @RELATIONSHIP AND (FAMILY_DAY_OF_DEATH IS NULL OR TRIM(FAMILY_DAY_OF_DEATH) = '') ORDER BY FAMILY_RELATIONSHIP";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("SHAINBANGO", shainbango);
                ps.Add("RELATIONSHIP", this.GetRequestVal("relationship"));

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);
                if (dt.Rows.Count == 0)
                {
                    string[] list = { "家族情報", "死亡届" };
                    return "msg@" + BP.Tools.Json.ToJson(wf_appfrom.SetRtnMessage("I0037", list));
                }

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
    }
}
