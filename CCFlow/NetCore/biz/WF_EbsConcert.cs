using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BP.DA;
using BP.Sys;
using Newtonsoft.Json;

namespace BP.WF.HttpHandler
{
    public class WF_EbsConcert : BP.WF.HttpHandler.DirectoryPageBase
    {
        //接続子
        public string strConnection = SystemConfig.AppCenterDSN;
        //APP共通
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// EBS連携。
        /// </summary>
        /// <param name="dic">API連携パラメータ</param>
        /// <return>実行結果</return>
        public string ebs_Concert()
        {
            //API連携関数を設定
            return Set_CM_Touroku_Data(JsonConvert.DeserializeObject<Dictionary<string, string>>(this.GetRequestVal("API_PRM")));
        }

        /// <summary>
        /// API連携関数。
        /// </summary>
        /// <param name="dic">API連携パラメータ</param>
        /// <return>インサート成功件数</return>
        public string Set_CM_Touroku_Data(Dictionary<string, string> dic)
        {
            string ret = string.Empty;
            Dictionary<string, string> dicOut = new Dictionary<string, string>();
            try
            {
                //届出種別取得
                string todokede_type = dic["TODOKEDE_TYPE"];
                //データタイプ取得
                string data_type = dic["DATA_TYPE"];
                //データ名称
                string data_meisyou = dic["DATA_MEISYOU"];
                //処理日付
                string strDate = string.Format("{0:yyyyMMdd}", DateTime.Now.AddDays(1));
                //テンプレートデータ取得
                DataTable dtTmp = getKbnValueName(data_type);
                //MAXテンプレート
                int MAXCNT = 100;

                //パラメータデータチエック
                if (dtTmp.Rows.Count != dic.Count - 8)
                {
                    return "入力テンプレートのパラメータが不正です。";
                }

                //登録番号
                //string torokuBango = "CCF" + todokede_type + "-" + strDate + "-" + getMaxTorokuEdaban("CCF" + todokede_type, dic["JUGYOUIN_BANGO"]);
                string torokuBango = "CCF" + todokede_type + "-" + strDate + "-" + "0001";
                dicOut.Add("TOUROKU_BANGO", torokuBango);
                //パスワード
                string password = "CCF" + todokede_type + "-" + strDate;
                dicOut.Add("PASSWORD", password);
                //処理名
                string syori_mei = getKbnName("TODOKEDE_TYPE", todokede_type);
                dicOut.Add("SYORI_MEI", "CCF" + syori_mei);
                //データタイプ
                dicOut.Add("DATA_TYPE", getKbnINfoName("DATA_TYPE_INFO", data_type));
                //データ名称
                dicOut.Add("DATA_MEISYOU", data_meisyou);
                //行番号
                dicOut.Add("GYOU_BANGOU", getMaxGyouBangou(torokuBango, password));
                //ロールバックユニット
                dicOut.Add("ROLLBACK_UNIT", "");
                //更新モード
                dicOut.Add("KOUSIN_MODE", dic["KOUSIN_MODE"]);
                //有効日自
                dicOut.Add("EFFECTIVE_START_DATE", stringToMMMDDYYYY(dic["EFFECTIVE_START_DATE"]));
                //有効日至
                dicOut.Add("EFFECTIVE_END_DATE", stringToMMMDDYYYY(dic["EFFECTIVE_END_DATE"]));
                //従業員番号
                dicOut.Add("JUGYOUIN_BANGO", dic["JUGYOUIN_BANGO"]);
                //データ項目、データ値（1～１００）設定
                for (int i = 0; i < MAXCNT; i++)
                {
                    string koumoku = string.Empty;
                    string datChi = string.Empty;
                    if (i < dtTmp.Rows.Count)
                    {
                        koumoku = dtTmp.Rows[i]["KBNNAME"].ToString();
                        datChi = dic[dtTmp.Rows[i]["KBNVALUE"].ToString()];
                    }

                    dicOut.Add("DATA_KOUMOKU" + (i + 1).ToString(), koumoku);
                    dicOut.Add("DATA_CHI" + (i + 1).ToString(), datChi);

                }
                //登録EBSユーザー名
                dicOut.Add("USER_NAME", "EBS_ADMIN");
                //下記項目入力不要
                //アップロード結果
                dicOut.Add("UPLOAD_KEKKA", "");
                //アップロードメッセージ
                dicOut.Add("UPLOAD_MESSAGE", "");
                //アップロード年月日
                dicOut.Add("UPLOAD_NENGAPPI", "");
                //登録可能フラグ
                dicOut.Add("TOUROKU_KANOU_FLG", "");
                //スレッド番号
                dicOut.Add("THREAD_BANGO", "");
                //登録結果
                dicOut.Add("TOUROKU_KEKKA", "");
                //登録メッセージ
                dicOut.Add("TOUROKU_MESSAGE", "");
                //登録済みフラグ
                dicOut.Add("TOUROKU_ZUMI_FLG", "");
                //登録年月日
                dicOut.Add("TOUROKU_NENGAPPI", "");
                //パーティションキー
                dicOut.Add("PARTITION_KEY", "");
                //OID設定
                dicOut.Add("OID", dic["OID"]);
                //データタイプ設定
                dicOut.Add("DATA_TYPE_CODE", data_type);

                // 同じ届き、登録したかどうかを判断
                deleteToroku("CCF" + todokede_type, dic["JUGYOUIN_BANGO"], dic["OID"]);
                if (form.Insert("ZHR_CM_TOUROKU_DATA_WKTB", dicOut) == 1)
                {
                    ret = syori_mei + "のAPI連携が成功しました。";

                }
                else
                {
                    ret = syori_mei + "のAPI連携が失敗しました。";
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 時間タイプからMMDDYYYY変換
        /// </summary>
        /// <param name="date"></param>
        /// <return>最大行番号＋１</return>
        private string stringToMMMDDYYYY(string date)
        {
            string ret = string.Empty;

            if (string.IsNullOrEmpty(date))
            {
                return ret;
            }

            ret = date.Substring(0, 4) + "/" + date.Substring(5, 2) + "/" + date.Substring(8, 2);

            return ret;
        }
        /// <summary>
        /// 従業員登録判断
        /// </summary>
        /// <return>最大行番号＋１</return>
        private void deleteToroku(string dataType, string jugyouinBango, string oid)
        {

            Paras ps = new Paras();
            ps.Add("DATA_TYPE", dataType);
            ps.Add("JUGYOUIN_BANGO", jugyouinBango);
            ps.Add("OID", oid);
            // 登録番号の枝番
            string sql = @"select MAX(right(TOUROKU_BANGO,4)) AS EDABAN from ZHR_CM_TOUROKU_DATA_WKTB where left(TOUROKU_BANGO,6) = @DATA_TYPE AND JUGYOUIN_BANGO = @JUGYOUIN_BANGO AND OID<>@OID";
            DataTable dt = Get_DataTable(sql, ps, strConnection);
            if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                sql = string.Format(@" where left(TOUROKU_BANGO,6) = '{0}' AND JUGYOUIN_BANGO = '{1}' AND OID <> {2}", dataType, jugyouinBango, oid);

                form.DeleteWhere("ZHR_CM_TOUROKU_DATA_WKTB", sql);
            }
        }
        /// <summary>
        /// 登録番号の枝番を取得
        /// </summary>
        /// <return>最大行番号＋１</return>
        private string getMaxTorokuEdaban(string dataType, string jugyouinBango)
        {
            string ret = "0001";

            Paras ps = new Paras();
            ps.Add("DATA_TYPE", dataType);
            ps.Add("JUGYOUIN_BANGO", jugyouinBango);
            // 登録番号の枝番
            string sql = @"select MAX(right(TOUROKU_BANGO,4)) AS EDABAN from ZHR_CM_TOUROKU_DATA_WKTB where left(TOUROKU_BANGO,6) = @DATA_TYPE AND JUGYOUIN_BANGO = @JUGYOUIN_BANGO";
            DataTable dt = Get_DataTable(sql, ps, strConnection);
            if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                ret = (int.Parse(dt.Rows[0][0].ToString()) + 1).ToString().PadLeft(4, '0');
            }
            //else 
            //{
            //    // 同じ届き採番しない
            //    sql = @"select MAX(right(TOUROKU_BANGO,4)) AS EDABAN from ZHR_CM_TOUROKU_DATA_WKTB where left(TOUROKU_BANGO,6) = @DATA_TYPE AND OID <> @OID";
            //    dt = Get_DataTable(sql, ps, strConnection);
            //    if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            //    {
            //        ret = dt.Rows[0][0].ToString();
            //    }
            //}

            return ret;
        }

        /// <summary>
        /// 区分マスターからVALUE、NAMEを取得
        /// </summary>
        /// <param name="typekbn">タイプ区分</param>
        /// <return>データ</return>
        private DataTable getKbnValueName(string typekbn)
        {
            Paras ps = new Paras();
            ps.Add("DATA_TYPE_KBN", typekbn);

            string sql = @"SELECT KBNVALUE,KBNNAME FROM MT_API_KBN WHERE DATA_TYPE_KBN = @DATA_TYPE_KBN  ORDER BY KBNJYUNNBANN";
            DataTable dt = Get_DataTable(sql, ps, strConnection);

            return dt;
        }
        /// <summary>
        /// 区分マスターからNAMEを取得
        /// </summary>
        /// <param name="code">コード</param>
        /// <param name="value">VALUE</param>
        /// <return>NAME</return>
        private string getKbnName(string code, string value)
        {
            string ret = string.Empty;

            Paras ps = new Paras();
            ps.Add("KBNCODE", code);
            ps.Add("KBNVALUE", value);

            string sql = @"SELECT KBNNAME FROM MT_API_KBN WHERE KBNCODE = @KBNCODE AND KBNVALUE = @KBNVALUE";
            DataTable dt = Get_DataTable(sql, ps, strConnection);
            if (dt.Rows.Count > 0)
            {
                ret = dt.Rows[0]["KBNNAME"].ToString();
            }
            return ret;
        }
        /// <summary>
        /// 区分マスターからNAMEを取得
        /// </summary>
        /// <param name="code">コード</param>
        /// <param name="value">VALUE</param>
        /// <return>NAME</return>
        private string getKbnINfoName(string code, string value)
        {
            string ret = string.Empty;

            Paras ps = new Paras();
            ps.Add("KBNCODE", code);

            string sql = @"SELECT KBNVALUE,KBNNAME FROM MT_API_KBN WHERE KBNCODE = @KBNCODE";
            DataTable dt = Get_DataTable(sql, ps, strConnection);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["KBNVALUE"].ToString().Contains(value))
                {
                    ret = dt.Rows[i]["KBNNAME"].ToString();
                }
            }
            return ret;
        }
        /// <summary>
        /// 最大行番号を取得
        /// </summary>
        /// <param name="bango">登録番号</param>
        /// <param name="password">パスワード</param>
        /// <return>最大行番号＋１</return>
        private string getMaxGyouBangou(string bango, string password)
        {
            string ret = "1";

            Paras ps = new Paras();
            ps.Add("TOUROKU_BANGO", bango);
            ps.Add("PASSWORD", password);

            string sql = @"SELECT MAX(GYOU_BANGOU) FROM ZHR_CM_TOUROKU_DATA_WKTB WHERE TOUROKU_BANGO = @TOUROKU_BANGO AND PASSWORD = @PASSWORD";
            DataTable dt = Get_DataTable(sql, ps, strConnection);
            if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                ret = (int.Parse(dt.Rows[0][0].ToString()) + 1).ToString();
            }

            return ret;
        }
        /// <summary>
        /// データ取得処理
        /// </summary>
        /// <param name="sql">実行SQL文</param>
        /// <param name="paras">パラメータ</param>
        /// <param name="connection">接続子</param>
        /// <return>データテーブル</return>
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
    }
}
