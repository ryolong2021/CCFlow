using BP.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace BP.NetPlatformImpl
{
    public static class Sys_PubClass
    {
        public static string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = "";
                if (HttpContextHelper.Request != null && HttpContextHelper.Request.QueryString.HasValue)
                    rawUrl = HttpContextHelper.Request.QueryString.Value;
                rawUrl = rawUrl.Substring(1); // 去掉开头的问号?
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
    }
}
