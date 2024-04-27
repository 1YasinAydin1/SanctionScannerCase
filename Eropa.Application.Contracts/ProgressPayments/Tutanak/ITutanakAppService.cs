using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public interface ITutanakAppService
    {
        Task<IQueryable<TutanakDto>> GetTutanakListAsync(string docEn);
        Task<IResult> DeleteTutanakAsync(string docEn, string lineId);
        Task<IResult> AddUpdateTutanakAsync(TutanakDto tutanakDto, string docEn, string key = "");
        Task<TutanakNoDto> GetTutanakNoInfoAsync(string docEn, string orderDocEn);
        Task<IResult> InvoiceAsync(int ContractNo, int DocEntry, string lineIds);
        Task<IQueryable<InvoiceInfoDto>> GetInvoiceInfo(string docEn, string LineId);
        Task<IQueryable<InvoiceLineInfoDto>> GetInvoiceLineInfo(string docEn);
    }
}
