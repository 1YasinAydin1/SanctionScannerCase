namespace Eropa.Domain.Interruptions
{
    public interface IInterruptionRepository
    {
        Task<IQueryable<Interruption>> GetInterruptionsAsync(string Code = "");
        Task<IQueryable<AccountCode>> GetAccountCodesAsync();
        Task<string> GetMaxCodeAsync();
    }
}
