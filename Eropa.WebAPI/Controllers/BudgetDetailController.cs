using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Validation;
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
    public class BudgetDetailController : ODataController
    {

        private readonly IBudgetDetailAppService _appService;

        public BudgetDetailController(IBudgetDetailAppService appService)
        {
            _appService = appService;
        }

        [EnableQuery(MaxNodeCount = 200000)]
        [HttpGet]
        public IQueryable<BudgetDetailDto> Get([FromODataUri] string docEn)
        {
            var response = (_appService.GetBadgetDetailAsync(docEn)).Result;
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromODataUri] string docEn, Delta<BudgetDetailDto> delta)
        {
            var model = delta.GetInstance();
            model.U_Seviye = (model.U_AktiviteKodu?.Length/2).ToString();
            var validationResult = (new BudgetDetailValidation()).Validate(model);
            if (!validationResult.IsValid)
                return BadRequest(new ODataError { ErrorCode = "500", Message = validationResult.Errors.First().ErrorMessage });
            var result = await _appService.AddBudgetDetailAsync(delta.GetInstance(), docEn);
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return NoContent();
        }
        [HttpPatch]
        public IActionResult Patch([FromODataUri] string docEn, [FromODataUri] string key, Delta<BudgetDetailDto> delta)
        {
            var result = _appService.UpdateBudgetDetailAsync(delta.GetInstance(), key, docEn);
            if (!result.Result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });
            return NoContent();
        }
        [HttpDelete]
        public IActionResult Delete([FromODataUri] string docEn, [FromODataUri] string key)
        {
            _appService.DeleteBudgetDetailAsync(docEn, key);
            return NoContent();
        }
        [EnableQuery]
        [HttpGet("GetItems")]
        public IQueryable<GetItemDto> GetItems()
        {
            return (_appService.GetItemsForBudgetDetail()).Result;
        }
        [EnableQuery]
        [HttpGet("GetMeasurementUnits")]
        public IQueryable<GetMeasurementUnitDto> GetMeasurementUnits()
        {
            return (_appService.GetMeasurementUnitsForBudgetDetail()).Result;
        }
        [EnableQuery]
        [HttpGet("GetCurrencies")]
        public IQueryable<GetCurrencyDto> GetCurrencies()
        {
            return (_appService.GetCurrencyForBudgetDetail()).Result;
        }

    }
}
