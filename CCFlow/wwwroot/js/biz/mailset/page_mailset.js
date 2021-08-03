/*▼グローバル変数定義エリア▼*/
var webUser = null;
var wfCommon = new wfCommon();                       // 共通関連
var rules1;                                          // 新規作成画面　画面項目チェックルール　その１
var rules2;                                          // 新規作成画面　画面項目チェックルール　その２
var rules3;                                          // 新規作成画面　画面項目チェックルール　その３
var modeFlg;                                         // モードフラグ

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {

    if (webUser == null)
        webUser = new WebUser();
    if (webUser.No == null)
        return;
    //画面初期化
    InitPage();
});
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/
/**
 * 画面初期化
 */
function InitPage() {

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

    // イベント定義
    createonevent();

    // 一覧設定
    if (createEmailSetTableInit() === null) {
        return;
    }

    // 送信先リスト取得
    if (setEmailTo() === null) {
        return;
    }

    // 入力チェックを設定する（新規作成画面）
    setinputcheck();

    // 入力チェックを設定する（詳細画面）
    setinputcheckDetail();

    // モードフラグ初期化
    modeFlg = 0;

    // 検索条件エリア非表示する
    $("#search_condition").hide();

    // テスト送信のその他の送信先非表示する
    $(".test-email-btn_1").hide();
}

/**
 * イベント定義メソッド
 */
function createonevent() {

    // 絞り込みボタン押す
    $("#formlist button.a-search-button").on("click", function () {

        // 絞り込み検索ダイアログ画面を開く
        var pageDialog = $("#setting-search-modal")[0].__component;
        pageDialog.opened = !0;
        pageDialog.onCloseRequested = function () {
            return pageDialog.opened = !1
        }

        // ワークフロー、申請者区分変更する時の処理
        setSearchDropdownEvent();
    })

    // 絞り込み検索ダイアログ画面のクリアボタンを押す
    $("#setting-search-modal button.a-button--third").on("click", function () {

        // 検索条件クリア
        clearSearchCondition();

        // ワークフロー、申請者区分変更する時の処理
        setSearchDropdownEvent();
    })

    // 絞り込み検索ダイアログ画面の検索ボタンを押す
    $("#setting-search-modal button.a-button--primary").on("click", function () {

        // 検索処理をする
        var searchRet = createEmailSetTableSearch();
        if (searchRet === null) {
            return;
        }

        // 絞り込みエリア設定
        var tagfilledStr = "";
        // 管理用タイトル
        if ($("#management_title_search").val() !== null && $("#management_title_search").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "管理用タイトル：" + $("#management_title_search").val() + "</span>";
        }
        // ドロップダウンリスト
        $("#setting-search-modal").find("select").each(function () {
            if ($(this).val() !== null && $(this).val() !== "") {

                tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + $(this).parents("div.m-form-field").find("label").html() + "：" + $(this).text() + "</span>";
            }
        });
        // 送信先TO
        var emailToTmp = "";
        $("#setting-search-modal").find("input[type=checkbox]:checked").each(function () {
            emailToTmp += $(this).parents("label.a-checkbox").find("div.a-checkbox__label").html() + ",";
        })
        if (emailToTmp !== "") {
            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "送信先TO：" + emailToTmp.substring(0, emailToTmp.length - 1) + "</span>";
        }
        // 送信先TO　その他送信先
        if ($("#email_content").val() !== null && $("#email_content").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "その他送信先：" + $("#email_content").val() + "</span>";
        }
        if (tagfilledStr !== "") {
            $("#search_condition .m-search-current-filters__tags").empty();
            $("#search_condition .m-search-current-filters__tags").append(tagfilledStr);
            $("#search_condition").show();
        } else {
            $("#search_condition").hide();
        }

        // 絞り込み検索ダイアログ画面を閉じる
        var pageDialog = $("#setting-search-modal")[0].__component;
        pageDialog.opened = !1;
    })

    // 検索条件バーエリアの条件クリアボタン押す
    $("button.m-search-current-filters__clear-button").on("click", function () {

        // 検索条件クリア
        clearSearchCondition();

        // ワークフロー、申請者区分変更する時の処理
        setSearchDropdownEvent();

        // 検索処理
        createEmailSetTableSearch();

        // 検索条件バーエリア非表示
        $("#search_condition").hide();
    })

    // 新規作成ボタン押す
    $("#creat_new").click(function () {

        // 新規作成画面へ遷移
        gotoPageTwo();
        $("body,html").scrollTop($("#setmailarea").offset().top);
        $("#management_id").val("");
        $("#mode_kbn").val(CREATE_MODE);
        $(".test-email-btn_1").hide();
        $(".kakunin-btn").show();

        // テキストを初期化する
        $("#setmailarea input").not("input[type=hidden],input[type=checkbox]").each(function () {
            $(this).val("");
        })
        // チェックボックスを初期化する
        $("#setmailarea").find("input[type=checkbox]").each(function () {
            $(this).prop("checked", false);
        })
        // テンプレートテキストを初期化する
        $("#email_template_editor").find("div.ql-editor").empty();
        // ワークフロー、申請者区分、送信タイミング区分を初期化する
        $("#workflow_p_div")[0].__component.reset();
        $("#workflow_p_div")[0].__component.enable();

        $("#seisinsyakbn_p_div")[0].__component.reset();
        $("#seisinsyakbn_p_div")[0].__component.disable();

        $("#sendtimingkbn_p_div")[0].__component.reset();
        $("#sendtimingkbn_p_div")[0].__component.disable();
        setAddOrCopyDropdownEvent(CREATE_MODE);
    });

    // 新規作成画面　確認ボタン押す
    $("#input-form-check-button").on("click", function () {

        $("#setmailform").validate().resetForm();
        $(".test-email-btn_1").hide();
        $(".kakunin-btn").show();

        modeFlg = 1;
        var tempArea = $("div.a-email-template-editor").find("div.ql-editor").html()
            .replace(/<p>/g, "")
            .replace(/<\/p>/g, "")
            .replace(/<br>/g, "");

        $("#hidden_temp_text").val(tempArea);

        addRules(rules1);
        removeRules(rules3);

        // チェックを実施する
        var flg = $("#setmailform").valid();

        if (!flg) {

            if ($("#hidden_temp_text").hasClass("error") && !$("div.a-email-template-editor").hasClass("error-border")) {
                $("div.a-email-template-editor").addClass("error-border");
            }
            return;
        }

        // 送信先Toチェック
        var errorFlg = 0;
        $("#email_to_other").find("input[type=email]").each(function () {
            if ($(this).val() !== null && $(this).val() !== "") {
                errorFlg = 1;
                return false;
            }
        })
        if (errorFlg === 0
            && $("#email_to").find("input[type=checkbox]:checked").length === 0) {
            wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["E0005"], "送信先To,その他の送信先"), null, null, setFocus);
            return;
        }

        // 確認画面設定
        setSettingCheckPage();

        modeFlg = 0;

        // 確認画面へ遷移
        var pageThree = $("#setmailcheck")[0].__component;
        pageThree.opened = !0;
    })

    // 新規作成画面　埋め込みコマンドマーク押す
    $("button.m-visit-record-input__button-close").on("click", function () {
        window.open('', '_self').close();
    })

    // 新規作成画面　保存せずに終了ボタンや一覧へボタン押す
    $("#setmailarea .back-to-list-page").on("click", function () {

        // 一覧画面へ戻る
        var pageTwo = $("#setmailarea")[0].__component;
        pageTwo.opened = !1; 
    })

    // 確認画面　戻るボタン押す
    $("#setmailcheckform button.a-button--third").on("click", function () {

        // 新規作成画面へ戻る
        var pageThree = $("#setmailcheck")[0].__component;
        pageThree.opened = !1;
    })

    // 確認画面　稼働停止で保存ボタン押す
    $("#setmailcheckform button.a-button--secondary").on("click", function () {

        // 稼働フラグ：停止
        saveInfo(OPERATION_FLG_OFF);
    })

    // 確認画面　稼働開始で保存ボタン押す
    $("#setmailcheckform button.a-button--primary").on("click", function () {

        // 稼働フラグ：開始
        saveInfo(OPERATION_FLG_ON);
    })

    // 新規作成画面　テスト送信ボタン押す
    $("#test_send_btn").on("click", function () {

        modeFlg = 2;
        $(".kakunin-btn").hide();
        $(".test-email-btn_2").show();
        $(".test-email-btn_1").show();

        var tempArea = $("div.a-email-template-editor").find("div.ql-editor").html()
            .replace(/<p>/g, "")
            .replace(/<\/p>/g, "")
            .replace(/<br>/g, "");

        $("#hidden_temp_text").val(tempArea);

        removeRules(rules1);
        addRules(rules2);

        $("#email_to").find("label.a-checkbox").each(function () {
            $(this).removeClass("a-checkbox--invalid-button");
            $(this).addClass("valid");
        })
        $("select").parents("div.choices__inner").each(function () {
            $(this).removeClass("error-other");
            $(this).addClass("valid");
        })

        // チェックを実施する
        var flg = $("#setmailform").valid();

        if (!flg) {
            if ($("#hidden_temp_text").hasClass("error") && !$("div.a-email-template-editor").hasClass("error-border")) {
                $("div.a-email-template-editor").addClass("error-border");
            }
            return;
        }

        modeFlg = 0;

        // テスト送信をする
        sendTestMail("test_email_div", encodeURIComponent(editTemplate($("div.a-email-template-editor__body").find("div.ql-editor"))), $("#email_temp_name").val());
    })

    // 詳細画面　テスト送信ボタン押す
    $("#test_send_btn_detail").on("click", function () {

        // チェックを実施する
        var flg = $("#setmaildetailform").valid();

        if (!flg) {
            return;
        }

        // テスト送信をする
        sendTestMail("test_email_div_detail", encodeURIComponent(editTemplate($("#email_temp_content_detail"))), $("#email_temp_name_detail").html());
    })

    // メイン画面へ遷移
    $("#goto_home").on("click", function () {
        window.location.href = '../menu/form_mainmenu.html';
    })

    // メイン画面表示
    window.onload = function () {

        // 絞り込み検索ダイアログ　ワークフロー、申請者区分、送信タイミング区分を取得し、設定する
        wfCommon.initDropdown(true, dtKbn["WF_EMAIL_KBN"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "workflow_search", "workflow_search_p_div");
        $("#seisinsyakbn_search_p_div")[0].__component.disable();
        $("#sendtimingkbn_search_p_div")[0].__component.disable();
        
        // 送信状況
        wfCommon.initDropdown(true, dtKbn["OPERATION_FLG"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "operation_flg", "operation_flg_p_div");

        // 新規作成画面　ワークフロー、申請者区分、送信タイミング区分を取得し、設定する
        wfCommon.initDropdown(true, dtKbn["WF_EMAIL_KBN"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "workflow", "workflow_p_div");
        $("#seisinsyakbn_p_div")[0].__component.disable();
        $("#sendtimingkbn_p_div")[0].__component.disable();

        // テスト送信エリア用　パラメータ選択ツール設定
        setParaArea();

        // 送信テンプレートテキストの変化を監視する
        watchEmailTemplateEditor();

        // テスト送信アドレステキストボックスの変化を監視する（新規作成画面）
        watchTestEmailAddress("");

        // テスト送信アドレステキストボックスの変化を監視する（詳細画面）
        watchTestEmailAddress("_detail");

        $("ul.m-setting-menu__list").children("li").first().addClass("m-setting-menu__item--active");
        $("ul.m-setting-menu__list li:nth-child(2)").removeClass("m-setting-menu__item--active");

        setAddOrCopyDropdownEvent(CREATE_MODE);
    }
}

/**
 * 送信先TOとその他の送信先に必須エラーが発生する時、フォーカス設定
 */
function setFocus() {
    if ($("#email_to").find("input[type=checkbox]").length > 0) {
        //$("#email_to").find("input[type=checkbox]").each(function () {
        //    $(this).parents("label.a-checkbox").addClass("a-checkbox--invalid");
        //    $(this).parents("label.a-checkbox").find("i").removeClass("a-icon--check-white");
        //    $(this).parents("label.a-checkbox").find("i").addClass("a-icon--check-purple");
        //})
        $($("#email_to").find("input[type=checkbox]")[0]).focus();
    } else {
        $($("#email_to_other").find("input[type=email]")[0]).focus();
    }
}

/**
 * ワークフロー、申請者区分変更する時の処理（検索用）
 */
function setSearchDropdownEvent() {

    // ワークフロー変更する
    $("#workflow_search").change(function () {

        var workflow = $("#workflow_search option:selected").val();

        if (workflow !== "") {
            var sinseisyakbn = getChildrenKbnList(dtKbn["SINSEISYA_KBN"], workflow, 3);
            //差出人区分
            wfCommon.initDropdown(true, sinseisyakbn, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "seisinsyakbn_search", "seisinsyakbn_search_p_div");
            //送信タイミング区分
            $("#seisinsyakbn_search_p_div")[0].__component.enable();
        } else {
            $("#seisinsyakbn_search_p_div")[0].__component.reset();
            $("#seisinsyakbn_search_p_div")[0].__component.disable();
        }

        $("#sendtimingkbn_search_p_div")[0].__component.reset();
        $("#sendtimingkbn_search_p_div")[0].__component.disable();

        // 申請者区分変更する
        $("#seisinsyakbn_search").on("change", function () {

            var seisinsyakbn = $("#seisinsyakbn_search option:selected").val();

            if (seisinsyakbn != "") {
                var timingkbn = getChildrenKbnList(dtKbn["EMAIL_TIMING_KBN"], seisinsyakbn.slice(3, 4), 1);
                //送信タイミング区分
                wfCommon.initDropdown(true, timingkbn, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sendtimingkbn_search", "sendtimingkbn_search_p_div");
                $("#sendtimingkbn_search_p_div")[0].__component.enable();
            } else {
                $("#sendtimingkbn_search_p_div")[0].__component.reset();
                $("#sendtimingkbn_search_p_div")[0].__component.disable();
            }
        });
    });
}

/**
 * ワークフロー、申請者区分変更する時の処理
 */
function setAddOrCopyDropdownEvent(modeKbn) {

    // ワークフロー変更する
    $("#workflow").change(function () {

        var workflow = $("#workflow option:selected").val();

        if (workflow !== "") {
            var sinseisyakbn = getChildrenKbnList(dtKbn["SINSEISYA_KBN"], workflow, 3);
            //差出人区分
            wfCommon.initDropdown(true, sinseisyakbn, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "seisinsyakbn", "seisinsyakbn_p_div");
            $("#seisinsyakbn_p_div")[0].__component.enable();
        } else {
            $("#seisinsyakbn_p_div")[0].__component.reset();
            $("#seisinsyakbn_p_div")[0].__component.disable();
        }
        if (modeFlg === 1) {
            $("#setmailform").validate().element($("#workflow"));
            $("#setmailform").validate().element($("#seisinsyakbn"));
            $("#setmailform").validate().element($("#sendtimingkbn"));
        }

        //送信タイミング区分
        $("#sendtimingkbn_p_div")[0].__component.reset();
        $("#sendtimingkbn_p_div")[0].__component.disable();

        //if (modeKbn === CREATE_MODE) {
            // 申請者区分変更する
            $("#seisinsyakbn").on("change", function () {

                var seisinsyakbn = $("#seisinsyakbn option:selected").val();

                if (seisinsyakbn != "") {
                    var timingkbn = getChildrenKbnList(dtKbn["EMAIL_TIMING_KBN"], seisinsyakbn.slice(3, 4), 1);
                    //送信タイミング区分
                    wfCommon.initDropdown(true, timingkbn, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sendtimingkbn", "sendtimingkbn_p_div");
                    $("#sendtimingkbn_p_div")[0].__component.enable();
                } else {
                    $("#sendtimingkbn_p_div")[0].__component.reset();
                    $("#sendtimingkbn_p_div")[0].__component.disable();
                }

                if (modeFlg === 1) {
                    $("#setmailform").validate().element($("#seisinsyakbn"));
                    $("#setmailform").validate().element($("#sendtimingkbn"));
                }

                // ステータス変更する
                $("#sendtimingkbn").on("change", function () {

                    if (modeFlg === 1) {
                        $("#setmailform").validate().element($("#sendtimingkbn"));
                    }
                })
            });
        //}
    });

    if (modeKbn === COPY_MODE || modeKbn === EDIT_MODE) {

        // 申請者区分変更する
        $("#seisinsyakbn").on("change", function () {

            var seisinsyakbn = $("#seisinsyakbn option:selected").val();

            if (seisinsyakbn != "") {
                var timingkbn = getChildrenKbnList(dtKbn["EMAIL_TIMING_KBN"], seisinsyakbn.slice(3, 4), 1);
                //送信タイミング区分
                wfCommon.initDropdown(true, timingkbn, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sendtimingkbn", "sendtimingkbn_p_div");
                $("#sendtimingkbn_p_div")[0].__component.enable();
            } else {
                $("#sendtimingkbn_p_div")[0].__component.reset();
                $("#sendtimingkbn_p_div")[0].__component.disable();
            }

            if (modeFlg === 1) {
                $("#setmailform").validate().element($("#seisinsyakbn"));
                $("#setmailform").validate().element($("#sendtimingkbn"));
            }

            // ステータス変更する
            $("#sendtimingkbn").on("change", function () {

                if (modeFlg === 1) {
                    $("#setmailform").validate().element($("#sendtimingkbn"));
                }
            })
        });
    }
}

/**
 * 検索条件クリア
 */
function clearSearchCondition() {

    $("#setting-search-modal").find("input[type=text],textarea").each(function () {
        $(this).val("");
    });

    $("#workflow_search_p_div")[0].__component.reset();
    $("#seisinsyakbn_search_p_div")[0].__component.reset();
    $("#sendtimingkbn_search_p_div")[0].__component.reset();

    $("#seisinsyakbn_search_p_div")[0].__component.disable();
    $("#sendtimingkbn_search_p_div")[0].__component.disable();

    // 送信先タイプ
    $("#setting-search-modal").find("input[type=checkbox]:checked").each(function () {
        $(this).prop("checked",false);
    });
    // 送信状況
    $("#operation_flg_p_div")[0].__component.reset();
}

/**
 * 「…」ボタン押すと表示するactionエリアの位置処理（初期化以外）
 */
function setActionsPosition(obj) {

    if (obj.hasClass("a-table-action-button--active")) {
        var t = obj.parents("div.a-table-action-button").find("button.a-table-action-button__button")[0]
            , e = obj.parents("div.a-table-action-button").find(".a-table-action-button__action")[0]
            , r = t.getBoundingClientRect()
            , i = e.getBoundingClientRect();
        if ("undefined" != typeof window) {
            var o = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0)
                , a = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
            r.right + r.width + i.width > o ? e.style.left = "".concat(r.left + r.width - i.width - 4, "px") : e.style.left = "".concat(r.left, "px"),
                r.bottom + r.height + i.height > a ? (e.style.top = "".concat(r.top - i.height - 4, "px"),
                    e.style.bottom = "auto") : (e.style.top = "".concat(r.top + r.height + 4, "px"),
                        e.style.bottom = "auto")
        }
    }
}

/**
 * 「…」ボタン押すと表示するactionエリアについての処理（初期化以外）
 */
function mentionEvent() {
    $(".a-table-action-button__button").on("click", function () {
        $(this)[0].classList.toggle("a-table-action-button--active");
        $(this).parents("div.a-table-action-button")[0].classList.toggle("a-table-action-button--active");

        setActionsPosition($(this));
    })

    if (window.addEventListener) {
        window.addEventListener("scroll", function () {
            setActionsPosition($("button.a-table-action-button--active"));
        });
        window.addEventListener("resize", function () {
            setActionsPosition($("button.a-table-action-button--active"));
        });
    } else if (window.attachEvent) {
        window.attachEvent("scroll", function () {
            setActionsPosition($("button.a-table-action-button--active"));
        });
        window.attachEvent("resize", function () {
            setActionsPosition($("button.a-table-action-button--active"));
        });
    }

    // mousedownイベント
    document.addEventListener("mousedown", (function (t) {
        $(".a-table-action-button__button").each(function () {

            if (!$(this).parents("div.a-table-action-button").find(".a-table-action-button__action")[0].contains(t.target)) {
                $(this).removeClass("a-table-action-button--active");
                $(this).parents("div.a-table-action-button").removeClass("a-table-action-button--active");
            }

        })
    }))
}

/**
 * 送信テンプレートテキストの変化を監視するメソッド
 */
function watchEmailTemplateEditor() {

    var targetNode = document.querySelector('#email_template_editor');

    var config = { attributes: true, childList: true, characterData: true, subtree: true };

    var callback = function (mutationsList) {
        for (var i = 0; i < mutationsList.length; i++) {
            if (mutationsList[i].type === 'childList') {
                var tempArea = $("div.a-email-template-editor").find("div.ql-editor").html()
                    .replace(/<p>/g, "")
                    .replace(/<\/p>/g, "")
                    .replace(/<br>/g, "");

                $("#hidden_temp_text").val(tempArea);

                if (modeFlg === 1 || modeFlg === 2) {
                    $("#setmailform").validate().element($("#hidden_temp_text"));
                    if ($("#hidden_temp_text-error").html() === "") {
                        $("div.a-email-template-editor").removeClass("error-border");
                    } else {
                        $("div.a-email-template-editor").addClass("error-border");
                    }
                }
            } else if (mutationsList[i].type === 'attributes') {
            } else if (mutationsList[i].type === 'characterData') {
            } else if (mutationsList[i].type === 'subtree') {
            }
        }
    };

    var observer = new MutationObserver(callback);
    observer.observe(targetNode, config);
}

/**
 * テスト送信アドレステキストボックスの変化を監視するメソッド
 */
function watchTestEmailAddress(name) {

    var targetNode = document.querySelector('#test_email_div' + name);

    var config = { attributes: true, childList: true, characterData: true, subtree: true };

    var callback = function (mutationsList) {
        for (var i = 0; i < mutationsList.length; i++) {
            if (mutationsList[i].type === 'childList') {
                if (mutationsList[i].addedNodes.length > 0
                    && mutationsList[i].addedNodes[0].classList
                    && mutationsList[i].addedNodes[0].classList.value === "m-recipient-input__item") {
                    $('#test_email_div' + name).find("input[type=email]").each(function (index, element) {
                        $(element).attr("id", "test_email_input" + name + (index + 1));
                        $(element).attr("name", "test_email_input" + name + (index + 1));
                        $(element).addClass("input-group-email" + name);
                    })
                }
            } else if (mutationsList[i].type === 'attributes') {
            } else if (mutationsList[i].type === 'characterData') {
            } else if (mutationsList[i].type === 'subtree') {
            }
        }
    };

    var observer = new MutationObserver(callback);
    observer.observe(targetNode, config);
}

/**
 * テスト送信処理
 */
function sendTestMail(id,emailContents,emailName) {

    // テスト送信先アドレス取得する
    var emaiList = "";
    $("#" + id).find("input[type=email]").each(function () {

        if ($(this).val() !== null && $(this).val() !== "") {
            emaiList += $(this).val() + ",";
        }
    })

    // テスト送信をする
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    handler.AddUrlData();
    handler.AddPara("EmailList", emaiList.substring(0, emaiList.length - 1));
    handler.AddPara("Temp_Contents", emailContents);
    handler.AddPara("Temp_Name", emailName);
    var data = handler.DoMethodReturnString("DoTestMailSend");

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }
    wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0001"], "テスト送信,成功"), "", "OK", null);
}

/**
 * 保存処理
 */
function saveInfo(operationFlg) {

    // サーバ側のメソッド指定
    var methodName = "";
    if ($("#mode_kbn").val() === EDIT_MODE) {
        methodName = "UpdateEmailSet";
    } else {
        methodName = "InsertEmailSet";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    // 共通対象の追加
    var ht = new HashTblCommon();
    
    // 管理用タイトル
    ht.Add("ManagementId", $("#management_id").val());
    ht.Add("ManagementTitle", $("#management_title").val());
    // ワークフロー
    ht.Add("WfEmailKbn", $("#workflow").val());
    // 申請者区分
    ht.Add("EmailTimingKbn", $("#sendtimingkbn").val());
    // ステータス
    ht.Add("SinseisyaKbn", $("#seisinsyakbn").val());
    // 送信先To
    ht.Add("EmailTo", $("#hidden_email_to_confirm").val());
    // 送信先To その他
    ht.Add("EmailToOther", $("#email_to_other_confirm").html());
    // 送信先CC
    ht.Add("EmailCc", $("#hidden_email_cc_confirm").val());
    // 送信先CC その他
    ht.Add("EmailCcOther", $("#email_cc_other_confirm").html());
    // 送信先BCC
    ht.Add("EmailBcc", $("#hidden_email_bcc_confirm").val());
    // 送信先BCC その他
    ht.Add("EmailBccOther", $("#email_bcc_other_confirm").html());
    // 件名
    ht.Add("EmailTempName", $("#email_temp_name").val());
    // テキスト
    var contentsBefore = $("#email_temp_contents_confirm").html();
    // 稼働フラグ
    ht.Add("OperationFlg", operationFlg);
    ht.Add("REC_ENT_USER", webUser.No);
    ht.Add("REC_ENT_USER_KANJI", webUser.Name);
    handler.AddPara("UpdateEmailSetReq", ht.stringify());
    handler.AddPara("EmailTempContents", encodeURIComponent(editTemplate($("#email_temp_contents_confirm"))));
    handler.AddPara("Remarks", encodeURIComponent(contentsBefore));
    var data = handler.DoMethodReturnString(methodName);

    // データ存在している場合の処理
    if (data.indexOf('msg@') === 0) {
        data = JSON.parse(data.split("@")[1]);
        var key = data["key"];
        var value = data["value"];
        wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg[key], value), null, null, gotoPageTwoFromThree);
        return;
    }

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 一覧画面へ遷移する
    gotoPageOne();

    // 検索処理
    createEmailSetTableSearch();

    wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0005"], "保存"), null, null, null);
}

/**
 * テキスト（テンプレート）文字処理
 */
function editTemplate(elment) {
    var tempStr = "";
    //var elmentTmp = elment.clone();
    //var elmentTmp = $("<p>" + elment.html().replaceAll("<p>", "").replaceAll("</p>", "&lt;br&gt;") + "</p>");
    var elmentTmp = $("<p>" + elment.html().replace(/<p>/g, "").replace(/<\/p>/g, "&lt;br&gt;").replace(/&nbsp;/g, "") + "</p>");
    elmentTmp.find("span").each(function () {
        var datacode = $(this).data("code");
        if (datacode !== undefined && /^(%%).+(%%)$/g.test(datacode)) {
            $(this).after(datacode);
            $(this).remove();
        }
    })
    tempStr = elmentTmp.text();
    elmentTmp.remove();
    return tempStr;
}

/**
 * 登録する処理に例外が発生する場合の処理
 */
function gotoPageTwoFromThree() {

    // 新規作成画面へ戻る
    var pageThree = $("#setmailcheck")[0].__component;
    pageThree.opened = !1;

    var dialogDivPage = $("#app-dialog-div")[0].__component;
    dialogDivPage.opened = !1;

    $("#management_title").focus();
}

/**
 * 正常に登録する場合の処理
 */
function gotoPageOne() {

    // 一覧画面へ戻る
    var pageThree = $("#setmailcheck")[0].__component;
    pageThree.opened = !1;

    var pageTwo = $("#setmailarea")[0].__component;
    pageTwo.opened = !1;
}

/**
 * テスト送信エリア用　パラメータ選択ツール設定
 */
function setParaArea() {
    var t = document.querySelector(".m-setting-menu");
    t && t.__component.changeActiveIndex(1);
    var n = document.querySelector(".a-email-template-editor");
    if (n) {
        var e = [{
            id: 1,
            value: "申請番号",
            code: "%%OID%%"
        }, {
            id: 2,
            value: "申請区分",
            code: "%%APP_KBN%%"
        }, {
            id: 3,
            value: "対象者社員番号",
            code: "%%SHAINBANGO%%"
        }, {
            id: 4,
            value: "対象者氏名",
            code: "%%SEIMEI_KANJI%%"
        }, {
            id: 5,
            value: "対象者所属",
            code: "%%SYOZOKU1%%"
        }, {
            id: 5,
            value: "対象者社籍",
            code: "%%KAISYAMEI%%"
        }];
        n.__component.onResolvePlaceholder = function (t) {
            return new Promise((function (n) {
                console.log("searchTerm", t),
                    setTimeout((function () {
                        n(e)
                    }
                    ), 250)
            }
            ))
        }
    }
}

/**
 * 連絡先リスト取得と設定（送信To、CC、BCC）
 */
function setEmailTo() {

    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    var data = handler.DoMethodReturnString("GetEmailStakeholderList");

    // 例外処理
    if (data.indexOf('err@') == 0) {
        wfCommon.Msgbox(data);
        return null;
    }

    // JSON対象に転換
    var jsondata = JSON.parse(data);
    setEmailToDetail("email_to", jsondata, "email_to_label");
    setEmailToDetail("email_cc", jsondata, "email_cc_label");
    setEmailToDetail("email_bcc", jsondata, "email_bcc_label");
    setEmailToDetail("email_to_search", jsondata, "email_to_search_label");
}

/**
 * 連絡先リスト取得と設定
 */
function setEmailToDetail(id, jsondata, labelId) {

    for (var i = 0; i < jsondata.length; i++) {
        var newId = labelId + "_" +  (i + 1);
        var checkBoxId = id + "_checkbox_" + (i + 1);

        $("#" + labelId).clone().appendTo("#" + id);
        $("#" + labelId).attr("id", newId);
        $("#" + newId).find("input[name=hidden_" + id + "]").val(jsondata[i].RNRAKUSAKI_ID);
        $("#" + newId).find("div.a-checkbox__label").html(jsondata[i].RNRAKUSAKI_NAME);
        $("#" + newId).find("input[type=checkbox]").attr("id", checkBoxId);
        $("#" + newId).find("input[type=checkbox]").attr("name", id + "_checkbox[]");
    }
    $("#" + labelId).remove();
}

/**
 * 新規作成確認画面　各項目設定
 */
function setSettingCheckPage() {

    // 管理用タイトル
    $("#management_title_confirm").html($("#management_title").val());
    // ワークフロー
    $("#workflow_confirm").html($("#workflow option:selected").text());
    // 申請者区分
    $("#seisinsyakbn_confirm").html($("#seisinsyakbn option:selected").text());
    // ステータス
    $("#sendtimingkbn_confirm").html($("#sendtimingkbn option:selected").text());
    // 送信先 To
    getCheckedEmailType("email_to");
    // 送信先 CC
    getCheckedEmailType("email_cc");
    // 送信先 BCC
    getCheckedEmailType("email_bcc");
    // 件名
    $("#email_temp_name_confirm").html($("#email_temp_name").val());
    // テキスト
    $("#email_temp_contents_confirm").html($("div.a-email-template-editor__body").find("div.ql-editor").html());
}

/**
 * 新規作成確認画面　送信先 To、CC、BCC項目設定
 */
function getCheckedEmailType(id) {

    var checkedConfirm = "";
    var checkedConfirmHidden = "";
    var checkedConfirmEdit = "なし";
    $("#" + id).find("input[type=checkbox]")
        .filter(function () { return $(this).prop("checked") }).each(function () {
        checkedConfirm += $(this).parents("label.a-checkbox").find("div.a-checkbox__label").html() + "、";
        checkedConfirmHidden += $(this).parents("label.a-checkbox").find("input[name=hidden_" + id + "]").val() + ",";
    });
    if (checkedConfirm !== "") {
        checkedConfirmEdit = checkedConfirm.substring(0, checkedConfirm.length - 1);
    }

    var otherEmailConfirm = "";
    $("#" + id + "_other").find("input[type=email]").each(function () {
        if ($(this).val() !== null && $(this).val() !== "") {
            otherEmailConfirm += $(this).val() + ",";
        }
    });

    // 送信先 To・CC・BCC
    var objConfirm = $("#" + id + "_confirm");
    objConfirm.html(checkedConfirmEdit);
    $("#hidden_" + id + "_confirm").val(checkedConfirmHidden.substring(0, checkedConfirmHidden.length - 1));

    if (otherEmailConfirm === "") {
        $("#" + id + "_other_confirm").html("");
        $("#" + id + "_other_div").hide();
    } else {
        $("#" + id + "_other_confirm").html(otherEmailConfirm.substring(0, otherEmailConfirm.length - 1));
        $("#" + id + "_other_div").show();
    }
}

/**
 * 入力チェックを設定する（新規作成画面）
 */
function setinputcheck() {

    rules1 = {
        management_title: {
            required: true
        },
        // ワークフロー
        workflow: {
            required: true
        },
        // 申請者区分
        seisinsyakbn: {
            required: true
        },
        // 送信タイミング
        sendtimingkbn: {
            required: true
        },
        //// 送信先（To）
        //"email_to_checkbox[]": {
        //    required: true
        //},
        // テンプレート件名
        email_temp_name: {
            required: true
        },
        // テンプレートテキスト
        hidden_temp_text: {
            required: true
        },
    };

    var require_group = {
        require_from_group: [1, '.input-group-email']
    };
    rules2 = {
        // テンプレート件名
        email_temp_name: {
            required: true
        },
        // テンプレートテキスト
        hidden_temp_text: {
            required: true
        },
    };
    rules3 = {};
    //var messages = {
    //    "email_to_checkbox[]": { required: "このフィールドは必須です。" },
    //};

    // テスト送信アドレス
    for (var i = 1; i <= 100; i++) {
        rules1["test_email_input" + i] = require_group;
        rules2["test_email_input" + i] = require_group;
        rules3["test_email_input" + i] = require_group;
    }

    $("#setmailform").validate({
        focusCleanup: true,
        onkeyup: false,
        ignore: ".ql-container *",
        /*messages: messages,*/
        rules: rules1,
        // エラーエリアを設定する
        errorPlacement: function (error, element) {

            if (element.attr("name") === "hidden_temp_text") {
                error.insertAfter($("div.a-email-template-editor"));
                $("div.a-email-template-editor").addClass("error-border");
            } else if (element.is("select,input[type=checkbox]")) {
                if (element.parents("div.m-form-field__row").next().attr("id") !== element.attr("name") + "-error") {
                    error.insertAfter(element.parents("div.m-form-field__row"));
                }
            } else {
                error.insertAfter(element);
            }
        },
        success: function (element) {
            if (element[0].id === "hidden_temp_text-error") {
                $("div.a-email-template-editor").removeClass("error-border");
            }
        }
    });
}

/**
 * 入力チェックを設定する（詳細画面）
 */
function setinputcheckDetail() {

    var require_group_detail = {
        require_from_group: [1, '.input-group-email_detail']
    };

    var rules3 = {};

    // テスト送信アドレス
    for (var i = 1; i <= 100; i++) {
        rules3["test_email_input_detail" + i] = require_group_detail;
    }

    $("#setmaildetailform").validate({
        focusCleanup: true,
        onkeyup: false,
        ignore: "",
        rules: rules3,
        // エラーエリアを設定する
        errorPlacement: function (error, element) {
            error.insertAfter(element.parents("div.a-text-field").find("input[type=email]"));
        }
    });
}

/**
 * 一覧作成（初期化）
 */
function createEmailSetTableInit() {

    // 一覧取得
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    handler.AddUrlData();
    var ht = new HashTblCommon();
    handler.AddPara("EmailSetReq", ht.stringify());
    var data = handler.DoMethodReturnString("GetEmailSetList");

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return null;
    }

    // テーブル設定
    createEmailSetTable(JSON.parse(data));
}

/**
 * 一覧作成（検索ボタン押す）
 */
function createEmailSetTableSearch() {

    // 一覧取得
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    var ht = new HashTblCommon();
    ht.Add("WfEmailKbn", $("#workflow_search").val());
    ht.Add("EmailTimingKbn", $("#sendtimingkbn_search").val());
    ht.Add("SinseisyaKbn", $("#seisinsyakbn_search").val());
    ht.Add("ManagementTitle", $.trim($("#management_title_search").val()));

    var email = [];
    if ($("#email_content").val() !== null && $.trim($("#email_content").val()) !== "") {
        email = getDataList($.trim($("#email_content").val()));
    }
    var emailTo = [];
    $("#email_to_search").find("input[type=checkbox]:checked").each(function () {

        emailTo.push($(this).parents("label.a-checkbox").find("input[name=hidden_email_to_search]").val());
    })
    ht.Add("EmailTo", emailTo);
    ht.Add("Email", email);
    ht.Add("OperationFlg", $("#operation_flg").val());
    handler.AddPara("EmailSetReq", ht.stringify());
    var data = handler.DoMethodReturnString("GetEmailSetList");

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return null;
    }

    // テーブル設定
    createEmailSetTable(JSON.parse(data));
    mentionEvent();
    return email;
}

/**
 * 一覧作成メソッド
 */
function createEmailSetTable(jsondata) {

    var dateHandle = function (res) {
        var result = "";
        if (res.REC_EDT_DATE !== null) {
            result = moment(res).format(DATETIME_FORMAT_MOMENT_PATTERN_1) + "<br>更新者：" +res.REC_EDT_USER_KANJI;
        }
        return result;
    }

    var timingHandle = function (res) {
        
        return res.SINSEISYA_KBN_NM + "が" + res.WF_EMAIL_KBN_NM + "の<br/>" + res.EMAIL_TIMING_KBN_NM + "を完了した時";
    }

    var atesakiHandle = function (res) {

        var typeList = [];
        var rnrakusakiStr = "";
        if (res.RNRAKUSAKI_NAME !== null && res.RNRAKUSAKI_NAME !== "") {
            typeList.push(res.RNRAKUSAKI_NAME);
        }
        if (res.RNRAKUSAKI_NAME2 !== null && res.RNRAKUSAKI_NAME2 !== "") {
            typeList.push(res.RNRAKUSAKI_NAME2);
        }
        rnrakusakiStr += typeList.join(",");

        var otherList = getDataList(res.EMAIL_TO_OTHER);
        if (res.EMAIL_TO_OTHER !== null && res.EMAIL_TO_OTHER !== ""
            && otherList.length > 0) {
            if (rnrakusakiStr !== "") {
                rnrakusakiStr += ",<br/>";
            }
            rnrakusakiStr += otherList[0];
            if (otherList.length > 1) {
                rnrakusakiStr += ",他" + (otherList.length - 1) + "名";
            }
        }
        return rnrakusakiStr;
    }

    var columns = [
        {
            field: 'MANAGEMENT_ID',
            title: 'ID',
            modify_type_class: 'type-id',
        }, {
            field: 'MANAGEMENT_TITLE', 	
            title: '管理用タイトル', 		
            modify_type_class: 'type-data', 
        }, {
            field: 'OPERATION_FLG_NM',
            title: '送信状況',
            modify_type_class: 'type-data',
        }, {
            field: 'WF_EMAIL_KBN_NM',
            title: '送信タイミング',
            modify_type_class: 'type-data',
            formatter: timingHandle
        }, {
            field: 'RNRAKUSAKI_NAME',
            title: '送信先',
            modify_type_class: 'type-data',
            formatter: atesakiHandle
        }, {
            field: 'REC_EDT_DATE',
            title: '更新日時',
            modify_type_class: 'type-data',
            formatter: dateHandle
        }, 
        {
            field: 'ActionEvent',
                modify_type_class: 'type-action', 
            show_class: "1", 
            title_action: [
                {
                    title: '編集する',
                    onClick: function (event, index, dataList, data) {
                        
                        //recordEdit(data, EDIT_MODE);
                        recordCopy(data, EDIT_MODE);
                    }
                }, {
                    title: '送信状況を切り替える',
                    onClick: function (event, index, dataList, data) {

                        var operationState = "";
                        if (data.OPERATION_FLG === OPERATION_FLG_ON) {
                            operationState = "選択されたレコードの送信状況,停止中に変更";
                        } else {
                            operationState = "選択されたレコードの送信状況,稼働中に変更";
                        }

                        var para = [];
                        para.push(data.MANAGEMENT_ID);
                        para.push(data.OPERATION_FLG);
                        wfCommon.ShowDialog(DIALOG_CONFIRM, wfCommon.MsgFormat(msg["W0002"], operationState), null, null, recordOperationState, para);
                    }
                }, {
                    title: 'コピーする',
                    onClick: function (event, index, dataList, data) {
                        
                        recordCopy(data, COPY_MODE);
                    }
                }, {
                    title: '削除する',
                    onClick: function (event, index, dataList, data) {

                        var para = [];
                        para.push(data.MANAGEMENT_ID);
                        wfCommon.ShowDialog(DIALOG_CONFIRM, wfCommon.MsgFormat(msg["W0002"], "選択されたレコード,削除"), null, null, recordDel, para);
                   }
                }
            ]
        }		
    ];

    var optionData = {
        data: jsondata,  // サーバから取得のデータ		
        pagination: true, // ページネーション：表示		
        pageNumber: 1, // 初期化１ページ目を設定		
        pageSize: 5, // １ページ表示件数を設定		
        columns: columns, // カラム設定
        norecordmsg: "該当データは存在しません。",
        onRowClick: function (event, index, dataList, data) { // 行クリック		

            // 詳細画面項目設定
            recordDetailDisplay(data);

            // 詳細画面表示
            $("#create-modal").addClass("o-modal--opened");
            var pageFour = $("#create-modal")[0].__component;
            pageFour.onCloseRequested = function () {
                return pageFour.opened = !1
            }
            $("#setmaildetailform").validate().resetForm();
        }
    };

    // 一覧の作成		
    $("#table").createTable(optionData);
}

/**
 * 詳細画面設定
 */
function recordDetailDisplay(data) {

    // タイトル
    $("#management_id_detail").html("ID: " + data.MANAGEMENT_ID);
    $("#management_title_detail").html(data.MANAGEMENT_TITLE);
    // 稼働状態
    $("#operation_flg_detail").html(data.OPERATION_FLG_NM);
    // ワークフロー
    $("#workflow_detail").html(data.WF_EMAIL_KBN_NM + "の");
    // 申請者区分
    $("#seisinsyakbn_detail").html(data.SINSEISYA_KBN_NM + "が");
    // ステータス
    $("#sendtimingkbn_detail").html(data.EMAIL_TIMING_KBN_NM + "を完了した時");
    // 件名
    $("#email_temp_name_detail").html(data.EMAIL_TEMP_NAME);
    // テキスト
    $("#email_temp_content_detail").html(data.REMARKS);
    // 送信先TO
    if (data.EMAIL_TO !== null && data.EMAIL_TO !== "") {
        var nameList = [];
        if (data.RNRAKUSAKI_NAME !== null && data.RNRAKUSAKI_NAME !== "") {
            nameList.push(data.RNRAKUSAKI_NAME);
        }
        if (data.RNRAKUSAKI_NAME2 !== null && data.RNRAKUSAKI_NAME2 !== "") {
            nameList.push(data.RNRAKUSAKI_NAME2);
        }
        $("#email_to_detail").html(nameList.join(","));
    } else {
        $("#email_to_detail").html("なし");
    }
    // 送信先CC
    if (data.EMAIL_CC !== null && data.EMAIL_CC !== "") {
        var nameList = [];
        if (data.RNRAKUSAKI_NAME_CC !== null && data.RNRAKUSAKI_NAME_CC !== "") {
            nameList.push(data.RNRAKUSAKI_NAME_CC);
        }
        if (data.RNRAKUSAKI_NAME_CC2 !== null && data.RNRAKUSAKI_NAME_CC2 !== "") {
            nameList.push(data.RNRAKUSAKI_NAME_CC2);
        }
        $("#email_cc_detail").html(nameList.join(","));
    } else {
        $("#email_cc_detail").html("なし");
    }
    // 送信先BCC
    if (data.EMAIL_BCC !== null && data.EMAIL_BCC !== "") {
        var nameList = [];
        if (data.RNRAKUSAKI_NAME_BCC !== null && data.RNRAKUSAKI_NAME_BCC !== "") {
            nameList.push(data.RNRAKUSAKI_NAME_BCC);
        }
        if (data.RNRAKUSAKI_NAME_BCC2 !== null && data.RNRAKUSAKI_NAME_BCC2 !== "") {
            nameList.push(data.RNRAKUSAKI_NAME_BCC2);
        }
        $("#email_bcc_detail").html(nameList.join(","));
    } else {
        $("#email_bcc_detail").html("なし");
    }
    // 送信先TO　その他送信先
    if (data.EMAIL_TO_OTHER === null || data.EMAIL_TO_OTHER === "") {
        $("#email_to_other_detail").parents("div.m-form-field").hide();
    } else {
        $("#email_to_other_detail").html(data.EMAIL_TO_OTHER);
        $("#email_to_other_detail").parents("div.m-form-field").show();
    }
    // 送信先CC　その他送信先
    if (data.EMAIL_CC_OTHER === null || data.EMAIL_CC_OTHER === "") {
        $("#email_cc_other_detail").parents("div.m-form-field").hide();
    } else {
        $("#email_cc_other_detail").html(data.EMAIL_CC_OTHER);
        $("#email_cc_other_detail").parents("div.m-form-field").show();
    }
    // 送信先BCC　その他送信先
    if (data.EMAIL_BCC_OTHER === null || data.EMAIL_BCC_OTHER === "") {
        $("#email_bcc_other_detail").parents("div.m-form-field").hide();
    } else {
        $("#email_bcc_other_detail").html(data.EMAIL_BCC_OTHER);
        $("#email_bcc_other_detail").parents("div.m-form-field").show();
    }
}

/**
 * 稼働状態変更ボタン押す時の処理
 */
function recordOperationState(managementId, operationFlg) {

    // 稼働状態変更処理
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    handler.AddPara("ManagementId", managementId);
    handler.AddPara("OperationFlg", operationFlg === OPERATION_FLG_OFF ? OPERATION_FLG_ON : OPERATION_FLG_OFF);
    var data = handler.DoMethodReturnString("UpdateOperationFlg");

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 検索処理
    createEmailSetTableSearch();

    wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0001"], "送信状況の切り替え,成功"), null, null, null);
}

/**
 * 削除ボタン押す時の処理
 */
function recordDel(managementId) {

    // 削除処理
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailSet");
    handler.AddPara("ManagementId", managementId);
    var data = handler.DoMethodReturnString("DeleteEmailSet");

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 検索処理
    createEmailSetTableSearch();

    wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0001"], "削除,成功"), null, null, null);
}

/**
 * 編集・コピーボタン押す時の処理（共通項目）
 */
function recordEditOrCopy(rowData,modeKbn) {

    // モード区分
    $("#mode_kbn").val(modeKbn);
    // 管理用タイトル
    if (modeKbn === EDIT_MODE) {
        $("#management_id").val(rowData.MANAGEMENT_ID);
    } else {
        $("#management_id").val("")
    }
    $("#management_title").val(rowData.MANAGEMENT_TITLE);
    // 送信先Toとその他の送信先
    setEditPageEmail(rowData.EMAIL_TO, rowData.EMAIL_TO_OTHER, "email_to");
    // 送信先CCとその他の送信先
    setEditPageEmail(rowData.EMAIL_CC, rowData.EMAIL_CC_OTHER, "email_cc");
    // 送信先BCCとその他の送信先
    setEditPageEmail(rowData.EMAIL_BCC, rowData.EMAIL_BCC_OTHER, "email_bcc");
    // 件名
    $("#email_temp_name").val(rowData.EMAIL_TEMP_NAME);
    // テキスト
    $("#email_template_editor").find("div.ql-editor").empty();
    $("#email_template_editor").find("div.ql-editor").append(rowData.REMARKS);
    // テスト送信アドレスクリア
    $("input[name^=test_email_input]").each(function () {
        $(this).val("");
    })
    // 新規作成画面へ遷移
    gotoPageTwo();
}

/**
 * 編集ボタン押す時の処理
 */
function recordEdit(rowData, modeKbn) {

    // ワークフロー
    $("#workflow_p_div")[0].__component._choices.setChoiceByValue(rowData.WF_EMAIL_KBN);
    $("#workflow_p_div")[0].__component.disable();

    //// 申請者区分
    wfCommon.initDropdown(true, dtKbn["SINSEISYA_KBN"], rowData.SINSEISYA_KBN, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "seisinsyakbn", "seisinsyakbn_p_div");
    $("#seisinsyakbn_p_div")[0].__component.disable();

    // ステータス
    wfCommon.initDropdown(true, dtKbn["EMAIL_TIMING_KBN"], rowData.EMAIL_TIMING_KBN, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sendtimingkbn", "sendtimingkbn_p_div");
    $("#sendtimingkbn_p_div")[0].__component.disable();

    // 共通項目
    recordEditOrCopy(rowData, modeKbn);
}

/**
 * コピーボタン押す時の処理
 */
function recordCopy(rowData, modeKbn) {

    // ワークフロー
    $("#workflow_p_div")[0].__component._choices.setChoiceByValue(rowData.WF_EMAIL_KBN);

    // 申請者区分
    var sinseisyakbn = getChildrenKbnList(dtKbn["SINSEISYA_KBN"], rowData.WF_EMAIL_KBN, 3);
    wfCommon.initDropdown(true, sinseisyakbn, rowData.SINSEISYA_KBN, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "seisinsyakbn", "seisinsyakbn_p_div");

    // ステータス
    var timingkbn = getChildrenKbnList(dtKbn["EMAIL_TIMING_KBN"], rowData.SINSEISYA_KBN.slice(3, 4), 1);
    wfCommon.initDropdown(true, timingkbn, rowData.EMAIL_TIMING_KBN, MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sendtimingkbn", "sendtimingkbn_p_div");

    $("#workflow_p_div")[0].__component.enable();
    $("#seisinsyakbn_p_div")[0].__component.enable();
    $("#sendtimingkbn_p_div")[0].__component.enable();
    setAddOrCopyDropdownEvent(COPY_MODE);

    // 共通項目
    recordEditOrCopy(rowData, modeKbn);
}

/**
 * 新規作成・編集・コピー画面へ遷移
 */
function gotoPageTwo() {

    // 新規作成・編集・コピー画面のバリエーションエラーをリセットする
    $("#setmailform").validate().resetForm();
    $("div.a-email-template-editor").removeClass("error-border");

    modeFlg = 0;

    // 新規作成・編集・コピー画面へ遷移
    var pageTwo = $("#setmailarea")[0].__component;
    pageTwo.opened = !0;
}

/**
 * 編集画面・コピー画面の送信To、CC、BCC及びその他送信先の設定
 */
function setEditPageEmail(email, otherEmail, id) {

    // 送信先To
    var emailTo = [];
    if (email !== null && email !== "") {
        emailTo = getDataList(email);
    }
    $("#" + id).find("input[name=hidden_" + id + "]").each(function () {
        if (emailTo.includes($(this).val())) {
            $(this).parents("label.a-checkbox").find("input[type=checkbox]").prop("checked", true);
        } else {
            $(this).parents("label.a-checkbox").find("input[type=checkbox]").prop("checked", false);
        }
    })

    // その他の送信先
    var emailToOther = [];
    if (otherEmail !== null && otherEmail !== "") {
        emailToOther = getDataList(otherEmail);
    }
    var cnt = 0;
    $("#" + id + "_other").find("input[type=email]").each(function () {

        if (emailToOther.length > cnt) {
            $(this).val(emailToOther[cnt]);
        } else {
            $(this).val("");
            if (cnt > 1) {
                $(this).parents("li.m-recipient-input__item").remove();
            }
        }
        cnt++;
    })
    for (var i = cnt; i < emailToOther.length; i++) {
        $("#" + id + "_other").next().find("button").click();
        $("#" + id + "_other").find("li").last().find("input[type=email]").val(emailToOther[i]);
    }
}

/**
 * 区分取得
 */
function getChildrenKbnList(childrenKbnObj, parentKbn, cnt) {

    var childrenKbnList = [];
    for (var i = 0; i < childrenKbnObj.length; i++) {
        if (childrenKbnObj[i][MT_KBN_KEYVALUE].slice(0, cnt) === parentKbn) {
            childrenKbnList.push(childrenKbnObj[i]);
        }
    }
    return childrenKbnList;
}

/**
 * 入力チェックルール追加
 */
function addRules(rulesObj) {
    for (var item in rulesObj) {
        if (item.indexOf("[]") === -1) {
            $('#' + item).rules('add', rulesObj[item]);
        }
        else {
            var itemArray = item.substring(0, item.length - 2);
            $("input[name^=" + itemArray + "]").rules('add', rulesObj[item]);
        }
    }
}

/**
 * 入力チェックルール削除
 */
function removeRules(rulesObj) {
    for (var item in rulesObj) {
        if (item.indexOf("[]") === -1) {
            $('#' + item).rules('remove');
        }
        else {
            var itemArray = item.substring(0, item.length - 2);
            $("input[name^=" + itemArray + "]").rules('remove');
        }
    }
}

/**
 * カンマで区切った文字列を配列にする
 */
function getDataList(data) {
    return data.split(",").filter(function (value) {
        return value !== null && $.trim(value) !== "";
    });
}
/*▲関数定義エリア▲*/