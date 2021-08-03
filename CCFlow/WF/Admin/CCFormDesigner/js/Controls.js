/**Object with default values for figures*/
var FigureDefaults = {
    /**Size of figure's segment*/
    segmentSize: 70,

    /**Size of figure's short segment*/
    segmentShortSize: 40,

    /**Size of radius*/
    radiusSize: 35,

    /**Size of offset for parallels
    * For example: for parallelogram it's projection of inclined line on X axis*/
    parallelsOffsetSize: 40,

    /**Corner radius
    * For example: for rounded rectangle*/
    corner: 10,

    /**Corner roundness
    * Value from 0 to 10, where 10 - it's circle radius.*/
    cornerRoundness: 8,

    /**Color of lines*/
    strokeStyle: "#000000",

    /**Color of fill*/
    fillStyle: "#ffffff",

    /**Text size*/
    textSize: 15,

    /**Text label*/
    textStr: "Text",

    /**Text font*/
    textFont: "Arial",

    /**Color of text*/
    textColor: "#000000"
};

/**Controls set declaration*/
figureSets["basic"] = {
    name: '基本的なコントロール',
    description: 'A basic set of Controls',
    figures: [
        { figureFunction: null, name: "Poiner", image: "Poiner.png", CNName: "ポインタ" },
        { figureFunction: null, name: CCForm_Controls.Line, image: "Line.png", CNName: "直線" },
        { figureFunction: "Label", name: CCForm_Controls.Label, image: "Label.png", CNName: "テキスト" },
        { figureFunction: "Button", name: CCForm_Controls.Button, image: "Button.png", CNName: "ボタン" },
        { figureFunction: "HyperLink", name: CCForm_Controls.HyperLink, image: "HyperLink.png", CNName: "ハイパーリンク" },
        { figureFunction: "Image", name: CCForm_Controls.Image, image: "Img.png", CNName: "画像" }

    ]
};

/**method of create Label **/
function figure_Label(x, y) {
    var f = new Figure('Label');
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Label;
    f.style.fillStyle = FigureDefaults.fillStyle;

    f.properties.push(new BuilderProperty('基本属性', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('テキスト', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('フォントサイズ', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('フォント', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    f.properties.push(new BuilderProperty('整列', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('アンダースコア', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('太字のフォント', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT));
    f.properties.push(new BuilderProperty('フォントの色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR));

    var t2 = new Text("Label", x, y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**method of create Button **/
function figure_Button(x, y) {
    var f = new Figure("Button");
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Button;

    f.style.strokeStyle = "#c0c0c0";
    f.style.lineWidth = 2;

    f.properties.push(new BuilderProperty('基本属性-Button', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('フォントサイズ', 'primitives.1.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('フォント', 'primitives.1.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    f.properties.push(new BuilderProperty('太字のフォント', 'primitives.1.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT));
    f.properties.push(new BuilderProperty('フォントの色', 'primitives.1.style.fillStyle', BuilderProperty.TYPE_COLOR));

    f.properties.push(new BuilderProperty('コントロールプロパティ', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('ボタンラベル', 'primitives.1.str', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('ボタンイベント', 'ButtonEvent', BuilderProperty.CCFormEnum));
    f.properties.push(new BuilderProperty('イベント内容', 'BtnEventDoc', BuilderProperty.TYPE_TEXT));

    var p = new Path();
    var hShrinker = 12;
    var vShrinker = 15;
    var l1 = new Line(new Point(x + hShrinker, y + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker, y + vShrinker));

    var c1 = new QuadCurve(new Point(x + FigureDefaults.segmentSize - hShrinker, y + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + FigureDefaults.corner / FigureDefaults.cornerRoundness + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + vShrinker))

    var l2 = new Line(new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker));

    var c2 = new QuadCurve(new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker),
        new Point(x + FigureDefaults.segmentSize - hShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10)),
        new Point(x + FigureDefaults.segmentSize - hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner))

    var l3 = new Line(new Point(x + FigureDefaults.segmentSize - hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner),
        new Point(x + hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner));

    var c3 = new QuadCurve(
        new Point(x + hShrinker, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner),
        new Point(x + hShrinker - FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker + FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10)),
        new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker))

    var l4 = new Line(new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + FigureDefaults.segmentShortSize - vShrinker),
        new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + vShrinker));

    var c4 = new QuadCurve(
        new Point(x + hShrinker - FigureDefaults.corner, y + FigureDefaults.corner + vShrinker),
        new Point(x + hShrinker - FigureDefaults.corner * (FigureDefaults.cornerRoundness / 10), y + vShrinker),
        new Point(x + hShrinker, y + vShrinker))

    p.addPrimitive(l1);
    p.addPrimitive(c1);
    p.addPrimitive(l2);
    p.addPrimitive(c2);
    p.addPrimitive(l3);
    p.addPrimitive(c3);
    p.addPrimitive(l4);
    p.addPrimitive(c4);
    f.addPrimitive(p);

    var t2 = new Text("Btn...", x + FigureDefaults.segmentSize / 2, y + FigureDefaults.segmentShortSize / 2 + FigureDefaults.corner, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**method of create HyperLink **/
function figure_HyperLink(x, y) {

    var f = new Figure('HyperLink');
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.HyperLink;
    f.style.fillStyle = FigureDefaults.fillStyle;

    f.properties.push(new BuilderProperty('基本属性-HyperLink', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('テキスト', 'primitives.0.str', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('フォントサイズ', 'primitives.0.size', BuilderProperty.TYPE_TEXT_FONT_SIZE));
    f.properties.push(new BuilderProperty('フォント', 'primitives.0.font', BuilderProperty.TYPE_TEXT_FONT_FAMILY));
    //f.properties.push(new BuilderProperty('对齐', 'primitives.0.align', BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT));
    f.properties.push(new BuilderProperty('アンダースコア', 'primitives.0.underlined', BuilderProperty.TYPE_TEXT_UNDERLINED));
    f.properties.push(new BuilderProperty('太字のフォント', 'primitives.0.fontWeight', BuilderProperty.TYPE_TEXT_FONTWEIGHT));
    f.properties.push(new BuilderProperty('フォントの色', 'primitives.0.style.fillStyle', BuilderProperty.TYPE_COLOR));

    f.properties.push(new BuilderProperty('コントロールプロパティ', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('接続アドレス', 'URL', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('窓を開ける', 'WinOpenModel', BuilderProperty.CCFormEnum));

    var t2 = new Text("マイハイパーコネクト。", x, y + FigureDefaults.radiusSize / 2, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = "#0000ff";
    t2.underlined = true;

    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**method of create image **/
function figure_Image(x,  y) {
    var f = new Figure("Image");
    //ccform Property
    f.CCForm_Shape = CCForm_Controls.Image;
    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/basic/TempleteFile.png";

    var ifig = new ImageFrame(url, x, y, true, 120, 140);
    ifig.debug = true;

    f.addPrimitive(ifig);

    //Text
    f.properties.push(new BuilderProperty('コントロールプロパティ-Image', 'group', BuilderProperty.TYPE_GROUP_LABEL));
    f.properties.push(new BuilderProperty(BuilderProperty.SEPARATOR));
    f.properties.push(new BuilderProperty('アプリの種類', 'ImgAppType', BuilderProperty.CCFormEnum));
    f.properties.push(new BuilderProperty('画像をアップロード', 'ImgURL', BuilderProperty.CCFormUpload));
    f.properties.push(new BuilderProperty('パスを指定します', 'ImgPath', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('接続されている画像', 'LinkURL', BuilderProperty.TYPE_SINGLE_TEXT));
    f.properties.push(new BuilderProperty('窓を開ける', 'WinOpenModel', BuilderProperty.CCFormEnum));

    var t2 = new Text("", x, y + 36, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**Controls set declaration*/
figureSets["Data"] = {
    name: 'フィールドコントロール',
    description: 'A Data set of Controls',
    figures: [
        { figureFunction: "TextBox", name: CCForm_Controls.TextBox, image: "TextBox.png",CNName:"テキストタイプ" },
        { figureFunction: "TextBox", name: CCForm_Controls.TextBoxInt, image: "TextBoxInt.png",CNName:"数値タイプ" },
        { figureFunction: "TextBox", name: CCForm_Controls.TextBoxMoney, image: "TextBoxMoney.png", CNName: "金額タイプ" },
        { figureFunction: "TextBox", name: CCForm_Controls.TextBoxFloat, image: "TextBoxFloat.png", CNName: "フローティングポイントタイプ" },
        { figureFunction: "TextBox", name: CCForm_Controls.Date, image: "TextBoxDate.png", CNName: "日付タイプ" },
        { figureFunction: "TextBox", name: CCForm_Controls.DateTime, image: "TextBoxDateTime.png", CNName: "日時タイプ" },
        { figureFunction: "TextBox", name: CCForm_Controls.CheckBox, image: "Checkbox.png", CNName: "選択ボックス" },
        { figureFunction: "TextBox", name: CCForm_Controls.RadioButton, image: "Radiobutton.png", CNName: "ラジオボタンを列挙する" },
        { figureFunction: "TextBox", name: CCForm_Controls.DropDownListEnum, image: "DropDownListEnum.png", CNName: "ドロップダウンボックスを列挙する" },
        { figureFunction: "TextBox", name: CCForm_Controls.DropDownListTable, image: "DropDownListTable.png", CNName: "外付けキードロップダウンボックス" }
     //   { figureFunction: "TextBox", name: CCForm_Controls.ListBox, image: "ListBox.png", CNName: "外部数据源" },
        //{ figureFunction: "TextBox", name: CCForm_Controls.HiddendField, image: "HiddendField.png", CNName: "隐藏字段" }
    ]
};

//文本框创建控件
function figure_TextBox(x, y) {

    var f = new Figure("TextBox");

    //ccform Property
    f.CCForm_Shape = CCForm_Controls.TextBox;

    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    if (createFigureName != "RadioButton") {
        //Image
        var url = figureSetsURL + "/Data/TextBoxBig.png";

        var ifig = new ImageFrame(url, x, y, true, 150, 30);
        ifig.debug = true;
        f.addPrimitive(ifig);
    }
    var t2 = new Text("", x, y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.finalise();
    return f;
}

/**Controls set declaration*/
figureSets["Components"] = {
    name: 'コンポーネントクラス',
    description: 'コンポーネント制御',
    figures: [
        { figureFunction: "Square", name: CCForm_Controls.Dtl, image: "Dtl.png", CNName: "スケジュール/テーブルから" },
        { figureFunction: "Square", name: "AthMulti", image: "AthMulti.png", CNName: "複数の添付ファイル" },
        { figureFunction: "Square", name: "AthSingle", image: "AthSingle.png", CNName: "シングルアタッチメント" },
        { figureFunction: "Square", name: "AthImg", image: "AthImg.png", CNName: "写真の添付" },
        { figureFunction: "Square", name: "HandSiganture", image: "HandSiganture.png", CNName: "署名ボード" },
        { figureFunction: "Square", name: "iFrame", image: "iFrame.png", CNName: "フレーム" },
        { figureFunction: "Square", name: "Fieldset", image: "Fieldset.png", CNName: "グループ化" }
    ]
};

/**Controls set declaration*/
figureSets["mobile"] = {
    name: 'モバイルコンポーネント',
    description: '携帯電話、タブレット、モバイルデバイスアプリケーションに関連するコンポーネント',
    figures: [
        { figureFunction: "Square", name: "Map", image: "Map.png", CNName: "マップコントロール" },
        { figureFunction: "Square", name: "Camera", image: "Camera.png", CNName: "写真のアップロード" },
        { figureFunction: "Square", name: "SoundRecord", image: "SoundRecord.png", CNName: "録音" },
        { figureFunction: "Square", name: "VideoRecord", image: "VideoRecord.png", CNName: "ビデオ" },
        { figureFunction: "Square", name: "QRCode", image: "QRCode.png", CNName: "QRコード" }
    ]
};


/**Controls set declaration*/
figureSets["ccbpm"] = {
    name: 'フローコンポーネント',
    description: 'ccbpmに関連するフローコンポーネント',
    figures: [
        { figureFunction: "Square", name: "CheckGroup", image: "CheckGroup.png", CNName: "監査グループ" },
        { figureFunction: "Square", name: "FlowChart", image: "FlowChart.png", CNName: "軌道グラフ" },
        { figureFunction: "Square", name: "FrmCheck", image: "FrmCheck.png", CNName: "監査コンポーネント" },
        { figureFunction: "Square", name: "SubFlowDtl", image: "SubFlowDtl.png", CNName: "サブフロー" },
        { figureFunction: "Square", name: "ThreadDtl", image: "ThreadDtl.png", CNName: "子スレッド" },
        { figureFunction: "Square", name: "FrmTransferCustom", image: "FrmTransferCustom.png", CNName: "流通習慣" }
    ]
};

function figure_Square(x, y) {

    var f = new Figure("Square");


    //ccform Property
    f.CCForm_Shape = CCForm_Controls.TextBox;

    f.style.fillStyle = FigureDefaults.fillStyle;
    f.style.strokeStyle = FigureDefaults.strokeStyle;

    //Image
    var url = figureSetsURL + "/Data/TextBoxBig.png";

    var ifig = new ImageFrame(url, x, y, true, 150, 30);
    ifig.debug = true;
    f.addPrimitive(ifig);

    var t2 = new Text("", x, y, FigureDefaults.textFont, FigureDefaults.textSize);
    t2.style.fillStyle = FigureDefaults.textColor;
    f.addPrimitive(t2);

    f.finalise();
    return f;
    
}
