using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.Budgets
{
    public interface IBudgetAppService
    {
        Task<IQueryable<CurrencyCurrDto>> GetCurrencyAsync();
        Task<IQueryable<BudgetDto>> GetBadgetAsync(string DocEn = "");
        Task<IQueryable<ProjectDto>> GetProjectAsync();
        Task<IResult> UpdateBudgetAsync(BudgetUpdateDto budgetUpdateDto);
        Task<IResult> DeleteBudgetAsync(string key);
        Task<IResult> BudgetExcelImportAsync(string excelBase64);
        Task<IResult> RevisedBudgetAsync(string revisedDocEn);
        Task<IResult> BudgetEmptyRecordAdd();
    }
}
