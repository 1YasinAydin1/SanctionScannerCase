using Eropa.Domain.Budgets;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;

namespace Eropa.Persistence.Budgets
{
    public class BudgetDetailRepository : IBudgetDetailRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public BudgetDetailRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IQueryable<BudgetDetail>> GetBudgetDetail(string docEn)
        {
            return await _sapOperationsService.RunQuery<BudgetDetail>($@"SELECT  Case T1.""U_Seviye""
                                                                        WHEN 1 THEN T1.""U_AktiviteKodu""
                                                                        WHEN 2 THEN T1.""U_AktiviteKodu"" + COALESCE(T0.""U_kisim"", '')
                                                                        WHEN 3 THEN COALESCE(T1.""U_UstAktiviteKodu"", '') + COALESCE(T0.""U_kisim"", '') + T1.""U_AktiviteKodu"" end AS rowNum,
                                                                        CASE T1.U_Seviye 
																		WHEN 1 THEN COALESCE(T1.""U_UstAktiviteKodu"",'-1')+ COALESCE(T0.""U_kisim"", '')
																		WHEN 3 THEN COALESCE(T1.""U_UstAktiviteKodu"",'-1')+ COALESCE(T0.""U_kisim"", '')
																		WHEN 2 THEN COALESCE(T1.""U_UstAktiviteKodu"",'-1') end ""U_UstAktiviteKodu"", {getQuery},T1.""U_Seviye"" from ""@ERP_PRJ_BUTCE_LINE"" T0
                                                                         JOIN  ""@ERP_PROJ_ACT"" T1 ON T0.""U_AktiviteKodu""=T1.""U_AktiviteKodu"" where T0.""DocEntry"" = {docEn}
                                                                        ORDER BY Case T1.""U_Seviye"" 
                                                                        WHEN 1 THEN T1.""U_AktiviteKodu"" 
                                                                        WHEN 2 THEN T1.""U_AktiviteKodu""+COALESCE(T0.""U_kisim"",'') 
                                                                        WHEN 3 THEN COALESCE(T1.""U_UstAktiviteKodu"",'')+COALESCE(T0.""U_kisim"",'')+T1.""U_AktiviteKodu"" END");

        }

        public async Task<IQueryable<BudgetDetail>> GetSingleBudgetDetail(string docEn, string lineId)
        {
            return await _sapOperationsService.RunQuery<BudgetDetail>($@"SELECT {getQuery} from ""@ERP_PRJ_BUTCE_LINE"" T0 
                                                                        Where T0.""DocEntry"" = {docEn} and T0.""LineId"" = {lineId}");
        }
        public async Task<string> GetLineIdByRowNum(string docEn, string key)
        {
            var result = await _sapOperationsService.RunQuery<BudgetDetail>($@"SELECT T0.""LineId""  from ""@ERP_PRJ_BUTCE_LINE"" T0
                                                                         JOIN  ""@ERP_PROJ_ACT"" T1 ON T0.""U_AktiviteKodu""=T1.""U_AktiviteKodu"" where T0.""DocEntry"" =  {docEn} and 
																		 Case T1.""U_Seviye""
                                                                        WHEN 1 THEN T1.""U_AktiviteKodu""
                                                                        WHEN 2 THEN T1.""U_AktiviteKodu"" + COALESCE(T0.""U_kisim"", '')
                                                                        WHEN 3 THEN COALESCE(T1.""U_UstAktiviteKodu"", '') + COALESCE(T0.""U_kisim"", '') + T1.""U_AktiviteKodu"" end = '{key}'");
            return result.FirstOrDefault() != null ? result.First().LineId.ToString() : "0";
        }

        public async Task<IQueryable<BudgetDetail>> GetSingleBudgetDetailByActivityCode(string docEn, string ActivityCode, string part = "")
        {
            return await _sapOperationsService.RunQuery<BudgetDetail>($@"SELECT {getQuery} from ""@ERP_PRJ_BUTCE_LINE"" T0 
                                                                        Where T0.""DocEntry"" = {docEn} and T0.""U_AktiviteKodu"" = '{ActivityCode}'  
                                                                        {(!string.IsNullOrEmpty(part) ? $@"and T0.""U_kisim"" = '{part}'" : "")} ");
        }

        public async Task<IResult> DeleteBudgetDetail(string docEn, string lineId = "")
        {
            await _sapOperationsService.RunQuery<string>($@"DELETE FROM ""@ERP_PRJ_BUTCE_LINE"" Where ""DocEntry"" = {docEn}
                                                                                {(!string.IsNullOrEmpty(lineId) ? $@" and ""LineId"" = {lineId}" : "")}");
            return new Result(true);
        }

        public async Task<IQueryable<GetItem>> GetItemsForBudgetDetail()
        {
            return await _sapOperationsService.RunQuery<GetItem>($@"SELECT ""ItemCode"",""ItemName"" FROM OITM");
        }

        public async Task<IQueryable<GetMeasurementUnit>> GetMeasurementUnitsForBudgetDetail()
        {
            return await _sapOperationsService.RunQuery<GetMeasurementUnit>($@"SELECT ""UomCode"",""UomName"" FROM OUOM");
        }

        public async Task<IQueryable<GetCurrency>> GetCurrencysForBudgetDetail()
        {
            return await _sapOperationsService.RunQuery<GetCurrency>($@"SELECT ""CurrCode"",""CurrName"" FROM OCRN");
        }

        //public async Task<IQueryable<GetCurrency>> UpdateUsdEurTotal(string DocEntry,string lineId,double amount)
        //{
        //    return await _sapOperationsService.RunQuery<GetCurrency>($@"UPDATE ""@ERP_PRJ_BUTCE_LINE"" SET ""U_ToplamTutar_USD"" = {amount} WHERE ""DocEntry");
        //}

        const string getQuery = $@"   T0.""LineId"",
                                            T0.""U_AktiviteKodu"",
                                            T0.""U_AktiviteAdi"",
                                            T0.""U_KalemKodu"",
                                            T0.""U_Birim"",
                                            T0.""U_kisim"",
                                            T0.""U_BirimFiyatReferans"",
                                            T0.""U_ParaBirimi"",
                                            T0.""U_Kur"",
                                            T0.""U_BirimFiyat"",
                                            T0.""U_Miktar"",
                                            T0.""U_BirimFiyat"",
                                            T0.""U_ToplamTutar"",
                                            T0.""U_ToplamTutar_TRY"",
                                            T0.""U_ToplamTutar_USD"",
                                            T0.""U_ToplamTutar_EUR"",
                                            T0.""U_TemelBirimFiyat"",
                                            T0.""U_TemelMiktar"",
                                            T0.""U_TemelToplamTutar_TRY"",
                                            T0.""U_TemelToplamTutar_USD"",
                                            T0.""U_TemelToplamTutar_EUR"",
                                            T0.""U_planbaslangic"",
                                            T0.""U_planbitis"",
                                            T0.""U_baslangic"",
                                            T0.""U_bitis"",
                                            T0.""U_fark""  ";
    }
}
