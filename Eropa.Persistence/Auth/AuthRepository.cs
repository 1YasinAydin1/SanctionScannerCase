using Eropa.Domain.Auth;
using Eropa.Domain.SAPConnection;

namespace Eropa.Persistence.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public AuthRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }
        public async Task<IQueryable<DbInfos>> GetCompanies()
        {
            return  await _sapOperationsService.RunQuery<DbInfos>($@"select dbName,cmpName from [SBO-COMMON].dbo.SRGC");
        }
    }
}
