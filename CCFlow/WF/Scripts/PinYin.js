
//生成拼音.
function ParsePinYin(str, model, textBoxId) {

    var data = SpecWords(str);
    if (data == null) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
        handler.AddPara("name", str);
        handler.AddPara("flag", model);

        data = handler.DoMethodReturnString("ParseStringToPinyin");
    }

    if (textBoxId == undefined || textBoxId == null)
        return data;

    document.getElementById(textBoxId).value = data;
    return data;
}

function StrToPinYin(str) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    handler.AddPara("name", str);
    handler.AddPara("flag", "false");
    data = handler.DoMethodReturnString("ParseStringToPinyin");
    return data;
}

//特别词汇.
function SpecWords(str) {

    if (str == '単価') return 'DanJia';
    if (str == '単位') return 'DanWei';

    if (str == '名前') return 'MingCheng';
    if (str == '項目番号') return 'PrjNo';
    if (str == '項目名') return 'PrjName';
    if (str == '電話') return 'Tel';
    if (str == '住所') return 'Addr';
    if (str == 'メール') return 'Email';
    if (str == '携帯電話') return 'Mobile';
    if (str == '合計') return 'HeJi';
    if (str.indexOf('番号付け') != -1) return 'BillNo';
    if (str.indexOf('レシート') != -1) return 'BillNo';

 //   str = str.replace('单', 'Dan');
 //   str = str.replace('称', 'Cheng');

    return null;
}