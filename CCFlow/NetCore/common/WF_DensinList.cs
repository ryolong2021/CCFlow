using BP.DA;
using BP.Sys;
using BP.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Common.WF_OutLog;

namespace BP.WF.HttpHandler
{
    public class WF_DensinList : BP.WF.HttpHandler.DirectoryPageBase
    {
        public const string SORT_KEY = "WorkID";        　　　// ソート順
        public const string GET_MY_COMPLETE = "0";      　　　// 自分を見るの完了
        public const string GET_MY_UNCOMPLETE = "1";    　　　// 自分を見るの未完了
        public const string GET_MY_DIFFERENCE = "2";    　　　// 自分を見るの差戻
        public const string GET_MY_DRAFT = "3";         　　　// 自分を見るの下書き
        public const string GET_APPROVAL_COMPLETE = "4";      // 承認依頼を見るの完了
        public const string GET_APPROVAL_UNCOMPLETE = "5";    // 承認依頼を見るの未完了
        public const string GET_APPROVAL_INPROCESS = "6";     // 承認依頼を見るの処理待ち
        public const int WF_STATE_DRAFT = 1;                  // フロー一時保存
        public const int WF_STATE_SINSEIZUMI = 2;             // フロー承認待ち
        public const int WF_STATE_OVER = 3;                　 // フロー完了
        public const int WF_STATE_BACK = 5;                  // フロー差戻
        public const string CANCEL_MODE = "3";    // 引戻
        public const string APPROVAL_MODE = "4";  // 承認
        public const string DENIAL_MODE = "5";    // 否認
        public const string DIFFERENCE_MODE = "6";// 差戻
        public const string RIREKICOPY_MODE = "9";// 履歴コピー
        public const string CANCEL_MSG = "引戻";
        public const string APPROVAL_MSG = "承認";
        public const string DENIAL_MSG = "否認";
        public const string DIFFERENCE_MSG = "差戻";
        public const string RIREKICOPY_MSG = "履歴コピー";

        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();
        //public 

        /// <summary>
        /// 一覧データ取得
        /// </summary>
        /// <returns></returns>
        public string GetDensinList()
        {
            // 取得データ格納
            DataTable dtRtn = new DataTable(); 
            // 絞り込み条件
            string selectKey = string.Empty;
            // 取得一覧種類
            string[] listType = this.GetRequestVal("listType").Split(';');
            // 返却データ格納
            List<DataTable> listRtn = new List<DataTable>();

            for ( int i=0;i< listType.Length;i++)
            {
                switch (listType[i])
                {
                    // 自分を見るの完了一覧取得
                    case GET_MY_COMPLETE:
                        // 絞り込み条件作成
                        selectKey = string.Format(" Starter = '{0}'", WebUser.No);
                        //データ取得
                        dtRtn = GetCompleteList(selectKey, SORT_KEY);
                        break;
                    // 自分を見るの未完了
                    case GET_MY_UNCOMPLETE:
                        // 絞り込み条件作成
                        selectKey = string.Format(" Starter = '{0}'", WebUser.No);
                        //データ取得
                        dtRtn = GetUnCompleteList(selectKey, SORT_KEY);
                        break;
                    // 自分を見るの差戻
                    case GET_MY_DIFFERENCE:
                        // 絞り込み条件作成
                        selectKey = string.Format(" WFState = {0}", WF_STATE_BACK);
                        //データ取得
                        dtRtn = GetTodolist(selectKey, SORT_KEY);
                        break;
                    // 自分を見るの下書き
                    case GET_MY_DRAFT:
                        // 絞り込み条件作成
                        selectKey = string.Format(" WFState in ({0},{1}) AND Starter = '{2}'", WF_STATE_DRAFT, WF_STATE_SINSEIZUMI, WebUser.No);
                        //データ取得
                        dtRtn = GetTodolist(selectKey, SORT_KEY);
                        break;
                    // 承認依頼を見るの完了
                    case GET_APPROVAL_COMPLETE:
                        //絞り込み条件作成
                        selectKey = string.Format(" Starter <> '{0}'", WebUser.No);
                        //データ取得
                        dtRtn = GetCompleteList(selectKey, SORT_KEY);
                        break;
                    // 承認依頼を見るの未完了
                    case GET_APPROVAL_UNCOMPLETE:
                        // 絞り込み条件作成
                        selectKey = string.Format(" Starter <> '{0}'", WebUser.No);
                        //データ取得
                        dtRtn = GetUnCompleteList(selectKey, SORT_KEY);
                        break;
                    // 承認依頼を見るの処理待ち
                    case GET_APPROVAL_INPROCESS:
                        // 絞り込み条件作成
                        selectKey = string.Format(" WFState = {0} AND Starter <> '{1}'", WF_STATE_SINSEIZUMI, WebUser.No);
                        //データ取得
                        dtRtn = GetTodolist(selectKey, SORT_KEY);
                        break;
                }
                listRtn.Add(dtRtn);
            }          
            //データ返却
            return BP.Tools.Json.ToJson(listRtn);
        }

        /// <summary>
        /// ボタン押下機能
        /// </summary>
        /// <returns></returns>
        public string DoListButton()
        {
            // パラメータ取得
            string btnType = this.GetRequestVal("btnType");
            string user = this.GetRequestVal("UserNo");
            string FK_Flow = this.GetRequestVal("FK_Flow");
            string WorkID = this.GetRequestVal("WorkID");
            string TblName = this.GetRequestVal("TblName");
            BP.WF.SendReturnObjs objs = new BP.WF.SendReturnObjs();

            // 返却データ格納
            string ret = string.Empty;
            try
            {
                switch (btnType)
                {
                    // 引戻
                    case CANCEL_MODE:
                        int unSendToNode = int.Parse(this.GetRequestVal("unSendToNode"));
                        int fid = int.Parse(this.GetRequestVal("fid"));
                        //フロー引戻処理
                        BP.WF.Dev2Interface.Flow_DoUnSend(FK_Flow, long.Parse(WorkID), unSendToNode, fid);
                        //機能完了後処理
                        ret = DoOverProc(user, CANCEL_MSG, objs, TblName);
                        break;
                    // 承認
                    case APPROVAL_MODE:
                        //フロー承認処理
                        objs = BP.WF.Dev2Interface.Node_SendWork(FK_Flow, long.Parse(WorkID));
                        //機能完了後処理
                        ret = DoOverProc(user, CANCEL_MSG, objs, TblName);
                        break;
                    // 否認
                    case DENIAL_MODE:
                        //フロー否認処理
                        BP.WF.Dev2Interface.Flow_DoFlowOver(FK_Flow, long.Parse(WorkID), "あなたの申請情報は不完全なので、否認しました、このフローを終了しました。");
                        //機能完了後処理
                        ret = DoOverProc(user, CANCEL_MSG, objs, TblName, DENIAL_MODE);
                        break;
                    // 差戻
                    case DIFFERENCE_MODE:
                        string FK_Node = this.GetRequestVal("FK_Node");
                        //フロー差戻処理
                        BP.WF.Dev2Interface.Node_ReturnWork(FK_Flow, long.Parse(WorkID), 0, Convert.ToInt32(FK_Node), 0, "あなたの申請情報は不完全なので、修正して再送信してください。", false);
                        //機能完了後処理
                        ret = DoOverProc(user, CANCEL_MSG, objs, TblName, DIFFERENCE_MODE);
                        break;
                    // 履歴コピー
                    case RIREKICOPY_MODE:
                        ret = RirekiCopyProc(user,FK_Flow, WorkID);
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = "err@" + ex.Message;
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            }
            //データ返却
            return BP.Tools.Json.ToJson(ret);
        }
        /// <summary>
        /// 履歴コピー処理
        /// </summary>
        /// <returns></returns>
        private string RirekiCopyProc(string user,string fkFlow, string workID) 
        {
            //新しWorkIDを作成する
            Int64 newOid = BP.WF.Dev2Interface.Node_CreateBlankWork(fkFlow, user);
            BP.WF.Dev2Interface.Node_SetDraft(fkFlow, newOid);
            //BP.WF.Dev2Interface.Node_SendWork(commonData.FlowType, newOid, commonData.NewFK_Node, user);
            //履歴フロー内容から新フローにコピー処理
            string ret = CopyTblProc(fkFlow, newOid, workID);
            return string.IsNullOrEmpty(ret) ? newOid.ToString(): ret;
        }
        /// <summary>
        /// 履歴フロー内容から新フローにコピー処理
        /// </summary>
        /// <returns></returns>
        private string CopyTblProc(string fkFlow,Int64 NewOid, string oid)
        {
            string ret = string.Empty;
            string tblName = string.Empty;
            //トランザクション共通カラム
            string [] commonColun = { "WFState", "FK_Dept", "Title", "PWorkID", "PFlowNo"
                                     , "NEXTNODEID", "WFSta", "FlowEmps", "ShenQingRen", "ShenQingRiJi"
                                    , "ShenQingRenBuMen", "RDT", "FID", "CDT", "Rec"
                                    , "Emps", "MyNum", "PNodeID", "PrjName", "PrjNo"
                                    , "AtPara", "BillNo", "Pemp", "GUID", "FlowNote"
                                    , "FlowEnderRDT", "FlowEndNode", "FlowStartRDT", "FlowDaySpan"
                                    , "FlowStarter", "FlowEnder", "FK_NY"};
            string TBL_KEY_OID = "OID";
            try
            {
                //トランザクションテーブル名を取得
                tblName = BP.DA.DBAccess.RunSQLReturnTable(string.Format(@"SELECT PTable from {0} WHERE No='{1}'", "WF_Flow", fkFlow)).Rows[0][0].ToString();
                //更新情報格納
                Dictionary<string, string> dicRtn = new Dictionary<string, string>();
                //データ取得
                string sql = string.Format(@"SELECT * from {0} WHERE OID in ({1},{2}) ORDER BY OID DESC", tblName, oid, NewOid);

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                DataRow dr = dt.Rows[0];
                
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string key = dt.Columns[i].ColumnName.ToString();
                    string value = dr[i].ToString();

                    if (string.IsNullOrEmpty(value) && !commonColun.Contains(key))
                    {
                        dicRtn.Add(key, dt.Rows[1][i].ToString());
                    }                      
                }
                form.Update(tblName, dicRtn, TBL_KEY_OID, NewOid.ToString());
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
                return "err@" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 機能完了後処理
        /// </summary>
        /// <returns></returns>
        private string DoOverProc(string user,string msg, SendReturnObjs objs,string TblName,string btnType=null )
        {
            // 返却結果
            string infoMsg = string.Empty;
            try
            {
                //フォロー完了後処理
                wf_appfrom.FlowOverProc(objs, form, this.WorkID.ToString(), true, TblName);
                //更新者と更新時間設定
                wf_appfrom.SetUpdateUserAndDate(user, TblName, this.WorkID.ToString(), btnType, this.GetRequestVal("WF_Comment"));
                infoMsg = wf_appfrom.SetFlowInfoMsg(this.WorkID.ToString(), msg);
            }
            catch (Exception ex)
            {
                infoMsg = ex.Message;
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            }
            //返却結果
            return infoMsg;
        }

        /// <summary>
        /// 完了一覧データ取得
        /// </summary>
        /// <param name="selectKey">絞り込み条件</param>
        /// <param name="sort">ソートキー</param>
        /// <returns></returns>
        private DataTable GetCompleteList(string selectKey,string sort)
        {
            DataTable dt = new DataTable();
            try
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT F.PTable AS TblName,* FROM WF_GenerWorkFlow AS G  " +
                         "left join WF_Flow AS F on G.FK_Flow =F.No " +
                        " WHERE (Emps LIKE '%@" + WebUser.No + "@%' " +
                        " OR Emps LIKE '%@" + WebUser.No + ",%') " +
                        "and WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
                dt = SelectCommonDataTable(BP.DA.DBAccess.RunSQLReturnTable(ps), selectKey, sort);
            }
            catch ( Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            }

            return dt;
        }

        /// <summary>
        /// 未完了一覧データ取得
        /// </summary>
        /// <param name="selectKey">絞り込み条件</param>
        /// <param name="sort">ソートキー</param>
        /// <returns></returns>
        private DataTable GetUnCompleteList(string selectKey, string sort)
        {
            DataTable dt = new DataTable();
            bool isContainFuture = this.GetRequestValBoolen("IsContainFuture");
            try
            {
                dt = SelectCommonDataTable(BP.WF.Dev2Interface.DB_GenerRuning(isContainFuture), selectKey, sort);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            }
            return dt;
        }

        /// <summary>
        /// 処理中一覧データ取得
        /// </summary>
        /// <param name="selectKey">絞り込み条件</param>
        /// <param name="sort">ソートキー</param>
        /// <returns></returns>
        private DataTable GetTodolist(string selectKey, string sort)
        {
            DataTable dt = new DataTable();
            try
            {
                string fk_node = this.GetRequestVal("FK_Node");
                string showWhat = this.GetRequestVal("ShowWhat");
                dt = SelectCommonDataTable(BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, this.FK_Node, showWhat), selectKey, sort);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            }
            return dt;
        }
        /// <summary>
        /// 絞り込みによって、データを取得する
        /// </summary>
        /// <param name="dt">入力データ</param>
        /// <param name="selectKey">絞り込み条件</param>
        /// <param name="sort">ソートキー</param>
        /// <returns>取得したデータ</returns>
        public DataTable SelectCommonDataTable( DataTable dt ,string selectKey,string sort)
        {
            DataTable dtNew = new DataTable();
            List<string> btnNasiList = new List<string>();
            try
            {
                // 引戻・差戻ボタン表示フロー
                DataTable dtFlowId = BP.DA.DBAccess.RunSQLReturnTable("SELECT KBNVALUE FROM MT_KBN WHERE KBNCODE = 'BACK_BTN_NASI_FLOW'");
                foreach (DataRow row in dtFlowId.Rows)
                {
                    btnNasiList.Add(row["KBNVALUE"].ToString());
                }

                System.Data.DataRow[] drs = dt.Select(selectKey, sort);
                string searchKey = "showFlag";
                string btnNasiKey = "btnNasiFlg";
                string orderNumber = "OrderNumber";
                string summry = "summry";
                if ( null != drs)
                {
                    dtNew = drs.CopyToDataTable();
                    // データ表示要否フラグを追加
                    dtNew.Columns.Add(new DataColumn() { ColumnName = searchKey, DataType = typeof(bool) });
                    // 引戻＆差戻ボタン表示要否フラグを追加
                    dtNew.Columns.Add(new DataColumn() { ColumnName = btnNasiKey, DataType = typeof(bool) });
                    //伝票番号を追加
                    dtNew.Columns.Add(new DataColumn() { ColumnName = orderNumber, DataType = typeof(string) });
                    //サマリーを追加
                    dtNew.Columns.Add(new DataColumn() { ColumnName = summry, DataType = typeof(string) });

                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        //表示要否フラグを設定
                        dtNew.Rows[i][searchKey] = true;
                        // 引戻＆差戻ボタン表示要否フラグを設定
                        if (btnNasiList.Contains(dtNew.Rows[i]["FK_Flow"]))
                        {
                            dtNew.Rows[i][btnNasiKey] = true;
                        }
                        else {
                            dtNew.Rows[i][btnNasiKey] = false;
                        }
                        //伝票番号を設定
                        dtNew.Rows[i][orderNumber] = WF_Order_Number.GetOrderNumber(dtNew.Rows[i]["WorkID"].ToString());
                        //サマリーを追加
                        dtNew.Rows[i][summry] = GetFlowSummry(dtNew.Rows[i]["TblName"].ToString(), dtNew.Rows[i]["WorkID"].ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.DEBUG_MODE);
            }
            return dtNew;
        }
        /// <summary>
        /// 各フローサマリー項目を取得する
        /// </summary>
        /// <param name="tblName">トランザクションテーブル名</param>
        /// <param name="oid">WorkID</param>
        /// <returns>取得したデータ</returns>
        public string GetFlowSummry( string tblName,string oid) {
            string ret = string.Empty;

            DataTable dt = new DataTable();
            try
            {
                Paras ps = new Paras();
                ps.SQL = string.Format("SELECT SUMMRY FROM {0}  WHERE OID = @OID", tblName);
                ps.Add("OID", oid);
                dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
                ret = dt.Rows[0]["SUMMRY"].ToString();
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            }
            return ret;
        }
    }
}
