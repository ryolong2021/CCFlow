using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.NetPlatformImpl
{
    public class WF_Glo
    {
        /// <summary>
        /// 得到WebService对象 
        /// </summary>
        /// <returns></returns>
        public static CCInterface.PortalInterfaceSoapClient GetPortalInterfaceSoapClient()
        {
            return new CCInterface.PortalInterfaceSoapClient();
        }
    }
}

namespace CCInterface
{
    public class PortalInterfaceSoapClient
    {
        public bool SendToWebServices(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo, string tag, string title = null, string openUrl = null)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool SendWhen(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool FlowOverBefore(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool SendToDingDing(string mypk, string sender, string sendToEmpNo, string tel, string msgInfo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool SendToWeiXin(string mypk, string sender, string sendToEmpNo, string tel, string msgInfo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool SendToEmail(string mypk, string sender, string sendToEmpNo, string email, string title, string maildoc)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool SendToCCIM(string mypk, string userNo, string msg, string sourceUserNo, string tag)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public void Print(string billFilePath)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool WriteUserSID(string miyue, string userNo, string sid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public int CheckUserNoPassWord(string userNo, string password)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetDept(string deptNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetDepts()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetDeptsByParentNo(string parentDeptNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetStations()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetStation(string stationNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetEmps()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetEmpsByDeptNo(string deptNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetEmp(string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetDeptEmp()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetEmpHisDepts(string empNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetEmpHisStations(string empNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetDeptEmpStations()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GenerEmpsByStations(string stationNos)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GenerEmpsByDepts(string deptNos)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GenerEmpsBySpecDeptAndStats(string deptNo, string stations)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public string SendSuccess(string flowNo, int nodeID, long workid, string userNo, string userName)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }
    }
}

