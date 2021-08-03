using BP.DA;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zipangu;

namespace BP.WF.HttpHandler
{
    public class Mn_PensionbookreissueList : BP.WF.HttpHandler.DirectoryPageBase
    {
        // 未ダウンロード
        const int DL_KBN_MI = 0;
        // ダウンロード済み
        const int DL_KBN_ZUMI = 1;
        // 年金手帳再交付のフローID
        const string WF_ID_ANNUITYNOTEREISSUE = "008";

        /// <summary>
        /// 全角かなから半角かなに変更する項目名の設定
        /// ※固定に設定すること
        /// 　設定のやり方は検索のSQL文のASの別名を合わせないといけない
        /// 　GetCsvSqlメソッドのSQL文
        /// </summary>
        public string[] KanaChg = {
            // 氏名（ｶﾅ）
            "NameKana",
            // 住所カナ
            "AddKana"
        };

        /// <summary>
        /// メモの変更処理
        /// </summary>
        /// <returns></returns>
        public string Memo_Update()
        {
            try
            {
                // パラメータの作成
                Paras ps = new Paras();

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // sql文の作成
                sqlSb.Append("UPDATE TT_WF_ANNUITYNOTEREISSUE");
                sqlSb.Append("   SET MEMO = @Memo");
                sqlSb.Append(" WHERE OID = @WorkID");

                // データを設定
                ps.Add("WorkID", this.GetRequestVal("WorkID"));
                ps.Add("Memo", this.GetRequestVal("Memo"));

                int result = BP.DA.DBAccess.RunSQL(sqlSb.ToString(), ps);

                return result.ToString();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// ダウンロード区分の変更処理
        /// </summary>
        /// <returns></returns>
        public string DownloadKbn_Update()
        {
            try
            {
                // パラメータの作成
                Paras ps = new Paras();

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // sql文の作成
                sqlSb.Append("UPDATE TT_WF_ANNUITYNOTEREISSUE");
                sqlSb.Append("   SET DOWNLOADKBN = @DlKbn");
                sqlSb.Append("      ,DOWNLOAD_LASTDATE = NULL");
                sqlSb.Append("      ,DOWNLOAD_USER = NULL");
                sqlSb.Append("      ,DOWNLOAD_USER_NAME = NULL");
                sqlSb.Append("      ,DL_CLEAR_DATE = CONVERT(NVARCHAR(20),GETDATE(),20)");
                sqlSb.Append("      ,DL_CLEAR_USER = @User");
                sqlSb.Append("      ,DL_CLEAR_USER_NAME = @Username");
                sqlSb.Append(" WHERE OID = @WorkID");

                // データを設定
                ps.Add("WorkID", this.GetRequestVal("WorkID"));
                ps.Add("DlKbn", DL_KBN_MI);
                ps.Add("User", this.GetRequestVal("LoginUserCode"));
                ps.Add("Username", this.GetRequestVal("LoginUserName"));

                int result = BP.DA.DBAccess.RunSQL(sqlSb.ToString(), ps);

                return result.ToString();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 年金手帳再交付詳細データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetPensionbookreissueSyosai()
        {
            try
            {
                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();
                // sql文の作成
                // 伝票番号
                sqlSb.Append("SELECT Ann.OID AS WorkID,");
                // 申請状態
                sqlSb.Append("       Ann.WFState AS AppState,");
                // 申請者社員番号
                sqlSb.Append("       Ann.APPLICANT_SHAINBANGO AS ApplicantShainbango,");
                // 申請理由
                sqlSb.Append("       Ann.APPLICANT_REASON AS ApplicantReason,");
                // DL
                sqlSb.Append("       Ann.DOWNLOADKBN AS DownloadKbn,");
                // DL年月日
                sqlSb.Append("       Ann.DOWNLOAD_LASTDATE AS DownloadDate,");
                // DL使用者名
                sqlSb.Append("       Ann.DOWNLOAD_USER_NAME AS DownloadUsername,");
                // (スナップショット)申請者会社コード元籍
                sqlSb.Append("       Ann.APPLICANT_KAISYACODE AS CompanyCode,");
                // (スナップショット)申請者会社名元籍
                sqlSb.Append("       Ann.APPLICANT_KAISYAMEI AS CompanyName,");
                // (スナップショット)申請者所属名
                sqlSb.Append("       Ann.APPLICANT_SYOZOKUMEI AS ApplicantSyozokumei,");
                // (スナップショット)申請者所属コード
                sqlSb.Append("       Ann.APPLICANT_SYOZOKU_NO AS ApplicantSyozokuCode,");
                // (スナップショット)申請者性別
                sqlSb.Append("       Ann.APPLICANT_SEIBETU AS ApplicantSeibetu,");
                // (スナップショット)申請者生年月日
                sqlSb.Append("       Ann.APPLICANT_BIRTHDAY AS ApplicantBirthday,");
                // 申請者社員番号
                sqlSb.Append("       Ann.APPLICANT_SHAINBANGO AS EmpCode,");
                // (スナップショット)申請者氏名漢字
                sqlSb.Append("       Ann.APPLICANT_KANJIMEI AS EmpNameKanji,");
                // (スナップショット)申請者氏名ｶﾅ
                sqlSb.Append("       Ann.APPLICANT_GANAMEI AS EmpNameKana,");
                // 申請年月日
                sqlSb.Append("       Ann.APPLICANT_LASTDATE AS ApplicationDate, ");
                // (スナップショット)住民票住所郵便番号
                sqlSb.Append("       Ann.JYUMINHYOU_YUBINBANGO AS JyuminhyouYubinbango, ");
                // (スナップショット)住民票住所１
                sqlSb.Append("       Ann.JYUMINHYOU_ADDKANJI1 AS JyuminhyouAddkanji1, ");
                // (スナップショット)住民票住所２
                sqlSb.Append("       Ann.JYUMINHYOU_ADDKANJI2 AS JyuminhyouAddkanji2, ");
                // (スナップショット)住民票住所カナ
                sqlSb.Append("       Ann.JYUMINHYOU_ADDGANA AS JyuminhyouAddgana, ");
                // (スナップショット)住民票電話番号
                sqlSb.Append("       Ann.JYUMINHYOU_TEL AS JyuminhyouTel, ");
                // 基礎年金番号
                sqlSb.Append("       Ann.APPLICANT_ANNUITYNO AS ApplicantAnnuityno, ");
                // 社会保険加入区分
                sqlSb.Append("       Ann.INSURANCEKBN AS Insurancekbn, ");
                // 社会保険取得年月日
                sqlSb.Append("       Ann.INSURANCE_SYUTOKUDATE AS InsuranceSyutokudate, ");
                // メモ
                sqlSb.Append("       Ann.MEMO AS Memo, ");
                // 最終更新日時
                sqlSb.Append("       Ann.REC_EDT_DATE AS ReceptDate,");
                // 最終更新者
                sqlSb.Append("       @shainName AS Receptionist, ");
                // 受託フラグ
                sqlSb.Append("       A.ENTRUSTED_FLG AS Entrustedflg ");
                // 主テーブル
                sqlSb.Append("  FROM TT_WF_ANNUITYNOTEREISSUE Ann ");
                // 連接
                sqlSb.Append(" LEFT JOIN MT_COMPANYACCEPTANCE A");
                sqlSb.Append("    ON Ann.APPLICANT_KAISYACODE = A.CORP_CODE");
                sqlSb.Append(" LEFT JOIN MT_BUSI_WF_REL B");
                sqlSb.Append("    ON B.BUSINESS_CODE = A.BUSINESS_CODE");
                // OID
                sqlSb.Append(" WHERE Ann.OID = @WorkID ");
                sqlSb.Append("   AND B.WF_NO = @wfno");
                sqlSb.Append("    AND (A.DELETE_FLG <> 'X' OR A.DELETE_FLG IS NULL)");
                sqlSb.Append("    AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL)");
                sqlSb.Append("    AND A.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(12), GETDATE(), 112))");
                sqlSb.Append("    AND A.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(12), GETDATE(), 112))");
                sqlSb.Append("    AND B.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(12), GETDATE(), 112))");
                sqlSb.Append("    AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(12), GETDATE(), 112))");
                // パラメータの作成
                Paras ps = new Paras();
                ps.Add("WorkID", this.GetRequestVal("WorkID"));
                ps.Add("wfno", this.GetRequestVal("wf_no")); 
                ps.Add("shainName", this.GetRequestVal("shainName"));
                // sql文の設定
                string sql = sqlSb.ToString();
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
        /// 年金手帳再交付照会一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetArrangeTraderReqList()
        {
            try
            {
                // 検索条件の取得
                ArrangeTraderReq cond =
                    JsonConvert.DeserializeObject<ArrangeTraderReq>(
                        this.GetRequestVal("ArrangeTraderReq"));

                // Sql文と条件設定の取得
                GetArrangeTraderReqListSql(cond, out string sql, out Paras ps);

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
        /// <param name="searchCond"></param>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <returns>sql文</returns>
        private void GetArrangeTraderReqListSql(ArrangeTraderReq searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT Ann.OID AS WorkID,");
            // 申請状態
            sqlSb.Append("       Ann.WFState AS AppState,");
            // DL
            sqlSb.Append("       Ann.DOWNLOADKBN AS DownloadKbn,");
            // DL年月日
            sqlSb.Append("       Ann.DOWNLOAD_LASTDATE AS DownloadDate,");
            // (スナップショット)申請者会社コード元籍
            sqlSb.Append("       Ann.APPLICANT_KAISYACODE AS CompanyCode,");
            // (スナップショット)申請者会社名元籍
            sqlSb.Append("       Ann.APPLICANT_KAISYAMEI AS CompanyName,");
            // 申請者社員番号
            sqlSb.Append("       Ann.APPLICANT_SHAINBANGO AS EmpCode,");
            // (スナップショット)申請者氏名漢字
            sqlSb.Append("       Ann.APPLICANT_KANJIMEI AS EmpNameKanji,");
            // (スナップショット)申請者氏名ｶﾅ
            sqlSb.Append("       Ann.APPLICANT_GANAMEI AS EmpNameKana,");
            // 申請年月日
            sqlSb.Append("       Ann.APPLICANT_LASTDATE AS ApplicationDate ");
            // 主テーブル
            sqlSb.Append("  FROM TT_WF_ANNUITYNOTEREISSUE Ann ");
            // 承認済み
            sqlSb.Append(" WHERE Ann.WFState = 3 ");
            // コメントがなし
            sqlSb.Append("   AND Ann.WFComment IS NULL");

            // パラメータの作成
            ps = new Paras();

            // 画面入力により、条件を作ること
            // ダウンロードがある場合
            if (!string.IsNullOrEmpty(searchCond.DownloadKbn))
            {
                // 完全一致【年金手帳再交付トランザクションテーブル】ダウンロード区分
                sqlSb.Append("       AND Ann.DOWNLOADKBN = @DownloadKbn");

                // ダウンロード 入力条件
                ps.Add("DownloadKbn", searchCond.DownloadKbn);
            }

            // 会社コードがある場合
            if (!string.IsNullOrEmpty(searchCond.CompanyCode))
            {
                // 「受託会社全部」を選択する場合
                if ("ALL" == searchCond.CompanyCode)
                {
                    this.setKaisyacodeByEntrustedFlg(sqlSb, ps);
                }
                else {
                    // 完全一致【年金手帳再交付トランザクションテーブル】(スナップショット)申請者会社コード元籍
                    sqlSb.Append("       AND Ann.APPLICANT_KAISYACODE = @CompanyCode");

                    // 会社コード 入力条件
                    ps.Add("CompanyCode", searchCond.CompanyCode);
                }
            }

            // 社員番号がある場合
            if (!string.IsNullOrEmpty(searchCond.EmpCode))
            {
                // 完全一致【年金手帳再交付トランザクションテーブル】申請者社員番号
                sqlSb.Append("       AND Ann.APPLICANT_SHAINBANGO = @EmpCode");

                // 社員番号 入力条件
                ps.Add("EmpCode", searchCond.EmpCode);
            }

            // 氏名（カナ）がある場合
            if (!string.IsNullOrEmpty(searchCond.EmpNameKana))
            {
                // あいまい一致【年金手帳再交付トランザクションテーブル】(スナップショット)申請者氏名ｶﾅ
                sqlSb.Append("       AND Ann.APPLICANT_GANAMEI LIKE @EmpNameKana");

                // 氏名（カナ）入力条件
                ps.Add("EmpNameKana", "%" + searchCond.EmpNameKana + "%");
            }

            // 氏名（漢字）がある場合
            if (!string.IsNullOrEmpty(searchCond.EmpNameKanji))
            {
                // あいまい一致【年金手帳再交付トランザクションテーブル】(スナップショット)申請者氏名漢字
                sqlSb.Append("       AND Ann.APPLICANT_KANJIMEI LIKE @EmpNameKanji");

                // 氏名（漢字）入力条件
                ps.Add("EmpNameKanji", "%" + searchCond.EmpNameKanji + "%");
            }

            // 申請日 Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateFrom))
            {
                // 完全一致検索【年金手帳再交付トランザクション】申請日
                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8), Ann.APPLICANT_LASTDATE,112)");
                // 申請日 入力条件
                ps.Add("AppDateFrom", searchCond.AppDateFrom);
            }
            // 申請日 Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateTo))
            {
                // 完全一致検索【年金手帳再交付トランザクション】申請日
                sqlSb.Append("       AND CONVERT(nvarchar(8), Ann.APPLICANT_LASTDATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");
                // 申請日 入力条件
                ps.Add("AppDateTo", searchCond.AppDateTo);
            }

            // 受付日 Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.ReceiptDateFrom))
            {
                // 完全一致検索【年金手帳再交付トランザクション】最終更新日時
                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @ReceiptDateFrom),112) <= CONVERT(nvarchar(8), Ann.REC_EDT_DATE,112)");
                // 受付日 入力条件
                ps.Add("ReceiptDateFrom", searchCond.ReceiptDateFrom);
            }
            // 受付日 Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.ReceiptDateTo))
            {
                // 完全一致検索【年金手帳再交付トランザクション】最終更新日時
                sqlSb.Append("       AND CONVERT(nvarchar(8), Ann.REC_EDT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @ReceiptDateTo),112)");
                // 受付日 入力条件
                ps.Add("ReceiptDateTo", searchCond.ReceiptDateTo);
            }

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
                // Sql文と条件設定の取得
                this.GetCsvSql(out string sql, out Paras ps);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                // フロントに戻ること
                string returnData = this.MakeCsvDataInfo(dt, out List<string> workIdList);

                // ダウンロードしたデータの状態の変更
                if (workIdList.Count > 0)
                {
                    // 更新sql文の取得
                    this.UpdateDownloadKbnForComp(workIdList, out string upSql, out Paras upPs);
                    // DBの更新
                    int result = BP.DA.DBAccess.RunSQL(upSql, upPs);
                }

                // CSV結果の戻り
                return returnData;
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
        /// <param name="workIdList">伝票番号リスト（ダウンロード区分更新用）</param>
        /// <returns>出力CSVデータ</returns>
        private string MakeCsvDataInfo(DataTable dt, out List<string> workIdList)
        {
            // 戻りリストの作成
            workIdList = new List<string>();

            // sql文対象の作成
            StringBuilder strCsv = new StringBuilder();

            // タイトルの追加
            strCsv.Append(this.GetCondolenceCsvTilie());

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
                    else
                    {
                        workIdList.Add(row[i].ToString());
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

            return strAft.ToString();
        }

        /// <summary>
        /// CSV出力用sqlの作成
        /// </summary>
        /// <param name="searchCond"></param>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <returns>sql文</returns>
        private void GetCsvSql(out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT Ann.OID AS WorkID,");

            // 会社コード（元籍） - (スナップショット)申請者会社コード元籍
            sqlSb.Append("       Ann.APPLICANT_KAISYACODE AS CompanyCode,");

            // 会社名（元籍） - (スナップショット)申請者会社名元籍
            sqlSb.Append("       Ann.APPLICANT_KAISYAMEI AS CompanyName,");

            // 所属名 - (スナップショット)申請者所属名
            sqlSb.Append("       Ann.APPLICANT_SYOZOKUMEI AS SyozokuMei,");

            // 事業所コード
            sqlSb.Append("       CASE");
            // 会社コード0105のイオンリテールの場合、固定で「010000」とする
            sqlSb.Append("           WHEN Ann.APPLICANT_KAISYACODE = '0105' THEN '010000' ");
            // イオンリテール以外は、「00」＋「会社コード4桁」を設定
            sqlSb.Append("           ELSE '00' + Ann.APPLICANT_KAISYACODE ");
            // 事業所コード
            sqlSb.Append("       END AS JigyoushoCode,");

            // 従業員コード - 申請者社員番号
            sqlSb.Append("       Ann.APPLICANT_SHAINBANGO AS EmpCode,");

            // 氏名（漢字） - (スナップショット)申請者氏名漢字
            sqlSb.Append("       CASE");
            // 会社コード0105のイオンリテールの場合、データを出力。
            sqlSb.Append("           WHEN Ann.APPLICANT_KAISYACODE = '0105' THEN Ann.APPLICANT_KANJIMEI ");
            // 会社コード0105のイオンリテール以外の場合、空白とする。
            sqlSb.Append("           ELSE '' ");
            // 氏名（漢字）
            sqlSb.Append("       END AS NameKanji,");

            // 氏名（カナ） - (スナップショット)申請者氏名ｶﾅ
            sqlSb.Append("       CASE");
            // 会社コード0105のイオンリテールの場合、データを出力。
            sqlSb.Append("           WHEN Ann.APPLICANT_KAISYACODE = '0105' THEN Ann.APPLICANT_GANAMEI ");
            // 会社コード0105のイオンリテール以外の場合、空白とする。
            sqlSb.Append("           ELSE '' ");
            // 氏名（カナ）
            sqlSb.Append("       END AS NameKana,");

            // 性別 - (スナップショット)申請者性別
            sqlSb.Append("       CASE");
            // 会社コード0105のイオンリテールの場合、データを出力。
            sqlSb.Append("           WHEN Ann.APPLICANT_KAISYACODE = '0105' THEN EmpSexKbn.KBNNAME ");
            // 会社コード0105のイオンリテール以外の場合、空白とする。
            sqlSb.Append("           ELSE '' ");
            // 性別
            sqlSb.Append("       END AS Sex,");

            // 生年月日 - (スナップショット)申請者生年月日 YYYY/MM/DDの形式で表示
            sqlSb.Append("       CASE");
            // 会社コード0105のイオンリテールの場合、データを出力。
            sqlSb.Append("           WHEN Ann.APPLICANT_KAISYACODE = '0105' THEN CONVERT(nvarchar(10), CONVERT(datetime, Ann.APPLICANT_BIRTHDAY), 111) ");
            // 会社コード0105のイオンリテール以外の場合、空白とする。
            sqlSb.Append("           ELSE '' ");
            // 生年月日
            sqlSb.Append("       END AS Birthday,");

            // 郵便番号（住民票住所） - (スナップショット)住民票住所郵便番号
            sqlSb.Append("       CASE");
            // 会社コード0105のイオンリテールの場合、データを出力。
            sqlSb.Append("           WHEN LEN(REPLACE(Ann.JYUMINHYOU_YUBINBANGO, '-', '')) > 3 ");
            sqlSb.Append("                THEN LEFT(REPLACE(Ann.JYUMINHYOU_YUBINBANGO, '-', ''), 3) + '-' + SUBSTRING(REPLACE(Ann.JYUMINHYOU_YUBINBANGO, '-', ''), 4, LEN(REPLACE(Ann.JYUMINHYOU_YUBINBANGO, '-', '')) - 3) ");
            // 会社コード0105のイオンリテール以外の場合、空白とする。
            sqlSb.Append("           ELSE Ann.JYUMINHYOU_YUBINBANGO ");
            // 郵便番号
            sqlSb.Append("       END AS ZipCode,");

            // 住所１（住民票住所） - (スナップショット)住民票住所１
            sqlSb.Append("       Ann.JYUMINHYOU_ADDKANJI1 AS AddKanji1,");

            // 住所２（住民票住所） - (スナップショット)住民票住所２
            sqlSb.Append("       Ann.JYUMINHYOU_ADDKANJI2 AS AddKanji2,");

            // 住所カナ - (スナップショット)住民票住所カナ
            sqlSb.Append("       Ann.JYUMINHYOU_ADDGANA AS AddKana,");

            // 電話番号 - (スナップショット)住民票電話番号 TODO 三つ項目を分けるようになるので、対応が必要です。
            sqlSb.Append("       Ann.JYUMINHYOU_TEL AS Telephone,");

            // 基礎年金番号 - 基礎年金番号
            sqlSb.Append("       Ann.APPLICANT_ANNUITYNO AS AnnuityNo,");

            // 社会保険加入区分 - 社会保険加入区分
            sqlSb.Append("       Ann.INSURANCEKBN AS InsuranceKbn,");

            // 申請事由 - 申請理由
            sqlSb.Append("       ReasonKbn.KBNVALUE + '.' + ReasonKbn.KBNNAME AS Reason,");

            // 社会保険取得年月日
            sqlSb.Append("       CONVERT(nvarchar(10), CONVERT(datetime, Ann.INSURANCE_SYUTOKUDATE), 111) AS InsuranceSyutokuDate ");

            // 主テーブル 年金手帳再交付トランザクション
            sqlSb.Append("  FROM TT_WF_ANNUITYNOTEREISSUE Ann ");

            // LEFT JOIN 区分マスタ (性別区分)
            sqlSb.Append("    LEFT JOIN MT_KBN EmpSexKbn ON ");
            // LEFT JOIN 性別区分 条件
            sqlSb.Append("           EmpSexKbn.KBNCODE = 'SEIBETSU_KBN' ");
            sqlSb.Append("       AND EmpSexKbn.KBNVALUE = STR(Ann.APPLICANT_SEIBETU, 1) ");

            // LEFT JOIN 区分マスタ (申請理由区分)
            sqlSb.Append("    LEFT JOIN MT_KBN ReasonKbn ON ");
            // LEFT JOIN 申請理由区分 条件
            sqlSb.Append("           ReasonKbn.KBNCODE = 'ANNUITYNOTEREISSUE_REASON' ");
            sqlSb.Append("       AND ReasonKbn.KBNVALUE = STR(Ann.APPLICANT_REASON, 1) ");

            // 抽出条件の設定
            // 申請状態が「3.受付完了」ということは
            // 申請状態が「3.受付完了」かつ、コメントがなし
            sqlSb.Append("  WHERE Ann.WFState = 3 ");
            // コメントがなし
            sqlSb.Append("        AND Ann.WFComment IS NULL");
            // ダウンロード区分が「0.未ダウンロード」
            sqlSb.Append("        AND Ann.DOWNLOADKBN = 0 ");

            // パラメータの作成
            ps = new Paras();

            // 「年金手帳再交付申請一覧画面」に入れるユーザーが「BS業務部」と「人事部」の権限を持ってないといけない
            this.setKaisyacodeByEntrustedFlg(sqlSb, ps);
            // sql文の設定
            sql = sqlSb.ToString();
        }

        private void setKaisyacodeByEntrustedFlg(StringBuilder sqlSb, Paras ps) {
            // 登録者がBS業務部の場合
            if ("Y" == this.GetRequestVal("EntrustedFlg"))
            {
                // 受託している会社の情報のみに絞り込みを行う
                sqlSb.Append("        AND Ann.APPLICANT_KAISYACODE IN ( ");
                sqlSb.Append("                SELECT C.CORP_CODE AS KAISHACODE FROM MT_COMPANYACCEPTANCE C");
                sqlSb.Append("                    INNER JOIN MT_BUSI_WF_REL B");
                sqlSb.Append("                          ON B.BUSINESS_CODE = C.BUSINESS_CODE");
                sqlSb.Append("                WHERE C.ENTRUSTED_FLG = 'Y' ");
                sqlSb.Append("                      AND B.WF_NO = @wfno");
                sqlSb.Append("                      AND (C.DELETE_FLG <> 'X' OR C.DELETE_FLG IS NULL)");
                sqlSb.Append("                      AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL)");
                sqlSb.Append("                      AND C.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sqlSb.Append("                      AND C.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sqlSb.Append("                      AND B.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sqlSb.Append("                      AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sqlSb.Append("            )");

                ps.Add("wfno", WF_ID_ANNUITYNOTEREISSUE);
            }
            // 以外の場合（人事部）
            else
            {
                // パラメータ設定
                Dictionary<string, object> apiParam = new Dictionary<string, object>();
                apiParam.Add("ShainBango", this.GetRequestVal("LoginUserCode"));

                // グループコード取得
                List<Dictionary<string, string>> rel = WF_AppForm.GetEbsDataWithApi("Get_KaishaCode", apiParam);
                if (rel.Count > 0)
                {
                    // 自社の情報のみを表示を行う
                    sqlSb.Append("        AND Ann.APPLICANT_KAISYACODE IN ( @kaishacode )");
                    ps.Add("kaishacode", rel[0]["KAISHACODE"]);
                }

            }
        }

        /// <summary>
        /// CSV出力タイトルの取得
        /// </summary>
        /// <returns>CSV出力タイトル</returns>
        private string GetCondolenceCsvTilie()
        {
            // csvタイトル対象の作成
            StringBuilder csvTitle = new StringBuilder();

            // 項目番号1 : 伝票番号
            csvTitle.Append("\"伝票番号\"");

            // 項目番号2 : 会社コード（元籍）
            csvTitle.Append(",\"会社コード（元籍）\"");

            // 項目番号3 : 会社名（元籍）
            csvTitle.Append(",\"会社名（元籍）\"");

            // 項目番号4 : 所属名
            csvTitle.Append(",\"所属名\"");

            // 項目番号5 : 事業所コード
            csvTitle.Append(",\"事業所コード\"");

            // 項目番号6 : 従業員コード
            csvTitle.Append(",\"従業員コード\"");

            // 項目番号7 : 氏名（漢字）
            csvTitle.Append(",\"氏名（漢字）\"");

            // 項目番号8 : 氏名（カナ）
            csvTitle.Append(",\"氏名（カナ）\"");

            // 項目番号9 : 性別
            csvTitle.Append(",\"性別\"");

            // 項目番号10 : 生年月日
            csvTitle.Append(",\"生年月日\"");

            // 項目番号11 : 郵便番号（住民票住所）
            csvTitle.Append(",\"郵便番号（住民票住所）\"");

            // 項目番号12 : 住所１（住民票住所）
            csvTitle.Append(",\"住所１（住民票住所）\"");

            // 項目番号13 : 住所２（住民票住所）
            csvTitle.Append(",\"住所２（住民票住所）\"");

            // 項目番号14 : 住所カナ
            csvTitle.Append(",\"住所カナ\"");

            // 項目番号15 : 電話番号
            csvTitle.Append(",\"電話番号\"");

            // 項目番号16 : 基礎年金番号
            csvTitle.Append(",\"基礎年金番号\"");

            // 項目番号17 : 社会保険加入区分
            csvTitle.Append(",\"社会保険加入区分\"");

            // 項目番号18 : 申請事由
            csvTitle.Append(",\"申請事由\"");

            // 項目番号19 : 社会保険取得年月日
            csvTitle.Append(",\"社会保険取得年月日\"");

            return csvTitle.ToString();
        }

        /// <summary>
        /// CSV出力したデータダウンロード区分を未ダウンロードからダウンロード済みに変更する処理
        /// </summary>
        /// <param name="workIdList">伝票番号リスト</param>
        /// <param name="sql">更新のsql文</param>
        /// <param name="ps">更新のパラメータ</param>
        private void UpdateDownloadKbnForComp(List<string> workIdList, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            sqlSb.Append("UPDATE TT_WF_ANNUITYNOTEREISSUE");
            // ダウンロード区分:「１：ダウンロード済み」を設定すること
            sqlSb.Append("    SET TT_WF_ANNUITYNOTEREISSUE.DOWNLOADKBN = @DlKbn,");
            sqlSb.Append("        TT_WF_ANNUITYNOTEREISSUE.DOWNLOAD_USER = @User,");
            sqlSb.Append("        TT_WF_ANNUITYNOTEREISSUE.DOWNLOAD_USER_NAME = @Username,");
            sqlSb.Append("        TT_WF_ANNUITYNOTEREISSUE.DOWNLOAD_LASTDATE = CONVERT(NVARCHAR(20),GETDATE(),20)");
            sqlSb.Append("    WHERE TT_WF_ANNUITYNOTEREISSUE.OID IN (");

            // パラメータの作成
            ps = new Paras();

            // データを設定
            int maxCount = workIdList.Count;
            for (int i = 0; i < maxCount; i++)
            {
                if (i > 0)
                {
                    sqlSb.Append(",");
                }
                sqlSb.Append("@WorkID" + i);

                ps.Add("WorkID" + i, workIdList[i]);
            }

            sqlSb.Append("          )");
            ps.Add("DlKbn", DL_KBN_ZUMI);
            ps.Add("User", this.GetRequestVal("LoginUserCode"));
            ps.Add("Username", this.GetRequestVal("LoginUserName"));
            // ｓｑｌ文の取得
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 年金手帳再交付照会一覧検索条件クラス
        /// </summary>
        private class ArrangeTraderReq
        {
            /// <summary>
            /// 伝票番号
            /// </summary>
            public string WorkID { get; set; }

            /// <summary>
            /// 申請状態
            /// </summary>
            public string AppState { get; set; }

            /// <summary>
            /// ダウンロード
            /// </summary>
            public string DownloadKbn { get; set; }

            /// <summary>
            /// ダウンロード実行日
            /// </summary>
            public string DownloadDate { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            public string CompanyCode { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 社員番号
            /// </summary>
            public string EmpCode { get; set; }

            /// <summary>
            /// 氏名（漢字）
            /// </summary>
            public string EmpNameKanji { get; set; }

            /// <summary>
            /// 氏名（カナ）
            /// </summary>
            public string EmpNameKana { get; set; }

            /// <summary>
            /// 申請日検索条件From
            /// </summary>
            public string AppDateFrom { get; set; }

            /// <summary>
            /// 申請日検索条件To
            /// </summary>
            public string AppDateTo { get; set; }

            /// <summary>
            /// 申請日
            /// </summary>
            public string ApplicationDate { get; set; }

            /// <summary>
            /// 受付日検索条件From
            /// </summary>
            public string ReceiptDateFrom { get; set; }

            /// <summary>
            /// 受付日検索条件To
            /// </summary>
            public string ReceiptDateTo { get; set; }

            /// <summary>
            /// 受付日
            /// </summary>
            public string ReceiptDate { get; set; }

        }
    }
}