/** 数字金额大写转换(可以处理整数,小数,负数) */
function RMBToDX() {

    alert('ss');

    var n = ctrl.value;

    var fraction = ['角', '分'];
    var digit = ['零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖'];
    var unit = [['元', '万', '亿'], ['', '拾', '佰', '仟']];
    var head = n < 0 ? '欠' : '';
    n = Math.abs(n);

    var s = '';

    for (var i = 0; i < fraction.length; i++) {
        s += (digit[Math.floor(n * 10 * Math.pow(10, i)) % 10] + fraction[i]).replace(/零./, '');
    }
    s = s || '整';
    n = Math.floor(n);

    for (var i = 0; i < unit[0].length && n > 0; i++) {
        var p = '';
        for (var j = 0; j < unit[1].length && n > 0; j++) {
            p = digit[n % 10] + unit[1][j] + p;
            n = Math.floor(n / 10);
        }
        s = p.replace(/(零.)*零$/, '').replace(/^$/, '零') + unit[0][i] + s;
    }
    return head + s.replace(/(零.)*零元/, '元').replace(/(零.)+/g, '零').replace(/^整$/, '零元整');
}



// 发送前执行数据安全检查.
function CheckBlank() {
    alert('sss');
    return true;

    var msg = "";
    if (ReqAthFileName('GaoJian') == null) {
        msg += '文書の添付ファイルがアップロードされていません \t\n';
    }
    if (ReqTB('BianXiaoRen') == "") {
        msg += '編集者:空いてはいけません \t\n';
    }

    if (ReqTB('BianXiaoRenDianHua') == "") {
        msg += '編集者電話:空いてはいけません \t\n';
    }

    if (ReqTB('QianFaRen') == "") {
        msg += '発行者:空いてはいけません \t\n';
    }

    if (ReqTB('QianFaRenDianHua') == "") {
        msg += '発行者電話:空いてはいけません \t\n';
    }

    if (ReqTB('WenZhangBiaoTi') == "") {
        msg += 'タイトル:空いてはいけません \t\n';
    }
    if (msg == "")
        return true; /*可以提交.*/
    alert(msg);
    return false; /*不能提交.*/
}
