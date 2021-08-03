$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
             //alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});

//Vue.component('model-component', {
//    template: '\
//            <div>\
//                <button @click="handleIncrease">+1</button>\
//                <button @click="handleReduce">-1</button>\
//            </div> ',
//    data: function () {
//        return {
//            nodeID: GetQueryString("FK_Node"),
//        }
//    },
//    methods: {
//        handleIncrease: function () {
//            this.counter++;
//            this.$emit('input', this.counter);
//        },
//        handleReduce: function () {
//            this.counter--;
//            this.$emit('input', this.counter);
//        }
//    }

//});
var optionKey = 0;
function InitBar(optionKey) {


    var nodeID = GetQueryString("FK_Node");
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    // var html = "<div style='background-color:Silver' > 请选择访问规则: ";
    var html = "<div style='padding:5px' >受信者のルール: ";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+組織構造による拘束</option>";

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;イニシエーターは、その位置に応じてインテリジェントに計算されます</option>";
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;ノードにバインドされている部門に従ってイニシエーターを計算します</option>";
        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;ノードにバインドされている人によってイニシエーターを計算します</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;バインドされた位置と部門の共通部分に基づいてスポンサーを計算します</option>";

    } else {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;投稿によるインテリジェントな計算</option>";
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;ノードにバインドされている部門によって計算されます</option>";
        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;ノードにバインドされている担当者によって計算されます</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;バインドされた位置と部門の共通部分によって計算されます</option>";
        html += "<option value=" + DeliveryWay.ByStationAndEmpDept + " >&nbsp;&nbsp;&nbsp;&nbsp;バインドされた位置に従って計算し、バインドされた部門を緯度として設定します</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmpStation + " >&nbsp;&nbsp;&nbsp;&nbsp;指定ノードの人員位置に応じて計算</option>";
        html += "<option value=" + DeliveryWay.ByStationOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;バインドされた位置によってのみ計算されます</option>";
        html += "<option value=" + DeliveryWay.BySetDeptAsSubthread + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定部门计算，该部门一人处理标识该工作结束(子线程)</option>";

        html += "<option value=" + DeliveryWay.FindSpecDeptEmps + ">&nbsp;&nbsp;&nbsp;&nbsp;部門内の投稿コレクションで人を探す.</option>";
        // 与按照岗位智能计算不同的是，仅仅找本部门的人员.
    }


    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+指定ノードによるプロセッサ</option>";
        html += "<option value=" + DeliveryWay.ByStarter + " >&nbsp;&nbsp;&nbsp;&nbsp;開始ノードプロセッサと同じ</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;前のノードのプロセッサと同じ</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;指定ノードプロセッサと同じ</option>";
    }

    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+カスタムSQLによるクエリ</option>";
        html += "<option value=" + DeliveryWay.BySQL + " >&nbsp;&nbsp;&nbsp;&nbsp;設定されたSQLに従って受信者の計算を取得します</option>";
        html += "<option value=" + DeliveryWay.BySQLTemplate + " >&nbsp;&nbsp;&nbsp;&nbsp;設定されたSQLTempateに従って受信者の計算を取得します</option>";
        html += "<option value=" + DeliveryWay.BySQLAsSubThreadEmpsAndData + " >&nbsp;&nbsp;&nbsp;&nbsp;SQLによってサブスレッドの受信者とデータソースを決定します</option>";
    }


    if (isSatrtNode == false) {
        //检查是否是项目类的流程如果
        var isPrjFlow = false;
        var node = new Entity("BP.WF.Node", nodeID);
        var flowNo = node.FK_Flow;
        var flow = new Entity("BP.WF.Flow", flowNo);
        if (flow.FlowAppType == 1) {
            html += "<option value=null disabled='disabled' >+プロジェクトフロー</option>";
            html += "<option value=" + DeliveryWay.ByStationForPrj + ">&nbsp;&nbsp;&nbsp;&nbsp;プロジェクトチームのポジションに応じて計算</option>";
            html += "<option value=" + DeliveryWay.BySelectedForPrj + " >&nbsp;&nbsp;&nbsp;&nbsp;前のノードの送信者は、「プロジェクトチーム人事セレクター」を介して受信者を選択します</option>";
        }
    }


    html += "<option value=null disabled='disabled' >+他の方法</option>";

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.BySelected + " >&nbsp;&nbsp;&nbsp;&nbsp;すべての担当者が開始できます。</option>";

    } else {
        html += "<option value=" + DeliveryWay.BySelected + " >&nbsp;&nbsp;&nbsp;&nbsp;前のノードの送信者は、「個人セレクター」を介して受信者を選択します</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormEmpsField + " >&nbsp;&nbsp;&nbsp;&nbsp;このステップの受信者として前のノードフォームで指定されたフィールド値を押します</option>";
        html += "<option value=" + DeliveryWay.ByDtlAsSubThreadEmps + " >&nbsp;&nbsp;&nbsp;&nbsp;子スレッドの受信者は、前のノードのリストによって決定されます</option>";
        html += "<option value=" + DeliveryWay.ByFEE + " >&nbsp;&nbsp;&nbsp;&nbsp;FEEにより決定</option>";
        html += "<option value=" + DeliveryWay.ByFromEmpToEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;構成された人員ルーティングリストに従って計算します</option>";
        html += "<option value=" + DeliveryWay.ByCCFlowBPM + " >&nbsp;&nbsp;&nbsp;&nbsp;ccBPMのBPMモードによる</option>";
    }
    html += "</select >";
    html += "<input  id='Btn_Save' type=button onclick='SaveRole()' value='保存' />";
    html += "<input id='Btn' type=button onclick='AdvSetting()' value='高度な設定' />";
    html += "<input  id='Btn_Help' type=button onclick='Help()' value='オンラインヘルプ' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function SaveRole() {
    $("#Btn_Save").val("保存してください。");

    try {
        Save();
    } catch (e) {
        alert(e);
        return;
    }

    $("#Btn_Save").val("正常に保存されました");
    setTimeout(function () { $("#Btn_Save").val("保存する"); }, 1000);
}
function OldVer() {

    var nodeID = GetQueryString("FK_Node");
    var flowNo = GetQueryString("FK_Flow");

    var url = '../NodeAccepterRole.aspx?FK_Flow=' + flowNo + '&FK_Node=' + nodeID;
    window.location.href = url;
}
function Help() {

    var url = "";
    switch (optionKey) {
        case DeliveryWay.ByStation:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        case DeliveryWay.ByDept:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        default:
            url = "http://ccbpm.mydoc.io/?v=5404&t=17906";
            break;
    }

    window.open(url);
}

//通用的设置岗位的方法。for admin.

function OpenDot2DotStations() {

    var nodeID = GetQueryString("FK_Node");

    var url = "../../../Comm/RefFunc/Dot2Dot.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeStations";
    url += "&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Station&EnsOfM=BP.WF.Port.Stations";
    url += "&DefaultGroupAttrKey=FK_StationType&NodeID=" + nodeID + "&PKVal=" + nodeID;
    OpenEasyUiDialogExtCloseFunc(url, 'ポジションを設定する', 800, 500,function () {
        Baseinfo.stas = getStas();
    });

}
/*
 * 获取节点绑定的岗位
 */
function getStas() {
    var ens = new Entities("BP.WF.Template.NodeStations");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;
   
}
/*
 * 获取节点绑定的部门
 */
function getDepts() {
    var ens = new Entities("BP.WF.Template.NodeDepts");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

}
/*
 * 获取节点绑定的人员
 */
function getEmps() {
    var ens = new Entities("BP.WF.Template.NodeEmps");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

}
function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 1) {
        optionKey = sele[index].value
    }
    var roleName = "";
    switch (parseInt(optionKey)) {
        case DeliveryWay.ByStation:
            roleName = "0.ByStation.htm";
            break;
        case DeliveryWay.ByDept:
            roleName = "1.ByDept.htm";
            break;
        case DeliveryWay.BySQL:
            roleName = "2.BySQL.htm";
            break;
        case DeliveryWay.ByBindEmp:
            roleName = "3.ByBindEmp.htm";
            break;
        case DeliveryWay.BySelected:
            roleName = "4.BySelected.htm";
            break;
        case DeliveryWay.ByPreviousNodeFormEmpsField:
            roleName = "5.ByPreviousNodeFormEmpsField.htm";
            break;
        case DeliveryWay.ByPreviousNodeEmp:
            roleName = "6.ByPreviousNodeEmp.htm";
            break;
        case DeliveryWay.ByStarter:
            roleName = "7.ByStarter.htm";
            break;
        case DeliveryWay.BySpecNodeEmp:
            roleName = "8.BySpecNodeEmp.htm";
            break;
        case DeliveryWay.ByDeptAndStation:
            roleName = "9.ByDeptAndStation.htm";
            break;
        case DeliveryWay.ByStationAndEmpDept:
            roleName = "10.ByStationAndEmpDept.htm";
            break;
        case DeliveryWay.BySpecNodeEmpStation:
            roleName = "11.BySpecNodeEmpStation.htm";
            break;
        case DeliveryWay.BySQLAsSubThreadEmpsAndData:
            roleName = "12.BySQLAsSubThreadEmpsAndData.htm";
            break;
        case DeliveryWay.ByDtlAsSubThreadEmps:
            roleName = "13.ByDtlAsSubThreadEmps.htm";
            break;
        case DeliveryWay.ByStationOnly:
            roleName = "14.ByStationOnly.htm";
            break;
        case DeliveryWay.ByFEE:
            roleName = "15.ByFEEp.htm";
            break;
        case DeliveryWay.BySetDeptAsSubthread:
            roleName = "16.BySetDeptAsSubthread.htm";
            break;
        case DeliveryWay.BySQLTemplate:
            roleName = "17.BySQLTemplate.htm";
            break;
        case DeliveryWay.ByFromEmpToEmp:
            roleName = "18.ByFromEmpToEmp.htm";
            break;
        case DeliveryWay.FindSpecDeptEmps:
            roleName = "19.FindSpecDeptEmpsInStationlist.htm";
            break;
        case DeliveryWay.ByStationForPrj:
            roleName = "20.ByStationForPrj.htm";
            break;
        case DeliveryWay.BySelectedForPrj:
            roleName = "21.BySelectedForPrj.htm";
            break;
        case DeliveryWay.ByCCFlowBPM:
            roleName = "100.ByCCFlowBPM.htm";
            break;
        default:
            roleName = "0.ByStation.htm";
            break;
    }

    // alert(roleName);

    window.location.href = roleName + "?FK_Node=" + nodeID;
}
function SaveAndClose() {
    Save();
    window.close();
}

function SaveIt() {

    $("#Btn_Save").val("保存してください。");

    try {
        Save();
        AfterSave();
    } catch (e) {
        alert(e);
        return;
    }

    $("#Btn_Save").val("正常に保存されました");
    setTimeout(function () { $("#Btn_Save").val("保存。"); }, 1000);
}
// 保存之后要做的事情.
function AfterSave() {
    //清除.
    DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows=''");
}

//打开窗体.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            window.location.href = window.location.href;
        }
    });
}

//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "AdvSetting.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高度な設定", 600, 500, false);
}

