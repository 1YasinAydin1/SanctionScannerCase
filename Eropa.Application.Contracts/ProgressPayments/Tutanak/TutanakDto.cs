using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.ProgressPayments
{
    public class TutanakDto
    {
        [Key]
        public int? LineId { get; set; }
        public int? DocEntry { get; set; }
        public string? U_HesapKodu { get; set; }
        public int? U_faturalanackmi { get; set; }
        public string? U_FatDurumu { get; set; }
        public int? U_FatNo { get; set; }
        public string? U_BelgeTipi { get; set; }
        public string? U_Kodu { get; set; }
        public int? U_TutanakTipi { get; set; }
        public string? U_TutanakNo { get; set; }
        public DateTime? U_TutanakTarihi { get; set; }
        public double? U_Oran { get; set; }
        public double? U_Tutar { get; set; }
        public string? U_Tanimi { get; set; }
        public string? U_Icerik { get; set; }
    }
}
