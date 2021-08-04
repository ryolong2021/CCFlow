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
    /// Method 的摘要说明
    /// </summary>
    public class UpdateCCFlow : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public UpdateCCFlow()
        {
            this.Title = "ccflowのアップグレード";
            this.Help = "ccflowのアップグレードを実行します。最新のコードを更新する場合は、この機能を実行してccflowのデータベースをアップグレードする必要があります。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
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
            if (BP.Web.WebUser.No != "admin")
                return "不正なユーザー実行。";

            BP.WF.Glo.UpdataCCFlowVer();
 
            return "正常に実行され、システムはデータベースの最新バージョンを修復しました.";
        }
    }
}
