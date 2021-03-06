
var frmData = null;
var IsLoadUEditor = false;
function GenerFoolFrm(mapData, frmData) {

    frmData = frmData;
    var Sys_GroupFields = frmData.Sys_GroupField;

    var node = frmData.WF_Node;
    if (node != undefined)
        node = node[0];

    var frmNode = frmData.WF_FrmNode;
    if (frmNode != undefined)
        frmNode = frmNode[0];
    var tableCol = frmData.Sys_MapData[0].TableCol;
    if (tableCol == 0)
        tableCol = 4;
    else if (tableCol == 1)
        tableCol = 6;
    else if (tableCol == 2)
        tableCol = 3;
    else
        tableCol = 4;


    var tableWidth = 800; //  w - 40;
    var html = "<table style='width:100%;' >";
    var frmName = mapData.Name;
    html += "<tr>";
    html += "<td colspan='" + tableCol + "' class='TitleFDesc' ><div style='float:left' ><img src='../../DataUser/ICON/LogBiger.png'  style='height:50px;' /></div><div class='form-unit-title' style='float:right;padding:10px;bordder:none;width:70%;font-size: 18px;' ><center><h4><b>" + frmName + "</b></h4></center></div></td>";
    html += "</tr>";


    //遍历循环生成 listview
    for (var i = 0; i < Sys_GroupFields.length; i++) {

        var gf = Sys_GroupFields[i];

        //从表..
        if (gf.CtrlType == 'Dtl') {


            var dtls = frmData.Sys_MapDtl;

            for (var k = 0; k < dtls.length; k++) {

                var dtl = dtls[k];

                if (dtl.No != gf.CtrlID)
                    continue;

                html += "<tr>";
                html += "  <th colspan='" + tableCol + "' class='form-unit'>" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr>";
                html += "  <td colspan='" + tableCol + "' class='FDesc'>";

                html += Ele_Dtl(dtl);

                html += "  </td>";
                html += "</tr>";
            }
            continue;
        }


        //附件类的控件.
        if (gf.CtrlType == 'Ath') {

            //获取附件的主键
            var MyPK = gf.CtrlID;
            if (MyPK == "")
                continue;
            //创建附件描述信息.
            var ath = new Entity("BP.Sys.FrmAttachment");
            ath.MyPK = gf.CtrlID;
            if (ath.RetrieveFromDBSources() == 0)
                continue;
            if (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck")
                continue;

            html += "<tr>";
            html += "  <th colspan='" + tableCol + "' class='form-unit'>" + gf.Lab + "</th>";
            html += "</tr>";


            html += "<tr>";
            html += "  <td colspan='" + tableCol + "' class='FDesc'>";

            html += Ele_Attachment(frmData, gf);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //父子流程
        if (gf.CtrlType == 'SubFlow' && node.SFSta != 0) {
            html += "<tr>";
            html += "  <th colspan='" + tableCol + "' class='form-unit'>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='" + tableCol + "' class='FDesc'>";

            html += Ele_SubFlow(node);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //框架类的控件.
        if (gf.CtrlType == 'Frame') {

            html += "<tr>";
            html += "  <th colspan='" + tableCol + "' class='form-unit'>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='" + tableCol + "' class='FDesc'>";

            html += Ele_Frame(frmData, gf);

            html += "  </td>";
            html += "</tr>";

            continue;
        }


        //审核组件,有节点信息,并且当前节点状态不是禁用的,就可以显示.
        if (gf.CtrlType == 'FWC' && node && node.FWCSta != 0) {
            if (node.FormType != 5 || (node.FormType == 5 && frmNode && frmNode.IsEnableFWC == 1)) {
                html += "<tr>";
                html += "  <th colspan='" + tableCol + "' class='form-unit'>" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr>";
                html += "  <td colspan='" + tableCol + "' class='FDesc'>";

                html += Ele_FrmCheck(node);

                html += "  </td>";
                html += "</tr>";

                continue;
            }
        }

        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {

            html += "<tr>";
            html += "  <th colspan='" + tableCol + "' class='form-unit attr-group'>" + gf.Lab + "</th>";
            html += "</tr>";
            if (tableCol == 4 || tableCol == 6)
                html += InitMapAttr(frmData.Sys_MapAttr, frmData, gf.OID, tableCol);
            else if (tableCol == 3)
                html += InitThreeColMapAttr(frmData.Sys_MapAttr, frmData, gf.OID, tableCol);
            continue;
        }
    }

    html += "</table>";

    ////加入隐藏控件.
    //var mapAttrs = frmData.Sys_MapAttr;
    //for (var i = 0; i < mapAttrs.length; i++) {
    //    var attr = mapAttrs[i];
    //    if (attr["UIVisible"] == 0) {
    //        var defval = ConvertDefVal(frmData, attr["DefVal"], attr["KeyOfEn"]);
    //        html += "<input type='hidden' id='TB_" + attr["KeyOfEn"] + "' name='TB_" + attr["KeyOfEn"] + "' value='" + defval + "' />";
    //    }
    //}

    $('#CCForm').html("").append(html);

    //处理附件的问题
    var aths = $(".athModel");
    $.each(aths, function (idx, ath) {
        //获取ID
        var name = $(ath).attr('id');
        var keyOfEn = name.replace("athModel_", "");
        $("#Lab_" + keyOfEn).html("<div style='text-align:left'>" + $("#Lab_" + keyOfEn).text() + "</div>");
    });

}

//子流程
function Ele_SubFlow(wf_node) {
    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var h = wf_node.SF_H + 100;

    if (sta == 0)
        return $('');

    var src = "../WorkOpt/SubFlow.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + GetQueryString("FID");
    paras += "&OID=" + pageData.OID;
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;
    if (sta == 2 || pageData.IsReadonly== 1)//只读
    {
        src += "&DoType=View";
    }
    src += "&r=q" + paras;

    if (h == 0)
        h = 400;
    var eleHtml = "<iframe id=FSF" + wf_node.NodeID + " style='width:100%;height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";

    return eleHtml;
}


function Set_Frm_Enable(frmData) {
    var mapAttrs = frmData.Sys_MapAttr;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        if (mapAttr.LGType != 1 && mapAttr.MyDataType != 4)
            continue;

        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
            if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1) {
                    /*启用了显示与隐藏.*/
                    var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                    //初始化页面的值
                    var nowKey = ddl.val();//ddl.val();
                    if (nowKey == null || nowKey == undefined || nowKey == "")
                        continue;


                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
                if (mapAttr.UIContralType == 3) {
                    /*启用了显示与隐藏.*/
                    var rb = $("#RB_" + mapAttr.KeyOfEn);
                    var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                    if (nowKey != null || nowKey != undefined)
                        setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
            }
        }
        //复选框
        if (mapAttr.MyDataType == 4 && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            //获取复选框的值
            if ($("#CB_" + mapAttr.KeyOfEn).checked == true)
                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 1);
            else
                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 0);
        }

    }
}

//审核组件
function Ele_FrmCheck(wf_node) {

    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;

    var h = wf_node.FWC_H + 1000;

    var isReadonly = GetQueryString('IsReadonly');
    if (isReadonly != "1") {
        isReadonly = "0";
    }
    if (sta == 2)//只读
        isReadonly = "1";

    var src = "";

    if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
        src = "../WorkOpt/WorkCheck.htm?s=2&IsReadonly=" + GetQueryString("IsReadonly");
    else
        src = "../WorkOpt/WorkCheck2019.htm?s=2&IsReadonly=" + GetQueryString("IsReadonly");

    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["OID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;


    src += "&r=q" + paras;
    var eleHtml = "<iframe width='100%' height='" + h + "' id='FWC' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=no ></iframe>";
    return eleHtml;
}

//解析表单是三列的情况
function InitThreeColMapAttr(Sys_MapAttr, frmData, groupID, tableCol) {
    var html = "";
    var isDropTR = true;

    var lab = "";
    var colSpan = 1;
    var rowSpan = 1;
    var textColSpan = 1;
    var textWidth = "33%";
    var colWidth = "33%";

    //记录一行已占用的列输
    var UseColSpan = 0;
    var IsMiddle = false;
    //跨行问题
    for (var i = 0; i < Sys_MapAttr.length; i++) {
        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0)
            continue;
       
        rowSpan = attr.RowSpan;
        colSpan = attr.ColSpan;
        textColSpan = attr.TextColSpan;


        colWidth = 33 * parseInt(colSpan) + "%";
        textWidth = 33 * parseInt(textColSpan) + "%";

        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            //获取文本信息
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str = filename+"このファイルは存在しません。管理者に連絡してください。";
            html += "<tr>";
            html += "<td  ColSpan='" + tableCol + "' class='FDesc' style='text-align:left:height:auto'>" + str + "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        //解析Lab 1、文本类型、DDL类型、RB类型、扩张（图片、附件、超链接）
        lab = GetLab(frmData, attr);

        //跨列设置(显示的是文本)
        if (colSpan == 0) {

            if (textColSpan == tableCol) {
                rowSpan = 1;
                html += "<td  class='LabelFDesc' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                isDropTR = true;
                continue;
            }
            //线性展示都跨一个单元格
            //换行的情况
            if (isDropTR == true) {
                html += "<tr >";
                UseColSpan = 0;
                UseColSpan += colSpan + textColSpan;
                html += "<td class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";

                if (UseColSpan == tableCol) {
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }
                continue;
            }

            if (isDropTR == false) {
                UseColSpan += colSpan + textColSpan;
                html += "<td class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                if (UseColSpan == tableCol) {
                    html += "</tr>";
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }
                continue;
            }
        }
        //解析占一行的情况
        if (colSpan == tableCol) {
            rowSpan = 1;
            html += "<tr>";
            html += "<td  ColSpan='" + colSpan + "' rowSpan=" + rowSpan + " class='LabelFDesc' style='text-align:left'>" + lab + "</br>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            html += "<tr >";
            UseColSpan = 0;

            UseColSpan += colSpan;
            html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + colSpan + " class='tdSpan'>" + lab + "<br/>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";
            if (UseColSpan == tableCol) {
                isDropTR = true;
            } else {
                isDropTR = false;
            }


            continue;
        }

        if (isDropTR == false) {

            UseColSpan += colSpan;
            html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + colSpan + " class='tdSpan'>" + lab + "<br/>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";

            if (UseColSpan == tableCol) {
                html += "</tr>";
                isDropTR = true;
            } else {
                isDropTR = false;
            }


            continue;
        }
    }
    return html;
}


//解析表单字段 MapAttr.
function InitMapAttr(Sys_MapAttr, frmData, groupID, tableCol) {

    var html = "";

    //右侧跨行
    var IsShowRight = true; // 是否显示右侧列
    var rRowSpan = 0; //跨的行数
    var ruRowSpan = 0; //已近解析的行数
    var ruColSpan = 0; //该跨行总共跨的列数

    //左侧跨行
    var IsShowLeft = true; // 是否显示左侧列
    var lRowSpan = 0; //跨的行数
    var luRowSpan = 0; //已近解析的行数
    var luColSpan = 0; //该跨行总共跨的列数

    //记录一行已占用的列输
    var UseColSpan = 0;

    var isDropTR = true;

    //跨列的字段
    var colSpan = 1;
    var textColSpan = 2;
    var textWidth = "15%";
    var colWidth = "35%";


    var lab = "";
    for (var i = 0; i < Sys_MapAttr.length; i++) {

        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0)
            continue;

        
        //赋值
        rowSpan = parseInt(attr.RowSpan);
        colSpan = parseInt(attr.ColSpan);
        textColSpan = parseInt(attr.TextColSpan);
        if (tableCol == 4) {
            colWidth = 35 * parseInt(colSpan) + "%";
            textWidth = 15 * parseInt(textColSpan) + "%";
        } else {
            colWidth = 23 * parseInt(colSpan) + "%";
            textWidth = 10 * parseInt(textColSpan) + "%";
        }

        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            //获取文本信息
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str = filename + "このファイルは存在しません。管理者に連絡してください。";
            html += "<tr>";
            html += "<td  ColSpan='" + tableCol + "' class='FDesc' style='text-align:left:height:auto'>" + str + "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        //解析Lab 1、文本类型、DDL类型、RB类型、扩张（图片、附件、超链接）
        lab = GetLab(frmData, attr);

        //单元格为0的情况
        if (colSpan == 0) {
            //占一行
            if (textColSpan == tableCol) {
                if (isDropTR == false) {
                    var unUseColSpan = tableCol - UseColSpan;
                    html += "<td colspan=" + unUseColSpan + "></td>";
                    html += "</tr>";
                }
                isDropTR = true;
                rowSpan = 1;
                html += "<tr>";
                html += "<td  colSpan=" + textColSpan + " rowSpan=" + rowSpan + " class='LabelFDesc' style='text-align:left'>" + lab + "</br>";
                html += "</tr>";
                continue;
            }
            //线性展示都跨一个单元格
            if (isDropTR == true) {
                html += "<tr >";
                UseColSpan = 0;
                luColSpan = 0;
                if (IsShowLeft == true) {
                    UseColSpan += colSpan + textColSpan + ruColSpan;
                    lRowSpan = rowSpan;
                    luColSpan += colSpan + textColSpan;
                    html += "<td class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                    if (rowSpan != 1) {
                        IsShowLeft = false;
                    }

                }
                if (UseColSpan == tableCol) {
                    ruRowSpan++;
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }

                //复位右侧信息
                if (ruRowSpan == rRowSpan) {
                    ruRowSpan = 0;
                    luRowSpan = 0;
                    rRowSpan = 0;
                    IsShowRight = true;
                    if (rowSpan == 1)
                        luColSpan = 0;
                    ruColSpan = 0;
                }


                if (IsShowRight == false && (UseColSpan == tableCol)) {
                    html += "</tr>";
                    isDropTR = true;
                    UseColSpan = ruColSpan;

                }

                continue;
            }

            if (isDropTR == false) {
                ruColSpan = 0;
                if (IsShowRight == true) {
                    UseColSpan += colSpan + textColSpan;
                    rRowSpan = rowSpan;
                    ruColSpan += colSpan + textColSpan;
                    html += "<td class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                    if (UseColSpan == tableCol) {
                        isDropTR = true;
                        if (rowSpan != 1) {
                            ruRowSpan++;
                        }
                    }
                    if (rowSpan != 1) {
                        IsShowRight = false;
                        lRowSpan = rowSpan;
                    }
                }

                if (UseColSpan == tableCol) {
                    luRowSpan++;
                    html += "</tr>";
                }

                //复位左侧信息
                if (luRowSpan == lRowSpan) {
                    luRowSpan = 0;
                    ruRowSpan = 0;
                    lRowSpan = 0;
                    IsShowLeft = true;
                    ruColSpan = 0;

                }

                if (IsShowLeft == false && (UseColSpan == tableCol)) {
                    html += "<tr>";
                    UseColSpan = 0;
                    isDropTR = false;
                    UseColSpan = luColSpan;
                }
                continue;
            }

        }


        //线性展示并且colspan=4
        if (colSpan == tableCol) {
            if (isDropTR == false) {
                var unUseColSpan = tableCol - UseColSpan;
                html += "<td colspan=" + unUseColSpan + "></td>";
                html += "</tr>";
            }
            isDropTR = true;
            rowSpan = 1;
            html += "<tr>";
            html += "<td  ColSpan='" + colSpan + "' rowSpan=" + rowSpan + " class='LabelFDesc' style='text-align:left'>" + lab + "</br>";
            html += "</tr>";
            html += "<tr>";
            html += "<td  id='Td_" + attr.KeyOfEn + "' ColSpan='" + colSpan + "' rowSpan=" + rowSpan + " class='FDesc' style='text-align:left'>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        var sumColSpan = colSpan + textColSpan;
        if (sumColSpan == tableCol) {
            if (isDropTR == false) {
                var unUseColSpan = tableCol - UseColSpan;
                html += "<td colspan=" + unUseColSpan + "></td>";
                html += "</tr>";
            }
            rowSpan = 1;
            isDropTR = true;
            html += "<tr >";
            html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
            html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            continue;
        }
        //换行的情况
        if (isDropTR == true) {
            html += "<tr >";
            UseColSpan = 0;
            luColSpan = 0;
            if (IsShowLeft == true) {
                UseColSpan += colSpan + textColSpan + ruColSpan;
                lRowSpan = rowSpan;
                luColSpan += colSpan + textColSpan;
                if (attr.MyDataType == 4) {
                    colSpan = colSpan + textColSpan;
                    colWidth = (parseInt(colSpan) * 23 + 10 * parseInt(textColSpan)) + "%";
                } else {
                    html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                }
                html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                html += InitMapAttrOfCtrl(attr);
                html += "</td>";

                if (rowSpan != 1) {
                    IsShowLeft = false;
                }

            }
            if (UseColSpan == tableCol) {
                ruRowSpan++;
                isDropTR = true;
            } else {
                isDropTR = false;
            }

            //复位右侧信息
            if (ruRowSpan == rRowSpan) {
                ruRowSpan = 0;
                luRowSpan = 0;
                rRowSpan = 0;
                IsShowRight = true;
                if (rowSpan == 1)
                    luColSpan = 0;
                ruColSpan = 0;
            }


            if (IsShowRight == false && (UseColSpan == tableCol)) {
                html += "</tr>";
                isDropTR = true;
                UseColSpan = ruColSpan;

            }

            continue;
        }

        if (isDropTR == false) {
            ruColSpan = 0;
            if (IsShowRight == true) {
                UseColSpan += colSpan + textColSpan;
                rRowSpan = rowSpan;
                ruColSpan += colSpan + textColSpan;
                if (attr.MyDataType == 4) {
                    colSpan = colSpan + textColSpan;
                    width = 35 * parseInt(colSpan) + "%";
                } else {
                    html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                }
                html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                html += InitMapAttrOfCtrl(attr);
                html += "</td>";
                if (UseColSpan == tableCol) {
                    isDropTR = true;
                    if (rowSpan != 1) {
                        ruRowSpan++;
                    }
                }
                if (rowSpan != 1) {
                    IsShowRight = false;
                    lRowSpan = rowSpan;
                }
            }

            if (UseColSpan == tableCol) {
                luRowSpan++;
                html += "</tr>";
            }

            //复位左侧信息
            if (luRowSpan == lRowSpan) {
                luRowSpan = 0;
                ruRowSpan = 0;
                lRowSpan = 0;
                IsShowLeft = true;
                ruColSpan = 0;

            }

            if (IsShowLeft == false && (UseColSpan == tableCol)) {
                html += "<tr>";
                UseColSpan = 0;
                isDropTR = false;
                UseColSpan = luColSpan;
            }
            continue;
        }

    }

    if (isDropTR == false) {
        var unUseColSpan = tableCol - UseColSpan;
        html += "<td colspan=" + unUseColSpan + "></td>";
        html += "</tr>";
    }

    return html;
}

function InitMapAttrOfCtrl(mapAttr) {

    var str = '';
    var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);

    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //

    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        return "<select style='width:100%' id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + " onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //判断外键是否为树形结构
        var uiBindKey = mapAttr.UIBindKey;
        if (uiBindKey != null && uiBindKey != undefined && uiBindKey != "") {
            var sfTable = new Entity("BP.Sys.SFTable");
            sfTable.SetPKVal(uiBindKey);
            var count = sfTable.RetrieveFromDBSources();
            if (count != 0 && sfTable.CodeStruct == "1") {
                return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='height:28px;width:60%'></select>";
            }
        }

        return "<select style='width:100%' id='DDL_" + mapAttr.KeyOfEn + "'  name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + " onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    //外部数据类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        if (mapAttr.UIContralType == 1)
            return "<select style='width:100%' id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + " onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr);

        }
    }


    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == "2") {//枚举复选框

        var rbHtmls = "";
        var ses = frmData[mapAttr.KeyOfEn];
        if (ses == undefined)
            ses = frmData[mapAttr.UIBindKey];
        if (ses == undefined) {
            //枚举类型的.
            if (mapAttr.LGType == 1) {
                var ses = frmData.Sys_Enum;
                ses = $.grep(ses, function (value) {
                    return value.EnumKey == mapAttr.UIBindKey;
                });
            }

        }
        var enableAttr = "";
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //显示方式,默认为横向展示.
        var RBShowModel = 0;
        if (mapAttr.AtPara.indexOf('@RBShowModel=0') > 0)
            RBShowModel = 1;

        for (var i = 0; i < ses.length; i++) {
            var se = ses[i];

            var br = "";
            if (RBShowModel == 1)
                br = "<br>";

            var checked = "";
            if ("," + defValue + ",".indexOf("," + se.IntKey + ",") == true)
                checked = " checked=true";

            rbHtmls += "<label style='font-weight:normal;'><input type=checkbox name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + enableAttr + " onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' />" + se.Lab + " </label>&nbsp;" + br;
        }
        return rbHtmls;
    }


    if (mapAttr.MyDataType == "1") {

        //附件
        if (mapAttr.UIContralType == "6") {

            //获取上传附件列表的信息及权限信息
            var nodeID = pageData.FK_Node;

            var IsStartNode = 0;
            if (nodeID != null) {
                var no = nodeID.substring(nodeID.length - 2);
                if (no == "01")
                    IsStartNode = 1;
            }

            //创建附件描述信息.
            var mypk = mapAttr.MyPK;

            //获取附件显示的格式
            var ath=null;
            var athShowModel = GetPara(mapAttr.AtPara, "AthShowModel");
            var frmAths = frmData.Sys_FrmAttachment;
            for (var i = 0; i < frmAths.length; i++) {
                if (frmAths[i].MyPK == mypk) {
                    ath = frmAths[i];
                    break;
                }
            }

           
            if (ath==null) {
                alert("添付ファイルのプロパティが見つかりませんでした。管理者に連絡してください。");
                return;
            }
            var noOfObj = mypk.replace(mapAttr.FK_MapData + "_", "");
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
            handler.AddPara("WorkID", pageData.OID);
            handler.AddPara("FID", pageData.FID);
            handler.AddPara("FK_Node", nodeID);
            handler.AddPara("FK_Flow", pageData.FK_Flow);
            handler.AddPara("IsStartNode", IsStartNode);
            handler.AddPara("PKVal", pageData.OID);
            handler.AddPara("Ath", noOfObj);
            handler.AddPara("FK_MapData", mapAttr.FK_MapData);
            handler.AddPara("FromFrm", mapAttr.FK_MapData);
            handler.AddPara("FK_FrmAttachment", mypk);
            data = handler.DoMethodReturnString("Ath_Init");

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            if (data.indexOf('url@') == 0) {
                var url = data.replace('url@', '');
                window.location.href = url;
                return;
            }
            data = JSON.parse(data);
            var dbs = data["DBAths"];
            var athDesc = data["AthDesc"][0];
            if (dbs.length == 0) {
                if (athDesc.IsUpload == 1 || pageData.IsReadOnly == 0)
                    return "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "'><label>以下をクリックし[" + mapAttr.Name + "]アップロードを実行してください</label></div>";
                else
                    return "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "' class='athModel'><label>添付ファイル(0)</label></div>";
            }

            var eleHtml = "";
            if (athShowModel == "" || athShowModel == 0)
                return "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='0'><label >添付ファイル(" + dbs.length + ")</label></div>";

            eleHtml = "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";
            for (var i = 0; i < dbs.length; i++) {
                var db = dbs[i];
                eleHtml += "<label><a style='font-weight:normal;font-size:12px' href=\"javascript:Down2018('" + mypk + "','" + pageData.OID + "','" + db.MyPK + "','" + pageData.FK_Flow + "','" + pageData.FK_Node + "','" + mapAttr.FK_MapData + "','" + mypk + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' />" + db.FileName + "</a></label>&nbsp;&nbsp;&nbsp;"
            }
            eleHtml += "</div>";
            return eleHtml;
        }

        //签字板
        if (mapAttr.UIContralType == "8") {
            //查找默认值
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (val.indexOf("../../") == -1)
                val = val.replace("../", "../../");
            //如果是图片签名，并且可以编辑
            var ondblclick = ""
            if (mapAttr.UIIsEnable == 1) {
                ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")'";
            }

            var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
            eleHtml += "<img src='" + val + "' " + ondblclick + " onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            return eleHtml;
        }
        //评分控件
        if (mapAttr.UIContralType == "101") {
            //查找默认值
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (val == "" || val == undefined || val == null) {
                val = 0;
            }
            //如果编辑
            var eleHtml = "<div class='score-star' style='text-align:left;padding-left:3px;height:30px;margin-top:10px' data-type='1' id='SC_" + mapAttr.KeyOfEn + "' >";
            if (mapAttr.UIIsEnable == 1) {
                eleHtml += "<span class='score-simplestar' id='Star_" + mapAttr.KeyOfEn + "'>";
            } else {
                eleHtml += "<span class='score-simplestar'>";
            }
            var num = mapAttr.Tag2;
            for (var i = 0; i < val; i++) {

                eleHtml += "<img src='../Style/Img/star_2.png' />";
            }
            for (var j = 0; j < num - val; j++) {

                eleHtml += "<img src='../Style/Img/star_1.png' />";
            }
            eleHtml += "&nbsp;&nbsp;<span class='score-tips' id='SP_" + mapAttr.KeyOfEn + "' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + val + "  分</strong></span>";
            eleHtml += "<input id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden /></span>";
            eleHtml += "</div>";
            return eleHtml;
        }
        //  地图
        if (mapAttr.UIContralType == "4") {
            //查找默认值
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            //如果是地图，并且可以编辑
            var eleHtml = "<div style='text-align:left;padding-left:0px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";
            if (mapAttr.UIIsEnable == 1) {
                eleHtml += "<input type='button' name='select' value='選択' onclick='figure_Template_Map(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.UIIsEnable + "\")'/>";
                eleHtml += "<input type = text style='width:75%' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />";
            } else {
                eleHtml += "<input type='button' name='select' value='選択' onclick='figure_Template_Map(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.UIIsEnable + "\")'/>";
                eleHtml += "<input type = text style='width:75%' readonly='readonly' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />";
            }
            eleHtml += "</div>";
            return eleHtml;
        }

        //进度条
        if (mapAttr.UIContralType == "50") {

            var url = '../WorkOpt/OneWork/JobSchedule.js';
            $.getScript(url, function () {


            });
            return "<div id='JobSchedule' >JobSchedule</div>";
        }


    }

    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 0) {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
        {
            //如果是图片签名，并且可以编辑
            if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1 && pageData.IsReadonly != 0) {
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  value='" + defValue + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", pageData.OID, "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

                if (sealData.length > 0) {
                    eleHtml += "<img src='../../DataUser/Siganture/" + defValue + ".jpg' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='../../DataUser/Siganture/siganture.jpg' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\" ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                }
                return eleHtml;
            }
            //如果不可编辑，并且是图片名称
            if (mapAttr.IsSigan == "1") {
                var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  value='" + val + "' type=hidden />";
                eleHtml += "<img src='../../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../../DataUser/Siganture/siganture.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }



            var enableAttr = '';
            if (mapAttr.UIIsEnable == 0)
                enableAttr = "disabled='disabled'";

            return "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' style='width:100%;height:28px;' type='text'  " + enableAttr + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        }

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0" || pageData.IsReadonly == 1) {
                //只读状态直接 div 展示富文本内容
                defValue = defValue.replace(/white-space: nowrap;/g, "");
                eleHtml += "<div class='richText'>" + defValue + "</div>";
            } else {
                //设置一个默认高度
                if (mapAttr.UIHeight < 180) {
                    mapAttr.UIHeight = 180;
                }

                
                if (IsLoadUEditor == false) {
                    //加载UEditor需要的JS
                    $("<link href='../Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css' type = 'text/css' rel = 'stylesheet' />").appendTo("head");
                    $("<script type='text/javascript' src='../Comm/umeditor1.2.3-utf8/third-party/template.min.js'></script>").appendTo("head");
                    $("<script type='text/javascript' src='../Comm/umeditor1.2.3-utf8/umeditor.config.js'></script>").appendTo("head");
                    $("<script type='text/javascript' src='../Comm/umeditor1.2.3-utf8/umeditor.min.js'></script>").appendTo("head");
                    $("<script type='text/javascript' src='../Comm/umeditor1.2.3-utf8/lang/zh-cn/zh-cn.js'></script>").appendTo("head");
                    IsLoadUEditor = true;
                    
                }
                document.BindEditorMapAttr.push(mapAttr); //存到全局备用
                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                var height = parseInt(mapAttr.UIHeight) - 54;
                styleText += "height:" + height + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的
                eleHtml += "<script class='EditorClass' id='editor_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }

        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;width:100%;' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 8) {
        //如果是图片签名，并且可以编辑
        var ondblclick = ""
        if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly == 0) {
            ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'";
        }

        var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + defValue + "' type=hidden />";
        if (defValue.indexOf("../DataUser") != 0)
            defValue = defValue.replace("../DataUser", "../../DataUser");
        eleHtml += "<img src='" + defValue + "' " + ondblclick + " onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
        return eleHtml;
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        var frmDate = mapAttr.IsSupperText;//获取日期格式
        var dateFmt = '';
        if (frmDate == 0) {
            dateFmt = "yyyy-MM-dd";
        } else if (frmDate == 3) {
            dateFmt = "yyyy-MM";
        } else if (frmDate == 6) {
            dateFmt = "MM-dd";
        }
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text' class='Wdate' style='width:100%'  placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        var frmDate = mapAttr.IsSupperText; //获取日期格式
        var dateFmt = '';
        if (frmDate == 1) {
            dateFmt = "yyyy-MM-dd HH:mm";
        } else if (frmDate == 2) {
            dateFmt = "yyyy-MM-dd HH:mm:ss";
        } else if (frmDate == 4) {
            dateFmt = "HH:mm";
        } else if (frmDate == 5) {
            dateFmt = "HH:mm:ss";
        }
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input style='width:100%' class='Wdate'  type='text'  " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "' />";
    }

    // boolen 类型.
    if (mapAttr.MyDataType == 4) {  // AppBoolean = 7

        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" ';
        }

        checkedStr = ConvertDefVal(frmData, '', mapAttr.KeyOfEn);

        return "<input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox'   id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " onchange='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'/><label for='CB_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        if (mapAttr.UIContralType == 1)
            return "<select style='width:100%' id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + "  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr);
        }

    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1)
            enableAttr = "disabled='disabled'";

        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;

        // alert(mapAttr.KeyOfEn);
        return "<input  onfocus='removeplaceholder(this," + bit + ");' onblur='addplaceholder(this," + bit + ");'  style='text-align:right;width:100%'  onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        //return "<input onfocus='removeplaceholder(this,0);' onblur='addplaceholder(this,0);'  style='text-align:right;;width:100%' onkeyup=" + '"' + "limitLength(this," + bit + ");valitationAfter(this, 'int');if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value)) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return "<input  onfocus='removeplaceholder(this,0);' onblur='addplaceholder(this,0);' value='" + defValue + "' style='text-align:right;' class='form-control' onkeyup=" + '"' + "limitLength(this," + bit + ");valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn +"' placeholder='" + (mapAttr.Tip || '') + "'/>";
    
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {

        } else {
            enableAttr = "disabled='disabled'";
        }

        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;
        else
            bit = 2;

        return "<input onfocus='removeplaceholder(this," + bit + ");' onblur='addplaceholder(this," + bit + ");numberFormat (this, " + bit + ")' style='text-align:right;width:100%' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";    }

    alert(mapAttr.Name + "のタイプは判断していません。");
    return;
}

//记录改变字段样式 不可编辑，不可见
var AllObjSet = {};

function changeEnable(obj, FK_MapData, KeyOfEn, AtPara,frmType) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selecedval = $(obj).children('option:selected').val();  //弹出select的值.
        cleanAll(KeyOfEn, frmType);
        setEnable(FK_MapData, KeyOfEn, selecedval, frmType);
        $(obj).val(selecedval);
    }
}
function clickEnable(obj, FK_MapData, KeyOfEn, AtPara, frmType) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selectVal = $(obj).val();
        cleanAll(KeyOfEn, frmType);
        setEnable(FK_MapData, KeyOfEn, selectVal, frmType);
        document.getElementById("RB_" + KeyOfEn + "_" + selectVal).checked = true
    }
}

function changeCBEnable(obj, FK_MapData, KeyOfEn, AtPara, frmType) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        cleanAll(KeyOfEn, frmType);
        if (obj.checked == true) {
            setEnable(FK_MapData, KeyOfEn, 1, frmType);
            obj.checked = true;
        }
        else {
            setEnable(FK_MapData, KeyOfEn, 0, frmType);
            obj.checked = false;
        }
           
    }
}

//清空所有的设置
function cleanAll(KeyOfEn, frmType) {
    var trs = $("#CCForm  table tr .attr-group"); //如果隐藏就显示
    $.each(trs, function (i, obj) {
        if ($(obj).parent().is(":hidden") == true)
            $(obj).parent().show();

    });
    if (AllObjSet.length == 0)
        return;
    if (AllObjSet[KeyOfEn]!=undefined && AllObjSet[KeyOfEn].length > 0) {
        var mapAttrs = AllObjSet[KeyOfEn][0];
        for (var i = 0; i < mapAttrs.length; i++) {
            if (frmType != null && frmType !== undefined && frmType == 8)
                SetDevelopCtrlShow(mapAttrs[i]);
            else
                SetCtrlShow(mapAttrs[i]);
            SetCtrlEnable(mapAttrs[i]);
            //CleanCtrlVal(mapAttrs[i]);
        }
    }

    if (frmAttrData != undefined && frmAttrData.length != 0) {
        $.each(frmAttrData, function (i, obj) {
            SetCtrlVal(obj.KeyOfEn, obj.Val);
        });

    }
   

}
//启用了显示与隐藏.
function setEnable(FK_MapData, KeyOfEn, selectVal, frmType) {
	if(selectVal==undefined)
        return;

    var pkval = FK_MapData + "_" + KeyOfEn + "_" + selectVal;


    var frmRBs = frmData["Sys_FrmRB"];
    if (frmRBs.length <= 0)
        return;


    var frmRB = null;
    for (var i = 0; i < frmRBs.length; i++) {
        if (frmRBs[i].MyPK == pkval) {
            frmRB = frmRBs[i];
            break;
        }
    }
    if (frmRB == null)
        return;

    var mapAttrs = [];

    //解决字段隐藏显示.
    var cfgs = frmRB.FieldsCfg;

    //解决为其他字段设置值.
    var setVal = frmRB.SetVal;
    if (setVal) {
        var strs = setVal.split('@');

        for (var i = 0; i < strs.length; i++) {

            var str = strs[i];
            if (str == "")
                continue;
            var kv = str.split('=');

            var key = kv[0];
            var value = kv[1];
            SetCtrlVal(key, value);
            mapAttrs.push(key);

        }
    }
    //@Title=3@OID=2@RDT=1@FID=3@CDT=2@Rec=1@Emps=3@FK_Dept=2@FK_NY=3
    if (cfgs) {

        var strs = cfgs.split('@');

        for (var i = 0; i < strs.length; i++) {

            var str = strs[i];
            var kv = str.split('=');

            var key = kv[0];
            var sta = kv[1];

            if (sta == 0)
                continue; //什么都不设置.


            if (sta == 1) {  //要设置为可编辑.
                if (frmType != null && frmType != undefined && frmType == 8)
                    SetDevelopCtrlShow(key);
                else
                    SetCtrlShow(key);
                SetCtrlEnable(key);
            }

            if (sta == 2) { //要设置为不可编辑.
                if (frmType != null && frmType != undefined && frmType == 8)
                    SetDevelopCtrlShow(key);
                else
                    SetCtrlShow(key);
                SetCtrlUnEnable(key);
                mapAttrs.push(key);
            }

            if (sta == 3) { //不可见.
                if (frmType!=null && frmType!=undefined && frmType == 8)
                    SetDevelopCtrlHidden(key);
                else
                    SetCtrlHidden(key); 

                mapAttrs.push(key);
            }

        }
        if (!$.isArray(AllObjSet[KeyOfEn])) {
            AllObjSet[KeyOfEn] = [];
        }
        AllObjSet[KeyOfEn] = [];
        AllObjSet[KeyOfEn].push(mapAttrs);

    }

    //设置是否隐藏分组、获取字段分组所有的tr 
    var trs = $("#CCForm  table tr .attr-group");
    var isHidden = false;
    $.each(trs, function (i, obj) {
        //获取所有跟随的同胞元素，其中有不隐藏的tr,就跳出循环
        var sibles = $(obj).parent().nextAll();
        for (var k = 0; k < sibles.length; k++) {
            var sible = $(sibles[k]);
            if (sible.find(".attr-group").length > 0 || sible.find(".form-unit").length > 0)
                break;
            if (sible.is(":hidden") == false) {
                isHidden = false;
                break;
            }
            isHidden = true;
        }
        if (isHidden == true)
            $(obj).parent().hide();

    });


}

//设置是否可以用?
function SetCtrlEnable(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        ctrl.addClass("form-control");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        ctrl.addClass("form-control");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
//        ctrl.addClass("form-control");
    }

    ctrl = document.getElementsByName('RB_' + key);
    if (ctrl != null && ctrl.length !=0 ) {

        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", key);


        for (var i = 0; i < ses.length; i++)
            $("#RB_" + key + "_" + ses[i].IntKey).removeAttr("disabled");
    }
}
function SetCtrlUnEnable(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "true");
        ctrl.removeClass("form-control");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "disabled");
        ctrl.removeClass("form-control");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {

        ctrl.attr("disabled", "disabled");
        ctrl.removeClass("form-control");
    }

    ctrl = $("#RB_" + key);
    if (ctrl != null) {
        $('input[name=RB_' + key + ']').attr("disabled", "disabled");

    }
}
//设置隐藏?
function SetCtrlHidden(key) {
    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0)
        ctrl.parent('tr').hide();

    var ctrl = $("#Td_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').hide();
    }


}
//设置显示?
function SetCtrlShow(key) {
    var ctrl = $("#Td_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').show();
    }

    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').show();
    }


}


//设置隐藏?
function SetDevelopCtrlHidden(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.hide();
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.hide();
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.hide();
        if (ctrl.parent() != undefined && ctrl.parent().length > 0) {
            if ($(ctrl.parent()[0]).context.nodeName.toLowerCase() == "label")
                $(ctrl.parent()[0]).hide();
        }
      
    }

    ctrl = $("#SR_" + key);
    if (ctrl.length > 0) {
        ctrl.hide();
    }

    ctrl = $("#SC_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
    }

    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0) {
        ctrl.hide();
    }

    CleanCtrlVal(key);


}
//设置显示?
function SetDevelopCtrlShow(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
        if (ctrl.parent() != undefined && ctrl.parent().length > 0) {
            if ($(ctrl.parent()[0]).context.nodeName.toLowerCase() == "label")
                $(ctrl.parent()[0]).show();
        }
    }

    ctrl = $("#SR_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
    }

    ctrl = $("#SC_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
    }

    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0) {
        ctrl.show();
    }
}




//初始化 框架
function Ele_Frame(frmData, gf) {


    var frames = frmData["Sys_MapFrame"];
    if (frames.length == 0)
        return "フレームの定義が見つかりませんでした。管理者に連絡してください。";

    var frame = null;
    for (var i = 0; i < frames.length; i++) {

        if (frames[i].MyPK == gf.CtrlID)
        {
            frame = frames[i];
            break;
        }
    }   
     
    if (frame == null)
        return;

    var eleHtml = '';
    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = frame.UrlSrcType;
    var url = "";
    if (urlType == 0) {
        url = frame.URL;
        if (url.indexOf('?') == -1)
            url += "?1=2";

        if (url.indexOf("@basePath") == 0)
            url = url.replace("@basePath", basePath);

        //处理URL需要的参数
        //1.替换@参数
        var pageParams = getQueryString();
        $.each(pageParams, function (i, pageParam) {
            var pageParamArr = pageParam.split('=');
            url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
        });

        var src = url.replace(new RegExp(/(：)/g), ':');
        if (src.indexOf("?") > 0) {
            var params = getQueryStringFromUrl(src);
            if (params != null && params.length > 0) {
                $.each(params, function (i, param) {
                    if (param.indexOf('@') != -1) {//是需要替换的参数
                        paramArr = param.split('=');
                        if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                            if (paramArr[1].indexOf('@WebUser.') == 0)
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                            else
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr(1)]);
                        }
                    }
                });
            }
        }

        //2.拼接参数
        var paras = this.pageData;
        var strs = "";
        for (var str in paras) {
            if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
                continue
            else
                strs += "&" + str + "=" + paras[str];
        }

        url = url + strs + "&IsReadonly=0";

        //4.追加GenerWorkFlow AtPara中的参数
        var gwf = frmData.WF_GenerWorkFlow[0];
        if (gwf != null) {
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }
    if (urlType == 2) //轨迹表
        url = "../WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;
    if (urlType == 3)//轨迹图
        url = "../WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;

    eleHtml += "<iframe style='width:100%;height:" + frame.H + "px;' ID='" + frame.MyPK + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;
}

//初始化 附件
function Ele_Attachment(workNode, gf) {

    var eleHtml = '';
    var nodeID = GetQueryString("FK_Node");
    var url = "";
    url += "&WorkID=" + pageData.OID;
    url += "&FK_Node=" + GetQueryString("FK_Node");
    url += "&FK_Flow=" + GetQueryString("FK_Flow");
    url += "&FormType=" + GetQueryString("FormType"); //表单类型，累加表单，傻瓜表单，自由表单.
	url += "&IsReadonly=" + pageData.IsReadonly;

    var nodeID = GetQueryString("FK_Node");
    if (nodeID != null && nodeID.length > 2) {
        var no = nodeID.substring(nodeID.length - 2);
        var IsStartNode = 0;
        if (no == "01")
            url += "&IsStartNode=1"; //是否是开始节点
    }

    var isReadonly = false;
    if (gf.FrmID.indexOf(nodeID) == -1)
        isReadonly = true;


    //创建附件描述信息.
    var ath = new Entity("BP.Sys.FrmAttachment", gf.CtrlID);

    var athPK = gf.CtrlID;
    var noOfObj = athPK.replace(gf.FrmID + "_", "");

    var src = "";

    var athUrl = "Ath.htm";
    var local = window.location.href;
    if (local.indexOf('CCBill') != -1) {
        athUrl = '../CCForm/' + athUrl;
    }

    //这里的连接要取 FK_MapData的值.
    src = athUrl + "?PKVal=" + pageData.OID +"&PWorkID="+GetQueryString("PWorkID")+ "&Ath=" + noOfObj + "&FK_MapData=" + ath.FK_MapData + "&FromFrm=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();

    //自定义表单模式.
    if (ath.AthRunModel == 2) {
        src = "../../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.OID+ "&PWorkID=" + GetQueryString("PWorkID")+ "&Ath=" + noOfObj + "&FK_MapData=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();
    }

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + gf.CtrlID + "' name='Attach'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;


}


var appPath = "../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量

//初始化从表
function Ele_Dtl(frmDtl) {
    var src = "";
    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    urlParam = urlParam.replace('EnsName=' + frmDtl.FK_MapData, '');
    urlParam = urlParam.replace('&RefPKVal=' + GetQueryString('RefPKVal'), '');

    urlParam = "";
    //alert(urlParam);

    var refPK = GetQueryString('OID');
    if (refPK == null)
        refPK = GetQueryString('WorkID');

    var isReadonly = GetQueryString("IsReadonly");
    if (isReadonly == "null" || isReadonly == "0" || isReadonly == null || isReadonly == undefined)
        isReadonly = "0";
    else
        isReadonly = "1";

    var dtlUrl = "Dtl2017";
    if (frmDtl.DtlVer == 1)
        dtlUrl = "Dtl2019";

    var local = window.location.href;
    if (local.indexOf('CCBill') != -1) {
        dtlUrl = '../CCForm/' + dtlUrl;
    }


    if (frmDtl.ListShowModel == "0") {
        src = dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&FK_MapData=" + frmDtl.FK_MapData + "&" + urlParam + "&Version=1";
    }
    else if (frmDtl.ListShowModel == "1") {
        src = "DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&FK_MapData=" + frmDtl.FK_MapData + "&" + urlParam + "&Version=1";
    }

    return "<iframe style='width:100%;height:" + frmDtl.H + "px;' name='Dtl' ID='Dtl_" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
}

function InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = frmData.Sys_Enum;
    enums = $.grep(enums, function (value) {
        return value.EnumKey == mapAttr.UIBindKey;
    });
    $.each(enums, function (i, obj) {
        var onclickEvent = "";
        if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            onclickEvent = "onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'";
        }

        if (RBShowModel == 3)
            //<input  " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name + "</label</div>";
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' " + onclickEvent + "  />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' " + onclickEvent + "   />&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}
function Ath_Init(mypk, FK_MapData) {
    var nodeID = pageData.FK_Node;
    var IsStartNode = 0;
    if (nodeID != null) {
        var no = nodeID.substring(nodeID.length - 2);
        if (no == "01")
            IsStartNode = 1;
    }

    var noOfObj = mypk.replace(FK_MapData + "_", "");
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("WorkID", pageData.OID);
    handler.AddPara("FID", pageData.FID);
    handler.AddPara("FK_Node", nodeID);
    handler.AddPara("FK_Flow", pageData.FK_Flow);
    handler.AddPara("IsStartNode", IsStartNode);
    handler.AddPara("PKVal", pageData.OID);
    handler.AddPara("Ath", noOfObj);
    handler.AddPara("FK_MapData", FK_MapData);
    handler.AddPara("FromFrm", FK_MapData);
    handler.AddPara("FK_FrmAttachment", mypk);
    data = handler.DoMethodReturnString("Ath_Init");
    return data;
}
//弹出附件
function OpenAth(url, title, keyOfEn, athMyPK, atPara, FK_MapData) {
    var H = document.body.clientHeight - 240;

    OpenBootStrapModal(url, "eudlgframe", title, frmData.Sys_MapData[0].FrmW, H, "icon-property", null, null, null, function () {


        //获取附件显示的格式
        var athShowModel = GetPara(atPara, "AthShowModel");

        var ath = new Entity("BP.Sys.FrmAttachment");
        ath.MyPK = athMyPK;
        if (ath.RetrieveFromDBSources() == 0) {
            alert("添付ファイルのプロパティが見つかりませんでした。管理者に連絡してください。");
            return;
        }
        var data = Ath_Init(athMyPK, FK_MapData)

        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }

        if (data.indexOf('url@') == 0) {
            var url = data.replace('url@', '');
            window.location.href = url;
            return;
        }
        data = JSON.parse(data);
        var dbs = data["DBAths"];
        if (dbs.length == 0) {
            $("#athModel_" + keyOfEn).html("<label>以下をクリックし[" + title + "]アップロードを実行してください</label>");
            return;
        }

        var eleHtml = "";
        if (athShowModel == "" || athShowModel == 0) {
            $("#athModel_" + keyOfEn).html("<label >添付ファイル(" + dbs.length + ")</label>");
            return;
        }

        for (var i = 0; i < dbs.length; i++) {
            var db = dbs[i];
            eleHtml += "<label><a style='font-weight:normal;font-size:12px'  href=\"javascript:Down2018('" + athMyPK + "','" + pageData.OID + "','" + db.MyPK + "','" + pageData.FK_Flow + "','" + pageData.FK_Node + "','" + FK_MapData + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' />" + db.FileName + "</a></label>&nbsp;&nbsp;&nbsp;"
        }
        $("#athModel_" + keyOfEn).html(eleHtml);
    }, null, "black", true);

}

var webUser = new WebUser();
function Down2018(fk_ath, pkVal, delPKVal, FK_Flow, FK_Node, FK_MapData, Ath) {
    if (plant == "CCFlow")
        window.location.href = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
        window.location.href = Url;
    }

}



//解析傻瓜表单的字段lab
function GetLab(frmData, attr) {
    var lab = "";
    var lab = "";
    var forID = "TB_" + attr.KeyOfEn;
    var contralType = attr.UIContralType;
    if (contralType == 1) {//外键下拉框
        forID = "DDL_" + attr.KeyOfEn;
    }
    if (contralType == 3) {//枚举
        forID = "RB_" + attr.KeyOfEn;
    }
    //文本框，下拉框，单选按钮
    if (contralType == 0 || contralType == 1 || contralType == 2 || contralType == 3 || contralType == 4 || contralType == 8 || contralType == 101) {
        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }
        lab += "<label id='Lab_" + attr.KeyOfEn + "' for='" + forID + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";
//        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
//            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
//        }
        return lab;
    }
    //附件控件
    if (contralType == 6) {
        //创建附件描述信息.
        var mypk = attr.MyPK;
        var ath = new Entity("BP.Sys.FrmAttachment");
        ath.MyPK = mypk;
        if (ath.RetrieveFromDBSources() == 0) {
            alert("添付ファイルのプロパティが見つかりませんでした。管理者に連絡してください。");
            return;
        }

        //附件的url
        var eleHtml = '';
        var nodeID = pageData.FK_Node;
        var url = "";
        url += "&WorkID=" + pageData.OID;
        url += "&FK_Node=" + nodeID;
        url += "&FK_Flow=" + pageData.FK_Flow;
        var isReadonly = false;
        if (nodeID == null)
            isReadonly = false;
        else {
            var no = nodeID.substring(nodeID.length - 2);
            var IsStartNode = 0;
            if (no == "01")
                url += "&IsStartNode=" + 1; //是否是开始节点


            if (attr.FK_MapData.indexOf(nodeID) == -1)
                isReadonly = true;
        }
        var noOfObj = mypk.replace(attr.FK_MapData + "_", "");
        var src = "";

        //这里的连接要取 FK_MapData的值.
        src = "../CCForm/Ath.htm?PKVal=" + pageData.OID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + attr.FK_MapData + "&FromFrm=" + attr.FK_MapData + "&FK_FrmAttachment=" + mypk + url + "&M=" + Math.random();
        //自定义表单模式.
        if (ath.AthRunModel == 2) {
            src = "../../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.OID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + attr.FK_MapData + "&FK_FrmAttachment=" + mypk + url + "&M=" + Math.random();
        }
        lab = "<label id='Lab_" + attr.KeyOfEn + "' for='athModel_" + attr.KeyOfEn + "'><div style='text-align:left'><a href='javaScript:void(0)' onclick='OpenAth(\"" + src + "\",\"" + attr.Name + "\",\"" + attr.KeyOfEn + "\",\"" + attr.MyPK + "\",\"" + attr.AtPara + "\",\"" + attr.FK_MapData + "\")' style='text-align:left'>" + attr.Name + "<image src='../Img/Tree/Dir.gif'></image></a></div></label>";
        return lab;
    }

    //超链接
    if (contralType == 9) {
        //URL @ 变量替换
        var url = attr.Tag2;
        //替换URL中的参数
        var pageParams = getQueryString();
        $.each(pageParams, function (i, pageParam) {
            var pageParamArr = pageParam.split('=');
            url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
        });

        $.each(frmData.Sys_MapAttr, function (i, obj) {
            if (url != null && url.indexOf('@' + obj.KeyOfEn) > 0) {
                url = url.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
            }
        });


        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var webUser = new WebUser();
        var userNo = webUser.No;
        var SID = webUser.SID;
        if (SID == undefined)
            SID = "";
        if (url.indexOf("?") == -1)
            url = url + "?1=1";

        if (url.indexOf("Search.htm") != -1)
            url = url + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
        else
            url = url  + "&UserNo=" + userNo + "&SID=" + SID;

        eleHtml = '<span ><a href="' + url + '" target="_blank">' + attr.Name + '</a></span>';

        return eleHtml;

    }
    //图片
    if (contralType == 11) {
        //获取图片控件的信息
        var frmImg = new Entity("BP.Sys.FrmUI.ExtImg");
        frmImg.SetPKVal(attr.MyPK);
        var count = frmImg.RetrieveFromDBSources();
        if (count == 0) {
            alert("主キーは" + attr.MyPK + "名前は" + attr.Name + "のイメージコントロールの情報がなくなりました。管理者に連絡してください。");
            return "";
        }

        //解析图片
        if (frmImg.ImgAppType == 0) {//图片类型
            //数据来源为本地.
            var webUser = new WebUser();
            var imgSrc = '';
            if (frmImg.ImgSrcType == 0) {
                //替换参数
                var frmPath = frmImg.ImgPath;
                frmPath = frmPath.replace('@basePath', basePath);
                imgSrc = DealJsonExp(frmData.MainTable[0], frmPath);

            }
            //数据来源为指定路径.
            if (frmImg.ImgSrcType == 1) {
                var url = frmImg.ImgURL.replace('@basePath', basePath);
                imgSrc = DealJsonExp(frmData.MainTable[0], url);
            }
            // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
            if (imgSrc == "" || imgSrc == null)
                imgSrc = "../../DataUser/ICON/CCFlow/LogBig.png";

            return "<img src='" + imgSrc + "' style='width:100%;height:100%' onerror=\"this.src='../../DataUser/ICON/CCFlow/LogBig.png'\" />";

        }
        return "";

    }

    return lab;
}

