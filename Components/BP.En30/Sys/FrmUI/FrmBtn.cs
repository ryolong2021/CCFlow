using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 按钮
    /// </summary>
    public class FrmBtn : EntityMyPK
    {
        #region 属性
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmBtnAttr.FK_MapData);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtn()
        {
        }
        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="mypk"></param>
        public FrmBtn(string mypk)
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

                Map map = new Map("Sys_FrmBtn", "ボタン");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                map.AddMyPK();
                map.AddTBString(FrmBtnAttr.FK_MapData, null, "フォームID", true, false, 1, 100, 20);
                map.AddTBString(FrmBtnAttr.Text, null, "ラベル", true, false, 0, 3900, 20);

                map.AddTBInt(FrmBtnAttr.IsView, 0, "見えるかどうか", false, false);
                map.AddTBInt(FrmBtnAttr.IsEnable, 0, "有効するかどうか", false, false);

                map.AddTBInt(FrmBtnAttr.UAC, 0, "制御タイプ", false, false);
                map.AddTBString(FrmBtnAttr.UACContext, null, "制御内容", false, false, 0, 3900, 20);

                map.AddDDLSysEnum(FrmBtnAttr.EventType, 0, "イベントタイプ", true, true, FrmBtnAttr.EventType, "@0=無効にする@1= URLを実行する@2= CCFromRef.jsを実行する");
                map.AddTBString(FrmBtnAttr.EventContext, null, "イベント内容", true, false, 0, 3900, 20);

                map.AddTBString(FrmBtnAttr.MsgOK, null, "操作成功させる提示", true, false, 0, 500, 20);
                map.AddTBString(FrmBtnAttr.MsgErr, null, "操作失敗させる提示", true, false, 0, 500, 20);

                map.AddTBString(FrmBtnAttr.BtnID, null, "ボタンID", true, false, 0, 128, 20);

                //显示的分组.
                map.AddDDLSQL(FrmBtnAttr.GroupID,0, "グループに所属",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData'", true);
             
                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.FrmBtn frmBtn = new BP.Sys.FrmBtn();
            frmBtn.MyPK = this.MyPK;
            frmBtn.RetrieveFromDBSources();
            frmBtn.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }


        #endregion
    }
    /// <summary>
    /// 按钮s
    /// </summary>
    public class FrmBtns : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 按钮s
        /// </summary>
        public FrmBtns()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmBtn();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmBtn> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmBtn>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmBtn> Tolist()
        {
            System.Collections.Generic.List<FrmBtn> list = new System.Collections.Generic.List<FrmBtn>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmBtn)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
