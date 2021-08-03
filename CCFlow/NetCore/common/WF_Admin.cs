using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin : DirectoryPageBase
    {
        #region 属性.
        public string RefNo
        {
            get
            {
                return this.GetRequestVal("RefNo");
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin()
        {
        }

        #region 测试页面.
        /// <summary>
        /// 获得运行的集成平台.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_GetRunOnPlant()
        {
            return BP.Sys.SystemConfig.RunOnPlant;
        }
        /// <summary>
        /// 初始化界面.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_Init()
        {
            //清除缓存.
            BP.Sys.SystemConfig.DoClearCash();

            if (1 == 2 && BP.Web.WebUser.IsAdmin == false)
                return "err@あなたは管理者ではないため、この操作を実行できません.";

            // 让admin 登录.
            //   BP.WF.Dev2Interface.Port_Login("admin");

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGener(emp);
                HttpContextHelper.SessionSet("FK_Flow", this.FK_Flow);
                return "url@../MyFlow.htm?FK_Flow=" + this.FK_Flow;
            }

            FlowExt fl = new FlowExt(this.FK_Flow);

            if (1 == 2 && BP.Web.WebUser.No != "admin" && fl.Tester.Length <= 1)
            {
                string msg = "err@副管理者[" + BP.Web.WebUser.Name + "]こんにちは、このフローのテスターをまだ設定していません.";
                msg += "フロープロパティの下部にある[Set Process Initiation Tester]のプロパティで開始できるテスターを設定する必要があります。複数の担当者はカンマで区切られます。";
                return msg;
            }

            /* 检查是否设置了测试人员，如果设置了就按照测试人员身份进入
             * 设置测试人员的目的是太多了人员影响测试效率.
             * */
            if (fl.Tester.Length > 2)
            {
                // 构造人员表.
                DataTable dtEmps = new DataTable();
                dtEmps.Columns.Add("No");
                dtEmps.Columns.Add("Name");
                dtEmps.Columns.Add("FK_DeptText");

                string[] strs = fl.Tester.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    Emp emp = new Emp();
                    emp.SetValByKey("No", str);
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        continue;

                    DataRow dr = dtEmps.NewRow();
                    dr["No"] = emp.No;
                    dr["Name"] = emp.Name;
                    dr["FK_DeptText"] = emp.FK_DeptText;
                    dtEmps.Rows.Add(dr);
                }
                return BP.Tools.Json.ToJson(dtEmps);
            }



            //fl.DoCheck();

            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt = null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);

            if (nd.IsGuestNode)
            {
                /*如果是guest节点，就让其跳转到 guest登录界面，让其发起流程。*/
                //这个地址需要配置.
                return "url@/SDKFlowDemo/GuestApp/Login.aspx?FK_Flow=" + this.FK_Flow;
            }

            try
            {

                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                    case DeliveryWay.ByStationOnly:

                        sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        // emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");
                        break;
                    case DeliveryWay.ByDept:
                        sql = "select No,Name from Port_Emp where FK_Dept in (select FK_Dept from WF_NodeDept where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("");
                        break;
                    case DeliveryWay.ByBindEmp:
                        sql = "select No,Name from Port_Emp where No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                        break;
                    case DeliveryWay.ByDeptAndStation:
                        //added by liuxc,2015.6.30.
                        //区别集成与BPM模式
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                        {
                            sql = "SELECT No FROM Port_Emp WHERE No IN ";
                            sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE FK_Dept IN ";
                            sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nodeid + ")";
                            sql += ")";
                            sql += "AND No IN ";
                            sql += "(";
                            sql += "SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ";
                            sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeid + ")";
                            sql += ") ORDER BY No ";
                        }
                        else
                        {
                            sql = "SELECT pdes.FK_Emp AS No"
                                  + " FROM   Port_DeptEmpStation pdes"
                                  + "        INNER JOIN WF_NodeDept wnd"
                                  + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                                  + "             AND wnd.FK_Node = " + nodeid
                                  + "        INNER JOIN WF_NodeStation wns"
                                  + "             ON  wns.FK_Station = pdes.FK_Station"
                                  + "             AND wnd.FK_Node =" + nodeid
                                  + " ORDER BY"
                                  + "        pdes.FK_Emp";
                        }
                        break;
                    case DeliveryWay.BySelected: //所有的人员多可以启动, 2016年11月开始约定此规则.
                        sql = "SELECT No as FK_Emp FROM Port_Emp ";
                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 300)
                        {
                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MSSQL)
                                sql = "SELECT top 300 No as FK_Emp FROM Port_Emp ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE ROWNUM <300 ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp   limit 0,300 ";
                        }
                        break;
                    case DeliveryWay.BySQL:
                        if (DataType.IsNullOrEmpty(nd.DeliveryParas))
                            return "err@SQLによるアクセスの開始ノードを設定したが、SQLを設定しなかった.";
                        break;
                    default:
                        break;
                }

                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return "err@フォローしています:" + nd.HisDeliveryWay + "開始ノードのアクセスルールが設定されていますが、開始ノードに担当者がいません。";

                if (dt.Rows.Count > 2000)
                    return "err@開始ノードを開始できるユーザーが多すぎると、システムがクラッシュして速度が低下します。フローのプロパティでフローを開始できるテストユーザーを設定する必要があります.";

                // 构造人员表.
                DataTable dtMyEmps = new DataTable();
                dtMyEmps.Columns.Add("No");
                dtMyEmps.Columns.Add("Name");
                dtMyEmps.Columns.Add("FK_DeptText");

                //处理发起人数据.
                string emps = "";
                foreach (DataRow dr in dt.Rows)
                {
                    string myemp = dr[0].ToString();
                    if (emps.Contains("," + myemp + ",") == true)
                        continue;

                    emps += "," + myemp + ",";
                    BP.Port.Emp emp = new Emp(myemp);

                    DataRow drNew = dtMyEmps.NewRow();

                    drNew["No"] = emp.No;
                    drNew["Name"] = emp.Name;
                    drNew["FK_DeptText"] = emp.FK_DeptText;

                    dtMyEmps.Rows.Add(drNew);
                }


                //检查物理表,避免错误.
                Nodes nds = new Nodes(this.FK_Flow);
                foreach (Node mynd in nds)
                {
                    mynd.HisWork.CheckPhysicsTable();
                }


                //返回数据源.
                return BP.Tools.Json.ToJson(dtMyEmps);
            }
            catch (Exception ex)
            {
                return "err@<h2>開始ノードのアクセスルールが正しく設定されていないため、スターター<a href='http://bbs.ccflow.org/showtopic-4103.aspx' target=_blank> <font color = red>がありませんソリューションを表示するには、ここをクリックしてください</ font>。</a>。 </ h2>システムエラーメッセージ:" + ex.Message + "<br><h3>OSModelを切り替えた可能性もあります。OSModelとは、オンラインヘルプドキュメントを確認してください。 <a href='http://ccbpm.mydoc.io' target=_blank>http://ccbpm.mydoc.io</a>  .</h3>";
            }
        }


        /// <summary>
        /// 转到指定的url.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_ReturnToUser()
        {
            string userNo = this.GetRequestVal("UserNo");
            BP.WF.Dev2Interface.Port_Login(userNo);
            string sid = BP.WF.Dev2Interface.Port_GenerSID(userNo);
            string url = "../../WF/Port.htm?UserNo=" + userNo + "&SID=" + sid + "&DoWhat=" + this.GetRequestVal("DoWhat") + "&FK_Flow=" + this.FK_Flow + "&IsMobile=" + this.GetRequestVal("IsMobile");
            return "url@" + url;
        }
        #endregion 测试页面.
       

        #region 安装.
        /// <summary>
        /// 初始化安装包
        /// </summary>
        /// <returns></returns>
        public string DBInstall_Init()
        {
            try
            {
                if (DBAccess.TestIsConnection() == false)
                    return "err@データベース接続構成エラー。データベース構成接続を表示するには、マニュアルを参照してください.";


             //   DBAccess.IsCaseSensitive
 
                //判断是否可以安装,不能安装就抛出异常.
                BP.WF.Glo.IsCanInstall();

                //判断是不是有.
                if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == true)
                    return "err@infoデータベースがインストールされているため、インストールする必要はありません。<a href='./CCBPMDesigner/Login.htm'>ここをクリックして、フローデザイナーに直接ログインします</a>";


                Hashtable ht = new Hashtable();
                ht.Add("OSModel", (int)BP.WF.Glo.OSModel); //组织结构类型.
                ht.Add("DBType", SystemConfig.AppCenterDBType.ToString()); //数据库类型.
                ht.Add("Ver", BP.WF.Glo.Ver); //版本号.

                return BP.Tools.Json.ToJson(ht);
            }
            catch (Exception ex)
            {

                return "err@" + ex.Message ;
            }
        }
        public string DBInstall_Submit()
        {
            string lang = "CH";

            //是否要安装demo.
            int demoTye = this.GetRequestValInt("DemoType");

            //运行ccflow的安装.
            BP.WF.Glo.DoInstallDataBase(lang, demoTye);

            //执行ccflow的升级。
            BP.WF.Glo.UpdataCCFlowVer();

            //加注释.
            BP.Sys.PubClass.AddComment();

            return "info@システムが正常にインストールされました。<a href='./CCBPMDesigner/Login.htm'>をクリックして、直接にフロー計算器を登録します。</a>";
            // this.Response.Redirect("DBInstall.aspx?DoType=OK", true);
        }
        #endregion


        public string ReLoginSubmit()
        {
            string userNo = this.GetValFromFrmByKey("TB_No");
            string password = this.GetValFromFrmByKey("TB_PW");

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@ユーザ名またはパスワードが間違っています。";

            if (emp.CheckPass(password) == false)
                return "err@ユーザ名またはパスワードが間違っています。";

            BP.Web.WebUser.SignInOfGener(emp);

            return "ログインしました。";
        }
        /// <summary>
        /// 加载模版.
        /// </summary>
        /// <returns></returns>
        public string SettingTemplate_Init()
        {
            //类型.
            string templateType = this.GetRequestVal("TemplateType");
            string condType = this.GetRequestVal("CondType");

            BP.WF.Template.SQLTemplates sqls = new SQLTemplates();
            //sqls.Retrieve(BP.WF.Template.SQLTemplateAttr.SQLType, sqlType);

            DataTable dt = null;
            string sql = "";

            #region 节点方向条件模版.
            if (templateType == "CondBySQL")
            {
                /*方向条件, 节点方向条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.SQL;
            }

            if (templateType == "CondByUrl")
            {
                /*方向条件, 节点方向url条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.Url;
            }

            if (templateType == "CondByPara")
            {
                /*方向条件, 节点方向url条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.Paras;
            }
            #endregion 节点方向条件模版.

            #region 表单扩展设置.

            string add = "+";

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                add = "||";

            if (templateType == "DDLFullCtrl")
                sql = "SELECT MyPK, 'ドロップダウンボックス： '" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='DDLFullCtrl'";

            if (templateType == "ActiveDDL")
                sql = "SELECT MyPK, 'ドロップダウンボックス： '" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='ActiveDDL'";

            //显示过滤.
            if (templateType == "AutoFullDLL")
                sql = "SELECT MyPK, 'ドロップダウンボックス： '" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='AutoFullDLL'";

            //文本框自动填充..
            if (templateType == "TBFullCtrl")
                sql = "SELECT MyPK, 'テキストボックス： '" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='TBFullCtrl'";

            //自动计算.
            if (templateType == "AutoFull")
                sql = "SELECT MyPK, 'ID:'" + add + " a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='AutoFull'";
            #endregion 表单扩展设置.

            #region 节点属性的模版.
            //自动计算.
            if (templateType == "NodeAccepterRole")
                sql = "SELECT NodeID, FlowName +' - '+Name, a.DeliveryParas as Docs FROM WF_Node a WHERE  a.DeliveryWay=" + (int)DeliveryWay.BySQL;
            #endregion 节点属性的模版.

            if (sql == "")
                return "err@マークとは関係ありません[" + templateType + "].";

            dt = DBAccess.RunSQLReturnTable(sql);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                BP.WF.Template.SQLTemplate en = new SQLTemplate();
                en.No = dr[0].ToString();
                en.Name = dr[1].ToString();
                en.Docs = dr[2].ToString();

                if (strs.Contains(en.Docs.Trim() + ";") == true)
                    continue;
                strs += en.Docs.Trim() + ";";
                sqls.AddEntity(en);
            }

            return sqls.ToJson();
        }
    }
}
