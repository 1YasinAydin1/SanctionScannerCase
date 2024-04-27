using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class ProgressPaymentDto
    {
        [Key]
        public int? DocEntry { get; set; }
        public int rowNum { get; set; }
        public int? U_HakedisNo { get; set; }
        public int? U_SozlesmeNo { get; set; }
        public string? U_SatRefNo { get; set; }
        public DateTime? U_HesapKesimTarihi { get; set; }
        public string? U_Durum { get; set; }
        public string? U_Proje { get; set; }
        public DateTime? U_Hakedis_baslangic { get; set; }
        public DateTime? U_Hakedis_bitis { get; set; }
        public string? U_Sant_yuk_yetkili { get; set; }
        public string? U_Sant_hak_sorumlu { get; set; }
        public string? U_Sant_yetkilisi { get; set; }
        public string? U_M_yetkili_1 { get; set; }
        public string? U_M_yetkili_2 { get; set; }
        public string? U_M_yetkili_3 { get; set; }
        public string? U_StopajKodu { get; set; }
        public double? U_ToplamHakedis { get; set; }
        public double? U_OncekiHakedis { get; set; }
        public double? U_SimdikiHakedis { get; set; }
        public double? U_TotHakAmnt { get; set; }
        public double? U_OncHakAmnt { get; set; }
        public double? U_SimHakAmnt { get; set; }
    }
}
