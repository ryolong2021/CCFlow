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
    /// 枚举字段
    /// </summary>
    public class MapAttrEnum : EntityMyPK
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
        /// 控件类型
        /// </summary>
        public UIContralType UIContralType
        {
            get
            {
                return (UIContralType)this.GetValIntByKey(MapAttrAttr.UIContralType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIContralType, (int)value);
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
        /// 枚举字段
        /// </summary>
        public MapAttrEnum()
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

                Map map = new Map("Sys_MapAttr", "列挙型フィールド");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主キー", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "エンティティID", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "フィールド日本名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "フィールド名", true, true, 1, 200, 20);

                string sql = "";
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.MySQL:
                        sql = "SELECT -1 AS No, '-なし（選択しない）-' as Name ";
                        break;
                    case DBType.Oracle:
                        sql = "SELECT -1 AS No, '-なし（選択しない）-' as Name FROM DUAL ";
                        break;
                    
                    case DBType.PostgreSQL:
                    default:
                        sql = "SELECT -1 AS No, '-なし（選択しない）-' as Name FROM Port_Emp WHERE 1=2 ";
                        break;
                }
                sql += " union ";
                sql += "SELECT  IntKey as No, Lab as Name FROM Sys_Enum WHERE EnumKey='@UIBindKey'";


                //默认值.
                map.AddDDLSQL(MapAttrAttr.DefVal, "0", "デフォルト値（選択されている）", sql, true);


                //map.AddTBString(MapAttrAttr.DefVal, "0", "默认值", true, true, 0, 3000, 20);

                map.AddDDLSysEnum(MapAttrAttr.UIContralType, 0, "制御タイプ", true, true, "EnumUIContralType",
                 "@1=ドロップダウンボックス@2=チェックボックス@3=ラジオボタン");

                map.AddDDLSysEnum("RBShowModel", 0, "ラジオボタンの表示方法", true, true, "RBShowModel",
            "@0=垂直 @3=水平");

                //map.AddDDLSysEnum(MapAttrAttr.LGType, 0, "逻辑类型", true, false, MapAttrAttr.LGType, 
                // "@0=普通@1=枚举@2=外键@3=打开系统页面");

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "幅", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高さ", true, true);

                map.AddTBString(MapAttrAttr.UIBindKey, null, "列挙ID", true, true, 0, 100, 20);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "見えるかどうか", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "編集できるかどうか", true, true);

                map.AddBoolean(MapAttrAttr.UIIsInput, false, "必須項目かどうか", true, true);

                map.AddBoolean("IsEnableJS", false, "JSの詳細設定を有効にするかどうか", false, true); //参数字段.
                //CCS样式
                map.AddDDLSQL(MapAttrAttr.CSS, "0", "カスタムスタイル", MapAttrString.SQLOfCSSAttr, true);
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
                map.AddDDLSQL(MapAttrAttr.GroupID,0, "表示されたグループ", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "シーケンス番号", true, false); //@李国文

                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "連動を設定する";
                rm.ClassMethodName = this.ToString() + ".DoActiveDDL()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "他のコントロールを埋める";
                rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "列挙値を編集";
                rm.ClassMethodName = this.ToString() + ".DoSysEnum()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "JSの詳細設定";
                rm.ClassMethodName = this.ToString() + ".DoRadioBtns()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高度な設定";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "イベントバインディング機能";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }


        protected override bool beforeUpdateInsertAction()
        {
            
            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            //是否启用高级js设置.
            attr.IsEnableJS = this.GetValBooleanByKey("IsEnableJS");

            //单选按钮的展现方式.
            attr.RBShowModel = this.GetValIntByKey("RBShowModel");

            if (this.UIContralType == UIContralType.DDL || this.UIContralType == UIContralType.RadioBtn)
                attr.MyDataType = DataType.AppInt;
            else
                attr.MyDataType = DataType.AppString;

            //执行保存.
            attr.Save();

            return base.beforeUpdateInsertAction();
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
        #endregion

        protected override void afterDelete()
        {
            //删除可能存在的数据.
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + this.FK_MapData + "'");
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

        #region 基本功能.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion

        #region 方法执行.
        /// <summary>
        /// 编辑枚举值
        /// </summary>
        /// <returns></returns>
        public string DoSysEnum()
        {
            return "../../Admin/CCFormDesigner/DialogCtr/EnumerationNew.htm?DoType=FrmEnumeration_SaveEnum&EnumKey=" + this.UIBindKey;
        }
        
        public string DoDDLFullCtrl2019()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl2019.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 设置自动填充
        /// </summary>
        /// <returns></returns>
        public string DoAutoFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDLL.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoRadioBtns()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RadioBtns.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoActiveDDL()
        {
            return "../../Admin/FoolFormDesigner/MapExt/ActiveDDL.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }

        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrEnums : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrEnums()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrEnum();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrEnum> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrEnum>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrEnum> Tolist()
        {
            System.Collections.Generic.List<MapAttrEnum> list = new System.Collections.Generic.List<MapAttrEnum>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrEnum)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
