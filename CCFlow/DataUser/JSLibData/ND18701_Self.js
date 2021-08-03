
// 发送前执行数据安全检查.
function CheckBlank() {

    alert('sssddddddddddddddd');
 //   return true;

    var msg = "";

 if (ReqTB('QingJiaShiJianCong') == "") {
        msg += '開始日:空ではいけません \t\n';
    }

 if (ReqTB('Dao') == "") {
        msg += '終了日：空ではいけません \t\n';
    }

 if (msg == "")
        return true; /*可以提交.*/

    alert(msg);
    return false; /*不能提交.*/
   
}
