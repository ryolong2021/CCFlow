using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Port;
using BP.WF;
using BP.Sys;

namespace BP.WF.Template
{
	/// <summary>
	/// Frm属性
	/// </summary>
    public class FrmFieldAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 字段
        /// </summary>
        public const string KeyOfEn = "KeyOfEn";
        /// <summary>
        /// FK_Node
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// FK_MapData
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 是否必填
        /// </summary>
        public const string IsNotNull = "IsNotNull";
        /// <summary>
        /// 正则表达式
        /// </summary>
        public const string RegularExp = "RegularExp";
        /// <summary>
        /// 类型
        /// </summary>
        public const string EleType = "EleType";
        /// <summary>
        /// 是否写入流程表？
        /// </summary>
        public const string IsWriteToFlowTable = "IsWriteToFlowTable";
        /// <summary>
        /// 是否写入流程注册表
        /// </summary>
        public const string IsWriteToGenerWorkFlow = "IsWriteToGenerWorkFlow";
    }
	/// <summary>
	/// 表单字段方案
	/// </summary>
    public class FrmField : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 元素类型.
        /// </summary>
        public string EleType
        {
            get
            {
                return this.GetValStringByKey(FrmFieldAttr.EleType);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.EleType, value);
            }
        }
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegularExp
        {
            get
            {
                return this.GetValStringByKey(FrmFieldAttr.RegularExp);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.RegularExp, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FrmFieldAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.Name, value);
            }
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNotNull
        {
            get
            {
                return this.GetValBooleanByKey(FrmFieldAttr.IsNotNull);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.IsNotNull, value);
            }
        }
        /// <summary>
        /// 是否写入流程数据表
        /// </summary>
        public bool IsWriteToFlowTable
        {
            get
            {
                return this.GetValBooleanByKey(FrmFieldAttr.IsWriteToFlowTable);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.IsWriteToFlowTable, value);
            }
        }
        
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FrmFieldAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(FrmFieldAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmFieldAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 解决方案
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmFieldAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmFieldAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool UIVisible
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIVisible);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIVisible, value);
            }
        }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool UIIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIIsEnable);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIIsEnable, value);
            }
        }
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.DefVal);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefVal, value);
            }
        }
        /// <summary>
        /// 是否是数字签名?
        /// </summary>
        public bool IsSigan
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsSigan);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSigan, value);
            }
        }

        public override string InitMyPKVals()
        {
            this.MyPK =  this.FK_MapData + "_" + this.FK_Flow + "_" + this.FK_Node + "_" + this.KeyOfEn + "_" + this.EleType;
            return base.InitMyPKVals();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单字段方案
        /// </summary>
        public FrmField()
        {
        }
        /// <summary>
        /// 表单字段方案
        /// </summary>
        /// <param name="no"></param>
        public FrmField(string mypk)
            : base(mypk)
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

                Map map = new Map("Sys_FrmSln", "フォームフィールドスキーム");
                map.Java_SetCodeStruct("4");

                map.AddMyPK();

                //该表单对应的表单ID
                map.AddTBString(FrmFieldAttr.FK_Flow, null, "フロー番号", true, false, 0, 4, 4);
                map.AddTBInt(FrmFieldAttr.FK_Node, 0, "ノード", true, false);

                map.AddTBString(FrmFieldAttr.FK_MapData, null, "フォームID", true, false, 0, 100, 10);
                map.AddTBString(FrmFieldAttr.KeyOfEn, null, "フィールド", true, false, 0, 200, 20);
                map.AddTBString(FrmFieldAttr.Name, null, "フィールド名", true, false, 0, 500, 20);
                map.AddTBString(FrmFieldAttr.EleType, null, "種類", true, false, 0, 20, 20);

                //控制内容.
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "使えるかどうか", true, true);
                map.AddBoolean(MapAttrAttr.UIVisible, true, "見えますか", true, true);
                map.AddBoolean(MapAttrAttr.IsSigan, false, "署名するかどうか", true, true);

                // Add 2013-12-26.
                map.AddTBInt(FrmFieldAttr.IsNotNull, 0, "空っぽですか", true, false);
                map.AddTBString(FrmFieldAttr.RegularExp, null, "正規表現", true, false, 0, 500, 20);

                // 是否写入流程表? 2014-01-26，如果是，则首先写入该节点的数据表，然后copy到流程数据表里
                // 在节点发送时有ccflow自动写入，写入目的就是为了
                map.AddTBInt(FrmFieldAttr.IsWriteToFlowTable, 0, "フローテーブルに書き込むかどうか", true, false);

                map.AddTBInt(FrmFieldAttr.IsWriteToGenerWorkFlow, 0, "フローレジストリに書き込むかどうか", true, false);

                //map.AddDDLSysEnum(FrmFieldAttr.IsWriteToFlowTable, 0, "写入规则", true, true, FrmFieldAttr.IsWriteToFlowTable,
                  //  "@0=不写入@1=写入流程数据表@2=写入流程注册表@3=写入全部");


                map.AddBoolean(MapAttrAttr.IsSigan, false, "署名するかどうか", true, true);

                map.AddTBString(MapAttrAttr.DefVal, null, "デフォルト", true, false, 0, 200, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.EleType))
                this.EleType = FrmEleType.Field;

            if(this.EleType == FrmEleType.Field)
                this.MyPK = this.FK_MapData + "_"+ this.FK_Node + "_" + this.KeyOfEn;


            return base.beforeInsert();
        }
    }
	/// <summary>
    /// 表单字段方案s
	/// </summary>
    public class FrmFields : EntitiesMyPK
    {
        public FrmFields()
        {
        }
        /// <summary>
        /// 查询
        /// </summary>
        public FrmFields(string fk_mapdata, int nodeID)
        {
            this.Retrieve(FrmFieldAttr.FK_MapData, fk_mapdata, 
                FrmFieldAttr.FK_Node, nodeID,FrmFieldAttr.EleType,  FrmEleType.Field);
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmField();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmField> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmField> Tolist()
        {
            System.Collections.Generic.List<FrmField> list = new System.Collections.Generic.List<FrmField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
