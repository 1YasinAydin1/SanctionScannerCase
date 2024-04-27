namespace Eropa.Domain.ProgressPayments
{
    public class Poz
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string U_butceId { get; set; }
        public string? U_Tur { get; set; }
        public string? U_PozNo { get; set; }
        public string? U_KalemKodu { get; set; }
        public string? U_KalemTanimi { get; set; }
        public string? U_AktiviteKodu { get; set; }
        public string? U_AktiviteTanimi { get; set; }
        public string? U_Kisim { get; set; }
        public double? U_DonemHakedis { get; set; }
        public double? U_ToplamHakedis { get; set; }
        public double? U_OncekiHakedis { get; set; }
        public double? U_SimdikiHakedis { get; set; }
        public double? U_TotHakAmnt { get; set; }
        public double? U_OncHakAmnt { get; set; }
        public double? U_SimHakAmnt { get; set; }
        public double? U_Miktar { get; set; }
        public double? U_BirimFiyat { get; set; }
        public string? U_ParaBirimi { get; set; }
        public string? U_VergiKodu { get; set; }
    }
    public class PozLine
    {
        public int LineId { get; set; }
        public int DocEntry { get; set; }
        public string? U_Aciklama { get; set; }
        public string? U_Blok { get; set; }
        public string? U_Kat { get; set; }
        public double? U_Kot { get; set; }
        public string? U_Daire { get; set; }
        public string? U_Mahal { get; set; }
        public string? U_PozAdi { get; set; }
        public double? U_Ad { get; set; }
        public double? U_Benzer { get; set; }
        public double? U_En { get; set; }
        public double? U_Boy { get; set; }
        public double? U_Yukseklik { get; set; }
        public double? U_Azi { get; set; }
        public double? U_Cogu { get; set; }
        public int? U_TamamlanmaOrani { get; set; }
        //public double? U_DonemHakedis { get; set; }
        public int? U_ToplamHakedis { get; set; }
    }
}
