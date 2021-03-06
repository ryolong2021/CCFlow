using System;
using System.Collections;
using BP.DA;
using BP.En;
using System.IO;

namespace BP.Sys
{
    /// <summary>
    /// 文件保存方式
    /// </summary>
    public enum AthSaveWay
    {
        /// <summary>
        /// IIS服务器
        /// </summary>
        IISServer,
        /// <summary>
        /// 保存到数据库
        /// </summary>
        DB,
        /// <summary>
        /// ftp
        /// </summary>
        FTPServer
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum AthRunModel
    {
        /// <summary>
        /// 记录模式
        /// </summary>
        RecordModel,
        /// <summary>
        /// 固定模式
        /// </summary>
        FixModel
    }
    /// <summary>
    /// 上传校验,方式.
    /// </summary>
    public enum UploadFileNumCheck
    {
        /// <summary>
        /// 不校验
        /// </summary>
        None,
        /// <summary>
        /// 不能为空
        /// </summary>
        NotEmpty,
        /// <summary>
        /// 每个类别不能为空.
        /// </summary>
        EverySortNoteEmpty
    }
    public enum AthCtrlWay
    {
        /// <summary>
        /// 表单主键
        /// </summary>
        PK,
        /// <summary>
        /// FID
        /// </summary>
        FID,
        /// <summary>
        /// 父流程ID
        /// </summary>
        PWorkID,
        /// <summary>
        /// 仅仅查看自己的
        /// </summary>
        MySelfOnly,
        /// <summary>
        /// 工作ID,对流程有效.
        /// </summary>
        WorkID,
        /// <summary>
        /// 爷孙流程
        /// </summary>
        PPWorkID
    }
    /// <summary>
    /// 附件上传类型
    /// </summary>
    public enum AttachmentUploadType
    {
        /// <summary>
        /// 单个的
        /// </summary>
        Single,
        /// <summary>
        /// 多个的
        /// </summary>
        Multi,
        /// <summary>
        /// 指定的
        /// </summary>
        Specifically
    }
    /// <summary>
    /// 附件上传方式
    /// </summary>
    public enum AthUploadWay
    {
        /// <summary>
        /// 继承模式
        /// </summary>
        Inherit,
        /// <summary>
        /// 协作模式
        /// </summary>
        Interwork
    }
    /// <summary>
    /// 文件展现方式
    /// </summary>
    public enum FileShowWay
    {
        /// <summary>
        /// 表格
        /// </summary>
        Table,
        /// <summary>
        /// 图片
        /// </summary>
        Pict,
        /// <summary>
        /// 自由模式
        /// </summary>
        Free
    }

    /// <summary>
    /// 附件删除规则
    /// </summary>
    public enum AthDeleteWay
    {
        /// <summary>
        /// 不删除 0
        /// </summary>
        None,
        /// <summary>
        /// 删除所有 1
        /// </summary>
        DelAll,
        /// <summary>
        /// 只删除自己上传 2
        /// </summary>
        DelSelf
    }

    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachmentAttr : EntityMyPKAttr
    {
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 运行模式
        /// </summary>
        public const string AthRunModel = "AthRunModel";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// 高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 要求上传的格式
        /// </summary>
        public const string Exts = "Exts";
        /// <summary>
        /// 附件编号
        /// </summary>
        public const string NoOfObj = "NoOfObj";
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public const string IsUpload = "IsUpload";
        /// <summary>
        /// 是否是合流汇总
        /// </summary>
        public const string IsHeLiuHuiZong = "IsHeLiuHuiZong";
        /// <summary>
        /// 是否汇总到合流节点上去？
        /// </summary>
        public const string IsToHeLiuHZ = "IsToHeLiuHZ";
        /// <summary>
        /// 是否增加
        /// </summary>
        public const string IsNote = "IsNote";
        /// <summary>
        /// 是否启用扩展列
        /// </summary>
        public const string IsExpCol = "IsExpCol";
        /// <summary>
        /// 是否显示标题列
        /// </summary>
        public const string IsShowTitle = "IsShowTitle";
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public const string IsDownload = "IsDownload";
        /// <summary>
        /// 是否可以排序
        /// </summary>
        public const string IsOrder = "IsOrder";
        /// <summary>
        /// 数据存储方式
        /// </summary>
        public const string AthSaveWay = "AthSaveWay";
        /// <summary>
        /// 保存到
        /// </summary>
        public const string SaveTo = "SaveTo";
        /// <summary>
        /// 是否要转换成html，方便在线浏览.
        /// </summary>
        public const string IsTurn2Html = "IsTurn2Html";
        /// <summary>
        /// 类别
        /// </summary>
        public const string Sort = "Sort";
        /// <summary>
        /// 上传类型
        /// </summary>
        public const string UploadType = "UploadType";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// RowIdx
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// <summary>
        /// 自动控制大小
        /// </summary>
        public const string IsAutoSize = "IsAutoSize";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 数据控制方式(对父子流程有效果)
        /// </summary>
        public const string CtrlWay = "CtrlWay";
        /// <summary>
        /// 上传方式(对父子流程有效果)
        /// </summary>
        public const string AthUploadWay = "AthUploadWay";
        /// <summary>
        /// 文件展现方式
        /// </summary>
        public const string FileShowWay = "FileShowWay";
        /// <summary>
        /// 上传方式
        /// 0，批量上传。
        /// 1，单个上传。
        /// </summary>
        public const string UploadCtrl = "UploadCtrl";
        /// <summary>
        /// 上传校验
        /// 0=不校验.
        /// 1=不能为空.
        /// 2=每个类别下不能为空.
        /// </summary>
        public const string UploadFileNumCheck = "UploadFileNumCheck";
        /// <summary>
        /// 上传最小数量
        /// </summary>
        public const string NumOfUpload = "NumOfUpload";
        /// <summary>
        /// 上传最大数量
        /// </summary>
        public const string TopNumOfUpload = "TopNumOfUpload";
             /// <summary>
        /// 附件最大限制
        /// </summary>
        public const string FileMaxSize = "FileMaxSize";
        /// <summary>
        /// 是否可见？
        /// </summary>
        public const string IsVisable = "IsVisable";

       /// <summary>
       /// 附件类型 0 普通附件 1 图片附件
       /// </summary>
        public const string FileType = "FileType";

        /// <summary>
        /// 附件删除方式
        /// </summary>
        public const string DeleteWay = "DeleteWay";

        #region 数据引用.
        /// <summary>
        /// 数据引用
        /// </summary>
        public const string DataRefNoOfObj = "DataRefNoOfObj";
        /// <summary>
        /// 阅读规则
        /// </summary>
        public const string ReadRole = "ReadRole";
        #endregion 数据引用.


        #region weboffice属性。
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public const string IsRowLock = "IsRowLock";
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public const string IsWoEnableWF = "IsWoEnableWF";
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public const string IsWoEnableSave = "IsWoEnableSave";
        /// <summary>
        /// 是否只读
        /// </summary>
        public const string IsWoEnableReadonly = "IsWoEnableReadonly";
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public const string IsWoEnableRevise = "IsWoEnableRevise";
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public const string IsWoEnableViewKeepMark = "IsWoEnableViewKeepMark";
        /// <summary>
        /// 是否打印
        /// </summary>
        public const string IsWoEnablePrint = "IsWoEnablePrint";
        /// <summary>
        /// 是否启用签章
        /// </summary>
        public const string IsWoEnableSeal = "IsWoEnableSeal";
        /// <summary>
        /// 是否启用套红
        /// </summary>
        public const string IsWoEnableOver = "IsWoEnableOver";
        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public const string IsWoEnableTemplete = "IsWoEnableTemplete";
        /// <summary>
        /// 是否自动写入审核信息
        /// </summary>
        public const string IsWoEnableCheck = "IsWoEnableCheck";
        /// <summary>
        /// 是否插入流程
        /// </summary>
        public const string IsWoEnableInsertFlow = "IsWoEnableInsertFlow";
        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public const string IsWoEnableInsertFengXian = "IsWoEnableInsertFengXian";
        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public const string IsWoEnableMarks = "IsWoEnableMarks";
        /// <summary>
        /// 是否启用下载
        /// </summary>
        public const string IsWoEnableDown = "IsWoEnableDown";
        #endregion weboffice属性。

        #region 快捷键.
        /// <summary>
        /// 是否启用快捷键
        /// </summary>
        public const string FastKeyIsEnable = "FastKeyIsEnable";
        /// <summary>
        /// 快捷键生成规则
        /// </summary>
        public const string FastKeyGenerRole = "FastKeyGenerRole";
        #endregion
     
    }
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachment : EntityMyPK
    {
        #region 参数属性.
        /// <summary>
        /// 是否可见？
        /// </summary>
        public bool IsVisable
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsVisable, true);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsVisable, value);
            }
        }
        /// <summary>
        /// 使用上传附件的 - 控件类型
        /// 0=批量.
        /// 1=单个。
        /// </summary>
        public int UploadCtrl
        {
            get
            {
                return this.GetParaInt(FrmAttachmentAttr.UploadCtrl);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.UploadCtrl, value);
            }
        }

        /// <summary>
        /// 最低上传数量
        /// </summary>
        public int NumOfUpload
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.NumOfUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.NumOfUpload, value);
            }
        }
        /// <summary>
        /// 最大上传数量
        /// </summary>
        public int TopNumOfUpload
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.TopNumOfUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.TopNumOfUpload, value);
            }
        }
        /// <summary>
        /// 附件最大限制
        /// </summary>
        public int FileMaxSize
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.FileMaxSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FileMaxSize, value);
            }
        }
        /// <summary>
        /// 上传校验
        /// 0=不校验.
        /// 1=不能为空.
        /// 2=每个类别下不能为空.
        /// </summary>
        public UploadFileNumCheck UploadFileNumCheck
        {
            get
            {
                return (UploadFileNumCheck)this.GetValIntByKey(FrmAttachmentAttr.UploadFileNumCheck);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.UploadFileNumCheck, (int)value);
            }
        }
        /// <summary>
        /// 保存方式
        /// 0 =文件方式保存。
        /// 1 = 保存到数据库.
        /// 2 = ftp服务器.
        /// </summary>
        public AthSaveWay AthSaveWay
        {
            get
            {
                return (AthSaveWay)this.GetValIntByKey(FrmAttachmentAttr.AthSaveWay);
            }
        }
        #endregion 参数属性.

        #region 属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 运行模式？
        /// </summary>
        public AthRunModel AthRunModel
        {
            get
            {
                return (AthRunModel)this.GetValIntByKey(FrmAttachmentAttr.AthRunModel);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.AthRunModel, (int)value);
            }
        }
        /// <summary>
        /// 上传类型（单个的，多个，指定的）
        /// </summary>
        public AttachmentUploadType UploadType
        {
            get
            {
                return (AttachmentUploadType)this.GetValIntByKey(FrmAttachmentAttr.UploadType);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.UploadType, (int)value);
            }
        }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string UploadTypeT
        {
            get
            {
                if (this.UploadType == AttachmentUploadType.Multi)
                    return "複数の添付ファイル";
                if (this.UploadType == AttachmentUploadType.Single)
                    return "シングルの添付ファイル";
                if (this.UploadType == AttachmentUploadType.Specifically)
                    return "指定";
                return "XXXXX";
            }
        }
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public bool IsUpload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsUpload, value);
            }
        }
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public bool IsDownload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsDownload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsDownload, value);
            }
        }

        /// <summary>
        /// 附件删除方式
        /// </summary>
        public AthDeleteWay HisDeleteWay
        {
            get
            {
                return (AthDeleteWay)this.GetValIntByKey(FrmAttachmentAttr.DeleteWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.DeleteWay, (int)value);
            }
        }

        /// <summary>
        /// 是否可以排序?
        /// </summary>
        public bool IsOrder
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsOrder);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsOrder, value);
            }
        }
        /// <summary>
        /// 自动控制大小
        /// </summary>
        public bool IsAutoSize
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsAutoSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsAutoSize, value);
            }
        }
        /// <summary>
        /// IsShowTitle
        /// </summary>
        public bool IsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 是否是节点表单.
        /// </summary>
        public bool IsNodeSheet
        {
            get
            {
                if (this.FK_MapData.StartsWith("ND") == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 备注列
        /// </summary>
        public bool IsNote
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsNote);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsNote, value);
            }
        }

        /// <summary>
        /// 是否启用扩张列
        /// </summary>
        public bool IsExpCol
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsExpCol);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsExpCol, value);
            }
        }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name
        {
            get
            {
                string str = this.GetValStringByKey(FrmAttachmentAttr.Name);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "名前なし";
                return str;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Name, value);
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Sort, value);
            }
        }
        /// <summary>
        /// 要求的格式
        /// </summary>
        public string Exts
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Exts);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Exts, value);
            }
        }
        /// <summary>
        /// 保存到
        /// </summary>
        public string SaveTo
        {
            get
            {
                if (this.AthSaveWay == Sys.AthSaveWay.IISServer)
                {
                    string s = this.GetValStringByKey(FrmAttachmentAttr.SaveTo);
                    if (s == "" || s == null)
                        s = SystemConfig.PathOfDataUser + Path.DirectorySeparatorChar + "UploadFile" + Path.DirectorySeparatorChar + this.FK_MapData + Path.DirectorySeparatorChar;
                    return s;
                }

                if (this.AthSaveWay == Sys.AthSaveWay.FTPServer)
                {
                    string s = this.GetValStringByKey(FrmAttachmentAttr.SaveTo);
                    if (s == "" || s == null)
                        s =   @"//"+ this.FK_MapData + "//";
                    return s;
                }

                return this.FK_MapData;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.SaveTo, value);
            }
        }
        /// <summary>
        /// 数据关联组件ID
        /// </summary>
        public string DataRefNoOfObj
        {
            get
            {
                string str= this.GetValStringByKey(FrmAttachmentAttr.DataRefNoOfObj);
                if (str == "")
                    str = this.NoOfObj;
                return str;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.DataRefNoOfObj, value);
            }
        }
        /// <summary>
        /// 附件编号
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.NoOfObj);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.NoOfObj, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.X);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.W);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.W, value);
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.H);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.H, value);
            }
        }
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.GroupID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.GroupID, value);
            }
        }
        /// <summary>
        /// 阅读规则:@0=不控制@1=未阅读阻止发送@2=未阅读做记录
        /// </summary>
        public int ReadRole
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.ReadRole);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.ReadRole, value);
            }
        }
        

        public int RowIdx
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.RowIdx);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.RowIdx, value);
            }
        }
        /// <summary>
        /// 数据控制方式
        /// </summary>
        public AthCtrlWay HisCtrlWay
        {
            get
            {
                return (AthCtrlWay)this.GetValIntByKey(FrmAttachmentAttr.CtrlWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.CtrlWay, (int)value);
            }
        }
        /// <summary>
        /// 是否是合流汇总多附件？
        /// </summary>
        public bool IsHeLiuHuiZong
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsHeLiuHuiZong);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsHeLiuHuiZong, value);
            }
        }
        /// <summary>
        /// 该附件是否汇总到合流节点上去？
        /// </summary>
        public bool IsToHeLiuHZ
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsToHeLiuHZ);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsToHeLiuHZ, value);
            }
        }
        /// <summary>
        /// 文件展现方式
        /// </summary>
        public FileShowWay FileShowWay
        {
            get
            {
                return (FileShowWay)this.GetParaInt(FrmAttachmentAttr.FileShowWay);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FileShowWay, (int)value);
            }
        }
        /// <summary>
        /// 上传方式（对于父子流程有效）
        /// </summary>
        public AthUploadWay AthUploadWay
        {
            get
            {
                return (AthUploadWay)this.GetValIntByKey(FrmAttachmentAttr.AthUploadWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.AthUploadWay, (int)value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmAttachmentAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_MapData, value);
            }
        }
        
        #endregion

        #region weboffice文档属性
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsRowLock, false);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsRowLock, value);
            }
        }
        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool IsWoEnablePrint
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnablePrint);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnablePrint, value);
            }
        }
        /// <summary>
        /// 是否启用只读
        /// </summary>
        public bool IsWoEnableReadonly
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableReadonly);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableReadonly, value);
            }
        }
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public bool IsWoEnableRevise
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableRevise);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableRevise, value);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool IsWoEnableSave
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableSave);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableSave, value);
            }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool IsWoEnableViewKeepMark
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableViewKeepMark);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableViewKeepMark, value);
            }
        }
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public bool IsWoEnableWF
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableWF);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableWF, value);
            }
        }

        /// <summary>
        /// 是否启用套红
        /// </summary>
        public bool IsWoEnableOver
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableOver);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableOver, value);
            }
        }

        /// <summary>
        /// 是否启用签章
        /// </summary>
        public bool IsWoEnableSeal
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableSeal);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableSeal, value);
            }
        }

        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public bool IsWoEnableTemplete
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableTemplete);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableTemplete, value);
            }
        }

        /// <summary>
        /// 是否记录节点信息
        /// </summary>
        public bool IsWoEnableCheck
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableCheck);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableCheck, value);
            }
        }

        /// <summary>
        /// 是否插入流程图
        /// </summary>
        public bool IsWoEnableInsertFlow
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableInsertFlow);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableInsertFlow, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableInsertFengXian
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableInsertFengXian);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableInsertFengXian, value);
            }
        }

        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public bool IsWoEnableMarks
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableMarks);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableMarks, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableDown
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsWoEnableDown);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsWoEnableDown, value);
            }
        }

        #endregion weboffice文档属性

        #region 快捷键
        /// <summary>
        /// 是否启用快捷键
        /// </summary>
        public bool FastKeyIsEnable
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.FastKeyIsEnable);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FastKeyIsEnable, value);
            }
        }
        /// <summary>
        /// 启用规则
        /// </summary>
        public string FastKeyGenerRole
        {
            get
            {
                return this.GetParaString(FrmAttachmentAttr.FastKeyGenerRole);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FastKeyGenerRole, value);
            }
        }
        #endregion 快捷键

        #region 构造方法
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachment()
        {
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="mypk"></param>
        public FrmAttachment(string mypk)
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

                Map map = new Map("Sys_FrmAttachment", "添付ファイル");

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = FrmAttachmentAttr.FK_MapData;

                map.AddMyPK();


                map.AddTBString(FrmAttachmentAttr.FK_MapData, null, "フォームID", true, false, 1, 100, 20);
                map.AddTBString(FrmAttachmentAttr.NoOfObj, null, "添付ファイル番号", true, false, 0, 50, 20);
                map.AddTBInt(FrmAttachmentAttr.FK_Node, 0, "ノード制御（slnに有効）", false, false);

                //for渔业厅增加.
                map.AddTBInt(FrmAttachmentAttr.AthRunModel, 0, "動作モード", false, false);
                map.AddTBInt(FrmAttachmentAttr.AthSaveWay, 0, "保管方法", false, false);

                map.AddTBString(FrmAttachmentAttr.Name, null, "名前", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentAttr.Exts, null, "アップロード形式", true, false, 0, 200, 20);
                map.AddTBInt(FrmAttachmentAttr.NumOfUpload, 0, "アップロードの最小数", true, false);
                map.AddTBInt(FrmAttachmentAttr.TopNumOfUpload, 99, "アップロードの最大数", true, false);
                map.AddTBInt(FrmAttachmentAttr.FileMaxSize, 10240, "最大添付ファイル制限（KB）", true, false);
                map.AddTBInt(FrmAttachmentAttr.UploadFileNumCheck, 0, "チェック方法をアップロード", true, false);

                //map.AddDDLSysEnum(FrmAttachmentAttr.UploadFileNumCheck, 0, "上传校验方式", true, true, FrmAttachmentAttr.UploadFileNumCheck,
                //  "@0=不用校验@1=不能为空@2=每个类别下不能为空");

                map.AddTBString(FrmAttachmentAttr.SaveTo, null, "に保存", true, false, 0, 150, 20);
                map.AddTBString(FrmAttachmentAttr.Sort, null, "カテゴリ（空でも可）", true, false, 0, 500, 20);

                map.AddTBFloat(FrmAttachmentAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmAttachmentAttr.Y, 5, "Y", false, false);
                map.AddTBFloat(FrmAttachmentAttr.W, 40, "TBWidth", false, false);
                map.AddTBFloat(FrmAttachmentAttr.H, 150, "H", false, false);

                map.AddBoolean(FrmAttachmentAttr.IsUpload, true, "アップロードできるかどうか", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsVisable, true, "見えるかどうか", false, false);
                //  map.AddTBInt(FrmAttachmentAttr.IsDelete, 1, "附件删除规则(0=不能删除1=删除所有2=只能删除自己上传的)", false, false);
                map.AddTBInt(FrmAttachmentAttr.FileType, 0, "添付ファイルタイプ", false,false);

                //hzm新增列
                map.AddTBInt(FrmAttachmentAttr.DeleteWay, 0, "添付ファイル削除ルール（0 =削除不可1 =すべて削除2 =自己アップロードのみ削除可能", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsDownload, true, "ダウンロードできるかどうか", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsOrder, false, "並べ替えできますかどうか", false, false);


                map.AddBoolean(FrmAttachmentAttr.IsAutoSize, true, "サイズ自動制御", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsNote, true, "備考を追加するかどうか", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsExpCol, false, "拡張列を有効にするかどうか", false, false);

                map.AddBoolean(FrmAttachmentAttr.IsShowTitle, true, "タイトルバーを表示するかどうか", false, false);
                map.AddTBInt(FrmAttachmentAttr.UploadType, 0, "アップロードタイプ0シングル1マルチ2を指定", false, false);
                #region 流程属性.
                //对于父子流程有效.
                map.AddTBInt(FrmAttachmentAttr.CtrlWay, 0, "プレゼンテーションコントロールモードの制御0 = PK、1 = FID、2 = ParentID", false, false);
                map.AddTBInt(FrmAttachmentAttr.AthUploadWay, 0, "アップロード制御モードの制御0 =継承モード、1 =協調モード.", false, false);
                map.AddTBInt(FrmAttachmentAttr.ReadRole, 0, "読むルール", true, true);

                //数据引用，如果为空就引用当前的.
                map.AddTBString(FrmAttachmentAttr.DataRefNoOfObj, null, "データ参照コンポーネントID", true, false, 0, 150, 20, true, null);
                #endregion 流程属性.

                #region WebOffice控制方式
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableWF, true, "Webofficeを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableSave, true, "保存を有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableReadonly, true, "読み取り専用かどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableRevise, true, "リビジョンを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableViewKeepMark, true, "ユーザートレースをチェックするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnablePrint, true, "印刷するかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableOver, true, "アービトラージを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableSeal, true, "署名を有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableTemplete, false, "テンプレートファイルを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableCheck, true, "ノード情報を記録するかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableInsertFlow, true, "挿入フローを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableInsertFengXian, true, "リスクポイントの挿入を有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableMarks, true, "マークモードに入るかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableDown, true, "ダウンロードを有効にするかどうか", true, true);
                #endregion WebOffice控制方式


                //参数属性.
                map.AddTBAtParas(3000);

              //  map.AddTBInt(FrmAttachmentAttr.RowIdx, 0, "RowIdx", false, false);
                map.AddTBInt(FrmAttachmentAttr.GroupID, 0, "GroupID", false, false);
                map.AddTBString(FrmAttachmentAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public bool IsUse = false;
        protected override bool beforeUpdateInsertAction()
        {
            if (this.FK_Node == 0)
            {
                //适应设计器新的规则 by dgq 
                if (!DataType.IsNullOrEmpty(this.NoOfObj) && this.NoOfObj.Contains(this.FK_MapData))
                    this.MyPK = this.NoOfObj;
                else
                    this.MyPK = this.FK_MapData + "_" + this.NoOfObj;
            }
            else
                this.MyPK = this.FK_MapData + "_" + this.NoOfObj + "_" + this.FK_Node;

            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeInsert()
        {
            this.IsWoEnableWF = true;
            this.IsWoEnableSave = false;
            this.IsWoEnableReadonly = false;
            this.IsWoEnableRevise = false;
            this.IsWoEnableViewKeepMark = false;
            this.IsWoEnablePrint = false;
            this.IsWoEnableOver = false;
            this.IsWoEnableSeal = false;
            this.IsWoEnableTemplete = false;

            if (this.FK_Node == 0)
                this.MyPK = this.FK_MapData + "_" + this.NoOfObj;
            else
                this.MyPK = this.FK_MapData + "_" + this.NoOfObj + "_" + this.FK_Node;
            
            //对于流程类的多附件，默认按照WorkID控制. add 2017.08.03  by zhoupeng.
            if (this.FK_Node != 0)
                this.HisCtrlWay = AthCtrlWay.WorkID;

            return base.beforeInsert();
        }
        /// <summary>
        /// 插入之后
        /// </summary>
        protected override void afterInsert()
        {
            GroupField gf = new GroupField();
            if (this.FK_Node == 0 && gf.IsExit(GroupFieldAttr.CtrlID, this.MyPK) == false)
            {
                gf.FrmID = this.FK_MapData;
                gf.CtrlID = this.MyPK;
                gf.CtrlType = "Ath";
                gf.Lab = this.Name;
                gf.Idx = 0;
                gf.Insert(); //插入.
            }
            base.afterInsert();
        }

        /// <summary>
        /// 删除之后.
        /// </summary>
        protected override void afterDelete()
        {
            GroupField gf = new GroupField();
            gf.Delete(GroupFieldAttr.CtrlID, this.MyPK);

            base.afterDelete();
        }
    }
    /// <summary>
    /// 附件s
    /// </summary>
    public class FrmAttachments : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件s
        /// </summary>
        public FrmAttachments()
        {
        }
        /// <summary>
        /// 附件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmAttachments(string fk_mapdata)
        {
            this.Retrieve(FrmAttachmentAttr.FK_MapData, fk_mapdata, FrmAttachmentAttr.FK_Node, 0);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachment();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmAttachment> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmAttachment>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmAttachment> Tolist()
        {
            System.Collections.Generic.List<FrmAttachment> list = new System.Collections.Generic.List<FrmAttachment>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmAttachment)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
