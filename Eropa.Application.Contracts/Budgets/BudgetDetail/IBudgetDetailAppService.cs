using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.Budgets
{
    public interface IBudgetDetailAppService
    {
        Task<IQueryable<BudgetDetailDto>> GetBadgetDetailAsync(string docEn);
        Task<IResult> AddBudgetDetailAsync(BudgetDetailDto budgetCreateDto, string docEn);
        Task<IResult> UpdateBudgetDetailAsync(BudgetDetailDto budgetUpdateDto, string key, string docEn);
        Task<IResult> DeleteBudgetDetailAsync(string docEn,string key);
        Task<IQueryable<GetItemDto>> GetItemsForBudgetDetail();
        Task<IQueryable<GetCurrencyDto>> GetCurrencyForBudgetDetail();
        Task<IQueryable<GetMeasurementUnitDto>> GetMeasurementUnitsForBudgetDetail();

    }
}
