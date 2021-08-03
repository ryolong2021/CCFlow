/**
 * Bootstrap Table Chinese translation
 * Author: Zhixin Wen<wenzhixin2010@gmail.com>
 */
(function ($) {
    'use strict';

    $.fn.bootstrapTable.locales['zh-TW'] = {
        formatLoadingMessage: function () {
            return '頑張って資料を載せていますので、少々お待ちください。';
        },
        formatRecordsPerPage: function (pageNumber) {
            return 'ページごとに表示' + pageNumber + ' レコード';
        },
        formatShowingRows: function (pageFrom, pageTo, totalRows) {
            return '表示第' + pageFrom + ' 第1位まで' + pageTo + ' レコード' + totalRows + ' レコード';
        },
        formatSearch: function () {
            return '検索';
        },
        formatNoMatches: function () {
            return '該当する結果が見つかりませんでした。';
        },
        formatPaginationSwitch: function () {
            return '改ページを隠す/表示する';
        },
        formatRefresh: function () {
            return '整理し直す';
        },
        formatToggle: function () {
            return '切り替え';
        },
        formatColumns: function () {
            return '列';
        }
    };

    $.extend($.fn.bootstrapTable.defaults, $.fn.bootstrapTable.locales['zh-TW']);

})(jQuery);