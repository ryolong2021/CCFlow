using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;

namespace BP.WF.HttpHandler
{
    public class Mn_EmailTemplate : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 手配業者依頼一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetEmailTemplateList()
        {
            try
            {
                // 検索条件の取得
                EmailTemplateReq cond =
                    JsonConvert.DeserializeObject<EmailTemplateReq>(
                        this.GetRequestVal("EmailTemplateReq"));

                //// Sql文と条件設定の取得
                string sql = "SELECT TEMPLATE_ID as templateId,TEMPLATE_NAME as templateName,TEMPLATE_TITLE as templateTitle,TEMPLATE_CONTENTS as templateContents,REMARKS as remarks FROM MT_MN_EMAIL_TEMPLATE";

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append(" WHERE");

                Paras ps = new Paras();
                // テンプレート名
                if (!string.IsNullOrEmpty(cond.TemplateName))
                {
                    // 部分一致検索 テンプレート名
                    sqlSb.Append(" TEMPLATE_NAME LIKE @TemplateName AND");

                    // 入力条件
                    ps.Add("TemplateName", "%" + cond.TemplateName + "%");
                }

                // テンプレートタイトル
                if (!string.IsNullOrEmpty(cond.TemplateTitle))
                {

                    // 部分一致検索 テンプレートタイトル
                    sqlSb.Append(" TEMPLATE_TITLE LIKE @TemplateTitle AND");

                    // 入力条件
                    ps.Add("TemplateTitle", "%" + cond.TemplateTitle + "%");
                }

                // テンプレートID
                if (!string.IsNullOrEmpty(cond.TemplateId))
                {
                    // 完全一致検索 テンプレートID
                    sqlSb.Append(" TEMPLATE_ID = @TemplateId AND");

                    // 入力条件
                    ps.Add("TemplateId", cond.TemplateId);
                }

                if (ps.Count > 0) {
                    sql = sql + sqlSb.ToString().Substring(0,sqlSb.ToString().LastIndexOf("AND")-1);
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
        /// 連絡先情報削除
        /// </summary>
        /// <returns></returns>
        public string DeleteTemplateData()
        {
            int result = -1;
            try
            {
                string sql = string.Format("DELETE FROM MT_MN_EMAIL_TEMPLATE WHERE TEMPLATE_ID = @TemplateId");
                Paras ps = new Paras();
                // 入力条件
                ps.Add("TemplateId", this.GetRequestVal("KEY"));
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
        /// 手配業者依頼一覧検索条件クラス
        /// </summary>
        private class EmailTemplateReq
        {
            /// <summary>
            /// 伝票番号
            /// </summary>
            public string TemplateName { get; set; }

            /// <summary>
            /// 依頼先
            /// </summary>
            public string TemplateTitle { get; set; }

            /// <summary>
            /// 申請者かな
            /// </summary>
            public string TemplateId { get; set; }
        }
    }
}
