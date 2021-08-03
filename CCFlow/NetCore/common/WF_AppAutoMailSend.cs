using BP.DA;
using BP.Sys;
using Common.WF_OutLog;
using Org.BouncyCastle.Utilities.Collections;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BP.WF.HttpHandler
{
    abstract public class WF_AppAutoMailSend : BP.WF.HttpHandler.DirectoryPageBase
    {
        protected const string KAHEN_FLG_KOTEI = "1";    // 可変区分:固定
        protected const string SINSEISYA_KBN_HONNIN = "0";  // 従業者本人
        protected const string SINSEISYA_KBN_HONNIN_PRO = "009A01";  // 従業者本人（プロパー）
        protected const string SINSEISYA_KBN_HONNIN_SYUKO = "009A02";  // 従業者本人（出向者）
        protected const string SINSEISYA_KBN_DAIRI_PRO = "009A03";  // 代理申請者（ご不幸の従業員：プロパー）
        protected const string SINSEISYA_KBN_DAIRI_SYUKO = "009A04";  // 代理申請者（ご不幸の従業員：出向者）
        protected const string SINSEISYA_KBN_TEHAI = "009B01";  // ⼿配業者

        protected const string SOUSINSAKI_TYPE_HONNIN = "01";  // 従業員本人
        protected const string SOUSINSAKI_TYPE_DAIRI = "02";  // 代理申請者
        protected const string SOUSINSAKI_TYPE_SYUKOMOTO_JYOSI = "03";  // （社籍・出向元）上司
        protected const string SOUSINSAKI_TYPE_SYUKOSAKI_JYOSI = "04";  // （出向先）上司
        protected const string SOUSINSAKI_TYPE_SYUKOMOTO_SOUMU = "05";  // （社籍・出向元）店舗総務部
        protected const string SOUSINSAKI_TYPE_SYUKOSAKI_SOUMU = "06";  // （出向先）店舗総務部
        protected const string SOUSINSAKI_TYPE_SYASEKI_JINJI = "S1";  // （社籍・出向元）所轄人事部
        protected const string SOUSINSAKI_TYPE_SYUKOSAKI_JINJI = "S2";  // （出向先）所轄人事部

        /// <summary>
        /// メール送信のメインメソッド
        /// </summary>
        public string doMailSend()
        {

            // 自動送信判定
            if (SystemConfig.AppSettings["AutoMailFlg"] == "0")
            {
                return "自動送信停止中...";
            }

            try
            {
                // トランザクション情報の取得
                DataTable transDt = getTransactionInfo();

                // 本人情報の取得
                List<Dictionary<string, string>> selfInfo = getSelfInfo(transDt);

                // 代理者情報の取得
                List<Dictionary<string, string>> agentInfo = getAgentInfo(transDt);

                // 申請者区分の取得
                string sinseisyaKbn = getSinseisyaKbn(transDt, selfInfo, agentInfo);

                // メール送信のマスタデータの取得
                DataTable mailDt = getMailMaster(sinseisyaKbn);

                // 送信ロジック
                doMailSendMainLogic(mailDt, transDt, selfInfo, agentInfo);

                // 正常結果
                return "ok";
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// トランザクション情報の取得
        /// </summary>
        /// <returns></returns>
        abstract protected DataTable getTransactionInfo();

        /// <summary>
        /// 申請者区分取得メソッド
        /// </summary>
        /// <returns></returns>
        abstract protected string getSinseisyaKbn(DataTable transDt, List<Dictionary<string, string>> selfInfo, List<Dictionary<string, string>> agentInfo);

        /// <summary>
        /// キーワード置換メソッド（子クラス用）
        /// </summary>
        /// <returns></returns>
        virtual protected string doTemplateRepaceCustom(string template, DataRow transRow, DataRow mailRow) { 
            return template; 
        }

        /// <summary>
        /// 本人情報の取得
        /// </summary>
        /// <returns></returns>
        private List<Dictionary<string, string>> getSelfInfo(DataTable transDt)
        {
            // APIパラメータ設定
            Dictionary<string, object> unfParam = new Dictionary<string, object>();
            unfParam.Add("ShainBango", transDt.Rows[0]["UNF_SHAINBANGO"].ToString());

            // 本人情報取得
            List<Dictionary<string, string>> unfInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Shain_Info", unfParam);

            return unfInfo;
        }

        /// <summary>
        /// 代理者情報の取得
        /// </summary>
        /// <returns></returns>
        private List<Dictionary<string, string>> getAgentInfo(DataTable transDt)
        {
            // APIパラメータ設定
            Dictionary<string, object> appParam = new Dictionary<string, object>();
            appParam.Add("ShainBango", transDt.Rows[0]["DAIRI_SHAINBANGO"].ToString());

            // 代理者情報取得
            List<Dictionary<string, string>> dairiInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Shain_Info", appParam);

            return dairiInfo;
        }

        /// <summary>
        /// メール送信のマスタデータの取得
        /// </summary>
        /// <returns>メール送信のマスタデータ</returns>
        private DataTable getMailMaster(string sinseisyaKbn)
        {

            // テンプレート取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("SELECT A.MANAGEMENT_ID AS ID,");
            sqlSb.Append("       A.EMAIL_TEMP_NAME AS TITLE,");
            sqlSb.Append("       A.EMAIL_TEMP_CONTENTS AS CONTENT,");
            sqlSb.Append("       A.EMAIL_TO AS ETO,");
            sqlSb.Append("       A.EMAIL_CC AS ECC,");
            sqlSb.Append("       A.EMAIL_BCC AS EBCC,");
            sqlSb.Append("       A.EMAIL_TO_OTHER AS TO_OTHER,");
            sqlSb.Append("       A.EMAIL_CC_OTHER AS CC_OTHER,");
            sqlSb.Append("       A.EMAIL_BCC_OTHER AS BCC_OTHER,");
            sqlSb.Append("       A.WF_EMAIL_KBN AS WF_EMAIL_KBN");

            // メール送信設定マスタ
            sqlSb.Append("  FROM MT_MN_EMAIL_SET A");

            // 条件
            sqlSb.Append(" WHERE A.WF_EMAIL_KBN = @wfKbn AND");
            sqlSb.Append("       A.EMAIL_TIMING_KBN = @timingKbn AND");
            sqlSb.Append("       A.SINSEISYA_KBN = @sinseisyaKbn AND");
            sqlSb.Append("       A.OPERATION_FLG = '1'");
            sqlSb.Append("ORDER BY ID;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフロー区分
            ps.Add("wfKbn", this.GetRequestVal("wfKbn"));

            // 送信タイミング区分
            ps.Add("timingKbn", this.GetRequestVal("timingKbn"));

            // 申請者区分
            ps.Add("sinseisyaKbn", sinseisyaKbn);

            try
            {
                // メールテンプレートの取得
                return BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// メール送信のメインロジック
        /// </summary>
        /// <returns>メール送信のマスタデータ</returns>
        private void doMailSendMainLogic(DataTable mailDt, DataTable transDt, List<Dictionary<string, string>> selfInfo, List<Dictionary<string, string>> agentInfo)
        {
            try
            {
                // 検索結果のループ
                int cnt = mailDt.Rows.Count;
                for (int i = 0; i < cnt; i++)
                {
                    // TOリスト
                    List<EmailAddress> toList = new List<EmailAddress>();

                    // CCリスト
                    List<EmailAddress> ccList = new List<EmailAddress>();

                    // BCCリスト
                    List<EmailAddress> bccList = new List<EmailAddress>();

                    // 送信宛先の設定
                    if (mailDt.Rows[i]["ETO"] != DBNull.Value && !string.IsNullOrWhiteSpace(mailDt.Rows[i]["ETO"].ToString())) {
                        setMailsByStakeholder(mailDt.Rows[i]["ETO"].ToString(), transDt.Rows[0], toList, selfInfo, agentInfo);
                    }

                    // CCの設定
                    if (mailDt.Rows[i]["ECC"] != DBNull.Value && !string.IsNullOrWhiteSpace(mailDt.Rows[i]["ECC"].ToString()))
                    {
                        setMailsByStakeholder(mailDt.Rows[i]["ECC"].ToString(), transDt.Rows[0], toList, selfInfo, agentInfo);
                    }

                    // BCCの設定
                    if (mailDt.Rows[i]["EBCC"] != DBNull.Value && !string.IsNullOrWhiteSpace(mailDt.Rows[i]["EBCC"].ToString()))
                    {
                        setMailsByStakeholder(mailDt.Rows[i]["EBCC"].ToString(), transDt.Rows[0], toList, selfInfo, agentInfo);
                    }

                    // 送信宛先その他
                    if (mailDt.Rows[i]["TO_OTHER"] != DBNull.Value && !string.IsNullOrWhiteSpace(mailDt.Rows[i]["TO_OTHER"].ToString()))
                    {
                        // 「，」で分割
                        string[] mails = mailDt.Rows[i]["TO_OTHER"].ToString().Split(",");

                        // 件数分ループ
                        foreach (string mail in mails)
                        {
                            // 送信宛先の設定
                            var mailAddress = new EmailAddress(mail);

                            // 送信宛先リストに追加
                            toList.Add(mailAddress);
                        }
                    }

                    // 送信宛先リストが空の場合
                    if (toList.Count == 0)
                    {
                        // 送信されない
                        continue;
                    }

                    // 送信元
                    var from = new EmailAddress(SystemConfig.AppSettings["SenderMail"], SystemConfig.AppSettings["SenderName"]);

                    // タイトル
                    var subject = mailDt.Rows[i]["TITLE"].ToString();

                    // 送信内容の設定
                    var mailContent = doTemplateRepace(mailDt.Rows[i]["CONTENT"].ToString(), mailDt.Rows[i]["WF_EMAIL_KBN"].ToString());

                    // 子クラスの送信内容の設定メソッド
                    mailContent = doTemplateRepaceCustom(mailContent, transDt.Rows[0], mailDt.Rows[i]);

                    // 認証キー
                    var apiKey = SystemConfig.AppSettings["SendGridKey"];

                    // 送信オブジェクト
                    var client = new SendGridClient(apiKey);

                    // メール送信設定
                    SendGridMessage msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toList, subject, mailContent, mailContent);

                    // CCその他
                    if (mailDt.Rows[i]["CC_OTHER"] != DBNull.Value && !string.IsNullOrWhiteSpace(mailDt.Rows[i]["CC_OTHER"].ToString()))
                    {
                        // 「，」で分割
                        string[] mails = mailDt.Rows[i]["CC_OTHER"].ToString().Split(",");

                        // 件数分ループ
                        foreach (string mail in mails)
                        {
                            // 送信宛先の設定
                            var mailAddress = new EmailAddress(mail);

                            // 送信宛先リストに追加
                            ccList.Add(mailAddress);
                        }
                    }

                    // CC
                    if (ccList.Count != 0)
                    {
                        msg.AddCcs(ccList);
                    }

                    // BCCその他
                    if (mailDt.Rows[i]["BCC_OTHER"] != DBNull.Value && !string.IsNullOrWhiteSpace(mailDt.Rows[i]["BCC_OTHER"].ToString()))
                    {
                        // 「，」で分割
                        string[] mails = mailDt.Rows[i]["BCC_OTHER"].ToString().Split(",");

                        // 件数分ループ
                        foreach (string mail in mails)
                        {
                            // 送信宛先の設定
                            var mailAddress = new EmailAddress(mail);

                            // 送信宛先リストに追加
                            bccList.Add(mailAddress);
                        }
                    }

                    // BCC
                    if (bccList.Count != 0)
                    {
                        msg.AddBccs(bccList);
                    }
                        
                    // 送信実行
                    Execute(client, msg).Wait();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 申請者区分取得メソッド
        /// </summary>
        /// <returns></returns>
        private void setMailsByStakeholder(string typeStr,DataRow transRow, List<EmailAddress> mailList, List<Dictionary<string, string>> selfInfo, List<Dictionary<string, string>> agentInfo)
        {
            // 区切り
            string[] types = typeStr.Split(",");

            // 件数分ループ
            foreach (string type in types)
            {
                // テンプレート取得用sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // 入力条件の置換
                Paras ps = new Paras();

                // 項目
                sqlSb.Append("SELECT A.SOUSINSAKI_TYPE AS TYPE,");
                sqlSb.Append("       A.EMAIL AS EMAIL");

                // メール送信設定マスタ
                sqlSb.Append("  FROM MT_MN_EMAIL_STAKEHOLDER A");

                // 送信先タイプの判定
                if (type.Length > 3 && type.Substring(3, 1) == "S")
                {
                    // 条件
                    sqlSb.Append(" WHERE A.WF_NO = @WF_NO AND");
                    sqlSb.Append("       A.KAISHACODE = @KAISHACODE AND");
                    sqlSb.Append("       A.SOUSINSAKI_TYPE = @SOUSINSAKI_TYPE;");

                    // ワークフロー区分
                    ps.Add("WF_NO", this.GetRequestVal("wfKbn"));

                    // 社籍・出向元の場合
                    if (type.Substring(4, 1) == "1")
                    {
                        // 出向元会社コード
                        ps.Add("KAISHACODE", transRow["KAISYACODE"]);
                    }
                    else
                    {
                        // 出向先会社コード
                        ps.Add("KAISHACODE", transRow["SYUKOSAKIKAISYACODE"]);
                    }

                    // 送信先タイプ
                    ps.Add("SOUSINSAKI_TYPE", type);
                }
                else
                {
                    // 連絡先ID
                    sqlSb.Append(" WHERE A.RNRAKUSAKI_ID = @RNRAKUSAKI_ID ;");

                    // 送信先タイプ
                    ps.Add("RNRAKUSAKI_ID", type);
                }

                // 検索結果
                DataTable stakeholderDt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

                // メールアドレス取得
                setMailAddress(stakeholderDt.Rows[0], mailList, selfInfo, agentInfo);
            }
        }

        /// <summary>
        /// メールアドレス取得メソッド
        /// </summary>
        /// <returns></returns>
        private void setMailAddress(DataRow stakeholderRow, List<EmailAddress> mailList, List<Dictionary<string, string>> unfInfo, List<Dictionary<string, string>> dairiInfo)
        {

            // 送信先タイプで判定する
            string type = stakeholderRow["TYPE"].ToString();
            switch (type.Substring(type.Length-2))
            {

                // 従業員本人
                case SOUSINSAKI_TYPE_HONNIN:

                    if (!(string.IsNullOrWhiteSpace(unfInfo[0]["USERMAILADDRESS1"])))
                    {
                        mailList.Add(new EmailAddress(unfInfo[0]["USERMAILADDRESS1"]));

                        if (!(string.IsNullOrWhiteSpace(unfInfo[0]["USERMAILADDRESS2"])))
                        {
                            mailList.Add(new EmailAddress(unfInfo[0]["USERMAILADDRESS2"]));
                        }
                    }
                    break;

                // 代理申請者
                case SOUSINSAKI_TYPE_DAIRI:
                    if (!(string.IsNullOrWhiteSpace(dairiInfo[0]["USERMAILADDRESS1"])))
                    {
                        mailList.Add(new EmailAddress(dairiInfo[0]["USERMAILADDRESS1"]));

                        if (!(string.IsNullOrWhiteSpace(dairiInfo[0]["USERMAILADDRESS2"])))
                        {
                            mailList.Add(new EmailAddress(dairiInfo[0]["USERMAILADDRESS2"]));
                        }
                    }
                    break;

                //（社籍・出向元）上司
                case SOUSINSAKI_TYPE_SYUKOMOTO_JYOSI:
                //（出向先）上司
                case SOUSINSAKI_TYPE_SYUKOSAKI_JYOSI:

                    // チームマスタからの取得
                    // APIパラメータ設定
                    Dictionary<string, object> seniorParam = new Dictionary<string, object>();
                    seniorParam.Add("SHOZOKUCODE", unfInfo[0]["JINJISHOZOKUCODE"]);
                    seniorParam.Add("TEAMCODE", unfInfo[0]["TEAMCODE"]);
                    seniorParam.Add("SHAINBANGO", unfInfo[0]["SHAINBANGO"]);

                    // 上長の取得
                    List<Dictionary<string, string>> seniorInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Senior_Info", seniorParam);

                    // チームマスタから取得できる場合
                    if (seniorInfo != null && seniorInfo.Count > 0)
                    {
                        // 送信先リストに追加する
                        if (!(string.IsNullOrWhiteSpace(seniorInfo[0]["USERMAILADDRESS1"])))
                        {
                            mailList.Add(new EmailAddress(seniorInfo[0]["USERMAILADDRESS1"]));

                            if (!(string.IsNullOrWhiteSpace(seniorInfo[0]["USERMAILADDRESS2"])))
                            {
                                mailList.Add(new EmailAddress(seniorInfo[0]["USERMAILADDRESS2"]));
                            }
                        }
                        break;
                    }

                    // 初期化
                    seniorParam.Clear();
                    seniorInfo.Clear();

                    // 承認者マスタからの取得
                    seniorParam = new Dictionary<string, object>();
                    seniorParam.Add("SHOZOKUCODE", unfInfo[0]["JINJISHOZOKUCODE"]);
                    seniorParam.Add("SHONINSHAKBN", "UW001");
                    seniorParam.Add("SHAINBANGO", unfInfo[0]["SHAINBANGO"]);

                    // 上長の取得
                    seniorInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Director_Info", seniorParam);

                    // 送信先リストに追加する
                    if (seniorInfo != null && seniorInfo.Count > 0)
                    {
                        // 送信先リストに追加する
                        if (!(string.IsNullOrWhiteSpace(seniorInfo[0]["USERMAILADDRESS1"])))
                        {
                            mailList.Add(new EmailAddress(seniorInfo[0]["USERMAILADDRESS1"]));

                            if (!(string.IsNullOrWhiteSpace(seniorInfo[0]["USERMAILADDRESS2"])))
                            {
                                mailList.Add(new EmailAddress(seniorInfo[0]["USERMAILADDRESS2"]));
                            }
                        }
                    }
                    break;

                //（社籍・出向元）店舗総務部
                case SOUSINSAKI_TYPE_SYUKOMOTO_SOUMU:
                //（出向先）店舗総務部
                case SOUSINSAKI_TYPE_SYUKOSAKI_SOUMU:

                    // APIパラメータ設定
                    Dictionary<string, object> managerParam = new Dictionary<string, object>();
                    managerParam.Add("BELONGS_CODE", unfInfo[0]["JINJISHOZOKUCODE"]);
                    managerParam.Add("CORP_CODE", unfInfo[0]["KAISHACODE"]);
                    managerParam.Add("GROUP_KBN", "G0001");

                    // 店舗総務の取得
                    List<Dictionary<string, string>> managerInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Approval_Info", managerParam);

                    // 送信先リストに追加する
                    if (managerInfo != null && managerInfo.Count > 0)
                    {
                        foreach (Dictionary<string, string> dr in managerInfo)
                        {
                            if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS1"])))
                            {
                                mailList.Add(new EmailAddress(dr["USERMAILADDRESS1"]));

                                if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS2"])))
                                {
                                    mailList.Add(new EmailAddress(dr["USERMAILADDRESS2"]));
                                }
                            }
                        }
                    }
                    break;

                //（社籍・出向元）所轄人事部
                case SOUSINSAKI_TYPE_SYASEKI_JINJI:

                    // （社籍・出向元）所轄人事部メールリスト
                    List<EmailAddress> motoJinji = new List<EmailAddress>();

                    // 出向されない場合
                    if (string.IsNullOrWhiteSpace(unfInfo[0]["SHUKKOKBN"]) || unfInfo[0]["SHUKKOKBN"] == "0000")
                    {
                        // APIパラメータ設定
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param.Add("KAISHACODE", unfInfo[0]["KAISHACODE"]);
                        param.Add("SHIKIBETSUKBN", "63");

                        // 店舗総務の取得
                        List<Dictionary<string, string>> info = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Jinji_Info", param);

                        // 送信先リストに追加する
                        if (info != null && info.Count > 0)
                        {
                            foreach (Dictionary<string, string> dr in info)
                            {
                                if (!(string.IsNullOrWhiteSpace(dr["BIKO"])))
                                {
                                    motoJinji.Add(new EmailAddress(dr["BIKO"]));
                                }
                            }
                        }

                        // 取得出来ない場合
                        if (motoJinji.Count == 0)
                        {
                            // 連絡先マスタのメール宛先を取得する
                            string[] jinjis = stakeholderRow["EMAIL"].ToString().Split(",");

                            // 連絡先マスタのメール宛先の件数分ループ
                            foreach (string jinji in jinjis)
                            {
                                // 送信宛先を連絡先マスタのメール宛先に設定する
                                var mailAddress = new EmailAddress(jinji);

                                // 送信宛先リストに追加
                                mailList.Add(mailAddress);
                            }
                        }
                        else
                        {
                            mailList.AddRange(motoJinji);
                        }
                        break;
                    }

                    // 出向の場合
                    // APIパラメータ設定
                    Dictionary<string, object> secondedParam = new Dictionary<string, object>();
                    secondedParam.Add("KAISHACODE", unfInfo[0]["KAISHACODE"]);
                    secondedParam.Add("SHONINKENGEN", "G2001");

                    // 各社所轄人事の取得
                    List<Dictionary<string, string>> secondedInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_SecondedSource_Info", secondedParam);

                    // 送信先リストに追加する
                    if (secondedInfo != null && secondedInfo.Count > 0)
                    {
                        foreach (Dictionary<string, string> dr in secondedInfo)
                        {
                            if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS1"])))
                            {
                                motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS1"]));

                                if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS2"])))
                                {
                                    motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS2"]));
                                }
                            }
                        }

                        // 取得出来る場合
                        if (motoJinji.Count != 0)
                        {
                            mailList.AddRange(motoJinji);
                            break;
                        }
                    }

                    // 初期化
                    secondedParam.Clear();
                    secondedInfo.Clear();

                    // APIパラメータ設定
                    secondedParam = new Dictionary<string, object>();
                    secondedParam.Add("KAISHACODE", unfInfo[0]["KAISHACODE"]);
                    secondedParam.Add("SHONINKENGEN", "G4001");

                    // 本社人事の取得
                    secondedInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_SecondedSource_Info", secondedParam);

                    // 送信先リストに追加する
                    if (secondedInfo != null && secondedInfo.Count > 0)
                    {
                        foreach (Dictionary<string, string> dr in secondedInfo)
                        {
                            if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS1"])))
                            {
                                motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS1"]));

                                if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS2"])))
                                {
                                    motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS2"]));
                                }
                            }
                        }

                        // 取得出来る場合
                        if (motoJinji.Count != 0)
                        {
                            mailList.AddRange(motoJinji);
                            break;
                        }
                        break;
                    }

                    // 初期化
                    secondedParam.Clear();
                    secondedInfo.Clear();

                    // APIパラメータ設定
                    secondedParam = new Dictionary<string, object>();
                    secondedParam.Add("KAISHACODE", unfInfo[0]["KAISHACODE"]);
                    secondedParam.Add("SHONINKENGEN", "G4002");

                    // イオンベーカリー本社人事の取得
                    secondedInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_SecondedSource_Info", secondedParam);

                    // 送信先リストに追加する
                    if (secondedInfo != null && secondedInfo.Count > 0)
                    {
                        foreach (Dictionary<string, string> dr in secondedInfo)
                        {
                            if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS1"])))
                            {
                                motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS1"]));

                                if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS2"])))
                                {
                                    motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS2"]));
                                }
                            }
                        }

                        // 取得出来る場合
                        if (motoJinji.Count != 0)
                        {
                            mailList.AddRange(motoJinji);
                            break;
                        }
                    }

                    // 初期化
                    secondedParam.Clear();
                    secondedInfo.Clear();

                    // APIパラメータ設定
                    secondedParam = new Dictionary<string, object>();
                    secondedParam.Add("KAISHACODE", unfInfo[0]["KAISHACODE"]);
                    secondedParam.Add("SHONINKENGEN", "G1001");

                    // BS業務部の取得
                    secondedInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_SecondedSource_Info", secondedParam);

                    // 送信先リストに追加する
                    if (secondedInfo != null && secondedInfo.Count > 0)
                    {
                        foreach (Dictionary<string, string> dr in secondedInfo)
                        {
                            if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS1"])))
                            {
                                motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS1"]));

                                if (!(string.IsNullOrWhiteSpace(dr["USERMAILADDRESS2"])))
                                {
                                    motoJinji.Add(new EmailAddress(dr["USERMAILADDRESS2"]));
                                }
                            }
                        }
                    }

                    // 取得出来ない場合
                    if (motoJinji.Count == 0)
                    {
                        // 連絡先マスタのメール宛先を取得する
                        string[] jinjis = stakeholderRow["EMAIL"].ToString().Split(",");

                        // 連絡先マスタのメール宛先の件数分ループ
                        foreach (string jinji in jinjis)
                        {
                            // 送信宛先を連絡先マスタのメール宛先に設定する
                            var mailAddress = new EmailAddress(jinji);

                            // 送信宛先リストに追加
                            mailList.Add(mailAddress);
                        }
                    }
                    else
                    {
                        mailList.AddRange(motoJinji);
                    }
                    break;

                //（出向先）所轄人事部
                case SOUSINSAKI_TYPE_SYUKOSAKI_JINJI:

                    // （社籍・出向元）所轄人事部メールリスト
                    List<EmailAddress> sakiJinji = new List<EmailAddress>();

                    // APIパラメータ設定
                    Dictionary<string, object> jinjiParam = new Dictionary<string, object>();
                    jinjiParam.Add("KAISHACODE", unfInfo[0]["SHUKKOKBN"]);
                    jinjiParam.Add("SHIKIBETSUKBN", "63");

                    // 店舗総務の取得
                    List<Dictionary<string, string>> jinjiInfo = WF_AppForm.GetEbsDataWithApi("Get_New_MailSend_Jinji_Info", jinjiParam);

                    // 送信先リストに追加する
                    if (jinjiInfo != null && jinjiInfo.Count > 0)
                    {
                        foreach (Dictionary<string, string> dr in jinjiInfo)
                        {
                            if (!(string.IsNullOrWhiteSpace(dr["BIKO"])))
                            {
                                sakiJinji.Add(new EmailAddress(dr["BIKO"]));
                            }
                        }
                    }

                    // 取得出来ない場合
                    if (sakiJinji.Count == 0)
                    {
                        // 連絡先マスタのメール宛先を取得する
                        string[] jinjis = stakeholderRow["EMAIL"].ToString().Split(",");

                        // 連絡先マスタのメール宛先の件数分ループ
                        foreach (string jinji in jinjis)
                        {
                            // 送信宛先を連絡先マスタのメール宛先に設定する
                            var mailAddress = new EmailAddress(jinji);

                            // 送信宛先リストに追加
                            mailList.Add(mailAddress);
                        }
                    }
                    else
                    {
                        mailList.AddRange(sakiJinji);
                    }
                    break;

                // 固定の場合
                default:
                    // 連絡先マスタのメール宛先を取得する
                    string[] mails = stakeholderRow["EMAIL"].ToString().Split(",");

                    // 連絡先マスタのメール宛先の件数分ループ
                    foreach (string mail in mails)
                    {
                        // 送信宛先を連絡先マスタのメール宛先に設定する
                        var mailAddress = new EmailAddress(mail);

                        // 送信宛先リストに追加
                        mailList.Add(mailAddress);
                    }
                    break;
            }
        }

        /// <summary>
        /// メールテンプレート可変部置換メソッド
        /// </summary>
        /// <returns></returns>
        private string doTemplateRepace(string template, string emailKbn)
        {
            // 置換結果
            string result = template;

            // キーワード情報取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("select KEY_WORD_NAME AS WORD,");
            sqlSb.Append("       TABLE_NAME AS TABLENAME,");
            sqlSb.Append("	     FIELD_NAME AS FIELD,");
            sqlSb.Append("	     CONVERT_FLG AS FLG,");
            sqlSb.Append("	     CONVERT_STR AS STR");

            // メール送信キーワードマスタ
            sqlSb.Append(" from MT_MN_EMAIL_KEYWORD");

            // 条件
            sqlSb.Append(" where WF_EMAIL_KBN = @wfKbn;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフロー区分
            ps.Add("wfKbn", emailKbn);

            // 置換用キーワード情報の取得
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

            // 取得したキーワードの置換
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // 取得したテーブルとフィールドで置換後の値を取得する
                DataRow afterInfo = getKeywordFromTable(dt.Rows[i]);

                // 変換要の場合
                string after = "－";
                if (dt.Rows[i]["FLG"] != DBNull.Value && dt.Rows[i]["FLG"].ToString() == "1")
                {
                    after = afterInfo["CONVERTVALUE"].ToString();
                } 
                else if (dt.Rows[i]["FLG"] != DBNull.Value && dt.Rows[i]["FLG"].ToString() == "2")
                {
                    if (afterInfo["VALUE"] != DBNull.Value)
                    {
                        after = string.Format("{0:" + dt.Rows[i]["STR"].ToString() + "}", afterInfo["VALUE"]);
                    }
                }
                else
                {
                    if (afterInfo["VALUE"] != DBNull.Value)
                    {
                        after = afterInfo["VALUE"].ToString();
                    }
                        
                }
               
                // 置換実行
                result = result.Replace("%%" + dt.Rows[i]["WORD"].ToString() + "%%", after);
            }

            // 置換したメール内容を戻す
            return result;
        }

        /// <summary>
        /// メール送信メソッド
        /// </summary>
        /// <returns></returns>
        private async Task Execute(SendGridClient client, SendGridMessage msg)
        {
            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// メールキーワード取得用メソッド
        /// </summary>
        /// <returns></returns>
        private DataRow getKeywordFromTable(DataRow row)
        {
            // キーワード情報取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("select A." + row["FIELD"].ToString() + " AS VALUE,");
            sqlSb.Append("       B.KBNNAME AS CONVERTVALUE");

            // メール送信キーワードマスタ
            sqlSb.Append(" from " + row["TABLENAME"].ToString() + " A");

            // 区分マスタ
            sqlSb.Append("       LEFT JOIN MT_KBN B");
            sqlSb.Append("    ON B.KBNCODE = '" + row["STR"].ToString() + "' AND ");
            sqlSb.Append("       A." + row["FIELD"].ToString() + " = B.KBNVALUE");

            // 条件
            sqlSb.Append(" where OID = @oid;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフロー区分
            ps.Add("oid", this.GetRequestVal("workingId"));

            // 置換用キーワード情報の取得
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

            // 結果を戻す
            return dt.Rows[0];
        }
    }
}
