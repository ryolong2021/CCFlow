using System;
using System.Data;
using BP.DA;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace BP.WF.HttpHandler
{
    public class Mn_Mainmenu : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 申請検査データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetFlowList()
        {
            try
            {
                // Sql文と条件設定の取得
                string sql = "SELECT NO, NAME FROM WF_FLOW WHERE NO IN (SELECT distinct * FROM string_split((SELECT string_agg(WF_KEY_VALUE,';') FROM WF_MN_KEY_REL WHERE WF_KEY_NAME LIKE @KeyWord), ';'))";

                Paras ps = new Paras();
                ps.Add("KeyWord", "%" + this.GetRequestVal("KeyWord") + "%");
                
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
        /// よくある質問一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetFQAList()
        {
            try
            {
                // 検索条件の取得
                FQAReq cond =
                    JsonConvert.DeserializeObject<FQAReq>(
                        this.GetRequestVal("FQAReq"));

                // 入力条件
                Paras ps = new Paras();

                // Sql文と条件設定の取得
                string sql = "";
                if (cond.Cnt > 0)
                {
                    sql = "SELECT t.* from (SELECT ROW_NUMBER() OVER (ORDER BY WF_COUNTS desc) as rownum, SEQNO as no, WF_KEY_QUESTIONS as keyQ, WF_KEY_ANSWER as keyA FROM MT_MN_FAQ) t WHERE t.rownum <= @Cnt";
                    ps.Add("Cnt", cond.Cnt);
                }
                else {
                    sql = "SELECT SEQNO as no, WF_KEY_QUESTIONS as keyQ, WF_KEY_ANSWER as keyA FROM MT_MN_FAQ WHERE WF_KEY_NAME LIKE @KeyName ORDER BY WF_COUNTS desc";
                    ps.Add("KeyName", "%" + cond.KeyName + "%");
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
        /// よくある質問のアクセス件数に1を加算する
        /// </summary>
        /// <returns>メッセージ情報/更新件数</returns>
        public string UpdateCnt()
        {
            int result = -1;
            try
            {
                string sql = "UPDATE MT_MN_FAQ SET WF_COUNTS = ISNULL(WF_COUNTS, 1) + 1 WHERE SEQNO = @No";
                Paras ps = new Paras();
                ps.Add("No", this.GetRequestVal("No"));
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
        /// 問い合わせデータの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetToiawase()
        {
            try
            {
                // Sql文と条件設定の取得
                string sql = "SELECT INQ_INQUIRY FROM MT_MN_INQUIRY MI " +
                    "LEFT JOIN MT_EMPLOYEE ME" +
                    " ON MI.INQ_CORPID = ME.KAISHACODE" +
                    " WHERE SHAINBANGO = @ShainBango";

                Paras ps = new Paras();
                ps.Add("ShainBango", this.GetRequestVal("ShainBango"));

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
        /// ユーザ名取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータ</returns>
        public string GetUserName()
        {
            try
            {
                // Sql文と条件設定の取得
                string sql = "SELECT SEI_KANJI,MEI_KANJI FROM MT_Employee WHERE SHAINBANGO = @ShainBango";

                Paras ps = new Paras();
                ps.Add("ShainBango", this.GetRequestVal("ShainBango"));

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
        /// ユーザ名取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータ</returns>
        public string getMenusDisplayInfo()
        {
            try
            {

                // パラメータ設定
                Dictionary<string, object> apiParam = new Dictionary<string, object>();
                apiParam.Add("ShainBango", this.GetRequestVal("ShainBango"));

                // グループコード取得
                List<Dictionary<string, string>> rel = WF_AppForm.GetEbsDataWithApi("Get_MT_Group_Menber", apiParam);

                // グループコード取得できない場合、何もせずに戻る
                if (rel.Count == 0)
                {
                    return BP.Tools.Json.ToJson(rel);
                }

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // 権限ある機能コードの取得
                // 機能別権限管理マスタ
                // 機能コード
                sqlSb.Append("SELECT FUNCTION_CODE FROM MT_FUNC_AUTH");

                // 条件
                // 削除フラグ<>'X'
                sqlSb.Append(" WHERE (DELETE_FLG <> 'X' OR DELETE_FLG IS NULL)");

                // 適用開始日≦システム日付≦適用終了日
                sqlSb.Append(" AND convert(varchar(8),getdate(),112) >= TEKIYOYMD_FROM AND convert(varchar(8),getdate(),112) <= TEKIYOYMD_TO");

                // グループコード
                sqlSb.Append(" AND GROUP_CODE in (");
                foreach(Dictionary<string, string> row in rel) 
                {
                    sqlSb.Append("'" + row["GROUP_CODE"] + "',");
                }
                sqlSb = sqlSb.Remove(sqlSb.ToString().LastIndexOf(','), 1);
                sqlSb.Append(")");

                // 権限
                sqlSb.Append(" AND AUTHORITY = 'Y'");

                // グループ化
                sqlSb.Append(" GROUP BY FUNCTION_CODE");

                // SQL実行
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
        /// よくある質問一覧検索条件クラス
        /// </summary>
        private class FQAReq
        {
            /// <summary>
            /// キーワード
            /// </summary>
            public string KeyName { get; set; }

            /// <summary>
            /// 表示件数
            /// </summary>
            public int Cnt { get; set; }
        }
    }
}
