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
/// 資格免許登録照会一覧
/// </summary>
namespace BP.WF.HttpHandler
{
    public class Mn_LicenseList : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// 全角かなから半角かなに変更する項目名の設定
        /// ※固定に設定すること
        /// 　設定のやり方は検索のSQL文のASの別名を合わせないといけない
        /// 　GetCsvDetailDataSqlメソッドのSQL文
        /// </summary>
        public string[] KanaChg = {
            // 氏名（ｶﾅ）
            //"SeimeiKana",

        };

        /// <summary>
        /// 半角英数字から全角英数字に変更する設定
        /// ※固定に設定すること
        /// 　設定のやり方は検索のSQL文のASの別名を合わせないといけない
        /// 　GetCsvDetailDataSqlメソッドのSQL文
        /// </summary>
        public string[] NumberChg = {
            // 通夜会場名
            //"TsuyaBasyouMei",

        };

        /// <summary>
        /// 確認済みメソッド
        /// </summary>
        /// <returns></returns>
        public string UpdateLicenseInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "UPDATE TT_WF_CERTIFICATION_REGI SET " +
                    "CHECK_FLG=1, " +
                    "CHECK_CORP_CODE=@CHECK_CORP_CODE, " +
                    "CHECK_CORP_NAME=@CHECK_CORP_NAME, " +
                    "CHECK_DEPT_CODE=@CHECK_DEPT_CODE, " +
                    "CHECK_DEPT_NAME=@CHECK_DEPT_NAME, " +
                    "CHECK_EMP_NUM=@CHECK_EMP_NUM, " +
                    "CHECK_LNAME=@CHECK_LNAME, " +
                    "CHECK_FNAME=@CHECK_FNAME, " +
                    "CHECK_DATE=@CHECK_DATE, " +
                    "REC_EDT_DATE=@REC_EDT_DATE, " +
                    "REC_EDT_USER=@REC_EDT_USER " +
                    "WHERE OID=@OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("CHECK_CORP_CODE", this.GetRequestVal("CHECK_CORP_CODE"));
                ps.Add("CHECK_CORP_NAME", this.GetRequestVal("CHECK_CORP_NAME"));
                ps.Add("CHECK_DEPT_CODE", this.GetRequestVal("CHECK_DEPT_CODE"));
                ps.Add("CHECK_DEPT_NAME", this.GetRequestVal("CHECK_DEPT_NAME"));
                ps.Add("CHECK_EMP_NUM", this.GetRequestVal("CHECK_EMP_NUM"));
                ps.Add("CHECK_LNAME", this.GetRequestVal("CHECK_LNAME"));
                ps.Add("CHECK_FNAME", this.GetRequestVal("CHECK_FNAME"));
                ps.Add("CHECK_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_USER", this.GetRequestVal("CHECK_EMP_NUM"));
                ps.Add("OID", this.GetRequestVal("OID"));

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
        /// 詳細画面に遷移する時に、表示データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetDetailData()
        {
            try
            {
                // 検索条件の取得
                string workId = this.GetRequestVal("WorkId");

                // Sql文と条件設定の取得
                GetDetailDataSql(workId, out string sql, out Paras ps);

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
        /// <param name="workId">伝票番号</param>
        /// <param name="sql">画面表示に合わせて、検索用SQL</param>
        /// <param name="ps">実行SQLの時に、必要のパラメータ</param>
        private void GetDetailDataSql(string workId, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();
            // パラメータの作成
            ps = new Paras();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT License.OID AS oid,");
            // --確認
            sqlSb.Append("  License.CHECK_FLG,chkKbn.KBNNAME AS CheckKbn_confirm,");
            // --申請状態
            sqlSb.Append("  License.SHINSEISYAKBN,cerKbn.KBNNAME AS ApplicationKbn_confirm,");
            // --確認日時
            sqlSb.Append("  FORMAT(License.CHECK_DATE,'yyyy年MM月dd日 HH時mm分') AS ChkDate_confirm, ");
            // --確認者_会社
            sqlSb.Append("  License.CHECK_CORP_CODE+'　'+License.CHECK_CORP_NAME AS ChkCorpName_confirm,");
            // --確認者人事所属
            sqlSb.Append("  License.CHECK_DEPT_CODE AS ChkDepartmentCode_confirm,");
            sqlSb.Append("  License.CHECK_DEPT_NAME AS ChkDepartmentName_confirm,");
            // --確認者_社員番号
            sqlSb.Append("  License.CHECK_EMP_NUM AS ChkEmployeeNo_confirm,");
            // --確認者_姓
            sqlSb.Append("  License.CHECK_LNAME AS ChkEmployeeLName_confirm,");
            // --確認者_名
            sqlSb.Append("  License.CHECK_FNAME AS ChkEmployeeFName_confirm,");

            // --申請会社
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_CORP_CODE+'　'+AGENT_CORP_NAME  ELSE APPLICANT_CORP_CODE+'　'+APPLICANT_CORP_NAME  END AS company_confirm,");
            // --申請日
            sqlSb.Append("  FORMAT(License.REC_ENT_DATE,'yyyy年MM月dd日') AS appdate_confirm,");
            // --申請人事所属
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_DEPT_CODE  ELSE APPLICANT_DEPT_CODE  END AS sozoku_code_confirm,");
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_DEPT_NAME  ELSE APPLICANT_DEPT_NAME  END AS sozoku_name_confirm,");
            // --申請社員番号
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_EMP_NUM  ELSE APPLICANT_EMP_NUM  END AS kaisyano_confirm,");
            // --申請氏名_姓
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_LNAME  ELSE APPLICANT_LNAME  END AS kanji_sei_confirm,");
            // --申請氏名_名
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_FNAME  ELSE APPLICANT_FNAME  END AS kanji_mei_confirm,");

            // --会社
            sqlSb.Append("  License.APPLICANT_CORP_CODE+'　'+APPLICANT_CORP_NAME AS hidairi_company_confirm,");
            // --人事所属
            sqlSb.Append("  License.APPLICANT_DEPT_CODE AS hidairi_sozoku_code_confirm,");
            sqlSb.Append("License.APPLICANT_DEPT_NAME AS hidairi_sozoku_name_confirm,");
            // --社員番号
            sqlSb.Append("  License.APPLICANT_EMP_NUM AS hidairi_kaisyano_confirm,");
            // --氏名_姓
            sqlSb.Append("  License.APPLICANT_LNAME AS hidairi_kanji_sei_confirm,");
            // --氏名_名
            sqlSb.Append("License.APPLICANT_FNAME AS hidairi_kanji_mei_confirm,");

            // --資格
            sqlSb.Append("  License.CERTIFICATION_NO+'　'+License.CERTIFICATION_NAME AS sikaku_mei_confirm,");
            // --取得日
            sqlSb.Append("  CASE WHEN License.ACQUISITION_DATE IS NULL THEN ''  ELSE SUBSTRING(License.ACQUISITION_DATE,1,4)+'年'+SUBSTRING(License.ACQUISITION_DATE,5,2)+'月'+SUBSTRING(License.ACQUISITION_DATE,7,2)+'日' END AS sikaku_syutoku_date_confirm,");
            // --失効日
            sqlSb.Append("  CASE WHEN License.EXPIRY_DATE IS NULL THEN ''  ELSE SUBSTRING(License.EXPIRY_DATE,1,4)+'年'+SUBSTRING(License.EXPIRY_DATE,5,2)+'月'+SUBSTRING(License.EXPIRY_DATE,7,2)+'日' END AS sikaku_sikkou_date_confirm,");
            // --資格_点数
            sqlSb.Append("  License.SCORE tensu_confirm,");
            // --資格_資格証明書
            sqlSb.Append("  License.UPLOADE_FILE_1 AS UPLOADE_FILE_1,");
            // --備考
            sqlSb.Append("  License.REMARKS AS bikou_confirm,");
            // --保有車両区分
            sqlSb.Append("  License.CAR_CATEGORY+'　'+License.CAR_CATEGORY_SUMMARY AS hoyu_syaryo_kbn_confirm");

            // テーブル名
            sqlSb.Append("  FROM TT_WF_CERTIFICATION_REGI License");
            // 関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN cerKbn ON cerKbn.KBNCODE = 'CERTIFICATION_REGI_CLASS' AND cerKbn.KBNVALUE = CONVERT(varchar, License.SHINSEISYAKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN chkKbn ON chkKbn.KBNCODE = 'CHECK_CODE' AND chkKbn.KBNVALUE = CONVERT(varchar, License.CHECK_FLG)");

            // 条件
            sqlSb.Append("    WHERE License.OID = @WorkID ");
            // パラメータの設定
            ps.Add("WorkID", workId);

            // sql文の設定
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetLicenseList()
        {
            try
            {
                // 検索条件の取得
                CondInfo cond =
                    JsonConvert.DeserializeObject<CondInfo>(
                        this.GetRequestVal("CondInfo"));

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
            sqlSb.Append("SELECT License.OID AS WorkID,");
            //--申請状態
            sqlSb.Append("  License.SHINSEISYAKBN,cerKbn.KBNNAME AS ApplicationKbn,");
            //確認
            sqlSb.Append("  License.CHECK_FLG,chkKbn.KBNNAME AS CheckKbn,");
            //会社
            sqlSb.Append("  License.APPLICANT_CORP_CODE + '　' + License.APPLICANT_CORP_NAME AS CorpName,");
            //人事所属
            sqlSb.Append("  License.APPLICANT_DEPT_NAME AS DepartmentName,");
            //社員番号
            sqlSb.Append("  License.APPLICANT_EMP_NUM AS EmployeeNo,");
            //氏名
            sqlSb.Append("  License.APPLICANT_LNAME + '　' + License.APPLICANT_FNAME AS EmployeeName,");
            //資格コード
            sqlSb.Append("  License.CERTIFICATION_NO AS CertificationNo,");
            //資格
            sqlSb.Append("  License.CERTIFICATION_NAME AS CertificationName,");
            //取得日
            sqlSb.Append("  CASE WHEN License.ACQUISITION_DATE IS NULL THEN ''  ELSE SUBSTRING(License.ACQUISITION_DATE,1,4)+'/'+SUBSTRING(License.ACQUISITION_DATE,5,2)+'/'+SUBSTRING(License.ACQUISITION_DATE,7,2) END AS AcquisitionDate,");
            //失効日
            sqlSb.Append("  CASE WHEN License.EXPIRY_DATE IS NULL THEN ''  ELSE SUBSTRING(License.EXPIRY_DATE,1,4)+'/'+SUBSTRING(License.EXPIRY_DATE,5,2)+'/'+SUBSTRING(License.EXPIRY_DATE,7,2) END AS ExpiryDate,");
            //申請日
            sqlSb.Append("  FORMAT(License.REC_ENT_DATE,'yyyy/MM/dd') AS AppDate");

            // テーブル名
            sqlSb.Append("  FROM TT_WF_CERTIFICATION_REGI License");

            // 関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN cerKbn ON cerKbn.KBNCODE = 'CERTIFICATION_REGI_CLASS' AND cerKbn.KBNVALUE = CONVERT(varchar, License.SHINSEISYAKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN chkKbn ON chkKbn.KBNCODE = 'CHECK_CODE' AND chkKbn.KBNVALUE = CONVERT(varchar, License.CHECK_FLG)");

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
        /// CSVデータの取得
        /// </summary>
        /// <returns>CSVデータ</returns>
        public string GetCsvDataInfo()
        {
            try
            {
                // 検索条件の取得
                CondInfo cond =
                    JsonConvert.DeserializeObject<CondInfo>(
                        this.GetRequestVal("CondInfo"));

                // Sql文と条件設定の取得
                this.GetCsvDetailDataSql(cond, out string sql, out Paras ps);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                // フロントに戻ること
                return this.MakeCsvDataInfo(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex;
            }
        }

        /// <summary>
        /// CSVデータの作成
        /// </summary>
        /// <param name="dt">DBから抽出データ</param>
        /// <returns>出力CSVデータ</returns>
        private string MakeCsvDataInfo(DataTable dt)
        {
            // csv文対象の作成
            StringBuilder strCsv = new StringBuilder();

            // タイトルの追加
            strCsv.Append(this.GetCsvTilie());

            //DataRowsに格納してからデータを取得する⑤
            foreach (DataRow row in dt.Rows)
            {
                // 改行の追加
                strCsv.Append(Environment.NewLine);

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    // コンマの追加
                    if (i > 0)
                    {
                        strCsv.Append(",");
                    }

                    // 特別文字列の変更
                    string csvVal = ConvertStr(row.Table.Columns[i].ColumnName, row[i].ToString());

                    strCsv.Append(string.Format("\"{0}\"", csvVal));
                }
            }

            return strCsv.ToString();
        }

        /// <summary>
        /// 1.特別文字列の切り替え※「"」
        /// 2.全角カナから半角かなに変更すること
        /// 3.半角英数字から全角英数字に変更すること
        /// </summary>
        /// <param name="strBef">変更文字列</param>
        /// <returns>出力CSVデータ</returns>
        private string ConvertStr(string key, string val)
        {
            // 戻り対応の作成
            string strAft = string.Empty;

            // 「"」のエスケープ
            strAft = val.Replace("\"", "\"\"");

            // 全角カナから半角かなに変更すること
            if (KanaChg.Contains(key))
            {
                strAft = strAft.KatakanaToHalfKatakana();
            }

            // 半角英数字から全角英数字に変更すること
            if (NumberChg.Contains(key))
            {
                Regex re = new Regex("[0-9A-Za-z]+");

                List<Match> matchRows = re.Matches(strAft).ToList();

                foreach (Match row in matchRows)
                {
                    strAft = strAft.Replace(row.Value, row.Value.Convert(KanaConv.None, AsciiConv.ToWide));
                }
            }

            return strAft.ToString();
        }

        /// <summary>
        /// CSVデータ検索用sqlの作成
        /// </summary>
        /// <param name="searchCond">検索条件</param>
        /// <param name="sql">作成後SQL</param>
        /// <param name="ps">検索の時必要条件</param>
        private void GetCsvDetailDataSql(CondInfo searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成

            // --伝票番号
            sqlSb.Append("SELECT License.OID AS WorkId,");
            // --申請日
            sqlSb.Append("  FORMAT(License.REC_ENT_DATE,'yyyy/MM/dd') AS AppDate,");
            // --申請状態
            sqlSb.Append("  License.SHINSEISYAKBN AS ApplicationKbn,");
            // --会社
            sqlSb.Append("  License.APPLICANT_CORP_CODE AS CorpCode,");
            sqlSb.Append("  License.APPLICANT_CORP_NAME AS CorpName,");

            // --人事所属
            sqlSb.Append("  License.APPLICANT_DEPT_CODE AS DepartmentCode,");
            sqlSb.Append("  License.APPLICANT_DEPT_NAME AS DepartmentName,");
            // 社員番号
            sqlSb.Append("  License.APPLICANT_EMP_NUM AS EmployeeNo,");
            // --氏名
            sqlSb.Append("  License.APPLICANT_LNAME AS EmployeeLName,");
            sqlSb.Append("  License.APPLICANT_FNAME AS EmployeeFName,");
            // --申請会社
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_CORP_CODE  ELSE APPLICANT_CORP_CODE  END AS AppCorpCode,");
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_CORP_NAME  ELSE APPLICANT_CORP_NAME  END AS AppCorpName,");
            // --申請人事所属
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_DEPT_CODE  ELSE APPLICANT_DEPT_CODE  END AS AppDepartmentCode,");
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_DEPT_NAME  ELSE APPLICANT_DEPT_NAME  END AS AppDepartmentName,");
            // --申請社員番号
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_EMP_NUM  ELSE APPLICANT_EMP_NUM  END AS AppEmployeeNo,");
            // --申請氏名
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_LNAME  ELSE APPLICANT_LNAME  END AS AppEmployeeLName,");
            sqlSb.Append("  CASE WHEN License.SHINSEISYAKBN='1' THEN AGENT_FNAME  ELSE APPLICANT_FNAME  END AS AppEmployeeFName,");
            // --資格コード
            sqlSb.Append("  License.CERTIFICATION_NO AS CertificationNo,");
            // --資格
            sqlSb.Append("  License.CERTIFICATION_NAME AS CertificationName,");
            // --取得日
            sqlSb.Append("  CASE WHEN License.ACQUISITION_DATE IS NULL THEN ''  ELSE SUBSTRING(License.ACQUISITION_DATE,1,4)+'/'+SUBSTRING(License.ACQUISITION_DATE,5,2)+'/'+SUBSTRING(License.ACQUISITION_DATE,7,2) END AS AcquisitionDate,");
            // --失効日
            sqlSb.Append("  CASE WHEN License.EXPIRY_DATE IS NULL THEN ''  ELSE SUBSTRING(License.EXPIRY_DATE,1,4)+'/'+SUBSTRING(License.EXPIRY_DATE,5,2)+'/'+SUBSTRING(License.EXPIRY_DATE,7,2) END AS ExpiryDate,");
            // --資格_点数
            sqlSb.Append("  License.SCORE Score,");
            // --資格_備考
            sqlSb.Append("  License.REMARKS Remarks,");
            // --資格_保有車両区分
            sqlSb.Append("  License.CAR_CATEGORY CarCategory,");
            // --資格_保有車両区分摘要
            sqlSb.Append("  License.CAR_CATEGORY_SUMMARY CarCategorySummary,");
            // --資格_資格証明書
            sqlSb.Append("  CASE WHEN License.UPLOADE_FILE_1 IS NULL THEN '有'  ELSE '無'  END AS UploadFile,");
            // --確認
            sqlSb.Append("  License.CHECK_FLG AS CheckKbn,");
            // --確認者_会社
            sqlSb.Append("  License.CHECK_CORP_CODE AS ChkCorpCode,");
            sqlSb.Append("  License.CHECK_CORP_NAME AS ChkCorpName,");
            // --確認者人事所属
            sqlSb.Append("  License.CHECK_DEPT_CODE AS ChkDepartmentCode,");
            sqlSb.Append("  License.CHECK_DEPT_NAME AS ChkDepartmentName,");
            // --確認者_社員番号
            sqlSb.Append("  License.CHECK_EMP_NUM AS ChkEmployeeNo,");
            // --確認者_氏名
            sqlSb.Append("  License.CHECK_LNAME+'　'+License.CHECK_FNAME AS ChkEmployeeFName,");
            // --確認日時
            sqlSb.Append("  FORMAT(License.CHECK_DATE,'yyyy/MM/dd HH:mm') AS ChkDate ");

            // 主テーブル  【資格免許登録】
            sqlSb.Append("  FROM TT_WF_CERTIFICATION_REGI License");
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

            // デフォルト条件の設定
            // 3：承認済み
            sqlCond.Append("    AND License.WFState = '3' ");

            // 画面入力により、条件を作ること
            // 会社コードif (!string.IsNullOrEmpty(searchCond.Kaishalist))
            if (searchCond.Kaishalist == "")
            {
                sqlCond.Append("    AND License.APPLICANT_CORP_CODE IN ( ");
                sqlCond.Append("            SELECT CompanyAcceptance.CORP_CODE");
                sqlCond.Append("              FROM MT_COMPANYACCEPTANCE CompanyAcceptance");
                sqlCond.Append("              INNER JOIN MT_BUSI_WF_REL B");
                sqlCond.Append("                    ON B.BUSINESS_CODE = CompanyAcceptance.BUSINESS_CODE");
                sqlCond.Append("            WHERE  CompanyAcceptance.ENTRUSTED_FLG = 'Y' ");
                sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sqlCond.Append("                   AND (CompanyAcceptance.DELETE_FLG <> 'X' OR CompanyAcceptance.DELETE_FLG IS NULL)");
                sqlCond.Append("                   AND B.WF_NO = @WfNo");
                sqlCond.Append("                   AND B.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sqlCond.Append("                   AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sqlCond.Append("                   AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL)");
                sqlCond.Append("            )");

                ps.Add("WfNo", searchCond.WfNo);
            }
            else
            {
                sqlCond.Append("    AND License.APPLICANT_CORP_CODE = @Kaishalist ");

                ps.Add("Kaishalist", searchCond.Kaishalist);
            }

            // 会社コード＋会社名を空欄で検索の場合は、受託会社全部を検索する
            if (string.IsNullOrEmpty(searchCond.Kaishalist) /*&& string.IsNullOrEmpty(searchCond.CompanyNm)*/)
            {
                // 受託している会社の情報のみに絞り込みを行う
                sqlCond.Append("    AND License.APPLICANT_CORP_CODE IN ( ");
                sqlCond.Append("            SELECT CompanyAcceptance.CORP_CODE");
                sqlCond.Append("              FROM MT_COMPANYACCEPTANCE CompanyAcceptance");
                sqlCond.Append("              INNER JOIN MT_BUSI_WF_REL B");
                sqlCond.Append("                    ON B.BUSINESS_CODE = CompanyAcceptance.BUSINESS_CODE");
                sqlCond.Append("            WHERE  CompanyAcceptance.ENTRUSTED_FLG = 'Y' ");
                sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sqlCond.Append("                   AND (CompanyAcceptance.DELETE_FLG <> 'X' OR CompanyAcceptance.DELETE_FLG IS NULL)");
                sqlCond.Append("                   AND B.WF_NO = @WfNo");
                sqlCond.Append("                   AND B.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sqlCond.Append("                   AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sqlCond.Append("                   AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL)");
                sqlCond.Append("            )");

                ps.Add("WfNo", searchCond.WfNo);
            }

            // 所属コード
            if (!string.IsNullOrEmpty(searchCond.AffiliationCd))
            {
                sqlCond.Append("    AND License.APPLICANT_DEPT_CODE = @AffiliationCd");
                ps.Add("AffiliationCd", searchCond.AffiliationCd);
            }

            // 所属名
            if (!string.IsNullOrEmpty(searchCond.AffiliationNm))
            {
                sqlCond.Append("    AND License.APPLICANT_DEPT_NAME LIKE @AffiliationNm");
                ps.Add("AffiliationNm", "%" + searchCond.AffiliationNm + "%");
            }

            // 社員番号 社員番号が入力されたら
            if (!string.IsNullOrEmpty(searchCond.EmployeeNo))
            {
                // 社員番号の設定
                sqlCond.Append("    AND License.APPLICANT_EMP_NUM = @EmployeeNo ");
                // パラメータの設定
                ps.Add("EmployeeNo", searchCond.EmployeeNo);
            }

            // 社員姓
            if (!string.IsNullOrEmpty(searchCond.EmployeeLastNm))
            {
                sqlCond.Append("    AND License.APPLICANT_LNAME LIKE @EmployeeLastNm");
                ps.Add("EmployeeLastNm", "%" + searchCond.EmployeeLastNm + "%");
            }

            // 社員名
            if (!string.IsNullOrEmpty(searchCond.EmployeeFirstNm))
            {
                sqlCond.Append("    AND License.APPLICANT_FNAME LIKE @EmployeeFirstNm");
                ps.Add("EmployeeFirstNm", "%" + searchCond.EmployeeFirstNm + "%");
            }

            // 資格コード
            if (!string.IsNullOrEmpty(searchCond.CertificationNo))
            {
                sqlCond.Append("    AND License.CERTIFICATION_NO = @CertificationNo ");

                ps.Add("CertificationNo", searchCond.CertificationNo);
            }

            // 資格名
            if (!string.IsNullOrEmpty(searchCond.CertificationNm))
            {
                sqlCond.Append("    AND License.CERTIFICATION_NAME LIKE @CertificationNm");
                ps.Add("CertificationNm", "%" + searchCond.CertificationNm + "%");
            }

            // 備考
            if (!string.IsNullOrEmpty(searchCond.Remarks))
            {
                sqlCond.Append("    AND License.Remarks LIKE @Remarks");
                ps.Add("Remarks", "%" + searchCond.Remarks + "%");
            }

            // 確認状態
            if (!string.IsNullOrEmpty(searchCond.CheckFlg))
            {
                sqlCond.Append("    AND License.CHECK_FLG = @CheckFlg");
                ps.Add("CheckFlg", searchCond.CheckFlg);
            }

            // 申請日
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDate))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDate),112) = CONVERT(nvarchar(8),License.REC_ENT_DATE,112)");

                // 入力条件
                ps.Add("AppDate", searchCond.AppDate);
            }

            //// Toのみがある場合
            //if (!string.IsNullOrEmpty(searchCond.AppDateTo))
            //{
            //    // 年月日の完全一致検索【トランザクションテーブル】登録日時
            //    sqlCond.Append("    AND CONVERT(nvarchar(8),License.REC_ENT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");

            //    // 入力条件
            //    ps.Add("AppDateTo", searchCond.AppDateTo);
            //}


            // 資格取得日
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AcquisitionDateFrom))
            {
                // 年月日の完全一致検索【トランザクションテーブル】資格_取得日
                sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @AcquisitionDateFrom),112) <= CONVERT(nvarchar(8),License.ACQUISITION_DATE,112)");

                // 入力条件
                ps.Add("AcquisitionDateFrom", searchCond.AcquisitionDateFrom);
            }

            // Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AcquisitionDateTo))
            {
                // 年月日の完全一致検索【トランザクションテーブル】資格_取得日
                sqlCond.Append("    AND CONVERT(nvarchar(8),License.ACQUISITION_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AcquisitionDateTo),112)");

                // 入力条件
                ps.Add("AcquisitionDateTo", searchCond.AcquisitionDateTo);
            }


            // 資格失効日
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.ExpiryDateFrom))
            {
                // 年月日の完全一致検索【トランザクションテーブル】資格_失効日
                sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @ExpiryDateFrom),112) <= CONVERT(nvarchar(8),License.EXPIRY_DATE,112)");

                // 入力条件
                ps.Add("ExpiryDateFrom", searchCond.ExpiryDateFrom);
            }

            // Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.ExpiryDateTo))
            {
                // 年月日の完全一致検索【弔事連絡トランザクションテーブル】資格_失効日
                sqlCond.Append("    AND CONVERT(nvarchar(8),License.EXPIRY_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @ExpiryDateTo),112)");

                // 入力条件
                ps.Add("ExpiryDateTo", searchCond.ExpiryDateTo);
            }

            // 条件の取得
            sql = sqlCond.ToString();
        }

        /// <summary>
        /// CSV出力タイトルの取得
        /// </summary>
        /// <returns>CSV出力タイトル</returns>
        private string GetCsvTilie()
        {
            // csvタイトル対象の作成
            StringBuilder csvTitle = new StringBuilder();

            csvTitle.Append("\"伝票番号\"");
            csvTitle.Append(",\"登録年月日\"");
            csvTitle.Append(",\"申請区分\"");
            csvTitle.Append(",\"資格登録者_会社コード\"");
            csvTitle.Append(",\"資格登録者_会社名\"");
            csvTitle.Append(",\"資格登録者_所属コード\"");
            csvTitle.Append(",\"資格登録者_所属名\"");
            csvTitle.Append(",\"資格登録者_社員番号\"");
            csvTitle.Append(",\"資格登録者_姓・漢字\"");
            csvTitle.Append(",\"資格登録者_名・漢字\"");
            csvTitle.Append(",\"申請者_会社コード\"");
            csvTitle.Append(",\"申請者_会社名\"");
            csvTitle.Append(",\"申請者_所属コード\"");
            csvTitle.Append(",\"申請者_所属名\"");
            csvTitle.Append(",\"申請者_社員番号\"");
            csvTitle.Append(",\"申請者_姓・漢字\"");
            csvTitle.Append(",\"申請者_名・漢字\"");
            csvTitle.Append(",\"資格_資格コード\"");
            csvTitle.Append(",\"資格_資格名称\"");
            csvTitle.Append(",\"資格_取得日\"");
            csvTitle.Append(",\"資格_失効日\"");
            csvTitle.Append(",\"資格_点数\"");
            csvTitle.Append(",\"資格_備考\"");
            csvTitle.Append(",\"資格_保有車両区分\"");
            csvTitle.Append(",\"資格_保有車両区分摘要\"");
            csvTitle.Append(",\"資格_資格証明書\"");
            csvTitle.Append(",\"確認状態\"");
            csvTitle.Append(",\"確認者_会社コード\"");
            csvTitle.Append(",\"確認者_会社名\"");
            csvTitle.Append(",\"確認者_所属コード\"");
            csvTitle.Append(",\"確認者_所属名\"");
            csvTitle.Append(",\"確認者_社員番号\"");
            csvTitle.Append(",\"確認者名\"");
            csvTitle.Append(",\"確認日時\"");

            return csvTitle.ToString();
        }

        /// <summary>
        /// 一覧検索条件クラス
        /// </summary>
        private class CondInfo
        {
            /// <summary>
            /// ワークフロー
            /// </summary>
            public string WfNo { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            public string Kaishalist { get; set; }

            /// <summary>
            /// 所属コード
            /// </summary>
            public string AffiliationCd { get; set; }

            /// <summary>
            /// 所属名
            /// </summary>
            public string AffiliationNm { get; set; }

            /// <summary>
            /// 社員番号
            /// </summary>
            public string EmployeeNo { get; set; }

            /// <summary>
            /// 社員姓
            /// </summary>
            public string EmployeeLastNm { get; set; }

            /// <summary>
            /// 社員名
            /// </summary>
            public string EmployeeFirstNm { get; set; }

            /// <summary>
            /// 資格コード
            /// </summary>
            public string CertificationNo { get; set; }

            /// <summary>
            /// 資格名称
            /// </summary>
            public string CertificationNm { get; set; }

            /// <summary>
            /// 備考
            /// </summary>
            public string Remarks { get; set; }

            /// <summary>
            /// 確認状態
            /// </summary>
            public string CheckFlg { get; set; }

            /// <summary>
            /// 申請日
            /// </summary>
            public string AppDate { get; set; }

            /// <summary>
            /// 資格取得日From
            /// </summary>
            public string AcquisitionDateFrom { get; set; }

            /// <summary>
            /// 資格取得日To
            /// </summary>
            public string AcquisitionDateTo { get; set; }

            /// <summary>
            /// 資格失効日From
            /// </summary>
            public string ExpiryDateFrom { get; set; }

            /// <summary>
            /// 資格失効日To
            /// </summary>
            public string ExpiryDateTo { get; set; }
        }
    }
}
