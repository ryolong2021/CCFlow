/*****************▼**********************共通******************▼********************/

// ▼▼▼汎用定数定義▼▼▼
/** 空白 */
const STRING_EMPTY = "";
/** 半角スペース */
const HALF_SPACE = " ";
/** 全角スペース */
const FULL_SPACE = "　";
/** 全角マイナス */
const FULL_MINUS = "－";
/**区分マスタ:Value */
const MT_KBN_KEYVALUE = "KBNVALUE";
/**区分マスタ:Name */
const MT_KBN_KEYNAME = "KBNNAME";
/**MODE:Y */
const MODE_KBN_YES = "Y";
/**自動承認ID */
const AUTO_APPROVAL_ID = "sys0000001";
// 自動生成フロー
const AUTO_CREATE_FLOW_NO = ["007", "017"];
// 自動メール送信　申請フロークラス名
const AUTO_MAIL_CLASS = {
    "CONDOLENCE": "Mn_CondolenceMailSend",
}
// ▲▲▲汎用定数定義▲▲▲

// ▼▼▼ボタン押下後処理区分▼▼▼
/** １：提出 */
const BTN_SUB_SUBMIT = 1;
/** ２：一時保存 */
const BTN_TEMPORARILY_SAVE = 2;
/** ３：引戻 */
const BTN_REFUND = 3;
/** ４：承認 */
const BTN_APPROVAL = 4;
/** ５：否認 */
const BTN_DENIAL = 5;
/** ６：差戻 */
const BTN_REMAND = 6;
/** ７：修正 */
const BTN_EDIT = 7;
/** ８：キャンセル */
const BTN_DELETE = 8;
/** ９：履歴コピー */
const BTN_RIREKICOPY = 9;

// ▲▲▲ボタン押下後処理区分▲▲▲

// ▼▼▼ダイアログ表示区分▼▼▼
/** alert : 警告 */
const DIALOG_ALERT = "alert";
/** confirm : 確認 */
const DIALOG_CONFIRM = "confirm";
/** info : 情報 */
const DIALOG_INFO = "info";
// ▲▲▲ボタン押下後処理区分▲▲▲ 

// ▼▼▼ラジオボタン表示区分▼▼▼
/**マスター区分:あり */
const MT_KBN_ARI = "1";
/**マスター区分:なし */
const MT_KBN_NASI = "0";
/**マスター区分:必要 */
const MT_KBN_HITUYOU = "0";
/**マスター区分:不要 */
const MT_KBN_FUYOU = "1";
/**送信要否:しない */
const MT_KBN_EMAIL_YOIHI_KBN_SINAI = "0";
/**送信要否:する */
const MT_KBN_EMAIL_YOIHI_KBN_SURU = "1";
// ▲▲▲ラジオボタン押下後処理区分▲▲▲

// ▼▼▼日付フォーマット定義▼▼▼
/**YYYY/MM/DD */
const DATE_FORMAT_MOMENT_PATTERN_1 = "YYYY/MM/DD";
/**HH:mm */
const TIME_FORMAT_MOMENT_PATTERN_1 = "HH:mm";
/**YYYY/MM/DD HH:mm */
const DATETIME_FORMAT_MOMENT_PATTERN_1 = "YYYY/MM/DD HH:mm";
/**YYYY/MM/DD HH:mm:ss */
const DATETIME_FORMAT_MOMENT_PATTERN_2 = "YYYY/MM/DD HH:mm:ss";
/**YYYYMMDDHHmmssSSS */
const DATETIME_FORMAT_MOMENT_PATTERN_3 = "YYYYMMDDHHmmssSSS";

// ▲▲▲日付フォーマット定義▲▲▲

// ▼▼▼ワークフローステータス▼▼▼
/**フロー初期値 */
const WF_STATE_NULL = "";
/**フロー新規申請*/
const WF_STATE_INIT = 0;
/**フロー一時保存*/
const WF_STATE_DRAFT = 1;
/**フロー承認待ち*/
const WF_STATE_SINSEIZUMI = 2;
/**フロー完了*/
const WF_STATE_OVER = 3;
/**フロー差戻*/
const WF_STATE_BACK = 5;
// ▲▲▲ワークフローステータス▲▲▲

// ▼▼▼ワークフロー番号▼▼▼
//出張申請
const WF_ID_SINSEI = "006"
//出張報告
const WF_ID_REPORT = "007"
//年金手帳再交付
const WF_ID_ANNUITYNOTEREISSUE = "008"
//弔事連絡票
const WF_ID_CONDOLENCE = "009"
//結婚届
const WF_ID_MARRIAGE = "010"
//資格免許登録
const WF_ID_LICENSE = "014"
//会社認定資格登録申請
const WF_ID_COMPANY_LICENSE = "016"
//本人情報変更
const WF_ID_PERSONALINFOCHANGE = "018"
//還付金申請・引戻
const WF_ID_REFUND_APPLY = "017"
//介護勤務新規・更新・変更
const WF_ID_NURSING_WORK_APPLY = "020";

// ▲▲▲ワークフロー番号▲▲▲

// ▼▼▼メール送信フロー▼▼▼
//弔事連絡
const MAIL_TYPE_CONDOLENCE = "009";
//メール送信ステータス
//新規作成
const MAIL_STSTE_NEW = "A01";
//修正
const MAIL_STSTE_EDIT = "A02";
//手配状態変更
const MAIL_STSTE_APPROVAL = "B01";
//メール送信先_手配業者
const MAIL_TO_TEHAIGIYOSIYA = "1";
// ▲▲▲メール送信フロー▲▲▲

// ▼▼▼一覧タイプ▼▼▼
/** 自分を見るの完了 */
const GET_MY_COMPLETE = "0";      　　　
/** 自分を見るの未完了 */
const GET_MY_UNCOMPLETE = "1";    　　　
/** 自分を見るの差戻 */
const GET_MY_DIFFERENCE = "2";    　　　
/** 自分を見るの下書き */
const GET_MY_DRAFT = "3";         　　　
/** 承認依頼を見るの完了 */
const GET_APPROVAL_COMPLETE = "4";      
/** 承認依頼を見るの未完了 */
const GET_APPROVAL_UNCOMPLETE = "5";    
/** 承認依頼を見るの処理待ち */
const GET_APPROVAL_INPROCESS = "6";
/** 代理申請 */
const GET_AGENT_APP = "7";

// ▲▲▲一覧タイプ▲▲▲

// ▼▼▼画面名▼▼▼
/**  申請する一覧*/
const FROM_PAGE_APPLYMENU = "form_applymenu.html";
/**  自分を見る一覧*/
const FROM_PAGE_REQUESTLIST = "form_requestlist.html";
/**  承認依頼を見る一覧*/
const FROM_PAGE_APPROVAL_REQUESTLIST = "form_approval_request_list.html";
/**  代理申請一覧 メニューパスではないので*/
const FROM_PAGE_AGENT_APP_LIST = "../condolence/form_agent_app_list.html";
// ▲▲▲画面名▲▲▲

// ▼▼▼申請区分▼▼▼
/**申請区分:本人申請 */
const SHINSEISYA_KBN_HONNIN = "0";
/**申請区分:代理申請 */
const SHINSEISYA_KBN_DAIRI = "1";
/**申請区分:会社認定 */
const SHINSEISYA_KBN_COMPANY = "2";
// ▲▲▲申請区分▲▲▲

// ▼▼▼対象者の障害区分▼▼▼
/** 対象外 */
const TARGET_OBSTACLE_NONE = "0";
/** 一般障害者 */
const TARGET_OBSTACLE_GENERAL = "1";
/** 特別障害者 */
const TARGET_OBSTACLE_SPECIAL = "2";
// ▲▲▲対象者の障害区分▲▲▲

/*****************▲**********************共通******************▲********************/
/*****************▼**********************API関数名******************▼********************/

//弔事連絡社員情報取得API名
const GET_CONDOLENCE_SHAIN_INFO_APINAME = "Get_New_Condolence_Shain_Info";

const GET_CONDOLENCE_MEIGi_INFO_APINAME = "Get_New_Condolence_Meigi_Info";

//社員情報取得API名
const GET_SHAIN_INFO_APINAME = "Get_Shain_Info";
//会社コードと会社名の情報の取得API名
const GET_COMPANIE_INFO_APINAME = "Get_KaishaCode";
//登録者名前情報の取得API名
const GET_USER_NAME_INFO_APINAME = "Get_User_Name";
//従業員区分リストデータ取得API名
const GET_EMPLOYEE_KBN_LIST_APINAME = "Get_Jg_Code_List";
//所属マスタ情報の取得の取得API名
const GET_DEPARTMENT_INFO_APINAME = "Get_Department_Info";
//電子申請連携マスタ（社員別）データ取得API名
const GET_EBS_EMPLOYEE_INFO_APINAME = "Get_Ebs_Employee_Info";
// 銀行口座マスタデータ取得API名
const GET_BANK_ACCOUNTS = "Get_Bank_Accounts";
// 国籍一覧マスタデータ取得API名
const GET_COUNTRY_LIST = "Get_Country_List";
// 金融機関一覧取得API名
const GET_FINANCIAL_CORP_LIST = "Get_Financial_Corp_List";
// 金融機関一覧取得API名　金融機関選択画面初期化用
const GET_FINANCIAL_CORP_LIST_INIT = "Get_Financial_Corp_List_Init";
// 注意事項内容取得API名
const GET_COMPANY_EMPLOYEE_CONDITIONS = "Get_Company_Employee_Conditions";
// BS業務部所属判定API名
const CHECK_BS_GROUP_KBN = "Check_BS_GROUP_KBN";
// 確定拠出年金会社負担金額情報取得API名
const GET_PENSION_CORP_BURDEN_BY_SHAINBANGO = "Get_Pension_Corp_Burden_By_ShainBango";
// 家族情報取得するAPI名
const GET_EMPLOYEE_FAMILY_INFO_BY_SHAINBANGO = "Get_Employee_Family_Info";

/*****************▲**********************API関数名******************▲********************/
/*****************▼**********************メニュー******************▼********************/

// 機能コード
const FUNCTION_CODE_MAINTENANCE = "K007";
const FUNCTION_CODE_ROMU_MENU = "K004";
const FUNCTION_CODE_MAINTENANCE_MAIL = "K007001";


/*****************▲**********************メニュー******************▲********************/
/*****************▼**********************弔事連絡票******************▼********************/
/** 逝去時刻 */
const deathTimeList = { "content": [{ "value": "00:00", "name": "０時頃" }, { "value": "01:00", "name": "１時頃" }, { "value": "02:00", "name": "２時頃" }, { "value": "03:00", "name": "３時頃" }, { "value": "04:00", "name": "４時頃" }, { "value": "05:00", "name": "５時頃" }, { "value": "06:00", "name": "６時頃" }, { "value": "07:00", "name": "７時頃" }, { "value": "08:00", "name": "８時頃" }, { "value": "09:00", "name": "９時頃" }, { "value": "10:00", "name": "１０時頃" }, { "value": "11:00", "name": "１１時頃" }, { "value": "12:00", "name": "１２時頃" }, { "value": "13:00", "name": "１３時頃" }, { "value": "14:00", "name": "１４時頃" }, { "value": "15:00", "name": "１５時頃" }, { "value": "16:00", "name": "１６時頃" }, { "value": "17:00", "name": "１７時頃" }, { "value": "18:00", "name": "１８時頃" }, { "value": "19:00", "name": "１９時頃" }, { "value": "20:00", "name": "２０時頃" }, { "value": "21:00", "name": "２１時頃" }, { "value": "22:00", "name": "２２時頃" }, { "value": "23:00", "name": "２３時頃" }] };
/** 性別：男 */
const GENDER_MALE = "5";
/** 性別：女 */
const GENDER_FMALE = "6";
/** 同居区分:同居 */
const DOKYO_BEKYO_KBN_YES = "1";
/** 同居区分:別居 */
const DOKYO_BEKYO_KBN_NO = "0";
/** 税扶養区分:扶養親族 */
const FUYOUKBN_BEKYO_KBN_YES = "1"
/** 税扶養区分:扶養でない */
const FUYOUKBN_BEKYO_KBN_NO = "0"
/**喪主区分:喪主が自分 */
const ORGANIZER_KBN_HONNIN = "1";
/**喪主区分:喪主が自分以外 */
const ORGANIZER_KBN_HONNIN_IGAI = "0";
/**通夜/告別式の有無区分：通夜と告別式 */
const FUNERAL_KBN_BOTH = "3";
/**通夜/告別式の有無区分：通夜 */
const FUNERAL_KBN_TSUYA = "1";
/**通夜/告別式の有無区分：告別式 */
const FUNERAL_KBN_KOKUBETSUSHIKI = "2";
/**通夜/告別式の有無区分：なし */
const FUNERAL_KBN_NONE = "0";
/**checkbox：checked */
const CHECKBOX_CHECKED = 1;
/**checkbox：unchecked */
const CHECKBOX_UNCHECKED = 0;
/**供花お届け先場所区分:通夜 */
const LOCATION_TSUYA_KBN = "0";
/**供花お届け先場所区分:告別式 */
const LOCATION_KOKUBETSUSHIKI_KBN = "1";
/**供花お届け先場所区分:後飾り */
const LOCATION_ATOKA_KBN = "2";
/**受取区分：受け取る */
const NECESSARY_KBN_HITUYOU = "0";
/**受取区分：辞退する */
const NECESSARY_KBN_JITAI = "1";
/**出向先区分_出向している */
const SYUKOU_KBN_YES = "1";
/**出向先区分_出向してない */
const SYUKOU_KBN_NO = "0";
/** 弔事基準項目 */
const STANDARD_ITEMS = {
    "KORYO": { "UNIT": " 円", "YES": "あり", "NONE": "なし", },
    "KYOKA": { "UNIT": " 基", },
    "TYODEN": { "UNIT": " 通", },
    "NOT_APPLICABLE": "支給基準外",
}




/*****************▲**********************弔事連絡票******************▲********************/

/*****************▼**********************手配業者依頼一覧画面********▼********************/
/**手配状態 */
/**未手配 */
const STATE_MITEHAI = "0";
/**手配済み */
const STATE_TEHAIZIMI = "1";
/**手配不能 */
const STATE_TEHAIFUNO = "2";
/**確認中 */
const STATE_CONFIRMING = "3";
/**キャンセル */
const STATE_CANCEL = "4";
/**キャンセル(有償) */
const STATE_CANCEL_PAID = "5";
/**手配不能コメントの最大長さ */
const MAX_COMMENT_LEN = "100";
/**手配状態 */
const TEHAI_STATE = "0";

/*****************▲**********************手配業者依頼一覧画面********▲********************/
/*****************▼**********************メール送信メンテナンス画面********▼********************/
/**新規作成又は修正 */
/**新規作成 */
const CREATE_MODE = "0";
/**修正 */
const EDIT_MODE = "1";
/**コピー */
const COPY_MODE = "2";
/**連絡先タイプ */
const SOUSINSAKI_TYPE_SITE = "00";
/**可変フラグ */
/**可変 */
const KAHEN_FLG_KAHEN = "0";
/**固定 */
const KAHEN_FLG_KOTEI = "1";
/** 稼働フラグ */
/** 停止中 */
const OPERATION_FLG_OFF = "0";
/** 稼働中 */
const OPERATION_FLG_ON = "1";
/** モードフラグ */
/** 初期化 */
const INIT_MODE = 0;
/** 確認ボタン押す後 */
const CONFIRM_MODE = 1;

/*****************▲**********************メール送信メンテナンス画面********▲********************/
/*****************▼**********************慶弔基準設定メンテナンス画面********▼******************/
/**画面処理モード*/
/**アップロード */
const UPLOAD_MODE = "0";
/**ダウンロード */
const DOWNLOAD_MODE = "1";
/**基準削除 */
const KIJUN_DELETE_MODE = "2";
/**適用開始年月日更新 */
const YMD_UPDATE_MODE = "3";
/**基準個数
/**基準が０の場合*/
const KIJUN_ZERO = "1";
/**基準が1の場合*/
const KIJUN_ONE = "2";
/**基準が2の場合*/
const KIJUN_TWO = "3";


/*****************▲**********************慶弔基準設定メンテナンス画面********▲******************/
/*****************▼**********************年金手帳再交付申請******************▼********************/
/** 年金手帳再交付申請事由 */
/** 紛失 */
const ANNUITYNOTEREISSUE_REASON_LOST = "0";
/** 破損（汚れ） */
const ANNUITYNOTEREISSUE_REASON_LOSS = "1";
/** その他 */
const ANNUITYNOTEREISSUE_REASON_OTHER = "9";

/** 住民票訂正 */
/** 訂正なし */
const JYUMINHYO_TEISEI_NASHI = "0";
/** 訂正あり */
const JYUMINHYO_TEISEI_ARI = "1";

/** 社会保険区分 */
/** 加入なし */
const SOCIAL_INSURANCE_JOIN_NASHI = 0;
/** 加入あり */
const SOCIAL_INSURANCE_JOIN_ARI = 1;
/*****************▲**********************年金手帳再交付申請******************▲********************/
/*****************▼**********************家族情報届******************▼********************/
/** 健康保険扶養申請- */
/** 申請する */
const INSUR_SUPPORT_APP = "0";
/** 申請しない */
const INSUR_SUPPORT_NON_APP = "1";

/** 配偶者情報-申請区分 */
/** 離婚 */
const SPOUSE_APP_DIVORCE = "0";
/** 死別 */
const SPOUSE_APP_BEREAVEMENT = "1";

/** 固定文言設定（年月日） */
const STR_DIVORCE_BEREAVEMENT = {};
STR_DIVORCE_BEREAVEMENT[SPOUSE_APP_DIVORCE] = "離婚日";
STR_DIVORCE_BEREAVEMENT[SPOUSE_APP_BEREAVEMENT] = "逝去日";

/** 固定文言設定（健康保険扶養申請済み/健康保険扶養申請なし） */
const STR_INSUR_STATUS = {};
STR_INSUR_STATUS[INSUR_SUPPORT_APP] = "健康保険扶養申請済み";
STR_INSUR_STATUS[INSUR_SUPPORT_NON_APP] = "健康保険扶養申請なし";

/** 性別 */
/** 男 */
const SEX_OTOKO = "0";
/** 女 */
const SEX_ONNA = "1";

/** 年齢制御 */
/** 男性：17 */
const MALE_AGE_CONTROL = 17;
/** 女性：15 */
const FEMALE_AGE_CONTROL = 15;

/** 婚姻区分 */
/** 独身：0 */
const MARRIAGE_CLASS_UNMARRIED = 0;
/** 既婚：1 */
const MARRIAGE_CLASS_MARRIED = 1;

/** 同居/別居 */
/** 同居 */
const LIVE_DOKYO = "0";
/** 別居 */
const LIVE_BEKYO = "1";

/** 祝電申請区分 */
/** 必要 */
const CONGRATULATEAPP_HITUYO = "0";
/** 辞退 */
const CONGRATULATEAPP_JITAI = "1";

/** 祝金申請区分 */
/** 必要 */
const CONGRATULATEMONEY_HITUYO = "0";
/** 辞退 */
const CONGRATULATEMONEY_JITAI = "1";

/** 対象者の扶養有無- */
/** ⾃分の扶養に⼊れる */
const DEPENDENT_TARGET_ENTER = "0";
/** ⾃分の扶養に⼊れない */
const DEPENDENT_TARGET_NOT_ENTER = "1";

/** 健康保険扶養申請区分- */
/** 健康保険扶養申請済み */
const HEALTH_INSURANCE_APPLIED = "0";
/** 健康保険扶養未申請 */
const HEALTH_INSURANCE_NOT_APPLIED = "1";

/** 続柄：配偶者 */
const FAMILY_RELATIONSHIP_SPOUSE = "00";

/** 従業員本人の年間所得見積額 */
/** 1000万円以下 */
const ESTIMATED_INCOME_CLASS = "0";
/** ⾃分の扶養に⼊れない */
const ESTIMATED_INCOME_CLASS_BEYOND = "1";

/** 対象合計所得金額 */
/** 配偶者の場合、95万円 */
const INCOME_AMOUNT_FOR_SPOUSE = 950000;
/** 配偶者の以外の場合、48万円 */
const INCOME_AMOUNT_FOR_SPOUSE_OUT = 480000;

/** 税扶養対象年齢制限 */
/** その親族が16歳以上であること */
const TAX_DEPENDENT_AGE = 16;

/** 税扶養区分（判定） */
/** 税扶養区分:税扶養対象 */
const TAX_TARGET = "0"
/** 税扶養区分:適用なし（税扶養対象外） */
const TAX_OUT_TARGET = "1"
/** 税扶養区分:税扶養対象　（16歳到達までは住民税控除のみ対象） */
const TAX_TARGET_UNDER_SIXTEEN = "3"

/** 従業員区分リストデータの取得 21:従業者区分 */
const EMPLOYEE_KBN = "21";

// ▼▼▼ 家族情報五つ業務のテーブル名の定義 ▼▼▼
/** TT_WF_MARRIAGE：結婚届 */
const FAMILY_MARRIAGE = "TT_WF_MARRIAGE";
const FAMILY_MARRIAGE_TYPE = "001";
/** TT_WF_ADD_FAMILY：家族追加届 */
const FAMILY_ADD = "TT_WF_ADD_FAMILY";
const FAMILY_ADD_TYPE = "002";
/** TT_WF_DIVORCE_BEREAVEMENT：離婚･死別届 */
const FAMILY_DIVORCE_BEREAVEMENT = "TT_WF_DIVORCE_BEREAVEMENT";
const FAMILY_DIVORCE_BEREAVEMENT_TYPE = "003";
/** TT_WF_DEATH：死亡届 */
const FAMILY_DEATH = "TT_WF_DEATH";
const FAMILY_DEATH_TYPE = "004";
/** TT_WF_TRANSFER_FAMILY：家族異動届 */
const FAMILY_TRANSFER = "TT_WF_TRANSFER_FAMILY";
const FAMILY_TRANSFER_TYPE = "005";

/** データタイプ */
/** 個人情報 */
const FAMILY_INDIVIDUAL_INFO = "001";
const FAMILY_INDIVIDUAL_STR = "個人情報";
/** 家族情報 */
const FAMILY_HOUSEHOLD_INFO = "004";
const FAMILY_HOUSEHOLD_STR = "家族情報";
/** 社保所得税情報_所得税 */
const FAMILY_TAX_INFO_INCOME_TAX = "005";
const FAMILY_TAX_STR_INCOME_TAX = "社保所得税情報";


/** 更新モード */
/** 新規登録。重複レコードがあればエラー */
const UPDATE_MODE_NEW_DUPLICATE_ERR = "10";
/** 新規登録。同一レコードがあれば更新 */
const UPDATE_MODE_NEW_DUPLICATE_NEW = "1A";
/** 新規登録。同一レコードがあれば訂正 */
const UPDATE_MODE_NEW_DUPLICATE_UPD = "1B";

/** 更新。レコードがなければエラー */
const UPDATE_MODE_UPD_RCN_ERR = "20";
/** 更新。レコードがなければエラー */
const UPDATE_MODE_UPD_RCN_NEW = "2A";

/** 訂正。レコードがなければエラー。 */
const UPDATE_MODE_CORRECTION_RCN_ERR = "30";
/** 訂正。レコードがなければ新規登録。 */
const UPDATE_MODE_CORRECTION_RCN_NEW = "3A";

/** 論理削除（デートトラックをする）。レコードがなければエラー。 */
const UPDATE_MODE_DEL_RCN_ERR = "40";
/** 物理削除。レコードがなければエラー。 */
const UPDATE_MODE_DEL_RCN_NEW = "50";

// ▲▲▲ 家族情報五つ業務のテーブル名の定義 ▲▲▲

/*****************▲**********************家族情報届******************▲********************/
/*****************▼**********************GLCメンテナンス依頼一覧画面********▼********************/
/**香料区分 */
/**事業所 */
const KORYO_KBN_GIUMUSIYO = "0";
/**辞退 */
const KORYO_KBN_JITAI = "1";
/**GLC */
const KORYO_KBN_GLC = "2";

/*****************▲**********************GLCメンテナンス依頼一覧画面********▲********************/
/*****************▼**********************確定拠出年金　申請、再開、変更、中断画面********▼********************/
/**申請区分 */
/**新規 */
const APPLY_CODE_START = "1";
/**変更 */
const APPLY_CODE_UPDATE = "2";
/**中断 */
const APPLY_CODE_INTERRUPTION = "3";
/**再開 */
const APPLY_CODE_RESTART = "4";

/**確定拠出年金_再申請許可状態 */
/**申請可 */
const REAPPLY_STATUS_POSSIBLE = "1";
/**権限削除(開始申請実施) */
const REAPPLY_STATUS_DELETE_AUTHORITY = "8";
/**申請済 */
const REAPPLY_STATUS_APPLIED = "9";
/**削除 */
const REAPPLY_STATUS_DELETE = "0";

/**現在掛金額 */
const CURRENT_AMT_PAID = "0";
/*****************▲**********************確定拠出年金　申請、再開、変更、中断画面********▲********************/
/*****************▼**********************マッチング拠出 再開申請許可設定画面********▼********************/
/**状態区分 */
/**新規 */
const STATUS_PERMISSION = "許可";
const STATUS_PERMISSION_REMOVE = "未許可";

/**状態 */
const PERMISSION_STATUS = "1";
const PERMISSION_STATUS_REMOVE = "0";


/**会社名 */
const BS_KAISHAMEI = "イオンリテール株式会社";
const BS_KAISHACODE = "0105";
/*****************▲**********************マッチング拠出 再開申請許可設定画面********▲********************/
/*****************▼**********************資格免許登録画面********▼********************/
/**必須項目区分 */
/**必須 */
const HISSU_FLG = "1";

/**確認状態 */
const CHECK_FLG_MIKAKUNIN = "0";

/**代理申請区分 */
/**代理申請 */
const PROXYKBN_AGENT = "Y";
/*****************▲**********************資格免許登録画面********▲********************/
/*****************▼**********************特別買物割引制度画面********▼********************/
/**WP項目区分 */
const REFUND_WP_KBN_KBNVALUE_0 = "0";

const REFUND_WP_KBN_KBNVALUE_1 = "1";

const REFUND_WP_KBN_KBNNAME_0 = "無"; 

const REFUND_WP_KBN_KBNNAME_1 = "有";
/*****************▲**********************特別買物割引制度画面********▲********************/
/*****************▼**********************健康保険扶養 追加申請画面********▼********************/
/**設問の選択肢：はい */
const QUESTION_YES = "1";
/**設問の選択肢：いいえ */
const QUESTION_NO = "0";


/*****************▲**********************健康保険扶養 追加申請画面********▲********************/
/*****************▼**********************本人情報変更画面********▼********************/

/**申請画面のすべての項目は変更なし */
const ALL_HENKOU_NASI = "0";
/**申請画面に変更がある項目が存在する */
const ALL_HENKOU_ARI = "1";
/**税表区分を変更しますか／障害区分を変更しますか：変更します */
const HENKOU_ARI = "1";
/**障害区分：なし */
const SYOUGAI_KBN_NASI = "0";
const HENKOU_ARI_KANJI = "変更あり";
const HENKOU_NASI_KANJI = "変更なし";
const HENKOU_ARI_KBN = "1";
const HENKOU_NASI_KBN = "0";
const HENKOU_NASI_KANJI_2 = "変更しない";
const FONT_WEIGHT_BOLD = "700";
/**寡夫区分 */
/**対象外 */
const WINDOW_CRITERIA_TAISYOUGAI = "0";
/**ひとり親 */
const WINDOW_CRITERIA_SINGLE_PARENT = "1";
/**寡婦 */
const WINDOW_CRITERIA_WIDOW = "2";
/**当てはまるか */
/**はい */
const ANSWER_YES = "0";
/**いいえ */
const ANSWER_NO = "1";
/**勤労学生区分 */
/**対象外 */
const WORKING_STUDENT_FLG_TAISYOUGAI = "0";
/**勤労学生 */
const WORKING_STUDENT_FLG_TAISYOU = "1";
/**障害内容区分 */
/**身体障害 */
const HANDICAPPED_CONTENTS_CLASS_BODY = "1";
/**精神障害 */
const HANDICAPPED_CONTENTS_CLASS_SPIRIT = "3";
/**障害等級 */
/**１級 */
const HANDICAPPED_LEVEL_1 = "1";
/**２級 */
const HANDICAPPED_LEVEL_2 = "2";
/**障害程度 */
/**重度 */
const HANDICAPPED_DEGREE_SEVERE = "0";
/**税表区分変更理由区分 */
/**給与収入のみ */
const ZEIHYOKBN_CHANGE_REASON_SALARY_ONLY = "1";
/**他会社からの給与収入がある（兼業） */
const ZEIHYOKBN_CHANGE_REASON_SIDE_BUSINESS = "2";
/**日払い給与収入 */
const ZEIHYOKBN_CHANGE_REASON_DAILY_PAYMENT = "3";
/**税表区分 */
/**月額甲 */
const TAX_TABLE_1 = "1";
/**月額乙 */
const TAX_TABLE_2 = "2";
/**月額丙 */
const TAX_TABLE_3 = "3";
/**従業員年間所得 */
/**年間所得が500万未満 */
const KAFU_YEAR_SYOTOKU_500_MIMAN = "0";
/**預金種別 */
/**普通 */
const KOZAKBN_FUTTU = "1";
/**ハイフン */
const ZENKAKU_HYPHEN = "―";
/*****************▲**********************本人情報変更画面********▲********************/
/*****************▼**********************本人情報変更照会一覧画面********▼********************/
/**WP項目区分 */
const PERSONAL_INFO_CHANGE = "1";
const PERSONAL_INFO_UNCHANGE = "0";
/**税表変更 */
const ZEIHYO_KBN_CHANGE = 1;
const ZEIHYO_KBN_UNCHANGE = 0;
/**性別 */
const GENDER_MAN = "男";
const GENDER_WOMEN = "女";
/**障害区分変更 */
const HANDICAPPEDEDT_KBN_CHANGE = 1;
const HANDICAPPEDEDT_KBN_UNCHANGE = 0;
/**変更後障害区分変更区分 */ 
const HANDICAPPEDEDT_NOT_APPLICABLE = 0;
const HANDICAPPEDEDT_GENERAL_DISABILITY = 1;
const HANDICAPPEDEDT_SPECIAL_DISABILITY = 2;
/*****************▲**********************本人情報変更照会一覧画面********▲********************/
/*****************▼**********************介護********▼********************/
// 申請内容区分：撤回
const NURSING_FINISH_APPLY_TEKAI = "04";
const NURSING_FINISH_APPLY_SYURYO = "05";
/*****************▲**********************介護********▲********************/
/*****************▼**********************介護勤務申請画面********▼********************/
/**分割取得する */
const NURSING_DIVISION_YES = "01";
/**分割取得しない */
const NURSING_DIVISION_NO = "02";
/**選択される */
const NURSING_SELECTED_YES = "1";
/**選択されない */
const NURSING_SELECTED_NO = "0";
/**介護勤務申請区分:新規 */
const NURSING_APPLY_NEW = "01";
/**介護勤務申請区分:更新 */
const NURSING_APPLY_UPD = "02";
/**日給月給社員 */
const EMPLOYEE_SALARY_TYPE_DAYPAY = "01";
/**時間給社員 */
const EMPLOYEE_SALARY_TYPE_HOURLYPAY = "02";


/*****************▲**********************介護勤務申請画面********▲********************/