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


var optionKey = 0;
function InitBar(key) {

    optionKey = key;


    var nodeID = GetQueryString("FK_Node");
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    // var html = "<div style='background-color:Silver' > 请选择访问规则: ";
    var html = "<div style='padding:5px' >受信者の範囲は次を選択できます:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+組織構造に応じて範囲を制限する</option>";

    html += "<option value=" + SelectorModel.Station + ">&nbsp;&nbsp;&nbsp;&nbsp;投稿によると</option>";
    html += "<option value=" + SelectorModel.Dept + " >&nbsp;&nbsp;&nbsp;&nbsp;部門別</option>";
    html += "<option value=" + SelectorModel.Emp + " >&nbsp;&nbsp;&nbsp;&nbsp;担当者による</option>";
    html += "<option value=" + SelectorModel.SQL + " >&nbsp;&nbsp;&nbsp;&nbsp;SQLで計算</option>";
    html += "<option value=" + SelectorModel.SQLTemplate + " >&nbsp;&nbsp;&nbsp;&nbsp;SQLテンプレートで計算</option>";
    html += "<option value=" + SelectorModel.GenerUserSelecter + " >&nbsp;&nbsp;&nbsp;&nbsp;ユニバーサルパーソンセレクターを使用する</option>";
    html += "<option value=" + SelectorModel.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;部門と役職の交差点</option>";

    html += "<option value=null  disabled='disabled'>+その他</option>";
    html += "<option value=" + SelectorModel.Url + ">&nbsp;&nbsp;&nbsp;&nbsp;カスタムURL</option>";
    html += "<option value=" + SelectorModel.AccepterOfDeptStationEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;一般部門のポジション担当者セレクターを使用する（開発中）</option>";
    html += "<option value=" + SelectorModel.AccepterOfDeptStationEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;ポスト（運用部門）によるインテリジェント計算（開発中）</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
//    html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
 //   html += "<input type=button onclick='Help()' value='我需要帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");


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
        case SelectorModel.Station:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        case SelectorModel.Dept:
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

    OpenEasyUiDialogExt(url, 'ポジションを設定する', 800, 500, true);
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
        case SelectorModel.Station:
            roleName = "0.Station.htm";
            break;
        case SelectorModel.Dept:
            roleName = "1.Dept.htm";
            break;
        case SelectorModel.Emp:
            roleName = "2.Emp.htm";
            break;
        case SelectorModel.SQL:
            roleName = "3.SQL.htm";
            break;
        case SelectorModel.SQLTemplate:
            roleName = "4.SQLTemplate.htm";
            break;
        case SelectorModel.GenerUserSelecter:
            roleName = "5.GenerUserSelecter.htm";
            break;
        case SelectorModel.DeptAndStation:
            roleName = "6.DeptAndStation.htm";
            break;
        case SelectorModel.Url:
            roleName = "7.Url.htm";
            break;
        case SelectorModel.BySpecNodeEmp:
            roleName = "8.AccepterOfDeptStationEmp.htm";
            break;
        case SelectorModel.DeptAndStation:
            roleName = "9.AccepterOfDeptStationOfCurrentOper.htm";
            break;
        default:

            roleName = "0.Station.htm";
            break;
    }

    window.location.href = roleName + "?FK_Node=" + nodeID;
}
function SaveAndClose() {
    Save();
    window.close();
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
