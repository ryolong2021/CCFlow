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


function InitBar(optionKey) {

    var html = "<b>ポップ戻り値モードの設定</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='None' >なし、設定しない（デフォルト）。</option>";
    html += "<option value='PopBranchesAndLeaf' >トランクリーフパターン</option>";
    html += "<option value='PopBranchesAndLeafLazyLoad' >トランクリーフモード-レイジーローディング</option>";
    html += "<option value='PopBranches' >トランクモード（シンプル）</option>";
    html += "<option value='PopBranchesLazyLoad' >トランクモード（シンプル）-レイジーローディング</option>";
    html += "<option value='PopGroupList' >グループ化されたリストタイル</option>";
    html += "<option value='PopTableList' >単一エンティティタイル</option>";
    html += "<option value='PopBindSFTable' >外部キー（辞書テーブル）テーブルをバインドします</option>";
    html += "<option value='PopBindEnum' >バインドされた列挙</option>";
    html += "<option value='PopTableSearch' >フォーム条件クエリ</option>";
    html += "<option value='PopSelfUrl' >カスタムURL</option>"; 
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
//    html += "<input type='button' value='删除' id='Btn_Delete' name='Btn_Delete' onclick='return Delete()' />"
    html += "<input  id='Btn_FullData' type=button onclick='FullData()' value='塗りつぶし設定' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='オンラインヘルプ' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}

function FullData() {
    var keyOfEn = GetQueryString("KeyOfEn");
    var optionKey = $("#changBar").val();
    var myPK = optionKey+"_" + GetQueryString("FK_MapData") + "_" + GetQueryString("KeyOfEn");
    var url = "../FullData/Default.htm?FK_MapData=" + this.GetQueryString("FK_MapData") + "&RefPK=" + myPK + "&KeyOfEn=" + keyOfEn;

    window.location.href = url;

}



function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function changeOption() {

    var fk_MapData = GetQueryString("FK_MapData");
    var KeyOfEn = GetQueryString("KeyOfEn");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_MapData=" + fk_MapData + "&KeyOfEn=" + KeyOfEn;
}

function GetUrl(popModel) {

    switch (popModel) {
        case "None":
            url = "0.None.htm";
            break;
        case "PopBranchesAndLeaf":
            url = "1.BranchesAndLeaf.htm";
            break;
        case "PopBranchesAndLeafLazyLoad":
            url = "2.BranchesAndLeafLazyLoad.htm";
            break;
        case "PopBranches":
            url = "3.Branches.htm";
            break;
        case "PopBranchesLazyLoad":
            url = "4.BranchesLazyLoad.htm";
            break;
        case "PopGroupList":
            url = "5.GroupList.htm";
            break;
        case "PopTableList":
            url = "6.TableList.htm";
            break;
        case "PopTableSearch":
            url = "7.TableSearch.htm";
            break;
        case "PopSelfUrl":
            url = "8.SelfUrl.htm";
            break;
        case "PopBindEnum":
            url = "9.BindEnum.htm";
            break;
        case "PopBindSFTable":
            url = "10.BindSFTable.htm";
            break;
        default:
            url = "0.None.htm";
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
