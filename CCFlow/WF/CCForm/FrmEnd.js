var frmAttrData = [];
function LoadFrmDataAndChangeEleStyle(frmData) {

    //加入隐藏控件.
    var mapAttrs = frmData.Sys_MapAttr;
    var html = "";
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0 && $("#TB_"+mapAttr.KeyOfEn).length ==0) {
            var defval = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            html = "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + defval + "' />";
            html = $(html);
            $('#CCForm').append(html);
        }
    }

 
    //为控件赋值.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        $('#TB_' + mapAttr.KeyOfEn).attr("name", "TB_" + mapAttr.KeyOfEn);
        $('#DDL_' + mapAttr.KeyOfEn).attr("name", "DDL_" + mapAttr.KeyOfEn);
        $('#CB_' + mapAttr.KeyOfEn).attr("name", "CB_" + mapAttr.KeyOfEn);

        var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (mapAttr.DefValType == 0 && mapAttr.DefVal == "10002" && (val == "10002"||val=="10002.0000"))
            val = "";
        frmAttrData.push({ "KeyOfEn": mapAttr.KeyOfEn, "Val": val });

        if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {
            var uiBindKey = mapAttr.UIBindKey;
            if (uiBindKey != null && uiBindKey != undefined && uiBindKey != "") {
                var sfTable = new Entity("BP.Sys.SFTable");
                sfTable.SetPKVal(uiBindKey);
                var count = sfTable.RetrieveFromDBSources();

                if (count != 0 && sfTable.CodeStruct == "1") {
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
                    handler.AddPara("EnsName", uiBindKey);  //增加参数.
                    //获得map基本信息.
                    var pushData = handler.DoMethodReturnString("Tree_Init");
                    if (pushData.indexOf("err@") != -1) {
                        alert(pushData);
                        continue;
                    }
                    pushData = ToJson(pushData);
                    $('#DDL_' + mapAttr.KeyOfEn).combotree('loadData', pushData);
                    if (mapAttr.UIIsEnable == 0)
                        $('#DDL_' + mapAttr.KeyOfEn).combotree({ disabled: true });

                    $('#DDL_' + mapAttr.KeyOfEn).combotree('setValue', val);
                    continue;
                }
            }
            
        }
        if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
            // 判断下拉框是否有对应option, 若没有则追加
            if (val != "" && $("option[value='" + val + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                var mainTable = frmData.MainTable[0];
                var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                if (selectText == null || selectText == undefined || selectText == "")
                    selectText = mainTable[mapAttr.KeyOfEn + "T"];

                if (selectText != null && selectText != undefined && selectText != "")
                    $('#DDL_' + mapAttr.KeyOfEn).append("<option value='" + val + "'>" + selectText + "</option>");
            }
            if (val != "")
                $('#DDL_' + mapAttr.KeyOfEn).val(val);
            continue;
        }

        $('#TB_' + mapAttr.KeyOfEn).val(val);

        //文本框.
        if (mapAttr.UIContralType == 0) {
            if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {
                $('#editor').val(val);
            } else {
                if (mapAttr.MyDataType == 8 && val!="") {
                    //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
                    var attrdefVal = mapAttr.DefVal;
                    var bit;
                    if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
                        bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
                    else
                        bit = 2;
                    if (bit == 2)
                        val = formatNumber(val, 2, ",");
                }
                $('#TB_' + mapAttr.KeyOfEn).val(val);
            }
            continue;
        }

        //checkbox.
        if (mapAttr.UIContralType == 2) {
            if (val == "1")
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", "true");
            else
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
        }

        //枚举
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            $("#RB_" + mapAttr.KeyOfEn + "_" + val).attr("checked", 'checked');
        }

        //枚举复选框
        if (mapAttr.MyDataType == 1 && mapAttr.LGType == 1) {
            var checkBoxArray = val.split(",");
            for (var k = 0; k < checkBoxArray.length; k++) {
                $("input[name='CB_" + mapAttr.KeyOfEn + "']").each(function () {
                    if ($(this).val() == checkBoxArray[k]) {
                        $(this).attr("checked", true);
                    }
                });
            }
        }
    }


    //设置为只读的字段.
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        //设置文本框只读.
        if (mapAttr.UIVisible != 0 && (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0 || pageData.IsReadonly == "1")) {
            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#RB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#TB_' + mapAttr.KeyOfEn).removeClass("form-control");
            $('#CB_' + mapAttr.KeyOfEn).removeClass("form-control");
            $('#RB_' + mapAttr.KeyOfEn).removeClass("form-control");
            $('#DDL_' + mapAttr.KeyOfEn).removeClass("form-control");
            if (mapAttr.MyDataType == "8")
                $('#TB_' + mapAttr.KeyOfEn).css("text-align", "");
        }
    }

    var mapAttrs = frmData.Sys_MapAttr;
    var mapData = frmData.Sys_MapData[0];
    var frmType = mapData.FrmType;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        if (mapAttr.LGType != 1 && mapAttr.MyDataType != 4)
            continue;

        //傻瓜表单/累加表单
        if (frmType == 0 || frmType == 10) {
            InitFoolLink(mapAttr, frmType); 
            continue;
        }
        //开发者表单
        if (frmType == 8) {
            InitDevelopLink(mapAttr, frmType);
        }

        
    }


}

//傻瓜表单/累加表单初始化联动
function InitFoolLink(mapAttr, frmType) {
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            if (mapAttr.UIContralType == 1) {
                /*启用了显示与隐藏.*/
                var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                //如果现在是隐藏状态就不可以设置
                var ctrl = $("#Td_" + mapAttr.KeyOfEn);
                if (ctrl.length > 0) {
                    if (ctrl.parent('tr').css('display') == "none")
                        return;
                }

                //初始化页面的值
                var nowKey = ddl.val();
                if (nowKey == null || nowKey == undefined || nowKey == "")
                    return;

                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey,frmType);

            }
            if (mapAttr.UIContralType == 3) {
                //如果现在是隐藏状态就不可以设置
                var ctrl = $("#Td_" + mapAttr.KeyOfEn);
                if (ctrl.length > 0) {
                    if (ctrl.parent('tr').css('display') == "none")
                        return;
                }

                var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                if (nowKey == null || nowKey == undefined || nowKey == "")
                    return;
                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);

            }
        }
    }

    //复选框
    if (mapAttr.MyDataType == 4 && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
        //获取复选框的值
        if ($("#CB_" + mapAttr.KeyOfEn).checked == true)
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 1, frmType);
        else
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 0, frmType);
    }

}

//开发者初始化联动
function InitDevelopLink(mapAttr, frmType) {
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            if (mapAttr.UIContralType == 1) {
                /*启用了显示与隐藏.*/
                var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                //如果现在是隐藏状态就不可以设置
                if (ddl.length > 0) {
                    if (ddl.css('display') == "none")
                        return;
                }
                //初始化页面的值
                var nowKey = ddl.val();
                if (nowKey == null || nowKey == undefined || nowKey == "")
                    return;

                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);

            }
            if (mapAttr.UIContralType == 3) {
                //如果现在是隐藏状态就不可以设置
                var ctrl = $("#SR_" + mapAttr.KeyOfEn);
                if (ctrl.length > 0) {
                    if (ctrl.parent('tr').css('display') == "none")
                        return;
                }

                var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                if (nowKey == null || nowKey == undefined || nowKey == "")
                    return;
                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);

            }
        }
    }

    //复选框
    if (mapAttr.MyDataType == 4 && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
        //获取复选框的值
        if ($("#CB_" + mapAttr.KeyOfEn).checked == true)
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 1, frmType);
        else
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 0, frmType);
    }

}

// 处理流程绑定表单字段权限的问题
function SetFilesAuth(FK_Node, Fk_Flow, FK_MapData) {
    var frmSlns = new Entities("BP.WF.Template.FrmFields", "FK_Node", FK_Node, "FK_MapData", FK_MapData);
    if (frmSlns == null || frmSlns.length == 0)
        return;
    for (var i = 0; i < frmSlns.length; i++) {
        var frmSln = frmSlns[i];
        var keyOfEn = frmSln.KeyOfEn;
        var myPk = FK_MapData + "_" + keyOfEn;
        var mapAttr = new Entity("BP.Sys.MapAttr", myPk);
        //checkbox复选框
        if (mapAttr.MyDataType == 4) {
            delFile(frmSln, $("#CB_" + keyOfEn), keyOfEn);

            //初始值
            if (frmSln.DefVal != null && frmSln.DefVal == 1)
                $("#CB_" + keyOfEn).attr("checked", "checked");
            else
                $("#CB_" + keyOfEn).attr("checked", "");

            continue;
        }
        //外部数据源
        if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {
            delFile(frmSln, $("#DDL_" + keyOfEn), keyOfEn);

            if (frmSln.DefVal != null && frmSln.DefVal != "")
                $("#DDL_" + keyOfEn).val(frmSln.DefVal);

            continue;

        }
        //外键类型.
        if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {
            delFile(frmSln, $("#DDL_" + keyOfEn), keyOfEn);

            if (frmSln.DefVal != null && frmSln.DefVal != "")
                $("#DDL_" + keyOfEn).val(frmSln.DefVal);
            continue;

        }
        //枚举类型.
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            //单选按钮
            if (mapAttr.UIContralType == 3) {
                frmSln.UIVisible == 0 ? $('input[name=RB_' + keyOfEn + ']').hide() : $('input[name=RB_' + keyOfEn + ']').show()

                //是否可用
                frmSln.UIIsEnable == 0 ? $('input[name=RB_' + keyOfEn + ']').attr("disabled", "disabled") : $('input[name=RB_' + keyOfEn + ']').attr("disabled", "");

                //是否必填
                //var showSpan = '<span style="color:red" class="mustInput" data-keyofen="' + keyOfEn + '">*</span>' ;
                //frmSln.IsNotNull==1?fileId.append(showSpan):fileId.append("");
                if (frmSln.DefVal != null && frmSln.DefVal != "")
                    document.getElementById("RB_" + keyOfEn + "_" + frmSln.DefVal).checked = true;
            } else {
                delFile(frmSln, $("#DDL_" + keyOfEn), keyOfEn);
                if (frmSln.DefVal != null && frmSln.DefVal != "")
                    $("#DDL_" + keyOfEn).val(frmSln.DefVal);
            }

            continue;
        }

        //其余为文本框显示
        delFile(frmSln, $("#TB_" + keyOfEn), keyOfEn);
        if (frmSln.DefVal != null && frmSln.DefVal != "")
            $("#TB_" + keyOfEn).val(frmSln.DefVal);
        $("#TB_" + keyOfEn).attr("onblur", frmSln.RegularExp);

    }


}

function delFile(frmSln, fileId, KeyOfEn) {
    frmSln.UIVisible == 0 ? fileId.hide() : fileId.show();

    //是否可用
    frmSln.UIIsEnable == 0 ? fileId.attr("disabled", "disabled") : fileId.attr("disabled", "");

    //是否必填
    if (frmSln.IsNotNull == 1) {
        var showSpan = '<span style="color:red" class="mustInput" data-keyofen="' + KeyOfEn + '">*</span>';
        fileId.after(showSpan);
    }

}


//处理 MapExt 的扩展. 工作处理器，独立表单都要调用他.
function AfterBindEn_DealMapExt(frmData) {

    var mapExts = frmData.Sys_MapExt;
    var mapAttrs = frmData.Sys_MapAttr;
    // 主表扩展(统计从表)
    var detailExt = {};

    //获取当前人员
    var webUser = new WebUser();

    for (var i = 0; i < mapExts.length; i++) {
        var mapExt1 = mapExts[i];

        //一起转成entity.
        var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
        mapExt.MyPK = mapExt1.MyPK;

        if (mapExt.ExtType == "DtlImp"
            || mapExt.MyPK.indexOf(mapExt.FK_MapData + '_Table') >= 0
            || mapExt.MyPK.indexOf('PageLoadFull') >= 0
            || mapExt.ExtType == 'StartFlow')
            continue;

        if (mapExt.AttrOfOper == '')
            continue; //如果是不操作字段，就conntinue;

        var mapAttr1 = null;
        for (var j = 0; j < mapAttrs.length; j++) {
            if (mapAttrs[j].FK_MapData == mapExt.FK_MapData && mapAttrs[j].KeyOfEn == mapExt.AttrOfOper) {
                mapAttr1 = mapAttrs[j];
                break;
            }
        }
        if (mapAttr1 == null)
            continue;

        var mapAttr = new Entity("BP.Sys.MapAttr", mapAttr1);
        mapAttr.MyPK = mapAttr1.MyPK;
        //判断MapAttr属性是否可编辑不可以编辑返回
        if (mapAttr.UIVisible == 0)
            continue;

        //var mapAttr = new Entity("BP.Sys.MapAttr");
        //mapAttr.SetPKVal(mapExt.FK_MapData + "_" + mapExt.AttrOfOper);
        ////由于客户pop有实效问题，此处暂时注掉
        //if (mapAttr.RetrieveFromDBSources() == 0) {
        //    //mapExt.Delete();
        //    continue;
        //}
       

        //处理Pop弹出框
        var PopModel = mapAttr.GetPara("PopModel");

        if (PopModel != undefined && PopModel != "" && mapExt.ExtType == mapAttr.GetPara("PopModel") && mapAttr.GetPara("PopModel") != "None") {
            PopMapExt(mapAttr, mapExt, frmData);
            continue;
        }

        //处理文本自动填充
        var TBModel = mapAttr.GetPara("TBFullCtrl");
        if (TBModel != undefined && TBModel != "" && TBModel != "None" && (mapExt.ExtType == "FullData")) {
            var tbAuto = $("#TB_" + mapExt.AttrOfOper);
            if (tbAuto == null)
                continue;

            tbAuto.attr("onkeyup", "DoAnscToFillDiv(this,this.value,\'TB_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\',\'" + TBModel + "\');");
            tbAuto.attr("AUTOCOMPLETE", "OFF");
            continue;
        }

        //下拉框填充其他控件
        var DDLFull = mapAttr.GetPara("IsFullData");
        if (DDLFull != undefined && DDLFull != "" && DDLFull == "1" && (mapExt.MyPK.indexOf("DDLFullCtrl") != -1)) {
            //枚举类型
            if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1 && mapAttr.UIContralType == 3) {
                var ddlOper = $('input:radio[name="RB_' + mapExt.AttrOfOper + '"]');
                if (ddlOper.length == 0)
                    continue;
                var enName = frmData.Sys_MapData[0].No;

                ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");

                //初始化填充数据
                var val = $('input:radio[name="RB_' + mapExt.AttrOfOper + '"]:checked').val();
                DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper, mapExt.MyPK);
                continue;
            }

            //外键类型
            var ddlOper = $("#DDL_" + mapExt.AttrOfOper);
            if (ddlOper.length == 0)
                continue;

            var enName = frmData.Sys_MapData[0].No;

            ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");
            //初始化填充数据
            var val = ddlOper.val();
            if (val != "" && val != undefined)
                DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper, mapExt.MyPK);
            continue;
        }

        switch (mapExt.ExtType) {
            case "MultipleChoiceSmall":
                if (mapExt.DoWay == 0)
                    break;
                if (mapAttr.UIIsEnable == 0 && mapExt.Tag == 0) {
                    var oid = (pageData.WorkID || pageData.OID || "");
                    var ens = new Entities("BP.Sys.FrmEleDBs");
                    ens.Retrieve("FK_MapData", mapAttr.FK_MapData, "EleID", mapAttr.KeyOfEn, "RefPKVal", oid);
                    var val = "";
                    var defaultVal = $("#TB_" + mapAttr.KeyOfEn).val();
                    for (var k = 0; k < ens.length; k++) {
                        if (defaultVal.indexOf(ens[k].Tag1) == -1)
                            continue;
                        val += ens[k].Tag2 + ",";
                    }                
                    if (val == "")
                        val = frmData.MainTable[0][mapAttr.KeyOfEn + "T"];
                    $("#TB_" + mapAttr.KeyOfEn).val(val);
                    break;
                }
                MultipleChoiceSmall(mapExt, mapAttr); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "MultipleChoiceSearch":
                if (mapAttr.UIIsEnable == 0)
                    break;
                MultipleChoiceSearch(mapExt); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "MultipleInputSearch":
                var defaultVal = $("#TB_" + mapAttr.KeyOfEn).val();
                if (mapAttr.UIIsEnable == 0) {
                    defaultVal = defaultVal.replace(new RegExp("[[]", "gm"), "").replace(/] /g, "");
                    defaultVal = defaultVal.substr(0, defaultVal.length - 1);
                    $("#TB_" + mapAttr.KeyOfEn).val(vals);
                    break;
                }
                    
                
                MultipleInputSearch(mapExt, defaultVal); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "BindFunction": //控件绑定函数
                if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7) {
                    if ($('#TB_' + mapExt.AttrOfOper).length == 1) {
                        var minDate = $('#TB_' + mapExt.AttrOfOper).attr("data-info");
                        $('#TB_' + mapExt.AttrOfOper).attr("data-funcionPK", mapExt.MyPK); // 记录绑定事件的MyPK
                        $('#TB_' + mapExt.AttrOfOper).removeAttr("onfocus");
                        $('#TB_' + mapExt.AttrOfOper).unbind("focus");
                        var frmDate = mapAttr.IsSupperText; //获取日期格式
                        var dateFmt = '';
                        if (frmDate == 0) {
                            dateFmt = "yyyy-MM-dd";
                        } else if (frmDate == 1) {
                            dateFmt = "yyyy-MM-dd HH:mm";
                        } else if (frmDate == 2) {
                            dateFmt = "yyyy-MM-dd HH:mm:ss";
                        } else if (frmDate == 3) {
                            dateFmt = "yyyy-MM";
                        } else if (frmDate == 4) {
                            dateFmt = "HH:mm";
                        } else if (frmDate == 5) {
                            dateFmt = "HH:mm:ss";
                        } else if (frmDate == 6) {
                            dateFmt = "MM-dd";
                        }

                        var mapextDoc = mapExt.Doc;
                        $('#TB_' + mapExt.AttrOfOper).bind("focus", function () {
                            if (minDate == "" || minDate == undefined)
                                WdatePicker({
                                    dateFmt: dateFmt, onpicked: function (dp) {
                                        $(this).blur(); //失去焦点 
                                        DBAccess.RunFunctionReturnStr(mapextDoc);
                                    }
                                });
                            else
                                WdatePicker({
                                    dateFmt: dateFmt, minDate: minDate, onpicked: function (dp) {
                                        $(this).blur(); //失去焦点 
                                        DBAccess.RunFunctionReturnStr(mapextDoc);
                                    }
                                });

                        });

                    }
                    break;
                }
                if ($('#TB_' + mapExt.AttrOfOper).length == 1) {
                    $('#TB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "TB_"));
                    break;
                }
                if ($('#DDL_' + mapExt.AttrOfOper).length == 1) {
                    $('#DDL_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "DDL_"));
                    break;
                }
                if ($('#CB_' + mapExt.AttrOfOper).length == 1) {
                    $('#CB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "CB_"));
                    break;
                }
                if ($('input[name="CB_' + mapExt.AttrOfOper + '"]').length > 1) {
                    $('input[name="CB_' + mapExt.AttrOfOper + '"]').bind(DynamicBind(mapExt, "CB_"));
                    break;
                }
                if ($('input[name="RB_' + mapExt.AttrOfOper + '"]').length > 0) {
                    $('input[name="RB_' + mapExt.AttrOfOper + '"]').bind(DynamicBind(mapExt, "RB_"));
                    break;
                }
                break;
            case "RegularExpression": //正则表达式  统一在保存和提交时检查


                var tb = $('#TB_' + mapExt.AttrOfOper);

                if (tb.attr('class') != undefined && tb.attr('class').indexOf('CheckRegInput') > 0) {
                    break;
                } else {
                    tb.addClass("CheckRegInput");
                    tb.data(mapExt)
                    tb.attr(mapExt.Tag, "CheckRegInput('" + tb.attr('name') + "','','" + mapExt.Tag1 + "')");

                }
                break;
            case "InputCheck": //输入检查
                break;
            case "FastInput": //是否启用快速录入
                if (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0 || GetQueryString("IsReadonly") == "1")
                    continue;
                var tbFastInput = $("#TB_" + mapExt.AttrOfOper);
                //获取大文本的长度
                var left = tbFastInput.parent().css('left') == "auto" ? 0 : parseInt(tbFastInput.parent().css('left').replace("px", ""));
                var width = tbFastInput.width() + left;
                width = tbFastInput.parent().css('left') == "auto" ? width - 70 : width - 180;

                var content = $("<span style='margin-left:" + width + "px;top: -15px;position: relative;'></span><br/>");
                tbFastInput.after(content);
                content.append("<a href='javascript:void(0)' onclick='TBHelp(\"TB_" + mapExt.AttrOfOper + "\",\"" + mapExt.MyPK + "\")'>常用語彙</a> <a href='javascript:void(0)' onclick='clearContent(\"TB_" + mapExt.AttrOfOper + "\")'>クリアする<a>");

                break;

            case "ActiveDDL": /*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
                if (ddlPerant == null || ddlChild == null)
                    continue;

                ddlPerant.attr("onchange", "DDLAnsc(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "\', \'" + mapExt.MyPK + "\',\'" + ddlPerant.val() + "\')");

                var valClient = ConvertDefVal(frmData, '', mapExt.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                //初始化页面时方法加载

                DDLAnsc($("#DDL_" + mapExt.AttrOfOper).val(), "DDL_" + mapExt.AttrsOfActive, mapExt.MyPK);
                break;
            case "AutoFullDLL": // 自动填充下拉框.
                continue; //已经处理了。
            case "AutoFull": //自动填充  //a+b=c DOC='@DanJia*@ShuLiang'  等待后续优化
                //循环  KEYOFEN
                //替换@变量
                //处理 +-*%

                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    continue;
                calculator(mapExt);
                break;
            case "AutoFullDtlField": //主表扩展(统计从表)
                var docs = mapExt.Doc.split("\.");

                //判断是否显示大写
                var tag3 = mapExt.Tag3;
                var DaXieAttrOfOper = "";
                if (tag3 == 1)
                    DaXieAttrOfOper = mapExt.Tag4;

                if (docs.length == 3) {
                    var ext = {
                        "DtlNo": docs[0],
                        "FK_MapData": mapExt.FK_MapData,
                        "AttrOfOper": mapExt.AttrOfOper,
                        "DaXieAttrOfOper": DaXieAttrOfOper,
                        "Doc": mapExt.Doc,
                        "DtlColumn": docs[1],
                        "exp": docs[2],
                        "Tag": mapExt.Tag,
                        "Tag1": mapExt.Tag1
                    };
                    if (!$.isArray(detailExt[ext.DtlNo])) {
                        detailExt[ext.DtlNo] = [];
                    }
                    detailExt[ext.DtlNo].push(ext);
                    //                    var iframeDtl = $("#F" + ext.DtlNo);
                    //                    iframeDtl.load(function () {
                    //                        $(this).contents().find(":input[id=formExt]").val(JSON.stringify(detailExt[ext.DtlNo]));
                    //                        if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                    //                            this.contentWindow.parentStatistics(detailExt[ext.DtlNo]);
                    //                        }
                    //                    });
                    $(":input[name=TB_" + ext.AttrOfOper + "]").attr("disabled", true);
                }
                break;

            case "SepcFieldsSepcUsers": //特殊字段的权限
                //获取字段
                var Filed = mapExt.Doc;
                //获取人员
                var emps = mapExt.Tag1;
                if (emps.indexOf(webUser.No) != -1) {
                    var fileds = Filed.split(",");
                    $.each(fileds, function (i, objID) {
                        var obj = $("#TB_" + objID);
                        if (obj.length == 0)
                            obj = $("#DDL_" + objID);
                        if (obj.length == 0)
                            obj = $("#CB_" + objID);
                        if (obj.length != 0)
                            obj.attr("disabled", false);
                    });
                }
                break;
            case "DataFieldInputRole": //时间限制
                if (mapExt.DoWay == 1) {
                    var tag1 = mapExt.Tag1;
                    if (tag1 == 1) {
                        $('#TB_' + mapExt.AttrOfOper).removeAttr("onfocus");
                        var frmDate = mapAttr.IsSupperText; //获取日期格式
                        var dateFmt = '';
                        if (frmDate == 0) {
                            dateFmt = "yyyy-MM-dd";
                        } else if (frmDate == 1) {
                            dateFmt = "yyyy-MM-dd HH:mm";
                        } else if (frmDate == 2) {
                            dateFmt = "yyyy-MM-dd HH:mm:ss";
                        } else if (frmDate == 3) {
                            dateFmt = "yyyy-MM";
                        } else if (frmDate == 4) {
                            dateFmt = "HH:mm";
                        } else if (frmDate == 5) {
                            dateFmt = "HH:mm:ss";
                        } else if (frmDate == 6) {
                            dateFmt = "MM-dd";
                        }

                        var minDate = '%y-%M-#{%d}';
                        $('#TB_' + mapExt.AttrOfOper).attr("data-info", minDate); //绑定时间大小限制的记录
                        var functionPK = $('#TB_' + mapExt.AttrOfOper).attr("data-funcionPK");
                        if (functionPK == null || functionPK == undefined || functionPK == "") {
                            $('#TB_' + mapExt.AttrOfOper).bind("focus", function () {
                                WdatePicker({ dateFmt: dateFmt, minDate: minDate });
                            });
                        } else {
                            $('#TB_' + mapExt.AttrOfOper).unbind("focus");

                            var bindFunctionExt = new Entity("BP.Sys.MapExt", functionPK);
                            $('#TB_' + mapExt.AttrOfOper).bind("focus", function () {

                                WdatePicker({
                                    dateFmt: dateFmt, minDate: minDate, onpicked: function (dp) {
                                        $(this).blur(); //失去焦点 
                                        DBAccess.RunFunctionReturnStr(bindFunctionExt.Doc);
                                    }
                                });
                            });

                        }


                    }

                }
                break;
            default:

        }
    }

    $.each(detailExt, function (idx, obj) {
        var iframeDtl = $("#Dtl_" + obj[0].DtlNo);
        iframeDtl.load(function () {
            $(this).contents().find(":input[id=formExt]").val(JSON.stringify(detailExt[obj[0].DtlNo]));
            if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                this.contentWindow.parentStatistics(detailExt[obj[0].DtlNo]);
            }
        });

    });
}

/**Pop弹出框的处理**/
function PopMapExt(mapAttr, mapExt, frmData) {
    switch (mapAttr.GetPara("PopModel")) {

        case "PopBranchesAndLeaf": //树干叶子模式.
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            PopBranchesAndLeaf(mapExt, val); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopBranches": //树干简单模式.
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            PopBranches(mapExt, val); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopBindSFTable": //绑定字典表，外部数据源.
            //var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            PopBindSFTable(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopBindEnum": //绑定枚举.
            //var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            PopBindEnum(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopTableList": //绑定实体表.
            //var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            PopTableList(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopGroupList": //分组模式.
            PopGroupList(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopSelfUrl": //自定义url.
            SelfUrl(mapExt); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
            break;
        case "PopTableSearch": //表格查询.
            PopTableSearch(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopVal": //PopVal窗返回值.
            var tb = $('[name$=' + mapExt.AttrOfOper + ']');
            tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
            tb.attr("ondblclick", "ReturnValCCFormPopValGoogle(this,'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');");

            tb.attr('readonly', 'true');
            var icon = '';
            var popWorkModelStr = '';
            var popWorkModelIndex = mapExt.AtPara != undefined ? mapExt.AtPara.indexOf('@PopValWorkModel=') : -1;
            if (popWorkModelIndex >= 0) {
                popWorkModelIndex = popWorkModelIndex + '@PopValWorkModel='.length;
                popWorkModelStr = mapExt.AtPara.substring(popWorkModelIndex, popWorkModelIndex + 1);
            }
            switch (popWorkModelStr) {
                /// <summary>                
                /// 自定义URL                
                /// </summary>                
                //SelfUrl =1,                
                case "1":
                    icon = "glyphicon glyphicon-th";
                    break;
                /// <summary>                
                /// 表格模式                
                /// </summary>                
                // TableOnly,                
                case "2":
                    icon = "glyphicon glyphicon-list";
                    break;
                /// <summary>                
                /// 表格分页模式                
                /// </summary>                
                //TablePage,                
                case "3":
                    icon = "glyphicon glyphicon-list-alt";
                    break;
                /// <summary>                
                /// 分组模式                
                /// </summary>                
                // Group,                
                case "4":
                    icon = "glyphicon glyphicon-list-alt";
                    break;
                /// <summary>                
                /// 树展现模式                
                /// </summary>                
                // Tree,                
                case "5":
                    icon = "glyphicon glyphicon-tree-deciduous";
                    break;
                /// <summary>                
                /// 双实体树                
                /// </summary>                
                // TreeDouble                
                case "6":
                    icon = "glyphicon glyphicon-tree-deciduous";
                    break;
                default:
                    break;
            }
            tb.width(tb.width() - 40);
            tb.height('auto');
            var eleHtml = ' <div class="input-group form_tree" style="width:' + tb.width() + 'px;height:' + tb.height() + 'px">' + tb.parent().html() +
                '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle(document.getElementById('TB_" + mapExt.AttrOfOper + "'),'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
            tb.parent().html(eleHtml);
            break;
        default: break;
    }
}


function TBHelp(ObjId, MyPK) {
    var url = basePath + "/WF/CCForm/Pop/HelperOfTBEUIBS.htm?PKVal=" + MyPK + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    var W = document.body.clientWidth / 2.5;//- 500;
    var H = document.body.clientHeight / 1.5; //- 140;
    //var str = OpenEasyUiDialogExt(url, "词汇选择", W, H, false);
    OpenBootStrapModal(url, "TBHelpIFram", "語彙の選択", W, H);
}
function changeFastInt(ctrl, value) {
    $("#TB_" + ctrl).val(value);
    if ($('#eudlg').length > 0)
        $('#eudlg').window('close');
    if ($('#bootStrapdlg').length > 0)
        $('#bootStrapdlg').modal('hide');
}
function clearContent(ctrl) {
    $("#" + ctrl).val("");
}
function DynamicBind(mapExt, ctrlType) {

    if (ctrlType == "RB_") {
        $('input[name="' + ctrlType + mapExt.AttrOfOper + '"]').on(mapExt.Tag, function () {
            DBAccess.RunFunctionReturnStr(mapExt.Doc);
        });
    } else if (ctrlType == "CB_") {
        $('input[name="' + ctrlType + mapExt.AttrOfOper + '"]').on(mapExt.Tag, function () {
            DBAccess.RunFunctionReturnStr(mapExt.Doc);
        });
    }
    else {
        $('#' + ctrlType + mapExt.AttrOfOper).on(mapExt.Tag, function () {
            DBAccess.RunFunctionReturnStr(mapExt.Doc);
        });
    }


}

/**
* 表单计算(包括普通表单以及从表弹出页表单)
*/
function calculator(o) {
    if (!testExpression(o.Doc)) {
        console.log("MyPk: " + o.MyPK + ", 表現式: '" + o.Doc + "'フォーマットエラー");
        return false;
    }
    var targets = [];
    var index = -1;
    for (var i = 0; i < o.Doc.length; i++) {	// 对于复杂表达式需要重点测试
        var c = o.Doc.charAt(i);
        if (c == "(") {
            index++;
        } else if (c == ")") {
            targets.push(o.Doc.substring(index + 1, i));
            i++;
            index = i;
        } else if (/[\+\-|*\/]/.test(c)) {
            targets.push(o.Doc.substring(index + 1, i));
            index = i;
        }
    }
    if (index + 1 < o.Doc.length) {
        targets.push(o.Doc.substring(index + 1, o.Doc.length));
    }
    //
    var expression = {
        "judgement": [],
        "execute_judgement": [],
        "calculate": o.Doc
    };
    $.each(targets, function (i, o) {
        var target = o.replace("@", "");
        var element = "$(':input[name=TB_" + target + "]')";
        expression.judgement.push(element + ".length == 0");
        expression.execute_judgement.push("!isNaN(parseFloat(" + element + ".val()))");
        expression.calculate = expression.calculate.replace(o, "parseFloat(" + element + ".val())");
    });
    (function (targets, expression, resultTarget, pk, expDefined) {
        $.each(targets, function (i, o) {

            var target = o.replace("@", "");

            $(":input[name=TB_" + target + "]").bind("change", function () {

                var evalExpression = " var result = ''; ";
                if (expression.judgement.length > 0) {
                    evalExpression += " if ( " + expression.judgement.join(" || ") + " ) { ";
                    evalExpression += " 	alert(\"MyPk: " + pk + ",  表現式: '" + expDefined + "' " + "の中にオブジェクトが現在のページに存在しません\");"
                    // evalExpression += " 	console.log(\"MyPk: " + pk + ", 表达式: '" + expDefined + "' " + "中有对象在当前页面不存在\");"

                    evalExpression += " } ";
                }
                if (expression.execute_judgement.length > 0) {
                    evalExpression += " else if ( " + expression.execute_judgement.join(" && ") + " ) { ";
                }
                if (expression.calculate.length > 0) {
                    evalExpression += " 	result = " + expression.calculate + "; ";
                }
                if (expression.execute_judgement.length > 0) {
                    evalExpression += " } ";
                }

                eval(evalExpression);


                $(":input[name=TB_" + resultTarget + "]").val(typeof result == "undefined" ? "" : result);
            });
            if (i == 0) {
                $(":input[name=TB_" + target + "]").trigger("change");
            }
        });
    })(targets, expression, o.AttrOfOper, o.MyPK, o.Doc);
    $(":input[name=TB_" + o.AttrOfOper + "]").attr("disabled", true);
}

function testExpression(exp) {
    if (exp == null || typeof exp == "undefined" || typeof exp != "string") {
        return false;
    }
    exp = exp.replace(/\s/g, "");
    if (exp == "" || exp.length == 0) {
        return false;
    }
    if (/[\+\-\*\/]{2,}/.test(exp)) {
        return false;
    }
    if (/\(\)/.test(exp)) {
        return false;
    }
    var stack = [];
    for (var i = 0; i < exp.length; i++) {
        var c = exp.charAt(i);
        if (c == "(") {
            stack.push("(");
        } else if (c == ")") {
            if (stack.length > 0) {
                stack.pop();
            } else {
                return false;
            }
        }
    }
    if (stack.length != 0) {
        return false;
    }
    if (/^[\+\-\*\/]|[\+\-\*\/]$/.test(exp)) {
        return false;
    }
    if (/\([\+\-\*\/]|[\+\-\*\/]\)/.test(exp)) {
        return false;
    }
    return true;
}

/** 为了保障以前的业务逻辑兼容性，特把旧方法移植到这里. **/
// 获取DDL值
function ReqDDL(ddlID) {
    var v = document.getElementById('DDL_' + ddlID).value;
    if (v == null) {
        alert('見つからないID=' + ddlID + 'のドロップダウンボックスコントロール。');
        return;
    }
    return v;
}

// 获取TB值
function ReqTB(tbID) {
    var v = document.getElementById('TB_' + tbID).value;
    if (v == null) {
        alert('見つからないID=' + tbID + 'のテキストボックスコントロール。');
        return;
    }
    return v;
}

// 获取CheckBox值
function ReqCB(cbID) {
    var v = document.getElementById('CB_' + cbID).value;
    if (v == null) {
        alert('見つからないID=' + cbID + 'のテキストボックスコントロール。');
        return;
    }
    return v;
}

// 获取 单选按钮的 值.
function ReqRadio(keyofEn, enumIntVal) {
    var v = document.getElementById('RB_' + keyofEn + '' + enumIntVal);
    if (v == null) {
        alert('見つからないフィールド名=' + keyofEn + '値=' + enumIntVal + 'のコントロール');
        return;
    }
    return v.checked;
}

/// 获取DDL Obj
function ReqDDLObj(ddlID) {
    var v = document.getElementById('DDL_' + ddlID);
    if (v == null) {
        alert('見つからないID=' + ddlID + 'のドロップダウンボックスコントロール。');
    }
    return v;
}
// 获取TB Obj
function ReqTBObj(tbID) {
    var v = document.getElementById('TB_' + tbID);
    if (v == null) {
        alert('見つからないID=' + tbID + 'のテキストボックスコントロール。');
    }
    return v;
}
// 获取CheckBox Obj值
function ReqCBObj(cbID) {
    var v = document.getElementById('CB_' + cbID);
    if (v == null) {
        alert('見つからないID=' + cbID + 'の単一選択肢のコントロール');
    }
    return v;
}

//设置值?
function SetCtrlVal(key, value) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        return;
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        return;
    }

  
    ctrl = $("input[name='CB_" + key + "']");
    if (ctrl.length == 1) {
        ctrl.val(value);
        if (parseInt(value) <= 0)
            ctrl.attr('checked', false);
        else
            ctrl.attr('checked', true);
        return;
    }
    if (ctrl.length > 1) {
        var checkBoxArray = value.split(",");
        ctrl.attr("checked", false);

        for (var k = 0; k < checkBoxArray.length; k++) {
            if (checkBoxArray[k] == "")
                continue;
            document.getElementById("CB_" + key + "_" + checkBoxArray[k]).checked = true;
        }
        return;
    }

    ctrl = $('input:radio[name=RB_' + key + ']');
    if (ctrl.length > 0) {
        var checkVal = $('input:radio[name=RB_' + key + ']:checked').val();
        if(checkVal!=null && checkVal!=undefined)
            document.getElementById("RB_" + key + "_" + checkVal).checked = false;
        if ($("#RB_" + key + "_" + value).length==1)
            document.getElementById("RB_" + key + "_" + value).checked = true;
        return;
    }
}

//清空值?
function CleanCtrlVal(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val('');
        return;
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        //ctrl.attr("value",'');
        ctrl.val('');
        return;
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr('checked', false);
        return;
    }

    ctrl = $("#RB_" + key + "_" + 0);
    if (ctrl.length > 0) {
        var checkVal = $('input:radio[name=RB_' + key + ']:checked').val();
        if (checkVal != null && checkVal != undefined)
            document.getElementById("RB_" + key + "_" + checkVal).checked = false;
        return;
    }
}

//显示大图
function imgShow(outerdiv, innerdiv, bigimg, _this) {
    var src = _this.attr("src"); //获取当前点击的pimg元素中的src属性  
    $(bigimg).attr("src", src); //设置#bigimg元素的src属性  

    /*获取当前点击图片的真实大小，并显示弹出层及大图*/
    $("<img/>").attr("src", src).load(function () {
        var windowW = $(window).width(); //获取当前窗口宽度  
        var windowH = $(window).height(); //获取当前窗口高度  
        var realWidth = this.width; //获取图片真实宽度  
        var realHeight = this.height; //获取图片真实高度  
        var imgWidth, imgHeight;
        var scale = 0.8; //缩放尺寸，当图片真实宽度和高度大于窗口宽度和高度时进行缩放  

        if (realHeight > windowH * scale) {//判断图片高度  
            imgHeight = windowH * scale; //如大于窗口高度，图片高度进行缩放  
            imgWidth = imgHeight / realHeight * realWidth; //等比例缩放宽度  
            if (imgWidth > windowW * scale) {//如宽度扔大于窗口宽度  
                imgWidth = windowW * scale; //再对宽度进行缩放  
            }
        } else if (realWidth > windowW * scale) {//如图片高度合适，判断图片宽度  
            imgWidth = windowW * scale; //如大于窗口宽度，图片宽度进行缩放  
            imgHeight = imgWidth / realWidth * realHeight; //等比例缩放高度  
        } if (realHeight > windowH * scale) {
            imgWidth = windowH * scale;
            imgHeight = windowH * scale;
        } else {//如果图片真实高度和宽度都符合要求，高宽不变  
            imgWidth = realWidth;
            imgHeight = realHeight;
        }
        $(bigimg).css("width", imgWidth); //以最终的宽度对图片缩放  

        var w = (windowW - imgWidth) / 2; //计算图片与窗口左边距  
        var h = (windowH - imgHeight) / 2; //计算图片与窗口上边距  
        $(innerdiv).css({ "top": h, "left": w }); //设置#innerdiv的top和left属性  
        $(outerdiv).fadeIn("fast"); //淡入显示#outerdiv及.pimg  
    });

    $(outerdiv).click(function () {//再次点击淡出消失弹出层  
        $(this).fadeOut("fast");
    });
}  