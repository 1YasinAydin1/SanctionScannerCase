using Eropa.Domain.Budgets;
using Eropa.Domain.SAPConnection;

namespace Eropa.Persistence.Budgets
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public BudgetRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IQueryable<Budget>> GetBudget(string DocEn = "")
        {
            return await _sapOperationsService.RunQuery<Budget>(
              $@"SELECT ROW_NUMBER() OVER (ORDER BY DocEntry) AS rowNum,DocEntry,U_BelgeNo,
                        U_BelgeTarihi,
                        U_BaslangicTarihi,
                        U_BitisTarihi,
                        U_SonlandirmaTarihi,
                        U_Durum,
                        U_Proje,
                        U_ProjeTanimi, 
                        U_Revizyon,
                        U_Aciklama,
                        U_UsdKur ,
                        U_EURKur ,
                        U_TemelUSDKur ,
                        U_TemelEURKur ,
                        U_ToplamTutar_TRY ,
                        U_ToplamTutar_USD ,
                        U_ToplamTutar_EUR ,
                        U_TemelToplamTutar_TRY ,
                        U_TemelToplamTutar_USD ,
                        U_TemelToplamTutar_EUR 
                        from ""@ERP_PRJ_BUTCE"" {(!string.IsNullOrEmpty(DocEn) ? $@" WHERE ""DocEntry"" = {DocEn} " : "")} ");
        }

        public async Task<IQueryable<CurrencyCurr>> GetCurrency()
        {
            return await _sapOperationsService.RunQuery<CurrencyCurr>(
                $@"SELECT Currency,Rate from ORTT WHERE RateDate = '{DateTime.Now.ToString("yyyyMMdd")}' and Currency IN('USD','EUR') order by Currency");
        }

        public async Task<string> GetDocumentNo()
        {
            var response = await _sapOperationsService.RunQuery<Budget>($@"SELECT TOP 1 ""U_BelgeNo"" FROM ""@ERP_PRJ_BUTCE"" ORDER BY ""U_BelgeNo"" DESC ");
            return (response.FirstOrDefault()?.U_BelgeNo+1 ?? 1).ToString();
        }

        public async Task<IQueryable<Project>> GetProject()
        {
            return await _sapOperationsService.RunQuery<Project>($@"SELECT PrjCode,PrjName from OPRJ");
        }

        public async Task<string> GetRevised(string project)
        {
            var result = await _sapOperationsService.RunQuery<Budget>($@"SELECT TOP 1 U_Revizyon from ""@ERP_PRJ_BUTCE"" Where U_Proje='{project}' order by DocEntry desc");
            return result.FirstOrDefault()?.U_Revizyon ?? "";
        }

    }
}
