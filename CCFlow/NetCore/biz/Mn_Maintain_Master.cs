using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;

namespace BP.WF.HttpHandler
{
    public class Mn_Maintain_Master : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// マスタテーブル一覧データの取得
        /// </summary>
        /// <returns>取得したマスタテーブルリスト</returns>
        public string GetMasterTableList()
        {
            try
            {
                // テンプレート取得用sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // 項目
                sqlSb.Append("select MASTER_ID,");
                sqlSb.Append("       MASTER_NAME,");
                sqlSb.Append("       MASTER_TBL_NAME,");
                sqlSb.Append("       APPLY_DATE_FLAG,");
                sqlSb.Append("       DATA_DIMENSION,");
                sqlSb.Append("       COLUMN_LIST,");
                sqlSb.Append("       CLASS_NAME,");
                sqlSb.Append("       RPT_PREFIX");

                // メンテナンス共通化管理マスタ
                sqlSb.Append(" from MT_MN_MASTER");

                // データ取得
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString());

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// CSVファイル内容の取得
        /// </summary>
        /// <returns>取得したマスタテーブルリスト</returns>
        public string GetCsvDataInfo()
        {
            try
            {
                // 検索条件の取得
                Dictionary<object, object> masterdata = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("masterdata"));
                
                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // メンテナンス共通化管理マスタ
                sqlSb.Append("select * from " + masterdata["MASTER_TBL_NAME"]);

                // 「適用開始日」が指定された場合
                if (!string.IsNullOrEmpty(this.GetRequestVal("dependent_start_ymd")))
                {
                    sqlSb.Append(" WHERE TEKIYOYMD_FROM <= CONVERT(VARCHAR, " + this.GetRequestVal("dependent_start_ymd") +  ", 112)");
                    sqlSb.Append("          AND TEKIYOYMD_TO >= CONVERT(VARCHAR, " + this.GetRequestVal("dependent_start_ymd") + ", 112)");
                    sqlSb.Append("          AND (DELETE_FLG IS NULL OR DELETE_FLG <> 'X')");
                }

                // データ取得
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlSb.ToString());

                // CSV対象の作成
                StringBuilder csvbuilder = new StringBuilder();

                // タイトルの追加
                foreach (DataColumn title in dt.Columns)
                {
                    csvbuilder.Append(string.Format("\"{0}\"", title.ColumnName));
                    csvbuilder.Append(",");
                }
                csvbuilder.Remove(csvbuilder.ToString().Length - 1, 1);

                // データ部の追加
                foreach (DataRow row in dt.Rows)
                {

                    // 改行の追加
                    csvbuilder.Append(Environment.NewLine);

                    // 各コラムの追加
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        // コンマの追加
                        if (i > 0)
                        {
                            csvbuilder.Append(",");
                        }
                        csvbuilder.Append(string.Format("\"{0}\"", row[i].ToString()));
                    }
                }

                // フロントに戻ること
                return csvbuilder.ToString();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// マスタテーブル更新用メソッド
        /// </summary>
        /// <returns>更新結果</returns>
        public string UpdateMasterWithExcel()
        {

            // アップロードファイル
            IFormFile iff = null;
            string excelPath = null;

            try
            {
                // 検索条件の取得
                Dictionary<object, object> masterdata = JsonConvert.DeserializeObject<Dictionary<object, object>>(this.GetRequestVal("masterdata"));

                // EXCELの取得
                iff = BP.Web.HttpContextHelper.RequestFiles(0);

                // サーバーに一時保存
                excelPath = FileUpload(iff);

                //文件を読み込む
                DataTable dtExcel = BP.DA.DBLoad.ReadExcelFileToDataTable(excelPath);

                // 「適用開始日」が指定された場合
                if (masterdata["APPLY_DATE_FLAG"].ToString() == "Y")
                {

                    // 適用開始日以後のデータを削除する
                    // sql文対象の作成
                    StringBuilder sqlSb = new StringBuilder();

                    // メンテナンス共通化管理マスタ
                    sqlSb.Append("delete from " + masterdata["MASTER_TBL_NAME"]);

                    sqlSb.Append(" WHERE TEKIYOYMD_FROM >= CONVERT(VARCHAR, " + this.GetRequestVal("dependent_start_ymd") + ", 112)");

                    // 削除実行
                    BP.DA.DBAccess.RunSQL(sqlSb.ToString());

                    // sql文対象初期化
                    sqlSb.Clear();

                    // メンテナンス共通化管理マスタ
                    sqlSb.Append("update " + masterdata["MASTER_TBL_NAME"] + " set");
                    sqlSb.Append(" TEKIYOYMD_TO = CONVERT(VARCHAR, " + this.GetRequestVal("dependent_start_ymd") + "-1, 112)");

                    sqlSb.Append(" WHERE TEKIYOYMD_FROM < CONVERT(VARCHAR, " + this.GetRequestVal("dependent_start_ymd") + ", 112)");
                    sqlSb.Append("          AND TEKIYOYMD_TO >= CONVERT(VARCHAR, " + this.GetRequestVal("dependent_start_ymd") + ", 112)");

                    // 更新実行
                    BP.DA.DBAccess.RunSQL(sqlSb.ToString());

                    // 新しいデータ登録
                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                    {
                        // sql文対象初期化
                        sqlSb.Clear();
                        sqlSb.Append("insert into " + masterdata["MASTER_TBL_NAME"] + " values(");

                        for (int j = 0; j < dtExcel.Columns.Count; j++) 
                        {

                            // SEQNOの場合
                            if (dtExcel.Columns[j].ColumnName == "SEQNO")
                            {
                                continue;
                            }

                            // 適用開始日
                            if (dtExcel.Columns[j].ColumnName == "TEKIYOYMD_FROM")
                            {
                                sqlSb.Append(string.Format("'{0}'", this.GetRequestVal("dependent_start_ymd")) + ",");
                            }

                            // 適用終了日
                            else if (dtExcel.Columns[j].ColumnName == "TEKIYOYMD_TO")
                            {
                                sqlSb.Append(string.Format("'{0}'", "99999999") + ",");
                            }

                            // 登録日時
                            else if (dtExcel.Columns[j].ColumnName == "REC_ENT_DATE")
                            {
                                sqlSb.Append(string.Format("'{0}'", DateTime.Now.ToString()) + ",");
                            }

                            // 登録日時
                            else if (dtExcel.Columns[j].ColumnName == "REC_ENT_USER")
                            {
                                sqlSb.Append(string.Format("'{0}'", BP.Web.WebUser.No) + ",");
                            }

                            // 最終更新日時
                            else if (dtExcel.Columns[j].ColumnName == "REC_EDT_DATE")
                            {
                                sqlSb.Append(string.Format("'{0}'", DateTime.Now.ToString()) + ",");
                            }

                            // 最終更新者
                            else if (dtExcel.Columns[j].ColumnName == "REC_EDT_USER")
                            {
                                sqlSb.Append(string.Format("'{0}'", BP.Web.WebUser.No) + ",");
                            }

                            // 上記以外の場合
                            else
                            {
                                sqlSb.Append(string.Format("'{0}'", dtExcel.Rows[i][j].ToString()) + ",");
                            }
                        }

                        sqlSb.Remove(sqlSb.Length - 1, 1);
                        sqlSb.Append(")");

                        // 登録実行
                        BP.DA.DBAccess.RunSQL(sqlSb.ToString());
                    }
                }

                // 「適用開始日」が指定されない場合
                else
                {

                    // データを削除する
                    // sql文対象の作成
                    StringBuilder sqlSb = new StringBuilder();

                    // メンテナンス共通化管理マスタ
                    sqlSb.Append("delete from " + masterdata["MASTER_TBL_NAME"]);

                    // 削除実行
                    BP.DA.DBAccess.RunSQL(sqlSb.ToString());

                    // 新しいデータ登録
                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                    {
                        // sql文対象初期化
                        sqlSb.Clear();
                        sqlSb.Append("insert into " + masterdata["MASTER_TBL_NAME"] + " values(");

                        for (int j = 0; j < dtExcel.Columns.Count; j++)
                        {
                            // SEQNOの場合
                            if (dtExcel.Columns[j].ColumnName == "SEQNO")
                            {
                                continue;
                            }

                            // 登録日時
                            if (dtExcel.Columns[j].ColumnName == "REC_ENT_DATE")
                            {
                                sqlSb.Append(string.Format("'{0}'", DateTime.Now.ToString()) + ",");
                            }

                            // 登録日時
                            else if (dtExcel.Columns[j].ColumnName == "REC_ENT_USER")
                            {
                                sqlSb.Append(string.Format("'{0}'", BP.Web.WebUser.No) + ",");
                            }

                            // 最終更新日時
                            else if (dtExcel.Columns[j].ColumnName == "REC_EDT_DATE")
                            {
                                sqlSb.Append(string.Format("'{0}'", DateTime.Now.ToString()) + ",");
                            }

                            // 最終更新者
                            else if (dtExcel.Columns[j].ColumnName == "REC_EDT_USER")
                            {
                                sqlSb.Append(string.Format("'{0}'", BP.Web.WebUser.No) + ",");
                            }

                            // 上記以外の場合
                            else
                            {
                                sqlSb.Append(string.Format("'{0}'", dtExcel.Rows[i][j].ToString()) + ",");
                            }
                        }

                        sqlSb.Remove(sqlSb.Length - 1, 1);
                        sqlSb.Append(")");

                        // 登録実行
                        BP.DA.DBAccess.RunSQL(sqlSb.ToString());
                    }
                }

                // フロントに戻ること
                return masterdata["MASTER_NAME"] + "マスタテーブル「" + masterdata["MASTER_TBL_NAME"] + "」が正常に更新されました。";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            finally
            {
                // ファイル削除
                if (File.Exists(excelPath))
                {
                    File.Delete(excelPath);
                }
            }
        }

        /// <summary>
        /// アップデート処理
        /// </summary>
        /// <param name="iff">ファイル</param>
        /// <returns></returns>
        private string FileUpload(IFormFile iff)
        {
            string fileSave = string.Empty;
            FileStream fs = null;
            BinaryWriter bw = null;

            try
            {
                // ファイル保存フォルダ
                string uploadPath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                                + "wwwroot" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar
                                + "import" + Path.DirectorySeparatorChar;

                // フォルダの存在チェック
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // 保存情報
                fileSave = uploadPath + iff.FileName;

                // ファイル削除
                if (File.Exists(fileSave))
                {
                    File.Delete(fileSave);
                }

                // ファイル形式変換
                Stream stream = iff.OpenReadStream();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                // ファイル出力
                fs = new FileStream(fileSave, FileMode.Create);
                bw = new BinaryWriter(fs);
                bw.Write(bytes);

                // 閉じる
                bw.Close();
                fs.Close();
            }
            catch (Exception)
            {
                if (bw != null)
                {
                    bw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return fileSave;
        }
    }
}
