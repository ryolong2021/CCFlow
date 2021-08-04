using BP.Sys;
using BP.WF.HttpHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.NetPlatformImpl
{
    public class WF_Admin_FoolFormDesigner
    {
        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public static List<WSMethod> GetWebServiceMethods(SFDBSrc dbsrc)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }
        public class WSMethod
        {
            public string No { get; set; }

            public string Name { get; set; }

            public Dictionary<string, string> ParaMS { get; set; }

            public string Return { get; set; }
        }

    }
}
