using System;
using System.Collections.Generic;
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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_RptSearch : DirectoryPageBase
    {
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
                    return "実行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@マーク[" + this.DoType + "]、が見つかりません。@ RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_RptSearch()
        {
        }

        #region 关键字查询.
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string KeySearch_OpenFrm()
        {
            BP.WF.HttpHandler.WF_RptSearch search = new WF_RptSearch();
            return search.KeySearch_OpenFrm();
        }
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public string KeySearch_Query()
        {
            BP.WF.HttpHandler.WF_RptSearch search = new WF_RptSearch();
            return search.KeySearch_Query();
        }
        #endregion 关键字查询.

    }
}
