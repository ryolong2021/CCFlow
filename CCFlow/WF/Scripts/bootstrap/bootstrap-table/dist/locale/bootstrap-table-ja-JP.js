/**
 * Bootstrap Table Chinese translation
 * Author: Zhixin Wen<wenzhixin2010@gmail.com>
 */
(function ($) {
    'use strict';

    $.fn.bootstrapTable.locales['ja-JP'] = {
        formatLoadingMessage: function () {
            return '読み込み中です。少々お待ちください…';
        },
        formatRecordsPerPage: function (pageNumber) {
            return 'ページ当たり最大' + pageNumber + '件レコード';
        },
        formatShowingRows: function (pageFrom, pageTo, totalRows) {
            return '第 ' + pageFrom + ' から第 ' + pageTo + ' 件まで表示、合計 ' + totalRows + ' 件レコード';
        },
        formatSearch: function () {
            return '検索';
        },
        formatNoMatches: function () {
            return '該当するレコードが見つかりません';
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
            return 'データエクスポート';
        },
        formatClearFilters: function () {
            return 'クリア';
        }
    };

    $.extend($.fn.bootstrapTable.defaults, $.fn.bootstrapTable.locales['ja-JP']);

})(jQuery);
