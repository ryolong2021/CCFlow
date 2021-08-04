using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 挂起 属性
	/// </summary>
    public class HungUpAttr:EntityMyPKAttr
    {
        #region 基本属性
        public const string Title = "Title";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 执行人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 通知给
        /// </summary>
        public const string NoticeTo = "NoticeTo";
        /// <summary>
        /// 挂起原因
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 挂起时间
        /// </summary>
        public const string DTOfHungUp = "DTOfHungUp";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 接受人
        /// </summary>
        public const string Accepter = "Accepter";
        /// <summary>
        /// 挂起方式.
        /// </summary>
        public const string HungUpWay = "HungUpWay";
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public const string DTOfUnHungUp = "DTOfUnHungUp";
        /// <summary>
        /// 计划解除挂起时间
        /// </summary>
        public const string DTOfUnHungUpPlan = "DTOfUnHungUpPlan";
        #endregion
    }
	/// <summary>
	/// 挂起
	/// </summary>
    public class HungUp:EntityMyPK
    {
        #region 属性
        public HungUpWay HungUpWay
        {
            get
            {
                return (HungUpWay)this.GetValIntByKey(HungUpAttr.HungUpWay);
            }
            set
            {
                this.SetValByKey(HungUpAttr.HungUpWay, (int)value);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(HungUpAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(HungUpAttr.FK_Node, value);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(HungUpAttr.WorkID);
            }
            set
            {
                this.SetValByKey(HungUpAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 挂起标题
        /// </summary>
        public string Title
        {
            get
            {
                string s= this.GetValStringByKey(HungUpAttr.Title);
                if (DataType.IsNullOrEmpty(s))
                    s = "@Recからの一時停止メッセージ。";
                return s;
            }
            set
            {
                this.SetValByKey(HungUpAttr.Title, value);
            }
        }
        /// <summary>
        /// 挂起原因
        /// </summary>
        public string Note
        {
            get
            {
               return this.GetValStringByKey(HungUpAttr.Note);
            }
            set
            {
                this.SetValByKey(HungUpAttr.Note, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.Rec);
            }
            set
            {
                this.SetValByKey(HungUpAttr.Rec, value);
            }
        }
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public string DTOfUnHungUp
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.DTOfUnHungUp);
            }
            set
            {
                this.SetValByKey(HungUpAttr.DTOfUnHungUp, value);
            }
        }
        /// <summary>
        /// 预计解除挂起时间
        /// </summary>
        public string DTOfUnHungUpPlan
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.DTOfUnHungUpPlan);
            }
            set
            {
                this.SetValByKey(HungUpAttr.DTOfUnHungUpPlan, value);
            }
        }
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public string DTOfHungUp
        {
            get
            {
                return this.GetValStringByKey(HungUpAttr.DTOfHungUp);
            }
            set
            {
                this.SetValByKey(HungUpAttr.DTOfHungUp, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 挂起
        /// </summary>
        public HungUp()
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_HungUp", "ハング");
                map.Java_SetEnType(EnType.Admin);
                map.IndexField = HungUpAttr.WorkID;


                map.AddMyPK();
                map.AddTBInt(HungUpAttr.FK_Node, 0, "ノードID", true, true);
                map.AddTBInt(HungUpAttr.WorkID, 0, "WorkID", true, true);
                map.AddDDLSysEnum(HungUpAttr.HungUpWay, 0, "サスペンドモード", true, true, HungUpAttr.HungUpWay, 
                    "@0=無制限の停止@1=指定した時間に停止を解除して自分に通知する@2=指定した時間に停止を解除して全員に通知する");

                map.AddTBStringDoc(HungUpAttr.Note, null, "ぶら下がり理由（タイトルとコンテンツのサポート変数）", true, false, true);

                map.AddTBString(HungUpAttr.Rec, null, "ハンガー", true, false, 0, 50, 10, true);

                map.AddTBDateTime(HungUpAttr.DTOfHungUp, null, "ハングタイム", true, false);
                map.AddTBDateTime(HungUpAttr.DTOfUnHungUp, null, "実際の停止解除時間", true, false);
                map.AddTBDateTime(HungUpAttr.DTOfUnHungUpPlan, null, "停止解除までの推定時間", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 执行释放挂起
        /// </summary>
        public void DoRelease()
        {
        }
        #endregion
    }
	/// <summary>
	/// 挂起
	/// </summary>
	public class HungUps: EntitiesMyPK
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new HungUp();
			}
		}
		/// <summary>
        /// 挂起
		/// </summary>
		public HungUps(){} 		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<HungUp> ToJavaList()
        {
            return (System.Collections.Generic.IList<HungUp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<HungUp> Tolist()
        {
            System.Collections.Generic.List<HungUp> list = new System.Collections.Generic.List<HungUp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((HungUp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
