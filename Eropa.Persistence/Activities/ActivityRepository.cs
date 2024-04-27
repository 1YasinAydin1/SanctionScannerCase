using Eropa.Domain.Activities;
using Eropa.Domain.SAPConnection;

namespace Eropa.Persistence.Activities
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ISapOperationsService _sapOperationsService;

        public ActivityRepository(ISapOperationsService sapOperationsService)
        {
            _sapOperationsService = sapOperationsService;
        }


        public async Task<IQueryable<Activity>> GetActivitiesAsync(string Code = "")
        {
            return await _sapOperationsService.RunQuery<Activity>(
                $@"SELECT ROW_NUMBER() OVER (ORDER BY Code) AS rowNum,
                         Code,
                         U_AktiviteKodu,
                         U_AktiviteTanimi,
                         COALESCE(U_UstAktiviteKodu,'-1') ""U_UstAktiviteKodu"",
                         COALESCE(U_Durum,'') ""U_Durum"",
                         U_Seviye FROM ""@ERP_PROJ_ACT"" {(!string.IsNullOrEmpty(Code)?$" WHERE Code = '{Code}'" :"")}");
        }
        public async Task<bool> GetActivityByCode(string code)
        {
            var response = await _sapOperationsService.RunQuery<string>(
                $@"SELECT Code FROM ""@ERP_PROJ_ACT"" where Code='{code}'");
            return response.AsEnumerable().FirstOrDefault() == null ? true : false;
        }

    }
}
