namespace Eropa.Domain.Budgets
{
    public interface IBudgetReportRepository
    {
        Task<IQueryable<BudgetReport>> GetBudgetReport(BudgetReportFilter budgetReportFilter);
    }
}
