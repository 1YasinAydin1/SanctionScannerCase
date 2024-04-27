namespace Eropa.Application.Contracts.Auth
{
    public interface IAuthAppService
    {
        Task SapLogin(SapLoginDto sapLoginDto);
        Task<IQueryable<DbInfosDto>> GetCompanies();
    }
}
