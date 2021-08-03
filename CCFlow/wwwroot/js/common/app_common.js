/// <reference path="../biz/menu/comm/page_left_menu.js" />
/// <reference path="../biz/menu/page_approval_request_list.js" />
/*▼画面起動共通処理▼*/
$(function () {

    // 日付日本語化対応
    if ($.datetimepicker) {
        moment.locale('ja')
        $.datetimepicker.setLocale('ja');
        $.datetimepicker.setDateFormatter('moment');
    }

    // メニュー（三級以上）の初期化
    $(".a-nav-item__custom").next().fadeToggle(200);
    //$(".a-nav-item__custom").next().children().each(function () {
    //    $(this).fadeToggle();
    //});
    $(".a-nav-item__custom").on("click", function () {
        //$(this).next().children().each(function () {
        //    $(this).fadeToggle();
        //});
      $(this).next().fadeToggle(200);
    });
});
/*▲画面起動共通処理▲*/

/**
 * リストデータ格納
 * @param bEnabled 項目入力可否(true：入力可、false：入力不可)
 */
var HashTblCommon = function () {
    this.ObjArr = {};
    this.Count = 0;
    //追加
    this.Add = function (key, value) {
        if (this.ObjArr.hasOwnProperty(key)) {
            return false; //存在すれば、追加しない
        } else {
            this.ObjArr[key] = value;
            this.Count++;
            return true;
        }
    }
    //指定した項目存在有無
    this.Contains = function (key) {
        return this.ObjArr.hasOwnProperty(key);
    }
    //指定した項目取得
    this.GetValue = function (key) {
        if (this.Contains(key)) {
            return this.ObjArr[key];
        } else {
            throw Error("Hashtable not cotains the key: " + String(key)); //脚本错误
            return;
        }
    }
    //移除
    this.Remove = function (key) {
        if (this.Contains(key)) {
            delete this.ObjArr[key];
            this.Count--;
        }
    }
    //クリア
    this.Clear = function () {
        this.ObjArr = {}; this.Count = 0;
    }
    //キー
    this.GetKeys = function () {
        return Object.keys(this.ObjArr);
    }
    //JSON
    this.stringify = function () {
        return JSON.stringify(this.ObjArr);
    }
}
/**
 * 共通関連
 * 備考：イオンリテール（株）社仕様のみ実現された
 */
var wfCommon = function () {

    /**
     * メッセージボックスの表示
     * @param {any} key　表示文言
     */
    var __Msgbox = function (key) {
        wfCommon.ShowDialog(DIALOG_ALERT, null, key);
    }
    wfCommon.prototype.Msgbox = __Msgbox;

    /**
      * ワークフロー初期データを取得
      * @param objGetTbNm　取得テーブルリスト
      */
    var __initGetDat = function (objGetTbNm) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddUrlData();
        handler.AddPara("ListTblName", JSON.stringify(objGetTbNm));
        // データを取る
        var data = handler.DoMethodReturnString("AppForm_Init");

        return data;
    }
    wfCommon.prototype.initGetDat = __initGetDat;

    /**
     * リスト区分表示
     * @param {any} flg　　一行目に空白行有無（true：空白有、false：空白無）
     * @param {any} obj　　表示データリスト
     * @param {any} selVal selected値
     * @param {any} value　表示値
     * @param {any} name　 表示内容
     * @param {any} id　   表示ID
     * @param {any} divId　外層のDIVのID（.a-pulldown）
     */
    var __initDropdown = function (flg, obj, selVal, value, name, id, divId) {

        // プルダウンリスト
        var list = '<select name="' + id + '" id="' + id + '" class="a-pulldown__select">';

        // オブジェクト定義
        var $object = $("#" + id).parent().parent().parent();

        // 初期化
        $object.empty();

        // 空欄処理
        if (flg) {

            // デフォルト値が空欄の場合
            if (selVal == "0") {
                list = list + '<option value="" selected></option>';
            } else {
                list = list + '<option value=""></option>';
            }
        }

        // リストの生成
        for (var i = 0; i < obj.length; i++) {

            // 選択されている場合
            if (obj[i][value] == selVal) {
                list = list + '<option value="' + obj[i][value] + '" selected>' + obj[i][name] + '</option>';
            } else {
                list = list + '<option value="' + obj[i][value] + '" >' + obj[i][name] + '</option>';
            }
        }

        // リスト固定部
        list = list + '</select><div class="a-pulldown__icon-container"><i class="a-icon a-icon--arrow-down"></i></div>';

        // リフレッシュ
        $object.html(list);
        $("#" + divId)[0].__component.customPulldown("#" + divId);
    }
    wfCommon.prototype.initDropdown = __initDropdown;

    /**
      * ラジオボタン設定
      * @param {any} radiosName　　     ラジオボタン値
      * @param {any} radiosVal　        表示ラジオボタン名
      * @param {any} initVal　          初期表示値
      */
    var __radiosSetVal = function (radiosName, radiosVal, initVal) {
        if (radiosVal === null || radiosVal === "") { radiosVal = initVal }

        $("[name=" + radiosName + "][value=" + radiosVal + "]").prop("checked", true).parent().addClass("a-radio--checked");

    }
    wfCommon.prototype.radiosSetVal = __radiosSetVal;

    /**
      * ダイアログ画面の表示であること。
      * @param {string} dialogType ダイアログ種類。
      *                 alert : 警告（一つボタン）
      *                 confirm : 確認（二うボタン）
      *                 info : 情報（入力可能）
      * @param {string} msgTitle 画面では表示メッセージ。
      * @param {string} msgInfo 画面では表示メッセージ。
      * 
      * @param {string} okBtnText Okボタンの表示名
      * @param {function} okMethodNm OKボタン押下してから実行のメソッド名。
      * @param {Array} okMethodPars OKボタン押下後、実行のメソッド名。
      * 　　　　　　　 呼び出しメソッドのパラメータがなかったら、nullを設定してください。
      * 　　　　　　　 
      * @param {string} cancelBtnText cancelボタンの表示名
      * @param {function} cancelMethodNm キャンセルボタン押下してから実行のメソッド名。
      * @param {Array} cancelMethodPars キャンセルボタン押下後、実行のメソッド名。
      * 　　　　　　　 呼び出しメソッドのパラメータがなかったら、nullを設定してください。
      */
    var __ShowDialog = function (dialogType, msgTitle, msgInfo, okBtnText, okMethodNm, okMethodPars, cancelBtnText, cancelMethodNm, cancelMethodPars) {

        // Form
        var formVa;

        // ダイアログオブジェクト
        var dialogDiv = $("#app-dialog-div")[0];

        // ボタン文言
        var okText = okBtnText ? okBtnText : "はい";
        var cancelText = cancelBtnText ? cancelBtnText : "いいえ";

        // タイトル
        if (msgTitle) {
            $("#app-dialog-title").html(msgTitle);
        }

        // メッセージ内容
        $("#app-dialog-content").html(msgInfo);

        // ボタンの制御
        // ダイアログ種類によって、ボタンエリアを設定する
        switch (dialogType) {

            // 一つボタンの場合
            case DIALOG_ALERT:

                // キャンセルボタンの削除
                $("#app-dialog-cancel").hide();

                // 入力エリアの削除
                $("#denyDiv").hide();
                break;

            // 二つボタンの場合
            case DIALOG_CONFIRM:

                // 入力エリアの削除
                $("#denyDiv").hide();

                // キャンセルボタンの表示
                $("#app-dialog-cancel").show();

                // キャンセルボタン文言の設定
                $("#app-dialog-cancel").children(":first").html(cancelText);

                // Cancelクリックイベント
                $("#app-dialog-cancel").unbind("click");
                $("#app-dialog-cancel").click(function () {
                    if (cancelMethodNm) {
                        cancelMethodNm.apply(this, cancelMethodPars);
                    } else {
                        dialogDiv.__component.opened = !1;
                    }
                });
                break;

            // 入力の場合
            case DIALOG_INFO:

                // キャンセルボタンの表示
                $("#app-dialog-cancel").show();

                // 入力エリアの表示
                $("#denyDiv").show();

                // 必須チェック
                formVa = $("#dialog-form").validate({
                    focusCleanup: true,
                    onkeyup: false,
                    rules: {
                        deny_content: { required: true },
                    }
                });

                // キャンセルボタン文言の設定
                $("#app-dialog-cancel").children(":first").html(cancelText);

                // Cancelクリックイベント
                $("#app-dialog-cancel").unbind("click");
                $("#app-dialog-cancel").click(function () {
                    if (formVa) {
                        formVa.resetForm();
                    }

                    // コメントクリア処理
                    $("#deny_content").val(STRING_EMPTY);

                    if (cancelMethodNm) {
                        cancelMethodNm.apply(this, cancelMethodPars);
                    } else {
                        dialogDiv.__component.opened = !1;
                    }
                });

                break;
        }

        // OKボタン文言の設定
        $("#app-dialog-ok").children(":first").html(okText);

        // OKクリックイベント
        $("#app-dialog-ok").unbind("click");
        $("#app-dialog-ok").click(function () {
            if (okMethodNm) {
                if (dialogType == DIALOG_INFO) {
                    var flg = $("#dialog-form").validate().element($("#deny_content"));
                    if (!flg) {
                        return;
                    }
                }
                okMethodNm.apply(this, okMethodPars);
            }
            dialogDiv.__component.opened = !1;
        });

        // ダイアログ表示
        if (dialogDiv) {
            var dialogComponent = dialogDiv.__component;
            dialogComponent.opened = !0;
        }
    }
    wfCommon.prototype.ShowDialog = __ShowDialog;

    /**
      * 日付を初期化する
      *
      * @param {string}  id_date　      日付のID
      * @param {string}  format　       フォーマット
      */
    var __setdatepicker = function (id_date, format) {

        // フォーマット設定
        var dateFormat = format ? format : DATE_FORMAT_MOMENT_PATTERN_1;

        // 日付適用
        $(id_date).datetimepicker({
            scrollMonth: false,
            scrollInput: false,
            timepicker: false,
            datepicker: true, 
            format: dateFormat
        });
    }
    wfCommon.prototype.setdatepicker = __setdatepicker;

    /**
      * 日付期間パネルを初期化する
      *
      * @param {string}   id_sta　   開始日のID
      * @param {string}   id_end　   終了日のID
      * @param {string}   format　   フォーマット
      * @param {boolean} datepicker　   日付選択パネル表示フラグ
      * @param {boolean} timepicker　   日時選択パネル表示フラグ
      * @param {Function} startShow　開始日付の表示ルール
      * @param {Function} endShow　  終了日付の表示ルール
      */
    var __setdatepickerWithStartEnd = function (id_sta, id_end, format, datepicker, timepicker, startShow, endShow) {

        // フォーマット設定
        var dateFormat = format ? format : DATE_FORMAT_MOMENT_PATTERN_1;

        // 開始日付の表示ルール設定
        var startonShow = startShow ? startShow : function (ct) {
            this.setOptions({
                minDate: 0,
                maxDate: $(id_end).val() ? $(id_end).val() : false
            })
        }

        // 終了日付の表示ルール設定
        var endonShow = endShow ? endShow : function (ct) {
            if ($(id_sta).val() === "") {
                this.setOptions({
                    minDate: 0
                })
            } else {
                this.setOptions({
                    minDate: $(id_sta).val() ? $(id_sta).val() : false
                })
            }
        }

        // 開始日付適用
        $(id_sta).datetimepicker({
            timepicker: timepicker ? timepicker : false
            , datepicker: datepicker ? datepicker : false
            , scrollMonth: false
            , scrollInput: false
            , format: dateFormat
            , onShow: startonShow
        });

        // 終了日付適用
        $(id_end).datetimepicker({
            timepicker: timepicker ? timepicker : false
            , datepicker: datepicker ? datepicker : false
            , scrollMonth: false
            , scrollInput: false
            , format: dateFormat
            , onShow: endonShow
        });
    }
    wfCommon.prototype.setdatepickerWithStartEnd = __setdatepickerWithStartEnd;

    /**
     * 区分コード通りに、区分マスタデータを取得
     * @param objGetTbNm　取得区分リスト
     */
    var __getKbnDat = function (objGetTbNm) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddUrlData();
        handler.AddPara("KbnCodeList", JSON.stringify(objGetTbNm));
        // データを取る
        var data = handler.DoMethodReturnString("Get_Kbn_Dat");
        return data;
    }
    wfCommon.prototype.getKbnDat = __getKbnDat;

    /**
    * API取得(一時廃棄)
    * @param api_name　API関数名
    * @param api_prm   API利用パラメタ格納(HashTable)
    */
    var __getApiInfoAjax = function getApiInfoAjax(api_name, api_prm) {

        //返却API情報
        var apiInfo = "";
        //取得API名前
        var methodName = 'Get_Info';
        //AzureアクセスURL
        var dynamicHandler = "";
        dynamicHandler = "https://aeonapi.azurewebsites.net";
        //HTTP　URL
        var handlerUrl = dynamicHandler + "/WF/Comm/Handler.ashx?";
        //パラメタ格納
        var parameters = new FormData();
        // API関数名
        parameters.append("API_Name", api_name);
        // API利用パラメタ格納
        parameters.append("API_PRM", api_prm.stringify());

        $.ajax({
            url: handlerUrl,
            method: "POST",
            dataType: "json",
            data: parameters,
            contentType: false,
            processData: false,
            async: false, // 非同期処理
            success: function (data) {
                apiInfo = data[methodName];
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                alert(URL + "err@システムに異常が発生しました, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                //ThrowMakeErrInfo("HttpHandler-DoMethodReturnString-" + methodName, textStatus, handlerUrl);
            }
        });
        return apiInfo;
    }
    wfCommon.prototype.getApiInfoAjax = __getApiInfoAjax;


    /**
    * API取得（非同期）
    * @param api_name　API関数名
    * @param api_prm   API利用パラメタ格納(HashTable)
    * @param callback  CALLBACK関数名
    */
    var __getApiInfoAjaxCallBack = function getApiInfoAjaxCallBack(api_name, api_prm, callback) {

        //HTTP　URL
        var handlerUrl = ApidynamicHandler + "API_Name=" + api_name + "&API_PRM=" + api_prm.stringify();
        //取得API名前
        var methodName = 'Get_Info';
        var OcpApimTrace = "true";

        $.ajax({
            type: 'get',
            async: true,
            crossDomain: true,
            url: handlerUrl,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Ocp-Apim-Subscription-Key", OcpApimSubscriptionKey);
                xhr.setRequestHeader("Ocp-Apim-Trace", OcpApimTrace);
            },
            success: function (data) {
                jsonString = data;
                callback(JSON.parse(jsonString)[methodName]);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                alert(URL + "err@システムに異常が発生しました, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
            }

        });
    }
    wfCommon.prototype.getApiInfoAjaxCallBack = __getApiInfoAjaxCallBack;

    /**
     * API基盤から承認情報取得
     * @param api_name　API関数名
     * @param api_prm   API利用パラメタ格納(HashTable)
    */
    var __getApiApprovalInfo = function getApiApprovalInfo(api_name, api_prm) {

        //返却API情報
        var toEmps = "";
        //承認情報取得
        var data = wfCommon.getApiInfoAjax(api_name, api_prm);

        if (data.length > 0) {

            for (var i = 0; i < data.length; i++) {
                if (i == data.length - 1) {
                    toEmps += data[i]["No"];
                }
                else {
                    toEmps += data[i]["No"] + ',';
                }
            }
        }
        return toEmps;
    }
    wfCommon.prototype.getApiApprovalInfo = __getApiApprovalInfo;

    /**
    * メッセージ整形
    * @param message　　メッセージ
    * @param arguments placeholder　メッセージにより、設定する
    */
    var __MsgFormat = function (message) {
        if (!message) return null;
        var str = message.split(/\{\d+?\}/);

        if (arguments.length > 1) {
            var value = arguments[1];
            var val = value.split(',');
            for (var i = 0; i < str.length; i++) {
                //if (!arguments[i + 1]) break;
                if (i == val.length) {
                    break;
                }
                str[i] += val[i];
            }
        }

        return str.join("");
    }
    wfCommon.prototype.MsgFormat = __MsgFormat;

    /**
      *
      * 区分日本語名の取得用メソッド
      *
      * @param value 区分値
      * @param kbnname 区分のDB名
      *
      */
    var __Findkbnname = function (value, kbnname) {
        var obj = dtKbn[kbnname];

        for (var i = 0; i < obj.length; i++) {
            if (obj[i][MT_KBN_KEYVALUE] == value) {
                return obj[i][MT_KBN_KEYNAME];
            }

            continue;
        }
        return "";
    }
    wfCommon.prototype.FindKbnName = __Findkbnname;

    /**
     *
     * 区分コードの取得用メソッド
     *
     * @param value 区分日本語名
     * @param kbnname 区分のDB名
     *
     */
    var __FindkbnCode = function (value, kbnname) {
        var obj = dtKbn[kbnname];

        for (var i = 0; i < obj.length; i++) {
            if (obj[i][MT_KBN_KEYNAME] === value) {
                return obj[i][MT_KBN_KEYVALUE];
            }

            continue;
        }
        return "";
    }
    wfCommon.prototype.FindKbnCode = __FindkbnCode;

    /**
      *
      * メール送信用共通メソッド
      * @param {String} className 処理クラス名
      * @param {String} wfKbn ワークフロー区分
      * @param {String} timingKbn 送信タイミング区分
      * @param {String} workingId ワークID
      * @param {String} tehaiFlg 手配業者区分　※　手配業者の場合「1」、それ以外の場合いらない
      */
    var __DoMailSend = function (className, wfKbn, timingKbn, workingId, tehaiFlg) {

        // 送信インスタンスの生成
        var handler = new HttpHandler("BP.WF.HttpHandler." + className);

        // パラメーター設定
        handler.AddUrlData();

        // ワークフロー区分
        handler.AddPara("wfKbn", wfKbn);

        // 送信タイミング区分
        handler.AddPara("timingKbn", timingKbn);

        // ワークID
        handler.AddPara("workingId", workingId);

        // 手配業者区分
        handler.AddPara("tehaiFlg", tehaiFlg);

        // 送信処理を呼び出す
        var data = handler.DoMethodReturnString("doMailSend");

        if (data.indexOf('err@') == 0) {
            wfCommon.Msgbox(data);
            return;
        }
    }
    wfCommon.prototype.DoMailSend = __DoMailSend;

    /**
      * 年齢計算
      * @param parDate　　 日付
      * @param parBirthday 生年月日（日付）
      */
    var __CalcAge = function calcAge(parDate, parBirthday) {

        dateOfBirth = moment(parBirthday); // 生年月日
        calcDate = moment(parDate); // 計算の日付

        // 西暦を比較して年齢を算出
        let baseAge = calcDate.year() - dateOfBirth.year();

        // 誕生日を作成
        let birthday = moment(
            new Date(
                calcDate.year() + "/" + (dateOfBirth.month() + 1) + "/" + dateOfBirth.date()
            )
        );

        // 今日が誕生日より前の日付である場合、算出した年齢から-1した値を返す
        if (calcDate.isBefore(birthday)) {
            return baseAge - 1;
        }

        // 今日が誕生日 または 誕生日を過ぎている場合は算出した年齢をそのまま返す
        return baseAge;
    }
    wfCommon.prototype.CalcAge = __CalcAge;

    /**
     * サーバの現在時間を取る
     */
    var __getServerDatetime = function () {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddUrlData();
        var data = handler.DoMethodReturnString("GetServerDateTime");
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return null;
        }
        data = JSON.parse(data);
        return new Date(data["serverDateTime"]);
    }
    wfCommon.prototype.getServerDatetime = __getServerDatetime;

    /**
     * サーバの現在日付を取る
     */
    var __getServerDate = function () {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddUrlData();
        var data = handler.DoMethodReturnString("GetServerDate");
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return null;
        }
        data = JSON.parse(data);
        return new Date(data["serverDate"]);
    }
    wfCommon.prototype.getServerDate = __getServerDate;

    /**
      * 左埋めする処理
      * 指定桁数になるまで対象文字列の左側に
      * 指定された文字を埋めます。
      * @param val 左埋め対象文字列
      * @param char 埋める文字
      * @param n 指定桁数
      * @return 左埋めした文字列
      */
    var __paddingLeft = function (val, char, n) {
        var leftval = "";
        for (; leftval.length < n; leftval += char);
        return (leftval + val).slice(-n);
    }
    wfCommon.prototype.paddingLeft = __paddingLeft;


 /**
 * フローシステムデータの取得
 * @param {string} workId 伝票番号
 **/
    var __getFlowSystemDataCallBack = function (workId,callback) {
        var getInfo = 'GetFlowSystemData';
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddPara("WorkId", workId);
        handler.DoMethodSetString(getInfo, function (data) {
            //例外処理
            if (data.indexOf('err@') === 0) {
                wfCommon.Msgbox(data);
                return;
            }
            // JSON対象に転換
            ret = JSON.parse(data);
            callback(ret);
        });
    }
    wfCommon.prototype.getFlowSystemDataCallBack = __getFlowSystemDataCallBack;

    /**
 * フローシステムデータの取得
 * @param {string} workId 伝票番号
 **/
    var __getFlowSystemData = function (workId, callback) {
        var getInfo = 'GetFlowSystemData';
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddPara("WorkId", workId);
        var data = handler.DoMethodReturnString(getInfo);

        // 例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON対象に転換
        data = JSON.parse(data);

        return data[0];
    }
    wfCommon.prototype.getFlowSystemData = __getFlowSystemData;

    /**
    　* 処理待ちフローに遷移する
    　* @param {string} url OID
    **/
    var __todoFlowOpenFrm = function (url,fromPage) {

        window.location.href = url + "&FromPage=" + fromPage;
        //var self = window.open(url);
        
        //var loop = setInterval(function () {
        //    if (self.closed) {
        //        clearInterval(loop);
        //        InitPage();
        //        InitHomeCount();
        //    }
        //}, 1);
    }
    wfCommon.prototype.todoFlowOpenFrm = __todoFlowOpenFrm;

    /**
     * 処理終了フローに遷移する
     * @param {string} workId OID
     * @param {string} nodeID ノードID
     * @param {string} flowNo フローナンバー
    **/
    var __completeFlowOpenFrm = function (workid, nodeID, flowNo, fromPage) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
        handler.AddUrlData();
        handler.AddPara("FromWorkOpt", "1");
        handler.AddPara("WorkID", workid);
        handler.AddPara("FK_Node", nodeID);
        handler.AddPara("FK_Flow", flowNo);
        var data = handler.DoMethodReturnString("Runing_OpenFrm");

        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        if (data.indexOf('urlForm@') === 0) {
            data = data.replace('urlForm@', '');

            if (data.indexOf('FromWorkOpt') === -1)
                data = data + "&FromWorkOpt=1" + "&FromPage=" + fromPage;
            //window.open(data);
            window.location.href = data;
            return;
        }
    }
    wfCommon.prototype.completeFlowOpenFrm = __completeFlowOpenFrm;

    /**
     * 未完了フローに遷移する
     * @param {string} workId OID
     * @param {string} nodeID ノードID
     * @param {string} flowNo フローナンバー
     * @param {string} fid FID
     * @param {string} currNodeId フローcurrノードID
    **/
    var __unCompleteFlowOpenFrm = function (workid, nodeID, flowNo, fid, currNodeId, fromPage) {

        // 現在のノードを取得
        var nowNode = new Entity("BP.WF.Node", nodeID);

        // ツリーフォームであれば、現在の人がいるノードのデータが表示されます
        if (nowNode.FormType === 5) {
            // フローノードを取得

            var currNode = new Entity("BP.WF.Node", currNodeId);
            if (currNode.HisToNDs.indexOf(nodeID) === -1) {
                if ((currNode.RunModel === RunModel.HL || currNode.RunModel === RunModel.FHL || currNode.RunModel === RunModel.FL) && (fid !== 0)) {
                    workID = fid;
                    fid = 0;
                    nodeID = currNodeId;
                }
            } else {
                // 現在のノードのモードを判断する
                if ((currNode.RunModel === RunModel.HL || currNode.RunModel === RunModel.FHL || currNode.RunModel === RunModel.FL) && nowNode.RunModel === RunModel.SubThread) {
                    workID = fid;
                    fid = 0;
                    nodeID = currNodeId;
                } else if (currNode.RunModel === RunModel.SubThread) {
                    nodeID = currNodeId;
                }
            }
        }

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
        handler.AddUrlData();
        handler.AddPara("FromWorkOpt", "1");
        handler.AddPara("WorkID", workid);
        handler.AddPara("FK_Node", nodeID);
        handler.AddPara("FK_Flow", flowNo);
        handler.AddPara("FID", fid);
        var data = handler.DoMethodReturnString("Runing_OpenFrm");

        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }

        // 自分定義フォームの場合
        if (data.indexOf('urlForm@') === 0) {
            data = data.replace('urlForm@', '');
            if (data.indexOf("http") === -1)
                data = basePath + data;
            if (data.indexOf('FromWorkOpt') === -1)
                data = data + "&FromWorkOpt=1" + "&FromPage=" + fromPage;
            //window.open(data);
            window.location.href = data;
            return;
        }
        // 上記以外
        if (data.indexOf('url@') === 0) {
            data = data.replace('url@', '');

            if (data.indexOf('FromWorkOpt') === -1)
                data = data + "&FromWorkOpt=1" + "&FromPage=" + fromPage;
            data = "../WF/" + data;
            //window.open(data);
            window.location.href = data;
            return;
        }
    }
    wfCommon.prototype.unCompleteFlowOpenFrm = __unCompleteFlowOpenFrm;

    /**
     * 一覧データ取得
     * @param {string} type 取得種類
     * @param {string} callback CALLBACK関数
    **/
    var __GetDensinList = function (type,callback) {
        var ret = '';
        var getInfo = 'GetDensinList';
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_DensinList");
        handler.AddUrlData();
        handler.AddPara("listType", type);
        handler.DoMethodSetString(getInfo, function (data) {
            //例外処理
            if (data.indexOf('err@') === 0) {
                wfCommon.Msgbox(data);
                return;
            }
            // JSON対象に転換
            ret = JSON.parse(data);
            callback(ret);
        });
    }
    wfCommon.prototype.GetDensinList = __GetDensinList;

    /**
     * 一覧のボタン（引戻、差戻、否認、承認）を押下する
     * @param {string} workId OID
     * @param {string} nodeID ノードID
     * @param {string} flowNo フローナンバー
     * @param {string} unSendToNode 引戻ノード
     * @param {string} fid 
     * @param {string} TblName フローの関連トランザクションテーブル
     * @param {string} FK_Node (差戻のみ利用)
     * @param {string} WF_Comment (差戻・否認の理由)
    **/
    var __DoListButton = function (type, user, FK_Flow, WorkID, unSendToNode, fid, TblName, FK_Node, WF_Comment) {
        var getInfo = 'DoListButton';
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_DensinList");
        handler.AddUrlData();
        handler.AddPara("btnType", type);
        handler.AddPara("UserNo", user);
        handler.AddPara("FK_Flow", FK_Flow);
        handler.AddPara("WorkID", WorkID);
        handler.AddPara("unSendToNode", unSendToNode);
        handler.AddPara("fid", fid);
        handler.AddPara("TblName", TblName);
        handler.AddPara("FK_Node", FK_Node);
        handler.AddPara("WF_Comment", WF_Comment);
        var data = handler.DoMethodReturnString(getInfo);

        // 例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON対象に転換
        return data;
    }
    wfCommon.prototype.DoListButton = __DoListButton;


    /**
    * フローへ遷移メソッド
     * @param {string} workId OID
     * @param {string} kind フロー種類
     * @param {string} frompage フロー種類
    */
    var __OpenFlowFrom = function (workid, kind, frompage) {

        //フローデータの取得
        var data = wfCommon.getFlowSystemData(workid);
        var WFState = data["WFState"];
        var WorkID = data["WorkID"];
        var FK_Node = data["FK_Node"];
        var FK_Flow = data["FK_Flow"];
        var FID = data["FID"];
        var PWorkID = data["PWorkID"];
        var CurrNodeId = data["CurrNode"];
        var timeKey = Math.random();

        //フロー状態判断
        if (WFState === WF_STATE_OVER) {
            //処理済みフローに遷移する
            wfCommon.completeFlowOpenFrm(WorkID, FK_Node, FK_Flow, frompage);
        }
        else if (WFState === WF_STATE_SINSEIZUMI || WFState === WF_STATE_DRAFT || WFState === WF_STATE_BACK) {
            // 未完了の場合
            if (kind === GET_MY_UNCOMPLETE) {
                //未完了フローに遷移する
                wfCommon.unCompleteFlowOpenFrm(WorkID, FK_Node, FK_Flow, FID, CurrNodeId, frompage);
            }
            else {
                var paras = data["AtPara"];
                if (paras === null)
                    paras = "";
                paras = paras.replace("'", "\\'");
                while (true) {
                    paras = paras.replace('@', '&');

                    if (paras.indexOf('@') < 0) {
                        break;
                    }
                }
                paras = "1" + paras;
                var url = "../../../../WF/MyFlow.htm?FK_Flow=" + FK_Flow + "&PWorkID=" + PWorkID + "&FK_Node=" + FK_Node + "&FID=" + FID + "&WorkID=" + WorkID + "&IsRead=0&T=" + timeKey + "&Paras=" + paras;
                //Urlによって、フローに遷移する
                wfCommon.todoFlowOpenFrm(url, frompage)
            }
        }
    }
    wfCommon.prototype.OpenFlowFrom = __OpenFlowFrom;

    /**
     * 時刻データリストを取得
     * @param {Int} step (15,30) （省略は60）
     */
    var __getTimeData = function (step) {
        const hour = 24;

        var time = { "content": [] }

        if (step === null || step === undefined) {
            step = 60;
        }

        var count = Math.round(60 / step);

        for (let i = 0; i < hour; i++) {
            for (let j = 0; j < count; j++) {
                let m = step * j;
                time["content"].push({ "value": __prefixInteger(i, 2) + ":" + __prefixInteger(m, 2), "name": __prefixInteger(i, 2) + ":" + __prefixInteger(m, 2) });
            }
        }
        return time;
    }
    wfCommon.prototype.getTimeData = __getTimeData;

    /**
     * 0を補足
     * @param {Int} num
     * @param {Int} length
     */
    var __prefixInteger = function (num, length) {
        return (Array(length).join('0') + num).slice(-length);
    }
    wfCommon.prototype.prefixInteger = __prefixInteger;

    /**
      * ワークフロー初期データを取得
      * @param {Array} objGetTbl　取得テーブルリスト
      * @param {string} callback CALLBACK関数
      */
    var __initGetData = function (objGetTbl, callback) {

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddUrlData();
        handler.AddPara("ListTblName", JSON.stringify(objGetTbl));
        handler.DoMethodSetString("AppForm_Init", function (data) {
            //例外処理
            if (data.indexOf('err@') === 0) {
                wfCommon.Msgbox(data);
                return;
            }
            // JSON対象に転換
            var ret = JSON.parse(data);
            callback(ret);
        });
    }
    wfCommon.prototype.initGetData = __initGetData;

    /**
     * バッジのローカルデータの取得
     */
    var __getBadgeLocalStorage = function () {

        // バッジ情報取得
        var usrLocal = localStorage.getItem(webUser.No);

        // 取得出来ない場合、新規作成
        if (!usrLocal) {
            usrLocal = {};
            usrLocal[GET_APPROVAL_INPROCESS] = {};
            usrLocal[GET_APPROVAL_COMPLETE] = {};
            usrLocal[GET_MY_DRAFT] = {};
            usrLocal[GET_MY_DIFFERENCE] = {};
            usrLocal[GET_MY_COMPLETE] = {};
            usrLocal["notice"] = {};
        } else {
            usrLocal = JSON.parse(usrLocal);
        }
        return usrLocal;
    }
    wfCommon.prototype.getBadgeLocalStorage = __getBadgeLocalStorage;

    /**
     * 「自分の申請を見る」ローカルデータの更新
     * @param {string} workid　WorkId
     */
    var __updateSelfBadgeLocalStorage = function (workid) {

        // タブ配列
        var selfs = [GET_MY_COMPLETE, GET_MY_DIFFERENCE, GET_MY_DRAFT];
        var usrLocal = __getBadgeLocalStorage();

        // 各タブの更新
        $.each(selfs, function (index, value) {
            delete usrLocal[value][workid];
        });
    }
    wfCommon.prototype.updateSelfBadgeLocalStorage = __updateSelfBadgeLocalStorage;

    /**
     * 「承認依頼を見る」ローカルデータの更新
     * @param {string} workid　WorkId
     */
    var __updateApplyBadgeLocalStorage = function (workid) {

        // タブ配列
        var selfs = [GET_APPROVAL_COMPLETE, GET_APPROVAL_INPROCESS];
        var usrLocal = __getBadgeLocalStorage();

        // 各タブの更新
        $.each(selfs, function (index, value) {
            delete usrLocal[value][workid];
        });
    }
    wfCommon.prototype.updateApplyBadgeLocalStorage = __updateApplyBadgeLocalStorage;

    /**
     * 電話番号のイベント初期化
     * 
     * @param {string} tel　ID
     */
    var __initTelEvent = function (tel) {
        for (let i = 1; i <= 3; i++) {
            $("#" + tel + i).on("change", function () {
                let tel1Value = $("#" + tel + "1").val();
                let tel2Value = $("#" + tel + "2").val();
                let tel3Value = $("#" + tel + "3").val();

                if (tel1Value !== "" && tel2Value !== "" && tel3Value !== "") {
                    $("#" + tel).val(tel1Value + "-" + tel2Value + "-" + tel3Value);
                } else {
                    $("#" + tel).val("");
                }
            });
        }
    }
    wfCommon.prototype.initTelEvent = __initTelEvent;

    /**
     * 金額のフォーマット
     *
     * @param {number} num
     */
    var __comma = function(num) {
        return String(num).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
    }
    wfCommon.prototype.comma = __comma;

    /**
     * 郵便番号の入力設定
     * 
     * @param {string} zipCode
     */
    var __setZipCodeInput = function (zipCode) {
        $("#" + zipCode).on("change", function () {
            var zipCodeValue = $("#" + zipCode).val();
            if (zipCodeValue.length === 7 && zipCodeValue.indexOf("-") === -1) {
                $("#" + zipCode).val(zipCodeValue.substring(0, 3) + "-" + zipCodeValue.substring(3));
            }
        });

        $("#" + zipCode).on("focus", function () {
            var zipCodeValue = $("#" + zipCode).val();

            $("#" + zipCode).val(zipCodeValue.replace("-",""));
        });
    }
    wfCommon.prototype.setZipCodeInput = __setZipCodeInput;

    /**
      * 日付(適用期間)を初期化する
      *
      * @param {string}  id_date　      日付のID
      * @param {string}  format　       フォーマット
      * @param {Date}    min　   　　　 適用期間最小値
      * @param {Date}    max　   　　   適用期間最大値
      */
    var __setdatepickerplus = function (id_date, format, min, max) {

        // フォーマット設定
        let dateFormat = format ? format : DATE_FORMAT_MOMENT_PATTERN_1;
        
        let onShow = function (ct) {
            this.setOptions({
                minDate: min ? min : false,
                maxDate: max ? max : false
            })
        }

        // 日付適用
        $(id_date).datetimepicker({
            scrollMonth: false,
            scrollInput: false,
            timepicker: false,
            format: dateFormat,
            onShow: onShow
        });
    }
    wfCommon.prototype.setdatepickerplus = __setdatepickerplus;
}

/**
  * HTMLファイルから内容を読み込んで、該当画面のDIVタグに追加すること。
  * @param {divName} string 該当DIVのID(親のDIVのID　※追加対象)。
  * @param {fileName} string 読み込むHTML。
  * @param {dicVal} duration 表示内容の設定（Dictionaryのタイプ）。
  *             {key1:value1,key2:value2,...}の形
  *             key:設定タグのid
  *             value:表示値
  * @param {replaceName} string 読み込んだ内容に対して、IDの切替。
  *                             設定しなかったら、divNameで切替。
  */
function GetSubHtmlDataToHtml(divName, fileName, dicVal, replaceName) {
    $.ajax({
        type: 'get',
        url: fileName,
        cache: false,
        async: false,
        dataType: 'html',
        success: function (data) {

            if (replaceName) {
                data = data.replace(/\{\pageId\}/g, replaceName); //header.htmlの{replaceName}を置換
            }
            else {
                data = data.replace(/\{\pageId\}/g, divName); //header.htmlの{pageId}を置換
            }

            var divId = "#" + divName;
            $(divId).append(data);

            // 画面に表示値を設定すること
            if (dicVal) {
                // 引数keyのデータより、valを取得して、画面に設定すること
                Object.keys(dicVal).forEach(
                    function (value) {
                        var setId = "#" + value;

                        // ラベル項目の判断
                        var innerHtml = STRING_EMPTY;
                        if ($(setId).get(0).tagName === "LABEL") {
                            // 子ノートの取得
                            $(setId).children().each(function (index, element) {
                                innerHtml += element.outerHTML;
                            });
                        }

                        $(setId).html(this[value] + innerHtml);
                    }, dicVal);
            }

            // $(divId).trigger("create");
        }
    });
}
