
function isPasswd(s) {
    var patrn = /^(\w){6,20}$/;
    if (!patrn.exec(s.value)) 
    {
       alert('不正なパスワードフォーマット.');
       return false;
    }
    return true
}