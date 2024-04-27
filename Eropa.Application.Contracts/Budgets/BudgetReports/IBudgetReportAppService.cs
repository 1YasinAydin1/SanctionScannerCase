namespace Eropa.Application.Contracts.Budgets
{
    public interface IBudgetReportAppService
    {
        Task<IQueryable<BudgetReportDto>> GetBudgetReportAsync(BudgetReportFilterDto budgetReportFilterDto);
    }
}
