using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Application.ProgressPayments;
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
    public class TutanakController : ODataController
    {
        private readonly ITutanakAppService _appService;

        public TutanakController(ITutanakAppService appService)
        {
            _appService = appService;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<TutanakDto> Get(int docEn)
        {
            var ss = _appService.GetTutanakListAsync(docEn.ToString());
            return ss.Result;
        }

        [HttpGet("GetOrderNumAtCard")]
        public async Task<TutanakNoDto> GetTutanakNoInfo(int docEn, int orderdocEn)
        {
            return await _appService.GetTutanakNoInfoAsync(docEn.ToString(), orderdocEn.ToString());
        }


        [HttpDelete]
        public IActionResult Delete([FromODataUri] int docEn, [FromODataUri] string key)
        {
            _appService.DeleteTutanakAsync(docEn.ToString(), key);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromODataUri] int docEn, Delta<TutanakDto> delta)
        {
            var result = await _appService.AddUpdateTutanakAsync(delta.GetInstance(), docEn.ToString());
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return NoContent();
        }
        [HttpPatch]
        public IActionResult Patch([FromODataUri] int docEn, [FromODataUri] string key, Delta<TutanakDto> delta)
        {
            var result = _appService.AddUpdateTutanakAsync(delta.GetInstance(), docEn.ToString(), key);
            if (!result.Result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });
            return NoContent();
        }
        [HttpPost("Invoice")]
        public async Task<IActionResult> PostInvoice(int ContractNo, int DocEntry,string lineIds)
        {
            var result = await _appService.InvoiceAsync(ContractNo, DocEntry, lineIds);
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return NoContent();
        }
        [HttpGet("GetInvoiceInfo")]
        public IQueryable<InvoiceInfoDto> GetInvoiceInfo(string docEn, string LineId)
        {
            var ss = _appService.GetInvoiceInfo(docEn, LineId);
            return ss.Result;
        }

        [HttpGet("GetInvoiceLineInfo")]
        public IQueryable<InvoiceLineInfoDto> GetInvoiceLineInfo(string docEn)
        {
            var ss = _appService.GetInvoiceLineInfo(docEn);
            return ss.Result;
        }
    }
}
