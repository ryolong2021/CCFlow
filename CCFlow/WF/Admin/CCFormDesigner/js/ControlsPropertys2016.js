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
                 { Text: '編集可否', Value: '1' }
    ],
    /**
    this enum use for TextBox,
    **/
    UIVisible: [{ Text: '非表示', Value: '0' },
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
                { Text: '現在の日付-yy年mm月dd日', Value: '@yy年mm月dd日' },
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
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal : 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'MinLen', ProText: '最小の長さ', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'MaxLen', ProText: '最大長', DefVal: '300', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
              //    { proName: 'MapExt', ProText: '扩展属性', DefVal: '', DType: 'grouplabel', ProType: BuilderProperty.TYPE_GROUP_LABEL },
               //   { proName: BuilderProperty.SEPARATOR, ProText: '', DefVal: '', DType: 'hr', ProType: BuilderProperty.SEPARATOR },
                  { proName: 'SignType', ProText: '署名モード', DefVal: '0', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'UIIsInput', ProText: '必要ですか', DefVal: '0', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'WinPOP', ProText: 'ウィンドウの戻り​​値を設定する', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/PopVal.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=PopVal_@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'Expression', ProText: '正規表現', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'TBFullCtrl', ProText: 'テキストボックスの自動完了', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/TBFullCtrl.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=@FrmID@_TBFullCtrl_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: 'スクリプト検証', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=@FrmID@&ExtType=InputCheck&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                  ],
    TextBoxInt: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'UIVisible', ProText: '表示可否', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'WinPOP', ProText: '设置开窗返回值', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/PopVal.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=PopVal_@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'Expression', ProText: '正規表現', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'TBFullCtrl', ProText: 'テキストボックスの自動完了', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/TBFullCtrl.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&MyPK=@FrmID@_TBFullCtrl_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自動計算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxFloat: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正規表現', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自動計算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxMoney: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '0.00', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正規表現', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自動計算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxDate: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '@RDT', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  //{ proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正規表現', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自動計算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxDateTime: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '@RDT', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                 // { proName: 'UIVisible', ProText: '是否可见', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'Expression', ProText: '正規表現', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/RegularExpression.aspx?FK_MapData=@FrmID@&RefNo=@KeyOfEn@&OperAttrKey=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'AutoFull', ProText: '自動計算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    TextBoxBoolean: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'AutoFull', ProText: '自動計算', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFull.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    DropDownListEnum: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '列挙キー', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'AutoFullDLL', ProText: 'リストフィルタリングを設定する', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFullDLL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'ActiveDDL', ProText: 'リンケージを設定する（例：州、市のリンケージ）', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'DDLFullCtrl', ProText: 'ドロップダウンボックスを自動的に完了するように設定します', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    RadioButton: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '列挙キー', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'AutoFullDLL', ProText: 'リストフィルタリングを設定する', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFullDLL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'ActiveDDL', ProText: 'リンケージを設定する（例：州、市のリンケージ）', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'DDLFullCtrl', ProText: 'ドロップダウンボックスを自動的に完了するように設定します', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    DropDownListTable: [{ proName: 'FieldText', ProText: '中国名', DefVal: 'FieldText', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'KeyOfEn', ProText: '英語名', DefVal: 'KeyOfEn', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'UIBindKey', ProText: '外部キー/外部テーブル', DefVal: 'UIBindKey', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'DefVal', ProText: 'デフォルト', DefVal: '0', DType: 'int', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'UIIsEnable', ProText: '編集可否', DefVal: '1', DType: 'enum', ProType: BuilderProperty.CCFormEnum },
                  { proName: 'AutoFullDLL', ProText: 'リストフィルタリングを設定する', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/AutoFullDLL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'ActiveDDL', ProText: 'リンケージを設定する（例：州、市のリンケージ）', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/ActiveDDL.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'DDLFullCtrl', ProText: 'ドロップダウンボックスを自動的に完了するように設定します', DefVal: '/WF/Admin/FoolFormDesigner/MapExt/DDLFullCtrl.aspx?FK_MapData=@FrmID@&ExtType=AutoFull&RefNo=@FrmID@_@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    Dtl: [{ proName: 'No', ProText: '明細表番号', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
                  { proName: 'PTable', ProText: '収納テーブル', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT_ReadOnly },
//                  { proName: 'Set', ProText: '设置aspx', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.aspx?FK_MapData=@FrmID@&FK_MapDtl=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink },
                  { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=@FrmID@&FK_MapDtl=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }

                 ],
    Fieldset: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
                 ],
    AthMulti: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=@FrmID@&Ath=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    AthSingle: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Set', ProText: '設定', DefVal: '/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=@FrmID@&Ath=@KeyOfEn@', DType: 'href', ProType: BuilderProperty.CCFormLink }
                 ],
    AthImg: [{ proName: 'No', ProText: 'ナンバリング', DefVal: 'No', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT },
                  { proName: 'Name', ProText: '名前', DefVal: 'Name', DType: 'string', ProType: BuilderProperty.TYPE_SINGLE_TEXT }
                 ]
     };

