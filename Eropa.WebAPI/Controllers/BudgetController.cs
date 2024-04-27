using Autofac.Features.Metadata;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Budgets;
using Eropa.Helper.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData;

namespace Eropa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ODataController
    {
        private readonly IBudgetAppService _budgetAppService;
        public BudgetController(IBudgetAppService budgetAppService)
        {
            _budgetAppService = budgetAppService;
        }
        [EnableQuery]
        [HttpGet]
        public Task<IQueryable<BudgetDto>> Get()
        {
            var response = _budgetAppService.GetBadgetAsync();
            return response;
        }

        [HttpGet("GetUpdatedAfter")]
        public Task<IQueryable<BudgetDto>> GetUpdatedAfter(string docEn="")
        {
            var response = _budgetAppService.GetBadgetAsync(docEn);
            return response;
        }

        [HttpPost]
        public IActionResult Post(BudgetUpdateDto data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _budgetAppService.UpdateBudgetAsync(data);
            return NoContent();
        }
        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<BudgetDto> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string stringJson = CustomJsonIgnore.JsonIgnore(delta.GetInstance());

            return NoContent();
        }
        [HttpGet("GetCurrency")]
        public IQueryable<CurrencyCurrDto> GetCurrency()
        {
            var response = (_budgetAppService.GetCurrencyAsync()).Result;
            return response;
        }

        [EnableQuery]
        [HttpGet("GetProject")]
        public IQueryable<ProjectDto> GetProject()
        {
            var response = (_budgetAppService.GetProjectAsync()).Result;
            return response;
        }

        [HttpPost("PostExcel")]
        public IActionResult PostExcel(ExcelRequest excelBase64)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _budgetAppService.BudgetExcelImportAsync(excelBase64.ExcelBase64);
            if (!result.Result.Success)
                return BadRequest(new ODataError
                {
                    ErrorCode = "500",
                    Message = "Aktarım Başarısız.",
                    Details = result.Result.Errors.Select(s => new ODataErrorDetail { Message = s }).ToList()
                });
            return NoContent();
        }

        [HttpPost("EmptyRecordAdd")]
        public IActionResult EmptyRecordAdd()
        {
            _budgetAppService.BudgetEmptyRecordAdd();
            return NoContent();
        }

        [HttpPost("Revised")]
        public IActionResult RevisedBudget(RevisedBudgetCreateDto revisedDocEn)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _budgetAppService.RevisedBudgetAsync(revisedDocEn.revisedDocEn.ToString());
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteBudget(string key)
        {
            _budgetAppService.DeleteBudgetAsync(key);
            return NoContent();
        }

    }
}
