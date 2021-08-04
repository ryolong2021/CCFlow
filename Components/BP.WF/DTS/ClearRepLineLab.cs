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
    /// 重新生成标题 的摘要说明
    /// </summary>
    public class ClearRepLineLab : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ClearRepLineLab()
        {
            this.Title = "重複したフォームのLine Labデータを消去します。";
            this.Help = "フォームテンプレートの以前のバグが原因で、ラベルと行のデータが重複しています。";
            this.GroupName = "システムのメンテナンス";

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
            FrmLines ens = new FrmLines();
            ens.RetrieveAllFromDBSource();
            string sql = "";
            foreach (FrmLine item in ens)
            {
                sql = "DELETE FROM " + item.EnMap.PhysicsTable + " WHERE FK_MapData='" + item.FK_MapData + "' AND  x1=" + item.X1 + " and x2=" + item.X2 + " and y1=" + item.Y1 + " and y2=" + item.Y2;
                DBAccess.RunSQL(sql);
                item.MyPK = BP.DA.DBAccess.GenerOIDByGUID().ToString();
                item.Insert();
            }

            FrmLabs labs = new FrmLabs();
            labs.RetrieveAllFromDBSource();
            foreach (FrmLab item in labs)
            {
                sql = "DELETE FROM " + item.EnMap.PhysicsTable + " WHERE FK_MapData='" + item.FK_MapData + "' and x=" + item.X + " and y=" + item.Y + " and Text='" + item.Text + "'";
                DBAccess.RunSQL(sql);
                item.MyPK = BP.DA.DBAccess.GenerOIDByGUID().ToString();
                item.Insert();
            }
            return "正常に削除されました";
        }
    }
}
