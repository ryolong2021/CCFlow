using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using SendGrid;
using SendGrid.Helpers.Mail;
using Common.WF_OutLog;

namespace BP.WF.HttpHandler
{
    public class Mn_EmailSet : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm appForm = new WF_AppForm();
        /// <summary>
        /// 一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetEmailSetList()
        {
            try
            {
                // 検索条件の取得
                EmailSetReq cond =
                    JsonConvert.DeserializeObject<EmailSetReq>(
                        this.GetRequestVal("EmailSetReq"));

                // Sql文と条件設定の取得
                StringBuilder sqlSb1 = new StringBuilder();
                sqlSb1.Append("    SELECT MT01.*, ");
                sqlSb1.Append("           KBN1.KBNNAME AS OPERATION_FLG_NM, ");
                sqlSb1.Append("           KBN2.KBNNAME AS WF_EMAIL_KBN_NM, ");
                sqlSb1.Append("           KBN3.KBNNAME AS EMAIL_TIMING_KBN_NM, ");
                sqlSb1.Append("           KBN4.KBNNAME AS SINSEISYA_KBN_NM, ");
                sqlSb1.Append("           MT03.RNRAKUSAKI_NAME, ");
                sqlSb1.Append("           MT04.RNRAKUSAKI_NAME_CC, ");
                sqlSb1.Append("           MT05.RNRAKUSAKI_NAME_BCC, ");
                sqlSb1.Append("           MT03.RNRAKUSAKI_NAME2, ");
                sqlSb1.Append("           MT04.RNRAKUSAKI_NAME_CC2, ");
                sqlSb1.Append("           MT05.RNRAKUSAKI_NAME_BCC2 ");
                sqlSb1.Append("      FROM MT_MN_EMAIL_SET AS MT01 ");
                sqlSb1.Append(" LEFT JOIN MT_KBN AS KBN1 ");
                sqlSb1.Append("        ON KBN1.KBNVALUE = MT01.OPERATION_FLG ");
                sqlSb1.Append("       AND KBN1.KBNCODE = 'OPERATION_FLG' ");
                sqlSb1.Append(" LEFT JOIN MT_KBN AS KBN2 ");
                sqlSb1.Append("        ON KBN2.KBNVALUE = MT01.WF_EMAIL_KBN ");
                sqlSb1.Append("       AND KBN2.KBNCODE = 'WF_EMAIL_KBN' ");
                sqlSb1.Append(" LEFT JOIN MT_KBN AS KBN3 ");
                sqlSb1.Append("        ON KBN3.KBNVALUE = MT01.EMAIL_TIMING_KBN ");
                sqlSb1.Append("       AND KBN3.KBNCODE = 'EMAIL_TIMING_KBN' ");
                sqlSb1.Append(" LEFT JOIN MT_KBN AS KBN4 ");
                sqlSb1.Append("        ON KBN4.KBNVALUE = MT01.SINSEISYA_KBN ");
                sqlSb1.Append("       AND KBN4.KBNCODE = 'SINSEISYA_KBN' ");
                sqlSb1.Append(" LEFT JOIN ");
                sqlSb1.Append("           (SELECT ");
                sqlSb1.Append("                   STRING_AGG(M2.RNRAKUSAKI_NAME, ',') AS RNRAKUSAKI_NAME, ");
                sqlSb1.Append("                   STRING_AGG(KBN11.KBNNAME, ',') AS RNRAKUSAKI_NAME2, ");
                sqlSb1.Append("                   M1.EMAIL_TO AS EMAIL_TO, M1.MANAGEMENT_ID AS MANAGEMENT_ID  ");
                sqlSb1.Append("              FROM MT_MN_EMAIL_SET M1 ");
                sqlSb1.Append("       CROSS APPLY STRING_SPLIT(M1.EMAIL_TO, ',') V ");
                sqlSb1.Append("         LEFT JOIN MT_MN_EMAIL_STAKEHOLDER M2 ");
                sqlSb1.Append("                ON M2.RNRAKUSAKI_ID IN (V.value) ");
                sqlSb1.Append("         LEFT JOIN MT_KBN AS KBN11 ");
                sqlSb1.Append("                ON KBN11.KBNVALUE IN (V.value) ");
                sqlSb1.Append("               AND KBN11.KBNCODE = 'SOUSINSAKI_TYPE' ");
                sqlSb1.Append("          GROUP BY M1.MANAGEMENT_ID, M1.EMAIL_TO) AS MT03 ");
                sqlSb1.Append("                ON MT03.MANAGEMENT_ID = MT01.MANAGEMENT_ID ");
                sqlSb1.Append(" LEFT JOIN ");
                sqlSb1.Append("           (SELECT ");
                sqlSb1.Append("                   STRING_AGG(M22.RNRAKUSAKI_NAME, ',') AS RNRAKUSAKI_NAME_CC, ");
                sqlSb1.Append("                   STRING_AGG(KBN12.KBNNAME, ',') AS RNRAKUSAKI_NAME_CC2, ");
                sqlSb1.Append("                   M12.MANAGEMENT_ID AS MANAGEMENT_ID  ");
                sqlSb1.Append("              FROM MT_MN_EMAIL_SET M12 ");
                sqlSb1.Append("       CROSS APPLY STRING_SPLIT(M12.EMAIL_CC, ',') V3 ");
                sqlSb1.Append("         LEFT JOIN MT_MN_EMAIL_STAKEHOLDER M22 ");
                sqlSb1.Append("                ON M22.RNRAKUSAKI_ID IN (V3.value) ");
                sqlSb1.Append("         LEFT JOIN MT_KBN AS KBN12 ");
                sqlSb1.Append("                ON KBN12.KBNVALUE IN (V3.value) ");
                sqlSb1.Append("               AND KBN12.KBNCODE = 'SOUSINSAKI_TYPE' ");
                sqlSb1.Append("          GROUP BY M12.MANAGEMENT_ID) AS MT04 ");
                sqlSb1.Append("                ON MT04.MANAGEMENT_ID = MT01.MANAGEMENT_ID ");
                sqlSb1.Append(" LEFT JOIN ");
                sqlSb1.Append("           (SELECT ");
                sqlSb1.Append("                   STRING_AGG(M23.RNRAKUSAKI_NAME, ',') AS RNRAKUSAKI_NAME_BCC, ");
                sqlSb1.Append("                   STRING_AGG(KBN13.KBNNAME, ',') AS RNRAKUSAKI_NAME_BCC2, ");
                sqlSb1.Append("                   M13.MANAGEMENT_ID AS MANAGEMENT_ID  ");
                sqlSb1.Append("              FROM MT_MN_EMAIL_SET M13 ");
                sqlSb1.Append("       CROSS APPLY STRING_SPLIT(M13.EMAIL_BCC, ',') V4 ");
                sqlSb1.Append("         LEFT JOIN MT_MN_EMAIL_STAKEHOLDER M23 ");
                sqlSb1.Append("                ON M23.RNRAKUSAKI_ID IN (V4.value) ");
                sqlSb1.Append("         LEFT JOIN MT_KBN AS KBN13 ");
                sqlSb1.Append("                ON KBN13.KBNVALUE IN (V4.value) ");
                sqlSb1.Append("               AND KBN13.KBNCODE = 'SOUSINSAKI_TYPE' ");
                sqlSb1.Append("          GROUP BY M13.MANAGEMENT_ID) AS MT05 ");
                sqlSb1.Append("                ON MT05.MANAGEMENT_ID = MT01.MANAGEMENT_ID ");

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append(" WHERE");

                Paras ps = new Paras();
                // 管理タイトル
                if (!string.IsNullOrEmpty(cond.ManagementTitle))
                {
                    // 部分一致検索 連絡先名
                    sqlSb.Append(" MT01.MANAGEMENT_TITLE LIKE @ManagementTitle AND");

                    // 入力条件
                    ps.Add("ManagementTitle", "%" + cond.ManagementTitle + "%");
                }

                // 稼働フラグ
                if (!string.IsNullOrEmpty(cond.OperationFlg))
                {

                    // 完全一致検索 送信先タイプ
                    sqlSb.Append(" MT01.OPERATION_FLG = @OperationFlg AND");

                    // 入力条件
                    ps.Add("OperationFlg", cond.OperationFlg);
                }

                // ワークフロー
                if (!string.IsNullOrEmpty(cond.WfEmailKbn))
                {
                    // 完全一致検索 連絡先ID
                    sqlSb.Append(" MT01.WF_EMAIL_KBN = @WfEmailKbn AND");

                    // 入力条件
                    ps.Add("WfEmailKbn", cond.WfEmailKbn);
                }

                // 申請者区分
                if (!string.IsNullOrEmpty(cond.SinseisyaKbn))
                {
                    // 完全一致検索 連絡先ID
                    sqlSb.Append(" MT01.SINSEISYA_KBN = @SinseisyaKbn AND");

                    // 入力条件
                    ps.Add("SinseisyaKbn", cond.SinseisyaKbn);
                }

                // ステータス
                if (!string.IsNullOrEmpty(cond.EmailTimingKbn))
                {
                    // 完全一致検索 連絡先ID
                    sqlSb.Append(" MT01.EMAIL_TIMING_KBN = @EmailTimingKbn AND");

                    // 入力条件
                    ps.Add("EmailTimingKbn", cond.EmailTimingKbn);
                }

                // 送信先TO
                if (cond.EmailTo != null && cond.EmailTo.Count > 0)
                {
                    if (cond.EmailTo.Count > 1)
                    {
                        var list = new List<string>();
                        sqlSb.Append(" (");
                        for (int i = 0; i < cond.EmailTo.Count; i++)
                        {
                            
                            // 部分一致検索 メールアドレス
                            list.Add(" ( ',' + MT01.EMAIL_TO + ',' LIKE @EmailA" + i + ") ");
                            ps.Add("EmailA" + i, "%," + cond.EmailTo[i] + ",%");

                            // 入力条件
                        }
                        sqlSb.Append(string.Join("OR", list));
                        sqlSb.Append(" ) AND");
                    }
                    else
                    {
                        sqlSb.Append("  ',' + MT01.EMAIL_TO + ',' LIKE @EmailA AND");

                        // 入力条件
                        ps.Add("EmailA", "%," + cond.EmailTo[0] + ",%");
                    }
                }

                // その他送信先
                if (cond.Email != null && cond.Email.Count > 0)
                {
                    if (cond.Email.Count > 1) {
                        var list = new List<string>();
                        sqlSb.Append(" (");
                        for (int i = 0; i < cond.Email.Count; i++)
                        {
                            // 部分一致検索 メールアドレス
                            list.Add(" (MT01.EMAIL_TO_OTHER LIKE @EmailB" + i + ") ");
                            ps.Add("EmailB" + i, "%" + cond.Email[i] + "%");

                            // 入力条件
                        }
                        sqlSb.Append(string.Join("OR", list));
                        sqlSb.Append(" ) AND");
                    }
                    else
                    {
                        // 部分一致検索 メールアドレス
                        sqlSb.Append("  MT01.EMAIL_TO_OTHER LIKE @EmailB AND");

                        // 入力条件
                        ps.Add("EmailB", "%" + cond.Email[0] + "%");
                    }
                }

                String sqlOrderby = "  ORDER BY MT01.MANAGEMENT_ID DESC";
                String sql = "";
                if (ps.Count > 0)
                {
                    sql = sqlSb1.ToString() + sqlSb.ToString().Substring(0, sqlSb.ToString().LastIndexOf("AND") - 1) + sqlOrderby;
                }
                else {
                    sql = sqlSb1.ToString() + sqlOrderby;
                }
                
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
        /// テスト送信用メソッド
        /// </summary>
        /// <returns></returns>
        public string DoTestMailSend()
        {

            try
            {
                // 送信元
                var from = new EmailAddress(SystemConfig.AppSettings["SenderMail"], SystemConfig.AppSettings["SenderName"]);

                // 送信宛先リスト
                List<EmailAddress> toList = new List<EmailAddress>();

                // 送信宛先の設定
                // テストメール宛先を取得する
                string[] masterTos = this.GetRequestVal("EmailList").ToString().Split(",");

                // テストメール宛先の件数分ループ
                foreach (string masterTo in masterTos)
                {
                    // 送信宛先を連絡先マスタのメール宛先に設定する
                    var to = new EmailAddress(masterTo, "aaa");

                    // 送信宛先リストに追加
                    toList.Add(to);
                }

                // タイトル
                var subject = this.GetRequestVal("Temp_Name");

                // 送信内容の設定
                var mailContent = this.GetRequestVal("Temp_Contents");

                // 認証キー
                var apiKey = SystemConfig.AppSettings["SendGridKey"];

                // 送信オブジェクト
                var client = new SendGridClient(apiKey);

                // メール送信
                appForm.Execute(client, from, toList, subject, mailContent).Wait();

                return "0";
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 管理用タイトルより、メール送信設定情報取得
        /// </summary>
        /// <returns></returns>
        private string getDataByName(string name, string id) {

            try
            {
                // Sql文と条件設定の取得
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append("SELECT COUNT(*) FROM MT_MN_EMAIL_SET WHERE MANAGEMENT_TITLE = @ManagementTitle ");
                Paras ps = new Paras();
                // 入力条件
                ps.Add("ManagementTitle", name);

                if (!string.IsNullOrEmpty(id))
                {

                    // 管理用ID
                    sqlSb.Append(" AND MANAGEMENT_ID <> @ManagementId");

                    // 入力条件
                    ps.Add("ManagementId", id);
                }

                int ret = BP.DA.DBAccess.RunSQLReturnValInt(sqlSb.ToString(), ps);
                if (ret > 0)
                {
                    string[] list = { "管理用タイトル" };
                    return "msg@" + BP.Tools.Json.ToJson(appForm.SetRtnMessage("E0002", list));
                }
                else
                {
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// メール送信設定情報登録
        /// </summary>
        /// <returns></returns>
        public string InsertEmailSet()
        {
            int result = -1;
            try
            {
                // メール送信設定登録情報の取得
                UpdateEmailSetReq cond =
                    JsonConvert.DeserializeObject<UpdateEmailSetReq>(
                        this.GetRequestVal("UpdateEmailSetReq"));

                string retGet = getDataByName(cond.ManagementTitle, null);
                if (retGet.StartsWith("err@") || retGet.StartsWith("msg@"))
                {
                    return retGet;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("insert into MT_MN_EMAIL_SET (");
                sb.Append("WF_EMAIL_KBN,EMAIL_TIMING_KBN,SINSEISYA_KBN,MANAGEMENT_ID,MANAGEMENT_TITLE,EMAIL_TO,EMAIL_TO_OTHER,EMAIL_CC,EMAIL_CC_OTHER,EMAIL_BCC,EMAIL_BCC_OTHER,EMAIL_TEMP_NAME,EMAIL_TEMP_CONTENTS,OPERATION_FLG,REMARKS,REC_ENT_DATE,REC_ENT_USER,REC_EDT_DATE,REC_EDT_USER,REC_ENT_USER_KANJI,REC_EDT_USER_KANJI");
                sb.Append(") values (");
                sb.Append("@WfEmailKbn,@EmailTimingKbn,@SinseisyaKbn,");
                sb.Append(" case when (select max(MANAGEMENT_ID) from MT_MN_EMAIL_SET) is null then 1 else (select max(MANAGEMENT_ID) from MT_MN_EMAIL_SET) + 1 end ,");
                sb.Append("@ManagementTitle,@EmailTo,@EmailToOther,@EmailCc,@EmailCcOther,@EmailBcc,@EmailBccOther,@EmailTempName,@EmailTempContents,@OperationFlg,@Remarks,@REC_ENT_DATE,@REC_ENT_USER,@REC_EDT_DATE,@REC_EDT_USER,@REC_ENT_USER_KANJI,@REC_EDT_USER_KANJI)");

                Paras ps = new Paras();
                // 入力条件
                ps.Add("ManagementTitle", cond.ManagementTitle);
                ps.Add("WfEmailKbn", cond.WfEmailKbn);
                ps.Add("EmailTimingKbn", cond.EmailTimingKbn);
                ps.Add("SinseisyaKbn", cond.SinseisyaKbn);
                ps.Add("EmailTo", cond.EmailTo);
                ps.Add("EmailToOther", cond.EmailToOther);
                ps.Add("EmailCc", cond.EmailCc);
                ps.Add("EmailCcOther", cond.EmailCcOther);
                ps.Add("EmailBcc", cond.EmailBcc);
                ps.Add("EmailBccOther", cond.EmailBccOther);
                ps.Add("EmailTempName", cond.EmailTempName);
                ps.Add("EmailTempContents", this.GetRequestVal("EmailTempContents"));
                ps.Add("OperationFlg", cond.OperationFlg);
                ps.Add("REMARKS", this.GetRequestVal("Remarks"));
                ps.Add("REC_ENT_USER", cond.REC_ENT_USER);
                ps.Add("REC_ENT_USER_KANJI", cond.REC_ENT_USER_KANJI);
                ps.Add("REC_ENT_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_USER", cond.REC_ENT_USER);
                ps.Add("REC_EDT_USER_KANJI", cond.REC_ENT_USER_KANJI);
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());

                result = BP.DA.DBAccess.RunSQL(sb.ToString(), ps);
                if (result < 1)
                {
                    return "err@" + "登録は失敗しました。";
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// メール送信設定情報更新
        /// </summary>
        /// <returns></returns>
        public string UpdateEmailSet() {

            // メール送信設定登録情報の取得
            UpdateEmailSetReq cond =
                JsonConvert.DeserializeObject<UpdateEmailSetReq>(
                    this.GetRequestVal("UpdateEmailSetReq"));

            string retGet = getDataByName(cond.ManagementTitle, cond.ManagementId);
            if (retGet.StartsWith("err@") || retGet.StartsWith("msg@"))
            {
                return retGet;
            }

            int result = -1;
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE MT_MN_EMAIL_SET ");
                sb.Append("   set MANAGEMENT_TITLE = @ManagementTitle, ");
                sb.Append("       WF_EMAIL_KBN = @WfEmailKbn, ");
                sb.Append("       EMAIL_TIMING_KBN = @EmailTimingKbn, ");
                sb.Append("       SINSEISYA_KBN = @SinseisyaKbn, ");
                sb.Append("       EMAIL_TO = @EmailTo, ");
                sb.Append("       EMAIL_TO_OTHER = @EmailToOther, ");
                sb.Append("       EMAIL_CC = @EmailCc, ");
                sb.Append("       EMAIL_CC_OTHER = @EmailCcOther, ");
                sb.Append("       EMAIL_BCC = @EmailBcc, ");
                sb.Append("       EMAIL_BCC_OTHER = @EmailBccOther, ");
                sb.Append("       EMAIL_TEMP_NAME = @EmailTempName, ");
                sb.Append("       EMAIL_TEMP_CONTENTS = @EmailTempContents, ");
                sb.Append("       OPERATION_FLG = @OperationFlg, ");
                sb.Append("       REMARKS = @Remarks, ");
                sb.Append("       REC_EDT_DATE = @REC_EDT_DATE, ");
                sb.Append("       REC_EDT_USER = @REC_EDT_USER, ");
                sb.Append("       REC_EDT_USER_KANJI = @REC_EDT_USER_KANJI ");
                sb.Append(" WHERE MANAGEMENT_ID = @ManagementId ");

                Paras ps = new Paras();
                // 入力条件
                ps.Add("WfEmailKbn", cond.WfEmailKbn);
                ps.Add("EmailTimingKbn", cond.EmailTimingKbn);
                ps.Add("SinseisyaKbn", cond.SinseisyaKbn);
                ps.Add("ManagementId", cond.ManagementId);
                ps.Add("ManagementTitle", cond.ManagementTitle);
                ps.Add("EmailTo", cond.EmailTo);
                ps.Add("EmailToOther", cond.EmailToOther);
                ps.Add("EmailCc", cond.EmailCc);
                ps.Add("EmailCcOther", cond.EmailCcOther);
                ps.Add("EmailBcc", cond.EmailBcc);
                ps.Add("EmailBccOther", cond.EmailBccOther);
                ps.Add("EmailTempName", cond.EmailTempName);
                ps.Add("EmailTempContents", this.GetRequestVal("EmailTempContents"));
                ps.Add("OperationFlg", cond.OperationFlg);
                ps.Add("REMARKS", this.GetRequestVal("Remarks"));
                ps.Add("REC_EDT_USER", cond.REC_ENT_USER);
                ps.Add("REC_EDT_USER_KANJI", cond.REC_ENT_USER_KANJI);
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());

                result = BP.DA.DBAccess.RunSQL(sb.ToString(), ps);
                if (result < 1)
                {
                    return "err@" + "更新は失敗しました。";
                }

            } catch (Exception ex) {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// メール送信設定情報の稼働フラグを更新
        /// </summary>
        /// <returns></returns>
        public string UpdateOperationFlg()
        {
            int result = -1;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE MT_MN_EMAIL_SET ");
                sb.Append("   SET OPERATION_FLG = @OperationFlg ");
                sb.Append(" WHERE MANAGEMENT_ID = @ManagementId ");

                Paras ps = new Paras();
                // 入力条件
                // 入力条件
                ps.Add("ManagementId", this.GetRequestVal("ManagementId"));
                ps.Add("OperationFlg", this.GetRequestVal("OperationFlg"));

                result = BP.DA.DBAccess.RunSQL(sb.ToString(), ps);
                if (result < 1)
                {
                    return "err@" + "稼働フラグの更新は失敗しました。";
                }

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// メール送信設定情報削除
        /// </summary>
        /// <returns></returns>
        public string DeleteEmailSet() {

            int result = -1;
            try
            {
                String sql = "DELETE FROM MT_MN_EMAIL_SET WHERE MANAGEMENT_ID = @ManagementId";
                Paras ps = new Paras();
                // 入力条件
                ps.Add("ManagementId", this.GetRequestVal("ManagementId"));

                result = BP.DA.DBAccess.RunSQL(sql, ps);
                if (result < 1) {
                    return "err@" + "削除に失敗しました。";
                }

            } catch (Exception ex) {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// 送信先一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetEmailStakeholderList()
        {
            try
            {
                // Sql文と条件設定の取得
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append("SELECT DISTINCT ");
                sqlSb.Append("       CASE WHEN SUBSTRING(SOUSINSAKI_TYPE , 4, 1) = 'S' THEN SOUSINSAKI_TYPE ");
                sqlSb.Append("            ELSE RNRAKUSAKI_ID END AS RNRAKUSAKI_ID, ");
                sqlSb.Append("       RNRAKUSAKI_NAME, ");
                sqlSb.Append("       SOUSINSAKI_TYPE ");
                sqlSb.Append("  FROM MT_MN_EMAIL_STAKEHOLDER ");
                sqlSb.Append("  order by SOUSINSAKI_TYPE desc ");

                // Sqlの実行
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
        /// 一覧検索条件クラス
        /// </summary>
        private class EmailSetReq
        {
            /// <summary>
            /// ワークフロー
            /// </summary>
            public string WfEmailKbn { get; set; }

            /// <summary>
            /// 送信タイミング
            /// </summary>
            public string EmailTimingKbn { get; set; }

            /// <summary>
            /// 申請者区分
            /// </summary>
            public string SinseisyaKbn { get; set; }

            /// <summary>
            /// 管理タイトル
            /// </summary>
            public string ManagementTitle { get; set; }

            /// <summary>
            /// 送信先To　その他送信先
            /// </summary>
            public List<string> Email { get; set; }

            /// <summary>
            /// 稼働フラグ
            /// </summary>
            public string OperationFlg { get; set; }

            /// <summary>
            /// 送信先To
            /// </summary>
            public List<string> EmailTo { get; set; }
        }

        /// <summary>
        /// 自動メール送信設定登録・更新用クラス
        /// </summary>
        private class UpdateEmailSetReq
        {
            /// <summary>
            /// ワークフロー
            /// </summary>
            public string WfEmailKbn { get; set; }

            /// <summary>
            /// 送信タイミング
            /// </summary>
            public string EmailTimingKbn { get; set; }

            /// <summary>
            /// 申請者区分
            /// </summary>
            public string SinseisyaKbn { get; set; }

            /// <summary>
            /// 管理ID
            /// </summary>
            public string ManagementId { get; set; }

            /// <summary>
            /// 管理タイトル
            /// </summary>
            public string ManagementTitle { get; set; }

            /// <summary>
            /// 送信先To
            /// </summary>
            public string EmailTo { get; set; }

            /// <summary>
            /// 送信先To　その他
            /// </summary>
            public string EmailToOther { get; set; }

            /// <summary>
            /// 送信先CC
            /// </summary>
            public string EmailCc { get; set; }

            /// <summary>
            /// 送信先CC　その他
            /// </summary>
            public string EmailCcOther { get; set; }

            /// <summary>
            /// 送信先BCC　その他
            /// </summary>
            public string EmailBcc { get; set; }

            /// <summary>
            /// 送信先BCC　その他
            /// </summary>
            public string EmailBccOther { get; set; }

            /// <summary>
            /// テンプレート件名
            /// </summary>
            public string EmailTempName { get; set; }

            /// <summary>
            /// 稼働フラグ
            /// </summary>
            public string OperationFlg { get; set; }

            /// <summary>
            /// 更新ユーザID
            /// </summary>
            public string REC_ENT_USER { get; set; }

            /// <summary>
            /// 更新ユーザ名
            /// </summary>
            public string REC_ENT_USER_KANJI { get; set; }
        }
    }
}
