
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

function checkMail(c) {
    var val = c.value;
    if (val == '')
        return;

    //存在@符号
    var a = val.indexOf("@");
    //是否存在点
    var point = val.indexOf(".");
    //存在@，点，并且 点在@之后，且不相邻
    if (a == -1 || point == -1 || point - a <= 1) {
        alert("Emailのフォーマットが間違っています。正しい例abc@ccflow.org");
        c.select();
        c.focus();
        return false;
    }
    //@不能够是第一个字符，点不能够是最后一个字符
    if (a == 0 || point == val.length - 1) {
        alert("Emailのフォーマットが間違っています。正しい例abc@ccflow.org");
        c.select();
        c.focus();
        return false;
    }
    return true;
}