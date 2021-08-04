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
    /// 数值字段
    /// </summary>
    public class MapAttrNum : EntityMyPK
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
        /// 数值字段
        /// </summary>
        public MapAttrNum()
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

                Map map = new Map("Sys_MapAttr", "数値フィールド");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主キー", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "エンティティID", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "フィールド日本名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "フィールド名", true, true, 1, 200, 20);

                map.AddDDLSysEnum(MapAttrAttr.MyDataType, 2, "データのタイプ", true, false);

                map.AddTBString(MapAttrAttr.DefVal, MapAttrAttr.DefaultVal, "デフォルト値/小数点以下の桁数", true, false, 0, 200, 20);
 
                map.AddDDLSysEnum(MapAttrAttr.DefValType,1,"デフォルト値の選択方法",true,true,"DefValType","@0=デフォルト値は空です @1=設定されたデフォルト値に従って設定されます",false);
                string help = "このフィールドのデフォルト値を設定します：\t\r";

                help += "\t\r 1.プラスチック製の場合は、プラスチック製の番号をデフォルト値として設定します。";
                help += "\t\r 2.floatおよびdecimalデータ型の場合、0.0000を設定すると、小数点以下4桁が保持され、1.0000であれば小数点以下4桁が保持され、デフォルト値は1になります。";
                map.SetHelperAlert("DefVal", help);

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "幅", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高さ", true, true);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "見えるかどうか", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "編集できるかどうか", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "必須項目かどうか", true, true);

                map.AddBoolean("ExtIsSum", false, "合計を表示するかどうか（スレーブテーブルで有効）", true, true);
                map.SetHelperAlert("ExtIsSum", "スレーブテーブルの場合は、スレーブテーブルの下部にスレーブテーブルの合計を表示する必要があります。");

                map.AddTBString(MapAttrAttr.Tip, null, "アクティベーション提示", true, false, 0, 400, 20, true);
                //CCS样式
                map.AddDDLSQL(MapAttrAttr.CSS, "0", "カスタムスタイル", MapAttrString.SQLOfCSSAttr, true);
                #endregion 基本信息.

                #region 傻瓜表单。
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
                rm.Title = "自動計算";
                rm.ClassMethodName = this.ToString() + ".DoAutoFull()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "テーブル列からの自動計算";
                rm.ClassMethodName = this.ToString() + ".DoAutoFullDtlField()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "正規表現";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpression()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "脚本验证";
                //rm.ClassMethodName = this.ToString() + ".DoInputCheck()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

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
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefVal
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.DefVal);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefVal, value);
            }
        }
        public int DefValType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.DefValType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefValType, value);
            }
        }
        protected override bool beforeUpdateInsertAction()
        {
            //如果没默认值.
            if (this.DefVal == "" && this.DefValType==0)
                this.DefVal =MapAttrAttr.DefaultVal;

            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            //是否显示合计
            attr.IsSum = this.GetValBooleanByKey("ExtIsSum");

            attr.Update();

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
        #endregion

        #region 方法执行.
        public string DoAutoFullDtlField()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDtlField.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn);
        }
        /// <summary>
        /// 自动计算
        /// </summary>
        /// <returns></returns>
        public string DoAutoFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFull.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn);
        }
        /// <summary>
        /// 设置开窗返回值
        /// </summary>
        /// <returns></returns>
        public string DoPopVal()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopVal.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
        }

        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpression()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpressionNum.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 文本框自动完成
        /// </summary>
        /// <returns></returns>
        public string DoTBFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/TBFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
        }
        
        /// <summary>
        /// 扩展控件
        /// </summary>
        /// <returns></returns>
        public string DoEditFExtContral()
        {
            return "../../Admin/FoolFormDesigner/EditFExtContral.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
        }
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrNums : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrNums()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrNum();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrNum> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrNum>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrNum> Tolist()
        {
            System.Collections.Generic.List<MapAttrNum> list = new System.Collections.Generic.List<MapAttrNum>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrNum)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
