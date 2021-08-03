using BP.DA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Zipangu;
using Common.WF_OutLog;

/// <summary>
/// 弔事連絡票照会一覧
/// </summary>
namespace BP.WF.HttpHandler
{
    public class Mn_CondolenceList : BP.WF.HttpHandler.DirectoryPageBase
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
        /// 文例番号設定
        /// ※固定に設定すること
        /// 　設定のやり方は検索のSQL文のASの別名を合わせないといけない
        /// 　GetCsvDetailDataSqlメソッドのSQL文
        /// </summary>
        public Dictionary<string, string> ExampleSentenceNoChg = new Dictionary<string, string> {
            // 会社よりの弔電の文例番号
            {"TyoudenMotoKaisyaCode", "JOKEN_SUCHI_1"},
            // 労働組合よりの弔電の文例番号
            {"TyoudenMotoKumiaiCode","JOKEN_SUCHI_2"},
            // 出向先会社よりの弔電の文例番号
            {"TyoudenSakiKaisyaCode","JOKEN_SUCHI_3"}
        };

        /// <summary>
        /// 一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetCondolenceInqList()
        {
            try
            {
                // 検索条件の取得
                CondolenceInq cond =
                    JsonConvert.DeserializeObject<CondolenceInq>(
                        this.GetRequestVal("CondolenceInq"));

                // Sql文と条件設定の取得
                GetCondolenceInqListSql(cond, out string sql, out Paras ps);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 検索用sqlの作成
        /// </summary>
        /// <param name="searchCond">検索条件</param>
        /// <param name="sql">検索sql文</param>
        /// <param name="ps">検索ｓｑｌ条件</param>
        /// <returns>sql文</returns>
        private void GetCondolenceInqListSql(CondolenceInq searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT Condolence.OID AS WorkID,");

            // 社員番号 - 不幸従業員社員番号
            sqlSb.Append("  Condolence.UNFORTUNATE_SHAINBANGO AS EmployeeNo,");

            // 氏名 - (スナップショット)不幸従業員の社員名(漢字)
            sqlSb.Append("  Condolence.UNFORTUNATE_KANJIMEI AS EmployeeName,");

            // 所属 - (スナップショット)不幸従業員の下
            sqlSb.Append("  Condolence.UNFORTUNATE_SEISHIKISOSHIKISHITA AS DepartmentName,");

            // チーム名
            sqlSb.Append("  Condolence.TEAMMEISHO AS TeamName,");

            // グッドライフ区分 - (スナップショット)不幸従業員のグッドライフ区分名
            sqlSb.Append("  Condolence.UNFORTUNATE_GLCKBN AS GoodlifeKbn,");

            // 組合区分 - (スナップショット)不幸従業員の組合区分名
            sqlSb.Append("  Condolence.UNFORTUNATE_KUMIAIKBN AS KumiaiKbn,");

            // 初回申請日 - 登録日時
            sqlSb.Append("  Condolence.REC_ENT_DATE AS InputDate,");

            // 香料申請日 - 香料申請日
            sqlSb.Append("  Condolence.KOURYOU_SHINNSEIBI AS KouryoDate,");

            // 申請番号
            sqlSb.Append("  orderNum.ORDER_NUMBER AS AppCode ");

            // 弔事連絡トランザクションテーブル
            sqlSb.Append(" FROM TT_WF_CONDOLENCE Condolence");

            // LEFT JOIN 採番マスタ
            sqlSb.Append("    LEFT JOIN TT_WF_ORDER_NUMBER orderNum ON ");
            // LEFT JOIN 会社マスタ 条件
            sqlSb.Append("           orderNum.OID = Condolence.OID ");

            // 条件
            sqlSb.Append(" WHERE 1 = 1 ");

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
                CondolenceInq cond =
                    JsonConvert.DeserializeObject<CondolenceInq>(
                        this.GetRequestVal("CondolenceInq"));

                // Sql文と条件設定の取得
                this.GetCsvDetailDataSql(cond, out string sql, out Paras ps);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                // フロントに戻ること
                return this.MakeCsvDataInfo(dt);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
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
            // 本人の場合、文例番号の取得
            List<Dictionary<string, string>> selfBunnreiInfo = Get_BunnreiCode("1");

            // 本人以外の場合、文例番号の取得
            List<Dictionary<string, string>> selfOutBunnreiInfo = Get_BunnreiCode("2");

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

                    // 特別文字列の変更
                    string csvVal = ConvertStr(row.Table.Columns[i].ColumnName, row[i].ToString());

                    // 文例番号編集
                    csvVal = ExeExampleSentenceNoChg(
                        row.Table.Columns[i].ColumnName,
                        csvVal,
                        selfBunnreiInfo,
                        selfOutBunnreiInfo,
                        row["UnfortunateKaisyacode"].ToString());

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
        private void GetCsvDetailDataSql(CondolenceInq searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 1.伝票番号
            // sqlSb.Append("SELECT Condolence.OID AS WorkID,");
            sqlSb.Append("SELECT orderNum.ORDER_NUMBER AS AppCode,");
            // 2.財務部署コード
            sqlSb.Append("       Condolence.ZAIMUBUSHOCODE AS ZaimubushoCode,");
            // 3.経費負担会社コード
            sqlSb.Append("       Condolence.KEIHIFUTANKAISHACODE AS KeihifutankaiShacode,");
            // 4.経費負担会社名
            sqlSb.Append("       Condolence.KEIHIFUTANKAISHAMEI AS KeihifutankaiShamei,");
            // 5.申請者連絡先メール
            sqlSb.Append("       Condolence.RENRAKUSAKIMAIL AS RenrakusakiMail,");
            // 6.申請者連絡先電話番号
            sqlSb.Append("       Condolence.RENRAKUSAKITEL AS RenrakusakiTel,");
            // 7.申請者区分 区分値→区分名に変換　例：0→本人
            sqlSb.Append("       AppKbn.KBNNAME AS ShinseisyaKbn,");
            // 8.代理申請者社員番号
            sqlSb.Append("       Condolence.DAIRISHINSNEISYA_SHAINBANGO AS ApplicantShainBango,");
            // 9.代理申請者の所属する所属コード
            sqlSb.Append("       Condolence.DAIRISHINSNEISYA_SYOZOKUCODE AS DairishinsneisyaSyozokucode,");
            // 10.代理申請者の所属する所属名称
            sqlSb.Append("       Condolence.DAIRISHINSNEISYA_SYOZOKU AS DairishinsneisyaSyozoku,");
            // 11.代理申請者氏名_姓
            sqlSb.Append("       SUBSTRING(Condolence.DAIRISHINSNEISYA_MEI,0,(select CHARINDEX(' ',Condolence.DAIRISHINSNEISYA_MEI))) AS DairishinsneisyaSei,");
            // 12.代理申請者氏名_名
            sqlSb.Append("       SUBSTRING(Condolence.DAIRISHINSNEISYA_MEI,(select CHARINDEX(' ',Condolence.DAIRISHINSNEISYA_MEI))+1,(select len(Condolence.DAIRISHINSNEISYA_MEI))) AS DairishinsneisyaMei,");
            // 13.ご不幸にあわれた従業員の社員番号
            sqlSb.Append("       Condolence.UNFORTUNATE_SHAINBANGO AS UnfortunateShainbango,");
            // 14.ご不幸にあわれた従業員の氏名_姓
            sqlSb.Append("       SUBSTRING(Condolence.UNFORTUNATE_KANJIMEI,0,(select CHARINDEX(' ',Condolence.UNFORTUNATE_KANJIMEI))) AS UnfortunateSei,");
            // 15.ご不幸にあわれた従業員の氏名_名
            sqlSb.Append("       SUBSTRING(Condolence.UNFORTUNATE_KANJIMEI,(select CHARINDEX(' ',Condolence.UNFORTUNATE_KANJIMEI))+1,(select len(Condolence.UNFORTUNATE_KANJIMEI))) AS UnfortunateMei,");
            // 16.ご不幸にあわれた従業員の出向元会社名
            sqlSb.Append("       Condolence.UNFORTUNATE_KAISYAMEI AS UnfortunateKaisyamei,");
            // 17.ご不幸にあわれた従業員の出向元会社コード
            sqlSb.Append("       Condolence.UNFORTUNATE_KAISYACODE AS UnfortunateKaisyacode,");
            // 18.ご不幸にあわれた従業員の出向先会社名
            sqlSb.Append("       Condolence.UNFORTUNATE_SYUKOSAKIKAISYAMEI AS UnfortunateSyukosakiKaisyamei,");
            // 19.ご不幸にあわれた従業員の出向先会社コード
            sqlSb.Append("       Condolence.UNFORTUNATE_SYUKOSAKIKAISYACODE AS UnfortunateSyukosakiKaisyacode,");
            // 20.ご不幸にあわれた従業員の正式組織名・上
            sqlSb.Append("       Condolence.UNFORTUNATE_SEISHIKISOSHIKIUE AS UnfortunateSeishikisoshikiue,");
            // 21.ご不幸にあわれた従業員の正式組織名・下
            sqlSb.Append("       Condolence.UNFORTUNATE_SEISHIKISOSHIKISHITA AS UnfortunateSeishikisoshishita,");
            // 22.ご不幸にあわれた従業員の所属コード
            sqlSb.Append("       Condolence.UNFORTUNATE_SYOZOKUCODE AS UnfortunateSyozokuCode,");
            // 23.ご不幸にあわれた従業員の職位
            sqlSb.Append("       Condolence.UNFORTUNATE_SYOKUIKBN AS UnfortunateSyokuikbn,");
            // 24.ご不幸にあわれた従業員の社員区分
            sqlSb.Append("       Condolence.UNFORTUNATE_SYAINNKBN AS UnfortunateSyainnkbn,");
            // 25.ご不幸にあわれた従業員の社員区分コード
            sqlSb.Append("       Condolence.UNFORTUNATE_SYAINNKBNCODE AS UnfortunateSyainnkbnCode,");
            // 26.ご不幸にあわれた従業員の組合区分名
            sqlSb.Append("       Condolence.UNFORTUNATE_KUMIAIKBN AS UnfortunateKumiaikbn,");
            // 27.ご不幸にあわれた従業員の組合区分コード
            sqlSb.Append("       Condolence.UNFORTUNATE_KUMIAIKBNCODE AS UnfortunateKumiaikbnCode,");
            // 28.ご不幸にあわれた従業員のＧＬＣ区分
            sqlSb.Append("       Condolence.UNFORTUNATE_GLCKBN AS UnfortunateGlckbn,");
            // 29.ご不幸にあわれた従業員のＧＬＣ区分コード
            sqlSb.Append("       Condolence.UNFORTUNATE_GLCKBNCODE AS UnfortunateGlckbnCode,");
            // 30.ご不幸にあわれた従業員の給与担当コード
            //sqlSb.Append("       '' AS UnfortunateTantouCode,");
            // 31.死亡者の死亡年月日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.DEAD_DATE, 111) AS DeadDate,");
            // 32.死亡者の死亡時刻
            sqlSb.Append("       CASE");
            // 選択不要の場合
            sqlSb.Append("           WHEN Condolence.DEAD_TIME IS NULL THEN NULL");
            // 上記以外の場合
            sqlSb.Append("           ELSE CONCAT(DATEPART(hh,Condolence.DEAD_TIME),':00:00')");
            // 分岐終了
            sqlSb.Append("       END AS DeadTime,");
            // 33.死亡者の氏名_姓_カナ
            sqlSb.Append("       Condolence.DEAD_KANASHIMEI_SEI AS DeadKanashimeiSei,");
            // 34.死亡者の氏名_名_カナ
            sqlSb.Append("       Condolence.DEAD_KANASHIMEI_MEI AS DeadKanashimeiMei,");
            // 35.死亡者の氏名_姓_漢字
            sqlSb.Append("       Condolence.DEAD_SHIMEI_SEI AS DeadShimeiSei,");
            // 36.死亡者の氏名_名_漢字
            sqlSb.Append("       Condolence.DEAD_SHIMEI_MEI AS DeadShimeiMei,");
            // 37.死亡者の従業員との続柄
            sqlSb.Append("       DeadZokugaraKbn.KBNNAME AS DeadZokugara,");
            // 38.死亡者の性別
            sqlSb.Append("       DeadSexKbn.KBNNAME AS DeadSex,");
            // 39.死亡者の年齢
            sqlSb.Append("       Condolence.DEAD_NENREI AS Nenrei,");
            // 40.死亡者の従業員との同居別居区分
            sqlSb.Append("       DeadDokyoBekyoKbn.KBNNAME AS DeadDokyoBekyo,");
            // 41.死亡者の税扶養区分
            sqlSb.Append("       CASE");
            // 選択不要の場合
            sqlSb.Append("           WHEN Condolence.DEAD_FUYOUKBN IS NULL THEN NULL ");
            // 上記以外の場合
            sqlSb.Append("           ELSE DeadFuyouKbn.KBNNAME ");
            // 分岐終了
            sqlSb.Append("       END AS DeadFuyou,");
            // 42.お通夜の有無
            // sqlSb.Append("       TsuyaKokubetsuKbn.KBNNAME AS TsuyaSts,");
            sqlSb.Append("       CASE");
            // 通夜を選択する場合
            sqlSb.Append("           WHEN Condolence.TSUYA_KOKUBETSUSHIKIKBN = 1 THEN 'あり' ");
            // 通夜と告別式を選択する場合
            sqlSb.Append("           WHEN Condolence.TSUYA_KOKUBETSUSHIKIKBN = 3 THEN 'あり' ");
            // 上記以外の場合
            sqlSb.Append("           ELSE 'なし' ");
            // 分岐終了
            sqlSb.Append("       END AS TsuyaSts,");

            // 43.お通夜の会場名のフリガナ
            sqlSb.Append("       Condolence.TSUYA_BASYOUFURIGANA AS TsuyaBasyouFurigana,");
            // 44.お通夜の会場名_漢字
            sqlSb.Append("       Condolence.TSUYA_BASYOUMEI AS TsuyaBasyouMei,");
            // 45.通夜会場電話番号
            sqlSb.Append("       Condolence.TSUYA_RENRAKUSAKITEL AS TsuyaRenrakusakiTel,");
            // 46.お通夜の会場の漢字住所
            sqlSb.Append("       CONCAT(Condolence.TSUYA_ADDRESS1, Condolence.TSUYA_ADDRESS2, Condolence.TSUYA_ADDRESS3) AS TsuyaTodofuken,");
            // 47.お通夜の会場の郵便番号
            sqlSb.Append("       Condolence.TSUYA_YUBINBANGO AS TsuyaYubinBango,");
            // 48.お通夜開催日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.TSUYA_DATE, 111) AS TsuyaDate,");
            // 49.お通夜開催時刻
            sqlSb.Append("       CASE");
            // 空白の場合
            sqlSb.Append("           WHEN Condolence.TSUYA_TIME IS NULL THEN NULL");
            // 上記以外の場合
            sqlSb.Append("           ELSE CONCAT(DATEPART(hh,Condolence.TSUYA_TIME),':00:00')");
            // 分岐終了
            sqlSb.Append("       END AS TsuyaTime,");

            // 50.告別式開催の有無
            // sqlSb.Append("       FuneralKbn.KBNNAME AS FuneralSts,");
            sqlSb.Append("       CASE");
            // 告別式を選択する場合
            sqlSb.Append("           WHEN Condolence.TSUYA_KOKUBETSUSHIKIKBN = 2 THEN 'あり' ");
            // 通夜と告別式を選択する場合
            sqlSb.Append("           WHEN Condolence.TSUYA_KOKUBETSUSHIKIKBN = 3 THEN 'あり' ");
            // 上記以外の場合
            sqlSb.Append("           ELSE 'なし' ");
            // 分岐終了
            sqlSb.Append("       END AS FuneralSts,");

            // 51.告別式の会場名のフリガナ
            sqlSb.Append("       Condolence.KOKUBETSUSHIKI_BASYOUFURIGANA AS KokubetsushikiBasyouFurigana,");
            // 52.告別式の会場名_漢字
            sqlSb.Append("       Condolence.KOKUBETSUSHIKI_BASYOUMEI AS KokubetsushikiBasyouMei,");
            // 53.告別式の会場の電話番号
            sqlSb.Append("       Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL AS KokubetsushikiRenrakusakiTel,");
            // 54.告別式の会場の漢字住所
            sqlSb.Append("       CONCAT(Condolence.KOKUBETSUSHIKI_ADDRESS1, Condolence.KOKUBETSUSHIKI_ADDRESS2, Condolence.KOKUBETSUSHIKI_ADDRESS3) AS KokubetsushikiTodofuken,");
            // 55.告別式の会場の郵便番号
            sqlSb.Append("       Condolence.KOKUBETSUSHIKI_YUBINBANGO AS KokubetsushikiYubinBango,");
            // 56.告別式開催日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.KOKUBETSUSHIKI_DATE, 111) AS KokubetsushikiDate,");
            // 57.告別式開催時刻
            sqlSb.Append("       CASE");
            // 空白の場合
            sqlSb.Append("           WHEN Condolence.KOKUBETSUSHIKI_TIME IS NULL THEN NULL");
            // 上記以外の場合
            sqlSb.Append("           ELSE CONCAT(DATEPART(hh,Condolence.KOKUBETSUSHIKI_TIME),':00:00')");
            // 分岐終了
            sqlSb.Append("       END AS KokubetsushikiTime,");
            // 58.供花お届先区分
            sqlSb.Append("       TodokeSakiKbn.KBNNAME AS TodokeSakiSts,");
            // 59.弔電申込
            sqlSb.Append("       CASE");
            // 通夜の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 0 THEN Condolence.TSUYA_RENRAKUSAKITEL");
            // 告別式の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 1 THEN Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL ");
            // 分岐終了
            sqlSb.Append("       END AS Tel,");
            /*
            // 59.市外局番_弔電申込
            sqlSb.Append("       CASE");
            // 通夜の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 0 THEN SUBSTRING(Condolence.TSUYA_RENRAKUSAKITEL,0,CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL))");
            // 告別式の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 1 THEN SUBSTRING(Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL,0,CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL))");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL ");
            // 分岐終了
            sqlSb.Append("       END AS Tel1,");
            // 60.局番_弔電申込
            sqlSb.Append("       CASE");
            // 通夜の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 0 THEN SUBSTRING(Condolence.TSUYA_RENRAKUSAKITEL,CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL)+1,(CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL,(CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL))+1))-(CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL))-1)");
            // 告別式の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 1 THEN SUBSTRING(Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL,CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL)+1,(CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL,(CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL))+1))-(CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL))-1)");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL ");
            // 分岐終了
            sqlSb.Append("       END AS Tel2,");
            // 61.番号_弔電申込
            sqlSb.Append("       CASE");
            // 通夜の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 0 THEN SUBSTRING(Condolence.TSUYA_RENRAKUSAKITEL,CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL,(CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL,(CHARINDEX('-',Condolence.TSUYA_RENRAKUSAKITEL))+1)))+1,LEN(Condolence.TSUYA_RENRAKUSAKITEL))");
            // 告別式の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 1 THEN SUBSTRING(Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL,CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL,(CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL,(CHARINDEX('-',Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL))+1)))+1,LEN(Condolence.KOKUBETSUSHIKI_RENRAKUSAKITEL))");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL ");
            // 分岐終了
            sqlSb.Append("       END AS Tel3,");
            */
            // 62.後飾りの郵便番号
            sqlSb.Append("       Condolence.ATOKAZARI_YUBINBANGO AS AtokazariYubinBango,");
            // 63.後飾りの漢字住所
            sqlSb.Append("       CONCAT(Condolence.ATOKAZARI_ADDRESS1, Condolence.ATOKAZARI_ADDRESS2, Condolence.ATOKAZARI_ADDRESS3) AS AtokazariTodofuken,");
            // 64.後飾りの連絡先電話番号
            sqlSb.Append("       Condolence.ATOKAZARI_RENRAKUSAKITEL AS AtokazariRenrakusakiTel,");
            // 65.後飾りの配送日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.ATOKAZARI_DATE, 111) AS AtokazariDateTime,");
            // 66.喪主の氏名_姓_カナ
            sqlSb.Append("       Condolence.ORGANIZER_KANASHIMEI_SEI AS OrganizerKanashimeiSei,");
            // 67.喪主の氏名_名_カナ
            sqlSb.Append("       Condolence.ORGANIZER_KANASHIMEI_MEI AS OrganizerKanashimeiMei,");
            // 68.喪主の氏名_姓_漢字
            sqlSb.Append("       Condolence.ORGANIZER_SHIMEI_SEI AS OrganizerShimeiSei,");
            // 69.喪主の氏名_名_漢字
            sqlSb.Append("       Condolence.ORGANIZER_SHIMEI_MEI AS OrganizerShimeiMei,");
            // 70.喪主の従業員との続柄
            sqlSb.Append("       MoshuKbn.KBNNAME AS MoshuKbn,");
            // 71.初回登録日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.REC_ENT_DATE, 111) AS RecEntDate,");
            // 72.香料・供花・弔電差出人_会社名（出向元）
            sqlSb.Append("       Condolence.SASHIDASHI_MOTO_KAISYA1 AS SashidashiMotoKaisya1,");
            // 73.香料・供花・弔電差出人_会社代表者肩書きと氏名
            sqlSb.Append("       Condolence.SASHIDASHI_MOTO_KAISYA2 AS SashidashiMotoKaisya2,");
            // 74.香料・供花・弔電差出人_労働組合名（出向元）
            sqlSb.Append("       Condolence.SASHIDASHI_MOTO_KUMIAI1 AS SashidashiMotoKumiai1,");
            // 75.香料・供花・弔電差出人_労働組合代表者の肩書きと氏名
            sqlSb.Append("       Condolence.SASHIDASHI_MOTO_KUMIAI2 AS SashidashiMotoKumiai2,");
            // 76.香料・供花・弔電差出人_会社名（出向先）
            sqlSb.Append("       Condolence.SASHIDASHI_SAKI_KAISYA1 AS SashidashiMotoKaisya1,");
            // 77.香料・供花・弔電差出人_会社代表者肩書きと氏名（出向先）
            sqlSb.Append("       Condolence.SASHIDASHI_SAKI_KAISYA2 AS SashidashiMotoKaisya2,");
            // 78.香料手配
            sqlSb.Append("       KoryoKbn.KBNNAME AS KoryoSts,");
            // 79.香料申請日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.KOURYOU_SHINNSEIBI, 111) AS KouryouShinnseibi,");
            // 80.GLC側最終更新日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.KOURYOU_GLC_KOUSHINNBI, 111) AS KouryouGlcKoushinnbi,");
            // 81.香料合計
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '0' THEN Condolence.KOURYOU_GOUKEI");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '1' AND Condolence.KOURYOU_GOUKEI IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KouryouGoukei,");
            // 82.出向元会社よりの香料
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '0' THEN Condolence.KOURYOU_MOTO_KAISYA");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '1' AND Condolence.KOURYOU_MOTO_KAISYA IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KouryouMotoKaisya,");
            // 83.出向元労働組合よりの香料
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '0' THEN Condolence.KOURYOU_MOTO_KUMIAI");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '1' AND Condolence.KOURYOU_MOTO_KUMIAI IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KouryouMotoKumiai,");
            // 84.出向先会社よりの香料
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '0' THEN Condolence.KOURYOU_SAKI_KAISYA");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KORYOKBN = '1' AND Condolence.KOURYOU_SAKI_KAISYA IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KouryouSakiKaisya,");
            // 85.供花手配
            sqlSb.Append("       KyokaKbn.KBNNAME AS KyokaSts,");
            // 86.供花・弔電申請日
            sqlSb.Append("       CONVERT(nvarchar(10), Condolence.KUGE_TYOUDEN_SHINNSEIBI, 111) AS KugeTyoudenShinnseibi,");
            // 87.供花差出人_会社（出向元）の供花の数
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = '0' THEN Condolence.KUGE_MOTO_KAISYA");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = '1' AND Condolence.KUGE_MOTO_KAISYA IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KugeMotoKaisya,");
            // 88.供花差出人_労働組合（出向元）の供花の数
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = '0' THEN Condolence.KUGE_MOTO_KUMIAI");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = '1' AND Condolence.KUGE_MOTO_KUMIAI IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KugeMotoKumiai,");
            // 89.供花差出人_会社の（出向先）供花の数
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = '0' THEN Condolence.KUGE_SAKI_KAISYA");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = '1' AND Condolence.KUGE_SAKI_KAISYA IS NOT NULL THEN 0");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS KugeSakiKaisya,");
            // 90.弔電手配
            sqlSb.Append("       TyodenKbn.KBNNAME AS TyodenSts,");
            // 91.会社よりの弔電の有無
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '0' AND Condolence.TYOUDEN_MOTO_KAISYA IS NOT NULL THEN 'あり'");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '1' AND Condolence.TYOUDEN_MOTO_KAISYA IS NOT NULL THEN 'なし'");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS TyoudenMotoKaisya,");
            // 92.会社よりの弔電の文例番号
            sqlSb.Append("       CASE");
            // 会社よりの弔電の有無は必要の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '0' AND Condolence.TYOUDEN_MOTO_KAISYA IS NOT NULL THEN Condolence.DEAD_JUGYOIN_ZOKUGARAKBN");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS TyoudenMotoKaisyaCode,");
            // 93.労働組合よりの弔電の有無
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '0' AND Condolence.TYOUDEN_MOTO_KUMIAI IS NOT NULL THEN 'あり'");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '1' AND Condolence.TYOUDEN_MOTO_KUMIAI IS NOT NULL THEN 'なし'");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS TyoudenMotoKumiai,");
            // 94.労働組合よりの弔電の文例番号
            sqlSb.Append("       CASE");
            // 労働組合よりの弔電の有無は必要の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '0' AND Condolence.TYOUDEN_MOTO_KUMIAI IS NOT NULL THEN Condolence.DEAD_JUGYOIN_ZOKUGARAKBN");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS TyoudenMotoKumiaiCode,");
            // 95.出向先会社よりの弔電の有無
            sqlSb.Append("       CASE");
            // 必要の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '0' AND Condolence.TYOUDEN_SAKI_KAISYA IS NOT NULL THEN 'あり'");
            // 辞退の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '1' AND Condolence.TYOUDEN_SAKI_KAISYA IS NOT NULL THEN 'なし'");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS TyoudenSakiKaisya,");
            // 96.出向先会社よりの弔電の文例番号
            sqlSb.Append("       CASE");
            // 出向先会社よりの弔電の有無は必要の場合
            sqlSb.Append("           WHEN Condolence.TYODENKBN = '0' AND Condolence.TYOUDEN_SAKI_KAISYA IS NOT NULL THEN Condolence.DEAD_JUGYOIN_ZOKUGARAKBN");
            // 上記以外の場合
            sqlSb.Append("           ELSE NULL");
            // 分岐終了
            sqlSb.Append("       END AS TyoudenSakiKaisyaCode");

            // 主テーブル  【弔事連絡票】
            sqlSb.Append("    FROM TT_WF_CONDOLENCE Condolence ");

            // LEFT JOIN 区分マスタ (申請区分)
            sqlSb.Append("    LEFT JOIN MT_KBN AppKbn ON ");
            // LEFT JOIN 申請区分 条件
            sqlSb.Append("           AppKbn.KBNCODE = 'APP_KBN' ");
            sqlSb.Append("       AND CONVERT(int, AppKbn.KBNVALUE) = Condolence.SHINSEISYAKBN ");

            // MT_Employee、MT_Companies、MT_Departmentはccflowの側に削除されること。
            //// LEFT JOIN 従業員マスタ
            //sqlSb.Append("    LEFT JOIN MT_Employee Employee ON ");
            //// LEFT JOIN 申請区分 条件
            //sqlSb.Append("           Employee.SHAINBANGO = Condolence.UNFORTUNATE_SHAINBANGO ");
            //// 申請者区分判断
            //sqlSb.Append("       CASE ");
            //// 0本人
            //sqlSb.Append("           WHEN Condolence.SHINSEISYAKBN = 0 THEN Condolence.UNFORTUNATE_SHAINBANGO ");
            //// 1代理人
            //sqlSb.Append("           WHEN Condolence.SHINSEISYAKBN = 1 THEN Condolence.APPLICANT_SHAINBANGO ");
            //sqlSb.Append("           ELSE '' ");
            //sqlSb.Append("       END");

            //// LEFT JOIN 会社マスタ
            //sqlSb.Append("    LEFT JOIN MT_Companies Companies ON ");
            //// LEFT JOIN 会社コード 条件
            //sqlSb.Append("           Companies.KAISHACODE = Employee.KAISHACODE ");
            //sqlSb.Append("       AND Companies.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(8),GETDATE(),112)) ");
            //sqlSb.Append("       AND Companies.TEKIYOYMD_TO >= (select CONVERT (nvarchar(8),GETDATE(),112)) ");

            //// LEFT JOIN 所属マスタ
            //sqlSb.Append("    LEFT JOIN MT_Department Department ON ");
            //// LEFT JOIN 人事所属コード 条件
            //sqlSb.Append("           Department.SHOZOKUCODE = Employee.JINJISHOZOKUCODE ");
            //sqlSb.Append("       AND Department.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(8),GETDATE(),112)) ");
            //sqlSb.Append("       AND Department.TEKIYOYMD_TO >= (select CONVERT (nvarchar(8),GETDATE(),112)) ");

            //// LEFT JOIN 区分マスタ (性別区分)
            //sqlSb.Append("    LEFT JOIN MT_KBN SexKbn ON ");
            //// LEFT JOIN 性別区分 条件
            //sqlSb.Append("           SexKbn.KBNCODE = 'SEIBETSU_KBN' ");
            //sqlSb.Append("       AND SexKbn.KBNVALUE = STR(Employee.SEIBETSUKBN, 1) ");

            //// LEFT JOIN 区分マスタ (グッドライフ区分)
            //sqlSb.Append("    LEFT JOIN MT_KBN GlcKbn ON ");
            //// LEFT JOIN グッドライフ区分 条件
            //sqlSb.Append("           GlcKbn.KBNCODE = 'GLC_KBN' ");
            //sqlSb.Append("       AND GlcKbn.KBNVALUE = Employee.GOODLIFEKBN ");

            //// LEFT JOIN 区分マスタ (組合区分)
            //sqlSb.Append("    LEFT JOIN MT_KBN KumiaiKbn ON ");
            //// LEFT JOIN 組合区分 条件
            //sqlSb.Append("           KumiaiKbn.KBNCODE = 'KUMIAI_KBN' ");
            //sqlSb.Append("       AND KumiaiKbn.KBNVALUE = STR(Employee.KUMIAIKBN, 1) ");

            // LEFT JOIN 区分マスタ (亡くなられた方続柄区分)
            sqlSb.Append("    LEFT JOIN MT_KBN DeadZokugaraKbn ON ");
            // LEFT JOIN 組合区分 条件
            sqlSb.Append("           DeadZokugaraKbn.KBNCODE = 'DEAD_KBN' ");
            sqlSb.Append("       AND CONVERT(int, DeadZokugaraKbn.KBNVALUE) = Condolence.DEAD_JUGYOIN_ZOKUGARAKBN ");

            // LEFT JOIN 区分マスタ (亡くなられた方性別区分)
            sqlSb.Append("    LEFT JOIN MT_KBN DeadSexKbn ON ");
            // LEFT JOIN 亡くなられた方性別区分 条件
            sqlSb.Append("           DeadSexKbn.KBNCODE = 'SEIBETSU_KBN' ");
            sqlSb.Append("       AND CONVERT(int, DeadSexKbn.KBNVALUE) = Condolence.DEAD_SEIBETSU ");

            // LEFT JOIN 区分マスタ (同居別居区分)
            sqlSb.Append("    LEFT JOIN MT_KBN DeadDokyoBekyoKbn ON ");
            // LEFT JOIN 同居別居区分 条件
            sqlSb.Append("           DeadDokyoBekyoKbn.KBNCODE = 'DOKYO_BEKYO_KBN' ");
            sqlSb.Append("       AND CONVERT(int, DeadDokyoBekyoKbn.KBNVALUE) = Condolence.DEAD_DOKYO_BEKYO ");

            // LEFT JOIN 区分マスタ (扶養区分)
            sqlSb.Append("    LEFT JOIN MT_KBN DeadFuyouKbn ON ");
            // LEFT JOIN 扶養区分 条件
            sqlSb.Append("           DeadFuyouKbn.KBNCODE = 'FUYOUKBN_BEKYO_KBN' ");
            sqlSb.Append("       AND CONVERT(int, DeadFuyouKbn.KBNVALUE) = Condolence.DEAD_FUYOUKBN ");

            // LEFT JOIN 区分マスタ (喪主区分)
            sqlSb.Append("    LEFT JOIN MT_KBN MoshuKbn ON ");
            // LEFT JOIN 喪主区分 条件
            sqlSb.Append("           MoshuKbn.KBNCODE = 'ORGANIZER_KBN' ");
            sqlSb.Append("       AND MoshuKbn.KBNVALUE = Condolence.ORGANIZER_JUGYOIN_ZOKUGARAKBN ");

            //// LEFT JOIN 区分マスタ (通夜区分)
            //sqlSb.Append("    LEFT JOIN MT_KBN TsuyaKbn ON ");
            //// LEFT JOIN 通夜区分 条件
            //sqlSb.Append("           TsuyaKbn.KBNCODE = 'EXIST_KBN' ");
            //sqlSb.Append("       AND TsuyaKbn.KBNVALUE = STR(Condolence.TSUYAKBN, 1) ");

            //// LEFT JOIN 区分マスタ (告別式区分)
            //sqlSb.Append("    LEFT JOIN MT_KBN FuneralKbn ON ");
            //// LEFT JOIN 告別式区分 条件
            //sqlSb.Append("           FuneralKbn.KBNCODE = 'KOKUBETSUSHIKI_KBN' ");
            //sqlSb.Append("       AND FuneralKbn.KBNVALUE = STR(Condolence.KOKUBETSUSHIKIKBN, 1) ");

            // LEFT JOIN 区分マスタ (供花届ける場所区分)
            sqlSb.Append("    LEFT JOIN MT_KBN TodokeSakiKbn ON ");
            // LEFT JOIN 供花届ける場所区分 条件
            sqlSb.Append("           TodokeSakiKbn.KBNCODE = 'TODOKESAKI_KBN' ");
            sqlSb.Append("       AND CONVERT(int, TodokeSakiKbn.KBNVALUE) = Condolence.TODOKESAKIKBN ");

            // LEFT JOIN 区分マスタ (香料発行区分)
            sqlSb.Append("    LEFT JOIN MT_KBN KoryoKbn ON ");
            // LEFT JOIN 香料発行区分 条件
            sqlSb.Append("           KoryoKbn.KBNCODE = 'GLCKORYOKBN' ");
            sqlSb.Append("       AND CONVERT(int, KoryoKbn.KBNVALUE) = Condolence.KORYOKBN ");

            // LEFT JOIN 区分マスタ (供花発行区分)
            sqlSb.Append("    LEFT JOIN MT_KBN KyokaKbn ON ");
            // LEFT JOIN 供花発行区分 条件
            sqlSb.Append("           KyokaKbn.KBNCODE = 'NECESSARY_KBN' ");
            sqlSb.Append("       AND CONVERT(int, KyokaKbn.KBNVALUE) = Condolence.KYOKAKBN ");

            // LEFT JOIN 区分マスタ (弔電発行区分)
            sqlSb.Append("    LEFT JOIN MT_KBN TyodenKbn ON ");
            // LEFT JOIN 弔電発行区分 条件
            sqlSb.Append("           TyodenKbn.KBNCODE = 'NECESSARY_KBN' ");
            sqlSb.Append("       AND CONVERT(int, TyodenKbn.KBNVALUE) = Condolence.TYODENKBN ");

            // LEFT JOIN 採番マスタ
            sqlSb.Append("    LEFT JOIN TT_WF_ORDER_NUMBER orderNum ON ");
            // LEFT JOIN 会社マスタ 条件
            sqlSb.Append("           orderNum.OID = Condolence.OID ");

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
        private void GetSearchCondSql(CondolenceInq searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlCond = new StringBuilder();

            // 条件 (一時保存以外)
            sqlCond.Append("    AND Condolence.WFState > 1 ");

            // パラメータの作成
            ps = new Paras();

            // 画面入力により、条件を作ること
            // 社員番号 社員番号が入力されたら、他の条件が無視にします。
            if (!string.IsNullOrEmpty(searchCond.ShainNo))
            {
                // 社員番号 - 不幸従業員社員番号
                sqlCond.Append("    AND @ShainNo = Condolence.UNFORTUNATE_SHAINBANGO");
                ps.Add("ShainNo", searchCond.ShainNo);
            }
            else
            {
                // 会社コード
                if (!string.IsNullOrEmpty(searchCond.KaishaCd))
                {
                    // 会社コード - (スナップショット)不幸従業員の出向元会社コード
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_KAISYACODE = @KaishaCd");

                    ps.Add("KaishaCd", searchCond.KaishaCd);
                }

                // 会社名 - (スナップショット)不幸従業員の出向元会社名
                if (!string.IsNullOrEmpty(searchCond.KaishaMei))
                {
                    // 会社名 - 
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_KAISYAMEI LIKE @KaishaMei");

                    ps.Add("KaishaMei", "%" + searchCond.KaishaMei + "%");
                }

                // 会社コード＋会社名を空欄で検索の場合は、受託会社全部を検索する
                if (string.IsNullOrEmpty(searchCond.KaishaCd) && string.IsNullOrEmpty(searchCond.KaishaMei))
                {
                    // (スナップショット)不幸従業員の出向元会社コード
                    // 受託している会社の情報のみに絞り込みを行う
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_KAISYACODE IN ( ");
                    sqlCond.Append("            SELECT CompanyAcceptance.CORP_CODE");
                    sqlCond.Append("              FROM MT_COMPANYACCEPTANCE CompanyAcceptance");
                    sqlCond.Append("              INNER JOIN MT_BUSI_WF_REL B");
                    sqlCond.Append("                    ON B.BUSINESS_CODE = CompanyAcceptance.BUSINESS_CODE");
                    sqlCond.Append("            WHERE  CompanyAcceptance.ENTRUSTED_FLG = 'Y' ");
                    sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                    sqlCond.Append("                   AND CompanyAcceptance.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                    sqlCond.Append("                   AND (CompanyAcceptance.DELETE_FLG <> 'X' OR CompanyAcceptance.DELETE_FLG IS NULL)");
                    sqlCond.Append("                   AND B.WF_NO = '009'");
                    sqlCond.Append("                   AND B.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                    sqlCond.Append("                   AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                    sqlCond.Append("                   AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL)");
                    sqlCond.Append("            )");
                }

                // 所属コード
                if (!string.IsNullOrEmpty(searchCond.SyozokuCd))
                {
                    // 所属コード - (スナップショット)不幸者の所属コード
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_SYOZOKUCODE = @SyozokuCd");
                    ps.Add("SyozokuCd", searchCond.SyozokuCd);
                }

                // 所属名
                if (!string.IsNullOrEmpty(searchCond.SyozokuMei))
                {
                    // 所属名 - (スナップショット)不幸従業員の下
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_SEISHIKISOSHIKISHITA LIKE @SyozokuMei");
                    ps.Add("SyozokuMei", "%" + searchCond.SyozokuMei + "%");
                }

                // 従業員区分
                if (!string.IsNullOrEmpty(searchCond.JyugyoinKbn))
                {
                    // 従業員区分 - (スナップショット)不幸者の社員区分コード
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_SYAINNKBNCODE = @JyugyoinKbn");
                    ps.Add("JyugyoinKbn", searchCond.JyugyoinKbn);
                }

                // 出向者区分(1:出向者,0:非出向者)
                if (!string.IsNullOrEmpty(searchCond.SyukousyaKbn))
                {
                    // 出向者区分 - 出向フラグ
                    sqlCond.Append("    AND Condolence.SHUKKOFLG = @SyukousyaKbn");
                    ps.Add("SyukousyaKbn", searchCond.SyukousyaKbn);
                }

                // グッドライフ区分
                if (!string.IsNullOrEmpty(searchCond.GoodlifeKbn))
                {
                    // グッドライフ区分 - (スナップショット)不幸従業員のグッドライフ区分コード
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_GLCKBNCODE = @GoodlifeKbn");
                    ps.Add("GoodlifeKbn", searchCond.GoodlifeKbn);
                }

                // 組合区分
                if (!string.IsNullOrEmpty(searchCond.KumiaiKbn))
                {
                    // 組合区分 - (スナップショット)不幸者の組合区分コード
                    sqlCond.Append("    AND Condolence.UNFORTUNATE_KUMIAIKBNCODE = @KumiaiKbn");
                    ps.Add("KumiaiKbn", searchCond.KumiaiKbn);
                }

                // 初回申請日
                // Fromのみがある場合
                if (!string.IsNullOrEmpty(searchCond.TorokuymdFrom))
                {
                    // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                    sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @TorokuymdFrom),112) <= CONVERT(nvarchar(8),Condolence.REC_ENT_DATE,112)");

                    // 入力条件
                    ps.Add("TorokuymdFrom", searchCond.TorokuymdFrom);
                }

                // Toのみがある場合
                if (!string.IsNullOrEmpty(searchCond.TorokuymdTo))
                {
                    // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                    sqlCond.Append("    AND CONVERT(nvarchar(8),Condolence.REC_ENT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @TorokuymdTo),112)");

                    // 入力条件
                    ps.Add("TorokuymdTo", searchCond.TorokuymdTo);
                }

                // 香料申請日
                // Fromのみがある場合
                if (!string.IsNullOrEmpty(searchCond.KouryoymdFrom))
                {
                    // 年月日の完全一致検索【弔事連絡トランザクションテーブル】香料申請日
                    sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @KouryoymdFrom),112) <= CONVERT(nvarchar(8),Condolence.KOURYOU_SHINNSEIBI,112)");

                    // 入力条件
                    ps.Add("KouryoymdFrom", searchCond.KouryoymdFrom);
                }

                // Toのみがある場合
                if (!string.IsNullOrEmpty(searchCond.KouryoymdTo))
                {
                    // 年月日の完全一致検索【弔事連絡トランザクションテーブル】香料申請日
                    sqlCond.Append("    AND CONVERT(nvarchar(8),Condolence.KOURYOU_SHINNSEIBI,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @KouryoymdTo),112)");

                    // 入力条件
                    ps.Add("KouryoymdTo", searchCond.KouryoymdTo);
                }
            }

            // 条件の取得
            sql = sqlCond.ToString();
        }

        /// <summary>
        /// CSV出力タイトルの取得
        /// </summary>
        /// <returns>CSV出力タイトル</returns>
        private string GetCondolenceCsvTilie()
        {
            // csvタイトル対象の作成
            StringBuilder csvTitle = new StringBuilder();

            // 項目番号1 : 帳票ＩＤ
            csvTitle.Append("\"帳票ＩＤ\"");

            // 項目番号2 : 財務部署コード
            csvTitle.Append(",\"財務部署コード\"");

            // 項目番号3 : 経費負担会社コード
            csvTitle.Append(",\"経費負担会社コード\"");

            // 項目番号4 : 経費負担会社名
            csvTitle.Append(",\"経費負担会社名\"");

            // 項目番号5 : 申請者連絡先メール
            csvTitle.Append(",\"申請者連絡先メール\"");

            // 項目番号6 : 申請者連絡電話番号
            csvTitle.Append(",\"申請者連絡電話番号\"");

            // 項目番号7 : 申請者区分
            csvTitle.Append(",\"申請者区分\"");

            // 項目番号8 : 代理申請者社員番号
            csvTitle.Append(",\"代理申請者社員番号\"");

            // 項目番号9 : 代理申請者の所属する所属コード
            csvTitle.Append(",\"代理申請者の所属する所属コード\"");

            // 項目番号10 : 代理申請者の所属する所属名称
            csvTitle.Append(",\"代理申請者の所属する所属名称\"");

            // 項目番号11 : 代理申請者氏名_姓
            csvTitle.Append(",\"代理申請者氏名_姓\"");

            // 項目番号12 : 代理申請者氏名_名
            csvTitle.Append(",\"代理申請者氏名_名\"");

            // 項目番号13 : ご不幸にあわれた従業員の社員番号
            csvTitle.Append(",\"ご不幸にあわれた従業員の社員番号\"");

            // 項目番号14 : ご不幸にあわれた従業員の氏名_姓
            csvTitle.Append(",\"ご不幸にあわれた従業員の氏名_姓\"");

            // 項目番号15 : ご不幸にあわれた従業員の氏名_名
            csvTitle.Append(",\"ご不幸にあわれた従業員の氏名_名\"");

            // 項目番号16 : ご不幸にあわれた従業員の出向元会社名
            csvTitle.Append(",\"ご不幸にあわれた従業員の出向元会社名\"");

            // 項目番号17 : ご不幸にあわれた従業員の出向元会社コード
            csvTitle.Append(",\"ご不幸にあわれた従業員の出向元会社コード\"");

            // 項目番号18 : ご不幸にあわれた従業員の出向先会社名
            csvTitle.Append(",\"ご不幸にあわれた従業員の出向先会社名\"");

            // 項目番号19 : ご不幸にあわれた従業員の出向先会社コード
            csvTitle.Append(",\"ご不幸にあわれた従業員の出向先会社コード\"");

            // 項目番号20 : ご不幸にあわれた従業員の正式組織名・上
            csvTitle.Append(",\"ご不幸にあわれた従業員の正式組織名・上\"");

            // 項目番号21 : ご不幸にあわれた従業員の正式組織名・下
            csvTitle.Append(",\"ご不幸にあわれた従業員の正式組織名・下\"");

            // 項目番号22 : ご不幸にあわれた従業員の所属コード
            csvTitle.Append(",\"ご不幸にあわれた従業員の所属コード\"");

            // 項目番号23 : ご不幸にあわれた従業員の職位
            csvTitle.Append(",\"ご不幸にあわれた従業員の職位\"");

            // 項目番号24 : ご不幸にあわれた従業員の社員区分
            csvTitle.Append(",\"ご不幸にあわれた従業員の社員区分\"");

            // 項目番号25 : ご不幸にあわれた従業員の社員区分コード
            csvTitle.Append(",\"ご不幸にあわれた従業員の社員区分コード\"");

            // 項目番号26 : ご不幸にあわれた従業員の組合区分名
            csvTitle.Append(",\"ご不幸にあわれた従業員の組合区分名\"");

            // 項目番号27 : ご不幸にあわれた従業員の組合区分コード
            csvTitle.Append(",\"ご不幸にあわれた従業員の組合区分コード\"");

            // 項目番号28 : ご不幸にあわれた従業員のＧＬＣ区分
            csvTitle.Append(",\"ご不幸にあわれた従業員のＧＬＣ区分\"");

            // 項目番号29 : ご不幸にあわれた従業員のＧＬＣ区分コード
            csvTitle.Append(",\"ご不幸にあわれた従業員のＧＬＣ区分コード\"");

            // 項目番号30 : ご不幸にあわれた従業員の給与担当コード
            //csvTitle.Append(",\"ご不幸にあわれた従業員の給与担当コード\"");

            // 項目番号31 : 死亡者の死亡年月日
            csvTitle.Append(",\"死亡者の死亡年月日\"");

            // 項目番号32 : 死亡者の死亡時刻
            csvTitle.Append(",\"死亡者の死亡時刻\"");

            // 項目番号33 : 死亡者の氏名_姓_カナ
            csvTitle.Append(",\"死亡者の氏名_姓_カナ\"");

            // 項目番号34 : 死亡者の氏名_名_カナ
            csvTitle.Append(",\"死亡者の氏名_名_カナ\"");

            // 項目番号35 : 死亡者の氏名_姓_漢字
            csvTitle.Append(",\"死亡者の氏名_姓_漢字\"");

            // 項目番号36 : 死亡者の氏名_名_漢字
            csvTitle.Append(",\"死亡者の氏名_名_漢字\"");

            // 項目番号37 : 死亡者の従業員との続柄
            csvTitle.Append(",\"死亡者の従業員との続柄\"");

            // 項目番号38 : 死亡者の性別
            csvTitle.Append(",\"死亡者の性別\"");

            // 項目番号39 : 死亡者の年齢
            csvTitle.Append(",\"死亡者の年齢\"");

            // 項目番号40 : 死亡者の従業員との同居別居区分
            csvTitle.Append(",\"死亡者の従業員との同居別居区分\"");

            // 項目番号41 : 死亡者の税扶養区分
            csvTitle.Append(",\"死亡者の税扶養区分\"");

            // 項目番号42 : お通夜の有無
            csvTitle.Append(",\"お通夜の有無\"");

            // 項目番号43 : お通夜の会場名のフリガナ
            csvTitle.Append(",\"お通夜の会場名のフリガナ\"");

            // 項目番号44 : お通夜の会場名_漢字
            csvTitle.Append(",\"お通夜の会場名_漢字\"");

            // 項目番号45 : お通夜の会場の電話番号
            csvTitle.Append(",\"お通夜の会場の電話番号\"");

            // 項目番号46 : お通夜の会場の漢字住所
            csvTitle.Append(",\"お通夜の会場の漢字住所\"");

            // 項目番号47 : お通夜の会場の郵便番号
            csvTitle.Append(",\"お通夜の会場の郵便番号\"");

            // 項目番号48 : お通夜開催日
            csvTitle.Append(",\"お通夜開催日\"");

            // 項目番号49 : お通夜開催時刻
            csvTitle.Append(",\"お通夜開催時刻\"");

            // 項目番号50 : 告別式開催の有無
            csvTitle.Append(",\"告別式開催の有無\"");

            // 項目番号51 : 告別式の会場名のフリガナ
            csvTitle.Append(",\"告別式の会場名のフリガナ\"");

            // 項目番号52 : 告別式の会場名_漢字
            csvTitle.Append(",\"告別式の会場名_漢字\"");

            // 項目番号53 : 告別式の会場の電話番号
            csvTitle.Append(",\"告別式の会場の電話番号\"");

            // 項目番号54 : 告別式の会場の漢字住所
            csvTitle.Append(",\"告別式の会場の漢字住所\"");

            // 項目番号55 : 告別式の会場の郵便番号
            csvTitle.Append(",\"告別式の会場の郵便番号\"");

            // 項目番号56 : 告別式開催日
            csvTitle.Append(",\"告別式開催日\"");

            // 項目番号57 : 告別式開催時刻
            csvTitle.Append(",\"告別式開催時刻\"");

            // 項目番号58 : 供花お届先区分
            csvTitle.Append(",\"供花お届先区分\"");

            // 項目番号59 : 弔電申込
            csvTitle.Append(",\"電話番号_弔電申込\"");
            /*
            // 項目番号59 : 市外局番_弔電申込
            csvTitle.Append(",\"市外局番_弔電申込\"");

            // 項目番号60 : 局番_弔電申込
            csvTitle.Append(",\"局番_弔電申込\"");

            // 項目番号61 : 番号_弔電申込
            csvTitle.Append(",\"番号_弔電申込\"");
            */

            // 項目番号62 : 後飾りの郵便番号
            csvTitle.Append(",\"後飾りの郵便番号\"");

            // 項目番号63 : 後飾りの漢字住所
            csvTitle.Append(",\"後飾りの漢字住所\"");

            // 項目番号64 : 後飾りの連絡先電話番号
            csvTitle.Append(",\"後飾りの連絡先電話番号\"");

            // 項目番号65 : 後飾りの配送日
            csvTitle.Append(",\"後飾りの配送日\"");

            // 項目番号66 : 喪主の氏名_姓_カナ
            csvTitle.Append(",\"喪主の氏名_姓_カナ\"");

            // 項目番号67 : 喪主の氏名_名_カナ
            csvTitle.Append(",\"喪主の氏名_名_カナ\"");

            // 項目番号68 : 喪主の氏名_姓_漢字
            csvTitle.Append(",\"喪主の氏名_姓_漢字\"");

            // 項目番号69 : 喪主の氏名_名_漢字
            csvTitle.Append(",\"喪主の氏名_名_漢字\"");

            // 項目番号70 : 喪主の従業員との続柄
            csvTitle.Append(",\"喪主の従業員との続柄\"");

            // 項目番号71 : 初回登録日
            csvTitle.Append(",\"初回登録日\"");

            // 項目番号72 : 香料・供花・弔電差出人_会社名（出向元）
            csvTitle.Append(",\"香料・供花・弔電差出人_会社名（出向元）\"");

            // 項目番号73 : 香料・供花・弔電差出人_会社代表者肩書きと氏名
            csvTitle.Append(",\"香料・供花・弔電差出人_会社代表者肩書きと氏名\"");

            // 項目番号74 : 香料・供花・弔電差出人_労働組合名（出向元）
            csvTitle.Append(",\"香料・供花・弔電差出人_労働組合名（出向元）\"");

            // 項目番号75 : 香料・供花・弔電差出人_労働組合代表者の肩書きと氏名
            csvTitle.Append(",\"香料・供花・弔電差出人_労働組合代表者の肩書きと氏名\"");

            // 項目番号76 : 香料・供花・弔電差出人_会社名（出向先）
            csvTitle.Append(",\"香料・供花・弔電差出人_会社名（出向先）\"");

            // 項目番号77 : 香料・供花・弔電差出人_会社代表者肩書きと氏名（出向先）
            csvTitle.Append(",\"香料・供花・弔電差出人_会社代表者肩書きと氏名（出向先）\"");

            // 項目番号78 : 香料手配
            csvTitle.Append(",\"香料手配\"");

            // 項目番号79 : 香料申請日
            csvTitle.Append(",\"香料申請日\"");

            // 項目番号80 : GLC側最終更新日
            csvTitle.Append(",\"GLC側最終更新日\"");

            // 項目番号81 : 香料合計
            csvTitle.Append(",\"香料合計\"");

            // 項目番号82 : 出向元会社よりの香料
            csvTitle.Append(",\"出向元会社よりの香料\"");

            // 項目番号83 : 出向元労働組合よりの香料
            csvTitle.Append(",\"出向元労働組合よりの香料\"");

            // 項目番号84 : 出向先会社よりの香料
            csvTitle.Append(",\"出向先会社よりの香料\"");

            // 項目番号85 : 供花手配
            csvTitle.Append(",\"供花手配\"");

            // 項目番号86 : 供花・弔電申請日
            csvTitle.Append(",\"供花・弔電申請日\"");

            // 項目番号87 : 供花差出人_会社（出向元）の供花の数
            csvTitle.Append(",\"供花差出人_会社（出向元）の供花の数\"");

            // 項目番号88 : 供花差出人_労働組合（出向元）の供花の数
            csvTitle.Append(",\"供花差出人_労働組合（出向元）の供花の数\"");

            // 項目番号89 : 供花差出人_会社の（出向先）供花の数
            csvTitle.Append(",\"供花差出人_会社の（出向先）供花の数\"");

            // 項目番号90 : 弔電手配
            csvTitle.Append(",\"弔電手配\"");

            // 項目番号91 : 会社よりの弔電の有無
            csvTitle.Append(",\"会社よりの弔電の有無\"");

            // 項目番号92 : 会社よりの弔電の文例番号
            csvTitle.Append(",\"会社よりの弔電の文例番号\"");

            // 項目番号93 : 労働組合よりの弔電の有無
            csvTitle.Append(",\"労働組合よりの弔電の有無\"");

            // 項目番号94 : 労働組合よりの弔電の文例番号
            csvTitle.Append(",\"労働組合よりの弔電の文例番号\"");

            // 項目番号95 : 出向先会社よりの弔電の有無
            csvTitle.Append(",\"出向先会社よりの弔電の有無\"");

            // 項目番号96 : 出向先会社よりの弔電の文例番号
            csvTitle.Append(",\"出向先会社よりの弔電の文例番号\"");

            return csvTitle.ToString();
        }

        /// <summary>
        /// 文例番号を取得
        /// </summary>
        /// <returns></returns>
        private List<Dictionary<string, string>> Get_BunnreiCode(string jokencode)
        {
            // 検索条件の設定
            Dictionary<string, object> dicCondi = new Dictionary<string, object>();
            dicCondi.Add("SHIKIBETSUKBN", "65");
            dicCondi.Add("JOKENCODE", jokencode);
            List<Dictionary<string, string>> ret =
                WF_AppForm.GetEbsDataWithApi("Get_Conditions_KeyForDiscAndCond_Data", dicCondi);

            // フロントに戻ること
            return ret;
        }

        /// <summary>
        /// 文例番号編集
        /// <param name="key">変数名</param>
        /// <param name="val">変数値</param>
        /// <param name="selfBunnreiInfo">本人の文例番号の集合</param>
        /// <param name="selfOutBunnreiInfo">本人以外の文例番号の集合</param>
        /// <param name="cmopanyNo">会社コード</param>
        /// <returns>編集後のデータ</returns>
        /// </summary>
        private string ExeExampleSentenceNoChg(
            string key,
            string val,
            List<Dictionary<string, string>> selfBunnreiInfo,
            List<Dictionary<string, string>> selfOutBunnreiInfo,
            string cmopanyNo) 
        {
            // 戻り値
            string resVal = val;

            // 文例番号変更すること
            if (ExampleSentenceNoChg.ContainsKey(key))
            {
                Dictionary<string, string> resDic = null;
                // 本人の場合
                if (val == "1")
                {
                    // APIで会社別条件マスタのデータの取得
                    resDic = selfBunnreiInfo.Find(obj =>
                        // 会社コード=従業員の出向元会社コード
                        obj["KAISHACODE"] == cmopanyNo);
                }
                // 本人以外の場合
                else
                {
                    // APIで会社別条件マスタのデータの取得
                    resDic = selfOutBunnreiInfo.Find(obj =>
                        // 会社コード=従業員の出向元会社コード
                        obj["KAISHACODE"] == cmopanyNo);
                }

                if (resDic != null && resDic.Count > 0)
                {
                    resVal = resDic[ExampleSentenceNoChg[key]];
                }
                else
                {
                    resVal = String.Empty;
                }
            }

            return resVal;
        }

        /// <summary>
        /// 一覧検索条件クラス
        /// </summary>
        [DataContract]
        private class CondolenceInq
        {
            /// <summary>
            /// 会社コード
            /// </summary>
            [DataMember(Name = "company_code_search")]
            public string KaishaCd { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            [DataMember(Name = "company_name_search")]
            public string KaishaMei { get; set; }

            /// <summary>
            /// 所属コード
            /// </summary>
            [DataMember(Name = "belongs_code_search")]
            public string SyozokuCd { get; set; }

            /// <summary>
            /// 所属名
            /// </summary>
            [DataMember(Name = "belongs_name_search")]
            public string SyozokuMei { get; set; }

            /// <summary>
            /// 従業員区分
            /// </summary>
            [DataMember(Name = "emp_kbn_search")]
            public string JyugyoinKbn { get; set; }

            /// <summary>
            /// 出向者
            /// </summary>
            [DataMember(Name = "loaned_staff_search")]
            public string SyukousyaKbn { get; set; }

            /// <summary>
            /// グッドライフ区分
            /// </summary>
            [DataMember(Name = "glc_kbn_search")]
            public string GoodlifeKbn { get; set; }

            /// <summary>
            /// 組合区分
            /// </summary>
            [DataMember(Name = "union_kbn_search")]
            public string KumiaiKbn { get; set; }

            /// <summary>
            /// 初回申請日From
            /// </summary>
            [DataMember(Name = "app_date_search_from")]
            public string TorokuymdFrom { get; set; }

            /// <summary>
            /// 初回申請日To
            /// </summary>
            [DataMember(Name = "app_date_search_to")]
            public string TorokuymdTo { get; set; }

            /// <summary>
            /// 香料申請日From
            /// </summary>
            [DataMember(Name = "spice_app_date_search_from")]
            public string KouryoymdFrom { get; set; }

            /// <summary>
            /// 香料申請日To
            /// </summary>
            [DataMember(Name = "spice_app_date_search_to")]
            public string KouryoymdTo { get; set; }

            /// <summary>
            /// 社員番号
            /// </summary>
            [DataMember(Name = "emp_code_search")]
            public string ShainNo { get; set; }

        }
    }
}
