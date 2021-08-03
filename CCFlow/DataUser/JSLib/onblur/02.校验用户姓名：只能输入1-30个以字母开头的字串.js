function isTrueName(s) {
    var patrn = /^[a-zA-Z]{1,30}$/;
    if (!patrn.exec(s.value))
    {
       alert('不正なユーザ名フォーマット.');
       return false;
    }
    return true
} 