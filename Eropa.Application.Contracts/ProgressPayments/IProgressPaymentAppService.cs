using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public interface IProgressPaymentAppService
    {
        Task<IQueryable<ProgressPaymentDto>> GetProgressPayments();
        Task<IQueryable<ContractNumberDto>> GetContractNumberAsync();
        Task<IQueryable<EmployeeMasterDataDto>> GetEmployeeMasterDataAsync();
        Task<IQueryable<WithholdingTaxDto>> GetWithholdingTaxAsync();
        Task<IResult> CreateUpdateProgressPaymentAsync(ProgressPaymentCreateUpdateDto progressPaymentCreateDto);
        Task<IResult> DeleteProgressPaymentAsync(string key);
        Task<IResult> DuplicateProgressPaymentAsync(string docEn);
        Task<IQueryable<ApprovalUserDto>> GetApprovalUserForPozAsync();
        Task UpdateStateProgressPaymentAsync(string docEn, string value);
        Task<IResult> PurchaseInvoiceAsync(int ContractNo,int DocEntry);
        Task<bool> GetPurchaseInvoiceRecord(string docEn);
        Task<IQueryable<PurchaseInvoiceInfoDto>> GetPurchaseInvoiceInfo(string docEn);
        Task<IQueryable<PurchaseInvoiceLineInfoDto>> GetPurchaseInvoiceLineInfo(string docEn);
    }
}
