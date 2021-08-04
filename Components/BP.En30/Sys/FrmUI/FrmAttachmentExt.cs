using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.IO;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachmentExt : EntityMyPK
    {
        /// <summary>
        /// 访问权限.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsView = true;
                uac.IsInsert = false;
                if (BP.Web.WebUser.No == "admin" || BP.Web.WebUser.IsAdmin == true)
                {
                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                    return uac;
                }
                return uac;
            }
        }

        #region 参数属性.
        /// <summary>
        /// 是否可见？
        /// </summary>
        public bool IsVisable
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsVisable, true);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsVisable, value);
            }
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public int FileType
        {
            get
            {
                return this.GetParaInt(FrmAttachmentAttr.FileType);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FileType, value);
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
                    return "シングル添付ファイル";
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
        public string SaveTo
        {
            get
            {
                string s = this.GetValStringByKey(FrmAttachmentAttr.SaveTo);
                if (s == "" || s == null)
                    s = SystemConfig.PathOfDataUser + Path.DirectorySeparatorChar + "UploadFile" + Path.DirectorySeparatorChar + this.FK_MapData + Path.DirectorySeparatorChar;
                return s;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.SaveTo, value);
            }
        }
        /// <summary>
        /// 附件标识
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

        /// <summary>
        /// 是否要转换成html
        /// </summary>
        public bool IsTurn2Html
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsTurn2Html);
            }
        }
        #endregion

        #region weboffice文档属性(参数属性)
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsRowLock, false);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsRowLock, value);
            }
        }
        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool IsWoEnablePrint
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnablePrint);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnablePrint, value);
            }
        }
        /// <summary>
        /// 是否启用只读
        /// </summary>
        public bool IsWoEnableReadonly
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableReadonly);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableReadonly, value);
            }
        }
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public bool IsWoEnableRevise
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableRevise);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableRevise, value);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool IsWoEnableSave
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableSave);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableSave, value);
            }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool IsWoEnableViewKeepMark
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableViewKeepMark);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableViewKeepMark, value);
            }
        }
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public bool IsWoEnableWF
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableWF);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableWF, value);
            }
        }

        /// <summary>
        /// 是否启用套红
        /// </summary>
        public bool IsWoEnableOver
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableOver);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableOver, value);
            }
        }

        /// <summary>
        /// 是否启用签章
        /// </summary>
        public bool IsWoEnableSeal
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableSeal);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableSeal, value);
            }
        }

        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public bool IsWoEnableTemplete
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableTemplete);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableTemplete, value);
            }
        }

        /// <summary>
        /// 是否记录节点信息
        /// </summary>
        public bool IsWoEnableCheck
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableCheck);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableCheck, value);
            }
        }

        /// <summary>
        /// 是否插入流程图
        /// </summary>
        public bool IsWoEnableInsertFlow
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableInsertFlow);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableInsertFlow, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableInsertFengXian
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableInsertFengXian);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableInsertFengXian, value);
            }
        }

        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public bool IsWoEnableMarks
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableMarks);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableMarks, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableDown
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableDown);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableDown, value);
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
        public FrmAttachmentExt()
        {
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="mypk">主键</param>
        public FrmAttachmentExt(string mypk)
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
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;

                map.AddMyPK();

                #region 基本属性。
                map.AddTBString(FrmAttachmentAttr.FK_MapData, null, "フォームID", true, true, 1, 100, 20);
                map.AddTBString(FrmAttachmentAttr.NoOfObj, null, "添付ファイルの識別", true, true, 0, 50, 20);
                map.AddTBInt(FrmAttachmentAttr.FK_Node, 0, "ノード制御（slnに有効）", false, false);

                //for渔业厅增加.
                map.AddDDLSysEnum(FrmAttachmentAttr.AthRunModel, 0, "動作モード", true, true, FrmAttachmentAttr.AthRunModel,
                  "@0=パイプラインモード@1=固定モード@2=カスタムページ");

                map.AddTBString(FrmAttachmentAttr.Name, null, "添付ファイル名", true, false, 0, 50, 20, true);

                map.AddTBString(FrmAttachmentAttr.Exts, null, "ファイルフォーマット", true, false, 0, 50, 20, true, null);
                map.SetHelperAlert(FrmAttachmentAttr.Exts, "アップロードの要件、設定モードは次のとおりです: *.*, *.doc, *.docx, *.png,カンマで区切られた複数。\t\nは、指定されたサフィックスを持つファイルのみがアップロードを許可されることを意味します。");

                map.AddTBInt(FrmAttachmentAttr.NumOfUpload, 0, "最小アップロード数量", true, false);
                map.SetHelperAlert("NumOfUpload", "0の場合、ロゴをアップロードする必要があります。\t\nユーザーがアップロードした添付ファイルの数は、指定した数より少なく、保存されません。");
               
                map.AddTBInt(FrmAttachmentAttr.TopNumOfUpload, 99, "アップロードの最大数", true, false);
                map.AddTBInt(FrmAttachmentAttr.FileMaxSize, 10240, "最大添付ファイル制限（KB）", true, false);
                map.AddDDLSysEnum(FrmAttachmentAttr.UploadFileNumCheck, 0, "チェック方法をアップロード", true, true, FrmAttachmentAttr.UploadFileNumCheck,
                  "@0=チェックしない@1=空にすることはできません@2=各カテゴリの下で空にすることはできません");

                //for tianye group 
                map.AddDDLSysEnum(FrmAttachmentAttr.AthSaveWay, 0, "保存方法", true, true, FrmAttachmentAttr.AthSaveWay,
                  "@0=Webサーバーに保存@1=データベースに保存@2= FTPサーバー");

                map.AddTBString(FrmAttachmentAttr.SaveTo, null, "に保存", false, false, 0, 150, 20, true, null);

                map.AddTBString(FrmAttachmentAttr.Sort, null, "カテゴリー", true, false, 0, 500, 20, true, null);
                map.SetHelperAlert(FrmAttachmentAttr.Sort, "設定形式：プロダクションタイプ、ファイルタイプ、その他。SelectName FROM Port_DeptなどのSQLを設定することもできます\t\n現在、拡張列がサポートされています。拡張列を使用して、追加のフィールドを定義できます。この設定はキャンセルされます。");

                map.AddBoolean(FrmAttachmentAttr.IsTurn2Html, false, "HTMLに変換するかどうか（モバイルブラウジング用）", true, true, true);

                //位置.
                map.AddTBFloat(FrmAttachmentAttr.X, 5, "X", false, false);
                map.AddTBFloat(FrmAttachmentAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(FrmAttachmentAttr.W, 40, "幅", true, false);
                map.AddTBFloat(FrmAttachmentAttr.H, 150, "高さ", true, false);

                //附件是否显示
                map.AddBoolean(FrmAttachmentAttr.IsVisable, true, "添付ファイルのグループを表示するかどうか", true, true, true);

                map.AddDDLSysEnum(FrmAttachmentAttr.FileType, 0, "添付ファイルタイプ", true, true, FrmAttachmentAttr.FileType, "@0=通常の添付ファイル@ 1 =画像ファイル");

                #endregion 基本属性。

                #region 权限控制。
                //hzm新增列
                // map.AddTBInt(FrmAttachmentAttr.DeleteWay, 0, "附件删除规则(0=不能删除1=删除所有2=只能删除自己上传的", false, false);

                map.AddDDLSysEnum(FrmAttachmentAttr.DeleteWay, 1, "添付ファイル削除ルール", true, true, FrmAttachmentAttr.DeleteWay,
                    "@0=削除できません@1=すべて削除@2=自分のアップロードのみを削除");

                map.AddBoolean(FrmAttachmentAttr.IsUpload, true, "アップロードできるかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsDownload, true, "ダウンロードできるかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsOrder, false, "並べ替えできるかどうか", true, true);

                map.AddBoolean(FrmAttachmentAttr.IsAutoSize, true, "サイズ自動制御", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsNote, true, "備考を追加するかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsExpCol, true, "拡張列を有効にするかどうか", true, true);

                map.AddBoolean(FrmAttachmentAttr.IsShowTitle, true, "タイトルバーを表示するかどうか", true, true);
                map.AddDDLSysEnum(FrmAttachmentAttr.UploadType, 0, "アップロードタイプ", true, false, FrmAttachmentAttr.CtrlWay, "@0=単一@1=複数@2=指定");

                map.AddDDLSysEnum(FrmAttachmentAttr.AthUploadWay, 0, "アップロード制御方法の制御", true, true, FrmAttachmentAttr.AthUploadWay, "@0=継承モード@1=コラボレーションモード");

                map.AddDDLSysEnum(FrmAttachmentAttr.CtrlWay, 0, "コントロールプレゼンテーションコントロール", true, true, "Ath" + FrmAttachmentAttr.CtrlWay,
                    "@0=PK-主キー@ 1 = FID-フローID @ 2 =親ID-親フローID @ 3 =自分がアップロードした添付ファイルのみを表示@ 4 = WorkIDに従って計算（フローノードフォームで有効）@ 5 = PPWorkID-grandpaフローID");


                //map.AddDDLSysEnum(FrmAttachmentAttr.DataRef, 0, "数据引用", true, true, FrmAttachmentAttr.DataRef,
                //    "@0=当前组件ID@1=指定的组件ID");
                #endregion 权限控制。

                #region WebOffice控制方式。
                map.AddBoolean(FrmAttachmentAttr.IsRowLock, true, "ロックされた行を有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableWF, true, "Webofficeを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableSave, true, "保存を有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableReadonly, true, "読み取り専用かどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableRevise, true, "リビジョンを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableViewKeepMark, true, "ユーザートレースをチェックするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnablePrint, true, "印刷するかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableSeal, true, "署名を有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableOver, true, "アービトラージを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableTemplete, true, "公式ドキュメントテンプレートを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableCheck, true, "監査情報を自動的に書き込むかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableInsertFlow, true, "フローを挿入するかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableInsertFengXian, true, "リスクポイントを挿入するかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableMarks, true, "マークモードを有効にするかどうか", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsWoEnableDown, true, "ダウンロードを有効にするかどうか", true, true);
                #endregion WebOffice控制方式。

                #region 节点相关
                //map.AddDDLSysEnum(FrmAttachmentAttr.DtlOpenType, 0, "附件删除规则", true, true, FrmAttachmentAttr.DeleteWay, 
                //    "@0=不能删除@1=删除所有@2=只能删除自己上传的");
                map.AddBoolean(FrmAttachmentAttr.IsToHeLiuHZ, true, "添付ファイルを合流ノードで要約する必要がありますか？ （子スレッドノードで有効）", true, true, true);
                map.AddBoolean(FrmAttachmentAttr.IsHeLiuHuiZong, true, "それは合流ノードの要約アクセサリコンポーネントですか？ （合流ノードで有効）", true, true, true);
                map.AddTBString(FrmAttachmentAttr.DataRefNoOfObj, "AttachM1", "対応する添付ファイルの識別", true, false, 0, 150, 20);
                map.SetHelperAlert("DataRefNoOfObj", "WorkIDパーミッションモードで有効であり、フロー全体を通して添付ファイルIDをクエリするために使用され、テーブルのIDと同じです。");

                map.AddDDLSysEnum(FrmAttachmentAttr.ReadRole, 0, "読むルール", true, true, FrmAttachmentAttr.ReadRole,
               "@0=コントロールしない@1=未読を発送すること制御@2=未読を記録する");
                #endregion 节点相关

                #region 其他属性。
                //参数属性.
                map.AddTBAtParas(3000);
                #endregion 其他属性。

                RefMethod rm = new RefMethod();
                rm.Title = "高度な構成";
                //  rm.Icon = "/WF/Admin/CCFormDesigner/Img/Menu/CC.png";
                //rm.ClassMethodName = this.ToString() + ".DoAdv";
                // rm.RefMethodType = RefMethodType.RightFrameOpen;
                //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "カテゴリー設定";
                rm.ClassMethodName = this.ToString() + ".DoSettingSort";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "FTPサーバーのテスト";
                rm.ClassMethodName = this.ToString() + ".DoTestFTPHost";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "拡張列を設定する";
                rm.ClassMethodName = this.ToString() + ".DtlOfAth";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DtlOfAth()
        {
            string url = "../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapDtl=" + this.MyPK + "&For=" + this.MyPK;
            return url;
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        public string DoTestFTPHost()
        {
            try
            {
                FtpSupport.FtpConnection conn = new FtpSupport.FtpConnection();
                conn.Connect(SystemConfig.FTPServerIP, SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);
                return "接続に成功しました。";
            }
            catch (Exception ex)
            {
                return "err@接続に失敗しました:" + ex.Message;
            }
        }
        /// <summary>
        /// 固定模式类别设置
        /// </summary>
        /// <returns></returns>
        public string DoSettingSort()
        {
            return "../../Admin/FoolFormDesigner/AttachmentSortSetting.htm?FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + "&Ath=" + this.NoOfObj;
        }
        /// <summary>
        /// 执行高级设置.
        /// </summary>
        /// <returns></returns>
        public string DoAdv()
        {
            return "/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + "&Ath=" + this.NoOfObj;
        }

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

            if (this.FK_Node != 0)
            {
                /*工作流程模式.*/
                if (this.HisCtrlWay == AthCtrlWay.PK)
                    this.HisCtrlWay = AthCtrlWay.WorkID;
                this.MyPK = this.FK_MapData + "_" + this.NoOfObj + "_" + this.FK_Node;
            }


            #region 处理分组.
            //更新相关的分组信息.
            if (this.IsVisable == true && this.FK_Node == 0)
            {
                GroupField gf = new GroupField();
                int i = gf.Retrieve(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);
                if (i == 0)
                {
                    gf.Lab = this.Name;
                    gf.FrmID = this.FK_MapData;
                    gf.CtrlType = "Ath";
                    //gf.CtrlID = this.MyPK;
                    gf.Insert();
                }
                else
                {
                    gf.Lab = this.Name;
                    gf.FrmID = this.FK_MapData;
                    gf.CtrlType = "Ath";
                    //gf.CtrlID = this.MyPK;
                    gf.Update();
                }
            }

            if (this.IsVisable == false)
            {
                GroupField gf = new GroupField();
                gf.Delete(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);
            }
            #endregion 处理分组.


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

        protected override void afterInsertUpdateAction()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.MyPK = this.MyPK;
            ath.RetrieveFromDBSources();
            ath.Update();
          

            //判断是否是字段附件
            MapAttr mapAttr = new MapAttr();
            mapAttr.MyPK = this.MyPK;
            if (mapAttr.RetrieveFromDBSources() != 0 && mapAttr.Name.Equals(this.Name) == false)
            {
                mapAttr.Name = this.Name;
                mapAttr.Update();
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除之后.
        /// </summary>
        protected override void afterDelete()
        {
            GroupFields gfs= new GroupFields();
            gfs.RetrieveByLike(GroupFieldAttr.CtrlID, this.MyPK+"%");
            gfs.Delete();
            //gf.Delete(GroupFieldAttr.CtrlID, this.MyPK);

            //把相关的字段也要删除.
            MapAttrString attr = new MapAttrString();
            attr.MyPK = this.MyPK;
            if (attr.RetrieveFromDBSources() != 0)
            {
                attr.Delete();  
            }
            

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterDelete();
        }
    }
    /// <summary>
    /// 附件s
    /// </summary>
    public class FrmAttachmentExts : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件s
        /// </summary>
        public FrmAttachmentExts()
        {
        }
        /// <summary>
        /// 附件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmAttachmentExts(string fk_mapdata)
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
                return new FrmAttachmentExt();
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
