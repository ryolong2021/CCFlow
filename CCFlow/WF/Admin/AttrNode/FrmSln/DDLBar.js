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
function InitBar(optionKey) {

    var html = "フォームスキーム：";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+ビルトインフォーム</option>";
    html += "<option value=" + FormSlnType.FoolForm + ">&nbsp;&nbsp;ばか形式（デフォルト）</option>";
    html += "<option value=" + FormSlnType.FreeForm + ">&nbsp;&nbsp;自由形式</option>";
    html += "<option value=" + FormSlnType.FoolTruck + " >&nbsp;&nbsp;累積モード形式</option>";
    html += "<option value=" + FormSlnType.Developer + " >&nbsp;&nbsp;開発者フォーム</option>";
    html += "<option value=" + FormSlnType.WebOffice + "  >&nbsp;&nbsp;公式文書フォーム（ウェブオフィス）</option>";


    html += "<option value=null  disabled='disabled'>+カスタムフォーム</option>";
    html += "<option value=" + FormSlnType.SelfForm + " >&nbsp;&nbsp;埋め込みフォーム</option>";
    html += "<option value=" + FormSlnType.SDKForm + " >&nbsp;&nbsp;SDKフォーム（私のカスタムフォーム）</option>";


    html += "<option value=null  disabled='disabled'>+フォームをフォームライブラリにバインドする</option>";
    html += "<option value=" + FormSlnType.RefOneFrmTree + " >&nbsp;&nbsp;フォームライブラリにバインドされたフォーム</option>";
    html += "<option value=" + FormSlnType.SheetTree + " >&nbsp;&nbsp;複数のフォームのバインド（フォームツリー）</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //   html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";

    //  html += "<input type=button onclick='OldVer()' value='使用旧版本' />";

    //  html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='オンラインヘルプ' />";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function OldVer() {

    var nodeID = GetQueryString("FK_Node");
    var flowNo = GetQueryString("FK_Flow");
    var url = '../NodeFromWorkModel.htm?FK_Flow=' + flowNo + '&FK_Node=' + nodeID;
    window.location.href = url;
}

///设置表单类型.
function SetNDxxRpt_FrmType(flowNo, frmType) {

    var flowID = parseInt(flowNo);
    var frmID = "ND" + flowID + "Rpt";

    var mapData = new Entity("BP.Sys.MapData", frmID);
    mapData.FrmType = frmType;
    mapData.Update();

}


//打开傻瓜表单设计器.
function DFoolFrm() {

    var nodeID = GetQueryString("FK_Node");
    var node = new Entity("BP.WF.Node", nodeID);
    var url = '../../FoolFormDesigner/Designer.htm?FK_Flow=' + node.FK_Flow + '&FK_Node=' + nodeID + "&FK_MapData=ND" + nodeID;
    window.open(url);
    //OpenEasyUiDialogExt(url, '傻瓜表单设计器', 800, 500, false);
}

//打开自由表单设计器.
function DFreeFrm() {
    var nodeID = GetQueryString("FK_Node");
    var node = new Entity("BP.WF.Node", nodeID);
    var url = '../../CCFormDesigner/FormDesigner.htm?FK_Flow=' + node.FK_Flow + '&FK_Node=' + nodeID + "&FK_MapData=ND" + nodeID;

    window.open(url);

    //OpenEasyUiDialogExt(url, '傻瓜表单设计器', 1100, 600, false);
}


//打开 开发者 表单设计器.
function DDeveloper() {
    var nodeID = GetQueryString("FK_Node");
    var node = new Entity("BP.WF.Node", nodeID);
    var url = '../../DevelopDesigner/Designer.htm?FK_Flow=' + node.FK_Flow + '&FK_Node=' + nodeID + "&FK_MapData=ND" + nodeID;

    window.open(url);
    //OpenEasyUiDialogExt(url, '开发者表单设计器', 1100, 600, false);
}


function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}



function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var roleName = "";
    switch (parseInt(optionKey)) {
        case FormSlnType.FoolForm:
            url = "0.FoolForm.htm";
            break;
        case FormSlnType.FreeForm:
            url = "1.FreeForm.htm";
            break;
        case FormSlnType.SelfForm:
            url = "2.SelfForm.htm";
            break;
        case FormSlnType.SDKForm:
            url = "3.SDKForm.htm";
            break;
        case FormSlnType.SLForm:
            url = "4.SLForm.htm";
            break;
        case FormSlnType.SheetTree:
            url = "5.SheetTree.htm";
            break;
        case FormSlnType.SheetAutoTree:
            url = "6.SheetAutoTree.htm";
            break;
        case FormSlnType.WebOffice:
            url = "7.WebOffice.htm";
            break;
        case FormSlnType.ExcelForm:
            url = "8.ExcelForm.htm";
            break;
        case FormSlnType.WordForm:
            url = "9.WordForm.htm";
            break;
        case FormSlnType.FoolTruck:
            url = "10.FoolTruck.htm";
            break;
        case FormSlnType.RefOneFrmTree:
            url = "11.RefOneFrmTree.htm";
            break;
        case FormSlnType.Developer:
            url = "12.Developer.htm";
            break;
        case FormSlnType.DisableIt:
            url = "100.DisableIt.htm";
            break;
        default:
            url = "0.FoolForm.htm";
            break;
    }
    window.location.href = url + "?FK_Node=" + nodeID;
}


function CheckFlow(flowNo) {
    var flow = new Entity('BP.WF.Flow', flowNo);
    flow.DoMethodReturnString("DoCheck"); //重置密码:不带参数的方法. 
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
