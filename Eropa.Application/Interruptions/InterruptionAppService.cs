using AutoMapper;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Interruption;
using Eropa.Domain.Budgets;
using Eropa.Domain.Interruptions;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eropa.Application.Interruptions
{
    public class InterruptionAppService : IInterruptionAppService
    {
        private readonly IInterruptionRepository _repository;
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IMapper _mapper;

        public InterruptionAppService(ISapOperationsService sapOperationsService, IMapper mapper, IInterruptionRepository repository)
        {
            _sapOperationsService = sapOperationsService;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IResult> AddInterruptionAsync(InterruptionDto interruptionCreateDto)
        {
            InterruptionCreateDto activityCreate = _mapper.Map<InterruptionCreateDto>(interruptionCreateDto);
            activityCreate.Code = await _repository.GetMaxCodeAsync();
            var ss = await _sapOperationsService.SapPostServiceAsync(activityCreate, "AL_ERP_CUTT_DEF");
            return new Result(true);
        }

        public async Task<IResult> DeleteInterruptionAsync(string key)
        {
            await _sapOperationsService.SapDeleteServiceAsync($"'{key}'", "AL_ERP_CUTT_DEF");
            return new Result(true);
        }
        public async Task<IResult> UpdateInterruptionAsync(InterruptionDto interruptionUpdateDto, string key)
        {
            InterruptionUpdateDto interruptionUpdate = _mapper.Map<InterruptionUpdateDto>(interruptionUpdateDto);
            await _sapOperationsService.SapPatchServiceAsync(interruptionUpdate, $"'{key}'", "AL_ERP_CUTT_DEF");
            return new Result(true);
        }

        public async Task<IQueryable<AccountCodeDto>> GetAccountCodes()
        {
            var result = await _repository.GetAccountCodesAsync();
            var response = _mapper.ProjectTo<AccountCodeDto>(result);
            return response;
        }

        public async Task<IQueryable<InterruptionDto>> GetInterruptions()
        {
            var result = await _repository.GetInterruptionsAsync();
            var response = _mapper.ProjectTo<InterruptionDto>(result);
            return response;
        }

    

     
    }
}
