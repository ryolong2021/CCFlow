
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BP.DA;
using BP.Sys;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.WF_OutLog;

namespace BP.WF.HttpHandler
{
    public class WF_AppForm : BP.WF.HttpHandler.DirectoryPageBase
    {
        public const string SUBMIT_MODE = "1";    // 提出
        public const string DRAFT_MODE = "2";     // 一時保存
        public const string CANCEL_MODE = "3";    // 取消
        public const string APPROVAL_MODE = "4";  // 承認
        public const string DENIAL_MODE = "5";    // 否認
        public const string DIFFERENCE_MODE = "6";// 差戻
        public const string EDIT_MODE = "7";　　　// 修正

        public const string TBL_KEY_OID = "OID";  // 更新テーブルキー
        public const string TBL_UPD_PATH = "TENPUSHIRYOKASYO";  // 添付ファイル格納場所
        public const string FIL_TBL_NAME = "TT_WF_REPORT_ATTACHMENT";  // 出張報告添付ファイル明細テーブル

        public const string TEHAIKBN_CANCEL = "4";    // 手配区分キャンセル

        public const string KAHEN_FLG_KOTEI = "1";    // 可変区分:固定
        public const string SINSEISYA_KBN_HONNIN = "0";  // 従業者本人
        public const string SINSEISYA_KBN_HONNIN_PRO = "AA01";  // 従業者本人（プロパー）
        public const string SINSEISYA_KBN_HONNIN_SYUKO = "AA02";  // 従業者本人（出向者）
        public const string SINSEISYA_KBN_DAIRI_PRO = "AA03";  // 代理申請者（ご不幸の従業員：プロパー）
        public const string SINSEISYA_KBN_DAIRI_SYUKO = "AA04";  // 代理申請者（ご不幸の従業員：出向者）
        public const string SINSEISYA_KBN_TEHAI = "AB01";  // ⼿配業者

        public string[] KbnCodeEntUserList = {
                                    "VISIT_KBN",//訪問先
                                    "PURPOSE_KBN",//用件
        };

        public const string MODE_KBN_YES = "Y";
        public const string SUBMIT_MSG = "提出";
        public const string DRAFT_MSG = "下書き";
        public const string CANCEL_MSG = "引戻";
        public const string APPROVAL_MSG = "承認";
        public const string DENIAL_MSG = "否認";
        public const string DIFFERENCE_MSG = "差戻";
        public const string EDIT_MSG = "修正";
        public const string DELETE_MSG = "キャンセル";


        // ▼▼▼バックからAPIにデータを取得する専用▼▼▼
        public const string API_HANDLER = "https://aeonapi.azurewebsites.net";
        public const string API_SUB_URL = "/WF/Comm/Handler.ashx?";
        // ▲▲▲バックからAPIにデータを取得する専用▲▲▲
        // 特別買物割引番号
        public string spNo = string.Empty;

        AppFormLogic form = new AppFormLogic();
        CommonData commonData = new CommonData();
        /// <summary>
        /// 画面初期化
        /// </summary>
        /// <returns></returns>
        public string AppForm_Init()
        {
            //AppFormLogic form = new AppFormLogic();
            string oid = this.GetRequestVal("WorkID");
            string userNo = this.GetRequestVal("UserNo");

            WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog("AppForm_Init Start", this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);
            Dictionary<string, Object> dicRtn = new Dictionary<string, Object>();
            DataTable dt = form.Select("select KBNCODE from MT_KBN group by (KBNCODE)");
            List<string> KbnCodeList = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    KbnCodeList.Add(dr[i].ToString());
                }
            }


            Dictionary<object, object> tblList = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("ListTblName"));
            foreach (var itemNm in tblList)
            {

                if (itemNm.Value.ToString() == "KBN")
                {
                    dicRtn.Add(itemNm.Value.ToString(), form.GetKbnInfo(KbnCodeList.ToArray(), KbnCodeEntUserList, userNo));
                }
                else
                {
                    dicRtn.Add(itemNm.Value.ToString(), form.Select(itemNm.Value.ToString(), TBL_KEY_OID, oid));
                }
            }
            WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog("AppForm_Init End", this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.DEBUG_MODE);

            return BP.Tools.Json.ToJson(dicRtn);
        }

        public string AppReportForm_Init()
        {
            string oid = this.GetRequestVal("WorkID");
            string userNo = this.GetRequestVal("UserNo");

            Dictionary<string, Object> dicRtn = new Dictionary<string, Object>();
            string[] KbnCodeList = {
                                    "WF_KBN",          //出張区分取得
                                    "WF_GOAL_KBN",     //目的
                                    "TRANSPORT_KBN",   //交通手段
                                    "START_KBN",       //出発
                                    "END_KBN",         //帰着
                                    "VISIT_KBN",       //訪問先
                                    "PURPOSE_KBN",     //用件
                                    "TICKET_LOC_KBN",  //発券場所
                                    "TICKET_TYPE_KBN",  //発券種別
                                    "COUPON_STATUS_KBN",//回数券種名
                                    "AIRLINES_KBN",     //航空会社
                                    "CLASS_KBN",        //クラス
                                    "FARE_TYPE_KBN",    //運貨種別
                                    "ROOM_TYPE_KBN",     //部屋タイプ
                                    "RIYOKOTSUKIKAN_KBN",//交通費明細
                                    "SHUKUHAKUSAKI_KBN",//宿泊先
                                    "KYORI_KBN"          //距離
                                   
            };

            //string[] KbnCodeEntUserList = {
            //                        "VISIT_KBN",//訪問先
            //                        "PURPOSE_KBN",//用件
            //};

            Dictionary<object, object> tblList = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("ListTblName"));
            foreach (var itemNm in tblList)
            {

                if (itemNm.Value.ToString() == "KBN")
                {
                    dicRtn.Add(itemNm.Value.ToString(), form.GetKbnInfo(KbnCodeList, KbnCodeEntUserList, userNo));
                }
                else
                {
                    if (itemNm.Value.ToString() == "TT_WF_VISIT")
                    {
                        if (string.IsNullOrEmpty(GetWfOid(true)))
                        {
                            oid = GetWfOid();
                        }
                        else
                        {
                            oid = this.GetRequestVal("WorkID");
                        }
                    }
                    else if (itemNm.Value.ToString() == "TT_WF_REPORT" ||
                            itemNm.Value.ToString() == "TT_WF_REPORT_TRAVELFEE" ||
                            itemNm.Value.ToString() == "TT_WF_REPORT_HOTELFEE" ||
                            itemNm.Value.ToString() == "TT_WF_REPORT_ATTACHMENT")
                    {
                        oid = this.GetRequestVal("WorkID");
                    }
                    else
                    {
                        oid = GetWfOid();
                    }
                    dicRtn.Add(itemNm.Value.ToString(), form.Select(itemNm.Value.ToString(), TBL_KEY_OID, oid));
                }

            }
            return BP.Tools.Json.ToJson(dicRtn);
        }

        public string AppForm_Init_MN()
        {
            //AppFormLogic form = new AppFormLogic();
            string oid = this.GetRequestVal("WorkID");
            string userNo = this.GetRequestVal("UserNo");

            Dictionary<string, Object> dicRtn = new Dictionary<string, Object>();
            string[] KbnCodeList = {
                                    "WF_MN_DPT_KBN",          //出張区分取得
            };

            //string[] KbnCodeEntUserList = {
            //                        "VISIT_KBN",//訪問先
            //                        "PURPOSE_KBN",//用件
            //};

            Dictionary<object, object> tblList = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("ListTblName"));
            foreach (var itemNm in tblList)
            {

                if (itemNm.Value.ToString() == "KBN")
                {
                    dicRtn.Add(itemNm.Value.ToString(), form.GetKbnInfo(KbnCodeList, KbnCodeEntUserList, userNo));
                }
                else
                {
                    dicRtn.Add(itemNm.Value.ToString(), form.Select("select * from " + itemNm.Value.ToString()));
                }
            }
            return BP.Tools.Json.ToJson(dicRtn);
        }

        public string GetWfOid(bool newFlg = false)
        {
            string wfOid = string.Empty;
            string sql = string.Format(@"SELECT WF_OID FROM TT_WF_REPORT where OID = '{0}'", this.GetRequestVal("WorkID"));
            wfOid = BP.DA.DBAccess.RunSQLReturnString(sql);
            if (newFlg)
            {
                return wfOid;
            }
            if (string.IsNullOrEmpty(wfOid))
            {
                string sqlGetWfOid = string.Format(@"SELECT Max(OID) FROM TT_WF where SHAINBANGO = '{0}'", this.GetRequestVal("UserNo"));
                wfOid = BP.DA.DBAccess.RunSQLReturnValInt(sqlGetWfOid).ToString();

            }
            return wfOid;
        }


        public string Report_Tbl_Init(Int64 NewOid, string oid)
        {
            try
            {
                // string oid = this.GetRequestVal("WorkID");
                string sql = string.Format(@"SELECT [SHAINBANGO]
                                          ,[BT_KBN]
                                          ,[GOAL_KBN]
                                          ,[TRANSPORTATION]
                                          ,[START_YMD]
                                          ,[START_KBN]
                                          ,[END_YMD]
                                          ,[END_KBN]
                                          ,[COMMENT]
                                          ,[BURDEN_DEP]
                                          FROM[dbo].[TT_WF] WHERE OID = '{0}'", oid);

                Dictionary<string, string> dicRtn = new Dictionary<string, string>();
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string key = dt.Columns[i].ColumnName.ToString();
                        string value = dr[i].ToString();
                        dicRtn.Add(key, value);
                    }
                }
                // dicRtn.Add("WF_OID", oid);
                form.Update("TT_WF_REPORT", dicRtn, TBL_KEY_OID, NewOid.ToString());

            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return "";
        }

        public string Get_Individual_Info()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("select WF.SHAINBANGO" +
                    ",Employee.SEI_KANJI" +
                    ",Employee.MEI_KANJI" +
                    ",Employee.SEI_KANA" +
                    ",Employee.MEI_KANA" +
                    ",EmployeeKbn.MEISHO_KANJI AS JUGYOINKBN" +
                    ",Companies.KAISHAMEI AS KAISHANAME" +
                    ",Employee.KAISHACODE AS KAISHACODE" +
                    ",Employee.JINJISHOZOKUCODE AS SHOZOKUCODE" +
                    ",Department.SHOZOKUMEISHORYAKU_KANJI AS SHOZOKUNAME" +
                    ",Position.SHIKAKUMEISHO_KANJI AS SHIKAKUMEISHO" +
                    ",Department.BUSHOCODE AS BUSHOCODE" +
                    ",FinancialDepartment.BUSHOMEI_ZENKAKUKANA AS BUSHOMEI_ZENKAKUKANA" +
                    ",Employee.SHIKAKUCODE AS SHIKAKUCODE" +
                    " FROM  TT_WF AS WF " +
                    " left join MT_Employee  AS Employee on WF.SHAINBANGO = Employee.SHAINBANGO " +
                    " left join MT_Department  AS Department on (Employee.JINJISHOZOKUCODE = Department.SHOZOKUCODE AND Employee.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Employee.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                    " left join MT_Position  AS Position on ( Position.KAISHACODE = Employee.KAISHACODE AND Position.SHIKAKUCODE = Employee.SHIKAKUCODE AND Position.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Position.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_Companies AS Companies ON (Companies.KAISHACODE= Employee.KAISHACODE AND Companies.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Companies.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_FinancialDepartment AS FinancialDepartment ON (FinancialDepartment.BUSHOCODE =Department.BUSHOCODE AND FinancialDepartment.KAISHACODE = Department.KAISHACODE " +
                    "       AND FinancialDepartment.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND FinancialDepartment.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_EmployeeKbn AS EmployeeKbn ON (EmployeeKbn.MEISHOCODE = Employee.JUGYOINKBN AND EmployeeKbn.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND EmployeeKbn.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " WHERE OID = '{0}'", GetWfOid());
                dic.Add("Individual_Info", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }
        public string Get_Individual_Info_Wf()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("select WF.FLOWSTARTER AS SHAINBANGO" +
                    ",Employee.SEI_KANJI" +
                    ",Employee.MEI_KANJI" +
                    ",Employee.SEI_KANA" +
                    ",Employee.MEI_KANA" +
                    ",EmployeeKbn.MEISHO_KANJI AS JUGYOINKBN" +
                    ",Companies.KAISHAMEI AS KAISHANAME" +
                    ",Employee.KAISHACODE AS KAISHACODE" +
                    ",Employee.JINJISHOZOKUCODE AS SHOZOKUCODE" +
                    ",Department.SHOZOKUMEISHORYAKU_KANJI AS SHOZOKUNAME" +
                    ",Position.SHIKAKUMEISHO_KANJI AS SHIKAKUMEISHO" +
                    ",Department.BUSHOCODE AS BUSHOCODE" +
                    ",FinancialDepartment.BUSHOMEI_ZENKAKUKANA AS BUSHOMEI_ZENKAKUKANA" +
                    ",Employee.SHIKAKUCODE AS SHIKAKUCODE" +
                    " FROM  TT_WF AS WF " +
                    " left join MT_Employee  AS Employee on WF.FLOWSTARTER = Employee.SHAINBANGO " +
                    " left join MT_Department  AS Department on (Employee.JINJISHOZOKUCODE = Department.SHOZOKUCODE AND Employee.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Employee.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                    " left join MT_Position  AS Position on ( Position.KAISHACODE = Employee.KAISHACODE AND Position.SHIKAKUCODE = Employee.SHIKAKUCODE AND Position.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Position.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_Companies AS Companies ON (Companies.KAISHACODE= Employee.KAISHACODE AND Companies.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Companies.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_FinancialDepartment AS FinancialDepartment ON (FinancialDepartment.BUSHOCODE =Department.BUSHOCODE AND FinancialDepartment.KAISHACODE = Department.KAISHACODE " +
                    "       AND FinancialDepartment.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND FinancialDepartment.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_EmployeeKbn AS EmployeeKbn ON (EmployeeKbn.MEISHOCODE = Employee.JUGYOINKBN AND EmployeeKbn.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND EmployeeKbn.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " WHERE OID = '{0}'", this.GetRequestVal("WorkID"));
                dic.Add("Individual_Info", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        public string Get_Shain_Info()
        {

            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string shainbango = this.GetRequestVal("shainbango");
                if (string.IsNullOrEmpty(shainbango))
                {
                    string sql = string.Format("select FlowStarter from {0} where OID = '{1}'",
                                               this.GetRequestVal("tblName"), this.GetRequestVal("WorkID"));
                    shainbango = form.Select(sql).Rows[0][0].ToString();
                }

                //パラメタ格納
                Dictionary<string, Object> dicret = new Dictionary<string, Object>();
                dicret.Add("SHAINBANGO", shainbango);
                //APIから返却結果を格納
                List<Dictionary<string, string>> listDic = new List<Dictionary<string, string>>();
                //APIでEBSからデータを取得
                listDic = GetEbsDataWithApi("Get_Shain_Info", dicret);
                //結果返却
                dic.Add("Get_Shain_Info", listDic);

                if (listDic.Count == 0)
                {
                    string[] list = { shainbango };
                    return "msg@" + BP.Tools.Json.ToJson(this.SetRtnMessage("I0001", list));
                }
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        public string Get_Kingaku_Info()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("select PayByPosition.KINGAKU" +
                    " FROM MT_PayByPosition AS PayByPosition " +
                    " left join MT_Employee AS Employee ON (PayByPosition.KAISHACODE = Employee.KAISHACODE " +
                    " AND  PayByPosition.SHIKAKUCODE = Employee.SHIKAKUCODE " +
                    " AND PayByPosition.SHUBETSUCODE IN ('01','02') " +
                    " AND PayByPosition.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND PayByPosition.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                    " left join TT_WF AS WF  ON WF.SHAINBANGO = Employee.SHAINBANGO " +
                    " WHERE OID = '{0}'  order by PayByPosition.KINGAKU", GetWfOid());
                dic.Add("Kingaku_Info", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// フォーム送信
        /// </summary>
        /// <returns></returns>
        public string Runing_Send()
        {
            try
            {
                commonData = GetCommonData();

                BP.WF.SendReturnObjs objs = new BP.WF.SendReturnObjs();
                string infoMsg = string.Empty;

                int toNodeID = 0;
                if (!string.IsNullOrEmpty(commonData.toNodeID))
                {
                    toNodeID = int.Parse(commonData.toNodeID);
                }
                switch (commonData.Mode)
                {
                    case SUBMIT_MODE: //提出
                        //業務伝票番号採番
                        //既存があれば、再採番しない（差戻後再提出）
                        String strOrderNumber = WF_Order_Number.GetOrderNumber(commonData.WorkID);
                        if (String.IsNullOrEmpty(strOrderNumber) == true)
                        {
                            //採番を行う
                            String strCurrDate = DateTime.Now.ToString("yyyyMMdd");
                            String strUserId = this.GetRequestVal("UserNo");
                            strOrderNumber = WF_Order_Number.CreateOrderNumber(commonData.WorkID, commonData.FK_Flow, strCurrDate, strUserId);
                            if (String.IsNullOrEmpty(strOrderNumber))
                            {
                                return "err@伝票番号作成できません。";
                            }
                        }

                        if (commonData.FK_Node != commonData.NextNode)
                        {
                            SetMainFromData();
                            SetSubFromData();

                        }
                        //传入流程编号, WorkID执行发送. 
                        if (string.IsNullOrEmpty(commonData.toEmps))
                        {
                            objs = BP.WF.Dev2Interface.Node_SendWork(commonData.FK_Flow, long.Parse(commonData.WorkID));

                        }
                        else {
                            objs = BP.WF.Dev2Interface.Node_SendWork(commonData.FK_Flow, long.Parse(commonData.WorkID), toNodeID, commonData.toEmps);
                        }
                        //フォロー完了後処理
                        FlowOverProc(objs, form, commonData.WorkID);

                        if (commonData.AutoApprovalMode == MODE_KBN_YES)
                        {
                            //自動承認処理
                            SetCondolenceFlowOverProc(objs);
                        }
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, SUBMIT_MSG);

                        break;
                    case DRAFT_MODE: //一時保存
                        if (commonData.FK_Node != commonData.NextNode)
                        {
                            SetMainFromData();
                            SetSubFromData();
                            // 一時保存APIを呼び出す
                            BP.WF.Dev2Interface.Node_SetDraft(commonData.FK_Flow, long.Parse(commonData.WorkID));
                            // AddTodolist(commonData);
                        }
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, DRAFT_MSG);
                        break;
                    case CANCEL_MODE: //引戻
                        BP.WF.Dev2Interface.Flow_DoUnSend(commonData.FK_Flow, long.Parse(commonData.WorkID));
                        //フォロー完了後処理
                        FlowOverProc(objs, form, commonData.WorkID, true);
                        //更新者と更新時間設定
                        SetUpdateUserAndDate(this.GetRequestVal("UserNo"));
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, CANCEL_MSG);
                        break;
                    case APPROVAL_MODE: //承認
                        if (string.IsNullOrEmpty(commonData.toEmps))
                        {
                            objs = BP.WF.Dev2Interface.Node_SendWork(commonData.FK_Flow, long.Parse(commonData.WorkID));
                        }
                        else
                        {
                            // 承認する場合、テーブルの更新を設定する
                            if (commonData.ApprovalUpdataMode == MODE_KBN_YES) {
                                SetMainFromData();
                                SetSubFromData();
                            }
                            objs = BP.WF.Dev2Interface.Node_SendWork(commonData.FK_Flow, long.Parse(commonData.WorkID), toNodeID, commonData.toEmps);
                        }
                        if (commonData.NewFlowMode == MODE_KBN_YES)
                        {

                            string ret = this.PrintPDF();
                            if (ret.Contains("err")) {
                                return ret;
                            }

                            Int64 newOid = BP.WF.Dev2Interface.Node_CreateBlankWork(commonData.FlowType, commonData.Starter);
                            SetMainFromNewFlowData(newOid, spNo);
                            BP.WF.Dev2Interface.Node_SendWork(commonData.FlowType, newOid, commonData.NewFK_Node, commonData.Starter);
                        }
                        //フォロー完了後処理
                        FlowOverProc(objs, form, commonData.WorkID);
                        //更新者と更新時間設定
                        SetUpdateUserAndDate(this.GetRequestVal("UserNo"));
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, APPROVAL_MSG);
                        break;
                    case DENIAL_MODE: //否認
                        //手動承認の場合
                        if (commonData.AutoApprovalMode != MODE_KBN_YES)
                        {
                            SetMainFromData();
                        }
                        BP.WF.Dev2Interface.Flow_DoFlowOver(commonData.FK_Flow, long.Parse(commonData.WorkID), "あなたの申請情報は不完全なので、否認しました、このフローを終了しました。");
                        //フォロー完了後処理
                        FlowOverProc(objs, form, commonData.WorkID, true);
                        //更新者と更新時間設定
                        SetUpdateUserAndDate(this.GetRequestVal("UserNo"));
                        // フォロー返却メッセージ設定
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, DENIAL_MSG);
                        break;
                    case DIFFERENCE_MODE: //差戻
                        //手動承認の場合
                        if (commonData.AutoApprovalMode != MODE_KBN_YES)
                        {
                            SetMainFromData();
                        }
                        BP.WF.Dev2Interface.Node_ReturnWork(commonData.FK_Flow, long.Parse(commonData.WorkID), 0, Convert.ToInt32(commonData.FK_Node), 0, "あなたの申請情報は不完全なので、修正して再送信してください。", false);
                        //フォロー完了後処理
                        FlowOverProc(objs, form, commonData.WorkID, true);
                        //更新者と更新時間設定
                        SetUpdateUserAndDate(this.GetRequestVal("UserNo"));
                        // フォロー返却メッセージ設定
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, DIFFERENCE_MSG);
                        break;
                    case EDIT_MODE: //申請修正
                        if (commonData.FK_Node == commonData.NextNode)
                        {
                            SetMainFromData();
                            SetSubFromData();

                        }
                        // フォロー返却メッセージ設定
                        infoMsg = SetFlowInfoMsg(commonData.WorkID, EDIT_MSG);
                        break;

                }
                return infoMsg;
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 業務伝票番号取得
        /// </summary>
        /// <returns>
        /// OIDに繋がり業務伝票番号。なければ、空
        /// ※JSON形式
        /// </returns>
        public String GetOrderNumber()
        {
            //結果格納定義
            Dictionary<String, String> dic = new Dictionary<String, String>();
            //OID取得
            String strOid = this.GetRequestVal("WorkID");

            //業務伝票番号取得
            try
            {
                dic.Add("GetOrderNumber", WF_Order_Number.GetOrderNumber(strOid));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }

            return BP.Tools.Json.ToJson(dic);
        }

        public string GetMailAddress(string shainbango)
        {
            // パラメータ設定
            Dictionary<string, object> apiParam = new Dictionary<string, object>();
            apiParam.Add("SHAINBANGO", shainbango);

            // 従業員情報を取得
            List<Dictionary<string, string>> rel = WF_AppForm.GetEbsDataWithApi("Get_MT_Employee", apiParam);

            if (rel.Count == 0)
            {
                return BP.Tools.Json.ToJson(rel);
            }

            string address = string.Empty;
            foreach (Dictionary<string, string> row in rel)
            {
                address = row["USERMAILADDRESS1"];
            }

            return address;
        }

        /// 更新者と更新時間設定
        /// </summary>
        /// <returns></returns>
        public void SetUpdateUserAndDate(string updatUser, string tblName = null, string WorkId = null, string btnType = null, string wfcomment = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!dic.ContainsKey("REC_EDT_DATE"))
            {
                dic.Add("REC_EDT_DATE", DateTime.Now.ToString());
            }
            if (!dic.ContainsKey("REC_EDT_USER"))
            {
                dic.Add("REC_EDT_USER", updatUser);
            }
            // 否認・差戻の場合
            if (!string.IsNullOrEmpty(btnType))
            {
                dic.Add("WFComment", wfcomment);
            }
            string MainTblName = string.IsNullOrEmpty(tblName) ? this.GetRequestVal("MainTblName") : tblName;
            string OID = string.IsNullOrEmpty(WorkId) ? commonData.WorkID : WorkId;
            form.Update(MainTblName, dic, TBL_KEY_OID, OID);
        }

        public string SetFlowInfoMsg(string workID, string context) {

            string strOrderNumber = WF_Order_Number.GetOrderNumber(commonData.WorkID);

            string msg = "正常に" + context + "されました。";

            if (!String.IsNullOrEmpty(strOrderNumber)) {
                msg += "<br/>申請番号：" + strOrderNumber;

            }
            return msg;
        }

        /// 代理操作者を追加
        /// </summary>
        public void AddTodolist(CommonData commonData)
        {

            if (commonData.AgentMode == "1")
            {
                // 代理申請者の承認組織区分、グループコードの取得SQL
                //パラメタ格納
                Dictionary<string, Object> dicret0 = new Dictionary<string, Object>
                {
                    { "SHAINBANGO", commonData.UserNo }
                };
                //APIから返却結果を格納
                List<Dictionary<string, string>> listDic0 = new List<Dictionary<string, string>>();
                //APIでEBSからデータを取得
                listDic0 = WF_AppForm.GetEbsDataWithApi("Get_New_Agent_Group_Info", dicret0);

                string groupcd = listDic0[0]["GROUP_CODE"];
                string groupkbn = listDic0[0]["GROUP_KBN"];

                // グループコードで同じ従業員の取得SQL
                //パラメタ格納
                Dictionary<string, Object> dicret1 = new Dictionary<string, Object>
                {
                    { "GROUPCODE", groupcd }
                };
                //APIから返却結果を格納
                List<Dictionary<string, string>> listDic1 = new List<Dictionary<string, string>>();
                //APIでEBSからデータを取得
                listDic1 = WF_AppForm.GetEbsDataWithApi("Get_New_Agent_Employee_Info", dicret1);

                StringBuilder empcodelist = new StringBuilder();
                foreach (Dictionary<string, string> dic1 in listDic1)
                {
                    empcodelist.Append("'");
                    empcodelist.Append(dic1["EMPLOYEE_CODE"]);
                    empcodelist.Append("',");
                    
                }

                if(empcodelist.Length > 0)
                {
                    empcodelist.Remove(empcodelist.Length - 1, 1);
                }

                // G0001:店舗総務の場合、
                if (groupkbn == "G0001")
                {
                    // 従業員マスタに申請者と同じ人事所属コードを持っている従業員を取得
                    //パラメタ格納
                    Dictionary<string, Object> dicret2 = new Dictionary<string, Object>
                    {
                        { "SHAINBANGO", commonData.UserNo },
                        { "GROUPCODE", groupcd }
                    };
                    //APIから返却結果を格納
                    List<Dictionary<string, string>> listDic2 = new List<Dictionary<string, string>>();
                    //APIでEBSからデータを取得
                    listDic2 = WF_AppForm.GetEbsDataWithApi("Get_New_Agent_Store_Employee_Info", dicret2);

                    empcodelist = new StringBuilder();
                    foreach (Dictionary<string, string> dic2 in listDic2)
                    {
                        empcodelist.Append("'");
                        empcodelist.Append(dic2["EMPLOYEE_CODE"]);
                        empcodelist.Append("',");

                    }
                    if (empcodelist.Length > 0)
                    {
                        empcodelist.Remove(empcodelist.Length - 1, 1);
                    }
                }

                string addcode = empcodelist.ToString().Replace("'","");

                if (string.IsNullOrEmpty(addcode))
                {
                    addcode = commonData.UserNo;
                }
                // 代理申請者を追加
                BP.WF.Dev2Interface.Node_AddTodolist(long.Parse(commonData.WorkID), addcode);
            }

        }

        /// 自動承認処理
        /// </summary>
        /// <returns></returns>
        public string SetCondolenceFlowOverProc(BP.WF.SendReturnObjs objs)
        {
            string ret = string.Empty;
            BP.WF.SendReturnObjs objsNext;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                string toEmpID = objs.VarAcceptersID.ToString();
                //自動承認者情報取得
                sql = string.Format(@"
                                    select Emp.No ,Emp.Name ,Emp.FK_Dept,Dept.Name AS DeptName from Port_Emp AS Emp 
                                    left join Port_Dept AS Dept on  Emp.FK_Dept = Dept.No where Emp.No = '{0}'"
                        , toEmpID);
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                objsNext = BP.WF.Dev2Interface.Node_SendWork(commonData.FK_Flow, long.Parse(commonData.WorkID), null, null, 0, toEmpID,
                    dt.Rows[0]["No"].ToString(), dt.Rows[0]["Name"].ToString(), dt.Rows[0]["FK_Dept"].ToString(), dt.Rows[0]["DeptName"].ToString(), null);
                //フォロー完了後処理
                ret =  FlowOverProc(objsNext, form, commonData.WorkID);
                // 更新者と更新時間設定
                SetUpdateUserAndDate(dt.Rows[0]["No"].ToString());

            }
            catch ( Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return ex.Message;
            }

            return ret;
        }

        public string PrintPDF()
        {
            string ret = string.Empty;
            // 特別買物割引制度申請
            if (commonData.FK_Flow == "015")
            {
                Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal("TT_WF_SPEC_COUPON_APPLY"));
                // 特別買物割引証番号
                string specialCouponNo = BP.WF.HttpHandler.Mn_SpecialCouponNo.SaveSpecialCouponNo(commonData.WorkID);
                // バーコードを生成
                BP.WF.HttpHandler.Mn_SpecialCouponNo.GetBarcode(specialCouponNo);
                // 人事部長氏名
                string rolePersonName = BP.WF.HttpHandler.Mn_SpecialCouponNo.GetDirectorName(commonData.Starter);
                // 購買店舗注意文
                string buyStoreWarning = BP.WF.HttpHandler.Mn_SpecialCouponNo.GetBuyStoreWarning(commonData.Starter);
                // mail From
                string mailForm = GetMailAddress(commonData.UserNo);
                // mail To
                string mailTo = GetMailAddress(commonData.Starter);
                dicTbl.Add("SPEC_DISCOUNT_COUPON_NO", specialCouponNo);
                dicTbl.Add("ROLE_PERSON_NAME", rolePersonName);
                dicTbl.Add("BUY_STORE_WARNING", buyStoreWarning);
                dicTbl.Add("MAIL_FROM", mailForm);
                dicTbl.Add("MAIL_TO", mailTo);
                // 
                spNo = specialCouponNo;
                // 特別買物割引制度申請PDFを出力
                ret = Pdf_SpecialCouponCertificate.Print(this.GetRequestVal("TT_WF_SPEC_COUPON_APPLY"), dicTbl);
            }

            return ret;
        }

        public void SetMainFromNewFlowData(Int64 workID, string specialCouponNo=null)
        {

            string tblName = this.GetRequestVal("NewMainTblName");
            Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal(tblName));
            if (!String.IsNullOrEmpty(specialCouponNo))
            {
                dicTbl.Add("SPEC_DISCOUNT_COUPON_NO", specialCouponNo);
            }
            // アップロード用メソッド
            //TaskDoAzureStorageUpload(dicTbl).Wait();
            form.Update(tblName, AddCommonInfo(dicTbl), TBL_KEY_OID, workID.ToString());
        }

        public void SetMainFromData()
        {
            string tblName = this.GetRequestVal("MainTblName");
            Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal(tblName));

            // アップロード用メソッド
            if (commonData.dispalyFlag != "Y") {
                TaskDoAzureStorageUpload(dicTbl).Wait();
            }
            

            if (commonData.newflag == "1")
            {
                form.Insert(tblName, AddCCFlowCommonInfo(dicTbl, tblName));
            }
            else
            {
                form.Update(tblName, AddCCFlowCommonInfo(dicTbl, tblName), TBL_KEY_OID, commonData.WorkID);
            }
        }

        public void SetSubFromData()
        {

            Dictionary<object, object> tblList = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("SubListTblName"));
            foreach (var itemNm in tblList)
            {
                Dictionary<object, object> dicTbl = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal(itemNm.Value.ToString()));
                //if (dicTbl.Count > 0)
                //{
                form.Delete(itemNm.Value.ToString(), TBL_KEY_OID, commonData.WorkID);
                //}
                int cnt = 0;
                for (int i = 0; i < dicTbl.Count; i++)
                {
                    Dictionary<string, string> insertInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(dicTbl[i.ToString()].ToString());

                    // アップロード用メソッド
                    if (commonData.dispalyFlag == "Y" && (!insertInfo.ContainsKey("UPLOADE_FILE_1")
                            || (insertInfo.ContainsKey("UPLOADE_FILE_1") && "".Equals(insertInfo["UPLOADE_FILE_1"]))))
                    {
                        TaskDoAzureStorageUploadForDetail(insertInfo, cnt).Wait();
                        cnt++;
                    }

                    form.Insert(itemNm.Value.ToString(), AddCommonInfo(insertInfo));
                }
            }
        }
        /// <summary>
        /// 区分コード通りに、区分マスタデータを取得
        /// </summary>
        /// <returns></returns>
        public string Get_Kbn_Dat()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string strKbn = string.Empty;
                Dictionary<object, object> kbnList = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("KbnCodeList"));
                foreach (var kbnNm in kbnList)
                {
                    strKbn += "'" + kbnNm.Value.ToString() +"',";
                }
                strKbn = strKbn.Remove(strKbn.ToString().LastIndexOf(','), 1);
                string sql = string.Format("SELECT KBNCODE,KBNVALUE,KBNNAME FROM MT_KBN WHERE IN '{0}' ORDER BY KBNORDER", strKbn);
                dic.Add("KbnCodeList", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }
        /// <summary>
        /// フローナンバー通りに、テーブル名前を取得
        /// </summary>
        /// <returns></returns>
        public string Get_Flow_TableName(string flowNo)
        {
            string ret = string.Empty;
            try
            {
                string sql = string.Format("SELECT PTable FROM WF_Flow WHERE No = '{0}'", flowNo);
                ret =  BP.DA.DBAccess.RunSQLReturnTable(sql).Rows[0]["WF_KEY_VALUE"].ToString();
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 会社コードリストデータを取得
        /// </summary>
        /// <returns></returns>
        public string Get_Kaisha_Code_List()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = "SELECT KAISHACODE,KAISHAMEI FROM MT_Companies WHERE TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND TEKIYOYMD_TO >= (select CONVERT(nvarchar(12), GETDATE(), 112)) group by KAISHACODE, KAISHAMEI";
                dic.Add("Get_Kaisha_Code_List", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        ///受託フラグを取得
        /// </summary>
        /// <returns></returns>
        public string Get_EntrustedByLoginUser()
        {

            string sql = string.Format(@"SELECT * FROM MT_COMPANYACCEPTANCE AS C 
												   INNER JOIN MT_BUSI_WF_REL AS B
                                                           ON B.BUSINESS_CODE = C.BUSINESS_CODE 
                                                        WHERE C.CORP_CODE = '{0}' 
                                                          AND B.WF_NO = '{1}'", this.GetRequestVal("kaishacode"), this.GetRequestVal("wf_no"));

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            if (dt.Rows.Count > 0) 
            {
                return dt.Rows[0]["ENTRUSTED_FLG"].ToString();
            }
            else
            {
                return String.Empty;
            }
            
        }

        /// <summary>
        ///フローシステム情報を取得
        /// </summary>
        /// <returns></returns>
        public string GetFlowSystemData()
        {
            string WorkId = this.GetRequestVal("WorkId");
            DataTable dt = new DataTable();
            try
            {
                string sql = string.Format("select * From WF_GenerWorkFlow  where WorkId = '{0}' ", WorkId);
                dt= BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        ///所属業務部を取得
        /// </summary>
        /// <returns></returns>
        public string Get_Business_List()
        {
            try
            {
                Dictionary<string, Object> dic = new Dictionary<string, Object>();
                // ワークフロー番号
                string wfno = this.GetRequestVal("wf_no");
                string sql = null;
                string ENTRUSTED_FLG = Get_EntrustedByLoginUser();

                if (ENTRUSTED_FLG == "Y")
                {// BS業務部

                    sql = string.Format(@"SELECT A.CORP_CODE AS KAISHACODE, A.CORP_NAME AS KAISHAMEI FROM MT_COMPANYACCEPTANCE A
                                                                 INNER JOIN MT_BUSI_WF_REL B
                                                                    ON B.BUSINESS_CODE = A.BUSINESS_CODE 
                                                                 WHERE A.ENTRUSTED_FLG = '{0}' 
                                                                   AND B.WF_NO = '{1}' 
                                                                   AND (B.DELETE_FLG <> 'X' OR B.DELETE_FLG IS NULL) 
                                                                   AND (A.DELETE_FLG <> 'X' OR A.DELETE_FLG IS NULL) 
                                                                   AND A.TEKIYOYMD_FROM <= (SELECT CONVERT (NVARCHAR(12),GETDATE(),112)) 
                                                                   AND A.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(12), GETDATE(), 112))
                                                                   AND B.TEKIYOYMD_FROM <= (SELECT CONVERT (NVARCHAR(12),GETDATE(),112)) 
                                                                   AND B.TEKIYOYMD_TO >= (SELECT CONVERT(NVARCHAR(12), GETDATE(), 112))
                                                              ORDER BY A.CORP_CODE", ENTRUSTED_FLG, wfno);

                    dic.Add("Get_Business_List", BP.DA.DBAccess.RunSQLReturnTable(sql));

                }
                else
                {// 人事部
                    // パラメータ設定
                    Dictionary<string, object> apiParam = new Dictionary<string, object>();
                    apiParam.Add("ShainBango", this.GetRequestVal("shainbango"));

                    // グループコード取得
                    List<Dictionary<string, string>> rel = WF_AppForm.GetEbsDataWithApi("Get_KaishaCode", apiParam);
                    dic.Add("Get_Business_List", rel);
                }
                return BP.Tools.Json.ToJson(dic);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }

        }

        public Dictionary<string, string> AddCommonInfo(Dictionary<string, string> dic ,bool updateflg =true)
        {
            if (updateflg)
            {
                if (!dic.ContainsKey("REC_ENT_DATE"))
                {
                    dic.Add("REC_ENT_DATE", DateTime.Now.ToString());
                }
                if (!dic.ContainsKey("REC_ENT_USER"))
                {
                    dic.Add("REC_ENT_USER", this.GetRequestVal("UserNo"));
                }
            }
            
            if (!dic.ContainsKey("REC_EDT_DATE"))
            {
                dic.Add("REC_EDT_DATE", DateTime.Now.ToString());
            }
            if (!dic.ContainsKey("REC_EDT_USER"))
            {
                dic.Add("REC_EDT_USER", this.GetRequestVal("UserNo"));
            }
            return dic;
        }
        public Dictionary<string, string> AddCCFlowCommonInfo(Dictionary<string, string> dic, string tblName)
        {
            string sql = string.Format("select REC_ENT_USER FROM {0} WHERE OID = {1}", tblName, this.GetRequestVal("WorkID"));
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (string.IsNullOrEmpty(dt.Rows[0]["REC_ENT_USER"].ToString()))
            {
                if (!dic.ContainsKey("REC_ENT_DATE"))
                {
                    dic.Add("REC_ENT_DATE", DateTime.Now.ToString());
                }
                if (!dic.ContainsKey("REC_ENT_USER"))
                {
                    dic.Add("REC_ENT_USER", this.GetRequestVal("UserNo"));
                }
            }
            if (!dic.ContainsKey("REC_EDT_DATE"))
            {
                dic.Add("REC_EDT_DATE", DateTime.Now.ToString());
            }
            if (!dic.ContainsKey("REC_EDT_USER"))
            {
                dic.Add("REC_EDT_USER", this.GetRequestVal("UserNo"));
            }
            return dic;
        }

        public CommonData GetCommonData()
        {
            return JsonConvert.DeserializeObject<CommonData>(this.GetRequestVal("CommonData"));
        }
        public string FlowOverProc(BP.WF.SendReturnObjs objs, AppFormLogic form, string oid, bool backFlg = false,string tblName=null)
        {
            Dictionary<string, string> dictnode = new Dictionary<string, string>();
            string infoMsg = string.Empty;
            string MainTblName = string.IsNullOrEmpty(tblName) ? this.GetRequestVal("MainTblName") : tblName;

            if (backFlg)
            {
                dictnode.Add("nextNodeId", string.Empty);
                form.Update(MainTblName, dictnode, TBL_KEY_OID, oid);
            }
            else
            {
                // 检查流程是否结束？ 
                bool isFlowOver = objs.IsStopFlow;

                // 获得发送到那个节点上去了？ 
                int toNodeID = objs.VarToNodeID;
                string toNodeName = objs.VarToNodeName;

                // 获得发送给谁了？ 注意：这里如果是多个接受人员就会使用逗号分开。 
                string toEmpID = objs.VarAcceptersID;
                string toEmpName = objs.VarAcceptersName;

                // 输出提示信息, 这个信息可以提示给操作员. 
                infoMsg = objs.ToMsgOfHtml();
                if (!isFlowOver)
                {
                    dictnode.Add("nextNodeId", toNodeID.ToString());
                    form.Update(MainTblName, dictnode, TBL_KEY_OID, oid);
                }
            }
            return infoMsg;
        }


        public string Get_Costburdendev_Code()
        {

            string shainbango = this.GetRequestVal("shainbango");
            string userNo = this.GetRequestVal("UserNo");
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("select Department.BUSHOCODE " +
                    " FROM MT_Employee AS Employee " +
                    " left join MT_Department AS Department ON Department.SHOZOKUCODE = Employee.JINJISHOZOKUCODE " +
                    " AND (Department.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Department.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                    " where SHAINBANGO = '{0}' ", userNo);
                dic.Add("Costburdendev_Code", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }
        public string Runing_KunDelete()
        {
            try
            {
                string kubname = this.GetRequestVal("KunName");
                string visitDelInfo = this.GetRequestVal("visitDelInfo");
                string userNo = this.GetRequestVal("UserNo");

                AppFormLogic form = new AppFormLogic();
                Dictionary<string, string> dic_visitDelInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(visitDelInfo);
                foreach (var item in dic_visitDelInfo)
                {
                    string sql = string.Format("WHERE KBNCODE='{0}' AND KBNVALUE='{1}' AND REC_ENT_USER='{2}'",
                                            kubname, item.Value.ToString(), userNo);
                    form.DeleteWhere("MT_KBN", sql);
                }

                return "success";
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        public string Runing_Delete()
        {
            string result = string.Empty;
            try
            {
                string TblName = this.GetRequestVal("TblName");
                string DeleteInfo = this.GetRequestVal("DeleteInfo");
                string DeleteKey = this.GetRequestVal("DeleteKey");

                Dictionary<string, string> dic_DeleteInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(DeleteInfo);
                foreach (var item in dic_DeleteInfo)
                {
                    string sql = string.Format("WHERE {0}='{1}' AND REC_ENT_USER='{2}'",
                                            DeleteKey, item.Value.ToString(), this.GetRequestVal("userNo"));
                    result = form.DeleteWhere(TblName, sql).ToString();
                }
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return result;

        }

        public string Runing_Delete_Sql()
        {
            string result = string.Empty;
            try
            {
                string strTblName = this.GetRequestVal("TblName");
                string sqlWhere = this.GetRequestVal("sqlwhere");
                result = form.DeleteWhere(strTblName, sqlWhere).ToString();
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return result;

        }
        public string Runing_KunSave()
        {
            try
            {
                string kubcode = this.GetRequestVal("KunCode");
                string kunName = this.GetRequestVal("KunName");
                string userNo = this.GetRequestVal("UserNo");

                AppFormLogic form = new AppFormLogic();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string sql = string.Format("SELECT MAX(KBNVALUE) FROM {0} WHERE KBNCODE = '{1}'", "MT_KBN", kubcode);
                int kunValue = BP.DA.DBAccess.RunSQLReturnValInt(sql);
                dic.Add("KBNCODE", kubcode);
                dic.Add("KBNVALUE", (kunValue + 1).ToString());
                dic.Add("KBNNAME", kunName);
                dic.Add("REC_ENT_USER", userNo);
                dic.Add("REC_ENT_DATE", DateTime.Now.ToString());

                form.Insert("MT_KBN", dic);

                return "success";
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        public string Runing_Save()
        {
            int restlt = -1;
            try
            {
                string tblName = this.GetRequestVal("TblName");
                Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal(tblName));

                restlt = form.Insert(tblName, AddCommonInfo(dicTbl));
                if (restlt < 1)
                {
                    return "err@" + "インサート失敗しました。";
                }

            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }

        public string Runing_Update()
        {
            int restlt = -1;
            try
            {
                string tblName = this.GetRequestVal("TblName");
                string tblKey = this.GetRequestVal("TblKey");
                string tblKeyValue = this.GetRequestVal("TblKeyValue");
                Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal(tblName));

                restlt = form.Update(this.GetRequestVal("TblName"), AddCommonInfo(dicTbl), tblKey, tblKeyValue);
                if (restlt < 1)
                {
                    return "err@" + "更新失敗しました。";
                }

            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }

        public string Runing_UpdateForKeys()
        {
            int restlt = -1;
            try
            {
                string tblName = this.GetRequestVal("TblName");
                string tblKeyInfo = this.GetRequestVal("TblKeyInfo");
                Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal(tblName));

                restlt = form.UpdateForKeys(this.GetRequestVal("TblName"), AddCommonInfo(dicTbl), tblKeyInfo);
                if (restlt < 1)
                {
                    return "err@" + "更新失敗しました。";
                }

            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }

        public string Runing_KunSelect()
        {
            AppFormLogic form = new AppFormLogic();
            string kubcode = this.GetRequestVal("KunCode");
            string userNo = this.GetRequestVal("UserNo");

            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                dic.Add("KBN", form.GetKbnName(kubcode, userNo));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        public string Runing_DeleteAllFile()
        {

            string oId = this.GetRequestVal("WorkID");

            // システムパスの取得
            string basePath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                            + "wwwroot" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + "upload";

            string filePath = basePath + Path.DirectorySeparatorChar + oId + Path.DirectorySeparatorChar;

            try
            {
                // フォルダ存在チェック
                if (Directory.Exists(filePath))
                {
                    DirectoryInfo dir = new DirectoryInfo(filePath);
                    FileInfo[] files = dir.GetFiles();

                    foreach (var item in files)
                    {
                        // ファイル削除
                        File.Delete(item.FullName);

                    }

                    // DB削除
                    form.Delete(FIL_TBL_NAME, "OID", oId);
                }
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }

            return "ok";
        }

        public string Runing_DeleteFile()
        {

            string oId = this.GetRequestVal("WorkID");
            string fileName = this.GetRequestVal("fileName");
            string seqno = this.GetRequestVal("seqno");

            // システムパスの取得
            string basePath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                            + "wwwroot" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + "upload";

            string dbFileName = basePath + Path.DirectorySeparatorChar + oId + Path.DirectorySeparatorChar + fileName;

            try
            {
                // ファイル削除
                if (File.Exists(dbFileName))
                {
                    File.Delete(dbFileName);
                }

                // DB削除
                form.DeleteWhere(FIL_TBL_NAME, "WHERE OID='" + oId + "' AND SEQNO='" + seqno + "'");

            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }

            return "ok";
        }

        public string Runing_FileUpload()
        {

            // アップロードファイル
            IFormFile iff = null;
            FileStream fs = null;
            BinaryWriter bw = null;

            // システムパスの取得
            string basePath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                            + "wwwroot" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + "upload";

            // ファイル保存フォルダ
            string uploadPath = basePath + Path.DirectorySeparatorChar + this.GetRequestVal("WorkID") + Path.DirectorySeparatorChar;

            try
            {
                // フォルダの存在チェック
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                for (var i = 0; i < BP.Web.HttpContextHelper.RequestFilesCount; i++)
                {
                    // i件目の処理
                    iff = BP.Web.HttpContextHelper.RequestFiles(i);
                    string[] fNames = iff.FileName.Split("\\");
                    string fName = fNames[fNames.Length - 1];

                    // 保存情報
                    string dbSave = uploadPath + fName;

                    // ファイル削除
                    if (File.Exists(dbSave))
                    {
                        File.Delete(dbSave);
                    }

                    // ファイル形式変換
                    Stream stream = iff.OpenReadStream();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    // ファイル出力
                    fs = new FileStream(dbSave, FileMode.Create);
                    bw = new BinaryWriter(fs);
                    bw.Write(bytes);

                    // 閉じる
                    bw.Close();
                    fs.Close();

                    // DB削除
                    form.DeleteWhere(FIL_TBL_NAME, "WHERE OID='" + this.GetRequestVal("WorkID") + "' AND TENPUSHIRYOKASYO='" + dbSave + "'");

                    // DB登録
                    Dictionary<string, string> newData = new Dictionary<string, string>();
                    newData.Add(TBL_KEY_OID, this.GetRequestVal("WorkID"));
                    newData.Add(TBL_UPD_PATH, dbSave);
                    form.Insert(FIL_TBL_NAME, AddCommonInfo(newData));
                }
            }
            catch (Exception ex)
            {
                if (bw != null)
                {
                    bw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }

            // アップロードしたファイル名を画面に戻す
            return "ok";
        }

        public string Runing_Select()
        {
            AppFormLogic form = new AppFormLogic();
            string TblName = this.GetRequestVal("TblName");
            string SelectInfo = this.GetRequestVal("SelectInfo");
            string FromInfo = this.GetRequestVal("FromInfo");
            string WhereInfo = this.GetRequestVal("WhereInfo");

            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("SELECT {0} FROM {1} {2} WHERE {3}", SelectInfo, TblName, FromInfo, WhereInfo);
                dic.Add(TblName, BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        public string Runing_TravelfeeKbnSelect()
        {
            AppFormLogic form = new AppFormLogic();
            string strTblName = this.GetRequestVal("TblName");
            string strKey = string.Empty;
            string userNo = this.GetRequestVal("UserNo");

            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("SELECT * FROM {0} WHERE OID is null AND REC_ENT_USER = '{1}'", strTblName, userNo);
                dic.Add("TT_WF_REPORT_TRAVELFEE", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
            }
            return BP.Tools.Json.ToJson(dic);
        }

        public string Runing_GetMNKeyRel()
        {
            AppFormLogic form = new AppFormLogic();
            string strDptName = this.GetRequestVal("dptCode");
            string strSeqNo = this.GetRequestVal("keyName");
            string strTblName = this.GetRequestVal("tblName");
            List<string> listKeyValue = new List<string>();

            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string[] listKeyName = strSeqNo.Split(",");
                for (int i = 0; i < listKeyName.Length; i++)
                {
                    string sql = string.Format("SELECT WF_KEY_VALUE FROM {0} WHERE WF_KEY_DEPT IN ({1}) AND SEQNO = '{2}'", strTblName, strDptName, listKeyName[i].ToString());
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    string[] listTemp = BP.DA.DBAccess.RunSQLReturnTable(sql).Rows[0]["WF_KEY_VALUE"].ToString().Split(";");
                    for (int j = 0; j < listTemp.Length; j++)
                    {
                        if (!listKeyValue.Contains(listTemp[j]))
                        {
                            listKeyValue.Add(listTemp[j]);
                        }
                    }
                }
                dic.Add(strTblName, listKeyValue);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
            }
            return BP.Tools.Json.ToJson(dic);
        }
        /// <summary>
        /// 経費負担部署情報の取得
        /// </summary>
        /// <returns></returns>
        public string FindShozokuName()
        {
            string bushCode = this.GetRequestVal("BUSHOCODE");
            string shainbango = string.IsNullOrEmpty(this.GetRequestVal("SHAINBANGO")) ? this.GetRequestVal("UserNo") : this.GetRequestVal("SHAINBANGO");
            string stMsg = "err@経費負担部署が存在しません。";
            string sql = string.Format("SELECT FinancialDepartment.BUSHOMEI_ZENKAKUKANA " +
                                       "from MT_Employee AS Employee " +
                                       "left join MT_Department AS Department ON Department.SHOZOKUCODE = Employee.JINJISHOZOKUCODE " +
                                       "left join MT_FinancialDepartment AS FinancialDepartment ON (FinancialDepartment.BUSHOCODE ='{0}' AND FinancialDepartment.KAISHACODE = Department.KAISHACODE " +
                                       "AND FinancialDepartment.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112))" +
                                       "AND FinancialDepartment.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                                       "where SHAINBANGO = '{1}'", bushCode, shainbango);
            return string.IsNullOrEmpty(BP.DA.DBAccess.RunSQLReturnString(sql)) ? stMsg : BP.DA.DBAccess.RunSQLReturnString(sql);
        }
        public Dictionary<string, string> SetRtnMessage( string id,string[] msg)
        {
            string str = "";
            for ( int i=0;i< msg.Length;i++)
            {
                if (i!=0)
                {
                    str +=  ",";
                }
                str += msg[i];
            }
            Dictionary<string, string> dicRtn = new Dictionary<string, string>();
            dicRtn.Add("key",id);
            dicRtn.Add("value", str);
            return dicRtn;
        }

        /// <summary>
        /// AzureStorageにアップロード用共通メソッド
        /// </summary>
        /// <returns></returns>
        private async Task TaskDoAzureStorageUpload(Dictionary<string, string> dicTbl)
        {
            // アップロード判定用キー
            string upload1 = this.GetRequestVal("UPLOADE_FILE_1");
            string upload2 = this.GetRequestVal("UPLOADE_FILE_2");

            // アップロードファイルがない場合
            if (BP.Web.HttpContextHelper.RequestFilesCount == 0)
            {
                // 空以外の場合更新を行う
                if (upload1 != null)
                {
                    dicTbl.Add("UPLOADE_FILE_1", "");
                }
                if (upload2 != null)
                {
                    dicTbl.Add("UPLOADE_FILE_2", "");
                }

                // アップロードされない
                return;
            }

            // AzureStorage接続情報の取得
            string connectionString = SystemConfig.AppSettings["AzureStorageKey"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // フォルダー名
            string containerName = this.GetRequestVal("WorkID");

            // フォルダー取得
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // フォルダー存在しない場合、新規作成
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // 初期化
            // 空以外の場合更新を行う
            if (upload1 != null)
            {
                dicTbl.Add("UPLOADE_FILE_1", upload1);
            }
            if (upload2 != null)
            {
                dicTbl.Add("UPLOADE_FILE_2", upload2);
            }

            // ファイル件数のループ
            for (var i = 0; i < BP.Web.HttpContextHelper.RequestFilesCount; i++)
            {

                // ファイル取得
                IFormFile upFile = BP.Web.HttpContextHelper.RequestFiles(i);

                // ファイル名にパスの削除
                string[] tempName = upFile.FileName.Split(Path.DirectorySeparatorChar);

                // AzureStorageにファイル名
                BlobClient blobClient = containerClient.GetBlobClient(tempName[tempName.Length - 1]);
                
                // 入力データStream 
                await blobClient.UploadAsync(upFile.OpenReadStream(), true);

                // DBにファイルパスを保存する
                dicTbl.Remove(upFile.Name);

                // ファイルアップロード
                dicTbl.Add(upFile.Name, blobClient.Uri.OriginalString);
            }
        }

        /// <summary>
        /// AzureStorageにアップロード用共通メソッド（サブテーブル用）
        /// </summary>
        /// <returns></returns>
        private async Task TaskDoAzureStorageUploadForDetail(Dictionary<string, string> dicTbl, int cnt)
        {
            // アップロード判定用キー
            string upload1 = this.GetRequestVal("UPLOADE_FILE_1");

            // アップロードファイルがない場合
            if (BP.Web.HttpContextHelper.RequestFilesCount == 0)
            {
                // 空以外の場合更新を行う
                if (upload1 != null)
                {
                    if (dicTbl.ContainsKey("UPLOADE_FILE_1"))
                    {
                        dicTbl.Remove("UPLOADE_FILE_1");
                    }
                    dicTbl.Add("UPLOADE_FILE_1", "");
                }

                // アップロードされない
                return;
            }

            // AzureStorage接続情報の取得
            string connectionString = SystemConfig.AppSettings["AzureStorageKey"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // フォルダー名
            string containerName = this.GetRequestVal("WorkID");

            // フォルダー取得
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // フォルダー存在しない場合、新規作成
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // 初期化
            // 空以外の場合更新を行う
            if (upload1 != null)
            {
                if (dicTbl.ContainsKey("UPLOADE_FILE_1"))
                {
                    dicTbl.Remove("UPLOADE_FILE_1");
                }
                dicTbl.Add("UPLOADE_FILE_1", upload1);
            }

            // ファイル取得
            IFormFile upFile = BP.Web.HttpContextHelper.RequestFiles(cnt);

            // ファイル名にパスの削除
            string[] tempName = upFile.FileName.Split(Path.DirectorySeparatorChar);

            // AzureStorageにファイル名
            BlobClient blobClient = containerClient.GetBlobClient(tempName[tempName.Length - 1]);

            // 入力データStream 
            await blobClient.UploadAsync(upFile.OpenReadStream(), true);

            // DBにファイルパスを保存する
            dicTbl.Remove(upFile.Name);

            // ファイルアップロード
            dicTbl.Add(upFile.Name, blobClient.Uri.OriginalString);
        }

        /// <summary>
        /// メール送信用共通メソッド
        /// </summary>
        /// <returns></returns>
        public string DoMailSend()
        {

            // テンプレート取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("select B.RNRAKUSAKI_NAME,");
            sqlSb.Append("       B.KAHEN_FLG,");
            sqlSb.Append("       B.SOUSINSAKI_TYPE,");
            sqlSb.Append("       B.EMAIL,");
            sqlSb.Append("       C.TEMPLATE_TITLE,");
            sqlSb.Append("       C.TEMPLATE_CONTENTS,");
            sqlSb.Append("       D.KBNNAME as WF_EMAIL_KBN_NAME,");
            sqlSb.Append("       E.KBNNAME as EMAIL_TIMING_KBN_NAME,");
            sqlSb.Append("       F.KBNNAME as SINSEISYA_KBN_NAME");

            // メール送信設定マスタ
            sqlSb.Append(" from MT_MN_EMAIL_SET A");

            // 連絡先マスタ
            sqlSb.Append("  left join MT_MN_EMAIL_STAKEHOLDER B");
            sqlSb.Append("      on A.RNRAKUSAKI_ID = B.RNRAKUSAKI_ID");

            // メールテンプレートマスタ
            sqlSb.Append("  left join MT_MN_EMAIL_TEMPLATE C");
            sqlSb.Append("      on A.EMAIL_TEMP_ID = C.TEMPLATE_ID");

            // 区分マスタ
            sqlSb.Append("  left join MT_KBN D");
            sqlSb.Append("      on D.KBNCODE = 'WF_EMAIL_KBN' and A.WF_EMAIL_KBN = D.KBNVALUE");
            sqlSb.Append("  left join MT_KBN E");
            sqlSb.Append("      on E.KBNCODE = 'EMAIL_TIMING_KBN' and A.EMAIL_TIMING_KBN = E.KBNVALUE");
            sqlSb.Append("  left join MT_KBN F");
            sqlSb.Append("      on F.KBNCODE = 'SINSEISYA_KBN' and A.SINSEISYA_KBN = F.KBNVALUE");

            // 条件
            sqlSb.Append(" where A.WF_EMAIL_KBN = @wfKbn and");
            sqlSb.Append("       A.EMAIL_TIMING_KBN = @timingKbn and");
            sqlSb.Append("       A.SINSEISYA_KBN = @sinseisyaKbn and");
            sqlSb.Append("	     A.SOUSIN_YOHI_FLG = '1';");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフロー区分
            ps.Add("wfKbn", this.GetRequestVal("wfKbn"));

            // 送信タイミング区分
            ps.Add("timingKbn", this.GetRequestVal("timingKbn"));

            try
            {
                // 申請者区分の取得
                string sinseisyaKbn = GetSinseisyaKbnByWorkId(this.GetRequestVal("workingId"), this.GetRequestVal("tehaiFlg"));

                // 申請者区分
                ps.Add("sinseisyaKbn", sinseisyaKbn);

                // メールテンプレートの取得
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

                // 検索結果のループ
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // 送信元
                    var from = new EmailAddress(SystemConfig.AppSettings["SenderMail"], SystemConfig.AppSettings["SenderName"]);

                    // 送信宛先リスト
                    List<EmailAddress> toList = new List<EmailAddress>();

                    // 送信宛先の設定
                    // 可変区分が「1：固定」の場合
                    if (dt.Rows[i][1].ToString() == KAHEN_FLG_KOTEI)
                    {
                        // 連絡先マスタのメール宛先を取得する
                        string[] masterTos = dt.Rows[i][3].ToString().Split(",");

                        // 連絡先マスタのメール宛先の件数分ループ
                        foreach (string masterTo in masterTos)
                        {
                            // 送信宛先を連絡先マスタのメール宛先に設定する
                            var to = new EmailAddress(masterTo, dt.Rows[i][0].ToString());

                            // 送信宛先リストに追加
                            toList.Add(to);
                        }
                    }
                    else
                    {

                        // 可変の場合 TODO
                    }

                    // タイトル
                    var subject = dt.Rows[i][4].ToString();

                    // 送信内容の設定
                    var mailContent = DoTemplateRepace(dt.Rows[i][5].ToString());

                    // 認証キー
                    var apiKey = SystemConfig.AppSettings["SendGridKey"];

                    // 送信オブジェクト
                    var client = new SendGridClient(apiKey);

                    // メール送信
                    Execute(client, from, toList, subject, mailContent).Wait();
                }

                return "ok";
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(WF_OutLog.CCFlowCommonLog(ex.Message, this.GetRequestVal("WorkID"), this.GetRequestVal("UserNo"), this.GetRequestVal("FK_Flow")), WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// メール送信メソッド
        /// </summary>
        /// <returns></returns>
        public async Task Execute(SendGridClient client, EmailAddress from, List<EmailAddress> toList, string subject, string mailContent)
        {
            var response = await client.SendEmailAsync(MailHelper.CreateSingleEmailToMultipleRecipients(from, toList, subject, mailContent, mailContent));
        }

        /// <summary>
        /// 申請者区分取得メソッド
        /// </summary>
        /// <returns></returns>
        private string GetSinseisyaKbnByWorkId(string workId, string tehaiFlg)
        {
            // 手配業者の場合
            if (tehaiFlg == "1")
            {
                return SINSEISYA_KBN_TEHAI;
            }

            // 申請者区分
            string sinseisyaKbn = "";

            // 申請者区分取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("select B.SHUKKOKBN,");
            sqlSb.Append("       A.SHINSEISYAKBN");

            // メール送信設定マスタ
            sqlSb.Append(" from TT_WF_CONDOLENCE A");

            // 連絡先マスタ
            sqlSb.Append("  left join MT_Employee B");
            sqlSb.Append("      on A.UNFORTUNATE_SHAINBANGO = B.SHAINBANGO");

            // 条件
            sqlSb.Append(" where A.OID = @workId;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフローID
            ps.Add("workId", workId);

            // 申請者区分取得
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

            // 出向されない場合
            if (dt.Rows[0][0] == DBNull.Value || dt.Rows[0][0].ToString() == "0000")
            {
                // 本人の場合
                if (dt.Rows[0][1].ToString() == SINSEISYA_KBN_HONNIN)
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
                if (dt.Rows[0][1].ToString() == "0")
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
        /// メールテンプレート可変部置換メソッド
        /// </summary>
        /// <returns></returns>
        private string DoTemplateRepace(string template)
        {
            // 置換結果
            string result = template;

            // キーワード情報取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("select KEY_WORD_NAME,");
            sqlSb.Append("       TABLE_NAME,");
            sqlSb.Append("	     FIELD_NAME");

            // メール送信キーワードマスタ
            sqlSb.Append(" from MT_MN_EMAIL_KEYWORD");

            // 条件
            sqlSb.Append(" where WF_EMAIL_KBN = @wfKbn;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフロー区分
            ps.Add("wfKbn", this.GetRequestVal("wfKbn"));

            // 置換用キーワード情報の取得
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

            // 取得したキーワードの置換
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // 取得したテーブルとフィールドで置換後の値を取得する
                string after = GetMailKeyWord(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString());

                // 置換実行
                result = result.Replace("%%" + dt.Rows[i][0].ToString() + "%%", after);
            }

            // 置換したメール内容を戻す
            return result;
        }

        /// <summary>
        /// メールキーワード取得用メソッド
        /// </summary>
        /// <returns></returns>
        private string GetMailKeyWord(string table, string field)
        {
            // キーワード情報取得用sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // 項目
            sqlSb.Append("select " + field);

            // メール送信キーワードマスタ
            sqlSb.Append(" from " + table);

            // 条件
            sqlSb.Append(" where OID = @oid;");

            // 入力条件の置換
            Paras ps = new Paras();

            // ワークフロー区分
            ps.Add("oid", this.GetRequestVal("workingId"));

            // 置換用キーワード情報の取得
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);

            // 結果を戻す
            return dt.Rows[0][0].ToString();
        }

        /// <summary>
        /// APIでEBSからデータを取得すること
        /// パラメータがないメソッド
        /// </summary>
        /// <param name="apiName">API側のメソッド名</param>
        /// <returns>EBSから取得のデータ</returns>
        public static List<Dictionary<string, string>> GetEbsDataWithApi(string apiName)
        {
            // パラメータ（ディクショナリ型）をＪＳＯＮに変更すること
            string jsonApiParam = JsonConvert.SerializeObject(new Dictionary<string, string>());

            // 結果を戻す
            return WF_AppForm.GetEbsDataWithApi(apiName, jsonApiParam);
        }

        /// <summary>
        /// APIでEBSからデータを取得すること
        /// </summary>
        /// <param name="apiName">API側のメソッド名</param>
        /// <param name="apiParam">API側で実行ｓｑｌの時に必要パラメータ（ディクショナリ型）</param>
        /// <returns>EBSから取得のデータ</returns>
        public static List<Dictionary<string, string>> GetEbsDataWithApi(string apiName, Dictionary<string, object> apiParam)
        {
            // パラメータ（ディクショナリ型）をＪＳＯＮに変更すること
            string jsonApiParam = JsonConvert.SerializeObject(apiParam);

            // 結果を戻す
            return WF_AppForm.GetEbsDataWithApi(apiName, jsonApiParam);
        }

        /// <summary>
        /// APIでEBSからデータを取得すること
        /// </summary>
        /// <param name="apiName">API側のメソッド名</param>
        /// <param name="apiParam">API側で実行ｓｑｌの時に必要パラメータ（json型）</param>
        /// <returns>EBSから取得のデータ</returns>
        public static List<Dictionary<string, string>> GetEbsDataWithApi(string apiName, string apiParam)
        {
            // APIにアクセスするＵＲＬのヘッダー
            string dynamicHandler = string.Empty;
            // APIにアクセスするＵＲＬのヘッダーの取得
            dynamicHandler = API_HANDLER;

            // APIにアクセスするＵＲＬの取得
            string url = dynamicHandler + API_SUB_URL;

            // WebClientのプロジェクトの作成
            System.Net.WebClient wc = new System.Net.WebClient();
            //NameValueCollectionの作成
            System.Collections.Specialized.NameValueCollection ps = new System.Collections.Specialized.NameValueCollection();

            //送信するデータ（フィールド名と値の組み合わせ）を追加
            // メソッド名の設定
            ps.Add("API_Name", apiName);
            // API側で実行ｓｑｌの時に必要パラメータの設定
            ps.Add("API_PRM", apiParam);

            //データを送信し、また受信する
            byte[] resData = wc.UploadValues(url, ps);
            wc.Dispose();

            // 受信したデータを取得する
            string resText = Encoding.UTF8.GetString(resData, 0, resData.Length);

            // BOMを付き状態を作成すること
            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };
            // BOMを付き状態の判断
            if (resData[0] == bomBuffer[0] && resData[1] == bomBuffer[1] && resData[2] == bomBuffer[2])
            {
                // BOMを付き状態を削除すること
                int copyLength = resData.Length - 3;
                byte[] dataNew = new byte[copyLength];
                Buffer.BlockCopy(resData, 3, dataNew, 0, copyLength);
                resText = System.Text.Encoding.UTF8.GetString(dataNew);
            }

            // json文字列をディクショナリに作成すること
            var retObj = JsonConvert.DeserializeObject<
                Dictionary<string, List<Dictionary<string, string>>>>(resText)["Get_Info"];

            // 結果を戻す
            return retObj;
        }

        /// <summary>
        ///サーバ時間を取る
        /// </summary>
        /// <returns>現在時間</returns>
        public string GetServerDateTime() 
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            dic.Add("serverDateTime", DateTime.Now.ToString());
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        ///サーバ日付を取る
        /// </summary>
        /// <returns>現在日付</returns>
        public string GetServerDate()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            dic.Add("serverDate", DateTime.Now.ToString("d"));
            return BP.Tools.Json.ToJson(dic);
        }
 
        public class CommonData
        {
            public string WorkID { get; set; }
            public string FK_Node { get; set; }
            public string FK_Flow { get; set; }
            public string UserNo { get; set; }
            public string SID { get; set; }
            public string NextNode { get; set; }
            public string newflag { get; set; }
            public string Mode { get; set; }
            public string NextFlowName { get; set; }
            public string FlowType { get; set; }
            public string Starter { get; set; }
            public int NewFK_Node { get; set; }
            public string AutoApprovalMode { get; set; }
            public string DoMode { get; set; }
            public string AgentMode { get; set; }
            public string NewFlowMode { get; set; }
            
            public string dispalyFlag { get; set; }

            public string toEmps { get; set; }
            public string toNodeID { get; set; }
            public string ApprovalUpdataMode { get; set; }
        }

    }
}
