using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Port;
using System.Drawing.Imaging;
using System.Drawing;
using System.Configuration;
using BP.Tools;

namespace BP.WF
{
    public class CCFlowAPI
    {
        /// <summary>
        /// 产生一个WorkNode 
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>返回dataset</returns>
        public static DataSet GenerWorkNode(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo, string fromWorkOpt = "0")
        {
            //节点.
            if (fk_node == 0)
                fk_node = int.Parse(fk_flow + "01");

            if (workID == 0)
                workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);

            Node nd = new Node(fk_node);        
            try
            {
                nd.WorkID = workID; //为获取表单ID提供参数.
                MapData md = new MapData(nd.NodeFrmID);

                Work wk = nd.HisWork;
                wk.OID = workID;

                wk.RetrieveFromDBSources();
                wk.ResetDefaultVal();

                // 第1.2: 调用,处理用户定义的业务逻辑.
                string sendWhen = nd.HisFlow.DoFlowEventEntity(EventListOfNode.FrmLoadBefore, nd,
                    wk, null);


                //获得表单模版.
                DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No, nd.Name);

                if (DataType.IsNullOrEmpty(nd.NodeFrmID) == false
                    && (nd.HisFormType== NodeFormType.FoolForm || nd.HisFormType == NodeFormType.FreeForm))
                {
                    string name  =  md.Name;
                    if (DataType.IsNullOrEmpty(md.Name) == true)
                        name = nd.Name;

                    myds.Tables["Sys_MapData"].Rows[0]["Name"]= name;
                }

                //移除MapAttr
                myds.Tables.Remove("Sys_MapAttr"); //移除.

                //获取表单的mapAttr
                //求出集合.
                MapAttrs mattrs = md.MapAttrs; // new MapAttrs(md.No);
                
                /*处理表单权限控制方案*/
                FrmNode frmNode = new FrmNode();
                int count = frmNode.Retrieve(FrmNodeAttr.FK_Frm, md.No, FrmNodeAttr.FK_Node, fk_node);
                if (count != 0 && frmNode.FrmSln != 0)
                {
                    FrmFields fls = new FrmFields(md.No, frmNode.FK_Node);
                    foreach (FrmField item in fls)
                    {
                        foreach (MapAttr attr in mattrs)
                        {
                            if (attr.KeyOfEn != item.KeyOfEn)
                                continue;

                            if (item.IsSigan)
                                item.UIIsEnable = false;

                            attr.UIIsEnable = item.UIIsEnable;
                            attr.UIVisible = item.UIVisible;
                            attr.IsSigan = item.IsSigan;
                            attr.DefValReal = item.DefVal;
                        }
                    }
                }
                 

                DataTable Sys_MapAttr = mattrs.ToDataTableField("Sys_MapAttr");
                myds.Tables.Add(Sys_MapAttr);

                //把流程信息表发送过去.
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = workID;
                gwf.RetrieveFromDBSources();
                myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));
                //加入WF_Node.
                DataTable WF_Node = nd.ToDataTableField("WF_Node");
                myds.Tables.Add(WF_Node);

                #region 加入组件的状态信息, 在解析表单的时候使用.
                BP.WF.Template.FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);

                nd.WorkID = workID; //为获取表单ID提供参数.
                if (nd.NodeFrmID != "ND" + nd.NodeID && nd.HisFormType != NodeFormType.RefOneFrmTree)
                {
                    /*说明这是引用到了其他节点的表单，就需要把一些位置元素修改掉.*/

                    int refNodeID = int.Parse(nd.NodeFrmID.Replace("ND", ""));

                    BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(refNodeID);

                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H));
                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W));
                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X));
                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y));

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, 500);

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, 600);

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, 200);

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, 200);


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

                    #region 没有审核组件分组就增加上审核组件分组. @杜需要翻译&测试.
                    if (md.HisFrmType == FrmType.FoolForm)
                    {
                        //判断是否是傻瓜表单，如果是，就要判断该傻瓜表单是否有审核组件groupfield ,没有的话就增加上.
                        DataTable gf = myds.Tables["Sys_GroupField"];
                        bool isHave = false;
                        foreach (DataRow dr in gf.Rows)
                        {
                            string cType = dr["CtrlType"] as string;
                            if (cType == null)
                                continue;

                            if (cType.Equals("FWC") == true)
                                isHave = true;
                        }

                        if (isHave == false)
                        {
                            DataRow dr = gf.NewRow();

                            nd.WorkID = workID; //为获取表单ID提供参数.
                            dr[GroupFieldAttr.OID] = 100;
                            dr[GroupFieldAttr.FrmID] = nd.NodeFrmID;
                            dr[GroupFieldAttr.CtrlType] = "FWC";
                            dr[GroupFieldAttr.CtrlID] = "FWCND" + nd.NodeID;
                            dr[GroupFieldAttr.Idx] = 100;
                            dr[GroupFieldAttr.Lab] = "審査情報";
                            gf.Rows.Add(dr);

                            myds.Tables.Remove("Sys_GroupField");
                            myds.Tables.Add(gf);

                            //执行更新,就自动生成那个丢失的字段分组.
                            refFnc.Update();
                        }

                    }
                    #endregion 没有审核组件分组就增加上审核组件分组.

                }

                #region 没有审核组件分组就增加上审核组件分组. @杜需要翻译&测试.
                if (nd.NodeFrmID == "ND" + nd.NodeID || (nd.HisFormType == NodeFormType.RefOneFrmTree && count != 0))
                {
                    bool isHaveFWC = false;
                    //绑定表单库中的表单
                    if ((count != 0 && frmNode.IsEnableFWC == true || nd.NodeFrmID == "ND" + nd.NodeID) && nd.FrmWorkCheckSta != FrmWorkCheckSta.Disable)
                        isHaveFWC = true;
                 
                    if (nd.FormType == NodeFormType.FoolForm && isHaveFWC == true)
                    {
                        //判断是否是傻瓜表单，如果是，就要判断该傻瓜表单是否有审核组件groupfield ,没有的话就增加上.
                        DataTable gf = myds.Tables["Sys_GroupField"];
                        bool isHave = false;
                        foreach (DataRow dr in gf.Rows)
                        {
                            string cType = dr["CtrlType"] as string;
                            if (cType == null)
                                continue;

                            if (cType.Equals("FWC") == true)
                                isHave = true;
                        }

                        if (isHave == false)
                        {
                            DataRow dr = gf.NewRow();

                            nd.WorkID = workID; //为获取表单ID提供参数.
                            dr[GroupFieldAttr.OID] = 100;
                            dr[GroupFieldAttr.FrmID] = nd.NodeFrmID;
                            dr[GroupFieldAttr.CtrlType] = "FWC";
                            dr[GroupFieldAttr.CtrlID] = "FWCND" + nd.NodeID;
                            dr[GroupFieldAttr.Idx] = 100;
                            dr[GroupFieldAttr.Lab] = "審査情報";
                            gf.Rows.Add(dr);

                            myds.Tables.Remove("Sys_GroupField");
                            myds.Tables.Add(gf);

                            //更新,为了让其自动增加审核分组.
                            BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(nd.NodeID);
                            refFnc.Update();

                        }
                    }
                }
                #endregion 没有审核组件分组就增加上审核组件分组.

                myds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent"));

                #endregion 加入组件的状态信息, 在解析表单的时候使用.

                #region 增加 groupfields
                if (nd.FormType == NodeFormType.FoolTruck && nd.IsStartNode == false
                    && DataType.IsNullOrEmpty(wk.HisPassedFrmIDs) == false)
                {

                    #region 处理字段分组排序.
                    //查询所有的分组, 如果是查看表单的方式，就不应该把当前的表单显示出来.
                    string myFrmIDs = "";
                    if (fromWorkOpt.Equals("1") == true)
                    {
                        if (gwf.WFState == WFState.Complete)
                            myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + fk_node + "'";
                        else
                            myFrmIDs = wk.HisPassedFrmIDs; //流程未完成并且是查看表单的情况.
                    }
                    else
                    {
                        myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + fk_node + "'";
                    }

                    GroupFields gfs = new GroupFields();
                    gfs.RetrieveIn(GroupFieldAttr.FrmID, "(" + myFrmIDs + ")");

                    //按照时间的顺序查找出来 ids .
                    string sqlOrder = "SELECT OID FROM  Sys_GroupField WHERE   FrmID IN (" + myFrmIDs + ")";
                    myFrmIDs = myFrmIDs.Replace("'", "");
                    if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
                    {
                        sqlOrder += " ORDER BY INSTR('" + myFrmIDs + "',FrmID) , Idx";
                    }

                    if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MSSQL)
                    {
                        sqlOrder += " ORDER BY CHARINDEX(FrmID, '" + myFrmIDs + "'), Idx";
                    }

                    if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
                    {
                        sqlOrder += " ORDER BY INSTR('" + myFrmIDs + "', FrmID ), Idx";
                    }
                    if (BP.Sys.SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    {
                        sqlOrder += " ORDER BY POSITION(FrmID  IN '" + myFrmIDs + "'), Idx";
                    }

                    if (BP.Sys.SystemConfig.AppCenterDBType == DBType.DM)
                    {
                        sqlOrder += " ORDER BY POSITION(FrmID  IN '" + myFrmIDs + "'), Idx";
                    }

                    DataTable dtOrder = DBAccess.RunSQLReturnTable(sqlOrder);

                    //创建容器,把排序的分组放入这个容器.
                    GroupFields gfsNew = new GroupFields();

                    //遍历查询出来的分组.
                    //只能增加一个审核分组
                    GroupField FWCG = null;
                    foreach (DataRow dr in dtOrder.Rows)
                    {
                        string pkOID = dr[0].ToString();
                        GroupField mygf = gfs.GetEntityByKey(pkOID) as GroupField;
                        if (mygf.CtrlType.Equals("FWC"))
                        {
                            FWCG = mygf;
                            continue;
                        }
                           
                        gfsNew.AddEntity(mygf); //把分组字段加入里面去.
                    }
                    if (FWCG != null)
                        gfsNew.AddEntity(FWCG);

                    DataTable dtGF = gfsNew.ToDataTableField("Sys_GroupField");
                    myds.Tables.Remove("Sys_GroupField");
                    myds.Tables.Add(dtGF);
                    #endregion 处理字段分组排序.

                    #region 处理 mapattrs
                    //求当前表单的字段集合.
                    MapAttrs attrs = new MapAttrs();
                    QueryObject qo = new QueryObject(attrs);
                    qo.AddWhere(MapAttrAttr.FK_MapData, "ND" + nd.NodeID);
                    qo.addOrderBy(MapAttrAttr.Idx);
                    qo.DoQuery();

                    //获取走过节点的表单方案
                    MapAttrs attrsLeiJia = new MapAttrs();

                    //存在表单方案只读
                    string sql1 = "Select FK_Frm From WF_FrmNode Where FK_Frm In("+wk.HisPassedFrmIDs+") And FrmSln="+(int)FrmSln.Readonly +" And FK_Node="+nd.NodeID;
                    DataTable dt1 = DBAccess.RunSQLReturnTable(sql1);
                    if(dt1.Rows.Count > 0){
                        //获取节点
                        string nodes ="";
                        foreach(DataRow dr in dt1.Rows)
                            nodes+="'"+dr[0].ToString()+"',";

                        nodes = nodes.Substring(0,nodes.Length-1);
                        qo = new QueryObject(attrsLeiJia);
                        qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" +nodes + ")");
                        qo.addOrderBy(MapAttrAttr.Idx);
                        qo.DoQuery();

                        foreach(MapAttr item in attrsLeiJia){
                            if (item.KeyOfEn.Equals("RDT") || item.KeyOfEn.Equals("Rec"))
                                continue;
                            item.UIIsEnable = false; //设置为只读的.
                            attrs.AddEntity(item);
                        }

                    }

                    //存在表单方案默认
                    sql1 = "Select FK_Frm From WF_FrmNode Where FK_Frm In(" + wk.HisPassedFrmIDs + ") And FrmSln=" + (int)FrmSln.Default + " And FK_Node=" + nd.NodeID;
                    dt1 = DBAccess.RunSQLReturnTable(sql1);
                     if(dt1.Rows.Count > 0){
                         //获取节点
                        string nodes ="";
                        foreach(DataRow dr in dt1.Rows)
                            nodes+="'"+dr[0].ToString()+"',";

                        nodes = nodes.Substring(0,nodes.Length-1);
                        qo = new QueryObject(attrsLeiJia);
                        qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" +nodes + ")");
                        qo.addOrderBy(MapAttrAttr.Idx);
                        qo.DoQuery();

                        foreach (MapAttr item in attrsLeiJia)
                        {
                            if (item.KeyOfEn.Equals("RDT") || item.KeyOfEn.Equals("Rec"))
                                continue;
                            attrs.AddEntity(item);
                        }

                    }

                    //存在表单方案自定义
                     sql1 = "Select FK_Frm From WF_FrmNode Where FK_Frm In(" + wk.HisPassedFrmIDs + ") And FrmSln=" + (int)FrmSln.Self + " And FK_Node=" + nd.NodeID;
                    dt1 = DBAccess.RunSQLReturnTable(sql1);

                     if(dt1.Rows.Count > 0){
                         //获取节点
                        string nodes ="";
                        foreach(DataRow dr in dt1.Rows)
                            nodes+="'"+dr[0].ToString()+"',";

                        nodes = nodes.Substring(0,nodes.Length-1);
                        qo = new QueryObject(attrsLeiJia);
                        qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" +nodes + ")");
                        qo.addOrderBy(MapAttrAttr.Idx);
                        qo.DoQuery();

                         //获取累加表单的权限
                        FrmFields fls = new FrmFields();
                        qo = new QueryObject(fls);
                        qo.AddWhere(FrmFieldAttr.FK_MapData, " IN ", "(" + nodes + ")");
                        qo.addAnd();
                        qo.AddWhere(FrmFieldAttr.EleType, FrmEleType.Field);
                        qo.addAnd();
                        qo.AddWhere(FrmFieldAttr.FK_Node, nd.NodeID);
                        qo.DoQuery();

                        foreach (MapAttr attr in attrsLeiJia)
                        {
                            if (attr.KeyOfEn.Equals("RDT") || attr.KeyOfEn.Equals("Rec"))
                                continue;

                            FrmField frmField = null;
                            foreach (FrmField item in fls)
                            {
                                if (attr.KeyOfEn == item.KeyOfEn)
                                {
                                    frmField = item;
                                    break;
                                }
                            }
                            if (frmField != null)
                            {
                                if (frmField.IsSigan)
                                    attr.UIIsEnable = false;

                                attr.UIIsEnable = frmField.UIIsEnable;
                                attr.UIVisible = frmField.UIVisible;
                                attr.IsSigan = frmField.IsSigan;
                                attr.DefValReal = frmField.DefVal;
                            }
                            attrs.AddEntity(attr);
                        }

                    }

                    //替换掉现有的.
                    myds.Tables.Remove("Sys_MapAttr"); //移除.
                    myds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr")); //增加.
                    #endregion 处理mapattrs

                    #region 把枚举放入里面去.
                    myds.Tables.Remove("Sys_Enum");

                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + fk_node + "'";
                    SysEnums enums = new SysEnums();
                    enums.RetrieveInSQL(SysEnumAttr.EnumKey,
                            "SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData in(" + myFrmIDs + ")", SysEnumAttr.IntKey);

                    // 加入最新的枚举.
                    myds.Tables.Add(enums.ToDataTableField("Sys_Enum"));
                    #endregion 把枚举放入里面去.

                    #region  MapExt .
                    myds.Tables.Remove("Sys_MapExt");

                    // 把扩展放入里面去.
                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + fk_node + "'";
                    BP.Sys.MapExts exts = new MapExts();
                    qo = new QueryObject(exts);
                    qo.AddWhere(MapExtAttr.FK_MapData, " IN ", "(" + myFrmIDs + ")");
                    qo.DoQuery();

                    // 加入最新的MapExt.
                    myds.Tables.Add(exts.ToDataTableField("Sys_MapExt"));
                    #endregion  MapExt .

                    #region  MapDtl .
                    myds.Tables.Remove("Sys_MapDtl");

                    //把从表放里面
                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + fk_node + "'";
                    BP.Sys.MapDtls dtls = new MapDtls();
                    qo = new QueryObject(dtls);
                    qo.AddWhere(MapDtlAttr.FK_MapData, " IN ", "(" + myFrmIDs + ")");
                    qo.addAnd();
                    qo.AddWhere(MapDtlAttr.FK_Node, 0);

                    qo.DoQuery();

                    // 加入最新的MapDtl.
                    myds.Tables.Add(dtls.ToDataTableField("Sys_MapDtl"));
                    #endregion  MapDtl .

                    #region  FrmAttachment .
                    myds.Tables.Remove("Sys_FrmAttachment");

                    //把附件放里面
                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + fk_node + "'";
                    BP.Sys.FrmAttachment frmAtchs = new FrmAttachment();
                    qo = new QueryObject(frmAtchs);
                    qo.AddWhere(FrmAttachmentAttr.FK_MapData, " IN ", "(" + myFrmIDs + ")");
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentAttr.FK_Node, 0);
                    qo.DoQuery();

                    // 加入最新的Sys_FrmAttachment.
                    myds.Tables.Add(frmAtchs.ToDataTableField("Sys_FrmAttachment"));
                    #endregion  FrmAttachment .


                }
                #endregion 增加 groupfields

                #region 流程设置信息.
                BP.WF.Dev2Interface.Node_SetWorkRead(fk_node, workID);

                if (nd.IsStartNode == false)
                {
                    if (gwf.TodoEmps.Contains(BP.Web.WebUser.No+",") == false)
                    {
                        gwf.TodoEmps += BP.Web.WebUser.No + "," + BP.Web.WebUser.Name+";";
                        gwf.Update();
                    }
                }

                //增加转向下拉框数据.
                if (nd.CondModel == CondModel.SendButtonSileSelect)
                {
                    if (nd.IsStartNode == true ||(gwf.TodoEmps.Contains(WebUser.No + ",") == true))
                    {
                        /*如果当前不是主持人,如果不是主持人，就不让他显示下拉框了.*/

                        /*如果当前节点，是可以显示下拉框的.*/
                        Nodes nds = nd.HisToNodes;

                        DataTable dtToNDs = new DataTable();
                        dtToNDs.TableName = "ToNodes";
                        dtToNDs.Columns.Add("No", typeof(string));   //节点ID.
                        dtToNDs.Columns.Add("Name", typeof(string)); //到达的节点名称.
                        dtToNDs.Columns.Add("IsSelectEmps", typeof(string)); //是否弹出选择人的对话框？
                        dtToNDs.Columns.Add("IsSelected", typeof(string));  //是否选择？

                        #region 增加到达延续子流程节点。
                        if (nd.SubFlowYanXuNum >= 0)
                        {
                            SubFlowYanXus ygflows = new SubFlowYanXus(fk_node);
                            foreach (SubFlowYanXu item in ygflows)
                            {
                                DataRow dr = dtToNDs.NewRow();
                                dr["No"] = item.SubFlowNo + "01";
                                dr["Name"] = "起動:" + item.SubFlowName;
                                dr["IsSelectEmps"] = "1";
                                dr["IsSelected"] = "0";
                                dtToNDs.Rows.Add(dr);
                            }
                        }
                        #endregion 增加到达延续子流程节点。

                        #region 到达其他节点.
                        //上一次选择的节点.
                        int defalutSelectedNodeID = 0;
                        if (nds.Count > 1)
                        {
                            string mysql = "";
                            // 找出来上次发送选择的节点.
                            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                                mysql = "SELECT  top 1 NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID DESC";
                            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
                                mysql = "SELECT * FROM ( SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                            else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                                mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1";
                            else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                                mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1";

                            //获得上一次发送到的节点.
                            defalutSelectedNodeID = DBAccess.RunSQLReturnValInt(mysql, 0);
                        }

                        #region 为天业集团做一个特殊的判断.
                        if (SystemConfig.CustomerNo == "TianYe" && nd.Name.Contains("会長") == true)
                        {
                            /*如果是董事长节点, 如果是下一个节点默认的是备案. */
                            foreach (Node item in nds)
                            {
                                if (item.Name.Contains("記録") == true && item.Name.Contains("することが") == false)
                                {
                                    defalutSelectedNodeID = item.NodeID;
                                    break;
                                }
                            }
                        }
                        #endregion 为天业集团做一个特殊的判断.


                        foreach (Node item in nds)
                        {
                            DataRow dr = dtToNDs.NewRow();
                            dr["No"] = item.NodeID;
                            dr["Name"] = item.Name;
                            //if (item.hissel

                            if (item.HisDeliveryWay == DeliveryWay.BySelected)
                                dr["IsSelectEmps"] = "1";
                            else
                                dr["IsSelectEmps"] = "0";  //是不是，可以选择接受人.

                            //设置默认选择的节点.
                            if (defalutSelectedNodeID == item.NodeID)
                                dr["IsSelected"] = "1";
                            else
                                dr["IsSelected"] = "0";

                            dtToNDs.Rows.Add(dr);
                        }
                        #endregion 到达其他节点。


                        //增加一个下拉框, 对方判断是否有这个数据.
                        myds.Tables.Add(dtToNDs);
                    }
                }

                // 节点数据.
                //string sql = "SELECT * FROM WF_Node WHERE NodeID=" + fk_node;
                //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                //dt.TableName = "WF_NodeBar";
                //myds.Tables.Add(dt);

                //// 流程数据.
                //Flow fl = new Flow(fk_flow);
                //myds.Tables.Add(fl.ToDataTableField("WF_Flow"));
                #endregion 流程设置信息.

                #region 把主从表数据放入里面.
                //.工作数据放里面去, 放进去前执行一次装载前填充事件.

                //重设默认值.
                wk.ResetDefaultVal();

                //@樊雷伟 把这部分代码搬到jflow上去. CCFlowAPI. 114行出.
                if (BP.Sys.SystemConfig.IsBSsystem == true)
                {
                    // 处理传递过来的参数。
                    foreach (string k in HttpContextHelper.RequestQueryStringKeys)
                    {
                        if (DataType.IsNullOrEmpty(k) == true)
                            continue;

                        wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                    }

                    // 处理传递过来的frm参数。
                    //2019-07-25 zyt改造
                    foreach (string k in HttpContextHelper.RequestParamKeys)
                    {
                        if (DataType.IsNullOrEmpty(k) == true)
                            continue;

                        wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                    }

                    //更新到数据库里.
                    wk.DirectUpdate();
                }

                // 执行表单事件..
                string msg = md.DoEvent(FrmEventList.FrmLoadBefore, wk);
                if (DataType.IsNullOrEmpty(msg) == false)
                    throw new Exception("err@エラー:" + msg);

                // 执行FEE事件.
                string msgOfLoad = nd.HisFlow.DoFlowEventEntity(EventListOfNode.FrmLoadBefore, nd,
                    wk, null);
                if (msgOfLoad != null)
                    wk.RetrieveFromDBSources();

                //执行装载填充.
                MapExt me = new MapExt();
                if (me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, wk.NodeFrmID) == 1)
                {
                    //执行通用的装载方法.
                    MapAttrs attrs = new MapAttrs(wk.NodeFrmID);
                    MapDtls dtls = new MapDtls(wk.NodeFrmID);
                    wk = BP.WF.Glo.DealPageLoadFull(wk, me, attrs, dtls) as Work;
                }

                //如果是累加表单，就把整个rpt数据都放入里面去.
                if (nd.FormType == NodeFormType.FoolTruck && nd.IsStartNode == false
                  && DataType.IsNullOrEmpty(wk.HisPassedFrmIDs) == false)
                {

                    GERpt rpt = new GERpt("ND" + int.Parse(nd.FK_Flow) + "Rpt", workID);
                    rpt.Copy(wk);

                    DataTable rptdt = rpt.ToDataTableField("MainTable");

                    myds.Tables.Add(rptdt);
                }
                else
                {
                    DataTable mainTable = wk.ToDataTableField(md.No);
                    mainTable.TableName = "MainTable";
                    myds.Tables.Add(mainTable);
                }
                string sql = "";
                DataTable dt = null;
                #endregion

                #region 把外键表加入DataSet
                DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
                MapExts mes = md.MapExts;
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

                    if (uiIsEnable.Equals("0") == true && lgType.Equals("0") == true)
                        continue; //如果是外部数据源，并且是不可以编辑的状态.

                    // 检查是否有下拉框自动填充。
                    string keyOfEn = dr["KeyOfEn"].ToString();
                    string fk_mapData = dr["FK_MapData"].ToString();


                    #region 处理下拉框数据范围. for 小杨.
                    me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                    if (me != null && myds.Tables.Contains(keyOfEn) == false)
                    {
                        string fullSQL = me.Doc.Clone() as string;
                        if (fullSQL == null)
                            throw new Exception("err@フィールド["+ keyOfEn + "]ドロップダウンボックスAutoFullDLL、SQLが構成されていません");

                        fullSQL = fullSQL.Replace("~", "'");
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
                        continue;

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

                #region 处理流程-消息提示.
                DataTable dtAlert = new DataTable();
                dtAlert.TableName = "AlertMsg";

                dtAlert.Columns.Add("Title", typeof(string));
                dtAlert.Columns.Add("Msg", typeof(string));
                dtAlert.Columns.Add("URL", typeof(string));

                //  string msg = "";
                switch (gwf.WFState)
                {
                    case WFState.AskForReplay: // 返回加签的信息.
                        string mysql = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + workID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.ForwardAskfor;

                        DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(mysql);
                        foreach (DataRow dr in mydt.Rows)
                        {
                            string msgAskFor = dr[TrackAttr.Msg].ToString();
                            string worker = dr[TrackAttr.EmpFrom].ToString();
                            string workerName = dr[TrackAttr.EmpFromT].ToString();
                            string rdt = dr[TrackAttr.RDT].ToString();

                            DataRow drMsg = dtAlert.NewRow();
                            drMsg["Title"] = worker + "," + workerName + "返信メッセージ:";
                            drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt;
                            dtAlert.Rows.Add(drMsg);
                        }
                        break;
                    case WFState.Askfor: //加签.

                        sql = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + workID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.AskforHelp;
                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        foreach (DataRow dr in dt.Rows)
                        {
                            string msgAskFor = dr[TrackAttr.Msg].ToString();
                            string worker = dr[TrackAttr.EmpFrom].ToString();
                            string workerName = dr[TrackAttr.EmpFromT].ToString();
                            string rdt = dr[TrackAttr.RDT].ToString();

                            DataRow drMsg = dtAlert.NewRow();
                            drMsg["Title"] = worker + "," + workerName + "承認をリクエスト:";
                            drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + "<a href='./WorkOpt/AskForRe.htm?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workID + "&FID=" + fid + "' >コメントに返信</a>-";
                            dtAlert.Rows.Add(drMsg);

                            //提示信息.
                            // this.FlowMsg.AlertMsg_Info(worker + "," + workerName + "请求加签:",
                            //   DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + " --<a href='./WorkOpt/AskForRe.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "' >回复加签意见</a> --");
                        }
                        // isAskFor = true;
                        break;
                    case WFState.ReturnSta:
                        /* 如果工作节点退回了*/
                        ReturnWorks rws = new ReturnWorks();
                        rws.Retrieve(ReturnWorkAttr.ReturnToNode, fk_node,
                            ReturnWorkAttr.WorkID, workID,
                            ReturnWorkAttr.RDT);

                        if (rws.Count != 0)
                        {
                            //string msgInfo = "";
                            //foreach (BP.WF.ReturnWork rw in rws)
                            //{
                            //    DataRow drMsg = dtAlert.NewRow();
                            //    //drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + fk_flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a>";
                            //    drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT;
                            //    drMsg["Msg"] = rw.BeiZhuHtml;
                            //    dtAlert.Rows.Add(drMsg);
                            //}

                            string msgInfo = "";
                            foreach (BP.WF.ReturnWork rw in rws)
                            {
                                //drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + fk_flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a>";
                                msgInfo += "\t\nノードから:" + rw.ReturnNodeName + " 戻る人:" + rw.ReturnerName + "  " + rw.RDT;
                                msgInfo += rw.BeiZhuHtml;
                            }

                            string str = nd.ReturnAlert;
                            if (str != "")
                            {
                                str = str.Replace("~", "'");
                                str = str.Replace("@PWorkID", workID.ToString());
                                str = str.Replace("@PNodeID", nd.NodeID.ToString());
                                str = str.Replace("@FK_Node", nd.NodeID.ToString());

                                str = str.Replace("@PFlowNo", fk_flow);
                                str = str.Replace("@FK_Flow", fk_flow);
                                str = str.Replace("@PWorkID", workID.ToString());

                                str = str.Replace("@WorkID", workID.ToString());
                                str = str.Replace("@OID", workID.ToString());

                                DataRow drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "戻るメッセージ";
                                drMsg["Msg"] = msgInfo + "\t\n" + str;
                                dtAlert.Rows.Add(drMsg);
                            }
                            else
                            {
                                DataRow drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "戻るメッセージ";
                                drMsg["Msg"] = msgInfo + "\t\n" + str;
                                dtAlert.Rows.Add(drMsg);
                            }
                        }
                        break;
                    case WFState.Shift:
                        /* 判断移交过来的。 */
                        ShiftWorks fws = new ShiftWorks();
                        BP.En.QueryObject qo = new QueryObject(fws);
                        qo.AddWhere(ShiftWorkAttr.WorkID, workID);
                        qo.addAnd();
                        qo.AddWhere(ShiftWorkAttr.FK_Node, fk_node);
                        qo.addOrderBy(ShiftWorkAttr.RDT);
                        qo.DoQuery();
                        if (fws.Count >= 1)
                        {
                            DataRow drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "履歴情報を転送する";
                            msg = "";
                            foreach (ShiftWork fw in fws)
                            {
                                string temp = "@引き渡す人[" + fw.FK_Emp + "," + fw.FK_EmpName + "]。 @受け入れ人：" + fw.ToEmp + "," + fw.ToEmpName + "。<br>引き渡しの理由：-------------" + fw.NoteHtml;
                                if (fw.FK_Emp == WebUser.No)
                                    temp = "<b>" + temp + "</b>";

                                temp = temp.Replace("@", "<br>@");
                                msg += temp + "<hr/>";
                            }
                            drMsg["Msg"] = msg;
                            dtAlert.Rows.Add(drMsg);
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                #region 增加流程节点表单绑定信息.
                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    /* 独立流程节点表单. */

                    nd.WorkID = workID; //为获取表单ID ( NodeFrmID )提供参数.

                    FrmNode fn = new FrmNode();
                    fn.MyPK = nd.NodeFrmID + "_" + nd.NodeID + "_" + nd.FK_Flow;
                    fn.Retrieve();
                    myds.Tables.Add(fn.ToDataTableField("FrmNode"));
                }
                #endregion 增加流程节点表单绑定信息.


                myds.Tables.Add(dtAlert);
                return myds;
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.StackTrace);
                throw new Exception("generoWorkNode@" + ex.Message);
            }
        }
    }
}
