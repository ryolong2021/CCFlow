/*▼グローバル変数定義エリア▼*/
var webUser = null;
//…追加変数

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {
    if (webUser === null)
        webUser = new WebUser();
    if (webUser.No === null)
        return;
    //画面初期化
    InitPage();
    //…追加関数			

});
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/
/**
 * 画面初期化
 */
function InitPage() {

    // 各タグ一覧データの取得
    this.getListData();
 
}
window.onload = function () {
    $(".m-tabs").attr("style", "--width:98px; --active-index:1");
    $(".m-tabs__content")[1].classList.add("m-tabs__content--active");
    $(".m-tabs__content")[0].classList.remove("m-tabs__content--active");
    $(".m-tabs__tab")[1].classList.add("m-tabs__tab--active");
    $(".m-tabs__tab")[0].classList.remove("m-tabs__tab--active");
    $(".a-tab-nav-item")[1].classList.add("a-tab-nav-item--active");
    $(".a-tab-nav-item")[0].classList.remove("a-tab-nav-item--active");}

/**
 * サーバから一覧データを取得すること
 */
function getListData() {

    var type = GET_MY_DRAFT + ';' + GET_MY_UNCOMPLETE + ';' + GET_MY_DIFFERENCE + ';' + GET_MY_COMPLETE;

    //一覧データを取得
    wfCommon.GetDensinList(type, setListData);  
}

/**
 * サーバから取得した一覧データを設定すること
 */
function setListData(listData) {

    // 下書きタグの初期化表示
    this.createCardListForDraft(listData[0]);

    // 未完了タグの初期化表示
    this.createCardListForUnComplete(listData[1]);

    // 差戻タグの初期化表示
    this.createCardListForDifference(listData[2]);

    // 完了タグの初期化表示
    this.createCardListForComplete(listData[3]);

    // 絞り込み検索画面の作成
    this.createRefinedSearchContorl(listData);

    // すべて確認済にする(差戻)
    $("#a-draft").click(function () {
        clearAllBadges(GET_MY_DRAFT, "#in-darft-card-list");
    });

    // すべて確認済にする(差戻)
    $("#a-diff").click(function () {
        clearAllBadges(GET_MY_DIFFERENCE, "#in-difference-card-list");
    });
    // すべて確認済にする(完了)
    $("#a-done").click(function () {
        clearAllBadges(GET_MY_COMPLETE, "#in-complete-card-list");
    });

    // 下書きバッジの設定
    if (usrLocal[GET_MY_DRAFT]["display"] != undefined && usrLocal[GET_MY_DRAFT]["display"] != 0) {
        $("#alert-draft").children(":first").children(":first").children(":first").html(usrLocal[GET_MY_DRAFT]["display"]);
        $("#alert-draft").removeAttr("hidden");
        $("#a-tab-draft").addClass("a-tab-nav-item__badge");
    }

    // 差戻バッジの設定
    if (usrLocal[GET_MY_DIFFERENCE]["display"] != undefined && usrLocal[GET_MY_DIFFERENCE]["display"] != 0) {
        $("#alert-diff").children(":first").children(":first").children(":first").html(usrLocal[GET_MY_DIFFERENCE]["display"]);
        $("#alert-diff").removeAttr("hidden");
        $("#a-tab-diff").addClass("a-tab-nav-item__badge");
    }

    // 完了バッジの設定
    if (usrLocal[GET_MY_COMPLETE]["display"] != undefined && usrLocal[GET_MY_COMPLETE]["display"] != 0) {
        $("#alert-done").children(":first").children(":first").children(":first").html(usrLocal[GET_MY_COMPLETE]["display"]);
        $("#alert-done").removeAttr("hidden");
        $("#a-tab-done").addClass("a-tab-nav-item__badge");
    }

    // localstorage更新
    localStorage.setItem(webUser.No, JSON.stringify(usrLocal));
}

/**
 * 下書きカード一覧画面の作成
 * @param {Array} jsondata サーバから取得のデータ配列
 */
function createCardListForDraft(jsondata) {

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
            iconFormatter: function (res) {
                if (res.WFState === WF_STATE_SINSEIZUMI) {
                    return "2";
                }
                else {
                    return "5";
                }
            },
            // icon_title: "承認",
            titleFormatter: function (res) {
                if (res.WFState === WF_STATE_SINSEIZUMI) {
                    return "引戻";
                }
                else {
                    return "下書き";
                }
            },
            badge_flag: "" // 赤点表示/非表示変数の定義（変数値はtrue:表示 false:非表示）
        },
        card_title: "FlowName",  // カードタイトル表示文字列の変数のID
        card_detail: [
            {
                field: 'RDT',
                title: '申請日',
                formatter: function (res) {

                    // 一時保存の時に、申請日が「-」で表示して、以外の場合、日付を表示すること
                    if (res.WFState > 1) {
                        return dateFormatter(res.RDT);
                    } else {
                        return "-";
                    }
                }
            }
        ],
        card_summary: true  // サマリー項目表示するかフラグ設定
    };

    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        columns: columns, // カラム設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        tabFlg: GET_MY_DRAFT, // タブ番号
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_MY_DRAFT, FROM_PAGE_REQUESTLIST);
        }
    };

    // 一覧の作成
    $("#in-darft-card-list").createCardList(optionData);
}

/**
 * 未完了カード一覧画面の作成
 * @param {Array} jsondata サーバから取得のデータ配列
 */
function createCardListForUnComplete(jsondata) {

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
                title: '伝票番号',  // 論理名
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
                field: 'TodoEmps',
                title: '承認者',
                formatter: function (res) {

                    return empFormatter(res.TodoEmps);
                }
            }
        ],
        card_action: [
            {
                // ボタン表示のクラス 
                // a-button--third：背景色がグレー
                // a-button--secondary：背景色が薄いピンク
                // a-button--primary：背景色が深いピンク
                // a-button--text：リンクの様式
                btn_class: "a-button--secondary cancel-botton-width",
                title: '引戻',  // 論理名
                disabled: "btnNasiFlg", // ボタン使用可能制御 true: 使用不可 false: 使用可
                onClick: function (event, index, dataList, data) { // 成功

                    // ダイアログ画面を表示して、確定を押下してから、exeConfirmメソッドを実行すること。
                    wfCommon.ShowDialog(
                        DIALOG_CONFIRM,
                        STRING_EMPTY,
                        wfCommon.MsgFormat(msg["W0001"], "引戻します。," + data.OrderNumber),
                        "はい",
                        backok,
                        new Array(data),
                        "いいえ"
                    );
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
            wfCommon.OpenFlowFrom(data.WorkID, GET_MY_UNCOMPLETE, FROM_PAGE_REQUESTLIST);

        }
    };

    // 一覧の作成
    $("#in-uncomplete-card-list").createCardList(optionData);
}

/**
 * 差戻カード一覧画面の作成
 * @param {Array} jsondata サーバから取得のデータ配列
 */
function createCardListForDifference(jsondata) {

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
            icon_status: "3",
            //iconFormatter: function (res) {
            //},
            icon_title: "差戻",
            //titleFormatter: function (res) {
            //},
            badge_flag: "" // 赤点表示/非表示変数の定義（変数値はtrue:表示 false:非表示）
        },
        card_title: "FlowName",  // カードタイトル表示文字列の変数のID
        card_detail: [
            {
                field: 'OrderNumber',  // 物理名
                title: '伝票番号',  // 論理名
            }, {
                field: 'RDT',
                title: '申請日',
                formatter: function (res) {
                    return dateFormatter(res.RDT);
                }
            }, {
                field: 'ADT',
                title: '差戻日',
                formatter: function (res) {
                    return dateFormatter(res.ADT);
                }
            }, {
                field: 'Sender',
                title: '承認者',
                formatter: function (res) {

                    return differenceEmpFormatter(res.Sender);
                }
            }
        ]
    };

    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        columns: columns, // カラム設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        tabFlg: GET_MY_DIFFERENCE, // タブ番号
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_MY_DIFFERENCE, FROM_PAGE_REQUESTLIST);
        }
    };

    // 一覧の作成
    $("#in-difference-card-list").createCardList(optionData);
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
                field: 'RDT',
                title: '申請日',
                formatter: function (res) {
                    return dateFormatter(res.RDT);
                }
            }
        ],
        card_summary: true  // サマリー項目表示するかフラグ設定
    };

    var optionData = {
        data: jsondata,  // サーバから取得のデータ
        columns: columns, // カラム設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        tabFlg: GET_MY_COMPLETE, // タブ番号
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_MY_COMPLETE, FROM_PAGE_REQUESTLIST);
        }
    };

    // 一覧の作成
    $("#in-complete-card-list").createCardList(optionData);

}

/**
 * 絞り込み検索画面の作成
 * @param {Array} jsondata サーバから取得のデータ複数配列
 *                         jsondata[0] = 下書きのデータリスト
 *                         jsondata[1] = 未完了のデータリスト
 *                         jsondata[2] = 差戻のデータリスト
 *                         jsondata[3] = 完了のデータリスト
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

            // 下書きタグの初期化表示
            createCardListForDraft(dataList[0]);

            // 未完了タグの初期化表示
            createCardListForUnComplete(dataList[1]);

            // 差戻タグの初期化表示
            createCardListForDifference(dataList[2]);

            // 完了タグの初期化表示
            createCardListForComplete(dataList[3]);

        },
        onClickForClearBtn: function (event, conditions, dataList) { // クリアボタンクリック

            // 下書きタグの初期化表示
            createCardListForDraft(dataList[0]);

            // 未完了タグの初期化表示
            createCardListForUnComplete(dataList[1]);

            // 差戻タグの初期化表示
            createCardListForDifference(dataList[2]);

            // 完了タグの初期化表示
            createCardListForComplete(dataList[3]);
        }
    };

    // 絞り込み検索画面の作成
    $("#search-bar-comp").createRefinedSearch(optionData);
}

/**
 * ポップアップのOKボタン押下メソッド
 */
function backok(data) {
    //alert(workid);

    // 自分を見るの完了を取得
    wfCommon.DoListButton(BTN_REFUND, webUser.No, data.FK_Flow, data.WorkID, data.FID, data.CurrNode, data.TblName);

    //フローへ遷移する
    wfCommon.OpenFlowFrom(data.WorkID, GET_MY_DRAFT, FROM_PAGE_REQUESTLIST)
    return true;
}

/**
 * 日付データ表示フォーマット用
 */
function dateFormatter(dateDt) {

    var result = STRING_EMPTY;
    if (dateDt) {
        result = moment(dateDt).format(DATE_FORMAT_MOMENT_PATTERN_1);
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
/**
 * 差戻時、承認者データ表示フォーマット用
 */
function differenceEmpFormatter(data) {
    var empsList = data.split(',');
    
    return empsList[empsList.length - 1];
}
/*▲関数定義エリア▲*/