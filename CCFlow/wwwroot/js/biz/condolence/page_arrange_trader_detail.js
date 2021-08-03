(function ($) {

    $.fn.extend({

        "InitPageForDetail": function (optionData) {

            // 画面初期化
            InitPageSyosai(optionData);
        }
    });

    /**
     * 画面初期化
     **/
    function InitPageSyosai(optionData) {
        var objGetTbl = {};
        objGetTbl[0] = 'KBN';
        var kbnList = new HashTblCommon();

        //画面表示データ取得
        var data = wfCommon.initGetDat(objGetTbl);
        // 例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        // JSON対象に転換
        data = JSON.parse(data);

        //区分マスタ格納
        dtKbn = data[objGetTbl[0]];

        //弔辞連絡情報を取得
        GetCondolenceInfo(optionData);

        // 入力チェックを設定する
        setInputCheck();   

        // 手配不能の理由の長さの制御の設定(ダイアログの理由項目)
        $("#inputlength").attr("maxlength", "100");
    }

    /*
     *
     * 確認画面初期化用メソッド（入力画面からの場合）
     *
     */
    function initPageTwo(dtCondolence, optionData) {

        //社員情報取得
        var shainInfo = getShainInfo(dtCondolence[0]["DAIRISHINSNEISYA_SHAINBANGO"] !== null ? dtCondolence[0]["DAIRISHINSNEISYA_SHAINBANGO"] : dtCondolence[0]["UNFORTUNATE_SHAINBANGO"]);

        //手配状態 手配不能
        if (dtCondolence[0]["TEHAIKBN"] === parseInt(STATE_TEHAIFUNO)) {

            $('.tehaijyotai').hide();
            $('.tehaifuno_details').show();
            $('#tehaifuno').html(setkubunInfo(dtKbn["TEHAIKBN"], parseInt(dtCondolence[0]["TEHAIKBN"])));

            //手配不能コメント
            $('.tehaifunocoment1').hide();
            $('.tehaifunocomentlab').show();
            $('.tehaifunocoment2').show();
            $('#comennt2').html(dtCondolence[0]["TEHAIFUNOU_COMMENT"]);
            $('.space12').hide();
            $('.space34').show();       

            // 履歴日時と履歴ユーザーの設定
            setHistoryInfo(dtCondolence);

            //保存ボタン非表示
            $('.hozon').hide();

            //手配状態 完了
        } else if (dtCondolence[0]["TEHAIKBN"] === parseInt(STATE_TEHAIZIMI)) {

            var mitehai = [];
            mitehai[0] = dtKbn["TEHAIKBN"][1];
            mitehai[1] = dtKbn["TEHAIKBN"][5];

            //⼿配状態設定
            wfCommon.initDropdown(false, mitehai, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "dead_jugyoin_zokugarakbn", "pulldown");

            $('.tehaifuno_details').hide();
            $('.tehaijyotai').show();

            //手配不能コメント
            $('.tehaifunocoment1').hide();
            $('.tehaifunocomentlab').hide();
            $('.tehaifunocoment2').hide();
            $('.space12').hide();
            $('.space34').show();

            // 履歴日時と履歴ユーザーの設定
            setHistoryInfo(dtCondolence);

            //保存ボタン非表示
            // $('.hozon').hide();

            //手配状態 完了（キャンセル）
        } else if (dtCondolence[0]["TEHAIKBN"] === parseInt(STATE_CANCEL)) {

            $('.tehaijyotai').hide();
            $('.tehaifuno_details').show();
            $('#tehaifuno').html(setkubunInfo(dtKbn["TEHAIKBN"], parseInt(dtCondolence[0]["TEHAIKBN"])));

            //手配不能コメント
            $('.tehaifunocoment1').hide();
            $('.tehaifunocomentlab').hide();
            $('.tehaifunocoment2').hide();
            $('.space12').hide();
            $('.space34').show();

            // 履歴日時と履歴ユーザーの設定
            setHistoryInfo(dtCondolence);

            //保存ボタン非表示
            $('.hozon').hide();

            //キャンセル(有償)
        } else if (dtCondolence[0]["TEHAIKBN"] === parseInt(STATE_CANCEL_PAID)) {

            var mitehai = [];
            mitehai[0] = dtKbn["TEHAIKBN"][5];

            //⼿配状態設定
            $('#tehaifuno').html(setkubunInfo(dtKbn["TEHAIKBN"], parseInt(dtCondolence[0]["TEHAIKBN"])));
            //wfCommon.initDropdown(false, mitehai, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "dead_jugyoin_zokugarakbn", "pulldown");

            $('.tehaijyotai').hide();
            $('.tehaifuno_details').show();

            //手配不能コメント
            $('.tehaifunocoment1').hide();
            $('.tehaifunocomentlab').hide();
            $('.tehaifunocoment2').hide();
            $('.space12').hide();
            $('.space34').show();

            // 履歴日時と履歴ユーザーの設定
            setHistoryInfo(dtCondolence);

            //保存ボタン非表示
            $('.hozon').hide();

            //手配状態 未手配
        } else if (dtCondolence[0]["TEHAIKBN"] === parseInt(STATE_MITEHAI)) {

            var mitehai = [];
            mitehai[0] = dtKbn["TEHAIKBN"][0];
            mitehai[1] = dtKbn["TEHAIKBN"][3];
            mitehai[2] = dtKbn["TEHAIKBN"][2];
            mitehai[3] = dtKbn["TEHAIKBN"][1];
            mitehai[4] = dtKbn["TEHAIKBN"][4];

            //⼿配状態設定
            wfCommon.initDropdown(false, mitehai, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "dead_jugyoin_zokugarakbn", "pulldown");
            $('.tehaifuno_details').hide();
            $('.tehaijyotai').show();

            //手配不能コメント
            $('.tehaifunocoment1').hide();
            $('.tehaifunocomentlab').hide();
            $('.tehaifunocoment2').hide();
            $('.space34').hide();
            $('.space12').show();

            // 履歴日時と履歴ユーザーの設定
            setHistoryInfo(dtCondolence);

            //保存ボタン非活性
            $("#btn_submit_detail").css('display', 'block');
            $("#btn_submit_detail").prop("disabled", true);

            //手配状態 確認中
        } else {

            var mitehai = [];
            mitehai[0] = dtKbn["TEHAIKBN"][3];
            mitehai[1] = dtKbn["TEHAIKBN"][2];
            mitehai[2] = dtKbn["TEHAIKBN"][1];
            mitehai[3] = dtKbn["TEHAIKBN"][4];
            mitehai[4] = dtKbn["TEHAIKBN"][5];

            //⼿配状態設定
            wfCommon.initDropdown(false, mitehai, "0", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "dead_jugyoin_zokugarakbn", "pulldown");

            $('.tehaifuno_details').hide();
            $('.tehaijyotai').show();

            //手配不能コメント
            $('.tehaifunocoment1').hide();
            $('.tehaifunocomentlab').hide();
            $('.tehaifunocoment2').hide();
            $('.space34').hide();
            $('.space12').show();
            $('.rec_ent_date_cancle').html(STRING_EMPTY);
            $('.rec_ent_user_cancle').html(STRING_EMPTY);
            $('.rec_ent_date_funo').html(STRING_EMPTY);
            $('.rec_ent_user_funo').html(STRING_EMPTY);
            $('.rec_ent_date_kannryowu').html(STRING_EMPTY);
            $('.rec_ent_user_kannryowu').html(STRING_EMPTY);

            // 履歴日時と履歴ユーザーの設定
            setHistoryInfo(dtCondolence);

            //保存ボタン
            $("#btn_submit_detail").css('display', 'block');
            $("#btn_submit_detail").prop("disabled", false);
        }

        // 申請番号
        $('#oid_details').html(optionData.applyNumber);

        //手配状態により、手配表示する内容
        $('body').on('change', '#dead_jugyoin_zokugarakbn', function () {

            //未手配
            if ($(this).val() === STATE_MITEHAI) {

                //手配不能コメントが非表示
                $('.tehaifunocoment1').hide();
                $('.tehaifunocoment2').hide();
                $('.required').hide();

                //保存ボタン非活性
                $("#btn_submit_detail").prop("disabled", true);

                //確認中
            } else if ($(this).val() === STATE_CONFIRMING) {

                //手配不能コメント12が非表示
                $('.tehaifunocoment1').hide();
                $('.tehaifunocoment2').hide();
                $('.required').hide();

                if (dtCondolence[0]["CHECK_DATETIME"] != null) {
                    //日時
                    //setdatetimeValue("#rec_ent_date_kakunintyuwu", dtCondolence[0]["CHECK_DATETIME"], DATE_FORMAT_PATTERN_2, true);
                    $('.rec_ent_date_kakunintyuwu').html(dtCondolence[0]["CHECK_DATETIME"]);

                    //ユーザー
                    //$('#rec_ent_user_kakunintyuwu').html(dtCondolence[0]["CHECK_EMP_NO"]);
                    $('.rec_ent_user_kakunintyuwu').html(shainInfo.SEI_KANJI + "&nbsp;" + shainInfo.MEI_KANJI);
                }

                //保存ボタン活性
                $("#btn_submit_detail").prop("disabled", false);

                //手配不能
            } else if ($(this).val() === STATE_TEHAIFUNO) {

                //手配不能コメント1が表示
                $('.tehaifunocoment1').show();
                $('.required').show();

                //手配不能コメント2が非表示
                $('.tehaifunocoment2').hide();
                $('#inputlength').val(STRING_EMPTY);

                //保存ボタン活性
                $("#btn_submit_detail").prop("disabled", false);

                //完了 キャンセル キャンセル（有償）
            } else if ($(this).val() === STATE_TEHAIZIMI || $(this).val() === STATE_CANCEL || $(this).val() === STATE_CANCEL_PAID) {

                //手配不能コメント1が非表示
                $('.tehaifunocoment1').hide();

                //手配不能コメント2が表示
                $('.tehaifunocoment2').hide();
                $('.required').hide();

                //保存ボタン活性
                $("#btn_submit_detail").prop("disabled", false);
            }
        });

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

        // フリガナ
        $('#furikana_details').html(dtCondolence[0]["UNFORTUNATE_FURIGANAMEI"]);

        // 氏名
        $('#simei_details').html(dtCondolence[0]["UNFORTUNATE_KANJIMEI"]);

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
        //カナ氏名（姓）
        $('#dead_kanashimei_sei_details').html(dtCondolence[0]["DEAD_KANASHIMEI_SEI"]);

        //カナ氏名（名）
        $('#dead_kanashimei_mei_details').html(dtCondolence[0]["DEAD_KANASHIMEI_MEI"]);

        //氏名（姓）
        $('#dead_shimei_sei_details').html(dtCondolence[0]["DEAD_SHIMEI_SEI"]);

        //氏名（名）
        $('#dead_shimei_mei_details').html(dtCondolence[0]["DEAD_SHIMEI_MEI"]);

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

        // 喪主情報エリア
        // 喪主区分
        $('#organizerkbn2').html(setkubunInfo(dtKbn["ORGANIZER_KBN"], parseInt(dtCondolence[0]["ORGANIZER_JUGYOIN_ZOKUGARAKBN"])));

        // 本人以外の場合
        if (dtCondolence[0]["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] !== ORGANIZER_KBN_HONNIN) {

            //カナ氏名（姓）
            $('#organizer_kanashimei_sei2').html(dtCondolence[0]["ORGANIZER_KANASHIMEI_SEI"]);

            //カナ氏名（名）
            $('#organizer_kanashimei_mei2').html(dtCondolence[0]["ORGANIZER_KANASHIMEI_MEI"]);

            //氏名（姓）
            $('#organizer_shimei_sei2').html(dtCondolence[0]["ORGANIZER_SHIMEI_SEI"]);

            //氏名（名）
            $('#organizer_shimei_mei2').html(dtCondolence[0]["ORGANIZER_SHIMEI_MEI"]);

            $('.organizerarea2').show();
        } else {
            $('.organizerarea2').hide();
        }

        // 通夜・告別式エリア

        // 通夜ありの場合
        if (String(dtCondolence[0]["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_TSUYA || String(dtCondolence[0]["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_BOTH) {

            // 通夜区分
            $('#tsuyakbn2').html("あり");

            // 通夜エリアの設定
            //フリガナ
            $('#tsuya_basyoufurigana2').html(dtCondolence[0]["TSUYA_BASYOUFURIGANA"]);

            //会場
            $('#tsuya_basyoumei2').html(dtCondolence[0]["TSUYA_BASYOUMEI"]);

            //郵便番号
            $('#tsuya_yubinbango2').html(dtCondolence[0]["TSUYA_YUBINBANGO"]);

            //通夜都道府県・市郡区
            $('#tsuya_todofuken2').html(dtCondolence[0]["TSUYA_ADDRESS1"]);

            //町村・番地
            $('#tsuya_sonmeibanti2').html(dtCondolence[0]["TSUYA_ADDRESS2"]);

            //マンション名
            $('#tsuya_mansyonmei2').html(dtCondolence[0]["TSUYA_ADDRESS3"]);

            //日付
            let tsuyadate = dtCondolence[0]["TSUYA_DATE"];
            if (tsuyadate !== null) {
                $("#tsuyadate_datetime2").html(moment(tsuyadate).format(DATE_FORMAT_MOMENT_PATTERN_1));
            }

            //開始時刻
            $('#tsuyatime_datetime2').html(dtCondolence[0]["TSUYA_TIME"]);

            //連絡電話番号
            $('#tsuya_renrakusakitel2').html(dtCondolence[0]["TSUYA_RENRAKUSAKITEL"]);

            $(".tsuyakbnari").show();
            $(".tsuyakbnnashi").hide();
        } else {

            $(".tsuyakbnari").hide();
            $(".tsuyakbnnashi").show();
        }

        // 告別式ありの場合
        if (String(dtCondolence[0]["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_KOKUBETSUSHIKI || String(dtCondolence[0]["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_BOTH) {

            // 告別式区分
            $('#kokubetsushikikbn2').html("あり");

            // 告別式エリアの設定
            //フリガナ
            $('#kokubetsushiki_basyoufurigana2').html(dtCondolence[0]["KOKUBETSUSHIKI_BASYOUFURIGANA"]);

            //会場
            $('#kokubetsushiki_basyoumei2').html(dtCondolence[0]["KOKUBETSUSHIKI_BASYOUMEI"]);

            //郵便番号
            $('#kokubetsushiki_yubinbango2').html(dtCondolence[0]["KOKUBETSUSHIKI_YUBINBANGO"]);

            //通夜都道府県・市郡区
            $('#kokubetsushiki_todofuken2').html(dtCondolence[0]["KOKUBETSUSHIKI_ADDRESS1"]);

            //町村・番地
            $('#kokubetsushiki_sonmeibanti2').html(dtCondolence[0]["KOKUBETSUSHIKI_ADDRESS2"]);

            //マンション名
            $('#kokubetsushiki_mansyonmei2').html(dtCondolence[0]["KOKUBETSUSHIKI_ADDRESS3"]);

            //日付
            let kokubetsushikidate = dtCondolence[0]["KOKUBETSUSHIKI_DATE"];
            if (kokubetsushikidate !== null) {
                $("#kokubetsushikidate_datetime2").html(moment(kokubetsushikidate).format(DATE_FORMAT_MOMENT_PATTERN_1));
            }

            //開始時刻
            $('#kokubetsushikiteime_datetime2').html(dtCondolence[0]["KOKUBETSUSHIKI_TIME"]);

            //連絡電話番号
            $('#kokubetsushiki_renrakusakitel2').html(dtCondolence[0]["KOKUBETSUSHIKI_RENRAKUSAKITEL"]);

            $(".kokubetsushikikbnari").show();
            $(".kokubetsushikikbnnashi").hide();
        } else {

            $(".kokubetsushikikbnnashi").show();
            $(".kokubetsushikikbnari").hide();
        }

        // 供花発行区分
        $('#kyokak_confirm').html(setkubunInfo(dtKbn["NECESSARY_KBN"], dtCondolence[0]["KYOKAKBN"]));

        // 弔電発行区分
        $('#tyoden_confirm').html(setkubunInfo(dtKbn["NECESSARY_KBN"], dtCondolence[0]["TYODENKBN"]));

        // 供花お届け先場所が通夜の場合
        if (dtCondolence[0]["TODOKESAKIKBN"] === parseInt(LOCATION_TSUYA_KBN)) {

            //供花届ける場所区分  
            $('#todokesakikbn_details').html('　　' + setkubunInfo(dtKbn["TODOKESAKI_KBN"], dtCondolence[0]["TODOKESAKIKBN"]));

            //共通部分の設定
            //settodokesakituyaInfo(dtCondolence);

            $('.atokazari').hide();

            // 供花お届け先場所が告別式の場合
        } else if (dtCondolence[0]["TODOKESAKIKBN"] === parseInt(LOCATION_KOKUBETSUSHIKI_KBN)) {

            //供花届ける場所区分
            $('#todokesakikbn_details').html('　　' + setkubunInfo(dtKbn["TODOKESAKI_KBN"], dtCondolence[0]["TODOKESAKIKBN"]));

            //共通部分の設定
            //settodokesakikobeshituInfo(dtCondolence);

            $('.atokazari').hide();

            // 供花お届け先場所が後飾りの場合
        } else if (dtCondolence[0]["TODOKESAKIKBN"] === parseInt(LOCATION_ATOKA_KBN)) {

            //供花届ける場所区分
            $('#todokesakikbn_details').html('　　' + setkubunInfo(dtKbn["TODOKESAKI_KBN"], dtCondolence[0]["TODOKESAKIKBN"]));

            //会場非表示
            $('.tsuya_basyoumei_details').hide();

            //フリガナ非表示
            $('.tsuya_basyoufurigana_details').hide();

            //共通部分の設定
            settodokesakiatoazariInfo(dtCondolence);

            $('.atokazari').show();
        }

        // 香料・供花・弔電区分が辞退の場合
        if (String(dtCondolence[0]["KYOKAKBN"]) === NECESSARY_KBN_JITAI && String(dtCondolence[0]["TYODENKBN"]) === NECESSARY_KBN_JITAI) {
            $('.koryo_kyokak_tyoden_area2').hide();
        } else {
            $('.koryo_kyokak_tyoden_area2').show();
        }

        // 情報が１件未満の場合
        if (dtCondolence[0]["KUGE_GOUKEI"] === 0 && dtCondolence[0]["TYOUDEN_GOUKEI"] === 0) {
            $("#label-standard-info0").parent().hide();
            $("#label-standard-info1").parent().hide();
            $("#label-standard-info2").parent().hide();
            $("#label-standard-info3").parent().show();
        } else {
            //出向元会社より供花の数、出向先会社より供花の数がある
            if (dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI1"] === null || dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI1"] === STRING_EMPTY) {

                $("#label-standard-info0").parent().hide();
            } else {
                $("#label-standard-info0").parent().show();
                $("#label-standard-kaishaname0").text(dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI1"]);
                $("#label-standard-yakushoku0").text(dtCondolence[0]["SASHIDASHI_MOTO_KAISYA2"]);
                $("#label-standard-shimei0").text(dtCondolence[0]["SASHIDASHI_MOTO_KAISYA3"]);
                
                if (dtCondolence[0]["KUGE_MOTO_KAISYA"] > 0) {
                    $('#label-standard-kyokanum0').html(dtCondolence[0]["KUGE_MOTO_KAISYA"] + '基');
                } else {
                    $('#label-standard-kyokanum0').html("0" + '基');
                }

                if (dtCondolence[0]["TYOUDEN_MOTO_KAISYA"] > 0) {
                    $('#label-standard-tyodennum0').html(dtCondolence[0]["TYOUDEN_MOTO_KAISYA"] + '通');
                } else {
                    $('#label-standard-tyodennum0').html("0" + '通');
                }
            }

            //出向元労働組合より供花の数、出向元労働組合より弔電有無がある
            if (dtCondolence[0]["SASHIDASHI_MOTO_KAISYA1"] === null || dtCondolence[0]["SASHIDASHI_MOTO_KAISYA1"] === STRING_EMPTY) {

                $("#label-standard-info1").parent().hide();
            } else {
                $("#label-standard-info1").parent().show();
                $("#label-standard-kaishaname1").text(dtCondolence[0]["SASHIDASHI_MOTO_KAISYA1"]);
                $("#label-standard-yakushoku1").text(dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI2"]);
                $("#label-standard-shimei1").text(dtCondolence[0]["SASHIDASHI_MOTO_KUMIAI3"]);

                if (dtCondolence[0]["KUGE_MOTO_KUMIAI"] > 0) {
                    $('#label-standard-kyokanum1').html(dtCondolence[0]["KUGE_MOTO_KUMIAI"] + '基');
                } else {
                    $('#label-standard-kyokanum1').html("0" + '基');
                }

                if (dtCondolence[0]["TYOUDEN_MOTO_KUMIAI"] > 0) {
                    $('#label-standard-tyodennum1').html(dtCondolence[0]["TYOUDEN_MOTO_KUMIAI"] + '通');
                } else {
                    $('#label-standard-tyodennum1').html("0" + '通');
                }
            }

            //出向先会社より供花の数、出向先会社より弔電有無がある
            if (dtCondolence[0]["SASHIDASHI_SAKI_KAISYA1"] === null || dtCondolence[0]["SASHIDASHI_SAKI_KAISYA1"] === STRING_EMPTY) {

                $("#label-standard-info2").parent().hide();
            } else {
                $("#label-standard-info2").parent().show();
                $("#label-standard-kaishaname2").text(dtCondolence[0]["SASHIDASHI_SAKI_KAISYA1"]);
                $("#label-standard-yakushoku2").text(dtCondolence[0]["SASHIDASHI_SAKI_KAISYA2"]);
                $("#label-standard-shimei2").text(dtCondolence[0]["SASHIDASHI_SAKI_KAISYA3"]);

                if (dtCondolence[0]["KUGE_SAKI_KAISYA"] > 0) {
                    $('#label-standard-kyokanum2').html(dtCondolence[0]["KUGE_SAKI_KAISYA"] + '基');
                } else {
                    $('#label-standard-kyokanum2').html("0" + '基');
                }

                if (dtCondolence[0]["TYOUDEN_SAKI_KAISYA"] > 0) {
                    $('#label-standard-tyodennum2').html(dtCondolence[0]["TYOUDEN_SAKI_KAISYA"] + '通');
                } else {
                    $('#label-standard-tyodennum2').html("0" + '通');
                }
            }

            $("#label-standard-info3").parent().hide();
        }
            
    }

    /**
 * 入力チェックを設定する
 */
    function setInputCheck() {

        // validateの設定
        $("#arrange_trader_list_form").validate({
            focusCleanup: true,
            onkeyup: false,
            ignore: "",
            rules: {
                inputlength: { required: true }
            }
        });
    }

    /*
     * 
     * 通夜・告別式エリア設定用メソッド（入力画面からの場合）_後飾り
     * 
     * @param target 設定目標IDの前半部
     * @param idPrefix 取得元IDの前半部
     * 
     */
    function settodokesakiatoazariInfo(dtCondolence) {

        //郵便番号
        $('#tsuya_yubinbango_details').html('〒' + dtCondolence[0]["ATOKAZARI_YUBINBANGO"]);

        //都道府県・市郡区
        $('#tsuya_todofuken_details').html(dtCondolence[0]["ATOKAZARI_ADDRESS1"]);

        //町村・番地
        $('#tsuya_sonmeibanti_details').html(dtCondolence[0]["ATOKAZARI_ADDRESS2"]);

        //マンション名
        $('#tsuya_mansyonmei_details').html(dtCondolence[0]["ATOKAZARI_ADDRESS3"]);

        //日時
        //setdatetimeValue("#tsuya_datetime_details", dtCondolence[0]["ATOKAZARI_DATETIME"], DATE_FORMAT_PATTERN_1, true);
        //日付
        let atokazaridate = dtCondolence[0]["ATOKAZARI_DATETIME"];
        if (atokazaridate !== null) {
            $("#tsuya_datetime_details").html(moment(atokazaridate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }

        //連絡先
        $('#rennrakusaki_details').html(dtCondolence[0]["ATOKAZARI_RENRAKUSAKITEL"]);
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
     * 手配状態更新処理_OKボタンの処理
     */
    function updateTehaiState(dtCondolence, optionData) {

        // ダイアログ画面を閉じること
        $("#app-dialog-div").removeClass("o-modal--opened");

        var ret = changeTehaiKbn(dtCondolence, optionData);
        if (ret) {

            //メール送付を行う：「申請者」、「イオンGLC」へ「手配済み時」のメール送付を行う
            //取得DB関連：【XXXマスタ】申請者情報、【XXXマスタ】イオンGLS情報、【XXXマスタ】手配済みメールテンプレート
            wfCommon.DoMailSend(MAIL_TYPE_CONDOLENCE, MAIL_STSTE_APPROVAL, optionData.oid, MAIL_TO_TEHAIGIYOSIYA);

            // 更新成功のダイアログを表示すること
            wfCommon.ShowDialog(
                DIALOG_ALERT,
                STRING_EMPTY,
                wfCommon.MsgFormat(msg["I0003"], optionData.applyNumber),
                "はい",
                detailClose
            ); 

            $("#detail-screen").removeClass("o-modal--opened");
            //window.location.href = "form_arrange_trader_list.html";

            // 一覧画面に戻る
            optionData.onOkClick($('#dead_jugyoin_zokugarakbn').val());
        }
    }

    /**
     * 手配状態更新処理
     */
    function createoneventSyosai(dtCondolence, optionData) {

        // 確認ボタンをクリックイベント
        $("#btn_submit_detail").on("click", function () {

            if ($('#dead_jugyoin_zokugarakbn').val() === STATE_TEHAIFUNO) {
                // チェックを実施する
                var flg = $("#arrange_trader_list_form").valid();
                if (!flg) {
                    return;
                }
            }

            wfCommon.ShowDialog(
                DIALOG_CONFIRM,
                STRING_EMPTY,
                wfCommon.MsgFormat(msg["W0003"], optionData.applyNumber),
                "はい",
                updateTehaiState,
                new Array(dtCondolence, optionData),
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
 　　* 詳細画面を閉じる
　　 */ 
    function detailClose() {
      
        $("#app-dialog-div").removeClass("o-modal--opened");      
    }

    /**
 　　* 更新処理チェック
　　 */ 
    function changeTehaiKbn(dtCondolence, optionData) {

        var ret = true;

        var handler = new HttpHandler("BP.WF.HttpHandler.Mn_ArrangeTraderList");
        handler.AddUrlData();
        //伝票番号を設定
        handler.AddPara("strOid", optionData.oid);
        //フローIDを設定
        // handler.AddPara("FK_Flow", WF_ID_CONDOLENCE);
        //手配状態を設定
        handler.AddPara("tehaiKbn", $('#dead_jugyoin_zokugarakbn').val());
        //手配履歴(ユーザ)を設定
        handler.AddPara("tehaisyaNo", webUser.No);
        //手配不能コメントを設定
        handler.AddPara("tehaiFunouComment", $("#inputlength").val());
        //最初手配状態
        handler.AddPara("firsttehaiKbn", dtCondolence[0]["TEHAIKBN"]);
        var data = handler.DoMethodReturnString("Tehai_States_Update");

        if (data === TEHAI_STATE) {
            //更新処理ができない旨のダイアログを表示
            wfCommon.ShowDialog(
                DIALOG_ALERT,
                STRING_EMPTY,
                wfCommon.MsgFormat(msg["E0001"], optionData.applyNumber),
                "はい"
            );
            ret = false;
        } else {
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
        }
        return ret;
    }

    function HtmlRedirectToItiran() {
        // 依頼一覧に戻ること
        window.location.href = 'form_arrange_trader_list.html';
    }

    function HtmlRedirectTosyosai() {
        // ⼿配業者依頼詳細画⾯に戻ること
        $(":mobile-pagecontainer").pagecontainer("change", "#pageTwo");
    }

    /**
     * 手配一覧画面から渡すOIDを取得
     */
    function GetRequest() {
        var url = location.search;
        var theRequest = new Object();
        if (url.indexOf("?") !== -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
            }
        }
        return theRequest;
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

    /*
     * 社員情報取得
     */
    function getShainInfo(shainbango) {

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_AppForm");
        handler.AddUrlData();
        handler.AddPara("shainbango", shainbango);
        handler.AddPara("tblName", "TT_WF_CONDOLENCE");
        var data = handler.DoMethodReturnString("Get_Shain_Info");
        // 異常処理
        if (data.indexOf('msg@') === 0) {
            data = JSON.parse(data.split("@")[1]);
            var key = data["key"];
            var value = data["value"];
            wfCommon.Msgbox(wfCommon.MsgFormat(msg[key], value));
            return;
        }
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return;
        }
        data = JSON.parse(data);
        return data["Get_Shain_Info"][0];
    }

    /*
     * 手配状態履歴の設定
     */
    function setHistoryInfo(dtCondolence) {

        //日時
        if (dtCondolence[0]["CHECK_DATETIME"] === null) {
            $('.rec_ent_date_kakunintyuwu').html(STRING_EMPTY);

            //ユーザー
            $('.rec_ent_user_kakunintyuwu').html(STRING_EMPTY);
        } else {
            //日時
            //setdatetimeValue("#rec_ent_date_kakunintyuwu", dtCondolence[0]["CHECK_DATETIME"], "YYYY-MM-DD HH: mm: ss", true);
            $('.rec_ent_date_kakunintyuwu').html(moment(dtCondolence[0]["CHECK_DATETIME"]).format(DATETIME_FORMAT_MOMENT_PATTERN_2));

            //ユーザー
            //$('#rec_ent_user_kakunintyuwu').html(dtCondolence[0]["CHECK_EMP_NO"]);
            $('.rec_ent_user_kakunintyuwu').html(dtCondolence[0]["CHECK_EMP_NO_SEIMEI_KANJI"]);
        }

        if (dtCondolence[0]["TEHAIFUNOU_DATETIME"] === null) {
            //日時
            $('.rec_ent_date_funo').html(STRING_EMPTY);

            //ユーザー
            $('.rec_ent_user_funo').html(STRING_EMPTY);
        } else {
            //日時
            //setdatetimeValue("#rec_ent_date_funo", dtCondolence[0]["TEHAIFUNOU_DATETIME"], DATE_FORMAT_PATTERN_2, true);
            $('.rec_ent_date_funo').html(moment(dtCondolence[0]["TEHAIFUNOU_DATETIME"]).format(DATETIME_FORMAT_MOMENT_PATTERN_2));

            //ユーザー
            //$('#rec_ent_user_funo').html(dtCondolence[0]["TEHAISYA_NO"]);
            $('.rec_ent_user_funo').html(dtCondolence[0]["TEHAISYA_NO_SEIMEI_KANJI"]);
        }

        if (dtCondolence[0]["CMP_EMP_NO"] === null) {
            //日時
            $('.rec_ent_date_kannryowu').html(STRING_EMPTY);

            //ユーザー
            $('.rec_ent_user_kannryowu').html(STRING_EMPTY);
        } else {
            //日時
            //setdatetimeValue("#rec_ent_date_kannryowu", dtCondolence[0]["COMP_DATETIME"], DATE_FORMAT_PATTERN_2, true);
            $('.rec_ent_date_kannryowu').html(moment(dtCondolence[0]["COMP_DATETIME"]).format(DATETIME_FORMAT_MOMENT_PATTERN_2));

            //ユーザー
            //$('#rec_ent_user_kannryowu').html(dtCondolence[0]["CMP_EMP_NO"]);
            $('.rec_ent_user_kannryowu').html(dtCondolence[0]["CMP_EMP_NO_SEIMEI_KANJI"]);
        }

        if (dtCondolence[0]["CANCEL_DATETIME"] === null) {
            //日時
            $('.rec_ent_date_cancle').html(STRING_EMPTY);

            //ユーザー
            $('.rec_ent_user_cancle').html(STRING_EMPTY);
        } else {
            //日時
            //setdatetimeValue("#rec_ent_date_cancle", dtCondolence[0]["CANCEL_DATETIME"], DATE_FORMAT_PATTERN_2, true);
            $('.rec_ent_date_cancle').html(moment(dtCondolence[0]["CANCEL_DATETIME"]).format(DATETIME_FORMAT_MOMENT_PATTERN_2));

            //ユーザー
            //$('#rec_ent_user_cancle').html(dtCondolence[0]["CANCEL_EMP_NO"]);
            $('.rec_ent_user_cancle').html(dtCondolence[0]["CANCEL_EMP_NO_SEIMEI_KANJI"]);
        }
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

            // イベント定義
            createoneventSyosai(JSON.parse(data), optionData);
        });
    }
})(jQuery);
/*▲関数定義エリア▲*/