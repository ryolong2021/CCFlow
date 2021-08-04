using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using System.IO;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class GenerTemplate : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerTemplate()
        {
            this.Title = "フローテンプレートとフォームテンプレートを生成する";
            this.Help = "システム内のフローとフォームをテンプレートに変換し、指定されたディレクトリに配置します。";
            this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成されたパス", true, false, 1, 1900, 200);
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
            string path = this.GetValStrByKey("Path") + "_" + DateTime.Now.ToString("yy年MM月dd日HH時mm分");
            if (System.IO.Directory.Exists(path))
                return "システムは実行中です。お待ちください。";

            System.IO.Directory.CreateDirectory(path);
            System.IO.Directory.CreateDirectory(path + Path.DirectorySeparatorChar + "Flow.フローテンプレート");
            System.IO.Directory.CreateDirectory(path + Path.DirectorySeparatorChar + "Frm.フォームテンプレート");

            Flows fls = new Flows();
            fls.RetrieveAll();
            FlowSorts sorts = new FlowSorts();
            sorts.RetrieveAll();

            // 生成流程模板。
            foreach (FlowSort sort in sorts)
            {
                string pathDir = path + Path.DirectorySeparatorChar + "Flow.フローテンプレート" + Path.DirectorySeparatorChar + sort.No + "." + sort.Name;
                System.IO.Directory.CreateDirectory(pathDir);
                foreach (Flow fl in fls)
                {
                    fl.DoExpFlowXmlTemplete(pathDir);
                }
            }

            // 生成表单模板。
            foreach (FlowSort sort in sorts)
            {
                string pathDir = path + Path.DirectorySeparatorChar + "Frm.フォームテンプレート" + Path.DirectorySeparatorChar + sort.No + "." + sort.Name;
                System.IO.Directory.CreateDirectory(pathDir);
                foreach (Flow fl in fls)
                {
                    string pathFlowDir = pathDir + Path.DirectorySeparatorChar + fl.No + "." + fl.Name;
                    System.IO.Directory.CreateDirectory(pathFlowDir);
                    Nodes nds = new Nodes(fl.No);
                    foreach (Node nd in nds)
                    {
                        MapData md = new MapData("ND" + nd.NodeID);
                        System.Data.DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);
                        ds.WriteXml(pathFlowDir + Path.DirectorySeparatorChar + nd.NodeID + "." + nd.Name + ".Frm.xml");
                    }
                }
            }

            // 独立表单模板.
            SysFormTrees frmSorts = new SysFormTrees();
            frmSorts.RetrieveAll();
            foreach (SysFormTree sort in frmSorts)
            {
                string pathDir = path + Path.DirectorySeparatorChar + "Frm.フォームテンプレート" + Path.DirectorySeparatorChar + sort.No + "." + sort.Name;
                System.IO.Directory.CreateDirectory(pathDir);

                MapDatas mds = new MapDatas();
                mds.Retrieve(MapDataAttr.FK_FrmSort, sort.No);
                foreach (MapData md in mds)
                {
                    System.Data.DataSet ds =BP.Sys.CCFormAPI.GenerHisDataSet(md.No);
                    ds.WriteXml(pathDir + Path.DirectorySeparatorChar + md.No + "." + md.Name + ".Frm.xml");
                }
            }
            return "正常に生成されました、開いてください" + path + "。 <br>共有したい場合は、圧縮してtemplate@ccflow.orgに送信してください。";
        }
    }
}
