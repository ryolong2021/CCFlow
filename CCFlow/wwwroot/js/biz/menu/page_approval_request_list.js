/*▼グローバル変数定義エリア▼*/
//…追加変数

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {

     if (webUser == null)
         webUser = new WebUser();
     if (webUser.No == null)
         return;

    //画面初期化
    GetDataAndInitPage();

    //…追加関数			

});
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/
/**
 * サーバーからデータ、画面初期化
 */
function GetDataAndInitPage() {

    getListData(InitPage); 
}

/**
 * 画面初期化(callback用)
 */
function InitPage(listData) {

    // 承認待ちタグの初期化表示
    this.createCardListForInProgress(listData[0]);

    // 未完了タグの初期化表示
    this.createCardListForInComplete(listData[1]);

    // 完了タグの初期化表示
    this.createCardListForComplete(listData[2]);

    // 絞り込み検索画面の作成
    this.createRefinedSearchContorl(listData);

    // すべて確認済にする(承認待ち)
    $("#a-wait").click(function () {
        clearAllBadges(GET_APPROVAL_INPROCESS, "#in-progress-card-list");
    });
    // すべて確認済にする(完了)
    $("#a-done").click(function () {
        clearAllBadges(GET_APPROVAL_COMPLETE, "#complete-card-list");
    });

    // 否認/差戻の理由の長さの制御の設定(ダイアログの理由項目)
    $("#deny_content").attr("maxlength", "250");

    // 承認待ちバッジの設定
    if (usrLocal[GET_APPROVAL_INPROCESS]["display"] != undefined && usrLocal[GET_APPROVAL_INPROCESS]["display"] != 0) {
        $("#alert-wait").children(":first").children(":first").children(":first").html(usrLocal[GET_APPROVAL_INPROCESS]["display"]);
        $("#alert-wait").removeAttr("hidden");
        $("#a-tab-wait").addClass("a-tab-nav-item__badge");
    }

    // 完了バッジの設定
    if (usrLocal[GET_APPROVAL_COMPLETE]["display"] != undefined && usrLocal[GET_APPROVAL_COMPLETE]["display"] != 0) {
        $("#alert-done").children(":first").children(":first").children(":first").html(usrLocal[GET_APPROVAL_COMPLETE]["display"]);
        $("#alert-done").removeAttr("hidden");
        $("#a-tab-done").addClass("a-tab-nav-item__badge");
    }

    // localstorage更新
    localStorage.setItem(webUser.No, JSON.stringify(usrLocal));
}

/**
 * サーバから一覧データを取得すること
 * @param {string} callBackMethod メソッド名
 */
function getListData(callBackMethod) {

    // 三つタグ一覧データの取得
    // 承認待ち/未完了/完了
    var type = GET_APPROVAL_INPROCESS + ';' + GET_APPROVAL_UNCOMPLETE + ';' + GET_APPROVAL_COMPLETE;

    //一覧データを取得
    wfCommon.GetDensinList(type, callBackMethod);  
}

/**
 * 承認待ちカード一覧画面の作成
 * @param {Array} jsondata サーバから取得のデータ配列
 */
function createCardListForInProgress(jsondata) {

    // カラムの設定
    var columns = {
        // 状態表示項目変数の指定
        // 変数のデータは下記の種類の通りです。
        //     "1":m-request-info--approval:承認
        //     "2":m-request-info--waiting:未対応
        //     "3":m-request-info--remand:差戻し
        //     "4":m-request-info--in-progress:承認待ち
        //     "5":m-request-info--draft:下書き
        //     "6":m-request-info--denial:否認
        card_icon: {
            icon_status: "2",
            //iconFormatter: function (res) {
            //},
            icon_title: "未承認",
            //titleFormatter: function (res) {
            //},

            badge_flag: "" // 赤点表示/非表示変数の定義（変数値はtrue:表示 false:非表示）
        },
        card_title: "FlowName",  // カードタイトル表示文字列の変数のID
        card_detail: [
            {
                field: 'OrderNumber',  // 物理名
                title: '申請番号',  // 論理名
            }, {
                field: 'RDT',
                title: '申請日',
                formatter: function (res) {
                    return dateFormatter(res.RDT);
                }
            }, {
                field: 'DeptName',
                title: '所属部署'
            }, {
                field: 'StarterName',
                title: '申請者'
            }
        ],
        card_action: [
            {
                // ボタン表示のクラス 
                // a-button--third：背景色がグレー
                // a-button--secondary：背景色が薄いピンク
                // a-button--primary：背景色が深いピンク
                // a-button--text：リンクの様式
                btn_class: "a-button--third",
                title: '否認',  // 論理名
                onClick: function (event, index, dataList, data) { // 成功

                    // ダイアログ画面を表示して、確定を押下してから、exeConfirmメソッドを実行すること。
                    wfCommon.ShowDialog(
                        DIALOG_INFO,
                        STRING_EMPTY,
                        wfCommon.MsgFormat(msg["W0001"], "否認します。," + data.OrderNumber),
                        "はい",
                        exeConfirm,
                        new Array(BTN_DENIAL, data, event),
                        "いいえ"
                    );
                }
            }, {
                // ボタン表示のクラス 
                // a-button--third：背景色がグレー
                // a-button--secondary：背景色が薄いピンク
                // a-button--primary：背景色が深いピンク
                // a-button--text：リンクの様式
                btn_class: "a-button--secondary",
                title: '差戻し',  // 論理名
                disabled: "btnNasiFlg", // ボタン使用可能制御 true: 使用不可 false: 使用可
                onClick: function (event, index, dataList, data) { // 成功

                    // ダイアログ画面を表示して、確定を押下してから、exeConfirmメソッドを実行すること。
                    wfCommon.ShowDialog(
                        DIALOG_INFO,
                        STRING_EMPTY,
                        wfCommon.MsgFormat(msg["W0001"], "差し戻します。," + data.OrderNumber),
                        "はい",
                        exeConfirm,
                        new Array(BTN_REMAND, data, event),
                        "いいえ"
                    );
                }
            }, {
                // ボタン表示のクラス 
                // a-button--third：背景色がグレー
                // a-button--secondary：背景色が薄いピンク
                // a-button--primary：背景色が深いピンク
                // a-button--text：リンクの様式
                btn_class: "a-button--primary",
                title: '承認',  // 論理名
                onClick: function (event, index, dataList, data) { // 成功

                    // ダイアログ画面を表示して、確定を押下してから、exeConfirmメソッドを実行すること。
                    wfCommon.ShowDialog(
                        DIALOG_CONFIRM,
                        STRING_EMPTY,
                        wfCommon.MsgFormat(msg["W0001"], "承認します。," + data.OrderNumber),
                        "はい",
                        exeConfirm,
                        new Array(BTN_APPROVAL, data, event),
                        "いいえ"
                    );
                }
            }
        ]
    };

    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        columns: columns, // カラム設定
        nextination: false, // 次へボタン機能あるか設定
        nextNumber: 1, // 初期化１ページ目を設定
        nextSize: 5, // １ページ表示件数を設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        tabFlg: GET_APPROVAL_INPROCESS, // タブ番号
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_APPROVAL_INPROCESS, FROM_PAGE_APPROVAL_REQUESTLIST);
        }
    };

    // 一覧の作成
    $("#in-progress-card-list").createCardList(optionData);
}

/**
 * 未完了カード一覧画面の作成
 * @param {Array} jsondata サーバから取得のデータ配列
 */
function createCardListForInComplete(jsondata) {

    // カラムの設定
    var columns = {
        // 状態表示項目変数の指定
        // 変数のデータは下記の種類の通りです。
        //     "1":m-request-info--approval:承認
        //     "2":m-request-info--waiting:未対応
        //     "3":m-request-info--remand:差戻し
        //     "4":m-request-info--in-progress:承認待ち
        //     "5":m-request-info--draft:下書き
        //     "6":m-request-info--denial:否認
        card_icon: {
            icon_status: "4",
            //iconFormatter: function (res) {
            //},
            icon_title: "承認待ち",
            //titleFormatter: function (res) {
            //},
            badge_flag: "" // 赤点表示/非表示変数の定義（変数値はtrue:表示 false:非表示）
        },
        card_title: "FlowName",  // カードタイトル表示文字列の変数のID
        card_detail: [
            {
                field: 'OrderNumber',  // 物理名
                title: '申請番号',  // 論理名
            }, {
                field: 'RDT',
                title: '申請日',
                formatter: function (res) {
                    return dateFormatter(res.RDT);
                }
            }, {
                field: 'NodeName',
                title: 'プロセス'
            }, {
                field: 'DeptName',
                title: '所属部署'
            }, {
                field: 'StarterName',
                title: '申請者'
            }, {
                field: 'TodoEmps',
                title: '承認者',
                formatter: function (res) {

                    return empFormatter(res.TodoEmps);
                }
            }
        ]
    };

    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        columns: columns, // カラム設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_APPROVAL_UNCOMPLETE, FROM_PAGE_APPROVAL_REQUESTLIST);

        }
    };

    // 一覧の作成
    $("#in-complete-card-list").createCardList(optionData);
}

/**
 * 完了カード一覧画面の作成
 * @param {Array} jsondata サーバから取得のデータ配列
 */
function createCardListForComplete(jsondata) {

    // カラムの設定
    var columns = {
        // 状態表示項目変数の指定
        // 変数のデータは下記の種類の通りです。
        //     "1":m-request-info--approval:承認
        //     "2":m-request-info--waiting:未対応
        //     "3":m-request-info--remand:差戻し
        //     "4":m-request-info--in-progress:承認待ち
        //     "5":m-request-info--draft:下書き
        //     "6":m-request-info--denial:否認
        card_icon: {
            // icon_status: "1",
            iconFormatter: function (res) {
                if (res.AtPara.split('@')[2].split('=')[1] == 0) {
                    return "1";
                }
                else {
                    return "6";
                }
            },
            // icon_title: "承認",
            titleFormatter: function (res) {
                if (res.AtPara.split('@')[2].split('=')[1] == 0) {
                    return "承認";
                }
                else {
                    return "否認";
                }
            },
            badge_flag: "" // 赤点表示/非表示変数の定義（変数値はtrue:表示 false:非表示）
        },
        card_title: "FlowName",  // カードタイトル表示文字列の変数のID
        card_detail: [
            {
                field: 'OrderNumber',  // 物理名
                title: '申請番号',  // 論理名
            }, {
                field: 'RDT',
                title: '申請日',
                formatter: function (res) {
                    return dateFormatter(res.RDT);
                }
            }, {
                field: 'SendDT',
                title: '完了日',
                formatter: function (res) {
                    return dateFormatter(res.SendDT);
                }
            }, {
                field: 'DeptName',
                title: '所属部署'
            }, {
                field: 'StarterName',
                title: '申請者'
            }, {
                field: 'Emps',
                title: '承認者',
                formatter: function (res) {

                    return overEmpFormatter(res.Emps);
                }
            }
        ]
    };

    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        columns: columns, // カラム設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        tabFlg: GET_APPROVAL_COMPLETE, // タブ番号
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_APPROVAL_COMPLETE, FROM_PAGE_APPROVAL_REQUESTLIST);

        }
    };

    // 一覧の作成
    $("#complete-card-list").createCardList(optionData);
}

/**
 * 絞り込み検索画面の作成
 * @param {Array} jsondata サーバから取得のデータ複数配列
 *                         jsondata[0] = 承認待ちのデータリスト
 *                         jsondata[1] = 未処理のデータリスト
 *                         jsondata[2] = 完了のデータリスト
 */
function createRefinedSearchContorl(jsondata) {

    // 絞り込み検索画面に引数を設定すること
    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        addSelectorForModal: ".o-whole__body", // 検索の画面をどちらのタグに追加する設定（※設定しなくて、又は、未設定の場合、デフォルト値は".o-whole__body-container"です）
        addSelectorForResult: "#page_title_area", // 検索結果表示画面をどちらのタグに追加する設定
        keyword_condition: 'FlowName',  // キーワードは検索の項目名
        app_date_condition: 'RDT',   // 申請日は検索の項目名
        showFlag: "showFlag", // 表示/非表示変数の設定（true:表示 false:非表示）
        onClickForSearchBtn: function (event, conditions, dataList) { // 検索ボタンクリック

            // 承認待ちタグの初期化表示
            createCardListForInProgress(dataList[0]);

            // 未完了タグの初期化表示
            createCardListForInComplete(dataList[1]);

            // 完了タグの初期化表示
            createCardListForComplete(dataList[2]);

        },
        onClickForClearBtn: function (event, conditions, dataList) { // クリアボタンクリック


            // 承認待ちタグの初期化表示
            createCardListForInProgress(dataList[0]);

            // 未完了タグの初期化表示
            createCardListForInComplete(dataList[1]);

            // 完了タグの初期化表示
            createCardListForComplete(dataList[2]);

        }
    };

    // 絞り込み検索画面の作成
    $("#search-bar-comp").createRefinedSearch(optionData);
}

/**
 * 絞り込み検索条件により、画面再表示
 * @param {string} exeTpye 更新用タイプ
 * @param {string} exeData 更新用データ
 * @param {any}    event   押下ボタン
 */
function exeConfirm(exeTpye, exeData, event) {

    // ダイアログ画面を閉じること
    $("#app-dialog-div").removeClass("o-modal--opened");

    // 否認/差し戻しの時に、コメントの設定
    var coment = STRING_EMPTY;
    if (exeTpye !== BTN_APPROVAL) {
        // コメントのデータの取得
        coment = $("#deny_content").val();

        // コメントの削除
        $("#deny_content").val(STRING_EMPTY);
    }

    // バッジの削除
    $(event).parent().prev().children(":first").children(":first").children(":first").children(":first").next().removeClass("a-status-info__badge");

    // localstorageのデータ設定
    delete usrLocal[GET_APPROVAL_INPROCESS][exeData.WorkID];
    var displayTmp = parseInt(usrLocal[GET_APPROVAL_INPROCESS]["display"]);
    displayTmp = displayTmp - 1;
    usrLocal[GET_APPROVAL_INPROCESS]["display"] = displayTmp;

    // バッジ0件の場合
    if (displayTmp == 0) {

        // 通知欄の削除
        $(event).parent().parent().parent().parent().prev().attr("hidden", "");

        // タブバッジの削除
        $("#a-tab-wait").removeClass("a-tab-nav-item__badge");
        if (usrLocal[GET_APPROVAL_COMPLETE]["display"] == 0 || usrLocal[GET_APPROVAL_COMPLETE]["display"] == undefined) {
            $(".a-nav-apply").removeClass("a-nav-icon__badge");
        }

    // 上記以外の場合
    } else {

        // 通知欄の更新
        $(event).parent().parent().parent().parent().prev().children(":first").children(":first").children(":first").html(displayTmp);
    }

    // localstorageの更新
    localStorage.setItem(webUser.No, JSON.stringify(usrLocal));

    // データ更新
    // 自分を見るの完了を取得
    var data = wfCommon.DoListButton(
        exeTpye,
        webUser.No,
        exeData.FK_Flow,
        exeData.WorkID,
        exeData.CurrNode,
        exeData.FID,
        exeData.TblName,
        exeData.FK_Node,
        coment
    );

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 絞り込み検索条件により、画面再表示
    getListData(screenRefreshShow);
}

/**
 * 絞り込み検索条件により、画面再表示(callback用)
 * @param {string} listData 更新用データ
 */
function screenRefreshShow(listData) {

    // 画面検索条件により、再検索すること
    var aftDataList = $(this).RefreshSearch({
        data: listData
    });

    // 承認待ちタグの再表示
    createCardListForInProgress(aftDataList[0]);

    // 未完了タグの再表示
    createCardListForInComplete(aftDataList[1]);

    // 完了タグの再表示
    createCardListForComplete(aftDataList[2]);
}

/**
 * 日付データ表示フォーマット用
 */
function dateFormatter(dateDt) {

    var result = STRING_EMPTY;
    if (dateDt) {
        result = moment(dateDt, DATE_FORMAT_MOMENT_PATTERN_1).format(DATE_FORMAT_MOMENT_PATTERN_1);
    }
    return result;
}

/**
 * 承認者データ表示フォーマット用
 */
function empFormatter(data) {

    var empsList = data.split(';');

    if (empsList.length === 0) {
        return data;
    }

    var emps = STRING_EMPTY;
    for (var i = 0; i < empsList.length - 1; i++) {
        emps += empsList[i].split(',')[1] + ' ';
    }
    return emps;
}

/**
 * 最終承認者データ表示フォーマット用
 */
function overEmpFormatter(data) {
    var empsList = data.split('@@');

    if (empsList.length === 2) {
        return empsList[1].split(',')[1];
    } else {
        return '-';
    }
}
/*▲関数定義エリア▲*/