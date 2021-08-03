
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
    public class WF_Sinsei : BP.WF.HttpHandler.DirectoryPageBase
    {

        AppFormLogic form = new AppFormLogic();
        /// <summary>
        /// 出張申請の社員情報の取得
        /// </summary>
        /// <returns></returns>
        public string Get_Individual_Info_Wf()
        {
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("select WF.FLOWSTARTER AS SHAINBANGO" +
                    ",Employee.SEI_KANJI" +
                    ",Employee.MEI_KANJI" +
                    ",Employee.SEI_KANA" +
                    ",Employee.MEI_KANA" +
                    ",EmployeeKbn.MEISHO_KANJI AS JUGYOINKBN" +
                    ",Companies.KAISHAMEI AS KAISHANAME" +
                    ",Employee.KAISHACODE AS KAISHACODE" +
                    ",Employee.JINJISHOZOKUCODE AS SHOZOKUCODE" +
                    ",Department.SHOZOKUMEISHORYAKU_KANJI AS SHOZOKUNAME" +
                    ",Position.SHIKAKUMEISHO_KANJI AS SHIKAKUMEISHO" +
                    ",Department.BUSHOCODE AS BUSHOCODE" +
                    ",FinancialDepartment.BUSHOMEI_ZENKAKUKANA AS BUSHOMEI_ZENKAKUKANA" +
                    ",Employee.SHIKAKUCODE AS SHIKAKUCODE" +
                    " FROM  TT_WF AS WF " +
                    " left join MT_Employee  AS Employee on WF.FLOWSTARTER = Employee.SHAINBANGO " +
                    " left join MT_Department  AS Department on (Employee.JINJISHOZOKUCODE = Department.SHOZOKUCODE AND Employee.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Employee.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                    " left join MT_Position  AS Position on ( Position.KAISHACODE = Employee.KAISHACODE AND Position.SHIKAKUCODE = Employee.SHIKAKUCODE AND Position.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Position.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_Companies AS Companies ON (Companies.KAISHACODE= Employee.KAISHACODE AND Companies.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Companies.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_FinancialDepartment AS FinancialDepartment ON (FinancialDepartment.BUSHOCODE =Department.BUSHOCODE AND FinancialDepartment.KAISHACODE = Department.KAISHACODE " +
                    "       AND FinancialDepartment.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND FinancialDepartment.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " left join MT_EmployeeKbn AS EmployeeKbn ON (EmployeeKbn.MEISHOCODE = Employee.JUGYOINKBN AND EmployeeKbn.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND EmployeeKbn.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112)))" +
                    " WHERE OID = '{0}'", this.GetRequestVal("WorkID"));
                dic.Add("Individual_Info", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

        /// <summary>
        /// 出張申請の権限の取得
        /// </summary>
        /// <returns></returns>
        public string Get_Costburdendev_Code()
        {

            string shainbango = this.GetRequestVal("shainbango");
            string userNo = this.GetRequestVal("UserNo");
            Dictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                string sql = string.Format("select Department.BUSHOCODE " +
                    " FROM MT_Employee AS Employee " +
                    " left join MT_Department AS Department ON Department.SHOZOKUCODE = Employee.JINJISHOZOKUCODE " +
                    " AND (Department.TEKIYOYMD_FROM <= (select CONVERT (nvarchar(12),GETDATE(),112)) AND Department.TEKIYOYMD_TO >= (select CONVERT (nvarchar(12),GETDATE(),112))) " +
                    " where SHAINBANGO = '{0}' ", userNo);
                dic.Add("Costburdendev_Code", BP.DA.DBAccess.RunSQLReturnTable(sql));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dic);
        }

    }
}
