namespace Eropa.Domain.Activities
{
    public interface IActivityRepository
    {
        Task<IQueryable<Activity>> GetActivitiesAsync(string Code = "");
        Task<bool> GetActivityByCode(string code);
    }
}
