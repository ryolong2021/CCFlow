function isRegisterUserName(s) {
    var patrn = /^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){4,19}$/;
    if (!patrn.exec(s.value))
   {
       alert('不正なユーザ名フォーマット.');
       return false;
    }
    return true
}