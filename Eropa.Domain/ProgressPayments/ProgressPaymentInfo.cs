namespace Eropa.Domain.ProgressPayments
{
    public class ProgressPaymentInfo
    {
        public int? DocNum { get; set; }
        public int? U_Tur { get; set; }
        public string? U_KalemKodu { get; set; }
        public string? U_PozNo { get; set; }
        public double? U_Miktar { get; set; }
        public double? U_BirimFiyat { get; set; }
        public string? U_ParaBirimi { get; set; }
        public string? U_VergiKodu { get; set; }
        public string? U_AktiviteKodu { get; set; }
        public string? U_AktiviteTanimi { get; set; }
        public string? U_Kisim { get; set; }
    }
    public class PozLineInfo
    {
        public double? U_Azi { get; set; }
        public double? U_Cogu { get; set; }
    }
}
