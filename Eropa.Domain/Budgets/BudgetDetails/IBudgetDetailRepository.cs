using Eropa.Helper.Results;

namespace Eropa.Domain.Budgets
{
    public interface IBudgetDetailRepository
    {
        Task<IQueryable<BudgetDetail>> GetBudgetDetail(string docEn);
        Task<IQueryable<BudgetDetail>> GetSingleBudgetDetail(string docEn, string lineId);
        Task<IQueryable<BudgetDetail>> GetSingleBudgetDetailByActivityCode(string docEn, string ActivityCode, string part = "");
        Task<IQueryable<GetItem>> GetItemsForBudgetDetail();
        Task<IQueryable<GetCurrency>> GetCurrencysForBudgetDetail();
        Task<IQueryable<GetMeasurementUnit>> GetMeasurementUnitsForBudgetDetail();
        Task<IResult> DeleteBudgetDetail(string docEn, string lineId = "");
        Task<string> GetLineIdByRowNum(string docEn, string key);
    }
}
