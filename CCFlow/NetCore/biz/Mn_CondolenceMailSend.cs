using BP.DA;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BP.WF.HttpHandler
{
    public class Mn_CondolenceMailSend : BP.WF.HttpHandler.WF_AppAutoMailSend
    {
        /// <summary>
        /// トランザクション情報の取得
        /// </summary>
        /// <returns></returns>
        protected override DataTable getTransactionInfo()
        {
            // 申請者区分取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 本人社員番号
            sqlSb.Append("select UNFORTUNATE_SHAINBANGO AS UNF_SHAINBANGO,");

            // 出向元会社コード
            sqlSb.Append("       UNFORTUNATE_KAISYACODE AS KAISYACODE,");

            // 出向先会社コード
            sqlSb.Append("       UNFORTUNATE_SYUKOSAKIKAISYACODE AS SYUKOSAKIKAISYACODE,");

            // 代理者社員番号
            sqlSb.Append("       DAIRISHINSNEISYA_SHAINBANGO AS DAIRI_SHAINBANGO,");

            // 申請者区分
            sqlSb.Append("       SHINSEISYAKBN AS SHINSEISYAKBN");

            // 弔事連絡トランザクションテーブル
            sqlSb.Append(" from TT_WF_CONDOLENCE");

            // 条件
            sqlSb.Append(" where OID = @workId;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフローID
            ps.Add("workId", this.GetRequestVal("workingId"));

            // 弔事情報取得
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

            return dt;
        }

        /// <summary>
        /// 申請者区分取得メソッド
        /// </summary>
        /// <returns></returns>
        protected override string getSinseisyaKbn(DataTable transDt, List<Dictionary<string, string>> selfInfo, List<Dictionary<string, string>> agentInfo)
        {

            // 手配業者の場合
            if (this.GetRequestVal("tehaiFlg") == "1")
            {
                return SINSEISYA_KBN_TEHAI;
            }

            // 申請者区分
            string sinseisyaKbn = "";

            // 出向されない場合
            if (string.IsNullOrEmpty(selfInfo[0]["SHUKKOKBN"]) || selfInfo[0]["SHUKKOKBN"] == "0000")
            {
                // 本人の場合
                if (transDt.Rows[0]["SHINSEISYAKBN"].ToString() == SINSEISYA_KBN_HONNIN)
                {
                    sinseisyaKbn = SINSEISYA_KBN_HONNIN_PRO;
                }
                else
                {
                    sinseisyaKbn = SINSEISYA_KBN_DAIRI_PRO;
                }
            }
            else
            {
                // 本人の場合
                if (transDt.Rows[0]["SHINSEISYAKBN"].ToString() == SINSEISYA_KBN_HONNIN)
                {
                    sinseisyaKbn = SINSEISYA_KBN_HONNIN_SYUKO;
                }
                else
                {
                    sinseisyaKbn = SINSEISYA_KBN_DAIRI_SYUKO;
                }
            }
            return sinseisyaKbn;
        }

        /// <summary>
        /// キーワード置換メソッド
        /// </summary>
        /// <returns></returns>
        protected override string doTemplateRepaceCustom(string template, DataRow transRow, DataRow mailRow)
        {
            
            // 置換結果
            string result = template;

            // 受付番号の置き換え
            result = result.Replace("%%OID%%", this.GetRequestVal("workingId"));

            // 申請区分の置き換え
            // 本人の場合
            if (transRow["SHINSEISYAKBN"].ToString() == SINSEISYA_KBN_HONNIN)
            {
                // 新規の場合
                if (this.GetRequestVal("timingKbn") == "A01")
                {
                    result = result.Replace("%%APP_KBN%%", "本人申請（新規）");
                }
                else
                {
                    result = result.Replace("%%APP_KBN%%", "本人申請（修正）");
                }
            }
            else
            {
                // 新規の場合
                if (this.GetRequestVal("timingKbn") == "A01")
                {
                    result = result.Replace("%%APP_KBN%%", "代理申請（新規）");
                }
                else
                {
                    result = result.Replace("%%APP_KBN%%", "代理申請（修正）");
                }
            }

            // 置換したメール内容を戻す
            return result;
        }
    }
}
