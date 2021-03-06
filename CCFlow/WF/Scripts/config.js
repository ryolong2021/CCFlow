// UI风格配置. UIPlant, 为了适应不同风格的版本需要. 我们增加这个配置, UIPlant=BS,Ele.
var uiPlant = 'BS'; //风格文件.

//  For .net 后台的调用的url ,  java的与.net的不同.
var plant = 'CCFlow'; //运行平台.
var basePath = basePath();
var Handler = "Handler.ashx"; //处理器,一般来说，都放在与当前处理程序的相同的目录下。
var MyFlow = "MyFlow.ashx"; //工作处理器.
var webUser = null; //定义通用变量用户信息
// API SubscriptionKey
var OcpApimSubscriptionKey = "e96961c3d7c64dde84137075fbc20179";
// API URL
var ApidynamicHandler = "https://aeonapi.azure-api.net/ebs/?";

function basePath() {
    //获取当前网址，如： http://localhost:80/jflow-web/index.jsp  
    var curPath = window.document.location.href;
    //获取主机地址之后的目录，如： jflow-web/index.jsp  
    var pathName = window.document.location.pathname;
    if (pathName == "/") { //说明不存在项目名
        return curPath;
    }
    var pos = curPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:80  
    var localhostPath = curPath.substring(0, pos);
    //获取带"/"的项目名，如：/jflow-web
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);

    return localhostPath;

}


/**
 * 获取项目路径
 * @returns
 */
function getContextPath(){
	return basePath.substring(basePath.lastIndexOf("/"));
}

//公共方法
function Handler_AjaxQueryData(param, callback, scope, method, showErrMsg) {
    if (!method) method = 'GET';
    $.ajax({
        type: method, //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "text/plain; charset=utf-8",
        url: Handler, //要访问的后台地址.
        data: param, //要发送的数据.
        async: true,
        cache: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}

 
// for Java.
// var controllerURLConfig = '/WF/Admin/CCFormDesigner/CCFromHandler';
// var plant = 'JFlow'; //运行平台.
// var Handler = "Handler";

 


