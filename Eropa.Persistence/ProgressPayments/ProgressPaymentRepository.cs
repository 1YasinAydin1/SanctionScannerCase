using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;

namespace Eropa.Persistence.ProgressPayments
{
    public class ProgressPaymentRepository : IProgressPaymentRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public ProgressPaymentRepository(ISapOperationsService operationsService)
        {
            _sapOperationsService = operationsService;
        }

        public async Task<IQueryable<ProgressPayment>> GetProgressPaymentsAsync(string docEn = "")
        {
            return await _sapOperationsService.RunQuery<ProgressPayment>(
              $@"SELECT ROW_NUMBER() OVER (ORDER BY DocEntry) AS rowNum,
                         DocEntry,
                         U_HakedisNo,
                         U_SozlesmeNo,
                         U_SatRefNo,
                         U_HesapKesimTarihi,
                         U_Durum,
                         U_Proje,
                         U_Hakedis_baslangic,
                         U_Hakedis_bitis,
                         U_Sant_yuk_yetkili,
                         U_Sant_hak_sorumlu,
                         U_Sant_yetkilisi,
                         U_M_yetkili_1,
                         U_M_yetkili_2,
                         U_M_yetkili_3,
                         U_StopajKodu,
                         U_ToplamHakedis,
                         U_OncekiHakedis,
                         U_SimdikiHakedis,
                         U_TotHakAmnt,
                         U_OncHakAmnt,
                         U_SimHakAmnt
                         FROM ""@ERP_HAKEDIS""
                         {(!string.IsNullOrEmpty(docEn) ? $@" WHERE ""DocEntry"" = {docEn}" : "")}
");
        }
        public async Task<IQueryable<ContractNumber>> GetContractNumberAsync()
        {
            return await _sapOperationsService.RunQuery<ContractNumber>($@"
                        SELECT A. ""DocDate"", A. ""NumAtCard"", A. ""DocNum"", A. ""U_btcKonu"", MAX ( B. ""U_btcBelgeNo"" ) AS ""budgetID"" 
                                FROM ""OPOR"" A , ""POR1"" B 
                                WHERE A. ""DocEntry"" = B. ""DocEntry"" AND A. ""CANCELED"" = N'N' AND A. ""DocStatus"" = N'O' 
                               GROUP BY A. ""DocDate"" , A. ""NumAtCard"" , A. ""DocNum"" , A. ""U_btcKonu"" ORDER BY A. ""DocNum""");
        }
        public async Task<IQueryable<EmployeeMasterData>> GetEmployeeMasterDataAsync()
        {
            return await _sapOperationsService.RunQuery<EmployeeMasterData>($@"SELECT firstName,middleName,lastName,Code from OHEM");
        }
        public async Task<int> GetMaxProgressPaymentByContractNo(int contractNo)
        {
            var result = await _sapOperationsService.RunQuery<int?>($@"SELECT TOP 1 U_HakedisNo FROM ""@ERP_HAKEDIS"" WHERE Canceled!='Y' and U_SozlesmeNo={contractNo} order by U_HakedisNo desc");
            return (result.FirstOrDefault() ?? 0) + 1;
        }
        public async Task<IQueryable<WithholdingTax>> GetWithholdingTaxAsync()
        {
            return await _sapOperationsService.RunQuery<WithholdingTax>($@"SELECT WTCode,WTName FROM OWHT");
        }

        public async Task<IQueryable<ApprovalUser>> GetApprovalUserForPoz()
        {
            return await _sapOperationsService.RunQuery<ApprovalUser>($@"SELECT ""USER_CODE"" FROM OUSR WHERE ""USER_CODE"" = 'manager'");
        }

        public async Task<IQueryable<ApprovalUser>> UpdateStateProgressPayment(string docEn, string value)
        {
            return await _sapOperationsService.RunQuery<ApprovalUser>($@"UPDATE ""@ERP_HAKEDIS"" SET ""U_Durum"" = '{value}' WHERE ""DocEntry"" = {docEn}");
        }
        public async Task<SalesOrderInfo?> GetOrderInfoForPurchaseInvoice(string contractNo)
        {
           return (await _sapOperationsService.RunQuery<SalesOrderInfo>($@"SELECT ""CardCode"",""U_btcProje"",""NumAtCard"" FROM OPOR WHERE ""DocEntry"" = {contractNo}")).FirstOrDefault();
        }

        public async Task<IQueryable<ProgressPaymentInfo>> GetProgressPaymentForPurchaseInvoice(string DocEntry)
        {
            // Birim ??????
            return await _sapOperationsService.RunQuery<ProgressPaymentInfo>($@"SELECT 
                                                                                T0.""DocNum"",
                                                                                T1.""U_KalemKodu"",
                                                                                T1.""U_Tur"",
                                                                                T1.""U_PozNo"",
                                                                                T1.""U_Miktar"",
                                                                                T1.""U_BirimFiyat"",
                                                                                T1.""U_ParaBirimi"",
                                                                                T1.""U_AktiviteKodu"",
                                                                                T1.""U_AktiviteTanimi"",
                                                                                T1.""U_Kisim"",
                                                                                T1.""U_VergiKodu""
                                                                                FROM ""@ERP_HAKEDIS"" T0 LEFT JOIN ""@ERP_HAKEDISPOZ"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
                                                                                WHERE T0.""DocEntry"" = {DocEntry}");
        }
        public async Task<double?> GetPozLineForPurchaseInvoice(int DocEntry, string pozName)
        {
            return (await _sapOperationsService.RunQuery<double>($@"SELECT SUM(COALESCE(""U_Azi"",0))+SUM(COALESCE(""U_Cogu"", 0)) ""Calculation""
                                                                                FROM ""@ERP_HAKEDISPOZLINE"" 
                                                                                WHERE ""DocEntry"" = {DocEntry} AND ""U_PozAdi"" = '{pozName}'  GROUP BY ""U_PozAdi""")).FirstOrDefault(); 
        }
        public async Task<int?> GetPurchaseInvoiceRecord(string DocEntry)
        {
            return (await _sapOperationsService.RunQuery<int>($@"SELECT ""DocEntry"" FROM ODRF WHERE ""ObjType"" = 18 and  ""U_btcHakedis"" = '{DocEntry}'")).FirstOrDefault();
        }

        public async Task<ProgressPayment?> GetProgressPaymentPreviousTotal(string docEn)
        {
            return (await _sapOperationsService.RunQuery<ProgressPayment>($@"select U_ToplamHakedis,U_SimdikiHakedis,U_OncekiHakedis,U_TotHakAmnt,U_OncHakAmnt,U_SimHakAmnt from ""@ERP_HAKEDIS"" WHERE ""DocEntry"" = {docEn}")).FirstOrDefault();
        }
        public async Task<IQueryable<PurchaseInvoiceInfo>> GetPurchaseInvoiceInfo(string DocEntry)
        {
            return await _sapOperationsService.RunQuery<PurchaseInvoiceInfo>($@" SELECT T0.""DocEntry"",T0.DocCur,
		                                                                 T0.""CardCode"",
		                                                                 T0.""CardName"",
		                                                                 (SELECT ""Name"" from OCPR WHERE CntctCode = T0.CntctCode) ""CntctName"",
		                                                                 T0.""NumAtCard"",
		                                                                 T0.""DocDate"",
		                                                                 T0.""DocDueDate"",
		                                                                 T0.""TaxDate"",
		                                                                 FORMAT(T0.""DocTotal"", 'N') +' '+T0.DocCur ""DocTotal""
                                                                         FROM ODRF T0 WHERE ""U_btcHakedis"" = '{DocEntry}'");
        }
        public async Task<IQueryable<PurchaseInvoiceLineInfo>> GetPurchaseInvoiceLineInfo(string DocEntry)
        {
            return await _sapOperationsService.RunQuery<PurchaseInvoiceLineInfo>($@"  SELECT T0.LineNum+1 ""LineNum"",
                                                                          T0.""ItemCode"",
		                                                                  T0.""Dscription"",
		                                                                  FORMAT(T0.""Quantity"", 'N') ""Quantity"" ,
		                                                                  FORMAT(T0.""PriceBefDi"", 'N') +' '+T0.Currency ""PriceBefDi"",
		                                                                  T0.""VatGroup"",
		                                                                  FORMAT(T0.""LineTotal"", 'N') +' '+T0.Currency ""LineTotal"" 
                                                                          FROM DRF1 T0 WHERE DocEntry = {DocEntry}");
        }
    }
}
