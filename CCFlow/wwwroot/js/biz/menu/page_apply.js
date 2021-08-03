/*▼グローバル変数定義エリア▼*/
var applyMenuItems;                                  // 申請メニューの格納オブジェクト
var flowNoAll;                                       // 申請メニューの全No.

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

    // メニューの検索ボタン表示
    $("#search-bar-comp").show();

    // 下書き、履歴から作成検索ボタン非表示
    $("#search-button-dispaly").hide();

    // イベント定義
    createonevent();   

    // 申請メニュー
    getApplyMenuItems();
}

/**
 * サーバから一覧データを取得すること
 */
function getListData() {

    var type = GET_MY_DRAFT + ';' + GET_MY_COMPLETE;

    //一覧データを取得
    wfCommon.GetDensinList(type, setListData); 
}

/**
 * サーバから取得した一覧データを設定すること
 */
function setListData(listData) {

    // 下書きタグの初期化表示
    this.createCardListForInProgress(listData[0]);

    // 完了タグの初期化表示
    this.createCardListForComplete(listData[1]);

    // 絞り込み検索画面の作成
    this.createRefinedSearchContorl(listData);
}

/**
 * 下書き一覧画面の作成
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
            //icon_status: "5",
            iconFormatter: function (res) {
                if (res.WFState === WF_STATE_SINSEIZUMI) {
                    return "2";
                }
                else if (res.WFState === WF_STATE_DRAFT) {
                    return "5";
                }
            },
            //icon_title: "下書き",
            titleFormatter: function (res) {
                if (res.WFState === WF_STATE_SINSEIZUMI) {
                    return "引戻";
                }
                else if (res.WFState === WF_STATE_DRAFT) {
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
        nextination: false, // 次へボタン機能あるか設定
        nextNumber: 1, // 初期化１ページ目を設定
        nextSize: 5, // １ページ表示件数を設定
        showFlag: "showFlag", // カード表示/非表示変数の判断（true:表示 false:非表示）
        onRowClick: function (event, index, dataList, data) { // 行クリック

            // フローに遷移すること
            wfCommon.OpenFlowFrom(data.WorkID, GET_MY_DRAFT, FROM_PAGE_APPLYMENU);
        }
    };

    // 一覧の作成
    $("#in-progress-card-list").createCardList(optionData);
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
            icon_status: "1",
            //iconFormatter: function (res) {
            //    if (res.AtPara.split('@')[2].split('=')[1] == 0) {
            //        return "1";
            //    }
            //    else {
            //        return "6";
            //    }
            //},
            icon_title: "承認",
            //titleFormatter: function (res) {
            //    if (res.AtPara.split('@')[2].split('=')[1] == 0) {
            //        return "承認";
            //    }
            //    else {
            //        return "否認";
            //    }
            //},
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
        card_summary: true,  // サマリー項目表示するかフラグ設定
        card_action: [
            {
                // ボタン表示のクラス 
                // a-button--third：背景色がグレー
                // a-button--secondary：背景色が薄いピンク
                // a-button--primary：背景色が深いピンク
                // a-button--text：リンクの様式
                btn_class: "a-button--text",
                title: 'コピーして作成する',  // 論理名
                onClick: function (event, index, dataList, data) { // 成功

                    // 申請するの完了を取得
                    var newWorkId = wfCommon.DoListButton(BTN_RIREKICOPY, webUser.No, data.FK_Flow, data.WorkID, data.CurrNode, data.FID, data.TblName);

                    // フローに遷移すること
                    wfCommon.OpenFlowFrom(newWorkId, GET_APPROVAL_COMPLETE, FROM_PAGE_APPLYMENU);
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
            wfCommon.OpenFlowFrom(data.WorkID, GET_APPROVAL_COMPLETE, FROM_PAGE_APPLYMENU);

        }
    };

    // 一覧の作成
    $("#complete-card-list").createCardList(optionData);
}

/**
 * イベント定義メソッド
 */
function createonevent() {

    // 下書きクリック
    $(".nodisplayUncomplete").on("click", function () {
        // メニューの検索ボタン非表示
        $("#search-bar-comp").hide();

        // 下書き検索ボタン表示
        $("#search-button-dispaly").show();

        // 下書きの検索条件が表示
        $("#search-bar-comp-apply").RefinedSearchControlShow(true);
    });

    // 履歴から作成クリック
    $(".nodisplayComplete").on("click", function () {
        // メニューの検索ボタン非表示
        $("#search-bar-comp").hide();

        // 履歴から作成検索ボタン表示
        $("#search-button-dispaly").show();

        // 履歴から作成の検索条件が表示
        $("#search-bar-comp-apply").RefinedSearchControlShow(true);
    });

    // メニュークリック
    $(".display").on("click", function () {
        // メニューの検索ボタン表示
        $("#search-bar-comp").show();

        // 下書き、履歴から作成検索ボタン非表示
        $("#search-button-dispaly").hide();

        // 下書き、履歴から作成の検索条件が表示
        $("#search-bar-comp-apply").RefinedSearchControlShow(false);
    });

    // キーワード検索ボタン
    $("#menu-search-button").click(function () {
        $("#applyMenu").hide();
        $("#menu-search-result").html($("#applyMenu").html());
        $("#menu-search-result").show();
    });

    // キーワード検索入力
    $("#menu-search-input").keyup(function () {

        // 何も入力されない場合
        if ($(this).val().trim() === "") {
            $("#menu-search-result").html($("#applyMenu").html());
            return;
        }
        createApplyMenu("menu-search-result", getApplyMenuItemsByKeyword());
    });

    // キーワード検索閉じるボタン
    $("#menu-close-button").click(function () {
        $("#menu-search-result").hide();
        $("#applyMenu").show();
    });
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

            // 下書きタグの初期化表示
            createCardListForInProgress(dataList[0]);

            // 完了タグの初期化表示
            createCardListForComplete(dataList[1]);

        },
        onClickForClearBtn: function (event, conditions, dataList) { // クリアボタンクリック

            // 下書きタグの初期化表示
            createCardListForInProgress(dataList[0]);

            // 完了タグの初期化表示
            createCardListForComplete(dataList[1]);
        }
    };

    // 絞り込み検索画面の作成
    $("#search-bar-comp-apply").createRefinedSearch(optionData);
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
 * FLOW画面へ遷移する
 *
 * @param flowNo
 * @param title
 */
function WinOpenStartFlow(flowNo, title) {
    location.href = "../../../../WF/MyFlow.htm?FK_Flow=" + flowNo + "&AgentMode=0";
    return;
}

/**
 * 申請メニューを取得する
 */
function getApplyMenuItems() {

    // 申請メニュー取得
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddUrlData();
    handler.DoMethodSetString("Start_Init", function (data) {

        //例外処理
        if (data.indexOf('err@') == 0) {
            wfCommon.Msgbox(data);
            return;
        }

        // JSON対象に転換
        applyMenuItems = JSON.parse(data);

        // 表示用フローNo一覧
        flowNoAll = getFlowNoAll();

        // 申請メニューの生成
        createApplyMenu("applyMenu");
    });
}

/**
 * すべてワークフローのNOを取得する
 */
function getFlowNoAll() {

    // 申請メニュークラス
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_Applymenu");
    var data = handler.DoMethodReturnString("GetFlowNoAll");

    // 例外処理
    if (data.indexOf('err@') == 0) {
        return;
    }

    // JSONにキャスト
    data = JSON.parse(data);

    // 文字列の作成
    var flowNo = "";
    data.forEach(function (self, index) {
        if (index == 0) {
            flowNo += self["NO"];
        } else {
            flowNo += "," + self["NO"];
        }
    });

    return flowNo;
}

/**
 * キーワードで表示されるメニューを取得する
 */
function getApplyMenuItemsByKeyword() {

    // 申請メニュー取得
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_Applymenu");
    handler.AddPara("KeyWord", $("#menu-search-input").val().trim());
    var data = handler.DoMethodReturnString("GetApplyMenuItemsByKeyword");

    if (data.indexOf('err@') == 0) {
        wfCommon.Msgbox(data);
        return;
    }

    return JSON.parse(data);
}

/**
 * 申請メニューを生成する
 */
function createApplyMenu(tagId, keywords) {

    // タグ宣言
    var contentDiv = '<div class="a-nav-item a-nav-item--menu-item">';
    var contentAfirst = '<a href="';
    var contentAlast = '" class="a-nav-item__link">';
    var contentSpanfirst = '<span class="a-nav-item__label">';

    // 申請メニューの作成
    var content = "";
    applyMenuItems["Start"].forEach(function (start) {

        // 非表示の場合
        if (flowNoAll.indexOf(start["No"]) === -1) {
            return;
        }

        // キーワードで判定
        if (keywords) {
            var len = keywords.length;
            var keyFlg = true;
            for (var i = 0; i < len; i++) {
                if (keywords[i]["WF_KEY_VALUE"].indexOf(start["No"]) !== -1) {
                    keyFlg = false;
                    break;
                }
            }

            // 非表示の場合
            if (keyFlg) {
                return;
            }
        }

        // イベント
        var href = "javascript:WinOpenStartFlow('" + start["No"] + "','" + start["FK_FlowSortText"] + " - " + start["Name"] + "');";

        // メニュー
        content = content + contentDiv;
        content = content + contentAfirst;
        content = content + href;
        content = content + contentAlast;
        content = content + contentSpanfirst;
        content = content + start["Name"];
        content = content + '</span></a></div>';
    });

    // 申請メニューの表示
    $("#" + tagId).html(content);
}
/*▲関数定義エリア▲*/