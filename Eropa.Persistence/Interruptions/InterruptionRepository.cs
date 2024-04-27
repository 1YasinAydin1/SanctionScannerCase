using Eropa.Domain.Interruptions;
using Eropa.Domain.SAPConnection;

namespace Eropa.Persistence.Interruptions
{
    public class InterruptionRepository : IInterruptionRepository
    {

        private readonly ISapOperationsService _sapOperationsService;

        public InterruptionRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IQueryable<AccountCode>> GetAccountCodesAsync()
        {
            return await _sapOperationsService.RunQuery<AccountCode>($@"SELECT AcctCode,AcctName from OACT");
        }

        public async Task<IQueryable<Interruption>> GetInterruptionsAsync(string Code="")
        {
            return await _sapOperationsService.RunQuery<Interruption>(
                $@"SELECT ROW_NUMBER() OVER (ORDER BY Code) AS rowNum,
                         Code,
                         U_KesintiKodu,
                         U_KesintiTanimi,
                         U_TutanakTipi,
                         U_HesapKodu,
                         U_Oran,
                         U_faturalanackmi FROM ""@ERP_CUTT_DEF""  {(!string.IsNullOrEmpty(Code)?$" WHERE Code = '{Code}'" :"")}");
        }

        public async Task<string> GetMaxCodeAsync()
        {
            int code = (await _sapOperationsService.RunQuery<int>($@"SELECT COALESCE(Max(CODE),0) FROM ""@ERP_CUTT_DEF""")).First();
            return (code+1).ToString();
        }

    }
}
