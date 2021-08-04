using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 评分控件
    /// </summary>
    public class ExtScore : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// URL
        /// </summary>
        public string URL
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.Tag2);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Tag2, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.Name);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Name, value);
            }
        }

        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                if (BP.Web.WebUser.No == "admin")
                {

                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                }

                return uac;
            }
        }
        /// <summary>
        /// 评分控件
        /// </summary>
        public ExtScore()
        {
        }
        /// <summary>
        /// 评分控件
        /// </summary>
        /// <param name="mypk"></param>
        public ExtScore(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
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
                Map map = new Map("Sys_MapAttr", "得点管理");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 通用的属性.
                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "フォームID", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "フィールド", true, true, 1, 100, 20);
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "表示されたグループ", MapAttrString.SQLOfGroupAttr, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "編集できるかどうか", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "必須項目かどうか", true, true);
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "テキストセルの数", true, true, "ColSpanAttrString",
                    "@1=スパン1セル@2=スパン2セル@3=スパン3セル@4=スパン4セル");                
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                #endregion 通用的属性.


                #region 个性化属性.
                map.AddTBString(MapAttrAttr.Name, null, "採点事項", true, false, 0, 500, 20, true);
                map.AddTBInt(MapAttrAttr.Tag2, 5, "合計スコア", true, false);
                #endregion 个性化属性.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 评分控件s
    /// </summary>
    public class ExtScores : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 评分控件s
        /// </summary>
        public ExtScores()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ExtLink();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ExtLink> ToJavaList()
        {
            return (System.Collections.Generic.IList<ExtLink>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ExtLink> Tolist()
        {
            System.Collections.Generic.List<ExtLink> list = new System.Collections.Generic.List<ExtLink>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ExtLink)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
