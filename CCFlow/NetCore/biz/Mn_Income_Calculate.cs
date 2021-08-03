using BP.DA;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

/// <summary>
/// 所得計算機能
/// </summary>
namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 所得計算機能
    /// </summary>
    public class Mn_Income_Calculate : BP.WF.HttpHandler.DirectoryPageBase
    {

        WF_AppForm wf_appfrom = new WF_AppForm();
        AppFormLogic form = new AppFormLogic();

        /// <summary>
        /// 給与以外の所得区分 0:いいえ 1:はい
        /// </summary>
        private const string CONST_SALARYKBN_YES = "1";

        /// <summary>
        /// 所得計算結果の取得
        /// </summary>
        /// <returns>所得計算結果</returns>
        public string GetIncomeCalculate()
        {
            try
            {
                long result = 0;

                // 所得計算条件の取得
                CalculationCond calculationCond =
                    JsonConvert.DeserializeObject<CalculationCond>(
                        this.GetRequestVal("CalculationCond"));

                // 給与所得の計算
                result += this.GetSalaryIncome(calculationCond);

                // 給与所得以外の所得
                if (calculationCond.SalaryKbn == CONST_SALARYKBN_YES)
                {
                    // 事業所得の計算
                    result += this.GetBusinessIncome(calculationCond);

                    // 雑所得の計算
                    result += this.GetMiscellaneousIncome(calculationCond);

                    // 配当所得の計算
                    result += this.GetDividendIncome(calculationCond);

                    // 不動産所得の計算
                    result += this.GetRealEstateIncome(calculationCond);

                    // 退職所得の計算
                    result += this.GetRetirementIncome(calculationCond);

                    // 上記以外の所得の計算
                    result += this.ProcessingMinusMoney(this.GetMoneyNum(calculationCond.IncomeOtherRevenueAmount));
                }

                // フロントに戻ること
                return result.ToString();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 給与所得の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>給与所得の計算結果</returns>
        private long GetSalaryIncome(CalculationCond calculationCond)
        {
            long salaryIncome = 0;

            // 給与所得金額の取得
            long salaryIncomeAmount = this.GetSalaryIncomeAmount(calculationCond);

            // 画面の入力の特定支出の取得
            long specificSpending = this.GetMoneyNum(calculationCond.SpecificSpending);

            // 特定支出の判断
            if (specificSpending > 0)
            {
                // 給与所得控除額の取得
                // 画面の入力の収入金額 - DBから取得の給与所得金額 = 給与所得控除額
                long deductionAmount = this.GetMoneyNum(calculationCond.RevenueAmount) - salaryIncomeAmount;
                // 給与所得控除額の1/2の取得
                long halfDeductionAmount = deductionAmount / 2;

                // その年の上記支出の合計額が、給与所得控除額の1/2を超えた場合
                if (specificSpending > halfDeductionAmount)
                {
                    // 給与所得金額	＝ 収入金額 -(給与所得控除額＋(その年の特定支出の合計額 - 給与所得控除額の1 / 2))
                    salaryIncome = this.GetMoneyNum(calculationCond.RevenueAmount) - (deductionAmount + (specificSpending - halfDeductionAmount));
                }
                else
                {
                    salaryIncome = salaryIncomeAmount;
                }
            }
            else
            {
                salaryIncome = salaryIncomeAmount;
            }

            return this.ProcessingMinusMoney(salaryIncome);
        }

        /// <summary>
        /// 給与所得金額の取得
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>給与所得金額結果</returns>
        private long GetSalaryIncomeAmount(CalculationCond calculationCond)
        {
            // 給与所得の入力条件の取得
            long revenueAmount = this.GetMoneyNum(calculationCond.RevenueAmount);

            // Sql文と条件設定の取得
            this.GetSalaryIncomeAmountSql(revenueAmount, out string sql, out Paras ps);

            // Sqlの実行
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql, ps);

            // 給与所得控除額
            long deductionAmount = 0;
            // フロントに戻ること
            if (dt.Rows.Count > 0)
            {
                deductionAmount = (long)(revenueAmount * (decimal)dt.Rows[0]["RATIO"]) + (long)dt.Rows[0]["SALARY_INCOME_DEDUCTION"];
            }

            return deductionAmount;
        }

        /// <summary>
        /// DBから給与所得金額の取得
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>給与所得金額結果</returns>
        private void GetSalaryIncomeAmountSql(long revenueAmount, out string sql, out Paras ps)
        {
            // sql文対象の作成
            StringBuilder sqlSb = new StringBuilder();
            // パラメータの作成
            ps = new Paras();

            // sql文の作成
            // 適用年月日_FROM
            sqlSb.Append("SELECT SalaryIncome.APP_YMD_FROM AS APP_YMD_FROM,");
            // 適用年月日_TO
            sqlSb.Append("       SalaryIncome.APP_YMD_TO AS APP_YMD_TO,");
            // 適用金額始
            sqlSb.Append("       SalaryIncome.APP_AMOUNT_START AS APP_AMOUNT_START,");
            // 適用金額終
            sqlSb.Append("       SalaryIncome.APP_AMOUNT_END AS APP_AMOUNT_END,");
            // 給与所得控除後の給与等の金額
            sqlSb.Append("       SalaryIncome.SALARY_INCOME_DEDUCTION AS SALARY_INCOME_DEDUCTION,");
            // 比率
            sqlSb.Append("       SalaryIncome.RATIO AS RATIO");

            // テーブル名
            sqlSb.Append("  FROM MT_SALARY_INCOME_CRITERIA SalaryIncome");

            // 条件
            // 適用金額始 < パラメータの金額
            sqlSb.Append("    WHERE SalaryIncome.APP_AMOUNT_START <= @MONEY ");
            // パラメータの金額 <= 適用金額終
            sqlSb.Append("          AND @MONEY < SalaryIncome.APP_AMOUNT_END");
            // 適用年月日_FROM <= システム時間
            sqlSb.Append("          AND SalaryIncome.APP_YMD_FROM <= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");
            // システム時間 >= 適用年月日_TO
            sqlSb.Append("          AND SalaryIncome.APP_YMD_TO >= (SELECT CONVERT(NVARCHAR(8), GETDATE(), 112)) ");


            // パラメータの設定
            ps.Add("MONEY", revenueAmount);

            // sql文の設定
            sql = sqlSb.ToString();
        }

        /// <summary>
        /// 事業所得の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>事業所得の計算結果</returns>
        private long GetBusinessIncome(CalculationCond calculationCond)
        {
            // 事業所得	＝ 総収入金額※1 - 必要経費※2（ - 青色申告特別控除額※3）
            long businessIncome = this.GetMoneyNum(calculationCond.BizIncRevenueAmount) -
                this.GetMoneyNum(calculationCond.BizIncNeedAmount);

            return businessIncome;
        }

        /// <summary>
        /// 雑所得の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>雑所得の計算結果</returns>
        private long GetMiscellaneousIncome(CalculationCond calculationCond)
        {
            // 雑所得 ＝ 公的年金等の雑所得 ＋ 公的年金等以外の雑所得
            // 雑所得 ＝ (収入金額ー公的年金等控除額※1) ＋ (総収入金額ー必要経費)
            long miscellaneousIncome =
                (this.GetMoneyNum(calculationCond.PubPensionRevenueAmount) - this.GetMoneyNum(calculationCond.PubPensionDeductionAmount)) +
                (this.GetMoneyNum(calculationCond.PubPensionOutRevenueAmount) - this.GetMoneyNum(calculationCond.MiscellaneousNeedAmount));

            return this.ProcessingMinusMoney(miscellaneousIncome);
        }

        /// <summary>
        /// 配当所得の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>配当所得の計算結果</returns>
        private long GetDividendIncome(CalculationCond calculationCond)
        {
            // 配当所得 ＝ 収入金額 - 株式等を取得するための負債利子※1
            long dividendIncome = 
                this.GetMoneyNum(calculationCond.DivIncRevenueAmount) - this.GetMoneyNum(calculationCond.DebtInterest);

            return this.ProcessingMinusMoney(dividendIncome);
        }

        /// <summary>
        /// 不動産所得の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>不動産所得の計算結果</returns>
        private long GetRealEstateIncome(CalculationCond calculationCond)
        {
            // 不動産所得 ＝ 総収入金額※1 - 必要経費※2（ - 青色申告特別控除額）
            long realEstateIncome =
                this.GetMoneyNum(calculationCond.RealEstateRevenueAmount) - this.GetMoneyNum(calculationCond.RealEstateNeedAmount);

            return realEstateIncome;
        }

        /// <summary>
        /// 退職所得の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>退職所得の計算結果</returns>
        private long GetRetirementIncome(CalculationCond calculationCond)
        {
            // 不動産所得 ＝ 課税退職所得金額 + 特定役員退職手当に係る退職所得 +
            //               使用人としての退職金と役員退職金の双方の支給があった場合の退職所得
            long retirementIncome =
                this.GetTaxableRetirementIncome(calculationCond) +
                this.GetMoneyNum(calculationCond.RetirementIncome) +
                this.GetMoneyNum(calculationCond.TwinsRetirementIncome);

            return this.ProcessingMinusMoney(retirementIncome);
        }

        /// <summary>
        /// 課税退職所得金額の計算
        /// </summary>
        /// <param name="calculationCond">所得計算条件</param>
        /// <returns>課税退職所得金額の計算結果</returns>
        private long GetTaxableRetirementIncome(CalculationCond calculationCond)
        {
            // 収入金額は入力されてない場合、または、勤続年数が入力されてない場合
            if (string.IsNullOrEmpty(calculationCond.RetirementIncomeRevenueAmount) ||
                string.IsNullOrEmpty(calculationCond.YearsContinuation))
            {
                return 0;
            }

            // 課税退職所得金額
            long taxableRetirementIncome = 0;
            // 退職所得控除額
            long deductionAmount;

            // 勤続年数の判断
            long yearsContinuation = this.GetMoneyNum(calculationCond.YearsContinuation);
            if (yearsContinuation <= 20)
            {
                // 20年以下
                // 40万円×勤続年数
                deductionAmount = 400000 * yearsContinuation;

                // 退職所得控除額は算式によって計算した金額が80万円未満の場合は、退職所得控除額は80万円になります。
                if (deductionAmount < 800000)
                {
                    deductionAmount = 800000;
                }
            }
            else
            {
                // 20年超
                // 800万円＋70万円×（継続年数 - 20年）
                deductionAmount = 8000000 + 700000 * (yearsContinuation - 20);
            }

            // 「主な退職理由は障害者になったためである」の判断
            if (calculationCond.ReasonRetirement)
            {
                // 障害者となったことに直接基因して退職した場合は、
                // 計算した金額に、100万円を加算した金額が退職所得控除額です。
                deductionAmount += 1000000;
            }

            // 課税退職所得金額 = (収入金額 - 退職所得控除額) * 0.5
            taxableRetirementIncome = (this.GetMoneyNum(calculationCond.RetirementIncomeRevenueAmount) - deductionAmount) / 2;

            // 1000円未満端数切り捨て
            taxableRetirementIncome = (taxableRetirementIncome / 1000) * 1000;

            return taxableRetirementIncome;
        }

        /// <summary>
        /// 数字型金額の取得
        /// </summary>
        /// <param name="money">所得計算条件</param>
        /// <returns>数字型金額結果</returns>
        private long GetMoneyNum(string money)
        {
            long moneyNum = 0;

            // 金額の判断
            if (!string.IsNullOrEmpty(money))
            {
                // 文字列の金額を数字の金額に変更するとこ
                if (!long.TryParse(money, out moneyNum))
                {
                    moneyNum = 0;
                }
            }

            return moneyNum;
        }

        /// <summary>
        /// 数字型金額の取得
        /// </summary>
        /// <param name="money">所得計算条件</param>
        /// <returns>数字型金額結果</returns>
        private long ProcessingMinusMoney(long money)
        {
            // 金額符号の判断
            if (money < 0)
            {
                // マイナスの場合
                return 0;
            }
            else
            {
                // そのまま戻り
                return money;
            }
        }

        /// <summary>
        /// 所得計算条件クラス
        /// </summary>
        [DataContract]
        private class CalculationCond
        {
            /// <summary>
            /// 給与以外の所得区分 0:いいえ 1:はい
            /// </summary>
            [DataMember(Name = "salaryKbn")]
            public string SalaryKbn { get; set; }

            /// <summary>
            /// 給与所得 収⼊⾦額
            /// </summary>
            [DataMember(Name = "revenue_amount")]
            public string RevenueAmount { get; set; }

            /// <summary>
            /// 給与所得 特定⽀出
            /// </summary>
            [DataMember(Name = "specific_spending")]
            public string SpecificSpending { get; set; }

            /// <summary>
            /// 事業所得 収⼊⾦額
            /// </summary>
            [DataMember(Name = "business_income_revenue_amount")]
            public string BizIncRevenueAmount { get; set; }

            /// <summary>
            /// 事業所得 必要経費
            /// </summary>
            [DataMember(Name = "business_income_need_amount")]
            public string BizIncNeedAmount { get; set; }

            /// <summary>
            /// 雑所得 公的年⾦等の収⼊⾦額
            /// </summary>
            [DataMember(Name = "public_pension_revenue_amount")]
            public string PubPensionRevenueAmount { get; set; }

            /// <summary>
            /// 雑所得 公的年⾦等控除額
            /// </summary>
            [DataMember(Name = "public_pension_deduction_amount")]
            public string PubPensionDeductionAmount { get; set; }

            /// <summary>
            /// 雑所得 公的年⾦等以外の収⼊⾦額
            /// </summary>
            [DataMember(Name = "public_pension_out_revenue_amount")]
            public string PubPensionOutRevenueAmount { get; set; }

            /// 雑所得 必要経費
            /// </summary>
            [DataMember(Name = "miscellaneous_need_amount")]
            public string MiscellaneousNeedAmount { get; set; }

            /// <summary>
            /// 配当所得 収⼊⾦額
            /// </summary>
            [DataMember(Name = "dividend_income_revenue_amount")]
            public string DivIncRevenueAmount { get; set; }

            /// <summary>
            /// 配当所得 要した負債の利⼦
            /// </summary>
            [DataMember(Name = "debt_interest")]
            public string DebtInterest { get; set; }

            /// <summary>
            /// 不動産所得 収⼊⾦額
            /// </summary>
            [DataMember(Name = "real_estate_revenue_amount")]
            public string RealEstateRevenueAmount { get; set; }

            /// <summary>
            /// 不動産所得 必要経費
            /// </summary>
            [DataMember(Name = "real_estate_need_amount")]
            public string RealEstateNeedAmount { get; set; }

            /// <summary>
            /// 退職所得 収⼊⾦額
            /// </summary>
            [DataMember(Name = "retirement_income_revenue_amount")]
            public string RetirementIncomeRevenueAmount { get; set; }

            /// <summary>
            /// 退職所得 継続年数
            /// </summary>
            [DataMember(Name = "years_continuation")]
            public string YearsContinuation { get; set; }

            /// <summary>
            /// 退職所得 主な退職理由は障害者になったためである
            /// checked : 1
            /// </summary>
            [DataMember(Name = "reason_retirement")]
            public bool ReasonRetirement { get; set; }

            /// <summary>
            /// 退職所得 退職所得
            /// </summary>
            [DataMember(Name = "retirement_allowance_retirement_income")]
            public string RetirementIncome { get; set; }

            /// <summary>
            /// 退職所得 双⽅の⽀給があった場合の退職所得
            /// </summary>
            [DataMember(Name = "twins_retirement_income")]
            public string TwinsRetirementIncome { get; set; }

            /// <summary>
            /// 上記以外の所得 収⼊⾦額
            /// </summary>
            [DataMember(Name = "income_other_revenue_amount")]
            public string IncomeOtherRevenueAmount { get; set; }
        }
    }
}
