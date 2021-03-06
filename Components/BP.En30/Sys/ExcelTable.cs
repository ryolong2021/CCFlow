using System;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// Excel数据表字段常量
    /// </summary>
    public class ExcelTableAttr : EntityNoNameAttr
    {
        /// <summary>
        /// Excel模板
        /// </summary>
        public const string FK_ExcelFile = "FK_ExcelFile";
        /// <summary>
        /// 是否明细表
        /// </summary>
        public const string IsDtl = "IsDtl";
        /// <summary>
        /// 数据表说明
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 同步到表
        /// </summary>
        public const string SyncToTable = "SyncToTable";
    }
    /// <summary>
    /// Excel数据表
    /// </summary>
    public class ExcelTable : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 获取或设置Excel模板
        /// </summary>
        public string FK_ExcelFile
        {
            get
            {
                return this.GetValStrByKey(ExcelTableAttr.FK_ExcelFile);
            }
            set
            {
                this.SetValByKey(ExcelTableAttr.FK_ExcelFile, value);
            }
        }

        /// <summary>
        /// 获取或设置是否明细表
        /// </summary>
        public bool IsDtl
        {
            get
            {
                return this.GetValBooleanByKey(ExcelTableAttr.IsDtl);
            }
            set
            {
                this.SetValByKey(ExcelTableAttr.IsDtl, value);
            }
        }

        /// <summary>
        /// 获取或设置数据表说明
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStrByKey(ExcelTableAttr.Note);
            }
            set
            {
                this.SetValByKey(ExcelTableAttr.Note, value);
            }
        }

        /// <summary>
        /// 获取或设置同步到表
        /// </summary>
        public string SyncToTable
        {
            get
            {
                return this.GetValStrByKey(ExcelTableAttr.SyncToTable);
            }
            set
            {
                this.SetValByKey(ExcelTableAttr.SyncToTable, value);
            }
        }

        #endregion 属性

        #region 构造方法
        public ExcelTable()
        {
        }
        #endregion 构造方法

        #region 权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        #endregion 权限控制

        #region EnMap
        /// <summary>
        /// Excel数据表Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_ExcelTable");
                map.EnDesc = "Excelデータテーブル";

                map.AddTBStringPK(ExcelTableAttr.No, null, "ナンバリング", true, true, 1, 36, 200);
                map.AddTBString(ExcelTableAttr.Name, null, "データテーブル名", true, false, 1, 50, 100);
                map.AddDDLEntities(ExcelTableAttr.FK_ExcelFile, null, "Excelテンプレート", new ExcelFiles(), true);
                map.AddBoolean(ExcelTableAttr.IsDtl, false, "明細表かどうか", true, false);
                map.AddTBStringDoc(ExcelTableAttr.Note, null, "データシートの説明", true, false, true);
                map.AddTBString(ExcelTableAttr.SyncToTable, null, "テーブルに同期", true, false, 1, 100, 100);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion EnMap

        #region 重写事件
        /// <summary>
        /// 记录添加前事件
        /// </summary>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }

        #endregion 重写事件
    }
    /// <summary>
    /// Excel数据表集合
    /// </summary>
    public class ExcelTables : EntitiesNoName
    {
        #region 属性
        /// <summary>
        /// 生成Excel数据表实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ExcelTable();
            }
        }
        #endregion 属性

        #region 构造方法
        public ExcelTables()
        {
        }

        public ExcelTables(string fk_excelfile)
        {
            this.Retrieve(ExcelTableAttr.FK_ExcelFile, fk_excelfile, ExcelTableAttr.Name);
        }
        #endregion 构造方法
    }
}
