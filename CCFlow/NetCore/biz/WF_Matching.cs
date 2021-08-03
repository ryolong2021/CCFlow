using BP.DA;
using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace BP.WF.HttpHandler
{
    public class WF_Matching: BP.WF.HttpHandler.DirectoryPageBase
    {

        WF_AppForm wf_appfrom = new WF_AppForm();

        /// <summary>
        /// 確定拠出年金_申請・連携管理情報の取得
        /// </summary>
        /// <returns>取得したデータリスト</returns>
        public string GetSinseiKikan()
        {
            try
            {
                DateTime today = DateTime.Now;
                string todayStr = today.ToString("yyyyMMdd");
                string year = today.Year.ToString();

                // Sql文と条件設定の取得
                StringBuilder sqlSb = new StringBuilder();

                // テーブル「確定拠出年金_申請・連携管理」から、申請可能期間とか締切日とか取得する
                sqlSb.Append("     SELECT MT01.*, '");
                sqlSb.Append(year);
                sqlSb.Append("' + RIGHT('00' + CONVERT(NVARCHAR, MT01.NEW1_APPLY_MONTH), 2) AS NEW1_APPLY_MONTH_2, '");
                sqlSb.Append(year);
                sqlSb.Append("' + RIGHT('00' + CONVERT(NVARCHAR, MT01.NEW2_APPLY_MONTH), 2) AS NEW2_APPLY_MONTH_2, '");
                sqlSb.Append(year);
                sqlSb.Append("' + RIGHT('00' + CONVERT(NVARCHAR, MT01.MOD_APPLY_MONTH), 2) AS MOD_APPLY_MONTH_2, ");
                sqlSb.Append("CONVERT(nvarchar, ");
                sqlSb.Append(todayStr);
                sqlSb.Append(") AS TODAY, ");
                sqlSb.Append(" CONVERT(nvarchar, '') AS INDEX_FOR_NEW ");
                sqlSb.Append("       FROM MT_PENSION_APPLY_MANAGE MT01 ");
                sqlSb.Append("      WHERE MT01.CORP_CODE = @KAISHACODE");

                Paras ps = new Paras();
                // 入力条件
                ps.Add("KAISHACODE", this.GetRequestVal("KaishaCode"));

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

                if (dt.Rows.Count == 0)
                {
                    string[] str = { };
                    return "msg@" + BP.Tools.Json.ToJson(wf_appfrom.SetRtnMessage("E0052", str));
                }

                string type = this.GetRequestVal("Type");

                // 中断申請と再開申請の場合、検索結果をリターンする
                if (!"1".Equals(type) && !"2".Equals(type))
                {
                    return BP.Tools.Json.ToJson(dt);
                }

                // 新規申請と変更申請の場合、チェックを行って検索結果をリターンする
                var rowData = dt.Rows[0];
                List<string> ymStart = new List<string>();
                List<string> ymEnd = new List<string>();
                DateTime dateTimeStart;
                DateTime dateTimeEnd;

                if ("1".Equals(type))
                {
                    ymStart.Add(year + rowData["NEW1_APPLY_FROM_MD"].ToString());
                    ymEnd.Add(year + rowData["NEW1_APPLY_TO_MD"].ToString());
                    ymStart.Add(year + rowData["NEW2_APPLY_FROM_MD"].ToString());
                    ymEnd.Add(year + rowData["NEW2_APPLY_TO_MD"].ToString());
                }
                else
                {
                    ymStart.Add(year + rowData["MOD_APPLY_FROM_MD"].ToString());
                    ymEnd.Add(year + rowData["MOD_APPLY_TO_MD"].ToString());
                }

                for (var i = 0; i < ymStart.Count; i++)
                {

                    if (DateTime.TryParseExact(ymStart[i], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dateTimeStart) == false
                    || DateTime.TryParseExact(ymEnd[i], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dateTimeEnd) == false)
                    {
                        return "err@" + "申請可能期間不正";
                    }

                    //操作日は申請可能期間にあるかどうかのチェック
                    if (todayStr.CompareTo(ymStart[i]) >= 0 && todayStr.CompareTo(ymEnd[i]) <= 0)
                    {
                        rowData.SetField("INDEX_FOR_NEW", i+1);
                        break;
                    }
                }

                DataTable dtReturn = dt.Clone();
                dtReturn.ImportRow(rowData);
                return BP.Tools.Json.ToJson(dtReturn);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 再開申請許可状態取得用メソッド
        /// </summary>
        /// <returns></returns>
        public string GetPensionInfo()
        {
            try
            {
                string shainbango = this.GetRequestVal("UserNo");

                // Sql文と条件設定の取得
                string sql = "SELECT STATUS FROM MT_PENSION_REAPPLY WHERE EMPLOYEE_NO = @SHAINBANGO";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("SHAINBANGO", shainbango);

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
        /// 再開申請許可状態更新用メソッド
        /// </summary>
        /// <returns></returns>
        public string UpdatePensionInfo()
        {
            int result = -1;
            try
            {
                Paras ps = new Paras();

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append("UPDATE MT_PENSION_REAPPLY SET STATUS = @STATUS, REC_EDT_USER = @REC_EDT_USER, REC_EDT_DATE = @REC_EDT_DATE WHERE EMPLOYEE_NO = @SHAINBANGO ");

                if (!string.IsNullOrEmpty(this.GetRequestVal("StatusPossible")))
                {
                    // 部分一致検索 連絡先名
                    sqlSb.Append(" AND STATUS = @STATUSPOSSIBLE");

                    // 入力条件
                    ps.Add("STATUSPOSSIBLE", this.GetRequestVal("StatusPossible"));
                }

                // 入力条件
                ps.Add("STATUS", this.GetRequestVal("Status"));
                ps.Add("SHAINBANGO", this.GetRequestVal("UserNo"));
                ps.Add("REC_EDT_USER", this.GetRequestVal("UserNo"));
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());

                // Sqlの実行
                result = BP.DA.DBAccess.RunSQL(sqlSb.ToString(), ps);
                if (result < 1)
                {
                    return "err@" + "更新に失敗しました。";
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// 確定拠出年金加入者拠出金額マスタから加入者拠出金額リストを取得する
        /// </summary>
        /// <returns></returns>
        public string GetPensionSubscriberList()
        {
            try
            {
                // Sql文と条件設定の取得
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT PENSION_AMT_EMPLOYEE ");
                sb.Append("  FROM MT_PENSION_SUBSCRIBER ");
                sb.Append(" WHERE CORP_CODE = @CorpCode ");
                sb.Append("   AND PENSION_AMT_COMPANY = @PensionAmtCompany ");
                sb.Append("   AND (DELETE_FLG <> 'X' OR DELETE_FLG IS NULL) ");
                sb.Append("   AND TEKIYOYMD_FROM <= CONVERT(varchar, GETDATE(), 112) ");
                sb.Append("   AND TEKIYOYMD_TO >= CONVERT(varchar, GETDATE(), 112) ");
                sb.Append(" ORDER BY PENSION_AMT_EMPLOYEE ");


                Paras ps = new Paras();
                // 入力条件
                ps.Add("CorpCode", this.GetRequestVal("CorpCode"));
                ps.Add("PensionAmtCompany", this.GetRequestVal("PensionAmtCompany"));

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sb.ToString(), ps);

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
