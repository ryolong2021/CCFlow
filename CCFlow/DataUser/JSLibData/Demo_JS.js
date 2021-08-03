
function isPostalCode(c) {
    var val = c.value;
    if (val == '')
        return;
    if (val.length != 6) {
        c.select();
        c.focus();
        alert('郵便番号不正フォーマット.');
        return false;
    }
    var patrn = /^[0-9]+$/;
    if (!patrn.exec(val)) {
        c.select();
        c.focus();
        alert('郵便番号不正フォーマット.');
        return false;
    }
    return true
}
