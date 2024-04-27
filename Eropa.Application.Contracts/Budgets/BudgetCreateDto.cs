using Eropa.Application.Contracts.Budgets;

namespace Eropa.Application.Contracts.Budgets
{
    public class BudgetCreateDto
    {
        public DateTime? U_BaslangicTarihi { get; set; }
        public DateTime? U_BitisTarihi { get; set; }
        public string? U_BelgeNo { get; set; }
        public string? U_Durum { get; set; }
        public string? U_Proje { get; set; }
        public double? U_UsdKur { get; set; }
        public double? U_EURKur { get; set; }
        public string? U_Revizyon { get; set; }
        public string? U_Aciklama { get; set; }
        public double? U_TemelUSDKur { get; set; }
        public double? U_TemelEURKur { get; set; }
        public double? U_ToplamTutar_TRY { get; set; }
        public double? U_ToplamTutar_USD { get; set; }
        public double? U_ToplamTutar_EUR { get; set; }
        public double? U_TemelToplamTutar_TRY { get; set; }
        public double? U_TemelToplamTutar_USD { get; set; }
        public double? U_TemelToplamTutar_EUR { get; set; }
        public List<BudgetDetailCreateOrUpdateDto> ERP_PRJ_BUTCE_LINECollection { get; set; }
    }
}
