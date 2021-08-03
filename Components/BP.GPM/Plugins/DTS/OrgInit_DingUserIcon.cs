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
    /// 钉钉人员头像同步
    /// </summary>
    public class OrgInit_DingUserIcon : Method
    {
        /// <summary>
        /// 钉钉人员头像同步
        /// </summary>
        public OrgInit_DingUserIcon()
        {
            this.Title = "DingTalkのアバターはDataUser/Iconに同期されます";
            this.Help = "この機能は、DingTalkエンタープライズアカウントのすべての担当者のアバターを、大きな画像と小さな画像を含むローカルエリアにダウンロードします。";
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
                if (BP.GPM.Glo.IsEnable_DingDing == true)
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
            string savePath = BP.Sys.SystemConfig.PathOfDataUser + "UserIcon";
            bool result = ding.DownLoadUserIcon(savePath);
            if (result == true)
                return "実行は成功しました。";
            else
                return "実行は失敗しました。";
        }
    }
}
