using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BP.Sys;
using System;
using System.Data;
using System.Text;

namespace Common.WF_OutLog
{
    public  class WF_OutLog
    {
        //ログ出力モード
        public const string DEBUG_MODE = "debug";
        public const string ERROR_MODE = "error";
        public const string TRACE_MODE = "trace";
        //ログレベル 0:NO Log 1:error,2:debug+error,3:error+debug+trace
        public const string NO_LOG_LEVEL = "0";
        public const string ERROR_LEVEL = "1";
        public const string DEBUG_ERROR_LEVELOG = "2";
        public const string DEBUG_REEOR_TRACE_LEVELOG = "3";

        /// <summary>
        /// CCFLOWのログ共通作成( ログインユーザー、OIDを共通追加)
        /// </summary>
        /// <param name="msg">ログ出力メッセージ</param>
        /// <param name="oid">OID</param>
        /// <param name="userNo">ログインユーザー</param>
        /// <param name="fkflow">実行フロー</param>
        public static string CCFlowCommonLog(string msg, string oid,string userNo,string fkflow)
        {
            string flowName = "CCフローじゃない";
            try
            {
                if (!string.IsNullOrEmpty(fkflow))
                {
                    // フロー名の取得
                    string sql = string.Format("SELECT Name FROM WF_Flow WHERE No = {0}", fkflow);
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        // フロー名の設定
                        flowName = dt.Rows[0]["Name"].ToString();
                    }
                }
            }
            catch (Exception)
            { 
            }
            return msg + " *** FlowName：" + flowName + "、LoginUser：" + userNo + "、OID：" + oid + " ***";
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="msg">ログ出力メッセージ</param>
        /// <param name="mode">ログ出力モード</param>
        public static void WriteLog(string msg, string mode)
        {
            //ログモード判断
            switch (SystemConfig.AppSettings["LogLevel"])
            {
                //ログ出力しない
                case NO_LOG_LEVEL:
                    return;
                //エラーログだけ出力
                case ERROR_LEVEL:
                    if (mode != ERROR_MODE)
                    {
                        return;
                    }
                    break;
                //ディバッグ、エラーログ出力
                case DEBUG_ERROR_LEVELOG:
                    if (mode != ERROR_MODE && mode != DEBUG_MODE)
                    {
                        return;
                    }
                    break;
                //全部ログ出力
                case DEBUG_REEOR_TRACE_LEVELOG:
                    break;
                default:
                    return;
            }
            //メッセージ作成
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now + " --" + mode + "-- " + "メッセージ:" + msg + "\r\n");

            try
            {
                //出力パス作成
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string fullPath = System.AppDomain.CurrentDomain.BaseDirectory + fileName;
                //文字コード設定
                Encoding encoder = Encoding.UTF8;
                byte[] bytes = encoder.GetBytes(sb.ToString());
                //ログファイに書き込む
                using (System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.OpenOrCreate, 
                                                                                    System.IO.FileAccess.Write, 
                                                                                    System.IO.FileShare.ReadWrite))
                {
                    fs.Position = fs.Length;
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();
                }
                //AzureStorage接続
                BlobServiceClient blobServiceClient = new BlobServiceClient(SystemConfig.AppSettings["AzureStorageKey"]);
                // フォルダー取得
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("log");
                // フォルダー存在しない場合、新規作成
                containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob).Wait();
                // AzureStorageにファイル名
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                // 入力データStreamアップロード 
                blobClient.UploadAsync(fullPath, true).Wait();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
