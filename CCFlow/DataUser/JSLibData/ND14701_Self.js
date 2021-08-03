
// 发送前执行数据安全检查.
function CheckBlank() {

    alert('sss');
  if (ReqTB('DianHua') == "") {
        alert( '電話は空ではいけません \t\n');
     return false;
 }

 alert('ok');
    return true;  
}
