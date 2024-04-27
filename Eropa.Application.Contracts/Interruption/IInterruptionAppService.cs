using Eropa.Helper.Results;

namespace Eropa.Application.Contracts.Interruption
{
    public interface IInterruptionAppService
    {
        Task<IQueryable<InterruptionDto>> GetInterruptions();
        Task<IQueryable<AccountCodeDto>> GetAccountCodes();
        Task<IResult> AddInterruptionAsync(InterruptionDto interruptionCreateDto);
        Task<IResult> UpdateInterruptionAsync(InterruptionDto interruptionUpdateDto, string key);
        Task<IResult> DeleteInterruptionAsync(string key);
    }
}
