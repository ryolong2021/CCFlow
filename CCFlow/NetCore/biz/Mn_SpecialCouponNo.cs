using BarcodeLib;
using BP.DA;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BP.WF.HttpHandler
{
    public class Mn_SpecialCouponNo : BP.WF.HttpHandler.DirectoryPageBase
    {
        private static readonly object locker = new object();

        /// <summary>
        /// 特別買物割引証番号の採番処理
        /// </summary>
        /// <returns>特別買物割引証番号</returns>
        public static string GetSpecialCouponNo()
        {
            try
            {
                // 年の下2桁
                string year = DateTime.Now.Year.ToString().Substring(2);
                // Sql文と条件設定の取得
                string sql = "SELECT MAX(SPEC_DISCOUNT_COUPON_NO) AS NO FROM TT_WF_SPEC_COUPON_APPLY WHERE SPEC_DISCOUNT_COUPON_NO LIKE @Year ";

                Paras ps = new Paras();
                // 条件設定
                ps.Add("Year", year + "%");
                // Sqlの実行
                string no = BP.DA.DBAccess.RunSQLReturnString(sql, ps);

                if (String.IsNullOrEmpty(no))
                {
                    no = year + "0001";
                }
                else
                {
                    no = (Int32.Parse(no) + 1).ToString();
                }

                return BP.Tools.Json.ToJson(no);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 特別買物割引証番号の保存処理
        /// </summary>
        /// <param name="workId">伝票番号</param>
        /// <returns>特別買物割引証番号</returns>
        public static string SaveSpecialCouponNo(string workId) 
        {
            lock (locker)
            {
                try
                {
                    // 特別買物割引証番号
                    string specialCouponNo = GetSpecialCouponNo();
                    // Sql文と条件設定 特別買物割引証番号を更新する
                    string sql = string.Format(@"UPDATE TT_WF_SPEC_COUPON_APPLY SET SPEC_DISCOUNT_COUPON_NO = '{0}' WHERE OID ='{1}'"
                                        , specialCouponNo
                                        , workId);
                    // Sqlの実行
                    BP.DA.DBAccess.RunSQLReturnTable(sql);

                    return specialCouponNo;
                }
                catch (Exception ex)
                {
                    return "err@" + ex.Message;
                }
            }
        }

        /// <summary>
        /// 人事部長氏名の取得処理(PDF用)
        /// </summary>
        /// <returns>人事部長氏名</returns>
        public static string GetDirectorName(string shainbango)
        {
            try
            {
                // パラメータ設定
                Dictionary<string, object> apiParam = new Dictionary<string, object>();
                apiParam.Add("ShainBango", shainbango);

                // グループコード取得
                List<Dictionary<string, string>> rel = WF_AppForm.GetEbsDataWithApi("Get_KaishaCode", apiParam);

                if (rel.Count == 0)
                {
                    return BP.Tools.Json.ToJson(rel);
                }

                string kaishacode = string.Empty;
                foreach (Dictionary<string, string> row in rel)
                {
                    kaishacode = row["KAISHACODE"];
                }

                // Sql文と条件設定の取得
                string sql = string.Format(@"SELECT ROLE_PERSON_NAME FROM MT_ROLE_PERSON WHERE GROUP_KBN = 'G2001' AND CORP_CODE = '{0}'"
                                        , kaishacode);

                // Sqlの実行
                string name = BP.DA.DBAccess.RunSQLReturnString(sql);
                return BP.Tools.Json.ToJson(name);

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 購買店舗注意文の取得処理(PDF用)
        /// </summary>
        /// <returns>購買店舗注意文</returns>
        public static string GetBuyStoreWarning(string shainbango)
        {
            try
            {
                // パラメータ設定
                Dictionary<string, object> apiParam = new Dictionary<string, object>();
                apiParam.Add("ShainBango", shainbango);

                // グループコード取得
                List<Dictionary<string, string>> rel = WF_AppForm.GetEbsDataWithApi("Get_KaishaCode", apiParam);

                if (rel.Count == 0)
                {
                    return BP.Tools.Json.ToJson(rel);
                }

                string kaishacode = string.Empty;
                foreach (Dictionary<string, string> row in rel)
                {
                    kaishacode = row["KAISHACODE"];
                }

                // Sql文と条件設定の取得
                string sql = string.Format(@"SELECT KBNNAME FROM MT_KBN WHERE KBNCODE = 'BUY_STORE_WARNING' AND KBNVALUE = '{0}'"
                                        , kaishacode);

                // Sqlの実行
                string buyStoreWarning = BP.DA.DBAccess.RunSQLReturnString(sql);
                return BP.Tools.Json.ToJson(buyStoreWarning);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// バーコードの生成処理(PDF用)
        /// </summary>
        /// <param name="specialCouponNo">特別買物割引証番号</param>
        public static void GetBarcode(string specialCouponNo) 
        {
            var barcode = new Barcode();
            // 番号表示　可
            barcode.IncludeLabel = true;
            // 表示ポゼッション　中央
            barcode.Alignment = AlignmentPositions.CENTER;
            // 長さ
            barcode.Width = 120;
            // 高さ
            barcode.Height = 45;
            // イメージ属性
            barcode.RotateFlipType = RotateFlipType.RotateNoneFlipNone;
            barcode.BackColor = Color.White;
            barcode.ForeColor = Color.Black;
            // バーコード種類
            barcode.Encode(TYPE.CODE128, specialCouponNo);
            // ファイル属性
            barcode.SaveImage("wwwroot/resource/barcodes/" + specialCouponNo + ".png", SaveTypes.PNG);
        }
    }
}
