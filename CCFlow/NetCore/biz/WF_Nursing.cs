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
    public class WF_Nursing : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 短時間勤務種別リスト
        /// </summary>
        /// <returns></returns>
        public string Get_Nursing_Part_Time_Type_List()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();

            try
            {
                var kaishacode = this.GetRequestVal("kaishacode");
                string format_sql = @"
                      SELECT MK.KBNVALUE,
                             MK.KBNNAME
                        FROM MT_CORP_PARTTIME MC 
                  INNER JOIN MT_KBN MK 
                          ON MC.PART_TIME_CODE = MK.KBNVALUE
	                   WHERE MK.KBNCODE = 'NURSING_PART_TIME_TYPE'
	                     AND MC.CORP_CODE = '{0}'
                ";

                string sql = string.Format(format_sql, kaishacode);

                DataTable result = BP.DA.DBAccess.RunSQLReturnTable(sql);

                dic.Add("Get_Nursing_Part_Time_Type_List", result);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

    }
}
