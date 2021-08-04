using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.WF.HttpHandler;

namespace BP.Frm
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Admin : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Admin()
        {
        }
        /// <summary>
        /// 获得js,sql内容.
        /// </summary>
        /// <returns></returns>
        public string MethodDoc_GetScript() 
        {
            var en = new BP.Frm.MethodFunc(this.MyPK);
            int type = this.GetRequestValInt("TypeOfFunc");
            if (type == 0)
                return en.MethodDoc_SQL;

            if (type == 1)
                return en.MethodDoc_JavaScript;

            if (type == 2)
                return en.MethodDoc_Url;

            return "err@判断されないタイプ。";
        }
        /// <summary>
        /// 保存脚本
        /// </summary>
        /// <returns></returns>
        public string MethodDoc_SaveScript()
        {
            var en = new BP.Frm.MethodFunc(this.MyPK);

            int type = this.GetRequestValInt("TypeOfFunc");
            string doc = this.GetRequestVal("doc");

            //sql模式.
            if (type == 0)
                en.MethodDoc_SQL = doc;

            //script.
            if (type == 1)
            {
                en.MethodDoc_JavaScript = doc;

                //string path=SystemConfig.PathOfDataUser + "JSLibData\\Method\\" ;
                //if (System.IO.Directory.Exists(path) == false)
                //    System.IO.Directory.CreateDirectory(path);
                ////写入文件.
                //string file = path + en.MyPK + ".js";
                //DataType.WriteFile(file, doc);
            }

            //url.
            if (type == 2)
                en.MethodDoc_Url = doc;

            en.MethodDocTypeOfFunc = type;
            en.Update();

            return "正常に保存しました。";
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "正常に実行しました。";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@マーク[" + this.DoType + "]DoMethod=[" + this.GetRequestVal("DoMethod") + "]、が見つかりません。@ RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.
    }
}
