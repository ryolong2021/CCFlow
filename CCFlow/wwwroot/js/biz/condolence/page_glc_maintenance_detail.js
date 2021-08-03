(function ($) {

    $.fn.extend({

        "InitPageForDetail": function (optionData) {

            // 画面初期化
            InitPageSyosai(optionData);
        }
    });

    /*▼関数定義エリア▼*/
    /**
     * 詳細画面初期化
     **/
    function InitPageSyosai(optionData) {

        $("#dennpyouno_selected").val(optionData.oid);

        //詳細画面表示されているフラグ
        detailsflg = true;

        //var objGetTbl = {};
        //objGetTbl[0] = 'KBN';

        ////画面表示データ取得
        //var data = wfCommon.initGetDat(objGetTbl);
        //// 例外処理
        //if (data.indexOf('err@') === 0) {
        //    wfCommon.Msgbox(data);
        //    return;
        //}
        //// JSON対象に転換
        //data = JSON.parse(data);

        ////区分マスタ格納
        //dtKbn = data[objGetTbl[0]];

        //弔事連絡票マスタ格納
        GetCondolenceInfo(optionData);

        //イベント定義
        createoneventSyosai(optionData);

        
    }

    /**
     * 詳細画面初期化
     **/
    function initPageTwo(dtCondolence, optionData) {

        //詳細画面香料手配区分設定
        setkoryokbndetails(dtCondolence[0]["GLCKORYOKBN"]);
        //wfCommon.initDropdown(false, dtKbn["GLCKORYOKBN"], dtCondolence[0]["GLCKORYOKBN"], MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "koryokbn_details");

        // 申請番号
        $('#oid_details').html(optionData.applyNumber);

        // 代理申請者の情報エリア
        // 代理申請のみを表示する
        if (dtCondolence[0]["SHINSEISYAKBN"] === parseInt(SHINSEISYA_KBN_DAIRI)) {

            //代理申請者の情報エリア表示
            $("#dairilistview").css('display', 'block');

            // 代理社員番号
            $('#dairi_syainbango_details').html(dtCondolence[0]["DAIRISHINSNEISYA_SHAINBANGO"]);

            // 代理社員名
            $('#dairi_syainname_details').html(dtCondolence[0]["DAIRISHINSNEISYA_MEI"]);

            // 代理所属
            $('#dairi_syozoku_details').html(dtCondolence[0]["DAIRISHINSNEISYA_SYOZOKU"]);

            // 連絡先電話番号
            $('#dairi_tel_details').html(dtCondolence[0]["RENRAKUSAKITEL"]);

            // 連絡先メール
            $('#dairi_mail_details').html(dtCondolence[0]["RENRAKUSAKIMAIL"]);

        }
        else {
            //代理申請者の情報エリア非表示
            $("#dairilistview").css('display', 'none');
        }

        // ご不幸にあわれた従業員の情報エリア
        // 社員番号
        $('#syainbango_details').html(dtCondolence[0]["UNFORTUNATE_SHAINBANGO"]);

        // 氏名
        $('#simei_details').html(dtCondolence[0]["UNFORTUNATE_KANJIMEI"]);

        // フリガナ
        $('#furikana_details').html(dtCondolence[0]["UNFORTUNATE_FURIGANAMEI"]);

        // 会社名称
        $('#kaisyameisyo_details').html(dtCondolence[0]["UNFORTUNATE_KAISYAMEI"]);

        // 正式組織名・上
        $('#syosikiwue_details').html(dtCondolence[0]["UNFORTUNATE_SEISHIKISOSHIKIUE"]);

        // 下
        $('#sosikisita_details').html(dtCondolence[0]["UNFORTUNATE_SEISHIKISOSHIKISHITA"]);

        // 社員区分
        $('#syainkubu').html(dtCondolence[0]["UNFORTUNATE_SYAINNKBN"]);

        // 職位
        $('#syokui_details').html(dtCondolence[0]["UNFORTUNATE_SYOKUIKBN"]);

        // 組合区分
        $('#kumiai_details').html(dtCondolence[0]["UNFORTUNATE_KUMIAIKBN"]);

        // グッドライフ区分
        $('#glc_details').html(dtCondolence[0]["UNFORTUNATE_GLCKBN"]);

        // 本人申請のみを表示する
        if (dtCondolence[0]["SHINSEISYAKBN"] === parseInt(SHINSEISYA_KBN_HONNIN)) {

            // 連絡先電話番号、連絡先メール表示
            //$("#telandmail").css('display', 'block');
            $('.telandmail').css('display', 'block');

            // 連絡先電話番号
            $('#tel_details').html(dtCondolence[0]["RENRAKUSAKITEL"]);

            // 連絡先メール
            $('#mail_details').html(dtCondolence[0]["RENRAKUSAKIMAIL"]);

            // ご不幸にあわれた従業員の情報エリアのタイトル
            $('#sinseiLi').html('社員情報');
        }
        else {
            // 連絡先電話番号、連絡先メール非表示
            //$("#telandmail").css('display', 'none');
            $('.telandmail').css('display', 'none');

            // ご不幸にあわれた従業員の情報エリアのタイトル
            $('#sinseiLi').html('ご不幸にあわれた方の社員情報');
        }

        // 亡くなられた方情報エリア
        //氏名（姓）
        $('#dead_shimei_sei_details').html(dtCondolence[0]["DEAD_SHIMEI_SEI"]);

        //氏名（名）
        $('#dead_shimei_mei_details').html(dtCondolence[0]["DEAD_SHIMEI_MEI"]);

        //カナ氏名（姓）
        $('#dead_kanashimei_sei_details').html(dtCondolence[0]["DEAD_KANASHIMEI_SEI"]);

        //カナ氏名（名）
        $('#dead_kanashimei_mei_details').html(dtCondolence[0]["DEAD_KANASHIMEI_MEI"]);

        //従業員との続柄
        $('#dead_jugyoin_zokugarakbn_details').html(setkubunInfo(dtKbn["DEAD_KBN"], dtCondolence[0]["DEAD_JUGYOIN_ZOKUGARAKBN"]));

        //性別
        $('#dead_seibetsu_details').html(setkubunInfo(dtKbn["SEIBETSU_KBN"], dtCondolence[0]["DEAD_SEIBETSU"]));

        //同居/別居
        $('#dead_dokyo_details').html(setkubunInfo(dtKbn["DOKYO_BEKYO_KBN"], dtCondolence[0]["DEAD_DOKYO_BEKYO"]));

        //逝去日
        //setdatetimeValue("#dead_seikyobi_details", dtCondolence[0]["DEAD_SEIKYOBI"], DATE_FORMAT_PATTERN_1, true);
        //逝去日付
        let deathdate = dtCondolence[0]["DEAD_DATE"];
        if (deathdate !== null) {
            $("#deaddate_seikyobi_details").html(moment(deathdate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }

        //逝去時刻
        $('#deadtime_seikyobi_details').html(deathTimeList["content"].find(obj => obj.value === dtCondolence[0]["DEAD_TIME"]).name);

        //年齢
        if (dtCondolence[0]["DEAD_NENREI"] === null) {
            $('#dead_nenrei_details').html(STRING_EMPTY);
        } else {
            $('#dead_nenrei_details').html(dtCondolence[0]["DEAD_NENREI"] + '歳');
        }

        // 香料発行区分
        $('#koryo_details').html(setkubunInfo(dtKbn["NECESSARY_KBN"], dtCondolence[0]["KORYOKBN"]));

        //香料給付エリア
        // 給付内容
        //出向元会社より香料がある
        if (dtCondolence[0]["KOURYOU_MOTO_KAISYA"] <= 0 || dtCondolence[0]["KOURYOU_MOTO_KAISYA"] === null || dtCondolence[0]["KOURYOU_MOTO_KAISYA"] === "") {

            $("#label-standard-info0").parent().hide();
        } else if (dtCondolence[0]["KOURYOU_MOTO_KAISYA"] > 0) {

            $("#label-standard-info0").parent().show();
            $("#label-standard-kaishaname0").text(dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI1"]);
            $("#label-standard-yakushoku0").text(dtCondolence[0]["SASHIDASHI_MOTO_KAISYA2"]);
            $("#label-standard-shimei0").text(dtCondolence[0]["SASHIDASHI_MOTO_KAISYA3"]);
            $('#label-standard-koryonum0').html(String(dtCondolence[0]["KOURYOU_MOTO_KAISYA"]).replace(/\B(?=(?:\d{3})+\b)/g, ",") + '円');
        }

        //出向元労働組合より香料がある
        if (dtCondolence[0]["KOURYOU_MOTO_KUMIAI"] <= 0 || dtCondolence[0]["KOURYOU_MOTO_KUMIAI"] === null || dtCondolence[0]["KOURYOU_MOTO_KUMIAI"] === "") {

            $("#label-standard-info1").parent().hide();
        } else if (dtCondolence[0]["KOURYOU_MOTO_KUMIAI"] > 0) {

            $("#label-standard-info1").parent().show();
            $("#label-standard-kaishaname1").text(dtCondolence[0]["SASHIDASHI_MOTO_KAISYA1"]);
            $("#label-standard-yakushoku1").text(dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI2"]);
            $("#label-standard-shimei1").text(dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI3"]);
            $('#label-standard-koryonum1').html(String(dtCondolence[0]["KOURYOU_MOTO_KUMIAI"]).replace(/\B(?=(?:\d{3})+\b)/g, ",") + '円');
        }

        //出向先会社より香料がある
        if (dtCondolence[0]["KOURYOU_SAKI_KAISYA"] <= 0 || dtCondolence[0]["KOURYOU_SAKI_KAISYA"] === null || dtCondolence[0]["KOURYOU_SAKI_KAISYA"] === "") {

            $("#label-standard-info2").parent().hide();
        } else if (dtCondolence[0]["KOURYOU_SAKI_KAISYA"] > 0) {

            $("#label-standard-info2").parent().show();
            $("#label-standard-kaishaname2").text(dtCondolence[0]["SASHIDASHI_SAKI_KAISYA1"]);
            $("#label-standard-yakushoku2").text(dtCondolence[0]["SASHIDASHI_SAKI_KAISYA2"]);
            $("#label-standard-shimei2").text(dtCondolence[0]["SASHIDASHI_SAKI_KAISYA3"]);
            $('#label-standard-koryonum2').html(String(dtCondolence[0]["KOURYOU_SAKI_KAISYA"]).replace(/\B(?=(?:\d{3})+\b)/g, ",") + '円');
        }
    }

    //香料区分設定
    function setkoryokbndetails(koryokbn) {
        // options
        //var str = "";

        switch (String(koryokbn)) {

            //事業所
            case KORYO_KBN_GIUMUSIYO:
                //保存ボタン活性
                $("#btn_submit_detail").prop("disabled", false);
                //香料区分活性
                $("#koryokbn_details").prop("disabled", false);

                var giumusiyo = [];
                giumusiyo[0] = dtKbn["GLCKORYOKBN"][0];
                giumusiyo[1] = dtKbn["GLCKORYOKBN"][2];

                wfCommon.initDropdown(false, giumusiyo, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "koryokbn_details", "pulldown");
                //str += '<option value="0" selected>事業所</option>';
                //str += '<option value="2">GLC</option>';
                break;
            //辞退
            case KORYO_KBN_JITAI:
                //保存ボタン活性
                $("#btn_submit_detail").prop("disabled", false);
                //香料区分活性
                $("#koryokbn_details").prop("disabled", false);

                var jitai = [];
                jitai[0] = dtKbn["GLCKORYOKBN"][1];
                jitai[1] = dtKbn["GLCKORYOKBN"][2];

                wfCommon.initDropdown(false, jitai, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "koryokbn_details", "pulldown");
                //str += '<option value="1" selected>辞退</option>';
                //str += '<option value="2">GLC</option>';
                break;
            //GLC
            case KORYO_KBN_GLC:
                //保存ボタン非活性
                $("#btn_submit_detail").prop("disabled", true);
                //香料区分活性
                $("#koryokbn_details").prop("disabled", true);

                var glc = [];
                glc[0] = dtKbn["GLCKORYOKBN"][2];

                wfCommon.initDropdown(false, glc, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "koryokbn_details", "pulldown");
                //$("#koryokbn_details").prop("disabled", true);
                //str += '<option value="2" selected >GLC</option>';
                break;
            case "null":
                var kuhaku = [];
                kuhaku[0] = dtKbn["GLCKORYOKBN"][1];
                kuhaku[1] = dtKbn["GLCKORYOKBN"][2];

                wfCommon.initDropdown(false, kuhaku, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "koryokbn_details", "pulldown");

                //str += '<option value="1" selected>辞退</option>';
                //str += '<option value="2">GLC</option>';
                break;
        }
        //$("#koryokbn_details").html(str);
        //$("#koryokbn_details").selectmenu().selectmenu('refresh');
    }


    //区分値取得
    function setkubunInfo(val1, val2) {
        var obj = val1;
        var text = STRING_EMPTY;
        for (var i = 0; i < obj.length; i++) {

            if (parseInt(obj[i]["KBNVALUE"]) === val2) {
                text = obj[i]["KBNNAME"];
            }
        }
        return text;
    }

    /**
     * 画面日時の共通設定
     * 
     * @param {any} id
     * @param {any} value
     * @param {any} format
     * @param {any} flg　日時フラグ　true:時間を含む
     */
    function setdatetimeValue(id, date, format, flg) {

        if (date === null || date === "") return;

        var result = date.substr(0, 4) + format.substr(4, 1);
        result += date.substr(5, 2) + format.substr(7, 1);
        result += date.substr(8, 2) + format.substr(10, 2);
        if (flg) {
            result += date.substr(11, 2) + format.substr(14);
        }
        // フォーマットを実行し、日時を設定
        $(id).html(result);
    }

    /**
     * 香料手配区分状態更新処理
     */
    function createoneventSyosai(optionData) {

        // 保存ボタンをクリックイベント
        $("#btn_submit_detail").on("click", function () {

            $("#dennpyouno_selected").val($('#koryokbn_details').val());
            $("#koryokbn_selected").val($("#dennpyouno_selected").val());
            var info = "";

            if (detailsflg === true) {
                info = "_details"
            }

            wfCommon.ShowDialog(
                DIALOG_CONFIRM,
                STRING_EMPTY,
                wfCommon.MsgFormat(msg["W0004"], optionData.applyNumber),
                "はい",
                updateKoryoState,
                new Array(optionData),
                "いいえ"
            );
        });

        //Cancelボタン押下処理
        $("#btn_back_details").on("click", function () {

            // ダイアログエリアを閉じる 
            $("#detail-screen").removeClass("o-modal--opened");
        });
    }

    /**
     * 香料手配区分状態更新処理_OKボタンの処理
     */
    function updateKoryoState(optionData) {

        // ダイアログ画面を閉じること
        $("#app-dialog-div").removeClass("o-modal--opened");

        var ret = changeKoryoKbn(optionData);
        if (ret) {

            // 更新成功のダイアログを表示すること
            wfCommon.ShowDialog(
                DIALOG_ALERT,
                STRING_EMPTY,
                wfCommon.MsgFormat(msg["I0004"], optionData.applyNumber),
                "はい",
                detailClose
            );

            $("#detail-screen").removeClass("o-modal--opened");
            //window.location.href = "form_arrange_trader_list.html";

            // 一覧画面に戻る
            optionData.onOkClick($('#koryokbn_details').val());
        }
    }

    /**
   * 更新処理チェック
   */
    function changeKoryoKbn(optionData) {

        var ret = true;

        var handler = new HttpHandler("BP.WF.HttpHandler.Mn_GLCMaintenanceList");
        handler.AddUrlData();
        //伝票番号を設定
        handler.AddPara("strOid", optionData.oid);
        //GLC香料変更区分
        handler.AddPara("glckoryokbn", $('#koryokbn_details').val());

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
   * 詳細画面を閉じる
   */
    function detailClose() {

        $("#app-dialog-div").removeClass("o-modal--opened");
    }

    /**
        * 弔辞連絡情報を取得
        */
    function GetCondolenceInfo(optionData) {

        var handler = new HttpHandler("BP.WF.HttpHandler.Mn_ArrangeTraderList");
        handler.AddUrlData();
        handler.AddPara("strOid", optionData.oid);
        handler.DoMethodSetString("GetCondolenceInfo", function (data) {

            if (data.indexOf('err@') === 0) {
                wfCommon.Msgbox(data);
                return;
            }

            // 確認画面初期化メソッドの呼び出し
            initPageTwo(JSON.parse(data), optionData)
        });
    }
})(jQuery);