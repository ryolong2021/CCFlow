using BP.DA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using Common.WF_OutLog;

/// <summary>
/// GLCメンテナンスクラス
/// </summary>
namespace BP.WF.HttpHandler
{
    /// <summary>
    /// GLCメンテナンスクラス
    /// </summary>
    public class Mn_GLCMaintenanceList : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// 香料区分変更処理
        /// </summary>
        /// <returns></returns>
        public string Koryo_States_Update()
        {
            try
            {
                //string sql = string.Format(@"update TT_WF_CONDOLENCE set GLCKORYOKBN='{0}',KORYOKBN='{1}' , KOURYOU_GLC_KOUSHINNBI ='{2}' where OID ='{3}'"
                //                    , this.GetRequestVal("glckoryokbn")
                //                    , this.GetRequestVal("koryokbn")
                //                    , DateTime.Now.ToString()
                //                    , this.GetRequestVal("strOid"));
                string sql = string.Format(@"update TT_WF_CONDOLENCE set GLCKORYOKBN='{0}', KOURYOU_GLC_KOUSHINNBI ='{1}' where OID ='{2}'"
                                    , this.GetRequestVal("glckoryokbn")
                                    , DateTime.Now.ToString()
                                    , this.GetRequestVal("strOid"));

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                return dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// GLCメンテナンス依頼一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetGLCMaintencnceList()
        {
            try
            {
                // 検索条件の取得
                GlcMaintenanceReq cond =
                    JsonConvert.DeserializeObject<GlcMaintenanceReq>(
                        this.GetRequestVal("GlcMaintenanceReq"));

                // Sql文と条件設定の取得
                GetGlcMaintenanceReqListSql(cond, out string sql, out Paras ps);

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
        /// <param name="searchCond"></param>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <returns>sql文</returns>
        private void GetGlcMaintenanceReqListSql(GlcMaintenanceReq searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT Condolence.OID AS WorkID,");
            // (スナップショット)不幸従業員の会社コード
            sqlSb.Append("       Condolence.UNFORTUNATE_KAISYACODE AS CompanyCode,");
            // (スナップショット)不幸従業員の会社名
            sqlSb.Append("       Condolence.UNFORTUNATE_KAISYAMEI AS CompanyName,");
            // 申請者氏名  (スナップショット)不幸従業員の社員名(漢字)
            sqlSb.Append("       Condolence.UNFORTUNATE_KANJIMEI AS ApplicationName,");
            // 申請者カナ  (スナップショット)不幸従業員の社員名(フリガナ)
            sqlSb.Append("       Condolence.UNFORTUNATE_FURIGANAMEI AS ApplicationKana,");
            // 従業員番号
            sqlSb.Append("       Condolence.UNFORTUNATE_SHAINBANGO AS JugyoinBango,");
            // 申請日
            sqlSb.Append("       CONVERT(nvarchar(10),Condolence.KOURYOU_SHINNSEIBI,111) AS ApplicationDate,");
            // 初回申請日
            sqlSb.Append("       CONVERT(nvarchar(10),Condolence.REC_ENT_DATE,111) AS FristSiseiDate, ");
            // 最終更新日
            sqlSb.Append("       CONVERT(nvarchar(10),Condolence.KOURYOU_GLC_KOUSHINNBI,111) AS LastUpdDate, ");
            // 香料手配区分
            sqlSb.Append("       Condolence.GLCKORYOKBN AS KoryoKbn, ");
            // 申請番号
            sqlSb.Append("       orderNum.ORDER_NUMBER AS AppCode ");
            // 主テーブル
            sqlSb.Append("    FROM TT_WF_CONDOLENCE Condolence ");


            // LEFT JOIN 区分マスタ
            sqlSb.Append("    LEFT JOIN TT_WF_ORDER_NUMBER orderNum ON ");
            // LEFT JOIN 会社マスタ 条件
            sqlSb.Append("           orderNum.OID = Condolence.OID ");

            // 条件
            sqlSb.Append("    WHERE Condolence.KOURYOU_GOUKEI > 0 ");
            // デフォルト条件の追加
            // ワークフロー状態が「3:承認済み」
            sqlSb.Append("               AND Condolence.WFState = 3");

            // パラメータの作成
            ps = new Paras();

            // 画面入力により、条件を作ること
            // 申請番号
            if (!string.IsNullOrEmpty(searchCond.AppCode))
            {
                // 完全一致検索、【弔事連絡票】申請番号
                sqlSb.Append("       AND orderNum.ORDER_NUMBER = @AppCode");

                // 入力条件
                ps.Add("AppCode", searchCond.AppCode);
            }

            // 申請日 
            // Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateFrom))
            {
                // 指定された日付を含む以降を検索
                sqlSb.Append("    AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8),Condolence.KOURYOU_SHINNSEIBI,112)");

                // 入力条件
                ps.Add("AppDateFrom", searchCond.AppDateFrom);
            }

            // Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateTo))
            {
                // 指定された日付を含む以前を検索
                sqlSb.Append("    AND CONVERT(nvarchar(8),Condolence.KOURYOU_SHINNSEIBI,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");

                // 入力条件
                ps.Add("AppDateTo", searchCond.AppDateTo);
            }

            // 会社コード
            if (!string.IsNullOrEmpty(searchCond.CompanyCode))
            {
                // 完全一致検索、【弔事連絡票】(スナップショット)不幸従業員の会社コード
                sqlSb.Append("       AND Condolence.UNFORTUNATE_KAISYACODE = @CompanyCode");

                // 入力条件
                ps.Add("CompanyCode", searchCond.CompanyCode);
            }

            // 会社名
            if (!string.IsNullOrEmpty(searchCond.CompanyName))
            {
                // 部分一致検索、【弔事連絡票】(スナップショット)不幸従業員の会社名
                sqlSb.Append("       AND Condolence.UNFORTUNATE_KAISYAMEI LIKE @CompanyName");

                // 入力条件
                ps.Add("CompanyName", "%" + searchCond.CompanyName + "%");
            }

            // 申請者氏名
            if (!string.IsNullOrEmpty(searchCond.ApplicationName))
            {
                // 部分一致検索、【弔事連絡票】(スナップショット)不幸従業員の社員名(漢字)
                sqlSb.Append("       AND Condolence.UNFORTUNATE_KANJIMEI LIKE @ApplicationName");

                // 入力条件
                ps.Add("ApplicationName", "%" + searchCond.ApplicationName + "%");
            }

            // 申請者カナ
            if (!string.IsNullOrEmpty(searchCond.ApplicationKana))
            {
                // 部分一致検索、【弔事連絡票】(スナップショット)不幸従業員の社員名(フリガナ)
                sqlSb.Append("       AND Condolence.UNFORTUNATE_FURIGANAMEI LIKE @ApplicationKana");

                // 入力条件
                ps.Add("ApplicationKana", "%" + searchCond.ApplicationKana + "%");
            }

            // 従業員番号
            if (!string.IsNullOrEmpty(searchCond.JugyoinBango))
            {
                // 完全一致検索、【弔事連絡票】不幸従業員社員番号
                sqlSb.Append("       AND Condolence.UNFORTUNATE_SHAINBANGO = @JugyoinBango");

                // 入力条件
                ps.Add("JugyoinBango", searchCond.JugyoinBango);
            }

            // 香料区分
            if (searchCond.KoryoKbn != null && searchCond.KoryoKbn.Count > 0)
            {
                // 選択された処理区分を検索。複数選択時は、OR検索
                sqlSb.Append("       AND (");

                int maxCount = searchCond.KoryoKbn.Count;
                for (int i = 0; i < maxCount; i++)
                {
                    if (i > 0)
                    {
                        sqlSb.Append("            OR");
                    }
                    // 選択された香料区分を検索。複数選択時は、OR検索
                    sqlSb.Append("                      Condolence.GLCKORYOKBN = @KoryoKbn" + i);

                    // 入力条件
                    ps.Add("KoryoKbn" + i, searchCond.KoryoKbn[i]);
                }

                sqlSb.Append("       )");
            }

            // sql文の設定
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 依頼一覧検索条件クラス
        /// </summary>
        [DataContract]
        private class GlcMaintenanceReq
        {
            /// <summary>
            /// 伝票番号
            /// </summary>
            [DataMember(Name = "app_code_search")]
            public string AppCode { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            [DataMember(Name = "company_code_search")]
            public string CompanyCode { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            [DataMember(Name = "company_name_search")]
            public string CompanyName { get; set; }

            /// <summary>
            /// 申請者氏名
            /// </summary>
            [DataMember(Name = "app_emp_name_search")]
            public string ApplicationName { get; set; }

            /// <summary>
            /// 申請者かな
            /// </summary>
            [DataMember(Name = "app_emp_kana_search")]
            public string ApplicationKana { get; set; }

            /// <summary>
            /// 申請日検索条件From
            /// </summary>
            [DataMember(Name = "app_date_search_from")]
            public string AppDateFrom { get; set; }

            /// <summary>
            /// 申請日検索条件To
            /// </summary>
            [DataMember(Name = "app_date_search_to")]
            public string AppDateTo { get; set; }

            /// <summary>
            /// 従業員番号
            /// </summary>
            [DataMember(Name = "emp_code_search")]
            public string JugyoinBango { get; set; }

            /// <summary>
            /// 処理区分
            /// </summary>
            [DataMember(Name = "spice_arrange_class_search")]
            public List<int> KoryoKbn { get; set; }
        }
    }
}