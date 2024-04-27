using AutoMapper;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Eropa.Application.ProgressPayments
{
    public class ProgressPaymentAppService : IProgressPaymentAppService
    {
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IProgressPaymentRepository _progressPaymentRepository;
        private readonly IPozAppService _pozAppService;
        private readonly IPozRepository _pozRepository;
        private readonly IMapper _mapper;
        public ProgressPaymentAppService(IProgressPaymentRepository progressPaymentRepository, IMapper mapper, ISapOperationsService sapOperationsService, IPozAppService pozAppService, IPozRepository pozRepository)
        {
            _progressPaymentRepository = progressPaymentRepository;
            _mapper = mapper;
            _sapOperationsService = sapOperationsService;
            _pozAppService = pozAppService;
            _pozRepository = pozRepository;
        }

        public async Task<IQueryable<ContractNumberDto>> GetContractNumberAsync()
        {
            var result = await _progressPaymentRepository.GetContractNumberAsync();
            var response = _mapper.ProjectTo<ContractNumberDto>(result);
            return response;
        }

        public async Task<IQueryable<EmployeeMasterDataDto>> GetEmployeeMasterDataAsync()
        {
            var result = await _progressPaymentRepository.GetEmployeeMasterDataAsync();
            var response = _mapper.ProjectTo<EmployeeMasterDataDto>(result);
            return response;
        }

        public async Task<IQueryable<ProgressPaymentDto>> GetProgressPayments()
        {
            var result = await _progressPaymentRepository.GetProgressPaymentsAsync();
            var response = _mapper.ProjectTo<ProgressPaymentDto>(result);
            return response;
        }

        public async Task<IQueryable<WithholdingTaxDto>> GetWithholdingTaxAsync()
        {
            var result = await _progressPaymentRepository.GetWithholdingTaxAsync();
            var response = _mapper.ProjectTo<WithholdingTaxDto>(result);
            return response;
        }

        public async Task<IResult> CreateUpdateProgressPaymentAsync(ProgressPaymentCreateUpdateDto progressPaymentCreateDto)
        {
            if (progressPaymentCreateDto.DocEntry != null)
            {
                //var progressPaymentPozList = await _pozAppService.GetPozAsync(progressPaymentCreateDto.DocEntry.ToString());
                //var list = await _pozAppService.GetSelectPozAsync(progressPaymentCreateDto.U_SozlesmeNo.ToString());

                //foreach (var item in list)
                //    if (!progressPaymentPozList.Any(i => i.U_PozNo.Contains(item.PozLineNum)))
                //        await _pozAppService.AddUpdatePozAsync(new PozDto
                //        {
                //            DocEntry = progressPaymentCreateDto.DocEntry,
                //            U_Tur = "0",
                //            U_KalemKodu = item.ItemCode,
                //            U_KalemTanimi = item.Dscription,
                //            U_AktiviteKodu = item.activityCode,
                //            U_AktiviteTanimi = item.activityName,
                //            U_Kisim = item.kisim,
                //            U_PozNo = item.PozLineNum
                //        });
                await _sapOperationsService.SapPatchServiceAsync(progressPaymentCreateDto, progressPaymentCreateDto.DocEntry.ToString(), "AL_ERP_HAKEDIS");
            }
            else
            {
                if ((progressPaymentCreateDto.U_SozlesmeNo ?? 0) == 0)
                    return new Result(false, $"'Sözleşme No' Boş Bırakılamaz");
                progressPaymentCreateDto.U_HakedisNo = await _progressPaymentRepository.GetMaxProgressPaymentByContractNo(progressPaymentCreateDto.U_SozlesmeNo ?? 1);
                var result = await _sapOperationsService.SapPostServiceAsync(progressPaymentCreateDto, "AL_ERP_HAKEDIS");

                ProgressPaymentDto progressPaymentDto = JsonSerializer.Deserialize<ProgressPaymentDto>(result.Data);

                var list = await _pozAppService.GetSelectPozAsync(progressPaymentCreateDto.U_SozlesmeNo.ToString());
                foreach (var item in list)
                    await _pozAppService.AddUpdatePozAsync(new PozDto
                    {
                        DocEntry = progressPaymentDto.DocEntry,
                        U_Tur = "0",
                        U_KalemKodu = item.ItemCode,
                        U_KalemTanimi = item.Dscription,
                        U_AktiviteKodu = item.activityCode,
                        U_AktiviteTanimi = item.activityName,
                        U_Kisim = item.kisim,
                        U_PozNo = item.PozLineNum,
                        U_BirimFiyat = item.Price,
                        U_Miktar = item.Quantity,
                        U_ParaBirimi = item.Currency,
                        U_VergiKodu = item.VatGroup
                    });
            }
            return new Result(true);
        }

        public async Task<IResult> DuplicateProgressPaymentAsync(string docEn)
        {
            string jsonStringProgressPayment = (await _sapOperationsService.SapGetApiAsync("AL_ERP_HAKEDIS", docEn)).Data;
            JObject jsonObject = JObject.Parse(jsonStringProgressPayment);
            foreach (var item in jsonObject["ERP_HAKEDISPOZCollection"])
            {
                item["U_OncekiHakedis"] = item["U_ToplamHakedis"];
                item["U_OncHakAmnt"] = item["U_TotHakAmnt"];
                item["U_SimdikiHakedis"] = 0;
                item["U_SimHakAmnt"] = 0;
            }
            jsonObject["U_OncekiHakedis"] = jsonObject["U_ToplamHakedis"];
            jsonObject["U_OncHakAmnt"] = jsonObject["U_TotHakAmnt"];
            jsonObject["U_SimdikiHakedis"] = 0;
            jsonObject["U_SimHakAmnt"] = 0;
            jsonObject["U_HakedisNo"] = await _progressPaymentRepository.GetMaxProgressPaymentByContractNo(int.Parse(jsonObject["U_SozlesmeNo"].ToString()));

            jsonStringProgressPayment = jsonObject.ToString();
            await _sapOperationsService.SapPostServiceAsync<string>(jsonStringProgressPayment, "AL_ERP_HAKEDIS");
            return new Result(true);
        }
        public async Task<IResult> DeleteProgressPaymentAsync(string key)
        {
            await _sapOperationsService.SapDeleteServiceAsync(key, "AL_ERP_HAKEDIS");
            return new Result(true);
        }

        public async Task<IQueryable<ApprovalUserDto>> GetApprovalUserForPozAsync()
        {
            var result = await _progressPaymentRepository.GetApprovalUserForPoz();
            var response = _mapper.ProjectTo<ApprovalUserDto>(result);
            return response;
        }
        public async Task UpdateStateProgressPaymentAsync(string docEn, string value)
        {
            await _progressPaymentRepository.UpdateStateProgressPayment(docEn, value);
        }

        public async Task<IResult> PurchaseInvoiceAsync(int ContractNo, int DocEntry)
        {
            var salesOrderInfo = await _progressPaymentRepository.GetOrderInfoForPurchaseInvoice(ContractNo.ToString());
            var progressPaymentInfo = (await _progressPaymentRepository.GetProgressPaymentForPurchaseInvoice(DocEntry.ToString())).ToList();

            PurchaseInvoiceDto purchase = new() { CardCode = salesOrderInfo.CardCode, U_btcHakedis = progressPaymentInfo[0].DocNum.ToString(), NumAtCard = salesOrderInfo.NumAtCard };
            List<PurchaseInvoiceLineDto> invoiceLines = new();
            foreach (var item in progressPaymentInfo)
            {
                if (item.U_Tur == 0)
                {
                    var calQuantity = await _progressPaymentRepository.GetPozLineForPurchaseInvoice(DocEntry, item.U_PozNo);
                    if ((calQuantity ?? 0) == 0)
                        return new Result(false, $"Miktar alanı '{item.U_PozNo}' için 0 olarak hesaplandı");

                    int baseLine = int.Parse(item.U_PozNo.Replace("POZ", ""));

                    PurchaseInvoiceLineDto lineDto = new()
                    {
                        ItemCode = item.U_KalemKodu,
                        Quantity = calQuantity ?? 1,
                        BaseEntry = ContractNo,
                        BaseLine = baseLine,
                        BaseType = 22,
                    };
                    invoiceLines.Add(lineDto);
                }
                else
                {
                    PurchaseInvoiceLineDto lineDto = new()
                    {
                        ItemCode = item.U_KalemKodu,
                        Quantity = item.U_Miktar ?? 0,
                        Currency = item.U_ParaBirimi,
                        UnitPrice = item.U_BirimFiyat,
                        U_btcAktiviteKodu = item.U_AktiviteKodu,
                        U_btcAktiviteTanimi = item.U_AktiviteTanimi,
                        U_btcKisim = item.U_Kisim,
                        U_btcProje = salesOrderInfo.U_btcProje,
                    };
                    invoiceLines.Add(lineDto);
                }
            }
            purchase.DocumentLines = invoiceLines;
            await _sapOperationsService.SapPostServiceAsync(purchase, "Drafts");
            return new Result(true);
        }

        public async Task<bool> GetPurchaseInvoiceRecord(string docEn)
        {
            var result = await _progressPaymentRepository.GetPurchaseInvoiceRecord(docEn);
            return (result ?? 0) > 0 ? true : false;
        }
        public async Task<IQueryable<PurchaseInvoiceInfoDto>> GetPurchaseInvoiceInfo(string docEn)
        {
            var result = await _progressPaymentRepository.GetPurchaseInvoiceInfo(docEn);
            var response = _mapper.ProjectTo<PurchaseInvoiceInfoDto>(result);
            return response;
        }
        public async Task<IQueryable<PurchaseInvoiceLineInfoDto>> GetPurchaseInvoiceLineInfo(string docEn)
        {
            var result = await _progressPaymentRepository.GetPurchaseInvoiceLineInfo(docEn);
            var response = _mapper.ProjectTo<PurchaseInvoiceLineInfoDto>(result);
            return response;
        }
    }
}
