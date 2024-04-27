namespace Eropa.Domain.ProgressPayments
{
    public interface IProgressPaymentRepository
    {
        Task<IQueryable<ProgressPayment>> GetProgressPaymentsAsync(string docEn = "");
        Task<IQueryable<ContractNumber>> GetContractNumberAsync();
        Task<IQueryable<EmployeeMasterData>> GetEmployeeMasterDataAsync();
        Task<IQueryable<WithholdingTax>> GetWithholdingTaxAsync();
        Task<int> GetMaxProgressPaymentByContractNo(int contractNo);
        Task<IQueryable<ApprovalUser>> GetApprovalUserForPoz();
        Task<IQueryable<ApprovalUser>> UpdateStateProgressPayment(string docEn, string value);
        Task<SalesOrderInfo?> GetOrderInfoForPurchaseInvoice(string contractNo); 
        Task<IQueryable<ProgressPaymentInfo>> GetProgressPaymentForPurchaseInvoice(string DocEntry);
        Task<double?> GetPozLineForPurchaseInvoice(int DocEntry, string pozName);
        Task<int?> GetPurchaseInvoiceRecord(string DocEntry);
        Task<IQueryable<PurchaseInvoiceInfo>> GetPurchaseInvoiceInfo(string DocEntry);
        Task<IQueryable<PurchaseInvoiceLineInfo>> GetPurchaseInvoiceLineInfo(string DocEntry);
        Task<ProgressPayment?> GetProgressPaymentPreviousTotal(string docEn);
    }
}
