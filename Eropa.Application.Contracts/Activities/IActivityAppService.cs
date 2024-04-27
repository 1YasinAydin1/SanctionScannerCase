using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.Activities
{
    public interface IActivityAppService
    {
        Task<IQueryable<ActivityDto>> GetActivities();
        Task<IResult> AddActivityAsync(ActivityDto activityCreateDto);
        Task<IResult> UpdateActivityAsync(ActivityDto activityUpdateDto, string key);
        Task<IResult> DeleteActivityAsync(string key);
        Task<IResult> ActivityExcelImport(string excelBase64);
    }
}
