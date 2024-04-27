namespace Eropa.Domain.Budgets
{
    public class BudgetDetail
    {
        public string rowNum { get; set; }
        public int LineId { get; set; }
        public string? U_Seviye { get; set; }
        public string? U_AktiviteKodu { get; set; }
        public string? U_UstAktiviteKodu { get; set; }
        public string? U_AktiviteAdi { get; set; }
        public string? U_KalemKodu { get; set; }
        public string? U_Birim { get; set; }
        public string? U_kisim { get; set; }
        public string? U_BirimFiyatReferans { get; set; }
        public string? U_ParaBirimi { get; set; }
        public double? U_Kur { get; set; }
        public double? U_BirimFiyat { get; set; }
        public double? U_Miktar { get; set; }
        public double? U_ToplamTutar { get; set; }
        public double? U_ToplamTutar_TRY { get; set; }
        public double? U_ToplamTutar_USD { get; set; }
        public double? U_ToplamTutar_EUR { get; set; }
        public double? U_TemelBirimFiyat { get; set; }
        public double? U_TemelMiktar { get; set; }
        public double? U_TemelToplamTutar_TRY { get; set; }
        public double? U_TemelToplamTutar_USD { get; set; }
        public double? U_TemelToplamTutar_EUR { get; set; }
        public DateTime? U_planbaslangic { get; set; }
        public DateTime? U_planbitis { get; set; }
        public DateTime? U_baslangic { get; set; }
        public DateTime? U_bitis { get; set; }
        public double? U_fark { get; set; }
    }
}
