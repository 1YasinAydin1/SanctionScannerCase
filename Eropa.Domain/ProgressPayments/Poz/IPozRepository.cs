using Eropa.Helper.Results;

namespace Eropa.Domain.ProgressPayments
{
    public interface IPozRepository
    {
        Task<IQueryable<SelectPoz>> GetSelectPozAsync(string contractNo);
        Task<IQueryable<GetItemProgressPayment>> GetItemsForProgressPayment();
        Task<IQueryable<GetActivityForProgressPayment>> GetActivityForProgressPayment(string budgetId);
        Task<IResult> DeletePozLine(string docEn, string lineId);
        Task<IQueryable<Poz>> GetPozAsync(string docEn);
        Task<IQueryable<PozLine>> GetPozLineAsync(string pozName, string DocEn, string LineId = "");
        Task<IDataResult<ProgressPayment>> DeletePoz(string docEn, string lineId);
        Task<IQueryable<GetPozCurrency>> GetCurrencysForPozAsync();
        Task<IQueryable<GetVatgroup>> GetVatgroupForPozAsync();
        Task<T?> GetPozLineTotalAsync<T>(string docEn, int? lineId, string pozName = "", bool isLine = true, bool isHeader = false);
        Task<Poz?> GetPozPreviousTotalAsync(string docEn,int? lineId, string pozName = "");
        Task<int> GetPozLineIdAsync(string docEn, string pozName);
    }
}
