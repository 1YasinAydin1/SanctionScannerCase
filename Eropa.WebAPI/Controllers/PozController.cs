using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Helper.Results;
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
    public class PozController : ODataController
    {
        IPozAppService _appService;

        public PozController(IPozAppService appService)
        {
            _appService = appService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromODataUri] string docEn, Delta<PozLineDto> delta)
        {
            var result = await _appService.AddUpdatePozLineAsync(delta.GetInstance(), docEn);
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return NoContent();
        }
        [HttpPatch]
        public IActionResult Patch([FromODataUri] string docEn, [FromODataUri] string key, Delta<PozLineDto> delta)
        {
            var result = _appService.AddUpdatePozLineAsync(delta.GetInstance(), docEn, key);
            if (!result.Result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });
            return NoContent();
        }
        [HttpDelete]
        public IActionResult Delete([FromODataUri] string pozName, [FromODataUri] string docEn, [FromODataUri] string key)
        {
            _appService.DeletePozLineAsync(docEn, key, pozName);
            return NoContent();
        }

        [HttpPost("PostPoz")]
        public async Task<IActionResult> PostPoz(PozDto delta)
        {
            var result = await _appService.AddUpdatePozAsync(delta);
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return Ok(result);
        }

        [HttpDelete("PozDelete")]
        public async Task<IDataResult<ProgressPaymentDto>> PozDeleteAsync([FromODataUri] string docEn, [FromODataUri] string key)
        {
            return (await _appService.DeletePozAsync(docEn, key));
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<PozLineDto> Get([FromODataUri] string pozName, [FromODataUri] string docEn)
        {
            var ss = _appService.GetPozLineAsync(pozName, docEn);
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetPozs")]
        public IQueryable<PozDto> GetPozs([FromODataUri] int docEn)
        {
            var ss = _appService.GetPozAsync(docEn.ToString());
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetSelectPoz")]
        public IQueryable<SelectPozDto> GetSelectPoz(string contractNo)
        {
            var ss = _appService.GetSelectPozAsync(contractNo);
            return ss.Result;
        }


        [EnableQuery]
        [HttpGet("GetItemsPoz")]
        public IQueryable<GetItemProgressPaymentDto> GetItemsPoz()
        {
            var ss = _appService.GetItemsAsync();
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetActivityPoz")]
        public IQueryable<GetActivityForProgressPaymentDto> GetActivityPoz(string budgetId)
        {
            if (budgetId.Contains("null"))
                return null;
            var ss = _appService.GetActivityForProgressPaymentAsync(budgetId);
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetPozCurrencies")]
        public IQueryable<GetPozCurrencyDto> GetPozCurrencies()
        {
            return (_appService.GetCurrencysForPozAsync()).Result;
        }

        [EnableQuery]
        [HttpGet("GetPozVatGroups")]
        public IQueryable<GetVatgroupDto> GetPozVatGroups()
        {
            return (_appService.GetVatgroupForPozAsync()).Result;
        }

        [EnableQuery]
        [HttpGet("GetPPAfterPozLineTransaction")]
        public ProgressPaymentCreateUpdateDto? GetPPAfterPozLineTransaction(string docEn, string pozName)
        {
            return (_appService.GetPPAfterPozLineTransactionAsync(docEn, pozName)).Result;
        }

        [HttpPost("PostPozExcel")]
        public IActionResult PostPozExcel(string docEn, ExcelRequest excelBase64)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _appService.PozExcelImportAsync(docEn, excelBase64.ExcelBase64);
            if (!result.Result.Success)
                return BadRequest(new ODataError
                {
                    ErrorCode = "500",
                    Message = "Aktarım Başarısız.",
                    Details = result.Result.Errors.Select(s => new ODataErrorDetail { Message = s }).ToList()
                });
            return NoContent();
        }

    }
}
