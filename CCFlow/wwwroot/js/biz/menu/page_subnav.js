document.addEventListener("readystatechange", function (event) {
  //DOMロードした後、タブの表示非表示処理を行う
  if (event.target.readyState === "interactive") {
    //権限より、タブの表示／非表示の制御
    doTabsDisplay();
  }
  //JS全部読み込み後、下記の処理を行う
  if (event.target.readyState === "complete") {
    doTabChange();
  }
});

//URLから「#」記号後ろの変数が変わった時、下記処理を行う
window.addEventListener("hashchange", doTabChange, false);

/**
 * タブの切り替え
 */
function doTabChange() {
  //m-tabsエレメント取得
  var tabEl = document.querySelector(".m-tabs");
  //m-tabs__tabエレメント取得
  var tabsEl = document.querySelectorAll(".m-tabs__tab");
  //m-tabs__contentエレメント取得
  var tabsContentEl = document.querySelectorAll(".m-tabs__content");
  //URLから「#」記号後ろの変数を取得
  var TabID = document.location.hash.split("#")[1];
  //URLから「#」記号後ろの変数より、エレメントを取得
  var child = document.getElementById(TabID);
  //親エレメントを取得
  var parent = child.parentNode;
  //m-tabsエレメントのインデックス設定すべき値を取得
  var NumID = Array.prototype.indexOf.call(parent.children, child);

  if (TabID) {
    //m-tabsエレメントのインデックス値を設定
    tabEl.style.setProperty("--active-index", NumID);

    //タブの切り替え表示スタイルを設定
    tabsEl.forEach(function (el) {
      if (TabID === el.id) {
        el.classList.add("m-tabs__tab--active");
        el.querySelector(".a-tab-nav-item").classList.add("a-tab-nav-item--active");
      } else {
        el.classList.remove("m-tabs__tab--active");
        el.querySelector(".a-tab-nav-item").classList.remove("a-tab-nav-item--active");
      }
    });

    //タブコンテンツの切り替え表示スタイルを設定
    tabsContentEl.forEach(function (el) {
      if ("content_" + TabID === el.id) {
        el.classList.add("m-tabs__content--active");
      } else {
        el.classList.remove("m-tabs__content--active");
      }
    });
  }

  //労務メニューのアクティブ化
  var subNavEL = document.querySelector("div.a-nav-item.a-nav-item--labor-menu.a-nav-item--has-subnav.K004");
  subNavEL.classList.add("a-nav-item--opened");
  var nonSelectedELs = document.querySelectorAll("div.a-nav-item.a-nav-item--selected.a-nav-item--subnav");
  nonSelectedELs.forEach(function (el) {
    el.classList.remove("a-nav-item--selected");
  });
  var selectChild = document.querySelector("div." + TabID);
  selectChild.classList.add("a-nav-item--selected");

  //画面のスクロールの設定
  window.scrollTo(0, 0);
}

/**
 * 権限より、タブの表示／非表示の制御
 */
function doTabsDisplay() {

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

  //m-tabs__tabエレメント取得
  var tabsEl = document.querySelectorAll(".m-tabs__tab");
  //m-tabs__contentエレメント取得
  var tabContentsEl = document.querySelectorAll(".m-tabs__content");
  //削除されるタブリスト変数の定義
  var tabDeleteList = [];
  //削除されるタブコンテンツリスト変数の定義
  var tabContentDeleteList = [];
  //削除されるタブの検索
  tabsEl.forEach(function (element) {
    //該当エレメントを取得して、IDとエレメント対象をArrayに一時保存
    var ind = [element.id, element];
    //存在しないフラグ値を定義し、true値を初期化する
    var nothas = true;
    //バックエンドから取得した権限リスト中に、該当エレメントIDをチェックする
    for (var i = 0; i < size; i++) {
      var funCode = data[i].FUNCTION_CODE;
      //存在する場合、存在しないフラグ値をfalseで設定する
      if (funCode === ind[0]) {
        nothas = false;
        //無駄なループを避けるように、for構文から飛ばす
        break;
      }
    }
    //存在しない場合、要らないエレメントを削除されるタブリストにプッシュする
    if (nothas) {
      tabDeleteList.push(ind[1]);
    }
  });

  //削除されるタブコンテンツの検索
  tabContentsEl.forEach(function (element) {
    //該当エレメントを取得して、IDとエレメント対象をArrayに一時保存
    var ind = [element.id, element];
    //存在しないフラグ値を定義し、true値を初期化する
    var nothas = true;
    //バックエンドから取得した権限リスト中に、該当エレメントIDをチェックする
    for (var i = 0; i < size; i++) {
      var funCode = "content_" + data[i].FUNCTION_CODE;
      //存在する場合、存在しないフラグ値をfalseで設定する
      if (funCode === ind[0]) {
        nothas = false;
        //無駄なループを避けるように、for構文から飛ばす
        break;
      }
    }
    //存在しない場合、要らないエレメントを削除されるタブコンテンツリストにプッシュする
    if (nothas) {
      tabContentDeleteList.push(ind[1]);
    }
  });

  //要らないタブエレメントを削除
  tabDeleteList.forEach(function (element) {
    element.remove();
  });
  //要らないタブコンテンツエレメントを削除
  tabContentDeleteList.forEach(function (element) {
    element.remove();
  });
}