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
    public class DTSCheckFlowAll : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DTSCheckFlowAll()
        {
            this.Title = "身体検査の全過程";
            this.Help = "機能が個別の身体検査フローと同じです、身体検査フローはデータに害を与えません。";
            this.Help += "<br>1、ノードフォームと物理テーブルを修復します。";
            this.Help += "<br>2、前処理およびノー​​ド計算データを生成して、フローの実行速度を最適化します。";
            this.Help += "<br>3、レポートデータを回復します。";
            this.Help += "<br>4、システムは検査の結果を知らせません。";
            this.Help += "<br>5、検査時間の長さは、フローの数、ノードの数、フォームフィールドの数に関係します。しばらくお待ちください。";
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
            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            foreach (Flow fl in fls)
            {
                fl.DoCheck();
            }

            return "提示："+fls.Count+"のフローは検査に参加しました。";
        }
    }
}
