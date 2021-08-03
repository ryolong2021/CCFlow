/**枚举数组值集合**/
CCForm_Control_Enum = {
    /**
    this enum use for Image,Button
    **/
    WinOpenModel: [{ Text: '新ウィンドウ', Value: '_blank' },
                 { Text: '親ウィンドウ', Value: '_parent' },
                 { Text: '本ウィンドウ', Value: '_self' },
                 { Text: 'カスタマイズ', Value: 'def' }
    ],
    /**
    this enum use for Image,
    **/
    ImgAppType: [{ Text: 'ローカル写真', Value: '0' },
                 { Text: 'パス指定', Value: '1' }
    ],
    /**
    this enum use for TextBox,
    **/
    UIIsEnable: [{ Text: '編集不可', Value: '0' },
                 { Text: '編集可能', Value: '1' }
    ],
    /**
    this enum use for TextBox,
    **/
    UIVisible: [{ Text: '表示不可', Value: '0' },
                { Text: 'レイアウト表示可', Value: '1' }
    ],
    /**
    this enum use for Button,
    **/
    ButtonEvent: [{ Text: '無効', Value: '0' },
                { Text: 'プロシージャを実行します', Value: '1' },
                { Text: 'sqlを実行します', Value: '2' },
                { Text: 'URLを実行する', Value: '3' },
                { Text: 'Webサービスを実行する', Value: '4' },
                { Text: 'exeを実行する', Value: '5' },
                { Text: 'JSスクリプトを実行する', Value: '6' }
    ],
    /**
    this enum use for TextBox,
    **/
    DefVal: [{ Text: 'システム規則のデフォルト値を選択', Value: '' },
                { Text: 'ログインメンバーアカウント', Value: '@WebUser.No' },
                { Text: 'ログインメンバー名前', Value: '@WebUser.Name' },
                { Text: 'ログインメンバー部署番号', Value: '@WebUser.FK_Dept' },
                { Text: 'ログインメンバー部門名', Value: '@WebUser.FK_DeptName' },
                { Text: 'ログインメンバー部署フルネーム', Value: '@WebUser.FK_DeptFullName' },
                { Text: '現在の日付-yyyy年mm月dd日', Value: '@yyyy年mm月dd日' },
                { Text: '現在日付-yy年mm月dd日', Value: '@yy年mm月dd日' },
                { Text: '今年', Value: '@FK_ND' },
                { Text: '今月', Value: '@FK_YF' },
                { Text: '現在の仕事は人員を扱うことができます', Value: '@CurrWorker' }
    ],
    
    /** 
       this enum use for SignType,
     **/
    SignType: [ { Text: 'なし', Value: '0' },
                { Text: '絵の署名', Value: '1' },
                { Text: 'シャンドンCA署名', Value: '2' },
                { Text: 'シャンドンCA署名', Value: '3' }
                ],
    /**
    this enum use for TextBox,
    **/
    UIIsInput: [{ Text: 'いいえ', Value: 'false' },
                { Text: 'はい', Value: 'true' }
    ]
};
/** CCForm 数据字段属性 
*** proName:属性英文名称
*** ProText:属性中文标签
*** DefVal:默认值,系统字段替换规则,节点编号@FrmID@,字段名@KeyOfEn@
*** DType:属性面板生成控件类型，字符型string,整数int,浮点型float,下拉框enum,超链接href,横线hr,分组标签grouplabel
*** ProType：属性类型，根据此类型生成不同的控件。
*************BuilderProperty.TYPE_GROUP_LABEL -- 分组标签
*************BuilderProperty.SEPARATOR        -- 横线
    
*************BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly -- 单行只读文本框
*************BuilderProperty.TYPE_SINGLE_TEXT          -- 单行可编辑文本框
*************BuilderProperty.TYPE_TEXT                 -- 多行可编辑文本框

*************BuilderProperty.TYPE_TEXT_FONT_FAMILY    -- 选择字体下拉框
*************BuilderProperty.TYPE_TEXT_FONT_SIZE      -- 选择字体大小下拉框
*************BuilderProperty.TYPE_TEXT_FONT_ALIGNMENT -- 字体对齐方式下拉框
*************BuilderProperty.TYPE_TEXT_UNDERLINED     -- 设置是否显示下划线按钮
*************BuilderProperty.TYPE_TEXT_FONTWEIGHT     -- 字体加粗样式下拉框
*************BuilderProperty.TYPE_COLOR               -- 颜色选择框

*************BuilderProperty.CCFormEnum   -- ccform定义枚举值下拉框
*************BuilderProperty.CCFormLink   -- ccform定义超链接
**/
CCForm_Control_Propertys = {
    TextBoxStr: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxInt: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxFloat: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxMoney: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxDate: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxDateTime: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    TextBoxBoolean: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    DropDownListEnum: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '列挙キー', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    RadioButton: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    DropDownListTable: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Propertys', ProText: '属性', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    Dtl: [{ proName: 'No', ProText: '明細表番号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                //  { proName: 'PTable', ProText: '存储表', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID@&FK_MapDtl=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }

    ],
    Fieldset: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
    ],
    AthMulti: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '設定', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    AthSingle: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '設定', DefVal: '/WF/Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PK=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    AthImg: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
    ],
    FlowChart: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    ThreadDtl: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    SubFlowDtl: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    FrmCheck: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    HandSiganture: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
             { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    iFrame: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    CheckGroup: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                 { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID', DType: 'href', ProType: BuilderProperty.CCFormLink }
    ],
    Image: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
    ]
};


/*控件的一些默认值  如宽、高  中英文 名称互换*/
CCForm_Control_DefaultPro = {
    TextBoxStr: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxInt: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxFloat: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxMoney: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxDate: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxDateTime: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    TextBoxBoolean: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    DropDownListEnum: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    RadioButton: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    DropDownListTable: { "DefaultWidth": 120, DefaultHeight: 23, ControlLab: "" },
    Dtl: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    Fieldset: { "DefaultWidth": 100, DefaultHeight: 200, ControlLab: "" },
    AthMulti: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    AthSingle: { "DefaultWidth": 100, DefaultHeight: 200, ControlLab: "" },
    AthImg: { "DefaultWidth": 200, DefaultHeight: 200, ControlLab: "" },
    FlowChart: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    ThreadDtl: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    SubFlowDtl: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    FrmCheck: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
     Image: { "DefaultWidth": 150, DefaultHeight: 150, ControlLab: "" },
    HandSiganture: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    iFrame: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
    CheckGroup: { "DefaultWidth": 500, DefaultHeight: 120, ControlLab: "" },
};


