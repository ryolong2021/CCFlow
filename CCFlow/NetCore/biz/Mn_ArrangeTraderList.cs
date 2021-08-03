using BP.DA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using Common.WF_OutLog;

/// <summary>
/// 手配業者依頼一覧画面
/// </summary>
namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 手配業者依頼一覧画面
    /// </summary>
    public class Mn_ArrangeTraderList : BP.WF.HttpHandler.DirectoryPageBase
    {
        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();
        const int TEHAI_KBN_ZUMI = 1;
        const int TEHAI_KBN_FUNOU = 2;
        const int TEHAI_KBN_CONFIRMING = 3;
        // キャンセル
        const int TEHAI_KBN_CANCEL = 4;
        // キャンセル（有償）
        const int TEHAI_KBN_CANCEL_PAID = 5;

        /// <summary>
        /// 手配状態変更処理
        /// </summary>
        /// <returns></returns>
        public string Tehai_States_Update()
        {
            try
            {
                // パラメータの作成
                Paras ps = new Paras();

                // sql文対象の作成
                StringBuilder sqlSb = new StringBuilder();

                // sql文の作成
                sqlSb.Append("Update TT_WF_CONDOLENCE");
                sqlSb.Append(" Set TEHAIKBN = @tehaiKbn");
                ps.Add("tehaiKbn", int.Parse(this.GetRequestVal("tehaiKbn")));
                if (this.GetRequestVal("tehaiKbn") == TEHAI_KBN_FUNOU.ToString())
                {
                    // 手配状態は「手配不能」の場合、供花と弔電を「1:辞退」に設定すること
                    // 供花
                    sqlSb.Append(" ,KYOKAKBN = 1");
                    // 弔電
                    sqlSb.Append(" ,TYODENKBN = 1");

                    // 供花届ける場所区分
                    sqlSb.Append(" ,TODOKESAKIKBN = null");

                    // 後飾り名前
                    sqlSb.Append(" ,ATOKAZARI_FULLNAME = null");

                    // 後飾り郵便番号
                    sqlSb.Append(" ,ATOKAZARI_YUBINBANGO = null");

                    // 後飾り都道府県・市郡区
                    sqlSb.Append(" ,ATOKAZARI_ADDRESS1 = null");

                    // 後飾り町村番地
                    sqlSb.Append(" ,ATOKAZARI_ADDRESS2 = null");

                    // 後飾りマンション名
                    sqlSb.Append(" ,ATOKAZARI_ADDRESS3 = null");

                    // 後飾り連絡先電話番号
                    sqlSb.Append(" ,ATOKAZARI_RENRAKUSAKITEL = null");

                    // 後飾り日付
                    sqlSb.Append(" ,ATOKAZARI_DATE = null");

                    sqlSb.Append(" ,TEHAIFUNOU_COMMENT = @comment");
                    sqlSb.Append(" ,TEHAIFUNOU_DATETIME = @datetime");
                    sqlSb.Append(" ,TEHAISYA_NO = @user");
                    sqlSb.Append(" ,REC_EDT_DATE = @datetime");
                    sqlSb.Append(" ,REC_EDT_USER = @user");

                    ps.Add("comment", this.GetRequestVal("tehaiFunouComment"));
                    ps.Add("datetime", DateTime.Now.ToString());
                    ps.Add("user", this.GetRequestVal("tehaisyaNo"));
                }
                else if (this.GetRequestVal("tehaiKbn") == TEHAI_KBN_ZUMI.ToString())
                {

                    sqlSb.Append(" ,COMP_DATETIME = @datetime");
                    sqlSb.Append(" ,CMP_EMP_NO = @user");
                    sqlSb.Append(" ,REC_EDT_DATE = @datetime");
                    sqlSb.Append(" ,REC_EDT_USER = @user");

                    ps.Add("datetime", DateTime.Now.ToString());
                    ps.Add("user", this.GetRequestVal("tehaisyaNo"));
                }
                else if (this.GetRequestVal("tehaiKbn") == TEHAI_KBN_CONFIRMING.ToString())
                {

                    sqlSb.Append(" ,CHECK_DATETIME = @datetime");
                    sqlSb.Append(" ,CHECK_EMP_NO = @user");
                    sqlSb.Append(" ,REC_EDT_DATE = @datetime");
                    sqlSb.Append(" ,REC_EDT_USER = @user");

                    ps.Add("datetime", DateTime.Now.ToString());
                    ps.Add("user", this.GetRequestVal("tehaisyaNo"));
                }
                // キャンセル、キャンセル（有償）
                else if (this.GetRequestVal("tehaiKbn") == TEHAI_KBN_CANCEL.ToString() || this.GetRequestVal("tehaiKbn") == TEHAI_KBN_CANCEL_PAID.ToString())
                {
                    // 手配状態は「手配不能」の場合、供花と弔電を「1:辞退」に設定すること
                    // 供花
                    sqlSb.Append(" ,KYOKAKBN = 1");
                    // 弔電
                    sqlSb.Append(" ,TYODENKBN = 1");

                    // 供花届ける場所区分
                    sqlSb.Append(" ,TODOKESAKIKBN = null");

                    // 後飾り名前
                    sqlSb.Append(" ,ATOKAZARI_FULLNAME = null");

                    // 後飾り郵便番号
                    sqlSb.Append(" ,ATOKAZARI_YUBINBANGO = null");

                    // 後飾り都道府県・市郡区
                    sqlSb.Append(" ,ATOKAZARI_ADDRESS1 = null");

                    // 後飾り町村番地
                    sqlSb.Append(" ,ATOKAZARI_ADDRESS2 = null");

                    // 後飾りマンション名
                    sqlSb.Append(" ,ATOKAZARI_ADDRESS3 = null");

                    // 後飾り連絡先電話番号
                    sqlSb.Append(" ,ATOKAZARI_RENRAKUSAKITEL = null");

                    // 後飾り日付
                    sqlSb.Append(" ,ATOKAZARI_DATE = null");

                    sqlSb.Append(" ,CANCEL_DATETIME = @datetime");
                    sqlSb.Append(" ,CANCEL_EMP_NO = @user");
                    sqlSb.Append(" ,REC_EDT_DATE = @datetime");
                    sqlSb.Append(" ,REC_EDT_USER = @user");

                    ps.Add("datetime", DateTime.Now.ToString());
                    ps.Add("user", this.GetRequestVal("tehaisyaNo"));
                }

                sqlSb.Append(" Where OID = @oid");
                sqlSb.Append(" and TEHAIKBN = @firsttehaiKbn");

                ps.Add("firsttehaiKbn", this.GetRequestVal("firsttehaiKbn"));
                
                string strOid = this.GetRequestVal("strOid");
                ps.Add("oid", strOid);

                //伝票番号を取得
                // string fkFlow = this.GetRequestVal("FK_Flow");

                //手配状態を取得
                // int tehaiKbn = int.Parse(this.GetRequestVal("tehaiKbn"));

                int result = BP.DA.DBAccess.RunSQL(sqlSb.ToString(), ps);

                //BP.WF.SendReturnObjs objs = new BP.WF.SendReturnObjs();

                //if (result == 0)
                //{
                //    objs = BP.WF.Dev2Interface.Node_SendWork(fkFlow, long.Parse(strOid));
                //    //フォロー完了後処理
                //    wf_appfrom.FlowOverProc(objs, form, strOid);
                //    infoMsg = "0";
                //}
                //else {
                //    switch (tehaiKbn)
                //    {
                //        //手配済み
                //        case TEHAI_KBN_ZUMI:
                //            objs = BP.WF.Dev2Interface.Node_SendWork(fkFlow, long.Parse(strOid));
                //            //フォロー完了後処理
                //            wf_appfrom.FlowOverProc(objs, form, strOid);
                //            infoMsg = "手配済みの処理を終了しました。";
                //            break;
                //        //手配不能
                //        case TEHAI_KBN_FUNOU:
                //            infoMsg = BP.WF.Dev2Interface.Flow_DoFlowOver(fkFlow, long.Parse(strOid), "手配不能の処理を終了しました。");
                //            //フォロー完了後処理
                //            wf_appfrom.FlowOverProc(objs, form, strOid, true);
                //            break;
                //        //確認中
                //        case TEHAI_KBN_CONFIRMING:
                //            infoMsg = "未手配の処理を終了しました。";
                //            break;

                //    }
                //}
                return result.ToString();
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 手配業者依頼一覧データの取得
        /// </summary>
        /// <returns>メッセージ情報/取得したデータリスト</returns>
        public string GetArrangeTraderReqList()
        {
            try
            {
                // 検索条件の取得
                ArrangeTraderReq cond =
                    JsonConvert.DeserializeObject<ArrangeTraderReq>(
                        this.GetRequestVal("ArrangeTraderReq"));

                // Sql文と条件設定の取得
                GetArrangeTraderReqListSql(cond, out string sql, out Paras ps);

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 検索用sqlの作成
        /// </summary>
        /// <param name="searchCond"></param>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <returns>sql文</returns>
        private void GetArrangeTraderReqListSql(ArrangeTraderReq searchCond, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();

            // sql文の作成
            // 伝票番号
            sqlSb.Append("SELECT Condolence.OID AS WorkID,");
            // (スナップショット)不幸従業員の会社コード
            sqlSb.Append("       Condolence.UNFORTUNATE_KAISYACODE AS CompanyCode,");
            // (スナップショット)不幸従業員の会社名
            sqlSb.Append("       Condolence.UNFORTUNATE_KAISYAMEI AS CompanyName,");
            // 申請者カナ  (スナップショット)不幸従業員の社員名(フリガナ)
            sqlSb.Append("       Condolence.UNFORTUNATE_FURIGANAMEI AS ApplicationName,");
            // 申請日
            sqlSb.Append("       CONVERT(nvarchar(10),Condolence.REC_ENT_DATE,111) AS ApplicationDate,");
            // 納品希望日
            sqlSb.Append("       CASE");
            // 【弔事連絡トランザクションテーブル】供花届ける場所区分が通夜の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 0 THEN CONVERT(nvarchar(10),Condolence.TSUYA_DATE,111) ");
            // 【弔事連絡トランザクションテーブル】供花届ける場所区分が告別式の場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 1 THEN CONVERT(nvarchar(10),Condolence.KOKUBETSUSHIKI_DATE,111) ");
            // 【弔事連絡トランザクションテーブル】供花届ける場所区分が後飾りの場合
            sqlSb.Append("           WHEN Condolence.TODOKESAKIKBN = 2 THEN CONVERT(nvarchar(10),Condolence.ATOKAZARI_DATE,111) ");
            // 【弔事連絡トランザクションテーブル】供花届ける場所区分が上記以外りの場合
            sqlSb.Append("           ELSE '' ");
            // 納品希望日
            sqlSb.Append("       END AS DeliveryDate,");
            // 供花区分
            sqlSb.Append("       Condolence.KYOKAKBN AS KyokaKbn,");
            // 弔電区分
            sqlSb.Append("       Condolence.TYODENKBN AS TyodenKbn,");
            // 手配区分
            sqlSb.Append("       CASE");
            // 両方の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = 1 AND Condolence.TYODENKBN = 1 THEN ''");
            // 供花の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = 0 AND Condolence.TYODENKBN = 1 THEN '供花'");
            // 弔電の場合
            sqlSb.Append("           WHEN Condolence.KYOKAKBN = 1 AND Condolence.TYODENKBN = 0 THEN '弔電'");
            // 上記以外の場合
            sqlSb.Append("           ELSE '供花、弔電'");
            // 分岐終了
            sqlSb.Append("       END AS ArrangeKbn,");
            // 最終更新日
            sqlSb.Append("       CONVERT(nvarchar(10),Condolence.REC_EDT_DATE,111) AS LastUpdDate, ");
            // 処理区分
            sqlSb.Append("       Condolence.TEHAIKBN AS ProcessKbn, ");
            // 申請番号
            sqlSb.Append("       orderNum.ORDER_NUMBER AS AppCode ");
            // 主テーブル
            sqlSb.Append("    FROM TT_WF_CONDOLENCE Condolence ");

            // LEFT JOIN 区分マスタ
            sqlSb.Append("    LEFT JOIN TT_WF_ORDER_NUMBER orderNum ON ");
            // LEFT JOIN 会社マスタ 条件
            sqlSb.Append("           orderNum.OID = Condolence.OID ");

            // 条件
            sqlSb.Append("    WHERE 1 = 1 ");
            // デフォルト条件の追加
            // ワークフロー状態が「2:申請提出した後/引戻し」、かつ、
            // 手配状態がnull以外の場合
            sqlSb.Append("          AND ((Condolence.WFState = 2 AND Condolence.TEHAIKBN IS NOT NULL)");
            // ワークフロー状態が「3:承認済み/否認」
            sqlSb.Append("               OR Condolence.WFState = 3)");

            // 香料・供花・弔電区分が「0:必要」
            // sqlSb.Append("          AND Condolence.KORYO_KYOKA_TYODEN_KBN = 0");
            // 供花発行区分が「0:必要」、弔電発行区分あるいは「0:必要」
            // sqlSb.Append("          AND (Condolence.KYOKAKBN = 0 OR Condolence.TYODENKBN = 0)");
            // 手配状態はnull以外の場合
            sqlSb.Append("          AND Condolence.TEHAIKBN IS NOT NULL");

            // パラメータの作成
            ps = new Paras();

            // 画面入力により、条件を作ること
            // 申請番号
            if (!string.IsNullOrEmpty(searchCond.AppCode))
            {
                // 部分一致で検索 【弔事連絡トランザクションテーブル】伝票番号
                sqlSb.Append("       AND orderNum.ORDER_NUMBER = @AppCode");

                // 入力条件
                ps.Add("AppCode", searchCond.AppCode);
            }

            // 会社コード
            if (!string.IsNullOrEmpty(searchCond.CompanyCode))
            {
                // 部分一致で検索 【弔事連絡トランザクションテーブル】伝票番号
                sqlSb.Append("       AND Condolence.UNFORTUNATE_KAISYACODE = @CompanyCode");

                // 入力条件
                ps.Add("CompanyCode", searchCond.CompanyCode);
            }

            // 申請日 FromとToの両方がある場合
            if (!string.IsNullOrEmpty(searchCond.AppDateFrom) && !string.IsNullOrEmpty(searchCond.AppDateTo))
            {
                // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                sqlSb.Append("       AND (CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8),Condolence.REC_ENT_DATE,112)");
                sqlSb.Append("       AND CONVERT(nvarchar(8),Condolence.REC_ENT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112))");

                // 入力条件
                ps.Add("AppDateFrom", searchCond.AppDateFrom);
                ps.Add("AppDateTo", searchCond.AppDateTo);
            }
            // Fromのみがある場合
            else if (!string.IsNullOrEmpty(searchCond.AppDateFrom) && string.IsNullOrEmpty(searchCond.AppDateTo))
            {
                // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @AppDateFrom),112) <= CONVERT(nvarchar(8),Condolence.REC_ENT_DATE,112)");

                // 入力条件
                ps.Add("AppDateFrom", searchCond.AppDateFrom);
            }
            // Toのみがある場合
            else if (string.IsNullOrEmpty(searchCond.AppDateFrom) && !string.IsNullOrEmpty(searchCond.AppDateTo))
            {
                // 年月日の完全一致検索【弔事連絡トランザクションテーブル】登録日時
                sqlSb.Append("       AND CONVERT(nvarchar(8),Condolence.REC_ENT_DATE,112) <= CONVERT(nvarchar(8),CONVERT(datetime, @AppDateTo),112)");

                // 入力条件
                ps.Add("AppDateTo", searchCond.AppDateTo);
            }

            // (スナップショット)不幸従業員の会社名
            if (!string.IsNullOrEmpty(searchCond.CompanyName))
            {
                // 部分一致検索 弔事連絡トランザクションテーブル】ご不幸にあわれた方の会社
                sqlSb.Append("       AND Condolence.UNFORTUNATE_KAISYAMEI LIKE @CompanyName");

                // 入力条件
                ps.Add("CompanyName", "%" + searchCond.CompanyName + "%");
            }

            // 申請者カナ (スナップショット)不幸従業員の社員名(フリガナ)
            if (!string.IsNullOrEmpty(searchCond.ApplicationName))
            {
                // 部分一致検索 弔事連絡トランザクションテーブル】ご不幸にあわれた方の会社
                sqlSb.Append("       AND Condolence.UNFORTUNATE_FURIGANAMEI LIKE @ApplicationName");

                // 入力条件
                ps.Add("ApplicationName", "%" + searchCond.ApplicationName + "%");
            }

            // 納品希望日 FromとToの両方がある場合
            if (!string.IsNullOrEmpty(searchCond.PreDateFrom) && !string.IsNullOrEmpty(searchCond.PreDateTo))
            {
                // 納品希望日 年月日の完全一致検索
                sqlSb.Append("       AND (CONVERT(nvarchar(8),CONVERT(datetime, @PreDateFrom),112) <= ");
                sqlSb.Append("           CASE");
                // お通夜：通夜日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 0 THEN CONVERT(nvarchar(8),Condolence.TSUYA_DATE,112) ");
                // 告別式：告別式日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 1 THEN CONVERT(nvarchar(8),Condolence.KOKUBETSUSHIKI_DATE,112) ");
                // 後飾り：後飾り日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 2 THEN CONVERT(nvarchar(8),Condolence.ATOKAZARI_DATE,112) ");
                // その他
                sqlSb.Append("               ELSE '' END ");

                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @PreDateTo),112) >= ");
                sqlSb.Append("           CASE");
                // お通夜：通夜日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 0 THEN CONVERT(nvarchar(8),Condolence.TSUYA_DATE,112) ");
                // 告別式：告別式日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 1 THEN CONVERT(nvarchar(8),Condolence.KOKUBETSUSHIKI_DATE,112) ");
                // 後飾り：後飾り日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 2 THEN CONVERT(nvarchar(8),Condolence.ATOKAZARI_DATE,112) ");
                // その他
                sqlSb.Append("               ELSE '' END )");

                // 入力条件
                ps.Add("PreDateFrom", searchCond.PreDateFrom);
                ps.Add("PreDateTo", searchCond.PreDateTo);
            }
            // Fromのみがある場合
            else if (!string.IsNullOrEmpty(searchCond.PreDateFrom) && string.IsNullOrEmpty(searchCond.PreDateTo))
            {
                // 納品希望日 年月日の完全一致検索
                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @PreDateFrom),112) <= ");
                sqlSb.Append("           CASE");
                // お通夜：通夜日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 0 THEN CONVERT(nvarchar(8),Condolence.TSUYA_DATE,112) ");
                // 告別式：告別式日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 1 THEN CONVERT(nvarchar(8),Condolence.KOKUBETSUSHIKI_DATE,112) ");
                // 後飾り：後飾り日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 2 THEN CONVERT(nvarchar(8),Condolence.ATOKAZARI_DATE,112) ");
                // その他
                sqlSb.Append("               ELSE '' END ");

                // 入力条件
                ps.Add("PreDateFrom", searchCond.PreDateFrom);
            }
            // Toのみがある場合
            else if (string.IsNullOrEmpty(searchCond.PreDateFrom) && !string.IsNullOrEmpty(searchCond.PreDateTo))
            {
                // 納品希望日 年月日の完全一致検索
                sqlSb.Append("       AND CONVERT(nvarchar(8),CONVERT(datetime, @PreDateTo),112) >= ");
                sqlSb.Append("           CASE");
                // お通夜：通夜日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 0 THEN CONVERT(nvarchar(8),Condolence.TSUYA_DATE,112) ");
                // 告別式：告別式日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 1 THEN CONVERT(nvarchar(8),Condolence.KOKUBETSUSHIKI_DATE,112) ");
                // 後飾り：後飾り日時
                sqlSb.Append("               WHEN Condolence.TODOKESAKIKBN = 2 THEN CONVERT(nvarchar(8),Condolence.ATOKAZARI_DATE,112) ");
                // その他
                sqlSb.Append("               ELSE '' END ");

                // 入力条件
                ps.Add("PreDateTo", searchCond.PreDateTo);
            }

            // 手配区分
            if (searchCond.ArrangeKbn != null && searchCond.ArrangeKbn.Count > 0)
            {
                // 選択された供花届ける場所区分を検索。複数選択時は、OR検索
                sqlSb.Append("       AND (");

                int maxCount = searchCond.ArrangeKbn.Count;
                for (int i = 0; i < maxCount; i++)
                {
                    if (i > 0)
                    {
                        sqlSb.Append("            OR");
                    }
                    // 選択された手配区分を検索。複数選択時は、OR検索
                    if (searchCond.ArrangeKbn[i] == 0)
                    {
                        // なし
                        sqlSb.Append("            (Condolence.KYOKAKBN = 1 AND Condolence.TYODENKBN = 1) ");
                    }
                    else if (searchCond.ArrangeKbn[i] == 1)
                    {
                        // 供花のみ
                        sqlSb.Append("            (Condolence.KYOKAKBN = 0 AND Condolence.TYODENKBN = 1) ");
                    }
                    else if (searchCond.ArrangeKbn[i] == 2)
                    {
                        // 弔電のみ
                        sqlSb.Append("            (Condolence.KYOKAKBN = 1 AND Condolence.TYODENKBN = 0) ");
                    }
                    else
                    {
                        // 供花・弔電両方
                        sqlSb.Append("            (Condolence.KYOKAKBN = 0 AND Condolence.TYODENKBN = 0) ");
                    }
                }

                sqlSb.Append("       )");
            }

            // 処理区分
            if (searchCond.ProcessKbn != null && searchCond.ProcessKbn.Count > 0)
            {
                // 選択された処理区分を検索。複数選択時は、OR検索
                sqlSb.Append("       AND (");

                int maxCount = searchCond.ProcessKbn.Count;
                for (int i = 0; i < maxCount; i++)
                {
                    if (i > 0)
                    {
                        sqlSb.Append("            OR");
                    }
                    // 選択された処理区分を検索。複数選択時は、OR検索
                    sqlSb.Append("                      Condolence.TEHAIKBN = @ProcessKbn" + i);

                    // 入力条件
                    ps.Add("ProcessKbn" + i, searchCond.ProcessKbn[i]);
                }

                sqlSb.Append("       )");
            }

            // sql文の設定
            sql = sqlSb.ToString();
        }

        // <summary>
        // 弔辞連絡情報を取得
        // </summary>
        public string GetCondolenceInfo()
        {
            try
            {

                // Sql文と条件設定の取得
                string sql = "SELECT * FROM TT_WF_CONDOLENCE WHERE OID = @OID";

                Paras ps = new Paras();
                // 入力条件
                ps.Add("OID", this.GetRequestVal("strOid"));

                // Sqlの実行
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

                string[] arrCol = new String[] { "CANCEL_EMP_NO", "CHECK_EMP_NO", "TEHAISYA_NO", "CMP_EMP_NO" };


                // パラメータ設定
                Dictionary<string, object> apiParam = new Dictionary<string, object>();
                List<Dictionary<string, string>> rel = null;

                for (int i = 0; i < arrCol.Length; i++)
                {
                    dt.Columns.Add(arrCol[i] + "_SEIMEI_KANJI");
                    dt.Rows[0][arrCol[i] + "_SEIMEI_KANJI"] = "";
                    if (dt.Rows[0][arrCol[i]].ToString().Length > 0)
                    {
                        apiParam.Clear();
                        apiParam.Add("SHAINBANGO", dt.Rows[0][arrCol[i]].ToString());
                        // 従業員情報を取得
                        rel = WF_AppForm.GetEbsDataWithApi("Get_MT_Employee", apiParam);

                        if (rel.Count > 0)
                        {
                            dt.Rows[0][arrCol[i] + "_SEIMEI_KANJI"] = rel[0]["SEI_KANJI"] + ' ' + rel[0]["MEI_KANJI"];
                        }
                    }
                }

                // フロントに戻ること
                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                WF_OutLog.WriteLog(ex.Message, WF_OutLog.ERROR_MODE);
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 手配業者依頼一覧検索条件クラス
        /// </summary>
        [DataContract]
        private class ArrangeTraderReq
        {
            /// <summary>
            /// 申請番号
            /// </summary>
            [DataMember(Name = "app_code_search")]
            public string AppCode { get; set; }

            /// <summary>
            /// 会社コード
            /// </summary>
            [DataMember(Name = "company_code_search")]
            public string CompanyCode { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            [DataMember(Name = "company_name_search")]
            public string CompanyName { get; set; }

            /// <summary>
            /// 申請者かな
            /// </summary>
            [DataMember(Name = "app_emp_kana_search")]
            public string ApplicationName { get; set; }

            /// <summary>
            /// 申請日検索条件From
            /// </summary>
            [DataMember(Name = "app_date_search_from")]
            public string AppDateFrom { get; set; }

            /// <summary>
            /// 申請日検索条件To
            /// </summary>
            [DataMember(Name = "app_date_search_to")]
            public string AppDateTo { get; set; }

            /// <summary>
            /// 納品希望日検索条件From
            /// </summary>
            [DataMember(Name = "delivery_date_search_from")]
            public string PreDateFrom { get; set; }

            /// <summary>
            /// 納品希望日検索条件To
            /// </summary>
            [DataMember(Name = "delivery_date_search_to")]
            public string PreDateTo { get; set; }

            /// <summary>
            /// 手配区分
            /// </summary>
            [DataMember(Name = "arrange_class_search")]
            public List<int> ArrangeKbn { get; set; }

            /// <summary>
            /// 処理区分
            /// </summary>
            [DataMember(Name = "process_class_search")]
            public List<int> ProcessKbn { get; set; }
        }
    }
}