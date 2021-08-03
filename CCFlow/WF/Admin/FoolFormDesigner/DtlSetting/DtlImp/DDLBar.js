
function InitBar(optionKey) {

    var html = "<b>テーブルからスキーマをインポートします</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=0 >なし、設定しない（デフォルト）。</option>";
    html += "<option value=1 >テーブルクエリモード（高級）</option>";
    html += "<option value=2 >Excelファイルモード</option>";
    html += "<option value=3 >ドキュメントモード</option>";
    html += "<option value=4 >テーブルクエリモード（簡潔）</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type='button' onclick='Save()' value='保存' />";
    html += "<input  id='Btn_Delete' type='button' name='Btn_Delete' onclick='Delete()' value='削除' />";
    html += "<input  id='Btn_Help' type='button' onclick='HelpOnline()' value='オンラインヘルプ' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}


function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function changeOption() {

    var fk_MapDtl = GetQueryString("FK_MapDtl");
    if (fk_MapDtl == null)
        fk_MapDtl = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_MapDtl=" + fk_MapDtl;
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case 0:
            url = "0.None.htm";
            break;
        case 1:
            url = "1.Table.htm";
            break;
        case 2:
            url = "2.ExcelFile.htm";
            break;
        case 3:
            url = "3.BillModel.htm";
            break;
        case 4:
            url = "4.TableSimple.htm";
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
