using AutoMapper;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;

namespace Eropa.Application.ProgressPayments
{
    public class TutanakAppService : ITutanakAppService
    {
        private readonly ISapOperationsService _sapOperationsService;
        private readonly ITutanakRepository _tutanakRepository;
        private readonly IMapper _mapper;

        public TutanakAppService(ISapOperationsService sapOperationsService, ITutanakRepository tutanakRepository, IMapper mapper)
        {
            _sapOperationsService = sapOperationsService;
            _tutanakRepository = tutanakRepository;
            _mapper = mapper;
        }

        public async Task<IResult> DeleteTutanakAsync(string docEn, string lineId)
        {
            await _tutanakRepository.DeleteTutanakLine(docEn, lineId);
            return new Result(true);
        }

        public async Task<IQueryable<TutanakDto>> GetTutanakListAsync(string docEn)
        {
            var result = await _tutanakRepository.GetTutanakAsync(docEn);
            var response = _mapper.ProjectTo<TutanakDto>(result);
            return response;
        }
        public async Task<TutanakNoDto> GetTutanakNoInfoAsync(string docEn, string orderDocEn)
        {
            var result = await _tutanakRepository.GetTutanakNoInfo(docEn, orderDocEn);
            var response = _mapper.Map<TutanakNoDto>(result);
            return response;
        }
        public async Task<IResult> AddUpdateTutanakAsync(TutanakDto tutanakDto, string docEn, string key = "")
        {
            if (!string.IsNullOrEmpty(key)) tutanakDto.LineId = int.Parse(key);
            tutanakDto.DocEntry = int.Parse(docEn);

            ProgressPaymentCreateUpdateDto req = new() { ERP_HAK_TUTANAKCollection = new List<TutanakDto> { tutanakDto } };
            await _sapOperationsService.SapPatchServiceAsync(req, docEn, "AL_ERP_HAKEDIS");
            return new Result(true);
        }
        public async Task<IResult> InvoiceAsync(int ContractNo, int DocEntry, string lineIds)
        {
            var salesOrderInfo = await _tutanakRepository.GetConnBPAsync(ContractNo.ToString());
            var tutanakInfo = (await _tutanakRepository.GetTutanakForInvoiceAsync(DocEntry.ToString(), lineIds)).ToList();

            if (tutanakInfo.Count == 0)
                return new Result(false, "Faturaya dönüştürülecek satır bulunamadı");

            InvoiceDto invoice = new() { CardCode = salesOrderInfo.ConnBP, U_btcHakedis = DocEntry.ToString() };
            List<InvoiceLineDto> invoiceLines = new();
            List<LineHolder> lnlist = new();
            int t = 0;
            foreach (var item in tutanakInfo)
            {
                if ((item.U_Tutar ?? 0) == 0)
                    return new Result(false, $"'{item.U_TutanakNo}' Tutanak Numaralı Satırda Tutar Alanı Doldurulmalı");
                InvoiceLineDto lineDto = new()
                {
                    AccountCode = item.U_HesapKodu,
                    ItemDescription = item.U_Icerik,
                    Quantity = 1,
                    UnitPrice = item.U_Tutar ?? 1,
                    LineTotal = item.U_Tutar ?? 1,
                    U_btcProje = salesOrderInfo.U_btcProje,
                };
                LineHolder lnhlder = new LineHolder();
                lnhlder.LineNo = t;
                lnhlder.btcTutLine = item.LineId;
                t += 1;
                lnlist.Add(lnhlder);
                invoiceLines.Add(lineDto);
            }
            invoice.DocumentLines = invoiceLines;
            var result = await _sapOperationsService.SapPostServiceAsync(invoice, "Drafts");
            JObject jsonObject = JObject.Parse(result.Data);

            await _tutanakRepository.UpdateTutanakForInvoiceAsync(jsonObject["DocEntry"].ToString(), DocEntry, lineIds);
            foreach (var item in lnlist)
                await _sapOperationsService.RunQuery<string>($@"UPDATE DRF1 set U_btcTutline = {item.btcTutLine},U_btcHakedis = {DocEntry} WHERE DocEntry = {jsonObject["DocEntry"]} AND LineNum = {item.LineNo}");

            return new Result(true);
        }
        public async Task<IQueryable<InvoiceInfoDto>> GetInvoiceInfo(string docEn, string LineId)
        {
            var result = await _tutanakRepository.GetInvoiceInfo(docEn, LineId);
            var response = _mapper.ProjectTo<InvoiceInfoDto>(result);
            return response;
        }
        public async Task<IQueryable<InvoiceLineInfoDto>> GetInvoiceLineInfo(string docEn)
        {
            var result = await _tutanakRepository.GetInvoiceLineInfo(docEn);
            var response = _mapper.ProjectTo<InvoiceLineInfoDto>(result);
            return response;
        }


    }
}
