using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;

namespace Eropa.Persistence.ProgressPayments
{
    public class TutanakRepository : ITutanakRepository
    {
        private readonly ISapOperationsService _sapOperationsService;
        private string zero = "000";
        public TutanakRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IResult> DeleteTutanakLine(string docEn, string lineId)
        {
            await _sapOperationsService.RunQuery<string>($@"DELETE FROM ""@ERP_HAK_TUTANAK"" Where ""DocEntry"" = {docEn} and ""LineId"" = {lineId}");
            return new Result(true);
        }
        public async Task<TutanakNo> GetTutanakNoInfo(string docEn,string orderDocEn)
        {
            TutanakNo tutanakNo = new();
            tutanakNo.NumAtCard = (await _sapOperationsService.RunQuery<string>($@"SELECT ""NumAtCard"" FROM OPOR Where ""DocEntry"" = {orderDocEn}")).FirstOrDefault() ?? "";
            tutanakNo.Counter = (await _sapOperationsService.RunQuery<string>($@"select top 1 SUBSTRING(U_TutanakNo,LEN(U_TutanakNo)-2,LEN(U_TutanakNo)+1) 
                                                                                   from  ""@ERP_HAK_TUTANAK""  Where ""DocEntry"" = {docEn}
                                                                                   order by SUBSTRING(U_TutanakNo,LEN(U_TutanakNo)-2,LEN(U_TutanakNo)+1) desc")).FirstOrDefault() ?? "0";
            int co = int.Parse(tutanakNo.Counter);
            tutanakNo.Counter = $"{zero.Substring(0, zero.Length-co.ToString().Length)}{co+1}";
            return tutanakNo;
        }

        public async Task<IQueryable<Tutanak>> GetTutanakAsync(string docEn)
        {
            return await _sapOperationsService.RunQuery<Tutanak>($@"select  ""LineId"",
		                                                             ""U_TutanakTipi"",
		                                                             ""U_TutanakNo"",
		                                                             ""U_Kodu"",
		                                                             ""U_Tanimi"",
		                                                             ""U_TutanakTarihi"",
		                                                             ""U_Oran"",
		                                                             ""U_Tutar"",
		                                                             ""U_HesapKodu"",
		                                                             ""U_Icerik"",
		                                                             ""U_faturalanackmi"",
		                                                             ""U_FatDurumu"",
		                                                             ""U_BelgeTipi"",
		                                                             ""U_FatNo"" from  ""@ERP_HAK_TUTANAK"" WHERE ""DocEntry"" = {docEn}");
        }

        public async Task<SalesOrderInfo?> GetConnBPAsync(string docEn)
        {
            return (await _sapOperationsService.RunQuery<SalesOrderInfo>($@"select T1.ConnBP,T0.U_btcProje from OPOR T0 JOIN OCRD T1 ON T0.""CardCode"" = T1.""CardCode"" WHERE T0.""DocEntry"" = {docEn}")).FirstOrDefault();
        }

        public async Task<IQueryable<Tutanak>> GetTutanakForInvoiceAsync(string docEn, string lineIds)
        {
            return await _sapOperationsService.RunQuery<Tutanak>($@"select ""LineId"",""U_HesapKodu"",""U_Icerik"",""U_Tutar"",""U_TutanakNo"" from ""@ERP_HAK_TUTANAK"" WHERE COALESCE(""U_FatNo"",'')='' AND ""DocEntry"" = {docEn} AND LineId IN ({lineIds}) and U_faturalanackmi = 1");
        }
        public async Task<IResult> UpdateTutanakForInvoiceAsync(string invoiceDocEn, int DocEntry, string lineIds)
        {
            await _sapOperationsService.RunQuery<string>($@"UPDATE ""@ERP_HAK_TUTANAK"" SET ""U_BelgeTipi"" = 'Taslak', ""U_FatNo"" = {invoiceDocEn} WHERE ""DocEntry"" = {DocEntry} AND ""LineId"" IN ({lineIds}) AND COALESCE(""U_FatNo"",'')='' AND U_faturalanackmi = 1");
            return new Result(true);
        }

        public async Task<IQueryable<InvoiceInfo>> GetInvoiceInfo(string DocEntry, string LineId)
        {
            return await _sapOperationsService.RunQuery<InvoiceInfo>($@" SELECT T0.""DocEntry"",T0.DocCur,
		                                                                 T0.""CardCode"",
		                                                                 T0.""CardName"",
		                                                                 (SELECT ""Name"" from OCPR WHERE CntctCode = T0.CntctCode) ""CntctName"",
		                                                                 T0.""NumAtCard"",
		                                                                 T0.""DocDate"",
		                                                                 T0.""DocDueDate"",
		                                                                 T0.""TaxDate"",
		                                                                 FORMAT(T0.""DocTotal"", 'N') +' '+T0.DocCur ""DocTotal""
                                                                         FROM ODRF T0 WHERE ""DocEntry"" = (SELECT ""DocEntry"" FROM DRF1 WHERE ""U_btcHakedis"" = '{DocEntry}' AND ""U_btcTutline"" = {LineId})");
        }
        public async Task<IQueryable<InvoiceLineInfo>> GetInvoiceLineInfo(string DocEntry)
        {
            return await _sapOperationsService.RunQuery<InvoiceLineInfo>($@"  SELECT T0.LineNum+1 ""LineNum"",
                                                                          T0.""AcctCode"" ""ItemCode"",
		                                                                  T0.""Dscription"",
		                                                                  T0.""VatGroup"",
		                                                                  FORMAT(T0.""Quantity"", 'N') ""Quantity"" ,
		                                                                  FORMAT(T0.""PriceBefDi"", 'N') +' '+T0.Currency ""PriceBefDi"",
		                                                                  FORMAT(T0.""LineTotal"", 'N') +' '+T0.Currency ""LineTotal"" 
                                                                          FROM DRF1 T0 WHERE DocEntry = {DocEntry}");
        }
    }
}
