using BP.DA;
using System;
using System.Data;
using System.Text;

namespace BP.WF.HttpHandler
{
    public class WF_RefundApply : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 還付金額合計保存メソッド
        /// </summary>
        /// <returns></returns>
        public string UpdateRefundAmountInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "UPDATE TT_WF_SPEC_REFUND_APPLY SET REFUND_AMOUNT = @REFUND_AMOUNT, REC_EDT_DATE = @REC_EDT_DATE, REC_EDT_USER = @REC_EDT_USER WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("REFUND_AMOUNT", this.GetRequestVal("refund_amount"));
                ps.Add("OID", this.GetRequestVal("oid"));
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
        ///経費分担明細削除メソッド
        /// </summary>
        /// <returns></returns>
        public string DeleteRefundDetailsInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "DELETE FROM TT_WF_SPEC_DISCOUNT_REFUND WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("oid"));

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
        /// 経費分担明細保存メソッド
        /// </summary>
        /// <returns></returns>
        public string InsertRefundDetailsInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "INSERT INTO TT_WF_SPEC_DISCOUNT_REFUND(OID, DET_NO, CORP_CODE, STROE_NAME, AMOUNT, HAS_WAON_PT, REFUND, REC_ENT_DATE, REC_ENT_USER, REC_EDT_DATE, REC_EDT_USER) VALUES(@OID, @DET_NO, @CORP_CODE, @STROE_NAME, @AMOUNT, @HAS_WAON_PT, @REFUND, @REC_ENT_DATE, @REC_ENT_USER, @REC_EDT_DATE, @REC_EDT_USER)";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("oid"));
                ps.Add("DET_NO", this.GetRequestVal("det_no"));
                ps.Add("CORP_CODE", this.GetRequestVal("corp_code"));
                ps.Add("STROE_NAME", this.GetRequestVal("stroe_name"));
                ps.Add("AMOUNT", this.GetRequestVal("amount"));
                ps.Add("HAS_WAON_PT", this.GetRequestVal("has_waon_pt"));
                ps.Add("REFUND", this.GetRequestVal("refund"));
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

        /// <summary>
        /// 部署管理者権限マスタの社員番号取得メソッド
        /// </summary>
        /// <returns></returns>
        public string GetEmployeeNoInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT EMPLOYEE_NO FROM MT_DEPT_MANAGER WHERE CORP_CODE = @CORP_CODE AND GROUP_KBN IN ('G1001') AND ROLE_CODE IN ('B')";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("CORP_CODE", this.GetRequestVal("corp_code"));

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
        /// 特別割引制度レシート明細情報取得メソッド
        /// </summary>
        /// <returns></returns>
        public string GetRefundInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT DET_NO, CORP_CODE, STROE_NAME, AMOUNT, HAS_WAON_PT, REFUND FROM TT_WF_SPEC_DISCOUNT_REFUND WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("oid"));

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
        /// 還付金申請トランザクション情報取得メソッド
        /// </summary>
        /// <returns></returns>
        public string GetRefundApplyInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT REFUND_AMOUNT FROM TT_WF_SPEC_REFUND_APPLY WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("oid"));

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
        /// 還特別割引制度レシート明細情報取得メソッド
        /// </summary>
        /// <returns></returns>
        public string GetReceiptInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT RECIEPT_AMOUNT, UPLOADE_FILE_1 FROM TT_WF_SPEC_DISCOUNT_RECEIPT WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("oid"));

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
