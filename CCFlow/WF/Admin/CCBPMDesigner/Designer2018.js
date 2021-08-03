var flow = null;
$(function () {

    var flowNo = GetQueryString("FK_Flow");
    flow = new Entity("BP.WF.Flow", flowNo);

    //设置状态.
    SetState();

    $("#pmfun,#nodeMenu").hover(function () {
        var mLeft = $("#jqContextMenu").css("left").replace('px', '');
        var mTop = $("#jqContextMenu").css("top").replace('px', '');
        $("#nodeMenu").css({ "left": parseInt(mLeft) + 148 + "px", "top": parseInt(mTop) + 62 + "px" });
        $("#nodeMenu").show();


    }, function () {
        $("#nodeMenu").hide();
    });

    //节点类型--普通
    $('#Node_Ordinary').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 0);

    });
    //节点类型--分流
    $('#Node_FL').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 2);
    });
    //节点类型--合流
    $('#Node_HL').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 1);
    });
    //节点类型--分合流
    $('#Node_FHL').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 3);
    });
    //节点类型--同表单的子线程
    $('#Node_SubThread0').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 4, 0);
    });

    //节点类型--异表单子线程
    $('#Node_SubThread1').on('click', function () {
        var nodeID = document.getElementById("leipi_active_id");
        SetNodeRunModel(nodeID.value, 4, 1);
    });

});

//设置状态。
function SetState() {
    var flowFrmType = flow.FlowFrmType;
    if (flowFrmType == 0) {

        $("#Btn_Frm").hide(); //如果是旧版本的就隐藏该按钮.
        $("#pmFrmPower").remove(); //如果是旧版本的就删除该按钮.
    }
    else {

        $("#Btn_Frm").show(); //新版本显示这个按钮
        $("#pmFrmD").after("<li id='pmFrmPower'> ★&nbsp;&nbsp;<span class='_label'>フォームの権限</span></li>");//新版本就追加这个按钮
    }

    //隐藏指定的菜单.
    $("#pmFrmSln").hide(); //表单方案
    $("#pmFrmD").hide(); //设计表单.

}


//设置节点类型。
function SetNodeRunModel(nodeID, runModel, subThreadType) {
    //获得nodeID.
    var node = new Entity("BP.WF.Node", nodeID);
    node.RunModel = runModel;

    //判断是否是同表单的,还是异表单的.
    if (runModel == 4) {
        node.SubThreadType = subThreadType;
    }

    node.Update();
    alert("正常に変更されました！");
}

//改变节点风格.
function ChangeNodeIcon(nodeID, runModel) {
    alert('実装されていません。');
}

//设计表单
function Frm() {

    var flowNo = GetQueryString("FK_Flow");
    var flow = new Entity("BP.WF.Flow", flowNo);
    if (flow.FlowFrmType == FlowFrmType.Ver2019Earlier) {
        alert('フローシートは古いバージョンであり、フルバージョンで設計する必要があります。');
        return;
    }

    var frmID = "ND" + parseInt(flowNo) + "01";
    var nodeID = parseInt(flowNo + "01");

    var url = "";
    if (flow.FlowFrmType == FlowFrmType.FoolFrm)
        url = "../FoolFormDesigner/Designer.htm?FrmID=" + frmID + "&FK_Flow=" + flowNo + "&FK_MapData=" + frmID + "&FK_Node=" + nodeID;

    if (flow.FlowFrmType == FlowFrmType.DeveloperFrm)
        url = "../DevelopDesigner/Designer.htm?FrmID=" + flowNo + "&FK_Flow=" + flowNo + "&FK_MapData=" + frmID + "&FK_Node=" + nodeID;

    window.parent.addTab(nodeID, "デザインフォーム" + nodeID, url);

    // window.open(url);
    //OpenEasyUiDialog(url, "eudlgframe", '流程检查', 800, 500, "icon-property", true, null, null, null, function () {
    //window.location.href = window.location.href;
    //});
}


var the_flow_id = '4';

/*页面回调执行    callbackSuperDialog
if(window.ActiveXObject){ //IE
    window.returnValue = globalValue
}else{ //非IE
if(window.opener) {
    window.opener.callbackSuperDialog(globalValue) ;
}
}
window.close();
*/
function callbackSuperDialog(selectValue) {
    var aResult = selectValue.split('@leipi@');
    $('#' + window._viewField).val(aResult[0]);
    $('#' + window._hidField).val(aResult[1]);
    //document.getElementById(window._hidField).value = aResult[1];
}
/**
* 弹出窗选择用户部门角色
* showModalDialog 方式选择用户
* URL 选择器地址
* viewField 用来显示数据的ID
* hidField 隐藏域数据ID
* isOnly 是否只能选一条数据
* dialogWidth * dialogHeight 弹出的窗口大小
*/
function superDialog(URL, viewField, hidField, isOnly, dialogWidth, dialogHeight) {
    dialogWidth || (dialogWidth = 620)
        , dialogHeight || (dialogHeight = 520)
        , loc_x = 500
        , loc_y = 40
        , window._viewField = viewField
        , window._hidField = hidField;
    // loc_x = document.body.scrollLeft+event.clientX-event.offsetX;
    //loc_y = document.body.scrollTop+event.clientY-event.offsetY;
    if (window.ActiveXObject) { //IE
        var selectValue = window.showModalDialog(URL, self, "edge:raised;scroll:1;status:0;help:0;resizable:1;dialogWidth:" + dialogWidth + "px;dialogHeight:" + dialogHeight + "px;dialogTop:" + loc_y + "px;dialogLeft:" + loc_x + "px");
        if (selectValue) {
            callbackSuperDialog(selectValue);
        }
    } else {  //非IE
        var selectValue = window.open(URL, 'newwindow', 'height=' + dialogHeight + ',width=' + dialogWidth + ',top=' + loc_y + ',left=' + loc_x + ',toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no');
    }
}

var flowNo = null;
$(function () {

    flowNo = GetQueryString("FK_Flow");
    if (flowNo == undefined || flowNo == null)
        flowNo = "002";

    var alertModal = $('#alertModal'), attributeModal = $("#attributeModal");
    var alertModal1 = $('#alertModal1'), attributeModal = $("#attributeModal");
    var alertModal2 = $('#alertModal2'), attributeModal = $("#attributeModal");
    var alertModal3 = $('#alertModal3'), attributeModal = $("#attributeModal");
    var alertModal4 = $('#alertModal4'), attributeModal = $("#attributeModal");
    //消息提示
    mAlert = function (messages, s) {
        if (!messages) messages = "";
        if (!s) s = 2000;
        alertModal.find(".modal-body").html(messages);
        alertModal.modal('toggle');
        setTimeout(function () { alertModal.modal("hide") }, s);
    }
    //消息弹出（节点）
    cAlert = function (messages, s) {
        if (!messages) messages = "";
        if (!s) s = 200000;
        alertModal1.find(".modal-body").html(messages);
        alertModal1.modal('toggle');
        setTimeout(function () { alertModal1.modal("hide") }, s);
    }
    //消息弹出（标签）
    labAlert = function (messages, s) {
        if (!messages) messages = "";
        if (!s) s = 200000;
        alertModal3.find(".modal-body").html(messages);
        alertModal3.modal('toggle');
        setTimeout(function () { alertModal3.modal("hide") }, s);
    }
    //连接线演示
    ShowGif = function (s) {

        if (!s) s = 200000;

        alertModal2.modal('toggle');
        setTimeout(function () { alertModal2.modal("hide") }, s);
    }
    //新建流程演示
    ShowNewFlowGif = function (s) {

        if (!s) s = 200000;

        alertModal4.modal('toggle');
        setTimeout(function () { alertModal4.modal("hide") }, s);
    }
    //消息弹出（线）
    fAlert = function (messages, s) {
        if (!messages) messages = "実行するアクションを選択してください";
        if (!s) s = 200000;
        $("#LineModal").find(".modal-body").html(messages);
        $("#LineModal").modal('toggle');
        setTimeout(function () { alertModal1.modal("hide") }, s);
    }
    //属性设置
    attributeModal.on("hidden", function () {
        $(this).removeData("modal"); //移除数据，防止缓存
    });

    ajaxModal = function (url, fn) {

        url += url.indexOf('?') ? '&' : '?';
        url += '_t=' + new Date().getTime();
        attributeModal.find(".modal-body").html('<img src="Public/images/loading.gif" />');
        attributeModal.modal({
            remote: url
        });

        //加载完成执行
        if (fn) {
            attributeModal.on('shown', fn);
        }
    }

    /*步骤数据*/
    var processData = GenerDrowFlowData();
    //标签数据
    var labNoteData = GetLabNoteData();
    console.log(processData);
    /*创建流程设计器*/
    var _canvas = $("#flowdesign_canvas").Flowdesign({
        "processData": processData,
        "labNoteData": labNoteData
        , mtAfterDrop: function (params) {
            //alert("连接：" + params.sourceId + " -> " + params.targetId);
        }
        /*画面右键*/
        , canvasMenus: {
            "cmNewNode": function (t) {
                $(".mymaskContainer").offset({ left: ($(document).innerWidth() - 120) / 2, top: ($(document).innerHeight() - 50) / 2 });
                $(".mymask").show();

                //获取坐标
                var mLeft = $("#jqContextMenu").css("left").replace('px', '');
                var mTop = $("#jqContextMenu").css("top").replace('px', '');


                //创建一个节点
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("X", mLeft);
                hander.AddPara("Y", mTop);
                hander.AddPara("FK_Flow", flowNo);

                var data = hander.DoMethodReturnString("CreateNode");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }

                //添加节点样式与坐标
                data = JSON.parse(data);
                var strs = "";
                strs += "{'id':'" + data.NodeID + "',";
                strs += "'flow_id':'" + flowNo + "',";
                strs += "'process_name':'" + data.Name + "',";
                strs += "'process_to':0,";
                strs += "'icon':'icon-ok',";
                strs += "'style':'width:auto;height:41px;line-height:41px;color:#0e76a8;left:" + mLeft + "px;top:" + mTop + "px;'";
                strs += "}";
                strs = eval("(" + strs + ")");

                if (_canvas.addProcess(strs) == false) //添加
                {
                    alert("追加に失敗しました");
                    return;
                }
                $(".mymask").hide();
            },
            "cmSave": function (t) {

                var processInfo = _canvas.getProcessInfo(); //连接信息

                /*重要提示 start*/
                alert("ここではAjax送信が使用されています。公式ウェブサイトの例を参照してください。Fiddlerソフトウェアを使用してパッケージをキャプチャし、リターンフォーマットccを取得できます。");
                /*重要提示 end */

                //                                      var url = "/index.php?s=/Flowdesign/save_canvas.html";
                //                                      $.post(url, {"flow_id": the_flow_id, "process_info": processInfo }, function (data) {
                //                                          mAlert(data.msg);
                //                                      }, 'json');
            },
            //刷新
            //添加标签
            "cmNewLabel": function (t) {
                var mLeft = $("#jqContextMenu").css("left").replace('px', '');
                var mTop = $("#jqContextMenu").css("top").replace('px', '');

                //创建一个标签
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("X", mLeft);
                hander.AddPara("Y", mTop);
                hander.AddPara("FK_Flow", flowNo);
                hander.AddPara("LabName", "ラベルを入力してください");

                var data = hander.DoMethodReturnString("CreatLabNote");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }

                //添加标签样式与坐标
                data = JSON.parse(data);
                var strs = "";
                strs += "{'id':'" + data.MyPK + "',";
                strs += "'flow_id':'" + data.FK_Flow + "',";
                strs += "'process_name':'ラベルを入力してください',";
                strs += "'style':'width:auto;height:30px;line-height:30px;color:#0e76a8;left:" + mLeft + "px;top:" + mTop + "px;'";
                strs += "}";
                strs = eval("(" + strs + ")");

                if (_canvas.addLabProcess(strs) == false) //添加
                {
                    alert("追加に失敗しました");
                    return;
                }
            },
            "cmPaste": function (t) {
                var pasteId = _canvas.paste(); //右键当前的ID
                if (pasteId <= 0) {
                    alert("ステップをコピーしませんでした");
                    return;
                }
                alert("ペースト：" + pasteId);
            },
            "cmHelp": function (t) {

                Help();
            }
        }
        /*步骤右键*/
        , processMenus: {

            "pmBegin": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                alert("最初のステップとして設定します。" + activeId);
            },
            "pmAddson": function (t)//添加子步骤
            {
                var activeId = _canvas.getActiveId(); //右键当前的ID
            },
            "pmCopy": function (t) {
                //var activeId = _canvas.getActiveId();//右键当前的ID
                _canvas.copy(); //右键当前的ID
                alert("正常にコピー");
            },
            "pmDelete": function (t) {

                var activeId = _canvas.getActiveId(); //右键当前的ID.
                var str = activeId.substring(activeId.length - 2);
                if (str == "01") {
                    /*如果是开始节点. */
                    alert('開始ノードは削除できません。');
                    return;
                }

                if (confirm("ノードを削除してもよろしいですか？ 現在のノードにやることやデータがある場合、システムはそれを削除することを拒否します。") == false)
                    return;

                //节点.
                var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner2018");
                hander.AddPara("FK_Node", activeId);
                var data = hander.DoMethodReturnString("DeleteNode");
                if (data.indexOf('err@') == 0) {
                    alert(data); //删除失败的情况.
                    return;
                }
                _canvas.delProcess(activeId);
            },
            "pmAttribute": function (t) {
                //节点属性.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeAttr(activeId);
            },
            "pmCondDir": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                CondDir(activeId);
            },
            "pmFrmSln": function (t) {
                //表单方案.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmSln(activeId); //表单方案.
            },
            "pmFrmD": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmD(activeId);
            },
            "pmFrmPower": function (t) {
                var activeId = _canvas.getActiveId(); //表单权限ID
                FrmPower(activeId);
            },
            "pmFrmFool": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmFool(activeId);
            },
            "pmFrmFree": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeFrmFree(activeId); cAlert
            },
            "pmNodeAccepterRole": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID
                NodeAccepterRole(activeId);
            }
        },
        canvasLabMenu: {
            "clmDelete": function (t) {
                var activeId = _canvas.getActiveId(); //右键当前的ID.
                if (confirm("このラベルを削除してもよろしいですか？") == false)
                    return;

                //删除标签.
                var lb = new Entity("BP.WF.Template.LabNote", activeId);
                lb.Delete();
                _canvas.delLabNote(activeId);

            },
            "clmNewName": function (t) {
                //修改标签名称.
                var activeId = _canvas.getActiveId(); //右键当前的ID
                var windowtext = $("#lab" + activeId).text();

                windowtext = windowtext.replace(/(^\s*)|(\s*$)/g, ""); //去掉左右空格.

                alert(windowtext);


                var baocunbut = $("#alertModal3 div:eq(2) button").attr("class", "btn btn-primary savetext" + activeId);
                var saveNodeName = $("#alertModal3 div:eq(2) button").attr("onclick", "saveLabName(\"" + activeId + "\")");
                var xiuNodename = '名前の変更：<input id="TB_LAB_' + activeId + '" type="text" value="' + windowtext + '">'
                var spanaa = $("#lab" + activeId + " span").html();

                labAlert(xiuNodename);
            }
        }
        , fnRepeat: function () {
            //alert("步骤连接重复1");//可使用 jquery ui 或其它方式提示
            mAlert("ステップ接続が繰り返されます。再接続するか、現在のフローを閉じて再度開いてください。");

        }
        , fnClick: function () {
            //点击修改名称方法
            var activeId = _canvas.getActiveId(); //右键当前的ID
            var windowtext = $("#window" + activeId).text();
            var baocunbut = $("#alertModal1 div:eq(2) button:eq(0)").attr("class", "btn btn-primary savetext" + activeId);
            var saveNodeName = $("#alertModal1 div:eq(2) button:eq(0)").attr("onclick", "saveNodeName(\"" + activeId + "\")");
            var baocunbut = $("#alertModal1 div:eq(2) button:eq(1)").attr("class", "btn btn-primary savetext" + activeId);
            var saveNodeName = $("#alertModal1 div:eq(2) button:eq(1)").attr("onclick", "saveAndUpdateNodeName(\"" + activeId + "\")");
            windowtext = windowtext.replace(/(^\s*)|(\s*$)/g, "");


            var xiuNodename = '<input id="TB_' + activeId + '" type="text" value="' + windowtext + '">'
            var spanaa = $("#window" + activeId + " span").html();

            cAlert(xiuNodename);

        }
        , fnDbClick: function () {
            //和 pmAttribute 一样
            var activeId = _canvas.getActiveId(); //右键当前的ID
            NodeAttr(activeId);

        }
    });

    /*新建*/

    $("#Btn_NewFlow").bind('click', function () {

        alert("フローツリーの右クリックメニューで新しいフローを作成してください。");


    });

    /*保存*/
    $("#Btn_Save").bind('click', function () {

        $("#Btn_Save").attr("disabled", true);
        $("#Btn_Save").html("保存しています...");

        SaveFlow(_canvas);


    });
    /*保存*/
    $("#Btn_SaveToColud").bind('click', function () {

        alert('開発中');
        return;
        $("#Btn_Save").attr("disabled", true);

    });
    /*清除连接线，用不到.*/
    $("#leipi_clear").bind('click', function () {
        return;
        if (_canvas.clear()) {
            //alert("清空连接成功");
            mAlert("クリア接続が成功しました。再接続できます");
        } else {
            //alert("清空连接失败");
            mAlert("接続をクリアできませんでした");
        }
    });

});

///保存方法
function SaveFlow(_canvas) {

    //获取所有节点信息.
    try {

        var nodes = new Entities("BP.WF.Template.NodeSimples");
        nodes.Retrieve("FK_Flow", flowNo);

        // 保存x,y位置.
        var processInfo = _canvas.getProcessInfo(); //连接信息.
        processInfo = JSON.parse(processInfo);

        //定义要生成的字符串.
        var nodePos = "";
        for (var nodeID in processInfo) {

            var nodeIDStr = JSON.stringify(nodeID);

            var nodeJSON = processInfo[nodeID];

            for (var idx = 0; idx < nodes.length; idx++) {

                var node = nodes[idx];
                var myID = "\"" + node.NodeID + "\"";
                if (myID != nodeIDStr)
                    continue;

                nodePos += "@" + node.NodeID + "," + nodeJSON.left + "," + nodeJSON.top;
                break;
            }
        }

        //方向的字符串.
        var myDirs = "";

        //保存方向.
        for (var nodeID in processInfo) {

            //获得toNode.
            var nodeJSON = processInfo[nodeID];
            var strs = JSON.stringify(nodeJSON);

            //获得toNode.
            var toNodes = nodeJSON.process_to;
            if (toNodes == "")
                continue;

            if (nodeID == undefined
                || nodeID == null
                || nodeID == "undefined"
                || nodeID == 'undefined') {
                continue;
            }

            for (var i = 0; i < toNodes.length; i++) {

                var toNodeID = toNodes[i];

                if (toNodeID == undefined
                    || toNodeID == null
                    || toNodeID == "undefined"
                    || toNodeID == 'undefined') {

                    var fromNode = new Entity("BP.WF.Template.NodeSimple", nodeID);
                    alert('エラー保存：\t\n\t\n1.Node [' + fromNode.Name + ']到着ノードが正しく接続されていません。\t\n2。現在のフローを閉じて再度開き、接続して保存してください。 \t\n3。残りの方向条件は正常に保存されます。');
                    $("#Btn_Save").attr("disabled", false);
                    $("#Btn_Save").html("保存");
                    continue;
                }
                var MyPK = flowNo + "_" + nodeID + "_" + toNodeID;
                myDirs += "@" + MyPK + "," + flowNo + "," + nodeID + "," + toNodeID;
            }
        }


        var labs = "";
        //标签.
        var labNoteInfo = _canvas.getLabNoteInfo(); //标签信息.
        labNoteInfo = JSON.parse(labNoteInfo);
        for (var lab in labNoteInfo) {

            var labJSON = labNoteInfo[lab];

            labs += "@" + lab + "," + labJSON.left + "," + labJSON.top;
        }

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
        handler.AddPara("Nodes", nodePos);
        handler.AddPara("Dirs", myDirs);
        handler.AddPara("Labs", labs);
        handler.AddPara("FK_Flow", flowNo);
        var data = handler.DoMethodReturnString("Designer_Save");

        if (data.indexOf('err@') == 0) {
            alert(data);
        }
    }
    catch (e) {
        alert(e);
    }

    //alert('保存成功!');

    $("#Btn_Save").attr("disabled", false);
    $("#Btn_Save").html("保存");
    return;
}

//修改节点名称
function saveNodeName(activeId) {
    var text = document.getElementById("TB_" + activeId).value; //新修改的值.

    //alert(text);

    var node = new Entity("BP.WF.Template.NodeExt", activeId);
    node.Name = text;
    node.Update();

    //修改表单名称.
    var mapData = new Entity("BP.Sys.MapData", "ND" + activeId);
    if (mapData.Name == null || mapData.Name == undefined || mapData.Name == "") {
        mapData.Name = text;
        mapData.Update();
    }


    //修改分组名称.
    var groups = new Entities("BP.Sys.GroupFields");
    groups.Retrieve("FrmID", "ND" + activeId);

    //  alert(groups.length);

    if (groups.length == 1) {

        var group = groups[0];

        var groupEn = new Entity("BP.Sys.GroupField", group.OID);
        groupEn.Lab = text;
        groupEn.Update();
    }


    //更新节点名称与显示
    $("#span_" + activeId).text(text);
}

//修改并更新节点表单名称
function saveAndUpdateNodeName(activeId) {
    var text = document.getElementById("TB_" + activeId).value; //新修改的值.

    //alert(text);

    var node = new Entity("BP.WF.Template.NodeExt", activeId);
    node.Name = text;
    node.Update();

    //修改表单名称.
    var mapData = new Entity("BP.Sys.MapData", "ND" + activeId);
    mapData.Name = text;
    mapData.Update();



    //修改分组名称.
    var groups = new Entities("BP.Sys.GroupFields");
    groups.Retrieve("FrmID", "ND" + activeId);

    //  alert(groups.length);

    if (groups.length == 1) {

        var group = groups[0];

        var groupEn = new Entity("BP.Sys.GroupField", group.OID);
        groupEn.Lab = text;
        groupEn.Update();
    }


    //更新节点名称与显示
    $("#span_" + activeId).text(text);
}
//修改标签名称
function saveLabName(activeId) {
    var lb = new Entity("BP.WF.Template.LabNote", activeId);
    var text = document.getElementById("TB_LAB_" + activeId).value; //新修改的值.
    lb.Name = text;
    lb.Update();

    //更新名称与显示
    $("#lab_span_" + activeId).text(text);
}
//获得流程数据并转化为指定格式的json.
function GenerDrowFlowData() {

    flowNo = GetQueryString("FK_Flow");
    if (flowNo == null || flowNo == undefined)
        flowNo = "002";

    //节点. 取出来显示
    var nodes = new Entities("BP.WF.Nodes");
    nodes.Retrieve("FK_Flow", flowNo);

    //方向. 取出来显示
    var dirs = new Entities("BP.WF.Template.Directions");
    dirs.Retrieve("FK_Flow", flowNo);


    var strs = "{'total':" + nodes.length + ", 'list':[";
    //遍历节点个数，输入节点.
    for (var i = 0; i < nodes.length; i++) {

        var node = nodes[i];

        //获得到达的节点.
        var toNodes = "";
        for (var idx = 0; idx < dirs.length; idx++) {
            var dir = dirs[idx];

            if (dir.Node == node.NodeID) {
                toNodes += "," + dir.ToNode;
            }
        }

        if (node.Name == null || node.Name == "")
            node.Name = "ノードx";

        strs += "{'id':'" + node.NodeID + "',";
        strs += "'flow_id':'" + flowNo + "',";
        strs += "'process_name':'" + node.Name.replace(/(^\s*)|(\s*$)/g, "") + "',";
        strs += "'process_to':'" + toNodes + "',";

        //判断是否是开始节点?
        var nodeID = "" + node.NodeID;
        var str = nodeID.substring(nodeID.length - 2);

        if (str == "01") {
            // strs += "'icon':'icon-ok',";
        } else if (toNodes == "") {
            strs += "'icon':'icon-ok',";
        } else {
            /* 如果是其他的情况,就要考虑分合流 */
        }


        strs += "'style':'width:auto;minWidth:121px;height:41px;line-height:41px;color:#0e76a8;left:" + node.X + "px" + ";top:" + node.Y + "px;'";

        if (i == nodes.length - 1)
            strs += "}";
        else
            strs += "},";
    }

    strs += "] }";


    return eval("(" + strs + ")");
}

function DealSpecStr(str) {

    str = str.toString().replace(new RegExp('(["\"])', 'g'), "\\\"");
    str = str.replace("\"", "\\\"").replace("\r\n", "<br />").replace("\n", "<br />").replace("\r", "<br />");
    str = str.replace("\"", "\'");

    if (str.indexOf('開発キーの説明') != -1) {
        return "";
    }

    return str;
}

function GetLabNoteData() {
    //标签. 取出来显示
    var labs = new Entities("BP.WF.Template.LabNotes");
    labs.Retrieve("FK_Flow", flowNo);

    var strs = "{'total':" + labs.length + ", 'list':[";
    //遍历标签个数
    for (var i = 0; i < labs.length; i++) {

        var lab = labs[i];

        //console.log(lab.Name);

        lab.Name = DealSpecStr(lab.Name);

        //alert(lab.Name);

        strs += "{'id':'" + lab.MyPK + "',";
        strs += "'flow_id':'" + lab.FK_Flow + "',";
        strs += "\"process_name\":\"" + lab.Name + "\",";
        strs += "'style':'width:auto;height:30px;line-height:30px;color:#0e76a8;left:" + lab.X + "px" + ";top:" + lab.Y + "px;'";

        if (i == labs.length - 1)
            strs += "}";
        else
            strs += "},";
    }

    strs += "] }";

    try {
        return eval("(" + strs + ")");
    } catch (e) {
        return "";
    }
}

//刷新页面
function page_reload() {
    location.reload();
}

function ShowGif() {
    alert("マウスを使用してノードの黒い領域を押したままドラッグすると、接続線が表示され、接続するノードをポイントします。デモをご覧ください");

    $("#Msg").css('display', 'block');
    setTimeout("HideGif()", 7000);
}
function HideGif() {
    $("#Msg").css('display', 'none');
}
function ShowNewFlowGif() {
    alert("フローツリーの右クリックメニューで新しいフローを作成してください。 デモを見る");

    $("#Msg").css('display', 'block');
    setTimeout("HideGif()", 7000);
}
function HideNewFlowGif() {
    $("#Msg").css('display', 'none');
}
//全局变量
function WinOpen(url) {
    window.open(url);
}

//流程属性.
function FlowProperty() {
    url = "../../Comm/En.htm?EnName=BP.WF.Template.FlowExt&PKVal=" + flowNo + "&Lang=CH";

    //OpenEasyUiDialogExt(url, "流程属性", 900, 500, false);
    window.parent.addTab(flowNo, "フロー属性" + flowNo, url);

    //  WinOpen(url);
    //    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 1000, 550, "icon-property", true, null, null, null, function () {
    //        //window.location.href = window.location.href;
    //    });
}

//报表设计.
function FlowRpt() {

    if (window.confirm('この機能はキャンセルしますが、内部開発者専用です。') == false)
        return;

    //  alert('该功能，我们将要取消.');
    // return;

    var flowId = Number(flowNo);
    flowId = String(flowId);
    url = "../RptDfine/Default.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";

    //OpenEasyUiDialogExt(url, "报表设计", 900, 500, false);
    window.parent.addTab(flowNo + "_BBSJ", "レポートのデザイン" + flowNo, url);
}

//检查流程.
function FlowCheck() {

    var flowId = Number(flowNo);
    flowId = String(flowId);
    url = "../AttrFlow/CheckFlow.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    // WinOpen(url);
    OpenEasyUiDialog(url, "FlowCheck" + flowNo, "検査フロー" + flowNo, 600, 500, "icon - library", false);
    //window.parent.addTab(flowNo + "_JCLC", "检查流程" + flowNo, url);
}

//运行流程
function FlowRun() {

    //执行流程检查.
    var flow = new Entity("BP.WF.Flow", flowNo);
    flow.DoMethodReturnString("ClearCash");

    var url = "../TestFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    //WinOpen(url);
    window.parent.addTab(flowNo + "_YXLH", "フローを実行する" + flowNo, url);
}
//运行流程
function FlowRun2020() {

    //执行流程检查.
    var flow = new Entity("BP.WF.Flow", flowNo);
    flow.DoMethodReturnString("ClearCash");

    var url = "../TestingContainer/TestFlow2020.htm?FK_Flow=" + flowNo + "&Lang=CH";

    //WinOpen(url);
    window.parent.addTab(flowNo + "_YXLH", "運用フロー2020" + flowNo, url);
}

//运行流程
function FlowRunAdmin() {

    //执行流程检查.
    var flow = new Entity("BP.WF.Flow", flowNo);
    flow.DoMethodReturnString("ClearCash");

    //var url = "../TestFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    var webUser = new WebUser();
    var url = "../TestFlow.htm?DoType=TestFlow_ReturnToUser&DoWhat=StartClassic&UserNo=" + webUser.No + "&FK_Flow=" + flowNo;
    //  var url = "../../MyFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    WinOpen(url);
    //window.parent.addTab(flowNo + "_YXLH", "运行流程" + flowNo, url);
}

//旧版本.
function OldVer() {
    var url = "Designer2016.htm?FK_Flow=" + flowNo + "&Lang=CH&&Flow_V=1";
    window.location.href = url;
}

function Help() {

    var msg = "<ul>";
    msg += "<li>開発者：Jinan Chicheng Information Technology Co.、Ltd。</li>";
    msg += "<li>公式ウェブサイト： <a href='http://www.ccflow.org?Ref=ccbpmApp' target=_blank>http://ccflow.org</a></li>";
    msg += "<li>営業担当者：0531-82374939、WeChat：18660153393 QQ：793719823</li>";
    msg += "<li>住所：Jinanは、ハイテクゾーンのQilu Software Park、Block C、RoomB301です。</li>";
    msg += "<li>パブリックアカウント<img src='' border=0/></li>";
    msg += "</ul>";
    mAlert(msg, 20000);
}

/***********************  节点信息. ******************************************/

//节点属性
function NodeAttr(nodeID) {

    //var url = "../../Comm/RefFunc/EnV2.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    var url = "../../Comm/En.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    var html = "";

    //var html = "<a href=\"javascript:OpenEasyUiDialogExt('" + url + "','';\" >主页</a> - ";
    window.parent.addTab(nodeID, "ノード属性" + nodeID, url);
    //OpenEasyUiDialogExt(url, html+"属性", 900, 500, false);
}
//节点属性
function NodeAttrOld(nodeID) {
    var url = "../../Comm/En.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    window.parent.addTab(nodeID, "ノード属性" + nodeID, url);
    //OpenEasyUiDialogExt(url, "节点属性", 800, 500, false);
}

//表单方案
function NodeFrmSln(nodeID) {
    //表单方案.
    var url = "../AttrNode/FrmSln/Default.htm?FK_Node=" + nodeID;

    window.parent.addTab(nodeID + "_JDFA", "フォームスキーム" + nodeID, url);
    // OpenEasyUiDialogExt(url, "表单方案", 800, 500, false);
}

//方向条件.
function CondDir(fromNodeID) {

    var flowNo = GetQueryString("FK_Flow");

    var targetId = fromNodeID;

    var url = "../Cond/ConditionLine.htm?FK_Flow=" + flowNo + "&FK_MainNode=" + fromNodeID + "&FK_Node=" + fromNodeID + "&ToNodeID=" + targetId + "&CondType=2&Lang=CH&t=" + new Date().getTime();
    $("#LineModal").hide();
    $(".modal-backdrop").hide();
    OpenEasyUiDialog(url, flowNo + fromNodeID + "DIRECTION" + targetId, "方向条件を設定する" + fromNodeID + "->" + targetId, 880, 500, "icon-property", true, null, null, null, function () {

    });
}



//设计表单
function NodeFrmD(nodeID) {

    var node = new Entity("BP.WF.Node", nodeID);
    if (node.FormType == 1) {//自由表单
        NodeFrmFree(nodeID);
        return;
    }

    if (node.FormType == 12) {//开发者表单
        NodeFrmDeveloper(nodeID);
        return;
    }

    //傻瓜表单
    NodeFrmFool(nodeID);
}

//表单的权限.
function FrmPower(nodeID) {

    var frmID = "ND" + parseInt(flowNo + "01");
    var mypk = frmID + "_" + nodeID + "_" + flowNo;

    //傻瓜表单.
    var url = "../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeJiJian&MyPK=" + mypk + "&Lang=CH";
    //WinOpen(url);
    window.parent.addTab(nodeID + "_Fool", "デザインフォーム" + nodeID, url);
}

function NodeFrmFool(nodeID) {
    //傻瓜表单.
    var url = "../FoolFormDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&IsFirst=1&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    //WinOpen(url);
    window.parent.addTab(nodeID + "_Fool", "デザインフォーム" + nodeID, url);
}

function NodeFrmFree(nodeID) {

    //自由表单.
    var url = "../CCFormDesigner/FormDesigner.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.parent.addTab(nodeID + "_Free", "デザインフォーム" + nodeID, url);
    ///CCFormDesigner/FormDesigner.htm?FK_Node=9502&FK_MapData=ND9502&FK_Flow=095&UserNo=admin&SID=c3466cb7-edbe-4cdc-92df-674482182d01
    //WinOpen(url);
}

function NodeFrmDeveloper(nodeID) {
    //开发者表单.
    var url = "../DevelopDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.parent.addTab(nodeID + "_Developer", "デザインフォーム" + nodeID, url);
}

//接受人规则.
function NodeAccepterRole(nodeID) {
    //接受人规则.
    var url = "../AttrNode/AccepterRole/Default.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.parent.addTab(nodeID + "_JSGZ", "受信者のルール" + nodeID, url);
    //OpenEasyUiDialogExt(url, "接受人规则", 800, 500, false);
}

function Reload() {
    if (confirm('更新してもよろしいですか？ 更新は保存されません。') == false)
        return;

    window.location.href = window.location.href;
}

//打开.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            window.location.href = window.location.href;
        }
    });
}
