/**
 * カードのような形の一覧画面の作成
 * 備考：イオンリテール（株）社仕様のみ実現された
 */

// 入り口
(function ($) {
    $.fn.extend({
        "createCardList": function (optionData) {

            // 一覧の作成
            createCardList(this.selector, optionData);

            // 画面一部再表示の時に、縦スクロールを表示しない対応
            $("body").removeAttr("style");
        }
    });

    // ▼▼▼ テーブル一覧定義 ▼▼▼
    const TBL_STRING_EMPTY = '';
    const TBL_STRING_HYPHEN = '-';
    const TBL_STRING_FULL_SPACE = '　';

    /**表示状態マッピング */
    // 状態表示項目変数の指定
    // 変数のデータは下記の種類の通りです。
    //     "1":m-request-info--approval:承認
    //     "2":m-request-info--waiting:未対応
    //     "3":m-request-info--remand:差戻し
    //     "4":m-request-info--in-progress:承認待ち
    //     "5":m-request-info--draft:下書き
    //     "6":m-request-info--denial:否認
    const iconSts =
    {
        // １：承認
        "1": "approval",
        // ２：未対応
        "2": "waiting",
        // ３：差戻し
        "3": "remand",
        // ４：承認待ち
        "4": "in-progress",
        // ５：下書き
        "5": "draft",
        // ６：否認
        "6": "denial"
    };

    // ステータス状態クラス
    const REQUEST_INFO_CLASS = "m-request-info--{0}";
    // ステータス情報クラス
    const STATUS_INFO_CLASS = "a-status-info--{0}";
    // アイコンクラス
    const ICON_CLASS = "a-icon--{0}";

    // サマリー項目表示最大数（上限7項目）
    var summaryMax = 7;

    // ▲▲▲ テーブル一覧定義 ▲▲▲

    /**
     * 一覧画面の作成
     * @param {string}} tableName 
     * @param {jsondata} optionData
     */
    function createCardList(tableName, optionData) {

        // カードリストテンプレートの読み込み
        var strHtml = getModalHtml();
        // htmlに一旦に追加すること
        $('body').append(strHtml);
        // 追加テンプレートを非表示に設定すること
        // $("#card-list-common-id").hide();

        // 既存一覧画面をすべてクリアすること
        $(tableName).find(".m-request-info").remove();

        // カード一覧の作成
        makeCardList(tableName, optionData);

        // イベントの追加
        createEventForCardList(tableName, optionData);

        // next機能があるかを判定すること
        if (isProperty(optionData.nextination) && optionData.nextination === true && optionData.data.length > 0) {

        }
    }

    /**
     * 外部カラムとデータの設定によりテーブル一覧を作成すること
     * @param {string} tableName アバンティタグ名
     * @param {jsondata} optionData 設定属性
     */
    function makeCardList(tableName, optionData) {

        // 設定のカラムにより、HTMLを作成すること。
        $.each(optionData.data, function (dataIndex, dataVal) {

            // サマリー対象の取得
            var summryObj = null;
            if (isProperty(dataVal["summry"])) {
                summryObj = JSON.parse(dataVal["summry"]);
            }

            // テンプレートからコピーすること
            var cpObject = $("#card-list-common-id").clone();

            // テンプレートIDの削除
            cpObject.removeAttr("id");

            // ▼▼▼ アイコン状態の設定 ▼▼▼
            // アイコン状態の設定
            var defaultVal = "2";
            var iconClass = iconSts[defaultVal];
            var iconTitle = "未対応";
            var badgeFlag = false;
            if (isProperty(optionData.columns.card_icon)) {

                // アイコンクラスの取得
                if (isProperty(optionData.columns.card_icon.iconFormatter)) {
                    iconClass = iconSts[optionData.columns.card_icon.iconFormatter(dataVal)];
                }
                else {
                    iconClass = iconSts[optionData.columns.card_icon.icon_status];
                }

                // アイコン文言の取得
                if (isProperty(optionData.columns.card_icon.titleFormatter)) {
                    iconTitle = optionData.columns.card_icon.titleFormatter(dataVal);
                }
                else {
                    iconTitle = optionData.columns.card_icon.icon_title;
                }

                // 赤点表示/非表示状態の取得
                //if (isProperty(optionData.columns.card_icon.badge_flag) && dataVal[optionData.columns.card_icon.badge_flag]) {
                //    badgeFlag = dataVal[optionData.columns.card_icon.badge_flag];
                //}

                // 7日以内の場合
                if (moment().diff(moment(dataVal.SendDT), 'days') <= 7) {

                    // バッジ表示要の場合
                    if ((optionData.tabFlg === GET_MY_DRAFT && checkAutoCreate(dataVal.FK_Flow)) ||
                        optionData.tabFlg === GET_APPROVAL_INPROCESS ||
                        optionData.tabFlg === GET_APPROVAL_COMPLETE ||
                        optionData.tabFlg === GET_MY_DIFFERENCE ||
                        optionData.tabFlg === GET_MY_COMPLETE) {

                        // バッジ状態の取得出来ない場合
                        if (usrLocal[optionData.tabFlg][dataVal.WorkID] == undefined) {

                            // バッジ表示
                            badgeFlag = true;

                            // localstorage更新
                            usrLocal[optionData.tabFlg][dataVal.WorkID] = "1";

                            // タブバッジの設定
                            if (usrLocal[optionData.tabFlg]["display"] == undefined) {
                                usrLocal[optionData.tabFlg]["display"] = 1;
                            } else {
                                var displayTmp = parseInt(usrLocal[optionData.tabFlg]["display"]);
                                displayTmp = displayTmp + 1;
                                usrLocal[optionData.tabFlg]["display"] = displayTmp;
                            }
                        } else if (usrLocal[optionData.tabFlg][dataVal.WorkID] == "1") {

                            // バッジ表示
                            badgeFlag = true;
                        }
                    }
                }
            }

            // ステータス情報クラスの設定
            cpObject.addClass(String.format(REQUEST_INFO_CLASS, iconClass));
            // ステータス情報クラスの設定
            cpObject.find(".a-status-info").addClass(String.format(STATUS_INFO_CLASS, iconClass));
            // アイコンクラスの設定
            cpObject.find(".a-icon").addClass(String.format(ICON_CLASS, iconClass));
            // アイコン文言の設定
            cpObject.find(".a-status-info__label").text(iconTitle);

            // 赤点表示/非表示の設定
            if (!badgeFlag) {
                cpObject.find(".a-status-info__badge").removeClass("a-status-info__badge");
            }

            // ▲▲▲ アイコン状態の設定 ▲▲▲

            // ▼▼▼ タイトル文言の設定 ▼▼▼
            if (isProperty(optionData.columns.card_title)) {

                // タイトルの取得
                var flowTitle = dataVal[optionData.columns.card_title];
                
                // 代理の文言の追加
                // 代理の場合、代理文言の追加
                if (summryObj != undefined && summryObj != null &&
                    isProperty(summryObj.AgentMode) && summryObj.AgentMode === "1") {
                    flowTitle += "(代理)";
                }

                // タイトルの設定
                cpObject.find(".m-request-info__detail-title").text(flowTitle);
            }
            else {
                cpObject.find(".m-request-info__detail-title").text(TBL_STRING_HYPHEN);
            }

            // ▲▲▲ タイトル文言の設定 ▲▲▲

            // ▼▼▼ 明細の設定 ▼▼▼
            // サマー項目表示数
            var iSummary = 0;
            if (isProperty(optionData.columns.card_detail)) {

                // 設定のカラムにより、HTMLを作成すること。
                $.each(optionData.columns.card_detail, function (colIndex, colVal) {

                    // 明細オブジェクトの作成
                    var detailObj = $("#request-detail-info-id").clone();
                    // テンプレートIDの削除
                    detailObj.removeAttr("id");

                    // データフォーマットは有無の判断
                    var showVal;
                    if (isProperty(colVal.field) && isProperty(dataVal[colVal.field])) {
                        if (isProperty(colVal.formatter)) {
                            showVal = colVal.formatter(dataVal);
                        } else {
                            showVal = dataVal[colVal.field];
                        }
                    }
                    else {
                        showVal = TBL_STRING_HYPHEN;
                    }

                    // タイトル文言の設定
                    detailObj.find(".a-request-info-item__label").text(colVal.title);
                    // タイトル文言の設定
                    detailObj.find(".a-request-info-item__value").text(showVal);

                    // 作成のオブジェクトを画面に追加すること
                    cpObject.find(".m-request-info__detail-list").append(detailObj);

                    // サマー項目表示数 + 1
                    iSummary++;
                });
            }

            // サマリー関数設定あるか判定
            if (isProperty(optionData.columns.card_summary) && optionData.columns.card_summary === true) {
                for (i = 0; i < summaryMax - iSummary; i++) {

                    // 明細オブジェクトの作成
                    var detailObj = $("#request-detail-info-id").clone();
                    // テンプレートIDの削除
                    detailObj.removeAttr("id");

                    if (summryObj != undefined && summryObj != null &&
                        summryObj.content != undefined && summryObj.content != null &&
                        summryObj.content[i] != undefined && summryObj.content[i] != null) {

                        // タイトル文言の設定
                        detailObj.find(".a-request-info-item__label").text(summryObj.content[i].name);
                        // タイトル文言の設定
                        detailObj.find(".a-request-info-item__value").text(summryObj.content[i].value);
                    } else {
                        // 「:」の設定の削除
                        detailObj.find(".a-request-info-item__separator").text(TBL_STRING_FULL_SPACE);
                    }

                    // 作成のオブジェクトを画面に追加すること
                    cpObject.find(".m-request-info__detail-list").append(detailObj);
                }
            }

            // テンプレートタグの削除
            cpObject.find("#request-detail-info-id").remove();

            // ▲▲▲ タイトル文言の設定 ▲▲▲


            // ▼▼▼ ボタンエリアの設定 ▼▼▼
            if (isProperty(optionData.columns.card_action)) {

                // リンクがあるか判断
                var linkFlag = false;

                // ボタンのクラスの追加
                // cpObject.addClass("m-request-info--buttons");

                // 設定のカラムにより、HTMLを作成すること。
                $.each(optionData.columns.card_action, function (colIndex, colVal) {

                    // ボタンオブジェクトの作成
                    var btnObj = $("#btn-action-id").clone();
                    // テンプレートIDの削除
                    btnObj.removeAttr("id");

                    if (colVal.btn_class === "a-button--text") {
                        linkFlag = true;
                    }
                    // ボタンクラスの設定
                    btnObj.addClass(colVal.btn_class);
                    // ボタン文言の設定
                    btnObj.find(".a-button__label").text(colVal.title);

                    // disabled指定の状態により、該当ボタン活性/非活性を設定すること
                    if (isProperty(colVal.disabled) && dataVal[colVal.disabled] === true) {
                        btnObj.prop('disabled', true);
                    }

                    // 作成のオブジェクトを画面に追加すること
                    cpObject.find(".m-button-container").append(btnObj);
                });

                if (linkFlag) {
                    cpObject.addClass("m-request-info--with-duplicate");
                }

                // テンプレートタグの削除
                cpObject.find("#btn-action-id").remove();
            }
            else {
                cpObject.find(".m-button-container").remove();
            }

            // ▲▲▲ ボタンエリアの設定 ▲▲▲


            // 非常/非表示の制御
            if (isProperty(optionData.showFlag) && !dataVal[optionData.showFlag]) {

                // showFlagのデータはfalseの場合、非表示になります
                cpObject.hide();
            }

            $(tableName).append(cpObject);

        });

        // テンプレートカードを削除すること
        $("#card-list-common-id").remove();
    }

    /**
     * 文字列フォーマット用
     * var msg = "he{0}o,{1}"
     * var arrPar = ["ll", "world"]
     * String.format(msg, arrPar) // hello,world
     */
    String.format = function () {
        // msgの取得
        var s = arguments[0];
        for (var i = 0; i < arguments.length - 1; i++) {
            var arr = arguments[i + 1];

            if ((typeof arr == 'object') && arr.constructor == Array) {
                for (var j = 0; j < arr.length; j++) {
                    // replace用
                    var reg = new RegExp("\\{" + j + "\\}", "gm");
                    s = s.replace(reg, arr[j]);
                }
            } else {
                // replace用
                var reg = new RegExp("\\{0\\}", "gm");
                s = s.replace(reg, arr);
            }
        }

        return s;
    }

    /**
     * 一覧イベントの追加
     * @param {string}} tableName セレクター
     * @param {jsonData}} optionData 設置属性
     */
    function createEventForCardList(tableName, optionData) {

        // ボタンイベントの作成
        $(tableName).find(".m-request-info").each(function (index, element) {

            $(this).find(".a-button").each(function (actIndex, actElement) {
                $(this).click(function () {

                    var fn = optionData.columns.card_action[actIndex].onClick;
                    if (fn) {

                        // 該当データのインデックス,データリスト、該当データ
                        fn(this, index, optionData.data, optionData.data[index]);
                    }
                });
            });
        });

        //  行クリック属性あるか判断
        if (isProperty(optionData.onRowClick)) {

            // 行の取得
            $(tableName).find(".m-request-info").each(function (rowIndex, rowElement) {

                // 列の取得
                $(this).find(".m-request-info__content").each(function (colIndex, colElement) {

                    // クリックイベントの設定
                    $(this).click(function (event) {

                        // バッジ削除対象の場合
                        if (usrLocal[optionData.tabFlg] != undefined && usrLocal[optionData.tabFlg][optionData.data[rowIndex].WorkID] == "1") {

                            // カードバッジ削除
                            $(this).children(":first").children(":first").children(":first").children(":first").next().removeClass("a-status-info__badge");

                            // localstorageのデータ設定
                            usrLocal[optionData.tabFlg][optionData.data[rowIndex].WorkID] = "0";
                            var displayTmp = parseInt(usrLocal[optionData.tabFlg]["display"]);
                            displayTmp = displayTmp - 1;
                            usrLocal[optionData.tabFlg]["display"] = displayTmp;

                            // バッジ0件の場合
                            if (displayTmp == 0) {

                                // 通知欄の削除
                                $(this).parent().parent().parent().prev().attr("hidden", "");

                                // タブバッジの削除
                                switch (optionData.tabFlg) {
                                    case GET_MY_DRAFT:
                                        $("#a-tab-draft").removeClass("a-tab-nav-item__badge");
                                        if ((usrLocal[GET_MY_COMPLETE]["display"] == 0 || usrLocal[GET_MY_COMPLETE]["display"] == undefined) &&
                                            (usrLocal[GET_MY_DIFFERENCE]["display"] == 0 || usrLocal[GET_MY_DIFFERENCE]["display"] == undefined)) {
                                            $(".a-nav-self").removeClass("a-nav-icon__badge");
                                        }
                                        break;
                                    case GET_MY_DIFFERENCE:
                                        $("#a-tab-diff").removeClass("a-tab-nav-item__badge");
                                        if ((usrLocal[GET_MY_COMPLETE]["display"] == 0 || usrLocal[GET_MY_COMPLETE]["display"] == undefined) &&
                                            (usrLocal[GET_MY_DRAFT]["display"] == 0 || usrLocal[GET_MY_DRAFT]["display"] == undefined)) {
                                            $(".a-nav-self").removeClass("a-nav-icon__badge");
                                        }
                                        break;
                                    case GET_MY_COMPLETE:
                                        $("#a-tab-done").removeClass("a-tab-nav-item__badge");
                                        if ((usrLocal[GET_MY_DIFFERENCE]["display"] == 0 || usrLocal[GET_MY_DIFFERENCE]["display"] == undefined) &&
                                            (usrLocal[GET_MY_DRAFT]["display"] == 0 || usrLocal[GET_MY_DRAFT]["display"] == undefined)) {
                                            $(".a-nav-self").removeClass("a-nav-icon__badge");
                                        }
                                        break;
                                    case GET_APPROVAL_INPROCESS:
                                        $("#a-tab-wait").removeClass("a-tab-nav-item__badge");
                                        if (usrLocal[GET_APPROVAL_COMPLETE]["display"] == 0 || usrLocal[GET_APPROVAL_COMPLETE]["display"] == undefined) {
                                            $(".a-nav-apply").removeClass("a-nav-icon__badge");
                                        }
                                        break;
                                    case GET_APPROVAL_COMPLETE:
                                        $("#a-tab-done").removeClass("a-tab-nav-item__badge");
                                        if (usrLocal[GET_APPROVAL_INPROCESS]["display"] == 0 || usrLocal[GET_APPROVAL_INPROCESS]["display"] == undefined) {
                                            $(".a-nav-apply").removeClass("a-nav-icon__badge");
                                        }
                                        break;
                                }

                                // 上記以外の場合
                            } else {

                                // 通知欄の更新
                                $(this).parent().parent().parent().prev().children(":first").children(":first").children(":first").html(displayTmp);
                            }

                            // localstorageの更新
                            localStorage.setItem(webUser.No, JSON.stringify(usrLocal));
                        }

                        // 各タブのメソッド
                        optionData.onRowClick(this, rowIndex, optionData.data, optionData.data[rowIndex]);
                    });
                });
            });
        }
    }

    /**
     * カード一覧画面画面を取得すること
     * @returns 取得のhtml
     */
    function getModalHtml() {

        // html内容のお読み込み
        var ajaxData = $.ajax({
            type: 'get',
            url: '../../../pages/common/card_list_for_menu_common.html',
            cache: false,
            async: false,
            dataType: 'html',
            success: function (data) {
                return data;
            }
        });

        // html読み込みデータを変数に設定すること
        return ajaxData.responseText;
    }

    /**
     * 属性存在するか、設定するか判断
     * @param {object} property 判断のプロパティ
     * @returns true : 設定有り  false : 設定無し
     */
    function isProperty(property) {

        // 属性が未定義、null、空白を設定すれば、falseを戻り
        if (property == undefined || property == null || property == TBL_STRING_EMPTY) {

            // 設定無し
            return false;
        }
        else {

            // 設定有り
            return true;
        }
    }
    /*▲関数定義エリア▲*/

})(jQuery);