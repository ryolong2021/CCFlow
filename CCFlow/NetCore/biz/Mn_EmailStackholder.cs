using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.DA;

namespace BP.WF.HttpHandler
{
    public class Mn_EmailStackholder : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm form = new WF_AppForm();

        /// <summary>
        /// 送信先一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetEmailStakeholderList()
        {
            try
            {
                // 検索条件の取得
                EmailStokeholderReq cond =
                    JsonConvert.DeserializeObject<EmailStokeholderReq>(
                        this.GetRequestVal("EmailStokeholderReq"));

                //// Sql文と条件設定の取得
                StringBuilder sqlSb1 = new StringBuilder();
                sqlSb1.Append("    SELECT MT01.*, ");
                sqlSb1.Append("           KBN.KBNNAME AS SOUSINSAKI_TYPE_NAME, ");
                sqlSb1.Append("           KBN2.KBNNAME AS WF_NO_NAME ");
                sqlSb1.Append("      FROM MT_MN_EMAIL_STAKEHOLDER AS MT01 ");
                sqlSb1.Append(" LEFT JOIN MT_KBN AS KBN ");
                sqlSb1.Append("        ON KBN.KBNVALUE = MT01.SOUSINSAKI_TYPE ");
                sqlSb1.Append("       AND KBN.KBNCODE = 'SOUSINSAKI_TYPE' ");
                sqlSb1.Append(" LEFT JOIN MT_KBN AS KBN2 ");
                sqlSb1.Append("        ON KBN2.KBNVALUE = MT01.WF_NO ");
                sqlSb1.Append("       AND KBN2.KBNCODE = 'WF_EMAIL_KBN' ");

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append(" WHERE");

                Paras ps = new Paras();
                // グループ名
                if (!string.IsNullOrEmpty(cond.Rnrakusaki_Name))
                {
                    // 部分一致検索 グループ名
                    sqlSb.Append(" RNRAKUSAKI_NAME LIKE @RnrakusakiName AND");

                    // 入力条件
                    ps.Add("RnrakusakiName", "%" + cond.Rnrakusaki_Name + "%");
                }

                // ワークフロー
                if (!string.IsNullOrEmpty(cond.WF_NO))
                {

                    // 完全一致検索 送信先タイプ
                    sqlSb.Append(" WF_NO = @WF_NO AND");

                    // 入力条件
                    ps.Add("WF_NO", cond.WF_NO);
                }

                // 送信先タイプ
                if (!string.IsNullOrEmpty(cond.Sousinsaki_Type))
                {

                    // 完全一致検索 送信先タイプ
                    sqlSb.Append(" SOUSINSAKI_TYPE = @SousinsakiType AND");

                    // 入力条件
                    ps.Add("SousinsakiType", cond.Sousinsaki_Type);
                }

                // 会社コード
                if (!string.IsNullOrEmpty(cond.KAISHACODE))
                {

                    // 完全一致検索 会社コード
                    sqlSb.Append(" KAISHACODE = @KAISHACODE AND");

                    // 入力条件
                    ps.Add("KAISHACODE", cond.KAISHACODE);
                }

                // メールアドレス
                if (cond.EmailList != null && cond.EmailList.Count > 0)
                {
                    if (cond.EmailList.Count > 1)
                    {
                        var list = new List<string>();
                        sqlSb.Append(" (");
                        for (int i = 0; i < cond.EmailList.Count; i++)
                        {
                            // 部分一致検索 メールアドレス
                            list.Add(" EMAIL LIKE @Email" + i + " ");

                            // 入力条件
                            ps.Add("Email" + i, "%" + cond.EmailList[i] + "%");
                        }
                        sqlSb.Append(string.Join("OR", list));
                        sqlSb.Append(" ) AND");
                    }
                    else {
                        // 部分一致検索 メールアドレス
                        sqlSb.Append(" EMAIL LIKE @Email AND");

                        // 入力条件
                        ps.Add("Email", "%" + cond.EmailList[0] + "%");
                    }
                }

                String sqlOrderby = "  ORDER BY convert(int, MT01.RNRAKUSAKI_ID) DESC";
                string sql = "";
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
        /// 送信先名よりデータ件数取得
        /// </summary>
        /// <returns>データ件数</returns>
        private string GetEmailStakeholderByName(string rnrakusakiName, string wfNo, string sousinsakiType, string kaishacode, string id)
        {
            try
            {
                // Sql文と条件設定の取得
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append(" SELECT * ");
                sqlSb.Append("   FROM MT_MN_EMAIL_STAKEHOLDER ");
                sqlSb.Append("  WHERE RNRAKUSAKI_NAME = @RnrakusakiName ");
                sqlSb.Append("    AND WF_NO = @WfNo ");
                sqlSb.Append("    AND SOUSINSAKI_TYPE = @SousinsakiType ");

                Paras ps = new Paras();
                ps.Add("RnrakusakiName", rnrakusakiName);
                ps.Add("WfNo", wfNo);
                ps.Add("SousinsakiType", sousinsakiType);

                // 会社コード
                if (!string.IsNullOrEmpty(kaishacode))
                {
                    sqlSb.Append(" AND KAISHACODE = @Kaishacode");

                    // 入力条件
                    ps.Add("Kaishacode", kaishacode);
                }
                else {
                    sqlSb.Append(" AND (KAISHACODE IS NULL OR KAISHACODE = '')");
                }

                // 送信先ID（グループID）
                if (!string.IsNullOrEmpty(id))
                {
                    sqlSb.Append(" AND RNRAKUSAKI_ID <> @Rnrakusaki_Id");

                    // 入力条件
                    ps.Add("Rnrakusaki_Id", id);
                }

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

                if (dt.Rows.Count > 0)
                {
                    string[] list = { "グループ名、ワークフロー、会社（NULL可）、送信先タイプ" };
                    return "msg@" + BP.Tools.Json.ToJson(form.SetRtnMessage("E0002", list));
                }
                else {
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 送信先情報登録
        /// </summary>
        /// <returns>登録件数</returns>
        public string SaveEmailStakeholderData()
        {
            int result = -1;
            try
            {
                Dictionary<string, string> dicTbl = form.AddCommonInfo(JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal("InsertData")));
                string cnt = GetEmailStakeholderByName(dicTbl["Rnrakusaki_Name"],dicTbl["WF_NO"],dicTbl["Sousinsaki_Type"], dicTbl["KAISHACODE"], null);
                if (cnt.StartsWith("err@") || cnt.StartsWith("msg@")) {
                    return cnt;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO  MT_MN_EMAIL_STAKEHOLDER ( ");

                sb.Append("RNRAKUSAKI_ID,");
                foreach (var item in dicTbl)
                {
                    sb.Append(item.Key + ",");
                }
                sb = sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append(" ) VALUES ( ");
                sb.Append("(case when (select max(convert(int,RNRAKUSAKI_ID)) from MT_MN_EMAIL_STAKEHOLDER) is null then 1");
                sb.Append(" else (select max(convert(int,RNRAKUSAKI_ID)) from MT_MN_EMAIL_STAKEHOLDER) + 1 end ),");
                foreach (var item in dicTbl)
                {
                    sb.Append("N'" + item.Value + "',");
                }
                sb = sb.Remove(sb.ToString().LastIndexOf(','), 1);

                sb.Append(") ");
                result = BP.DA.DBAccess.RunSQL(sb.ToString());
                if (result < 1)
                {
                    return "err@" + "インサートに失敗しました。";
                }

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// 送信先情報削除
        /// </summary>
        /// <returns></returns>
        public string DeleteEmailStakeholder()
        {
            int result = -1;
            try
            {
                string sql = string.Format("DELETE FROM MT_MN_EMAIL_STAKEHOLDER WHERE RNRAKUSAKI_ID = @RnrakusakiId");
                Paras ps = new Paras();
                // 入力条件
                ps.Add("RnrakusakiId", this.GetRequestVal("KEY"));
                result = BP.DA.DBAccess.RunSQL(sql, ps);
                if (result < 1)
                {
                    return "err@" + "削除に失敗しました。";
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return result.ToString();
        }

        /// <summary>
        /// 送信先情報更新
        /// </summary>
        /// <returns></returns>
        public string UpdateEmailStakeholder()
        {
            int result = -1;
            try
            {
                // 検索条件の取得
                EmailStokeholderReq cond =
                    JsonConvert.DeserializeObject<EmailStokeholderReq>(
                        this.GetRequestVal("EmailStokeholderReq"));

                string cnt = GetEmailStakeholderByName(cond.Rnrakusaki_Name, cond.WF_NO, cond.Sousinsaki_Type, cond.KAISHACODE, cond.Rnrakusaki_Id);
                if (cnt.StartsWith("err@") || cnt.StartsWith("msg@"))
                {
                    return cnt;
                }

                string sql = "UPDATE MT_MN_EMAIL_STAKEHOLDER SET RNRAKUSAKI_NAME = @RnrakusakiName ,WF_NO = @WF_NO, KAISHACODE = @KAISHACODE, KAISHANAME = @KAISHANAME, SOUSINSAKI_TYPE = @SousinsakiType ,KAHEN_FLG = @KAHEN_FLG ,EMAIL = @Email ,REC_EDT_DATE = @REC_EDT_DATE, REC_EDT_USER_KANJI = @REC_EDT_USER_KANJI, REC_EDT_USER = @REC_EDT_USER WHERE RNRAKUSAKI_ID = @RnrakusakiId";
                Paras ps = new Paras();
                // 入力条件
                ps.Add("RnrakusakiName", cond.Rnrakusaki_Name);
                ps.Add("WF_NO", cond.WF_NO);
                ps.Add("KAISHACODE", cond.KAISHACODE);
                ps.Add("KAISHANAME", cond.KAISHANAME);
                ps.Add("SousinsakiType", cond.Sousinsaki_Type);
                ps.Add("Email", cond.Email);
                ps.Add("KAHEN_FLG", cond.KAHEN_FLG);
                ps.Add("REC_EDT_DATE", DateTime.Now.ToString());
                ps.Add("REC_EDT_USER", cond.REC_EDT_USER);
                ps.Add("REC_EDT_USER_KANJI", cond.REC_EDT_USER_KANJI);
                ps.Add("RnrakusakiId", cond.Rnrakusaki_Id);
                result = BP.DA.DBAccess.RunSQL(sql, ps);
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
        /// ワークフロー番号より会社リスト取得
        /// </summary>
        /// <returns></returns>
        public string GetCompanyList() {

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT DISTINCT MT02.CORP_CODE, MT02.CORP_NAME ");
                sb.Append("   FROM MT_BUSI_WF_REL MT01 ");
                sb.Append("  INNER JOIN MT_COMPANYACCEPTANCE MT02 ");
                sb.Append("     ON MT01.BUSINESS_CODE = MT02.BUSINESS_CODE ");
                sb.Append("  WHERE MT02.ENTRUSTED_FLG = 'Y' ");
                sb.Append("    AND MT02.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sb.Append("    AND MT02.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sb.Append("    AND (MT02.DELETE_FLG <> 'X' OR MT02.DELETE_FLG IS NULL) ");
                sb.Append("    AND MT01.WF_NO = @WF_NO");
                sb.Append("    AND MT01.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8),GETDATE(),112)) ");
                sb.Append("    AND MT01.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
                sb.Append("    AND (MT01.DELETE_FLG <> 'X' OR MT01.DELETE_FLG IS NULL) ");

                Paras ps = new Paras();
                ps.Add("WF_NO", this.GetRequestVal("WF_NO"));
                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sb.ToString(), ps);

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);

                //if (dt.Rows.Count == 0) {
                //    return "";
                //}
                //var companyCodeList = new List<string>();
                //foreach (DataRow row in dt.Rows) {
                //    companyCodeList.Add(row["CORP_CODE"].ToString());
                //}

                //Dictionary<string, object> dic = new Dictionary<string, object>();
                ////dic.Add("CompanyCodeList", companyCodeList);
                //dic.Add("CompanyCodeList", "0092,0060");
                //List<Dictionary<string, string>> ret = WF_AppForm.GetEbsDataWithApi("Get_New_Companies_CodeList", dic);

                //// フロントに戻ること
                //return JsonConvert.SerializeObject(ret);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 送信先一覧検索条件クラス
        /// </summary>
        private class EmailStokeholderReq
        {
            /// <summary>
            /// 送信先ID
            /// </summary>
            public string Rnrakusaki_Id { get; set; }

            /// <summary>
            /// 送信先名
            /// </summary>
            public string Rnrakusaki_Name { get; set; }

            /// <summary>
            /// ワークフロー番号
            /// </summary>
            public string WF_NO { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            public string KAISHACODE { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            public string KAISHANAME { get; set; }

            /// <summary>
            /// 送信先タイプ
            /// </summary>
            public string Sousinsaki_Type { get; set; }

            /// <summary>
            /// 送信先タイプ
            /// </summary>
            public string KAHEN_FLG { get; set; }

            /// <summary>
            /// メール
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// メール
            /// </summary>
            public List<string> EmailList { get; set; }

            /// <summary>
            /// 更新者ID
            /// </summary>
            public string REC_EDT_USER { get; set; }

            /// <summary>
            /// 更新者漢字名
            /// </summary>
            public string REC_EDT_USER_KANJI { get; set; }
        }
    }
}
