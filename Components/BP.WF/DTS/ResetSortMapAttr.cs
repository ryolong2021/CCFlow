using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
using System.Data;
namespace BP.WF.DTS
{
    /// <summary>
    /// 根据坐标排序字段
    /// </summary>
    public class ResetSortMapAttr : Method
    {
        /// <summary>
        /// 根据坐标排序字段
        /// </summary>
        public ResetSortMapAttr()
        {
            this.Title = "座標に従ってMapAttrフィールドを並べ替える-モバイル用";
            this.Help = "MapAttrテーブルのIdxフィールドをリセットし、座標y、xで並べ替え";
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
            try
            {
                string sql = "select NO from Sys_MapData where No not in(select No from Sys_MapDtl) and No not like '%Rpt'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        MapAttrs attrs = new MapAttrs();
                        QueryObject qo = new QueryObject(attrs);
                        qo.AddWhere(MapAttrAttr.FK_MapData, row["NO"].ToString());
                        qo.addAnd();
                        qo.AddWhere(MapAttrAttr.UIVisible, true);
                        qo.addOrderBy(MapAttrAttr.Y, MapAttrAttr.X);
                        qo.DoQuery();
                        int rowIdx = 0;
                        foreach (MapAttr mapAttr in attrs)
                        {
                            mapAttr.Idx = rowIdx;
                            mapAttr.DirectUpdate();
                            rowIdx++;
                        }
                    }
                }
                return "正常に実行しました。";
            }
            catch (Exception ex)
            {
            }
            return "実行は失敗しました。";
        }
    }
}
