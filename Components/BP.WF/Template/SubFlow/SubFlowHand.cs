using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 手工启动子流程属性
    /// </summary>
    public class SubFlowHandAttr : SubFlowAttr
    {


    }
    /// <summary>
    /// 手工启动子流程.
    /// </summary>
    public class SubFlowHand : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
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
        /// 主流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.FK_Flow);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.FK_Flow, value);
            }
        }   
        /// <summary>
        /// 流程编号
        /// </summary>
        public string SubFlowNo
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.SubFlowNo);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.SubFlowNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string SubFlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.SubFlowName);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 仅仅可以启动一次?
        /// </summary>
        public bool StartOnceOnly
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowYanXuAttr.StartOnceOnly);
            }
        }

        /// <summary>
        /// 该流程启动的子流程运行结束后才可以再次启动
        /// </summary>
        public bool CompleteReStart
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowAutoAttr.CompleteReStart);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowHandAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.ExpType, (int)value);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.FK_Node);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 指定的流程结束后,才能启动该子流程(请在文本框配置子流程).
        /// </summary>
        public bool IsEnableSpecFlowOver
        {
            get
            {
                var val = this.GetValBooleanByKey(SubFlowAutoAttr.IsEnableSpecFlowOver);
                if (val == false)
                    return false;

                if (this.SpecFlowOver.Length > 2)
                    return true;
                return false;
            }
        }
        public string SpecFlowOver
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.SpecFlowOver);
            }
        }
        public string SpecFlowStart
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.SpecFlowStart);
            }
        }
        /// <summary>
        /// 自动发起的子流程发送方式
        /// </summary>
        public int SendModel
        {
            get
            {
                return this.GetValIntByKey(SubFlowAutoAttr.SendModel);
            }
        }
        /// <summary>
        /// 指定的流程启动后,才能启动该子流程(请在文本框配置子流程).
        /// </summary>
        public bool IsEnableSpecFlowStart
        {
            get
            {
                var val = this.GetValBooleanByKey(SubFlowAutoAttr.IsEnableSpecFlowStart);
                if (val == false)
                    return false;

                if (this.SpecFlowStart.Length > 2)
                    return true;
                return false;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 手工启动子流程
        /// </summary>
        public SubFlowHand() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "サブフローを手動で開始する");

                map.AddMyPK();

               map.AddTBString(SubFlowAttr.FK_Flow, null, "メインフロー番号", true, true, 0, 10, 100);

                map.AddTBInt(SubFlowHandAttr.FK_Node, 0, "ノード", false, true);
                map.AddDDLSysEnum(SubFlowHandAttr.SubFlowType, 0, "サブフロータイプ", true, false, SubFlowHandAttr.SubFlowType,
                "@0=サブフローを手動で開始@1=サブフローの開始をトリガー@2=サブフローを続行");

                map.AddTBString(SubFlowYanXuAttr.SubFlowNo, null, "サブフロー番号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowYanXuAttr.SubFlowName, null, "サブフロー名", true, true, 0, 200, 150, false);

                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowModel, 0, "サブフローパターン", true, true, SubFlowYanXuAttr.SubFlowModel,
                "@0=下位レベルのサブフロー@1=同じレベルのサブフロー");

                map.AddDDLSysEnum(FlowAttr.IsAutoSendSubFlowOver, 0, "親子フロー終了ルール", true, true,
                 FlowAttr.IsAutoSendSubFlowOver, "@0=処理しない@1=親フローに次のステップを自動的に実行させる@2=親フローを終了する");


                map.AddDDLSysEnum(FlowAttr.IsAutoSendSLSubFlowOver, 0, "同じレベルのサブフローの終了ルール", true, true,
               FlowAttr.IsAutoSendSLSubFlowOver, "@0=処理しない@1=同じレベルのサブフローを自動的に次のステップを実行させる@2=同じレベルでサブフローを終了する");

                map.AddBoolean(SubFlowHandAttr.StartOnceOnly, false, "1回だけ呼び出すことができます（繰り返し呼び出すことはできません）。",
                    true, true, true);

                map.AddBoolean(SubFlowHandAttr.CompleteReStart, false, "実行の終了後にサブフローを再開できます。",
                    true, true, true);
                //启动限制规则.
                map.AddBoolean(SubFlowHandAttr.IsEnableSpecFlowStart, false, "指定されたフローが開始された後、サブフローを開始できます（テキストボックスでサブフローを構成してください）。",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowStart, null, "サブフロー番号", true, false, 0, 200, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SpecFlowStart, "指定したフローが開始された後、サブフローを開始できます。複数のサブフローはカンマで区切られます。 001、002");
                map.AddTBString(SubFlowHandAttr.SpecFlowStartNote, null, "備考", true, false, 0, 500, 150, true);

                //启动限制规则.
                map.AddBoolean(SubFlowHandAttr.IsEnableSpecFlowOver, false, "指定したフローが終了すると、サブフローを開始できます（テキストボックスでサブフローを構成してください）。",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowOver, null, "サブフロー番号", true, false, 0, 200, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SpecFlowOver, "指定したフローが終了した後にサブフローを開始できます。複数のサブフローはカンマで区切られます。 001、002");
                map.AddTBString(SubFlowHandAttr.SpecFlowOverNote, null, "備考", true, false, 0, 500, 150, true);

                map.AddTBInt(SubFlowHandAttr.Idx, 0, "表示順", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_Node + "_" + this.SubFlowNo + "_0";
            return base.beforeInsert();
        }
        #region 移动.
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(SubFlowAutoAttr.FK_Node, this.FK_Node.ToString(), SubFlowAutoAttr.SubFlowType, "0", SubFlowAutoAttr.Idx);
            return "実行成功";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(SubFlowAutoAttr.FK_Node, this.FK_Node.ToString(), SubFlowAutoAttr.SubFlowType, "0", SubFlowAutoAttr.Idx);
            return "実行成功";
        }
        #endregion 移动.
    }
    
    /// <summary>
    /// 手工启动子流程集合
    /// </summary>
    public class SubFlowHands : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowHand();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 手工启动子流程集合
        /// </summary>
        public SubFlowHands()
        {
        }
        /// <summary>
        /// 手工启动子流程集合
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public SubFlowHands(int fk_node)
        {
            this.Retrieve(SubFlowYanXuAttr.FK_Node, fk_node,
                SubFlowYanXuAttr.SubFlowType, (int)SubFlowType.HandSubFlow, SubFlowYanXuAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowHand> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowHand>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowHand> Tolist()
        {
            System.Collections.Generic.List<SubFlowHand> list = new System.Collections.Generic.List<SubFlowHand>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowHand)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
