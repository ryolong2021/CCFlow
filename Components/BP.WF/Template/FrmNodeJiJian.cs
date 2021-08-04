using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点表单
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FrmNodeJiJian : EntityMyPK
    {
        #region 属性.
        public string FK_Frm
        {
            get
            {
                return this.GetValStrByKey(FrmNodeAttr.FK_Frm);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Node);
            }
        }
        /// <summary>
        /// @李国文 
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
        }
        #endregion

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

        #endregion

        #region 构造方法
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodeJiJian() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="mypk"></param>
        public FrmNodeJiJian(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
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

                Map map = new Map("WF_FrmNode", "ノードフォーム");

                map.AddMyPK();

                map.AddDDLEntities(FrmNodeAttr.FK_Frm, null, "形", new MapDatas(), false);

                map.AddTBString(FrmNodeAttr.FK_Flow, null, "フロー番号", true, true, 0, 4, 20);
                map.AddTBInt(FrmNodeAttr.FK_Node, 0, "ノードID", true, true);

                map.AddBoolean(FrmNodeAttr.IsPrint, false, "印刷できます", true, true);
                map.AddBoolean(FrmNodeAttr.IsEnableLoadData, false, "イベントの読み込みと入力を有効にするかどうか", true, true);

                map.AddBoolean(FrmNodeAttr.IsCloseEtcFrm, false, "他のページを開いたときに閉じますか？", true, true, true);
                map.SetHelperAlert(FrmNodeAttr.IsCloseEtcFrm, "デフォルトでは閉じていません。フォームもタブラベルとして開いている場合、他のタブページを閉じますか？");

                //map.AddDDLSysEnum(FrmNodeAttr.WhoIsPK, 0, "谁是主键?", true, true);
                //map.SetHelperAlert(FrmNodeAttr.WhoIsPK, "用来控制谁是表单事例的主键的方案，对于父子流程如果子流程需要在看到父流程的表单，就需要设置ParentID是主键。");

                map.AddDDLSysEnum(FrmNodeAttr.FrmSln, 0, "管理計画", true, true, FrmNodeAttr.FrmSln,
                    "@0=デフォルト計画@1=読み取り専用計画@2=カスタム計画");
                map.SetHelperAlert(FrmNodeAttr.FrmSln, "フォームデータ要素の権限を制御するスキーム。カスタムスキームの場合は、各フォーム要素の権限を設定します");


                map.AddBoolean(FrmNodeAttr.IsEnableFWC, false, "監査コンポーネントは有効になっていますか？", true, true, true);
                map.SetHelperAlert(FrmNodeAttr.IsEnableFWC, "監査コンポーネントでフォームを有効にするかどうかを制御しますか？有効にすると、フォームに表示されます。監査コンポーネントを表示する前提は、ノードフォームの監査コンポーネントが有効になっていることであり、監査コンポーネントのステータスもノード監査コンポーネントのステータスに従って決定されます。");

                //map.AddDDLSysEnum( BP.WF.Template.FrmWorkCheckAttr.FWCSta, 0, "审核组件(是否启用审核组件？)", true, true);

                //显示的
                // map.AddTBInt(FrmNodeAttr.Idx, 0, "顺序号", true, false);
                // map.SetHelperAlert(FrmNodeAttr.Idx, "在表单树上显示的顺序,可以通过列表调整.");

                //add 2016.3.25.
                //   map.AddBoolean(FrmNodeAttr.Is1ToN, false, "是否1变N？(分流节点有效)", true, true, true);
                //   map.AddTBString(FrmNodeAttr.HuiZong, null, "汇总的数据表名", true, false, 0, 300, 20);
                //   map.SetHelperAlert(FrmNodeAttr.HuiZong, "子线程要汇总的数据表，对当前节点是子线程节点有效。");

                //模版文件，对于office表单有效.
                //   map.AddTBString(FrmNodeAttr.TempleteFile, null, "模版文件", true, false, 0, 500, 20);

                //是否显示
                map.AddTBString(FrmNodeAttr.GuanJianZiDuan, null, "キーフィールド", true, false, 0, 20, 20);

                #region 表单启用规则. @袁丽娜
                //   map.AddDDLSysEnum(FrmNodeAttr.FrmEnableRole, 0, "启用规则", false, false, FrmNodeAttr.FrmEnableRole,
                //    "@0=始终启用@1=有数据时启用@2=有参数时启用@3=按表单的字段表达式@4=按SQL表达式@5=不启用@6=按岗位@7=按部门");

                //  map.SetHelperAlert(FrmNodeAttr.FrmEnableRole, "用来控制该表单是否显示的规则.");
                //   map.AddTBStringDoc(FrmNodeAttr.FrmEnableExp, null, "启用的表达式", false, false, true);
                #endregion 表单启用规则.


                map.AddTBString(FrmNodeAttr.FrmNameShow, null, "フォーム表示名", true, false, 0, 100, 20);
                map.SetHelperAlert(FrmNodeAttr.FrmNameShow, "フォームツリーに表示される名前。デフォルトは空で、これはフォームの実際の名前と同じです。主に、フォームツリーに表示されるノードフォームの名前に使用されます。");


                RefMethod rm = new RefMethod();
                ////@袁丽娜
                //rm.Title = "启用规则";
                //rm.ClassMethodName = this.ToString() + ".DoEnableRole()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "フィールド権限";
                rm.ClassMethodName = this.ToString() + ".DoFields()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "テーブル権限から";
                rm.ClassMethodName = this.ToString() + ".DoDtls()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "添付ファイルの権限";
                rm.ClassMethodName = this.ToString() + ".DoAths()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "画像添付権限";
                rm.ClassMethodName = this.ToString() + ".DoImgAths()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "他のノードから権限設定をコピーする";
                rm.ClassMethodName = this.ToString() + ".DoCopyFromNode()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "改变表单类型";
                //rm.ClassMethodName = this.ToString() + ".DoChangeFrmType()";
                //rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "修改表单类型", true, true);
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "表单启用规则";
                //rm.ClassMethodName = this.ToString() + ".DoFrmEnableRole()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 表单元素权限.
        public string DoDtls()
        {
            return "../../Admin/Sln/Dtls.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }
        public string DoFields()
        {
            return "../../Admin/Sln/Fields.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }
        public string DoAths()
        {
            return "../../Admin/Sln/Aths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }

        public string DoImgAths()
        {
            return "../../Admin/Sln/ImgAths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }

        public string DoCopyFromNode()
        {
            return "../../Admin/Sln/Aths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&DoType=Field";
        }
        public string DoEnableRole()
        {
            return "../../Admin/AttrNode/BindFrmsNodeEnableRole.htm?MyPK=" + this.MyPK;
        }
        #endregion 表单元素权限.

    }
    /// <summary>
    /// 节点表单s
    /// </summary>
    public class FrmNodeJiJians : EntitiesMyPK
    {
        #region 构造方法..
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodeJiJians() { }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNodeJiJian();
            }
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmNodeJiJian> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmNodeJiJian>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmNodeJiJian> Tolist()
        {
            System.Collections.Generic.List<FrmNodeJiJian> list = new System.Collections.Generic.List<FrmNodeJiJian>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmNodeJiJian)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
