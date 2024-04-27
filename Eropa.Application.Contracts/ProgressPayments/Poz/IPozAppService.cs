using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public interface IPozAppService
    {
        Task<IQueryable<GetItemProgressPaymentDto>> GetItemsAsync();
        Task<IQueryable<GetActivityForProgressPaymentDto>> GetActivityForProgressPaymentAsync(string budgetId);
        Task<IResult> DeletePozLineAsync(string docEn, string lineId, string? U_PozAdi);
        Task<IResult> AddUpdatePozLineAsync(PozLineDto PozLineCreateDto, string docEn,string key = "");
        Task<IResult> AddUpdatePozAsync(PozDto PozCreateDto);
        Task<IQueryable<PozDto>> GetPozAsync(string docEn);
        Task<IQueryable<PozLineDto>> GetPozLineAsync(string pozName, string docEn);
        Task<IQueryable<SelectPozDto>> GetSelectPozAsync(string contractNo);
        Task<IDataResult<ProgressPaymentDto>> DeletePozAsync(string docEn, string lineId);
        Task<IQueryable<GetPozCurrencyDto>> GetCurrencysForPozAsync();
        Task<IQueryable<GetVatgroupDto>> GetVatgroupForPozAsync();
        Task<IResult> PozExcelImportAsync(string docEn, string excelBase64);
        Task<ProgressPaymentCreateUpdateDto> GetPPAfterPozLineTransactionAsync(string docEn, string? U_PozAdi);
    }
}
