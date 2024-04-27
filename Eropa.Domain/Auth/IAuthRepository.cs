namespace Eropa.Domain.Auth
{
    public interface IAuthRepository
    {
        Task<IQueryable<DbInfos>> GetCompanies();
    }
}
