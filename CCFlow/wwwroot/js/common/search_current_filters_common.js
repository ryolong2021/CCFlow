/**
 * 検索条件表示エリアの作成
 * 備考：イオンリテール（株）社仕様のみ実現された
 */

// 入り口
(function ($) {
    $.fn.extend({
        "createSearchCurrentFilters": function (optionData) {

            // 検索条件表示エリアの作成
            var obj = new createSearchCurrentFilters();
            obj.createSearchCurrentFilters(this.selector, optionData);
        },

        "Clear": function () {

            // 検索条件表示エリアの初期化（クリア）
            $(this.selector).find("#search-current-filters-area").remove();
        },

        "SearchCurrentFiltersControlShow": function (showFlag) {

            // 該当タグ存在の場合
            if ($("#search-current-filters-area").length) {

                // showの状態により、コントロール非常/非表示を制御すること
                if (showFlag) {
                    $("#search-current-filters-area").show();
                }
                else {
                    $("#search-current-filters-area").hide();
                }
            }
        }
    });
})(jQuery);

var createSearchCurrentFilters = function () {

    // ▼▼▼ テーブル一覧定義 ▼▼▼
    const TBL_STRING_EMPTY = '';
    const TBL_HTML_SPACE = '&nbsp;';
    
    // 検索条件表示エリアの定義
    const SEARCH_CURRENT_FILTERS_AREA =
        '<div id="search-current-filters-area" class="m-search-current-filters">' +
        '<div id="search-current-filters-show-area" class="m-search-current-filters__tags">{0}</div>' +
        '<div class="m-search-current-filters__wrap">' +
        '<span class="m-search-current-filters__date">{1}</span>' +
        '<button id="search-current-filters-clear-btn" type="button" class="a-button a-button--text m-search-current-filters__clear-button">' +
        '<div class="a-button__label">条件クリア</div>' +
        '</button>' +
        '</div>' +
        '</div>';
    // 検索条件明細表示の定義
    const SEARCH_CURRENT_FILTERS_DETAIL = '<span class="{0}">{1}</span>';

    // クラス1
    const TAGS_CLASS = "m-search-current-filters__tags";
    // クラス2
    const WRAP_CLASS = "m-search-current-filters__wrap";
    //クラス3
    const DATE_CLASS = "m-search-current-filters__date";
    //クラス4
    const TAG_CLASS = "a-tag a-tag--filled";
    
    // ▲▲▲ テーブル一覧定義 ▲▲▲

    /**
     * 検索条件表示エリアの作成
     * @param {string}} selector
     * @param {jsondata} optionData
     */
    var __createSearchArea = function (selector, optionData) {
     
        // 検索条件表示エリアの初期化（クリア）
        $(selector).find("#search-current-filters-area").remove();

        // 検索条件は存在の場合
        if ((optionData.data && optionData.data.length > 0) ||
            (optionData.app_date_show && optionData.app_date_show != TBL_STRING_EMPTY)) {

            // 検索条件の取得
            var html = getConditionHtml(optionData);
            $(selector).append(html);

            // イベントの追加
            createEvent(selector, optionData);
        }
    }
    createSearchCurrentFilters.prototype.createSearchCurrentFilters = __createSearchArea;
   
    /**
     * 検索条件により、検索条件表示エリアを作成すること
     * @param {jsondata} optionData 設定属性
     * @returns 作成のhtml
     */
    function getConditionHtml(optionData) {
         
        // 設定属性の取得
        // sub_classの取得
        var sub_class;
        if (optionData.sub_class) {
            sub_class = optionData.sub_class;
        } else {
            sub_class = TAG_CLASS;
        }

        // 半角空白の取得
        var half_space_num;
        if (optionData.half_space) {
            half_space_num = optionData.half_space;
        } else {
            half_space_num = 0;
        }

        // 初期設定
        var strHtml = TBL_STRING_EMPTY;
    
        // 設定のカラムにより、HTMLを作成すること。
        $.each(optionData.data, function (index, val) {
            
            // 引数の取得と使用
            var arrPar = [];

            // sub_classの設定
            arrPar[0] = sub_class;

            // 検索条件の設定
            arrPar[1] = val;

            for (var i = 0; i < half_space_num; i++){
                arrPar[1] += TBL_HTML_SPACE;
            }

            // 初期設定
            strHtml += String.format(SEARCH_CURRENT_FILTERS_DETAIL, arrPar);
        });

        // 検索結果表示エリアの作成
        var fullArr = [];
        // 明細条件の設定
        fullArr[0] = strHtml;
        // 特別設定の判定
        if (optionData.app_date_show != undefined &&
            optionData.app_date_show != null &&
            optionData.app_date_show != TBL_STRING_EMPTY) {
            fullArr[1] = optionData.app_date_show;
        }
        else {
            fullArr[1] = TBL_STRING_EMPTY;
        }

        // 携帯一覧htmlを戻り
        return String.format(SEARCH_CURRENT_FILTERS_AREA, fullArr);
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

            if ((typeof arr == 'object') && arr.constructor == Array)
            {
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
     * イベントの追加
     * @param {string}} selector
     * @param {jsondata} optionData
     */
    function createEvent(selector, optionData) {
        
        // 条件クリアボタン処理の追加
         $("#search-current-filters-clear-btn").on("click", function () {
            
            // 検索条件エリアをクリアすること 
             $(selector).find("#search-current-filters-area").remove();

             // 該当エリアを非表示に設定すること
             // $(selector).find("#search-current-filters-area").hide();

            var fn = optionData.onClickForClear;
            if (fn) {
                fn();
            }
        });
    }
}
/*▲関数定義エリア▲*/