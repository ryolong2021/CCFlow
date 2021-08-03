-- 系统类别;
DELETE FROM GPM_AppSort;
INSERT INTO GPM_AppSort(No,Name,RefMenuNo) VALUES ('01','ビジネスシステム','2000');
INSERT INTO GPM_AppSort(No,Name,RefMenuNo) VALUES ('02','オフィスシステム','2001');

-- 系统;
DELETE FROM GPM_App;
INSERT INTO GPM_App(No,Name,AppModel,Url,MyFileName,MyFilePath,MyFileExt,WebPath,FK_AppSort,RefMenuNo) VALUES ('CCOA','ギャロッピングOA',0,'http://www.chichengoa.org/App/login/login.ashx','ccoa.png','Path','GIF','/DataUser/BP.GPM.STem/CCOA.png','01','2002'); /*此处地址被孙战平改动*/

DELETE FROM GPM_Menu;

-- root;
INSERT INTO GPM_Menu(FK_App,No,ParentNo,Name,MenuType,Url,IsEnable) VALUES ('UnitFullName','1000','0','Jinan Chicheng Information Technology Co., Ltd.',0,'',1);
INSERT INTO GPM_Menu(FK_App,No,ParentNo,Name,MenuType,Url,IsEnable) VALUES ('AppSort','2000','1000','ビジネスシステム',1,'',1);
INSERT INTO GPM_Menu(FK_App,No,ParentNo,Name,MenuType,Url,IsEnable) VALUES ('AppSort','2001','1000','オフィスシステム',1,'',1);
INSERT INTO GPM_Menu(FK_App,No,ParentNo,Name,MenuType,Url,IsEnable) VALUES ('CCOA','2002','2000','ギャロッピングOA',2,'',1);


-- 工作流程.;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200200', '作業過程', '2002', 0, 3, 'CCOA', '', 0,'icon-home',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200200-1', '開始', '200200',  1, 4, 'CCOA', '/WF/Start.htm',0,'icon-plane',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200200-2', '対応待ち', '200200',  2, 4, 'CCOA', '/WF/Todolist.htm', 0,'icon-cup',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200200-3', '途中', '200200',  3, 4, 'CCOA', '/WF/Runing.htm',  0,'icon-hourglass',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200200-4', 'クエリ', '200200',  4, 4, 'CCOA', '/WF/SearchBS.htm',  0,'icon-eyeglass',1);

-- 公告通知;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200201', 'ニュース＆アナウンス', '2002',  1, 3, 'CCOA', '',0,'',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200201-1', 'アナウンス管理', '200201', 1, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Notices',0,'',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200201-2', 'アナウンスカテゴリ', '200201', 2, 4, 'CCOA', '/WF/Comm/Ens.htm?EnsName=BP.OA.NoticeCategorys', 0,'',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200201-3', 'ニュース台帳', '200201', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Newss', 0,'',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200201-4', 'ニュースカテゴリ', '200201', 4, 4, 'CCOA', '/WF/Comm/Ens.htm?EnsName=BP.OA.NewsSorts',0,'',1);
   
-- CCOA 菜单;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200202', 'マイツール', '2002',3, 3, 'CCOA', '', 0,'',1);
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('200202-1', 'マイアドレス帳', '200202',  2, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.ALEmps', 0,'',1);
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('253', '科学计算器', '252', 9, 4, 'CCOA', '/App/SmallTools/ComputingCounter.htm',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('254', '万年历', '252', 10, 4, 'CCOA', '/App/SmallTools/Calendar.htm',0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('255', '记事便签', '252', 7, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Notepaper.Notepapers', 0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('1011', '我的计划', '252',  6, 4, 'CCOA', '/App/PrivPlan/MyPlan.htm',0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('1037', '我的日程', '252', 5, 4, 'CCOA', '/App/Calendar/MyCalendar.htm', 0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('2010', '发送短消息', '252',  8, 4, 'CCOA', '/Main/Sys/SendShortMsg.htm', 0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('250', '发起投票', '252',  3, 4, 'CCOA', '/App/Vote/CreateVote.htm', 0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('251', '我的投票', '252',  4, 4, 'CCOA', '/App/Vote/VoteList.htm',  0,'');

-- 文档中心;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('106', 'ドキュメントセンター（再構築中）', '2002',  10, 3, 'CCOA', '', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('107', 'ナレッジツリー', '106',  1, 4, 'CCOA', '/App/KM/Sort.htm', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('147', 'マイドキュメント', '106', 2, 4, 'CCOA', '/App/Km/MyDoc.htm',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('160', '知財管理', '106', 5, 4, 'CCOA', '/App/KM/Main.htm',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('186', '最新のドキュメント', '106', 4, 4, 'CCOA', '/App/KM/NewlyDoc.htm',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('256', '同僚共有', '106',  3, 4, 'CCOA', '/App/KM/EmpsShare.htm', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('262', '知識共有', '106', 6, 4, 'CCOA', '/App/KM/CompanyDoc.htm',  0,'');

-- 车辆管理;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('145', '車両管理', '2002', 12, 3, 'CCOA', '',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('146', 'ドライバー情報', '145', 0, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Car.Drivers',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('185', 'インデックス管理', '145', 0, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Car.ZhiBiaos',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('261', '車両情報', '145',  0, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Car.CarInfos',  0,'');

-- 办公用品;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('198', '事務用品', '2002', 15, 3, 'CCOA', '',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('199', '在庫台帳', '198',  2, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.DS.DSMains',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('200', '購入台帳', '198',  3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.DS.DSBuys', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('204', '取得台帳', '198',  4, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.DS.DSTakes', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('263', '消耗品カテゴリー', '198', 1, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.DS.DSSorts',  0,'');

-- 人力资源;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('201', '人事', '2002', 16, 3, 'CCOA','', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('202', 'スタッフファイル', '201', 1, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HR.HRRecords', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('203', '契約締結情報', '201', 4, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HR.HRContracts', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('214', '契約終了情報', '201',  3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HR.HRContractRemoves',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('228', '転送記録', '201',  0, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HRTransfers', 0,'');
-- INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('230', '发起人员调动流程', '201', 0, 4, 'CCOA', '/WF/MyFlow.htm?FK_Flow=Transfer', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('217', 'プロビデントファンド台帳', '201',  6, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HR.GongJiJins', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('219', '社会保障と利益の台帳', '201', 7, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HR.FuLis', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('259', '契約変更口座', '201', 5, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.HR.HRContractModifys',  0,'');

-- 固定资产管理;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('196', '固定資産', '2002', 14, 3, 'CCOA', '', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('243', 'カテゴリー', '196',  0, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.FixAssBigCatagorys',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('244', 'サブカテゴリ', '196',  1, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.FixAssBigCatSons',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('246', '資産の取得', '196',  3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.GetUses', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('248', '資産の修理', '196', 5, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.Repairs',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('249', '資産の廃止', '196',  6, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.GetBads', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('197', '資産登録', '196', 2, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.FixMans', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('257', '資産の返還', '196', 4, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.FixAss.GiveBacks',  0,'');

-- 会议管理;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('265', '会議管理（再構築中）', '2002', 11, 3, 'CCOA', '', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('267', '会議タイプ', '265',  1, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Meeting.MeetingTypes',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('268', '会議室のリソースのメンテナンス', '265', 4, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Meeting.RoomResources',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('269', '会議室リスト', '265',  3, 4, 'CCOA', '/App/Meeting/RoomListEUI.htm',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('270', '会議室の予約', '265',  2, 4, 'CCOA', '/App/Meeting/OrdingEUI.htm',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('271', 'マイ会議', '265',  0, 4, 'CCOA', '/App/Meeting/MyMeetingEUI.htm',  0,'');

-- 快捷入口;
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('297', '快捷入口', '2002',  0, 3, 'CCOA', '', 0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('299', '待办', '297', 0, 4, 'CCOA', '/WF/Todolist.htm',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('300', '在途', '297',  0, 4, 'CCOA', '/WF/Runing.htm',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('301', '发起', '297', 0, 4, 'CCOA', '/WF/Start.htm ',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('302', '我的邮件', '297',  0, 4, 'CCOA', '/App/Message/InBox.htm ',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('303', '我的公告', '297',  0, 4, 'CCOA', '/App/Notice/MyNoticeList.htm ',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('305', '我的会议', '297', 0, 4, 'CCOA', '/App/Meeting/MyMeetingEUI.htm',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('304', '我的新闻', '297',  0, 4, 'CCOA', '/App/News/MyNewsList.htm',  0,'');

/*
-- 日常办公;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('187', '日常办公', '2002', 1, 3, 'CCOA', '/Main/DeskTop2.htm',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('188', '我的工作台', '187', 1, 4, 'CCOA', '/Main/DeskTop2.htm',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('189', '所有资讯', '187',  2, 4, 'CCOA', '/App/Notice/NoticeList.htm', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('191', '我的日程', '187', 4, 4, 'CCOA', '/App/Calendar/MyCalendar.htm', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('192', '我的文档', '187',  5, 4, 'CCOA', '/App/Km/MyDoc.htm',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('193', '信息块维护', '187',  6, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.Desktop.DesktopPanels', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('264', '知识库', '187',  6, 4, 'CCOA', '/App/KM/CompanyDoc.htm',  0,'');
*/

-- 论坛;
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('272', '我的论坛', '2002', 13, 3, 'CCOA','', 0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('273', '论坛板块', '272',  1, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.OA.BBS.BBSClasss',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('274', '论坛首页', '272',  2, 4, 'CCOA', '/App/BBS/Default.htm?fid=0',  0,'');
--INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('195', '参数设置', '272', 3, 4, 'CCOA', '/App/BBS/SysAdmin/Config.htm',0,'');

-- 权限管理;
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('900', 'システムマネジメント', '2002', 99, 3, 'CCOA','', 0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('961', 'プロセスデザイナー', '900',  1, 4, 'CCOA', '/WF/Admin/CCBPMDesigner/Default.htm',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('962', '組織構造', '900',  2, 4, 'CCOA', '/GPM/Organization.htm',  0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('964', '職種', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.StationTypes',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('966', '役職', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.Stations',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('967', '人員台帳', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.Emps',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('968', 'システムカテゴリ', '900', 3, 4, 'CCOA', '/WF/Comm/Ens.htm?EnsName=BP.GPM.AppSorts',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('969', 'システムアカウント', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.Apps',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('970', '権利グループ', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.Groups',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('971', '投稿メニュー', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.StationExts',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('972', 'スタッフメニュー', '900', 3, 4, 'CCOA', '/WF/Comm/Search.htm?EnsName=BP.GPM.GPMEmps',0,'');
INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon) VALUES ('973', 'メニューツリー', '900', 3, 4, 'CCOA', '/WF/Comm/Tree.htm?EnsName=BP.GPM.Menus',0,'');

UPDATE GPM_Menu SET IsEnable=1;   
 