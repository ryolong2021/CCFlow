/*▼グローバル変数定義エリア▼*/
var webUser = null;
var wfCommon = new wfCommon();                       // 共通関連
var rules1;                                          // 画面チェックルール①
var rules2;                                          // 画面チェックルール②
var modeFlg;                                         // 確認ボタン押すかどうかを表すフラグ
var modeKbn;                                         // 新規作成と編集を区別する区分
var companyCode = null;
var companyName = null;
var dtKbn;

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
 * 区分リスト一覧の取得
 */
function InitPage() {

    // 区分リスト一覧の取得
    var objGetTbl = {};
    objGetTbl[0] = 'KBN';
    wfCommon.initGetData(objGetTbl, InitPageCallbak);
}

/**
 * 画面初期化
 */
function InitPageCallbak(data) {

    //区分マスタ格納
    setPullDown(data);

    // イベント定義
    createonevent();

    // 検索条件バーエリア非表示
    $("#search_condition").hide();

    // 入力チェックを設定する
    setinputcheck();

    modeFlg = INIT_MODE;
    modeKbn = "";


    // 送信アドレステキストボックスの変化を監視する
    watchTestEmailAddress();

    //$("ul.m-setting-menu__list").children("li").first().addClass("m-setting-menu__item--active");
    //$("ul.m-setting-menu__list").children("li").first().click();

    // 権限判定及び会社設定
    checkAuthority();
}

/**
 * ドロップダウン項目設定
 */
function setPullDown(data) {

    dtKbn = data['KBN'];

    // 絞り込み検索ダイアログ　ワークフロー
    wfCommon.initDropdown(true, dtKbn["WF_EMAIL_KBN"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "workflow_search", "workflow_search_p_div");

    // 新規作成画面　ワークフロー
    wfCommon.initDropdown(true, dtKbn["WF_EMAIL_KBN"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "workflow", "workflow_p_div");
}

/**
 * BS業務部所属かどうか判定
 */
function checkAuthority() {

    var ht = new HashTblCommon();

    // 社員番号
    ht.Add("SHAINBANGO", webUser.No);

    // 社員番号
    ht.Add("GroupKbn", "G1001");

    // APIでEBSからBS業務部所属判定を取得すること
    wfCommon.getApiInfoAjaxCallBack("Check_New_Authority", ht, editCompanyByAuthority);
}

/**
 * BS業務部なら、受託会社リストを取得し、それ以外の場合は、自社会社情報取得する
 */
function editCompanyByAuthority(data) {

    // BS業務部の場合
    if (data[0]["AUTHORITY_FLG"] > 0) {

        if (getCompanyList(setCompanyList) === null) {
            return null;
        }
    } else {
        if (getUserCompanyInfo() === null) {
            return null;
        }
        $(".company_search-div").hide();
    }  
}

/**
 * 受託会社一覧データ取得
 */
function getCompanyList(callback) {
    var ret = "";
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailStackholder");
    handler.AddPara("WF_NO", "009");
    handler.DoMethodSetString("GetCompanyList", function (data) {
        //例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return null;
        }
        if (data.length === 0) {
            wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0012"], "受託会社情報"));
            return null;
        }
        // JSON対象に転換
        ret = JSON.parse(data);
        callback(ret);
    });
}

/**
 * 受託会社ドロップダウンリストデータ設定
 */
function setCompanyList(companyInfo) {
    wfCommon.initDropdown(true, companyInfo, "", "CORP_CODE", "CORP_NAME", "company", "company_p_div");
    wfCommon.initDropdown(true, companyInfo, "", "CORP_CODE", "CORP_NAME", "company_search", "company_search_p_div");

    // 送信先タイプ
    wfCommon.initDropdown(true, dtKbn["SOUSINSAKI_TYPE"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sousinsaki_type", "sousinsaki_type_p_div");
    wfCommon.initDropdown(true, dtKbn["SOUSINSAKI_TYPE"], "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sousinsaki_type_search", "sousinsaki_type_search_p_div");

    // 一覧設定
    getTableListData(new HashTblCommon(), createEmailingTable);
}

/**
 * BS業務部以外のユーザの会社情報取得
 */
function getUserCompanyInfo() {

    var ht = new HashTblCommon();

    // 社員番号
    ht.Add("SHAINBANGO", webUser.No);

    // APIでEBSからユーザの会社情報取得すること
    wfCommon.getApiInfoAjaxCallBack("Get_New_Condolence_Shain_Info", ht, setUserCompanyInfo);
}

/**
 * BS業務部以外のユーザの会社情報取得　CallBack
 */
function setUserCompanyInfo(data) {
    if (data.length === 0) {
        wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0012"], "ユーザ" + webUser.No + "の会社情報"));
        return null;
    }

    companyCode = data[0]["KAISHACODE"];
    companyName = data[0]["KAISHANAME"];

    // 会社
    wfCommon.initDropdown(false, data, data[0]["KAISHACODE"], "KAISHACODE", "KAISHANAME", "company", "company_p_div");

    // 送信先タイプ
    var obj = dtKbn["SOUSINSAKI_TYPE"];
    var objAfter = [];
    for (var i = 0; i < obj.length; i++) {

        // 選択されている場合
        if (obj[i][MT_KBN_KEYVALUE].slice(3, 4) === "S") {
            objAfter.push(obj[i]);
        }
    }
    wfCommon.initDropdown(true, objAfter, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sousinsaki_type", "sousinsaki_type_p_div");
    wfCommon.initDropdown(true, objAfter, "", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "sousinsaki_type_search", "sousinsaki_type_search_p_div");

    $("span.a-nav-item__arrow").hide();
    $("#subnav-1").hide();

    // 一覧設定
    var ht = new HashTblCommon();
    ht.Add("KAISHACODE", companyCode);
    getTableListData(ht, createEmailingTable);
}

/**
 * イベント定義メソッド
 */
function createonevent() {

    // 絞り込みボタン押す
    $("button.a-search-button").on("click", function () {

        // 絞り込み検索ダイアログ画面を開く
        var pageSearch = $("#setting-search-modal")[0].__component;
        pageSearch.opened = !0;
        pageSearch.onCloseRequested = function () {
            return pageSearch.opened = !1
        }
    })

    // 絞り込み検索ダイアログ画面のクリアボタンを押す
    $("#setting-search-modal button.a-button--third").on("click", function () {

        // 検索条件クリア
        clearSearchCondition();
    })

    // 絞り込み検索ダイアログ画面の検索ボタンを押す
    $("#setting-search-modal button.a-button--primary").on("click", function () {

        // 検索処理をする
        var searchRet = searchMailingData();
        if (searchRet === null) {
            return;
        }

        // 絞り込みエリア設定
        var tagfilledStr = "";
        // グループ名
        if ($("#group_name_search").val() !== null && $("#group_name_search").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "グループ名：" + $("#group_name_search").val() + "</span>";
        }
        // ワークフロー
        if ($("#workflow_search").val() !== null && $("#workflow_search").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "ワークフロー：" + $("#workflow_search").text() + "</span>";
        }
        // 会社
        if ($("#company_search").val() !== null && $("#company_search").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "会社：" + $("#company_search").text() + "</span>";
        }
        // 送信先タイプ
        if ($("#sousinsaki_type_search").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "送信先タイプ：" + $("#sousinsaki_type_search").text() + "</span>";
        }
        // 送信先アドレス
        if ($("#email_content").val() !== null && $("#email_content").val() !== "") {

            tagfilledStr += "<span class=\"a-tag a-tag--filled\">" + "送信先アドレス：" + $("#email_content").val() + "</span>";
        }
        $("#search_condition .m-search-current-filters__tags").empty();
        if (tagfilledStr !== "") {
            $("#search_condition .m-search-current-filters__tags").append(tagfilledStr);
            $("#search_condition").show();
        } else {
            $("#search_condition").hide();
        }

        // 絞り込み検索ダイアログ画面を閉じる
        var pageSearch = $("#setting-search-modal")[0].__component;
        pageSearch.opened = !1;
    })

    // 検索条件バーエリアの条件クリアボタン押す
    $("button.m-search-current-filters__clear-button").on("click", function () {

        // 検索条件クリア
        clearSearchCondition();

        // 検索処理
        searchMailingData();

        // 検索条件バーエリア非表示
        $("#search_condition").hide();
    })

    // 新規作成ボタン押す
    $("#creat_new").click(function () {

        modeKbn = CREATE_MODE;

        // テキストボックスを初期化する
        $("#setmailingarea").find("input").each(function () {
            $(this).val("");
        })

        // チェックボックスを初期化する
        //$("#sousinsaki_type_p_div")[0].__component._choices.setChoiceByValue($("#workflow").val() + SOUSINSAKI_TYPE_SITE);
        $("#sousinsaki_type_p_div")[0].__component.reset();
        $("#workflow_p_div")[0].__component.reset();
        if (companyCode === null) {
            $("#company_p_div")[0].__component.reset();
        }

        // 各項目の制御
        $("#company_p_div")[0].__component.disable();
        $("input[type=email]").prop("disabled", true);
        $("#group_name").prop("disabled", false);
        $(".email-span-marck").hide();
        $(".company-span-marck").hide();
        removeRules(rules2);

        // 新規作成画面へ遷移
        gotoPageTwo();

        // 送信先タイプ変更する場合のイベント
        sousinsakiTypechangeEvent();
    });

    // 新規作成画面　確認ボタン押す
    $("#setmailingarea .a-button--primary").on("click", function () {

        modeFlg = CONFIRM_MODE;

        if ($("#sousinsaki_type").val().substr(3) === SOUSINSAKI_TYPE_SITE || $("#sousinsaki_type").val().substr(3, 1) === "S") {
            addRules(rules2);
        } else {
            removeRules(rules2);
        }

        // チェックを実施する
        var flg = $("#setmailingform").valid();

        if (!flg) {
            return;
        }

        // 登録処理
        saveInfo();
    })

    // 新規作成画面　キャンセルボタンや一覧へボタン押す
    $("#setmailingarea .back-to-list-page").on("click", function () {

        modeFlg = INIT_MODE;

        // 一覧画面へ戻る
        gotoPageOne();
    })

    // メイン画面へ遷移
    $("#goto_home").on("click", function () {
        window.location.href = '../menu/form_mainmenu.html';
    })
}

/**
 * 送信先タイプ変更イベント
 */
function sousinsakiTypechangeEvent() {

    // 送信先タイプ変更イベント
    $("#sousinsaki_type").on("change", function () {

        // 指定アドレス又は所轄人事の場合
        if ($("#sousinsaki_type").val().substr(3) === SOUSINSAKI_TYPE_SITE || $("#sousinsaki_type").val().substr(3, 1) === "S") {
            if (modeFlg === CONFIRM_MODE) {
                addRules(rules2);
                $("#setmailingform").validate().element($("input[name^=email_input]"));
            }
            $("input[type=email]").prop("disabled", false);
            $(".email-span-marck").show(); 
            // 上記以外の場合
        } else {
            if (modeFlg === CONFIRM_MODE) {
                removeRules(rules2);
                $("input[type=email]").each(function () {
                    if ($(this).hasClass("error")) {
                        $(this).removeClass("error");
                        $(this).next().hide();
                    }
                })
            }
            $("input[type=email]").prop("disabled", true);
            $(".email-span-marck").hide();  
        }

        // 指定アドレスと空欄以外の場合、グループ名が選択された送信先タイプで表示され、非活性にする
        if ($("#sousinsaki_type").val().substr(3) !== SOUSINSAKI_TYPE_SITE && $("#sousinsaki_type").val() !== "") {
            $("#group_name").val($("#sousinsaki_type").text());
            $("#group_name").prop("disabled", true);
        } else {
            //$("#group_name").val("");
            $("#group_name").prop("disabled", false);
        }

        // ユーザがBS部、且つ所轄人事の場合だけ、会社が活性
        if ($("#sousinsaki_type").val().substr(3, 1) !== "S") {
            $("#company_p_div")[0].__component.reset();
            $("#company_p_div")[0].__component.disable();
            $(".company-span-marck").hide();
        } else {
            if (companyCode === null) {
                $("#company_p_div")[0].__component.enable();
            } else {
                $("#company_p_div")[0].__component.disable();
            }
            $(".company-span-marck").show();
        }

        if (modeFlg === CONFIRM_MODE) {
            $("#setmailingform").validate().element($("#sousinsaki_type"));
            $("#setmailingform").validate().element($("#company"));
        }
    })

    // ワークフロー変更イベント
    $("#workflow").on("change", function () {
        if (modeFlg === CONFIRM_MODE) {
            $("#setmailingform").validate().element($("#workflow"));
        }
    })

    // 会社変更イベント
    $("#company").on("change", function () {
        if (modeFlg === CONFIRM_MODE) {
            $("#setmailingform").validate().element($("#company"));
        }
    })

    // グループ名変更イベント
    $("#group_name").on('DOMSubtreeModified propertychange', function () {
        if (modeFlg === CONFIRM_MODE && $("#group_name").val() !== "") {
            $("#setmailingform").validate().element($("#group_name"));
        }
    })
}

/**
 * 検索条件クリア
 */
function clearSearchCondition() {

    // 送信先タイプ
    $("#setting-search-modal").find("input[type=text],textarea").each(function () {
        $(this).val("");
    });

    // 送信先タイプ（絞り込み検索用）
    $("#sousinsaki_type_search_p_div")[0].__component.reset();
    // ワークフロー（絞り込み検索用）
    $("#workflow_search_p_div")[0].__component.reset();
    // 会社（絞り込み検索用）
    if (companyCode === null) {
        $("#company_search_p_div")[0].__component.reset();
    }
}

/**
 * 保存処理
 */
function saveInfo() {

    // 登録対象の追加
    var ht = new HashTblCommon();

    // 送信先タイプ
    ht.Add('Sousinsaki_Type', $("#sousinsaki_type").val());
    // グループ名
    ht.Add('Rnrakusaki_Name', $("#group_name").val());
    
    var emailAddress = "";
    if ($("#sousinsaki_type").val().substr(3) === SOUSINSAKI_TYPE_SITE || $("#sousinsaki_type").val().substr(3, 1) === "S") {
        $("input[type=email]").each(function () {

            if ($(this).val() !== "" && $(this).val() !== null) {
                emailAddress += $(this).val() + ",";
            }
        })
        
        if ($("#sousinsaki_type").val().substr(3, 1) === "S") {
            // 会社
            ht.Add('KAISHACODE', $("#company").val());
            ht.Add('KAISHANAME', $("#company").text());
            // 可変区分
            ht.Add('KAHEN_FLG', KAHEN_FLG_KAHEN);
        } else {
            // 会社
            ht.Add('KAISHACODE', "");
            ht.Add('KAISHANAME', "");
            // 可変区分
            ht.Add('KAHEN_FLG', KAHEN_FLG_KOTEI);
        }
    } else {
        // 可変区分
        ht.Add('KAHEN_FLG', KAHEN_FLG_KAHEN);
        // 会社
        ht.Add('KAISHACODE', "");
        ht.Add('KAISHANAME', "");
    }

    // ワークフロー
    ht.Add('WF_NO', $("#workflow").val());

    // メール宛先
    var emailAddressAfter = "";
    if (emailAddress !== "") {
        emailAddressAfter = emailAddress.substring(0, emailAddress.length - 1);
    }
    ht.Add('EMAIL', emailAddressAfter);

    // 最終更新者
    ht.Add("REC_EDT_USER", webUser.No);
    // 最終更新者漢字名
    ht.Add("REC_EDT_USER_KANJI", webUser.Name);

    var method = "";
    var para = "";
    if (modeKbn === CREATE_MODE) {

        // 登録者
        ht.Add("REC_ENT_USER", webUser.No);
        // 登録者漢字名
        ht.Add("REC_ENT_USER_KANJI", webUser.Name);
        
        method = "SaveEmailStakeholderData";
        para = "InsertData";
    } else {
        method = "UpdateEmailStakeholder";
        para = "EmailStokeholderReq";
        // グループID
        ht.Add("Rnrakusaki_Id", $("#rnrakusaki_id").val());
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailStackholder");
    handler.AddPara(para, ht.stringify());
    var data = handler.DoMethodReturnString(method);

    // データ存在している場合の処理
    if (data.indexOf('msg@') === 0) {
        data = JSON.parse(data.split("@")[1]);
        var key = data["key"];
        var value = data["value"];
        wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg[key], value), null);
        return;
    }

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 検索処理
    searchMailingData();

    // 一覧画面へ遷移する
    gotoPageOne();

    wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0005"], "保存"));
}

/**
 * 一覧画面へ戻る処理
 */
function gotoPageOne() {

    // 一覧画面へ戻る
    var pageTwo = $("#setmailingarea")[0].__component;
    pageTwo.opened = !1;
}

/**
 * 入力チェックを設定する（新規作成画面）
 */
function setinputcheck() {

    rules1 = {
        // グループ名
        group_name: {
            required: true
        },
        // 送信先タイプ
        sousinsaki_type: {
            required: true
        },
        // ワークフロー
        workflow: {
            required: true
        },
        // 会社
        company: {
            required: function () {
                return $("#sousinsaki_type").val() !== null && $("#sousinsaki_type").val().substr(3, 1) === "S";
            }
        }
    };
    rules2 = {};
    var require_group = {
        require_from_group: [1, '.input-group-email']
    };
    // テスト送信アドレス
    for (var i = 1; i <= 100; i++) {
        rules1["email_input" + i] = require_group;
        rules2["email_input" + i] = require_group;
    }

    $("#setmailingform").validate({
        focusCleanup: true,
        onkeyup: false,
        ignore: "",
        rules: rules1,// エラーエリアを設定する
        errorPlacement: function (error, element) {
            if (element.is("input[type=email]")) {
                error.insertAfter(element.parents("div.a-text-field").find("input[type=email]"));
            } else if (element.is("select")) {
                error.insertAfter(element.parents("div.a-pulldown"));
            } else {
                error.insertAfter(element);
            }
        }
    });
}

/**
 * 一覧データ取得
 */
function getTableListData(ht, callback) {
    var ret = "";
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailStackholder");
    handler.AddPara("EmailStokeholderReq", ht.stringify());
    handler.DoMethodSetString("GetEmailStakeholderList", function (data) {
        //例外処理
        if (data.indexOf('err@') === 0) {
            wfCommon.Msgbox(data);
            return null;
        }
        // JSON対象に転換
        ret = JSON.parse(data);
        callback(ret);
    });
}

/**
 * 一覧作成（検索ボタン押す）
 */
function searchMailingData() {

    // 一覧取得
    var ht = new HashTblCommon();
    // グループ名
    ht.Add("Rnrakusaki_Name", $("#group_name_search").val());
    // 送信先タイプ
    ht.Add("Sousinsaki_Type", $("#sousinsaki_type_search").val());
    // ワークフロー
    ht.Add("WF_NO", $("#workflow_search").val());
    // 会社コード
    if (companyCode === null) {
        ht.Add("KAISHACODE", $("#company_search").val());
    } else {
        ht.Add("KAISHACODE", companyCode);
    }
    // 送信先アドレス
    var email = [];
    if ($("#email_content").val() !== null && $.trim($("#email_content").val()) !== "") {
        email = getDataList($.trim($("#email_content").val()));
    }

    ht.Add("EmailList", email);
    // テーブル設定
    getTableListData(ht, createEmailingTable);
}

/**
 * 送信アドレステキストボックスの変化を監視するメソッド
 */
function watchTestEmailAddress() {

    var targetNode = document.querySelector('#email_ul');

    var config = { attributes: true, childList: true, characterData: true, subtree: true };

    var callback = function (mutationsList) {
        for (var i = 0; i < mutationsList.length; i++) {
            if (mutationsList[i].type === 'childList') {

                if (mutationsList[i].addedNodes.length > 0
                    && mutationsList[i].addedNodes[0].classList
                    && mutationsList[i].addedNodes[0].classList.value === "m-recipient-input__item") {
                    $('#email_ul').find("input[type=email]").each(function (index, element) {
                        $(element).attr("id", "email_input" + (index + 1));
                        $(element).attr("name", "email_input" + (index + 1));
                        $(element).addClass("input-group-email");
                        if ($("#sousinsaki_type").val().substr(3) === SOUSINSAKI_TYPE_SITE
                            || $("#sousinsaki_type").val().substr(3, 1) === "S") {
                            $(element).prop("disabled", false);
                        } else {
                            $(element).prop("disabled", true);
                        }
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
 * 一覧作成メソッド
 */
function createEmailingTable(jsondata) {

    var dateHandle = function (res) {
        var result = "";
        if (res.REC_EDT_DATE !== null) {
            result = moment(res).format(DATETIME_FORMAT_MOMENT_PATTERN_1) + "<br>更新者：" +res.REC_EDT_USER_KANJI;
        }
        return result;
    }

    var emailHandle = function (res) {

        var rnrakusakiStr = "";
        if (res.EMAIL !== null && res.EMAIL !== "") {
            var match = res.EMAIL.match(/,/g);
            if (match === null || match.length < 3) {
                rnrakusakiStr += res.EMAIL;
            } else {
                var emailList = getDataList(res.EMAIL);
                for (var i = 0; i < 3; i++) {
                    rnrakusakiStr += emailList[i] + ",";
                }
                rnrakusakiStr += "他" + (emailList.length - 3) + "件";
            }
        }
        if (rnrakusakiStr === "") {
            rnrakusakiStr = "-";
        }
        return rnrakusakiStr;
    }

    var columns = [
        {
            field: 'RNRAKUSAKI_ID',
            title: 'ID',
            modify_type_class: 'type-id',
        }, {
            field: 'RNRAKUSAKI_NAME', 	
            title: 'グループ名', 		
            modify_type_class: 'type-data', 
        }, {
            field: 'WF_NO_NAME',
            title: 'ワークフロー',
            modify_type_class: 'type-data',
        }, {
            field: 'KAISHANAME',
            title: '会社',
            modify_type_class: 'type-data',
        }, {
            field: 'SOUSINSAKI_TYPE_NAME',
            title: '送信先タイプ',
            modify_type_class: 'type-data',
        }, {
            field: 'EMAIL',
            title: '送信先アドレス',
            modify_type_class: 'type-data',
            formatter: emailHandle
        }, {
            field: 'REC_EDT_DATE',
            title: '更新日時',
            modify_type_class: 'type-data',
            formatter: dateHandle
        }, 
        {
            field: 'ActionEvent',
                modify_type_class: 'type-action', 
            show_class: "2", 
            title_action: [
                {
                    title: '編集',
                    onClick: function (event, index, dataList, data) {

                        modeKbn = EDIT_MODE;
                        modeFlg = INIT_MODE;
                        recordEdit(data);
                    }
                },  {
                    title: '削除',
                    onClick: function (event, index, dataList, data) {

                        var para = [];
                        para.push(data.RNRAKUSAKI_ID);
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
        norecordmsg: msg["I0002"]
    };

    // 一覧の作成		
    $("#table").createTable(optionData);
}

/**
 * 削除ボタン押す時の処理
 */
function recordDel(id) {

    // 削除処理
    var handler = new HttpHandler("BP.WF.HttpHandler.Mn_EmailStackholder");
    handler.AddPara("KEY", id);
    var data = handler.DoMethodReturnString("DeleteEmailStakeholder");

    // 例外処理
    if (data.indexOf('err@') === 0) {
        wfCommon.Msgbox(data);
        return;
    }

    // 検索処理
    searchMailingData();

    wfCommon.ShowDialog(DIALOG_ALERT, wfCommon.MsgFormat(msg["I0001"], "削除,成功"));
}

/**
 * 編集ボタン押す時の処理
 */
function recordEdit(rowData) {

    // 送信先タイプ
    $("#sousinsaki_type_p_div")[0].__component._choices.setChoiceByValue(rowData.SOUSINSAKI_TYPE);
    // ワークフロー
    $("#workflow_p_div")[0].__component._choices.setChoiceByValue(rowData.WF_NO);
    // 会社
    if (rowData.KAISHACODE !== null && rowData.KAISHACODE !== "") {
        $("#company_p_div")[0].__component._choices.setChoiceByValue(rowData.KAISHACODE);
    } else {
        $("#company_p_div")[0].__component.reset();
    }
    // グループ名
    $("#rnrakusaki_id").val(rowData.RNRAKUSAKI_ID);
    $("#group_name").val(rowData.RNRAKUSAKI_NAME);
    // 送信先アドレス
    var emailList = [];
    if (rowData.EMAIL !== null && rowData.EMAIL !== "") {
        emailList = getDataList(rowData.EMAIL);
    }
    var cnt = 0;
    $("#email_ul").find("input[type=email]").each(function () {

        if (emailList.length > cnt) {
            $(this).val(emailList[cnt]);
        } else {
            $(this).val("");
            if (cnt > 1) {
                $(this).parents("li.m-recipient-input__item").remove();
            }
        }
        cnt++;
    })
    for (var i = cnt; i < emailList.length; i++) {
        $(".a-add-item-button").click();
        $("#email_ul").find("li").last().find("input[type=email]").val(emailList[i]);
    }

    if (rowData.SOUSINSAKI_TYPE.substr(3) === SOUSINSAKI_TYPE_SITE || rowData.SOUSINSAKI_TYPE.substr(3, 1) === "S") {
        $("input[type=email]").prop("disabled", false);
        $(".email-span-marck").show();

        if (rowData.SOUSINSAKI_TYPE.substr(3, 1) === "S") {
            if (companyCode === null) {
                $("#company_p_div")[0].__component.enable();
            } else {
                $("#company_p_div")[0].__component.disable();
            }
            $(".company-span-marck").show();
        } else {
            $("#company_p_div")[0].__component.disable();
            $(".company-span-marck").hide();
        }
    } else {
        $("input[type=email]").prop("disabled", true);
        $(".email-span-marck").hide();
        $("#company_p_div")[0].__component.disable();
        $(".company-span-marck").hide();
    }

    // 送信先タイプ変更する場合のイベント
    sousinsakiTypechangeEvent();

    // 新規作成画面へ遷移
    gotoPageTwo();
}

/**
 * 新規作成・編集画面へ遷移
 */
function gotoPageTwo() {

    // 新規作成・編集画面のバリエーションエラーをリセットする
    $("#setmailingform").validate().resetForm();
    $("input[type=email]").removeClass("error");

    modeFlg = INIT_MODE;
    //addRules(rules2);

    // 新規作成・編集・コピー画面へ遷移
    var pageTwo = $("#setmailingarea")[0].__component;
    pageTwo.opened = !0;
}

/**
 * 入力チェックルール追加
 */
function addRules(rulesObj) {
    for (var item in rulesObj) {
        $('#' + item).rules('add', rulesObj[item]);
    }
}

/**
 * 入力チェックルール削除
 */
function removeRules(rulesObj) {
    for (var item in rulesObj) {
        $('#' + item).rules('remove');
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