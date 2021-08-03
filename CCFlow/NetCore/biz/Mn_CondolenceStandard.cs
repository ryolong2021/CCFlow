using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BP.WF.HttpHandler
{
    public class Mn_CondolenceStandard : BP.WF.HttpHandler.DirectoryPageBase
    {

        WF_AppForm wf_appfrom = new WF_AppForm();　　　　　　　　　　　　　　　　　　　// 共通部品
        AppFormLogic form = new AppFormLogic();　　　　　　　　　　　　　　　　　　　　// 共通部品(ロジック)
        List<kbnDat> listKbnDat = new List<kbnDat>();　　　　　　　　　　　　　　　　　// 区分マスタ格納
        Dictionary<string, string> dicStandard = new Dictionary<string, string>();　　 // 慶弔基準マスタ格納（値、DBインサート用）
        Dictionary<string, string> dicStandardNm = new Dictionary<string, string>();   // 慶弔基準マスタ格納（名、ログ用）
        int index = 0;                                                                 // DBインサート件数
        string errMessage = string.Empty;                                              //エラーメッセージ返却
        string ReadExcelFile= string.Empty;                                            //リードファイル

        /// <summary>
        /// ECXELデータインサート処理
        /// </summary>
        /// <returns></returns>
        public string BD_MaterialImport()
        {
            string ret = string.Empty;
            int loopIndex = 2;
            // アップロードファイル
            IFormFile iff = null;

            try
            {
                //ユーザーIDを取得
                string strUesrid = this.GetRequestVal("uesrid");
                //慶弔基準マスタ番号を取得
                string strCondolenceNo = this.GetRequestVal("condolence_no");
                //文件を取得
                iff = BP.Web.HttpContextHelper.RequestFiles(0);
                ReadExcelFile = FileUpload(iff);
                //文件を読み込む
                DataTable dtExcel = BP.DA.DBLoad.ReadExcelFileToDataTable(ReadExcelFile);
                //区分マスタデータを取得する
                GetKbnLIst();
                // 慶弔基準マスタバックアップ処理
                if (!BackupCondolenceStandard(strCondolenceNo))
                {
                    return "err@慶弔基準マスタバックアップに失敗しました。";
                }

                for (int i = 0; i < dtExcel.Rows.Count; i++)
                {
                    dicStandard = new Dictionary<string, string>();
                    //慶弔基準マスタ番号
                    dicStandard["CONDOLENCE_NO"] = strCondolenceNo;
                    for (int j = 0; j < dtExcel.Columns.Count; j++)
                    {
                        //string val = dtExcel.Rows[i][j].ToString();
                        if (string.IsNullOrEmpty(dtExcel.Rows[i][j].ToString()))
                        {
                            continue;
                        }
                        else
                        {
                            if (j < 8)
                            {
                                if (!getInfo(strCondolenceNo, dtExcel.Rows[i][j].ToString()))
                                {
                                    return errMessage;
                                }
                            }
                            else
                            {
                                for (int m = 0; m < loopIndex; m++)
                                {
                                    //差出人区分
                                    dicStandard["SASHIDASHI_KBN"] = (m).ToString();
                                    //香料チェック
                                    string strKoryonum = CheckPattern(dtExcel.Rows[i][j++].ToString(), strCondolenceNo);
                                    if (!string.IsNullOrEmpty(strKoryonum))
                                    {
                                        //香料数量
                                        dicStandard["KORYONUM"] = strKoryonum;
                                    }
                                    else
                                    {
                                        return errMessage;
                                    }
                                    //供花チェック
                                    string strKyokanum = CheckPattern(dtExcel.Rows[i][j++].ToString(), strCondolenceNo);
                                    if (!string.IsNullOrEmpty(strKyokanum))
                                    {
                                        //供花数量
                                        dicStandard["KYOKANUM"] = strKyokanum;
                                    }
                                    else
                                    {
                                        return errMessage;
                                    }
                                    //弔電チェック
                                    string strTyoden = ChangeTyodenToNum(dtExcel.Rows[i][j++].ToString(), strCondolenceNo);
                                    if (!string.IsNullOrEmpty(strTyoden))
                                    {
                                        //弔電数量
                                        dicStandard["TYODENNUM"] = strTyoden;
                                    }
                                    else {
                                        return errMessage;
                                    }
                                    //登録日時
                                    dicStandard["REC_ENT_DATE"] = DateTime.Now.ToString();
                                    //登録者
                                    dicStandard["REC_ENT_USER"] = strUesrid;
                                    //最終更新日時
                                    dicStandard["REC_EDT_DATE"] = DateTime.Now.ToString();
                                    //最終更新者
                                    dicStandard["REC_EDT_USER"] = strUesrid;
                                    //登録する
                                    ret = DbInsert(strCondolenceNo);
                                    if (!string.IsNullOrEmpty(ret))
                                    {
                                        return ret;
                                    }
                                    //j++;
                                }
                                break;
                            }
                        }
                    }

                }
                // バックアップ慶弔基準マストデータを削除する
                if (!DelBackupCondolenceStandard(strCondolenceNo, "bak"))
                {
                    return "err@バックアップ慶弔基準マストデータを削除に失敗しました。";
                }


                return ret;
            }
            catch (Exception ex)
            {

                return "err@" + ex.Message;
            }
            finally
            {
                // ファイル削除
                if (File.Exists(ReadExcelFile))
                {
                    File.Delete(ReadExcelFile);
                }
            }
        }
        /// <summary>
        /// 慶弔基準マストをバックアップする
        /// </summary>
        /// <returns></returns>
        private bool BackupCondolenceStandard(string condolenceNo)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("CONDOLENCE_NO", condolenceNo + "bak");
                if (form.Update("MT_MN_CONDOLENCE_STANDARD", dic, "CONDOLENCE_NO", condolenceNo) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// エクセル取得した弔電内容を変化処理
        /// </summary>
        /// <returns></returns>
        private string ChangeTyodenToNum(string strTyoden,string strCondolenceNo)
        {
            if (strTyoden == "あり")
            {
                return "1";
            }
            else if (strTyoden == "なし")
            {
                return "0";
            }
            else
            {
                GetErrMessage(strCondolenceNo, "エクセル中に弔電は「あり/なし」以外を入力しました。");
                return string.Empty;
            }

        }
        /// <summary>
        /// エクセル出力弔電内容を変化処理
        /// </summary>
        /// <returns></returns>
        private string ChangeNumToTyoden(string strTyoden)
        {
            if (strTyoden == "1")
            {
                return "あり";
            }
            else
            {
                return "なし";
            }         
        }
        /// <summary>
        /// 数字以外チェックする
        /// </summary>
        /// <returns></returns>
        private string CheckPattern(string pattern, string strCondolenceNo)
        {

            Regex regex = new Regex("^[0-9]*$");

            if (regex.IsMatch(pattern))
            {
                return pattern;
            }
            else
            {
                GetErrMessage(strCondolenceNo, "エクセル中に香料/供花数は数字以外を入力しました。");
                return string.Empty;
            }

        }
        /// <summary>
        /// バックアップ慶弔基準マストデータを削除する
        /// </summary>
        /// <returns></returns>
        private bool DelBackupCondolenceStandard(string condolenceNo, string strbak =null)
        {
            try
            {
                if (form.DeleteWhere("MT_MN_CONDOLENCE_STANDARD", "WHERE CONDOLENCE_NO = '" + condolenceNo + strbak +"'") >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }       
        }

        /// <summary>
        /// 慶弔基準適用日マストを取得する
        /// </summary>
        /// <returns></returns>
        public string Get_Condolence_ApplydayTbl()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string strKaishaCode = this.GetRequestVal("kaishacode");
                string strWhere = string.Empty;
                if (string.IsNullOrEmpty(strKaishaCode))
                {
                    strWhere = " KAISHACODE IS NOT NULL";
                }
                else
                {
                    strWhere = " KAISHACODE = '" + strKaishaCode + "'";
                }

                string sql = "SELECT KAISHACODE,TEKIYOYMD_FROM,TEKIYOYMD_TO,CONDOLENCE_NO FROM MT_MN_CONDOLENCE_APPLYDAY WHERE " + strWhere;
                dic.Add("Get_Condolence_ApplydayTbl", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        /// 慶弔基準適用日マストを設定する
        /// </summary>
        /// <returns></returns>
        public string Set_Condolence_ApplydayTbl()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int restlt = 0;
            try
            {
                //会社コード
                string strKAISHACODE = this.GetRequestVal("KAISHACODE");
                //適用年月日From
                string strTEKIYOYMD_FROM = this.GetRequestVal("TEKIYOYMD_FROM");
                //適用年月日To
                string strTEKIYOYMD_TO = this.GetRequestVal("TEKIYOYMD_TO");
                //慶弔基準マスタ番号
                string strCONDOLENCE_NO = this.GetRequestVal("CONDOLENCE_NO");
                //更新適用年月日From
                string strUpdateTEKIYOYMD_FROM = this.GetRequestVal("UPDATE_TEKIYOYMD_FROM");

                dic.Add("KAISHACODE", strKAISHACODE);
                dic.Add("TEKIYOYMD_FROM", strTEKIYOYMD_FROM);
                dic.Add("TEKIYOYMD_TO", strTEKIYOYMD_TO);
                dic.Add("CONDOLENCE_NO", strCONDOLENCE_NO);

                //慶弔基準適用日マスタに新たな適用開始日のデータを登録する
                restlt = form.Insert("MT_MN_CONDOLENCE_APPLYDAY", wf_appfrom.AddCommonInfo(dic));
                if (restlt < 1)
                {
                    return "err@登録失敗しました";
                }
                else
                {
                    if (!string.IsNullOrEmpty(strUpdateTEKIYOYMD_FROM))
                    {
                        Dictionary<string, string> dicTbl = new Dictionary<string, string>();
                        dicTbl.Add("TEKIYOYMD_TO", changeAddDays(strTEKIYOYMD_FROM));
                        string sqlWhere = string.Format(@" KAISHACODE = '{0}' AND TEKIYOYMD_FROM = '{1}'", strKAISHACODE, strUpdateTEKIYOYMD_FROM);
                        restlt = form.UpdateForKeys("MT_MN_CONDOLENCE_APPLYDAY", wf_appfrom.AddCommonInfo(dicTbl,false), sqlWhere);
                        if (restlt < 1)
                        {
                            return "err@" + "更新失敗しました。";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }

        /// <summary>
        /// 慶弔基準適用日と慶弔基準マスタデータを削除する
        /// </summary>
        /// <returns></returns>
        public string Delete_Condolence_ApplydayTbl()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int restlt = 0;
            string sqlWhere = string.Empty;
            try
            {
                //会社コード
                string strKAISHACODE = this.GetRequestVal("KAISHACODE");
                //適用年月日From
                string strTEKIYOYMD_FROM = this.GetRequestVal("TEKIYOYMD_FROM");
                //慶弔基準マスタ番号
                string strCONDOLENCE_NO = this.GetRequestVal("CONDOLENCE_NO");

                //Where条件を作成
                sqlWhere = string.Format(@" WHERE KAISHACODE = '{0}' AND TEKIYOYMD_FROM = '{1}'", strKAISHACODE, strTEKIYOYMD_FROM);
                //慶弔基準適用日マスタを削除する
                restlt = form.DeleteWhere("MT_MN_CONDOLENCE_APPLYDAY", sqlWhere);
                if (restlt < 1)
                {
                    return "err@削除失敗しました";
                }
                else
                {
                    //Where条件を作成
                    sqlWhere = string.Format(@" WHERE CONDOLENCE_NO = '{0}'", strCONDOLENCE_NO);
                    //慶弔基準マスタデータを削除する
                    restlt = form.DeleteWhere("MT_MN_CONDOLENCE_STANDARD", sqlWhere);
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }

        /// <summary>
        /// 適用開始年月日を更新する
        /// </summary>
        /// <returns></returns>
        public string Update_Condolence_ApplydayTbl()
        {

            Dictionary<string, string> dic = new Dictionary<string, string>();
            int restlt = 0;
            string sqlWhere = string.Empty;
            try
            {
                //会社コード
                string strKAISHACODE = this.GetRequestVal("KAISHACODE");
                //適用年月日From
                string strTEKIYOYMD_FROM = this.GetRequestVal("TEKIYOYMD_FROM");
                //慶弔基準マスタ番号
                string strCONDOLENCE_NO = this.GetRequestVal("CONDOLENCE_NO");
                //更新適用年月日From
                string strUpdateTEKIYOYMD_FROM = this.GetRequestVal("UPDATE_TEKIYOYMD_FROM");
                //更新適用年月日From
                string strUpdateTEKIYOYMD_TO = this.GetRequestVal("UPDATE_TEKIYOYMD_TO");
             
                //更新フラグ
                string strUpdateFlg = this.GetRequestVal("UPDATE_FLG");


                dic.Add("TEKIYOYMD_FROM", strTEKIYOYMD_FROM);
                dic.Add("CONDOLENCE_NO", strKAISHACODE + strTEKIYOYMD_FROM);

                //慶弔基準適用日マスタに新たな適用開始日のデータを登録する
                sqlWhere = string.Format(@" CONDOLENCE_NO = '{0}'", strKAISHACODE + strUpdateTEKIYOYMD_FROM);
                restlt = form.UpdateForKeys("MT_MN_CONDOLENCE_APPLYDAY", wf_appfrom.AddCommonInfo(dic,false), sqlWhere);
                if (restlt < 1)
                {
                    return "err@登録失敗しました";
                }
                else
                {
                    Dictionary<string, string> dicTbl = new Dictionary<string, string>();
                    if (strUpdateFlg == "true")
                    {
                        
                        dicTbl.Add("TEKIYOYMD_TO", changeAddDays(strTEKIYOYMD_FROM));
                        sqlWhere = string.Format(@" TEKIYOYMD_TO = '{0}' AND KAISHACODE = '{1}'", strUpdateTEKIYOYMD_TO, strKAISHACODE);
                        restlt = form.UpdateForKeys("MT_MN_CONDOLENCE_APPLYDAY", wf_appfrom.AddCommonInfo(dicTbl,false), sqlWhere);
                        if (restlt < 1)
                        {
                            return "err@" + "更新失敗しました。";
                        }

                    }
                    dicTbl = new Dictionary<string, string>();
                    dicTbl.Add("CONDOLENCE_NO", strKAISHACODE + strTEKIYOYMD_FROM);
                    //慶弔基準適用日マスタに新たな適用開始日のデータを登録する
                    sqlWhere = string.Format(@" CONDOLENCE_NO = '{0}'", strKAISHACODE + strUpdateTEKIYOYMD_FROM);
                    restlt = form.UpdateForKeys("MT_MN_CONDOLENCE_STANDARD", wf_appfrom.AddCommonInfo(dicTbl, false), sqlWhere);
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }

        private string changeAddDays(string dateString)
        {
            string retStr = string.Empty;

            DateTime dt = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            retStr = dt.AddDays(-1).ToString("yyyyMMdd");
            return retStr;
        }

        /// <summary>
        /// ダウンロード処理
        /// </summary>
        /// <returns></returns>
            public string DownLoad_Condolence_StandardDat()
        {
            try
            {
                //慶弔基準マスタ番号
                string strCONDOLENCE_NO = this.GetRequestVal("CONDOLENCE_NO");
                //Where条件を作成
                string sql = string.Format(@"
SELECT GLC_KBN
       ,KUMIAI_KBN
       ,JYUUGYOUINN_KBN
       ,SYUKOU_KBN
       ,DEAD_KBN
       ,ORGANIZER_KBN
       ,DOKYO_BEKYO_KBN
       ,FUYOU_KBN
       ,KORYONUM
       ,KYOKANUM
       ,TYODENNUM
FROM MT_MN_CONDOLENCE_STANDARD WHERE CONDOLENCE_NO = '{0}'", strCONDOLENCE_NO);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                // フロントに戻ること
                return this.MakeCsvDataInfo(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 区分マスターデータを取得
        /// </summary>
        /// <returns></returns>
        private void GetKbnLIst()
        {
            try
            {
                //区分マスターデータを取得
                string[] KbnCodeList = {
                                    "GLC_KBN",          //ＧＬＣ会員区分
                                    "KUMIAI_KBN",     //組合区分
                                    "JYUUGYOUINN_KBN",   //従業員区分
                                    "SYUKOU_KBN",       //出向区分
                                    "DEAD_KBN",         //亡くなられた方の続柄
                                    "ORGANIZER_KBN",       //喪主区分
                                    "DOKYO_BEKYO_KBN",     //同居区分
                                    "FUYOU_KBN"  //税扶養区分
                 };
                string str = string.Empty;
                for (int i = 0; i < KbnCodeList.Length; i++)
                {
                    if (i != 0)
                    {
                        str += ",";
                    }
                    str += "'" + KbnCodeList[i] + "'";
                }

                DataTable dtKbn = form.Select("select KBNCODE,KBNVALUE,KBNNAME from MT_KBN where REC_ENT_USER = 'admin' AND KBNCODE in (" + str + ")");
                listKbnDat = new List<kbnDat>();
                for (int i = 0; i < dtKbn.Rows.Count; i++)
                {
                    Console.WriteLine(dtKbn.Rows[i]["KBNCODE"].ToString() + "," + dtKbn.Rows[i]["KBNVALUE"].ToString() + "," + dtKbn.Rows[i]["KBNNAME"].ToString());
                    kbnDat kbndat = new kbnDat();
                    kbndat.KBNCODE = dtKbn.Rows[i]["KBNCODE"].ToString();
                    kbndat.KBNVALUE = dtKbn.Rows[i]["KBNVALUE"].ToString();
                    kbndat.KBNNAME = dtKbn.Rows[i]["KBNNAME"].ToString();
                    listKbnDat.Add(kbndat);
                }
            }
            catch (Exception)
            { 

            }
        }

        /// <summary>
        /// アップデート処理
        /// </summary>
        /// <param name="iff">ファイル</param>
        /// <returns></returns>
        private string FileUpload(IFormFile iff) {
            string fileSave = string.Empty;
            FileStream fs = null;
            BinaryWriter bw = null;

            try
            {
                // ファイル保存フォルダ
                string uploadPath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                                + "wwwroot" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar 
                                + "import" + Path.DirectorySeparatorChar;

                // フォルダの存在チェック
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // 保存情報
                fileSave = uploadPath + iff.FileName;

                // ファイル削除
                if (File.Exists(fileSave))
                {
                    File.Delete(fileSave);
                }

                // ファイル形式変換
                Stream stream = iff.OpenReadStream();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                // ファイル出力
                fs = new FileStream(fileSave, FileMode.Create);
                bw = new BinaryWriter(fs);
                bw.Write(bytes);

                // 閉じる
                bw.Close();
                fs.Close();
            }
            catch (Exception)
            {
                if (bw != null)
                {
                    bw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return fileSave;
        }

        /// <summary>
        /// データインサート処理
        /// </summary>
        /// <returns></returns>
        private string DbInsert(string strCondolenceNo)
        {
            string ret = string.Empty;
            index++;

            if (form.Insert("MT_MN_CONDOLENCE_STANDARD", wf_appfrom.AddCommonInfo(dicStandard)) > 0)
            {
                ret = "";
            }
            else
            {
                GetErrMessage(strCondolenceNo);
            }

            return ret; 
        }
        /// <summary>
        /// ファイルのフォーマットチェックがあれば、エラーメッセージ返却する
        /// </summary>
        /// <returns></returns>
        private void GetErrMessage(string strCondolenceNo,string strMsg=null)
        {
            
            DelBackupCondolenceStandard(strCondolenceNo);
            string str = string.Empty;
            foreach (var dic in dicStandardNm)
            {
                str = dic.Key.ToString() + dic.Value.ToString();
            }
            if (!string.IsNullOrEmpty(strMsg))
            {
                errMessage = "err@:" + (index / 2 + 1).ToString() + "行目登録失敗しました。エラー内容： " + strMsg ;
            }
            else {
                errMessage = "err@:" + (index / 2 + 1).ToString() + "行目登録失敗しました。エラー内容： 区分マスタ中に「" + str + "」が存在しません。";
            }

            
        }

        /// <summary>
        /// 区分マスタ通りに、エクセルデータの区分名から区分値に変化する
        /// </summary>
        /// <param name="kunNm">区分名</param>
        /// <returns></returns>
        private bool getInfo(string strCondolenceNo, string kunNm)
        {
            bool ret = false;
            dicStandardNm = new Dictionary<string, string>();
            int i = 0;
            for (i = 0; i < listKbnDat.Count; i++)
            {
                if (listKbnDat[i].KBNNAME == kunNm)
                {

                    if (dicStandard.ContainsKey(listKbnDat[i].KBNCODE))
                    {
                        dicStandard[listKbnDat[i].KBNCODE] = listKbnDat[i].KBNVALUE;
                    }
                    else
                    {
                        dicStandard.Add(listKbnDat[i].KBNCODE, listKbnDat[i].KBNVALUE);
                    }

                    return true;
                }
            }
            dicStandardNm.Add(kunNm, "");
            GetErrMessage(strCondolenceNo);
            return ret;
        }

        /// <summary>
        /// 区分マスタ通りに、区分値から区分名に変化する
        /// </summary>
        /// <param name="kunCode">区分コード</param>
        /// <param name="kunValue">区分値</param>
        /// <returns></returns>
        private string getKbnName(string kunCode,string kunValue)
        {
            string kbnName = string.Empty;
            for (int i = 0; i < listKbnDat.Count; i++)
            {
                if (listKbnDat[i].KBNCODE == kunCode && listKbnDat[i].KBNVALUE == kunValue)
                {
                    kbnName = listKbnDat[i].KBNNAME;
                    break;
                }
            }
            return kbnName;
        }

        /// <summary>
        /// CSVデータの作成
        /// </summary>
        /// <param name="dt">DBから抽出データ</param>
        /// <returns>出力CSVデータ</returns>
        private string MakeCsvDataInfo(DataTable dt)
        {
            // sql文対象の作成
            StringBuilder strCsv = new StringBuilder();

            //区分マスタデータを取得する
            GetKbnLIst();

            // タイトルの追加
            strCsv.Append(this.GetCondolenceCsvTilie());
            int loopCnt = 0;
            string inStr = string.Empty;
            //DataRowsに格納してからデータを取得する⑤
            foreach (DataRow row in dt.Rows)
            {
                // 改行の追加            
                if (loopCnt == 0)
                {
                    strCsv.Append(Environment.NewLine);
                    inStr = string.Empty;
                }

                for (int i = 0; i < row.ItemArray.Length; i++)
                {                 
                    if (i<8)
                    {
                        if (loopCnt == 0)
                        {
                            // コンマの追加
                            if (i > 0)
                            {
                                strCsv.Append(",");
                            }
                            inStr = getKbnName(row.Table.Columns[i].ColumnName, row[i].ToString());
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // コンマの追加
                        if (i > 0)
                        {
                            strCsv.Append(",");
                        }
                        if (i == 10)
                        {
                            inStr = ChangeNumToTyoden(row[i].ToString());
                        }
                        else {
                            inStr = row[i].ToString();
                        }
                        
                    }
                    strCsv.Append(string.Format("\"{0}\"", inStr));
                }
                loopCnt++;
                if (loopCnt == 2)
                {
                    loopCnt = 0;
                    
                }
            }

            return strCsv.ToString();
        }
        /// <summary>
        /// CSV出力タイトルの取得
        /// </summary>
        /// <returns>CSV出力タイトル</returns>
        private string GetCondolenceCsvTilie()
        {
            // csvタイトル対象の作成
            StringBuilder csvTitle = new StringBuilder();

            // 項目番号1 : グッドライフ区分名称
            csvTitle.Append("\"グッドライフ区分名称\"");

            // 項目番号2 : 組合区分名称
            csvTitle.Append(",\"組合区分名称\"");

            // 項目番号3 : 従業員区分名称
            csvTitle.Append(",\"従業員区分名称\"");

            // 項目番号4 : 出向先区分名称
            csvTitle.Append(",\"出向先区分名称\"");

            // 項目番号5 : 続柄区分名称
            csvTitle.Append(",\"続柄区分名称\"");

            // 項目番号6 : 喪主区分名称
            csvTitle.Append(",\"喪主区分名称\"");

            // 項目番号7 : 同居区分名称
            csvTitle.Append(",\"同居区分名称\"");

            // 項目番号8 : 税扶養区分
            csvTitle.Append(",\"税扶養区分\"");

            // 項目番号9 : 出向元会社_香料
            csvTitle.Append(",\"出向元会社_香料\"");

            // 項目番号10 : 出向元会社_供花数
            csvTitle.Append(",\"出向元会社_供花数\"");

            // 項目番号11 : 出向元会社_弔電
            csvTitle.Append(",\"出向元会社_弔電\"");

            // 項目番号12 : 組合_香料
            csvTitle.Append(",\"組合_香料\"");

            // 項目番号13 : 組合_供花数
            csvTitle.Append(",\"組合_供花数\"");

            // 項目番号14 : 組合_弔電
            csvTitle.Append(",\"組合_弔電\"");

            // 項目番号15 : 出向先会社_香料
          //  csvTitle.Append(",\"出向先会社_香料\"");

            // 項目番号16 : 出向先会社_供花数
          //  csvTitle.Append(",\"出向先会社_供花数\"");

            // 項目番号17 : 出向先会社_弔電
         //   csvTitle.Append(",\"出向先会社_弔電\"");

            return csvTitle.ToString();
        }
    }

    public class kbnDat
    {
        public string KBNCODE { get; set; }
        public string KBNVALUE { get; set; }
        public string KBNNAME { get; set; }
    }
}
