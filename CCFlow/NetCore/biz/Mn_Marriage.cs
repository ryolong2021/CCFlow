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
/// 結婚届申請
/// </summary>
namespace BP.WF.HttpHandler
{
    public class Mn_Marriage : BP.WF.HttpHandler.DirectoryPageBase
    {

        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// 結婚祝金マスタから、祝金を取得すること
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetAllowanceData()
        {
            try
            {
                // 検索条件の取得
                string glcClass = this.GetRequestVal("glcClass");

                // Sql文と条件設定の取得
                GetAllowanceDataSql(glcClass, out string sql, out Paras ps);

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
        private void GetAllowanceDataSql(string glcClass, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();
            // パラメータの作成
            ps = new Paras();

            // sql文の作成
            // シーケンス番号
            sqlSb.Append("SELECT Allowance.SEQNO AS SEQNO,");
            // GLC会員区分
            sqlSb.Append("       Allowance.GLC_CLASS AS GLC_CLASS,");
            // 所属コード
            sqlSb.Append("       Allowance.MARRIAGE_ALLOWANCE AS MARRIAGE_ALLOWANCE,");
            // 適用年月日_from
            sqlSb.Append("       Allowance.TEKIYOYMD_FROM AS TEKIYOYMD_FROM,");
            // 適用年月日_to
            sqlSb.Append("       Allowance.TEKIYOYMD_TO AS TEKIYOYMD_TO");
            // テーブル名
            sqlSb.Append("  FROM MT_MARRIAGE_ALLOWANCE Allowance");

            // 条件
            sqlSb.Append("    WHERE Allowance.GLC_CLASS = @GLC_CLASS ");
            sqlSb.Append("          AND Allowance.TEKIYOYMD_FROM <= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
            sqlSb.Append("          AND Allowance.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
            sqlSb.Append("          AND (Allowance.DELETE_FLG <> 'X' OR Allowance.DELETE_FLG IS NULL)");

            // パラメータの設定
            ps.Add("GLC_CLASS", glcClass);

            // sql文の設定
            sql = sqlSb.ToString();
        }
    }
}
