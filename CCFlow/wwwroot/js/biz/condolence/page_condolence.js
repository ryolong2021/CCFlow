/*▼グローバル変数定義エリア▼*/
var webUser = null;
var wfCommon = new wfCommon();                       // 共通関連
var dtKbn;                                           // 区分情報格納オブジェクト
var dtCondolence;                                    // 弔事連絡情報格納オブジェクト
var dtApplicant;                                     // 申請者情報格納オブジェクト
var dtDelegator;                                     // 委任者情報格納オブジェクト
var nextNode;                                        // 次ノード
var wfstate;                                         // フロー状態ステータス
var flowStater;            　　                      // フロー作成ユーザー
var serverDate;                                      // サーバ日付
var agentMode;                                       // 代理モード 0:本人　1:代理
var checkRefer;                                      // 参照用
var checkInput;                                      // 入力用
//…追加変数

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
(function ($) {
    $.fn.extend({
        "getElements": function () {
            let str = STRING_EMPTY;
            let els = this[0].elements;
            for (let i = 0; i < els.length; i++) {
                let el_type = els[i].type;
                if (el_type === "checkbox" || el_type === "radio") {
                    str += els[i].checked + ";"
                } else if (el_type === "text" || el_type === "email" || el_type === "select-one") {
                    str += els[i].value + ";"
                } 
            }
            return str;
        }
    });

    if (webUser === null)
        webUser = new WebUser();
    if (webUser.No === null)
        return;

    //画面初期化
    InitPage();

})(jQuery);
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/
/**
 * 画面初期化
 */
function InitPage() {

    var objGetTbl = {};
    objGetTbl[0] = "KBN";
    objGetTbl[1] = "TT_WF_CONDOLENCE";
    // 画面項目を初期設定
    wfCommon.initGetData(objGetTbl, iniPageItems);

}

/**
 * 画面項目を初期設定
 * 
 * @param {Array<object>} data サーバから取得のデータ配列
 */
function iniPageItems(data) {
    //console.log("iniPageItems start");
    // 初期データを設定
    serverDate = wfCommon.getServerDate();
    dtKbn = data["KBN"];
    dtCondolence = data["TT_WF_CONDOLENCE"][0];
    nextNode = dtCondolence["NEXTNODEID"];
    wfstate = dtCondolence["WFState"];
    flowStater = dtCondolence["FlowStarter"];
    agentMode = GetQueryString("AgentMode");
    if (agentMode === null) {
        agentMode = String(dtCondolence["SHINSEISYAKBN"]);
    }
    //console.log(data);

    // 郵便番号項目を初期化
    initPostcodeItems();
    // 日付項目を初期化
    initDateItems();
    // 代理項目を初期化
    initAgentItems();
    // 入力チェックを設定する
    setInputCheck();
    // イベント定義を設定する
    setEvents();
    // START PAGEにDATAをセット
    var ht = new HashTblCommon();
    // 社員番号
    ht.Add("SHAINBANGO", webUser.No);
    wfCommon.getApiInfoAjaxCallBack(GET_CONDOLENCE_SHAIN_INFO_APINAME, ht, setDataItems);

    //console.log("iniPageItems end");
}

/**
 * 初期設定：郵便番号項目
 * */
function initPostcodeItems() {

    // 通夜-郵便番号のコンポーネントを初期化
    $("#postal").jpostal({
        click: "#btn-wake-postcode",
        postcode: "#text-wake-postcode",
        address: {
            "#text-wake-address1": "%3%4",
            "#text-wake-address2": "%5"
        }
    });

    // 告別式-郵便番号のコンポーネントを初期化
    $("#postal").jpostal({
        click: "#btn-funeral-postcode",
        postcode: "#text-funeral-postcode",
        address: {
            "#text-funeral-address1": "%3%4",
            "#text-funeral-address2": "%5"
        }
    });

    // 後飾り-郵便番号のコンポーネントを初期化
    $("#postal").jpostal({
        click: "#btn-rear-postcode",
        postcode: "#text-rear-postcode",
        address: {
            "#text-rear-address1": "%3%4",
            "#text-rear-address2": "%5"
        }
    });

    wfCommon.setZipCodeInput("text-wake-postcode");
    wfCommon.setZipCodeInput("text-funeral-postcode");
    wfCommon.setZipCodeInput("text-rear-postcode");
}

/**
 * 初期設定：日付項目
 */
function initDateItems() {
    // 逝去日
    wfCommon.setdatepickerplus("#text-died-death_date", false, moment(serverDate).subtract(1, "years"), serverDate);
    // 通夜日付
    wfCommon.setdatepickerplus("#text-wake-date", false, serverDate, false);
    // 告別式日付
    wfCommon.setdatepickerplus("#text-funeral-date", false, serverDate, false);
    // お届け日付
    wfCommon.setdatepickerplus("#text-rear-date", false, serverDate, false);
}

/**
 * 初期設定：代理申請関する項目
 */
function initAgentItems() {
    
    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請の場合
        $(".p-title").text("弔事の連絡(代理申請)");
        $(".f-agent").show();
        $(".f-noagent").hide();
    } else { // 本人申請の場合
        $(".p-title").text("弔事の連絡(本人申請)");
        $(".f-agent").hide();
        $(".f-noagent").show();
    }
    
    // 修正モードの場合
    if (wfstate === WF_STATE_OVER) {
        $(".p-title").text("弔事の連絡(修正申請)");
    }

}

/**
 * 喪主をセット
 * */
function setMournerItems() {
    // 喪主エリアを設定
    setMournerArea();
    let value = dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"];
    // 喪主区分選択値を設定
    wfCommon.radiosSetVal("radio-mourner", value, ORGANIZER_KBN_HONNIN);

    if (value === ORGANIZER_KBN_HONNIN_IGAI) { // 自分以外
        // 喪主氏名（姓）
        $("#text-mourner-lastname").val(dtCondolence["ORGANIZER_SHIMEI_SEI"]);
        // 喪主氏名（名）
        $("#text-mourner-firstname").val(dtCondolence["ORGANIZER_SHIMEI_MEI"]);
        // 喪主カナ氏名（姓）
        $("#text-mourner-lastname_kana").val(dtCondolence["ORGANIZER_KANASHIMEI_SEI"]);
        // 喪主カナ氏名（名）
        $("#text-mourner-firstname_kana").val(dtCondolence["ORGANIZER_KANASHIMEI_MEI"]);
    }
}

/**
 * 喪主を設定
 *
 * */
function setMournerArea() {

    if (dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] === ORGANIZER_KBN_HONNIN_IGAI) { // 自分以外
        $(".area-mourner").show();
        // 喪主氏名（姓）
        $("#text-mourner-lastname").val(STRING_EMPTY);
        // 喪主氏名（名）
        $("#text-mourner-firstname").val(STRING_EMPTY);
        // 喪主カナ氏名（姓）
        $("#text-mourner-lastname_kana").val(STRING_EMPTY);
        // 喪主カナ氏名（名）
        $("#text-mourner-firstname_kana").val(STRING_EMPTY);

    } else { // 自分
        $(".area-mourner").hide();
    }
}

/**
 * 通夜/告別式についてをセット
 * */
function setWakeAndFuneralItems() {

    // 通夜/告別式エリアを設定
    setWakeAndFuneralArea();
    // 通夜/告別式選択値
    let value = String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]);
    // 通夜/告別式の有無について
    wfCommon.radiosSetVal("radio-funeral", value, FUNERAL_KBN_NONE);
    // 一般参列を辞退する
    $("#chk-funeral-decline").prop("checked", dtCondolence["SANRETSU_JITAI_KBN"] === CHECKBOX_CHECKED);
    // 通夜時刻
    wfCommon.initDropdown(true, wfCommon.getTimeData(15)["content"], dtCondolence["TSUYA_TIME"], "value", "name", "select-wake-time", "pulldown-wake-time");
    // 通夜開始時刻プルダウンの再検証
    $("#select-wake-time").on("change", function () {
        $("#form1").validate().element($("#select-wake-time"));
    });
    // 告別式時刻
    wfCommon.initDropdown(true, wfCommon.getTimeData(15)["content"], dtCondolence["KOKUBETSUSHIKI_TIME"], "value", "name", "select-funeral-time", "pulldown-funeral-time");
    // 告別式開始時刻プルダウンの再検証
    $("#select-funeral-time").on("change", function () {
        $("#form1").validate().element($("#select-funeral-time"));
    });
    if (value === FUNERAL_KBN_BOTH) { // 通夜と告別式
        // 通夜と同じ場所
        $("#chk-funeral-sameplace").prop("checked", dtCondolence["TSUYA_SAME_KBN"] === CHECKBOX_CHECKED);
        setWakeItems();
        setFuneralItems();
        // 表示・非表示
        changeFuneralItemsDisabled($("#chk-funeral-sameplace").prop("checked"));
    } else if (value === FUNERAL_KBN_TSUYA) { // 通夜
        setWakeItems();
    } else if (value === FUNERAL_KBN_KOKUBETSUSHIKI) { // 告別式
        setFuneralItems();
    }
}

/**
 * 通夜についてをセット
 * */
function setWakeItems() {
    // 通夜日付
    let tsuyadate = dtCondolence["TSUYA_DATE"];
    if (tsuyadate !== null) {
        $("#text-wake-date").val(moment(tsuyadate).format(DATE_FORMAT_MOMENT_PATTERN_1));
    }
    // 通夜会場名
    $("#text-wake-placeName").val(dtCondolence["TSUYA_BASYOUMEI"]);
    // 通夜会場名（カナ）
    $("#text-wake-placeName_kana").val(dtCondolence["TSUYA_BASYOUFURIGANA"]);
    // 通夜郵便番号
    $("#text-wake-postcode").val(dtCondolence["TSUYA_YUBINBANGO"]);
    // 通夜都道府県・市郡区
    $("#text-wake-address1").val(dtCondolence["TSUYA_ADDRESS1"]);
    // 通夜町村・番地
    $("#text-wake-address2").val(dtCondolence["TSUYA_ADDRESS2"]);
    // 通夜建物名・部屋番号
    $("#text-wake-address3").val(dtCondolence["TSUYA_ADDRESS3"]);
    // 通夜会場電話番号
    let wakeTel = dtCondolence["TSUYA_RENRAKUSAKITEL"];
    if (wakeTel !== null) {
        let wakeTels = wakeTel.split("-");
        $("#text-wake-tel1").val(wakeTels[0]);
        $("#text-wake-tel2").val(wakeTels[1]);
        $("#text-wake-tel3").val(wakeTels[2]);
        $("#text-wake-tel").val(wakeTel);
    }
}

/**
 * 告別式についてをセット
 * */
function setFuneralItems() {
    // 告別式日付
    let kokubetsushikidate = dtCondolence["KOKUBETSUSHIKI_DATE"];
    if (kokubetsushikidate !== null) {
        $("#text-funeral-date").val(moment(kokubetsushikidate).format(DATE_FORMAT_MOMENT_PATTERN_1));
    }
    // 告別式会場名
    $("#text-funeral-placeName").val(dtCondolence["KOKUBETSUSHIKI_BASYOUMEI"]);
    // 告別式会場名（カナ）
    $("#text-funeral-placeName_kana").val(dtCondolence["KOKUBETSUSHIKI_BASYOUFURIGANA"]);
    // 告別式郵便番号
    $("#text-funeral-postcode").val(dtCondolence["KOKUBETSUSHIKI_YUBINBANGO"]);
    // 告別式都道府県・市郡区
    $("#text-funeral-address1").val(dtCondolence["KOKUBETSUSHIKI_ADDRESS1"]);
    // 告別式町村・番地
    $("#text-funeral-address2").val(dtCondolence["KOKUBETSUSHIKI_ADDRESS2"]);
    // 告別式建物名・部屋番号
    $("#text-funeral-address3").val(dtCondolence["KOKUBETSUSHIKI_ADDRESS3"]);
    // 告別式会場電話番号
    let funeralTel = dtCondolence["KOKUBETSUSHIKI_RENRAKUSAKITEL"];
    if (funeralTel !== null) {
        let funeralTels = funeralTel.split("-");
        $("#text-funeral-tel1").val(funeralTels[0]);
        $("#text-funeral-tel2").val(funeralTels[1]);
        $("#text-funeral-tel3").val(funeralTels[2]);
        $("#text-funeral-tel").val(funeralTel);
    }
}

/**
 * 通夜/告別式を設定
 * 
 * */
function setWakeAndFuneralArea() {

    let value = String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]); 

    if (value === FUNERAL_KBN_BOTH) { // 通夜と告別式
        // 一般参列を辞退する:表示
        $("#area-decline").show();
        // 通夜エリア:表示
        $(".area-wake").show();
        // 告別式エリア:表示
        $(".area-funeral").show();
        // 通夜と同じ場所エリア:表示
        $("#area-sameplace").show();
    } else if (value === FUNERAL_KBN_TSUYA) { // 通夜
        // 一般参列を辞退する:表示
        $("#area-decline").show();
        // 通夜エリア:表示
        $(".area-wake").show();
        // 告別式エリア:非表示
        $(".area-funeral").hide();
    } else if (value === FUNERAL_KBN_KOKUBETSUSHIKI) { // 告別式
        // 一般参列を辞退する:表示
        $("#area-decline").show();
        // 通夜エリア:非表示
        $(".area-wake").hide();
        // 告別式エリア:表示
        $(".area-funeral").show();
        // 通夜と同じ場所エリア:表示
        $("#area-sameplace").hide();
    } else { // なし
        // 一般参列を辞退する:非表示
        $("#area-decline").hide();
        // 通夜エリア:非表示
        $(".area-wake").hide();
        // 告別式エリア:非表示
        $(".area-funeral").hide();
        // 一般参列を辞退する:クリア
        $("#chk-funeral-decline").prop("checked", false);
    }
}

/**
 * 供花届ける場所区分について
 * */
function setAllnightItems() {
    // 香料
    wfCommon.radiosSetVal("radio-opener-koryo", dtCondolence["KORYOKBN"], NECESSARY_KBN_HITUYOU);
    // 供花
    wfCommon.radiosSetVal("radio-opener-kuge", dtCondolence["KYOKAKBN"], NECESSARY_KBN_HITUYOU);
    // 弔電
    wfCommon.radiosSetVal("radio-opener-tyoden", dtCondolence["TYODENKBN"], NECESSARY_KBN_HITUYOU);

    // 供花届ける場所区分
    let value = dtCondolence["TODOKESAKIKBN"];
    // 通夜/告別式の有無エリアを設定
    setAllnightArea();
    // 供花届ける場所区分を設定
    wfCommon.radiosSetVal("radio-allnight", value, LOCATION_TSUYA_KBN);

    if (String(value) === LOCATION_ATOKA_KBN) { // 後飾り
        // 後飾り名前
        $("#text-rear-name").val(dtCondolence["ATOKAZARI_FULLNAME"]);
        // 後飾り郵便番号
        $("#text-rear-postcode").val(dtCondolence["ATOKAZARI_YUBINBANGO"]);
        // 後飾り都道府県・市郡区
        $("#text-rear-address1").val(dtCondolence["ATOKAZARI_ADDRESS1"]);
        // 後飾り町村・番地
        $("#text-rear-address2").val(dtCondolence["ATOKAZARI_ADDRESS2"]);
        // 後飾り建物名・部屋番号
        $("#text-rear-address3").val(dtCondolence["ATOKAZARI_ADDRESS3"]);
        // 後飾り会場電話番号
        let rearTel = dtCondolence["ATOKAZARI_RENRAKUSAKITEL"];
        if (rearTel !== null) {
            let rearTels = rearTel.split("-");
            $("#text-rear-tel1").val(rearTels[0]);
            $("#text-rear-tel2").val(rearTels[1]);
            $("#text-rear-tel3").val(rearTels[2]);
            $("#text-rear-tel").val(rearTel);
        }

        // 後飾りお届け日
        let atokazaridate = dtCondolence["ATOKAZARI_DATE"];
        if (atokazaridate !== null) {
            $("#text-rear-date").val(moment(atokazaridate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }
    }

    setAddresseeDisplay();
}

/**
 * 通夜/告別式の有無エリアを設定
 *
 * */
function setAllnightArea() {

    if (String(dtCondolence["TODOKESAKIKBN"]) === LOCATION_ATOKA_KBN) { // 後飾り
        $(".area-rear").show();
    } else { // 後飾り以外
        $(".area-rear").hide();
    }
}

/**
 * 告別式：通夜をコピー
 *
 * */
function copyWakeItemsToFuneral() {
    // 会場名
    $("#text-funeral-placeName").val($("#text-wake-placeName").val());
    // 会場名（カナ）
    $("#text-funeral-placeName_kana").val($("#text-wake-placeName_kana").val());
    // 郵便番号
    $("#text-funeral-postcode").val($("#text-wake-postcode").val());
    // 都道府県・市郡区
    $("#text-funeral-address1").val($("#text-wake-address1").val());
    // 町村・番地
    $("#text-funeral-address2").val($("#text-wake-address2").val());
    // 建物名・部屋番号
    $("#text-funeral-address3").val($("#text-wake-address3").val());
    // 会場電話番号
    $("#text-funeral-tel1").val($("#text-wake-tel1").val());
    $("#text-funeral-tel2").val($("#text-wake-tel2").val());
    $("#text-funeral-tel3").val($("#text-wake-tel3").val());
    $("#text-funeral-tel").val($("#text-wake-tel").val());
}

/**
 * 通夜：項目値をクリア
 *
 * */
function clearWakeItems() {
    // 日付
    $("#text-wake-date").val(STRING_EMPTY);
    // 開始時刻
    $("#pulldown-wake-time")[0].__component.reset();
    // 会場名
    $("#text-wake-placeName").val(STRING_EMPTY);
    // 会場名（カナ）
    $("#text-wake-placeName_kana").val(STRING_EMPTY);
    // 郵便番号
    $("#text-wake-postcode").val(STRING_EMPTY);
    // 都道府県・市郡区
    $("#text-wake-address1").val(STRING_EMPTY);
    // 町村・番地
    $("#text-wake-address2").val(STRING_EMPTY);
    // 建物名・部屋番号
    $("#text-wake-address3").val(STRING_EMPTY);
    // 会場電話番号
    $("#text-wake-tel1").val(STRING_EMPTY);
    $("#text-wake-tel2").val(STRING_EMPTY);
    $("#text-wake-tel3").val(STRING_EMPTY);
    $("#text-wake-tel").val(STRING_EMPTY);
    // 通夜と同じ場所
    $("#chk-funeral-sameplace").prop("checked", false);
}

/**
 * 告別式：項目値をクリア
 *
 * */
function clearFuneralItems() {
    // 日付
    $("#text-funeral-date").val(STRING_EMPTY);
    // 開始時刻
    $("#pulldown-funeral-time")[0].__component.reset();
    // 会場名
    $("#text-funeral-placeName").val(STRING_EMPTY);
    // 会場名（カナ）
    $("#text-funeral-placeName_kana").val(STRING_EMPTY);
    // 郵便番号
    $("#text-funeral-postcode").val(STRING_EMPTY);
    // 都道府県・市郡区
    $("#text-funeral-address1").val(STRING_EMPTY);
    // 町村・番地
    $("#text-funeral-address2").val(STRING_EMPTY);
    // 建物名・部屋番号
    $("#text-funeral-address3").val(STRING_EMPTY);
    // 会場電話番号
    $("#text-funeral-tel1").val(STRING_EMPTY);
    $("#text-funeral-tel2").val(STRING_EMPTY);
    $("#text-funeral-tel3").val(STRING_EMPTY);
    $("#text-funeral-tel").val(STRING_EMPTY);
}

/**
 * 告別式：活性・非活性を設定
 *
 * @param {boolean} value 通夜と同じ場所の選択値
 * */
function changeFuneralItemsDisabled(value) {
    // 郵便番号 検索
    $("#btn-funeral-postcode").attr("disabled", value);
    // 会場名
    $("#text-funeral-placeName").attr("disabled", value);
    // 会場名（カナ）
    $("#text-funeral-placeName_kana").attr("disabled", value);
    // 郵便番号
    $("#text-funeral-postcode").attr("disabled", value);
    // 都道府県・市郡区
    $("#text-funeral-address1").attr("disabled", value);
    // 町村・番地
    $("#text-funeral-address2").attr("disabled", value);
    // 建物名・部屋番号
    $("#text-funeral-address3").attr("disabled", value);
    // 会場電話番号
    $("#text-funeral-tel1").attr("disabled", value);
    $("#text-funeral-tel2").attr("disabled", value);
    $("#text-funeral-tel3").attr("disabled", value);
}

/**
 * イベントを設定する
 */
function setEvents() {

    // 扶養ヘルプボタン
    $("#help-opener-huyo").on("click", function () {
        let modalHelp = $("#modal-help-huyo")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // 香料ヘルプボタン
    $("#help-opener-koryo").on("click", function () {
        let modalHelp = $("#modal-help-koryo")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // 供花ヘルプボタン
    $("#help-opener-kuge").on("click", function () {
        let modalHelp = $("#modal-help-kuge")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // 弔電ヘルプボタン
    $("#help-opener-tyoden").on("click", function () {
        let modalHelp = $("#modal-help-tyoden")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // 通夜/告別式の有無ヘルプボタン
    $("#help-opener").on("click", function () {
        let modalHelp = $("#modal-help")[0].__component;
        modalHelp.opened = !0;
        modalHelp.onCloseRequested = function () {
            return modalHelp.opened = !1
        }
    });

    // PDF表示ボタン
    $("#link-page-pdf").on("click", function () {

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
        handler.AddUrlData();
        // 会社コード
        handler.AddPara("kaishacode", dtApplicant["KAISHACODE"]);
        handler.DoMethodSetString("Get_PDF_Info", function (data) {
            //例外処理
            if (data.indexOf("err@") === 0) {
                wfCommon.Msgbox(data);
                return;
            }
            // JSON対象に転換
            var jsondata = JSON.parse(data);
            dtCondolence["PDF_URL"] = jsondata["Get_PDF_Info"];

            if (dtCondolence["PDF_URL"] === STRING_EMPTY) {

                $("#link-page-pdf").hide();
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0012"], "PDFファイル"));
            } else {

                $("#link-page-pdf").show();
                $("#iframe-pdf").attr("src", dtCondolence["PDF_URL"]);


                let pdfpage = $("#pdf-page")[0].__component;
                pdfpage.opened = !0;
                pdfpage.onCloseRequested = function () {
                    return pdfpage.opened = !1
                }
            }
        });

    });

    // 委任者の社員番号
    $("#text-unhappiness-shainbango").on("change", function () {
        // 弔事基準情報をセット
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });

    // 喪主ラジオボタン
    $("input[name=radio-mourner]").on("change", function () {
        dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] = this.value;
        setMournerArea();
        // 弔事基準情報をセット
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });

    // 通夜/告別式ラジオボタン
    $("input[name=radio-funeral]").on("change", function () {
        dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"] = this.value;
        setWakeAndFuneralArea();
        clearWakeItems();
        clearFuneralItems();
        // 表示・非表示
        changeFuneralItemsDisabled(false);
    });

    // お届け先ラジオボタン
    $("input[name=radio-allnight]").on("change", function () {
        dtCondolence["TODOKESAKIKBN"] = this.value;
        setAllnightArea();
    });

    // 通夜と同じ場所
    $("#chk-funeral-sameplace").on("change", function () {

        let value = $(this).prop("checked");

        if (value) {
            // コピー
            copyWakeItemsToFuneral();
        } else {
            // クリア
            clearFuneralItems();
        }
        // 表示・非表示
        //changeFuneralItemsDisabled(value);
    });

    // 中止ボタン
    $("#btn-page-close").on("click", function () {
        // ダイアログボックス
        wfCommon.ShowDialog(DIALOG_CONFIRM, STRING_EMPTY, msg["W0005"], null, backFromPage);
    });

    // 下書きボタン
    $("#btn-form-draft_save").on("click", function () {
        // 一時保存処理を行います
        var data = saveLogic(BTN_TEMPORARILY_SAVE);

        // 例外処理
        if (data.indexOf("err@") === 0) {
            wfCommon.Msgbox(data);
            return;
        }

        // 親画面（申請メニュー）に戻ること
        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, data, null, backFromPage);
        
    });

    // 確認ボタン
    $("#btn-form-confirm").on("click", function () {
        if (wfstate > WF_STATE_DRAFT) {
            checkInput = $("#form1").getElements();
            if (checkInput === checkRefer) {
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0010"]);
                return;
            } 
        }

        // チェックを実施する
        // 単項目チェック
        let flg = $("#form1").valid();
        if (!flg) {
            return;
        }

        // 整合性チェック : 通夜/告別式/後飾り
        let allnight = $("input[name=radio-allnight]:checked").val();
        // 供花区分
        let kugekbn = $("input[name=radio-opener-kuge]:checked").val();
        // 弔電区分
        let tyodenkbn = $("input[name=radio-opener-tyoden]:checked").val();
        // 通夜/告別式/なし
        let funeral = $("input[name=radio-funeral]:checked").val();
        // 弔事基準の給付内容あり
        if ($(".arrange-no").is(":hidden")) {
            // 供花 または　弔電　何れか受け取る場合
            if (kugekbn === NECESSARY_KBN_HITUYOU || tyodenkbn === NECESSARY_KBN_HITUYOU) {
                // 通夜
                if (allnight === LOCATION_TSUYA_KBN) {
                    if (!(funeral === FUNERAL_KBN_TSUYA || funeral === FUNERAL_KBN_BOTH)) {
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0007"]);
                        return
                    }
                }
                // 告別式
                else if (allnight === LOCATION_KOKUBETSUSHIKI_KBN) {
                    if (!(funeral === FUNERAL_KBN_KOKUBETSUSHIKI || funeral === FUNERAL_KBN_BOTH)) {
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0007"]);
                        return;
                    }
                }
            }
        }

        // 整合性チェック : 逝去日/通夜日/告別式日/後飾り日
        // 逝去日
        let deathdatetime = moment($("#text-died-death_date").val() + HALF_SPACE + $("#select-died-death_time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
        if (deathdatetime.isAfter(moment(wfCommon.getServerDatetime(), DATETIME_FORMAT_MOMENT_PATTERN_1))){
            wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "逝去日,未来日"));
            return;
        }
        // 通夜エリア　表示の場合
        if ($(".area-wake").is(":visible")) {
            // 通夜日
            let wakedatetime = moment($("#text-wake-date").val() + HALF_SPACE + $("#select-wake-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
            if (deathdatetime.isAfter(wakedatetime)) {
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "通夜日,逝去日の前"));
                return;
            }
        }
        // 告別式エリア　表示の場合
        if ($(".area-funeral").is(":visible")) {
            // 告別式日
            let funeraldatetime = moment($("#text-funeral-date").val() + HALF_SPACE + $("#select-funeral-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
            if (deathdatetime.isAfter(funeraldatetime)) {
                wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "告別式日,逝去日の前"));
                return;
            }
        }
        // 弔事基準エリア　表示の場合
        if ($("#label-standard-info3").is(":hidden")) {
            // 供花お届け先場所：通夜を選択場合
            if (allnight === LOCATION_TSUYA_KBN) {
                // 通夜日
                let wakedatetime = moment($("#text-wake-date").val() + HALF_SPACE + $("#select-wake-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                let serverdatetime = moment(wfCommon.getServerDatetime(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                if (serverdatetime.isAfter(wakedatetime)) {

                    if (dtCondolence["WFState"] === WF_STATE_OVER) {
                        // 葬儀場への手配はできません。後飾りを選択ください。
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["E0007"]);
                    } else {
                        // 通夜日に過去日の日付は指定できません。
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "通夜日,過去日"));
                    }
                    return
                }
            }
            // 供花お届け先場所：告別式を選択場合
            if (allnight === LOCATION_KOKUBETSUSHIKI_KBN) {
                // 告別式日
                let funeraldatetime = moment($("#text-funeral-date").val() + HALF_SPACE + $("#select-funeral-time").val(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                let serverdatetime = moment(wfCommon.getServerDatetime(), DATETIME_FORMAT_MOMENT_PATTERN_1);
                if (serverdatetime.isAfter(funeraldatetime)) {
                    if (dtCondolence["WFState"] === WF_STATE_OVER) {
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["E0007"]);
                    } else {
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "告別式日,過去日"));
                    }
                    return
                }
            }
            // 供花お届け先場所：後飾りを選択場合
            if (allnight === LOCATION_ATOKA_KBN) {
                // 後飾り日
                let reardate = moment($("#text-rear-date").val(), DATE_FORMAT_MOMENT_PATTERN_1);
                if (reardate.isBefore(moment(serverDate, DATE_FORMAT_MOMENT_PATTERN_1))) {
                    wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, wfCommon.MsgFormat(msg["I0008"], "お届け日,過去日"));
                    return;
                }
            }

        }

        // 修正するときに、供花と弔電は両方選択のチェック
        if (wfstate === WF_STATE_OVER) { // 修正の場合
            if (dtCondolence["KUGE_GOUKEI"] > 0 && dtCondolence["TYOUDEN_GOUKEI"] > 0) { //供花と弔電は使用可の場合
                if (dtCondolence["KYOKAKBN"] === parseInt(NECESSARY_KBN_HITUYOU) && dtCondolence["TYODENKBN"] === parseInt(NECESSARY_KBN_HITUYOU)) { // 供花と弔電はスナップショットカラムに受け取るの場合
                    if ($("input[name=radio-opener-kuge]:checked").val() !== $("input[name=radio-opener-tyoden]:checked").val()) { // 供花と弔電は選択値が違う場合
                        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["E0008"]);
                        return;
                    }
                }
            }
        }

        // 重複チェック
        var count = doubleCheck();
        if (Number(count["Check_Double_Info"]) > 0) {
            wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0009"]);
            return;
        }

        // 確認画面
        gotoConfirmPage();

    });

    // 戻るボタン
    $("#btn-form-back").on("click", function () {
        // 入力画面へ遷移
        $("#confirm-page")[0].__component.opened = !1;
    });

    // 戻るボタン
    $("#btn-other-back").on("click", function () {
        // 親画面へ遷移
        backFromPage();
    });

    // 検索ボタン
    $("#btn-search-empcode").on("click", function () {
        // 社員番号　チェック
        let flg = $("#text-unhappiness-shainbango").valid();

        if (flg) {
            setDelegatorInfo();
        }

    });

    // 申請ボタン
    $("#btn-form-request").on("click", function () {
        // 申請処理を行います、申請された内容をDBに登録する
        saveCore(BTN_SUB_SUBMIT);
    });

    // 修正ボタン
    $("#btn-form-modify").on("click", function () {
        // 修正処理を行います、申請された内容をDBに登録する
        saveCore(BTN_EDIT);
    });

    // 供花ラジオボタン
    $("input[name=radio-opener-kuge]").on("change", function () {
        // お届け先情報エリア
        setAddresseeDisplay();
    });

    // 弔電ラジオボタン
    $("input[name=radio-opener-tyoden]").on("change", function () {
        // お届け先情報エリア
        setAddresseeDisplay();
    });

    // 連絡の取れる電話番号
    wfCommon.initTelEvent("text-contact-tel");
    // 通夜電話番号
    wfCommon.initTelEvent("text-wake-tel");
    // 告別式電話番号
    wfCommon.initTelEvent("text-funeral-tel");
    // 後飾り電話番号
    wfCommon.initTelEvent("text-rear-tel");
}

/**
 * 遷移先を指定
 * */
function backFromPage() {

    let pagename = GetQueryString("FromPage");

    if (pagename === null) {
        pagename = "form_applymenu.html";
    }

    window.location.href = "../../../pages/biz/menu/" + pagename;

}  

/**
 * 保存・自動送信
 * @param {number} key　実行モード
 * */
function saveCore(key) {
    var data = saveLogic(key);

    // 例外処理
    if (data.indexOf("err@") === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 自動送信処理
    if (key === BTN_SUB_SUBMIT) {    // 申請の場合
        wfCommon.DoMailSend(AUTO_MAIL_CLASS.CONDOLENCE, MAIL_TYPE_CONDOLENCE, MAIL_STSTE_NEW, GetQueryString("WorkID"));

    } else if (key === BTN_EDIT) {        // 申請修正の場合
        wfCommon.DoMailSend(AUTO_MAIL_CLASS.CONDOLENCE, MAIL_TYPE_CONDOLENCE, MAIL_STSTE_EDIT, GetQueryString("WorkID"));
    }

    // 親画面（申請メニュー画面）に戻る
    wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, data, null, backFromPage);
}

/**
  * 画面に入力した情報を登録情報変数に設定すること
  * @param {number} key　実行モード
  *                       パラメータ説明
  *                       １：提出
  *                       ２：一時保存
  *                       ３：引戻
  *                       ４：承認
  *                       ５：否認
  *                       ６：差戻
  *                       ７：修正
  */
function saveLogic(key) {

    // バック対象の作成
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
    // URLの追加
    handler.AddUrlData();

    // 弔事連絡情報設定
    saveMainTblData(handler, key);

    // サブテーブル情報設定
    saveSubListData(handler);

    // 共通対象の追加
    saveCommonData(handler, key);

    // 設定後の内容をバックに送信すること
    data = handler.DoMethodReturnString("Runing_Send");

    return data;
}

/**
  * バックと連携する共通設定
  * @param {HashTblCommon} handler　設定情報対象
  * @param {any} key　実行モード
  */
function saveCommonData(handler, key) {

    var ht = new HashTblCommon();

    //oidの値と同じ
    ht.Add("WorkID", GetQueryString("WorkID"));
    //ノードID
    ht.Add("FK_Node", GetQueryString("FK_Node"));
    //フローID
    ht.Add("FK_Flow", GetQueryString("FK_Flow"));
    //ログインユーザーID
    ht.Add("UserNo", GetQueryString("UserNo"));
    //SIDのこと
    ht.Add("SID", GetQueryString("SID"));
    //次のノードID
    ht.Add("NextNode", nextNode);
    // 自動承認モード
    ht.Add("AutoApprovalMode", MODE_KBN_YES);
    // 自動承認者
    ht.Add("toEmps", AUTO_APPROVAL_ID);
    // 申請区分
    ht.Add("AgentMode", agentMode);
    // 実行モード
    ht.Add("Mode", key);
    handler.AddPara("CommonData", ht.stringify());
}

/**
  * 弔事連絡情報設定
  * @param {HashTblCommon} handler　バックと連携する対象
  * @param {any} key　実行モード
  */
function saveMainTblData(handler, key) {

    var ht = new HashTblCommon();
    // 初期設定
    initMainTblData(ht);
    // データ設定
    setMainTblData(ht, key);
    //TT_WF_CONDOLENCEテーブル格納
    handler.AddPara("MainTblName", "TT_WF_CONDOLENCE");
    handler.AddPara("TT_WF_CONDOLENCE", ht.stringify());
}

/**
  * バックと連携するサブ共通設定
  * @param {HashTblCommon} handler　設定情報対象
  */
function saveSubListData(handler) {

    // 更新のテーブル名
    var opjTbNm = {};
    handler.AddPara("SubListTblName", JSON.stringify(opjTbNm));
}

/**
  * 弔事連絡情報初期化設定
  * @param {HashTblCommon} ht　バックと連携する対象
  * @param {any} key　実行モード
  */
function initMainTblData(ht, key) {

    if (key === BTN_SUB_SUBMIT || key === BTN_TEMPORARILY_SAVE) { // 提出　または　下書き
        // 伝票番号 - 空白設定
        ht.Add("OID", STRING_EMPTY);
        // 申請者区分 - 空白設定
        ht.Add("SHINSEISYAKBN", STRING_EMPTY);
        // 代理申請者社員番号 - 空白設定
        ht.Add("DAIRISHINSNEISYA_SHAINBANGO", STRING_EMPTY);
        // (スナップショット)代理申請社員名(漢字) - 空白設定
        ht.Add("DAIRISHINSNEISYA_MEI", STRING_EMPTY);
        // (スナップショット)代理申請社員名(フリガナ) - 空白設定
        ht.Add("DAIRISHINSNEISYA_FURIGANAMEI", STRING_EMPTY);
        // (スナップショット)代理申請社員所属コード - 空白設定
        ht.Add("DAIRISHINSNEISYA_SYOZOKUCODE", STRING_EMPTY);
        // (スナップショット)代理申請社員所属名 - 空白設定
        ht.Add("DAIRISHINSNEISYA_SYOZOKU", STRING_EMPTY);
        // 出向フラグ - 空白設定
        ht.Add("SHUKKOFLG", STRING_EMPTY);
        // チームコード  - 空白設定
        ht.Add("TEAMCODE", STRING_EMPTY);
        // チーム名  - 空白設定
        ht.Add("TEAMMEISHO", STRING_EMPTY);
        // 財務部署コード - 空白設定
        ht.Add("ZAIMUBUSHOCODE", STRING_EMPTY);
        // 経費負担会社コード - 空白設定
        ht.Add("KEIHIFUTANKAISHACODE", STRING_EMPTY);
        // 経費負担会社名 - 空白設定
        ht.Add("KEIHIFUTANKAISHAMEI", STRING_EMPTY);
        // 不幸従業員社員番号 - 空白設定
        ht.Add("UNFORTUNATE_SHAINBANGO", STRING_EMPTY);
        // (スナップショット)不幸従業員の社員名(フリガナ) - 空白設定
        ht.Add("UNFORTUNATE_FURIGANAMEI", STRING_EMPTY);
        // (スナップショット)不幸従業員の社員名(漢字) - 空白設定
        ht.Add("UNFORTUNATE_KANJIMEI", STRING_EMPTY);
        // (スナップショット)不幸従業員の出向元会社コード - 空白設定
        ht.Add("UNFORTUNATE_KAISYACODE", STRING_EMPTY);
        // (スナップショット)不幸従業員の出向元会社名 - 空白設定
        ht.Add("UNFORTUNATE_KAISYAMEI", STRING_EMPTY);
        // (スナップショット)不幸従業員の出向先会社コード - 空白設定
        ht.Add("UNFORTUNATE_SYUKOSAKIKAISYACODE", STRING_EMPTY);
        // (スナップショット)不幸従業員の出向先会社名 - 空白設定
        ht.Add("UNFORTUNATE_SYUKOSAKIKAISYAMEI", STRING_EMPTY);
        // (スナップショット)不幸者の所属コード - 空白設定
        ht.Add("UNFORTUNATE_SYOZOKUCODE", STRING_EMPTY);
        // (スナップショット)不幸従業員の正式組織・上 - 空白設定
        ht.Add("UNFORTUNATE_SEISHIKISOSHIKIUE", STRING_EMPTY);
        // (スナップショット)不幸従業員の下 - 空白設定
        ht.Add("UNFORTUNATE_SEISHIKISOSHIKISHITA", STRING_EMPTY);
        // (スナップショット)不幸従業員の社員区分コード - 空白設定
        ht.Add("UNFORTUNATE_SYAINNKBNCODE", STRING_EMPTY);
        // (スナップショット)不幸従業員の社員区分名 - 空白設定
        ht.Add("UNFORTUNATE_SYAINNKBN", STRING_EMPTY);
        // (スナップショット)不幸従業員の職位名 - 空白設定
        ht.Add("UNFORTUNATE_SYOKUIKBN", STRING_EMPTY);
        // (スナップショット)不幸従業員の組合区分コード - 空白設定
        ht.Add("UNFORTUNATE_KUMIAIKBNCODE", STRING_EMPTY);
        // (スナップショット)不幸従業員の組合区分名 - 空白設定
        ht.Add("UNFORTUNATE_KUMIAIKBN", STRING_EMPTY);
        // (スナップショット)不幸従業員のグッドライフ区分コード - 空白設定
        ht.Add("UNFORTUNATE_GLCKBNCODE", STRING_EMPTY);
        // (スナップショット)不幸従業員のグッドライフ区分名 - 空白設定
        ht.Add("UNFORTUNATE_GLCKBN", STRING_EMPTY);
        // 亡くなられた方カナ氏名（姓） - 空白設定
        ht.Add("DEAD_KANASHIMEI_SEI", STRING_EMPTY);
        // 亡くなられた方カナ氏名（名） - 空白設定
        ht.Add("DEAD_KANASHIMEI_MEI", STRING_EMPTY);
        // 亡くなられた方氏名（姓） - 空白設定
        ht.Add("DEAD_SHIMEI_SEI", STRING_EMPTY);
        // 亡くなられた方氏名（名） - 空白設定
        ht.Add("DEAD_SHIMEI_MEI", STRING_EMPTY);
        // 亡くなられた方続柄区分  - 空白設定
        ht.Add("DEAD_JUGYOIN_ZOKUGARAKBN", STRING_EMPTY);
        // 亡くなられた方性別 - 空白設定
        ht.Add("DEAD_SEIBETSU", STRING_EMPTY);
        // 亡くなられた方同居別居区分 - 空白設定
        ht.Add("DEAD_DOKYO_BEKYO", STRING_EMPTY);
        // 亡くなられた方年齢 - 空白設定
        ht.Add("DEAD_NENREI", STRING_EMPTY);
        // 亡くなられた方逝去日 - 空白設定
        ht.Add("DEAD_DATE", STRING_EMPTY);
        // 亡くなられた方逝去日 - 空白設定
        ht.Add("DEAD_TIME", STRING_EMPTY);
        // 亡くなられた方扶養区分 - 空白設定
        ht.Add("DEAD_FUYOUKBN", STRING_EMPTY);
        // 喪主区分 - 空白設定
        ht.Add("ORGANIZER_JUGYOIN_ZOKUGARAKBN", STRING_EMPTY);
        // 喪主カナ氏名（姓） - 空白設定
        ht.Add("ORGANIZER_KANASHIMEI_SEI", STRING_EMPTY);
        // 喪主カナ氏名（名） - 空白設定
        ht.Add("ORGANIZER_KANASHIMEI_MEI", STRING_EMPTY);
        // 喪主氏名（姓） - 空白設定
        ht.Add("ORGANIZER_SHIMEI_SEI", STRING_EMPTY);
        // 喪主氏名（名） - 空白設定
        ht.Add("ORGANIZER_SHIMEI_MEI", STRING_EMPTY);
    }

    // 申請者連絡先電話番号 - 空白設定
    ht.Add("RENRAKUSAKITEL", STRING_EMPTY);
    // 申請者連絡先メール - 空白設定
    ht.Add("RENRAKUSAKIMAIL", STRING_EMPTY);
    // 通夜/告別式区分 - 空白設定
    ht.Add("TSUYA_KOKUBETSUSHIKIKBN", STRING_EMPTY);
    // 通夜会場フリガナ - 空白設定
    ht.Add("TSUYA_BASYOUFURIGANA", STRING_EMPTY);
    // 通夜会場名  - 空白設定
    ht.Add("TSUYA_BASYOUMEI", STRING_EMPTY);
    // 通夜郵便番号 - 空白設定
    ht.Add("TSUYA_YUBINBANGO", STRING_EMPTY);
    // 通夜都道府県・市郡区 - 空白設定
    ht.Add("TSUYA_ADDRESS1", STRING_EMPTY);
    // 通夜町村番地 - 空白設定
    ht.Add("TSUYA_ADDRESS2", STRING_EMPTY);
    // 通夜マンション名 - 空白設定
    ht.Add("TSUYA_ADDRESS3", STRING_EMPTY);
    // 通夜日付 - 空白設定
    ht.Add("TSUYA_DATE", STRING_EMPTY);
    // 通夜時刻 - 空白設定
    ht.Add("TSUYA_TIME", STRING_EMPTY);
    // 通夜連絡先電話番号 - 空白設定
    ht.Add("TSUYA_RENRAKUSAKITEL", STRING_EMPTY);
    // 告別式会場フリガナ - 空白設定
    ht.Add("KOKUBETSUSHIKI_BASYOUFURIGANA", STRING_EMPTY);
    // 告別式会場名 - 空白設定
    ht.Add("KOKUBETSUSHIKI_BASYOUMEI", STRING_EMPTY);
    // 告別式郵便番号 - 空白設定
    ht.Add("KOKUBETSUSHIKI_YUBINBANGO", STRING_EMPTY);
    // 告別式都道府県・市郡区 - 空白設定
    ht.Add("KOKUBETSUSHIKI_ADDRESS1", STRING_EMPTY);
    // 告別式町村番地 - 空白設定
    ht.Add("KOKUBETSUSHIKI_ADDRESS2", STRING_EMPTY);
    // 告別式マンション名 - 空白設定
    ht.Add("KOKUBETSUSHIKI_ADDRESS3", STRING_EMPTY);
    // 告別式日付 - 空白設定
    ht.Add("KOKUBETSUSHIKI_DATE", STRING_EMPTY);
    // 告別式時刻 - 空白設定
    ht.Add("KOKUBETSUSHIKI_TIME", STRING_EMPTY);
    // 告別式連絡先電話番号 - 空白設定
    ht.Add("KOKUBETSUSHIKI_RENRAKUSAKITEL", STRING_EMPTY);
    // 通夜と同じ区分 - 空白設定
    ht.Add("TSUYA_SAME_KBN", STRING_EMPTY);
    // 一般参列を辞退する区分 - 空白設定
    ht.Add("SANRETSU_JITAI_KBN", STRING_EMPTY);
    // 供花届ける場所区分 - 空白設定
    ht.Add("TODOKESAKIKBN", STRING_EMPTY);
    // 後飾り名前 - 空白設定
    ht.Add("ATOKAZARI_FULLNAME", STRING_EMPTY);
    // 後飾り郵便番号 - 空白設定
    ht.Add("ATOKAZARI_YUBINBANGO", STRING_EMPTY);
    // 後飾り都道府県・市郡区 - 空白設定
    ht.Add("ATOKAZARI_ADDRESS1", STRING_EMPTY);
    // 後飾り町村番地 - 空白設定
    ht.Add("ATOKAZARI_ADDRESS2", STRING_EMPTY);
    // 後飾りマンション名 - 空白設定
    ht.Add("ATOKAZARI_ADDRESS3", STRING_EMPTY);
    // 後飾り電話番号 - 空白設定
    ht.Add("ATOKAZARI_RENRAKUSAKITEL", STRING_EMPTY);
    // 後飾り日付 - 空白設定
    ht.Add("ATOKAZARI_DATE", STRING_EMPTY);
    // 香料発行区分 - 空白設定
    ht.Add("KORYOKBN", STRING_EMPTY);
    // GLC香料変更区分 - 空白設定
    ht.Add("GLCKORYOKBN", STRING_EMPTY);
    // 供花発行区分 - 空白設定
    ht.Add("KYOKAKBN", STRING_EMPTY);
    // 弔電発行区分 - 空白設定
    ht.Add("TYODENKBN", STRING_EMPTY);
    // 香料合計 - 空白設定
    ht.Add("KOURYOU_GOUKEI", STRING_EMPTY);
    // 供花合計 - 空白設定
    ht.Add("KUGE_GOUKEI", STRING_EMPTY);
    // 弔電合計 - 空白設定
    ht.Add("TYOUDEN_GOUKEI", STRING_EMPTY);
    // 香料申請日 - 空白設定
    ht.Add("KOURYOU_SHINNSEIBI", STRING_EMPTY);
    // 供花・弔電申請日 - 空白設定
    ht.Add("KUGE_TYOUDEN_SHINNSEIBI", STRING_EMPTY);
    // 香料・供花・弔電差出人_会社名（出向元） - 空白設定
    ht.Add("SASHIDASHI_MOTO_KAISYA1", STRING_EMPTY);
    // 香料・供花・弔電差出人_会社代表者肩書き - 空白設定
    ht.Add("SASHIDASHI_MOTO_KAISYA2", STRING_EMPTY);
    // 香料・供花・弔電差出人_会社代表者氏名 - 空白設定
    ht.Add("SASHIDASHI_MOTO_KAISYA3", STRING_EMPTY);
    // 出向元会社より香料 - 空白設定
    ht.Add("KOURYOU_MOTO_KAISYA", STRING_EMPTY);
    // 出向元会社より供花の数 - 空白設定
    ht.Add("KUGE_MOTO_KAISYA", STRING_EMPTY);
    // 出向元会社より弔電有無 - 空白設定
    ht.Add("TYOUDEN_MOTO_KAISYA", STRING_EMPTY);
    // 香料・供花・弔電差出人_労働組合名（出向元） - 空白設定
    ht.Add("SASHIDASHI_MOTO_KUMIAI1", STRING_EMPTY);
    // 香料・供花・弔電差出人_労働組合代表者の肩書き - 空白設定
    ht.Add("SASHIDASHI_MOTO_KUMIAI2", STRING_EMPTY);
    // 香料・供花・弔電差出人_労働組合代表者の氏名 - 空白設定
    ht.Add("SASHIDASHI_MOTO_KUMIAI3", STRING_EMPTY);
    // 出向元労働組合より香料 - 空白設定
    ht.Add("KOURYOU_MOTO_KUMIAI", STRING_EMPTY);
    // 出向元労働組合より供花の数 - 空白設定
    ht.Add("KUGE_MOTO_KUMIAI", STRING_EMPTY);
    // 出向元労働組合より弔電有無 - 空白設定
    ht.Add("TYOUDEN_MOTO_KUMIAI", STRING_EMPTY);
    // 香料・供花・弔電差出人_会社名（出向先） - 空白設定
    ht.Add("SASHIDASHI_SAKI_KAISYA1", STRING_EMPTY);
    // 香料・供花・弔電差出人_会社代表者肩書き（出向先） - 空白設定
    ht.Add("SASHIDASHI_SAKI_KAISYA2", STRING_EMPTY);
    // 香料・供花・弔電差出人_会社代表者氏名（出向先） - 空白設定
    ht.Add("SASHIDASHI_SAKI_KAISYA3", STRING_EMPTY);
    // 出向先会社より香料 - 空白設定
    ht.Add("KOURYOU_SAKI_KAISYA", STRING_EMPTY);
    // 出向先会社より供花の数 - 空白設定
    ht.Add("KUGE_SAKI_KAISYA", STRING_EMPTY);
    // 出向先会社より弔電有無 - 空白設定
    ht.Add("TYOUDEN_SAKI_KAISYA", STRING_EMPTY);
    // 香料合計 - 空白設定
    ht.Add("KOURYOU_GOUKEI", STRING_EMPTY);
    // 供花合計 - 空白設定
    ht.Add("KUGE_GOUKEI", STRING_EMPTY);
    // 弔電合計 - 空白設定
    ht.Add("TYOUDEN_GOUKEI", STRING_EMPTY);
    // サマーリー - 空白設定
    ht.Add("SUMMRY", STRING_EMPTY);
    // PDFURL - 空白設定
    ht.Add("PDF_URL", STRING_EMPTY);
}

/**
  * 弔事連絡情報 設定
  * @param {HashTblCommon} ht　バックと連携する対象
  * @param {any} key　実行モード
  */
function setMainTblData(ht, key) {

    if (key === BTN_SUB_SUBMIT || key === BTN_TEMPORARILY_SAVE) { // 提出　または　下書き
        // 伝票番号
        ht.ObjArr["OID"] = GetQueryString("WorkID");
        // 申請者区分
        ht.ObjArr["SHINSEISYAKBN"] = agentMode;
        // 代理申請者社員番号
        ht.ObjArr["DAIRISHINSNEISYA_SHAINBANGO"] = dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"];
        // (スナップショット)代理申請社員名(漢字)
        ht.ObjArr["DAIRISHINSNEISYA_MEI"] = dtCondolence["DAIRISHINSNEISYA_MEI"];
        // (スナップショット)代理申請社員名(フリガナ)
        ht.ObjArr["DAIRISHINSNEISYA_FURIGANAMEI"] = dtCondolence["DAIRISHINSNEISYA_FURIGANAMEI"];
        // (スナップショット)代理申請社員所属コード
        ht.ObjArr["DAIRISHINSNEISYA_SYOZOKUCODE"] = dtCondolence["DAIRISHINSNEISYA_SYOZOKUCODE"];
        // (スナップショット)代理申請社員所属名
        ht.ObjArr["DAIRISHINSNEISYA_SYOZOKU"] = dtCondolence["DAIRISHINSNEISYA_SYOZOKU"];
        // 出向フラグ
        ht.ObjArr["SHUKKOFLG"] = dtCondolence["SHUKKOFLG"];
        // チームコード
        ht.ObjArr["TEAMCODE"] = dtCondolence["TEAMCODE"];
        // チーム名
        ht.ObjArr["TEAMMEISHO"] = dtCondolence["TEAMMEISHO"];
        // 財務部署コード
        ht.ObjArr["ZAIMUBUSHOCODE"] = dtCondolence["ZAIMUBUSHOCODE"];
        // 経費負担会社コード
        ht.ObjArr["KEIHIFUTANKAISHACODE"] = dtCondolence["KEIHIFUTANKAISHACODE"];
        // 経費負担会社名
        ht.ObjArr["KEIHIFUTANKAISHAMEI"] = dtCondolence["KEIHIFUTANKAISHAMEI"];
        // 不幸従業員社員番号
        ht.ObjArr["UNFORTUNATE_SHAINBANGO"] = dtCondolence["UNFORTUNATE_SHAINBANGO"];
        // (スナップショット)不幸従業員の社員名(フリガナ)
        ht.ObjArr["UNFORTUNATE_FURIGANAMEI"] = dtCondolence["UNFORTUNATE_FURIGANAMEI"];
        // (スナップショット)不幸従業員の社員名(漢字)
        ht.ObjArr["UNFORTUNATE_KANJIMEI"] = dtCondolence["UNFORTUNATE_KANJIMEI"];
        // (スナップショット)不幸従業員の出向元会社コード
        ht.ObjArr["UNFORTUNATE_KAISYACODE"] = dtCondolence["UNFORTUNATE_KAISYACODE"];
        // (スナップショット)不幸従業員の出向元会社名
        ht.ObjArr["UNFORTUNATE_KAISYAMEI"] = dtCondolence["UNFORTUNATE_KAISYAMEI"];
        // (スナップショット)不幸従業員の出向先会社コード
        ht.ObjArr["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"];
        // (スナップショット)不幸従業員の出向先会社名
        ht.ObjArr["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"];
        // (スナップショット)不幸者の所属コード
        ht.ObjArr["UNFORTUNATE_SYOZOKUCODE"] = dtCondolence["UNFORTUNATE_SYOZOKUCODE"];
        // (スナップショット)不幸従業員の正式組織・上
        ht.ObjArr["UNFORTUNATE_SEISHIKISOSHIKIUE"] = dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"];
        // (スナップショット)不幸従業員の下
        ht.ObjArr["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"];
        // (スナップショット)不幸従業員の社員区分コード
        ht.ObjArr["UNFORTUNATE_SYAINNKBNCODE"] = dtCondolence["UNFORTUNATE_SYAINNKBNCODE"];
        // (スナップショット)不幸従業員の社員区分名
        ht.ObjArr["UNFORTUNATE_SYAINNKBN"] = dtCondolence["UNFORTUNATE_SYAINNKBN"];
        // (スナップショット)不幸従業員の職位名
        ht.ObjArr["UNFORTUNATE_SYOKUIKBN"] = dtCondolence["UNFORTUNATE_SYOKUIKBN"];
        // (スナップショット)不幸従業員の組合区分コード
        ht.ObjArr["UNFORTUNATE_KUMIAIKBNCODE"] = dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"];
        // (スナップショット)不幸従業員の組合区分名
        ht.ObjArr["UNFORTUNATE_KUMIAIKBN"] = dtCondolence["UNFORTUNATE_KUMIAIKBN"];
        // (スナップショット)不幸従業員のグッドライフ区分コード
        ht.ObjArr["UNFORTUNATE_GLCKBNCODE"] = dtCondolence["UNFORTUNATE_GLCKBNCODE"];
        // (スナップショット)不幸従業員のグッドライフ区分名
        ht.ObjArr["UNFORTUNATE_GLCKBN"] = dtCondolence["UNFORTUNATE_GLCKBN"];
        // 亡くなられた方カナ氏名（姓）
        ht.ObjArr["DEAD_KANASHIMEI_SEI"] = $("#text-died-lastname_kana").val();
        // 亡くなられた方カナ氏名（名）
        ht.ObjArr["DEAD_KANASHIMEI_MEI"] = $("#text-died-firstname_kana").val();
        // 亡くなられた方氏名（姓）
        ht.ObjArr["DEAD_SHIMEI_SEI"] = $("#text-died-lastname").val();
        // 亡くなられた方氏名（名）
        ht.ObjArr["DEAD_SHIMEI_MEI"] = $("#text-died-firstname").val();
        // 亡くなられた方続柄区分 
        ht.ObjArr["DEAD_JUGYOIN_ZOKUGARAKBN"] = $("#select-died-relationship option:selected").val();
        // 亡くなられた方性別
        ht.ObjArr["DEAD_SEIBETSU"] = $("input[name=radio-died-gender]:checked").val();
        // 亡くなられた方同居別居区分
        ht.ObjArr["DEAD_DOKYO_BEKYO"] = $("input[name=radio-died-living]:checked").val();
        // 亡くなられた方年齢
        ht.ObjArr["DEAD_NENREI"] = $("#text-died-age").val();
        // 亡くなられた方逝去日
        ht.ObjArr["DEAD_DATE"] = $("#text-died-death_date").val();
        // 亡くなられた方逝去時
        ht.ObjArr["DEAD_TIME"] = $("#select-died-death_time option:selected").val();
        // 亡くなられた方扶養区分
        ht.ObjArr["DEAD_FUYOUKBN"] = $("input[name=radio-died-dependent]:checked").val();
        // 喪主区分
        ht.ObjArr["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] = $("input[name=radio-mourner]:checked").val();

        if (dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] === ORGANIZER_KBN_HONNIN_IGAI) { // 自分以外
            // 喪主カナ氏名（姓）
            ht.ObjArr["ORGANIZER_KANASHIMEI_SEI"] = $("#text-mourner-lastname_kana").val();
            // 喪主カナ氏名（名）
            ht.ObjArr["ORGANIZER_KANASHIMEI_MEI"] = $("#text-mourner-firstname_kana").val();
            // 喪主氏名（姓）
            ht.ObjArr["ORGANIZER_SHIMEI_SEI"] = $("#text-mourner-lastname").val();
            // 喪主氏名（名）
            ht.ObjArr["ORGANIZER_SHIMEI_MEI"] = $("#text-mourner-firstname").val();
        } else {
            // 氏名
            let kanjimei = dtCondolence["UNFORTUNATE_KANJIMEI"];
            if (kanjimei !== null) {
                kanjimei = kanjimei.split(FULL_SPACE);
                // 喪主氏名（姓）
                ht.ObjArr["ORGANIZER_SHIMEI_SEI"] = kanjimei[0];
                // 喪主氏名（名）
                ht.ObjArr["ORGANIZER_SHIMEI_MEI"] = kanjimei[1];
            }
            // 氏名（カナ）
            let furiganamei = dtCondolence["UNFORTUNATE_FURIGANAMEI"]
            if (furiganamei !== null) {
                furiganamei = furiganamei.split(FULL_SPACE);
                // 喪主カナ氏名（姓）
                ht.ObjArr["ORGANIZER_KANASHIMEI_SEI"] = furiganamei[0];
                // 喪主カナ氏名（名）
                ht.ObjArr["ORGANIZER_KANASHIMEI_MEI"] = furiganamei[1];
            } 
        }

    } 

    // 申請者連絡先電話番号
    ht.ObjArr["RENRAKUSAKITEL"] = $("#text-contact-tel").val();
    // 申請者連絡先メール
    ht.ObjArr["RENRAKUSAKIMAIL"] = $("#text-contact-email").val();
    // 通夜/告別式区分
    let funeralkbn = $("input[name=radio-funeral]:checked").val();
    ht.ObjArr["TSUYA_KOKUBETSUSHIKIKBN"] = funeralkbn;
    // 通夜/告別式　または　通夜
    if (funeralkbn === FUNERAL_KBN_BOTH || funeralkbn === FUNERAL_KBN_TSUYA) {
        // 通夜会場フリガナ
        ht.ObjArr["TSUYA_BASYOUFURIGANA"] = $("#text-wake-placeName_kana").val();
        // 通夜会場名 
        ht.ObjArr["TSUYA_BASYOUMEI"] = $("#text-wake-placeName").val();
        // 通夜郵便番号
        ht.ObjArr["TSUYA_YUBINBANGO"] = $("#text-wake-postcode").val();
        // 通夜都道府県・市郡区
        ht.ObjArr["TSUYA_ADDRESS1"] = $("#text-wake-address1").val();
        // 通夜町村番地
        ht.ObjArr["TSUYA_ADDRESS2"] = $("#text-wake-address2").val();
        // 通夜マンション名
        ht.ObjArr["TSUYA_ADDRESS3"] = $("#text-wake-address3").val();
        // 通夜日付
        ht.ObjArr["TSUYA_DATE"] = $("#text-wake-date").val();
        // 通夜時刻
        ht.ObjArr["TSUYA_TIME"] = $("#select-wake-time option:selected").val();
        // 通夜連絡先電話番号
        ht.ObjArr["TSUYA_RENRAKUSAKITEL"] = $("#text-wake-tel").val();
    }
    // 通夜/告別式　または　告別式
    if (funeralkbn === FUNERAL_KBN_BOTH || funeralkbn === FUNERAL_KBN_KOKUBETSUSHIKI) {
        // 告別式会場フリガナ
        ht.ObjArr["KOKUBETSUSHIKI_BASYOUFURIGANA"] = $("#text-funeral-placeName_kana").val();
        // 告別式会場名
        ht.ObjArr["KOKUBETSUSHIKI_BASYOUMEI"] = $("#text-funeral-placeName").val();
        // 告別式郵便番号
        ht.ObjArr["KOKUBETSUSHIKI_YUBINBANGO"] = $("#text-funeral-postcode").val();
        // 告別式都道府県・市郡区
        ht.ObjArr["KOKUBETSUSHIKI_ADDRESS1"] = $("#text-funeral-address1").val();
        // 告別式町村番地
        ht.ObjArr["KOKUBETSUSHIKI_ADDRESS2"] = $("#text-funeral-address2").val();
        // 告別式マンション名
        ht.ObjArr["KOKUBETSUSHIKI_ADDRESS3"] = $("#text-funeral-address3").val();
        // 告別式日付
        ht.ObjArr["KOKUBETSUSHIKI_DATE"] = $("#text-funeral-date").val();
        // 告別式時刻
        ht.ObjArr["KOKUBETSUSHIKI_TIME"] = $("#select-funeral-time option:selected").val();
        // 告別式連絡先電話番号
        ht.ObjArr["KOKUBETSUSHIKI_RENRAKUSAKITEL"] = $("#text-funeral-tel").val();
    }
    // なし
    if (funeralkbn !== FUNERAL_KBN_NONE) {
        // 一般参列を辞退する区分
        ht.ObjArr["SANRETSU_JITAI_KBN"] = $("#chk-funeral-decline").prop("checked") === true ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED;
    }
    // 通夜/告別式
    if (funeralkbn === FUNERAL_KBN_BOTH) {
        // 通夜と同じ区分
        ht.ObjArr["TSUYA_SAME_KBN"] = $("#chk-funeral-sameplace").prop("checked") === true ? CHECKBOX_CHECKED : CHECKBOX_UNCHECKED;
    }
    // 供花届ける場所区分
    let allnight = $("input[name=radio-allnight]:checked").val();
    ht.ObjArr["TODOKESAKIKBN"] = allnight;
    if (String(allnight) === LOCATION_ATOKA_KBN) {
        // 後飾り名前
        ht.ObjArr["ATOKAZARI_FULLNAME"] = $("#text-rear-name").val();
        // 後飾り郵便番号
        ht.ObjArr["ATOKAZARI_YUBINBANGO"] = $("#text-rear-postcode").val();
        // 後飾り都道府県・市郡区
        ht.ObjArr["ATOKAZARI_ADDRESS1"] = $("#text-rear-address1").val();
        // 後飾り町村番地
        ht.ObjArr["ATOKAZARI_ADDRESS2"] = $("#text-rear-address2").val();
        // 後飾りマンション名
        ht.ObjArr["ATOKAZARI_ADDRESS3"] = $("#text-rear-address3").val();
        // 後飾り電話番号
        ht.ObjArr["ATOKAZARI_RENRAKUSAKITEL"] = $("#text-rear-tel").val();
        // 後飾り日付
        ht.ObjArr["ATOKAZARI_DATE"] = $("#text-rear-date").val();
    }

    var koryokbn = $("input[name=radio-opener-koryo]:checked").val();
    var kugekbn = $("input[name=radio-opener-kuge]:checked").val();
    var tyodenkbn = $("input[name=radio-opener-tyoden]:checked").val();
    // 香料発行区分
    ht.ObjArr["KORYOKBN"] = koryokbn;
    // GLC香料変更区分
    ht.ObjArr["GLCKORYOKBN"] = koryokbn;
    // 供花発行区分
    ht.ObjArr["KYOKAKBN"] = kugekbn;
    // 弔電発行区分
    ht.ObjArr["TYODENKBN"] = tyodenkbn;
    // 香料・供花・弔電差出人_会社名（出向元）
    ht.ObjArr["SASHIDASHI_MOTO_KAISYA1"] = dtCondolence["SASHIDASHI_MOTO_KAISYA1"];
    // 香料・供花・弔電差出人_会社代表者肩書き
    ht.ObjArr["SASHIDASHI_MOTO_KAISYA2"] = dtCondolence["SASHIDASHI_MOTO_KAISYA2"];
    // 香料・供花・弔電差出人_会社代表者氏名
    ht.ObjArr["SASHIDASHI_MOTO_KAISYA3"] = dtCondolence["SASHIDASHI_MOTO_KAISYA3"];
    // 出向元会社より香料
    ht.ObjArr["KOURYOU_MOTO_KAISYA"] = dtCondolence["KOURYOU_MOTO_KAISYA"];
    // 出向元会社より供花の数
    ht.ObjArr["KUGE_MOTO_KAISYA"] = dtCondolence["KUGE_MOTO_KAISYA"];
    // 出向元会社より弔電有無
    ht.ObjArr["TYOUDEN_MOTO_KAISYA"] = dtCondolence["TYOUDEN_MOTO_KAISYA"];
    // 香料・供花・弔電差出人_労働組合名（出向元）
    ht.ObjArr["SASHIDASHI_MOTO_KUMIAI1"] = dtCondolence["SASHIDASHI_MOTO_KUMIAI1"];
    // 香料・供花・弔電差出人_労働組合代表者の肩書き
    ht.ObjArr["SASHIDASHI_MOTO_KUMIAI2"] = dtCondolence["SASHIDASHI_MOTO_KUMIAI2"];
    // 香料・供花・弔電差出人_労働組合代表者の氏名
    ht.ObjArr["SASHIDASHI_MOTO_KUMIAI3"] = dtCondolence["SASHIDASHI_MOTO_KUMIAI3"];
    // 出向元労働組合より香料
    ht.ObjArr["KOURYOU_MOTO_KUMIAI"] = dtCondolence["KOURYOU_MOTO_KUMIAI"];
    // 出向元労働組合より供花の数
    ht.ObjArr["KUGE_MOTO_KUMIAI"] = dtCondolence["KUGE_MOTO_KUMIAI"];
    // 出向元労働組合より弔電有無
    ht.ObjArr["TYOUDEN_MOTO_KUMIAI"] = dtCondolence["TYOUDEN_MOTO_KUMIAI"];
    // 香料・供花・弔電差出人_会社名（出向先）
    ht.ObjArr["SASHIDASHI_SAKI_KAISYA1"] = dtCondolence["SASHIDASHI_SAKI_KAISYA1"];
    // 香料・供花・弔電差出人_会社代表者肩書き（出向先）
    ht.ObjArr["SASHIDASHI_SAKI_KAISYA2"] = dtCondolence["SASHIDASHI_SAKI_KAISYA2"];
    // 香料・供花・弔電差出人_会社代表者氏名（出向先）
    ht.ObjArr["SASHIDASHI_SAKI_KAISYA3"] = dtCondolence["SASHIDASHI_SAKI_KAISYA3"];
    // 出向先会社より香料
    ht.ObjArr["KOURYOU_SAKI_KAISYA"] = dtCondolence["KOURYOU_SAKI_KAISYA"];
    // 出向先会社より供花の数
    ht.ObjArr["KUGE_SAKI_KAISYA"] = dtCondolence["KUGE_SAKI_KAISYA"];
    // 出向先会社より弔電有無
    ht.ObjArr["TYOUDEN_SAKI_KAISYA"] = dtCondolence["TYOUDEN_SAKI_KAISYA"];
    // 香料合計
    ht.ObjArr["KOURYOU_GOUKEI"] = dtCondolence["KOURYOU_GOUKEI"];
    // 供花合計
    ht.ObjArr["KUGE_GOUKEI"] = dtCondolence["KUGE_GOUKEI"];
    // 弔電合計
    ht.ObjArr["TYOUDEN_GOUKEI"] = dtCondolence["TYOUDEN_GOUKEI"];
    // サマーリー
    ht.ObjArr["SUMMRY"] = setSummry();
    // PDFURL
    ht.ObjArr["PDF_URL"] = dtCondolence["PDF_URL"];


    // 手配状態の更新
    if (key === BTN_SUB_SUBMIT || key === BTN_EDIT) {
        // 給付内容が存在しない場合
        if (dtCondolence["KOURYOU_GOUKEI"] <= 0 && dtCondolence["KUGE_GOUKEI"] <= 0 && dtCondolence["TYOUDEN_GOUKEI"] <= 0) {

            if (agentMode === SHINSEISYA_KBN_DAIRI) {
                // 供花届ける場所区分
                ht.ObjArr['TODOKESAKIKBN'] = null;
                // GLC香料変更区分
                ht.ObjArr['GLCKORYOKBN'] = null;
                // 香料発行区分
                ht.ObjArr['KORYOKBN'] = null;
                // 供花発行区分
                ht.ObjArr['KYOKAKBN'] = null;
                // 弔電発行区分
                ht.ObjArr['TYODENKBN'] = null;
                // 香料申請日
                ht.ObjArr['KOURYOU_SHINNSEIBI'] = null;
                // 供花・弔電申請日
                ht.ObjArr['KUGE_TYOUDEN_SHINNSEIBI'] = null;
                // 香料合計
                ht.ObjArr['KOURYOU_GOUKEI'] = null;
                // 供花合計
                ht.ObjArr['KUGE_GOUKEI'] = null;
                // 弔電合計
                ht.ObjArr['TYOUDEN_GOUKEI'] = null;
            }
        }

        // 弔事基準の給付内容あり
        if ($(".arrange-no").is(":hidden")) {
            // 香料は「受け取る」を選択場合
            if (koryokbn === NECESSARY_KBN_HITUYOU) {
                // 香料申請日
                ht.ObjArr["KOURYOU_SHINNSEIBI"] = moment(wfCommon.getServerDatetime()).format(DATETIME_FORMAT_MOMENT_PATTERN_2);
            } else {
                // 香料申請日
                ht.ObjArr['KOURYOU_SHINNSEIBI'] = null;
            }

            // 供花/弔電は「受け取る」を選択場合
            if (kugekbn === NECESSARY_KBN_HITUYOU || tyodenkbn === NECESSARY_KBN_HITUYOU) {
                // 手配状態は「null」/「手配不能」/「キャンセル」の場合
                if (dtCondolence["TEHAIKBN"] === null || dtCondolence["TEHAIKBN"] === parseInt(STATE_TEHAIFUNO) || dtCondolence["TEHAIKBN"] === parseInt(STATE_CANCEL) || dtCondolence["TEHAIKBN"] === parseInt(STATE_CANCEL_PAID)) {
                    if (dtCondolence["KUGE_GOUKEI"] > 0 || dtCondolence["TYOUDEN_GOUKEI"] > 0) { // 供花と弔電は使用可の場合
                        // 供花・弔電手配状態
                        ht.ObjArr["TEHAIKBN"] = STATE_MITEHAI;

                    }
                }
                // 供花・弔電申請日
                ht.ObjArr["KUGE_TYOUDEN_SHINNSEIBI"] = moment(wfCommon.getServerDatetime()).format(DATETIME_FORMAT_MOMENT_PATTERN_2);
            } else {
                // 供花届ける場所区分
                ht.ObjArr['TODOKESAKIKBN'] = null;
                // 供花・弔電申請日
                ht.ObjArr['KUGE_TYOUDEN_SHINNSEIBI'] = null;
            }
        }
    }
}

/**
 * サマーリーを設定
 * 
 * */
function setSummry() {

    let summry = { "AgentMode": agentMode, "AutoApprovalMode": MODE_KBN_YES, "content": [] }
    // 更新日	最終更新日時
    summry["content"].push({ "value": moment(serverDate).format(DATE_FORMAT_MOMENT_PATTERN_1), "name": "更新日" });

    if (dtCondolence["UNFORTUNATE_SHAINBANGO"] === null || dtCondolence["UNFORTUNATE_SHAINBANGO"] === STRING_EMPTY) {
        // 社員番号	不幸従業員社員番号
        summry["content"].push({ "value": FULL_MINUS, "name": "社員番号" });
        // 続柄	亡くなられた方との続柄区分
        summry["content"].push({ "value": FULL_MINUS, "name": "続柄" });
        // 同居 / 別居	亡くなられた方の同居 / 別居区分
        summry["content"].push({ "value": FULL_MINUS, "name": "同居/別居" });
    } else {
        // 社員番号	不幸従業員社員番号
        summry["content"].push({ "value": dtCondolence["UNFORTUNATE_SHAINBANGO"], "name": "社員番号" });
        // 続柄	亡くなられた方との続柄区分
        summry["content"].push({ "value": $("#select-died-relationship").val() === STRING_EMPTY ? FULL_MINUS : $("#select-died-relationship").text(), "name": "続柄" });
        // 同居 / 別居	亡くなられた方の同居 / 別居区分
        summry["content"].push({ "value": $("input[name=radio-died-living]:checked").next().find("div").html(), "name": "同居/別居" });
    }

    // 逝去日	亡くなられた方の逝去日
    summry["content"].push({ "value": $("#text-died-death_date").val() === STRING_EMPTY ? FULL_MINUS : $("#text-died-death_date").val(), "name": "逝去日" });
    // 通夜日or告別式日or後飾り日	通夜日時→告別式日時→後飾り日時
    // 通夜/告別式区分
    let funeralkbn = $("input[name=radio-funeral]:checked").val();
    if (funeralkbn === FUNERAL_KBN_BOTH || funeralkbn === FUNERAL_KBN_TSUYA) {    // 通夜/告別式　または　通夜
        let wakedate = $("#text-wake-date").val();
        if (wakedate !== null || wakedate !== STRING_EMPTY) {
            summry["content"].push({ "value": wakedate, "name": "通夜日" });
        }
     } else if (funeralkbn === FUNERAL_KBN_KOKUBETSUSHIKI) {    // 告別式
        let funeraldate = $("#text-funeral-date").val();
        if (funeraldate !== null || funeraldate !== STRING_EMPTY) {
            summry["content"].push({ "value": funeraldate, "name": "告別式日" });
        }
    } else {    // 以外
        if ($("input[name=radio-allnight]:checked").val() === LOCATION_ATOKA_KBN) {
            let reardate = $("#text-rear-date").val();
            if (reardate !== null || reardate !== STRING_EMPTY) {
                summry["content"].push({ "value": reardate, "name": "後飾り日" });
            }
        }
    }
    return JSON.stringify(summry);
}

/**
 * 入力チェックを設定する
 */
function setInputCheck() {

    $("#form1").validate({
        focusCleanup: true,
        onkeyup: false,
        ignore: STRING_EMPTY,
        groups: {
            contact_tel_mail: "text-contact-tel text-contact-email",
            contact_tel: "text-contact-tel1 text-contact-tel2 text-contact-tel3",
            wake_tel: "text-wake-tel1 text-wake-tel2 text-wake-tel3",
            funeral_tel: "text-funeral-tel1 text-funeral-tel2 text-funeral-tel3",
            rear_tel: "text-rear-tel1 text-rear-tel2 text-rear-tel3",
        },
        rules: {

             // 連絡のメールアドレス
            "text-contact-email": {
                required: function () {
                    return $("#text-contact-tel").val() === STRING_EMPTY;
                },
                email: true
            },
            // 連絡の取れる電話番号
            "text-contact-tel": {
                required: function () {
                    return $("#text-contact-email").val() === STRING_EMPTY;
                }
            },
            // 連絡の取れる電話番号1
            "text-contact-tel1": {
                digits: true
            },
            // 連絡の取れる電話番号2
            "text-contact-tel2": {
                digits: true
            },
            // 連絡の取れる電話番号3
            "text-contact-tel3": {
                digits: true
            },
            // 委任者社員番号
            "text-unhappiness-shainbango": {
                required: function () {
                    return $(".f-agent").is(":visible");
                },
                sameData: function () {
                    return webUser.No;
                },
                halfalphanum: function () {
                    return $(".f-agent").is(":visible");
                },
            },
            // 亡くなられた方情報:姓
            "text-died-lastname": {
                required: true,
                fullchar: true
            },
            // 亡くなられた方情報:名
            "text-died-firstname": {
                required: true,
                fullchar: true
            },
            // 亡くなられた方情報:姓（カナ）
            "text-died-lastname_kana": {
                required: true,
                fullkatakana: true
            },
            // 亡くなられた方情報:名（カナ）
            "text-died-firstname_kana": {
                required: true,
                fullkatakana: true
            },
            // 亡くなられた方情報:あなたから見た続柄
            "select-died-relationship": {
                required: true
            },
            // 亡くなられた方情報:年齢
            "text-died-age": {
                digits: true
            },
            // 亡くなられた方情報:逝去日
            "text-died-death_date": {
                required: true,
                dateJP: true,
            },
            // 亡くなられた方情報:逝去時刻
            "select-died-death_time": {
                required: true
            },
            // 喪主情報:姓
            "text-mourner-lastname": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // 喪主情報:名
            "text-mourner-firstname": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // 喪主情報:姓（カナ）
            "text-mourner-lastname_kana": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // 喪主情報:名（カナ）
            "text-mourner-firstname_kana": {
                required: function () {
                    return $(".area-mourner").is(":visible");
                },
                fullchar: function () {
                    return $(".area-mourner").is(":visible");
                },
            },
            // 通夜について:日付
            "text-wake-date": {
                required: function() {
                    return $(".area-wake").is(":visible");
                },
                dateJP: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 開始時刻
            "select-wake-time": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 会場名
            "text-wake-placeName": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 会場名（カナ）
            "text-wake-placeName_kana": {
                fullkatakana: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 郵便番号
            "text-wake-postcode": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                zipcodeJP: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 都道府県・市郡区
            "text-wake-address1": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 町村・番地
            "text-wake-address2": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 通夜について: 会場電話番号
            "text-wake-tel1": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                digits: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            "text-wake-tel2": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                digits: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            "text-wake-tel3": {
                required: function () {
                    return $(".area-wake").is(":visible");
                },
                digits: function () {
                    return $(".area-wake").is(":visible");
                },
            },
            // 告別式について:日付
            "text-funeral-date": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                dateJP: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 開始時刻
            "select-funeral-time": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 会場名
            "text-funeral-placeName": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 会場名（カナ）
            "text-funeral-placeName_kana": {
                fullkatakana: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 郵便番号
            "text-funeral-postcode": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                zipcodeJP: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 都道府県・市郡区
            "text-funeral-address1": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 町村・番地
            "text-funeral-address2": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 告別式について: 会場電話番号
            "text-funeral-tel1": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                digits: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            "text-funeral-tel2": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                digits: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            "text-funeral-tel3": {
                required: function () {
                    return $(".area-funeral").is(":visible");
                },
                digits: function () {
                    return $(".area-funeral").is(":visible");
                },
            },
            // 後飾り: 名前
            "text-rear-name": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // 後飾り: 郵便番号
            "text-rear-postcode": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                zipcodeJP: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // 後飾り: 都道府県・市郡区
            "text-rear-address1": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // 後飾り: 町村・番地
            "text-rear-address2": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // 後飾り: 電話番号
            "text-rear-tel1": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                digits: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            "text-rear-tel2": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                digits: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            "text-rear-tel3": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                digits: function () {
                    return $(".area-rear").is(":visible");
                },
            },
            // 後飾り:日付
            "text-rear-date": {
                required: function () {
                    return $(".area-rear").is(":visible");
                },
                dateJP: function () {
                    return $(".area-rear").is(":visible");
                },

            },


        },
        messages: {

            "text-contact-email": {
                required: msg["E0004"]
            },
            "text-contact-tel": {
                required: msg["E0004"]
            },
            "text-unhappiness-shainbango": {
                sameData: msg["I0006"]
            },

        }
    });
}

/**
 * 画面項目にデータをセット
 *
 * @param {Array<object>} data サーバから取得のデータ配列
 */
function setDataItems(data) {

    //console.log(data);
    //console.log(webUser);

    // 申請者情報
    dtApplicant = data[0];

    // 新規・下書きの場合
    if (wfstate <= WF_STATE_DRAFT) {

        copyApplicantToCondolence();

        if (agentMode === SHINSEISYA_KBN_DAIRI) {
            setDelegatorInfo();
        }

    }

    // PDFのURL
    setPDFUrl();
    // 申請者・委任者情報
    setApplicantItems();
    // 亡くなられた方
    setDiedItems();
    // 扶養の状況
    setDependentItems();
    // 喪主
    setMournerItems();
    // 通夜/告別式
    setWakeAndFuneralItems();
    // 香料・供花・弔電
    setAllnightItems();
    // 弔事基準
    setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    setCondolenceStandardAreaDisplay();
    // ボタン
    setButtonDisplay();

    // 承認済みの場合
    if (wfstate === WF_STATE_OVER) { 

        let deathday = moment(dtCondolence["DEAD_DATE"], DATE_FORMAT_MOMENT_PATTERN_1).add(1, "years");
        // 一年間超える
        if (deathday.isSameOrBefore(moment(serverDate, DATE_FORMAT_MOMENT_PATTERN_1))) {
            // 確認画面へ遷移する
            $(".o-modal__footer").hide();
            $("#btn-form-back").hide();
            $("#btn-other-back").show();
            $(".state-over").hide();
            gotoConfirmPage();
        } else {
            // 活性・非活性を設定
            changeItemsDisabled();
            changeOpenerItemsDisabled();
            if ($("input[name=radio-opener-koryo]").attr("disabled") && $("input[name=radio-opener-kuge]").attr("disabled") && $("input[name=radio-opener-tyoden]").attr("disabled")
                || dtCondolence["KOURYOU_GOUKEI"] === null && dtCondolence["KUGE_GOUKEI"] === null && dtCondolence["TYOUDEN_GOUKEI"] === null) {
                // 確認画面へ遷移する
                $(".o-modal__footer").hide();
                $("#btn-form-back").hide();
                $("#btn-other-back").show();
                $(".state-over").hide();
                gotoConfirmPage();
            }
        }
    } 

    checkRefer = $("#form1").getElements();
    //console.log(checkRefer);
}

/**
 * PDFのURLを取得
 *
 */
function setPDFUrl(){
    var url = dtCondolence["PDF_URL"];
    if (url === null) {
        setPDFInfo(setPDFInfoItems);
    } else {
        $("#link-page-pdf").show();
        $("#iframe-pdf").attr("src", url);
    }
}

/**
 * 申請者情報を弔事情報にコピー
 *
 */
function copyApplicantToCondolence() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請の場合
        // 社員番号
        dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"] = dtApplicant["SHAINBANGO"];
        // 氏名
        dtCondolence["DAIRISHINSNEISYA_MEI"] = dtApplicant["SEI_KANJI"] + FULL_SPACE + dtApplicant["MEI_KANJI"];
        // 氏名（カナ）
        dtCondolence["DAIRISHINSNEISYA_FURIGANAMEI"] = dtApplicant["SEI_KANA"] + FULL_SPACE + dtApplicant["MEI_KANA"];
        // 所属コード
        dtCondolence["DAIRISHINSNEISYA_SYOZOKUCODE"] = dtApplicant["SHOZOKUCODE"];
        // 所属
        dtCondolence["DAIRISHINSNEISYA_SYOZOKU"] = dtApplicant["SHOZOKUMEISHOJO_KANJI"];
    } else { // 本人申請の場合
        // 社員番号
        dtCondolence["UNFORTUNATE_SHAINBANGO"] = dtApplicant["SHAINBANGO"];
        // 氏名
        dtCondolence["UNFORTUNATE_KANJIMEI"] = dtApplicant["SEI_KANJI"] + FULL_SPACE + dtApplicant["MEI_KANJI"];
        // 氏名（カナ）
        dtCondolence["UNFORTUNATE_FURIGANAMEI"] = dtApplicant["SEI_KANA"] + FULL_SPACE + dtApplicant["MEI_KANA"];
        // 出向元会社コード
        dtCondolence["UNFORTUNATE_KAISYACODE"] = dtApplicant["KAISHACODE"];
        // 出向元会社名称
        dtCondolence["UNFORTUNATE_KAISYAMEI"] = dtApplicant["KAISHANAME"];
        // 出向先会社コード
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = dtApplicant["SHUKKOKAISHACODE"];
        // 出向先会社名称
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = dtApplicant["SHUKKOKAISHANAME"];
        // 所属コード
        dtCondolence["UNFORTUNATE_SYOZOKUCODE"] = dtApplicant["SHOZOKUCODE"];
        // 正式組織名・上
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"] = dtApplicant["SHOZOKUMEISHOJO_KANJI"];
        // 正式組織名・下
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = dtApplicant["SHOZOKUMEISHOGE_KANJI"];
        // 社員区分
        dtCondolence["UNFORTUNATE_SYAINNKBNCODE"] = dtApplicant["JUGYOINKBN"];
        // 社員区分名称
        dtCondolence["UNFORTUNATE_SYAINNKBN"] = dtApplicant["MEISHO_KANJI"];
        // 職位名称
        dtCondolence["UNFORTUNATE_SYOKUIKBN"] = dtApplicant["PLACE_MEISHO_KANJI"];
        // 組合区分
        dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"] = dtApplicant["KUMIAIKBN"];
        // 組合区分名称
        dtCondolence["UNFORTUNATE_KUMIAIKBN"] = dtApplicant["KUMIAINAME"];
        // グッドライフ区分
        dtCondolence["UNFORTUNATE_GLCKBNCODE"] = dtApplicant["GOODLIFEKBN"];
        // グッドライフ区分名称
        dtCondolence["UNFORTUNATE_GLCKBN"] = dtApplicant["GOODLIFENAME"];
        // 出向フラグ
        dtCondolence["SHUKKOFLG"] = dtApplicant["SHUKKOFLG"];
        // チームコード
        dtCondolence["TEAMCODE"] = dtApplicant["TEAMCODE"];
        // チーム名称
        dtCondolence["TEAMMEISHO"] = dtApplicant["TEAMNAME"];
        // 財務部署コード
        dtCondolence["ZAIMUBUSHOCODE"] = dtApplicant["BUSHOCODE"];
        if (dtCondolence["SHUKKOFLG"] === SYUKOU_KBN_YES) { // 出向あり
            // 経費負担会社コード
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtApplicant["SHUKKOKAISHACODE"];
            // 経費負担会社名
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtApplicant["SHUKKOKAISHANAME"];
        } else { // 出向なし
            // 経費負担会社コード
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtApplicant["KAISHACODE"];
            // 経費負担会社名
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtApplicant["KAISHANAME"];
        }

    }
 }

/**
 * 委任者情報を弔事情報にコピー
 *
 */
function copyDelegatorToCondolence() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請の場合
        // 社員番号
        dtCondolence["UNFORTUNATE_SHAINBANGO"] = dtDelegator["SHAINBANGO"];
        // 氏名
        dtCondolence["UNFORTUNATE_KANJIMEI"] = dtDelegator["SEI_KANJI"] + FULL_SPACE + dtDelegator["MEI_KANJI"];
        // 氏名（カナ）
        dtCondolence["UNFORTUNATE_FURIGANAMEI"] = dtDelegator["SEI_KANA"] + FULL_SPACE + dtDelegator["MEI_KANA"];
        // 出向元会社コード
        dtCondolence["UNFORTUNATE_KAISYACODE"] = dtDelegator["KAISHACODE"];
        // 出向元会社名称
        dtCondolence["UNFORTUNATE_KAISYAMEI"] = dtDelegator["KAISHANAME"];
        // 出向先会社コード
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = dtDelegator["SHUKKOKAISHACODE"];
        // 出向先会社名称
        dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = dtDelegator["SHUKKOKAISHANAME"];
        // 所属コード
        dtCondolence["UNFORTUNATE_SYOZOKUCODE"] = dtDelegator["SHOZOKUCODE"];
        // 正式組織名・上
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"] = dtDelegator["SHOZOKUMEISHOJO_KANJI"];
        // 正式組織名・下
        dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = dtDelegator["SHOZOKUMEISHOGE_KANJI"];
        // 社員区分
        dtCondolence["UNFORTUNATE_SYAINNKBNCODE"] = dtDelegator["JUGYOINKBN"];
        // 社員区分名称
        dtCondolence["UNFORTUNATE_SYAINNKBN"] = dtDelegator["MEISHO_KANJI"];
        // 職位名称
        dtCondolence["UNFORTUNATE_SYOKUIKBN"] = dtDelegator["PLACE_MEISHO_KANJI"];
        // 組合区分
        dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"] = dtDelegator["KUMIAIKBN"];
        // 組合区分名称
        dtCondolence["UNFORTUNATE_KUMIAIKBN"] = dtDelegator["KUMIAINAME"];
        // グッドライフ区分
        dtCondolence["UNFORTUNATE_GLCKBNCODE"] = dtDelegator["GOODLIFEKBN"];
        // グッドライフ区分名称
        dtCondolence["UNFORTUNATE_GLCKBN"] = dtDelegator["GOODLIFENAME"];
        // 出向フラグ
        dtCondolence["SHUKKOFLG"] = dtDelegator["SHUKKOFLG"];
        // チームコード
        dtCondolence["TEAMCODE"] = dtDelegator["TEAMCODE"];
        // チーム名称
        dtCondolence["TEAMMEISHO"] = dtDelegator["TEAMNAME"];
        // 財務部署コード
        dtCondolence["ZAIMUBUSHOCODE"] = dtDelegator["BUSHOCODE"];
        if (dtCondolence["SHUKKOFLG"] === SYUKOU_KBN_YES) { // 出向あり
            // 経費負担会社コード
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtDelegator["SHUKKOKAISHACODE"];
            // 経費負担会社名
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtDelegator["SHUKKOKAISHANAME"];
        } else { // 出向なし
            // 経費負担会社コード
            dtCondolence["KEIHIFUTANKAISHACODE"] = dtDelegator["KAISHACODE"];
            // 経費負担会社名
            dtCondolence["KEIHIFUTANKAISHAMEI"] = dtDelegator["KAISHANAME"];
        }
    }
}

/**
 * 委任者情報を弔事情報にクリア
 *
 */
function clearDelegatorToCondolence() {

    // 社員番号
    dtCondolence["UNFORTUNATE_SHAINBANGO"] = STRING_EMPTY;
    // 氏名
    dtCondolence["UNFORTUNATE_KANJIMEI"] = STRING_EMPTY;
    // 氏名（カナ）
    dtCondolence["UNFORTUNATE_FURIGANAMEI"] = STRING_EMPTY;
    // 出向元会社コード
    dtCondolence["UNFORTUNATE_KAISYACODE"] = STRING_EMPTY;
    // 出向元会社名称
    dtCondolence["UNFORTUNATE_KAISYAMEI"] = STRING_EMPTY;
    // 出向先会社コード
    dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"] = STRING_EMPTY;
    // 出向先会社名称
    dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYAMEI"] = STRING_EMPTY;
    // 所属コード
    dtCondolence["UNFORTUNATE_SYOZOKUCODE"] = STRING_EMPTY;
    // 正式組織名・上
    dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"] = STRING_EMPTY;
    // 正式組織名・下
    dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"] = STRING_EMPTY;
    // 社員区分
    dtCondolence["UNFORTUNATE_SYAINNKBNCODE"] = STRING_EMPTY;
    // 社員区分名称
    dtCondolence["UNFORTUNATE_SYAINNKBN"] = STRING_EMPTY;
    // 職位名称
    dtCondolence["UNFORTUNATE_SYOKUIKBN"] = STRING_EMPTY;
    // 組合区分
    dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"] = STRING_EMPTY;
    // 組合区分名称
    dtCondolence["UNFORTUNATE_KUMIAIKBN"] = STRING_EMPTY;
    // グッドライフ区分名称
    dtCondolence["UNFORTUNATE_GLCKBNCODE"] = STRING_EMPTY;
    // グッドライフ区分名称
    dtCondolence["UNFORTUNATE_GLCKBN"] = STRING_EMPTY;
    // 出向フラグ
    dtCondolence["SHUKKOFLG"] = STRING_EMPTY;
    // チームコード
    dtCondolence["TEAMCODE"] = STRING_EMPTY;
    // チーム名称
    dtCondolence["TEAMMEISHO"] = STRING_EMPTY;
    // 財務部署コード
    dtCondolence["ZAIMUBUSHOCODE"] = STRING_EMPTY;
    // 経費負担会社コード
    dtCondolence["KEIHIFUTANKAISHACODE"] = STRING_EMPTY;
    // 経費負担会社名
    dtCondolence["KEIHIFUTANKAISHAMEI"] = STRING_EMPTY;
}

/**
 * 申請者の社員情報項目にデータをセット
 *
 */
function setApplicantItems() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請の場合
        // 社員番号
        $("#label-agent-shainbango").text(dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"]);
        // 氏名
        $("#label-agent-shainshime").text(dtCondolence["DAIRISHINSNEISYA_MEI"]);
        // 氏名（カナ）
        $("#label-agent-shainkana").text(dtCondolence["DAIRISHINSNEISYA_FURIGANAMEI"]);
        // 所属
        $("#label-agent-shozokuname").text(dtCondolence["DAIRISHINSNEISYA_SYOZOKU"]);
        // 委任者社員番号
        $("#text-unhappiness-shainbango").val(dtCondolence["UNFORTUNATE_SHAINBANGO"]);
        // 委任者氏名
        $("#text-unhappiness-shainshime").val(dtCondolence["UNFORTUNATE_KANJIMEI"]);
        // 委任者氏名（カナ）
        $("#text-unhappiness-shainkana").val(dtCondolence["UNFORTUNATE_FURIGANAMEI"]);
        // 委任者会社名称
        $("#text-unhappiness-kaishaname").val(dtCondolence["UNFORTUNATE_KAISYAMEI"]);
        // 委任者正式組織名・上
        $("#text-unhappiness-shozokuname").val(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"]);
        // 委任者正式組織名・下
        $("#text-unhappiness-bushoname").val(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"]);
        // 委任者社員区分
        $("#text-unhappiness-kbnname").val(dtCondolence["UNFORTUNATE_SYAINNKBN"]);
        // 委任者職位
        $("#text-unhappiness-shokuiname").val(dtCondolence["UNFORTUNATE_SYOKUIKBN"]);
        // 委任者組合区分
        $("#text-unhappiness-kumiainame").val(dtCondolence["UNFORTUNATE_KUMIAIKBN"]);
        // 委任者グッドライフ区分
        $("#text-unhappiness-goodlifename").val(dtCondolence["UNFORTUNATE_GLCKBN"]);
    } else { // 本人申請の場合
        // 社員番号
        $("#label-emp-shainbango").text(dtCondolence["UNFORTUNATE_SHAINBANGO"]);
        // 氏名
        $("#label-emp-shainshime").text(dtCondolence["UNFORTUNATE_KANJIMEI"]);
        // 氏名（カナ）
        $("#label-emp-shainkana").text(dtCondolence["UNFORTUNATE_FURIGANAMEI"]);
        // 会社名称
        $("#label-emp-kaishaname").text(dtCondolence["UNFORTUNATE_KAISYAMEI"]);
        // 正式組織名・上
        $("#label-emp-shozokuname").text(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"]);
        // 正式組織名・下
        $("#label-emp-bushoname").text(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"]);
        // 社員区分
        $("#label-emp-kbnname").text(dtCondolence["UNFORTUNATE_SYAINNKBN"]);
        // 職位
        $("#label-emp-shokuiname").text(dtCondolence["UNFORTUNATE_SYOKUIKBN"]);
        // 組合区分
        $("#label-emp-kumiainame").text(dtCondolence["UNFORTUNATE_KUMIAIKBN"]);
        // グッドライフ区分
        $("#label-emp-goodlifename").text(dtCondolence["UNFORTUNATE_GLCKBN"]);
    }

    // 連絡の取れる電話番号
    let contactTel = dtCondolence["RENRAKUSAKITEL"];
    if (contactTel !== null) {
        let contactTels = contactTel.split("-");
        $("#text-contact-tel1").val(contactTels[0]);
        $("#text-contact-tel2").val(contactTels[1]);
        $("#text-contact-tel3").val(contactTels[2]);
        $("#text-contact-tel").val(contactTel);
    }
    // メールアドレス
    $("#text-contact-email").val(dtCondolence["RENRAKUSAKIMAIL"]);

}

/**
 * 亡くなられた方について
 * 
 */
function setDiedItems() {
    // 氏名（姓）
    $("#text-died-lastname").val(dtCondolence["DEAD_SHIMEI_SEI"]);
    // 氏名（名）
    $("#text-died-firstname").val(dtCondolence["DEAD_SHIMEI_MEI"]);
    // 氏名カナ（姓）
    $("#text-died-lastname_kana").val(dtCondolence["DEAD_KANASHIMEI_SEI"]);
    // 氏名カナ（名）
    $("#text-died-firstname_kana").val(dtCondolence["DEAD_KANASHIMEI_MEI"]);
    // 続柄区分
    if (agentMode !== SHINSEISYA_KBN_DAIRI) { // 本人申請
        dtKbn["DEAD_KBN"].splice(6, 1); // 従業員本人選択肢を削除
    }
    wfCommon.initDropdown(true, dtKbn["DEAD_KBN"], dtCondolence["DEAD_JUGYOIN_ZOKUGARAKBN"], MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "select-died-relationship", "pulldown-died-relationship");
    // あなたから見た続柄プルダウン
    $("#select-died-relationship").on("change", function () {
        // 再検証
        $("#form1").validate().element($("#select-died-relationship"));
        // あなたから見た続柄をセット 
        dtCondolence["DEAD_JUGYOIN_ZOKUGARAKBN"] = this.value;
        // 扶養の状況をセット
        setDependentItems();
        // 弔事基準情報をセット
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });
    // 性別
    wfCommon.radiosSetVal("radio-died-gender", dtCondolence["DEAD_SEIBETSU"], GENDER_MALE);
    // 同居/別居区分
    wfCommon.radiosSetVal("radio-died-living", dtCondolence["DEAD_DOKYO_BEKYO"], DOKYO_BEKYO_KBN_YES);
    $("input[name=radio-died-living]").on("change", function () {
        // 弔事基準情報をセット
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });
    // 年齢
    $("#text-died-age").val(dtCondolence["DEAD_NENREI"]);
    // 逝去日付
    let deathdate = dtCondolence["DEAD_DATE"];
    if (deathdate !== null) {
        $("#text-died-death_date").val(moment(deathdate).format(DATE_FORMAT_MOMENT_PATTERN_1));
    }
    // 逝去時刻
    wfCommon.initDropdown(true, deathTimeList["content"], dtCondolence["DEAD_TIME"], "value", "name", "select-died-death_time", "pulldown-died-death_time");
    // 逝去時刻プルダウン
    $("#select-died-death_time").on("change", function () {
        // 再検証
        $("#form1").validate().element($("#select-died-death_time"));
    });
    // 扶養区分
    wfCommon.radiosSetVal("radio-died-dependent", dtCondolence["DEAD_FUYOUKBN"], FUYOUKBN_BEKYO_KBN_YES);
    $("input[name=radio-died-dependent]").on("change", function () {
        // 弔事基準情報をセット
        setCondolenceStandardInfo(setCondolenceStandardInfoItems);
    });
}

/**
 * 委任者の社員情報項目にデータをセット
 *
 */
function setDelegatorItems(data) {

    if (data.length === 1) {
        // 委任者情報
        dtDelegator = data[0];
        // 氏名
        $("#text-unhappiness-shainshime").val(dtDelegator["SEI_KANJI"] + FULL_SPACE + dtDelegator["MEI_KANJI"]);
        // 氏名（カナ）
        $("#text-unhappiness-shainkana").val(dtDelegator["SEI_KANA"] + FULL_SPACE + dtDelegator["MEI_KANA"]);
        // 会社名称
        $("#text-unhappiness-kaishaname").val(dtDelegator["KAISHANAME"]);
        // 正式組織名・上
        $("#text-unhappiness-shozokuname").val(dtDelegator["SHOZOKUMEISHOJO_KANJI"]);
        // 正式組織名・下
        $("#text-unhappiness-bushoname").val(dtDelegator["SHOZOKUMEISHOGE_KANJI"]);
        // 社員区分
        $("#text-unhappiness-kbnname").val(dtDelegator["MEISHO_KANJI"]);
        // 職位
        $("#text-unhappiness-shokuiname").val(dtDelegator["PLACE_MEISHO_KANJI"]);
        // 組合区分
        $("#text-unhappiness-kumiainame").val(dtDelegator["KUMIAINAME"]);
        // グッドライフ区分
        $("#text-unhappiness-goodlifename").val(dtDelegator["GOODLIFENAME"]);
        // コピー
        copyDelegatorToCondolence();
    } else {
        // 氏名
        $("#text-unhappiness-shainshime").val(STRING_EMPTY);
        // 氏名（カナ）
        $("#text-unhappiness-shainkana").val(STRING_EMPTY);
        // 会社名称
        $("#text-unhappiness-kaishaname").val(STRING_EMPTY);
        // 正式組織名・上
        $("#text-unhappiness-shozokuname").val(STRING_EMPTY);
        // 正式組織名・下
        $("#text-unhappiness-bushoname").val(STRING_EMPTY);
        // 社員区分
        $("#text-unhappiness-kbnname").val(STRING_EMPTY);
        // 職位
        $("#text-unhappiness-shokuiname").val(STRING_EMPTY);
        // 組合区分
        $("#text-unhappiness-kumiainame").val(STRING_EMPTY);
        // グッドライフ区分
        $("#text-unhappiness-goodlifename").val(STRING_EMPTY);
        // クリア
        clearDelegatorToCondolence();

        wfCommon.ShowDialog(DIALOG_ALERT, STRING_EMPTY, msg["I0011"]);
    }

}

/**
 * 扶養の状況 設定
 *
 */
function setDependentItems() {

    let glc_kbn = dtCondolence["UNFORTUNATE_GLCKBNCODE"];
    let relationship_kbn = null;
    let jugyoin_kbn = null;

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請の場合
        glc_kbn = wfCommon.FindKbnCode($("#text-unhappiness-goodlifename").val(), "GLC_KBN");
        jugyoin_kbn = wfCommon.FindKbnCode($("#text-unhappiness-kbnname").val(), "JYUUGYOUINN_KBN");

    } else { // 本人申請の場合
        glc_kbn = wfCommon.FindKbnCode($("#label-emp-goodlifename").text(), "GLC_KBN");
        jugyoin_kbn = wfCommon.FindKbnCode($("#label-emp-kbnname").text(), "JYUUGYOUINN_KBN");
    }

    relationship_kbn = $("#select-died-relationship option:selected").val();

    // GLC区分「非加入員」また「非加入員*」　且つ　亡くなられた 「子」を選択　且つ　従業員区分「コミュニティ社員（時間給）」
    if ((glc_kbn === "2" || glc_kbn === "E") && relationship_kbn === "3" && jugyoin_kbn === "T") {
        $(".area-died-dependent").show();
    } else {
        $(".area-died-dependent").hide();
        // 扶養の状況：扶養親族に戻す
        wfCommon.radiosSetVal("radio-died-dependent", FUYOUKBN_BEKYO_KBN_YES, FUYOUKBN_BEKYO_KBN_YES);
    }
}

/**
 * 確認画面にデータをセット
 * 
 * */
function setDataToConfirmPage() {

    $("#applicantInfoArea").empty();
    // 申請者情報エリア
    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請
        $("#applicantAgentInfo").clone().appendTo("#applicantInfoArea");
    } else {
        $("#applicantInfo").clone().appendTo("#applicantInfoArea");
    }

    // 連絡の取れる電話番号／メールアドレス
    $("#p-contact-tel").html($("#text-contact-tel").val());
    $("#p-contact-email").html($("#text-contact-email").val());

    // ご不幸にあわれた方の社員情報
    $("#p-delegator-shainbango").html($("#text-unhappiness-shainbango").val());
    $("#p-delegator-shainshime").html($("#text-unhappiness-shainshime").val());
    $("#p-delegator-shainkana").html($("#text-unhappiness-shainkana").val());
    $("#p-delegator-kaishaname").html($("#text-unhappiness-kaishaname").val());
    $("#p-delegator-shozokuname").html($("#text-unhappiness-shozokuname").val());
    $("#p-delegator-bushoname").html($("#text-unhappiness-bushoname").val());
    $("#p-delegator-kbnname").html($("#text-unhappiness-kbnname").val());
    $("#p-delegator-shokuiname").html($("#text-unhappiness-shokuiname").val());
    $("#p-delegator-kumiainame").html($("#text-unhappiness-kumiainame").val());
    $("#p-delegator-goodlifename").html($("#text-unhappiness-goodlifename").val());

    // 亡くなられた方について
    $("#p-died-fullname").html($("#text-died-lastname").val() + FULL_SPACE + $("#text-died-firstname").val());
    $("#p-died-fullname_kana").html($("#text-died-lastname_kana").val() + FULL_SPACE + $("#text-died-firstname_kana").val());
    $("#p-died-relationship").html($("#select-died-relationship").html());
    $("#p-died-gender").html($("input[name=radio-died-gender]:checked").next().find("div").html());
    $("#p-died-living").html($("input[name=radio-died-living]:checked").next().find("div").html());
    if ($("#text-died-age").val() !== STRING_EMPTY) {
        $("#p-died-age").html($("#text-died-age").val());
    }
    $("#p-died-death_date").html($("#text-died-death_date").val());
    $("#p-died-death_time").html($("#select-died-death_time").html());
    $("#p-died-dependent").html($("input[name=radio-died-dependent]:checked").next().find("div").html());

    // 葬儀について
    $("#p-mourner").html($("input[name=radio-mourner]:checked").next().find("div").html());
    $("#p-mourner-fullname").html($("#text-mourner-lastname").val() + FULL_SPACE + $("#text-mourner-firstname").val());
    $("#p-mourner-fullname_kana").html($("#text-mourner-lastname_kana").val() + FULL_SPACE + $("#text-mourner-firstname_kana").val());
    if ($("#chk-funeral-decline").prop("checked")) {
        $("#p-funeral").html($("input[name=radio-funeral]:checked").next().find("div").html() + "／" + $("#chk-funeral-decline").next().find("div").html());
    } else {
        $("#p-funeral").html($("input[name=radio-funeral]:checked").next().find("div").html());
    }

    // 通夜について
    $("#p-wake-date").html($("#text-wake-date").val());
    $("#p-wake-time").html($("#select-wake-time").html());
    $("#p-wake-placeName").html($("#text-wake-placeName").val());
    if ($("#text-wake-placeName_kana").val() !== STRING_EMPTY) {
        $("#p-wake-placeName_kana").html($("#text-wake-placeName_kana").val());
    }
    $("#p-wake-postcode").html($("#text-wake-postcode").val());
    $("#p-wake-address1").html($("#text-wake-address1").val());
    $("#p-wake-address2").html($("#text-wake-address2").val());
    if ($("#text-wake-address3").val() !== STRING_EMPTY) {
        $("#p-wake-address3").html($("#text-wake-address3").val());
    }
    $("#p-wake-tel").html($("#text-wake-tel").val());

    // 告別式について
    $("#p-funeral-date").html($("#text-funeral-date").val());
    $("#p-funeral-time").html($("#select-funeral-time").html());
    $("#p-funeral-placeName").html($("#text-funeral-placeName").val());
    if ($("#text-funeral-placeName_kana").val() !== STRING_EMPTY) {
        $("#p-funeral-placeName_kana").html($("#text-funeral-placeName_kana").val());
    }
    $("#p-funeral-postcode").html($("#text-funeral-postcode").val());
    $("#p-funeral-address1").html($("#text-funeral-address1").val());
    $("#p-funeral-address2").html($("#text-funeral-address2").val());
    if ($("#text-funeral-address3").val() !== STRING_EMPTY) {
        $("#p-funeral-address3").html($("#text-funeral-address3").val());
    }
    $("#p-funeral-tel").html($("#text-funeral-tel").val());

    // 香料・供花・弔電について
    if (dtCondolence["KOURYOU_GOUKEI"] > 0) {
        $("#p-opener-koryo").html($("input[name=radio-opener-koryo]:checked").next().find("div").html());
    } else {
        if (dtCondolence["KORYOKBN"] === Number(NECESSARY_KBN_HITUYOU)) {
            $("#p-opener-koryo").html(STANDARD_ITEMS.NOT_APPLICABLE);
        } else {
            $("#p-opener-koryo").html($("input[name=radio-opener-koryo]:checked").next().find("div").html());
        }

    }

    if (dtCondolence["KUGE_GOUKEI"] > 0) {
        $("#p-opener-kuge").html($("input[name=radio-opener-kuge]:checked").next().find("div").html());
    } else {

        if (dtCondolence["KYOKAKBN"] === Number(NECESSARY_KBN_HITUYOU)) {
            $("#p-opener-kuge").html(STANDARD_ITEMS.NOT_APPLICABLE);
        } else {
            $("#p-opener-kuge").html($("input[name=radio-opener-kuge]:checked").next().find("div").html());
        }
       
    }

    if (dtCondolence["TYOUDEN_GOUKEI"] > 0) {
        $("#p-opener-tyoden").html($("input[name=radio-opener-tyoden]:checked").next().find("div").html());
    } else {

        if (dtCondolence["TYODENKBN"] === Number(NECESSARY_KBN_HITUYOU)) {
            $("#p-opener-tyoden").html(STANDARD_ITEMS.NOT_APPLICABLE);
        } else {
            $("#p-opener-tyoden").html($("input[name=radio-opener-tyoden]:checked").next().find("div").html());
        }
    }

    // 通夜/告別式の有無について
    $("#p-allnight").html($("input[name=radio-allnight]:checked").next().find("div").html())

    // 後飾りのお届け先情報
    $("#p-rear-name").html($("#text-rear-name").val());
    $("#p-rear-postcode").html($("#text-rear-postcode").val());
    $("#p-rear-address1").html($("#text-rear-address1").val());
    $("#p-rear-address2").html($("#text-rear-address2").val());
    if ($("#text-rear-address3").val() !== STRING_EMPTY) {
        $("#p-rear-address3").html($("#text-rear-address3").val());
    }
    $("#p-rear-tel").html($("#text-rear-tel").val());
    $("#p-rear-date").html($("#text-rear-date").val());

}

/**
  * 弔事基準情報 表示・非表示
  */
function setCondolenceStandardAreaDisplay() {

    if (agentMode === SHINSEISYA_KBN_DAIRI) { // 代理申請
        $("#area-standard").show();
    } else { // 本人申請
        if (wfstate === WF_STATE_OVER) {  // 承認済
            $("#area-standard").show();
        } else {
            $("#area-standard").hide();
        }
    }
}

/**
  * 弔事基準情報をセット
  * @param {string} callback CALLBACK関数
  */
function setCondolenceStandardInfo(callback) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
    handler.AddUrlData();

    // 出向元会社コード
    handler.AddPara("kaishacode", dtCondolence["UNFORTUNATE_KAISYACODE"]);
    // 出向先会社コード
    handler.AddPara("shukokaishacode", dtCondolence["UNFORTUNATE_SYUKOSAKIKAISYACODE"]);
    // 社員番号
    handler.AddPara("shainbango", dtCondolence["UNFORTUNATE_SHAINBANGO"]);
    // 組合区分
    handler.AddPara("kumiai_kbn", dtCondolence["UNFORTUNATE_KUMIAIKBNCODE"]);
    // グッドライフ区分
    handler.AddPara("glc_kbn", dtCondolence["UNFORTUNATE_GLCKBNCODE"]);
    // 社員区分
    handler.AddPara("jyuugyouinn_kbn", dtCondolence["UNFORTUNATE_SYAINNKBNCODE"]);
    // 出向区分
    handler.AddPara("syukou_kbn", dtCondolence["SHUKKOFLG"]);

    //亡くなられた方の続柄
    handler.AddPara("dead_kbn", $("#select-died-relationship option:selected").val());
    //喪主区分
    handler.AddPara("organizer_kbn", $("input[name=radio-mourner]:checked").val());
    //同居区分
    handler.AddPara("dokyo_bekyo_kbn", $("input[name=radio-died-living]:checked").val());
    //税扶養区分
    //if ($(".area-died-dependent").is(":visible") === true) {
    handler.AddPara("fuyou_kbn", $("input[name=radio-died-dependent]:checked").val());
    //}

    handler.DoMethodSetString("Get_Condolence_Standard_Info", function (data) {
        //例外処理
        if (data.indexOf("err@") === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON対象に転換
        var ret = JSON.parse(data);
        callback(ret);
    });
}

/**
 * 弔事基準項目にデータをセット
 *
 * @param {Array<object>} data サーバから取得のデータ配列
 */
function setCondolenceStandardInfoItems(data) {
    //console.log(data);
    let infoArray = data["Get_Condolence_Standard_Info"];

    for (let i = 0; i < infoArray.length; i++) {
        let htc = new HashTblCommon();
        // 会社コード
        htc.Add("KAISHACODE", infoArray[i]["KAISHACODE"]);
        // 名義区分
        htc.Add("CHOJIMEIGIKBN", infoArray[i]["CHOJIMEIGIKBN"]);
        let meigiArray = wfCommon.getApiInfoAjax(GET_CONDOLENCE_MEIGi_INFO_APINAME, htc);

        for (let j = 0; j < meigiArray.length; j++) {

            infoArray[i]["KAISHAMEI"] = meigiArray[j]["KAISHAMEI"];
            infoArray[i]["YAKUSHOKU"] = meigiArray[j]["YAKUSHOKU"];
            infoArray[i]["SHIMEI"] = meigiArray[j]["SHIMEI"];

        }
    }

    setCondolenceStandardArea(infoArray);
}


/**
 * 弔事基準エリアにデータをセット
 *
 * @param {Array<object>} infoArray 弔事基準配列
 */
function setCondolenceStandardArea(infoArray) {

    copyStandardToCondolence(infoArray);

    if (agentMode === SHINSEISYA_KBN_DAIRI) {

        // 弔事基準データが0件の場合
        if (dtCondolence["KOURYOU_GOUKEI"] === 0 && dtCondolence["KUGE_GOUKEI"] === 0 && dtCondolence["TYOUDEN_GOUKEI"] === 0) {
            $(".arrange-ok").hide();
            $(".arrange-no").show();
        } else {
            $(".arrange-ok").show();
            $(".arrange-no").hide();

            setCondolenceStandardItems();
        }
        // 香料合計が０：支給基準外
        if (dtCondolence["KOURYOU_GOUKEI"] === 0) {
            wfCommon.radiosSetVal("radio-opener-koryo", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
            $("input[name=radio-opener-koryo]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
        } else {

            // 香料を受取った場合
            if (dtCondolence["KORYOKBN"] === parseInt(NECESSARY_KBN_HITUYOU)) {
                $("input[name=radio-opener-koryo]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
            } else {
                wfCommon.radiosSetVal("radio-opener-koryo", dtCondolence["KORYOKBN"] === parseInt(NECESSARY_KBN_JITAI) ? NECESSARY_KBN_JITAI : NECESSARY_KBN_HITUYOU, NECESSARY_KBN_JITAI);
                $("input[name=radio-opener-koryo]").attr("disabled", false).parent().addClass("a-radio--grey").removeClass("a-radio--disabled");
            }
        }
        // 供花合計０：支給基準外
        if (dtCondolence["KUGE_GOUKEI"] === 0) {
            wfCommon.radiosSetVal("radio-opener-kuge", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
            $("input[name=radio-opener-kuge]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
        } else {
            wfCommon.radiosSetVal("radio-opener-kuge", dtCondolence["KYOKAKBN"] === parseInt(NECESSARY_KBN_JITAI) ? NECESSARY_KBN_JITAI : NECESSARY_KBN_HITUYOU, NECESSARY_KBN_JITAI);
            $("input[name=radio-opener-kuge]").attr("disabled", false).parent().addClass("a-radio--grey").removeClass("a-radio--disabled");
        }
        // 弔電合計０：支給基準外
        if (dtCondolence["TYOUDEN_GOUKEI"] === 0) {
            wfCommon.radiosSetVal("radio-opener-tyoden", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
            $("input[name=radio-opener-tyoden]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
        } else {
            wfCommon.radiosSetVal("radio-opener-tyoden", dtCondolence["TYODENKBN"] === parseInt(NECESSARY_KBN_JITAI) ? NECESSARY_KBN_JITAI : NECESSARY_KBN_HITUYOU, NECESSARY_KBN_JITAI);
            $("input[name=radio-opener-tyoden]").attr("disabled", false).parent().addClass("a-radio--grey").removeClass("a-radio--disabled");
        }


    } else {
        setCondolenceStandardItems();
    }

    // 差出人情報
    $("#p-standard-table").empty();
    $("#label-standard-table").clone().appendTo("#p-standard-table");
    $("#p-standard-table").find("#label-standard-info0").attr("id", "label-clone-standard-info0");
    $("#p-standard-table").find("#label-standard-info1").attr("id", "label-clone-standard-info1");
    $("#p-standard-table").find("#label-standard-info2").attr("id", "label-clone-standard-info2");
    $("#p-standard-table").find("#label-standard-info3").attr("id", "label-clone-standard-info3");
}

/**
 * 弔事基準データを設定
 * 
 * */
function setCondolenceStandardItems() {
    if (dtCondolence["SASHIDASHI_MOTO_KAISYA1"] === null || dtCondolence["SASHIDASHI_MOTO_KAISYA1"] === STRING_EMPTY) {
        $("#label-standard-info0").parent().hide();
    } else {
        $("#label-standard-info0").parent().show();
        $("#label-standard-kaishaname0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA1"]);
        $("#label-standard-yakushoku0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA2"]);
        $("#label-standard-shimei0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA3"]);
        $("#label-standard-koryonum0").text(editKoryoByCondolenceStandard(dtCondolence["KOURYOU_MOTO_KAISYA"]));
        $("#label-standard-kyokanum0").text(editKyokaByCondolenceStandard(dtCondolence["KUGE_MOTO_KAISYA"]));
        $("#label-standard-tyodennum0").text(editTyodenByCondolenceStandard(dtCondolence["TYOUDEN_MOTO_KAISYA"]));
    }

    if (dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] === null || dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] === STRING_EMPTY) {
        $("#label-standard-info1").parent().hide();
    } else {
        $("#label-standard-info1").parent().show();
        $("#label-standard-kaishaname1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI1"]);
        $("#label-standard-yakushoku1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI2"]);
        $("#label-standard-shimei1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI3"]);
        $("#label-standard-koryonum1").text(editKoryoByCondolenceStandard(dtCondolence["KOURYOU_MOTO_KUMIAI"]));
        $("#label-standard-kyokanum1").text(editKyokaByCondolenceStandard(dtCondolence["KUGE_MOTO_KUMIAI"]));
        $("#label-standard-tyodennum1").text(editTyodenByCondolenceStandard(dtCondolence["TYOUDEN_MOTO_KUMIAI"]));
    }

    if (dtCondolence["SASHIDASHI_SAKI_KAISYA1"] === null || dtCondolence["SASHIDASHI_SAKI_KAISYA1"] === STRING_EMPTY) {
        $("#label-standard-info2").parent().hide();
    } else {
        $("#label-standard-info2").parent().show();
        $("#label-standard-kaishaname2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA1"]);
        $("#label-standard-yakushoku2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA2"]);
        $("#label-standard-shimei2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA3"]);
        $("#label-standard-koryonum2").text(editKoryoByCondolenceStandard(dtCondolence["KOURYOU_SAKI_KAISYA"]));
        $("#label-standard-kyokanum2").text(editKyokaByCondolenceStandard(dtCondolence["KUGE_SAKI_KAISYA"]));
        $("#label-standard-tyodennum2").text(editTyodenByCondolenceStandard(dtCondolence["TYOUDEN_SAKI_KAISYA"]));
    }

    if (dtCondolence["KOURYOU_GOUKEI"] === 0 && dtCondolence["KUGE_GOUKEI"] === 0 && dtCondolence["TYOUDEN_GOUKEI"] === 0) {
        $("#label-standard-info3").parent().show();
    } else {
        $("#label-standard-info3").parent().hide();
    }

}

/**
 * 弔事基準項目:香料を編集する
 *
 * @param {number} koryo 香料
 */
function editKoryoByCondolenceStandard(koryo) {
    let display;
    if (agentMode === SHINSEISYA_KBN_DAIRI) {
        display = wfCommon.comma(koryo) + STANDARD_ITEMS.KORYO.UNIT;
    } else {
        display = String(koryo) === "0" ? STANDARD_ITEMS.KORYO.NONE : STANDARD_ITEMS.KORYO.YES;
    }

    return display;
}

/**
 * 弔事基準項目:供花を編集する
 *
 * @param {Number} kyoka 供花
 */
function editKyokaByCondolenceStandard(kyoka) {
    let display = String(kyoka) + STANDARD_ITEMS.KYOKA.UNIT;

    return display;
}

/**
 * 弔事基準項目:弔電を編集する
 *
 * @param {Number} tyoden 弔電
 */
function editTyodenByCondolenceStandard(tyoden) {
    let display = String(tyoden) + STANDARD_ITEMS.TYODEN.UNIT;

    return display;
}

/**
 * 弔事基準を弔事情報にコピー
 *
 */
function copyStandardToCondolence(infoArray) {

    let koryo_sum = 0;
    let kyoka_sum = 0;
    let tyoden_sum = 0;

    let info0 = infoArray[0];

    if (info0 !== undefined) {
        // 香料・供花・弔電差出人_会社名（出向元）
        dtCondolence["SASHIDASHI_MOTO_KAISYA1"] = info0["KAISHAMEI"];
        // 香料・供花・弔電差出人_会社代表者肩書き（出向元）
        dtCondolence["SASHIDASHI_MOTO_KAISYA2"] = info0["YAKUSHOKU"];
        // 香料・供花・弔電差出人_会社代表者氏名（出向元）
        dtCondolence["SASHIDASHI_MOTO_KAISYA3"] = info0["SHIMEI"];
        // 出向元会社より香料
        dtCondolence["KOURYOU_MOTO_KAISYA"] = info0["KORYONUM"];
        // 出向元会社より供花の数
        dtCondolence["KUGE_MOTO_KAISYA"] = info0["KYOKANUM"];
        // 出向元会社より弔電有無
        dtCondolence["TYOUDEN_MOTO_KAISYA"] = info0["TYODENNUM"];

        koryo_sum += Number(info0["KORYONUM"]);
        kyoka_sum += Number(info0["KYOKANUM"]);
        tyoden_sum += Number(info0["TYODENNUM"]);
    } else {
        // 香料・供花・弔電差出人_会社名（出向元）
        dtCondolence["SASHIDASHI_MOTO_KAISYA1"] = null;
        // 香料・供花・弔電差出人_会社代表者肩書き（出向元）
        dtCondolence["SASHIDASHI_MOTO_KAISYA2"] = null;
        // 香料・供花・弔電差出人_会社代表者氏名（出向元）
        dtCondolence["SASHIDASHI_MOTO_KAISYA3"] = null;
        // 出向元会社より香料
        dtCondolence["KOURYOU_MOTO_KAISYA"] = null;
        // 出向元会社より供花の数
        dtCondolence["KUGE_MOTO_KAISYA"] = null;
        // 出向元会社より弔電有無
        dtCondolence["TYOUDEN_MOTO_KAISYA"] = null;
    }

    let info1 = infoArray[1];

    if (info1 !== undefined) {
        // 香料・供花・弔電差出人_労働組合名（出向元）
        dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] = info1["KAISHAMEI"];
        // 香料・供花・弔電差出人_労働組合代表者肩書き
        dtCondolence["SASHIDASHI_MOTO_KUMIAI2"] = info1["YAKUSHOKU"];
        // 香料・供花・弔電差出人_労働組合代表者氏名
        dtCondolence["SASHIDASHI_MOTO_KUMIAI3"] = info1["SHIMEI"];
        // 出向元労働組合より香料
        dtCondolence["KOURYOU_MOTO_KUMIAI"] = info1["KORYONUM"];
        // 出向元労働組合より供花の数
        dtCondolence["KUGE_MOTO_KUMIAI"] = info1["KYOKANUM"];
        // 出向元労働組合より弔電有無
        dtCondolence["TYOUDEN_MOTO_KUMIAI"] = info1["TYODENNUM"];

        koryo_sum += Number(info1["KORYONUM"]);
        kyoka_sum += Number(info1["KYOKANUM"]);
        tyoden_sum += Number(info1["TYODENNUM"]);
    } else {
        // 香料・供花・弔電差出人_労働組合名（出向元）
        dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] = null;
        // 香料・供花・弔電差出人_労働組合代表者肩書き
        dtCondolence["SASHIDASHI_MOTO_KUMIAI2"] = null;
        // 香料・供花・弔電差出人_労働組合代表者氏名
        dtCondolence["SASHIDASHI_MOTO_KUMIAI3"] = null;
        // 出向元労働組合より香料
        dtCondolence["KOURYOU_MOTO_KUMIAI"] = null;
        // 出向元労働組合より供花の数
        dtCondolence["KUGE_MOTO_KUMIAI"] = null;
        // 出向元労働組合より弔電有無
        dtCondolence["TYOUDEN_MOTO_KUMIAI"] = null;
    }

    let info2 = infoArray[2];

    if (info2 !== undefined) {
        // 香料・供花・弔電差出人_会社名（出向先）
        dtCondolence["SASHIDASHI_SAKI_KAISYA1"] = info2["KAISHAMEI"];
        // 香料・供花・弔電差出人_会社代表者肩書き（出向先）
        dtCondolence["SASHIDASHI_SAKI_KAISYA2"] = info2["YAKUSHOKU"];
        // 香料・供花・弔電差出人_会社代表者氏名（出向先）
        dtCondolence["SASHIDASHI_SAKI_KAISYA3"] = info2["SHIMEI"];
        // 出向先会社より香料
        dtCondolence["KOURYOU_SAKI_KAISYA"] = info2["KORYONUM"];
        // 出向先会社より供花の数
        dtCondolence["KUGE_SAKI_KAISYA"] = info2["KYOKANUM"];
        // 出向先会社より弔電有無
        dtCondolence["TYOUDEN_SAKI_KAISYA"] = info2["TYODENNUM"];

        koryo_sum += Number(info2["KORYONUM"]);
        kyoka_sum += Number(info2["KYOKANUM"]);
        tyoden_sum += Number(info2["TYODENNUM"]);
    } else {
        // 香料・供花・弔電差出人_会社名（出向先）
        dtCondolence["SASHIDASHI_SAKI_KAISYA1"] = null;
        // 香料・供花・弔電差出人_会社代表者肩書き（出向先）
        dtCondolence["SASHIDASHI_SAKI_KAISYA2"] = null;
        // 香料・供花・弔電差出人_会社代表者氏名（出向先）
        dtCondolence["SASHIDASHI_SAKI_KAISYA3"] = null;
        // 出向先会社より香料
        dtCondolence["KOURYOU_SAKI_KAISYA"] = null;
        // 出向先会社より供花の数
        dtCondolence["KUGE_SAKI_KAISYA"] = null;
        // 出向先会社より弔電有無
        dtCondolence["TYOUDEN_SAKI_KAISYA"] = null;
    }


    // 香料合計
    dtCondolence["KOURYOU_GOUKEI"] = koryo_sum;
    // 供花合計
    dtCondolence["KUGE_GOUKEI"] = kyoka_sum;
    // 弔電合計
    dtCondolence["TYOUDEN_GOUKEI"] = tyoden_sum;

}

/**
 * 委任者情報を設定
 * */
function setDelegatorInfo() {
    var ht = new HashTblCommon();
    let shainbango = $("#text-unhappiness-shainbango").val();
    if (shainbango === STRING_EMPTY) {
        shainbango = dtCondolence["UNFORTUNATE_SHAINBANGO"];
    }

    if (shainbango !== null) {
        ht.Add("SHAINBANGO", shainbango);
        wfCommon.getApiInfoAjaxCallBack(GET_CONDOLENCE_SHAIN_INFO_APINAME, ht, setDelegatorItems);
    }
}

/**
 * 重複チェック
 * */
function doubleCheck() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
    handler.AddUrlData();
    // 不幸従業員社員番号
    handler.AddPara("shainbango", dtCondolence["UNFORTUNATE_SHAINBANGO"]);
    // 亡くなられた方カナ氏名（姓）
    handler.AddPara("kanashimeisei", $("#text-died-lastname_kana").val());
    // 亡くなられた方カナ氏名（名）
    handler.AddPara("kanashimeimei", $("#text-died-firstname_kana").val());
    // 亡くなられた方氏名（姓）
    handler.AddPara("shimeisei", $("#text-died-lastname").val());
    // 亡くなられた方氏名（名）
    handler.AddPara("shimeimei", $("#text-died-firstname").val());
    // 亡くなられた方続柄区分
    handler.AddPara("zokugarakbn", $("#select-died-relationship").val());
    // 亡くなられた方性別
    handler.AddPara("seibetsu", $("input[name=radio-died-gender]:checked").val());
    // 亡くなられた方同居別居区分
    handler.AddPara("dokyobekyo", $("input[name=radio-died-living]:checked").val());
    // 亡くなられた方年齢
    handler.AddPara("nenrei", $("#text-died-age").val());
    // 亡くなられた方逝去日
    handler.AddPara("seikyobi", $("#text-died-death_date").val());
    // 亡くなられた方逝去時
    handler.AddPara("seikyojikoku", $("#select-died-death_time").val());

    var result = handler.DoMethodReturnString("Check_Double_Info");
    if (result.indexOf("err@") === 0) {
        wfCommon.Msgbox(result);
        return;
    }
    return JSON.parse(result);
}

/**
  * PDF情報をセット
  * @param {string} callback CALLBACK関数
  */
function setPDFInfo(callback) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Condolence");
    handler.AddUrlData();

    // 会社コード
    handler.AddPara("kaishacode", dtApplicant["KAISHACODE"]);

    handler.DoMethodSetString("Get_PDF_Info", function (data) {
        //例外処理
        if (data.indexOf("err@") === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON対象に転換
        var ret = JSON.parse(data);
        callback(ret);
    });
}

/**
 * PDF　URLを設定
 * 
 * @param {any} data
 */
function setPDFInfoItems(data) {

    dtCondolence["PDF_URL"] = data["Get_PDF_Info"];

    if (dtCondolence["PDF_URL"] === STRING_EMPTY) {
        $("#link-page-pdf").hide();

    } else {
        $("#link-page-pdf").show();
        $("#iframe-pdf").attr("src", dtCondolence["PDF_URL"]);
    }

}

/**
 * 確認画面へ遷移する
 *
 * */
function gotoConfirmPage() {
    // 確認画面にデータをセット
    setDataToConfirmPage();

    // 確認画面へ遷移
    let confirmPage = $("#confirm-page")[0].__component;
    confirmPage.opened = !0;
}

/**
 * 申請修正 各項目：活性・非活性を設定
 *
 * */
function changeItemsDisabled() {
    $("#text-unhappiness-shainbango").attr("disabled", true);
    $("#btn-search-empcode").attr("disabled", true);
    $("#text-died-lastname").attr("disabled", true);
    $("#text-died-firstname").attr("disabled", true);
    $("#text-died-lastname_kana").attr("disabled", true);
    $("#text-died-firstname_kana").attr("disabled", true);
    $("#pulldown-died-relationship")[0].__component._choices.disable();
    $("#pulldown-died-relationship").addClass("a-pulldown--disabled").children().addClass("is-disabled");
    $("input[name=radio-died-gender]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    $("input[name=radio-died-living]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    $("#text-died-age").attr("disabled", true);
    $("#text-died-age").next().children().addClass("a-icon--unit-age-white").removeClass("a-icon--unit-age");
    $("#text-died-death_date").attr("disabled", true);
    $("#text-died-death_date").parents(".a-calendar-field").addClass("a-calendar-field--disabled")
    $("#pulldown-died-death_time")[0].__component._choices.disable();
    $("#pulldown-died-death_time").addClass("a-pulldown--disabled").children().addClass("is-disabled");
    $("input[name=radio-died-dependent]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    $("input[name=radio-mourner]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    $("#text-mourner-lastname").attr("disabled", true);
    $("#text-mourner-firstname").attr("disabled", true);
    $("#text-mourner-lastname_kana").attr("disabled", true);
    $("#text-mourner-firstname_kana").attr("disabled", true);
}
/**
 * 香料・供花・弔電 項目：活性・非活性を設定
 * 
 * */
function changeOpenerItemsDisabled() {
    // 香料合計が０：支給基準外
    if (dtCondolence["KOURYOU_GOUKEI"] === 0) {
        wfCommon.radiosSetVal("radio-opener-koryo", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
        $("input[name=radio-opener-koryo]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    } else {
        // 香料を受取った場合
        if (dtCondolence["KORYOKBN"] === parseInt(NECESSARY_KBN_HITUYOU)) {
            $("input[name=radio-opener-koryo]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
        }
    }
    // 供花合計０：支給基準外
    if (dtCondolence["KUGE_GOUKEI"] === 0) {
        wfCommon.radiosSetVal("radio-opener-kuge", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
        $("input[name=radio-opener-kuge]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    } else {
        // 手配済み
        if (dtCondolence["TEHAIKBN"] === parseInt(STATE_TEHAIZIMI)) {
            $("input[name=radio-opener-kuge]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
        } else {
            // 供花が「受け取る」を選択される場合、弔電が「辞退する」を選択される場合、弔電は非活性になる
            if (dtCondolence["KYOKAKBN"] !== dtCondolence["TYODENKBN"]) {
                if (dtCondolence["KYOKAKBN"] === parseInt(NECESSARY_KBN_HITUYOU)) {
                    $("input[name=radio-opener-tyoden]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
                }
            }
        }
    }
    // 弔電合計０：支給基準外
    if (dtCondolence["TYOUDEN_GOUKEI"] === 0) {
        wfCommon.radiosSetVal("radio-opener-tyoden", NECESSARY_KBN_JITAI, NECESSARY_KBN_JITAI);
        $("input[name=radio-opener-tyoden]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
    } else {
        // 手配済み
        if (dtCondolence["TEHAIKBN"] === parseInt(STATE_TEHAIZIMI)) {
            $("input[name=radio-opener-tyoden]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
        } else {
            // 供花が「辞退する」を選択される場合、弔電が「受け取る」を選択される場合、供花は非活性になる
            if (dtCondolence["KYOKAKBN"] !== dtCondolence["TYODENKBN"]) {
                if (dtCondolence["TYODENKBN"] === parseInt(NECESSARY_KBN_HITUYOU)) {
                    $("input[name=radio-opener-kuge]").attr("disabled", true).parent().addClass("a-radio--disabled").removeClass("a-radio--grey");
                }
            }
        }
    }


}

/**
 * ボタン：表示・非表示を設定
 *
 * */
function setButtonDisplay() {

    if (wfstate <= WF_STATE_DRAFT) {
        // 下書き
        $("#btn-form-draft_save").show();
        // 確認
        $("#btn-form-confirm").show();
        // 申請
        $("#btn-form-request").show();
        // 修正
        $("#btn-form-modify").hide();

    } else {
        // 下書き
        $("#btn-form-draft_save").hide();
        // 確認
        $("#btn-form-confirm").show();
        $("#btn-form-confirm").children().text("申請修正");
        // 申請
        $("#btn-form-request").hide();
        // 修正
        $("#btn-form-modify").show();
        $(".f-modify").hide();
    }
}

/**
 * お届け先エリア：表示・非表示を設定
 *
 * */
function setAddresseeDisplay() {
    let kugekbn = $("input[name=radio-opener-kuge]:checked").val();
    let tyodenkbn = $("input[name=radio-opener-tyoden]:checked").val();
    let allnight = $("input[name=radio-allnight]:checked").val();

    if (kugekbn === NECESSARY_KBN_JITAI && tyodenkbn === NECESSARY_KBN_JITAI) {
        $(".area-allnight").hide();
        if (allnight === LOCATION_ATOKA_KBN) {
            $(".area-rear").hide();
        }
    } else {
        $(".area-allnight").show();
        if (allnight === LOCATION_ATOKA_KBN) {
            $(".area-rear").show();
        }
    }
}
/*▲関数定義エリア▲*/