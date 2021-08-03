/*▼画面起動エリア▼*/
(function ($) {

    $.fn.extend({

        "InitPageApprovalRoot": function (oid) {

            // 画面初期化
            InitPageRootArea(oid);
        }
    });

    /*▼関数定義エリア▼*/
    /**
     * 画面初期化
     */
    function InitPageRootArea(oid) {

        // フローシステムデータの取得
        wfCommon.getFlowSystemDataCallBack(oid, getFlowSystemDataCallBackPrc);

        // イベント定義
        createonevent();
    }

    /**
     * フローシステムデータの取得CALL　BACK
     */
    function getFlowSystemDataCallBackPrc(data) {

        var workId = data[0]["WorkID"];
        var fk_flow = data[0]["FK_Flow"];
        var fid = data[0]["FID"];
        var wfstate = data[0]["WFState"];
        var fknode = data[0]["FK_Node"];

        // フロー初期値、新規申請、一時保存の場合、承認ルートを表示されない。
        if (wfstate === WF_STATE_NULL || wfstate === WF_STATE_INIT || wfstate === WF_STATE_DRAFT) {
            $('#approvalroot').hide();
            return;
        }

        // 承認ルート取得
        getRootInfo(fk_flow, workId, fid, fknode, approvalRootCallback);

    }


    /**
     * 承認ルート取得
     */
    function getRootInfo(fk_flow, workId, fid, fknode, callback) {

        var ret = '';
        var getInfo = 'GetApprovalRootInfo';
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Approval_Root");
        handler.AddUrlData();
        handler.AddPara("FK_Flow", fk_flow);
        handler.AddPara("WorkID", workId);
        handler.AddPara("FID", fid);
        handler.DoMethodSetString(getInfo, function (data) {
            //例外処理
            if (data.indexOf('err@') === 0) {
                wfCommon.Msgbox(data);
                return;
            }
            // JSON対象に転換
            ret = JSON.parse(data);
            callback(ret, fknode);
        });
    }

    /**
     * 承認ルート取得CALL BACK
     */
    function approvalRootCallback(data, fknode) {

        // 承認ルート画面設定
        approvalRootAreaShow(data, '', fknode);

        // 承認ルート詳細画面設定
        approvalRootAreaShow(data, 'detail', fknode);
    }

    /**
     * 承認ルートエリア設定
     */
    function approvalRootAreaShow(data, type, fknode) {

        var jidosionin = '自動承認';
        var jidosioninCnt = 2;

        //自動承認の場合、自動承認タイトル非表示
        if (data.length === jidosioninCnt && data[1]["NDFromT"] === jidosionin) {
            //自動承認タイトル設定
            $('#rootlb').text(jidosionin);
        }
        else {
            $('#rootlb').hide();
        }

        for (var i = 0; i < data.length; i++) {
            var icon = '';
            var name = '';
            var tital = '';
            var node = ''

            //フローエリア設定
            $("#rootarea" + type).clone().appendTo("#rootarealist" + type);
            $('#rootarea' + type).attr('id', 'rootarea' + i).attr('name', 'rootarea' + i);

            // インジケーター判断
            if (fknode >= data[i]["NDFrom"]) {
                tital = '申請完了';
                icon = 'a-icon a-icon--status-a';
                name = data[i]["EmpFromT"] ;
            }
            else {
                tital = '未完了';
                icon = 'a-icon a-icon--status-b'
                name = data[i]["NDFromT"];
            }
            //自動承認の場合、自動承認タイトル非表示
            if (data.length === jidosioninCnt && data[i]["NDFromT"] === jidosionin) {
                //承認ルート設定
                name = jidosionin + "済み";
            }
            else {
                node = '(' + data[i]["NDFromT"] + ')';
            }

            // インジケーター設定
            $('#rooticon' + type).attr('class', icon).attr('id', 'rooticon' + i);

            // 詳細画面設定
            if (type !== '') {
                // タイトル
                $('#roottitle').text(tital).attr('id', 'roottitle' + i);
                // 時間
                $('#rootdatatime').text(data[i]["RDT"]).attr('id', 'rootdatatime' + i);
            }

            //承認ルート設定
            $('#rootname' + type).text(name).attr('id', 'rootname' + i);
            $('#rootnode' + type).text(node).attr('id', 'rootnode' + i);

        }
        $('#rootarea' + type).remove();
    }

    /**
     * イベント定義メソッド
     */
    function createonevent() {

        // 詳しく見るクリック
        $("#internal-link").click(function () {

            // ダイアログ画面を表示すること
            $("#modal-approval-detail").addClass("o-modal--opened");

        });

        // 詳細画面の×ボタン押下イベント
        $("#approval-detail-close").click(function () {

            // ダイアログ画面を表示すること
            $("#modal-approval-detail").removeClass("o-modal--opened");
        });

    }


})(jQuery);


/*▲画面起動エリア▲*/