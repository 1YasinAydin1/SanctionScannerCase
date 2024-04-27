using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Activities
{
    public class ActivityDto
    {
        [Key]
        public string Code { get; set; }
        public int rowNum { get; set; }
        public string? U_AktiviteKodu { get; set; }
        public string? U_AktiviteTanimi { get; set; }
        public string? U_UstAktiviteKodu { get; set; }
        public string? U_Durum { get; set; }
        public string? U_Seviye { get; set; }
    }
}
