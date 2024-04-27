using Eropa.Domain.Budgets;
using Eropa.Domain.SAPConnection;

namespace Eropa.Persistence.Budgets
{
    public class BudgetReportRepository : IBudgetReportRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public BudgetReportRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IQueryable<BudgetReport>> GetBudgetReport(BudgetReportFilter budgetReportFilter)
        {
            return await _sapOperationsService.RunQuery<BudgetReport>(string.Format(query, 
                 budgetReportFilter.Budget, 
                !string.IsNullOrEmpty(budgetReportFilter.Project)? $"and A.U_Proje LIKE '{budgetReportFilter.Project}'" : "",
                !string.IsNullOrEmpty(budgetReportFilter.Part) ? budgetReportFilter.Part : "Hayır",
                budgetReportFilter.USDCurr, budgetReportFilter.EURCurr,
                !string.IsNullOrEmpty(budgetReportFilter.Budget) ? "1=1" : "1!=1", string.IsNullOrEmpty(budgetReportFilter.Budget) ? "1=1" : "1!=1"));
        }


        private string query = @"
IF 'Hayır' = '{2}'

       SELECT ROW_NUMBER() OVER (ORDER BY MIN(A.VisOrder)) AS rowNum,A.U_AktiviteKodu,

       A.U_AktiviteAdi,

       A.U_KalemKodu,

       A.U_Birim,

       '' as kisim,

       U_BirimFiyatReferans,

       A.U_ParaBirimi,

       ISNULL(AVG(A.U_Kur),0) as ""Kur"",

       ISNULL(AVG(A.U_BirimFiyat),0) as ""BirimFiyat"",

       ISNULL(SUM(A.U_Miktar),0) as ""Miktar"",

       ISNULL(SUM(A.U_ToplamTutar),0) as ""ToplamTutar"",

       ISNULL(SUM(A.U_ToplamTutar_TRY),0) as ""ToplamTutar_TRY"",

       ISNULL(SUM(A.U_ToplamTutar_USD),0) as ""ToplamTutar_USD"",

       ISNULL(SUM(A.U_ToplamTutar_EUR),0) as ""ToplamTutar_EUR"",

       ISNULL(AVG(A.U_TemelBirimFiyat),0) as ""TemelBirimFiyat"",

       ISNULL(SUM(A.U_TemelMiktar),0) as ""TemelMiktar"",

       ISNULL(SUM(A.U_TemelToplamTutar_TRY),0) as ""TemelToplamTutar_TRY"",

       ISNULL(SUM(A.U_TemelToplamTutar_USD),0) as ""TemelToplamTutar_USD"",

       ISNULL(SUM(A.U_TemelToplamTutar_EUR),0) as ""TemelToplamTutar_EUR"",

       ISNULL(SUM(A.U_Fark),0) as ""FarkTRY"",

       ((select ISNULL(sum(P.Quantity),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.Quantity),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu)) as ""GrcMiktar"",

       (case when ((select ISNULL(sum(P.Quantity),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.Quantity),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu)) = 0 then 0.0 else

       (((select ISNULL(sum(P.LineTotal),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu))

       /

       ((select ISNULL(sum(P.Quantity),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.Quantity),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu))) end) as ""GrcOrtTRYBirimFiyat"",

       ((select ISNULL(sum(P.LineTotal),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu)) as ""GrcToplamTutar_TRY"",

       (case when {3} --@USDKUR 
	   = 0 then 0.0 else ((select ISNULL(sum(P.LineTotal),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu)) * {3}--@USDKUR 
	   end) as ""GrcToplamTutar_USD"",

       (case when  {4}--@EURKUR 
	   = 0 then 0.0 else ((select ISNULL(sum(P.LineTotal),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu)) *  {4}--@EURKUR 
	   end) as ""GrcToplamTutar_EUR"",

       (case when ISNULL(SUM(A.U_ToplamTutar_TRY),0) = 0 then 0.0 else ((select ISNULL(sum(P.LineTotal),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu))

       /ISNULL(SUM(A.U_ToplamTutar_TRY),0) end) as ""TamamlanmaOrani"",

       (ISNULL(SUM(A.U_ToplamTutar_TRY),0) - ((select ISNULL(sum(P.LineTotal),0) from PCH1 P where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu))) as ""Kalan"",

       ISNULL(MIN(A.U_Seviye),0) as ""Seviye""

       from ""@ERP_PRJ_BUTCE_LINE"" A,""@ERP_PRJ_BUTCE"" B

       WHERE A.DocEntry = B.DocEntry

       and (({5} --@BELGEDURUMU 
	   and  A.U_BelgeNo = '{0}'
	   ) or ({6}--@BELGEDURUMU 
	   and B.U_Durum !='2' ))

      {1}

       GROUP BY A.U_BelgeNo,A.U_AktiviteKodu, A.U_AktiviteAdi, A.U_KalemKodu, A.U_Birim, U_BirimFiyatReferans, A.U_ParaBirimi

       ORDER BY MIN(A.VisOrder)

 

ELSE

       SELECT ROW_NUMBER() OVER (ORDER BY A.VisOrder) AS rowNum,
A.U_AktiviteKodu ,

       A.U_AktiviteAdi,

       A.U_KalemKodu,

       A.U_Birim,

       U_kisim as kisim,

       U_BirimFiyatReferans,

       A.U_ParaBirimi,

       A.U_Kur as ""Kur"",

       A.U_BirimFiyat as ""BirimFiyat"",

       A.U_Miktar as ""Miktar"",

       A.U_ToplamTutar as ""ToplamTutar"",

       A.U_ToplamTutar_TRY as ""ToplamTutar_TRY"",

       A.U_ToplamTutar_USD as ""ToplamTutar_USD"",

       A.U_ToplamTutar_EUR as ""ToplamTutar_EUR"",

       A.U_TemelBirimFiyat as ""TemelBirimFiyat"",

       A.U_TemelMiktar as ""TemelMiktar"",

       A.U_TemelToplamTutar_TRY as ""TemelToplamTutar_TRY"",

       A.U_TemelToplamTutar_USD as ""TemelToplamTutar_USD"",

       A.U_TemelToplamTutar_EUR as ""TemelToplamTutar_EUR"",

       A.U_Fark as ""FarkTRY"",

       ((select ISNULL(sum(P.Quantity),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and COALESCE(P.U_btcKisim,'') = COALESCE(A.U_kisim,''))-

       (select ISNULL(sum(R.Quantity),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and COALESCE(R.U_btcKisim,'') = COALESCE(A.U_kisim,''))) as ""GrcMiktar"",

       (case when ((select ISNULL(sum(P.Quantity),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and COALESCE(P.U_btcKisim,'') = COALESCE(A.U_kisim,''))-

       (select ISNULL(sum(R.Quantity),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and COALESCE(R.U_btcKisim,'') = COALESCE(A.U_kisim,''))) = 0 then 0.0 else

       (((select ISNULL(sum(P.LineTotal),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim))/((select ISNULL(sum(P.Quantity),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.Quantity),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim))) end) as ""GrcOrtTRYBirimFiyat"",

       ((select ISNULL(sum(P.LineTotal),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim))  as ""GrcToplamTutar_TRY"",

       (case when {3}--@USDKUR 
	   = 0 then 0.0 else ((select ISNULL(sum(P.LineTotal),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim)) * {3}--@USDKUR 
	   end) as ""GrcToplamTutar_USD"",

       (case when  {4}--@EURKUR 
	   = 0 then 0.0 else ((select ISNULL(sum(P.LineTotal),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim)) *  {4}--@EURKUR 
	   end) as ""GrcToplamTutar_EUR"",

       (case when A.U_ToplamTutar_TRY = 0 then 0.0 else (((select ISNULL(sum(P.LineTotal),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim)) /A.U_ToplamTutar_TRY) end) as ""TamamlanmaOrani"",

       (A.U_ToplamTutar_TRY - ((select ISNULL(sum(P.LineTotal),0) from PCH1 P

       where P.U_btcBelgeNo = A.U_BelgeNo and P.U_btcAktiviteKodu = A.U_AktiviteKodu and P.U_btcKisim = A.U_kisim)-

       (select ISNULL(sum(R.LineTotal),0) from RPC1 R

       where R.U_btcBelgeNo = A.U_BelgeNo and R.U_btcAktiviteKodu = A.U_AktiviteKodu and R.U_btcKisim = A.U_kisim))) as ""Kalan"",

       A.U_Seviye as ""Seviye""

       from ""@ERP_PRJ_BUTCE_LINE"" A,""@ERP_PRJ_BUTCE"" B

       WHERE A.DocEntry = B.DocEntry

       and (({5}-- @BELGEDURUMU 
	   and  A.U_BelgeNo = '{0}'
	   ) or ({6}--@BELGEDURUMU 
	   and B.U_Durum !='2' ))

       {1}

       ORDER BY A.VisOrder
";


    }


}
