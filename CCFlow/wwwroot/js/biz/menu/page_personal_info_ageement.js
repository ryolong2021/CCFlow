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

    // イベント設定
    createEvent();
}

/**
 * イベント定義
 */
function createEvent() {

    // 同意しないボタン押下
    $("#btn_disagree").on("click", function () {
        location.href = "../../../login.html?DoType=Out";
    });

    // 同意するボタン押下
    $("#btn_agree").on("click", function () {
        window.location.href = '../menu/form_requestlist.html';
    });
}
/*▲関数定義エリア▲*/