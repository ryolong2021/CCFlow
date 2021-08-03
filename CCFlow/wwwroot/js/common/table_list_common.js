/**
 * 一覧画面の作成
 * 備考：イオンリテール（株）社仕様のみ実現された
 */

// 入り口
(function ($) {
    $.fn.extend({
        "createTable": function (optionData) {

            // セレクターの設定
            tblSelector = this.selector;
            // 一覧の作成
            createTable(optionData);
        }
    });

    // ▼▼▼ テーブル一覧定義 ▼▼▼
    const TBL_STRING_EMPTY = '';
    const TBL_STRING_HYPHEN = '-';

    // 一覧の枠の定義
    const TBL_DIV_OUTER_LAYER = '<div class="m-data-table {1} undefined">{0}</div>';
    // 一覧の携帯の定義
    const TBL_DIV_MOBILE = '<div class="m-data-table__container-card">{0}</div>';
    // 一覧の携帯の定義(cardを単位として、定義すること)
    const TBL_DIV_MOBILE_UNIT = '<div class="m-data-table__card">{0}</div>';
    // 一覧の携帯の定義(明細)※繰り返し部分
    const TBL_DIV_MOBILE_UNIT_DETAIL =
        '<div class="m-data-table__card-item m-data-table__card-item--{0}"><span class="m-data-table__card-header m-data-table__card-header--{0}">{1}</span><span class="m-data-table__card-content m-data-table__card-content--{0} m-data-table__card-content--align-left m-data-table__card-content--valign-top">{2}</span></div>';
    // 一覧の携帯の定義(アクティブボタン)※繰り返し部分 種類1
    const TBL_DIV_ACTIVE_CLASS_ONE =
        '<div class="a-table-action-button"><button type="button" class="a-table-action-button__button"><i class="a-icon a-icon--dot-action"></i></button><div class="a-table-action-button__action">{0}</div></div>';
    // 一覧の携帯の定義(アクティブボタン)※繰り返し部分 種類1
    const TBL_DIV_UNIT_ACTIVE_CLASS_ONE =
        '<div class="a-nav-item a-nav-item--subnav"><a href="#" class="a-nav-item__link"><span class="a-nav-item__label">{0}</span></a></div>';
    // 一覧の携帯の定義(アクティブボタン)※繰り返し部分 種類2
    const TBL_DIV_ACTIVE_CLASS_TWO =
        '<div class="m-button-container m-button-container--border">{0}</div>';
    // 一覧の携帯の定義(アクティブボタン)※繰り返し部分 種類2
    const TBL_DIV_UNIT_ACTIVE_CLASS_TWO =
        '<button type="button" class="a-button a-button--text"><div class="a-button__label">{0}</div></button>';

    // 一覧のPCの定義
    const TBL_DIV_PC = '<div class="m-data-table__container">{0}</div>';
    // 一覧のPCの定義(ヘッダー)
    const TBL_DIV_PC_HEADER = '<div class="m-data-table__header">{0}</div>';
    // 一覧のPCの定義(ヘッダー、繰り返し用)
    const TBL_DIV_PC_HEADER_DETAIL = '<div class="m-data-table__header-content m-data-table__header-content--{0}">{1}</div>';
    // 一覧のPCの定義(明細)
    const TBL_DIV_PC_DATA = '<div class="m-data-table__container-item">{0}</div>';
    // 一覧のPCの定義(明細)
    const TBL_DIV_PC_ROW_DATA = '<div class="m-data-table__item">{0}</div>';
    // 一覧のPCの定義(明細、繰り返し用)
    const TBL_DIV_PC_DATA_DETAIL =
        '<span class="m-data-table__content m-data-table__content--{0} m-data-table__content--align-left m-data-table__content--valign-center"><span class="m-data-table__truncate-content">{1}</span></span>';

    // ページングの定義
    const TBL_PAGINATION =
        '<div class="a-pagination"><button type="button" class="a-pagination__action a-pagination__action--prev {1}"><i class="a-icon a-icon--arrow-left"></i><i class="a-icon a-icon--arrow-left-white"></i></button><ul class="a-pagination__pages">{0}</ul><button type="button" class="a-pagination__action a-pagination__action--next {2}"><i class="a-icon a-icon--arrow-right"></i><i class="a-icon a-icon--arrow-right-white"></i></button></div>';
    // ページングインデックスの定義
    const TBL_PAGINATION_PAGE = '<li class="a-pagination__page-item {0}"><span class="a-pagination__page-index">{1}</span></li>';

    // アクティブクラス
    const PAGE_ACTIVE_CLASS = "a-pagination__page-item--active";
    // オプションクラス
    const PAGE_OPTIONAL_CLASS = "a-pagination__page-item--optional";
    // 非活性クラス
    const PAGE_ACTION_DISABLED_CLASS = "a-pagination__action--disabled";
    // 省略記号クラス
    const PAGE_ELLIPSIS_CLASS = "a-pagination__page-item--ellipsis";

    // レコードがない場合、画面では表示のメッセージ定義
    const NO_RECORD_MSG = "<p class='a-text--align-center'>{0}</p>";

    // アクションボタンのクラス（種類2）
    const ACTION_BTN_CLASS = "m-data-table--with-action-buttons";
    
    // ソート機能
    const SORT_CLASS = "<span><i class='a-icon a-icon--arrow-up sort-cursor sort-up'></i><i class='a-icon a-icon--arrow-down sort-cursor sort-down'></i></span>";
    // 昇順ソート
    const SORT_ASC = 'asc';
    // 降順ソート
    const SORT_DESC = 'desc';

    // テーブル一覧のセレクター
    var tblSelector;

    // アクションボタンフラグ（種類2） true : あり false : なし
    var actionFlag = false;

    // ▲▲▲ テーブル一覧定義 ▲▲▲

    /*▼関数定義エリア▼*/

    /**
     * 一覧画面の作成
     * @param {jsondata} optionData
     */
    function createTable(optionData) {

        // ソート項目指定有無の判定
        if (optionData.sortName !== undefined && optionData.sortName !== null && optionData.sortName !== TBL_STRING_EMPTY) {
            getSortData(optionData, optionData.sortName, optionData.sortOrder);
        }

        // 一覧内容を画面に表示すること
        getTableHtml(optionData.columns, optionData.data, optionData.norecordmsg);

        // ソートアイコンの編集
        if (optionData.sortName !== undefined && optionData.sortName !== null && optionData.sortName !== TBL_STRING_EMPTY) {
            editSortClass(optionData.columns, optionData.sortName, optionData.sortOrder);
        }

        // イベントの追加
        createEventForList(optionData);

        // ページング有無の判断
        if (optionData.pagination && optionData.data.length > 0) {

            // ページングの設定とイベントの設定
            mkTblPage(optionData);
        }
    }

    /**
     * 外部カラムとデータの設定によりテーブル一覧を作成すること
     * @param {jsondata} columns 設定カラム属性
     * @param {jsondata} datas 表示でデータ
     * @param {string} noRecordMsg レコードない表示メッセージ
     * @returns 作成のhtml
     */
    function getTableHtml(columns, datas, noRecordMsg) {
        // 携帯の画面イメージの取得
        var mobileHtml = getTableHtmlForMobile(columns, datas);

        // PCの画面イメージの取得
        var pcHtml = getTableHtmlForPc(columns, datas);

        // アクションボタンクラスの取得
        var actBtnClass = actionFlag === false ? TBL_STRING_EMPTY : ACTION_BTN_CLASS;
        var tblHtml = String.format(TBL_DIV_OUTER_LAYER, [mobileHtml + pcHtml, actBtnClass]);

        // データは存在しない場合、提示データの表示
        if (datas.length < 1 && noRecordMsg != undefined && noRecordMsg != null) {
            tblHtml += String.format(NO_RECORD_MSG, noRecordMsg);
        }

        $(tblSelector + " *").remove();
        $(tblSelector).append(tblHtml);
    }

    /**
     * 外部カラムとデータの設定によりテーブル一覧を作成すること
     * 携帯用
     * @param {jsondata} columns 設定カラム属性
     * @param {jsondata} datas 表示でデータ
     * @returns 作成のhtml
     */
    function getTableHtmlForMobile(columns, datas) {

        // 初期設定
        var strHtml = TBL_STRING_EMPTY;

        // 設定のカラムにより、HTMLを作成すること。
        $.each(datas, function (datasIndex, datasVal) {

            // 初期設定
            var colHtml = TBL_STRING_EMPTY;

            // 設定のカラムにより、HTMLを作成すること。
            $.each(columns, function (colIndex, colVal) {

                // 引数の取得と使用
                var arrPar = [];

                // クラス種類の設定
                if (colVal.modify_type_class === undefined ||
                    colVal.modify_type_class === null ||
                    colVal.modify_type_class === TBL_STRING_EMPTY) {
                    arrPar[0] = 'type-data';
                } else {
                    arrPar[0] = colVal.modify_type_class;
                }

                if (arrPar[0] === "type-action") {

                    // 文字列（論理名）の設定 -- なし
                    arrPar[1] = TBL_STRING_EMPTY;

                    // 1:省略記号で表示の場合
                    if (colVal.show_class === "1") {

                        var editHtml = TBL_STRING_EMPTY;
                        $.each(colVal.title_action, function (titleActIndex, titleActVal) {
                            // １カラムの作成
                            editHtml += String.format(TBL_DIV_UNIT_ACTIVE_CLASS_ONE, titleActVal.title);
                        });

                        arrPar[2] = String.format(TBL_DIV_ACTIVE_CLASS_ONE, editHtml);

                        colHtml += String.format(TBL_DIV_MOBILE_UNIT_DETAIL, arrPar);
                    }
                    // 2:ボタンの形で表示の場合
                    else if (colVal.show_class === "2") {

                        // アクションボタンにtrueを設定すること
                        actionFlag = true;

                        var editHtml = TBL_STRING_EMPTY;

                        $.each(colVal.title_action, function (titleActIndex, titleActVal) {
                            // １カラムの作成
                            editHtml += String.format(TBL_DIV_UNIT_ACTIVE_CLASS_TWO, titleActVal.title);
                        });

                        arrPar[2] = String.format(TBL_DIV_ACTIVE_CLASS_TWO, editHtml);

                        colHtml += String.format(TBL_DIV_MOBILE_UNIT_DETAIL, arrPar);
                    }
                } else {

                    // 文字列（論理名）の設定
                    arrPar[1] = colVal.title;

                    // データフォーマットは有無の判断
                    if (colVal.formatter != undefined && colVal.formatter != null) {
                        // データフォーマットは有無の判断
                        arrPar[2] = colVal.formatter(datasVal);
                    } else {
                        // 表示値の設定
                        if (datasVal[colVal.field]) {
                            // 表示値の設定
                            arrPar[2] = datasVal[colVal.field];
                        } else {
                            // 表示値の設定
                            arrPar[2] = TBL_STRING_HYPHEN;
                        }
                    }

                    // １カラムの作成
                    colHtml += String.format(TBL_DIV_MOBILE_UNIT_DETAIL, arrPar);
                }
            });

            // １行の作成
            strHtml += String.format(TBL_DIV_MOBILE_UNIT, colHtml);
        });

        // 携帯一覧htmlを戻り
        return String.format(TBL_DIV_MOBILE, strHtml);
    }

    /**
     * 外部カラムとデータの設定によりテーブル一覧を作成すること
     * PC用
     * @param {jsondata} columns 設定カラム属性
     * @param {jsondata} datas 表示でデータ
     * @returns 作成のhtml
     */
    function getTableHtmlForPc(columns, datas) {

        // ヘッダーの取得
        var header = getTableHtmlForPcHeader(columns);
        // 明細の取得
        var detail = getTableHtmlForPcDetail(columns, datas);

        return String.format(TBL_DIV_PC, header + detail);
    }

    /**
     * 外部カラムとデータの設定によりテーブル一覧を作成すること
     * PC用
     * ヘッダーの作成
     * @param {jsondata} columns 設定カラム属性
     * @returns 作成のhtml(ヘッダー部)
     */
    function getTableHtmlForPcHeader(columns) {

        // 初期設定
        var strHtml = TBL_STRING_EMPTY;

        // 設定のカラムにより、HTMLを作成すること。
        $.each(columns, function (colIndex, colVal) {

            // 引数の取得と使用
            var arrPar = [];

            // クラス種類の設定
            if (colVal.modify_type_class === undefined ||
                colVal.modify_type_class === null ||
                colVal.modify_type_class === TBL_STRING_EMPTY) {
                arrPar[0] = 'type-data';
            } else {
                arrPar[0] = colVal.modify_type_class;
            }

            if (arrPar[0] === 'type-action') {
                // 文字列（論理名）の設定
                arrPar[1] = TBL_STRING_EMPTY;
            }
            else {
                // 文字列（論理名）の設定
                arrPar[1] = colVal.title;

                // ソート項目設定
                if (colVal.sortable !== undefined && colVal.sortable !== null && colVal.sortable === true) {
                    arrPar[1] += SORT_CLASS;
                }
            }

            // １ヘッダーの作成
            strHtml += String.format(TBL_DIV_PC_HEADER_DETAIL, arrPar);
        });

        return String.format(TBL_DIV_PC_HEADER, strHtml);
    }

    /**
     * 外部カラムとデータの設定によりテーブル一覧を作成すること
     * PC用
     * 明細の作成
     * @param {jsondata} columns 設定カラム属性
     * @param {jsondata} datas 表示でデータ
     * @returns 作成のhtml(明細部)
     */
    function getTableHtmlForPcDetail(columns, datas) {

        // 初期設定
        var strHtml = TBL_STRING_EMPTY;

        // 設定のカラムにより、HTMLを作成すること。
        $.each(datas, function (datasIndex, datasVal) {

            // 初期設定
            var colHtml = TBL_STRING_EMPTY;

            // 設定のカラムにより、HTMLを作成すること。
            $.each(columns, function (colIndex, colVal) {

                // 引数の取得と使用
                var arrPar = [];

                // クラス種類の設定
                if (colVal.modify_type_class === undefined ||
                    colVal.modify_type_class === null ||
                    colVal.modify_type_class === TBL_STRING_EMPTY) {
                    arrPar[0] = 'type-data';
                } else {
                    arrPar[0] = colVal.modify_type_class;
                }

                if (arrPar[0] === "type-action") {

                    // 1:省略記号で表示の場合
                    if (colVal.show_class === "1") {

                        var editHtml = TBL_STRING_EMPTY;
                        $.each(colVal.title_action, function (titleActIndex, titleActVal) {
                            // １カラムの作成
                            editHtml += String.format(TBL_DIV_UNIT_ACTIVE_CLASS_ONE, titleActVal.title);
                        });

                        arrPar[1] = String.format(TBL_DIV_ACTIVE_CLASS_ONE, editHtml);

                        colHtml += String.format(TBL_DIV_PC_DATA_DETAIL, arrPar);
                    }
                    // 2:ボタンの形で表示の場合
                    else if (colVal.show_class === "2") {

                        // アクションボタンにtrueを設定すること
                        actionFlag = true;

                        var editHtml = TBL_STRING_EMPTY;

                        $.each(colVal.title_action, function (titleActIndex, titleActVal) {
                            // １カラムの作成
                            editHtml += String.format(TBL_DIV_UNIT_ACTIVE_CLASS_TWO, titleActVal.title);
                        });

                        arrPar[1] = String.format(TBL_DIV_ACTIVE_CLASS_TWO, editHtml);

                        colHtml += String.format(TBL_DIV_PC_DATA_DETAIL, arrPar);
                    }
                } else {

                    // データフォーマットは有無の判断
                    if (colVal.formatter != undefined && colVal.formatter != null) {
                        // データフォーマットは有無の判断
                        arrPar[1] = colVal.formatter(datasVal);
                    } else {
                        // 表示値の設定
                        if (datasVal[colVal.field]) {
                            // 表示値の設定
                            arrPar[1] = datasVal[colVal.field];
                        } else {
                            // 表示値の設定
                            arrPar[1] = TBL_STRING_HYPHEN;
                        }
                    }

                    // １カラムの作成
                    colHtml += String.format(TBL_DIV_PC_DATA_DETAIL, arrPar);
                }
            });

            // １行の作成
            strHtml += String.format(TBL_DIV_PC_ROW_DATA, colHtml);
        });

        // PC一覧htmlを戻り
        return String.format(TBL_DIV_PC_DATA, strHtml);
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
     * ページングの作成を行うこと
     * @param {jsondata} optionData 設定属性
     */
    function mkTblPage(optionData) {

        // 1ページには表示の数
        var curNum;
        if (optionData.pageSize) {
            curNum = optionData.pageSize;
        } else {
            curNum = 10;
        }

        // 総ページ数
        var len = Math.ceil(optionData.data.length / curNum);

        // 該当のインデックス
        var pageNum;
        if (optionData.pageNumber) {
            pageNum = optionData.pageNumber;
        } else {
            pageNum = 1;
        }

        // ページインデックスの作成
        createPageHtml(pageNum, len);

        // 初ページの表示 (0件データから表示すること)
        contrListShowHide(pageNum, curNum)

        // イベントの設定（改ページ）
        createEventForPage(curNum, len);
    }

    /**
     * ページhtmlの作成こと
     * @param {int} pageNum ページングのアクティブページ数
     * @param {int} pageLen 総ページ数
     * @returns 編集後データ
     */
    function createPageHtml(pageNum, pageLen) {

        // ページインデックスの作成
        var pageList = getPageHtml(pageNum, pageLen);

        $(tblSelector + " .a-pagination").remove();
        $(tblSelector + " .m-data-table").after(pageList);
    }

    /**
     * ページイングhtmlの取得
     * @param {int} pageNum ページングのアクティブページ数
     * @param {int} pageLen 総ページ数
     * @returns 編集後データ
     */
    function getPageHtml(pageNum, pageLen) {

        // ページリスト
        var pageList = TBL_STRING_EMPTY;

        // 総ページ数は9より小さい
        if (pageLen <= 9) {
            // ページインデックスの作成
            for (var i = 1; i <= pageLen; i++) {

                // ページhtmlの取得
                pageList += makePageHtml(pageNum, i);
            }
        } else {

            // 総ページ数は該当ページ数より遠い
            if (pageNum <= 4) {
                // 1-7を表示
                for (var i = 1; i <= 7; i++) {

                    // ページhtmlの取得
                    pageList += makePageHtml(pageNum, i);
                }

                // 「...」を追加する
                pageList += makePageHtmlForPoint();

                // 「...」の後にページの追加
                pageList += makePageHtml(pageNum, pageLen);

            }
            // 総ページ数は10より多くて、該当ページ数より近い　(総ページ数-4)
            else if (pageNum > pageLen - 4) {

                // １ページ
                pageList += makePageHtml(pageNum, 1);

                // 「...」を追加する
                pageList += makePageHtmlForPoint();

                // 最後の7ページを作成すること
                for (var i = pageLen - 6; i <= pageLen; i++) {
                    // ページhtmlの取得
                    pageList += makePageHtml(pageNum, i);
                }
            }
            else {

                // １ページ
                pageList += makePageHtml(pageNum, 1);
                // 「...」を追加する
                pageList += makePageHtmlForPoint();
                //生成当前页和 前跟后一个页数
                for (var i = pageNum - 2; i <= pageNum + 2; i++) {
                    pageList += makePageHtml(pageNum, i);
                }
                // 「...」を追加する
                pageList += makePageHtmlForPoint();
                // 「...」の後にページの追加
                pageList += makePageHtml(pageNum, pageLen);
            }
        }

        // 前ページ、次ページCSSの設定
        var pageArr = [];
        // ページの設定
        pageArr[0] = pageList;

        // 前ページ、次ページCSSの設定
        if (pageNum === 1 && pageLen === pageNum) {
            // 前ページ非活性
            pageArr[1] = PAGE_ACTION_DISABLED_CLASS;
            // 前ページ非活性
            pageArr[2] = PAGE_ACTION_DISABLED_CLASS;
        }
        else if (pageNum === 1 && pageLen !== pageNum) {
            // 前ページ非活性
            pageArr[1] = PAGE_ACTION_DISABLED_CLASS;
            // 前ページ活性
            pageArr[2] = TBL_STRING_EMPTY;
        }
        else if (pageNum !== 1 && pageLen === pageNum) {
            // 前ページ活性
            pageArr[1] = TBL_STRING_EMPTY;
            // 前ページ非活性
            pageArr[2] = PAGE_ACTION_DISABLED_CLASS;
        }
        else {
            // 前ページ活性
            pageArr[1] = TBL_STRING_EMPTY;
            // 前ページ活性
            pageArr[2] = TBL_STRING_EMPTY;
        }

        return String.format(TBL_PAGINATION, pageArr);
    }

    /**
     * ページイングhtmlの取得
     * @param {int} pageNum アクティブパージ数
     * @param {int} pageIndex 各ページ数
     * @returns 編集後データ
     */
    function makePageHtml(pageNum, pageIndex) {

        // ページリスト
        var pageHtml = TBL_STRING_EMPTY;

        // 引数の設定
        var arr = [];

        // アクティブページ数の設定
        if (pageIndex < pageNum) {
            arr[0] = TBL_STRING_EMPTY;
        }
        else if (pageIndex === pageNum) {
            arr[0] = PAGE_ACTIVE_CLASS;
        }
        else {
            // arr[0] = PAGE_OPTIONAL_CLASS;  // 携帯版の場合、ページはデフォルト値が非表示になる
            arr[0] = TBL_STRING_EMPTY;
        }

        // ページ数の設定
        arr[1] = pageIndex;

        pageHtml = String.format(TBL_PAGINATION_PAGE, arr);

        return pageHtml;
    }

    /**
     * ページイング[...]htmlの取得
     * @returns 編集後データ
     */
    function makePageHtmlForPoint() {
        // 引数の設定
        var arr = [];
        // スタイルの設定
        arr[0] = PAGE_ELLIPSIS_CLASS;
        // ページ数の設定
        arr[1] = "...";
        return String.format(TBL_PAGINATION_PAGE, arr);
    }

    /**
     * 改ページにより、一覧表示/非表示制御
     * @param {int} pageNum アクティブパージ数
     * @param {int} curNum 1ページには表示の数
     */
    function contrListShowHide(pageNum, curNum) {

        // 表示データ開始位置の取得
        var pageStart = (pageNum - 1) * curNum;
        // 表示データ終了位置の取得
        var pageEnd = pageNum * curNum;

        // 一覧データを全部非表示します（PCと携帯）
        $(tblSelector + " .m-data-table").find(".m-data-table__item").hide();
        $(tblSelector + " .m-data-table").find(".m-data-table__card").hide();
        /************条件により、一覧データを表示します*********/
        for (var i = pageStart; i < pageEnd; i++) {
            $(tblSelector + " .m-data-table").find(".m-data-table__item").eq(i).show();
            $(tblSelector + " .m-data-table").find(".m-data-table__card").eq(i).show();
        }
    }

    /**
     * イベントの追加
     * @param {int} curNum 1ページには表示の数
     * @param {int} len 総ページ数
     */
    function createEventForPage(curNum, len) {

        /*******ページインデックスのイベント*******/
        $(tblSelector + " .a-pagination__pages").find(".a-pagination__page-item").each(function () {
            $(this).click(function () {

                // 遷移ページの取得
                var pageS = parseInt($(this).find(".a-pagination__page-index").html());

                // ページインデックスの作成
                createPageHtml(pageS, len);
                // イベントの設定（改ページ）
                contrListShowHide(pageS, curNum)
                // イベントの設定（改ページ）
                createEventForPage(curNum, len);
            });
        })

        /*次ページ*/
        $(tblSelector + " .a-pagination__action--next").click(function () {

            // 遷移ページの取得
            var pageS = parseInt($(tblSelector + " ." + PAGE_ACTIVE_CLASS).find(".a-pagination__page-index").html()) + 1;

            // ページインデックスの作成
            createPageHtml(pageS, len);
            // イベントの設定（改ページ）
            contrListShowHide(pageS, curNum)
            // イベントの設定（改ページ）
            createEventForPage(curNum, len);
        });

        /*前ページ*/
        $(tblSelector + " .a-pagination__action--prev").click(function () {
            // 遷移ページの取得
            var pageS = parseInt($(tblSelector + " ." + PAGE_ACTIVE_CLASS).find(".a-pagination__page-index").html()) - 1;

            // ページインデックスの作成
            createPageHtml(pageS, len);
            // イベントの設定（改ページ）
            contrListShowHide(pageS, curNum)
            // イベントの設定（改ページ）
            createEventForPage(curNum, len);
        })
    }

    /**
     * 一覧イベントの追加
     * @param {jsonData}} optionData 設置属性
     */
    function createEventForList(optionData) {

        // PC版クリックイベントの追加 種類1
        addActEvent(optionData, ".m-data-table__item", ".a-nav-item__link");

        // 携帯版クリックイベントの追加 種類1
        addActEvent(optionData, ".m-data-table__card", ".a-nav-item__link");

        // PC版クリックイベントの追加 種類2
        addActEvent(optionData, ".m-data-table__item", ".a-button");

        // 携帯版クリックイベントの追加 種類2
        addActEvent(optionData, ".m-data-table__card", ".a-button");

        //  行クリック属性あるか判断
        if (optionData.onRowClick) {
            // PC版行クリックイベントの追加
            addActEventForRowClick(optionData, ".m-data-table__item", ".m-data-table__content");

            // 携帯版行クリックイベントの追加
            addActEventForRowClick(optionData, ".m-data-table__card", ".m-data-table__card-item");
        }

        // ソートイベントの追加
        sortIconEvent(optionData);
    }

    /**
     * 一覧表編集ボタンイベントの追加
     * @param {jsonData}} optionData 設置属性
     * @param {string}} selectorRow 設置属性
     * @param {string}} selectorAct イベントの追加設置属性
     */
    function addActEvent(optionData, selectorRow, selectorAct) {

        $(tblSelector + " .m-data-table").find(selectorRow).each(function (index, element) {

            $(this).find(selectorAct).each(function (actIndex, actElement) {
                $(this).click(function () {

                    var fn = optionData.columns.find(function (e) { return e.field === "ActionEvent"; }).title_action[actIndex].onClick;
                    if (fn) {

                        // 該当データのインデックス,データリスト、該当データ
                        fn(this, index, optionData.data, optionData.data[index]);
                    }
                });
            });
        });
    }

    /**
     * 一覧表編集ボタンイベントの追加
     * @param {jsonData}} optionData 設置属性
     * @param {string}} selectorRow 設置属性
     * @param {string}} selectorAct イベントの追加設置属性
     */
    function addActEventForRowClick(optionData, selectorRow, selectorCol) {

        // 行の取得
        $(tblSelector + " .m-data-table").find(selectorRow).each(function (rowIndex, rowElement) {

            // 列の取得
            $(this).find(selectorCol).each(function (colIndex, colElement) {

                // 行クリック無効かを判断すること
                if (optionData.columns[colIndex].row_click_enable == undefined ||
                    optionData.columns[colIndex].row_click_enable == null ||
                    optionData.columns[colIndex].row_click_enable == true) {
                    // クリックイベントの設定
                    $(this).click(function (event) {

                        var isOpen = true;
                        this.classList.forEach(
                            function (classElement) {
                                if (classElement.indexOf("m-data-table__content--type-action") == 0 ||
                                    classElement.indexOf("m-data-table__card-item--type-action") == 0) {
                                    isOpen = false;
                                }
                            }
                        );

                        if (isOpen) {
                            optionData.onRowClick(this, rowIndex, optionData.data, optionData.data[rowIndex]);
                        }
                    });
                }
            });
        });
    }

    /**
     * ソート後のデータの取得
     * @param {jsonData} optionData 設置属性
     * @param {string} key ソートの項目名
     * @param {string} order asc:昇順 desc:降順
     */
    function getSortData(optionData, key, order) {

        var orderby = (order.toLowerCase() === SORT_DESC) ? SORT_DESC : SORT_ASC;

        optionData.data.sort(compareValues(key, orderby));
    }

    /**
     * 指定のキーにより、ソートを行うこと
     * @param {string} key ソートの項目名
     * @param {string} order asc:昇順 desc:降順
     */
    function compareValues(key, order = SORT_ASC) {
        return function (a, b) {
            if (!a.hasOwnProperty(key) || !b.hasOwnProperty(key)) {
                // property doesn't exist on either object
                return 0;
            }

            const varA = (typeof a[key] === 'string') ?
                a[key].toUpperCase() : a[key];
            const varB = (typeof b[key] === 'string') ?
                b[key].toUpperCase() : b[key];

            let comparison = 0;
            if (varA > varB) {
                comparison = 1;
            } else if (varA < varB) {
                comparison = -1;
            }
            return (
                (order == SORT_DESC) ? (comparison * -1) : comparison
            );
        };
    }

    /**
     * ソート後のデータの取得
     * @param {jsonData} columns カラム設置属性
     * @param {string} key ソートの項目名
     * @param {string} order asc:昇順 desc:降順
     */
    function editSortClass(columns, key, order) {

        // 昇順/降順の取得
        var orderby = (order.toLowerCase() === SORT_DESC) ? SORT_DESC : SORT_ASC;

        // 設定のカラムにより、HTMLを作成すること。
        $.each(columns, function (colIndex, colVal) {

            // ソート項目の探し
            if (colVal.field === key &&
                colVal.sortable !== undefined && colVal.sortable !== null && colVal.sortable === true) {

                if (orderby === SORT_ASC) {
                    $(tblSelector + " .m-data-table__header-content").eq(colIndex).find(".sort-up").removeClass("a-icon--arrow-up sort-cursor");
                }
                else {
                    $(tblSelector + " .m-data-table__header-content").eq(colIndex).find(".sort-down").removeClass("a-icon--arrow-down sort-cursor");
                }
            }
        });
    }

    /**
     * ソートアイオンイベントの追加
     * @param {jsonData} optionData 設置属性
     */
    function sortIconEvent(optionData) {

        $(tblSelector + " .m-data-table__header").find(".m-data-table__header-content").each(function (index, element) {

            // 昇順イベントの追加
            $(this).find(".sort-up").each(function (upIndex, upElement) {
                $(this).click(function () {

                    // ソート名の設定
                    optionData.sortName = optionData.columns[index].field;
                    // ソート順の設定
                    optionData.sortOrder = SORT_ASC;

                    // 一覧の作成
                    createTable(optionData);
                });
            });

            // 降順イベントの追加
            $(this).find(".sort-down").each(function (downIndex, downElement) {
                $(this).click(function () {

                    // ソート名の設定
                    optionData.sortName = optionData.columns[index].field;
                    // ソート順の設定
                    optionData.sortOrder = SORT_DESC;

                    // 一覧の作成
                    createTable(optionData);
                });
            });
        });

    }
    /*▲関数定義エリア▲*/

})(jQuery);