
function InitBar(optionKey) {

    var html = "<b>データ入力</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='Main' >メインテーブルにデータを入力します</option>";
    html += "<option value='Dtls' >テーブルから記入</option>";
    html += "<option value='DDLs' >ドロップダウンボックスに入力します</option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存する' />";
    html += "<input type='button' value='戻る' onclick='Back()' id='Btn_Back' title='' />"
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='オンラインヘルプ' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}


function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function Back() {

//    var myPK = GetQueryString('MyPK');
    var refPK = GetQueryString("RefPK");
    //    var keyOfEn = refPK.split("_")[2];
    var extType = GetQueryString("ExtType");


    if (refPK.indexOf('TBFullCtrl') == 0)
        var url = '../TBFullCtrl/Default.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&KeyOfEn=" + GetQueryString("KeyOfEn");

    if (refPK.indexOf('DDLFullCtrl') == 0)
        var url = '../MapExt/DDLFullCtrl2019.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&KeyOfEn=" + GetQueryString("KeyOfEn") + "&ExtType=" + extType;

    if (refPK.indexOf('Pop') == 0)
        var url = '../Pop/Default.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&KeyOfEn=" + GetQueryString("KeyOfEn");

  
    window.location.href = url;
    return;
}

function changeOption() {

    var refPK = GetQueryString("RefPK");
    var fk_mapData = GetQueryString("FK_MapData");


    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?RefPK=" + refPK + "&FK_MapData=" + fk_mapData + "&KeyOfEn=" + GetQueryString('KeyOfEn');
}

function GetUrl(popModel) {

    switch (popModel) {
        case "DDLs":
            url = "DDLs.htm";
            break;
        case "Dtls":
            url = "Dtls.htm";
            break;
        case "Main":
            url = "Main.htm";
            break;
        default:
            url = "Main.htm";
            break;
    }
    return url;
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
