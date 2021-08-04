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
    public class ChangeUserNo : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ChangeUserNo()
        {
            this.Title = "スタッフ番号を変更します（元の番号は操作でAと呼ばれていましたが、現在はBに変更されています）。";
            this.Help = "注意して実行してください。実行前にデータベースをバックアップしてください。システムは生成されたSQLをログに入れ、ログファイルを開いて、(" + BP.Sys.SystemConfig.PathOfDataUser + "\\Log)、そしてこれらのSQLを見つけます.";
            this.GroupName = "システムのメンテナンス";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            this.Warning = "実行してもよろしいですか？";
            HisAttrs.AddTBString("P1", null, "元のユーザー名", true, false, 0, 10, 10);
            HisAttrs.AddTBString("P2", null, "新しいユーザー", true, false, 0, 10, 10);
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
            string oldNo = this.GetValStrByKey("P1");
            string newNo = this.GetValStrByKey("P2");

            string sqls = "";

            sqls += "UPDATE Port_Emp Set No='" + newNo + "' WHERE No='" + oldNo + "'";
            sqls += "\t\n UPDATE " + BP.WF.Glo.EmpStation + " Set FK_Emp='" + newNo + "' WHERE FK_Emp='" + oldNo + "'";

            MapDatas mds = new MapDatas();
            mds.RetrieveAll();

            foreach (MapData md in mds)
            {
                MapAttrs attrs = new MapAttrs(md.No);
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIIsEnable == false && attr.DefValReal == "@WebUser.No")
                    {
                        sqls += "\t\n UPDATE " + md.PTable + " SET ";
                    }
                    continue;

                }
                sqls += "UPDATE";

            }

            return "正常に実行しました。" + sqls;
        }
    }
}
