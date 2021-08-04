using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using BP.WF;
using BP.Port;

namespace BP.Frm
{
    /// <summary>
    ///  控制模型-属性
    /// </summary>
    public class CtrlModelAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 
        /// </summary>
        public const string CtrlObj = "CtrlObj";
        /// <summary>
        /// 所有的人
        /// </summary>
        public const string IsEnableAll = "IsEnableAll";
        public const string IsEnableStation = "IsEnableStation";
        public const string IsEnableDept = "IsEnableDept";
        public const string IsEnableUser = "IsEnableUser";

        public const string IDOfUsers = "IDOfUsers";
        public const string IDOfStations = "IDOfStations";
        public const string IDOfDepts = "IDOfDepts";
    }
    /// <summary>
    /// 控制模型
    /// </summary>
    public class CtrlModel : BP.En.EntityMyPK
    {
        #region 基本属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(CtrlModelAttr.FrmID);
            }
            set
            {
                SetValByKey(CtrlModelAttr.FrmID, value);
            }
        }

        /// <summary>
        /// 控制权限
        /// </summary>
        public string CtrlObj
        {
            get
            {
                return this.GetValStringByKey(CtrlModelAttr.CtrlObj);
            }
            set
            {
                SetValByKey(CtrlModelAttr.CtrlObj, value);
            }
        }

        public string IDOfUsers
        {
            get
            {
                return this.GetValStringByKey(CtrlModelAttr.IDOfUsers);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IDOfUsers, value);
            }
        }


        public string IDOfStations
        {
            get
            {
                return this.GetValStringByKey(CtrlModelAttr.IDOfStations);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IDOfStations, value);
            }
        }


        public string IDOfDepts
        {
            get
            {
                return this.GetValStringByKey(CtrlModelAttr.IDOfDepts);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IDOfDepts, value);
            }
        }

        public bool IsEnableAll
        {
            get
            {
                return this.GetValBooleanByKey(CtrlModelAttr.IsEnableAll);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IsEnableAll, value);
            }
        }

        public bool IsEnableStation
        {
            get
            {
                return this.GetValBooleanByKey(CtrlModelAttr.IsEnableStation);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IsEnableStation, value);
            }
        }

        public bool IsEnableDept
        {
            get
            {
                return this.GetValBooleanByKey(CtrlModelAttr.IsEnableDept);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IsEnableDept, value);
            }
        }

        public bool IsEnableUser
        {
            get
            {
                return this.GetValBooleanByKey(CtrlModelAttr.IsEnableUser);
            }
            set
            {
                SetValByKey(CtrlModelAttr.IsEnableUser, value);
            }
        }
        #endregion 基本属性.

        #region 字段属性.
        #endregion attrs

        #region 构造.

        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_CtrlModel", "制御モデルテーブル");

                #region 字段
                map.AddMyPK();  //增加一个自动增长的列.

               
                map.AddTBString(CtrlModelAttr.FrmID, null, "フォームID", true, false, 0, 300, 100);
                //BtnNew,BtnSave,BtnSubmit,BtnDelete,BtnSearch
                map.AddTBString(CtrlModelAttr.CtrlObj, null, "制御権限", true, false, 0, 20, 100);
                map.AddTBInt(CtrlModelAttr.IsEnableAll, 0, "誰でもできる", true, false);
                map.AddTBInt(CtrlModelAttr.IsEnableStation, 0, "部署別に計算する", true, false);
                map.AddTBInt(CtrlModelAttr.IsEnableDept, 0, "バインドされた部門に従って計算されます", true, false);
                map.AddTBInt(CtrlModelAttr.IsEnableUser, 0, "バインドされた要員に基づいて計算", true, false);
                map.AddTBString(CtrlModelAttr.IDOfUsers, null, "バインドされた個人ID", true, false, 0, 1000, 300);
                map.AddTBString(CtrlModelAttr.IDOfStations, null, "バインドされた部署ID", true, false, 0, 1000, 300);
                map.AddTBString(CtrlModelAttr.IDOfDepts, null, "バインドされた部門ID", true, false, 0, 1000, 300);
                #endregion 字段

                //权限设置绑定岗位. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.Frm.CtrlModelDtls(), new BP.Port.Emps(),
                    BP.Frm.CtrlModelDtlAttr.FrmID,
                    BP.Frm.CtrlModelDtlAttr.IDs, "バインドされた部署に基づく権限", "FK_StationType", EmpAttr.Name, EmpAttr.No);

                //权限设置绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.Frm.CtrlModelDtls(), new BP.Port.Emps(),
                   BP.Frm.CtrlModelDtlAttr.FrmID,
                   BP.Frm.CtrlModelDtlAttr.IDs, "バインドされた人員に基づく権限", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 控制模型
        /// </summary>
        public CtrlModel()
        {
        }
        /// <summary>
        /// 增加授权人
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.MyPK = this.FrmID + "_" + CtrlObj;
            return base.beforeInsert();
        }

        #endregion 构造.
    }
    /// <summary>
    /// 控制模型集合s
    /// </summary>
    public class CtrlModels : BP.En.EntitiesMyPK
    {
        #region 构造方法.
        /// <summary>
        /// 控制模型集合
        /// </summary>
        public CtrlModels()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new CtrlModel();
            }
        }
        #endregion 构造方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CtrlModel> ToJavaList()
        {
            return (System.Collections.Generic.IList<CtrlModel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CtrlModel> Tolist()
        {
            System.Collections.Generic.List<CtrlModel> list = new System.Collections.Generic.List<CtrlModel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CtrlModel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}