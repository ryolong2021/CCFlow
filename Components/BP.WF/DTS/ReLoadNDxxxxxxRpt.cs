using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 修复表单物理表字段长度 的摘要说明
    /// </summary>
    public class ReLoadNDxxxxxxRpt : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ReLoadNDxxxxxxRpt()
        {
            this.Title = "フローチャートをクリアして再読み込みします。";
            this.Help = "NDxxxRptテーブルのデータを削除してリロードしてください。この機能の実行には時間がかかると推定されます。データ量が多いと、Webプログラムでの実行に失敗する場合があります。";
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
            string msg = "";

            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            foreach (Flow fl in fls)
            {
                try
                {
                    msg += fl.DoReloadRptData();
                }
                catch(Exception ex)
                {
                    msg += "@フロー(" + fl.Name + ")を処理する時、異常が発生しました。" + ex.Message;
                }
            }
            return "提示："+fls.Count+"件のフローで検察を受けました。情報は以下の通りです。"+msg;
        }
    }
}
