using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.Sys;
using Newtonsoft.Json;

namespace BP.WF.HttpHandler
{
    public class APIBase : BP.WF.HttpHandler.DirectoryPageBase
    {
        const string strConnection = "Password=ccflow;Persist Security Info=True;User ID=sa;Initial Catalog=API;Data Source=localhost;Timeout=999;MultipleActiveResultSets=true";
        //string strConnection = SystemConfig.AppCenterDSN;
        const string sqlAPI = @"
SELECT kbn.KBNNAME AS CONNECT
      ,SELECT_SQL 
FROM API_INFO_MAST AS info
left join MT_KBN AS kbn on (kbn.KBNVALUE =  info.CONNECT AND kbn.KBNCODE = 'APP_CENTER_DSN')
WHERE BUSINESS_KBN = '1' 
AND API_Name = @API_Name
";

        public string Get_Info()
        {

            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            //業務区分
            //string strBUSINESS_KBN = this.GetRequestVal("BUSINESS_KBN");
            //API名称
            string strAPIName = this.GetRequestVal("API_Name");
            //パラメタ
            Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal("API_PRM"));

            try
            {
                Paras ps = new Paras();
               // ps.Add("BUSINESS_KBN", strBUSINESS_KBN);
                ps.Add("API_Name", strAPIName);
                DataTable dt = Get_DataTable(sqlAPI, ps, strConnection);

                string strCon = dt.Rows[0]["CONNECT"].ToString();
                string strSelect = dt.Rows[0]["SELECT_SQL"].ToString();
                ps = new Paras();
                foreach (var item in dicTbl)
                {
                    ps.Add(item.Key, item.Value);
                }

                dt = Get_DataTable(strSelect, ps, strCon);
                dic.Add("Get_Info", dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);

        }


        private int RunSQL_Insert(string sql, Paras paras, string connection)
        {
            SqlConnection conn = new SqlConnection(connection);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            try
            {
                var count = DBAccess.RunSQL(sql, conn, strConnection);
                conn.Close();
                return count;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
            }
        }

        private static DataTable Get_DataTable(string sql, Paras paras, string connection)
        {
            SqlConnection conn = new SqlConnection(connection);
            if (conn.State != ConnectionState.Open)
                conn.Open();

            SqlDataAdapter ada = new SqlDataAdapter(sql, conn);
            ada.SelectCommand.CommandType = CommandType.Text;

            // 参数追加
            if (paras != null)
            {
                foreach (Para para in paras)
                {
                    SqlParameter myParameter = new SqlParameter(para.ParaName, para.val);
                    myParameter.Size = para.Size;
                    ada.SelectCommand.Parameters.Add(myParameter);
                }
            }
            try
            {
                DataTable oratb = new DataTable("otb");
                ada.Fill(oratb);
                ada.Dispose();
                conn.Close();
                return oratb;
            }
            catch (Exception ex)
            {
                ada.Dispose();
                conn.Close();
                throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
            }
        }


        /// <summary>
        /// API連携データを登録（CCFLOW側から呼び出し）
        /// </summary>
        /// <returns></returns>
        //public string SET_API_BIZ_Info()
        public string SET_API_BIZ_Info()
        {
            int restlt = 0;
            try
            {
                string tblName = this.GetRequestVal("APIBizTblName");
                string APIName = this.GetRequestVal("API_NAME");
                string webUser = this.GetRequestVal("WEBUSER");
                Dictionary<string, string> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal(tblName));

                //API_NOを取得
                string sqlAPI_No = string.Format(" SELECT COALESCE( MAX(API_NO),0) FROM API_BIZ_MAST_VALUE WHERE API_NAME = @API_Name ");
                Paras ps = new Paras();
                ps.Add("API_Name", APIName);
                DataTable dt = Get_DataTable(sqlAPI_No, ps, strConnection);
                var APINo = Convert.ToInt32(dt.Rows[0][0]) + 1;

                ////区分一覧を取得
                string sqlKBN = string.Format(" SELECT MT_KBN.KBNVALUE FROM API_BIZ_MAST LEFT JOIN MT_KBN ON API_BIZ_MAST.FILE_TYPE = MT_KBN.DATA_TYPE_KBN WHERE API_BIZ_MAST.API_NAME  =  @API_Name AND MT_KBN.HYOUJIFLG = '1' ");
                Paras psKBN = new Paras();
                psKBN.Add("API_Name", APIName);
                DataTable dtKBN = Get_DataTable(sqlKBN, psKBN, strConnection);


                //マスタキー一覧を取得
                string sqlMastKeyList = string.Format("SELECT NAME_EN FROM API_BIZ_MAST_KEY WHERE API_NAME = @API_Name ");
                Paras psMastKeyList = new Paras();
                psMastKeyList.Add("API_Name", APIName);
                dt = Get_DataTable(sqlMastKeyList, psMastKeyList, strConnection);

                //パラメータ正確性チェック
                foreach (KeyValuePair<string, string> item in dicTbl)
                {
                    if (dt.Select(string.Format(" NAME_EN = '{0}' ", item.Key)).Length == 0)
                    {
                        return "err@パラメータ設定エラー、リセットしてください。";
                    }
                }

                //データを作成
                StringBuilder sBuilder = new StringBuilder();
                sBuilder.Append("INSERT INTO API_BIZ_MAST_VALUE(API_NAME,API_NO,NAME_EN,PRM_VAL,REC_ENT_DATE,REC_ENT_USER,REC_EDT_DATE,REC_EDT_USER) values ");

                foreach (DataRow item in dtKBN.Rows)
                {
                    var strNAME_EN = item["KBNVALUE"].ToString();
                    var strPRM_VAL = dicTbl.FirstOrDefault(x => x.Key == strNAME_EN).Value;

                    //APIレコード詳細情報を登録する
                    sBuilder.Append("('" + APIName + "', '");
                    sBuilder.Append(APINo.ToString() + "', '");
                    sBuilder.Append(strNAME_EN + "', '");
                    sBuilder.Append(strPRM_VAL + "', '");
                    sBuilder.Append(DateTime.Now.ToString() + "', '");
                    sBuilder.Append(webUser + "', '");
                    sBuilder.Append(DateTime.Now.ToString() + "', '");
                    sBuilder.Append(webUser + "'),");

                }
                var strSQL = sBuilder.ToString().Substring(0, sBuilder.ToString().LastIndexOf(','));
                restlt = RunSQL_Insert(strSQL, null, strConnection);

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return restlt.ToString();
        }
    }
}
