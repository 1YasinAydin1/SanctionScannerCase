using Eropa.Application.Activities;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Interruption;
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
    public class InterruptionController : ODataController
    {

        private readonly IInterruptionAppService _appService;

        public InterruptionController(IInterruptionAppService appService)
        {
            _appService = appService;
        }

        [HttpPost]
        public IActionResult Post(Delta<InterruptionDto> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _appService.AddInterruptionAsync(delta.GetInstance());
            if (!result.Result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });

            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] string key)
        {
            _appService.DeleteInterruptionAsync(key);
            return NoContent();
        }
        [EnableQuery]
        [HttpGet]
        public IQueryable<InterruptionDto> Get()
        {
            return (_appService.GetInterruptions()).Result;
        }

        [HttpPatch]
        public IActionResult Patch([FromODataUri] string key, Delta<InterruptionDto> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _appService.UpdateInterruptionAsync(delta.GetInstance(), key);
            if (!result.Result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });
            return NoContent();
        }

        [EnableQuery]
        [HttpGet("GetAccountCodes")]
        public IQueryable<AccountCodeDto> GetAccountCodes()
        {
            return (_appService.GetAccountCodes()).Result;
        }
    }
}
