using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class GenerDeptTree : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerDeptTree()
        {
            this.Title = "部門のPort_DeptテーブルのTreeNoフィールドを生成します。ルートノードは01です。。";
            this.Help = "このフィールドはLIKEクエリにのみ使用され、関連する主キーとしては使用できません。これは、フィールドが変化し、部門が増えるにつれて変化するためです。";
            this.Help += "この機能を実行するには、1. Port_Dept、TreeNoフィールドが必要です。 2. Port_DeptにはDeptTreeNoフィールドが必要です。3. Port_DeptEmpにはDeptTreeNoフィールドが必要です。4. Port_DeptEmpStationにはDeptTreeNoフィールドが必要です。";
            //  this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成的路径", true, false, 1, 1900, 200);
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
            if (DBAccess.IsExitsTableCol("Port_Dept", "TreeNo") == false)
                return "err@ Port_DeptのTreeNoの列が見つかりません。";

            BP.GPM.Dept dept = new GPM.Dept();
            int i = dept.Retrieve(DeptAttr.ParentNo, "0");
            if (i == 0)
                return "err@ParentNo=0のルートノードが見つかりませんでした。";

            //更新跟节点的TreeNo. 
            string sql = "UPDATE Port_Dept SET TreeNo='01' WHERE No='" + dept.No + "'";
            DBAccess.RunSQL(sql);

            BP.Port.Depts depts = new Depts();
            depts.Retrieve(BP.Port.DeptAttr.ParentNo, dept.No);

            int idx = 0;
            foreach (BP.Port.Dept item in depts)
            {
                idx++;
                 
                string subNo = idx.ToString().PadLeft(2, '0');
                sql = "UPDATE Port_Dept SET TreeNo='01" + subNo + "' WHERE No='" + item.No + "'";
                DBAccess.RunSQL(sql);

                sql = "UPDATE Port_DeptEmp SET DeptTreeNo='01" + subNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);
                sql = "UPDATE Port_DeptEmpStation SET DeptTreeNo='01" + subNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);

                SetDeptTreeNo(item, "01"+subNo);
            }

            return "正常に実行しました。";
        }

        public void SetDeptTreeNo(Dept dept, string pTreeNo)
        {
            BP.Port.Depts depts = new Depts();
            depts.Retrieve(BP.Port.DeptAttr.ParentNo, dept.No);

            int idx = 0;
            foreach (BP.Port.Dept item in depts)
            {
                idx++;
                string subNo = idx.ToString().PadLeft(2, '0');
                string sql = "UPDATE Port_Dept SET TreeNo='" + pTreeNo + "" + subNo + "' WHERE No='" + item.No + "'";
                DBAccess.RunSQL(sql);

                //更新其他的表字段.
                sql = "UPDATE Port_DeptEmp SET DeptTreeNo='" + pTreeNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);
                sql = "UPDATE Port_DeptEmpStation SET DeptTreeNo='" + pTreeNo + "' WHERE FK_Dept='" + item.No + "'";
                DBAccess.RunSQL(sql);

                //递归调用.
                SetDeptTreeNo(item, pTreeNo + subNo);
            }
        }
    }
}
