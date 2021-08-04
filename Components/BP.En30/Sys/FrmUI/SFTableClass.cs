using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTableClass : EntityNoName
    {

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFTableClass()
        {
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_SFTable", "辞書テーブル");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddTBStringPK(SFTableAttr.No, null, "テーブル英語名", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "テーブル日本名", true, false, 0, 200, 20);

                map.AddDDLSysEnum(SFTableAttr.SrcType, 0, "データテーブルタイプ", true, true, SFTableAttr.SrcType,
                    "@0=ローカルクラス@1=Create table @2=Table or view @3= SQL query table @4=WebServices");

                map.AddDDLSysEnum(SFTableAttr.CodeStruct, 0, "辞書テーブルタイプ", true, false, SFTableAttr.CodeStruct);

                map.AddTBString(SFTableAttr.FK_Val, null, "デフォルトで作成されるフィールド名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "テーブルの説明", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "デフォルト", true, false, 0, 200, 20);

                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "データソース", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(SFTableAttr.SrcTable, null, "データソーステーブル", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnValue, null, "表示値（数値列）", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "表示されるテキスト（名前列）", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ParentValue, null, "親値（親列）", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.SelectStatement, null, "チェック文", false, false, 0, 1000, 600, true);

                map.AddTBDateTime(SFTableAttr.RDT, null, "入会日", false, false);

                RefMethod rm = new RefMethod();
                rm.Title = "データを表示する";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            return SystemConfig.CCFlowWebPath + "WF/Comm/Ens.htm?EnsName=" + this.No;
        }
        /// <summary>
        /// 执行删除.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            BP.Sys.SFTable sf = new Sys.SFTable(this.No);
            sf.Delete();
            return base.beforeDelete();
        }
        protected override bool beforeInsert()
        {
            //利用这个时间串进行排序.
            this.SetValByKey("RDT", DataType.CurrentDataTime);
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFTableClasss : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFTableClasss()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTableClass();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTableClass> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTableClass>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTableClass> Tolist()
        {
            System.Collections.Generic.List<SFTableClass> list = new System.Collections.Generic.List<SFTableClass>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTableClass)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
