using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.GPM;
using BP.En;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class OrgInit_NameOfPath : Method
    {
        /// <summary>
        /// 组织结构处理部门全路径
        /// </summary>
        public OrgInit_NameOfPath()
        {
            this.Title = "組織構造-部門のフルパスNameOfPath";
            this.Help = "すべての部門を循環させ、NameOfPathを再生成します";
            this.GroupName = "組織の解体";

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
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            try
            {
                Depts depts = new Depts();
                depts.RetrieveAll();
                foreach (Dept dept in depts)
                    dept.GenerNameOfPath();

                return "正常に実行しました";
            }
            catch (Exception ex)
            {
                return "実行は失敗しました：" + ex.Message;
            }
        }
    }
}
