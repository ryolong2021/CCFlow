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
    /// 修改人员编号 的摘要说明
    /// </summary>
    public class GenerPinYinForEmp : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerPinYinForEmp()
        {
            this.Title = "スタッフのピンインを生成し、Port_Emp.PinYinフィールドに入力します。";
            this.Help = "検索を容易にするために、すべての担当者のピンインを生成します。これは、副署、引き渡し、および受信者の問い合わせに便利です。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原用户名", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新用户名", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.IsAdmin == true)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            if (BP.DA.DBAccess.IsView("Port_Emp", SystemConfig.AppCenterDBType) == true)
                return "port_emp ビューはピンインを生成できません。";

            if (BP.DA.DBAccess.IsExitsTableCol("Port_Emp", BP.GPM.EmpAttr.PinYin) == false)
                return "port_emp ピンインは、ピンイン列がないと生成できません.";

            BP.GPM.Emps emps = new BP.GPM.Emps();
            emps.RetrieveAll();
            foreach (BP.GPM.Emp item in emps)
            {
                if (item.PinYin.Contains("/") == true)
                    continue;
                item.Update();
            }
            return "正常に実行しました。";
        }
    }
}
