
/** -- ========================= 系统升级SQL (为了方便系统升级代码写入的问题,增加该SQL) 目的是为了方便JFlow CCFlow 的统一版本升级. **/

-- 升级旧版本，删除连接线, 如果升级到这里有错误，就需要删除重复的连接线.
update WF_Direction set mypk=replace(mypk, '_0','');

UPDATE Sys_MapAttr SET IsSupperText=1 WHERE (IsSupperText=0 OR IsSupperText IS NULL ) AND MyDataType=7;

UPDATE Sys_MapData SET FK_FormTree='' WHERE No LIKE 'ND%';

DELETE FROM Sys_Enum WHERE EnumKey ='SelectorModel';
DELETE FROM Sys_Enum WHERE EnumKey ='SrcType';
DELETE FROM Sys_Enum WHERE EnumKey ='CondModel';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmTrackSta'; 
DELETE FROM Sys_Enum WHERE EnumKey ='EventDoType'; 
DELETE FROM Sys_Enum WHERE EnumKey ='EditModel';
DELETE FROM Sys_Enum WHERE EnumKey ='AthRunModel'; 
-- 更新枚举值;
DELETE FROM Sys_Enum WHERE EnumKey ='CodeStruct';
DELETE FROM Sys_Enum WHERE EnumKey ='DBSrcType';
DELETE FROM Sys_Enum WHERE EnumKey ='TBModel';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmType';
DELETE FROM Sys_Enum WHERE EnumKey ='WebOfficeEnable';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmEnableRole';
DELETE FROM Sys_Enum WHERE EnumKey ='BlockModel';
DELETE FROM Sys_Enum WHERE EnumKey ='FWCType';
DELETE FROM Sys_Enum WHERE EnumKey ='SelectAccepterEnable';
DELETE FROM Sys_Enum WHERE EnumKey ='NodeFormType';
DELETE FROM Sys_Enum WHERE EnumKey ='StartGuideWay';
DELETE FROM Sys_Enum WHERE EnumKey ='StartLimitRole';
DELETE FROM Sys_Enum WHERE EnumKey ='BillFileType';
DELETE FROM Sys_Enum WHERE EnumKey ='EventDoType';
DELETE FROM Sys_Enum WHERE EnumKey ='FormType';
DELETE FROM Sys_Enum WHERE EnumKey ='BatchRole';
DELETE FROM Sys_Enum WHERE EnumKey ='StartGuideWay';
DELETE FROM Sys_Enum WHERE EnumKey ='NodeFormType';
DELETE FROM Sys_Enum WHERE EnumKey ='FrmType';
DELETE FROM Sys_Enum WHERE EnumKey ='FTCSta';
DELETE FROM Sys_Enum WHERE EnumKey ='SrcType';
DELETE FROM Sys_Enum WHERE EnumKey IN ('TodolistModel','CCStaWay','TWay' );

DELETE FROM Sys_GloVar WHERE GroupKey='DefVal';

INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('0','システム規則のデフォルト値を選択','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.No','ログイン人員アカウント','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.Name','ログイン人員名前','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.FK_Dept','ログイン人員部門番号','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.FK_DeptName','ログイン人員部門名','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@WebUser.FK_DeptFullName','ログイン人員部門フルネーム','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yyyy年MM月dd日','現在日付（yyyy年MM月dd日）','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yyyy年MM月dd日HH時mm分','現在日付（yyyy年MM月dd日HH時mm分）','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yy年MM月dd日','現在日付（yy年MM月dd日）','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@yy年MM月dd日HH時mm分','現在日付（yy年MM月dd日HH時mm分）','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@FK_ND','現在の年度','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@FK_YF','現在の月','DefVal');
INSERT INTO Sys_GloVar (No,Name,GroupKey) VALUES ('@CurrWorker','現在の仕事を扱える人員','DefVal');
 
-- 升级数据源 2016.
UPDATE Sys_SFTable SET SrcType=0 WHERE No LIKE '%.%';
UPDATE Sys_SFTable SET SrcType=1 WHERE No NOT LIKE '%.%' AND SrcType=0;

--更新日期长度.
UPDATE SYS_MAPATTR SET UIWidth=125 WHERE MYDATATYPE=6;
UPDATE SYS_MAPATTR SET UIWidth=145 WHERE MYDATATYPE=7;


-- 2016.11.18 升级维护附件属性.;
DELETE FROM Sys_EnCfg WHERE No='BP.Sys.FrmUI.FrmAttachmentExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.Sys.FrmUI.FrmAttachmentExt',
'@MyPK=基本情報,添付ファイルの基本構成.
@DeleteWay=権限制御,添付ファイルのダウンロードおよびアップロード権限を制御します.@IsRowLock=WebOffice属性,公式ドキュメントに関連する属性構成を設定します.
@IsToHeLiuHZ=プロセスに関連,制御ノード分合流.');

-- 2020.02.25 升级明细表维护分组.;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapDtlExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapDtlExt','@No=基本情報,基本情報機関情報.@IsExp=データのインポートとエクスポート,データのインポートとエクスポート.@IsEnableLink=ハイパーリンク,スレーブテーブルの右側に表示.@IsCopyNDData=プロセス関連,プロセスに関連する構成はプロセスでない場合は無視できます.');


DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FrmNodeComponent';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FrmNodeComponent','@NodeID=監査コンポーネント,sdkフォーム監査コンポーネントおよびccformの監査コンポーネントプロパティ設定に適しています.@SFLab=親子プロセスコンポーネント,このノードで親子プロセスを構成および表示します.@FrmThreadLab=子スレッドコンポーネント,合流ノードに有効,子スレッドの実行ステータスを構成および表示するために使用されます.@FrmTrackLab=トラックコンポーネント,実行中のプロセスの軌道グラフを表示する.@FTCLab=流通自定義,各ノードのノードのプロセッサを制御します.');

-- 傻瓜表单属性; 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapFrmFool';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapFrmFool','@No=基本属性,基本属性@Designer=デザイナー情報,デザイナーのユニット情報,人事情報.フォームクラウドにアップロードできます.');


-- 2018.07.24 ; 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowExt','@No=基本情報,基本情報機関情報.@IsBatchStart=データとフォーム,データのインポートとエクスポート.@DesignerNo=デザイナー,プロセス開発者情報');


--新版本的流程属性,节点属性;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeExt','@NodeID=基本構成@SendLab=ボタン権限,制御作業ノードはボタンを操作できます.@RunModel=実行モード,分合流,親子プロセス@AutoJumpRole0=ジャンプ,自動ジャンプルールこのノードに遭遇したときに自動にする方法次のステップに進みます.');
  
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapDataExt';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapDataExt','@No=基本属性@Designer=デザイナー情報');
UPDATE Sys_MapData SET AppType=0 WHERE No NOT LIKE 'ND%';

-- 旧版本的流程属性;
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeSheet';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeSheet','@NodeID=基本構成@FormType=フォーム@FWCSta=監査コンポーネント,SDKフォーム監査コンポーネントおよびccformの監査コンポーネントプロパティ設定に適用可能@SFSta=親子プロセス,開始,親子プロセスの制御設定を表示します.@SendLab=ボタン権限,制御作業ノードはボタンを操作できます.@RunModel=実行モード,分合流,親子プロセス@AutoJumpRole0=ジャンプ,自動ジャンプルールこのノードに遭遇したときに次のステップを自動的に実行する方法.@MPhone_WorkModel=移動,携帯電話やタブレットに関連するアプリケーション設定.@OfficeOpen=公式ドキュメントボタン,ノードが公式ドキュメントプロセスの場合にのみ有効');
 
DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowSheet';                 
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowSheet','@No=基本構成@FlowRunWay=起動方法,配置ワークフローがどのように自動に起動すること,このオプションとプロセスサービスが同時に実行すると有効になる.@StartLimitRole=起動制限ルール@StartGuideWay=ナビゲーション起動@CFlowWay=プロセスの続行@DTSWay=プロセスデータとビジネスデータの同期@PStarter=トラックビューのアクセス権限');

--2016.07 升级数据源;
UPDATE Sys_SFTable SET FK_SFDBSrc='local' WHERE FK_SFDBSrc IS NULL OR FK_SFDBSrc='';
UPDATE Sys_SFTable SET  SrcType=0 WHERE SrcType IS NULL ;
--UPDATE Sys_MapAttr SET ColSpan=4 WHERE ColSpan>=3;

-- 2019.03.10 ; 
DELETE FROM Sys_EnCfg WHERE No='BP.Frm.FrmBill';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.Frm.FrmBill','@No=基本情報,ドキュメントの基本構成情報.@BtnNewLable=ドキュメントボタンの権限,各ボタン起動ルールの制御.@BtnImpExcel=リストボタン,リストボタンコントロール@Designer=デザイナー,プロセス開発デザイナー情報');

-- 2019.05.15 ; 
DELETE FROM Sys_EnCfg WHERE No='BP.Frm.FrmDict';
INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.Frm.FrmDict','@No=基本情報,ドキュメントの基本構成情報@BtnNewLable=ドキュメントボタンの権限,各ボタン起動ルールの制御.@BtnImpExcel=リストボタン,リストボタンコントロール@Designer=デザイナー,プロセス開発デザイナー情報');
 
 ----2019.06.03
 update Sys_MapAttr set ColSpan =0 ,TextColSpan = 2 where UIContralType=9 and ColSpan = 1;

 --2019.5.23
DELETE FROM Sys_Enum WHERE EnumKey ='CondModel';
INSERT INTO Sys_Enum(MyPK,Lab,EnumKey,IntKey,Lang) VALUES('CondModel_CH_0','接続線の状態によって制御されます','CondModel',0,'CH');
INSERT INTO Sys_Enum(MyPK,Lab,EnumKey,IntKey,Lang) VALUES('CondModel_CH_1','ユーザーの選択による','CondModel',1,'CH');
INSERT INTO Sys_Enum(MyPK,Lab,EnumKey,IntKey,Lang) VALUES('CondModel_CH_2','送信ボタンの横にあるドロップダウンボックスから選択します','CondModel',2,'CH');
 