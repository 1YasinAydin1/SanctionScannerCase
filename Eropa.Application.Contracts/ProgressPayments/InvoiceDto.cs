namespace Eropa.Application.Contracts.ProgressPayments
{
    public class InvoiceDto
    {
        public string CardCode { get; set; }
        public List<InvoiceLineDto> DocumentLines { get; set; }
        public string DocType { get; set; } = "dDocument_Service";
        public DateTime DocDate { get; set; } = DateTime.Today;
        public string U_btcHakedis { get; set; }
        public string DocObjectCode { get; set; } = "oInvoices";
    }
    public class InvoiceLineDto
    {
        public string? AccountCode { get; set; }
        public string? ItemDescription { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double LineTotal { get; set; }
        public string? U_btcProje { get; set; }

    }
    public class LineHolder
    {
        public int LineNo;
        public int btcTutLine;
    }
}
