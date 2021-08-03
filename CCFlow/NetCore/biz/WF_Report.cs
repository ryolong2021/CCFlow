
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.WF.HttpHandler
{
    public class WF_Report : BP.WF.HttpHandler.DirectoryPageBase
    {
        public const string TBL_KEY_OID = "OID";  // 更新テーブルキー
        public const string TBL_UPD_PATH = "TENPUSHIRYOKASYO";  // 添付ファイル格納場所
        public const string FIL_TBL_NAME = "TT_WF_REPORT_ATTACHMENT";  // 出張報告添付ファイル明細テーブル
        AppFormLogic form = new AppFormLogic();
        /// <summary>
        /// 画面初期化
        /// </summary>
        /// <returns></returns>
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

            string[] KbnCodeEntUserList = {
                                    "VISIT_KBN",//訪問先
                                    "PURPOSE_KBN",//用件
            };

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
        /// <summary>
        /// ワークフローOIDの取得
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 出張報告トランザクションテーブルの初期設定
        /// </summary>
        /// <returns></returns>
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
                return "err@" + ex.Message;
            }
            return "";
        }

        /// <summary>
        /// 出張報告の社員情報の取得
        /// </summary>
        /// <returns></returns>
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
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        /// 資格別手当情報の取得
        /// </summary>
        /// <returns></returns>
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
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        /// アップロードファイルの全部削除
        /// </summary>
        /// <returns></returns>
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
                return "err@" + ex.Message;
            }

            return "ok";
        }
        /// <summary>
        /// アップロードファイルの削除
        /// </summary>
        /// <returns></returns>
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
                return "err@" + ex.Message;
            }

            return "ok";
        }
        /// <summary>
        /// ファイルのアップロード
        /// </summary>
        /// <returns></returns>
        //public string Runing_FileUpload()
        //{

        //    // アップロードファイル
        //    IFormFile iff = null;
        //    FileStream fs = null;
        //    BinaryWriter bw = null;

        //    // システムパスの取得
        //    string basePath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
        //                    + "wwwroot" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + "upload";

        //    // ファイル保存フォルダ
        //    string uploadPath = basePath + Path.DirectorySeparatorChar + this.GetRequestVal("WorkID") + Path.DirectorySeparatorChar;

        //    try
        //    {
        //        // フォルダの存在チェック
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }

        //        for (var i = 0; i < BP.Web.HttpContextHelper.RequestFilesCount; i++)
        //        {
        //            // i件目の処理
        //            iff = BP.Web.HttpContextHelper.RequestFiles(i);
        //            string[] fNames = iff.FileName.Split("\\");
        //            string fName = fNames[fNames.Length - 1];

        //            // 保存情報
        //            string dbSave = uploadPath + fName;

        //            // ファイル削除
        //            if (File.Exists(dbSave))
        //            {
        //                File.Delete(dbSave);
        //            }

        //            // ファイル形式変換
        //            Stream stream = iff.OpenReadStream();
        //            byte[] bytes = new byte[stream.Length];
        //            stream.Read(bytes, 0, bytes.Length);
        //            stream.Seek(0, SeekOrigin.Begin);

        //            // ファイル出力
        //            fs = new FileStream(dbSave, FileMode.Create);
        //            bw = new BinaryWriter(fs);
        //            bw.Write(bytes);

        //            // 閉じる
        //            bw.Close();
        //            fs.Close();

        //            // DB削除
        //            form.DeleteWhere(FIL_TBL_NAME, "WHERE OID='" + this.GetRequestVal("WorkID") + "' AND TENPUSHIRYOKASYO='" + dbSave + "'");

        //            // DB登録
        //            Dictionary<string, string> newData = new Dictionary<string, string>();
        //            newData.Add(TBL_KEY_OID, this.GetRequestVal("WorkID"));
        //            newData.Add(TBL_UPD_PATH, dbSave);
        //            form.Insert(FIL_TBL_NAME, AddCommonInfo(newData));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (bw != null)
        //        {
        //            bw.Close();
        //        }
        //        if (fs != null)
        //        {
        //            fs.Close();
        //        }
        //        return "err@" + ex.Message;
        //    }

        //    // アップロードしたファイル名を画面に戻す
        //    return "ok";
        //}
        /// <summary>
        /// 交通費明細を取得
        /// </summary>
        /// <returns></returns>
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
            catch (Exception)
            {
            }
            return BP.Tools.Json.ToJson(dic);
        }
    }
}
