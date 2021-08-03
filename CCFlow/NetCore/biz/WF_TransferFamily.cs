using BP.DA;
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
    public class WF_TransferFamily : BP.WF.HttpHandler.DirectoryPageBase
    {

        public string GetFamilyInfo()
        {
            try
            {
                string shainbango = this.GetRequestVal("UserNo");

                // Sql文と条件設定の取得
                string sql = "SELECT * FROM MT_EBS_EMPLOYEE_FAMILY_INFO WHERE EBS_SHAINBANGO = @SHAINBANGO ORDER BY FAMILY_RELATIONSHIP";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("SHAINBANGO", shainbango);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);
                if (dt.Rows.Count == 0)
                {
                    return "msg@" + BP.Tools.Json.ToJson("家族情報の登録が無いため、家族異動届を申請することができません。");
                }

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
    }
}
