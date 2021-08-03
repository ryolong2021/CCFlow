using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using PDFlib_dotnet;

namespace BP.WF.HttpHandler
{
    public class WF_PersonalInfoChange: BP.WF.HttpHandler.DirectoryPageBase
    {

        const string font_dir = "wwwroot/resource/fonts";
        const string template_dir = "wwwroot/resource/templates";
        const string import_pdf = "本人情報変更届（貼り付け台紙）.pdf";
        const string ZENKAKU_HYPHEN = "―";
        const string FUTOJI_FLG = "1";
        const string HOSI_MARK = "＊";

        /// <summary>
        /// 金融機関一覧取得用メソッド
        /// </summary>
        /// <returns></returns>
        public string GetBankList()
        {
            try
            {
                // 検索条件の取得
                BankListReq cond =
                    JsonConvert.DeserializeObject<BankListReq>(
                        this.GetRequestVal("BankListReq"));

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("BankCode", (cond.BankCode == null || cond.BankCode == "") ? "" : cond.BankCode);
                dic.Add("BankNameKana", (cond.BankNameKana == null || cond.BankNameKana == "") ? "" : cond.BankNameKana + "%");
                List<Dictionary<string, string>> ret = WF_AppForm.GetEbsDataWithApi("Get_Financial_Corp_List", dic);

                // フロントに戻ること
                return JsonConvert.SerializeObject(ret);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 支店一覧取得用メソッド
        /// </summary>
        /// <returns></returns>
        public string GetBranchList()
        {
            try
            {
                // 検索条件の取得
                BranchListReq cond =
                    JsonConvert.DeserializeObject<BranchListReq>(
                        this.GetRequestVal("BranchListReq"));

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("BankCode", cond.BankCode);
                dic.Add("BranchNameKana", cond.BranchNameKana + "%");
                List<Dictionary<string, string>> ret = WF_AppForm.GetEbsDataWithApi("Get_BranchOfBank_By_Kana", dic);

                // フロントに戻ること
                return JsonConvert.SerializeObject(ret);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// PDFの出力処理
        /// </summary>
        public string Print()
        {
            Dictionary<string, List<string>> dicTbl = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(this.GetRequestVal("PrintData"));

            PDFlib p = null;
            StringBuilder errs = new StringBuilder();
            try
            {
                //本人情報変更届（貼り付け台紙）PDFの初期化処理
                p = new PDFlib();
                //各パスの設定
                p.set_option("SearchPath={" + font_dir + "}");
                p.set_option("SearchPath={" + template_dir + "}");
                //フォントの設定
                p.set_option("FontOutline={IPAGP=ipagp.ttf}");
                p.set_option("FontOutline={IPAEXG=ipaexg.ttf}");
                //PDF各情報の設定
                p.set_info("Creator", "AEON");
                p.set_info("Author", "AEON");
                p.set_info("Title", "本人情報変更届（貼り付け台紙）");
                //出力PDFを生成
                int output = p.begin_document("", "");
                if (output == -1)
                {
                    errs.AppendLine("err@Error:").Append(p.get_errmsg());
                    // Console.WriteLine("Error: {0}\n", p.get_errmsg());
                }
                //PDF座標(原点)を設定
                p.begin_page_ext(0, 0, "");
                //PDFテンプレートを読み込む
                int pdi = p.open_pdi_document(import_pdf, "");
                if (pdi == -1)
                {
                    errs.AppendLine("err@Error:").Append(p.get_errmsg());
                    //Console.WriteLine("Error: {0}\n", p.get_errmsg());
                }
                //PDFテンプレートのプリンターページを決める
                int pdi_page = p.open_pdi_page(pdi, 1, "");
                if (pdi_page == -1)
                {
                    errs.AppendLine("err@Error:").Append(p.get_errmsg());
                    //Console.WriteLine("Error: {0}\n", p.get_errmsg());
                }
                // ページを生成
                p.fit_pdi_page(pdi_page, 0, 0, "adjustpage");
                // 各項目の値を設定
                errs.Append(SetPDFData(p, dicTbl));
                // インポートしたページと PDF を閉じる
                p.close_pdi_page(pdi_page);
                p.close_pdi_document(pdi);
                // ページを閉じる
                p.end_page_ext("");
                // 文書を閉じる
                p.end_document("");
                // メールを発送
                byte[] bt = p.get_buffer();
                return Convert.ToBase64String(bt);
            }
            catch (PDFlibException e)
            {
                // PDFlibが投げた例外をキャッチした
                errs.AppendLine("PDFlib例外がhelloサンプル内で発生しました:\n");
                errs.AppendLine("[" + e.get_errnum() + "] " + e.get_apiname() + ": " + e.get_errmsg() + "\n");
                //Console.WriteLine("PDFlib例外がhelloサンプル内で発生しました:\n");
                //Console.WriteLine("[{0}] {1}: {2}\n", e.get_errnum(), e.get_apiname(), e.get_errmsg());
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            finally
            {
                if (p != null)
                {
                    p.Dispose();
                }
            }

            return "err@" + errs.ToString();
        }

        /// <summary>
        /// 値の設定処理
        /// </summary>
        /// <param name="p">PDFlib</param>
        /// <param name="dicTbl">dicTbl</param>
        private static string SetPDFData(PDFlib p, Dictionary<string, List<string>> dicTbl)
        //private static string SetPDFData(PDFlib p)
        {
            StringBuilder errs = new StringBuilder();
            //フォントゴシックを読み込む
            int ipaexg = p.load_font("IPAEXG", "unicode", "embedding");
            if (ipaexg == -1)
            {
                errs.AppendLine("err@Error:").Append(p.get_errmsg());
                //Console.WriteLine("Error: {0}\n", p.get_errmsg());
            }

            //フォントゴシックを読み込む
            int ipaexgBold = p.load_font("IPAEXG", "unicode", "embedding fontstyle=bold");
            if (ipaexgBold == -1)
            {
                errs.AppendLine("err@Error:").Append(p.get_errmsg());
                //Console.WriteLine("Error: {0}\n", p.get_errmsg());
            }

            //フォントPゴシックを読み込む
            int ipagp = p.load_font("IPAGP", "unicode", "embedding");
            if (ipagp == -1)
            {
                errs.AppendLine("err@Error:").Append(p.get_errmsg());
                //Console.WriteLine("Error: {0}\n", p.get_errmsg());
            }

            foreach (KeyValuePair<string, List<string>> kvp in dicTbl)
            {
                //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                if (kvp.Key == "WORK_ID") // 伝票番号
                {
                    //p.fit_textline("weight:", 414, 720, "");
                    p.fit_textline(kvp.Value[0], 414, 720, "font=" + ipaexg + " fontsize=9");
                }
                else if (kvp.Key == "PRINT_REASON_CODE") // 変更理由
                {
                    p.fit_textline(kvp.Value[0], 184, 705, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_IDOU_DAY") // 異動日
                {
                    p.fit_textline(kvp.Value[0], 414, 705, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_COMPANY_CODE") // 会社コード
                {
                    p.fit_textline(kvp.Value[0], 184, 690, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_COMPANY_NAME") // 会社名
                {
                    p.fit_textline(kvp.Value[0], 414, 690, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOZOKU_CODE") // 人事所属コード
                {
                    p.fit_textline(kvp.Value[0], 184, 675, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOZOKU_NAME") // 所属名
                {
                    p.fit_textline(kvp.Value[0], 414, 675, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SHAINBANGO") // 社員番号
                {
                    p.fit_textline(kvp.Value[0], 184, 660, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_LAST_NAME_KANJI") // 姓・漢字
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 184, 645, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 170, 645, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else {
                        p.fit_textline(kvp.Value[0], 184, 645, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_FIRST_NAME_KANJI") // 名・漢字
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 414, 645, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 400, 645, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 414, 645, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_LAST_NAME_KANA") // 姓・カナ
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 184, 630, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 170, 630, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 184, 630, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_FIRST_NAME_KANA") // 名・カナ
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 414, 630, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 400, 630, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 414, 630, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_BIRTHDAY") // 生年月日
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 184, 615, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 170, 615, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 184, 615, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_GENDER") // 性別
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 414, 615, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 400, 615, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 414, 615, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_MARRIGE_KBN_NAME") // 婚姻区分
                {
                    p.fit_textline(kvp.Value[0], 184, 598, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_KOKUSEKI") // 国籍
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 414, 598, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 400, 598, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 414, 598, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_SINNSEI_DAY") // 申請日
                {
                    p.fit_textline(kvp.Value[0], 221, 575, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_ZEIHYOU_EDIT_KBN") // 税表区分の変更
                {
                    p.fit_textline(kvp.Value[0], 221, 560, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_ZEIHYOU_KBN_BEFORE") // 現在税表区分
                {
                    p.fit_textline(kvp.Value[0], 451, 560, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "ZEIHYOU_KBN_P") // 税表区分
                {
                    p.fit_textline(kvp.Value[0], 221, 545, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_ZEIHYOU_KBN_AFTER") // 変更後税表区分
                {
                    if (ZENKAKU_HYPHEN.Equals(kvp.Value[0]))
                    {
                        
                    }
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 451, 545, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 439, 545, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 451, 545, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_ZEIHYOU_EDIT_REASON") // 税表区分変更理由
                {
                    p.fit_textline(kvp.Value[0], 221, 530, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "KAFU_KBN_P") // 寡夫区分
                {
                    p.fit_textline(kvp.Value[0], 221, 507, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_HITORIOYA_KBN_BEFORE") // 寡夫区分
                {
                    p.fit_textline(kvp.Value[0], 451, 507, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SINSEISYA_GENDER") // ①申請者性別
                {
                    p.fit_textline(kvp.Value[0], 221, 492, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_HITORIOYA_CHECK_KBN") // 寡夫区分
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 451, 492, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 439, 492, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 451, 492, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_SINSEISYA_MARRIAGE_KBN") // ②申請者婚姻区分
                {
                    p.fit_textline(kvp.Value[0], 221, 477, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_MARRIAGE_IN_FACT_SELECT") // ③事実婚状態ではないですか。
                {
                    p.fit_textline(kvp.Value[0], 451, 477, "font=" + ipaexg + " fontsize=8 ");
                }
                else if (kvp.Key == "PRINT_KAFU_YEAR_SYOTOKU_SELECT") // ④従業員自身の年間所得が500万円以下ですか。
                {
                    if (kvp.Value[0].Length <= 5) {
                        p.fit_textline(kvp.Value[0], 221, 460, "font=" + ipaexg + " fontsize=9 ");
                    } else {
                        p.fit_textline(kvp.Value[0].Substring(0,5), 221, 460, "font=" + ipaexg + " fontsize=9 ");
                        p.fit_textline(kvp.Value[0].Substring(5), 221, 448, "font=" + ipagp + " fontsize=9");
                    }
                }
                else if (kvp.Key == "PRINT_RIKON_SELECT") // ⑤離婚歴がありますか。
                {
                    p.fit_textline(kvp.Value[0], 451, 460, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "GAKUSEI_KBN_P") // 勤労学生区分
                {
                    p.fit_textline(kvp.Value[0], 221, 423, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_GAKUSEI_KBN_BEFORE") // 勤労学生区分
                {
                    p.fit_textline(kvp.Value[0], 451, 423, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_GAITOU_GAKUSEI_SELECT") // ①従業員自身の所定の学校の学生・生徒に該当しますか。
                {
                    p.fit_textline(kvp.Value[0], 221, 401, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_GAKUSEI_CHECK_KBN") // 勤労学生区分
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 451, 401, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 439, 401, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 451, 401, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_GAKUSEI_YEAR_SYOTOKU_SELECT") // ②本人の年間所得金額が48万円以上75万円以下
                {
                    p.fit_textline(kvp.Value[0], 375, 371, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_GAKUSEI_OTHER_SYOTOKU_SELECT") // ③本人の給与所得（アルバイト代）以外の所得が、10万円以下である。
                {
                    p.fit_textline(kvp.Value[0], 375, 344, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_EDIT_KBN") // 障害者区分の変更
                {
                    p.fit_textline(kvp.Value[0], 221, 322, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_KBN_BEFORE") // 本人障害者区分
                {
                    p.fit_textline(kvp.Value[0], 451, 322, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "SYOUGAI_KBN_P") // 障害者区分
                {
                    p.fit_textline(kvp.Value[0], 221, 308, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_KBN_AFTER") // 本人障害者区分
                {
                    if (kvp.Value[1] == FUTOJI_FLG)
                    {
                        p.fit_textline(kvp.Value[0], 451, 308, "font=" + ipaexgBold + " fontsize=9 ");
                        p.fit_textline(HOSI_MARK, 439, 308, "font=" + ipaexg + " fontsize=9 ");
                    }
                    else
                    {
                        p.fit_textline(kvp.Value[0], 451, 308, "font=" + ipaexg + " fontsize=9 ");
                    }
                }
                else if (kvp.Key == "PRINT_SYOUGAI_KAKUNIN_SELECT") // ①以下のいずれかに該当しますか
                {
                    p.fit_textline(kvp.Value[0], 221, 292, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_TEITYOU_NO") // ②手帳番号
                {
                    p.fit_textline(kvp.Value[0], 377, 292, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_TEITYOU_DAY") // ③手帳交付日
                {
                    p.fit_textline(kvp.Value[0], 146, 277, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_NAIYO_SELECT") // ④障害内容区分
                {
                    p.fit_textline(kvp.Value[0], 377, 277, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_TOKYU_SELECT") // ⑤障害等級
                {
                    p.fit_textline(kvp.Value[0], 146, 262, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_TEIDO_SELECT") // ⑥障害程度
                {
                    p.fit_textline(kvp.Value[0], 377, 262, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_SYOUGAI_NAIYO") // ⑦障害内容
                {
                    if (kvp.Value[0].Length <= 54) {
                        p.fit_textline(kvp.Value[0], 146, 247, "font=" + ipaexg + " fontsize=7 ");
                    }
                    else if (kvp.Value[0].Length <= 108)
                    {
                        p.fit_textline(kvp.Value[0].Substring(0, 54), 146, 247, "font=" + ipaexg + " fontsize=7 ");
                        p.fit_textline(kvp.Value[0].Substring(54), 146, 240, "font=" + ipaexg + " fontsize=7");
                    } else if (kvp.Value[0].Length <= 162)
                    {
                        p.fit_textline(kvp.Value[0].Substring(0, 54), 146, 247, "font=" + ipaexg + " fontsize=7 ");
                        p.fit_textline(kvp.Value[0].Substring(54, 54), 146, 240, "font=" + ipaexg + " fontsize=7");
                        p.fit_textline(kvp.Value[0].Substring(108), 146, 233, "font=" + ipaexg + " fontsize=7");
                    } else if (kvp.Value[0].Length <= 216)
                    {
                        p.fit_textline(kvp.Value[0].Substring(0, 54), 146, 247, "font=" + ipaexg + " fontsize=7 ");
                        p.fit_textline(kvp.Value[0].Substring(54, 54), 146, 240, "font=" + ipaexg + " fontsize=7");
                        p.fit_textline(kvp.Value[0].Substring(108, 54), 146, 233, "font=" + ipaexg + " fontsize=7");
                        p.fit_textline(kvp.Value[0].Substring(162), 146, 226, "font=" + ipaexg + " fontsize=7");
                    } else
                    {
                        p.fit_textline(kvp.Value[0].Substring(0, 54), 146, 247, "font=" + ipaexg + " fontsize=7 ");
                        p.fit_textline(kvp.Value[0].Substring(54, 54), 146, 240, "font=" + ipaexg + " fontsize=7");
                        p.fit_textline(kvp.Value[0].Substring(108, 54), 146, 233, "font=" + ipaexg + " fontsize=7");
                        p.fit_textline(kvp.Value[0].Substring(162, 54), 146, 226, "font=" + ipaexg + " fontsize=7");
                        p.fit_textline(kvp.Value[0].Substring(216), 146, 219, "font=" + ipaexg + " fontsize=7");
                    }
                }
                else if (kvp.Key == "PRINT_IMG_UPLOAD_1") // アップロードファイル１
                {
                    p.fit_textline(kvp.Value[0], 184, 202, "font=" + ipaexg + " fontsize=9 ");
                }
                else if (kvp.Key == "PRINT_IMG_UPLOAD_2") // アップロードファイル２
                {
                    p.fit_textline(kvp.Value[0], 414, 202, "font=" + ipaexg + " fontsize=9 ");
                }
            }

            return errs.ToString();
        }
        
        /// <summary>
        /// 金融機関一覧検索条件クラス
        /// </summary>
        private class BankListReq
        {
            /// <summary>
            /// 金融機関コード
            /// </summary>
            public string BankCode { get; set; }

            /// <summary>
            /// 金融機関名称・カナ
            /// </summary>
            public string BankNameKana { get; set; }
        }

        /// <summary>
        /// 支店一覧検索条件クラス
        /// </summary>
        private class BranchListReq
        {
            /// <summary>
            /// 金融機関コード
            /// </summary>
            public string BankCode { get; set; }

            /// <summary>
            /// 支店名称・カナ
            /// </summary>
            public string BranchNameKana { get; set; }
        }
    }
}
