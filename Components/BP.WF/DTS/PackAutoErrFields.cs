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
    /// 修复非法字段名称
    /// </summary>
    public class PackAutoErrFormatFieldTable : Method
    {
        /// <summary>
        /// 修复非法字段名称
        /// </summary>
        public PackAutoErrFormatFieldTable()
        {
            this.Title = "不正なフィールド名、物理テーブル名を修正する";
            this.Help = "以前のバージョンでは、ユーザーが作成したフォームの物理テーブル名とフィールド名の有効性がチェックされていなかった場合、物理テーブルが自動的に作成および修復されたときにシステムがエラーを引き起こしていました。このパッチはグローバルフォームをバッチで修正できます。";
            // this.Warning = "您确定要执行吗？";
            // this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成的路径", true, false, 1, 1900, 200);
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
            string keys = "~!@#$%^&*()+{}|:<>?`=[];,./～！＠＃￥％……＆×（）——＋｛｝｜：“《》？｀－＝［］；＇，．／";
            char[] cc = keys.ToCharArray();
            foreach (char c in cc)
            {
                DBAccess.RunSQL("update sys_mapattr set keyofen=REPLACE(keyofen,'" + c + "' , '')");
            }

            BP.Sys.MapAttrs attrs = new Sys.MapAttrs();
            attrs.RetrieveAll();
            int idx = 0;
            string msg = "";
            foreach (BP.Sys.MapAttr item in attrs)
            {
                string f = item.KeyOfEn.Clone().ToString();
                try
                {
                    int i = int.Parse( item.KeyOfEn.Substring(0, 1) );
                    item.KeyOfEn = "F" + item.KeyOfEn;
                    try
                    {
                        MapAttr itemCopy = new MapAttr();
                        itemCopy.Copy(item);
                        itemCopy.Insert();
                        item.DirectDelete();
                    }
                    catch (Exception ex)
                    {
                        msg += "@" + ex.Message;
                    }
                }
                catch
                {
                    continue;
                }
                DBAccess.RunSQL("UPDATE sys_mapAttr set KeyOfEn='"+item.KeyOfEn+"', mypk=FK_MapData+'_'+keyofen where keyofen='"+item.KeyOfEn+"'");
                msg += "@第(" + idx + ")件エラーを正常に修復され、元の（"+f+"）から("+item.KeyOfEn+")に修正済みです。";
                idx++;
            }

            BP.DA.DBAccess.RunSQL("UPDATE Sys_MapAttr SET MyPK=FK_MapData+'_'+KeyOfEn WHERE MyPK!=FK_MapData+'_'+KeyOfEn");
            return "修正情報は以下の通りです:"+msg;
        }
    }
}
