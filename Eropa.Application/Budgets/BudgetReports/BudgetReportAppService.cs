using AutoMapper;
using Eropa.Application.Contracts.Budgets;
using Eropa.Domain.Budgets;

namespace Eropa.Application.Budgets
{
    public class BudgetReportAppService : IBudgetReportAppService
    {
        private readonly IBudgetReportRepository _budgetReportRepository;
        private readonly IMapper _mapper;
        public BudgetReportAppService(IBudgetReportRepository budgetReportRepository, IMapper mapper = null)
        {
            _budgetReportRepository = budgetReportRepository;
            _mapper = mapper;
        }

        public async Task<IQueryable<BudgetReportDto>> GetBudgetReportAsync(BudgetReportFilterDto budgetReportFilterDto)
        {
            var filter = _mapper.Map<BudgetReportFilter>(budgetReportFilterDto);
            var result = await _budgetReportRepository.GetBudgetReport(filter);
            return _mapper.ProjectTo<BudgetReportDto>(result);
        }
    }
}
