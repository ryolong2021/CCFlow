using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using BP.DA;
using System.Data;

namespace BP.WF.HttpHandler
{
    public class WF_LicenseApplicatin: BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 資格情報リスト取得用メソッド
        /// </summary>
        /// <returns></returns>
        public string GetLicenseList()
        {
            try
            {
                // 検索条件の取得
                LicenseListReq cond =
                    JsonConvert.DeserializeObject<LicenseListReq>(
                        this.GetRequestVal("LicenseListReq"));

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("CertificationNo", cond.CertificationNo == null ? "" : cond.CertificationNo);
                dic.Add("CertificationName", cond.CertificationName == null ? "" : "%" + cond.CertificationName + "%");
                List<Dictionary<string, string>> ret = WF_AppForm.GetEbsDataWithApi("Get_Certification_List", dic);

                // フロントに戻ること
                return JsonConvert.SerializeObject(ret);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 資格情報入力必須情報取得用メソッド
        /// </summary>
        /// <returns></returns>
        public string GetCertificationConditions()
        {
            try
            {
                DataTable dt = new DataTable();
                Dictionary<string, Object> dic = new Dictionary<string, Object>();

                // Sql文と条件設定の取得
                StringBuilder sqlSb = new StringBuilder();
                sqlSb.Append("SELECT * ");
                sqlSb.Append("  FROM MT_CERTIFICATION_CONDITIONS ");
                sqlSb.Append(" WHERE (DELETE_FLG <> 'X' OR DELETE_FLG IS NULL)");
                
                Paras ps = new Paras();
                // 入力条件 資格コード
                if (!string.IsNullOrEmpty(this.GetRequestVal("CertificationNo")))
                {
                    sqlSb.Append("   AND CERTIFICATION_NO = @CertificationNo");
                    ps.Add("CertificationNo", this.GetRequestVal("CertificationNo"));

                    // Sqlの実行
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString(), ps);
                }
                else {

                    // Sqlの実行
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString());
                }
                dic.Add("Get_Certification_Conditions", dt);

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dic);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 資格一覧検索条件クラス
        /// </summary>
        private class LicenseListReq
        {
            /// <summary>
            /// 資格コード
            /// </summary>
            public string CertificationNo { get; set; }

            /// <summary>
            /// 資格名称
            /// </summary>
            public string CertificationName { get; set; }
        }
    }
}
