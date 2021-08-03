/*
* 设计器私有的配置说明 
* 一
* UE.leipiFormDesignUrl  插件路径
* 
* 二
*UE.getEditor('myFormDesign',{
*          toolleipi:true,//是否显示，设计器的清单 tool
*/
UE.leipiFormDesignUrl = 'formdesign';
/**
 * 文本框
 * @command textfield
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'textfield');
 * ```
 */
//插入回收站字段.
UE.plugins['impfrmfields'] = function () {
    var me = this, thePlugins = 'impfrmfields';
    var frmID = pageParam.fk_mapdata;
    var W = document.body.clientWidth - 120;
    var H = document.body.clientHeight - 220;
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './Fields.html?FrmID=' + frmID,
                name: thePlugins,
                editor: this,
                title: 'ゴミ箱フィールド',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
//导入表单模板..
UE.plugins['impfrm'] = function () {
    var me = this, thePlugins = 'impfrm';
    var frmID = pageParam.fk_mapdata;
    var W = document.body.clientWidth - 120;
    var H = document.body.clientHeight - 80;
    var url = "../FoolFormDesigner/ImpExp/Imp.htm?FK_MapData=" + GetQueryString("FK_MapData") + "&FrmID=" + GetQueryString("FK_MapData") + "&DoType=FunList&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: url,
                name: thePlugins,
                editor: this,
                title: 'インポート',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
//手机模板..
UE.plugins['frmmobile'] = function () {
    var me = this, thePlugins = 'frmmobile';
    var frmID = pageParam.fk_mapdata;
    var W = document.body.clientWidth - 120;
    var H = document.body.clientHeight - 80;
    var url = '../AttrNode/SortingMapAttrs.htm?FK_Flow=' + GetQueryString("FK_Flow") + '&FK_Node=' + GetQueryString('FK_Node') + '&FK_MapData=' + GetQueryString("FK_MapData");
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: url,
                name: thePlugins,
                editor: this,
                title: '携帯電話テンプレート',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
UE.plugins['text'] = function () {
    var me = this, thePlugins = 'text';
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/FrmTextBox.htm?FK_MapData=' + pageParam.fk_mapdata + '&DataType=' + dataType,
                name: thePlugins,
                editor: this,
                title: 'テキストボックス',
                cssRules: "width:600px;height:310px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);

                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();

        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('フィールドは利用できません。管理者に連絡してください');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + keyOfEn);
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);

                //删除富文本中html
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);

            }
            this.hide();
        },
        _setwidth: function () {
            var w = prompt("値を入力してください：例：25", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不正な入力");
            } else {
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>テキストボックス: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">幅度</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
    me.addListener('keydown', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            switch (evt.keyCode) {
                case 46:
                    popup.anchorEl = el;
                    eval(baidu.editor.utils.html(popup.formatHtml('$$._delete()')));
                    break;
                default:
            }
        }
    });
};

UE.plugins['edit'] = function () {
    var me = this, thePlugins = 'edit';
    me.commands[thePlugins] = {
        execCommand: function (method, datatype, obj) {
            if (obj != null) {
                var keyOfEn = obj.getAttribute("data-key");

                if (keyOfEn == null || keyOfEn == undefined || keyOfEn == "") {
                    alert('フィールドは利用できません。管理者に連絡してください');
                    return false;
                }
                showFigurePropertyWin(datatype, keyOfEn, pageParam.fk_mapdata);

            }
        }
    };
}

function showFigurePropertyWin(shap, mypk, fk_mapdata) {

    if (shap == 'Text') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドStringプロパティ');
        return;
    }

    if (shap == 'Textarea') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドの大きなテキスト属性');
        return;
    }

    
    if (shap == 'Date') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドDateプロパティ');
        return;
    }

    if (shap == 'DateTime') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドDateTimeプロパティ');
        return;
    }

    if (shap == 'Money') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドMoney属性');
        return;
    }

    if (shap == 'Double') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドDouble属性');
        return;
    }

    if (shap == 'Int') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドInt属性');
        return;
    }

    if (shap == 'Float') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドFloat属性');
        return;
    }

    if (shap == 'Radio' || shap == 'EnumSelect' || shap=='EnumCheckBox') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールドEnum属性');
        return;
    }

    if (shap == 'CheckBox') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールド Boolen 属性');
        return;
    }

    if (shap == 'BPClass' || shap == "CreateTable" || shap == "TableOrView") {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールド外部キー属性');
        return;
    }
    if (shap == 'SQL' || shap == "Handler" || shap == "JQuery") {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, 'フィールド外部データソースのプロパティ');
        return;
    }

    if (shap == 'Dtl') {
        var url = '../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=' + fk_mapdata + '&FK_MapDtl=' + mypk;
        var W = leipiEditor.body.clientWidth - 40;
        var H = leipiEditor.body.clientHeight - 40;
        CCForm_ShowDialog(url, 'テーブル/明細表から' + mypk + '属性', W, H);
        return;
    }

    if (shap == 'Img') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&PKVal=' + mypk;
        CCForm_ShowDialog(url, '画像' + mypk + '属性');
        return;
    }

    if (shap == 'Button') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&PKVal=' + mypk;
        CCForm_ShowDialog(url, 'ボタン' + fmypk + '属性');
        return;
    }



    if (shap == 'AthMulti') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + mypk;
        CCForm_ShowDialog(url, '複数の添付ファイル属性');
        return;
    }

    if (shap == 'AthImg') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&PKVal=' + mypk;
        CCForm_ShowDialog(url, '写真の添付');
        return;
    }

    //流程类的组件.
    if (shap == 'FlowChart') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmTrack&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=トラックコンポーネント';
        CCForm_ShowDialog(url, 'トラックコンポーネント');
        return;
    }

    if (shap == 'FrmCheck') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmWorkCheck&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=子スレッドコンポーネント';
        CCForm_ShowDialog(url, '監査コンポーネント');
        return;
    }

    if (shap == 'SubFlow') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmSubFlow&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=子スレッドコンポーネント';
        CCForm_ShowDialog(url, '親子フローコンポーネント');
        return;
    }


    if (shap == 'HyperLink') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmLink&PKVal=' + mypk;
        CCForm_ShowDialog(url, 'ハイパーリンク属性');
        return;
    }


    //枚举类型.
    if (shap == 'RadioButton') {
        mypk = mypk.replace('RB_', "");
        mypk = mypk.substr(0, mypk.lastIndexOf('_'));
        mypk = mypk.replace('_0', "");
        mypk = mypk.replace('_1', "");
        mypk = mypk.replace('_2', "");
        mypk = mypk.replace('_3', "");

        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + fk_mapData + "_" + mypk;
        CCForm_ShowDialog(url, 'ラジオボタンのプロパティ');
        return;
    }

    if (shap == 'IFrame') {


        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&PKVal=' + mypk;
        CCForm_ShowDialog(url, 'フレーム');
        return;
    }

    if (shap == 'HandWriting') {


        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&PKVal=' + mypk;
        CCForm_ShowDialog(url, '署名版');
        return;
    }

    if (shap == 'Score') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&PKVal=' + mypk;
        CCForm_ShowDialog(url, 'スコア');
        return;
    }

    if (shap == 'Map') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&MyPK=' + mypk;
        CCForm_ShowDialog(url, '地図');
        return;
    }

    alert('判断せずにタイプをダブルクリックします。' + shap);
}


//打开窗体
function CCForm_ShowDialog(url, title, w, h) {

    if (w == undefined)
        w = 760;

    if (h == undefined)
        h = 460;

    if (plant == 'JFlow') {
        url = url.replace('.aspx', '.jsp');
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, w, h, 'icon-library', false);
    } else {
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, w, h, 'icon-library', false);
    }
}


/**
 * 宏控件
 * @command macros
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'macros');
 * ```
 */
UE.plugins['macros'] = function () {
    var me = this, thePlugins = 'macros';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/macros.html',
                name: thePlugins,
                editor: this,
                title: 'マクロ制御',
                cssRules: "width:600px;height:270px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>マクロ制御: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

/**
 * 单选框组
 * @command radios
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'radio');
 * ```
 */
UE.plugins['enum'] = function () {
    var me = this, thePlugins = 'enum';
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var W = document.body.clientWidth - 160;
            var H = document.body.clientHeight - 220;
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/FrmEnumeration.htm?FK_MapData=' + pageParam.fk_mapdata + "&DataType=" + dataType,
                name: thePlugins,
                editor: this,
                title: 'シングルボックス',
                cssRules: "width:"+W+"px;height:"+H+"px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {

            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            if (this.anchorEl.tagName.toLowerCase() == "label")
                this.anchorEl = this.anchorEl.parentNode;
            if (this.anchorEl.tagName.toLowerCase() == "span")
                this.anchorEl.setAttribute("data-key", this.anchorEl.id.substr(3));
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                if (this.anchorEl.tagName.toLowerCase() == "label")
                    this.anchorEl = this.anchorEl.parentNode;
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('フィールドは利用できません。管理者に連絡してください');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + keyOfEn);
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);

                //删除富文本中html
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);

            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (leipiPlugins == null && $(el).parent().length > 0)
            leipiPlugins = $($(el).parent()[0]).attr('leipiplugins');

        if (/select|span|label/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var type = el.getAttribute('data-type');
            var html = "";
            if (type == 'EnumSelect')
                html = popup.formatHtml(
                    '<nobr>単一選択ドロップダウンメニュー: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            else
                html = popup.formatHtml(
                    '<nobr>ラジオグループ: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                var elInput = el.getElementsByTagName("input");
                var rEl = elInput.length > 0 ? elInput[0] : el;
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(rEl);
            } else {
                popup.hide();
            }
        }
    });
};

/**
 * 多行文本框
 * @command textarea
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'textarea');
 * ```
 */
UE.plugins['textarea'] = function () {
    var me = this, thePlugins = 'textarea';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/FrmTextBox.htm?FK_MapData=' + pageParam.fk_mapdata + '&DataType=Textarea',
                name: thePlugins,
                editor: this,
                title: '複数行のテキストボックス',
                cssRules: "width:600px;height:330px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            me.execCommand("edit", "Textarea", this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('フィールドは利用できません。管理者に連絡してください');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + keyOfEn);
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        },
        _setwidth: function () {
            var w = prompt("値を入力してください：例：25", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不正な入力");
            } else {
                var hh = baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width');
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        if (/textarea/ig.test(el.tagName)) {
            var html = popup.formatHtml(
                '<nobr>複数行のテキストボックス: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">幅度</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
    me.addListener('keydown', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/textarea/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            switch (evt.keyCode) {
                case 46:
                    popup.anchorEl = el;
                    eval(baidu.editor.utils.html(popup.formatHtml('$$._delete()')));
                    break;
                default:
            }
        }
    });
};
/**
 * 下拉菜单
 * @command select
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'select');
 * ```
 */
UE.plugins['select'] = function () {
    var me = this, thePlugins = 'select';
    me.commands[thePlugins] = {
        execCommand: function () {
            var W = document.body.clientWidth - 120;
            var H = document.body.clientHeight - 120;
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/SFList.htm?FK_MapData=' + pageParam.fk_mapdata,
                name: thePlugins,
                editor: this,
                title: 'ドロップダウンメニュー',
                cssRules: "width:" + W + "px;height:" + H + "px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('フィールドは利用できません。管理者に連絡してください');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + keyOfEn);
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (leipiPlugins == null && $(el).parent().length > 0)
            leipiPlugins = $($(el).parent()[0]).attr('leipiplugins');
        if (/select|span/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>ドロップダウンメニュー: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                if (el.tagName == 'SPAN') {
                    var elInput = el.getElementsByTagName("select");
                    el = elInput.length > 0 ? elInput[0] : el;
                }
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });

};
/**
 * 进度条
 * @command progressbar
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'progressbar');
 * ```
 */
UE.plugins['progressbar'] = function () {
    var me = this, thePlugins = 'progressbar';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/progressbar.html',
                name: thePlugins,
                editor: this,
                title: 'プログレスバー',
                cssRules: "width:600px;height:450px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>プログレスバー: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};
/**
 * 二维码
 * @command qrcode
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['qrcode'] = function () {
    var me = this, thePlugins = 'qrcode';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/qrcode.html',
                name: thePlugins,
                editor: this,
                title: 'QRコード',
                cssRules: "width:600px;height:370px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: 'キャンセル',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
               
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>QRコード: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};
/**
 * 列表控件
 * @command listctrl
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['dtl'] = function () {
    var me = this, thePlugins = 'dtl';
    me.commands[thePlugins] = {
        execCommand: function () {
            var val = prompt('セカンダリテーブルのIDを入力してください。テーブルは一意である必要があります。', pageParam.fk_mapdata + 'Dtl1');

            if (val == null) {
                return;
            }

            //秦 18.11.16
            if (!CheckID(val)) {
                alert("番号はルールを足りない");
                return;
            }

            if (val == '') {
                alert('セカンダリテーブルIDを空にすることはできません。再入力してください。');
                NewMapDtl(pageParam.fk_mapdata);
                return;
            }
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
            handler.AddPara("FK_MapData", pageParam.fk_mapdata);
            handler.AddPara("DtlNo", val);
            handler.AddPara("FK_Node", 0); //从表为原始属性的时候FK_Node=0,设置从表权限的时候FK_Node为该节点的值

            var data = handler.DoMethodReturnString("Designer_NewMapDtl");

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.WF.Template.MapDtlExt&FK_MapData=' + pageParam.fk_mapdata + '&No=' + data;
            OpenEasyUiDialog(url, "eudlgframe", 'テーブル属性から', 800, 500, "icon-edit", true, null, null, null, function () {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/Dtl.png' style='width:67%;height:200px'  leipiplugins='dtl' data-key='" + data + "'/>"
                leipiEditor.execCommand('insertHtml', _html);
            });

        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", "Dtl", this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                //在Sys_MapDtl中删除除控件属性
                var no = this.anchorEl.getAttribute("data-key");
                if (no == null || no == undefined) {
                    alert('テーブルのプロパティから取得できない場合は、管理者に連絡してください');
                    return false;
                }
                var mapDtl = new Entity("BP.Sys.MapDtl", no);
                mapDtl.Delete();
               
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>リストコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

/**
 * 附件控件
 * @command ath
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['ath'] = function () {
    var me = this, thePlugins = 'ath';
    me.commands[thePlugins] = {
        execCommand: function () {

            var val = prompt('添付ファイルIDを入力してください:(必要なのは、英数字の下線、中国語以外の文字、およびその他の特殊文字です）。', 'Ath1');
            if (val == null) {
                return;
            }

            if (val == '') {
                alert('添付ファイルIDを空にすることはできません。再入力してください。');
                return;
            }

            //秦 18.11.16
            if (!CheckID(val)) {
                alert("番号はルールを足りない");
                return;
            }

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
            handler.AddPara("FK_MapData", pageParam.fk_mapdata);
            handler.AddPara("AthNo", val);
            var data = handler.DoMethodReturnString("Designer_AthNew");

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + pageParam.fk_mapdata + '&MyPK=' + data;
            OpenEasyUiDialog(url, "eudlgframe", '添付ファイル', 800, 500, "icon-edit", true, null, null, null, function () {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/AthMulti.png' style='width:67%;height:200px'  leipiplugins='ath' data-key='" + data + "' />"
                leipiEditor.execCommand('insertHtml', _html);
            });


        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", "AthMulti", this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                //在Sys_FrmAttachment中删除除控件属性
                var mypk = this.anchorEl.getAttribute("data-key");
                if (mypk == null || mypk == undefined) {
                    alert('添付ファイルの属性が取得されていません。管理者に連絡してください');
                    return false;
                }
                var ath = new Entity("BP.Sys.FrmAttachment", mypk);
                ath.Delete();

                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>添付ファイルコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};



/**
 *控件
 * @command ath
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['component'] = function () {
    var me = this, thePlugins = 'component';
    me.commands[thePlugins] = {
        execCommand: function (methode, dataType) {
            if (dataType == "Dtl") { //从表

            }
            if (dataType == "AthMulti") { //多附件

            }
            if (dataType == "Img") {//图片
                ExtImg();
            }
            if (dataType == "IFrame") {//框架
                NewFrame();
            }
            if (dataType == "Map") {//地图控件
                ExtMap();
            }

            if (dataType == "Score") {//评分
                ExtScore();
            }

            if (dataType == "AthImg") {//图片附件
                ExtImgAth();
            }

            if (dataType == "HandWriting") {//手写签字版
                ExtHandWriting();
            }
            if (dataType == "SubFlow") {//父子流程
                var mypk = GetQueryString("FK_Node");

                if (mypk == null || mypk == undefined) {
                    alert('非ノード形式');
                    return;
                }
                var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PKVal=' + mypk + '&tab=親子フローコンポーネント';
                OpenEasyUiDialog(url, "eudlgframe", 'コンポーネント', 800, 550, "icon-property", true, null, null, null, function () {
                    //加载js
                    // $("<script type='text/javascript' src='../../WorkOpt/SubFlow.js'></script>").appendTo("head");
                    var _html = "<img src='../CCFormDesigner/Controls/DataView/SubFlowDtl.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + mypk + "'  data-type='SubFlow'/>"
                    leipiEditor.execCommand('insertHtml', _html);
                    return;

                });
            }

        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('コントロールを削除してもよろしいですか？')) {
                var dataType = this.anchorEl.getAttribute("data-type");
                var mypk = this.anchorEl.getAttribute("data-key");
                if (mypk == null || mypk == undefined) {
                    alert('要素属性のデータキーがありません。管理者に連絡してください');
                    return false;
                }

                if (dataType == "AthImg") {
                    var imgAth = new Entity("BP.Sys.FrmImgAth", mypk);
                    imgAth.Delete();
                }
                if (dataType == "Img") {
                    var en = new Entity("BP.Sys.FrmUI.ExtImg", mypk);
                    en.Delete();
                }
                if (dataType == "IFrame") {
                    var en = new Entity("BP.Sys.FrmUI.MapFrameExt", mypk);
                    en.Delete();
                }
                if (dataType == "Map") {
                    var mapAttr = new Entity("BP.Sys.MapAttr", mypk);
                    mapAttr.Delete();
                }
                if (dataType == "Score") {
                    var mapAttr = new Entity("BP.Sys.MapAttr", mypk);
                    mapAttr.Delete();
                }
                if (dataType == "HandWriting") {
                    var mapAttr = new Entity("BP.Sys.MapAttr", mypk);
                    mapAttr.Delete();
                }
                if (dataType == "SubFlow") {
                    var nodeID = GetQueryString("FK_Node");
                    var subFlow = new Entity("BP.WF.Template.FrmSubFlow", nodeID);
                    subFlow.SFSta = 0;//禁用
                    subFlow.Update();
                }
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        var dataType = el.getAttribute("data-type");
        if (/img|span/ig.test(el.tagName.toLowerCase()) && leipiPlugins == thePlugins) {
            var _html;
            if (dataType == "Dtl")
                _html = popup.formatHtml(
                    '<nobr>リストコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "AthMulti")
                _html = popup.formatHtml(
                    '<nobr>添付ファイルコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "AthImg")
                _html = popup.formatHtml(
                    '<nobr>ピクチャーアタッチメントコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "Img")
                _html = popup.formatHtml(
                    '<nobr>画像コントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "IFrame")
                _html = popup.formatHtml(
                    '<nobr>フレームコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "Map")
                _html = popup.formatHtml(
                    '<nobr>マップコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "Score")
                _html = popup.formatHtml(
                    '<nobr>スコアリングコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "HandWriting")
                _html = popup.formatHtml(
                    '<nobr>手書きの署名バージョンコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');
            if (dataType == "SubFlow")
                _html = popup.formatHtml(
                    '<nobr>親子フローコントロール: <span onclick=$$._edittext() class="edui-clickable">編集</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">削除</span></nobr>');

            if (_html) {
                popup.getDom('content').innerHTML = _html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};


UE.plugins['more'] = function () {
    var me = this, thePlugins = 'more';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/more.html',
                name: thePlugins,
                editor: this,
                title: 'フォームデザイナーと一緒に楽しんで、一緒に参加して、改善を手伝ってください',
                cssRules: "width:600px;height:200px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};
UE.plugins['error'] = function () {
    var me = this, thePlugins = 'error';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/error.html',
                name: thePlugins,
                editor: this,
                title: '異常提示',
                cssRules: "width:400px;height:130px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};
UE.plugins['leipi'] = function () {
    var me = this, thePlugins = 'leipi';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/leipi.html',
                name: thePlugins,
                editor: this,
                title: 'フォームデザイナー - チェックリスト',
                cssRules: "width:620px;height:220px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};
UE.plugins['leipi_template'] = function () {
    var me = this, thePlugins = 'leipi_template';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/template.html',
                name: thePlugins,
                editor: this,
                title: 'フォームテンプレート',
                cssRules: "width:640px;height:380px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '確定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};

UE.registerUI('button_leipi', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            editor.execCommand('leipi');
        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "フォームデザイナー",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -401px -40px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });
    /*
        //当点到编辑内容上时，按钮要做的状态反射
        editor.addListener('selectionchange', function () {
            var state = editor.queryCommandState(uiName);
            if (state == -1) {
                btn.setDisabled(true);
                btn.setChecked(false);
            } else {
                btn.setDisabled(false);
                btn.setChecked(state);
            }
        });
    */
    //因为你是添加button,所以需要返回这个button
    return btn;
});
UE.registerUI('button_template', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            try {
                leipiFormDesign.exec('leipi_template');
                //leipiFormDesign.fnCheckForm('save');
            } catch (e) {
                alert('テンプレートを開く例外');
            }

        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "フォームテンプレート",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -339px -40px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });

    //因为你是添加button,所以需要返回这个button
    return btn;
});
UE.registerUI('button_preview', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            try {
                leipiFormDesign.fnReview();
            } catch (e) {
                alert('leipiFormDesign.fnReview プレビュー異常');
            }
        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "プレビュー",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -420px -19px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });

    //因为你是添加button,所以需要返回这个button
    return btn;
});

UE.registerUI('button_save', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            try {
                SaveForm();
            } catch (e) {
                alert('leipiFormDesign.fnCheckForm("save") 保存異常');
            }

        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "保存フォーム",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -481px -20px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });

    //因为你是添加button,所以需要返回这个button
    return btn;
});

//手写签名版.
function ExtHandWriting() {

    var name = window.prompt('署名バージョンの名前を入力してください：\t\n例：署名バージョン、署名', '署名版');
    if (name == null || name == undefined)
        return;

    var frmID = pageParam.fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名前：[' + name + "]もう存在している。");
        ExtHandWriting();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名前：[' + name + "]もう存在している。");
        ExtHandWriting();
        return;
    }

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 8; //手写签名版.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&MyPK=" + mapAttr.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '署名版', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<img src='../../../DataUser/Siganture/admin.jpg' onerror=\"this.src='../../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' data-type='HandWriting' data-key='" + mapAttr.MyPK + "'  leipiplugins='component'/>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}

//图片附件
function ExtImgAth() {
    var name = window.prompt('画像の添付ファイルの名前を入力してください：\t\n例：ポートレート、アバター、ICON、マップの場所', '肖像画');
    if (name == null || name == undefined)
        return;
    var ImgAths = new Entities("BP.Sys.FrmImgAths");
    ImgAths.Retrieve("FK_MapData", pageParam.fk_mapdata, "Name", name);
    if (ImgAths.length >= 1) {
        alert('名前：[' + name + "]もう存在している。");
        ExtImgAth();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);

    var imgAth = new Entity("BP.Sys.FrmImgAth");
    imgAth.FK_MapData = pageParam.fk_mapdata;
    imgAth.CtrlID = id;
    imgAth.MyPK = pageParam.fk_mapdata + "_" + id;
    imgAth.Name = name;
    imgAth.Insert();

    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&MyPK=" + imgAth.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '写真の添付', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<img src='../CCFormDesigner/Controls/DataView/AthImg.png' style='width:" + imgAth.W + "px;height:" + imgAth.H + "px'  leipiplugins='component' data-key='" + imgAth.MyPK + "' data-type='AthImg'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}


//图片
function ExtImg() {
    var name = window.prompt('写真の名前を入力してください：\t\n例：肖像画画、アバター、ICON、地図の場所', '肖像画');
    if (name == null || name == undefined)
        return;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", pageParam.fk_mapdata, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名前：[' + name + "]もう存在している。");
        ExtImg();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = pageParam.fk_mapdata + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名前：[' + name + "]もう存在している。");
        return;
    }
    mapAttr.FK_MapData = pageParam.fk_mapdata;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.UIContralType = 11; //FrmImg 类型的控件.
    mapAttr.MyDataType = 1;
    mapAttr.ColSpan = 0; //单元格.
    mapAttr.LGType = 0;
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();

    var en = new Entity("BP.Sys.FrmUI.ExtImg");
    en.MyPK = pageParam.fk_mapdata + "_" + id;
    en.FK_MapData = pageParam.fk_mapdata;
    en.KeyOfEn = id;

    en.ImgAppType = 0; //图片.
    en.FK_MapData = pageParam.fk_mapdata;
    en.GroupID = mapAttr.GroupID; //设置分组列.
    en.Name = name;
    en.Insert(); //插入到数据库.

    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&MyPK=" + en.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '画像', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<img src='../CCFormDesigner/Controls/basic/Img.png' style='width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px'  leipiplugins='component' data-key='" + en.MyPK + "' data-type='Img'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}

///框架
function NewFrame() {
    var alert = "\t\n1。アプリケーションの拡張をより適切にサポートするために、ccformはiFrameマップ、フロー軌道グラフ、および軌道テーブル表示を使用できます。";
    alert += "\t\n2。フレームを作成したら、フレームのプロパティで設定します。";
    alert += "\t\n3。フレームIDを入力してください。これには、英数字の下線、中国語以外の文字、およびその他の特殊文字が必要です。";

    var val = prompt('新しいフレーム：' + alert, 'Frame1');

    if (val == null) {
        return;
    }

    if (val == '') {
        alert('フレームIDを空にすることはできません。再入力してください。');
        NewFrame(pageParam.fk_mapdata);
        return;
    }

    var en = new Entity("BP.Sys.FrmUI.MapFrameExt");
    en.MyPK = pageParam.fk_mapdata + "_" + val;
    if (en.IsExits() == true) {
        alert("該当番号[" + val + "]もう存在している");
        return;
    }

    en.FK_MapData = pageParam.fk_mapdata;
    en.Name = "マイフレーム" + val;
    en.FrameURL = 'MapFrameDefPage.htm';
    en.H = 200;
    en.W = 200;
    en.X = 100;
    en.Y = 100;
    en.Insert();

    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&FK_MapData=' + pageParam.fk_mapdata + '&MyPK=' + en.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", 'フレーム', 800, 500, "icon-edit", true, null, null, null, function () {

        var _html = "<img src='../CCFormDesigner/Controls/DataView/iFrame.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + en.MyPK + "' data-type='IFrame'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}
//地图
function ExtMap() {
    var name = window.prompt('マップ名を入力してください：\t\n例：中国のマップ', '地図');
    if (name == null || name == undefined)
        return;

    var frmID = pageParam.fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名前：[' + name + "]もう存在している。");
        ExtMap();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名前：[' + name + "]もう存在している。");
        ExtMap();
        return;
    }

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 4; //地图.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 800;//宽度
    mapAttr.UIHeight = 500;//高度
    mapAttr.Insert(); //插入字段.

    var mapAttr1 = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 0;
    mapAttr1.MyPK = frmID + "_AtPara";
    mapAttr1.FK_MapData = frmID;
    mapAttr1.KeyOfEn = "AtPara";
    mapAttr1.UIVisible = 0;
    mapAttr1.Name = "AtPara";
    mapAttr1.MyDataType = 1;
    mapAttr1.LGType = 0;
    mapAttr1.ColSpan = 1; // 
    mapAttr1.UIWidth = 100;
    mapAttr1.UIHeight = 23;
    mapAttr1.Insert(); //插入字段

    mapAttr.Retrieve();
    var url = './../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&MyPK=' + mapAttr.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '地図', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<div style='text-align:left;padding-left:0px' id='Map_" + mapAttr.KeyOfEn + "' data-type='Map' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
        _html += "<input type='button' name='select' value='選択'  style='background: #fff;color: #545454;font - size: 12px;padding: 4px 15px;margin: 5px 3px 5px 3px;border - radius: 3px;border: 1px solid #d2cdcd;'/>";
        _html += "<input type = text style='width:200px' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' />";
        _html += "</div>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}
//评分
function ExtScore() {
    var name = window.prompt('スコアリングアイテムの名前を入力してください：\t\n例：速達速度、サービスレベル', 'スコアリングの問題');
    if (name == null || name == undefined)
        return;

    var frmID = pageParam.fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名前：[' + name + "]もう存在している。");
        ExtScore();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名前：[' + name + "]もう存在している。");
        ExtScore();
        return;
    }

    var score = window.prompt('合計スコアを設定してください：\t\n例：5、10', '5');
    if (score == null || score == undefined)
        return;

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 101; //评分控件.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Tag2 = score; // 总分
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&MyPK=" + mapAttr.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", 'スコア', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<span class='score-star'style='text-align:left;padding-left:0px'  data-type='Score' data-key='" + mapAttr.MyPK + "' leipiplugins='component' id='SC_" + mapAttr.KeyOfEn + "'>";
        _html += "<span class='simplestar' data-type='Score'  leipiplugins='component'  data-key='" + mapAttr.MyPK + "' id='Star_" + mapAttr.KeyOfEn + "'>";

        var num = mapAttr.Tag2;
        for (var i = 0; i < num; i++) {

            _html += "<img src='../../Style/Img/star_2.png' data-type='Score'  leipiplugins='component'  data-key='" + mapAttr.MyPK + "'/>";
        }
        _html += "&nbsp;&nbsp;<span class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",ソンティ,sans-serif;'><strong>" + num + "  分</strong></span>";
        _html += "</span></span>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}


//全局变量
var pageParam = {};
pageParam.fk_mapdata = GetQueryString("FK_MapData");

function SaveForm() {

    $("#Btn_Save").val("保存してください。");

    try {
        Save();
    } catch (e) {
        alert(e);
        return;
    }

    $("#Btn_Save").val("正常に保存されました");
    setTimeout(function () { $("#Btn_Save").val("保存。"); }, 1000);
}

//保存表单的htm代码
function Save() {

    //清空MapData的缓存
    var en = new Entity("BP.Sys.MapData", pageParam.fk_mapdata);
    en.SetPKVal(pageParam.fk_mapdata);
    en.DoMethodReturnString("ClearCash");

    if (leipiEditor.queryCommandState('source'))
        leipiEditor.execCommand('source');//切换到编辑模式才提交，否则有bug

    if (leipiEditor.hasContents() == false) {
        alert('フォームの内容を空にすることはできません。');
        return false;
    }

    $("#Btn_Save").val("保存中...");


    leipiEditor.sync();       //同步内容


    if (typeof type !== 'undefined') {
        type_value = type;
    }


    //比对Sys_MapAttr,如果html存在符合我们代码规则的保存到Sys_MapAttr中
    var strs = "FID,FK_Dept,FK_Emp,FK_NY,MyNum,OID,RDT,CDT,Rec"//デフォルト
    var ens = new Entities("BP.Sys.MapAttrs");
    ens.Retrieve("FK_MapData", pageParam.fk_mapdata);
    var mapAttrs = {};
    $.each(ens, function (i, en) {
        if ($.isArray(mapAttrs[en.MyPK]) == false)
            mapAttrs[en.MyPK] = [];
        mapAttrs[en.MyPK].push(en);
    })

    //获取含有data-type的元素
    var inputs = leipiEditor.document.getElementsByTagName("input");
    //遍历所有的input元素属性
    $.each(inputs, function (i, tag) {
        var dataType = tag.getAttribute("data-type");
        if (dataType != null && dataType != undefined && dataType != "") {
            //判断是否保存在Sys_MapAttr中，没有则保存
            var keyOfEn = tag.getAttribute("data-key");
            if (dataType == "Radio")
                keyOfEn = $($(tag).parent()[0]).parent()[0].getAttribute("data-key");//获取父级的data-key
            if (keyOfEn != null && keyOfEn != undefined && keyOfEn != "") {
                var mapAttr = mapAttrs[pageParam.fk_mapdata + "_" + keyOfEn];
                if ((mapAttr == undefined || mapAttr == null) && keyOfEn != "" && uiBindKey != "") {
                    if (dataType == "Radio") {
                        var uiBindKey = tag.getAttribute("data-bindKey");
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                        handler.AddPara("KeyOfEn", keyOfEn);
                        handler.AddPara("FK_MapData", pageParam.fk_mapdata);
                        handler.AddPara("EnumKey", uiBindKey);
                        var data = handler.DoMethodReturnString("SysEnumList_SaveEnumField");
                        if (data.indexOf("err@") >= 0) {
                            alert(data);
                            return;
                        }
                    }
                    var name = tag.getAttribute("data-name");
                    mapAttr = new Entity("BP.Sys.MapAttr");

                    mapAttr.MyPK = pageParam.fk_mapdata + "_" + keyOfEn;
                    mapAttr.FK_MapData = pageParam.fk_mapdata;
                    mapAttr.KeyOfEn = keyOfEn;
                    mapAttr.Name = name;
                    if (dataType == "Text")
                        dataType = 1;
                    if (dataType == "Int")
                        dataType = 2;
                    if (dataType == "Float")
                        dataType = 3
                    if (dataType == "Money")
                        dataType = 8;
                    if (dataType == "Date")
                        dataType = 6;
                    if (dataType == "DateTime")
                        dataType = 7;
                    if (dataType == "CheckBox")
                        dataType = 4;
                    mapAttr.MyDataType = dataType;
                    if (dataType == 4) {
                        mapAttr.UIContralType = 2//checkbox
                        mapAttr.LGType = 0;
                    }
                    else if (dataType == "Radio" || dataType == "Select") {
                        mapAttr.UIContralType = 1;//下拉框
                        mapAttr.LGType = 1;//枚举
                    } else {
                        mapAttr.UIContralType = 0;//TB
                        mapAttr.LGType = 0;
                    }
                    mapAttr.Insert();
                }
            }
           

        }
    });
    var selects = leipiEditor.document.getElementsByTagName("select");
    $.each(selects, function (i, tag) {
        var dataType = tag.getAttribute("data-type");
        if (dataType != null && dataType != undefined && dataType != "") {
            //找到父节点
            var ptag = $(tag).parent()[0];
            var sfTable = "";
            var keyOfEn = "";
            var uiBindKey = "";
            if (ptag.tagName.toLowerCase() == "span" && (ptag.getAttribute('leipiplugins') == "select" || ptag.getAttribute('leipiplugins') == "enum")) {
                sfTable = ptag.getAttribute("data-sfTable");
                keyOfEn = tag.getAttribute("data-key");
                uiBindKey = tag.getAttribute("data-bindKey");
            }
            var mapAttr = mapAttrs[pageParam.fk_mapdata + "_" + keyOfEn];
            if ((mapAttr == undefined || mapAttr == null) &&  keyOfEn != "" && uiBindKey != "") {
                if (dataType == "EnumSelect") {
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                    handler.AddPara("KeyOfEn", keyOfEn);
                    handler.AddPara("FK_MapData", pageParam.fk_mapdata);
                    handler.AddPara("EnumKey", uiBindKey);
                    var data = handler.DoMethodReturnString("SysEnumList_SaveEnumField");
                    if (data.indexOf("err@") >= 0) {
                        alert(data);
                        return;
                    }
                } else {
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                    handler.AddPara("KeyOfEn", keyOfEn);
                    handler.AddPara("FK_MapData", pageParam.fk_mapdata);
                    handler.AddPara("SFTable", sfTable);
                    var data = handler.DoMethodReturnString("SFList_SaveSFField");
                    if (data.indexOf("err@") >= 0) {
                        alert(data);
                        return;
                    }
                }
            }
        }
    });

    //补充枚举值不全的情况
    var spans = leipiEditor.document.getElementsByTagName("span");
    for (var i = 0; i < spans.length; i++) {
        var tag = spans[i];
        var uiBindKey = tag.getAttribute("data-bindKey");
        if (uiBindKey == null || uiBindKey == undefined || uiBindKey == "")
            continue;
        if (tag.getAttribute("data-type") != "Radio")
            continue;
        //获取枚举值
        //获取枚举值
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", uiBindKey);
        if (enums.length == 0)
            continue;
        var keyOfEn = tag.getAttribute("data-key");
        $.each(enums, function (idx, obj) {

            if (leipiEditor.document.getElementById("RB_" + keyOfEn + "_" + obj.IntKey) == null)
                $(tag).append('<input type="radio" value="' + obj.IntKey + '" id="RB_' + keyOfEn + '_' + obj.IntKey + '" name="RB_' + keyOfEn + '" data-key="' + keyOfEn + '" data-type="Radio" data-bindkey="' + uiBindKey + '" class="form-control" style="width: 15px; height: 15px;">' + obj.Lab);
        });

    }

    formeditor = leipiEditor.getContent();
    //保存表单的html信息
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
    handler.AddPara("FK_MapData", pageParam.fk_mapdata);
    handler.AddPara("HtmlCode", formeditor);

    var data = handler.DoMethodReturnString("SaveForm");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }
    //$("#Btn_Save").val("保存成功.....");
    //  alert("保存成功!");
    $("#Btn_Save").val("保存");
}
//预览
function PreviewForm() {
    //debugger
    if (leipiEditor.queryCommandState('source'))
        leipiEditor.execCommand('source');//切换到编辑模式才提交，否则有bug

    if (leipiEditor.hasContents()) {
        leipiEditor.sync();       //同步内容
        document.saveform.target = "mywin";
        window.open('', 'mywin', "menubar=0,toolbar=0,status=0,resizable=1,left=0,top=0,scrollbars=1,width=" + (screen.availWidth - 10) + ",height=" + (screen.availHeight - 50) + "\"");
        SaveForm();
    } else {
        alert('フォームの内容を空にすることはできません。');
        return false;
    }
}

