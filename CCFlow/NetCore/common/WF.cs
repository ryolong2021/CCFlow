using BP.DA;
using BP.Sys;
using BP.Web;
using BP.WF;
using BP.WF.Data;
using BP.WF.Port;
using BP.WF.Rpt;
using BP.WF.Template;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BP.WF.HttpHandler
{
    public class WF : DirectoryPageBase
    {
        #region 单表单查看.
        /// <summary>
        /// 流程单表单查看
        /// </summary>
        /// <returns></returns>
        public string FrmView_Init()
        {
            Node nd = new Node(this.FK_Node);
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

            MapData md = new MapData();
            md.No = nd.NodeFrmID;
            if (md.RetrieveFromDBSources() == 0)
            {
                throw new Exception("読み込みエラー、フォームID=" + md.No + "を失われました。フローを修正して、もう一度ロードしてください。");
            }



            //获得表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);


            #region 把主从表数据放入里面.
            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            //重设默认值.
            //wk.ResetDefaultVal();

            DataTable mainTable = wk.ToDataTableField("MainTable");
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);
            #endregion


            //加入WF_Node.
            DataTable WF_Node = nd.ToDataTableField("WF_Node").Copy();
            myds.Tables.Add(WF_Node);


            #region 加入组件的状态信息, 在解析表单的时候使用.
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            BP.WF.Template.FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);
            if (nd.NodeFrmID != "ND" + nd.NodeID)
            {
                /*说明这是引用到了其他节点的表单，就需要把一些位置元素修改掉.*/
                int refNodeID = int.Parse(nd.NodeFrmID.Replace("ND", ""));

                BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(refNodeID);

                fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H));
                fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W));
                fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X));
                fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y));


                fnc.SetValByKey(FrmSubFlowAttr.SF_H, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_H));
                fnc.SetValByKey(FrmSubFlowAttr.SF_W, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_W));
                fnc.SetValByKey(FrmSubFlowAttr.SF_X, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_X));
                fnc.SetValByKey(FrmSubFlowAttr.SF_Y, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_Y));

                fnc.SetValByKey(FrmThreadAttr.FrmThread_H, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_H));
                fnc.SetValByKey(FrmThreadAttr.FrmThread_W, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_W));
                fnc.SetValByKey(FrmThreadAttr.FrmThread_X, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_X));
                fnc.SetValByKey(FrmThreadAttr.FrmThread_Y, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_Y));

                fnc.SetValByKey(FrmTrackAttr.FrmTrack_H, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_H));
                fnc.SetValByKey(FrmTrackAttr.FrmTrack_W, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_W));
                fnc.SetValByKey(FrmTrackAttr.FrmTrack_X, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_X));
                fnc.SetValByKey(FrmTrackAttr.FrmTrack_Y, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_Y));

                fnc.SetValByKey(FTCAttr.FTC_H, refFnc.GetValFloatByKey(FTCAttr.FTC_H));
                fnc.SetValByKey(FTCAttr.FTC_W, refFnc.GetValFloatByKey(FTCAttr.FTC_W));
                fnc.SetValByKey(FTCAttr.FTC_X, refFnc.GetValFloatByKey(FTCAttr.FTC_X));
                fnc.SetValByKey(FTCAttr.FTC_Y, refFnc.GetValFloatByKey(FTCAttr.FTC_Y));
            }
            myds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent").Copy());
            #endregion 加入组件的状态信息, 在解析表单的时候使用.

            #region 增加附件信息.
            BP.Sys.FrmAttachments athDescs = new FrmAttachments();

            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            athDescs.Retrieve(FrmAttachmentAttr.FK_MapData, nd.NodeFrmID);
            if (athDescs.Count != 0)
            {
                FrmAttachment athDesc = athDescs[0] as FrmAttachment;

                //查询出来数据实体.
                BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
                if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
                {
                    Paras ps = new Paras();
                    ps.SQL = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                    ps.Add("WorkID", this.WorkID);
                    string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0).ToString();
                    if (pWorkID == null || pWorkID == "0")
                    {
                        pWorkID = this.WorkID.ToString();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /* 继承模式 */
                        BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                        qo.addOr();
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                        qo.addOrderBy("RDT");
                        qo.DoQuery();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*共享模式*/
                        dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                    }
                }
                else if (athDesc.HisCtrlWay == AthCtrlWay.WorkID)
                {
                    /* 继承模式 */
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
                    qo.addOrderBy("RDT");
                    qo.DoQuery();
                }

                //增加一个数据源.
                myds.Tables.Add(dbs.ToDataTableField("Sys_FrmAttachmentDB").Copy());
            }
            #endregion

            #region 把外键表加入DataSet
            DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
            DataTable dt = new DataTable();
            MapExts mes = md.MapExts;
            MapExt me = new MapExt();
            DataTable ddlTable = new DataTable();
            ddlTable.Columns.Add("No");
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                string uiBindKey = dr["UIBindKey"].ToString();

                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue; //为空就continue.

                if (lgType.Equals("1") == true)
                    continue; //枚举值就continue;

                string uiIsEnable = dr["UIIsEnable"].ToString();
                if (uiIsEnable.Equals("0") == true && lgType.Equals("1") == true)
                    continue; //如果是外键，并且是不可以编辑的状态.

                if (uiIsEnable.Equals("1") == true && lgType.Equals("0") == true)
                    continue; //如果是外部数据源，并且是不可以编辑的状态.



                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null)
                {
                    string fullSQL = me.Doc.Clone() as string;
                    fullSQL = fullSQL.Replace("~", ",");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
                    dt = DBAccess.RunSQLReturnTable(fullSQL);
                    //重构新表
                    DataTable dt_FK_Dll = new DataTable();
                    dt_FK_Dll.TableName = keyOfEn;//可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                    dt_FK_Dll.Columns.Add("No", typeof(string));
                    dt_FK_Dll.Columns.Add("Name", typeof(string));
                    foreach (DataRow dllRow in dt.Rows)
                    {
                        DataRow drDll = dt_FK_Dll.NewRow();
                        drDll["No"] = dllRow["No"];
                        drDll["Name"] = dllRow["Name"];
                        dt_FK_Dll.Rows.Add(drDll);
                    }
                    myds.Tables.Add(dt_FK_Dll);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                {
                    continue;
                }

                DataTable mydt = BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey);
                if (mydt == null)
                {
                    DataRow ddldr = ddlTable.NewRow();
                    ddldr["No"] = uiBindKey;
                    ddlTable.Rows.Add(ddldr);
                }
                else
                {
                    myds.Tables.Add(mydt);
                }
            }
            ddlTable.TableName = "UIBindKey";
            myds.Tables.Add(ddlTable);
            #endregion End把外键表加入DataSet

            #region 图片附件
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            FrmImgAthDBs imgAthDBs = new FrmImgAthDBs(nd.NodeFrmID, this.WorkID.ToString());
            if (imgAthDBs != null && imgAthDBs.Count > 0)
            {
                DataTable dt_ImgAth = imgAthDBs.ToDataTableField("Sys_FrmImgAthDB");
                myds.Tables.Add(dt_ImgAth);
            }
            #endregion


            return BP.Tools.Json.ToJson(myds);
        }
        #endregion



        /// <summary>
        /// 流程数据
        /// </summary>
        /// <returns></returns>
        public string Watchdog_Init()
        {
            string sql = " SELECT FK_Flow,FlowName, COUNT(workid) as Num FROM V_MyFlowData WHERE MyEmpNo='" + WebUser.No + "' ";
            sql += " GROUP BY  FK_Flow,FlowName ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Group";
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 流程数据初始化
        /// </summary>
        /// <returns></returns>
        public string Watchdog_InitFlows()
        {
            string sql = " SELECT *  FROM V_MyFlowData WHERE MyEmpNo='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Flows";
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF()
        {

        }
        protected override string DoDefaultMethod()
        {
            return base.DoDefaultMethod();
        }
        public string HasSealPic()
        {
            string no = GetRequestVal("No");
            if (string.IsNullOrWhiteSpace(no))
            {
                return "";
            }

            string path = "/DataUser/Siganture/" + no + ".jpg";
            //如果文件存在

            if (File.Exists(SystemConfig.PathOfWebApp + (path)) == false)
            {
                path = "/DataUser/Siganture/" + no + ".JPG";
                if (File.Exists(SystemConfig.PathOfWebApp + (path)) == true)
                {
                    return "";
                }

                //如果不存在，就返回名称
                BP.Port.Emp emp = new BP.Port.Emp(no);
                return emp.Name;
            }
            return "";
        }
        /// <summary>
        /// 执行的方法.
        /// </summary>
        /// <returns></returns>
        public string Do_Init()
        {
            string at = this.GetRequestVal("ActionType");
            if (DataType.IsNullOrEmpty(at))
            {
                at = this.GetRequestVal("DoType");
            }

            if (DataType.IsNullOrEmpty(at) && this.SID != null)
            {
                at = "Track";
            }

            try
            {
                switch (at)
                {

                    case "Focus": //把任务放入任务池.
                        BP.WF.Dev2Interface.Flow_Focus(this.WorkID);
                        return "info@Close";
                    case "PutOne": //把任务放入任务池.
                        BP.WF.Dev2Interface.Node_TaskPoolPutOne(this.WorkID);
                        return "info@Close";
                        break;
                    case "DoAppTask": // 申请任务.
                        BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(this.WorkID);
                        return "info@Close";
                    case "DoOpenCC":
                    case "WFRpt":
                        string Sta = this.GetRequestVal("Sta");
                        if (Sta == "0")
                        {
                            BP.WF.Template.CCList cc1 = new BP.WF.Template.CCList();
                            cc1.MyPK = this.MyPK;
                            cc1.Retrieve();
                            cc1.HisSta = CCSta.Read;
                            cc1.Update();
                        }
                        return "url@./WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                    case "DelCC": //删除抄送.
                        CCList cc = new CCList();
                        cc.MyPK = this.MyPK;
                        cc.Retrieve();
                        cc.HisSta = CCSta.Del;
                        cc.Update();
                        return "info@Close";
                    case "DelSubFlow": //删除进程。
                        try
                        {
                            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手動で削除");
                            return "info@Close";
                        }
                        catch (Exception ex)
                        {
                            return "err@" + ex.Message;
                        }
                        break;
                    case "DownBill":
                        Bill b = new Bill(this.MyPK);
                        b.DoOpen();
                        break;
                    case "DelDtl":
                        GEDtls dtls = new GEDtls(this.EnsName);
                        GEDtl dtl = (GEDtl)dtls.GetNewEntity;
                        dtl.OID = this.RefOID;
                        if (dtl.RetrieveFromDBSources() == 0)
                        {
                            return "info@Close";
                        }
                        FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.

                        // 处理删除前事件.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelBefore, dtl);
                        }
                        catch (Exception ex)
                        {
                            return "err@" + ex.Message;
                        }
                        dtl.Delete();

                        // 处理删除后事件.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            return "err@" + ex.Message;
                            break;
                        }
                        return "info@Close";
                        break;
                    case "EmpDoUp":
                        BP.WF.Port.WFEmp ep = new BP.WF.Port.WFEmp(this.GetRequestVal("RefNo"));
                        ep.DoUp();

                        BP.WF.Port.WFEmps emps111 = new BP.WF.Port.WFEmps();
                        //  emps111.RemoveCash();
                        emps111.RetrieveAll();
                        return "info@Close";
                        break;
                    case "EmpDoDown":
                        BP.WF.Port.WFEmp ep1 = new BP.WF.Port.WFEmp(this.GetRequestVal("RefNo"));
                        ep1.DoDown();

                        BP.WF.Port.WFEmps emps11441 = new BP.WF.Port.WFEmps();
                        //  emps11441.RemoveCash();
                        emps11441.RetrieveAll();
                        return "info@Close";
                    case "Track": //通过一个串来打开一个工作.
                        string mySid = this.SID; // this.Request.QueryString["SID"];
                        string[] mystrs = mySid.Split('_');

                        Int64 myWorkID = int.Parse(mystrs[1]);
                        string fk_emp = mystrs[0];
                        int fk_node = int.Parse(mystrs[2]);
                        Node mynd = new Node();
                        mynd.NodeID = fk_node;
                        mynd.RetrieveFromDBSources();

                        string fk_flow = mynd.FK_Flow;
                        string myurl = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Node=" + mynd.NodeID + "&WorkID=" + myWorkID + "&FK_Flow=" + fk_flow;
                        Web.WebUser.SignInOfGener(new BP.Port.Emp(fk_emp));

                        return "url@" + myurl;
                    case "OF": //通过一个串来打开一个工作.
                        string sid = this.SID;
                        string[] strs = sid.Split('_');
                        GenerWorkerList wl = new GenerWorkerList();
                        int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                            GenerWorkerListAttr.WorkID, strs[1],
                            GenerWorkerListAttr.IsPass, 0);

                        if (i == 0)
                        {
                            return "info@このジョブは他の誰かによって処理されたか、このフローは削除されました";
                        }

                        GenerWorkFlow gwf = new GenerWorkFlow(wl.WorkID);
                        BP.Port.Emp empOF = new BP.Port.Emp(wl.FK_Emp);
                        Web.WebUser.SignInOfGener(empOF);
                        string u = "MyFlow.htm?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID + "&FK_Node=" + wl.FK_Node + "&FID=" + wl.FID+"&PWorkID="+gwf.PWorkID;

                        return "url@" + u;
                    case "ExitAuth":
                        BP.Port.Emp emp = new BP.Port.Emp(this.FK_Emp);
                        //首先退出，再进行登录
                        BP.Web.WebUser.Exit();
                        BP.Web.WebUser.SignInOfGener(emp, WebUser.SysLang);
                        return "info@Close";
                    case "LogAs":
                        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.FK_Emp);
                        if (wfemp.AuthorIsOK == false)
                        {
                            return "err@承認に失敗しました";
                        }
                        BP.Port.Emp emp1 = new BP.Port.Emp(this.FK_Emp);
                        BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, wfemp.Author, WebUser.Name);
                        return "info@Close";
                    case "TakeBack": // 取消授权。
                        BP.WF.Port.WFEmp myau = new BP.WF.Port.WFEmp(WebUser.No);
                        BP.DA.Log.DefaultLogWriteLineInfo("承認をキャンセル:" + WebUser.No + "(" + myau.Author + ")の授権がキャンセルされました。");
                        myau.Author = "";
                        myau.AuthorWay = 0;
                        myau.Update();
                        return "info@Close";
                    case "AutoTo": // 执行授权。
                        BP.WF.Port.WFEmp au = new BP.WF.Port.WFEmp();
                        au.No = WebUser.No;
                        au.RetrieveFromDBSources();
                        au.AuthorDate = BP.DA.DataType.CurrentData;
                        au.Author = this.FK_Emp;
                        au.AuthorWay = 1;
                        au.Save();
                        BP.DA.Log.DefaultLogWriteLineInfo("承認を実行:" + WebUser.No + "(" + au.Author + ")の授権が実行されました。");
                        return "info@Close";
                    case "UnSend": //执行撤消发送。
                        string url = "./WorkOpt/UnSend.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                        return "url@" + url;
                    case "SetBillState":
                        break;
                    case "WorkRpt":
                        //  Bill bk1 = new Bill(this.OID);
                        //  Node nd = new Node(bk1.FK_Node);
                        // this.Response.Redirect("WFRpt.htm?WorkID=" + bk1.WorkID + "&FID=" + bk1.FID + "&FK_Flow=" + nd.FK_Flow + "&NodeId=" + bk1.FK_Node, false);
                        //this.WinOpen();
                        //this.WinClose();
                        break;
                    case "PrintBill":
                        //Bill bk2 = new Bill(this.Request.QueryString["OID"]);
                        //Node nd2 = new Node(bk2.FK_Node);
                        //this.Response.Redirect("NodeRefFunc.aspx?NodeId=" + bk2.FK_Node + "&FlowNo=" + nd2.FK_Flow + "&NodeRefFuncOID=" + bk2.FK_NodeRefFunc + "&WorkFlowID=" + bk2.WorkID);
                        ////this.WinClose();
                        break;
                    //删除流程中第一个节点的数据，包括待办工作
                    case "DeleteFlow":
                        //调用DoDeleteWorkFlowByReal方法
                        WorkFlow wf = new WorkFlow(new Flow(this.FK_Flow), this.WorkID);
                        wf.DoDeleteWorkFlowByReal(true);
                        //Glo.ToMsg("流程删除成功");
                        return "フローは正常に削除されました";

                        //this.ToWFMsgPage("流程删除成功");
                        break;
                    case "DownFlowSearchExcel":    //下载流程查询结果，转到下面的逻辑，不放在此try..catch..中
                        break;
                    case "DownFlowSearchToTmpExcel":    //导出到模板
                        break;
                    default:
                        throw new Exception("判断されないatマーク:" + at);
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            //此处之所以再加一个switch，是因为在下载文件逻辑中，调用Response.End()方法，如果此方法放在try..catch..中，会报线程中止异常
            switch (at)
            {
                case "DownFlowSearchExcel":
                    //  DownMyStartFlowExcel();
                    break;
                case "DownFlowSearchToTmpExcel":    //导出到模板
                    // DownMyStartFlowToTmpExcel();
                    break;
            }
            return "";
        }

        /// <summary>
        /// 获取设置的PC端和移动端URL
        /// </summary>
        /// <returns></returns>
        public string PCAndMobileUrl()
        {
            Hashtable ht = new Hashtable();
            ht.Add("PCUrl", SystemConfig.HostURL);
            ht.Add("MobileUrl", SystemConfig.MobileURL);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 页面调整移动端OR手机端
        /// </summary>
        /// <returns></returns>
        public string Do_Direct()
        {
            //获取地址
            string baseUrl = this.GetRequestVal("DirectUrl");
            ////判断是移动端还是PC端打开的页面
            //Regex RegexMobile = new Regex(@"(iemobile|iphone|ipod|android|nokia|sonyericsson|blackberry|samsung|sec\-|windows ce|motorola|mot\-|up.b|midp\-)",
            //RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //移动端打开
            if (HttpContextHelper.RequestIsFromMobile)
            {
                return SystemConfig.MobileURL + baseUrl;
            }
            else
            {
                return SystemConfig.HostURL + baseUrl;
            }

        }

        #region 我的关注流程.
        /// <summary>
        /// 我的关注流程
        /// </summary>
        /// <returns></returns>
        public string Focus_Init()
        {
            string flowNo = this.GetRequestVal("FK_Flow");

            int idx = 0;
            //获得关注的数据.
            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_Focus(flowNo, BP.Web.WebUser.No);
            SysEnums stas = new SysEnums("WFSta");
            string[] tempArr;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                int wfsta = int.Parse(dr["WFSta"].ToString());
                //edit by liuxc,2016-10-22,修复状态显示不正确问题
                string wfstaT = (stas.GetEntityByKey(SysEnumAttr.IntKey, wfsta) as SysEnum).Lab;
                string currEmp = string.Empty;

                if (wfsta != (int)BP.WF.WFSta.Complete)
                {
                    //edit by liuxc,2016-10-24,未完成时，处理当前处理人，只显示处理人姓名
                    foreach (string emp in dr["ToDoEmps"].ToString().Split(';'))
                    {
                        tempArr = emp.Split(',');

                        currEmp += tempArr.Length > 1 ? tempArr[1] : tempArr[0] + ",";
                    }

                    currEmp = currEmp.TrimEnd(',');

                    //currEmp = dr["ToDoEmps"].ToString();
                    //currEmp = currEmp.TrimEnd(';');
                }
                dr["ToDoEmps"] = currEmp;
                dr["FlowNote"] = wfstaT;
                dr["AtPara"] = (wfsta == (int)BP.WF.WFSta.Complete ? dr["Sender"].ToString().TrimStart('(').TrimEnd(')').Split(',')[1] : "");
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <returns></returns>
        public string Focus_Delete()
        {
            BP.WF.Dev2Interface.Flow_Focus(this.WorkID);
            return "正常に実行しました。";
        }
        #endregion 我的关注.

        /// <summary>
        /// 方法
        /// </summary>
        /// <returns></returns>
        public string HandlerMapExt()
        {
            WF_CCForm wf = new WF_CCForm();
            return wf.HandlerMapExt();
        }
        /// <summary>
        /// 节水公司
        /// </summary>
        /// <returns></returns>
        public string Start_InitTianYe_JieShui()
        {
            //获得当前人员的部门,根据部门获得该人员的组织集合.
            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //找到当前人员所在的部门集合, 应该找到他的组织集合为了减少业务逻辑.
            string orgNos = "'0','18099','103'"; //空的数据.
            foreach (DataRow dr in dt.Rows)
            {
                string deptNo = dr[0].ToString();
                orgNos += ",'" + deptNo + "'";
            }

            #region 获取类别列表(根据当前人员所在组织结构进行过滤类别.)
            FlowSorts fss = new FlowSorts();
            BP.En.QueryObject qo = new En.QueryObject(fss);
            qo.AddWhereIn(FlowSortAttr.OrgNo, "(" + orgNos + ")");  //指定的类别.
            qo.addOr();
            qo.AddWhere(FlowSortAttr.Name, " LIKE ", "%節水％");  //指定的类别.

            //排序.
            qo.addOrderBy(FlowSortAttr.No, FlowSortAttr.Idx);

            DataTable dtSort = qo.DoQueryToTable();
            dtSort.TableName = "Sort";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtSort.Columns["NO"].ColumnName = "No";
                dtSort.Columns["NAME"].ColumnName = "Name";
                dtSort.Columns["PARENTNO"].ColumnName = "ParentNo";
                dtSort.Columns["ORGNO"].ColumnName = "OrgNo";
            }

            //定义容器.
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSort); //增加到里面去.
            #endregion 获取类别列表.

            //构造流程实例数据容器。
            DataTable dtStart = new DataTable();
            dtStart.TableName = "Start";
            dtStart.Columns.Add("No");
            dtStart.Columns.Add("Name");
            dtStart.Columns.Add("FK_FlowSort");
            dtStart.Columns.Add("IsBatchStart");
            dtStart.Columns.Add("IsStartInMobile");
            //dtStart.Columns.Add("Note");

            //获得所有的流程（包含了所有子公司与集团的可以发起的流程但是没有根据组织结构进行过滤.）
            DataTable dtAllFlows = Dev2Interface.DB_StarFlows(Web.WebUser.No);

            //按照当前用户的流程类别权限进行过滤.
            foreach (DataRow drSort in dtSort.Rows)
            {
                foreach (DataRow drFlow in dtAllFlows.Rows)
                {
                    if (drSort["No"].ToString() != drFlow["FK_FlowSort"].ToString())
                    {
                        continue;
                    }

                    DataRow drNew = dtStart.NewRow();

                    drNew["No"] = drFlow["No"];
                    drNew["Name"] = drFlow["Name"];
                    drNew["FK_FlowSort"] = drFlow["FK_FlowSort"];
                    drNew["IsBatchStart"] = drFlow["IsBatchStart"];
                    drNew["IsStartInMobile"] = drFlow["IsStartInMobile"];
                    //drNew["Note"] = drFlow["Note"];
                    dtStart.Rows.Add(drNew); //增加到里里面去.
                }
            }

            //把经过权限过滤的流程实体放入到集合里.
            ds.Tables.Add(dtStart); //增加到里面去.

            //返回组合
            string json = BP.Tools.Json.DataSetToJson(ds, false);

            //放入缓存里面去.
            BP.WF.Port.WFEmp em = new WFEmp();
            em.No = BP.Web.WebUser.No;

            //把json存入数据表，避免下一次再取.
            if (json.Length > 40)
            {
                em.StartFlows = json;
                em.Update();
            }
            return json;
        }
        /// <summary>
        /// 天业集团的发起，特殊处理.
        /// </summary>
        /// <returns></returns>
        public string Start_InitTianYe()
        {
            //如果请求了刷新.
            if (this.GetRequestVal("IsRef") != null)
            {
                //清除权限.
                DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows='' WHERE No='" + BP.Web.WebUser.No + "' ");

                //处理权限,为了防止未知的错误.
                DBAccess.RunSQL("UPDATE WF_FLOWSORT SET ORGNO='0' WHERE ORGNO='' OR ORGNO IS NULL OR ORGNO='101'");

                DBAccess.RunSQL("UPDATE wf_flowsort SET ORGNO = REPLACE(NO,'Inc','') where  no like 'Inc%'");
            }

            //需要翻译.
            BP.WF.Port.WFEmp em = new WFEmp();
            em.No = BP.Web.WebUser.No;
            if (em.RetrieveFromDBSources() == 0)
            {
                em.FK_Dept = BP.Web.WebUser.FK_Dept;
                em.Name = Web.WebUser.Name;
                em.Email = new BP.GPM.Emp(WebUser.No).Email;
                em.Insert();
            }
            string json = em.StartFlows;
            if (DataType.IsNullOrEmpty(json) == false)
            {
                return json;
            }

            //如果是节水公司的，就特别处理.
            if (WebUser.FK_Dept.IndexOf("18099") == 0)
            {
                return Start_InitTianYe_JieShui();
            }

            //获得当前人员的部门,根据部门获得该人员的组织集合.
            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //找到当前人员所在的部门集合, 应该找到他的组织集合为了减少业务逻辑.
            string orgNos = "'0'";
            foreach (DataRow dr in dt.Rows)
            {
                string deptNo = dr[0].ToString();
                orgNos += ",'" + deptNo + "'";
            }

            #region 获取类别列表(根据当前人员所在组织结构进行过滤类别.)
            FlowSorts fss = new FlowSorts();
            BP.En.QueryObject qo = new En.QueryObject(fss);
            if (orgNos.Contains(",") == false)
            {
                qo.AddWhere(FlowSortAttr.OrgNo, "0");  //..
                qo.addOr();
                qo.AddWhere(FlowSortAttr.OrgNo, "");  //..
            }
            else
            {
                qo.AddWhereIn(FlowSortAttr.OrgNo, "(" + orgNos + ")");  //指定的类别.
            }

            //排序.
            qo.addOrderBy(FlowSortAttr.No, FlowSortAttr.Idx);

            DataTable dtSort = qo.DoQueryToTable();
            dtSort.TableName = "Sort";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtSort.Columns["NO"].ColumnName = "No";
                dtSort.Columns["NAME"].ColumnName = "Name";
                dtSort.Columns["PARENTNO"].ColumnName = "ParentNo";
                dtSort.Columns["ORGNO"].ColumnName = "OrgNo";
            }

            //定义容器.
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSort); //增加到里面去.
            #endregion 获取类别列表.

            //构造流程实例数据容器。
            DataTable dtStart = new DataTable();
            dtStart.TableName = "Start";
            dtStart.Columns.Add("No");
            dtStart.Columns.Add("Name");
            dtStart.Columns.Add("FK_FlowSort");
            dtStart.Columns.Add("IsBatchStart");
            dtStart.Columns.Add("IsStartInMobile");
            //dtStart.Columns.Add("Note");

            //获得所有的流程（包含了所有子公司与集团的可以发起的流程但是没有根据组织结构进行过滤.）
            DataTable dtAllFlows = Dev2Interface.DB_StarFlows(Web.WebUser.No);

            //按照当前用户的流程类别权限进行过滤.
            foreach (DataRow drSort in dtSort.Rows)
            {
                foreach (DataRow drFlow in dtAllFlows.Rows)
                {
                    if (drSort["No"].ToString() != drFlow["FK_FlowSort"].ToString())
                    {
                        continue;
                    }

                    DataRow drNew = dtStart.NewRow();

                    drNew["No"] = drFlow["No"];
                    drNew["Name"] = drFlow["Name"];
                    drNew["FK_FlowSort"] = drFlow["FK_FlowSort"];
                    drNew["IsBatchStart"] = drFlow["IsBatchStart"];
                    drNew["IsStartInMobile"] = drFlow["IsStartInMobile"];
                    //drNew["Note"] = drFlow["Note"];
                    dtStart.Rows.Add(drNew); //增加到里里面去.
                }
            }

            //把经过权限过滤的流程实体放入到集合里.
            ds.Tables.Add(dtStart); //增加到里面去.

            //返回组合
            json = BP.Tools.Json.DataSetToJson(ds, false);

            //把json存入数据表，避免下一次再取.
            if (json.Length > 40)
            {
                em.StartFlows = json;
                em.Update();
            }

            return json;
        }
        /// <summary>
        /// 获得发起列表 
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            //通用的处理器.
            if (BP.Sys.SystemConfig.CustomerNo == "TianYe")
            {
                return Start_InitTianYe();
            }
            string json = "";

            BP.WF.Port.WFEmp em = new WFEmp();
            em.No = BP.Web.WebUser.No;
            if (DataType.IsNullOrEmpty(em.No) == true)
                return "err@ログイン情報が失われました。もう一度ログインしてください.";

            if (em.RetrieveFromDBSources() == 0)
            {
                em.FK_Dept = BP.Web.WebUser.FK_Dept;
                em.Name = Web.WebUser.Name;
                em.Insert();
            }
           
            json = BP.DA.DBAccess.GetBigTextFromDB("WF_Emp", "No", WebUser.No, "StartFlows");
            if (DataType.IsNullOrEmpty(json) == false)
                return json;

            //定义容器.
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = Dev2Interface.DB_StarFlows(Web.WebUser.No);
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);

            //返回组合
            json = BP.Tools.Json.DataSetToJson(ds, false);

            //把json存入数据表，避免下一次再取.
            BP.DA.DBAccess.SaveBigTextToDB(json, "WF_Emp", "No", WebUser.No, "StartFlows");

           

            //返回组合
            return json;
        }
        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <returns></returns>
        public string FlowSearch_Init()
        {
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = DBAccess.RunSQLReturnTable("SELECT No,Name, FK_FlowSort FROM WF_Flow ORDER BY FK_FlowSort,Idx");
            dtStart.TableName = "Start";

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtStart.Columns["NO"].ColumnName = "No";
                dtStart.Columns["NAME"].ColumnName = "Name";
                dtStart.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }

            ds.Tables.Add(dtStart);



            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        #region 获得列表.
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init()
        {
            DataTable dt = null;
            bool isContainFuture = this.GetRequestValBoolen("IsContainFuture");
            dt = BP.WF.Dev2Interface.DB_GenerRuning(isContainFuture);

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 我参与的已经完成的工作.
        /// </summary>
        /// <returns></returns>
        public string Complete_Init()
        {
            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT  * FROM WF_GenerWorkFlow  WHERE (Emps LIKE '%@" + WebUser.No + "@%' OR Emps LIKE '%@" + WebUser.No + ",%') and WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            //添加oracle的处理
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
                dt.Columns["TSPAN"].ColumnName = "TSpan";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["SENDDT"].ColumnName = "SendDT";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["PFID"].ColumnName = "PFID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["MYNUM"].ColumnName = "MyNum";
                dt.Columns["GUID"].ColumnName = "Guid";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["FK_NY"].ColumnName = "FK_NY";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["DOMAIN"].ColumnName = "Domain";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
            }

            if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["weeknum"].ColumnName = "WeekNum";
                dt.Columns["tspan"].ColumnName = "TSpan";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["senddt"].ColumnName = "SendDT";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["sdtofflow"].ColumnName = "SDTOfFlow";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["pfid"].ColumnName = "PFID";
                dt.Columns["pemp"].ColumnName = "PEmp";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["mynum"].ColumnName = "MyNum";
                dt.Columns["guid"].ColumnName = "Guid";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["fk_ny"].ColumnName = "FK_NY";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["emps"].ColumnName = "Emps";
                dt.Columns["domain"].ColumnName = "Domain";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["billno"].ColumnName = "BillNo";
            }

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string Runing_UnSend()
        {
            try
            {
                //获取撤销到的节点
                int unSendToNode = this.GetRequestValInt("UnSendToNode");
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID, unSendToNode, this.FID);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Runing_Press()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, this.GetRequestVal("Msg"), false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string Runing_OpenFrm()
        {
            int nodeID = this.FK_Node;
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (nodeID == 0)
            {
                gwf = new GenerWorkFlow(this.WorkID);
                nodeID = gwf.FK_Node;
            }

            string appPath = BP.WF.Glo.CCFlowAppPath;
            Node nd = null;
            Track tk = new Track();
            tk.FK_Flow = this.FK_Flow;


            tk.WorkID = this.WorkID;
            if (this.MyPK != null)
            {
                tk = new Track(this.FK_Flow, this.MyPK);
                nd = new Node(tk.NDFrom);
            }
            else
            {
                nd = new Node(nodeID);
            }

            Flow fl = new Flow(this.FK_Flow);
            Int64 workid = 0;
            if (nd.HisRunModel == RunModel.SubThread)
            {
                if (tk.FID == 0)
                {
                    if (gwf == null)
                    {
                        gwf = new GenerWorkFlow(this.WorkID);
                    }

                    workid = gwf.FID;
                }
                else
                {
                    workid = tk.FID;
                }
            }
            else
            {
                workid = tk.WorkID;
            }

            Int64 fid = this.FID;
            if (this.FID == 0)
            {
                fid = tk.FID;
            }

            if (fid > 0)
            {
                workid = fid;
            }

            if (workid == 0)
            {
                workid = this.WorkID;
            }

            string urlExt = "";

            // gwf.atPara.HisHT

            DataTable ndrpt = DBAccess.RunSQLReturnTable("SELECT PFlowNo,PWorkID FROM " + fl.PTable + " WHERE OID=" + workid);
            if (ndrpt.Rows.Count == 0)
            {
                urlExt = "&PFlowNo=0&PWorkID=0&IsToobar=0&IsHidden=true";
            }
            else
            {
                urlExt = "&PFlowNo=" + ndrpt.Rows[0]["PFlowNo"] + "&PWorkID=" + ndrpt.Rows[0]["PWorkID"] + "&IsToobar=0&IsHidden=true";
            }

            urlExt += "&From=CCFlow&TruckKey=" + tk.GetValStrByKey("MyPK") + "&DoType=" + this.DoType + "&UserNo=" + WebUser.No ?? string.Empty + "&SID=" + WebUser.SID ?? string.Empty;

            urlExt = urlExt.Replace("PFlowNo=null", "");
            urlExt = urlExt.Replace("PWorkID=null", "");

            if (gwf.atPara.HisHT.Count > 0)
            {
                foreach (var item in gwf.atPara.HisHT.Keys)
                {
                    urlExt += "&" + item + "=" + gwf.atPara.HisHT[item];
                }
            }


            if (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm)
            {
                //added by liuxc,2016-01-25
                if (nd.FormUrl.Contains("?"))
                    return "urlForm@" + nd.FormUrl + "&IsReadonly=1&WorkID=" + workid + "&FK_Node=" + nd.NodeID + "&FK_Flow=" + nd.FK_Flow + "&FID=" + fid + urlExt;

                return "urlForm@" + nd.FormUrl + "?IsReadonly=1&WorkID=" + workid + "&FK_Node=" + nd.NodeID + "&FK_Flow=" + nd.FK_Flow + "&FID=" + fid + urlExt;
            }

            if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.SheetAutoTree)
            {
                return "url@./MyFlowTreeReadonly.htm?3=4&WorkID=" + this.WorkID + "&FID=" + this.FID + "&OID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + this.WorkID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + urlExt;
            }

            Work wk = nd.HisWork;
            wk.OID = workid;
            if (wk.RetrieveFromDBSources() == 0)
            {
                GERpt rtp = nd.HisFlow.HisGERpt;
                rtp.OID = workid;
                if (rtp.RetrieveFromDBSources() == 0)
                {
                    string info = "開く(" + nd.Name + ")エラー";
                    info += "現在のノードデータは削除されました！ ！ ！ <br>この問題の原因は次のとおりです。";
                    info += "1、現在のノードデータが不正に削除されました。";
                    info += "2、ノードデータは返された人と返された人の間のノードです。ノードデータのこの部分はサポートされていません。";
                    info += "技術情報：テーブル" + wk.EnMap.PhysicsTable + " WorkID=" + workid;
                    return "err@" + info;
                }
                wk.Row = rtp.Row;
            }

            if (nd.HisFlow.IsMD5 && wk.IsPassCheckMD5() == false)
            {
                string err = "開く(" + nd.Name + ")エラー";
                err += "現在のノードデータは改ざんされています。管理者に報告してください。";
                return "err@" + err;
            }
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            if (nd.HisFormType == NodeFormType.FreeForm)
            {
                MapData md = new MapData(nd.NodeFrmID);
                if (md.HisFrmType != FrmType.FreeFrm)
                {
                    md.HisFrmType = FrmType.FreeFrm;
                    md.Update();
                }
            }
            else if (nd.HisFormType == NodeFormType.FoolForm)
            {
                nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
                MapData md = new MapData(nd.NodeFrmID);
                if (md.HisFrmType != FrmType.FoolForm)
                {
                    md.HisFrmType = FrmType.FoolForm;
                    md.Update();
                }
            }
            string endUrl = "";

            if (gwf.atPara.HisHT.Count > 0)
            {
                foreach (var item in gwf.atPara.HisHT.Keys)
                {
                    endUrl += "&" + item + "=" + gwf.atPara.HisHT[item];
                }
            }



            //加入是累加表单的标志，目的是让附件可以看到.

            if (nd.HisFormType == NodeFormType.FoolTruck)
            {
                endUrl = "&FormType=10&FromWorkOpt=" + this.GetRequestVal("FromWorkOpt");
            }

            return "url@./CCForm/Frm.htm?FK_MapData=" + nd.NodeFrmID + "&OID=" + wk.OID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + wk.OID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + endUrl;
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 删除草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Delete()
        {
            return BP.WF.Dev2Interface.Flow_DoDeleteDraft(this.FK_Flow, this.WorkID, false);
        }
        /// <summary>
        /// 获得会签列表
        /// </summary>
        /// <returns></returns>
        public string HuiQianList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B ";
            sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node ";
            sql += " AND (B.IsPass=90 OR A.AtPara LIKE '%HuiQianZhuChiRen=" + WebUser.No + "%') ";
            sql += " AND B.FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";

            Paras ps = new Paras();
            ps.Add("FK_Emp", WebUser.No);
            ps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 协作模式待办
        /// </summary>
        /// <returns></returns>
        public string TeamupList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B, WF_Node C ";
            sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node AND A.FK_Node=C.NodeID AND C.TodolistModel=1 ";
            sql += " AND B.IsPass=0 AND B.FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            //   sql += " AND B.IsPass=0 AND B.FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";

            Paras ps = new Paras();
            ps.Add("FK_Emp", WebUser.No);
            ps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得加签人的待办
        /// @LQ 
        /// </summary>
        /// <returns></returns>
        public string HuiQianAdderList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B, WF_Node C ";
            sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node AND B.IsPass=0 AND B.FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            sql += " AND B.AtPara LIKE '%IsHuiQian=1%' ";
            sql += " AND A.FK_Node=C.NodeID ";
            sql += " AND C.TodolistModel= 4";

            Paras ps = new Paras(); 
            ps.Add("FK_Emp", WebUser.No);
            ps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 初始化待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            string fk_node = this.GetRequestVal("FK_Node");
            string showWhat = this.GetRequestVal("ShowWhat");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, this.FK_Node, showWhat);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得授权人的待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Author()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(this.No, this.FK_Node);

            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TodolistOfAuth_Init()
        {
            return "err@まだ完全に再建されていません.";

            //DataTable dt = null;
            //foreach (BP.WF.Port.WFEmp item in ems)
            //{
            //    if (dt == null)
            //    {
            //        dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(item.No, null);
            //    }
            //    else
            //    {
            //    }
            //}

            //    return BP.Tools.Json.DataTableToJson(dt, false);

            //string fk_emp = this.FK_Emp;
            //if (fk_emp == null)
            //{
            //    //移除不需要前台看到的数据.
            //    DataTable dt = ems.ToDataTableField();
            //    dt.Columns.Remove("SID");
            //    dt.Columns.Remove("Stas");
            //    dt.Columns.Remove("Depts");
            //    dt.Columns.Remove("Msg");
            //    return BP.Tools.Json.DataTableToJson(dt, false);
            //}

        }
        /// <summary>
        /// 获得挂起.
        /// </summary>
        /// <returns></returns>
        public string HungUpList_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();

            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }

        public string FutureTodolist_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_FutureTodolist();

            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }

        
        #endregion 获得列表.


        #region 共享任务池.
        /// <summary>
        /// 初始化共享任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_TaskPool();

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 申请任务.
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Apply()
        {
            bool b = BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(this.WorkID);
            if (b == true)
            {
                return "正常に申請しました。";
            }
            else
            {
                return "err@申請は失敗しました。";
            }
        }
        /// <summary>
        /// 我申请下来的任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolApply_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_TaskPoolOfMyApply();

            return BP.Tools.Json.ToJson(dt);
        }
        public string TaskPoolApply_PutOne()
        {
            BP.WF.Dev2Interface.Node_TaskPoolPutOne(this.WorkID);
            return "正常に配置すると、他の同僚がこのジョブを表示できます。タスクプールで表示して再適用できます.";
        }
        #endregion

        #region 登录相关.
        /// <summary>
        /// 返回当前会话信息.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            Hashtable ht = new Hashtable();

            if (BP.Web.WebUser.NoOfRel == null)
            {
                ht.Add("UserNo", "");
            }
            else
            {
                ht.Add("UserNo", BP.Web.WebUser.No);
            }

            if (BP.Web.WebUser.IsAuthorize)
            {
                ht.Add("Auth", BP.Web.WebUser.Auth);
            }
            else
            {
                ht.Add("Auth", "");
            }

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 执行登录.
        /// </summary>
        /// <returns></returns>
        public string LoginSubmit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetValFromFrmByKey("TB_No");

            if (emp.RetrieveFromDBSources() == 0)
            {
                return "err@ユーザ名またはパスワードが間違っています。";
            }

            string pass = this.GetValFromFrmByKey("TB_PW");
            if (emp.Pass.Equals(pass) == false)
            {
                return "err@ユーザ名またはパスワードが間違っています。";
            }

            //让其登录.
            BP.WF.Dev2Interface.Port_Login(emp.No);

            return "ログインしました。";
        }
        /// <summary>
        /// 执行授权登录
        /// </summary>
        /// <returns></returns>
        public string AuthorList_LoginAs()
        {
            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.No);

            //if (wfemp.AuthorIsOK == false)
            //   return "err@授权登录失败！";

            BP.Port.Emp emp1 = new BP.Port.Emp(this.No);
            BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, BP.Web.WebUser.No, BP.Web.WebUser.Name);

            return "承認されたログインに成功しました！";
        }
        /// <summary>
        /// 批处理审批
        /// </summary>
        /// <returns></returns>
        public string Batch_Init()
        {
            string fk_node = GetRequestVal("FK_Node");

            //没有传FK_Node
            if (DataType.IsNullOrEmpty(fk_node))
            {
                string sql = "SELECT a.NodeID, a.Name,a.FlowName, COUNT(WorkID) AS NUM  FROM WF_Node a, WF_EmpWorks b WHERE A.NodeID=b.FK_Node AND B.FK_Emp='" + WebUser.No + "' AND b.WFState NOT IN (7) AND a.BatchRole!=0 GROUP BY A.NodeID, a.Name,a.FlowName ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return BP.Tools.Json.ToJson(dt);
            }


            return "";
        }

        public string BatchList_Init()
        {
            DataSet ds = new DataSet();

            string FK_Node = GetRequestVal("FK_Node");

            //获取节点信息
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Flow fl = nd.HisFlow;
            ds.Tables.Add(nd.ToDataTableField("WF_Node"));

            string sql = "";

            if (nd.HisRunModel == RunModel.SubThread)
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                          + " a , WF_EmpWorks b WHERE a.OID=B.FID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                          + " AND b.FK_Emp='" + WebUser.No + "'";
            }
            else
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            }

            //获取待审批的流程信息集合
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Batch_List";
            ds.Tables.Add(dt);

            //获取按钮权限
            BtnLab btnLab = new BtnLab(this.FK_Node);

            ds.Tables.Add(btnLab.ToDataTableField("Sys_BtnLab"));

            //获取报表数据
            string inSQL = "SELECT WorkID FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "' AND WFState!=7 AND FK_Node=" + this.FK_Node;
            Works wks = nd.HisWorks;
            wks.RetrieveInSQL(inSQL);

            ds.Tables.Add(wks.ToDataTableField("WF_Work"));

            //获取字段属性
            MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);

            //获取实际中需要展示的列
            string batchParas = nd.BatchParas;
            MapAttrs realAttr = new MapAttrs();
            if (DataType.IsNullOrEmpty(batchParas) == false)
            {
                string[] strs = batchParas.Split(',');
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str)
                        || str.Contains("@PFlowNo") == true)
                    {
                        continue;
                    }

                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                        {
                            continue;
                        }

                        realAttr.AddEntity(attr);
                    }
                }
            }

            ds.Tables.Add(realAttr.ToDataTableField("Sys_MapAttr"));

            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 批量发送
        /// </summary>
        /// <returns></returns>
        public string Batch_Send()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string[] strs = nd.BatchParas.Split(',');

            MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);

            //获取数据
            string sql = string.Format("SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}'", WebUser.No, this.FK_Node);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            int idx = -1;
            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                if (idx == nd.BatchListCount)
                {
                    break;
                }

                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string cb = this.GetValFromFrmByKey("CB_" + workid, "0");
                if (cb == "on")
                {
                    cb = "1";
                }
                else
                {
                    cb = "0";
                }

                if (cb == "0") //没有选中
                {
                    continue;
                }
                #region 给字段赋值
                Hashtable ht = new Hashtable();
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str))
                    {
                        continue;
                    }

                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                        {
                            continue;
                        }

                        if (attr.MyDataType == DataType.AppDateTime || attr.MyDataType == DataType.AppDate)
                        {

                            string val = this.GetValFromFrmByKey("TB_" + workid + "_" + attr.KeyOfEn, null);
                            ht.Add(str, val);
                            continue;
                        }


                        if (attr.UIContralType == BP.En.UIContralType.TB && attr.UIIsEnable == true)
                        {
                            string val = this.GetValFromFrmByKey("TB_" + workid + "_" + attr.KeyOfEn, null);
                            ht.Add(str, val);
                            continue;
                        }

                        if (attr.UIContralType == BP.En.UIContralType.DDL && attr.UIIsEnable == true)
                        {
                            string val = this.GetValFromFrmByKey("DDL_" + workid + "_" + attr.KeyOfEn);
                            ht.Add(str, val);
                            continue;
                        }

                        if (attr.UIContralType == BP.En.UIContralType.CheckBok && attr.UIIsEnable == true)
                        {
                            string val = this.GetValFromFrmByKey("CB_" + +workid + "_" + attr.KeyOfEn, "-1");
                            if (val == "-1")
                            {
                                ht.Add(str, 0);
                            }
                            else
                            {
                                ht.Add(str, 1);
                            }

                            continue;
                        }
                    }
                }
                #endregion 给字段赋值

                //获取审核意见的值

                string checkNote = this.GetValFromFrmByKey("TB_" + workid + "_WorkCheck_Doc", null);
                if (DataType.IsNullOrEmpty(checkNote) == false)
                {
                    BP.WF.Dev2Interface.WriteTrackWorkCheck(nd.FK_Flow, nd.NodeID, workid, Int64.Parse(dr["FID"].ToString()), checkNote, null);
                }

                msg += "@仕事(" + dr["Title"] + ")処理は次のとおりです";
                BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(nd.FK_Flow, workid, ht);
                msg += objs.ToMsgOfHtml();
                msg += "<br/>";
            }

            if (msg == "")
            {
                msg = "処理する作業を選択しませんでした";
            }

            return msg;
        }

        /// <summary>
        /// 批量退回 待定
        /// </summary>
        /// <returns></returns>
        public string Batch_Return()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string[] strs = nd.BatchParas.Split(',');

            MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);

            //获取数据
            string sql = string.Format("SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}'", WebUser.No, this.FK_Node);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            int idx = -1;
            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                if (idx == nd.BatchListCount)
                {
                    break;
                }

                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string cb = this.GetValFromFrmByKey("CB_" + workid, "0");
                if (cb == "0") //没有选中
                {
                    continue;
                }

                msg += "@仕事(" + dr["Title"] + ")処理は以下のとおりです。 <br>";
                BP.WF.SendReturnObjs objs = null;// BP.WF.Dev2Interface.Node_ReturnWork(nd.FK_Flow, workid,fid,this.FK_Node,"バッチリターン");
                msg += objs.ToMsgOfHtml();
                msg += "<hr>";

            }
            return "進行中の作業";
        }

        /// <summary>
        /// 授权列表
        /// </summary>
        /// <returns></returns>
        public string AuthorTodolist_Init()
        {

            return "";
        }

        /// <summary>
        /// 授权列表
        /// </summary>
        /// <returns></returns>
        public string AuthorTodolist_Todolist()
        {

            return "";
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <returns></returns>
        public string Batch_Delete()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string[] strs = nd.BatchParas.Split(',');

            MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);

            //获取数据
            string sql = string.Format("SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}'", WebUser.No, this.FK_Node);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            int idx = -1;
            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                if (idx == nd.BatchListCount)
                {
                    break;
                }

                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                string cb = this.GetValFromFrmByKey("CB_" + workid, "0");
                if (cb == "0") //没有选中
                {
                    continue;
                }

                msg += "@仕事(" + dr["Title"] + ")処理は以下のとおりです。 <br>";
                string mes = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(nd.FK_Flow, workid, "バッチリターン", true);
                msg += mes;
                msg += "<hr>";

            }
            if (msg == "")
            {
                msg = "処理する作業を選択しませんでした";
            }

            return "バッチ削除しました" + msg;
        }


        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="Author"></param>
        /// <returns></returns>
        public string AuthExitAndLogin(string UserNo, string Author)
        {
            string msg = "suess@正常に終了します！";
            try
            {
                BP.Port.Emp emp = new BP.Port.Emp(UserNo);
                //首先退出
                BP.Web.WebUser.Exit();
                //再进行登录
                BP.Port.Emp emp1 = new BP.Port.Emp(Author);
                BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, null, null);
            }
            catch (Exception ex)
            {
                msg = "err@終了中にエラーが発生しました:" + ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 获取授权人列表
        /// </summary>
        /// <returns></returns>
        public string AuthorList_Init()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT No,Name,AuthorDate FROM WF_Emp WHERE AUTHOR=" + SystemConfig.AppCenterDBVarStr + "AUTHOR";
            ps.Add("AUTHOR", BP.Web.WebUser.No);
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["AUTHORDATE"].ColumnName = "AuthorDate";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 当前登陆人是否有授权
        /// </summary>
        /// <returns></returns>
        public string IsHaveAuthor()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_EMP WHERE AUTHOR=" + SystemConfig.AppCenterDBVarStr + "AUTHOR";
            ps.Add("AUTHOR", BP.Web.WebUser.No);
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            WFEmp em = new WFEmp();
            em.Retrieve(WFEmpAttr.Author, BP.Web.WebUser.No);

            if (dt.Rows.Count > 0 && BP.Web.WebUser.IsAuthorize == false)
            {
                return "suess@認可済み";
            }
            else
            {
                return "err@承認なし";
            }
        }
        /// <summary>
        /// 退出.
        /// </summary>
        /// <returns></returns>
        public string LoginExit()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return null;
        }
        /// <summary>
        /// 授权退出.
        /// </summary>
        /// <returns></returns>
        public string AuthExit()
        {
            return this.AuthExitAndLogin(this.No, BP.Web.WebUser.Auth);
        }
        #endregion 登录相关.

        /// <summary>
        /// 获得抄送列表
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
        {
            string sta = this.GetRequestVal("Sta");
            if (sta == null || sta == "")
                sta = "-1";

            int pageSize = 6; // int.Parse(pageSizeStr);

            string pageIdxStr = this.GetRequestVal("PageIdx");
            if (pageIdxStr == null)
                pageIdxStr = "1";

            int pageIdx = int.Parse(pageIdxStr);

            //实体查询.
            //BP.WF.SMSs ss = new BP.WF.SMSs();
            //BP.En.QueryObject qo = new BP.En.QueryObject(ss);

            System.Data.DataTable dt = null;
            if (sta == "-1")
                dt = BP.WF.Dev2Interface.DB_CCList(BP.Web.WebUser.No);

            if (sta == "0")
                dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);

            if (sta == "1")
                dt = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);

            if (sta == "2")
                dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);

            //int allNum = qo.GetCount();
            //qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);

            return BP.Tools.Json.ToJson(dt);
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Search_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");

            if (tSpan == "")
                tSpan = null;
            //查询关键字
            string keyWord = this.GetRequestVal("KeyWord");
            if (("").Equals(keyWord))
            {
                keyWord = null;
            }
            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            if (this.FK_Flow == null)
                sql = "SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "') AND FID = 0 AND WFState > 1 GROUP BY TSpan";
            else
                sql = "SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.FK_Flow + "' AND (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "') AND FID = 0  AND WFState > 1 GROUP BY TSpan";

            DataTable dtTSpanNum = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow drEnum in dtTSpan.Rows)
            {
                string no = drEnum["IntKey"].ToString();
                foreach (DataRow dr in dtTSpanNum.Rows)
                {
                    if (dr["No"].ToString() == no)
                    {
                        drEnum["Lab"] = drEnum["Lab"].ToString() + "(" + dr["Num"] + ")";
                        break;
                    }
                }
            }
            #endregion

            #region 2、处理流程类别列表.
            if (tSpan == "-1")
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0 GROUP BY FK_Flow, FlowName";
            else
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE TSpan=" + tSpan + " AND (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0 GROUP BY FK_Flow, FlowName";

            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion

            #region 3、处理流程实例列表.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            String sqlWhere = "";
            //当前页
            int pageIdx = 1;
            if (DataType.IsNullOrEmpty(this.GetRequestVal("pageIdx")) == false)
                pageIdx = int.Parse(this.GetRequestVal("pageIdx"));
            //每页条数
            int pageSize = 10;
            if (DataType.IsNullOrEmpty(this.GetRequestVal("pageSize")) == false)
                pageSize = int.Parse(this.GetRequestVal("pageSize"));

            int num = pageSize * (pageIdx - 1);
            sqlWhere = "(((Emps LIKE '%" + WebUser.No + "%')OR(TodoEmps LIKE '%" + WebUser.No + "%')OR(Starter = '" + WebUser.No + "')) AND (FID = 0) AND (WFState > 1)";
            if (tSpan != "-1")
            {
                sqlWhere += "AND (TSpan = '" + tSpan + "') ";
            }
            if (keyWord != null)
            {
                sqlWhere += "AND (Title like '%" + keyWord + "%') ";
            }
            if (this.FK_Flow != null)
            {
                sqlWhere += "AND (FK_Flow = '" + this.FK_Flow + "')) ";
            }
            else
            {
                sqlWhere += ")";
            }


            //获取总条数
            string totalNumSql = "SELECT count(*) from WF_GenerWorkFlow where " + sqlWhere;
            int totalNum = BP.DA.DBAccess.RunSQLReturnValInt(totalNumSql);
            int totalPage = 0;
            //当前页开始索引
            int startIndex = (pageIdx - 1) * pageSize;
            //总页数
            if (totalNum % pageSize != 0)
            {
                totalPage = totalNum / pageSize + 1;
            }
            else
            {
                totalPage = totalNum / pageSize;
            }

            /*
             * 分页信息放到table
             */
            DataTable dtT = new DataTable();
            dtT.Columns.Add("totalPage");
            dtT.Columns.Add("totalNum");
            dtT.Columns.Add("startIndex");
            dtT.TableName = "PageInfo";
            DataRow row = dtT.NewRow();

            row["totalPage"] = totalPage;
            row["totalNum"] = totalNum;
            row["startIndex"] = startIndex;
            dtT.Rows.Add(row);


            ds.Tables.Add(dtT);
            sqlWhere += "ORDER BY RDT DESC";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT NVL(WorkID, 0) WorkID,NVL(FID, 0) FID ,FK_Flow,FlowName,Title, NVL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,NVL(RDT, '2018-05-04 19:29') RDT,NVL(FK_Node, 0) FK_Node,NodeName, TodoEmps " +
                    "FROM (select A.*, rownum r from (select * from WF_GenerWorkFlow where " + sqlWhere + ") A) where r between " + (pageIdx * pageSize - pageSize + 1) + " and " + (pageIdx * pageSize);
            else if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP " + pageSize + " ISNULL(WorkID, 0) WorkID,ISNULL(FID, 0) FID ,FK_Flow,FlowName,Title, ISNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,ISNULL(RDT, '2018-05-04 19:29') RDT,ISNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow " +
                    "where WorkID not in (select top(" + num + ") WorkID from WF_GenerWorkFlow where " + sqlWhere + ") AND" + sqlWhere;
            //sql = "SELECT  TOP 500 ISNULL(WorkID, 0) WorkID,ISNULL(FID, 0) FID ,FK_Flow,FlowName,Title, ISNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,ISNULL(RDT, '2018-05-04 19:29') RDT,ISNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                sql = "SELECT IFNULL(WorkID, 0) WorkID,IFNULL(FID, 0) FID ,FK_Flow,FlowName,Title, IFNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,IFNULL(RDT, '2018-05-04 19:29') RDT,IFNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where (1=1) AND " + sqlWhere + " LIMIT " + startIndex + "," + pageSize;
            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                sql = "SELECT COALESCE(WorkID, 0) WorkID,COALESCE(FID, 0) FID ,FK_Flow,FlowName,Title, COALESCE(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,COALESCE(RDT, '2018-05-04 19:29') RDT,COALESCE(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where (1=1) AND " + sqlWhere + " LIMIT " + pageSize + "offset " + startIndex;
            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                mydt.Columns[0].ColumnName = "WorkID";
                mydt.Columns[1].ColumnName = "FID";
                mydt.Columns[2].ColumnName = "FK_Flow";
                mydt.Columns[3].ColumnName = "FlowName";
                mydt.Columns[4].ColumnName = "Title";
                mydt.Columns[5].ColumnName = "WFSta";
                mydt.Columns[6].ColumnName = "WFState";
                mydt.Columns[7].ColumnName = "Starter";
                mydt.Columns[8].ColumnName = "StarterName";
                mydt.Columns[9].ColumnName = "Sender";
                mydt.Columns[10].ColumnName = "RDT";
                mydt.Columns[11].ColumnName = "FK_Node";
                mydt.Columns[12].ColumnName = "NodeName";
                mydt.Columns[13].ColumnName = "TodoEmps";


            }
            mydt.TableName = "WF_GenerWorkFlow";
            if (mydt != null)
            {
                mydt.Columns.Add("TDTime");
                foreach (DataRow dr in mydt.Rows)
                {
                    dr["TDTime"] = GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            #endregion


            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
        public static string GetTraceNewTime(string fk_flow, Int64 workid, Int64 fid)
        {
            #region 获取track数据.
            string sqlOfWhere2 = "";
            string sqlOfWhere1 = "";
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (fid == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                ps.Add("WorkID11", workid);
                ps.Add("WorkID12", workid);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                ps.Add("FID11", fid);
                ps.Add("FID12", fid);
            }

            string sql = "";
            sql = "SELECT MAX(RDT) FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
            sql = "SELECT RDT FROM  ND" + int.Parse(fk_flow) + "Track  WHERE RDT=(" + sql + ")";
            ps.SQL = sql;

            try
            {
                return DBAccess.RunSQLReturnString(ps);
            }
            catch
            {
                // 处理track表.
                Track.CreateOrRepairTrackTable(fk_flow);
                return DBAccess.RunSQLReturnString(ps);
            }
            #endregion 获取track数据.
        }
        #region 处理page接口.
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                return this.GetRequestVal("DoWhat");
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {
                return this.GetRequestVal("UserNo");
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                return this.GetRequestVal("SID");
            }
        }
        /// <summary>
        /// 调用页面入口
        /// </summary>
        /// <returns></returns>
        public string Port_Init()
        {
            #region 安全性校验.
            //if (this.UserNo == null )
            //    return "err@必要的参数没有传入，请参考接口规则。UserNo";

            if (this.SID == null)
                return "err@必要なパラメーターが渡されていません。インターフェースのルールを参照してください。 SID";

            if (this.DoWhat == null)
                return "err@必要なパラメーターが渡されていません。インターフェースのルールを参照してください。DoWhat";

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
                return "err@不正アクセスです。管理者に連絡してください。 SID=" + this.SID;
            else
                BP.WF.Dev2Interface.Port_Login(this.UserNo);

            //if (DataType.IsNullOrEmpty(WebUser.No) == true || BP.Web.WebUser.No.Equals(this.UserNo) == false)
            //{
            //    BP.WF.Dev2Interface.Port_SigOut();
            //    try
            //    {
            //        BP.WF.Dev2Interface.Port_Login(this.UserNo);
            //    }
            //    catch (Exception ex)
            //    {
            //        return "err@安全校验出现错误:" + ex.Message;
            //    }
            //}
            #endregion 安全性校验.

            if (this.DoWhat.Equals("PortLogin") == true)
            {
                return "ログインしました。";
            }

            #region 生成参数串.
            string paras = "";
            foreach (string str in HttpContextHelper.RequestQueryStringKeys)
            {
                string val = this.GetRequestVal(str);
                if (val.IndexOf('@') != -1)
                {
                    return "err@パラメータ: [ " + str + " ," + val + " ] に渡された値では、URLは実行されません。";
                }

                switch (str)
                {
                    case DoWhatList.DoNode:
                    case DoWhatList.Emps:
                    case DoWhatList.EmpWorks:
                    case DoWhatList.FlowSearch:
                    case DoWhatList.Login:
                    case DoWhatList.MyFlow:
                    case DoWhatList.MyWork:
                    case DoWhatList.Start:
                    case DoWhatList.Start5:
                    case DoWhatList.StartSimple:
                    case DoWhatList.FlowFX:
                    case DoWhatList.DealWork:
                    case "FK_Flow":
                    case "WorkID":
                    case "FK_Node":
                    case "SID":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            string nodeID = int.Parse(this.FK_Flow + "01").ToString();
            #endregion 生成参数串.


            //发起流程.
            if (this.DoWhat.Equals("StartClassic") == true)
            {
                if (this.FK_Flow == null)
                {
                    return "url@./AppClassic/Home.htm";
                }
                else
                {
                    return "url@./AppClassic/Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID;
                }
            }

            //打开工作轨迹。
            if (this.DoWhat.Equals(DoWhatList.OneWork) == true)
            {
                if (this.FK_Flow == null || this.WorkID == null)
                {
                    throw new Exception("@パラメータFK_FlowまたはWorkIDがNullです。");
                }

                return "url@WFRpt.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //发起页面.
            if (this.DoWhat.Equals(DoWhatList.Start) == true)
            {
                if (this.FK_Flow == null)
                {
                    return "url@Start.htm";
                }
                else
                {
                    return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID;
                }
            }

            //处理工作.
            if (this.DoWhat.Equals(DoWhatList.DealWork) == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow) || this.WorkID == 0)
                {
                    return "err@パラメータFK_FlowまたはWorkIDがNullです。";
                }
                return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //请求在途.
            if (this.DoWhat.Equals(DoWhatList.Runing) == true)
                return "url@Runing.htm?FK_Flow=" + this.FK_Flow;

            //请求在途.
            if (this.DoWhat.Equals("Home") == true)
                return "url@Home.htm?FK_Flow=" + this.FK_Flow;

            //请求在途.
            if (this.DoWhat.Equals(DoWhatList.Runing) == true)
                return "url@Runing.htm?FK_Flow=" + this.FK_Flow;


            //请求待办。
            if (this.DoWhat.Equals(DoWhatList.EmpWorks) == true || this.DoWhat.Equals("Todolist") == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow))
                {
                    return "url@Todolist.htm";
                }
                else
                {
                    return "url@Todolist.htm?FK_Flow=" + this.FK_Flow;
                }
            }

            //请求流程查询。
            if (this.DoWhat.Equals(DoWhatList.FlowSearch) == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow))
                {
                    return "url@./RptSearch/Default.htm";
                }
                else
                {
                    return "url@./RptDfine/Search.htm?2=1&FK_Flow=001&EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras;
                }
            }

            //流程查询小页面.
            if (this.DoWhat.Equals(DoWhatList.FlowSearchSmall) == true)
            {
                if (this.FK_Flow == null)
                {
                    return "url@./RptSearch/Default.htm";
                }
                else
                {
                    return "url./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras;
                }
            }

            //打开消息.
            if (this.DoWhat.Equals(DoWhatList.DealMsg) == true)
            {
                string guid = this.GetRequestVal("GUID");
                BP.WF.SMS sms = new SMS();
                sms.MyPK = guid;
                sms.Retrieve();

                //判断当前的登录人员.
                if (BP.Web.WebUser.No != sms.SendToEmpNo)
                {
                    BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);
                }

                BP.DA.AtPara ap = new AtPara(sms.AtPara);
                switch (sms.MsgType)
                {
                    case SMSMsgType.SendSuccess: // 发送成功的提示.

                        if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValInt64ByKey("WorkID"), BP.Web.WebUser.No) == true)
                        {
                            return "url@MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        }
                        else
                        {
                            return "url@WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        }

                    default: //其他的情况都是查看工作报告.
                        return "url@WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                }
            }

            return "err@合意されたマークなし：DoWhat=" + this.DoWhat;
        }
        #endregion 处理page接口.

    }
}
