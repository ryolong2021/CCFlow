/**
 * Bootstrap Table Chinese translation
 * Author: Zhixin Wen<wenzhixin2010@gmail.com>
 */
(function ($) {
    'use strict';

    $.fn.bootstrapTable.locales['zh-CN'] = {
        formatLoadingMessage: function () {
            return '頑張ってデータをロードしていますので、少々お待ちください。';
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
            return 'マッチするレコードが見つかりませんでした。';
        },
        formatPaginationSwitch: function () {
            return '改ページを隠す/表示する';
        },
        formatRefresh: function () {
            return '更新';
        },
        formatToggle: function () {
            return '切り替え';
        },
        formatColumns: function () {
            return '列';
        },
        formatExport: function () {
            return 'データをエクスポート';
        },
        formatClearFilters: function () {
            return 'フィルタをクリア';
        }
    };

    $.extend($.fn.bootstrapTable.defaults, $.fn.bootstrapTable.locales['zh-CN']);

})(jQuery);