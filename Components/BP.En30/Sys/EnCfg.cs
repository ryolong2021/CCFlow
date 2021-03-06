using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
using System.IO;

namespace BP.Sys
{
    /// <summary>
    /// UI的设置.Search. Card, Group信息.
    /// </summary>
    public class UIConfig
    {
        public Entity HisEn;
        public AtPara HisAP;
        public UIConfig()
        {
        }
        /// <summary>
        /// UI的设置.Search. Card, Group信息.
        /// </summary>
        /// <param name="enName"></param>
        public UIConfig(Entity en)
        {
            this.HisEn = en;
            EnCfg cfg = new EnCfg(en.ToString());
            string paraStr = cfg.UI;
            if (DataType.IsNullOrEmpty(paraStr) == true)
                paraStr = "@UIRowStyleGlo=0@IsEnableDouclickGlo=1@IsEnableRefFunc=1@IsEnableFocusField=1@IsEnableOpenICON=1@FocusField=''@WinCardH=600@@WinCardW=800@ShowColumns=";
            HisAP = new AtPara(paraStr);
        }
        /// <summary>
        /// 获取显示列数组，中间用,隔开
        /// </summary>
        public string[] ShowColumns
        {
            get
            {
                var colstr = this.HisAP.GetValStrByKey("ShowColumns");

                if(string.IsNullOrWhiteSpace(colstr))
                    return new string[0];

                return colstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #region 移动.
        /// <summary>
        /// 移动到方式.
        /// </summary>
        public MoveToShowWay MoveToShowWay
        {
            get
            {
                return (MoveToShowWay)this.HisAP.GetValIntByKey("MoveToShowWay");
            }
        }
        public EditerType EditerType
        {
            get
            {
                return (EditerType)this.HisAP.GetValIntByKey("EditerType");
            }
        }

        /// <summary>
        /// 移动到字段
        /// </summary>
        public string MoveTo
        {
            get
            {
                string s = this.HisAP.GetValStrByKey("MoveTo");
                return s;
            }
        }
        #endregion 移动.

        /// <summary>
        /// 风格类型
        /// </summary>
        public int UIRowStyleGlo
        {
            get
            {
                return this.HisAP.GetValIntByKey("UIRowStyleGlo");
            }
        }
        /// <summary>
        /// 是否启用双击打开？
        /// </summary>
        public bool IsEnableDouclickGlo
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableDouclickGlo");
            }
        }
        /// <summary>
        /// 是否显示相关功能?
        /// </summary>
        public bool IsEnableRefFunc
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableRefFunc");
            }
        }
        /// <summary>
        /// 是否启用焦点字段
        /// </summary>
        public bool IsEnableFocusField
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableFocusField");
            }
        }
        /// <summary>
        /// 是否打开ICON
        /// </summary>
        public bool IsEnableOpenICON
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableOpenICON");
            }
        }
        /// <summary>
        /// 焦点字段
        /// </summary>
        public string FocusField
        {
            get
            {
                string s = this.HisAP.GetValStrByKey("FocusField");
                if (DataType.IsNullOrEmpty(s))
                {
                    if (this.HisEn.EnMap.Attrs.Contains("Name"))
                        return "Name";
                    if (this.HisEn.EnMap.Attrs.Contains("Title"))
                        return "Title";
                }
                return s;
            }
        }
        public int WinCardW
        {
            get
            {
                return this.HisAP.GetValIntByKey("WinCardW");
            }
        }
        public int WinCardH
        {
            get
            {
                return this.HisAP.GetValIntByKey("WinCardH");
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            EnCfg cfg = new EnCfg(this.HisEn.ToString());
            cfg.UI = this.HisAP.GenerAtParaStrs();
            return cfg.Save();
        }

    }
    /// <summary>
    ///  配置信息
    /// </summary>
    public class EnCfgAttr : EntityNoAttr
    {
        /// <summary>
        /// 分组标签
        /// </summary>
        public const string GroupTitle = "GroupTitle";
        /// <summary>
        /// Url
        /// </summary>
        public const string UrlExt = "UrlExt";
        /// <summary>
        /// 附件路径
        /// </summary>
        public const string FJSavePath = "FJSavePath";
        /// <summary>
        /// 附件路径
        /// </summary>
        public const string FJWebPath = "FJWebPath";
        /// <summary>
        /// 数据分析方式
        /// </summary>
        public const string Datan = "Datan";
        /// <summary>
        /// Search,Group,设置.
        /// </summary>
        public const string UI = "UI";

        /// <summary>
        /// 字段颜色设置
        /// </summary>
        public const string ColorSet = "ColorSet";
        /// <summary>
        /// 字段求和求平均设置
        /// </summary>
        public const string FieldSet = "FieldSet";
    }
    /// <summary>
    /// EnCfgs
    /// </summary>
    public class EnCfg : EntityNo
    {
        #region UI设置.
        public string UI
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.UI);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.UI, value);
            }
        }
        #endregion UI设置.

        #region 基本属性
        /// <summary>
        /// 数据分析方式
        /// </summary>
        public string Datan
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.Datan);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.Datan, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string GroupTitle
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.GroupTitle);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.GroupTitle, value);
            }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string FJSavePath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJSavePath);
                if (str == "" || str == null || str == string.Empty)
                    return BP.Sys.SystemConfig.PathOfDataUser + this.No + Path.DirectorySeparatorChar;
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJSavePath, value);
            }
        }
        /// <summary>
        /// 附件存储位置.
        /// </summary>
        public string FJWebPath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJWebPath);
                if (str == "" || str == null)
                    str = Path.Combine(HttpContextHelper.RequestApplicationPath ,
                        "DataUser/" , this.No);
                str = str.Replace("\\", "/");
                if (!str.EndsWith("/"))
                    str += "/";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJWebPath, value);
            }
        }
        #endregion

        #region 参数属性.
        /// <summary>
        /// 批处理-设置页面大小
        /// </summary>
        public int PageSizeOfBatch
        {
            get
            {
                return this.GetParaInt("PageSizeOfBatch", 600);
            }
            set
            {
                this.SetPara("PageSizeOfBatch", value);
            }
        }
        /// <summary>
        /// 批处理-设置页面大小
        /// </summary>
        public int PageSizeOfSearch
        {
            get
            {
                return this.GetParaInt("PageSizeOfSearch", 15);
            }
            set
            {
                this.SetPara("PageSizeOfSearch", value);
            }
        }

        public string FieldSet
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.FieldSet);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FieldSet, value);
            }
        }
        #endregion 参数属性.

        #region 构造方法
        /// <summary>
        /// 系统实体
        /// </summary>
        public EnCfg()
        {

        }
        /// <summary>
        /// 系统实体
        /// </summary>
        /// <param name="no"></param>
        public EnCfg(string enName)
        {
            this.No = enName;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_EnCfg", "エンティティ配置");
                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);


                map.AddTBStringPK(EnCfgAttr.No, null, "エンティティ名", true, false, 1, 100, 60);
                map.AddTBString(EnCfgAttr.GroupTitle, null, "グループラベル", true, false, 0, 2000, 60);
                map.AddTBString(EnCfgAttr.UrlExt, null, "開くURL", true, false, 0, 500, 60);

                map.AddTBString(EnCfgAttr.FJSavePath, null, "保存パス", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.FJWebPath, null, "添付ファイルのWebパス", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.Datan, null, "フィールドデータ分析法", true, false, 0, 200, 60);
                map.AddTBString(EnCfgAttr.UI, null, "UI設置", true, false, 0, 2000, 60);

                //字段颜色设置
                map.AddTBString(EnCfgAttr.ColorSet, null, "色設定", true, false, 0, 500, 60);
                //对字段求总和平均
                map.AddTBString(EnCfgAttr.FieldSet, null, "フィールド設定", true, false, 0, 500, 60);


                map.AddTBAtParas(3000);  //参数属性.
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 实体集合
    /// </summary>
    public class EnCfgs : EntitiesNo
    {
        #region 构造
        /// <summary>
        /// 配置信息
        /// </summary>
        public EnCfgs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnCfg();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EnCfg> ToJavaList()
        {
            return (System.Collections.Generic.IList<EnCfg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EnCfg> Tolist()
        {
            System.Collections.Generic.List<EnCfg> list = new System.Collections.Generic.List<EnCfg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EnCfg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
