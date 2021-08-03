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
/// 本人情報変更_照会一覧と詳細画面
/// </summary>
namespace BP.WF.HttpHandler
{
    public class WF_PersonalInfoChangeinquiry : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();
        // 本人情報変更_照会一覧と詳細画面のフローID
        const string WF_ID_PERSONALINFOCHANGE = "018";

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
        public string UpdatePersonalInfoChange()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "UPDATE TT_WF_PERSONAL_INFO_CHANGE SET " +
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
                    "REC_EDT_USER=@REC_EDT_USER, " +
                    "COMMENT=@COMMENT " +
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
                ps.Add("CHECK_DATE", this.GetRequestVal("CHECK_DATE")); 
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_USER", this.GetRequestVal("CHECK_EMP_NUM"));
                ps.Add("OID", this.GetRequestVal("OID"));
                ps.Add("COMMENT", this.GetRequestVal("COMMENT"));

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
        /// 確認コメントメソッド
        /// </summary>
        /// <returns></returns>
        public string UpdatePersonalInfoChangeComent()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "UPDATE TT_WF_PERSONAL_INFO_CHANGE SET " +
                    "COMMENT=@COMMENT, " +
                    "REC_EDT_DATE=@REC_EDT_DATE, " +
                    "REC_EDT_USER=@REC_EDT_USER " +
                    "WHERE OID=@OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("COMMENT", this.GetRequestVal("COMMENT"));
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
        /// DBデータの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetPersonalData()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT * FROM TT_WF_PERSONAL_INFO_CHANGE WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("WorkId"));

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
            // --確認状態エリア
            // --確認状態
            sqlSb.Append("SELECT  PersonalInfo.CHECK_FLG,chkKbn.KBNNAME AS CheckKbn_confirm,");
            // --確認日時
            sqlSb.Append("  FORMAT(PersonalInfo.CHECK_DATE,'yyyy年MM月dd日 HH時mm分') AS ChkDate_confirm, ");
            // --確認状態
            sqlSb.Append("  PersonalInfo.CHECK_FLG,chkKbn.KBNNAME AS CheckKbn_confirm2,");
            // --確認日時
            sqlSb.Append("  FORMAT(PersonalInfo.CHECK_DATE,'yyyy年MM月dd日 HH時mm分') AS ChkDate_confirm2, ");
            // --確認者　会社
            sqlSb.Append("  PersonalInfo.CHECK_CORP_CODE+'　'+CHECK_CORP_NAME AS ChkCorpName_confirm,");
            // --人事所属コード
            sqlSb.Append("  PersonalInfo.CHECK_DEPT_CODE AS ChkDepartmentCode_confirm,");
            // --所属名
            sqlSb.Append("  PersonalInfo.CHECK_DEPT_NAME AS ChkDepartmentName_confirm,");
            // --社員番号
            sqlSb.Append("  PersonalInfo.CHECK_EMP_NUM AS ChkEmployeeNo_confirm,");
            // --姓・漢字
            sqlSb.Append("  PersonalInfo.AFTER_LAST_NAME_KANJI AS ChkEmployeeLName_confirm,");
            // --名・漢字
            sqlSb.Append("  PersonalInfo.AFTER_FIRST_NAME_KANJI AS ChkEmployeeFName_confirm,");
            // --確認コメント
            sqlSb.Append("  PersonalInfo.COMMENT AS ChkCommont_confirm,");

            // --本人情報変更エリア
            // --伝票番号
            sqlSb.Append(" PersonalInfo.OID AS oid_confirm,");
            // --申請日
            sqlSb.Append("  SUBSTRING(PersonalInfo.APPLY_DATE, 1, 4) + '年' + SUBSTRING(PersonalInfo.APPLY_DATE, 5, 2) + '月' + SUBSTRING(PersonalInfo.APPLY_DATE, 7, 2) + '日' AS appdate_confirm,");           
            // --修正理由区分
            sqlSb.Append(" PersonalInfo.REASON_CODE,reasoncode.KBNNAME AS reasonCode_confirm,");
            // --異動日
            sqlSb.Append("  CASE WHEN PersonalInfo.CORRECTION_DATE IS NULL THEN ''  ELSE SUBSTRING(PersonalInfo.CORRECTION_DATE,1,4)+'年' + SUBSTRING(PersonalInfo.CORRECTION_DATE, 5, 2) + '月' + SUBSTRING(PersonalInfo.CORRECTION_DATE, 7, 2) + '日' END AS correctionDate_confirm,");            
            // --会社コード
            sqlSb.Append(" PersonalInfo.COMPANY_CODE AS companyCode_confirm,");
            // --会社名
            sqlSb.Append(" PersonalInfo.CMOPANY_NAME AS companyName_confirm,");
            // --人事所属コード
            sqlSb.Append(" PersonalInfo.DEPT_NO AS sozoku_code_confirm,");
            // --所属
            sqlSb.Append(" PersonalInfo.DEPT_NAME AS sozoku_name_confirm,");
            // --社員番号
            sqlSb.Append(" PersonalInfo.EMPLOYEE_NO AS kaisyano_confirm,");
            // --姓・漢字
            sqlSb.Append("  PersonalInfo.AFTER_LAST_NAME_KANJI AS kanji_sei_confirm,");
            // --姓・カナ
            sqlSb.Append("  PersonalInfo.AFTER_LAST_NAME_KANA AS kana_sei_confirm,");
            // --名・漢字
            sqlSb.Append("  PersonalInfo.AFTER_FIRST_NAME_KANJI AS kanji_mei_confirm,");
            // --名・カナ
            sqlSb.Append("  PersonalInfo.AFTER_FIRST_NAME_KANA AS kana_mei_confirm,");
            // --生年月日
            sqlSb.Append("  SUBSTRING(PersonalInfo.AFTER_BIRTHDAY,1,4)+'年' + SUBSTRING(PersonalInfo.AFTER_BIRTHDAY, 5, 2) + '月' + SUBSTRING(PersonalInfo.AFTER_BIRTHDAY, 7, 2) + '日' AS birthday_confirm,");           
            // --性別
            sqlSb.Append(" PersonalInfo.AFTER_GENDER,gender1.KBNNAME AS gender_confirm,");
            // --婚姻区分
            sqlSb.Append(" PersonalInfo.MARRIAGE_CLASS,marriage.KBNNAME AS marriage_confirm,");
            // --国籍
            sqlSb.Append("  PersonalInfo.AFTER_COUNTRY AS country_confirm,");

            // --税扶養申告エリア
            // --現在の税表区分
            sqlSb.Append("  PersonalInfo.BEFORE_ZEIHYOKBN,beforezeihyokbe.KBNNAME AS beforeZeihyokbn_confirm,");
            // --税表区分を変更しますか
            sqlSb.Append(" PersonalInfo.ZEIHYO_EDT_KBN,zeihyokbn.KBNNAME AS zeihyoEdtKbn_confirm,");
            // --税表区分変更理由
            sqlSb.Append("  PersonalInfo.ZEIHYOKBN_CHANGE_REASON,zeichangerea.KBNNAME AS afterZeihyoKbn_confirm,");
            // --（変更後）税表区分
            sqlSb.Append("  PersonalInfo.AFTER_ZEIHYOKBN,afterzeihyokbe.KBNNAME AS afterzeihyoEdtKbn_confirm,");
            // --現在の税表区分
            //sqlSb.Append("  PersonalInfo.BEFORE_ZEIHYOKBN,beforezeihyokbe.KBNNAME AS beforeZeihyokbn_confirm2,");
            // --税表区分を変更しますか
            //sqlSb.Append("  PersonalInfo.ZEIHYO_EDT_KBN,zeihyokbn.KBNNAME AS zeihyoEdtKbn_confirm2,");          
            // --申請日
            sqlSb.Append("  SUBSTRING(PersonalInfo.APPLY_DATE, 1, 4) + '年' + SUBSTRING(PersonalInfo.APPLY_DATE, 5, 2) + '月' + SUBSTRING(PersonalInfo.APPLY_DATE, 7, 2) + '日' AS appleDate_confirm,");
            // --現在の寡夫・ひとり親区分
            sqlSb.Append("  PersonalInfo.BEFORE_KAFU_CLASS,beforekafu.KBNNAME AS beforeKafu_confirm,");
            // --①申請者性別
            sqlSb.Append(" PersonalInfo.BEFORE_GENDER,gender.KBNNAME AS beforeGender_confirm,");
            // --②申請者婚姻区分
            sqlSb.Append(" PersonalInfo.MARRIAGE_CLASS,marriage.KBNNAME AS beforeMarriage_confirm,");
            // --③事実婚状態ではないですか。
            sqlSb.Append("  PersonalInfo.MARRIAGE_IN_FACT_CLASS,marriageinfact.KBNNAME AS genderTrue_confirm,");
            // --④従業員自身の年間所得が500万円以下ですか
            sqlSb.Append("  PersonalInfo.EMPLOYEE_ANNUAL_INCOME_CLASS,employeeincome.KBNNAME AS zeihyokbnY_confirm,");
            // --⑤離婚歴はありますか（女性のみ）
            sqlSb.Append("  PersonalInfo.DIVORCE_EXPERIENCE_CLASS,divorceexperience.KBNNAME AS likon_confirm,");
            // --寡夫・ひとり親判定区分
            sqlSb.Append("  PersonalInfo.AFTER_KAFU_CLASS,beforekafu2.KBNNAME AS afterKafu_confirm,");
            // --現在の勤労学生区分
            sqlSb.Append("  PersonalInfo.BEFORE_KINROGAKUSEIKBN,beforeKinrogakuseikbn.KBNNAME AS beferKinrogakuseikbn_confirm,");
            // --①従業員自身が所定の学校の学生・生徒に該当しますか。
            sqlSb.Append("  PersonalInfo.DESIGNATED_SCHOOL_STUDENT_CLASS,student.KBNNAME AS gakusei_confirm,");
            // --②本人の年間所得金額が48万円以上75万円以下（給与収入で言えば103万円以上130万円以下）の見込である。
            sqlSb.Append("  PersonalInfo.PERSONAL_ANNUAL_INCOME_CLASS,personalincome.KBNNAME AS moneymikomi_confirm,");
            // --③本人の給与所得（アルバイト代）以外の所得が、10万円以下である。
            sqlSb.Append("  PersonalInfo.NON_WAGE_INCOME_CLASS,wageincome.KBNNAME AS adobaishu_confirm,");
            // --勤労学生区分判定
            sqlSb.Append("  PersonalInfo.AFTER_KINROGAKUSEIKBN,afterkinrogakuseikbn.KBNNAME AS afterkinrogakuseikbn_confirm,");
            // --現在の本人障害区分
            sqlSb.Append("  PersonalInfo.BEFORE_HANDICAPPED_CLASS,beforehandicapped.KBNNAME AS feforehandicapped_confirm,");
            // --障害区分を変更しますか
            sqlSb.Append("  PersonalInfo.HANDICAPPED_EDT_KBN,handicappedkbn.KBNNAME AS handicappedEdtKbn_confirm,");
            // --現在の本人障害区分
            //sqlSb.Append("  PersonalInfo.BEFORE_HANDICAPPED_CLASS,beforehandicapped.KBNNAME AS feforehandicapped_confirm2,");
            // --障害区分を変更しますか
            //sqlSb.Append("  PersonalInfo.HANDICAPPED_EDT_KBN,handicappedkbn.KBNNAME AS handicappedEdtKbn_confirm2,");
            // --①以下いずれかに該当しますか。
            sqlSb.Append("  PersonalInfo.DISABILITY_APPLICABLE_CATEGORY,disabilitygaito.KBNNAME AS disability_confirm,");
            // --手帳番号
            sqlSb.Append("  PersonalInfo.ANNUITYNOTE_NO AS tetyowuNo_confirm,");
            // --手帳交付日
            sqlSb.Append("  SUBSTRING(PersonalInfo.ANNUITYNOTE_APPLY_DATE, 1, 4) + '年' + SUBSTRING(PersonalInfo.ANNUITYNOTE_APPLY_DATE, 5, 2) + '月' + SUBSTRING(PersonalInfo.ANNUITYNOTE_APPLY_DATE, 7, 2) + '日' AS tetyowuDate_confirm,");
            // --障害内容区分
            sqlSb.Append("  PersonalInfo.DISABILITY_CONTENTS_CLASS,diablilitycontents.KBNNAME AS syowugaiContentKbn_confirm,");
            // --障害等級
            sqlSb.Append("  PersonalInfo.DISABILITY_LEVEL,disablitylevel.KBNNAME AS syowugaiLever_confirm,");
            // --障害程度
            sqlSb.Append("  PersonalInfo.DISABILITY_DEGREE,disablility.KBNNAME AS syowugaiDegree_confirm,");
            // --障害内容
            sqlSb.Append("  PersonalInfo.DISABILITY_CONTENTS AS syowugaiContent_confirm,");
            // --（変更後）障害区分
            sqlSb.Append("  PersonalInfo.AFTER_HANDICAPPED_CLASS,afterhandicaped.KBNNAME AS afterHandicappedKbn_confirm,");
            // --（アップロードファイル１
            sqlSb.Append("  PersonalInfo.UPLOADE_FILE_1 AS UploadFile1_confirm,");
            // --（アップロードファイル２
            sqlSb.Append("  PersonalInfo.UPLOADE_FILE_2 AS UploadFile2_confirm,");

            // --口座変更エリア
            // --口座変更日
            sqlSb.Append("  CASE WHEN PersonalInfo.KOZA_EDT_DATE IS NULL THEN ''  ELSE SUBSTRING(PersonalInfo.KOZA_EDT_DATE,1,4)+'年'+SUBSTRING(PersonalInfo.KOZA_EDT_DATE,5,2)+'月'+SUBSTRING(PersonalInfo.KOZA_EDT_DATE,7,2)+'日' END AS kozaedtdate_confirm,");
            // --銀行コード
            sqlSb.Append("  PersonalInfo.AFTER_BANK_CODE AS bankcode_confirm,");
            // --支店コード
            sqlSb.Append("  PersonalInfo.AFTER_BRANCH_CODE AS branchcode_confirm,");
            // --金融機関名
            sqlSb.Append("  PersonalInfo.AFTER_BANK_NAME AS bankname_confirm,");
            // --支店名
            sqlSb.Append("  PersonalInfo.AFTER_BRANCH_NAME AS branchname_confirm,");
            // --預金種別
            sqlSb.Append("  PersonalInfo.KOZAKBN,kozakbn.KBNNAME AS kozakbn_confirm,");
            // --番号
            sqlSb.Append("  PersonalInfo.AFTER_KOZABANGO AS kozabango_confirm,");
            // --口座名義
            sqlSb.Append("  PersonalInfo.AFTER_KOZAMEIGI AS kozameigi_confirm");

            // テーブル名
            sqlSb.Append("  FROM TT_WF_PERSONAL_INFO_CHANGE PersonalInfo");
            // 関連検索　
            sqlSb.Append("  LEFT JOIN MT_KBN beforezeihyokbe ON beforezeihyokbe.KBNCODE = 'TAX_TABLE' AND beforezeihyokbe.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_ZEIHYOKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN afterzeihyokbe ON afterzeihyokbe.KBNCODE = 'TAX_TABLE' AND afterzeihyokbe.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_ZEIHYOKBN)");          
            sqlSb.Append("  LEFT JOIN MT_KBN reasoncode ON reasoncode.KBNCODE = 'PERSONAL_INFO_CHANGE_TYPE' AND reasoncode.KBNVALUE = CONVERT(varchar, PersonalInfo.REASON_CODE)");
            sqlSb.Append("  LEFT JOIN MT_KBN gender ON gender.KBNCODE = 'SEIBETSU_KBN' AND gender.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_GENDER)");
            sqlSb.Append("  LEFT JOIN MT_KBN gender1 ON gender1.KBNCODE = 'SEIBETSU_KBN' AND gender1.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_GENDER)");
            sqlSb.Append("  LEFT JOIN MT_KBN marriage ON marriage.KBNCODE = 'MARRIAGE_CLASS' AND marriage.KBNVALUE = CONVERT(varchar, PersonalInfo.MARRIAGE_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN chkKbn ON chkKbn.KBNCODE = 'CHECK_CODE' AND chkKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.CHECK_FLG)");
            sqlSb.Append("  LEFT JOIN MT_KBN zeihyokbn ON zeihyokbn.KBNCODE = 'CHNGE_FLG' AND zeihyokbn.KBNVALUE = CONVERT(varchar, PersonalInfo.ZEIHYO_EDT_KBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN zeichangerea ON zeichangerea.KBNCODE = 'ZEIHYOKBN_CHANGE_REASON' AND zeichangerea.KBNVALUE = PersonalInfo.ZEIHYOKBN_CHANGE_REASON");
            sqlSb.Append("  LEFT JOIN MT_KBN marriageinfact ON marriageinfact.KBNCODE = 'MARRIAGE_IN_FACT_CLASS' AND marriageinfact.KBNVALUE = PersonalInfo.MARRIAGE_IN_FACT_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN employeeincome ON employeeincome.KBNCODE = 'EMPLOYEE_ANNUAL_INCOME' AND employeeincome.KBNVALUE = CONVERT(varchar, PersonalInfo.EMPLOYEE_ANNUAL_INCOME_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN divorceexperience ON divorceexperience.KBNCODE = 'APPLY_CLASS' AND divorceexperience.KBNVALUE = PersonalInfo.DIVORCE_EXPERIENCE_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN student ON student.KBNCODE = 'APPLY_CLASS' AND student.KBNVALUE = CONVERT(varchar, PersonalInfo.DESIGNATED_SCHOOL_STUDENT_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN personalincome ON personalincome.KBNCODE = 'APPLY_CLASS' AND personalincome.KBNVALUE = CONVERT(varchar, PersonalInfo.PERSONAL_ANNUAL_INCOME_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN wageincome ON wageincome.KBNCODE = 'APPLY_CLASS' AND wageincome.KBNVALUE = CONVERT(varchar, PersonalInfo.NON_WAGE_INCOME_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN handicappedkbn ON handicappedkbn.KBNCODE = 'CHNGE_FLG' AND handicappedkbn.KBNVALUE = CONVERT(varchar, PersonalInfo.HANDICAPPED_EDT_KBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN diablilitycontents ON diablilitycontents.KBNCODE = 'DISABILITY_CONTENTS_CLASS' AND diablilitycontents.KBNVALUE = CONVERT(varchar, PersonalInfo.DISABILITY_CONTENTS_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN disablitylevel ON disablitylevel.KBNCODE = 'DISABILITY_LEVEL' AND disablitylevel.KBNVALUE = PersonalInfo.DISABILITY_LEVEL");
            sqlSb.Append("  LEFT JOIN MT_KBN disablility ON disablility.KBNCODE = 'DISABILITY_DEGREE' AND disablility.KBNVALUE = CONVERT(varchar, PersonalInfo.DISABILITY_DEGREE)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforeKinrogakuseikbn ON beforeKinrogakuseikbn.KBNCODE = 'WORKING_STUDENT_FLG' AND beforeKinrogakuseikbn.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_KINROGAKUSEIKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN afterkinrogakuseikbn ON afterkinrogakuseikbn.KBNCODE = 'WORKING_STUDENT_FLG' AND afterkinrogakuseikbn.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_KINROGAKUSEIKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN afterhandicaped ON afterhandicaped.KBNCODE = 'DISABILITY_CRITERIA' AND afterhandicaped.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_HANDICAPPED_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforekafu ON beforekafu.KBNCODE = 'WINDOW_CRITERIA' AND beforekafu.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_KAFU_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforekafu2 ON beforekafu2.KBNCODE = 'WINDOW_CRITERIA' AND beforekafu2.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_KAFU_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforehandicapped ON beforehandicapped.KBNCODE = 'DISABILITY_CRITERIA' AND beforehandicapped.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_HANDICAPPED_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN kozakbn ON kozakbn.KBNCODE = 'KOZAKBN' AND kozakbn.KBNVALUE = CONVERT(varchar, PersonalInfo.KOZAKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN disabilitygaito ON disabilitygaito.KBNCODE = 'APPLY_CLASS' AND disabilitygaito.KBNVALUE = CONVERT(varchar, PersonalInfo.DISABILITY_APPLICABLE_CATEGORY)");
            
            // 条件
            sqlSb.Append("    WHERE PersonalInfo.OID = @WorkID ");
            // パラメータの設定
            ps.Add("WorkID", workId);

            // sql文の設定
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetPersonalInfoChangeList()
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

            //伝票番号
            sqlSb.Append("SELECT PersonalInfo.OID AS WorkID,");
            //修正理由区分
            sqlSb.Append("  PersonalInfo.REASON_CODE,cerKbn.KBNNAME AS ApplicationKbn,");
            //確認状態
            sqlSb.Append("  PersonalInfo.CHECK_FLG,chfKbn.KBNNAME AS CheckKbn,");
            //会社
            sqlSb.Append("  PersonalInfo.COMPANY_CODE + '　' + PersonalInfo.CMOPANY_NAME AS CorpName,");
            //人事所属
            sqlSb.Append("  PersonalInfo.DEPT_NAME AS DepartmentName,");
            //社員番号
            sqlSb.Append("  PersonalInfo.EMPLOYEE_NO AS EmployeeNo,");
            //氏名(漢字)
            sqlSb.Append("  PersonalInfo.BEFORE_LAST_NAME_KANJI + '　' + PersonalInfo.BEFORE_FIRST_NAME_KANJI AS EmployeeName,");
            //申請日
            sqlSb.Append("  CASE WHEN PersonalInfo.APPLY_DATE IS NULL THEN ''  ELSE SUBSTRING(PersonalInfo.APPLY_DATE,1,4)+'/'+SUBSTRING(PersonalInfo.APPLY_DATE,5,2)+'/'+SUBSTRING(PersonalInfo.APPLY_DATE,7,2) END AS AppDate,");
            //異動日
            sqlSb.Append("  CASE WHEN PersonalInfo.CORRECTION_DATE IS NULL THEN ''  ELSE SUBSTRING(PersonalInfo.CORRECTION_DATE,1,4)+'/'+SUBSTRING(PersonalInfo.CORRECTION_DATE,5,2)+'/'+SUBSTRING(PersonalInfo.CORRECTION_DATE,7,2) END AS CorrectionDate,");
            //税表区分変更
            sqlSb.Append("  PersonalInfo.ZEIHYO_EDT_KBN,chkKbn.KBNNAME AS ZeihyoKbn,");
            //障害区分変更
            sqlSb.Append("  PersonalInfo.HANDICAPPED_EDT_KBN,celKbn.KBNNAME AS HandicappedKbn,");
            //口座変更日
            sqlSb.Append("  CASE WHEN PersonalInfo.KOZA_EDT_DATE IS NULL THEN ''  ELSE SUBSTRING(PersonalInfo.KOZA_EDT_DATE,1,4)+'/'+SUBSTRING(PersonalInfo.KOZA_EDT_DATE,5,2)+'/'+SUBSTRING(PersonalInfo.KOZA_EDT_DATE,7,2) END AS ZozaEdtDate");
            // テーブル名
            sqlSb.Append("  FROM TT_WF_PERSONAL_INFO_CHANGE PersonalInfo");

            // 関連検索
            sqlSb.Append("  LEFT JOIN MT_KBN cerKbn ON cerKbn.KBNCODE = 'PERSONAL_INFO_CHANGE_TYPE' AND cerKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.REASON_CODE)");
            sqlSb.Append("  LEFT JOIN MT_KBN chkKbn ON chkKbn.KBNCODE = 'CHNGE_FLG' AND chkKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.ZEIHYO_EDT_KBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN celKbn ON celKbn.KBNCODE = 'CHNGE_FLG' AND celKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.HANDICAPPED_EDT_KBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN chfKbn ON chfKbn.KBNCODE = 'CHECK_CODE' AND chfKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.CHECK_FLG)");

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
            sqlSb.Append("SELECT PersonalInfo.OID AS WorkId,");
            // --申請日
            sqlSb.Append("  PersonalInfo.APPLY_DATE AS AppDate,");
            // --修正理由区分
            sqlSb.Append("  cerKbn.KBNNAME AS ApplicationKbn,");
            // --異動日
            sqlSb.Append("  CASE WHEN PersonalInfo.CORRECTION_DATE IS NULL THEN ''  ELSE PersonalInfo.CORRECTION_DATE END AS CorrectionDate,");          
            // --会社コード
            sqlSb.Append("  PersonalInfo.COMPANY_CODE AS CompanyCode,");
            // --会社名
            sqlSb.Append("  PersonalInfo.CMOPANY_NAME AS CompanyName,");
            // --人事所属コード
            sqlSb.Append("  PersonalInfo.DEPT_NO AS DepartmentCode,");
            // --所属名
            sqlSb.Append("  PersonalInfo.DEPT_NAME AS DepartmentName,");
            // --社員番号
            sqlSb.Append("  PersonalInfo.EMPLOYEE_NO AS EmployeeNo,");
            // --変更前_姓・漢字
            sqlSb.Append("  PersonalInfo.BEFORE_LAST_NAME_KANJI AS BeforeLastNameKanji,");
            // --変更前_名・漢字
            sqlSb.Append("  PersonalInfo.BEFORE_FIRST_NAME_KANJI AS BeforeFirstNameKanji,");
            // --変更前_姓・カナ
            sqlSb.Append("  PersonalInfo.BEFORE_LAST_NAME_KANA AS BeforeLastNameKana,");
            // --変更前_名・カナ
            sqlSb.Append("  PersonalInfo.BEFORE_FIRST_NAME_KANA AS BeforeFirstNameKana,");
            // --変更後_姓・漢字
            sqlSb.Append("  PersonalInfo.AFTER_LAST_NAME_KANJI AS AfterLastNameKanji,");
            // --変更後_名・漢字
            sqlSb.Append("  PersonalInfo.AFTER_FIRST_NAME_KANJI AS AfterFirstNameKanji,");
            // --変更後_姓・カナ
            sqlSb.Append("  PersonalInfo.AFTER_LAST_NAME_KANA AS AfterLastNameKana,");
            // --変更後_名・カナ
            sqlSb.Append("  PersonalInfo.AFTER_FIRST_NAME_KANA AS AfterFirstNameKana,");
            // --変更前_生年月日         
            sqlSb.Append("  CASE WHEN PersonalInfo.BEFORE_BIRTHDAY IS NULL THEN ''  ELSE PersonalInfo.BEFORE_BIRTHDAY END AS BerforeBirthday,");
            // --変更後_生年月日           
            sqlSb.Append("  CASE WHEN PersonalInfo.AFTER_BIRTHDAY IS NULL THEN ''  ELSE PersonalInfo.AFTER_BIRTHDAY END AS AfterBirthday,");
            // --変更前_性別
            sqlSb.Append(" gender.KBNNAME AS BerforeGender,");
            // --変更前_性別
            sqlSb.Append(" gender.KBNNAME AS AfterGender,");
            // --変更前_国籍
            sqlSb.Append("  PersonalInfo.BEFORE_COUNTRY AS BerforeCountry,");
            // --変更後_国籍
            sqlSb.Append("  PersonalInfo.AFTER_COUNTRY AS AfterCountry,");
            // --変更前_税表区分
            sqlSb.Append("  beforezeihyokbe.KBNNAME AS BeforeZeihyokbn,");
            // --変更後_税表区分
            sqlSb.Append("  afterzeihyokbe.KBNNAME AS AferZeihyokbn,");
            // --税表区分変更
            sqlSb.Append("  chkKbn.KBNNAME AS Zeihyoedt,");
            // --税表区分変更理由
            sqlSb.Append("  zeichangerea.KBNNAME AS ZeihyokbnChangeReason,");
            // --変更前_寡婦・ひとり親区分
            sqlSb.Append("  beforekafu.KBNNAME AS BerforeKafu,");
            // --変更後_寡婦・ひとり親区分
            sqlSb.Append("  beforekafu2.KBNNAME AS AfterKafu,");
            // --申請者性別
            sqlSb.Append(" gender.KBNNAME AS ApplayGender,");
            // --申請者婚姻区分
            sqlSb.Append(" marriage.KBNNAME AS Marriage,");
            // --事実婚状態ではない
            sqlSb.Append("  marriagefact.KBNNAME AS Marriage,");
            // --年間所得
            sqlSb.Append("  empincome.KBNNAME AS EmpAnnualIncome,");           
            // --変更前_勤労学生区分
            sqlSb.Append("  beforeKinrogakuseikbn.KBNNAME AS BeforeKinrogakuseikbn,");
            // --変更後_勤労学生区分
            sqlSb.Append("  afterkinrogakuseikbn.KBNNAME AS AfterKinrogakuseikbn,");
            // --学生
            sqlSb.Append("  schoolstudent.KBNNAME AS DesSchoolStudent,");
            // --本人年間所得
            sqlSb.Append("  personalincome.KBNNAME AS PersonalIncome,");
            // --給与外所得
            sqlSb.Append("  nonwageincome.KBNNAME AS NonWageIncome,");
            // --障害区分変更
            sqlSb.Append("  celKbn.KBNNAME AS Handicappedkbn,");
            // --変更前_障害区分
            sqlSb.Append("  beforehandicaped.KBNNAME AS BeforeHandicappedkbn,");
            // --変更後_障害区分
            sqlSb.Append("  afterhandicaped.KBNNAME AS AfterHandicappedkbn,");
            // --該当するか
            sqlSb.Append("  appcategory.KBNNAME AS ApplicableCategory,");
            // --手帳番号
            sqlSb.Append("  PersonalInfo.ANNUITYNOTE_NO AS Annuitynote,");
            // --手帳交付日
            sqlSb.Append("  PersonalInfo.ANNUITYNOTE_APPLY_DATE AS Annuitynote,");
            // --障害内容区分
            sqlSb.Append("  disanilitycontents.KBNNAME AS DisanilityContents,");
            // --障害等級
            sqlSb.Append("  disablitylevel.KBNNAME AS DisanilityLevel,");
            // --障害程度
            sqlSb.Append("  disablility.KBNNAME AS DisabilityDegree,");
            // --障害内容
            sqlSb.Append("  PersonalInfo.DISABILITY_CONTENTS AS DisabilityContents,");
            // --アップロードファイル１
            sqlSb.Append("  PersonalInfo.UPLOADE_FILE_1 AS UploadFile1,");
            // --アップロードファイル２
            sqlSb.Append("  PersonalInfo.UPLOADE_FILE_2 AS UploadFile2,");
            // --口座変更日           
            sqlSb.Append("  CASE WHEN PersonalInfo.KOZA_EDT_DATE IS NULL THEN ''  ELSE PersonalInfo.KOZA_EDT_DATE END AS KozaDate,");
            // --変更前_銀行コード
            sqlSb.Append("  PersonalInfo.BEFORE_BANK_CODE AS BeforeBank,");
            // --変更後_銀行コード
            sqlSb.Append("  PersonalInfo.AFTER_BANK_CODE AS AfterBank,");
            // --変更前_支店コード
            sqlSb.Append("  PersonalInfo.BEFORE_BRANCH_CODE AS BeforeBranchCode,");
            // --変更前_支店コード
            sqlSb.Append("  PersonalInfo.AFTER_BRANCH_CODE AS AfterBranchCode,");
            // --変更前_金融機関名
            sqlSb.Append("  PersonalInfo.BEFORE_BANK_NAME AS BeforeBankName,");
            // --変更後_金融機関名
            sqlSb.Append("  PersonalInfo.AFTER_BANK_NAME AS AfterBankName,");
            // --変更前_支店名
            sqlSb.Append("  PersonalInfo.BEFORE_BRANCH_NAME AS BeforeBranchName,");
            // --変更後_支店名
            sqlSb.Append("  PersonalInfo.AFTER_BRANCH_NAME AS AfterBranchName,");
            // --預金種別
            sqlSb.Append("  kozakbn.KBNNAME AS Kozakbn,");
            // --変更前_口座コード
            sqlSb.Append("  PersonalInfo.BEFORE_KOZABANGO AS BeforeKozabango,");
            // --変更後_口座コード
            sqlSb.Append("  PersonalInfo.AFTER_KOZABANGO AS AfterKozabango,");
            // --変更前_口座名義
            sqlSb.Append("  PersonalInfo.BEFORE_KOZAMEIGI AS BeforeKozameigi,");
            // --変更後_口座名義
            sqlSb.Append("  PersonalInfo.AFTER_KOZAMEIGI AS AfterKozameigi,");
            // --確認状態
            sqlSb.Append("  chfKbn.KBNNAME AS CheckFlg,");
            // --確認者_会社コード
            sqlSb.Append("  PersonalInfo.CHECK_CORP_CODE AS CheckCoroCode,");
            // --確認者_会社名
            sqlSb.Append("  PersonalInfo.CHECK_CORP_NAME AS CheckCoroName,");
            // --確認者_所属コード
            sqlSb.Append("  PersonalInfo.CHECK_DEPT_CODE AS CheckDeptCode,");
            // --確認者_所属名
            sqlSb.Append("  PersonalInfo.CHECK_DEPT_NAME AS CheckDeptName,");
            // --確認者_社員番号
            sqlSb.Append("  PersonalInfo.CHECK_EMP_NUM AS CheckEmpNum,");
            // --確認者名
            sqlSb.Append("  PersonalInfo.CHECK_LNAME +'　'+ PersonalInfo.CHECK_FNAME AS CheckName,");
            // --確認日時
            sqlSb.Append("  PersonalInfo.CHECK_DATE AS CheckDate");

            // 主テーブル  【本人情報変更トランザクションテーブル】
            sqlSb.Append("  FROM TT_WF_PERSONAL_INFO_CHANGE PersonalInfo");

            // 関連検索
            sqlSb.Append("  LEFT JOIN MT_KBN empincome ON empincome.KBNCODE = 'EMPLOYEE_ANNUAL_INCOME' AND empincome.KBNVALUE = PersonalInfo.EMPLOYEE_ANNUAL_INCOME_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN schoolstudent ON schoolstudent.KBNCODE = 'APPLY_CLASS' AND schoolstudent.KBNVALUE = PersonalInfo.DESIGNATED_SCHOOL_STUDENT_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN personalincome ON personalincome.KBNCODE = 'APPLY_CLASS' AND personalincome.KBNVALUE = PersonalInfo.PERSONAL_ANNUAL_INCOME_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN nonwageincome ON nonwageincome.KBNCODE = 'APPLY_CLASS' AND nonwageincome.KBNVALUE = PersonalInfo.NON_WAGE_INCOME_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN appcategory ON appcategory.KBNCODE = 'APPLY_CLASS' AND appcategory.KBNVALUE = PersonalInfo.DISABILITY_APPLICABLE_CATEGORY");
            sqlSb.Append("  LEFT JOIN MT_KBN kozakbn ON kozakbn.KBNCODE = 'KOZAKBN' AND kozakbn.KBNVALUE = PersonalInfo.KOZAKBN");
            sqlSb.Append("  LEFT JOIN MT_KBN disanilitycontents ON disanilitycontents.KBNCODE = 'DISABILITY_CONTENTS_CLASS' AND disanilitycontents.KBNVALUE = PersonalInfo.DISABILITY_CONTENTS_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN disablitylevel ON disablitylevel.KBNCODE = 'DISABILITY_LEVEL' AND disablitylevel.KBNVALUE = PersonalInfo.DISABILITY_LEVEL");
            sqlSb.Append("  LEFT JOIN MT_KBN disablility ON disablility.KBNCODE = 'DISABILITY_DEGREE' AND disablility.KBNVALUE = CONVERT(varchar, PersonalInfo.DISABILITY_DEGREE)");
            sqlSb.Append("  LEFT JOIN MT_KBN cerKbn ON cerKbn.KBNCODE = 'PERSONAL_INFO_CHANGE_TYPE' AND cerKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.REASON_CODE)");
            sqlSb.Append("  LEFT JOIN MT_KBN gender ON gender.KBNCODE = 'SEIBETSU_KBN' AND gender.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_GENDER)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforezeihyokbe ON beforezeihyokbe.KBNCODE = 'TAX_TABLE' AND beforezeihyokbe.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_ZEIHYOKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN afterzeihyokbe ON afterzeihyokbe.KBNCODE = 'TAX_TABLE' AND afterzeihyokbe.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_ZEIHYOKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN chkKbn ON chkKbn.KBNCODE = 'CHNGE_FLG' AND chkKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.ZEIHYO_EDT_KBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN zeichangerea ON zeichangerea.KBNCODE = 'ZEIHYOKBN_CHANGE_REASON' AND zeichangerea.KBNVALUE = PersonalInfo.ZEIHYOKBN_CHANGE_REASON");
            sqlSb.Append("  LEFT JOIN MT_KBN beforekafu ON beforekafu.KBNCODE = 'WINDOW_CRITERIA' AND beforekafu.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_KAFU_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforekafu2 ON beforekafu2.KBNCODE = 'WINDOW_CRITERIA' AND beforekafu2.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_KAFU_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN marriage ON marriage.KBNCODE = 'MARRIAGE_CLASS' AND marriage.KBNVALUE = CONVERT(varchar, PersonalInfo.MARRIAGE_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN marriagefact ON marriagefact.KBNCODE = 'MARRIAGE_IN_FACT_CLASS' AND marriagefact.KBNVALUE = PersonalInfo.MARRIAGE_IN_FACT_CLASS");
            sqlSb.Append("  LEFT JOIN MT_KBN afterkinrogakuseikbn ON afterkinrogakuseikbn.KBNCODE = 'WORKING_STUDENT_FLG' AND afterkinrogakuseikbn.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_KINROGAKUSEIKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforeKinrogakuseikbn ON beforeKinrogakuseikbn.KBNCODE = 'WORKING_STUDENT_FLG' AND beforeKinrogakuseikbn.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_KINROGAKUSEIKBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN celKbn ON celKbn.KBNCODE = 'CHNGE_FLG' AND celKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.HANDICAPPED_EDT_KBN)");
            sqlSb.Append("  LEFT JOIN MT_KBN beforehandicaped ON beforehandicaped.KBNCODE = 'DISABILITY_CRITERIA' AND beforehandicaped.KBNVALUE = CONVERT(varchar, PersonalInfo.BEFORE_HANDICAPPED_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN afterhandicaped ON afterhandicaped.KBNCODE = 'DISABILITY_CRITERIA' AND afterhandicaped.KBNVALUE = CONVERT(varchar, PersonalInfo.AFTER_HANDICAPPED_CLASS)");
            sqlSb.Append("  LEFT JOIN MT_KBN chfKbn ON chfKbn.KBNCODE = 'CHECK_CODE' AND chfKbn.KBNVALUE = CONVERT(varchar, PersonalInfo.CHECK_FLG)");

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
            sqlCond.Append("    AND PersonalInfo.WFState = '3' ");

            // 画面入力により、条件を作ること
            // 修正理由区分
            if (!string.IsNullOrEmpty(searchCond.CorrectionReasonKbn))
            {
                sqlCond.Append("    AND PersonalInfo.REASON_CODE = @CorrectionReasonKbn");
                ps.Add("CorrectionReasonKbn", searchCond.CorrectionReasonKbn);
            }

            // 確認状態
            if (!string.IsNullOrEmpty(searchCond.CheckFlg))
            {
                sqlCond.Append("    AND PersonalInfo.CHECK_FLG = @CheckFlg");
                ps.Add("CheckFlg", searchCond.CheckFlg);
            }

            // 会社コード＋会社名を空欄で検索の場合は、受託会社全部を検索する
            if (string.IsNullOrEmpty(searchCond.Kaishalist))
            {
                // 受託している会社の情報のみに絞り込みを行う
                sqlCond.Append("    AND PersonalInfo.COMPANY_CODE IN ( ");
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
                // 自社の情報のみを表示を行う
                sqlCond.Append("    AND PersonalInfo.COMPANY_CODE = @Kaishalist ");

                ps.Add("Kaishalist", searchCond.Kaishalist);
            }

            // 所属コード
            if (!string.IsNullOrEmpty(searchCond.AffiliationCd))
            {
                sqlCond.Append("    AND PersonalInfo.DEPT_NO = @AffiliationCd");
                ps.Add("AffiliationCd", searchCond.AffiliationCd);
            }

            // 所属名
            if (!string.IsNullOrEmpty(searchCond.AffiliationNm))
            {
                sqlCond.Append("    AND PersonalInfo.DEPT_NAME LIKE @AffiliationNm");
                ps.Add("AffiliationNm", "%" + searchCond.AffiliationNm + "%");
            }

            // 社員番号 社員番号が入力されたら
            if (!string.IsNullOrEmpty(searchCond.EmployeeNo))
            {
                // 社員番号の設定
                sqlCond.Append("    AND PersonalInfo.EMPLOYEE_NO = @EmployeeNo ");
                // パラメータの設定
                ps.Add("EmployeeNo", searchCond.EmployeeNo);
            }

            // 社員名・姓
            if (!string.IsNullOrEmpty(searchCond.EmployeeLnm))
            {
                sqlCond.Append("    AND PersonalInfo.BEFORE_LAST_NAME_KANJI LIKE @EmployeeLnm");
                ps.Add("EmployeeLnm", "%" + searchCond.EmployeeLnm + "%");
            }

            // 社員名・名
            if (!string.IsNullOrEmpty(searchCond.EmployeeFnm))
            {
                sqlCond.Append("    AND PersonalInfo.BEFORE_FIRST_NAME_KANJI LIKE @EmployeeFnm");
                ps.Add("EmployeeFnm", "%" + searchCond.EmployeeFnm + "%");
            }

            // 申請日
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateFrom))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8),PersonalInfo.APPLY_DATE,112)");

                // 入力条件
                ps.Add("AppDateFrom", searchCond.AppDateFrom);
            }

            // Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateTo))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),PersonalInfo.APPLY_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");

                // 入力条件
                ps.Add("AppDateTo", searchCond.AppDateTo);
            }

            // 異動日
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.CorrectionDateFrom))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @CorrectionDateFrom),112) <= CONVERT(nvarchar(8),PersonalInfo.CORRECTION_DATE,112)");

                // 入力条件
                ps.Add("CorrectionDateFrom", searchCond.CorrectionDateFrom);
            }

            // Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.CorrectionDateTo))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),PersonalInfo.CORRECTION_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @CorrectionDateTo),112)");

                // 入力条件
                ps.Add("CorrectionDateTo", searchCond.CorrectionDateTo);
            }

            // 税表区分変更
            if (!string.IsNullOrEmpty(searchCond.ZeihyoEdtKbn))
            {
                sqlCond.Append("    AND PersonalInfo.ZEIHYO_EDT_KBN = @ZeihyoEdtKbn");
                ps.Add("ZeihyoEdtKbn", searchCond.ZeihyoEdtKbn);
            }

            // 変更前税表区分
            if (!string.IsNullOrEmpty(searchCond.BeforeZeihyokbn))
            {
                sqlCond.Append("    AND PersonalInfo.BEFORE_ZEIHYOKBN = @BeforeZeihyokbn");
                ps.Add("BeforeZeihyokbn", searchCond.BeforeZeihyokbn);
            }

            // 変更前ひとり親
            if (!string.IsNullOrEmpty(searchCond.BeforeKafu))
            {
                sqlCond.Append("    AND PersonalInfo.BEFORE_KAFU_CLASS = @BeforeKafu");
                ps.Add("BeforeKafu", searchCond.BeforeKafu);
            }

            // 変更前勤労学生区分
            if (!string.IsNullOrEmpty(searchCond.BeforeKinrogakuseikbn))
            {
                sqlCond.Append("    AND PersonalInfo.BEFORE_KINROGAKUSEIKBN = @BeforeKinrogakuseikbn");
                ps.Add("BeforeKinrogakuseikbn", searchCond.BeforeKinrogakuseikbn);
            }

            // 変更後税表区分
            if (!string.IsNullOrEmpty(searchCond.AfterZeihyokbn))
            {
                sqlCond.Append("    AND PersonalInfo.AFTER_ZEIHYOKBN = @AfterZeihyokbn");
                ps.Add("AfterZeihyokbn", searchCond.AfterZeihyokbn);
            }

            // 変更後ひとり親
            if (!string.IsNullOrEmpty(searchCond.AfterKafu))
            {
                sqlCond.Append("    AND PersonalInfo.AFTER_KAFU_CLASS = @AfterKafu");
                ps.Add("AfterKafu", searchCond.AfterKafu);
            }

            // 変更後勤労学生区分
            if (!string.IsNullOrEmpty(searchCond.AfterKinrogakuseikbn))
            {
                sqlCond.Append("    AND PersonalInfo.AFTER_KINROGAKUSEIKBN = @AfterKinrogakuseikbn");
                ps.Add("AfterKinrogakuseikbn", searchCond.AfterKinrogakuseikbn);
            }

            // 障害区分変更
            if (!string.IsNullOrEmpty(searchCond.HandicappedEdtKbn))
            {
                sqlCond.Append("    AND PersonalInfo.HANDICAPPED_EDT_KBN = @HandicappedEdtKbn");
                ps.Add("HandicappedEdtKbn", searchCond.HandicappedEdtKbn);
            }

            // 変更前障害区分
            if (!string.IsNullOrEmpty(searchCond.BeforeHandicapped))
            {
                sqlCond.Append("    AND PersonalInfo.BEFORE_HANDICAPPED_CLASS = @BeforeHandicapped");
                ps.Add("BeforeHandicapped", searchCond.BeforeHandicapped);
            }

            // 変更後障害区分
            if (!string.IsNullOrEmpty(searchCond.AfterHandicapped))
            {
                sqlCond.Append("    AND PersonalInfo.AFTER_HANDICAPPED_CLASS = @AfterHandicapped");
                ps.Add("AfterHandicapped", searchCond.AfterHandicapped);
            }

            // 口座変更日
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.KozaDateFrom))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @KozaDateFrom),112) <= CONVERT(nvarchar(8),PersonalInfo.KOZA_EDT_DATE,112)");

                // 入力条件
                ps.Add("KozaDateFrom", searchCond.KozaDateFrom);
            }

            // Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.KozaDateTo))
            {
                // 年月日の完全一致検索【トランザクションテーブル】登録日時
                sqlCond.Append("    AND CONVERT(nvarchar(8),PersonalInfo.KOZA_EDT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @KozaDateTo),112)");

                // 入力条件
                ps.Add("KozaDateTo", searchCond.KozaDateTo);
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
            csvTitle.Append(",\"申請日\"");
            csvTitle.Append(",\"修正理由区分\"");
            csvTitle.Append(",\"異動日\"");
            csvTitle.Append(",\"会社コード\"");
            csvTitle.Append(",\"会社名\"");
            csvTitle.Append(",\"人事所属コード\"");
            csvTitle.Append(",\"所属名\"");
            csvTitle.Append(",\"社員番号\"");
            csvTitle.Append(",\"変更前_姓・漢字\"");
            csvTitle.Append(",\"変更前_名・漢字\"");
            csvTitle.Append(",\"変更前_姓・カナ\"");
            csvTitle.Append(",\"変更前_名・カナ\"");
            csvTitle.Append(",\"変更後_姓・漢字\"");
            csvTitle.Append(",\"変更後_名・漢字\"");
            csvTitle.Append(",\"変更後_姓・カナ\"");
            csvTitle.Append(",\"変更後_名・カナ\"");
            csvTitle.Append(",\"変更前_生年月日\"");
            csvTitle.Append(",\"変更後_生年月日\"");
            csvTitle.Append(",\"変更前_性別\"");
            csvTitle.Append(",\"変更後_性別\"");
            csvTitle.Append(",\"変更前_国籍\"");
            csvTitle.Append(",\"変更後_国籍\"");
            csvTitle.Append(",\"変更前_税表区分\"");
            csvTitle.Append(",\"変更後_税表区分\"");
            csvTitle.Append(",\"税表区分変更\"");
            csvTitle.Append(",\"税表区分変更理由\"");
            csvTitle.Append(",\"変更前_寡婦・ひとり親区分\"");
            csvTitle.Append(",\"変更後_寡婦・ひとり親区分\"");
            csvTitle.Append(",\"申請者性別\"");
            csvTitle.Append(",\"申請者婚姻区分\"");
            csvTitle.Append(",\"事実婚状態ではない\"");
            csvTitle.Append(",\"年間所得\"");
            csvTitle.Append(",\"変更前_勤労学生区分\"");
            csvTitle.Append(",\"変更後_勤労学生区分\"");
            csvTitle.Append(",\"学生\"");
            csvTitle.Append(",\"本人年間所得\"");
            csvTitle.Append(",\"給与外所得\"");
            csvTitle.Append(",\"障害区分変更\"");
            csvTitle.Append(",\"変更前_障害区分\"");
            csvTitle.Append(",\"変更後_障害区分\"");
            csvTitle.Append(",\"該当するか\"");
            csvTitle.Append(",\"手帳番号\"");
            csvTitle.Append(",\"手帳交付日\"");
            csvTitle.Append(",\"障害内容区分\"");
            csvTitle.Append(",\"障害等級\"");
            csvTitle.Append(",\"障害程度\"");
            csvTitle.Append(",\"障害内容\"");
            csvTitle.Append(",\"アップロードファイル１\"");
            csvTitle.Append(",\"アップロードファイル２\"");
            csvTitle.Append(",\"口座変更日\"");
            csvTitle.Append(",\"変更前_銀行コード\"");
            csvTitle.Append(",\"変更後_銀行コード\"");
            csvTitle.Append(",\"変更前_支店コード\"");
            csvTitle.Append(",\"変更後_支店コード\"");
            csvTitle.Append(",\"変更前_金融機関名\"");
            csvTitle.Append(",\"変更後_金融機関名\"");
            csvTitle.Append(",\"変更前_支店名\"");
            csvTitle.Append(",\"変更後_支店名\"");
            csvTitle.Append(",\"預金種別\"");
            csvTitle.Append(",\"変更前_口座コード\"");
            csvTitle.Append(",\"変更後_口座コード\"");
            csvTitle.Append(",\"変更前_口座名義\"");
            csvTitle.Append(",\"変更後_口座名義\"");
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
            /// 修正理由区分
            /// </summary>
            public string CorrectionReasonKbn { get; set; }

            /// <summary>
            /// 確認状態
            /// </summary>
            public string CheckFlg { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            public string Kaishalist { get; set; }

            /// <summary>
            /// 人事所属コード
            /// </summary>
            public string AffiliationCd { get; set; }

            /// <summary>
            /// 人事所属名
            /// </summary>
            public string AffiliationNm { get; set; }

            /// <summary>
            /// 社員番号
            /// </summary>
            public string EmployeeNo { get; set; }

            /// <summary>
            ///  社員名・姓
            /// </summary>
            public string EmployeeLnm { get; set; }

            /// <summary>
            ///  社員名・名
            /// </summary>
            public string EmployeeFnm { get; set; }

            /// <summary>
            /// 申請日From
            /// </summary>
            public string AppDateFrom { get; set; }

            /// <summary>
            /// 申請日To
            /// </summary>
            public string AppDateTo { get; set; }

            /// <summary>
            /// 異動日From
            /// </summary>
            public string CorrectionDateFrom { get; set; }

            /// <summary>
            /// 異動日To
            /// </summary>
            public string CorrectionDateTo { get; set; }

            /// <summary>
            /// 税表区分変更
            /// </summary>
            public string ZeihyoEdtKbn { get; set; }

            /// <summary>
            /// 変更前税表区分
            /// </summary>
            public string BeforeZeihyokbn { get; set; }

            /// <summary>
            /// 変更前ひとり親
            /// </summary>
            public string BeforeKafu { get; set; }

            /// <summary>
            /// 変更前勤労学生区分
            /// </summary>
            public string BeforeKinrogakuseikbn { get; set; }

            /// <summary>
            /// 変更後税表区分
            /// </summary>
            public string AfterZeihyokbn { get; set; }

            /// <summary>
            /// 変更後ひとり親
            /// </summary>
            public string AfterKafu { get; set; }

            /// <summary>
            /// 変更後勤労学生区分
            /// </summary>
            public string AfterKinrogakuseikbn { get; set; }

            /// <summary>
            /// 障害区分変更
            /// </summary>
            public string HandicappedEdtKbn { get; set; }

            /// <summary>
            /// 変更前障害区分
            /// </summary>
            public string BeforeHandicapped { get; set; }

            /// <summary>
            /// 変更後障害区分
            /// </summary>
            public string AfterHandicapped { get; set; }

            /// <summary>
            /// 口座変更日From
            /// </summary>
            public string KozaDateFrom { get; set; }

            /// <summary>
            /// 口座変更日To
            /// </summary>
            public string KozaDateTo { get; set; }
        }
    }
}
