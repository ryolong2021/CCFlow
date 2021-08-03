using BP.DA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zipangu;

/// <summary>
/// 介護勤務・休職申請_承認一覧
/// </summary>
namespace BP.WF.HttpHandler
{
    public class Mn_Nursing_Work_layoff_List : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// 介護関連データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetNursingWorklayoffList()
        {
            try
            {
                // 検索条件の取得
                CondInfo cond =JsonConvert.DeserializeObject<CondInfo>(this.GetRequestVal("CondInfo"));

                // Sql文と条件設定の取得
                GetListSql(cond, out string sql, out Paras ps);

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
        /// 検索用sqlの作成
        /// </summary>
        /// <param name="searchCond">画面入力の検索条件</param>
        /// <param name="sql">画面表示に合わせて、検索用SQL</param>
        /// <param name="ps">実行SQLの時に、必要のパラメータ</param>
        private void GetListSql(CondInfo searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成

            // 伝票番号
            sqlSb.Append("SELECT NURSING.OID AS WorkID,");
            // 介護勤務・休職区分
            sqlSb.Append("       NusingKbn.KBNNAME AS NusingKbn,");
            // 申請区分
            sqlSb.Append("       SinseiKbn.KBNNAME AS SinseiKbn,");
            // 社員番号
            sqlSb.Append("       NURSING.EMPLOYEE_NO AS EmployeeNo,");
            // 会社
            sqlSb.Append("       NURSING.COMPANY_NAME AS CompanyNm,");
            // 氏名
            sqlSb.Append("       NURSING.EMPLOYEE_KANZI_NAME AS EmployeeName,");
            // 申請日
            sqlSb.Append("       FORMAT(CONVERT(date, NURSING.NURSEING_WORK_APPLY_DATE),'yyyy年MM月dd日') AS AppDate,");
            // ステータス
            sqlSb.Append("       StatusKbn.KBNNAME AS StatusKbn,");
            // 承認日
            sqlSb.Append("       FORMAT(CONVERT(date, NURSING.REC_EDT_DATE),'yyyy年MM月dd日') AS WeddingDate");

            // テーブル名
            sqlSb.Append("  FROM V_Nursing_Work_layoff_List NURSING");
            // 区分マスターの介護勤務・休職区分
            sqlSb.Append("  left join MT_KBN  NusingKbn on NusingKbn.KBNCODE = 'NURSING_WORK_FINISH_KBN' AND NusingKbn.KBNVALUE = NURSING.NUSINGKBN");
            // 区分マスターの申請区分
            sqlSb.Append("  left join MT_KBN  SinseiKbn on SinseiKbn.KBNCODE = 'NURSING_SINSEI_KBN' AND SinseiKbn.KBNVALUE = NURSING.NURSEING_WORK_KBN");
            // 区分マスターのステータス
            sqlSb.Append("  left join MT_KBN  StatusKbn on StatusKbn.KBNCODE = 'NURSING_STATUS_KBN' AND StatusKbn.KBNVALUE = NURSING.WFState");

            // 条件
            sqlSb.Append("    WHERE 1 = 1 ");

            // 検索条件の追加
            this.GetSearchCondSql(searchCond, out string sqlCond, out ps);

            // 条件の追加
            sqlSb.Append(sqlCond);

            // sql文の設定
            sql = sqlSb.ToString();
        }


        /// <summary>
        /// DBに検索する条件sqlの作成
        /// </summary>
        /// <param name="searchCond">検索条件</param>
        /// <param name="sql">作成後SQL</param>
        /// <param name="ps">検索の時必要条件</param>
        private void GetSearchCondSql(CondInfo searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlCond = new StringBuilder();

            // パラメータの作成
            ps = new Paras();

            // 画面入力により、条件を作ること
            // 社員番号 社員番号が入力されたら、他の条件が無視にします。
            if (!string.IsNullOrEmpty(searchCond.EmployeeNo))
            {
                // 社員番号の設定
                sqlCond.Append("    AND NURSING.EMPLOYEE_NO = @EMPLOYEE_NO ");
                // パラメータの設定
                ps.Add("EMPLOYEE_NO", searchCond.EmployeeNo);
            }
            else
            {
                // 会社コード
                if (!string.IsNullOrEmpty(searchCond.CompanyCd))
                {
                    sqlCond.Append("    AND NURSING.COMPANY_CODE = @COMPANY_CODE ");

                    ps.Add("COMPANY_CODE", searchCond.CompanyCd);
                }

                // 会社名
                if (!string.IsNullOrEmpty(searchCond.CompanyNm))
                {
                    sqlCond.Append("    AND NURSING.COMPANY_NAME LIKE @CompanyNm ");

                    ps.Add("CompanyNm", "%" + searchCond.CompanyNm + "%");
                }

                // 会社コード＋会社名を空欄で検索の場合は、受託会社全部を検索する
                if (string.IsNullOrEmpty(searchCond.CompanyCd) && string.IsNullOrEmpty(searchCond.CompanyNm))
                {
                    // 受託している会社の情報のみに絞り込みを行う
                    sqlCond.Append("    AND NURSING.COMPANY_CODE IN ( ");
                    sqlCond.Append("            SELECT CompanyAcceptance.CORP_CODE");
                    sqlCond.Append("              FROM MT_COMPANYACCEPTANCE CompanyAcceptance");
                    sqlCond.Append("              INNER JOIN MT_BUSI_WF_REL B");
                    sqlCond.Append("                    ON B.BUSINESS_CODE = CompanyAcceptance.BUSINESS_CODE");
                    sqlCond.Append("            WHERE  CompanyAcceptance.ENTRUSTED_FLG = 'Y' ");
                    sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                    sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                    sqlCond.Append("                   AND (CompanyAcceptance.DELETE_FLG <> 'X' OR CompanyAcceptance.DELETE_FLG IS NULL)");
                    sqlCond.Append("                   AND B.WF_NO in (" + searchCond.WfNo + ") ");
                    sqlCond.Append("                   AND B.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                    sqlCond.Append("                   AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                    sqlCond.Append("                   AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL)");
                    sqlCond.Append("            )");
                }

                // 伝票番号
                if (!string.IsNullOrEmpty(searchCond.WorkID))
                {
                    sqlCond.Append("    AND NURSING.OID = @OID");
                    ps.Add("OID", searchCond.WorkID);
                }

                // 介護勤務・休職区分
                if (!string.IsNullOrEmpty(searchCond.NusingKbn))
                {
                    sqlCond.Append("    AND NURSING.NUSINGKBN = @NUSINGKBN");
                    ps.Add("NUSINGKBN", searchCond.NusingKbn);
                }

                // 申請区分
                if (!string.IsNullOrEmpty(searchCond.SinseiKbn))
                {
                    sqlCond.Append("    AND NURSING.NURSEING_WORK_KBN = @NURSEING_WORK_KBN");
                    ps.Add("NURSEING_WORK_KBN", searchCond.SinseiKbn);
                }

                // ステータス
                if (!string.IsNullOrEmpty(searchCond.StatusKbn))
                {
                    sqlCond.Append("    AND NURSING.WFState = @WFState");
                    ps.Add("WFState", searchCond.StatusKbn);
                }

                // 申請日
                // Fromのみがある場合
                if (!string.IsNullOrEmpty(searchCond.AppDateFrom))
                {
                    // 年月日の完全一致検索
                    sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8),NURSING.NURSEING_WORK_APPLY_DATE,112)");

                    // 入力条件
                    ps.Add("AppDateFrom", searchCond.AppDateFrom);
                }

                // Toのみがある場合
                if (!string.IsNullOrEmpty(searchCond.AppDateTo))
                {
                    // 年月日の完全一致検索
                    sqlCond.Append("    AND CONVERT(nvarchar(8),NURSING.NURSEING_WORK_APPLY_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");

                    // 入力条件
                    ps.Add("AppDateTo", searchCond.AppDateTo);
                }
            }

            // 条件の取得
            sql = sqlCond.ToString();
        }

        /// <summary>
        /// 一覧検索条件クラス
        /// </summary>
        private class CondInfo
        {
            /// <summary>
            /// ワークフローID
            /// </summary>
            public string WfNo { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            public string CompanyCd { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            public string CompanyNm { get; set; }

            /// <summary>
            /// 従業員番号
            /// </summary>
            public string EmployeeNo { get; set; }

            /// <summary>
            /// 伝票番号
            /// </summary>
            public string WorkID { get; set; }

            /// <summary>
            /// 申請FROM
            /// </summary>
            public string AppDateFrom { get; set; }

            /// <summary>
            /// 申請TO
            /// </summary>
            public string AppDateTo { get; set; }

            /// <summary>
            /// 介護勤務・休職区分
            /// </summary>
            public string NusingKbn { get; set; }

            /// <summary>
            /// 申請区分
            /// </summary>
            public string SinseiKbn { get; set; }

            /// <summary>
            /// ステータス
            /// </summary>
            public string StatusKbn { get; set; }
        }
    }
}
