using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.NetPlatformImpl
{
    public class Cloud_Glo
    {
        /// <summary>
        /// 获得Soap
        /// </summary>
        /// <returns></returns>
        public static BP.WF.CloudWS.WSSoapClient GetSoap()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }
    }
}

namespace BP.WF.CloudWS
{
    public partial class WSSoapClient
    {
        public bool GetNetState()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool AddPriFormDir(string userNo, string pwd, string guid, string parentNo, string dirName)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool DeletePriFormDir(string userNo, string pwd, string guid, string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool EditPriFormDir(string userNo, string pwd, string guid, string no, string dirName)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetFormPagingData(bool isPublic, string searchText, int pageIdx, int pageSize, string dirStr, string userNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public int GetFormTotalCount(bool isPublic, string searchText, string dirStr, string userNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable PriFormDir(string userNo, string pwd, string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetRecentlyPriFormTemp(string userNo, string pwd, string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public string SavePubFormToPri(string userNo, string pwd, string guid, string dir, string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetPubFormTree()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetRecentlyFormTemp()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public object[] GetFormData(bool isPublic, string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public string SavePubToPri(string userNo, string pwd, string guid, string dir, string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public string PriLogin(string userNo, string password)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public string IsExitUser(string userNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public string PriRegUser(string userNo, string userName, string password, string email, string tel, string qq, int userType, int userTarget)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable PriFlowDir(string userNo, string pwd, string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool AddPriFlowDir(string userNo, string pwd, string guid, string parentNo, string dirName)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool DeletePriFlowDir(string userNo, string pwd, string guid, string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool EditPriFlowDir(string userNo, string pwd, string guid, string no, string dirName)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetRecentlyPriFlowTemp(string userNo, string pwd, string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public bool SavePriFlowTemplete(string dir, string flowName, byte[] bytes, string userNo, string nodeNames, int nodesCount)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetPriFlowTemplateByGuid(string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetPubFlowTree()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetDataByNo(string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetSonDirByParentNo(string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetRecentlyFlowTemp()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public int GetTotalCount(bool isPublic, string searchText, string dirStr, string userNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetPagingData(bool isPublic, string searchText, int pageIdx, int pageSize, string dirStr, string userNo)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetFlowTemplateByGuid(string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public byte[] GetFlowXML(bool isPublic, string guid)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetCloudFlowsDDlTreeDt()
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }


        public bool SaveToFlowTemplete(string no, string flowName, byte[] bytes, string sharer, string nodeNames, int nodesCount)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public System.Data.DataTable GetFlowTemFromCloud(string str)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }

        public byte[] GetImageBytesByNo(string no)
        {
            throw new NotImplementedException(".net coreバージョンがサポートされていない");
        }
    }
}
