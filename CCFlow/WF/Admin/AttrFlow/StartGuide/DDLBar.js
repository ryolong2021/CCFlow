
function InitBar(optionKey) {

    var html = "<b>事前ナビゲーションを開始する</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=" + StartGuideWay.None + ">なし、設定しないでください（デフォルト）。</option>";
    html += "<option value=" + StartGuideWay.BySQLOne + ">設定されたSQLによると-シングルモード</option>";
    html += "<option value=" + StartGuideWay.SubFlowGuide + ">履歴（クエリ履歴）から開始されたフローのデータをコピーする</option>";
    html += "<option value=" + StartGuideWay.BySelfUrl + ">カスタムUrl</option>";
    html += "<option value=" + StartGuideWay.ByParentFlowModel + ">父子フローモード</option>";

    html += "<option value=" + StartGuideWay.FoolForm + " disabled='disabled'> SQLの設定-複数モード（バッチ起動に使用）</option>";
    html += "<option value=" + StartGuideWay.FoolForm + " disabled='disabled'>サブフローインスタンスリストモード-複数</option>";
    html += "</select >";

    html += "<input id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";
    //    html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    html += "<input id='Btn_Help' type=button onclick='HelpOnline()' value='オンラインヘルプ' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}


function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}

function changeOption() {

    var flowNo = GetQueryString("FK_Flow");
    if (flowNo == null)
        flowNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_Flow=" + flowNo;
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case StartGuideWay.None:
            url = "0.None.htm";
            break;
        case StartGuideWay.BySQLOne:
            url = "1.BySQLOne.htm";
            break;
        case StartGuideWay.SubFlowGuide:
            url = "2.SubFlowGuide.htm";
            break;
        case StartGuideWay.BySelfUrl:
            url = "7.BySelfUrl.htm";
            break;
        case StartGuideWay.ByFrms:
            url = "8.ByFrms.htm";
            break;
        case StartGuideWay.ByParentFlowModel:
            url = "9.ByParentFlowModel.htm";
            break;
        default:
            url = "0.None.htm";
            break;
    }

    return url;
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
$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
            // alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});
