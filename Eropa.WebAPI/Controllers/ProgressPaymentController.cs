using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Helper.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData;

namespace Eropa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressPaymentController : ODataController
    {
        private readonly IProgressPaymentAppService _progressPaymentAppService;

        public ProgressPaymentController(IProgressPaymentAppService progressPaymentAppService)
        {
            _progressPaymentAppService = progressPaymentAppService;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<ProgressPaymentDto> Get()
        {
            var ss = _progressPaymentAppService.GetProgressPayments();
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetContractNumber")]
        public IQueryable<ContractNumberDto> GetContractNumber()
        {
            var ss = _progressPaymentAppService.GetContractNumberAsync();
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetEmployeeMasterData")]
        public IQueryable<EmployeeMasterDataDto> GetEmployeeMasterData()
        {
            var ss = _progressPaymentAppService.GetEmployeeMasterDataAsync();
            return ss.Result;
        }

        [EnableQuery]
        [HttpGet("GetWithholdingTax")]
        public IQueryable<WithholdingTaxDto> GetWithholdingTax()
        {
            var ss = _progressPaymentAppService.GetWithholdingTaxAsync();
            return ss.Result;
        }

        [HttpPost]
        public IActionResult CreateUpdateProgressPayment(ProgressPaymentCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _progressPaymentAppService.CreateUpdateProgressPaymentAsync(dto);
            if (!result.Result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Result.Message });
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteProgressPayment(string key)
        {
            _progressPaymentAppService.DeleteProgressPaymentAsync(key);
            return NoContent();
        }


        [HttpPost("Duplicate")]
        public async Task<IActionResult> PostDuplicate(string docEn)
        {
            var result = await _progressPaymentAppService.DuplicateProgressPaymentAsync(docEn);
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return NoContent();
        }

        [HttpGet("GetApprovalUserForPoz")]
        public IQueryable<ApprovalUserDto> GetApprovalUserForPoz()
        {
            var ss = _progressPaymentAppService.GetApprovalUserForPozAsync();
            return ss.Result;
        }

        [HttpGet("GetPurchaseInvoiceRecord")]
        public bool GetPurchaseInvoiceRecord(string docEn)
        {
            var ss = _progressPaymentAppService.GetPurchaseInvoiceRecord(docEn);
            return ss.Result;
        }

        [HttpPost("UpdateState")]
        public async Task<IActionResult> UpdateStateProgressPayment(string docEn,string value)
        {
            await _progressPaymentAppService.UpdateStateProgressPaymentAsync(docEn, value);
            return NoContent();
        }

        [HttpGet("GetPurchaseInvoiceInfo")]
        public IQueryable<PurchaseInvoiceInfoDto> GetPurchaseInvoiceInfo(string docEn)
        {
            var ss = _progressPaymentAppService.GetPurchaseInvoiceInfo(docEn);
            return ss.Result;
        }

        [HttpGet("GetPurchaseInvoiceLineInfo")]
        public IQueryable<PurchaseInvoiceLineInfoDto> GetPurchaseInvoiceLineInfo(string docEn)
        {
            var ss = _progressPaymentAppService.GetPurchaseInvoiceLineInfo(docEn);
            return ss.Result;
        }

        [HttpPost("PurchaseInvoice")]
        public async Task<IActionResult> PostPurchaseInvoice(int ContractNo,int DocEntry)
        {
            var result = await _progressPaymentAppService.PurchaseInvoiceAsync(ContractNo, DocEntry);
            if (!result.Success)
                return BadRequest(new ODataError { ErrorCode = "500", Message = result.Message });
            return NoContent();
        }
    }
}
