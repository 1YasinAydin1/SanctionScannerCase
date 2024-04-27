namespace Eropa.Application.Contracts.Activities
{
    public class ActivityCreateDto
    {
        public string? Code { get; set; }
        public string U_AktiviteKodu { get; set; }
        public string? U_AktiviteTanimi { get; set; }
        public string? U_UstAktiviteKodu { get; set; }
        public string? U_Durum { get; set; }
        public string? U_Seviye { get; set; }
    }
}
