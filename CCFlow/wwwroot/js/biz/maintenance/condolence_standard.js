/*▼グローバル変数定義エリア▼*/
var wfCommon = new wfCommon();                       // 共通関連

//…追加変数

/*▲グローバル関数定義エリア▲*/

/*▼画面起動エリア▼*/
$(function () {

  //画面初期化
  InitPage();

  //…追加関数			

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

  // ラジオボタン
  wfCommon.radiosSetVal('livingliving', '0', LIVE_DOKYO);

  // イベント定義
  createonevent();

  // 入力チェックを設定する
  setinputcheck();

  // 日時項目をの設定
  setdateitems();
}

/**
 * イベント定義メソッド
 */
function createonevent() {

  // 下書き保存
  $(".cancel-confirm-button").click(function () {
    wfCommon.ShowDialog(DIALOG_CONFIRM, "下書きに保存しますか？", "終了する前に編集中の内容を保存できます。", null, testok, new Array("jjjjj"));
  });

  // 確認
  $("#input-form-check-button").click(function () {

    // チェックを実施する
    var flg = $("#form1").valid();
    if (!flg) {
      return;
    }

    //エラーがある場合、エラーの所に移動する
    var pageTwo = $("#pageTwo")[0].__component;
    pageTwo.opened = !0;
    pageTwo.onCloseRequested = function () {
      return pageTwo.opened = !1
    }
  });

  // 戻るイベント
  $("#btn_back").on("click", function () {
    $("#pageTwo")[0].__component.opened = !1;
  });

  // プルダウンの再検証
  //$("#something").change(function () {
  //  $("#form1").validate().element($("#something"));
  //});


  // alertサンプル
  $("#sample-alert").on("click", function () {
    wfCommon.ShowDialog(DIALOG_ALERT, "alertサンプル", "メッセージ内容：XXXXXXXXXXXXXXXXXX");
  });

  // confirmサンプル
  $("#sample-confirm").on("click", function () {
    wfCommon.ShowDialog(DIALOG_CONFIRM, "中止しますか？", "お申込手続きを終了します。 入力内容は保存されません。 よろしいでしょうか？ このページから移動してもよろしいですか？", null, cancelok);
  });

  // infoサンプル
  $("#sample-info").on("click", function () {
    wfCommon.ShowDialog(DIALOG_INFO, "否認の処理", "伝票番号：XXXXXXXXXXXXXXXXXX", null, cancelok);
  });

  // メイン画面表示
  window.onload = function () {

    // プルダウンリスト
    //wfCommon.initDropdown(true, dtKbn["JYUUGYOUINN_KBN"], "11", MT_KBN_KEYVALUE, MT_KBN_KEYNAME, "something1", "pulldown1");

    
  }
}

/**
 * 入力チェックを設定する
 */
function setinputcheck() {

  $("#form1").validate({
    focusCleanup: true,
    onkeyup: false,
    ignore: "",
    groups: {
      datetest999: "datetest002 datetest003"
    },
    rules: {
      tel: { required: true },
      email: { required: true },
      something: { required: true },
      livingliving: { required: true },
      address001: { required: true },
      datetest: { required: true },
      datetest002: { required: true },
      datetest003: { required: true },
      content: { required: true },
      sample_check: { required: true },
      upload_test1: { required: true },
    }
  });
}

/**
 * 日時項目を設定する
 */
function setdateitems() {
  wfCommon.setdatepickerWithStartEnd("#datetest", null, "YYYY/MM/DD", true, false);
}

function testok(aaaaa) {
  alert(aaaaa);
  return false;
}

function cancelok() {
  window.location.href = 'densin-sample-menu.html';
}
/*▲関数定義エリア▲*/