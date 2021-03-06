(function ($, window, document, undefined) {
    let comboTreePlugin = 'comboTree',
        //定义一个存储数据的数组，用于下面重复选择判断，删除标签,
        defaults = {
            source: '',
            listSource:'',
            isMultiple: false,
            cascadeSelect: false,
            selected: [],
            selectedlength: 3,
            keyOfEn: '',
            isShowSignature:false

        },
        tempstructerarray = [];

    let ComboTree = function (element, options) {
        this.options = $.extend({}, defaults, options);
        this.element = element;
        this.ulcontainer = null;
        this.oliIdArray = [];
        this.oliNameArray = [];
        this.copysource = [];
        this.roleSelect = null;
        this.copydom = null;
        this.comboxinput = null;
        this.comboxinputcontainer = null;
        this.comboxulcontainer = null;
        this.selectul = null;
        this.myplaceholder = null;
        this.noresults = null;
        this.init();
    };

    ComboTree.prototype.init = function () {
        var src = this.options.listSource;
        var dt = DBAccess.RunDBSrc(src);
        $.extend(true, this.copysource, dt);
        this.initstruct();
        this.initdom();
        this.ulcontainer = $(this.element).find('div[_id=comboxulcontainer]');
        this.comboxinput = $(this.element).find('input[_id=comboxinput]');
        this.roleSelect = $(this.element).find('div[_id=role_select]');
        this.comboxinputcontainer = $(this.element).find('div[_id=comboxinputcontainer]');
        this.comboxulcontainer = $(this.element).find('div[_id=comboxulcontainer]')
        this.copydom = this.selectul.clone();
        this.myplaceholder = $(this.element).find('span[_id=myplaceholder]');
        this.noresults = $(this.element).find('.noresults');
        this.initData();
        this.initevent();
        

    };

    ComboTree.prototype.initstruct = function () {
        for (let i = 0; i < this.copysource.length; i++) {
            this.copysource[i]['class'] = 'first'
        }
    };

    ComboTree.prototype.initevent = function () {
        let _this = this;

        //点击输入框时候
        $(this.element).on('click', function (event) {

            event = event || window.event;

            let closet = $(event.target).closest('.input-keyword-wrap'), icon = null;
            if (($(event.target).hasClass('imitationSelect') || $(event.target).hasClass('fa')) && closet.length > 0) {
                $(_this.element).find('.drop-down-wrap').toggle();
                if (!icon) {
                    icon = $(_this.element).find('.input-keyword-wrap>i');
                }
                if (icon.hasClass("fa-caret-down")) {
                    icon.removeClass("fa-caret-down").addClass("fa-caret-up"); //点击input选择适合，小图标动态切换
                } else {
                    icon.removeClass("fa-caret-up").addClass("fa-caret-down"); //点击input选择适合，小图标动态切换
                }
            }

            let target = $(event.target).hasClass('title-container') ? $(event.target) : $(event.target)
                .parent().hasClass('title-container') ? $(event.target).parent() : null;

            if (target) {
                // 如果已选择的大于设定的数目，并且当前是选择动作 则不执行
                if ((_this.oliIdArray.length >= _this.options.selectedlength) && !target.hasClass('actived_li') && _this.options.isMultiple) {
                    console.log('最大オプションが設定されています');
                    return false;
                }

                if (target.attr('role') !== 'parent') {
                    event.target = target;
                    let oliId = target.attr("data-id");
                    if (target.hasClass('actived_li')) {
                        _this.uncheckrow(oliId);
                    } else {
                        if (!_this.options.isMultiple) { //如果是单选，则已选条目大于零，并且所点击的不是 激活状态的，不执行
                            if (_this.oliIdArray.length > 0) {
                                for (let j = 0; j < _this.oliIdArray.length; j++) {
                                    _this.uncheckrow(_this.oliIdArray[j]);
                                }
                            }
                        }
                        _this.checkrow(target, oliId, false); //第三个参数表示点击的是否是checkbox
                    }

                    if (!_this.options.isMultiple) {
                        _this.hideul();
                    }
                }
            }

            if ($(event.target).attr('type') === 'checkbox') {
                if ((_this.oliIdArray.length >= _this.options.selectedlength) && $(event.target).prop(
                    'checked')) {
                    console.log('最大項目を超えています');
                    return false;
                }

                let _target = $(event.target).closest('.title-container');

                if (_target.hasClass('actived_li')) {
                    _this.uncheckrow($(event.target).attr('data-id'));
                } else {
                    _this.checkrow(_target, $(event.target).attr('data-id'), true);
                }
            }

            //点击x关闭事件处理
            if ($(event.target).attr('class') === 'close') {
                _this.uncheckrow($(event.target).attr('data-id'));
            }

            let containerparent = $(event.target).attr('role') === 'parent' ? $(event.target) : $(event
                .target).parent().attr('role') === 'parent' ? $(event.target).parent() : null;

            if (!containerparent) {
                target = $(event.target).attr('tag') === 'closeitem' ?
                    $(event.target) : $(event.target).parent().attr('tag') === 'closeitem' ?
                        $(event.target).parent() : null;
            }

            if ($(event.target).attr('tag') === 'search' || $(event.target).parent().attr('tag') === 'search') {
                _this.comboxinput.val('');
                _this.comboxinput.trigger('keyup');
            }

            if (target || containerparent) {
                let _parent = null;
                if (containerparent) {
                    _parent = containerparent
                } else {
                    _parent = target.closest('.title-container');
                }

                _parent.next().toggle();
                target = target.find('i');

                if (target.hasClass('fa-caret-down')) {
                    target.removeClass('fa-caret-down').addClass('fa-caret-right');
                } else {
                    target.removeClass('fa-caret-right').addClass('fa-caret-down');
                }
            }

            if (event.stopPropagation) {
                event.stopPropagation(); // 针对 Mozilla 和 Opera
            } else if (window.event) {
                window.event.cancelBubble = true; // 针对 IE
            }
        });

        this.comboxinput.on('keyup', function (event) {
            event = event || window.event;

            _this.selectul.find('.hide').removeClass('hide');


            // 判断搜索框里是否有内容，如果有则添加删除按钮
            if (event.currentTarget.value != "") {
                $(this).siblings('span').find('i').removeClass('fa-search').addClass('fa-close');
                //修改选择的下拉框内容
                var src = _this.options.source;
                src = src.replace(/@Key/g, event.currentTarget.value);
                var dt = DBAccess.RunDBSrc(src);
                _this.selectul.html("");
                ComboTree.prototype.createitem(dt, _this.selectul,_this.oliIdArray);

            } else {
                $(this).siblings('span').find('i').removeClass('fa-close').addClass('fa-search');
                //修改选择的下拉框内容
                var src = _this.options.listSource;
                var dt = DBAccess.RunDBSrc(src);
                _this.selectul.html("");
                ComboTree.prototype.createitem(dt, _this.selectul, _this.oliIdArray);

            }

            //let lis = _this.selectul.find('li'),
            //    targetli = null,
            //    _tempattr = null;

            //lis.each(function (index, item) {
            //    $(item).attr('matched', '');
            //});
            //let val = $(event.target).val();

            //function getChildren(parent) {
            //    let lichild = parent.children('li');

            //    if (lichild.length) {
            //        for (let i = 0, _p = lichild, len = _p.length; i < len; i++) {
            //            targetli = _p.eq(i), _tempattr = targetli.attr('data-name');

            //            if (_tempattr.indexOf(val) >= 0 && _tempattr !== ' ' && _tempattr !== '') {
            //                targetli.attr('matched', 'matched');
            //            }

            //            let subulcontainer = targetli.find('>.tree-sub-body');

            //            if (subulcontainer.length > 0) {
            //                let _tempul = subulcontainer.find('>ul');
            //                getChildren(_tempul);
            //            }
            //        }
            //    }
            //}
            //getChildren(_this.selectul);

            //if (val.trim() !== '') {
            //    let lis1 = _this.selectul.find('li');
            //    lis1.each(function (index, item) {
            //        let _item = $(item);
            //        let matched = _item.find('li[matched="matched"]');
            //        if (_item.length === 0) {
            //            return true; //相当于continue
            //        }
            //        if (matched.length === 0) {
            //            if (_item.attr('matched') === 'matched') { //如果当前元素匹配，则保留当前的删除它后面的所有元素

            //                let _matcheditem = _item.find('>div.tree-sub-body');
            //                _matcheditem.each(function (index, item) {
            //                    $(item).addClass('hide');
            //                });
            //            } else {
            //                _item.addClass('hide');
            //            }
            //        }
            //    });

            //    let children = _this.comboxulcontainer.find('li[matched=matched]');

            //    if (children.length === 0) {
            //        _this.selectul.hide();
            //        _this.noresults.show();
            //    } else {
            //        _this.noresults.hide();
            //        _this.selectul.show();
            //    }

            //} else {
            //    _this.selectul.show();
            //    _this.selectul.find('.hide').removeClass('hide');
            //    _this.noresults.hide();
            //}

        });

        //点击任意地方隐藏下拉
        $(document).click(function (event) {
            _this.hideul();
        });
    };

    ComboTree.prototype.initData = function () {
        let _this = this;
       

        if (_this.options.selected.length != 0) {
            $.each(_this.options.selected, function (i, targetName) {
                if (targetName != "") {
                    var oliId = "";
                    //获取数据oliId
                    for (var i = 0; i < _this.copysource.length; i++) {
                        if (_this.copysource[i].Name == targetName) {
                            oliId = _this.copysource[i].No;
                            break;
                        }

                    }

                    _this.oliIdArray.push(oliId);
                    _this.oliNameArray.push(targetName);
                    _this.roleSelect.attr("data-id", _this.oliIdArray); //把当前点击的oliId赋值到显示的input的oliId里面
                    _this.roleSelect.attr("data-name", _this.oliNameArray)


                    if (_this.oliIdArray.length > 0) {
                        _this.myplaceholder.hide();
                    }
                    let length = targetName.length;
                    //向input里面存放的内容，是一个span //onchange='_this.ChangeDataName(this.value,\""+ oliId+"\")'
                    let item, namespan, checkicon;
                    let signature
                    if (_this.options.isShowSignature == false) {
                        item = $("<span data-id='" + oliId + "' class='input-keyword-item'></span>");
                        namespan = $("<input type='text' style='border:0px;width:" + length * 15 + "px;margin:1px 1px !important;' value='" + targetName + "' onchange='ChangeDataName(\"" + _this.options.keyOfEn + "\")'  />");
                        checkicon = $("<i class='close' data-id='" + oliId + "' >x</i>");
                    } else {
                        item = $("<span data-id='" + oliId + "' class='input-keyword-item only-print-hidden'></span>");
                        namespan = $("<input type='text' style='border:0px;width:" + length * 15 + "px;margin:1px 1px !important;' value='" + targetName + "' onchange='ChangeDataName(\"" + _this.options.keyOfEn + "\")'  />");
                        checkicon = $("<i class='close' data-id='" + oliId + "' >x</i>");
                        if ("undefined" == typeof UserICon) {
                            UserICon = '../../DataUser/Siganture/';
                        } else {
                            UserICon = UserICon.replace("@basePath", basePath);
                        }
                        if ("undefined" == typeof UserIConExt) {
                            UserIConExt = '.jpg';
                        } 
                        signature = $("<img src='" + UserICon + oliId + UserIConExt+ "' class='only-print-show' id='Img_" + oliId + "'/>");
                    }
                    


                    item.append(namespan);
                    if (_this.options.isMultiple) {
                        item.append(checkicon);
                    } else {
                        item.addClass('single-keyword')
                    }

                    _this.roleSelect.append(item);

                    //打印显示签名
                    if (_this.options.isShowSignature == true)
                        _this.roleSelect.append(signature);

                }
                
            });
        }
    };

    ComboTree.prototype.initdom = function () {
        $(this.element).append(
            '<div class="input-keyword-wrap" style="display:block">' +
            '<div _id="role_select" class="select-menu-input imitationSelect role_select">' +
            '<span _id="myplaceholder" class="input-tips"></span>' +
            '</div>' +
            '<i class="fa fa-caret-down handle-arrow only-print-hidden"></i>' +
            '</div>' +
            '<div class="drop-down-wrap">' +
            '<div _id="comboxinputcontainer" class="comboxinputcontainer keyword-search">' +
            '<input _id="comboxinput" placeholder="キーワードを入力して検索" type="text">' +
            '<span tag="search" class="search-icons"><i class="fa fa-search"></i></span>' +
            '</div>' +
            '<div _id="comboxulcontainer">' +
            '<div _id="noresault" class="noresults">検索結果がありません</div>' +
            '<ul class="select-tree-list" _id="selectUl"></ul>' +
            '</div>' +
            '</div>');

        this.selectul = $(this.element).find('ul[_id=selectUl]');
        this.createitem(this.copysource, this.selectul, this.options.selected);

    };

    ComboTree.prototype.createitem = function (SampleJSONData, container,selected) {
        for (let j = 0; j < SampleJSONData.length; j++) {

            var oliName = SampleJSONData[j].Name;
            var oliId = SampleJSONData[j].No;
            //判断该节点是否被选中
            var isSelected = false;
            if (selected.join(",").indexOf(oliName) != -1)
                isSelected = true;
            if (selected.join(",").indexOf(oliId) != -1)
                isSelected = true;
            // li容器
            let item = $('<li data-name="' + oliName + '" data-id="' + oliId + '"></li>');
            let divitem = $('<div data-name="' + oliName + '" data-id="' + oliId + '" class="title-container"></div>');
            if (isSelected == true)
                divitem = $('<div data-name="' + oliName + '" data-id="' + oliId + '" class="title-container actived_li"></div>');
            spanitem = $('<span tag="closeitem" class="handle-left-icons"></span>');
            divitem.append(spanitem);

            ////如果是第一层，给他设置一个属性叫firstclass
            //if (SampleJSONData[j]['class'] === 'first') {
            //    divitem.attr('role', 'parent');
            //}

            //if (SampleJSONData[j]['subs']) {
            //    spanitem.append('<i class="fa fa-caret-down"></i>');
            //    divitem.append(spanitem);
            //    divitem.attr('role', 'parent');
            //}

            ////放入checkbox  ,这段业务表示如果是第一层，并且在配置中写了，第一层允许选择才加入checkbox
            //if (this.options.isMultiple) {
            //    if (SampleJSONData[j]['class'] === 'first') {
            //        if (this.options.isFirstClassSelectable) {
            //            appendcheckbox(divitem, oliId);
            //        }
            //    } else {
            //        if (!SampleJSONData[j]['subs']) {
            //            appendcheckbox(divitem, oliId);
            //        }
            //    }
            //}
            // 放入名称
            divitem.append('<span class="title-group-name">' + oliName + '</span>');
            item.append(divitem);
            container.append(item);

            if (SampleJSONData[j]['subs']) {
                let titlediv = $('<div class="tree-sub-body"></div>');
                item.append(titlediv);
                let subul = $('<ul></ul>');
                titlediv.append(subul);
                this.createitem(SampleJSONData[j]['subs'], subul);
            }
        }
    };

    ComboTree.prototype.checkrow = function (target, oliId, ischeckbox) {
        let _this = this;
        target.addClass("actived_li"); //点击当前的添加   actived_li这个类；
        // 判断当前元素前面是否有checkbox，如果有就选中
        let inputcheckbox = target.find('input');
        if (inputcheckbox.length > 0 && !ischeckbox) {
            inputcheckbox.prop('checked', !inputcheckbox.prop('checked'));
        }
        
        _this.oliIdArray.push(oliId);
        _this.oliNameArray.push(target.attr('data-name'));
        _this.roleSelect.attr("data-id", _this.oliIdArray); //把当前点击的oliId赋值到显示的input的oliId里面
        _this.roleSelect.attr("data-name", _this.oliNameArray)
        var vals = $("#TB_" + this.options.keyOfEn).val();
        
        $("#TB_" + this.options.keyOfEn).val(vals + "[" + target.attr('data-name')+"]");

        if (_this.oliIdArray.length > 0) {
            _this.myplaceholder.hide();
        }
        let length = target.attr('data-name').length;
        //向input里面存放的内容，是一个span //onchange='_this.ChangeDataName(this.value,\""+ oliId+"\")'
        let item, namespan, checkicon;
        let signature
        if (_this.options.isShowSignature == false) {
            item = $("<span data-id='" + oliId + "' class='input-keyword-item'></span>");
            namespan = $("<input type='text' style='border:0px;width:" + length * 15 + "px;margin:1px 1px !important;' value='" + target.attr('data-name') + "' onchange='ChangeDataName(\"" + _this.options.keyOfEn + "\")'  />");
            checkicon = $("<i class='close' data-id='" + oliId + "' >x</i>");
        } else {
            item = $("<span data-id='" + oliId + "' class='only-print-hidden input-keyword-item'></span>");
            namespan = $("<input type='text' style='border:0px;width:" + length * 15 + "px;margin:1px 1px !important;' value='" + target.attr('data-name') + "' onchange='ChangeDataName(\"" + _this.options.keyOfEn + "\")'  />");
            checkicon = $("<i class='close' data-id='" + oliId + "' >x</i>");
            if ("undefined" == typeof UserICon) {
                UserICon = '../../DataUser/Siganture/';
            } else {
                UserICon = UserICon.replace("@basePath", basePath);
            }
            if ("undefined" == typeof UserIConExt) {
                UserIConExt = '.jpg';
            }
            signature = $("<img src='" + UserICon + oliId + UserIConExt + "' class='only-print-show' id='Img_" + oliId + "'/>");
        }



        item.append(namespan);
        if (_this.options.isMultiple) {
            item.append(checkicon);
        } else {
            item.addClass('single-keyword')
        }

        _this.roleSelect.append(item);

        //打印显示签名
        if (_this.options.isShowSignature == true)
            _this.roleSelect.append(signature);

       
    };

    ComboTree.prototype.uncheckrow = function (oliId) {
        let _this = this, icon = null;
        let id = null;

        for (let i = 0; i < _this.oliIdArray.length; i++) {
            if (_this.oliIdArray[i] === oliId) { //表示数组里面有这个元素
                id = i; //元素位置
                _this.oliIdArray.splice(i, 1);
                _this.oliNameArray.splice(i, 1);
                //把当前点击的oliId赋值到显示的input的oliId里面
                _this.roleSelect.attr("data-id", _this.oliIdArray);
                _this.roleSelect.attr("data-name", _this.oliNameArray);

                var vals = $("#TB_" + this.options.keyOfEn).val();
                //转换成数组
                vals = vals.replace(new RegExp("[[]", "gm"), "").replace(/]/g, ",");
                vals = vals.substr(0, vals.length-1);
                var valArray = vals.split(",");
                valArray.splice(i, 1);
                $("#Img_" + oliId).remove();

                $("#TB_" + this.options.keyOfEn).val("[" + valArray.join("][") + "]");
                // console.log('删除当前的序号' + oliId + ';' + '剩下数组' + _this.oliIdArray)
            }
        }

        $(_this.element).find('.title-container').each(function (index, item) {
            if ($(item).attr('data-id') === oliId) {
                $(item).removeClass('actived_li');
                let $checkbox = $(item).find('input');
                $checkbox.prop('checked', false);
            }
        });

        if (!icon) {
            icon = $(_this.element).find('.input-keyword-wrap>i');
            icon.removeClass("fa-caret-up").addClass("fa-caret-down"); //点击input选择适合，小图标动态切换
        }

        _this.roleSelect.find('>span').each(function (index, item) {
            if ($(item).attr('data-id') === oliId) {
                item.remove();
            }
        });

        if (_this.oliIdArray.length === 0) {
            _this.myplaceholder.show();
        }
    };

    ComboTree.prototype.hideul = function () {
        // event=event||window.event;
        $(this.element).find('.input-keyword-wrap .fa').removeClass("fa-caret-up").addClass("fa-caret-down"); //当点隐藏ul弹窗时候，把小图标恢复原状
        $(this.element).find('.drop-down-wrap').hide(); //当点击空白处，隐藏ul弹窗
    };

    /**
     * 清空搜索输入框里面的内容
     */
    ComboTree.prototype.clearSearchValue = function () {
        console.log(3333);
    };

    ComboTree.prototype.datas = function () {

        let arr = [];
        $(this.element).find('.input-keyword-item').each(function (index, item) {
            arr.push({
                id: $(item).attr('data-id'),
                val: $(item).find('span').html()
            });
        });
        return arr;
    };

    function appendcheckbox(divitem, oliId) {
        let $checkboxspan = $('<span class="handle-checkbox"></span>'),
            $checkbox = $('<input data-id="' + oliId + '" type="checkbox">');
        $checkboxspan.append($checkbox);
        divitem.append($checkboxspan);
    }

   
    $.fn[comboTreePlugin] = function (options) {
        var ctArr = [];
        this.each(function () {
            if (!$.data(this, 'plugin_' + comboTreePlugin)) {
                $.data(this, 'plugin_' + comboTreePlugin, new ComboTree(this, options));
                ctArr.push($(this).data()['plugin_' + comboTreePlugin]);
            }
        });

        if (this.length === 1)
            return ctArr[0];
        else
            return ctArr;
    };

})(jQuery, window, document);

function ChangeDataName(keyOfEn) {
    var inputs = $("#" + keyOfEn + "_comboTree .role_select input");
    var vals = "";
    $.each(inputs, function (i, tag) {
        if (i == 0)
            vals += "[";
        vals += tag.value + "][";
    });
    vals = vals.substr(0, vals.length - 1);
    $("#TB_" + keyOfEn).val(vals);
}

