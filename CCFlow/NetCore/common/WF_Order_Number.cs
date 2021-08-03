using System;
using System.Collections.Generic;
using Common.WF_OutLog;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 業務伝票番号作成クラス
    ///指定したワークフローの業務伝票番号作成を作成し、
    ///DBへ格納する。
    /// </summary>
    public class WF_Order_Number
    {
        /// <summary>
        /// ロック用オブジェクト
        /// </summary>
        private static object lockObj = new object();

        /// <summary>
        /// EBS連携先頭文字（CCF固定）
        /// </summary>
        private const String EBS_PREFIX = "CCF";

        /// <summary>
        /// 当該ワークフロー、当該申請日の1件目の番号
        /// </summary>
        private const String FIRST_NUMBER = "0001";

        /// <summary>
        /// 業務伝票番号作成
        /// </summary>
        /// <param name="strOid">申請のOID</param>
        /// <param name="strWFNo">ワークフロー番号（3桁）</param>
        /// <param name="strApplyDate">申請日（YYYYMMDD形式）</param>
        /// <param name="userId">利用者ID</param>
        /// <returns>業務伝票番号</returns>
        public static String CreateOrderNumber(String strOid, String strWFNo, String strApplyDate, String userId)
        {
            String strOrderNumber = "";

            try
            {
                //入力パラメータチェック
                argumentCheck(strOid, strWFNo, strApplyDate, userId);

                //ワークフローの伝票区分取得
                String strPrefix = GetWFOrderNumberCode(strWFNo);

                //業務伝票番号作成
                RegisterOrderNumber(strOid, strWFNo, strApplyDate, userId, strPrefix);

                //登録した伝票番号取得
                strOrderNumber = GetOrderNumber(strOid);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strOrderNumber;
        }

        /// <summary>
        /// 業務伝票番号取得
        /// </summary>
        /// <param name="strOid">OID</param>
        /// <returns>
        /// OIDに繋がり業務伝票番号。なければ、空
        /// </returns>
        public static String GetOrderNumber(String strOid)
        {
            //既存有無確認用SQL
            String strSelExistFmt = "SELECT ORDER_NUMBER FROM TT_WF_ORDER_NUMBER WHERE OID = {0}";
            //既存有無確認
            String strSql = String.Format(strSelExistFmt, strOid);
            String strOrderNumber = BP.DA.DBAccess.RunSQLReturnString(strSql);

            return strOrderNumber;
        }

        /// <summary>
        /// パラメータチェック
        /// 問題があれば、Exceptionを出す
        /// <param name="strOid">OID</param>
        /// <param name="strWFNo">ワークフロー番号</param>
        /// <param name="strDate">申請日</param>
        /// <param name="userId">利用者ID</param>
        private static void argumentCheck(String strOid, String strWFNo, String strDate, String userId)
        {
            String strErrFmt = "";
            String strError;

            //NULLチェック
            if ((String.IsNullOrWhiteSpace(strOid))
                || (String.IsNullOrWhiteSpace(strWFNo))
                || (String.IsNullOrWhiteSpace(strDate))
                || (String.IsNullOrWhiteSpace(userId)))
            {
                strErrFmt = @"未設定パラメータがあります。伝票番号作成ができません。\n
                    OID：{0}\nワークフロー番号：{1}\n申請日：{2}\n利用者ID{3}";
                strError = String.Format(strErrFmt, strOid, strWFNo, strDate, userId);
                throw new ArgumentException(strError);
            }
        }

        /// <summary>
        /// 伝票区分取得
        /// </summary>
        /// <param name="strWFNo">ワークフロー番号</param>
        /// <returns>伝票区分</returns>
        private static String GetWFOrderNumberCode(String strWFNo)
        {
            String strSqlFmt = "SELECT ORDER_NUMBER_PREFIX FROM MT_WF_ORDER_PREFIX WHERE WF_NO = '{0}'";
            String strSql = String.Format(strSqlFmt, strWFNo);

            String strPrefix = BP.DA.DBAccess.RunSQLReturnString(strSql);

            // 定義がなければ、Exceptionを出す
            if (String.IsNullOrEmpty(strPrefix))
            {
                String strErrFmt = "伝票区分取得できません。マスタデータをチェックしてください。\nワークフロー番号：{0}";
                String strError = String.Format(strErrFmt, strWFNo);
                throw new Exception(strError);
            }

            return strPrefix;
        }

        /// <summary>
        /// 業務伝票番号作成
        /// 業務伝票番号フォーマット
        /// CCF＜伝票区分＞＜提出日＞＜当日連番＞
        /// 当日連番連番は、1からの連番、4桁（不足分は前に0を埋め込み）。
        /// ワークフロー番号、かつ提出日毎再採番する。
        /// </summary>
        /// <param name="strOid">OID</param>
        /// <param name="strWFNo">ワークフロー番号</param>
        /// <param name="strApplyDate">申請日</param>
        /// <param name="strUserId">ログインユーザーID</param>
        /// <param name="strCode">伝票区分</param>
        private static void RegisterOrderNumber(String strOid, String strWFNo, String strApplyDate, String strUserId, String strCode)
        {
            //最大値取得SQL
            String strInsSqlFmt = @"INSERT INTO TT_WF_ORDER_NUMBER(OID, WF_NO, APPLY_DATE, ORDER_NUMBER, REC_ENT_DATE, REC_ENT_USER)
                           VALUES({0}, '{1}', '{2}', '{3}' + (
                           SELECT FORMAT(CONVERT(int, RIGHT(ISNULL(MAX(ORDER_NUMBER), '{3}0000'), 4)) + 1, '0000')  
                           FROM TT_WF_ORDER_NUMBER WHERE ORDER_NUMBER LIKE '{3}%'
                           ), getDate(), '{4}') ";
            String strSql, strOrderNumberPrefix;

            strOrderNumberPrefix = String.Format("{0}{1}{2}", EBS_PREFIX, strCode, strApplyDate);

            //業務伝票番号登録
            strSql = String.Format(strInsSqlFmt, strOid, strWFNo, strApplyDate, strOrderNumberPrefix, strUserId);
            BP.DA.DBAccess.RunSQL(strSql);

        }
    }

 }
