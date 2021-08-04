using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.NetPlatformImpl
{
   public class WF_DynamicWebService
    {
        /// <summary>
        /// 动态调用web服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="classname"></param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object InvokeWebService(string url, string className, string methodName, object[] args)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }
    }
}
