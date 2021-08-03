using BP.DA;
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
    public class Mn_DivorceBereavement : BP.WF.HttpHandler.DirectoryPageBase
    {
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// 家族情報取得
        /// </summary>
        /// <returns></returns>
        public string Get_Shain_Info()
        {

            string shainbango = this.GetRequestVal("shainbango");
            string mkbn = this.GetRequestVal("mkbn");
            if (string.IsNullOrEmpty(shainbango))
            {
                string sql = string.Format("select FlowStarter from {0} where OID = '{1}'",
                                           this.GetRequestVal("tblName"), this.GetRequestVal("WorkID"));
                shainbango = form.Select(sql).Rows[0][0].ToString();
            }


            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format(@"
select  Employee.SHAINBANGO ,
    Employee.JUGYOINKBN,
    Employee.KAISHACODE AS KAISHACODE,
    Employee.JINJISHOZOKUCODE AS SHOZOKUCODE,
    Employee.SEIREKISEINENYMD AS SEINENYMD,
    Employee.SEIBETSUKBN AS SEIBETSUKBN,
    Employee.SHIKAKUCODE AS SHIKAKUCODE,
    Employee.SHOKUICODE AS SHOKUICODE,
    EBS_EMP.EBS_INSURANCEKBN AS INSURANCEKBN,
    EBS_EMP.EBS_INSURANCEYMD AS INSURANCEYMD,
    EBS_EMP.EBS_JYUMINHYOU_YUBINBANGO AS YUBINBANGO,
    EBS_EMP.EBS_JYUMINHYOU_ADDKANJI1 AS ADDKANJI1,
    EBS_EMP.EBS_JYUMINHYOU_ADDKANJI2 AS ADDKANJI2,
    EBS_EMP.EBS_JYUMINHYOU_ADDKANJI3 AS ADDKANJI3,
    EBS_EMP.EBS_JYUMINHYOU_ADDGANA1 AS ADDGANA1,
    EBS_EMP.EBS_JYUMINHYOU_ADDGANA2 AS ADDGANA2,
    EBS_EMP.EBS_JYUMINHYOU_ADDGANA3 AS ADDGANA3,
    EBS_EMP.EBS_JYUMINHYOU_TEL AS EBSTEL,
    EBS_EMP.EBS_ANNUITYNO AS ANNUITYNO
    ,EBS_EMP.EBS_SHAINBANGO AS EBS_SHAINBANGO
    ,EBS_EMP2.EBS_SHAINBANGO AS EBS_SHAINBANGO2
    ,EBS_EMP.LAST_NAME_KANJI AS SEI_KANJI
    ,EBS_EMP.FIRST_NAME_KANJI AS MEI_KANJI
    ,EBS_EMP.LAST_NAME_KANA AS SEI_KANA
    ,EBS_EMP.FIRST_NAME_KANA AS MEI_KANA
    ,EBS_EMP2.FAMILY_RELATIONSHIP AS FMRELATIONID
    ,EBS_EMP2.FAMILY_LAST_NAME_KANJI AS FMSEIKANJI
    ,EBS_EMP2.FAMILY_FIRST_NAME_KANJI AS FMMEIKANJI
    ,EBS_EMP2.FAMILY_LAST_NAME_KANA AS FMSEIKANA
    ,EBS_EMP2.FAMILY_FIRST_NAME_KANA AS FMMEIKANA
    ,EBS_EMP.MARRIAGE_CLASS AS MARRYID
    ,EBS_EMP.MARRIAGE_DATE AS MARRYDT
    ,EBS_EMP.DIVORCE_DATE AS DIVORCEDT
    ,EBS_EMP2.FAMILY_DAY_OF_DEATH AS DEATHDT
    ,EBS_EMP2.FAMILY_BIRTHDAY AS FMBIRTHDAY
    ,EBS_EMP2.FAMILY_GENDER AS FMSEXID
    ,EBS_EMP2.FAMILY_TAX_DEPENDENT AS FMFUYOID
    ,EBS_EMP2.FAMILY_HEALTH_INSURANCE AS FMKENKOID
    ,EBS_EMP2.FAMILY_DATE_START AS STARTDT
FROM MT_Employee  AS Employee   
left join MT_Department  AS Department on (Employee.JINJISHOZOKUCODE = Department.SHOZOKUCODE 
                                            AND Employee.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) 
                                            AND Employee.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))  
left join MT_EBS_EMPLOYEEINFO AS EBS_EMP on (Employee.SHAINBANGO = EBS_EMP.EBS_SHAINBANGO )
left join MT_EBS_EMPLOYEE_FAMILY_INFO AS EBS_EMP2 on (Employee.SHAINBANGO = EBS_EMP2.EBS_SHAINBANGO AND EBS_EMP2.FAMILY_RELATIONSHIP='{0}')
WHERE Employee.SHAINBANGO = '{1}'
                    ", mkbn, shainbango);
                dic.Add("Get_Shain_Info", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }
    }
}
