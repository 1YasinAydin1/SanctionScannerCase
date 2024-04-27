namespace Eropa.Domain.ProgressPayments
{

    public class PurchaseInvoiceInfo
    {
        public int DocEntry { get; set; }
        public string? DocCur { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? CntctName { get; set; }
        public string? NumAtCard { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public DateTime? TaxDate { get; set; }
        public string? DocTotal { get; set; }
    }

    public class PurchaseInvoiceLineInfo
    {
        public int LineNum { get; set; }
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? Quantity { get; set; }
        public string? PriceBefDi { get; set; }
        public string? VatGroup { get; set; }
        public string? LineTotal { get; set; }
    }
}
