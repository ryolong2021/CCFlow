/*▼グローバル変数定義エリア▼*/
var webUser = null;
var wfCommon = new wfCommon();                       //共通関連
var conditions = null;                               // 絞り込み検索条件array
var empKbnList = [];                                 // 従業員区分リスト
var loanedStaffKbnList = [];                         // 出向者区分リスト
var glcKbnList = [];                                 // グッドライフ区分リスト
var unionKbnList = [];                               // 組合区分リスト

//…追加変数

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {

    if (webUser == null)
        webUser = new WebUser();
    if (webUser.No == null)
        return;

    //画面初期化
    InitPageForAsync();

    //…追加関数

});
  
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/

/**
 * 画面初期化
 */
function InitPageForAsync() {

    var objGetTbl = {};
    objGetTbl[0] = 'KBN';

    // 区分リスト一覧の取得、画面の初期表示
    wfCommon.initGetData(objGetTbl, InitPage);
}

/**
 * 画面初期化
 * @param {object} data サーバーから取得のデータ
 */
function InitPage(data) {

    // 区分リスト一覧の取得
    getKbnList(data)

    // プルダウンリストの設定
    setSelectControl();

    // 日付入力パネルの設定
    setDatePicker();

    // 一覧表示の作成
    createList();

    // イベント設定
    createEvent();
}

/**
 * 区分リスト一覧の取得
 * @param {object} data サーバーから取得のデータ
 **/
function getKbnList(data) {

    //区分マスタ格納
    var dtKbn = data['KBN'];

    // 従業員区分リストの取得
    empKbnList = dtKbn["JYUUGYOUINN_KBN"];

    // 出向者区分リストの取得
    loanedStaffKbnList = dtKbn["SYUKOU_KBN"];

    // グッドライフ区分リストの取得
    glcKbnList = dtKbn["GLC_KBN"];

    // 組合区分リストの取得
    unionKbnList = dtKbn["KUMIAI_KBN"];
}

/**
 * プルダウンリストの設定
 **/
function setSelectControl() {

    // 従業員区分リストの設定
    wfCommon.initDropdown(true, empKbnList, STRING_EMPTY, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "emp_kbn_search", "emp_kbn_search_pull");

    // 出向者区分リストの取得
    wfCommon.initDropdown(true, loanedStaffKbnList, STRING_EMPTY, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "loaned_staff_search", "loaned_staff_search_pull");

    // グッドライフ区分リストの取得
    wfCommon.initDropdown(true, glcKbnList, STRING_EMPTY, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "glc_kbn_search", "glc_kbn_search_pull");

    // 組合区分リストの取得
    wfCommon.initDropdown(true, unionKbnList, STRING_EMPTY, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "union_kbn_search", "union_kbn_search_pull");
}


/**
 * 日付入力パネルの設定
 */
function setDatePicker() {

    // 検索条件 -- 初回申請日の設定
    wfCommon.setdatepickerWithStartEnd(
        "#app_date_search_from",
        "#app_date_search_to",
        DATE_FORMAT_MOMENT_PATTERN_1,
        true,
        false,
        function () { },
        function () { });

    // 検索条件 -- 香料申請日の設定
    wfCommon.setdatepickerWithStartEnd(
        "#spice_app_date_search_from",
        "#spice_app_date_search_to",
        DATE_FORMAT_MOMENT_PATTERN_1,
        true,
        false,
        function () { },
        function () { });
}

/**
 * 一覧表示の作成
 */
function createList() {

    // 検索条件の初期設定
    initConditions();

    // サーバーをアクセスして、データを取得すること
    // 一覧の作成
    getDataWithServerAndCreatList();
}

/**
 * 絞り込み検索条件のクリア
 */
function initConditions() {

    // 検索条件のインスタンス
    conditions = new HashTblCommon();

    // 入力項目のクリア
    $("#search-condition-area").find('input:text').each(function (index, element) {

        // テキスト、email、隠し項目
        conditions.ObjArr[element.id] = STRING_EMPTY;
    });

    // プルダウンリスト項目のクリア
    $("#search-condition-area").find('select').each(function (index, element) {

        // テキスト、email、隠し項目
        conditions.ObjArr[element.id] = STRING_EMPTY;
    });
}

/**
 * 検索条件でサーバーからデータを取得すること
 */
function getDataWithServerAndCreatList() {

    // 弔事連絡票照会一覧検索条件設定
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_CondolenceList");
    handler.AddUrlData();

    // 弔事連絡票照会一覧検索条件
    handler.AddPara("CondolenceInq", conditions.stringify());
    handler.DoMethodSetString("GetCondolenceInqList", function (data) {
        //例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }

        // 一覧の作成
        createListTable(JSON.parse(data));
    });
}

/**
 * 一覧を生成する
 * @param {object} jsonData サーバーから取得のデータ
 */
function createListTable(jsonData) {
     
    // カラムの設定
    //社員番号,氏名,所属,チーム名,グッドライフ区分,組合区分,初回申請日,香料申請日
    var columns = [
        {
            field: 'AppCode',  // 物理名
            title: '申請番号',  // 論理名
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'EmployeeNo',
            title: '社員番号',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'EmployeeName',
            title: '氏名',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'DepartmentName',
            title: '所属',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'TeamName',
            title: 'チーム名',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'GoodlifeKbn',
            title: 'グッドライフ区分',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'KumiaiKbn',
            title: '組合区分',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'InputDate',
            title: '初回申請日',
            sortable: true,
            modify_type_class: 'type-data', // クラス種類  type-id / type-data / type-action
            formatter: function (res) {
                return dateFormatter(res.InputDate);
            }
        },
        {
            field: 'KouryoDate',
            title: '香料申請日',
            sortable: true,
            modify_type_class: 'type-data', // クラス種類  type-id / type-data / type-action
            formatter: function (res) {
                return dateFormatter(res.KouryoDate);
            }
        }
    ];

    var optionData = {
        data: jsonData,  // サーバから取得のデータ
        pagination: true, // ページネーション：表示
        pageNumber: 1, // 初期化１ページ目を設定
        pageSize: 20, // １ページ表示件数を設定
        columns: columns, // カラム設定
        sortName: 'AppCode', // 初期化並び順項目
        sortOrder: 'desc', // 初期化並び順：降順
        norecordmsg: msg["I0002"],
        onRowClick: function (event, index, dataList, data) { // 行クリック

            $("#detail-screen").addClass("o-modal--opened");

            // 詳細画面を呼び出す関数設定
            var optionDetail = {
                oid: data.WorkID,
                applyNumber: data.AppCode
            };

            // 詳細画面を呼び出す
            $().InitPageForDetail(optionDetail);
        }
    };

    // 一覧の作成
    $("#table").createTable(optionData);
}

/**
 * イベント定義
 */
function createEvent() {

    // CSV出力ボタンを押す処理
    $("#btn_csv_output").on("click", function () {

        // 「CSV出力」ボタンを押す処理
        btnCsvOutputEvent();
    });

    // 絞り込みボタンを押す処理
    $("#show-search-screen").on("click", function () {

        // 検索条件入力エリアを表示すること
        $("#search-condition-area").addClass("o-modal--opened");
    });

    // 絞り込み画面の「×」ボタンを押す処理
    $("#close_btn_search").on("click", function () {

        // 検索条件入力エリアを閉じること
        $("#search-condition-area").removeClass("o-modal--opened");
    });

    // 絞り込み画面の「クリア」ボタンを押す処理
    $("#clear_btn_search").on("click", function () {

        // 絞り込み画面の「クリア」ボタンを押す処理
        btnClearRunEvent();
    });

    // 絞り込み画面の「検索」ボタンを押す処理
    $("#search_btn_search").on("click", function () {

        // 絞り込み画面の「検索」ボタンを押す処理
        btnSearchRunEvent();
    });

    // 子画面の閉じる処理　「×」ボタンを押す処理
    $("#detail-screen-close-btn").on("click", function () {
        $("#detail-screen").removeClass("o-modal--opened");
    });

    // 「ホームへ」ボタンのリンクの設定
    $("#btn_home_back").on("click", function () {
        location.href = "../../../pages/biz/menu/form_mainmenu.html";
    });
}

/**
 * 「CSV出力」ボタンを押す処理
 */
function btnCsvOutputEvent() {

    // バック対象の作成
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_CondolenceList");
    // URLの追加
    handler.AddUrlData();

    // 一覧検索条件
    handler.AddPara("CondolenceInq", conditions.stringify());

    // 設定後の内容をバックに送信すること
    handler.DoMethodSetString("GetCsvDataInfo", function (data) {

        //例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.ShowDialog(
                DIALOG_ALERT,
                STRING_EMPTY,
                data,
                "はい"
            );
            return;
        }

        // CSV出力
        csvDownload(data);
    });
}

/**
 * CSVファイルダウンロード処理
 * @param {object} csvData サーバーから取得のデータ
 */
function csvDownload(csvData) {

    var bom = new Uint8Array([0xEF, 0xBB, 0xBF]);
    var blob = new Blob([bom, csvData], { "type": "text/csv" });

    // CSV名前の取得
    var now = wfCommon.getServerDatetime();
    var csvFile = "弔事連絡票照会一覧_" + moment(now).format(DATETIME_FORMAT_MOMENT_PATTERN_3) + ".csv";

    if (window.navigator.msSaveBlob) {

        // msSaveOrOpenBlobの場合はファイルを保存せずに開ける
        window.navigator.msSaveOrOpenBlob(blob, csvFile);
    } else {

        let link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = csvFile;
        link.click();
    }
}

/**
 * 絞り込み画面の「クリア」ボタンを押す処理
 */
function btnClearRunEvent() {

    // 入力項目のクリア
    $("#search-condition-area").find('input:text').each(function (index, element) {

        // テキスト
        var inputId = "#" + element.id;
        $(inputId).val(STRING_EMPTY);
    });

    // プルダウンリストのクリア
    $("#search-condition-area").find('select').each(function (index, element) {

        // プルダウンリストのクリア
        var inputId = "#" + element.id;
        $(inputId).parent().parent().parent()[0].__component.reset();
    });
}

/**
 * 絞り込み画面の「検索」ボタンを押す処理
 */
function btnSearchRunEvent() {

    // 入力条件の関連のチェック
    if (!checkRelation()) {
        return;
    }

    // 検索条件の取得
    var condition = getSearchCondition();

    // 入力項目のクリア
    createSearchShowArea(condition);

    // 検索条件入力エリアを閉じること
    $("#search-condition-area").removeClass("o-modal--opened");

    // サーバーをアクセスして、データを取得すること
    // 一覧の作成
    getDataWithServerAndCreatList();
}

/**
 * ボタン押下前、関連項目チェック
 * チェック正常だったら、trueを戻っていきます
 * チェック異常だったら、falseを戻っていきます
 */
function checkRelation() {

    // 所属コードや所属名が入力された場合、会社コードが必要になる
    if ((($("#belongs_code_search").val() !== null && $("#belongs_code_search").val() !== STRING_EMPTY) ||
        ($("#belongs_name_search").val() !== null && $("#belongs_name_search").val() !== STRING_EMPTY)) &&
        ($("#company_code_search").val() === null || $("#company_code_search").val() === STRING_EMPTY))
    {
        // 所属コードや所属名が入力された場合、会社コードが必要になる
        wfCommon.ShowDialog(
            DIALOG_ALERT,
            STRING_EMPTY,
            wfCommon.MsgFormat(msg["E0006"]),
            "はい"
        );

        return false;
    }

    // 初回申請日From、Toの両方に入力されている場合
    // Toの日付が、Fromより過去、Fromの日付が、Toより未来であった場合
    if (checkEndTime($("#app_date_search_from").val(), $("#app_date_search_to").val())) {
        // Toの日付が、Fromより過去、Fromの日付が、Toより未来であった場合
        // 有効な日付を入力してください。
        wfCommon.ShowDialog(
            DIALOG_ALERT,
            STRING_EMPTY,
            wfCommon.MsgFormat(msg["E0003"], $("#app_date_search_title").text()),
            "はい"
        );

        return false;
    }

    // 香料申請日From、Toの両方に入力されている場合
    // Toの日付が、Fromより過去、Fromの日付が、Toより未来であった場合
    if (checkEndTime($("#spice_app_date_search_from").val(), $("#spice_app_date_search_to").val())) {
        // Toの日付が、Fromより過去、Fromの日付が、Toより未来であった場合
        // 有効な日付を入力してください。
        wfCommon.ShowDialog(
            DIALOG_ALERT,
            STRING_EMPTY,
            wfCommon.MsgFormat(msg["E0003"], $("#spice_app_date_search_title").text()),
            "はい"
        );

        return false;
    }

    // 正常に終了
    return true;
}

/**
 * 検索項目入力条件の取得
 */
function getSearchCondition() {

    // 検索条件の取得
    var arrCondition = [];

    // 入力項目のクリア
    $("#search-condition-area").find('input:text').each(function (index, element) {

        // idの取得
        var inputId = "#" + element.id;

        // 表示条件のタイトル
        var condiTitle = STRING_EMPTY;
        // 表示条件の内容
        var condiVal = STRING_EMPTY;

        // 日付範囲項目の判断
        if (element.id.indexOf("_from") != -1) {

            // タイトルIDの取得
            var titleId = "#" + element.id.replace("_from", STRING_EMPTY) + "_title";

            // 終了日IDの取得
            var toId = "#" + element.id.replace("_from", "_to")

            // 開始日と終了日あるか判断
            if ($(inputId).val() !== STRING_EMPTY && $(toId).val() !== STRING_EMPTY) {

                // 表示条件のタイトルの取得
                condiTitle = $(titleId).text();
                // 表示条件の内容の取得
                condiVal = $(inputId).val() + "～" + $(toId).val();

                arrCondition.push(condiTitle + "：" + condiVal);

                // 開始日を変数に設定すること
                conditions.ObjArr[element.id] = $(inputId).val();
                // 終了日を変数に設定すること
                conditions.ObjArr[element.id.replace("_from", "_to")] = $(toId).val();
            }
            // 開始日と終了日あるか判断
            else if ($(inputId).val() !== STRING_EMPTY && $(toId).val() === STRING_EMPTY) {

                // 表示条件のタイトルの取得
                condiTitle = $(titleId).text();
                // 表示条件の内容の取得
                condiVal = $(inputId).val() + "～";

                arrCondition.push(condiTitle + "：" + condiVal);
                // 開始日を変数に設定すること
                conditions.ObjArr[element.id] = $(inputId).val();
            }
            // 開始日と終了日あるか判断
            else if ($(inputId).val() === STRING_EMPTY && $(toId).val() !== STRING_EMPTY) {

                // 表示条件のタイトルの取得
                condiTitle = $(titleId).text();
                // 表示条件の内容の取得
                condiVal = "～" + $(toId).val();

                arrCondition.push(condiTitle + "：" + condiVal);
                // 終了日を変数に設定すること
                conditions.ObjArr[element.id.replace("_from", "_to")] = $(toId).val();
            }
        }
        // 日付範囲項目は終了日が定義される場合、該当データをステップすること
        else if (element.id.indexOf("_to") != -1) {
            // 何もしない
        }
        // 日付範囲項目以外
        else {

            // 入力条件あるかの判断
            if ($(inputId).val() !== STRING_EMPTY) {

                // 表示条件のタイトルの取得
                condiTitle = $(inputId + "_title").text();
                // 表示条件の内容の取得
                condiVal = $(inputId).val();

                arrCondition.push(condiTitle + "：" + condiVal);

                // 検索条件を変数に設定すること
                conditions.ObjArr[element.id] = element.value;
            }
        }
    });

    // プルダウンリスト項目の設定値の取得
    $("#search-condition-area").find('select').each(function (index, element) {

        // 設定値有無の判定
        var selId = "#" + element.id;
        if ($(selId).val() !== null && $(selId).val() !== STRING_EMPTY) {

            // 選択値を条件表示アリアに設定すること
            var condiTitle = $(selId + "_title").text();
            var condiVal = $(selId).text();
            arrCondition.push(condiTitle + "：" + condiVal);

            // 検索条件を変数に設定すること
            conditions.ObjArr[element.id] = element.value;
        }
    });

    // 検索条件の戻り
    return arrCondition;
}

/**
 * 検索条件表示エリアを生成する
 * 
 * @param {Array} conditionDatas 一覧作成のid
 */
function createSearchShowArea(conditionDatas) {
 
    var searchOptionData = {
        data: conditionDatas,  // 検索条件入力のデータ
        sub_class: "a-tag a-tag--filled", // 1:"m-search-current-filters__tags" 2:"m-search-current-filters__wrap"  3:"m-search-current-filters__date" 4:"a-tag a-tag--filled"
        half_space: 0,  // 条件の間では半角空白でフォーマットを調整するため、半角空白数の設定
        // app_date_show: showAppDate,  // 絞り込み検索画面のみに適用すること（申請日の区間の設定）文字列
        onClickForClear: function () {

            // 条件クリア
            createList();

            // 絞り込み画面の「クリア」ボタンを押す処理
            btnClearRunEvent();
        }
    };

    // 検索条件表示エリアの作成
    $("#search-current-filters").createSearchCurrentFilters(searchOptionData);
}

/**
 * 日付データ表示フォーマット用
 */
function dateFormatter(dateDt) {

    var result = STRING_EMPTY;
    if (dateDt != undefined && dateDt != null && dateDt != STRING_EMPTY) {
        result = moment(dateDt, DATE_FORMAT_MOMENT_PATTERN_1).format(DATE_FORMAT_MOMENT_PATTERN_1);
    }
    else {
        result = "-";
    }
    return result;
}

/**
 * 日付From～To　チェック
 * 
 * @param {any} startTime
 * @param {any} endTime
 */
function checkEndTime(startTime, endTime) {

    if ((startTime !== null && startTime !== STRING_EMPTY) &&
        (endTime !== null && endTime !== STRING_EMPTY)) {
        var start = new Date(startTime);
        var end = new Date(endTime);

        if (end < start) {
            return true;
        }
    }

    return false;
}
/*▲関数定義エリア▲*/