using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using System.IO;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class OneKeyBackCCFlow : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public OneKeyBackCCFlow()
        {
            this.Title = "バックアップフローとフォーム。";
            this.Help = "フロー、フォーム、および組織構造データのxmlドキュメントを生成し、C：\\ CCFlowTempleteにバックアップします。";
            this.GroupName = "データのバックアップ/復元";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string path = "C:\\CCFlowTemplete" + DateTime.Now.ToString("yy年MM月dd日HH時mm分ss秒");
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            #region 1.备份流程类别信息
            DataSet dsFlows = new DataSet();
            //WF_FlowSort
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM WF_FlowSort");
            dt.TableName = "WF_FlowSort";
            dsFlows.Tables.Add(dt);
            dsFlows.WriteXml(path + Path.DirectorySeparatorChar + "FlowTables.xml");
            #endregion 备份流程类别信息.

            #region 2.备份组织结构.
            DataSet dsPort = new DataSet();
            //emps
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Port_Emp");
            dt.TableName = "Port_Emp";
            dsPort.Tables.Add(dt);

            //Port_Dept
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Port_Dept");
            dt.TableName = "Port_Dept";
            dsPort.Tables.Add(dt);

            //Port_Station
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Port_Station");
            dt.TableName = "Port_Station";
            dsPort.Tables.Add(dt);

            //Port_EmpStation
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Port_DeptEmpStation");
            dt.TableName = "Port_DeptEmpStation";
            dsPort.Tables.Add(dt);
            

            dsPort.WriteXml(path + Path.DirectorySeparatorChar + "PortTables.xml");
            #endregion 备份表单相关数据.

            #region 3.备份系统数据
            DataSet dsSysTables = new DataSet();

            //Sys_EnumMain
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Sys_EnumMain");
            dt.TableName = "Sys_EnumMain";
            dsSysTables.Tables.Add(dt);

            //Sys_Enum
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Sys_Enum");
            dt.TableName = "Sys_Enum";
            dsSysTables.Tables.Add(dt);

            //Sys_FormTree
            dt = DBAccess.RunSQLReturnTable("SELECT * FROM Sys_FormTree");
            dt.TableName = "Sys_FormTree";
            dsSysTables.Tables.Add(dt);
            dsSysTables.WriteXml(path + Path.DirectorySeparatorChar + "SysTables.xml");
            #endregion 备份系统数据.

            #region 4.备份表单相关数据.
            string pathOfTables = path + Path.DirectorySeparatorChar + "SFTables";
            System.IO.Directory.CreateDirectory(pathOfTables);
            SFTables tabs = new SFTables();
            tabs.RetrieveAll();
            foreach (SFTable item in tabs)
            {
                if (item.No.Contains("."))
                    continue;

                if (item.SrcType != SrcType.CreateTable)
                    continue;

                try
                {
                    string sql = "SELECT * FROM " + item.No + " ";
                    DataSet ds = new DataSet();
                    ds.Tables.Add(BP.DA.DBAccess.RunSQLReturnTable(sql));
                    ds.WriteXml(pathOfTables + Path.DirectorySeparatorChar + item.No + ".xml");
                }
                catch
                {

                }
            }
            #endregion 备份表单相关数据.

            #region 5.备份流程.
            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            foreach (Flow fl in fls)
            {
                FlowSort fs = new FlowSort();
                fs.No = fl.FK_FlowSort;
                fs.RetrieveFromDBSources();

                string pathDir = path + Path.DirectorySeparatorChar + "Flow" + Path.DirectorySeparatorChar + fs.No + "." + fs.Name + Path.DirectorySeparatorChar;
                if (System.IO.Directory.Exists(pathDir) == false)
                    System.IO.Directory.CreateDirectory(pathDir);

                fl.DoExpFlowXmlTemplete(pathDir);
            }
            #endregion 备份流程.

            #region 6.备份表单.
            MapDatas mds = new MapDatas();
            mds.RetrieveAllFromDBSource();
            foreach (MapData md in mds)
            {
                if (md.FK_FrmSort.Length < 2)
                    continue;

                SysFormTree fs = new SysFormTree();
                fs.No = md.FK_FormTree;
                fs.RetrieveFromDBSources();

                string pathDir = path + Path.DirectorySeparatorChar + "Form" + Path.DirectorySeparatorChar + fs.No + "." + fs.Name;
                if (System.IO.Directory.Exists(pathDir) == false)
                    System.IO.Directory.CreateDirectory(pathDir);
                DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);
                ds.WriteXml(pathDir + Path.DirectorySeparatorChar + md.Name + ".xml");
            }
            #endregion 备份表单.

            return "正常に実行しました。保存するパス:" + path;
        }
    }
}
