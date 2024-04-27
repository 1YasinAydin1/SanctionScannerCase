using AutoMapper;
using Eropa.Application.Contracts.Auth;
using Eropa.Domain.Auth;
using Eropa.Domain.SAPConnection;

namespace Eropa.Application.Auth
{
    public class AuthAppService : IAuthAppService
    {
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        public AuthAppService(ISapOperationsService sapOperationsService, IMapper mapper, IAuthRepository authRepository)
        {
            _sapOperationsService = sapOperationsService;
            _mapper = mapper;
            _authRepository = authRepository;
        }

        public async Task<IQueryable<DbInfosDto>> GetCompanies()
        {
            var result = await _authRepository.GetCompanies();
            var response = _mapper.ProjectTo<DbInfosDto>(result);
            return response;
        }

        public async Task SapLogin(SapLoginDto sapLoginDto)
        {
            await _sapOperationsService.SapLoginServiceAsync(_mapper.Map<SapLogin>(sapLoginDto));
        }
    }
}
