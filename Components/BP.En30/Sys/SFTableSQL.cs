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

namespace BP.Sys
{
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTableSQL : EntityNoName
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
        public SFTableSQL()
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
                map.AddTBString(SFTableAttr.Name, null, "テーブル日本語名", true, false, 0, 200, 20);

                map.AddDDLSysEnum(SFTableAttr.SrcType, 0, "データテーブルタイプ", true, false, SFTableAttr.SrcType,
                    "@0=ローカルクラス@1=作成テーブル@2=テーブルあはいはビュー@3=SQLクエリテーブル@4=WebServices@5=マイクロサービスHandler外部データソース@6=JavaScript外部データソース@7=ダイナミックJson");


                map.AddTBString(SFTableAttr.FK_Val, null, "デフォルトで作成されるフィールド名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "テーブルの説明", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "デフォルト", true, false, 0, 200, 20);


                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "データソース", new BP.Sys.SFDBSrcs(), true);

                map.AddTBString(SFTableAttr.ColumnValue, null, "表示値（数値列）", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "表示されるテキスト（名前列）", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ParentValue, null, "親値（親列）", true, false, 0, 200, 20);
                map.AddTBStringDoc(SFTableAttr.SelectStatement, null, "クェリ文", true, false);
                map.AddTBDateTime(SFTableAttr.RDT, null, "入会日", false, false);

                //查找.
                map.AddSearchAttr(SFTableAttr.FK_SFDBSrc);

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
            if (this.IsClass)
                return SystemConfig.CCFlowWebPath + "WF/Comm/Ens.htm?EnsName=" + this.No;
            else
                return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + this.No;
        }

        /// <summary>
        /// 是否是类
        /// </summary>
        public bool IsClass
        {
            get
            {
                if (this.No.Contains("."))
                    return true;
                else
                    return false;
            }
        }

        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (attrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in attrs)
                    err += " @ " + item.MyPK + " " + item.Name;
                throw new Exception("@次のエンティティフィールドが参照されます:" + err + "。テーブルを削除できません。");
            }
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFTableSQLs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFTableSQLs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTableSQL();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTableSQL> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTableSQL>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTableSQL> Tolist()
        {
            System.Collections.Generic.List<SFTableSQL> list = new System.Collections.Generic.List<SFTableSQL>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTableSQL)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
