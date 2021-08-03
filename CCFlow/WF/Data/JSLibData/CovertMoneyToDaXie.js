function Rmb2DaXie(s) {
    s = clearNoNum(s.toString());  //此处可预防如果控件是普通的文本框时，录入非数字的符号自动去除
    var dx = AmountLtoU(s);
    if (dx == '無効') {
        alert('無効な数字の書式！');
        return false;
    }

    return dx;
}

var DaXie_Ctrl_ID = 'DaXie';
var dxctrl;

function AmountLtoU(num) {
    ///<summery>小写金额转化大写金额</summery>
    ///<param name=num type=number>金额</param>
    if (isNaN(num)) return "無効";
    var strPrefix = "";
    if (num < 0) strPrefix = "(マイナス)";
    num = Math.abs(num);
    if (num > 999000000000000) return "超過額（999兆円を超えない）";    //不超过999万亿
    var strOutput = "";
    var strUnit = '百十万千百十億千百十万千百十円十銭';
    var strCapDgt = '一二三四五六七八九';
    num += "00";
    var intPos = num.indexOf('.');
    if (intPos >= 0) {
        num = num.substring(0, intPos) + num.substr(intPos + 1, 2);
    }
    strUnit = strUnit.substr(strUnit.length - num.length);
    for (var i = 0; i < num.length; i++) {
        strOutput += strCapDgt.substr(num.substr(i, 1), 1) + strUnit.substr(i, 1);
    }
    return strPrefix + strOutput.replace(/ゼロ玉ゼロ銭$/, '整').replace(/ゼロ[仟佰拾]/g, 'ゼロ').replace(/ゼロ{2,}/g, 'ゼロ').replace(/ゼロ([億|万])/g, '$1').replace(/ゼロ+円/, '円').replace(/億ゼロ{0,3}万/, '億').replace(/^円/, "ゼロ円");
};

function getArgsFromHref(sArgName) {
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";

    if (args[0] == sHref) /*参数为空*/
    {
        return retval; /*无需做任何处理*/
    }

    var str = args[1];
    args = str.split("&");

    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }

    return retval;
}

function clearNoNum(obj) {
    obj = obj.toString();
    obj = obj.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符
    obj = obj.replace(/^\./g, "");  //验证第一个字符是数字而不是.
    obj = obj.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的.
    obj = obj.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    return obj;
}