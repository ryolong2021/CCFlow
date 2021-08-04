using System;
using System.Data;
using System.Collections;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.DA;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 创建索引
    /// </summary>
    public class CreateIndex : Method
    {
        /// <summary>
        /// 创建索引
        /// </summary>
        public CreateIndex()
        {
            this.Title = "インデックスを作成します（すべてのフローに対して、NDxxxTrack、NDxxRpt、インデックスを作成します。）";
            this.Help = "インデックスフィールドを作成してフローの効率を向上させる.";
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
            string info = "Trackテーブルのインデックスの作成を開始します.";

            Flows fls = new Flows();
            foreach (Flow fl in fls)
            {
                info += fl.CreateIndex();
            }
            return info;

        }
    }
}
