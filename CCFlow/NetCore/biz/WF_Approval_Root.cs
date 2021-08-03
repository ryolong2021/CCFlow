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
    public class WF_Approval_Root : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 承認フロールート情報取得
        /// </summary>
        /// <returns></returns>
        public string GetApprovalRootInfo()
        {
            DataTable dtCopy = new DataTable();

            try
            {
                DataTable dt = BP.WF.Dev2Interface.DB_GenerTrackTable(this.GetRequestVal("FK_Flow"),
                                                                long.Parse(this.GetRequestVal("WorkID")),
                                                                long.Parse(this.GetRequestVal("FID")));
                DataView dv = dt.DefaultView;
                dv.Sort = "NDFrom";
                dtCopy = dv.ToTable();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dtCopy);
        }

    }
}
