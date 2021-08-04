using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using System.IO;

namespace BP.Frm
{
    /// <summary>
    /// 功能执行
    /// </summary>
    public class MethodFunc : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(MethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodID, value);
            }
        }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodName);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodName, value);
            }
        }
        public string MsgErr
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MsgErr);
            }
            set
            {
                this.SetValByKey(MethodAttr.MsgErr, value);
            }
        }
        public string MsgSuccess
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MsgSuccess);
            }
            set
            {
                this.SetValByKey(MethodAttr.MsgSuccess, value);
            }
        }


        public string MethodDoc_Url
        {
            get
            {
                string s = this.GetValStringByKey(MethodAttr.MethodDoc_Url);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "http://192.168.0.100/MyPath/xxx.xx";
                return s;
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDoc_Url, value);
            }
        }
        /// <summary>
        /// 获得或者设置sql脚本.
        /// </summary>
        public string MethodDoc_SQL
        {
            get
            {
                string strs = this.GetBigTextFromDB("SQLScript");
                if (strs == null || strs.Equals("") == true)
                    return this.MethodDoc_SQL_Demo; //返回默认信息.
                return strs;
            }
            set
            {
                this.SaveBigTxtToDB("SQLScript", value);
            }
        }
        /// <summary>
        /// 获得该实体的demo.
        /// </summary>
        public string MethodDoc_JavaScript_Demo
        {
            get
            {
                string file = SystemConfig.CCFlowAppPath + "WF" + Path.DirectorySeparatorChar + "CCBill" + Path.DirectorySeparatorChar + "Admin" + Path.DirectorySeparatorChar + "MethodDocDemoJS.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("/#", "+"); //为什么？
                doc = doc.Replace("/$", "-"); //为什么？

                doc = doc.Replace("@FrmID", this.FrmID);

                return doc;
            }
        }
        public string MethodDoc_SQL_Demo
        {
            get
            {
                string file = SystemConfig.CCFlowAppPath + "WF" + Path.DirectorySeparatorChar + "CCBill" + Path.DirectorySeparatorChar + "Admin" + Path.DirectorySeparatorChar + "MethodDocDemoSQL.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("@FrmID", this.FrmID);
                return doc;
            }
        }
        /// <summary>
        /// 获得JS脚本.
        /// </summary>
        /// <returns></returns>
        public string Gener_MethodDoc_JavaScript()
        {
            return this.MethodDoc_JavaScript;
        }

        public string Gener_MethodDoc_JavaScript_function()
        {
            string paras = "";
            MapAttrs attrs = new MapAttrs(this.MyPK);
            foreach (MapAttr item in attrs)
            {
                paras += item.KeyOfEn + ",";
            }
            if (attrs.Count > 1)
                paras = paras.Substring(0, paras.Length-1);

            string strs = " function " + this.MethodID + "(" + paras + ") {";
            strs += this.MethodDoc_JavaScript;
            strs += "}";
            return strs;
        }
        /// <summary>
        /// 获得SQL脚本
        /// </summary>
        /// <returns></returns>
        public string Gener_MethodDoc_SQL()
        {
            return this.MethodDoc_SQL;
        }
        /// <summary>
        /// 获得或者设置js脚本.
        /// </summary>
        public string MethodDoc_JavaScript
        {
            get
            {
                string strs = this.GetBigTextFromDB("JSScript");
                if (strs == null || strs.Equals("") == true)
                    return this.MethodDoc_JavaScript_Demo;

                strs = strs.Replace("/#", "+"); 
                strs = strs.Replace("/$", "-"); 
                return strs;
            }
            set
            {

                this.SaveBigTxtToDB("JSScript", value);
                 
            }
        }

        /// <summary>
        /// 方法类型：@0=SQL@1=URL@2=JavaScript@3=业务单元
        /// </summary>
        public int MethodDocTypeOfFunc
        {
            get
            {
                return this.GetValIntByKey(MethodAttr.MethodDocTypeOfFunc);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDocTypeOfFunc, value);
            }
        }
        /// <summary>
        /// 方法类型
        /// </summary>
        public RefMethodType RefMethodType
        {
            get
            {
                return (RefMethodType)this.GetValIntByKey(MethodAttr.RefMethodType);
            }
            set
            {
                this.SetValByKey(MethodAttr.RefMethodType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 功能执行
        /// </summary>
        public MethodFunc()
        {
        }
        public MethodFunc(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_Method", "機能方法");
                map.AddMyPK();

                map.AddTBString(MethodAttr.FrmID, null, "フォームID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodName, null, "メソッド名", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.MethodID, null, "メソッドID", true, true, 0, 300, 10);

                map.AddDDLSysEnum(MethodAttr.WhatAreYouTodo, 0, "実行後に何をすべきか？", true, true, MethodAttr.WhatAreYouTodo,
                "@0=プロンプトウィンドウを閉じる@1=プロンプトウィンドウを閉じて更新する@2= Search.htmページに移動する");

                map.AddTBString(MethodAttr.WarningMsg, null, "機能実行警告メッセージ", true, false, 0, 300, 10, true);
                map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "表示方法", true, true, MethodAttr.ShowModel,
                  "@0=ボタン @1=ハイパーリンク");

                map.AddDDLSysEnum(MethodAttr.MethodDocTypeOfFunc, 0, "コンテンツタイプ", true, false, "MethodDocTypeOfFunc",
               "@0=SQL@1=URL@2=JavaScript@3=ビジネスユニット");

                map.AddTBString(MethodAttr.MethodDoc_Url, null, "URL実行内容", false, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MsgSuccess, null, "成功メッセージ", true, false, 0, 300, 10, true);
                map.AddTBString(MethodAttr.MsgErr, null, "失敗メッセージ", true, false, 0, 300, 10, true);

                #region 外观.
                map.AddTBInt(MethodAttr.PopHeight, 100, "ポップアップの高さ", true, false);
                map.AddTBInt(MethodAttr.PopWidth, 260, "ポップアップウィンドウの幅", true, false);
                #endregion 外观.


                #region 显示位置控制.
                map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "MyBill.htmツールバーに表示するかどうか", true, true, true);
                map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "MyBill.htmツールバーの右側にほかのボタンに表示するかどうか", true, true, true);
                map.AddBoolean(MethodAttr.IsSearchBar, false, "Search.htmツールバーに表示するかどうか（バッチ処理の場合）", true, true, true);
                #endregion 显示位置控制.

                RefMethod rm = new RefMethod();
                rm.Title = "メソッドパラメータ"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoParas";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "メソッドの内容"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDocs";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 方法参数
        /// </summary>
        /// <returns></returns>
        public string DoParas()
        {
            return "../../CCBill/Admin/MethodParas.htm?MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 方法内容
        /// </summary>
        /// <returns></returns>
        public string DoDocs()
        {
            return "../../CCBill/Admin/MethodDoc.htm?MyPK=" + this.MyPK;
        }
    }
    /// <summary>
    /// 功能执行
    /// </summary>
    public class MethodFuncs : EntitiesMyPK
    {
        /// <summary>
        /// 功能执行
        /// </summary>
        public MethodFuncs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodFunc();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodFunc> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodFunc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodFunc> Tolist()
        {
            System.Collections.Generic.List<MethodFunc> list = new System.Collections.Generic.List<MethodFunc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodFunc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
