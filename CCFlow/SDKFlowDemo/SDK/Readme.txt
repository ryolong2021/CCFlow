
ccflow爱好者必读:
------------------------------------------------------------------------------------
1, /SDKFlowDemo/SDK/ 下面是一个经典简单的demo程序，它包括了如下几方面的内容。
   1， 4大功能界面的api调用，通过查看代码，您就会知道如何使用ccflow的api 来div自己的mis系统。
   2，登录接口调用Login.aspx,  
   3，一些sdk流程演示，它与ccflow的demo流程相结合。

2, 目录解构说明。
   1， /SDK/CCFlow/ 存放了ccflow开发者们做的一些组件，当然你可以不用他们。

   2， 以F+流程编号开头的目录就是存放流程sdk表单的目录，比如: F110 对应流程编号为 110的流程业务表单。

   3， 每个独立表单都有S开头加上节点编号，比如： S11001.aspx. 他是节点编号为11001的业务表单。

   4， ccflow允许一个业务表单绑定到节点上。
       绑定方法： 
	      第1步：打开节点属性选择表单类型为，sdk 表单。
	      第2步：配置参数例如:/SDKFlowDemo/SDK/F110/S11001.aspx。

3, Info.aspx 是一个消息提示功能页面，在一个功能执行完后，把执行的消息通过这个通用的界面提示出来。