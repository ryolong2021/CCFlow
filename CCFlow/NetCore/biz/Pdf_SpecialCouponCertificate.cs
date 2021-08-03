using BP.Sys;
using PDFlib_dotnet;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BP.WF.HttpHandler
{
    public class Pdf_SpecialCouponCertificate
    {
        const string date_format = "yyyy年MM月dd日";
        const string font_dir = "wwwroot/resource/fonts";
        const string barcodeImage_dir = "wwwroot/resource/barcodes";
        const string template_dir = "wwwroot/resource/templates";
        const string import_pdf = "特別買物割引証.pdf";

        /// <summary>
        /// PDFの出力処理
        /// </summary>
        /// <param name="p">PDFlib</param>
        /// <param name="tblName">tblName</param>
        /// <param name="dicTbl">dicTbl</param>
        public static string Print(string tblName, Dictionary<string, string> dicTbl)
        {
            PDFlib p = null;
            StringBuilder errs = new StringBuilder();
            try
            {
                //特別買物割引証PDFの初期化処理
                p = new PDFlib();
                //各パスの設定
                p.set_option("SearchPath={" + font_dir + "}");
                p.set_option("SearchPath={" + barcodeImage_dir + "}");
                p.set_option("SearchPath={" + template_dir + "}");
                //フォントの設定
                p.set_option("FontOutline={IPAGP=ipagp.ttf}");
                p.set_option("FontOutline={IPAEXG=ipaexg.ttf}");
                //PDF各情報の設定
                p.set_info("Creator", "AEON");
                p.set_info("Author", "AEON");
                p.set_info("Title", "特別買物割引証");
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
                errs.Append(SetPDFData(p, tblName, dicTbl));
                // インポートしたページと PDF を閉じる
                p.close_pdi_page(pdi_page);
                p.close_pdi_document(pdi);
                // ページを閉じる
                p.end_page_ext("");
                // 文書を閉じる
                p.end_document("");
                // メールを発送
                SendMail(p, dicTbl);
            }
            catch (PDFlibException e)
            {
                // PDFlibが投げた例外をキャッチした
                errs.AppendLine("PDFlib例外がhelloサンプル内で発生しました:\n");
                errs.AppendLine("["+ e.get_errnum() + "] "+ e.get_apiname() + ": "+ e.get_errmsg() + "\n");
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

            return errs.ToString();
        }

        /// <summary>
        /// 値の設定処理
        /// </summary>
        /// <param name="p">PDFlib</param>
        /// <param name="tblName">tblName</param>
        /// <param name="dicTbl">dicTbl</param>
        private static string SetPDFData(PDFlib p, string tblName, Dictionary<string, string> dicTbl)
        {
            StringBuilder errs = new StringBuilder();
            //フォントゴシックを読み込む
            int ipaexg = p.load_font("IPAEXG", "unicode", "embedding");
            if (ipaexg == -1)
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

            foreach (KeyValuePair<string, string> kvp in dicTbl)
            {
                //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                if (kvp.Key == "SPEC_DISCOUNT_COUPON_NO") // 特別買物割引証番号
                {
                    p.fit_textline(kvp.Value, 140, 623, "font=" + ipaexg + " fontsize=12 ");

                    int barcode = p.load_image("auto", kvp.Value + ".png", "");
                    if (barcode == -1)
                    {
                        errs.AppendLine("err@Error:").Append(p.get_errmsg());
                        //Console.WriteLine("Error: {0}\n", p.get_errmsg());
                    }
                    p.fit_image(barcode, 240, 746, "boxsize={120 40} position=center fitmethod=auto");
                    p.close_image(barcode);
                    string[] pngList = Directory.GetFiles(barcodeImage_dir, kvp.Value + ".png");
                    foreach (string f in pngList)
                    {
                        File.Delete(f);
                    }
                }
                else if (kvp.Key == "USE_START_DATE") // 有効期間From
                {
                    p.fit_textline(DateTime.Parse(kvp.Value).ToString(date_format), 210, 730, "font=" + ipaexg + " fontsize=18 ");
                }
                else if (kvp.Key == "USE_END_DATE") // 有効期間To
                {
                    p.fit_textline(DateTime.Parse(kvp.Value).ToString(date_format), 375, 730, "font=" + ipaexg + " fontsize=18 ");
                }
                else if (kvp.Key == "CORP_NAME") // 会社名
                {
                    p.fit_textline(kvp.Value, 210, 700, "font=" + ipaexg + " fontsize=18 ");
                }
                else if (kvp.Key == "DEPART_NAME") // 所属
                {
                    p.fit_textline(kvp.Value, 210, 600, "font=" + ipaexg + " fontsize=16 ");
                }
                else if (kvp.Key == "EMPLOYEE_NO") // 社員番号
                {
                    p.fit_textline(kvp.Value, 210, 573, "font=" + ipaexg + " fontsize=16 ");
                }
                else if (kvp.Key == "EMPLOYEE_KANZI_NAME") // 氏名
                {
                    p.fit_textline(kvp.Value, 210, 546, "font=" + ipaexg + " fontsize=16 ");
                }
                else if (kvp.Key == "ROLE_PERSON_NAME") // 人事部長
                {
                    p.fit_textline(kvp.Value, 210, 654, "font=" + ipaexg + " fontsize=18 ");
                }
                else if (kvp.Key == "BUY_STORE_WARNING") // 購買店舗注意文
                {
                    p.fit_textline(kvp.Value.Substring(0, 25), 210, 522, "font=" + ipagp + " fontsize=16 fillcolor=red");
                    if (kvp.Value.Length > 25) 
                    {
                        p.fit_textline(kvp.Value.Substring(25), 210, 504, "font=" + ipagp + " fontsize=16 fillcolor=red");
                    }
                }
            }

            return errs.ToString();
        }

        /// <summary>
        /// メール発信処理
        /// </summary>
        /// <param name="p">PDFlib</param>
        private static void SendMail(PDFlib p, Dictionary<string, string> dicTbl)
        {
            byte[] bt = p.get_buffer();
            string b64 = Convert.ToBase64String(bt);
            // 認証キー
            var apiKey = SystemConfig.AppSettings["SendGridKey"];
            var client = new SendGridClient(apiKey);
            // 差出人
            var from = new EmailAddress(dicTbl["MAIL_FROM"], "");
            // 件名
            var subject = "件名未定";
            var content = "メールテンプレート未定";
            List<EmailAddress> toList = new List<EmailAddress>();
            // 宛先
            toList.Add(new EmailAddress(dicTbl["MAIL_TO"], "")); // "ou-chir@aeonpeople.biz" dicTbl["MAIL_TO"]
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toList, subject, content, content);
            msg.AddAttachment("買物特別割引証_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf", b64);
            client.SendEmailAsync(msg);
        }
    }
}
