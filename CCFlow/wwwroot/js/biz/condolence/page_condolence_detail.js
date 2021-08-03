/*▼グローバル変数定義エリア▼*/
var dtCondolence;                                    //弔事連絡情報格納オブジェクト

//…追加変数

/*▲グローバル関数定義エリア▲*/

(function ($) {

    $.fn.extend({

        "InitPageForDetail": function (optionData) {

            // 画面初期化
            InitPageSyosai(optionData);

            // 承認状況を呼び出す
            $().InitPageApprovalRoot(optionData.oid);
        }
    });

    /**
 * 画面初期化
 **/
    function InitPageSyosai(optionData) {
        var objGetTbl = {};
        objGetTbl[0] = 'KBN';

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

        //イベント定義
        createoneventSyosai();

        //弔辞連絡情報
        //dtCondolence = GetCondolenceInfo(optionData.oid);
        GetCondolenceInfo(optionData.oid);

        // 確認画面初期化メソッドの呼び出し
        //initPageTwo();

        // 確認画面表示の制御
        //setPageTwoDisplay();
    }

    /**
     * 香料手配区分状態更新処理
     */
    function createoneventSyosai() {

        // 詳細戻るボタンをクリックイベント
        $("#btn_back").on("click", function () {
            
            // ダイアログエリアを閉じる 
            $("#detail-screen").removeClass("o-modal--opened");
        });
    } 

    /*
     *
     * 確認画面初期化用メソッド（入力画面からの場合）
     *
     */
    function initPageTwo(dtCondolence) {

        // 代理申請者の情報エリア
        // 代理申請のみを表示する
        if (dtCondolence["SHINSEISYAKBN"] === parseInt(SHINSEISYA_KBN_DAIRI)) {

            //代理申請者の情報エリア表示
            $("#dairilistview").css('display', 'block');

            // 代理社員番号
            $('#dairi_syainbango_details').html(dtCondolence["DAIRISHINSNEISYA_SHAINBANGO"]);

            // 代理社員名
            $('#dairi_syainname_details').html(dtCondolence["DAIRISHINSNEISYA_MEI"]);

            // 代理所属
            $('#dairi_syozoku_details').html(dtCondolence["DAIRISHINSNEISYA_SYOZOKU"]);

            // 連絡先電話番号
            $('#dairi_tel_details').html(dtCondolence["RENRAKUSAKITEL"]);

            // 連絡先メール
            $('#dairi_mail_details').html(dtCondolence["RENRAKUSAKIMAIL"]);

        }
        else {
            //代理申請者の情報エリア非表示
            $("#dairilistview").css('display', 'none');
        }

        // ご不幸にあわれた従業員の情報エリア
        // 社員番号
        $('#employee_shainbango2').html(dtCondolence["UNFORTUNATE_SHAINBANGO"]);

        // フリガナ
        $('#employee_furigana2').html(dtCondolence["UNFORTUNATE_FURIGANAMEI"]);

        // 氏名
        $('#employee_simei2').html(dtCondolence["UNFORTUNATE_KANJIMEI"]);

        // 会社名称
        $('#employee_shaseki2').html(dtCondolence["UNFORTUNATE_KAISYAMEI"]);

        // 正式組織名・上
        $('#employee_shozoku2').html(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKIUE"]);

        // 下
        $('#employee_bushocode2').html(dtCondolence["UNFORTUNATE_SEISHIKISOSHIKISHITA"]);

        // 社員区分
        $('#employee_shainkbn2').html(dtCondolence["UNFORTUNATE_SYAINNKBN"]);

        // 職位
        $('#employee_shokui2').html(dtCondolence["UNFORTUNATE_SYOKUIKBN"]);

        // 組合区分
        $('#employee_kumiaikbn2').html(dtCondolence["UNFORTUNATE_KUMIAIKBN"]);

        // グッドライフ区分
        $('#employee_glckbn2').html(dtCondolence["UNFORTUNATE_GLCKBN"]);

        // 本人申請のみを表示する
        if (dtCondolence["SHINSEISYAKBN"] === parseInt(SHINSEISYA_KBN_HONNIN)) {

            // 連絡先電話番号、連絡先メール表示
            //$("#telandmail").css('display', 'block');
            $('.telandmail').css('display', 'block');

            // 連絡先電話番号
            $('#renrakusakitel2').html(dtCondolence["RENRAKUSAKITEL"]);

            // 連絡先メール
            $('#renrakusakimail2').html(dtCondolence["RENRAKUSAKIMAIL"]);

        }
        else {
            // 連絡先電話番号、連絡先メール非表示
            //$("#telandmail").css('display', 'none');
            $('.telandmail').css('display', 'none');
        }

        // 亡くなられた方情報エリア
        //カナ氏名（姓）
        $('#dead_kanashimei_sei2').html(dtCondolence["DEAD_KANASHIMEI_SEI"]);

        //カナ氏名（名）
        $('#dead_kanashimei_mei2').html(dtCondolence["DEAD_KANASHIMEI_MEI"]);

        //氏名（姓）
        $('#dead_shimei_sei2').html(dtCondolence["DEAD_SHIMEI_SEI"]);

        //氏名（名）
        $('#dead_shimei_mei2').html(dtCondolence["DEAD_SHIMEI_MEI"]);

        //従業員との続柄
        $('#dead_jugyoin_zokugarakbn2').html(setkubunInfo(dtKbn["DEAD_KBN"], dtCondolence["DEAD_JUGYOIN_ZOKUGARAKBN"]));

        //性別
        $('#dead_seibetsu2').html(setkubunInfo(dtKbn["SEIBETSU_KBN"], dtCondolence["DEAD_SEIBETSU"]));

        //同居 / 別居
        $('#dead_dokyo_bekyo2').html(setkubunInfo(dtKbn["DOKYO_BEKYO_KBN"], dtCondolence["DEAD_DOKYO_BEKYO"]));

        //年齢
        if (dtCondolence["DEAD_NENREI"] === null) {
            $('#dead_nenrei2').html(STRING_EMPTY);
        } else {
            $('#dead_nenrei2').html(dtCondolence["DEAD_NENREI"] + '歳');
        }

        //逝去日付
        let deathdate = dtCondolence["DEAD_DATE"];
        if (deathdate !== null) {
            $("#deadbi_seikyobi2").html(moment(deathdate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }

        //逝去時刻
        $('#deadtime_seikyobi2').html(deathTimeList["content"].find(obj => obj.value === dtCondolence["DEAD_TIME"]).name);

        // 喪主情報エリア
        // 喪主区分
        $('#organizerkbn2').html(setkubunInfo(dtKbn["ORGANIZER_KBN"], parseInt(dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"])));

        // 本人以外の場合
        if (dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] === ORGANIZER_KBN_HONNIN_IGAI){

            //カナ氏名（姓）
            $('#organizer_kanashimei_sei2').html(dtCondolence["ORGANIZER_KANASHIMEI_SEI"]);

            //カナ氏名（名）
            $('#organizer_kanashimei_mei2').html(dtCondolence["ORGANIZER_KANASHIMEI_MEI"]);

            //氏名（姓）
            $('#organizer_shimei_sei2').html(dtCondolence["ORGANIZER_SHIMEI_SEI"]);

            //氏名（名）
            $('#organizer_shimei_mei2').html(dtCondolence["ORGANIZER_SHIMEI_MEI"]);
        }

        // 通夜・告別式エリア
        
        // 通夜ありの場合
        if (String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_TSUYA || String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_BOTH) {

            // 通夜区分
            $('#tsuyakbn2').html("あり");

            // 通夜エリアの設定
            //フリガナ
            $('#tsuya_basyoufurigana2').html(dtCondolence["TSUYA_BASYOUFURIGANA"]);

            //会場
            $('#tsuya_basyoumei2').html(dtCondolence["TSUYA_BASYOUMEI"]);

            //郵便番号
            $('#tsuya_yubinbango2').html(dtCondolence["TSUYA_YUBINBANGO"]);

            //通夜都道府県・市郡区
            $('#tsuya_todofuken2').html(dtCondolence["TSUYA_ADDRESS1"]);

            //町村・番地
            $('#tsuya_sonmeibanti2').html(dtCondolence["TSUYA_ADDRESS2"]);

            //マンション名
            $('#tsuya_mansyonmei2').html(dtCondolence["TSUYA_ADDRESS3"]);

            //日付
            let tsuyadate = dtCondolence["TSUYA_DATE"];
            if (tsuyadate !== null) {
                $("#tsuyadate_datetime2").html(moment(tsuyadate).format(DATE_FORMAT_MOMENT_PATTERN_1));
            }

            //開始時刻
            $('#tsuyatime_datetime2').html(dtCondolence["TSUYA_TIME"]);

            //連絡電話番号
            $('#tsuya_renrakusakitel2').html(dtCondolence["TSUYA_RENRAKUSAKITEL"]);

            $(".tsuyakbnari").show();
            $(".tsuyakbnnashi").hide();
        }　else {

            $(".tsuyakbnari").hide();
            $(".tsuyakbnnashi").show();
        }
        
        // 告別式ありの場合
        if (String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_KOKUBETSUSHIKI || String(dtCondolence["TSUYA_KOKUBETSUSHIKIKBN"]) === FUNERAL_KBN_BOTH) {

            // 告別式区分
            $('#kokubetsushikikbn2').html("あり");

            // 告別式エリアの設定
            //フリガナ
            $('#kokubetsushiki_basyoufurigana2').html(dtCondolence["KOKUBETSUSHIKI_BASYOUFURIGANA"]);

            //会場
            $('#kokubetsushiki_basyoumei2').html(dtCondolence["KOKUBETSUSHIKI_BASYOUMEI"]);

            //郵便番号
            $('#kokubetsushiki_yubinbango2').html(dtCondolence["KOKUBETSUSHIKI_YUBINBANGO"]);

            //通夜都道府県・市郡区
            $('#kokubetsushiki_todofuken2').html(dtCondolence["KOKUBETSUSHIKI_ADDRESS1"]);

            //町村・番地
            $('#kokubetsushiki_sonmeibanti2').html(dtCondolence["KOKUBETSUSHIKI_ADDRESS2"]);

            //マンション名
            $('#kokubetsushiki_mansyonmei2').html(dtCondolence["KOKUBETSUSHIKI_ADDRESS3"]);

            //日付
            let kokubetsushikidate = dtCondolence["KOKUBETSUSHIKI_DATE"];
            if (kokubetsushikidate !== null) {
                $("#kokubetsushikidate_datetime2").html(moment(kokubetsushikidate).format(DATE_FORMAT_MOMENT_PATTERN_1));
            }

            //開始時刻
            $('#kokubetsushikiteime_datetime2').html(dtCondolence["KOKUBETSUSHIKI_TIME"]);

            //連絡電話番号
            $('#kokubetsushiki_renrakusakitel2').html(dtCondolence["KOKUBETSUSHIKI_RENRAKUSAKITEL"]);

            $(".kokubetsushikikbnari").show();
            $(".kokubetsushikikbnnashi").hide();
        }　else {

            $(".kokubetsushikikbnnashi").show();
            $(".kokubetsushikikbnari").hide();
        }

        // 香料発行区分
        $('#kouryou_confirm').html(setkubunInfo(dtKbn["NECESSARY_KBN"], dtCondolence["KORYOKBN"]));

        // 供花発行区分
        $('#kyokak_confirm').html(setkubunInfo(dtKbn["NECESSARY_KBN"], dtCondolence["KYOKAKBN"]));

        // 弔電発行区分
        $('#tyoden_confirm').html(setkubunInfo(dtKbn["NECESSARY_KBN"], dtCondolence["TYODENKBN"]));

        // 情報が１件未満の場合
        if (dtCondolence["KOURYOU_GOUKEI"] === 0 && dtCondolence["KUGE_GOUKEI"] === 0 && dtCondolence["TYOUDEN_GOUKEI"] === 0) {
            $("#label-standard-info0").parent().hide();
            $("#label-standard-info1").parent().hide();
            $("#label-standard-info2").parent().hide();
            $("#label-standard-info3").parent().show();
        } else {  // 以外の場合

            if (dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] === null || dtCondolence["SASHIDASHI_MOTO_KUMIAI1"] === STRING_EMPTY) {
                $("#label-standard-info0").parent().hide();
            } else {
                $("#label-standard-info0").parent().show();
                $("#label-standard-kaishaname0").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI1"]);
                $("#label-standard-yakushoku0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA2"]);
                $("#label-standard-shimei0").text(dtCondolence["SASHIDASHI_MOTO_KAISYA3"]);
                if (dtCondolence["KOURYOU_MOTO_KAISYA"] > 0) {
                    $("#label-standard-koryonum0").text(String(dtCondolence["KOURYOU_MOTO_KAISYA"]).replace(/\B(?=(?:\d{3})+\b)/g, ",") + '円');
                } else {
                    $("#label-standard-koryonum0").text("0" + '円');
                }
                if (dtCondolence["KUGE_MOTO_KAISYA"] > 0) {
                    $("#label-standard-kyokanum0").text(dtCondolence["KUGE_MOTO_KAISYA"] + '基');
                } else {
                    $("#label-standard-kyokanum0").text("0" + '基');
                }
                if (dtCondolence["TYOUDEN_MOTO_KAISYA"] > 0) {
                    $("#label-standard-tyodennum0").text(dtCondolence["TYOUDEN_MOTO_KAISYA"] + '通');
                } else {
                    $("#label-standard-tyodennum0").text("0" + '通');
                }
            }

            if (dtCondolence["SASHIDASHI_MOTO_KAISYA1"] === null || dtCondolence["SASHIDASHI_MOTO_KAISYA1"] === STRING_EMPTY) {
                $("#label-standard-info1").parent().hide();
            } else {
                $("#label-standard-info1").parent().show();
                $("#label-standard-kaishaname1").text(dtCondolence["SASHIDASHI_MOTO_KAISYA1"]);
                $("#label-standard-yakushoku1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI2"]);
                $("#label-standard-shimei1").text(dtCondolence["SASHIDASHI_MOTO_KUMIAI3"]);
                if (dtCondolence["KOURYOU_MOTO_KUMIAI"] > 0) {
                    $("#label-standard-koryonum1").text(String(dtCondolence["KOURYOU_MOTO_KUMIAI"]).replace(/\B(?=(?:\d{3})+\b)/g, ",") + '円');
                } else {
                    $("#label-standard-koryonum1").text("0" + '円');
                }
                if (dtCondolence["KUGE_MOTO_KUMIAI"] > 0) {
                    $("#label-standard-kyokanum1").text(dtCondolence["KUGE_MOTO_KUMIAI"] + '基');
                } else {
                    $("#label-standard-kyokanum1").text("0" + '基');
                }
                if (dtCondolence["TYOUDEN_MOTO_KUMIAI"] > 0) {
                    $("#label-standard-tyodennum1").text(dtCondolence["TYOUDEN_MOTO_KUMIAI"] + '通');
                } else {
                    $("#label-standard-tyodennum1").text("0" + '通');
                } 
            }

            if (dtCondolence["SASHIDASHI_SAKI_KAISYA1"] === null || dtCondolence["SASHIDASHI_SAKI_KAISYA1"] === STRING_EMPTY) {
                $("#label-standard-info2").parent().hide();
            } else {
                $("#label-standard-info2").parent().show();
                $("#label-standard-kaishaname2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA1"]);
                $("#label-standard-yakushoku2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA2"]);
                $("#label-standard-shimei2").text(dtCondolence["SASHIDASHI_SAKI_KAISYA3"]);
                if (dtCondolence["KOURYOU_SAKI_KAISYA"] > 0) {
                    $("#label-standard-koryonum2").text(String(dtCondolence["KOURYOU_SAKI_KAISYA"]).replace(/\B(?=(?:\d{3})+\b)/g, ",") + '円');
                } else {
                    $("#label-standard-koryonum2").text("0" + '円');
                }
                if (dtCondolence["KUGE_SAKI_KAISYA"] > 0) {
                    $("#label-standard-kyokanum2").text(dtCondolence["KUGE_SAKI_KAISYA"] + '基');
                } else {
                    $("#label-standard-kyokanum2").text("0" + '基');
                }
                if (dtCondolence["TYOUDEN_SAKI_KAISYA"] > 0) {
                    $("#label-standard-tyodennum2").text(dtCondolence["TYOUDEN_SAKI_KAISYA"] + '通');
                } else {
                    $("#label-standard-tyodennum2").text("0" + '通');
                }               
            }

            $("#label-standard-info3").parent().hide();
        }

        // 香料・供花・弔電区分が辞退の場合
        if (String(dtCondolence["KYOKAKBN"]) === NECESSARY_KBN_JITAI && String(dtCondolence["TYODENKBN"]) === NECESSARY_KBN_JITAI) {
            $('.koryo_kyokak_tyoden_area2').hide();
        } else {
            $('.koryo_kyokak_tyoden_area2').show();
        }

        // 供花お届け先場所が通夜の場合
        if (dtCondolence["TODOKESAKIKBN"] === parseInt(LOCATION_TSUYA_KBN)) {

            //供花届ける場所区分
            $('#todokesakikbn_details').html('　　' + setkubunInfo(dtKbn["TODOKESAKI_KBN"], dtCondolence["TODOKESAKIKBN"]));

            //共通部分の設定
            //settodokesakituyaInfo(dtCondolence);

            $('.atokazari').hide();          

            // 供花お届け先場所が告別式の場合
        } else if (dtCondolence["TODOKESAKIKBN"] === parseInt(LOCATION_KOKUBETSUSHIKI_KBN)) {

            //供花届ける場所区分
            $('#todokesakikbn_details').html('　　' + setkubunInfo(dtKbn["TODOKESAKI_KBN"], dtCondolence["TODOKESAKIKBN"]));

            //共通部分の設定
            //settodokesakikobeshituInfo(dtCondolence);

            $('.atokazari').hide();

            // 供花お届け先場所が後飾りの場合
        } else if (dtCondolence["TODOKESAKIKBN"] === parseInt(LOCATION_ATOKA_KBN)) {

            //供花届ける場所区分
            $('#todokesakikbn_details').html('　　' + setkubunInfo(dtKbn["TODOKESAKI_KBN"], dtCondolence["TODOKESAKIKBN"]));

            //会場非表示
            $('.tsuya_basyoumei_details').hide();

            //フリガナ非表示
            $('.tsuya_basyoufurigana_details').hide();

            //共通部分の設定
            settodokesakiatoazariInfo(dtCondolence);

            $('.atokazari').show();
        }
    }

    /*
     * 
     * 通夜・告別式エリア設定用メソッド（入力画面からの場合）_通夜
     * 
     * @param target 設定目標IDの前半部
     * @param idPrefix 取得元IDの前半部
     * 
     */
    function settodokesakituyaInfo(dtCondolence) {

        //郵便番号
        $('#tsuya_yubinbango_details').html('〒' + dtCondolence["TSUYA_YUBINBANGO"]);

        //都道府県・市郡区
        $('#tsuya_todofuken_details').html(dtCondolence["TSUYA_ADDRESS1"]);

        //町村・番地
        $('#tsuya_sonmeibanti_details').html(dtCondolence["TSUYA_ADDRESS2"]);

        //マンション名
        $('#tsuya_mansyonmei_details').html(dtCondolence["TSUYA_ADDRESS3"]);

        //日時
        let tsuyadate = dtCondolence["TSUYA_DATETIME"];
        if (tsuyadate !== null) {
            $("#tsuya_datetime_details").html(moment(tsuyadate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }

        //連絡先
        $('#rennrakusaki_details').html(dtCondolence["TSUYA_RENRAKUSAKITEL"]);
    }

    /*
     * 
     * 通夜・告別式エリア設定用メソッド（入力画面からの場合）_告別式
     * 
     * @param target 設定目標IDの前半部
     * @param idPrefix 取得元IDの前半部
     * 
     */
    function settodokesakikobeshituInfo(dtCondolence) {

        //郵便番号
        $('#tsuya_yubinbango_details').html(dtCondolence["KOKUBETSUSHIKI_YUBINBANGO"]);

        //都道府県・市郡区
        $('#tsuya_todofuken_details').html(dtCondolence["KOKUBETSUSHIKI_ADDRESS1"]);

        //市郡区
        $('#tsuya_shigunku_details').html(dtCondolence["KOKUBETSUSHIKI_ADDRESS2"]);

        //町村・番地
        $('#tsuya_sonmeibanti_details').html(dtCondolence["KOKUBETSUSHIKI_ADDRESS3"]);

        //マンション名
        $('#tsuya_mansyonmei_details').html(dtCondolence["KOKUBETSUSHIKI_MANSYONMEI"]);

        //日時
        let kokubetsushikidate = dtCondolence["KOKUBETSUSHIKI_DATETIME"];
        if (kokubetsushikidate !== null) {
            $("#tsuya_datetime_details").html(moment(kokubetsushikidate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }

        //連絡先
        $('#rennrakusaki_details').html(dtCondolence["KOKUBETSUSHIKI_RENRAKUSAKITEL"]);
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
        $('#tsuya_yubinbango_details').html('〒' + dtCondolence["ATOKAZARI_YUBINBANGO"]);

        //都道府県・市郡区
        $('#tsuya_todofuken_details').html(dtCondolence["ATOKAZARI_ADDRESS1"]);

        //町村・番地
        $('#tsuya_sonmeibanti_details').html(dtCondolence["ATOKAZARI_ADDRESS2"]);

        //マンション名
        $('#tsuya_mansyonmei_details').html(dtCondolence["ATOKAZARI_ADDRESS3"]);

        //日時
        let atokazaridate = dtCondolence["ATOKAZARI_DATE"];
        if (atokazaridate !== null) {
            $("#tsuya_datetime_details").html(moment(atokazaridate).format(DATE_FORMAT_MOMENT_PATTERN_1));
        }

        //連絡先
        $('#rennrakusaki_details').html(dtCondolence["ATOKAZARI_RENRAKUSAKITEL"]);
    }

    /*
     *
     * 確認画面表示制御設定用メソッド（入力画面からの場合）
     *
     */
    function setPageTwoDisplay(dtCondolence) {

        // 本人の場合
        if (dtCondolence["SHINSEISYAKBN"] === parseInt(SHINSEISYA_KBN_HONNIN)) {

            // 代理申請者情報エリア表示
            $('#dairiInfo2').hide();

            // ご不幸にあわれた従業員の情報エリアのタイトル
            $('#sinseiLi').html('社員情報');

            // 連絡先、メールの制御
            $('.telmail2').show();
        } else {

            // 代理申請者情報エリア表示
            $('#dairiInfo2').show();

            // ご不幸にあわれた従業員の情報エリアのタイトル
            $('#sinseiLi').html('ご不幸にあわれた方の社員情報');

            // 連絡先、メールの制御
            $('.telmail2').hide();
        }

        // 喪主エリア
        // 本人の場合
        if (dtCondolence["ORGANIZER_JUGYOIN_ZOKUGARAKBN"] === ORGANIZER_KBN_HONNIN) {
            $('.organizerarea2').hide();
        } else {
            $('.organizerarea2').show();
        }
    }

    /**
        * 弔辞連絡情報を取得
        */
    function GetCondolenceInfo(OID) {
        
        var handler = new HttpHandler("BP.WF.HttpHandler.Mn_ArrangeTraderList");
        handler.AddUrlData();
        handler.AddPara("strOid", OID);
        handler.DoMethodSetString("GetCondolenceInfo", function (data) {

            if (data.indexOf('err@') === 0) {
                wfCommon.Msgbox(data);
                return;
            }
           
            // 確認画面初期化メソッドの呼び出し
            initPageTwo(JSON.parse(data)[0])

            // 確認画面表示の制御
            setPageTwoDisplay(JSON.parse(data)[0]);
        });
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
})(jQuery);