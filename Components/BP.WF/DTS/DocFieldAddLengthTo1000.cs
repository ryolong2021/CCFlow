using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 扩充Doc字段的长度 的摘要说明
    /// </summary>
    public class DocFieldAddLengthTo1000 : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DocFieldAddLengthTo1000()
        {
            this.Title = "Docフィールドの長さを拡張する";
            this.Help = "doc タイプのフィールドの長さを拡張し、1000の下の文字を1000に拡張します。";
            this.Help += "<br>低減の原因は実施のために文字長を無視するので、画面にエラーが発生しました。";
            this.GroupName = "システムのメンテナンス";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
       
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No == "admin")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string strs = "実行を開始...";
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.MyDataType, DataType.AppString, MapAttrAttr.FK_MapData);
            strs += "<br>@次のフィールドに影響を与えました。";
            foreach (MapAttr attr in attrs)
            {
                if (attr.UIHeightInt > 50 && attr.MaxLen < 1000 )
                {
                    strs += " @ クラス:" + attr.FK_MapData + " フィールド:" + attr.KeyOfEn + " , " + attr.Name + " "; 
                    attr.MaxLen = 1000;
                    attr.Update();
                }
            }
            return "正常に実行しました。"+strs;
        }
    }
}
