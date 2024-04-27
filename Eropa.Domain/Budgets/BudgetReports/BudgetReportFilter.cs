namespace Eropa.Domain.Budgets
{
    public class BudgetReportFilter
    {
        public string Budget { get; set; }
        public string Project { get; set; }
        public string Part { get; set; }
        public double USDCurr { get; set; }
        public double EURCurr { get; set; }
    }
}
