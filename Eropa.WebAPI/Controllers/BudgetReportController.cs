using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Budgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Eropa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetReportController : ODataController
    {
        private readonly IBudgetReportAppService _budgetReportAppService;

        public BudgetReportController(IBudgetReportAppService budgetReportAppService)
        {
            _budgetReportAppService = budgetReportAppService;
        }

        //[EnableQuery]
        [HttpGet]
        public IQueryable<BudgetReportDto> Get([FromQuery] BudgetReportFilterDto budgetReportFilterDto)
        {
            var ss = _budgetReportAppService.GetBudgetReportAsync(budgetReportFilterDto);
            return ss.Result;
        }
    }
}
