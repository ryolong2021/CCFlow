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
/// 結婚届照会一覧
/// </summary>
namespace BP.WF.HttpHandler
{
    public class Mn_MarriageList : BP.WF.HttpHandler.DirectoryPageBase
    {
        // 会社別条件マスタ 識別コード 82
        private const string SHIKIBETSU_KBN_EXAMPLE_NO = "82";
        // 条件コード 1
        private const string COND_CODE_ONE = "1";
        // ベース社員番号 9999
        private const string EMPLOYEE_BASE = "9999";

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
            sqlSb.Append("SELECT Marriage.OID AS WorkID,");
            // 申請日
            sqlSb.Append("       FORMAT(Marriage.REC_ENT_DATE,'yyyy年MM月dd日') AS REC_ENT_DATE,");
            // 会社名
            sqlSb.Append("       Marriage.CMOPANY_NAME AS CMOPANY_NAME,");
            // 所属コード
            sqlSb.Append("       Marriage.DEPT_NO AS DEPT_NO,");
            // 所属名
            sqlSb.Append("       Marriage.DEPT_NAME AS DEPT_NAME,");
            // 社員番号
            sqlSb.Append("       Marriage.EMPLOYEE_NO AS EMPLOYEE_NO,");
            // 社員名（漢字）
            sqlSb.Append("       Marriage.LAST_NAME_KANJI + '　' + Marriage.FIRST_NAME_KANJI AS NAME_KANJI,");
            // 社員名（カナ）
            sqlSb.Append("       Marriage.LAST_NAME_KANA + '　' + Marriage.FIRST_NAME_KANA AS NAME_KANA,");
            // 従業員区分
            sqlSb.Append("       Marriage.LOAM_EMPLOYEE_NAME AS LOAM_EMPLOYEE_NAME,");
            // 申請者の資格
            sqlSb.Append("       Marriage.QUALIFICATION_NAME AS QUALIFICATION_NAME,");
            // 入社日
            sqlSb.Append("       FORMAT(Marriage.HIRE_DATE,'yyyy年MM月dd日') AS HIRE_DATE,");
            // グッドライフ区分
            sqlSb.Append("       glcKbn.KBNNAME AS GOOD_LIFE_CLASS,");
            // 組合区分
            sqlSb.Append("       unionKbn.KBNNAME AS UNION_CLASS,");
            // 入籍日
            sqlSb.Append("       FORMAT(CONVERT(date, Marriage.MARRIAGE_DATE),'yyyy年MM月dd日') + '（' + ");
            sqlSb.Append("           CASE");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 1 THEN '日'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 2 THEN '月'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 3 THEN '火'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 4 THEN '水'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 5 THEN '木'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 6 THEN '金'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 7 THEN '土'");
            sqlSb.Append("           END");
            sqlSb.Append("       + '）' AS MARRIAGE_DATE,");
            // 入力区分
            sqlSb.Append("       CASE");
            sqlSb.Append("           WHEN Marriage.ALLOWANCE_REGI_CLASS = '0' and Marriage.TELEGRAM_REGI_CLASS = '0' THEN '祝い金申請／祝電申請'");
            sqlSb.Append("           WHEN Marriage.ALLOWANCE_REGI_CLASS = '0' and Marriage.TELEGRAM_REGI_CLASS = '1' THEN '祝い金申請'");
            sqlSb.Append("           WHEN Marriage.ALLOWANCE_REGI_CLASS = '1' and Marriage.TELEGRAM_REGI_CLASS = '0' THEN '祝電申請'");
            sqlSb.Append("           ELSE ''");
            sqlSb.Append("       END AS INPUT_CLASS,");
            // 配偶者社員番号
            sqlSb.Append("       Marriage.FAMILY_EMPLOYEE_NO AS FAMILY_EMPLOYEE_NO,");
            // 配偶者氏名（漢字）
            sqlSb.Append("       Marriage.FAMILY_LAST_NAME_KANJI + '　' + Marriage.FAMILY_FIRST_NAME_KANJI AS FAMILY_NAME_KANJI,");
            // 配偶者氏名（カナ）
            sqlSb.Append("       Marriage.FAMILY_LAST_NAME_KANA + '　' + Marriage.FAMILY_FIRST_NAME_KANA AS FAMILY_NAME_KANA,");
            // 祝い金
            sqlSb.Append("       CASE");
            sqlSb.Append("           WHEN Marriage.ALLOWANCE_REGI_CLASS = '0' THEN Marriage.ALLOWANCE_REGI");
            sqlSb.Append("           ELSE ''");
            sqlSb.Append("       END AS ALLOWANCE_REGI,");
            // 結婚式場名
            sqlSb.Append("       Marriage.CEREMONY_NAME_KANJI AS CEREMONY_NAME_KANJI,");
            // 挙式日時
            sqlSb.Append("       FORMAT(Marriage.WEDDING_DAY,'yyyy年MM月dd日') + '（' + ");
            sqlSb.Append("           CASE");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 1 THEN '日'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 2 THEN '月'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 3 THEN '火'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 4 THEN '水'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 5 THEN '木'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 6 THEN '金'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,Marriage.WEDDING_DAY) = 7 THEN '土'");
            sqlSb.Append("           END");
            sqlSb.Append("       + '）' + FORMAT(Marriage.WEDDING_DAY,'hh時mm分') AS WEDDING_DAY,");

            // 式場郵便番号
            sqlSb.Append("       Marriage.CEREMONY_POSTAL_CODE AS CEREMONY_POSTAL_CODE,");
            // 式場電話番号
            sqlSb.Append("       Marriage.CEREMONY_TELEPHONE_NUMBER1 + '-' + Marriage.CEREMONY_TELEPHONE_NUMBER2 + '-' + Marriage.CEREMONY_TELEPHONE_NUMBER3 AS CEREMONY_TELEPHONE_NUMBER,");
            // 式場住所
            sqlSb.Append("       Marriage.CEREMONY_ADDRESS1 + Marriage.CEREMONY_ADDRESS2 + Marriage.CEREMONY_ADDRESS3 AS CEREMONY_ADDRESS");

            // テーブル名
            sqlSb.Append("  FROM TT_WF_MARRIAGE Marriage");

            // グッドライフ区分の関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN glcKbn ON");
            sqlSb.Append("       glcKbn.KBNCODE = 'GLC_KBN'");
            sqlSb.Append("   AND glcKbn.KBNVALUE = CONVERT(varchar, Marriage.GOOD_LIFE_CLASS)");

            // グッドライフ区分の関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN unionKbn ON");
            sqlSb.Append("       unionKbn.KBNCODE = 'KUMIAI_KBN'");
            sqlSb.Append("   AND CONVERT(int, unionKbn.KBNVALUE) = Marriage.UNION_CLASS");

            // 条件
            sqlSb.Append("    WHERE Marriage.OID = @WorkID ");
            // パラメータの設定
            ps.Add("WorkID", workId);

            // sql文の設定
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetMarriageList()
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
            sqlSb.Append("SELECT Marriage.OID AS WorkID,");
            // 社員番号
            sqlSb.Append("       Marriage.EMPLOYEE_NO AS EmployeeNo,");
            // 氏名
            sqlSb.Append("       Marriage.LAST_NAME_KANJI + '　' + Marriage.FIRST_NAME_KANJI AS EmployeeName,");
            // 所属
            sqlSb.Append("       Marriage.DEPT_NAME AS DepartmentName,");
            // グッドライフ区分
            sqlSb.Append("       glcKbn.KBNNAME AS GoodlifeKbn,");
            // 組合区分
            sqlSb.Append("       unionKbn.KBNNAME AS KumiaiKbn,");
            // 申請日
            sqlSb.Append("       FORMAT(Marriage.REC_ENT_DATE,'yyyy年MM月dd日') AS AppDate,");
            // 入籍日
            sqlSb.Append("       FORMAT(CONVERT(date, Marriage.MARRIAGE_DATE),'yyyy年MM月dd日') AS EnrollmentDate,");
            // 挙式日
            sqlSb.Append("       FORMAT(Marriage.WEDDING_DAY,'yyyy年MM月dd日') AS WeddingDate");

            // テーブル名
            sqlSb.Append("  FROM TT_WF_MARRIAGE Marriage");

            // グッドライフ区分の関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN glcKbn ON");
            sqlSb.Append("       glcKbn.KBNCODE = 'GLC_KBN'");
            sqlSb.Append("   AND glcKbn.KBNVALUE = CONVERT(varchar, Marriage.GOOD_LIFE_CLASS)");

            // グッドライフ区分の関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN unionKbn ON");
            sqlSb.Append("       unionKbn.KBNCODE = 'KUMIAI_KBN'");
            sqlSb.Append("   AND CONVERT(int, unionKbn.KBNVALUE) = Marriage.UNION_CLASS");

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
            // システム時間の取得
            string sysDate = DateTime.Now.ToString("yyyyMMdd");

            // APIでEBSから会社別条件マスタのデータの取得
            List<Dictionary<string, string>> apiCompaniesCondData =
                WF_AppForm.GetEbsDataWithApi("Get_Companies_Conditions_All_Data");

            // APIでEBSから名義名称マスタのデータの取得
            List<Dictionary<string, string>> apiOwnerNameData =
                WF_AppForm.GetEbsDataWithApi("Get_Owner_Name_All_Data");

            // sql文対象の作成
            StringBuilder strCsv = new StringBuilder();

            // タイトルの追加
            strCsv.Append(this.GetCsvTilie());

            //DataRowsに格納してからデータを取得する⑤
            foreach (DataRow row in dt.Rows)
            {
                // 改行の追加
                strCsv.Append(Environment.NewLine);

                // 会社コードの取得
                string cmopanyNo = row["COMPANY_CODE"].ToString();

                // 出向元会社コードの取得
                string cmopanyNoSeconded = row["SECONDED_COMPANY"].ToString();

                // 社員番号の取得
                string employeeNo = row["EMPLOYEE_NO"].ToString();
                // APIでEBSから銀行口座マスタのデータの取得
                Dictionary<string, string> apiBankInfoData = this.GetBankInfoByPk(employeeNo);

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    // コンマの追加
                    if (i > 0)
                    {
                        strCsv.Append(",");
                    }

                    // 特別処理（EBS連携データ整理用）
                    bool specialFlag = this.SetSpecialColData(
                        apiCompaniesCondData,
                        apiOwnerNameData,
                        apiBankInfoData,
                        sysDate,
                        cmopanyNo,
                        cmopanyNoSeconded,
                        row.Table.Columns[i].ColumnName,
                        out string specialVal);
                    // 設定要否の判断
                    if (specialFlag)
                    {
                        row[i] = specialVal;
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

            // 伝票番号
            sqlSb.Append("SELECT Marriage.OID AS WorkId,");

            // 会社名_祝電申込
            sqlSb.Append("       Marriage.CMOPANY_NAME AS CMOPANY_NAME_CEREMONY_APP,");

            // 所属コード
            sqlSb.Append("       Marriage.DEPT_NO AS DEPT_NO,");

            // 所属名称
            sqlSb.Append("       Marriage.DEPT_NAME AS DEPT_NAME,");

            // 財務部署コード
            sqlSb.Append("       Marriage.BUSHOCODE AS BUSHOCODE,");

            // 社員番号
            sqlSb.Append("       Marriage.EMPLOYEE_NO AS EMPLOYEE_NO,");

            // 従業員の漢字氏名_姓
            sqlSb.Append("       Marriage.LAST_NAME_KANJI AS LAST_NAME_KANJI,");

            // 従業員の漢字氏名_名
            sqlSb.Append("       Marriage.FIRST_NAME_KANJI AS FIRST_NAME_KANJI,");

            // 社員区分
            sqlSb.Append("       Marriage.LOAM_EMPLOYEE_NAME AS LOAM_EMPLOYEE_NAME,");

            // 社員区分コード
            sqlSb.Append("       Marriage.LOAM_EMPLOYEE AS LOAM_EMPLOYEE,");

            // 組合区分
            sqlSb.Append("       unionKbn.KBNNAME AS UNION_CLASS_NAME,");

            // 組合区分コード
            sqlSb.Append("       Marriage.UNION_CLASS AS UNION_CLASS,");

            // GLC区分
            sqlSb.Append("       glcKbn.KBNNAME AS GOOD_LIFE_CLASS_NAME,");

            // GLC区分コード
            sqlSb.Append("       Marriage.GOOD_LIFE_CLASS AS GOOD_LIFE_CLASS,");

            // 入社区分
            sqlSb.Append("       Marriage.HIRE_KBN_NAME AS HIRE_KBN_NAME,");

            // 入社年月日
            sqlSb.Append("       CONVERT(nvarchar, Marriage.HIRE_DATE, 111) AS HIRE_DATE,");

            // 従業員の出向元会社コード
            sqlSb.Append("       Marriage.COMPANY_CODE AS COMPANY_CODE,");

            // 従業員の出向元会社名
            sqlSb.Append("       Marriage.CMOPANY_NAME AS CMOPANY_NAME,");

            // 従業員の出向先会社コード
            sqlSb.Append("       Marriage.SECONDED_COMPANY AS SECONDED_COMPANY,");

            // 従業員の出向先会社名
            sqlSb.Append("       Marriage.SECONDED_COMPANY_NAME AS SECONDED_COMPANY_NAME,");

            // 入力区分「祝い金申請」
            sqlSb.Append("       CASE");
            sqlSb.Append("           WHEN Marriage.ALLOWANCE_REGI_CLASS = '0' THEN '祝い金申請'");
            sqlSb.Append("           ELSE ''");
            sqlSb.Append("       END AS ALLOWANCE_REGI_CLASS,");

            // 入力区分「祝電申請」
            sqlSb.Append("       CASE");
            sqlSb.Append("           WHEN Marriage.TELEGRAM_REGI_CLASS = '0' THEN '祝電申請'");
            sqlSb.Append("           ELSE ''");
            sqlSb.Append("       END AS TELEGRAM_REGI_CLASS,");

            // 入籍年月日（予定日）
            sqlSb.Append("       FORMAT(CONVERT(date, Marriage.MARRIAGE_DATE),'yyyy年MM月dd日') + '（' + ");
            sqlSb.Append("           CASE");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 1 THEN '日'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 2 THEN '月'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 3 THEN '火'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 4 THEN '水'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 5 THEN '木'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 6 THEN '金'");
            sqlSb.Append("               WHEN  DATEPART(WEEKDAY,CONVERT(date, Marriage.MARRIAGE_DATE)) = 7 THEN '土'");
            sqlSb.Append("           END");
            sqlSb.Append("       + '）' AS MARRIAGE_DATE,");

            // 配偶者のカナ氏名
            sqlSb.Append("       Marriage.FAMILY_LAST_NAME_KANA + '　' + Marriage.FAMILY_FIRST_NAME_KANA AS FAMILY_OLD_NAME,");

            // 配偶者の漢字氏名
            sqlSb.Append("       Marriage.FAMILY_LAST_NAME_KANJI + '　' + Marriage.FAMILY_FIRST_NAME_KANJI AS FAMILY_NAME_KANJI,"); 

            // 配偶者の社員番号
            sqlSb.Append("       Marriage.FAMILY_EMPLOYEE_NO AS FAMILY_EMPLOYEE_NO,");

            // 配偶者の社員番号より抽出された漢字氏名
            sqlSb.Append("       '' AS FAMILY_OLD_NAME_KANJI,");

            // 結婚式の年月日
            sqlSb.Append("       FORMAT(Marriage.WEDDING_DAY,'yyyy年MM月dd日') AS WEDDING_DAY,");

            // 結婚式の時刻
            sqlSb.Append("       FORMAT(Marriage.WEDDING_DAY,'hh時mm分') AS WEDDING_TIME,");

            // お届け先会場_祝電
            sqlSb.Append("       Marriage.CEREMONY_NAME_KANJI AS CEREMONY_NAME_KANJI,");

            // お届け先会場_祝電_郵便番号
            sqlSb.Append("       Marriage.CEREMONY_POSTAL_CODE AS CEREMONY_POSTAL_CODE,");

            // お届け先会場_祝電_都道府県
            sqlSb.Append("       Marriage.CEREMONY_ADDRESS1 AS CEREMONY_ADDRESS1,");

            // お届け先会場_祝電_市・区・郡
            sqlSb.Append("       Marriage.CEREMONY_ADDRESS2 AS CEREMONY_ADDRESS2,");

            // お届け先会場_祝電_町・村
            sqlSb.Append("       Marriage.CEREMONY_ADDRESS3 AS CEREMONY_ADDRESS3,");

            // お届け先会場_祝電_丁番
            sqlSb.Append("       Marriage.CEREMONY_ADDRESS3 AS CEREMONY_CHOBAN_ADDRESS3,");

            // お届け先会場_祝電_市外局番
            sqlSb.Append("       Marriage.CEREMONY_TELEPHONE_NUMBER1 AS CEREMONY_TELEPHONE_NUMBER1,");

            // お届け先会場_祝電_局番
            sqlSb.Append("       Marriage.CEREMONY_TELEPHONE_NUMBER2 AS CEREMONY_TELEPHONE_NUMBER2,");

            // お届け先会場_祝電_番号
            sqlSb.Append("       Marriage.CEREMONY_TELEPHONE_NUMBER3 AS CEREMONY_TELEPHONE_NUMBER3,");

            // 文例番号（会社用）
            sqlSb.Append("       '' AS EXAMPLE_NO_CMOPANY,");

            // 祝電差出人_会社名
            sqlSb.Append("       Marriage.CMOPANY_NAME AS CEREMONY_CMOPANY,");

            // 祝電差出人_会社代表者肩書きと氏名
            sqlSb.Append("       '' AS CEREMONY_CMOPANY_TITLE_NAME,");

            // 文例番号（組合用）
            sqlSb.Append("       '' AS EXAMPLE_NO_UNIONS,");

            // 祝電差出人_組合名
            sqlSb.Append("       '' AS CEREMONY_UNIONS,");

            // 祝電差出人_組合代表者肩書きと氏名
            sqlSb.Append("       '' AS CEREMONY_UNIONS_TITLE_NAME,");

            // 文例番号（出向先会社用）
            sqlSb.Append("       '' AS EXAMPLE_NO_CMOPANY_SECONDED,");

            // 祝電差出人_出向先会社名
            sqlSb.Append("       Marriage.SECONDED_COMPANY_NAME  AS CEREMONY_CMOPANY_SECONDED,");

            // 祝電差出人_出向先会社代表者肩書きと氏名
            sqlSb.Append("       '' AS CEREMONY_CMOPANY_SECONDED_TITLE_NAME,");

            // 従業員の給与振込金融機関コード
            sqlSb.Append("       '' AS BANK_CODE, ");

            // 従業員の給与振込金融機関名
            sqlSb.Append("       '' AS BANK_NAME,");
            
            // 従業員の給与振込支店コード
            sqlSb.Append("       '' AS BANK_BRANCH_CODE,");

            // 従業員の給与振込支店名
            sqlSb.Append("       '' AS BANK_BRANCH_NAME,");
            
            // 従業員の給与振込口座区分
            sqlSb.Append("       '' AS BANK_ACCOUNT_KBN,");

            // 従業員の給与振込口座番号
            sqlSb.Append("       '' AS BANK_ACCOUNT_CODE,");

            // 従業員のカナ氏名_姓
            sqlSb.Append("       Marriage.LAST_NAME_KANA AS LAST_NAME_KANA,");
            
            // 従業員のカナ氏名_名
            sqlSb.Append("       Marriage.FIRST_NAME_KANA AS FIRST_NAME_KANA,");
            
            // 祝金金額
            sqlSb.Append("       CASE");
            sqlSb.Append("           WHEN Marriage.ALLOWANCE_REGI_CLASS = '0' THEN Marriage.ALLOWANCE_REGI");
            sqlSb.Append("           ELSE ''");
            sqlSb.Append("       END AS ALLOWANCE_REGI,");

            // 申請日
            sqlSb.Append("       CONVERT(nvarchar, Marriage.REC_ENT_DATE, 111) AS REC_ENT_DATE");

            // 主テーブル  【結婚届】
            sqlSb.Append("  FROM TT_WF_MARRIAGE Marriage");

            // グッドライフ区分の関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN glcKbn ON");
            sqlSb.Append("       glcKbn.KBNCODE = 'GLC_KBN'");
            sqlSb.Append("   AND glcKbn.KBNVALUE = CONVERT(varchar, Marriage.GOOD_LIFE_CLASS)");

            // 組合区分の関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN unionKbn ON");
            sqlSb.Append("       unionKbn.KBNCODE = 'KUMIAI_KBN'");
            sqlSb.Append("   AND CONVERT(int, unionKbn.KBNVALUE) = Marriage.UNION_CLASS");

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
            sqlCond.Append("    AND Marriage.WFState = '3' ");

            // 画面入力により、条件を作ること
            // 社員番号 社員番号が入力されたら、他の条件が無視にします。
            if (!string.IsNullOrEmpty(searchCond.EmployeeNo))
            {
                // 社員番号の設定
                sqlCond.Append("    AND Marriage.EMPLOYEE_NO = @EmployeeNo ");
                // パラメータの設定
                ps.Add("EmployeeNo", searchCond.EmployeeNo);
            }
            else
            {
                // 会社コード
                if (!string.IsNullOrEmpty(searchCond.CompanyCd))
                {
                    sqlCond.Append("    AND Marriage.COMPANY_CODE = @CompanyCd ");

                    ps.Add("CompanyCd", searchCond.CompanyCd);
                }

                // 会社名
                if (!string.IsNullOrEmpty(searchCond.CompanyNm))
                {
                    sqlCond.Append("    AND Marriage.CMOPANY_NAME LIKE @CompanyNm ");

                    ps.Add("CompanyNm", "%" + searchCond.CompanyNm + "%");
                }

                // 会社コード＋会社名を空欄で検索の場合は、受託会社全部を検索する
                if (string.IsNullOrEmpty(searchCond.CompanyCd) && string.IsNullOrEmpty(searchCond.CompanyNm))
                {
                    // 受託している会社の情報のみに絞り込みを行う
                    sqlCond.Append("    AND Marriage.COMPANY_CODE IN ( ");
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
                    sqlCond.Append("    AND Marriage.DEPT_NO = @AffiliationCd");
                    ps.Add("AffiliationCd", searchCond.AffiliationCd);
                }

                // 所属名
                if (!string.IsNullOrEmpty(searchCond.AffiliationNm))
                {
                    sqlCond.Append("    AND Marriage.DEPT_NAME LIKE @AffiliationNm");
                    ps.Add("AffiliationNm", "%" + searchCond.AffiliationNm + "%");
                }

                // 従業員区分
                if (!string.IsNullOrEmpty(searchCond.WorkerKbn))
                {
                    sqlCond.Append("    AND Marriage.LOAM_EMPLOYEE = @WorkerKbn");
                    ps.Add("WorkerKbn", searchCond.WorkerKbn);
                }

                // 出向者区分
                if (!string.IsNullOrEmpty(searchCond.SecondedStaffKbn))
                {
                    sqlCond.Append("    AND Marriage.SYUKOU_KBN = @SecondedStaffKbn");
                    ps.Add("SecondedStaffKbn", searchCond.SecondedStaffKbn);
                }

                // グッドライフ区分
                if (!string.IsNullOrEmpty(searchCond.GoodLifeKbn))
                {
                    sqlCond.Append("    AND Marriage.GOOD_LIFE_CLASS = @GoodLifeKbn");
                    ps.Add("GoodLifeKbn", searchCond.GoodLifeKbn);
                }

                // 組合区分
                if (!string.IsNullOrEmpty(searchCond.UnionKbn))
                {
                    sqlCond.Append("    AND Marriage.UNION_CLASS = @UnionKbn");
                    ps.Add("UnionKbn", searchCond.UnionKbn);
                }

                // 申請日
                // Fromのみがある場合
                if (!string.IsNullOrEmpty(searchCond.AppDateFrom))
                {
                    // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                    sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8),Marriage.REC_ENT_DATE,112)");

                    // 入力条件
                    ps.Add("AppDateFrom", searchCond.AppDateFrom);
                }

                // Toのみがある場合
                if (!string.IsNullOrEmpty(searchCond.AppDateTo))
                {
                    // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                    sqlCond.Append("    AND CONVERT(nvarchar(8),Marriage.REC_ENT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");

                    // 入力条件
                    ps.Add("AppDateTo", searchCond.AppDateTo);
                }
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

            // 伝票番号
            csvTitle.Append("\"伝票番号\"");

            // 会社名_祝電申込
            csvTitle.Append(",\"会社名_祝電申込\"");

            // 所属コード
            csvTitle.Append(",\"所属コード\"");

            // 所属名称
            csvTitle.Append(",\"所属名称\"");

            // 財務部署コード
            csvTitle.Append(",\"財務部署コード\"");

            // 社員番号
            csvTitle.Append(",\"社員番号\"");

            // 従業員の漢字氏名_姓
            csvTitle.Append(",\"従業員の漢字氏名_姓\"");

            // 従業員の漢字氏名_名
            csvTitle.Append(",\"従業員の漢字氏名_名\"");

            // 社員区分
            csvTitle.Append(",\"社員区分\"");

            // 社員区分コード
            csvTitle.Append(",\"社員区分コード\"");

            // 組合区分
            csvTitle.Append(",\"組合区分\"");

            // 組合区分コード
            csvTitle.Append(",\"組合区分コード\"");

            // GLC区分
            csvTitle.Append(",\"GLC区分\"");

            // GLC区分コード
            csvTitle.Append(",\"GLC区分コード\"");

            // 入社区分
            csvTitle.Append(",\"入社区分\"");

            // 入社年月日
            csvTitle.Append(",\"入社年月日\"");

            // 従業員の出向元会社コード
            csvTitle.Append(",\"従業員の出向元会社コード\"");

            // 従業員の出向元会社名
            csvTitle.Append(",\"従業員の出向元会社名\"");

            // 従業員の出向先会社コード
            csvTitle.Append(",\"従業員の出向先会社コード\"");

            // 従業員の出向先会社名
            csvTitle.Append(",\"従業員の出向先会社名\"");

            // 入力区分「祝い金申請」
            csvTitle.Append(",\"入力区分「祝い金申請」\"");

            // 入力区分「祝電申請」
            csvTitle.Append(",\"入力区分「祝電申請」\"");

            // 入籍年月日（予定日）
            csvTitle.Append(",\"入籍年月日（予定日）\"");

            // 配偶者のカナ氏名
            csvTitle.Append(",\"配偶者のカナ氏名\"");

            // 配偶者の漢字氏名
            csvTitle.Append(",\"配偶者の漢字氏名\"");

            // 配偶者の社員番号
            csvTitle.Append(",\"配偶者の社員番号\"");

            // 配偶者の社員番号より抽出された漢字氏名
            csvTitle.Append(",\"配偶者の社員番号より抽出された漢字氏名\"");

            // 結婚式の年月日
            csvTitle.Append(",\"結婚式の年月日\"");

            // 結婚式の時刻
            csvTitle.Append(",\"結婚式の時刻\"");

            // お届け先会場_祝電
            csvTitle.Append(",\"お届け先会場_祝電\"");

            // お届け先会場_祝電_郵便番号
            csvTitle.Append(",\"お届け先会場_祝電_郵便番号\"");

            // お届け先会場_祝電_都道府県
            csvTitle.Append(",\"お届け先会場_祝電_都道府県\"");

            // お届け先会場_祝電_市・区・郡
            csvTitle.Append(",\"お届け先会場_祝電_市・区・郡\"");

            // お届け先会場_祝電_町・村
            csvTitle.Append(",\"お届け先会場_祝電_町・村\"");

            // お届け先会場_祝電_丁番
            csvTitle.Append(",\"お届け先会場_祝電_丁番\"");

            // お届け先会場_祝電_市外局番
            csvTitle.Append(",\"お届け先会場_祝電_市外局番\"");

            // お届け先会場_祝電_局番
            csvTitle.Append(",\"お届け先会場_祝電_局番\"");

            // お届け先会場_祝電_番号
            csvTitle.Append(",\"お届け先会場_祝電_番号\"");

            // 文例番号（会社用）
            csvTitle.Append(",\"文例番号（会社用）\"");

            // 祝電差出人_会社名
            csvTitle.Append(",\"祝電差出人_会社名\"");

            // 祝電差出人_会社代表者肩書きと氏名
            csvTitle.Append(",\"祝電差出人_会社代表者肩書きと氏名\"");

            // 文例番号（組合用）
            csvTitle.Append(",\"文例番号（組合用）\"");

            // 祝電差出人_組合名
            csvTitle.Append(",\"祝電差出人_組合名\"");

            // 祝電差出人_組合代表者肩書きと氏名
            csvTitle.Append(",\"祝電差出人_組合代表者肩書きと氏名\"");

            // 文例番号（出向先会社用）
            csvTitle.Append(",\"文例番号（出向先会社用）\"");

            // 祝電差出人_出向先会社名
            csvTitle.Append(",\"祝電差出人_出向先会社名\"");

            // 祝電差出人_出向先会社代表者肩書きと氏名
            csvTitle.Append(",\"祝電差出人_出向先会社代表者肩書きと氏名\"");

            // 従業員の給与振込金融機関コード
            csvTitle.Append(",\"従業員の給与振込金融機関コード\"");

            // 従業員の給与振込金融機関名
            csvTitle.Append(",\"従業員の給与振込金融機関名\"");

            // 従業員の給与振込支店コード
            csvTitle.Append(",\"従業員の給与振込支店コード\"");

            // 従業員の給与振込支店名
            csvTitle.Append(",\"従業員の給与振込支店名\"");

            // 従業員の給与振込口座区分
            csvTitle.Append(",\"従業員の給与振込口座区分\"");

            // 従業員の給与振込口座番号
            csvTitle.Append(",\"従業員の給与振込口座番号\"");

            // 従業員のカナ氏名_姓
            csvTitle.Append(",\"従業員のカナ氏名_姓\"");

            // 従業員のカナ氏名_名
            csvTitle.Append(",\"従業員のカナ氏名_名\"");

            // 祝金金額
            csvTitle.Append(",\"祝金金額\"");

            // 申請日
            csvTitle.Append(",\"申請日\"");

            return csvTitle.ToString();
        }

        /// <summary>
        /// EBSから取得データはCCFLOWのデータと整理されています
        /// </summary>
        /// <param name="apiCompaniesCondData">APIでEBSから会社別条件マスタのデータ</param>
        /// <param name="apiOwnerNameData">APIでEBSから名義名称マスタのデータ</param>
        /// <param name="apiBankInfoData">APIでEBSから銀行口座マスタのデータ</param>
        /// <param name="sysDate">システム時間（YYYYMMDD）</param>
        /// <param name="cmopanyNo">従業員の出向元会社コード</param>
        /// <param name="cmopanyNoSeconded">従業員の出向先会社コード</param>
        /// <param name="key">キー</param>
        /// <param name="val">値</param>
        /// <returns>戻り値設定のフラグ: true 設定  false 不設定</returns>
        private bool SetSpecialColData(
            List<Dictionary<string, string>> apiCompaniesCondData,
            List<Dictionary<string, string>> apiOwnerNameData,
            Dictionary<string, string> apiBankInfoData,
            string sysDate,
            string cmopanyNo,
            string cmopanyNoSeconded,
            string key,
            out string val)
        {
            // デフォルト値
            val = string.Empty;

            // 設定値
            Dictionary<string, string> dataVal;

            // 文例番号（会社用）
            if (key == "EXAMPLE_NO_CMOPANY")
            {
                dataVal = this.GetCompaniesConditionsData(
                    apiCompaniesCondData,
                    cmopanyNo,
                    sysDate);

                if (dataVal != null)
                {
                    val = dataVal["JOKEN_SUCHI_1"];
                }

                return true;
            }

            // 祝電差出人_会社代表者肩書きと氏名
            if (key == "CEREMONY_CMOPANY_TITLE_NAME")
            {
                dataVal = this.GetOwnerNameData(
                    apiOwnerNameData,
                    cmopanyNo,
                    "0",
                    sysDate);

                if (dataVal != null)
                {
                    // 【名義名称マスタ】名義人役職、名義人氏名
                    val = dataVal["YAKUSHOKU"] + "　" + dataVal["SHIMEI"];
                }

                return true;
            }

            // 文例番号（組合用）
            if (key == "EXAMPLE_NO_UNIONS")
            {
                dataVal = this.GetCompaniesConditionsData(
                    apiCompaniesCondData,
                    cmopanyNo,
                    sysDate);

                if (dataVal != null)
                {
                    val = dataVal["JOKEN_SUCHI_2"];
                }

                return true;
            }

            // 祝電差出人_組合名
            if (key == "CEREMONY_UNIONS")
            {
                dataVal = this.GetOwnerNameData(
                    apiOwnerNameData,
                    cmopanyNo,
                    "1",
                    sysDate);

                if (dataVal != null)
                {
                    // 【名義名称マスタ】組合名
                    val = dataVal["KUMIAIMEI"];
                }
                else
                {
                    dataVal = this.GetOwnerNameData(
                        apiOwnerNameData,
                        cmopanyNo,
                        "2",
                        sysDate);

                    // レコードなければ、【名義名称マスタ】名義区分 = "2"のレコードがあれば、
                    // "従業員一同"を出力、なければ、空白を出力。
                    if (dataVal != null)
                    {
                        val = "従業員一同";
                    }
                }

                return true;
            }

            // 祝電差出人_組合代表者肩書きと氏名
            if (key == "CEREMONY_UNIONS_TITLE_NAME")
            {
                dataVal = this.GetOwnerNameData(
                    apiOwnerNameData,
                    cmopanyNo,
                    "1",
                    sysDate);

                if (dataVal != null)
                {
                    // 【名義名称マスタ】名義人役職、名義人氏名
                    val = dataVal["YAKUSHOKU"] + "　" + dataVal["SHIMEI"];
                }

                return true;
            }

            // 文例番号（出向先会社用）
            if (key == "EXAMPLE_NO_CMOPANY_SECONDED")
            {
                dataVal = this.GetCompaniesConditionsData(
                    apiCompaniesCondData,
                    cmopanyNo,
                    sysDate);

                if (dataVal != null)
                {
                    // 条件_数値_3
                    val = dataVal["JOKEN_SUCHI_3"];
                }

                return true;
            }

            // 祝電差出人_出向先会社代表者肩書きと氏名
            if (key == "CEREMONY_CMOPANY_SECONDED_TITLE_NAME")
            {
                dataVal = this.GetOwnerNameData(
                    apiOwnerNameData,
                    cmopanyNoSeconded,
                    "0",
                    sysDate);

                if (dataVal != null)
                {
                    // 【名義名称マスタ】名義人役職、名義人氏名
                    val = dataVal["YAKUSHOKU"] + "　" + dataVal["SHIMEI"];
                }

                return true;
            }

            // 銀行情報の設定
            if (apiBankInfoData != null)
            {
                // 従業員の給与振込金融機関コード
                if (key == "BANK_CODE")
                {
                    // 【銀行口座マスタ】銀行コード_銀行振込口座　　★新設
                    val = apiBankInfoData["GINKOCODE_GINKOFURIKOMIKOZA"];
                    return true;
                }

                // 従業員の給与振込金融機関名
                if (key == "BANK_NAME")
                {
                    // 【銀行口座マスタ】銀行名_銀行振込口座　　★新設
                    val = apiBankInfoData["GINKOMEI_GINKOFURIKOMIKOZA"];
                    return true;
                }

                // 従業員の給与振込支店コード
                if (key == "BANK_BRANCH_CODE")
                {
                    // 【銀行口座マスタ】支店コード_銀行振込口座　　★新設
                    val = apiBankInfoData["SHITENCODE_GINKOFURIKOMIKOZA"];
                    return true;
                }

                // 従業員の給与振込支店名
                if (key == "BANK_BRANCH_NAME")
                {
                    // 【銀行口座マスタ】支店名_銀行振込口座　　★新設
                    val = apiBankInfoData["SHITENMEI_GINKOFURIKOMIKOZA"];
                    return true;
                }

                // 従業員の給与振込口座区分
                if (key == "BANK_ACCOUNT_KBN")
                {
                    // 【銀行口座マスタ】口座区分_銀行振込口座　　★新設
                    val = apiBankInfoData["KOZAKBN_GINKOFURIKOMIKOZA"];
                    return true;
                }

                // 従業員の給与振込口座番号
                if (key == "BANK_ACCOUNT_CODE")
                {
                    // 【銀行口座マスタ】口座番号_銀行振込口座　　★新設
                    val = apiBankInfoData["KOZABANGO_GINKOFURIKOMIKOZA"];
                    return true;
                }
            }

            // 特別処理ではないので、設定不要
            return false;
        }

        /// <summary>
        /// APIでEBSから会社別条件マスタのデータの取得
        /// </summary>
        /// <param name="apiData">APIでEBSから会社別条件マスタのデータ</param>
        /// <param name="cmopanyNo">従業員の出向元会社コード</param>
        /// <param name="sysDate">システム時間（YYYYMMDD）</param>
        /// <returns>会社別条件マスタのデータ</returns>
        private Dictionary<string, string> GetCompaniesConditionsData(
            List<Dictionary<string, string>> apiData,
            string cmopanyNo,
            string sysDate)
        {
            // APIで会社別条件マスタのデータの取得
            Dictionary<string, string> resData =　apiData.Find(obj =>
                // 識別区分="82"
                obj["SHIKIBETSUKBN"] == SHIKIBETSU_KBN_EXAMPLE_NO &&
                // 会社コード=従業員の出向元会社コード
                obj["KAISHACODE"] == cmopanyNo &&
                // 条件コード="1"
                obj["JOKENCODE"] == COND_CODE_ONE &&
                // 適用期間_from≦システム日時
                obj["TEKIYOYMD_FROM"].CompareTo(sysDate) <= 0 &&
                // システム日時≦【会社別条件マスタ】適用期間_ｔｏ
                obj["TEKIYOYMD_TO"].CompareTo(sysDate) >= 0);

            // 出向元会社コードの文例がなければ、「9999」会社の文例を使う
            if (resData == null)
            {
                resData = apiData.Find(obj =>
                  // 識別区分="82"
                  obj["SHIKIBETSUKBN"] == SHIKIBETSU_KBN_EXAMPLE_NO &&
                  // 会社コード
                  obj["KAISHACODE"] == EMPLOYEE_BASE &&
                  // 条件コード="1"
                  obj["JOKENCODE"] == COND_CODE_ONE &&
                  // 適用期間_from≦システム日時
                  obj["TEKIYOYMD_FROM"].CompareTo(sysDate) <= 0 &&
                  // システム日時≦【会社別条件マスタ】適用期間_ｔｏ
                  obj["TEKIYOYMD_TO"].CompareTo(sysDate) >= 0);
            }

            // 抽出値の戻り
            return resData;
        }

        /// <summary>
        /// APIでEBSから名義名称マスタのデータの取得
        /// </summary>
        /// <param name="apiData">APIでEBSから会社別条件マスタのデータ</param>
        /// <param name="employeeNo">従業員の出向元会社コード</param>
        /// <param name="nameCode">名義区分</param>
        /// <param name="sysDate">システム時間（YYYYMMDD）</param>
        /// <returns>名義名称マスタのデータ</returns>
        private Dictionary<string, string> GetOwnerNameData(
            List<Dictionary<string, string>> apiData,
            string cmopanyNo,
            string nameCode,
            string sysDate)
        {
            // APIで名義名称マスのデータの取得
            Dictionary<string, string> resData = apiData.Find(obj =>
                // 会社コード = 従業員の出向元会社コード
                obj["KAISHACODE"] == cmopanyNo &&
                // 名義区分 = nameCode
                obj["CHOJIMEIGIKBN"] == nameCode &&
                // 適用期間_from≦システム日時
                obj["TEKIYOYMD_FROM"].CompareTo(sysDate) <= 0 &&
                // システム日時≦【名義名称マスタ】適用期間_ｔｏ
                obj["TEKIYOYMD_TO"].CompareTo(sysDate) >= 0);

            return resData;
        }

        /// <summary>
        /// APIでEBSから銀行口座マスタのデータの取得
        /// </summary>
        /// <param name="employeeNo">従業員の出向元会社コード</param>
        /// <returns>銀行口座マスタのデータ</returns>
        private Dictionary<string, string> GetBankInfoByPk(string employeeNo)
        {
            // APIで銀行口座マスタのデータの取得
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SHAINBANGO", employeeNo);

            List<Dictionary<string, string>> apiData =
                WF_AppForm.GetEbsDataWithApi("Get_Bank_Info_By_Pk", dic);

            return apiData.Find(obj => obj["SHAINBANGO"] == employeeNo);
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
            public string CompanyCd { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            public string CompanyNm { get; set; }

            /// <summary>
            /// 所属コード
            /// </summary>
            public string AffiliationCd { get; set; }

            /// <summary>
            /// 所属名
            /// </summary>
            public string AffiliationNm { get; set; }

            /// <summary>
            /// 従業員区分
            /// </summary>
            public string WorkerKbn { get; set; }

            /// <summary>
            /// 出向者
            /// </summary>
            public string SecondedStaffKbn { get; set; }

            /// <summary>
            /// グッドライフ区分
            /// </summary>
            public string GoodLifeKbn { get; set; }

            /// <summary>
            /// 組合区分
            /// </summary>
            public string UnionKbn { get; set; }

            /// <summary>
            /// 申請日From
            /// </summary>
            public string AppDateFrom { get; set; }

            /// <summary>
            /// 申請日To
            /// </summary>
            public string AppDateTo { get; set; }

            /// <summary>
            /// 社員番号
            /// </summary>
            public string EmployeeNo { get; set; }

        }
    }
}
