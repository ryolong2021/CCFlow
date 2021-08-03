
// 发送前执行数据安全检查.
function CheckBlank() {
     
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
