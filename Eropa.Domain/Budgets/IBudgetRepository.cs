namespace Eropa.Domain.Budgets
{
    public interface IBudgetRepository
    {
        Task<IQueryable<CurrencyCurr>> GetCurrency();
        Task<IQueryable<Budget>> GetBudget(string DocEn = "");
        Task<IQueryable<Project>> GetProject();
        Task<string> GetRevised(string project);
        Task<string> GetDocumentNo();
    }
}
