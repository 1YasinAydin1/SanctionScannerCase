using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;

namespace Eropa.Persistence.ProgressPayments
{
    public class PozRepository : IPozRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public PozRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IQueryable<SelectPoz>> GetSelectPozAsync(string contractNo)
        {
            string key = "";
            if (contractNo.Contains("("))
            {
                var strings = contractNo.Split('(');
                contractNo = strings[0];
                key = strings[1].Substring(0, strings[1].Length - 1);
            }
            return await _sapOperationsService.RunQuery<SelectPoz>($@" 
                                        SELECT B. ""LineNum"", 
                                        CONCAT( CASE WHEN ""LineNum"" <10 THEN 'POZ00' WHEN ""LineNum"" >10 AND ""LineNum"" <100 THEN 'POZ0' WHEN ""LineNum"" >100 THEN 'POZ' END , B. ""LineNum"" ) AS ""PozLineNum"" ,
                                        B. ""U_btcBelgeNo"" AS ""budgetId"" , B. ""ItemCode"" , B. ""Dscription"" , B. ""U_btcAktiviteKodu"" AS ""activityCode"" ,
                                        B. ""U_btcAktiviteTanimi"" AS ""activityName"" , B. ""U_btcKisim"" AS ""kisim"" ,B.""Quantity"",B.""Price"",B.""Currency"",B.""VatGroup""
                                        FROM ""OPOR"" A , ""POR1"" B 
                                        WHERE A. ""DocEntry"" = B. ""DocEntry"" AND A. ""DocNum"" = N'{contractNo}' 
                                        {(!string.IsNullOrEmpty(key) ?
                                            $@" AND  CONCAT( CASE WHEN ""LineNum"" <10 THEN 'POZ00' WHEN ""LineNum"" >10 AND ""LineNum"" <100 
                                            THEN 'POZ0' WHEN ""LineNum"" >100 THEN 'POZ' END , B. ""LineNum"" ) = {key}" : "")} AND A. ""CANCELED"" = N'N' AND A. ""DocStatus"" = N'O'");
        }
        public async Task<IQueryable<GetItemProgressPayment>> GetItemsForProgressPayment()
        {
            return await _sapOperationsService.RunQuery<GetItemProgressPayment>($@"SELECT ""ItemCode"",""ItemName"" FROM OITM");
        }
        public async Task<IResult> DeletePozLine(string docEn, string lineId)
        {
            var result = await _sapOperationsService.RunQuery<PozLine>($@"SELECT ""U_ToplamHakedis"",""U_PozAdi"" FROM ""@ERP_HAKEDISPOZLINE"" Where ""DocEntry"" = {docEn} and ""LineId"" = {lineId}");
            await _sapOperationsService.RunQuery<string>($@"DELETE FROM ""@ERP_HAKEDISPOZLINE"" Where ""DocEntry"" = {docEn} and ""LineId"" = {lineId}");
            await _sapOperationsService.RunQuery<string>($@"UPDATE ""@ERP_HAKEDISPOZ"" SET U_ToplamHakedis=U_ToplamHakedis-{result.First().U_ToplamHakedis},
                                                                                           U_SimdikiHakedis=U_SimdikiHakedis-{result.First().U_ToplamHakedis} Where ""DocEntry"" = {docEn} and ""U_PozNo"" = '{result.First().U_PozAdi}'");
            await _sapOperationsService.RunQuery<string>($@"UPDATE ""@ERP_HAKEDIS"" SET U_ToplamHakedis=U_ToplamHakedis-{result.First().U_ToplamHakedis},
                                                                                        U_SimdikiHakedis=U_SimdikiHakedis-{result.First().U_ToplamHakedis} Where ""DocEntry"" = {docEn}");
            return new Result(true);
        }
        public async Task<IDataResult<ProgressPayment>> DeletePoz(string docEn, string lineId)
        {
            var result = await _sapOperationsService.RunQuery<Poz>($@"SELECT ""U_ToplamHakedis"",""U_PozNo"" FROM ""@ERP_HAKEDISPOZ"" Where ""DocEntry"" = {docEn} and ""LineId"" = {lineId}");
            await _sapOperationsService.RunQuery<string>($@"DELETE FROM ""@ERP_HAKEDISPOZ"" Where ""DocEntry"" = {docEn} and ""LineId"" = {lineId}");
            await _sapOperationsService.RunQuery<string>($@"DELETE FROM ""@ERP_HAKEDISPOZLINE"" Where ""DocEntry"" = {docEn} and ""U_PozAdi"" = '{result.First().U_PozNo}'");
            await _sapOperationsService.RunQuery<string>($@"UPDATE ""@ERP_HAKEDIS"" SET U_ToplamHakedis=U_ToplamHakedis-{result.First().U_ToplamHakedis ?? 0},
                                                                                        U_SimdikiHakedis=U_SimdikiHakedis-{result.First().U_ToplamHakedis ?? 0} Where ""DocEntry"" = {docEn}");
            var response = await _sapOperationsService.RunQuery<ProgressPayment>($@"SELECT ""U_ToplamHakedis"",U_SimdikiHakedis FROM ""@ERP_HAKEDIS"" Where ""DocEntry"" = {docEn}");
            return new DataResult<ProgressPayment>(response.First(), true);
        }
        public async Task<IQueryable<GetActivityForProgressPayment>> GetActivityForProgressPayment(string budgetId)
        {
            string key = "";
            if (budgetId.Contains("("))
            {
                var strings = budgetId.Split('(');
                budgetId = strings[0];
                key = strings[1].Substring(0, strings[1].Length - 1);
            }
            return await _sapOperationsService.RunQuery<GetActivityForProgressPayment>($@" SELECT T0.""U_AktiviteKodu"" + COALESCE(T0.""U_kisim"", '') [UniqueCode],T0.""U_AktiviteKodu"" AS ""ActivityCode"" , T0.""U_AktiviteAdi"" AS ""ActivityName"" ,
                                                                                    T0.""U_kisim"" AS ""Kisim"" FROM ""@ERP_PRJ_BUTCE_LINE"" T0
                                                                                     JOIN  ""@ERP_PROJ_ACT"" T1 ON T0.""U_AktiviteKodu""=T1.""U_AktiviteKodu"" AND T1.""U_Seviye"" = N'3'
                                                                                        WHERE T0.""DocEntry"" = N'{budgetId}'{(!string.IsNullOrEmpty(key) ? $@" AND T0.""U_AktiviteKodu"" + COALESCE(T0.""U_kisim"", '')={key}" : "")}");
        }
        public async Task<IQueryable<Poz>> GetPozAsync(string docEn)
        {
            return await _sapOperationsService.RunQuery<Poz>($@"SELECT LineId,
                                                                      DocEntry,
                                                                      U_butceId,
                                                                      U_Tur,
                                                                      U_PozNo,
                                                                      U_KalemKodu,
                                                                      U_KalemTanimi,
                                                                      U_AktiviteKodu,
                                                                      U_AktiviteTanimi,
                                                                      U_Kisim,
                                                                      U_DonemHakedis,
                                                                      U_ToplamHakedis,
                                                                      U_OncekiHakedis,
                                                                      U_SimdikiHakedis,
                                                                      U_TotHakAmnt,
                                                                      U_OncHakAmnt,
                                                                      U_SimHakAmnt,
                                                                      U_Miktar,
                                                                      U_BirimFiyat,
                                                                      U_ParaBirimi,
                                                                      U_VergiKodu FROM ""@ERP_HAKEDISPOZ"" WHERE DocEntry = {docEn}");
        }
        public async Task<IQueryable<PozLine>> GetPozLineAsync(string pozName, string DocEn, string LineId = "")
        {
            return await _sapOperationsService.RunQuery<PozLine>($@"SELECT   LineId,
                                                                             DocEntry,
                                                                             U_Aciklama,
                                                                             U_Blok,
                                                                             U_Kat,
                                                                             U_Kot,
                                                                             U_Daire,
                                                                             U_Mahal,
                                                                             U_PozAdi,
                                                                             U_Ad,
                                                                             U_Benzer,
                                                                             U_En,
                                                                             U_Boy,
                                                                             U_Yukseklik,
                                                                             U_Azi,
                                                                             U_Cogu,
                                                                             U_TamamlanmaOrani,
                                                                             U_ToplamHakedis
                                                                              FROM ""@ERP_HAKEDISPOZLINE"" WHERE  DocEntry = {DocEn} 
                                                                            {(!string.IsNullOrEmpty(pozName) ? $@"AND ""U_PozAdi"" = '{pozName}'" : "")}
                                                                            {(!string.IsNullOrEmpty(LineId) ? $@"AND ""LineId"" = {LineId}" : "")}");
        }

        public async Task<IQueryable<GetPozCurrency>> GetCurrencysForPozAsync()
        {
            return await _sapOperationsService.RunQuery<GetPozCurrency>($@"SELECT ""CurrCode"",""CurrName"" FROM OCRN");
        }

        public async Task<IQueryable<GetVatgroup>> GetVatgroupForPozAsync()
        {
            return await _sapOperationsService.RunQuery<GetVatgroup>($@"SELECT ""Code"",""Name"" FROM OVTG");
        }
        public async Task<T?> GetPozLineTotalAsync<T>(string docEn, int? lineId, string pozName = "", bool isLine = true, bool isHeader = false)
        {
            if (isHeader)
            {
                return (await _sapOperationsService.RunQuery<T?>($@"select COALESCE(sum(TBL.total),0)
                                                                        
                                                                        {(lineId!=null?$@"-(select T1.U_ToplamHakedis*T0.U_BirimFiyat  from ""@ERP_HAKEDISPOZ"" T0 JOIN ""@ERP_HAKEDISPOZLINE"" T1 ON T0.""U_PozNo""=T1.""U_PozAdi"" where T0.""DocEntry"" = {docEn} AND  T0.""U_PozNo"" ='{pozName}' and T1.LineId ={lineId})":"")}
                                                                        from 
                                                                        (select SUM(T1.U_ToplamHakedis)*T0.U_BirimFiyat total from ""@ERP_HAKEDISPOZ"" T0 JOIN ""@ERP_HAKEDISPOZLINE"" T1 ON T0.""U_PozNo""=T1.""U_PozAdi"" and T0.""DocEntry"" = {docEn}  GROUP BY T0.""U_PozNo"",T0.U_BirimFiyat) TBL
                                                                        ")).FirstOrDefault();
            }
            else
            {
                if (lineId == -1)
                    return (await _sapOperationsService.RunQuery<T>($@"select  U_ToplamHakedis,U_SimdikiHakedis FROM ""@ERP_HAKEDISPOZ""
                                                                                                            WHERE ""DocEntry"" = {docEn}
                                                                                                            AND U_PozNo = '{pozName}'")).FirstOrDefault();

                return (await _sapOperationsService.RunQuery<T>($@"select COALESCE(SUM(U_ToplamHakedis), 0) FROM ""@ERP_HAKEDISPOZLINE""
                                                                                                             WHERE ""DocEntry"" = {docEn}
                                                                                                             {(lineId != null ? $@"AND ""LineId"" !={lineId}" : "")}
                                                                                                             {(isLine ? $@"AND ""U_PozAdi"" = {(!string.IsNullOrEmpty(pozName) ? $"'{pozName}'" :
                                                                                                                 $@" (SELECT ""U_PozAdi"" FROM ""@ERP_HAKEDISPOZLINE"" WHERE ""DocEntry"" = {docEn} AND ""LineId"" ={lineId})")}" : "")} ")).FirstOrDefault();
            }
        }
        public async Task<int> GetPozLineIdAsync(string docEn, string pozName)
        {
            return (await _sapOperationsService.RunQuery<int>($@"SELECT ""LineId"" FROM ""@ERP_HAKEDISPOZ"" WHERE ""DocEntry"" = {docEn} AND ""U_PozNo"" = '{pozName}'")).FirstOrDefault();
        }
        public async Task<Poz?> GetPozPreviousTotalAsync(string docEn, int? lineId, string pozName = "")
        {
            return (await _sapOperationsService.RunQuery<Poz>($@"select U_OncekiHakedis,U_BirimFiyat,LineId,U_PozNo FROM ""@ERP_HAKEDISPOZ"" 
                                                                WHERE ""DocEntry"" = {docEn} AND ""U_PozNo"" = {(lineId != null ? $@"(select TOP 1 U_PozAdi FROM ""@ERP_HAKEDISPOZLINE"" WHERE ""LineId"" = {lineId})" : "")} 
                                                                                    {(!string.IsNullOrEmpty(pozName) ? $"'{pozName}'" : "")}")).FirstOrDefault();
        }
    }
}
