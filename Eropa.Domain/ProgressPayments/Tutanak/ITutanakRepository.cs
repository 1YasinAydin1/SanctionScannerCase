using Eropa.Helper.Results;

namespace Eropa.Domain.ProgressPayments
{
    public interface ITutanakRepository
    {
        Task<IQueryable<Tutanak>> GetTutanakAsync(string docEn);
        Task<IResult> DeleteTutanakLine(string docEn, string lineId);
        Task<TutanakNo> GetTutanakNoInfo(string docEn, string orderDocEn);
        Task<SalesOrderInfo?> GetConnBPAsync(string docEn);
        Task<IQueryable<Tutanak>> GetTutanakForInvoiceAsync(string docEn, string lineIds);
        Task<IResult> UpdateTutanakForInvoiceAsync(string invoiceDocEn, int DocEntry, string lineIds);
        Task<IQueryable<InvoiceInfo>> GetInvoiceInfo(string DocEntry, string LineId);
        Task<IQueryable<InvoiceLineInfo>> GetInvoiceLineInfo(string DocEntry);
    }
}
