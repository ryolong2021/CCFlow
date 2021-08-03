using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BP.DA;
using BP.Sys;
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
    public class WF_Condolence : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 弔事名義を取得
        /// </summary>
        /// <returns></returns>
        public void Get_Condolence_Meigi_Info(DataRow dr)
        {
            //パラメタ格納
            Dictionary<string, Object> dicret = new Dictionary<string, Object>
            {
                { "KAISHACODE", dr["KAISHACODE"] },
                { "CHOJIMEIGIKBN", dr["CHOJIMEIGIKBN"] }
            };
            //APIから返却結果を格納
            List<Dictionary<string, string>> listDic = new List<Dictionary<string, string>>();
            //APIでEBSからデータを取得
            listDic = WF_AppForm.GetEbsDataWithApi("Get_New_Condolence_Meigi_Info", dicret);

            string kaishamei = listDic[0]["KAISHAMEI"];
            string yakushoku = listDic[0]["YAKUSHOKU"];
            string shimei = listDic[0]["SHIMEI"];
            if (String.IsNullOrEmpty(kaishamei))
            {
                kaishamei = String.Empty;
            }
            if (String.IsNullOrEmpty(yakushoku))
            {
                yakushoku = String.Empty;
            }
            if (String.IsNullOrEmpty(shimei))
            {
                shimei = String.Empty;
            }
            dr["KAISHAMEI"] = kaishamei;
            dr["YAKUSHOKU"] = yakushoku;
            dr["SHIMEI"] = shimei;

        }

        /// <summary>
        /// 弔事基準を取得
        /// </summary>
        /// <returns></returns>
        public string Get_Condolence_Standard_Info()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();

            try
            {
                // 出向先区分
                const string SYUKOU_KBN_YES = "1";
                const string SYUKOU_KBN_NO = "0";

                // 出向元情報
                string format_syukou_0 = @"
                SELECT STA.KAISHACODE AS KAISHACODE
		              ,'0' AS CHOJIMEIGIKBN
                      ,STA.SHUKKOMOTO_KORYO AS KORYONUM
	                  ,STA.SHUKKOMOTO_KYOKASU AS KYOKANUM
	                  ,STA.SHUKKOMOTO_CHODEN AS TYODENNUM
      	              ,'' AS KAISHAMEI
	                  ,'' AS YAKUSHOKU
	                  ,'' AS SHIMEI
                  FROM MT_MN_CONDOLENCE_STANDARD AS STA
                 WHERE STA.KAISHACODE = '{0}'
                   AND STA.GOODLIFEKBN = '{1}'
	               AND STA.KUMIAIKBN = '{2}'
	               AND STA.SHUKKOSAKIKBN = '{4}'
	               AND STA.JUGYOINKBN = '{3}'
	               AND STA.ZOKUGARAKBN = '{5}'
	               AND STA.MOSHUKBN = '{6}'
	               AND STA.DOKYOKBN = '{7}'
	               AND STA.ZEIFUYOKBN = '{8}'
	               AND STA.TEKIYOYMD_FROM  <= (select CONVERT (nvarchar(12),GETDATE(),112)) 
	               AND STA.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))
";
                // 組合情報
                string format_syukou_1 = @"
                SELECT STA.KAISHACODE AS KAISHACODE
		                  ,CASE WHEN STA.KUMIAIKBN = '1' OR STA.KUMIAIKBN = '6' THEN '1' ELSE '2' END AS CHOJIMEIGIKBN
                          ,STA.SHUKKOMOTO_KORYO AS KORYONUM
	                      ,STA.SHUKKOMOTO_KYOKASU AS KYOKANUM
	                      ,STA.SHUKKOMOTO_CHODEN AS TYODENNUM
      	                  ,'' AS KAISHAMEI
	                      ,'' AS YAKUSHOKU
	                      ,'' AS SHIMEI
                      FROM MT_MN_CONDOLENCE_STANDARD AS STA
                     WHERE STA.KAISHACODE = '{0}'
                       AND STA.GOODLIFEKBN = '{1}'
	                   AND STA.KUMIAIKBN = '{2}'
	                   AND STA.SHUKKOSAKIKBN = '{4}'
	                   AND STA.JUGYOINKBN = '{3}'
	                   AND STA.ZOKUGARAKBN = '{5}'
	                   AND STA.MOSHUKBN = '{6}'
	                   AND STA.DOKYOKBN = '{7}'
	                   AND STA.ZEIFUYOKBN = '{8}'
	                   AND STA.TEKIYOYMD_FROM  <= (select CONVERT (nvarchar(12),GETDATE(),112)) 
	                   AND STA.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))
";
                //出向区分
                string syukou_kbn = this.GetRequestVal("syukou_kbn");
                //会社コード
                string kaishacode = this.GetRequestVal("kaishacode");

                DataTable dt0 = Get_Condolence_Standard_Info_DT(format_syukou_0, SYUKOU_KBN_NO, kaishacode);

                DataTable dt1 = Get_Condolence_Standard_Info_DT(format_syukou_1, SYUKOU_KBN_NO, kaishacode);

                DataTable dt = dt0.Clone();

                foreach (DataRow dr in dt0.Rows)
                {
                    if (!(dr["KORYONUM"].ToString() == "0" && dr["KYOKANUM"].ToString() == "0" && dr["TYODENNUM"].ToString() == "0"))
                    {
                        dr["KAISHACODE"] = this.GetRequestVal("kaishacode");

                        //Get_Condolence_Meigi_Info(dr);

                        dt.Merge(dt0);
                    }
                }

                foreach (DataRow dr in dt1.Rows)
                {
                    if (!(dr["KORYONUM"].ToString() == "0" && dr["KYOKANUM"].ToString() == "0" && dr["TYODENNUM"].ToString() == "0"))
                    {

                        dr["KAISHACODE"] = this.GetRequestVal("kaishacode");

                        //Get_Condolence_Meigi_Info(dr);

                        dt.Merge(dt1);
                    }
                }

                // 出向しているの場合
                if (syukou_kbn == SYUKOU_KBN_YES)
                {
                    kaishacode = this.GetRequestVal("shukokaishacode");

                    DataTable dt2 = Get_Condolence_Standard_Info_DT(format_syukou_0, SYUKOU_KBN_YES, kaishacode);
                    foreach (DataRow dr in dt2.Rows)
                    {
                        if (!(dr["KORYONUM"].ToString() == "0" && dr["KYOKANUM"].ToString() == "0" && dr["TYODENNUM"].ToString() == "0"))
                        {
                            dr["KAISHACODE"] = this.GetRequestVal("shukokaishacode");

                            //Get_Condolence_Meigi_Info(dr);

                            dt.Merge(dt2);
                        }
                    }
                }
                dic.Add("Get_Condolence_Standard_Info", dt);

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        /// 給付内容DTを取得する
        /// </summary>
        /// <returns></returns>
        public DataTable Get_Condolence_Standard_Info_DT(string format, String syukou_kbn, string kaishacode)
        {
            // グッドライフ区分
            string glc_kbn = this.GetRequestVal("glc_kbn");
            // 組合区分
            string kumiai_kbn = this.GetRequestVal("kumiai_kbn");
            // 従業員区分
            string jyuugyouinn_kbn = this.GetRequestVal("jyuugyouinn_kbn");
            // 亡くなられた方の続柄
            string dead_kbn = this.GetRequestVal("dead_kbn");
            // 喪主区分
            string organizer_kbn = this.GetRequestVal("organizer_kbn");
            // 同居区分
            string dokyo_bekyo_kbn = this.GetRequestVal("dokyo_bekyo_kbn");
            // 税扶養区分
            string fuyou_kbn = this.GetRequestVal("fuyou_kbn");

            //会社コード変更
            if (isBSdepartment(glc_kbn, kumiai_kbn))
            {
                kaishacode = "ALL";
            }

            string sql = string.Format(format, kaishacode, glc_kbn, kumiai_kbn, jyuugyouinn_kbn, syukou_kbn, dead_kbn, organizer_kbn, dokyo_bekyo_kbn, fuyou_kbn);

            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// グッドライフ区分と組合区分により、BS関連判断
        /// </summary>
        /// <returns></returns>
        private bool isBSdepartment(string glc_kbn, string kumiai_kbn)
        {

            if (glc_kbn == "1" || glc_kbn == "3" || glc_kbn == "5" || glc_kbn == "7" || glc_kbn == "A" || glc_kbn == "B" || glc_kbn == "C" || glc_kbn == "D")
            {
                if(kumiai_kbn == "0" || kumiai_kbn == "1" || kumiai_kbn == "5" || kumiai_kbn == "6")
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// 重複申請チェック
        /// </summary>
        /// <returns></returns>
        public string Check_Double_Info()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();

            try
            {
                // ①不幸従業員社員番号
                var shainbango = this.GetRequestVal("shainbango");
                // ④亡くなられた方のカナ姓
                var kanashimeisei = this.GetRequestVal("kanashimeisei");
                // ④亡くなられた方のカナ名
                var kanashimeimei = this.GetRequestVal("kanashimeimei");
                // ③亡くなられた方の漢字姓
                var shimeisei = this.GetRequestVal("shimeisei");
                // ③亡くなられた方の漢字名
                var shimeimei = this.GetRequestVal("shimeimei");
                // ②亡くなられた方との続柄区分
                var zokugarakbn = this.GetRequestVal("zokugarakbn");
                // ⑥亡くなられた方の性別
                var seibetsu = this.GetRequestVal("seibetsu");
                // ⑤亡くなられた方の同居/別居区分
                var dokyobekyo = this.GetRequestVal("dokyobekyo");
                // ⑦亡くなられた方の逝去日
                var seikyobi = this.GetRequestVal("seikyobi");
                // ⑦亡くなられた方の逝去時刻
                var seikyojikoku = this.GetRequestVal("seikyojikoku");
                // OID
                var oid = this.GetRequestVal("WorkID");
                string check_double_sql = @"
                    SELECT COUNT(*)
                      FROM TT_WF_CONDOLENCE 
                     WHERE UNFORTUNATE_SHAINBANGO = '{0}'
                       AND DEAD_KANASHIMEI_SEI = '{1}'
                       AND DEAD_KANASHIMEI_MEI = '{2}'
                       AND DEAD_SHIMEI_SEI = '{3}'
                       AND DEAD_SHIMEI_MEI = '{4}'
                       AND DEAD_JUGYOIN_ZOKUGARAKBN = {5}
                       AND DEAD_SEIBETSU = {6}
                       AND DEAD_DOKYO_BEKYO = {7}
                       AND DEAD_DATE = '{8}'
                       AND DEAD_TIME = '{9}'
                       AND OID != {10}
                       AND WFState = 3
                ";

                string sql = string.Format(check_double_sql, shainbango, kanashimeisei, kanashimeimei, shimeisei, shimeimei, zokugarakbn, seibetsu, dokyobekyo, seikyobi, seikyojikoku, oid);

                string result = BP.DA.DBAccess.RunSQLReturnString(sql);

                dic.Add("Check_Double_Info", result);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        /// AzureからPDFを取得
        /// </summary>
        /// <returns></returns>
        public string Get_PDF_Info()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();

            string kaishacode = this.GetRequestVal("kaishacode");

            // AzureStorage接続情報の取得
            string connectionString = SystemConfig.AppSettings["AzureStorageKey"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // フォルダー名
            string containerName = "pdf";

            // フォルダー取得
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobs = containerClient.GetBlobs();
            // すべてのBlobsをルールする
            foreach (var blob in blobs)
            {
                string sub_names = blob.Name;
                // ファイルが存在する
                if (sub_names.Contains(kaishacode + "/" + kaishacode + ".pdf"))
                {
                    // AzureStorageにファイル名
                    BlobClient blobClient = containerClient.GetBlobClient(kaishacode + "/" + kaishacode + ".pdf");

                    dic.Add("Get_PDF_Info", blobClient.Uri.AbsoluteUri);

                    return BP.Tools.Json.ToJson(dic);
                }
            }

            dic.Add("Get_PDF_Info", String.Empty);
            return BP.Tools.Json.ToJson(dic);
        }

    }

}
