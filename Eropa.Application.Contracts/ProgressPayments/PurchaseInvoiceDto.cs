using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
   
    public class PurchaseInvoiceDto
    {
        public string CardCode { get; set; }
        public string U_btcHakedis { get; set; }
        public string NumAtCard { get; set; }
        public string DocObjectCode { get; set; } = "oPurchaseInvoices";
        public List<PurchaseInvoiceLineDto> DocumentLines { get; set; }
    }

    public class PurchaseInvoiceLineDto
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string? MeasureUnit { get; set; }
        public string? Currency { get; set; }
        public string? U_btcProje { get; set; }
        public string? U_btcAktiviteKodu { get; set; }
        public string? U_btcAktiviteTanimi { get; set; }
        public string? U_btcKisim { get; set; }
        public string? U_btcBelgeNo { get; set; }
    }

    public class PurchaseInvoiceInfoDto
    {
        [Key]
        public int DocEntry { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? CntctName { get; set; }
        public string? NumAtCard { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public DateTime? TaxDate { get; set; }
        public string? DocTotal { get; set; }
        public List<PurchaseInvoiceLineInfoDto> details { get; set; }
    }

    public class PurchaseInvoiceLineInfoDto
    {
        [Key]
        public int LineNum { get; set; }
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public string? Quantity { get; set; }
        public string? PriceBefDi { get; set; }
        public string? VatGroup { get; set; }
        public string? LineTotal { get; set; }
    }
}
