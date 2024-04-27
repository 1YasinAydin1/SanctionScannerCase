using Eropa.Application.Contracts.Activities;
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
    public class ActivitiesController : ODataController
    {
        private readonly IActivityAppService _activityAppService;

        public ActivitiesController(IActivityAppService activityAppService)
        {
            _activityAppService = activityAppService;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<ActivityDto> Get()
        {
            var ss = _activityAppService.GetActivities();
            return ss.Result;
        }
        [HttpPost]
        public IActionResult Post(Delta<ActivityDto> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _activityAppService.AddActivityAsync(delta.GetInstance());
            if (!result.Result.Success)
            return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });

            return NoContent();
        }
        [HttpPatch]
        public IActionResult Patch([FromODataUri] string key, Delta<ActivityDto> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _activityAppService.UpdateActivityAsync(delta.GetInstance(), key);
            return NoContent();
        }
        [HttpDelete]
        public IActionResult Delete([FromODataUri] string key)
        {
            _activityAppService.DeleteActivityAsync(key);
            return NoContent();
        }

        [HttpPost("PostExcel")]
        public IActionResult PostExcel(ExcelRequest excelBase64)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _activityAppService.ActivityExcelImport(excelBase64.ExcelBase64);
            return NoContent();
        }
    }
}
