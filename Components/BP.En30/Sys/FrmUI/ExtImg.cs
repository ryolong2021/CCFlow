using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 装饰图片
    /// </summary>
    public class ExtImg : EntityMyPK
    {
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
        /// 装饰图片
        /// </summary>
        public ExtImg()
        {
        }
        /// <summary>
        /// 装饰图片
        /// </summary>
        /// <param name="mypk"></param>
        public ExtImg(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }

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
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FrmImg", "デコレーション画像");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);          
                map.IndexField = MapAttrAttr.FK_MapData;



                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "フォームID", true, true, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "対応するフィールド", true, true, 0, 200, 20);
                map.AddTBString(FrmImgAttr.Name, null, "日本語の名前", true, true, 0, 500, 20);
                map.AddDDLSysEnum(FrmImgAttr.ImgSrcType, 0, "画像ソース", true, true, FrmImgAttr.ImgSrcType, "@0=Local @1=URL");

                map.AddTBString(FrmImgAttr.ImgPath, null, "画像パス", true, false, 0, 200, 20, true);

                string strs = "ローカル画像パス:";
                strs += " \n 1.＠baseBaseまたは＠ + field英語名を変数として使用して、ファイルパスを識別できます。";
                strs += " \n 2.注＠半角文字が必要です。";
                strs += " \n 3.例：＠basePath/DataUser/UserIcon/＠QingJiaRenID.png";

                map.SetHelperAlert(FrmImgAttr.ImgPath, strs);

                map.AddTBString(FrmImgAttr.ImgURL, null, "画像のURL", true, false, 0, 200, 20, true);

                map.AddTBString(FrmImgAttr.LinkURL, null, "URLに接続", true, false, 0, 200, 20, true);
                map.AddTBString(FrmImgAttr.LinkTarget, "_blank", "接続先", true, false, 0, 200, 20);

                //UIContralType.FrmImg
                //map.AddTBString(FrmImgAttr.Tag0, null, "参数", true, false, 0, 500, 20); 2
                //map.AddTBString(FrmImgAttr.EnPK, null, "英文名称", true, false, 0, 500, 20);
                //map.AddTBInt(FrmImgAttr.ImgAppType, 0, "应用类型", false, false);
                //map.AddTBString(FrmImgAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                map.AddTBInt(FrmImgAttr.ImgAppType, 0, "アプリケーションタイプ", false, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID,0, "表示されたグループ", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.ColSpan, 0, "セルの数", false, true);

                //跨单元格
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "テキストセルの数", true, true, "ColSpanAttrString",
                    "@1=スパン1セル@2=スパン2セル@3 =スパン3セル@4 =スパン4セル");
                //跨行
                map.AddDDLSysEnum(MapAttrAttr.RowSpan, 1, "行数", true, true, "RowSpanAttrString",
                   "@1=スパン1行@2 = 2行にわたる@3 = 3行にわたる");

                map.AddTBInt(MapAttrAttr.UIWidth, 0, "幅", true, false);
                map.AddTBInt(MapAttrAttr.UIHeight, 0, "高さ", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.FrmImg imgAth = new BP.Sys.FrmImg();
            imgAth.MyPK = this.MyPK;
            imgAth.RetrieveFromDBSources();
            imgAth.Update();

            //同步更新MapAttr 
            if (DataType.IsNullOrEmpty(this.KeyOfEn) == false)
            {
                MapAttrString attr = new MapAttrString(this.MyPK);
                attr.SetValByKey(MapAttrAttr.ColSpan, this.GetValStrByKey(MapAttrAttr.ColSpan));
                attr.SetValByKey(MapAttrAttr.TextColSpan, this.GetValStrByKey(MapAttrAttr.TextColSpan));
                attr.SetValByKey(MapAttrAttr.RowSpan, this.GetValStrByKey(MapAttrAttr.RowSpan));

                attr.SetValByKey(MapAttrAttr.Name, this.GetValStrByKey(FrmImgAttr.Name)); //名称.

                attr.SetValByKey(MapAttrAttr.X, this.GetValStrByKey(FrmImgAttr.X)); 
                attr.SetValByKey(MapAttrAttr.Y, this.GetValStrByKey(FrmImgAttr.Y)); 
                attr.Update();
            }

            base.afterInsertUpdateAction();
        }
        protected override void afterDelete()
        {
            //把相关的字段也要删除.
            MapAttrString attr = new MapAttrString();
            attr.MyPK = this.MyPK;
            attr.FK_MapData = this.FK_MapData;
            attr.Delete();

            base.afterDelete();
        }

        #endregion
    }
    /// <summary>
    /// 装饰图片s
    /// </summary>
    public class ExtImgs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 装饰图片s
        /// </summary>
        public ExtImgs()
        {
        }
        /// <summary>
        /// 装饰图片s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public ExtImgs(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmImg();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ExtImg> ToJavaList()
        {
            return (System.Collections.Generic.IList<ExtImg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ExtImg> Tolist()
        {
            System.Collections.Generic.List<ExtImg> list = new System.Collections.Generic.List<ExtImg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ExtImg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
