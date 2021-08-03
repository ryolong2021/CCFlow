//…追加変数
var wfCommon = new wfCommon();                       // 共通関連
var usrLocal;
/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {

  if (webUser == null)
    webUser = new WebUser();
  if (webUser.No == null)
    return;

  // 権限より、メニューの表示／非表示の制御
  doMenusDisplay();

  // 登録者名前の設定
  aisatuInfo();

  // ログアウトボタンのリンクの設定
  $(".o-global-navigation__logout-button").on("click", function () {
    location.href = "/?DoType=Out";
  });

  // カレントページのメニューをアクティブ化にする
  menuActive();

  // バッジ情報取得
  usrLocal =wfCommon.getBadgeLocalStorage();

  // 左メニューバッジの設定
  setSelfBadge();
  setApplyBadge();

});
/*▲画面起動エリア▲*/

/*▼関数定義エリア▼*/
/**
 * カレントページのメニューをアクティブ化にする
 *
 */
function menuActive() {
  // ページファイル名を取得
  var path = window.location.pathname;
  var page = path.split("/").pop();
  //URLから「#」記号後ろの変数を取得
  var hashID = document.location.hash.split("#")[1];
  var subNav = "form_subnav.html" === page;
  // var subMaintains = ""  // 未定

  if (subNav) {
    //労務メニューのアクティブ化
    var subNavEL = document.querySelector("div.a-nav-item.a-nav-item--labor-menu.a-nav-item--has-subnav.K004");
    subNavEL.classList.add("a-nav-item--opened");
    var selectChild = document.querySelector("div." + hashID);
    selectChild.classList.add("a-nav-item--selected");
  }
  else {
    // 自分の申請を見る
    if ("form_requestlist.html" === page) {
      var navItemELs = document.querySelectorAll("div.a-nav-item.a-nav-item--request-list");
      navItemELs.forEach(function(navItemEL) {
        navItemEL.classList.add("a-nav-item--selected");
        var selectChild = document.querySelector("div.a-nav-icon.a-nav-icon--request-list.a-nav-item__icon");
        selectChild.classList.add("a-nav-icon--active");
        var selectChildChild = document.querySelector("i.a-icon.a-icon--request-list");
        selectChildChild.classList.remove("a-icon--request-list");
        selectChildChild.classList.add("a-icon--request-list-selected");
      });
    }
    // 承認依頼を見る
    if ("form_approval_request_list.html" === page) {
      var navItemELs = document.querySelectorAll("div.a-nav-item.a-nav-item--approval-list");
      navItemELs.forEach(function(navItemEL) {
        navItemEL.classList.add("a-nav-item--selected");
        var selectChild = document.querySelector("div.a-nav-icon.a-nav-icon--approval-list.a-nav-item__icon");
        selectChild.classList.add("a-nav-icon--active");
        var selectChildChild = document.querySelector("i.a-icon.a-icon--approval-list");
        selectChildChild.classList.remove("a-icon--approval-list");
        selectChildChild.classList.add("a-icon--approval-list-selected");
      });
    }
  }
}

/**
 * 権限より、メニューの表示／非表示の制御
 *
 */
function doMenusDisplay() {

  // ユーザー情報の取得
  var webUser = new WebUser();

  // 権限情報の取得
  var handler = new HttpHandler("BP.WF.HttpHandler.Mn_Mainmenu");
  handler.AddPara("ShainBango", webUser.No);
  var data = handler.DoMethodReturnString("getMenusDisplayInfo");

  // 例外処理
  if (data.indexOf('err@') === 0) {
    return;
  }

  // 取得データの解析
  var data = JSON.parse(data);

  // データサイズの取得
  var size = data.length;
  if (size === 0) {
    return;
  }

  // メニューの制御
  for (var i = 0; i < size; i++) {
    var funCode = data[i].FUNCTION_CODE;
    if (funCode.indexOf(FUNCTION_CODE_MAINTENANCE) === 0) {
      $('.' + FUNCTION_CODE_MAINTENANCE).show();
    }
    if (funCode.indexOf(FUNCTION_CODE_ROMU_MENU) === 0) {
      $('.' + FUNCTION_CODE_ROMU_MENU).show();
    }
    if (funCode.indexOf(FUNCTION_CODE_MAINTENANCE_MAIL) === 0) {
      $('.' + FUNCTION_CODE_MAINTENANCE_MAIL).show();
    }
    
    $('.' + funCode).show();
  }
}

/**
  * 登録者名前の設定
  *
  */
function aisatuInfo() {
  $(".a-nav-item__user-name").html(sessionStorage.getItem("userName"));
}

/**
 * 自分の申請を見るバッジの設定
 */
function setSelfBadge() {

    // localstorageから取得
    if ((usrLocal[GET_MY_DIFFERENCE]["display"] != undefined && usrLocal[GET_MY_DIFFERENCE]["display"] != 0) ||
        (usrLocal[GET_MY_COMPLETE]["display"] != undefined && usrLocal[GET_MY_COMPLETE]["display"] != 0)) {
        $(".a-nav-self").addClass("a-nav-icon__badge");
        return;
    }

    // 取得出来ない場合
    var type = GET_MY_DIFFERENCE + ';' + GET_MY_COMPLETE + ';' + GET_MY_DRAFT;
    wfCommon.GetDensinList(type, setSelfBadgeCallback);
}

/**
 * 自分の申請を見るバッジの設定(callback)
 */
function setSelfBadgeCallback(data) {

    var selfbadge = false;

    // 差戻
    $.each(data[0], function (dataIndex, dataVal) {
        if (moment().diff(moment(dataVal.SendDT), 'days') <= 7 &&
            (usrLocal[GET_MY_DIFFERENCE][dataVal.WorkID] == undefined || usrLocal[GET_MY_DIFFERENCE][dataVal.WorkID] == "1")) {
            selfbadge = true;
            return false;
        }
    });

    // 完了
    if (!selfbadge) {
        $.each(data[1], function (dataIndex, dataVal) {
            if (moment().diff(moment(dataVal.SendDT), 'days') <= 7 &&
                (usrLocal[GET_MY_COMPLETE][dataVal.WorkID] == undefined || usrLocal[GET_MY_COMPLETE][dataVal.WorkID] == "1")) {
                selfbadge = true;
                return false;
            }
        });
    }

    // 下書き
    if (!selfbadge) {
        $.each(data[1], function (dataIndex, dataVal) {
            if (moment().diff(moment(dataVal.SendDT), 'days') <= 7 &&
                ((usrLocal[GET_MY_DRAFT][dataVal.WorkID] == undefined && checkAutoCreate(dataVal.FK_Flow)) || usrLocal[GET_MY_DRAFT][dataVal.WorkID] == "1")) {
                selfbadge = true;
                return false;
            }
        });
    }

    // バッジ表示
    if (selfbadge) {
        $(".a-nav-self").addClass("a-nav-icon__badge");
    }
}

/**
 * 承認依頼を見るバッジの設定
 */
function setApplyBadge() {

    // localstorageから取得
    if ((usrLocal[GET_APPROVAL_COMPLETE]["display"] != undefined && usrLocal[GET_APPROVAL_COMPLETE]["display"] != 0) ||
        (usrLocal[GET_APPROVAL_INPROCESS]["display"] != undefined && usrLocal[GET_APPROVAL_INPROCESS]["display"] != 0)) {
        $(".a-nav-apply").addClass("a-nav-icon__badge");
        return;
    }

    // 取得出来ない場合
    var type = GET_APPROVAL_COMPLETE + ';' + GET_APPROVAL_INPROCESS;
    wfCommon.GetDensinList(type, setApplyBadgeCallback);
}

/**
 * 自分の申請を見るバッジの設定(callback)
 */
function setApplyBadgeCallback(data) {

    var appbadge = false;

    // 完了
    $.each(data[0], function (dataIndex, dataVal) {
        if (moment().diff(moment(dataVal.SendDT), 'days') <= 7 &&
            (usrLocal[GET_APPROVAL_COMPLETE][dataVal.WorkID] == undefined || usrLocal[GET_APPROVAL_COMPLETE][dataVal.WorkID] == "1")) {
            appbadge = true;
            return false;
        }
    });

    // 処理待ち
    if (!appbadge) {
        $.each(data[1], function (dataIndex, dataVal) {
            if (moment().diff(moment(dataVal.SendDT), 'days') <= 7 &&
                (usrLocal[GET_APPROVAL_INPROCESS][dataVal.WorkID] == undefined || usrLocal[GET_APPROVAL_INPROCESS][dataVal.WorkID] == "1")) {
                appbadge = true;
                return false;
            }
        });
    }

    // バッジ表示
    if (appbadge) {
        $(".a-nav-apply").addClass("a-nav-icon__badge");
    }
}

/**
 * すべて確認済
 */
function clearAllBadges(tabflg, sectionId) {

    // バッジの削除
    $(sectionId + " i").removeClass("a-status-info__badge")

    // 通知欄の削除
    $(sectionId).parent().prev().attr("hidden", "");

    // タブ／メニューバッジの削除
    switch (tabflg) {
        case GET_MY_DRAFT:
            $("#a-tab-draft").removeClass("a-tab-nav-item__badge");
            if ((usrLocal[GET_MY_COMPLETE]["display"] == 0 || usrLocal[GET_MY_COMPLETE]["display"] == undefined) &&
                (usrLocal[GET_MY_DIFFERENCE]["display"] == 0 || usrLocal[GET_MY_DIFFERENCE]["display"] == undefined)) {
                $(".a-nav-self").removeClass("a-nav-icon__badge");
            }
            break;
        case GET_MY_DIFFERENCE:
            $("#a-tab-diff").removeClass("a-tab-nav-item__badge");
            if ((usrLocal[GET_MY_COMPLETE]["display"] == 0 || usrLocal[GET_MY_COMPLETE]["display"] == undefined) &&
                (usrLocal[GET_MY_DRAFT]["display"] == 0 || usrLocal[GET_MY_DRAFT]["display"] == undefined)) {
                $(".a-nav-self").removeClass("a-nav-icon__badge");
            }
            break;
        case GET_MY_COMPLETE:
            $("#a-tab-done").removeClass("a-tab-nav-item__badge");
            if ((usrLocal[GET_MY_DIFFERENCE]["display"] == 0 || usrLocal[GET_MY_DIFFERENCE]["display"] == undefined) &&
                (usrLocal[GET_MY_DRAFT]["display"] == 0 || usrLocal[GET_MY_DRAFT]["display"] == undefined)) {
                $(".a-nav-self").removeClass("a-nav-icon__badge");
            }
            break;
        case GET_APPROVAL_INPROCESS:
            $("#a-tab-wait").removeClass("a-tab-nav-item__badge");
            if (usrLocal[GET_APPROVAL_COMPLETE]["display"] == 0 || usrLocal[GET_APPROVAL_COMPLETE]["display"] == undefined) {
                $(".a-nav-apply").removeClass("a-nav-icon__badge");
            }
            break;
        case GET_APPROVAL_COMPLETE:
            $("#a-tab-done").removeClass("a-tab-nav-item__badge");
            if (usrLocal[GET_APPROVAL_INPROCESS]["display"] == 0 || usrLocal[GET_APPROVAL_INPROCESS]["display"] == undefined) {
                $(".a-nav-apply").removeClass("a-nav-icon__badge");
            }
            break;
    }

    // localstorageの更新
    $.each(usrLocal[tabflg], function (dataIndex, dataVal) {
        usrLocal[tabflg][dataIndex] = 0;
    });
    localStorage.setItem(webUser.No, JSON.stringify(usrLocal));
}

/**
 * すべて確認済
 */
function checkAutoCreate(flowNo) {

    var rel = false;

    $.each(AUTO_CREATE_FLOW_NO, function (dataIndex, dataVal) {
        if (flowNo == dataVal) {
            rel = true;
            return false;
        }
    });

    return rel;
}
/*▲関数定義エリア▲*/