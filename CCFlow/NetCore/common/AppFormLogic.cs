using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BP.En;
using BP.DA;
using BP.Port;
using BP.Sys;
using BP.Web;
using System.Text;
using System.Data;

namespace BP.WF.HttpHandler
{
    public class AppFormLogic
    {

        public AppFormLogic()
        {
        }

        public Dictionary<string, DataTable> GetKbnInfo(string[] KbnCodeList, string[] KbnCodeEntUserList, string strEntUser = "admin")
        {

            string json = string.Empty;
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
            try
            {
                for (int i = 0; i < KbnCodeList.Length; i++)
                {
                    if (KbnCodeEntUserList.Contains(KbnCodeList[i]))
                    {
                        dic.Add(KbnCodeList[i].ToString(), GetKbnName(KbnCodeList[i].ToString(), strEntUser));
                    }
                    else 
                    {
                        dic.Add(KbnCodeList[i].ToString(), GetKbnName(KbnCodeList[i].ToString()));
                    }

                }
            }
            catch (Exception)
            {
            }
            return dic;
        }
        public  DataTable GetKbnName(string strKbnCode, string strEntUser = "admin")
        {
            string json = string.Empty;
            DataTable dt = new DataTable();
            try
            {

                //画面区分取得
                string sql = string.Format("SELECT KBNCODE,KBNVALUE,KBNNAME FROM MT_KBN WHERE KBNCODE = '{0}' AND REC_ENT_USER = '{1}' ORDER BY KBNORDER", strKbnCode, strEntUser);
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return dt;
        }

        public int Delete(string strTblName, string strKey, string strKeyValue)
        {
            int result = -1;
            try
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1}='{2}'", strTblName, strKey, strKeyValue);
                result = BP.DA.DBAccess.RunSQL(sql);
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return result;
        }

        public int DeleteWhere(string strTblName, string sqlWhere)
        {
            int result = -1;
            try
            {
                string sql = string.Format("DELETE FROM {0} {1}", strTblName, sqlWhere);
                result = BP.DA.DBAccess.RunSQL(sql);
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return result;
        }

        public int Update(string strTblName, Dictionary<string, string> dic, string strKey, string strKeyValue)
        {
            int result = -1;
            try
            {
                StringBuilder sb = new StringBuilder();
                Paras ps = new Paras();
                sb.Append("UPDATE " + strTblName + " SET ");
                foreach (var item in dic)
                {
                    sb.Append(item.Key + " = @" + item.Key + " ,");

                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        ps.Add(item.Key, item.Value);
                    }
                    else
                    {
                        ps.AddDBNull(item.Key);
                    }
                }
                sb = sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append(string.Format(" WHERE {0}='{1}'", strKey, strKeyValue));
                result = BP.DA.DBAccess.RunSQL(sb.ToString(), ps);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public int UpdateForKeys(string strTblName, Dictionary<string, string> dic, string strKeyInfo)
        {
            int result = -1;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE " + strTblName + " SET ");
                foreach (var item in dic)
                {
                    sb.Append(item.Key + " = N'" + item.Value + "',");
                }
                sb = sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append(string.Format(" WHERE {0}", strKeyInfo));
                result = BP.DA.DBAccess.RunSQL(sb.ToString());
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return result;
        }

        public int Insert(string strTblName, Dictionary<string, string> dic)
        {
            int result = -1;
            try
            {
                StringBuilder sb = new StringBuilder();
                Paras ps = new Paras();
                sb.Append("INSERT INTO  " + strTblName + "( ");
                foreach (var item in dic)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        sb.Append(item.Key + ",");
                        ps.Add(item.Key, item.Value);
                    }
                }
                sb = sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append(" ) VALUES ( ");
                foreach (var item in dic)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        sb.Append("@" + item.Key + " ,");
                    }
                }
                sb = sb.Remove(sb.ToString().LastIndexOf(','), 1);

                sb.Append(") ");
                result = BP.DA.DBAccess.RunSQL(sb.ToString(), ps);
            }
            catch (Exception ex)
            {
                //エラー発生する
                return result;
            }
            return result;
        }


        public DataTable Select(string strTblName, string strKey, string strKeyValue)
        {
            string json = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                //画面区分取得
                string sql = string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", strTblName, strKey, strKeyValue);
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                //json = BP.Tools.Json.DataTableToJson(dt);
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return dt;
        }
        public DataTable Select(string sql)
        {
            string json = string.Empty;
            DataTable dt = new DataTable();
            try
            {

                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                //json = BP.Tools.Json.DataTableToJson(dt);
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return dt;
        }

        public int CheckCount(string strTblName, string strKey)
        {
            int empNum = -1;
            try
            {
                string sql = string.Format("SELECT count({0}) as Num FROM {1} ", strKey, strTblName);
                empNum = BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
            catch (Exception)
            {
                //エラー発生する
            }
            return empNum;
        }
    }
}