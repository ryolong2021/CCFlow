/*▼グローバル変数定義エリア▼*/
var webUser = null;
var wfCommon = new wfCommon();                       //共通関連
var conditions = null;                               // 絞り込み検索条件array
var glcStsList = [];

//…追加変数

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {

    if (webUser == null)
        webUser = new WebUser();
    if (webUser.No == null)
        return;

    //画面初期化(非同期)
    InitPageForAsync();

    //…追加関数

});
  
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/

/**
 * 画面初期化(非同期)
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

    // glc手配状態一覧の取得
    getGlcStsList(data);

    // 一覧表示の作成
    createList();

    // イベント設定
    createEvent();

    // 日付入力パネルの設定
    setDatePicker();
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
 * イベント定義
 */
function createEvent() {

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

    // ログアウトボタンのリンクの設定
    $("#btn_logout").on("click", function () {
        location.href = "/?DoType=Out";
    });
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
 * 日付入力パネルの設定
 */
function setDatePicker() {

    // 検索条件 -- 申請日の設定
    wfCommon.setdatepickerWithStartEnd(
        "#app_date_search_from",
        "#app_date_search_to",
        DATE_FORMAT_MOMENT_PATTERN_1,
        true,
        false,
        function () { },
        function () { });
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
 * 絞り込み画面の「クリア」ボタンを押す処理
 */
function btnClearRunEvent() {

    // 入力項目のクリア
    $("#search-condition-area").find('input').each(function (index, element) {

        // テキスト、email、隠し項目
        if (element.type === "text" || element.type === "email" || element.type === "hidden" || element.type === "tel") {
            var inputId = "#" + element.id;
            $(inputId).val(STRING_EMPTY);
        }
        // `チェックボックスの場合
        else if (element.type === "checkbox") {

            // クリア
            $("input[name=" + element.name + "]").prop('checked', false);
        }
    });
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

    // 香料手配区分の取得
    var arrangeClass = getSearchConditionForCheckbox("spice_arrange_class_search");
    if (arrangeClass !== STRING_EMPTY) {
        arrCondition.push(arrangeClass);
    }

    // 検索条件の戻り
    return arrCondition;
}

/**
 * チェックボックス検索項目入力条件の取得
 */
function getSearchConditionForCheckbox(controlName) {

    // 検索条件の取得
    var conditionVal = STRING_EMPTY;

    $("input[name='" + controlName + "']").each(function (index, element) {

        // 検索条件の設定
        if (element.checked === true) {
            if (conditionVal === STRING_EMPTY) {
                conditionVal = $(this).next().children().next().text();
            } else {
                conditionVal += "," + $(this).next().children().next().text();
            }

            // チェックボックス選択肢を変数に設定すること
            conditions.ObjArr[element.name].push(element.value);
        }
    });

    // 選択肢がある場合、タイトルを取得すること
    if (conditionVal !== STRING_EMPTY) {
        return $("#" + controlName + "_title").text() + "：" + conditionVal;
    }
    else {
        return STRING_EMPTY;
    }
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

    // 香料手配区分条件の初期化
    conditions.ObjArr["spice_arrange_class_search"] = [];
}

/**
 * 検索条件でサーバーからデータを取得すること
 */
function getDataWithServerAndCreatList() {

    // 香料手配業者依頼一覧検索条件設定
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_GLCMaintenanceList");
    handler.AddUrlData();

    // 手配業者依頼一覧検索条件
    handler.AddPara("GlcMaintenanceReq", conditions.stringify());
    // 非同期データの取得
    handler.DoMethodSetString("GetGLCMaintencnceList", function (data) {
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
    var columns = [
        {
            field: 'AppCode',
            title: '申請番号',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'CompanyCode',
            title: '会社コード',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'CompanyName',
            title: '会社名',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'ApplicationName',
            title: '申請者氏名',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'ApplicationKana',
            title: '申請者カナ',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'JugyoinBango',
            title: '従業員番号',
            sortable: true,
            modify_type_class: 'type-data' // クラス種類  type-id / type-data / type-action
        },
        {
            field: 'ApplicationDate',
            title: '申請日',
            sortable: true,
            modify_type_class: 'type-data', // クラス種類  type-id / type-data / type-action
            formatter: function (res) {
                return dateFormatter(res.ApplicationDate);
            }
        },
        {
            field: 'FristSiseiDate',
            title: '初回申請日',
            sortable: true,
            modify_type_class: 'type-data', // クラス種類  type-id / type-data / type-action
            formatter: function (res) {
                return dateFormatter(res.FristSiseiDate);
            }
        },
        {
            field: 'LastUpdDate',
            title: '最終更新日',
            sortable: true,
            modify_type_class: 'type-data', // クラス種類  type-id / type-data / type-action
            formatter: function (res) {
                return dateFormatter(res.LastUpdDate);
            }
        },
        {
            field: 'KoryoKbn',
            title: '香料手配区分 ',
            sortable: true,
            modify_type_class: 'type-data', // クラス種類  type-id / type-data / type-action
            row_click_enable: false,  // 該当カラムは行クリックに応答するか設定  true/未設定:有効 false : 無効
            formatter: function (res) {
                return pulldownFormatter(res.KoryoKbn, res.WorkID, res.AppCode);
            }
        }
    ];

    // パラメータの設定
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

            // 詳細画面を表示すること
            $("#detail-screen").addClass("o-modal--opened");

            // 詳細画面を呼び出す関数設定
            var optionDetail = {
                oid: data.WorkID,
                applyNumber: data.AppCode,
                onOkClick: function (resSts) { // 保存ボタンクリック

                    // 画面データ再表示処理
                    // サーバーをアクセスして、データを取得すること
                    // 一覧の作成
                    getDataWithServerAndCreatList();
                }
            };

            // 詳細画面を呼び出す
            $().InitPageForDetail(optionDetail);
        }
    };

    // 一覧の作成
    $("#table").createTable(optionData);
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
 * プルダウンリストデータ表示フォーマット用
 * @param {string} koryoKbn 香料状態
 * @param {string} WorkID ワークID
 * @param {string} AppCode 申請番号
 */
function pulldownFormatter(koryoKbn, WorkID, AppCode) {

    // プルダウンリストタグの定義
    const pulldownDiv = '<div class="cp_ipselect cp_ipselect_glc cp_sl01">';
    const pulldownEndDiv = '</div>';
    const pulldownIconDiv = '<div class="a-pulldown__icon-container"><i class="a-icon a-icon--arrow-down"></i>' + pulldownEndDiv;

    // 非活性・活性
    var isdisabled = STRING_EMPTY;
    // options
    var option_jigiyosiu = STRING_EMPTY;
    var option_jitai = STRING_EMPTY;
    var option_glc = STRING_EMPTY;

    switch (String(koryoKbn)) {

        //事業所
        case KORYO_KBN_GIUMUSIYO:

            option_jigiyosiu = getOptionStr(0, true);
            //option_jitai = getOptionStr(1, false);
            option_glc = getOptionStr(2, false);

            break;

        //辞退
        case KORYO_KBN_JITAI:

            //option_jigiyosiu = getOptionStr(0, false);
            option_jitai = getOptionStr(1, true);
            option_glc = getOptionStr(2, false);

            break;

        //GLC
        case KORYO_KBN_GLC:

            isdisabled = 'disabled = "disabled"';
            //option_jigiyosiu = getOptionStr(0, false);
            //option_jitai = getOptionStr(1, false);
            option_glc = getOptionStr(2, true);

            break;

        default:

            //option_jigiyosiu = '<option value="0" >事業所</option>';
            option_jitai = getOptionStr(1, true);
            option_glc = getOptionStr(2, false);
            koryoKbn = 1;

            break;
    }

    return [
        pulldownDiv,
        '<select name="selectlist" id="selectlist' + WorkID + '" class="a-pulldown__select" ' + isdisabled + ' onchange="changeKoryokbn(this, \'' + AppCode + '\', \'' + WorkID + '\', \'' + koryoKbn + '\')">',
        option_jigiyosiu,
        option_jitai,
        option_glc,
        '</select>',
        // pulldownIconDiv,
        pulldownEndDiv
    ].join(STRING_EMPTY);
}

/**
 * 手配状態の取得
 * @param {object} data サーバーから取得のデータ
 **/
function getGlcStsList(data) {

    //区分マスタ格納
    dtKbn = data['KBN'];

    // 手配状態の取得
    glcStsList = dtKbn["GLCKORYOKBN"];
}

/**
 * 手配状態トロプダウンリストの取得の取得
 **/
function getOptionStr(optionKey, isSelected) {

    var retVal = '<option value="' + optionKey + '"';

    if (isSelected) {
        retVal += ' selected ';
    }

    retVal += '>' + glcStsList.find(obj => obj.KBNVALUE == optionKey).KBNNAME + '</option >';

    return retVal;
}

/**
 * 香料手配状態変更時の処理
 * @param {object} element イベント選択対象
 * @param {string} appCode 申請番号
 * @param {string} workId 申請番号
 * @param {string} befSelected 変更前の状態
 */
function changeKoryokbn(element, appCode, workId, befSelected) {

    // 確認ダイアログを出てくる
    wfCommon.ShowDialog(
        DIALOG_CONFIRM,
        STRING_EMPTY,
        // wfCommon.MsgFormat(msg["W0001"], element.options[element.options.selectedIndex].label + "にします。," + appCode),
        wfCommon.MsgFormat(msg["W0004"], appCode),
        "はい",
        exeConfirm,
        new Array(element.value, workId, appCode, befSelected),
        "いいえ"
    );

    setSelectVal(workId, befSelected);
} 

/**
 * プルダウンリスト選択肢変更してから、更新の処理
 * @param {string} selectVal 更新の状態
 * @param {string} workId ワークID
 * @param {string} appCode 申請番号
 * @param {string} chgBefSelectVal 変更前の手配状態
 */
function exeConfirm(selectVal, workId, appCode, chgBefSelectVal) {

    // ダイアログ画面を閉じること
    $("#app-dialog-div").removeClass("o-modal--opened");

    // 手配状態更新処理
    var ret = changeKoryoKbn(selectVal, workId, appCode, chgBefSelectVal);

    // 更新成功の場合、画面を再表示すること
    if (ret) {

        // 更新成功のダイアログを表示すること
        wfCommon.ShowDialog(
            DIALOG_ALERT,
            STRING_EMPTY,
            wfCommon.MsgFormat(msg["I0004"], appCode),
            "はい"
        );

        // サーバーをアクセスして、データを取得すること
        // 一覧の作成
        getDataWithServerAndCreatList();
    }
}

/**
 * 手配状態更新処理
 * @param {string} selectVal 更新の状態
 * @param {string} workId ワークID
 * @param {string} appCode 申請番号
 * @param {string} chgBefSelectVal 変更前の手配状態
 */
function changeKoryoKbn(selectVal, workId, appCode, chgBefSelectVal) {

    var ret = true;

    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_GLCMaintenanceList");
    handler.AddUrlData();
    //伝票番号を設定
    handler.AddPara("strOid", workId);
    //GLC香料変更区分
    handler.AddPara("glckoryokbn", selectVal);

    var data = handler.DoMethodReturnString("Koryo_States_Update");

    // 異常処理
    if (data.indexOf('err@') === 0) {
        wfCommon.ShowDialog(
            DIALOG_ALERT,
            STRING_EMPTY,
            data,
            "はい"
        );
        ret = false;
    }

    return ret;
}

/**
 * プルダウンリスト値の設定
 * @param {string} workId ワークId
 * @param {string} selectVal プルダウンリスト設定値
 */
function setSelectVal(workId, selectVal) {

    $("#selectlist" + workId + " option[value='" + selectVal + "']").prop('selected', true);
}

/**
 * ボタン押下前、関連項目チェック
 * チェック正常だったら、trueを戻っていきます
 * チェック異常だったら、falseを戻っていきます
 */
function checkRelation() {

    // 申請日From、Toの両方に入力されている場合
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

    // 正常に終了
    return true;
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