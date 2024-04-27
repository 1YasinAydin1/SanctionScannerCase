using Eropa.Application.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Eropa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ODataController
    {
        private readonly IAuthAppService _appService;

        public AuthController(IAuthAppService appService)
        {
            _appService = appService;
        }
        [HttpPost("saplogin")]
        public async Task SapLogin(SapLoginDto loginObj)
        {
            await _appService.SapLogin(loginObj);
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<DbInfosDto> Get()
        {
            var ss = _appService.GetCompanies();
            return ss.Result;
        }

        [HttpGet("test")]
        public string Gettest()
        {
            return "testc";
        }
    }
}
