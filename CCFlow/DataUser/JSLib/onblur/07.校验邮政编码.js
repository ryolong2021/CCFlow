function isPostalCode(s) {
    var patrn = /^[a-zA-Z0-9 ]{3,12}$/;
    if (!patrn.exec(s.value)) 
    {
       alert('郵便番号不正フォーマット.');
       return false;
    }
    return true
} 
