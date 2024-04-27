namespace Eropa.Application.Contracts.Budgets
{
    public class BudgetReportDto
    {
        public string U_AktiviteKodu { get; set; }
        public int rowNum { get; set; }
        public string U_AktiviteAdi { get; set; }
        public string U_KalemKodu { get; set; }
        public string U_Birim { get; set; }
        public string kisim { get; set; }
        public string U_BirimFiyatReferans { get; set; }
        public string U_ParaBirimi { get; set; }
        public double Kur { get; set; }
        public double BirimFiyat { get; set; }
        public double Miktar { get; set; }
        public double ToplamTutar { get; set; }
        public double ToplamTutar_TRY { get; set; }
        public double ToplamTutar_USD { get; set; }
        public double ToplamTutar_EUR { get; set; }
        public double TemelBirimFiyat { get; set; }
        public double TemelMiktar { get; set; }
        public double TemelToplamTutar_TRY { get; set; }
        public double TemelToplamTutar_USD { get; set; }
        public double TemelToplamTutar_EUR { get; set; }
        public double FarkTRY { get; set; }
        public double GrcMiktar { get; set; }
        public double GrcOrtTRYBirimFiyat { get; set; }
        public double GrcToplamTutar_TRY { get; set; }
        public double GrcToplamTutar_USD { get; set; }
        public double GrcToplamTutar_EUR { get; set; }
        public double TamamlanmaOrani { get; set; }
        public double Kalan { get; set; }
        public double Seviye { get; set; }
    }
}
