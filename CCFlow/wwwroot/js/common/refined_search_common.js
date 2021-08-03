/**
 * 絞り込み検索共通の作成
 * 備考：イオンリテール（株）社仕様のみ実現された
 */

// 入り口
(function ($) {

    // ▼▼▼ テーブル一覧定義 ▼▼▼
    const TBL_STRING_EMPTY = '';
    const TBL_HALF_SPACE = ' ';
    const TBL_FULL_SPACE = '　';

    // 絞り込み検索ボタンの定義
    const REFINED_SEARCH_BUTTON =
        '<button id="refined_search_common_btn" type="button" class="a-search-button a-search-button--dark">' +
        '<i class="a-icon a-icon--search-white"></i>' +
        '<span class="a-search-button__label">絞り込み検索</span>' +
        '</button>';

    //// オプションクラス
    //const PAGE_OPTIONAL_CLASS = "a-pagination__page-item--optional";
    //// 非活性クラス
    //const PAGE_ACTION_DISABLED_CLASS = "a-pagination__action--disabled";
    //// 省略記号クラス
    //const PAGE_ELLIPSIS_CLASS = "a-pagination__page-item--ellipsis";

    // 共通関連
    // var common = new wfCommon();

    // 検索条件
    var conditions = [];

    // 設定オプション属性
    var optionData;

    // ▲▲▲ テーブル一覧定義 ▲▲▲

    $.fn.extend({
        
        "createRefinedSearch": function (option) {

            //  絞り込み検索の作成
            // mkRefinedSearch = new createRefinedSearch();
            // mkRefinedSearch.createRefinedSearch(this.selector, optionData);
            optionData = option;
            createRefinedSearch(this.selector);
        },

        "RefreshSearch": function (option) {

            // データのみを設定すること
            optionData.data = option.data;
            //  絞り込み検索の作成
            getSearchedData();

            return optionData.data;
        },

        "RefinedSearchControlShow": function (showFlag) {

            // データのみを設定すること
            if (showFlag === true) {
                $(this.selector).show();
            }
            else {
                $(this.selector).hide();
            }

            $(optionData.addSelectorForResult).SearchCurrentFiltersControlShow(showFlag);
        }
    });

    /**
     * 一覧画面の作成
     * @param {string} tableName 
     */
    function createRefinedSearch(tableName) {

        // 既存コントロールの削除
        $(tableName).find("#refined_search_common_btn").remove();
        $("#refined_search_common_area").remove();
        $(optionData.addSelectorForResult).Clear();

        // 絞り込み検索ボタンを画面に追加すること
        $(tableName).append(REFINED_SEARCH_BUTTON);

        // 絞り込み検索画面を取得すること
        var dialogHtml = getSearchModalHtml();
        // ダイアログ画面を画面に追加すること
        if (isProperty(optionData.addSelectorForModal)) {
            $(optionData.addSelectorForModal).append(dialogHtml);
        }
        else {
            $(".o-whole__body-container").append(dialogHtml);
        }
        
        //
        createEvent();

        // 日時項目を設定する
        var common;
        if (typeof (wfCommon) === "function") {
            common = new wfCommon();
        }
        else {
            common = wfCommon;
        }

        common.setdatepickerWithStartEnd(
            "#refined_search_commmon_app_date_from",
            "#refined_search_commmon_app_date_to",
            DATE_FORMAT_MOMENT_PATTERN_1,
            true,
            false,
            function () { },
            function () { });
    }

    /**
     * 絞り込み検索画面を取得すること
     * @returns 取得のhtml
     */
    function getSearchModalHtml() {

        // html内容のお読み込み
        var ajaxData = $.ajax({
            type: 'get',
            url: '../../../pages/common/refined_search_common_area.html',
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
     * イベントの追加
     * @param {int} curNum 1ページには表示の数
     * @param {int} len 総ページ数
     */
    function createEvent() {

        // 絞り込み検索ボタン押下イベント
        $("#refined_search_common_btn").click(function () {

            // ダイアログ画面を表示すること
            $("#refined_search_common_area").addClass("o-modal--opened");
        });

        // 絞り込み検索画面の×ボタン押下イベント
        $("#refined_search_common_close_btn").click(function () {

            // ダイアログ画面を表示すること
            $("#refined_search_common_area").removeClass("o-modal--opened");
        });


        // 絞り込み検索画面の検索ボタン押下イベント
        $("#refined_search_common_search_btn").click(function () {

            // 検索条件の取得
            getConditions();

            // 検索結果の取得
            getSearchedData();

            // 検索結果エリア表示コントロールの設定
            createSearchShowArea();

            // ダイアログ画面を閉じること
            $("#refined_search_common_area").removeClass("o-modal--opened");

            // クリックイベントの呼び出す
            if (isProperty(optionData.onClickForSearchBtn)) {
                optionData.onClickForSearchBtn(this, conditions, optionData.data);
            }
        });
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

    /**
     * 検索条件の取得
     * 検索条件を全域変数（conditions）に設定すること。
     */
    function getConditions() {

        conditions = [];

        // キーワード
        conditions[0] = { id: "refined_search_common_search_text", title: "キーワード", val: $("#refined_search_common_search_text").val() };

        // 申請日from
        conditions[1] = { id: "refined_search_commmon_app_date_from", title: "申請日（始）", val: $("#refined_search_commmon_app_date_from").val() };

        // 申請日to
        conditions[2] = { id: "refined_search_commmon_app_date_to", title: "申請日（終）", val: $("#refined_search_commmon_app_date_to").val() };
    }

    /**
     * キーワード検索データを取得すること
     * @param {string} selector 検索データ
     * @returns キーワード検索データ
     */
    function getKeyword(selector) {

        var word = [];
        if ($(selector).val().trim() != TBL_STRING_EMPTY) {
            
            word = $(selector).val().replaceAll(TBL_FULL_SPACE, TBL_HALF_SPACE).trim().replace(/\s+/g, TBL_HALF_SPACE).split(TBL_HALF_SPACE);
            return word;
        }
        else {
            return word;
        }
    }

    /**
     * 検索後データの取得
     * @returns 検索後データ
     */
    function getSearchedData() {

        // データリストをコピーすること
        // var aftDataList = $.extend(true, [], optionData.data);

        // 検索条件の取得
        // キーワードの取得
        var keyWord = getKeyword("#refined_search_common_search_text");

        // 表示/非表示変数の取得
        var showFlag;
        if (isProperty(optionData.showFlag)) {
            showFlag = optionData.showFlag;
        }
        else {
            showFlag = "showFlag";
        }

        // 設定のカラムにより、HTMLを作成すること。
        $.each(optionData.data, function (typeIndex, typeVal) {

            // タグのごとに絞り込みを行います
            $.each(typeVal, function (rowIndex, rowVal) {

                // キーワード検索結果の取得
                var isKeyword = compKeyword(keyWord, rowVal[optionData.keyword_condition]);

                // 申請日検索結果の取得
                var isAppDate = compAppdate(
                    $("#refined_search_commmon_app_date_from").val(),
                    $("#refined_search_commmon_app_date_to").val(),
                    rowVal[optionData.app_date_condition]);

                // showフラグの設定

                if (isKeyword && isAppDate) {
                    rowVal[showFlag] = true;
                } else {
                    rowVal[showFlag] = false;
                }
            });
        });

        // return aftDataList;
    }

    /**
     * キーワード検索データを取得すること
     * @param {Array} condiArr 検索条件
     * @param {string} compData 検索対象
     * @returns true : 検索結果有り  false : 検索結果無し
     */
    function compKeyword(condiArr, compData) {

        // 検索条件がない場合、該当条件は無しになります
        if (condiArr.length === 0) {
            return true;
        }
        else {

            // 検索条件は検索対象中に存在すれば、trueを戻ること
            // return condiArr.some(value => compData.toString().indexOf(value) >= 0) ? true : false;  // IEをサポートするため、下記のほうほうを変わること
            return condiArr.some(function (element, index, array) {
                return compData.toString().indexOf(element) >= 0;
            }) ? true : false;
        }
    }

    /**
     * 申請日検索データを取得すること
     * @param {Array} appDateFrom 検索条件 申請日始
     * @param {Array} appDateTo 検索条件 申請日終
     * @param {string} compData 検索対象
     * @returns true : 検索結果有り  false : 検索結果無し
     */
    function compAppdate(appDateFrom, appDateTo, compData) {

        // 申請日始と申請日終は両方が空白の場合
        if ((appDateFrom === TBL_STRING_EMPTY && appDateTo === TBL_STRING_EMPTY) ||
            compData === TBL_STRING_EMPTY) {
            return true;
        }
        else if (appDateFrom != TBL_STRING_EMPTY && appDateTo === TBL_STRING_EMPTY) {

            // 申請日始＜＝場合
            // return (appDateFrom <= compData) ? true : false;
            return moment(compData, DATE_FORMAT_MOMENT_PATTERN_1).isSameOrAfter(moment(appDateFrom, DATE_FORMAT_MOMENT_PATTERN_1)) ? true : false;
        }
        else if (appDateFrom === TBL_STRING_EMPTY && appDateTo != TBL_STRING_EMPTY) {

            // 申請日終＞＝場合
            // return (compData <= appDateTo) ? true : false;
            return moment(appDateTo, DATE_FORMAT_MOMENT_PATTERN_1).isSameOrAfter(moment(compData, DATE_FORMAT_MOMENT_PATTERN_1)) ? true : false;
        }
        else {
            // return ((appDateFrom <= compData) && (compData <= appDateTo)) ? true : false;
            return ((
                moment(compData, DATE_FORMAT_MOMENT_PATTERN_1).isSameOrAfter(moment(appDateFrom, DATE_FORMAT_MOMENT_PATTERN_1))
            ) && (
                moment(appDateTo, DATE_FORMAT_MOMENT_PATTERN_1).isSameOrAfter(moment(compData, DATE_FORMAT_MOMENT_PATTERN_1)))) ? true : false;
        }
    }

    /**
     * 検索条件表示エリアを生成する
     * 
     * @param {Array} conditionDatas 検索条件
     * @param {string} appDate 申請日期間
     */
    function createSearchShowArea() {

        // 検索条件の取得
        // キーワードの取得
        var keyWord = getKeyword("#refined_search_common_search_text");

        var showAppDate = TBL_STRING_EMPTY;
        if ($("#refined_search_commmon_app_date_from").val() != TBL_STRING_EMPTY && $("#refined_search_commmon_app_date_to").val() != TBL_STRING_EMPTY) {
            showAppDate = $("#refined_search_commmon_app_date_from").val() + "～" + $("#refined_search_commmon_app_date_to").val();
        }
        else if ($("#refined_search_commmon_app_date_from").val() != TBL_STRING_EMPTY && $("#refined_search_commmon_app_date_to").val() === TBL_STRING_EMPTY) {
            showAppDate = $("#refined_search_commmon_app_date_from").val() + "～";
        }
        else if ($("#refined_search_commmon_app_date_from").val() === TBL_STRING_EMPTY && $("#refined_search_commmon_app_date_to").val() != TBL_STRING_EMPTY) {
            showAppDate = "～" + $("#refined_search_commmon_app_date_to").val();
        }

        var searchOptionData = {
            data: keyWord,  // 検索条件入力のデータ
            sub_class: "a-tag a-tag--filled", // 1:"m-search-current-filters__tags" 2:"m-search-current-filters__wrap"  3:"m-search-current-filters__date" 4:"a-tag a-tag--filled"
            half_space: 0,  // 条件の間では半角空白でフォーマットを調整するため、半角空白数の設定
            app_date_show: showAppDate,  // 絞り込み検索画面のみに適用すること（申請日の区間の設定）文字列
            onClickForClear: function () {

                // 条件クリア
                $("#refined_search_common_search_text").val(TBL_STRING_EMPTY);
                $("#refined_search_commmon_app_date_from").val(TBL_STRING_EMPTY);
                $("#refined_search_commmon_app_date_to").val(TBL_STRING_EMPTY);

                // 検索条件の取得
                getConditions();

                // 検索結果の取得
                getSearchedData();

                // クリックイベントの呼び出す
                if (isProperty(optionData.onClickForClearBtn)) {
                    optionData.onClickForClearBtn(this, conditions, optionData.data);
                }
            }
        };

        // 検索条件表示エリアの作成
        $(optionData.addSelectorForResult).createSearchCurrentFilters(searchOptionData);
    }

    /*▲関数定義エリア▲*/

})(jQuery);
