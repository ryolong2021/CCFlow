using System;
using System.Collections;
using System.Reflection;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.EAI.Plugins.DTS
{
    /// <summary>
    /// 钉钉组织结构同步
    /// </summary>
    public class OrgInit_Ding : Method
    {
        /// <summary>
        /// 钉钉组织结构同步
        /// </summary>
        public OrgInit_Ding()
        {
            this.Title = "DingTalkアドレス帳をCCGPMに同期";
            this.Help = "この関数は、最初に<b style = 'color：red;'>組織構造をクリア</b>してから、DingTalkアドレス帳を同期します。 <br> Dingding関連の構成はWeb.configに書き込まれ、構成が正しい場合にのみ実行できます";
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
                if(BP.GPM.Glo.IsEnable_DingDing == true)
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
            DingDing ding = new DingDing();
            bool result = ding.AnsyOrgToCCGPM();
            if (result == true)
                return "実行は成功しました。";
            else
                return "実行は失敗しました。";
        }
    }
}
