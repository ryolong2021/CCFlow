using BP.DA;
using System;
using System.Data;
using System.Text;

namespace BP.WF.HttpHandler
{
    public class WF_Permission : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 再開許可情報取得用メソッド
        /// </summary>
        /// <returns></returns>
        public string GetPensionInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT STATUS, REC_ENT_DATE, REC_ENT_USER FROM MT_PENSION_REAPPLY WHERE EMPLOYEE_NO = @SHAINBANGO";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("SHAINBANGO", this.GetRequestVal("shainbango"));

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
        /// 許可メソッド
        /// </summary>
        /// <returns></returns>
        public string UpdatePensionInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "UPDATE MT_PENSION_REAPPLY SET STATUS = @STATUS, REC_EDT_DATE = @REC_EDT_DATE, REC_EDT_USER = @REC_EDT_USER WHERE EMPLOYEE_NO = @SHAINBANGO";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("STATUS", this.GetRequestVal("Status"));
                ps.Add("SHAINBANGO", this.GetRequestVal("shainbango"));
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_USER", this.GetRequestVal("shainbango"));

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
        ///再申請許可削除メソッド
        /// </summary>
        /// <returns></returns>
        public string DeletePensionInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "DELETE FROM MT_PENSION_REAPPLY WHERE EMPLOYEE_NO = @SHAINBANGO";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("SHAINBANGO", this.GetRequestVal("shainbango"));

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
        ///再申請許可挿入メソッド
        /// </summary>
        /// <returns></returns>
        public string InsertPensionInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "INSERT INTO MT_PENSION_REAPPLY(EMPLOYEE_NO, STATUS, REC_ENT_DATE, REC_ENT_USER, REC_EDT_DATE, REC_EDT_USER) VALUES(@SHAINBANGO, @STATUS, @REC_ENT_DATE, @REC_ENT_USER, @REC_EDT_DATE, @REC_EDT_USER)";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("SHAINBANGO", this.GetRequestVal("shainbango"));
                ps.Add("STATUS", this.GetRequestVal("Status"));
                ps.Add("REC_ENT_DATE", DateTime.Now.ToString());
                ps.Add("REC_ENT_USER", this.GetRequestVal("shainbango"));
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_USER", this.GetRequestVal("shainbango"));

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
    }
}
