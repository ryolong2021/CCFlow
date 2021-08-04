using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// Boolen字段
    /// </summary>
    public class MapAttrBoolen : EntityMyPK
    {
        #region 文本字段参数属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// 绑定的枚举ID
        /// </summary>
        public string UIBindKey
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.UIBindKey);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIBindKey, value);
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int MyDataType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MyDataType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.MyDataType, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// Boolen字段
        /// </summary>
        public MapAttrBoolen()
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

                Map map = new Map("Sys_MapAttr", "Boolenフィールド");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 基本信息.

                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主キー", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "エンティティID", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "フィールド日本名", true, false, 0, 200, 20, true); //@李国文
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "フィールド名", true, true, 1, 200, 20);

                //数据类型.
                map.AddDDLSysEnum(MapAttrAttr.MyDataType, 4, "データのタイプ", true, false);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "見えるかどうか", true, true);

                map.AddTBString(MapAttrAttr.DefVal, "0", "デフォルト値（チェックされているかどうか？0 =いいえ、1 =はい）", true, false, 0, 200, 20);

                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "編集できるかどうか", true, true);
                map.AddTBStringDoc(MapAttrAttr.Tip, null, "アクティベーション提示", true, false); //@李国文
                #endregion 基本信息.

                #region 傻瓜表单。
                //单元格数量 2013-07-24 增加。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "セルの数", true, true, "ColSpanAttrDT",
                  "@0=クロス0セル@1=スパン1セル@2=スパン2セル@3=スパン3セル@4=スパン4セル");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "テキストセルの数", true, true, "ColSpanAttrString",
                    "@1=スパン1セル@2=スパン2セル@3=スパン3セル@4=スパン4セル");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "表示されたグループ", MapAttrString.SQLOfGroupAttr, true);

                map.AddTBInt(MapAttrAttr.UIWidth, 0, "幅（自由形式で有効）", true, false);
                map.AddTBInt(MapAttrAttr.Idx, 0, "シーケンス番号", true, false); //@李国文

                //CCS样式
                map.AddDDLSQL(MapAttrAttr.CSS, "0", "カスタムスタイル", MapAttrString.SQLOfCSSAttr, true);

                #endregion 傻瓜表单。


                RefMethod rm = new RefMethod();
                rm.Title = "イベントバインディング機能";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "JSの詳細設定";
                rm.ClassMethodName = this.ToString() + ".DoCheckboxs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.MyPK = this.MyPK;
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);
            }
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }
        #endregion

        #region 基本功能.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public  String DoCheckboxs() 
        {
		    return "../../Admin/FoolFormDesigner/MapExt/CheckBoxs.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
    #endregion
}
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrBoolens : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrBoolens()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrBoolen();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrBoolen> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrBoolen>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrBoolen> Tolist()
        {
            System.Collections.Generic.List<MapAttrBoolen> list = new System.Collections.Generic.List<MapAttrBoolen>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrBoolen)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
