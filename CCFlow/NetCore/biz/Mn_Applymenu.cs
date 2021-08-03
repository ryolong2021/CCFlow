using BP.DA;
using System;
using System.Data;
using System.Text;

namespace BP.WF.HttpHandler
{
    public class Mn_Applymenu : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// すべてワークフローのNOを取得する
        /// </summary>
        /// <returns>すべてワークフローのNO</returns>
        public string GetFlowNoAll()
        {
            try
            {
                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // フローのNO
                sqlSb.Append("SELECT NO FROM WF_Flow ");

                // WHERE条件
                sqlSb.Append("WHERE No is not null AND FlowMark !='0'");

                // SQL実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString());

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// キーワードで表示されるメニューを取得する
        /// </summary>
        /// <returns>すべてワークフローのNO</returns>
        public string GetApplyMenuItemsByKeyword()
        {
            try
            {
                // キーワード分割
                StringBuilder keySb = new StringBuilder();
                string[] keywords = this.GetRequestVal("KeyWord").Split(" ");
                foreach (string key in keywords)
                {
                    keySb.Append("'");
                    keySb.Append(key);
                    keySb.Append("',");
                }
                keySb = keySb.Remove(keySb.ToString().LastIndexOf(','), 1);

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // フローのNO
                sqlSb.Append("SELECT WF_KEY_VALUE FROM WF_MN_KEY_REL ");

                // WHERE条件
                sqlSb.Append("WHERE WF_KEY_NAME IN (" + keySb.ToString() + ") AND WF_KEY_DEPT = '1'");

                // SQL実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString());

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
