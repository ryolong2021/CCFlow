function Rmb2DaXie(s) {
    clearNoNum(s);  //此处可预防如果控件是普通的文本框时，录入非数字的符号自动去除
    var rmb = s.value;
    var dx = AmountLtoU(rmb);
    if (dx == '無効') {
        alert('無効な数の書式！');
        return false;
    }

    if(!dxctrl){
        var ctrls = document.getElementsByTagName("input");
        var idx;
        var ctrlid = 'TB_' + DaXie_Ctrl_ID;

        for (var i = 0; i < ctrls.length; i++) {
            idx = ctrls[i].id.indexOf(ctrlid);
            if (idx != -1 && (idx + ctrlid.length) == ctrls[i].id.length) {
                dxctrl = ctrls[i];
                break;
            }
        }
    }

    if (!dxctrl) {
        alert('idが見つかりませんでしたTB_' + DaXie_Ctrl_ID + 'のテキストボックス！管理者に連絡して、フローノードに対応するJSファイルを修正してくださいDataUser/JSLibData/ND' + getArgsFromHref('FK_Node') + '.js中にDaXie_Ctrl_ID大文字の金額を保存するテキストフィールドの名前を変更します！');
        return false;
    }

    dxctrl.value = dx;
    return true
}

var DaXie_Ctrl_ID = 'DaXie';
var dxctrl;

function AmountLtoU(num) {
    ///<summery>小写金额转化大写金额</summery>
    ///<param name=num type=number>金额</param>
    if (isNaN(num)) return "無効";
    var strPrefix = "";
    if (num < 0) strPrefix = "(负)";
    num = Math.abs(num);
    if (num > 999000000000000) return "超额(不大于999万亿)";    //不超过999万亿
    var strOutput = "";
    var strUnit = '佰拾万仟佰拾亿仟佰拾万仟佰拾圆角分';
    var strCapDgt = '零壹贰叁肆伍陆柒捌玖';
    num += "00";
    var intPos = num.indexOf('.');
    if (intPos >= 0) {
        num = num.substring(0, intPos) + num.substr(intPos + 1, 2);
    }
    strUnit = strUnit.substr(strUnit.length - num.length);
    for (var i = 0; i < num.length; i++) {
        strOutput += strCapDgt.substr(num.substr(i, 1), 1) + strUnit.substr(i, 1);
    }
    return strPrefix + strOutput.replace(/零角零分$/, '整').replace(/零[仟佰拾]/g, '零').replace(/零{2,}/g, '零').replace(/零([亿|万])/g, '$1').replace(/零+圆/, '圆').replace(/亿零{0,3}万/, '亿').replace(/^圆/, "零圆");
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
    obj.value = obj.value.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符
    obj.value = obj.value.replace(/^\./g, "");  //验证第一个字符是数字而不是.
    obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的.
    obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
}