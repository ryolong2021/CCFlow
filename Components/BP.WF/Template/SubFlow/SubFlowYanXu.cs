using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
  
    /// <summary>
    /// 延续子流程属性
    /// </summary>
    public class SubFlowYanXuAttr :SubFlowAttr
    {
    }
    /// <summary>
    /// 延续子流程.
    /// </summary>
    public class SubFlowYanXu : EntityMyPK
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
                return this.GetValStringByKey(SubFlowYanXuAttr.SubFlowNo);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.SubFlowNo, value);
            }
        }   
        /// <summary>
        /// 流程名称
        /// </summary>
        public string SubFlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.SubFlowName);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowYanXuAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.ExpType, (int)value);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.FK_Node);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.FK_Node, value);
            }
        }

        /// <summary>
        /// 运行类型
        /// </summary>
        public SubFlowModel HisSubFlowModel
        {
            get
            {
                return (SubFlowModel)this.GetValIntByKey(SubFlowAutoAttr.SubFlowModel);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 延续子流程
        /// </summary>
        public SubFlowYanXu() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "継続サブフロー");

                map.AddMyPK();

                map.AddTBString(SubFlowAttr.FK_Flow, null, "メインフロー番号", true, false, 0, 10, 100, true);
                 
                map.AddTBInt(SubFlowYanXuAttr.FK_Node, 0, "ノード", false, true);
                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowType, 2, "サブフロータイプ", true, false, SubFlowYanXuAttr.SubFlowType,
                "@0=サブフローを手動で開始@1=サブフローの開始をトリガー@2=サブフローを続行");

                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowModel, 0, "サブフローパターン", true, true, SubFlowYanXuAttr.SubFlowModel,
                "@0=下位レベルのサブフロー@1=同じレベルのサブフロー");


                map.AddTBString(SubFlowYanXuAttr.SubFlowNo, null, "サブフロー番号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowYanXuAttr.SubFlowName, null, "サブフロー名", true, true, 0, 200, 150, false);

                map.AddDDLSysEnum(FlowAttr.IsAutoSendSubFlowOver, 0, "親子フロー終了ルール", true, true,
               FlowAttr.IsAutoSendSubFlowOver, "@0=処理しない@1=親フローに次のステップを自動的に実行させる@2=親フローを終了する");


                map.AddDDLSysEnum(FlowAttr.IsAutoSendSLSubFlowOver, 0, "同じレベルのサブフローの終了ルール", true, true,
               FlowAttr.IsAutoSendSLSubFlowOver, "@0=処理しない@1=同じレベルのサブフローを自動的に次のステップを実行させる@2=同じレベルでサブフローを終了する");


                map.AddDDLSysEnum(SubFlowYanXuAttr.ExpType, 3, "表現式のタイプ", true, true, SubFlowYanXuAttr.ExpType,
                   "@3=SQLに従って計算@4=パラメータに従って計算");

                map.AddTBString(SubFlowYanXuAttr.CondExp, null, "条件表現式", true, false, 0, 500, 150, true);

                //@du.
                map.AddDDLSysEnum(SubFlowYanXuAttr.YBFlowReturnRole, 0, "返品方法", true, true, SubFlowYanXuAttr.YBFlowReturnRole,
                  "@0=返品できません@1=親フローの開始ノードに戻る@2=親フローの任意のノードに戻る@3=親フローの開始ノードに戻る@4=指定されたノードに戻る");

                // map.AddTBString(SubFlowYanXuAttr.ReturnToNode, null, "要退回的节点", true, false, 0, 200, 150, true);
                map.AddDDLSQL(SubFlowYanXuAttr.ReturnToNode, "0", "返されるノード",
                    "SELECT NodeID AS No, Name FROM WF_Node WHERE FK_Flow IN (SELECT FK_Flow FROM WF_Node WHERE NodeID=@FK_Node; )", true);

                map.AddTBInt(SubFlowYanXuAttr.Idx, 0, "表示順", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 设置主键
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_Node + "_" + this.SubFlowNo + "_2";
            return base.beforeInsert();
        }

        #region 移动.
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(SubFlowYanXuAttr.FK_Node, this.FK_Node, SubFlowYanXuAttr.SubFlowType, "2", SubFlowYanXuAttr.Idx);
            return "実行成功";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(SubFlowYanXuAttr.FK_Node, this.FK_Node, SubFlowYanXuAttr.SubFlowType, "2", SubFlowYanXuAttr.Idx);
            return "実行成功";
        }
        #endregion 移动.

    }
    /// <summary>
    /// 延续子流程集合
    /// </summary>
    public class SubFlowYanXus : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowYanXu();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 延续子流程集合
        /// </summary>
        public SubFlowYanXus()
        {
        }
        /// <summary>
        /// 延续子流程集合.
        /// </summary>
        /// <param name="fk_node"></param>
        public SubFlowYanXus(int fk_node)
        {
            this.Retrieve(SubFlowYanXuAttr.FK_Node, fk_node, 
                SubFlowYanXuAttr.SubFlowType, (int)SubFlowType.YanXuFlow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowYanXu> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowYanXu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowYanXu> Tolist()
        {
            System.Collections.Generic.List<SubFlowYanXu> list = new System.Collections.Generic.List<SubFlowYanXu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowYanXu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
