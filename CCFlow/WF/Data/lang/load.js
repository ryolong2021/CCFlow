//定义多语言.
//zh-cn   zh-tw  zh-hk  en-us ja-jp ko-kr
var lang = navigator.language||navigator.userLanguage;
lang = lang.substr(0, 2);
if(lang == 'zh'){
	var currentLang = "zh-cn";
}else if(lang == 'ja'){
	var currentLang = "ja-jp";
}else{
	var currentLang = "en-us";
}

//document.write("<script language=javascript src='./Data/lang/js/" + currentLang + ".js'></script>");
//document.write("<script language=javascript src='../Data/lang/js/" + currentLang + ".js'></script>");
document.write("<script language=javascript src='../WF/Data/lang/js/" + currentLang + ".js'></script>");