function isMobil(s) {
    var patrn = /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/;
    if (!patrn.exec(s.value)) 
    {
       alert('不正な携帯番号.');
       return false;
    }
    return true
}