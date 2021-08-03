using BP.DA;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using Common.WF_OutLog;

namespace BP.WF.HttpHandler
{
    public class Mn_AgentRequest : BP.WF.HttpHandler.DirectoryPageBase
    {
        // 弔事連絡票
        public const string FLOW_NO_009 = "009";
        // 資格免許登録
        public const string FLOW_NO_014 = "014";

        /// <summary>
        /// 代理申請一覧情報の取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetAgentReqList()
        {
            try
            {
                // 検索条件の取得
                AgentReqCond cond = JsonConvert.DeserializeObject<AgentReqCond>(this.GetRequestVal("AgentReqCond"));

                // Sql文と条件設定の取得
                GetArrangeTraderReqListSql(cond, out string sql, out Paras ps);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);


                foreach (DataRow row in dt.Rows) {

                    ChangeBizTableSql(row, cond);
                }
                // データをコミット
                dt.AcceptChanges();
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
        /// 代理申請一覧情報の変更
        /// </summary>
        private void ChangeBizTableSql(DataRow row, AgentReqCond cond) {
            int workid = (int)row["WorkID"];
            string wftype = row["WFType"].ToString();

            // ワークフロー種別により、トランザクションテーブル名を取得する
            string sqlkbn = string.Format(@"SELECT KBNNAME FROM MT_KBN WHERE KBNCODE = 'APPLICANT_FUN_TABLE' AND KBNVALUE = {0}", wftype);
            string tblname = BP.DA.DBAccess.RunSQLReturnString(sqlkbn);

            if (tblname == null) 
            {
                return;
            }

            string whereInfo = String.Empty;

            string sinseishaCodeColumn = String.Empty;
            string sinseishaNameColumn = String.Empty;

            if (FLOW_NO_009.Equals(wftype))
            {
                sinseishaCodeColumn = "UNFORTUNATE_SHAINBANGO";
                sinseishaNameColumn = "UNFORTUNATE_KANJIMEI";
            }
            else if (FLOW_NO_014.Equals(wftype))
            {
                sinseishaCodeColumn = "APPLICANT_EMP_NUM";
                sinseishaNameColumn = "(APPLICANT_LNAME + APPLICANT_FNAME) AS APPLICANT_EMP_NAME";
            }

            // 申請対象者の社員番号がある場合
            if (!string.IsNullOrEmpty(cond.EmpCode))
            {
                // 完全一致
                StringBuilder sb = new StringBuilder();
                // 代理起票者の社員番号 入力条件
                sb.Append("AND " + sinseishaCodeColumn + " = '").Append(cond.EmpCode).Append("'");
                whereInfo = sb.ToString();

            }

            // トランザクションテーブルにより、申請対象者社員番号と氏名を取得する
            string sql = string.Format(@"SELECT {3}, {4} FROM {0} WHERE SHINSEISYAKBN = 1 AND OID = {1} {2}", tblname, workid, whereInfo, sinseishaCodeColumn, sinseishaNameColumn);
            DataTable bizdt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            if (bizdt.Rows.Count == 1)
            {
                // 申請対象者社員番号を設定
                row["EmpCode"] = bizdt.Rows[0].ItemArray[0];
                // 申請対象者氏名を設定
                row["EmpName"] = bizdt.Rows[0].ItemArray[1];
            }
            else
            {
                row.Delete();
            }
        }

        /// <summary>
        /// 検索用sqlの作成
        /// </summary>
        /// <param name="searchCond"></param>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <returns>sql文</returns>
        private void GetArrangeTraderReqListSql(AgentReqCond searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT A.WorkID AS WorkID, ");
            // 代理起票者社員番号
            sqlSb.Append("       A.Starter AS AgentEmpCode, ");
            // 代理起票者氏名
            sqlSb.Append("       A.StarterName AS AgentEmpName, ");
            // ワークフロー種別
            sqlSb.Append("       A.FK_Flow AS WFType, ");
            // 申請対象者社員番号
            sqlSb.Append("       '' AS EmpCode, ");
            // 申請対象者氏名
            sqlSb.Append("       '' AS EmpName, ");
            // 作成日
            sqlSb.Append("       A.RDT AS CreateDate, ");
            // PWorkID
            sqlSb.Append("       A.PWorkID, ");
            // FK_Node
            sqlSb.Append("       A.FK_Node, ");
            // FID
            sqlSb.Append("       A.FID, ");
            // 申請番号
            sqlSb.Append("       orderNum.ORDER_NUMBER AS AppCode ");

            // sqlSb.Append("  FROM WF_EmpWorks A WHERE A.FK_Emp = @FKEmpCode AND A.WFState = 1");
            sqlSb.Append("  FROM WF_EmpWorks A ");

            // LEFT JOIN 採番マスタ
            sqlSb.Append("    LEFT JOIN TT_WF_ORDER_NUMBER orderNum ON ");
            // LEFT JOIN 会社マスタ 条件
            sqlSb.Append("           orderNum.OID = A.WorkID ");

            // where条件
            sqlSb.Append("  WHERE A.FK_Emp = @FKEmpCode AND A.WFState = 1 ");

            // パラメータの作成
            ps = new Paras();
            // 代理操作者の社員番号
            ps.Add("@FKEmpCode", this.GetRequestVal("LoginUserCode"));


            // 代理起票者の社員番号がある場合
            if (!string.IsNullOrEmpty(searchCond.AgentEmpCode))
            {
                // 完全一致
                sqlSb.Append(" AND A.Starter = @AgentEmpCode ");

                // 代理起票者の社員番号 入力条件
                ps.Add("@AgentEmpCode", searchCond.AgentEmpCode);
            }


            // ワークフロー種別がある場合
            if (!string.IsNullOrEmpty(searchCond.WFType))
            {

                sqlSb.Append(" AND A.FK_Flow IN (");
                string[] wftypes = searchCond.WFType.Split(",");

                for (int i = 0; i < wftypes.Length; i++) {
                    sqlSb.Append("'").Append(wftypes[i]).Append("'");
                    if (i < wftypes.Length - 1) {
                        sqlSb.Append(",");
                    }
                }

                sqlSb.Append(")");

            }

            // 作成日 Fromのみがある場合
            if (!string.IsNullOrEmpty(searchCond.CreateDateFrom))
            {
                // 完全一致
                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @CreateDateFrom), 112) <= CONVERT(nvarchar(8), CONVERT(datetime, A.RDT), 112)");
                // 申請日 入力条件
                ps.Add("CreateDateFrom", searchCond.CreateDateFrom);
            }
            // 作成日 Toのみがある場合
            if (!string.IsNullOrEmpty(searchCond.CreateDateTo))
            {
                // 完全一致
                sqlSb.Append("       AND CONVERT(nvarchar(8), CONVERT(datetime, A.RDT), 112) <= CONVERT(nvarchar(8),CONVERT(datetime, @CreateDateTo), 112)");
                // 申請日 入力条件
                ps.Add("CreateDateTo", searchCond.CreateDateTo);
            }

            // sql文の設定
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 代理申請一覧検索条件クラス
        /// </summary>
        [DataContract]
        private class AgentReqCond
        {
            ///// <summary>
            ///// 伝票番号
            ///// </summary>
            //public string WorkID { get; set; }

            /// <summary>
            /// 代理起票者番号　検索条件
            /// </summary>
            [DataMember(Name = "agent_emp_code_search")]
            public string AgentEmpCode { get; set; }

            ///// <summary>
            ///// 代理起票者氏名
            ///// </summary>
            //public string AgentEmpName { get; set; }

            /// <summary>
            /// ワークフロー種別　検索条件
            /// </summary>
            [DataMember(Name = "work_flow_kbn_search")]
            public string WFType { get; set; }

            /// <summary>
            /// 申請対象者番号　検索条件
            /// </summary>
            [DataMember(Name = "app_emp_code_search")]
            public string EmpCode { get; set; }

            ///// <summary>
            ///// 申請対象者氏名
            ///// </summary>
            //public string EmpName { get; set; }

            /// <summary>
            /// 作成日From　検索条件
            /// </summary>
            [DataMember(Name = "create_date_search_from")]
            public string CreateDateFrom { get; set; }

            /// <summary>
            /// 作成日To　検索条件
            /// </summary>
            [DataMember(Name = "create_date_search_to")]
            public string CreateDateTo { get; set; }

            ///// <summary>
            ///// 作成日
            ///// </summary>
            //public string CreateDate { get; set; }

        }
    }
}