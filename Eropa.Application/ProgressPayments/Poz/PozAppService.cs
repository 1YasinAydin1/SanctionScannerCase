using AutoMapper;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using ExcelDataReader;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Eropa.Application.ProgressPayments
{
    public class PozAppService : IPozAppService
    {
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IPozRepository _pozRepository;
        private readonly IProgressPaymentRepository _progressPaymentRepository;
        private readonly IMapper _mapper;

        public PozAppService(ISapOperationsService sapOperationsService, IMapper mapper, IPozRepository pozRepository, IProgressPaymentRepository progressPaymentRepository)
        {
            _sapOperationsService = sapOperationsService;
            _mapper = mapper;
            _pozRepository = pozRepository;
            _progressPaymentRepository = progressPaymentRepository;
        }

        public async Task<IQueryable<SelectPozDto>> GetSelectPozAsync(string contractNo)
        {
            var result = await _pozRepository.GetSelectPozAsync(contractNo);
            var response = _mapper.ProjectTo<SelectPozDto>(result);
            return response;
        }

        public async Task<IResult> PozExcelImportAsync(string docEn, string excelBase64)
        {
            try
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(excelBase64.Substring(78));
                MemoryStream stream = new MemoryStream(base64EncodedBytes, 0, base64EncodedBytes.Length);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });
                string contractNo = "", tur = ""; SelectPoz selectPoz = null;
                var table = dataSet.Tables[0].AsEnumerable();

                foreach (var item in table)
                {
                    if (!string.IsNullOrEmpty(item.Field<string>("Poz No")))
                    {
                        tur = "1";
                        if (item.Field<string>("Tür") == "Sözleşmeye Bağlı")
                        {
                            tur = "0";
                            if (string.IsNullOrEmpty(contractNo))
                            {
                                var header = (await _progressPaymentRepository.GetProgressPaymentsAsync(docEn)).FirstOrDefault();
                                if (header != null)
                                    contractNo = header.U_SozlesmeNo.ToString();
                            }
                            selectPoz = (await _pozRepository.GetSelectPozAsync(contractNo)).FirstOrDefault();
                        }

                        var reqPoz = new PozDto
                        {
                            DocEntry = int.Parse(docEn),
                            U_Tur = tur,
                            U_PozNo = item.Field<string>("Poz No"),
                            U_KalemKodu = selectPoz != null ? selectPoz.ItemCode : item.Field<string>("Kalem Kodu"),
                            U_KalemTanimi = selectPoz != null ? selectPoz.Dscription : item.Field<string>("Kalem Adı"),
                            U_AktiviteKodu = selectPoz != null ? selectPoz.activityCode : item.Field<double?>("Aktivite Kodu").ToString(),
                            U_AktiviteTanimi = selectPoz != null ? selectPoz.activityName : item.Field<string>("Aktivite Adı"),
                            U_Kisim = selectPoz != null ? selectPoz.kisim : item.Field<string>("Kısım"),
                            U_Miktar = selectPoz != null ? selectPoz.Quantity : item.Field<double?>("Miktar"),
                            U_BirimFiyat = selectPoz != null ? selectPoz.Price : item.Field<double?>("Birim Fiyat"),
                            U_ParaBirimi = selectPoz != null ? selectPoz.Currency : item.Field<string>("Para Birimi"),
                            U_VergiKodu = selectPoz != null ? selectPoz.VatGroup : item.Field<string>("Vergi Kodu")
                        };
                        await AddUpdatePozAsync(reqPoz);
                    }

                    if (!string.IsNullOrEmpty(item.Field<string>("Poz Adı")))
                    {
                        var reqPozline = new PozLineDto
                        {
                            U_Aciklama = item.Field<string>("Açıklama"),
                            U_Blok = item.Field<string>("Blok"),
                            U_Kat = item.Field<string>("Kat"),
                            U_Kot = item.Field<double>("Kot"),
                            U_Daire = item.Field<string>("Daire"),
                            U_Mahal = item.Field<string>("Mahal"),
                            U_PozAdi = item.Field<string>("Poz Adı"),
                            U_Ad = item.Field<double>("Ad"),
                            U_Benzer = item.Field<double>("Benzer"),
                            U_En = item.Field<double>("En"),
                            U_Boy = item.Field<double>("Boy"),
                            U_Yukseklik = item.Field<double>("Yükseklik"),
                            U_TamamlanmaOrani = (int)item.Field<double>("Tamamlanma Oranı")
                        };
                        await AddUpdatePozLineAsync(reqPozline, docEn);
                    }
                }
                return new Result(true);
            }
            catch (Exception ex)
            {
                return new Result(false, "Excel içeri aktarılırken bir hata oluştu");
            }
        }

        public async Task<IResult> AddUpdatePozLineAsync(PozLineDto pozLine, string docEn, string key = "")
        {
            if (!string.IsNullOrEmpty(key)) pozLine.LineId = int.Parse(key);
            pozLine.DocEntry = int.Parse(docEn);

            PozLine result = null;
            if (!string.IsNullOrEmpty(key))
            {
                result = (await _pozRepository.GetPozLineAsync("", docEn, key)).FirstOrDefault();
                if (result == null)
                    return new Result(false, $"İlgili satır Poz içerisinde bulunamadı");
            }
            double calculation = (pozLine.U_Ad ?? result.U_Ad ?? 0) * (pozLine.U_En ?? result.U_En ?? 0) * (pozLine.U_Boy ?? result.U_Boy ?? 0) * (pozLine.U_Benzer ?? result.U_Benzer ?? 0);
            if (calculation < 0)
            {
                pozLine.U_Azi = calculation;
                pozLine.U_Cogu = 0;
            }
            else
            {
                pozLine.U_Azi = 0;
                pozLine.U_Cogu = calculation;
            }

            pozLine.U_ToplamHakedis = (int)(calculation * (pozLine.U_TamamlanmaOrani ?? result.U_TamamlanmaOrani ?? 0)) / 100;

            //ProgressPaymentCreateUpdateDto req = new() { ERP_HAKEDISPOZLINECollection = new List<PozLineDto> { pozLine } };
            //await _sapOperationsService.SapPatchServiceAsync(req, docEn, "AL_ERP_HAKEDIS");

            var poz = await _pozRepository.GetPozPreviousTotalAsync(docEn, pozLine.LineId,pozLine.U_PozAdi);
            var pp = await _progressPaymentRepository.GetProgressPaymentPreviousTotal(docEn);
            var lineTotal = (await _pozRepository.GetPozLineTotalAsync<double>(docEn, pozLine.LineId, pozLine.U_PozAdi)) + pozLine.U_ToplamHakedis;
            var pozTotal = (await _pozRepository.GetPozLineTotalAsync<double>(docEn, pozLine.LineId, pozLine.U_PozAdi, false)) + pozLine.U_ToplamHakedis;
            var headerTotal = (await _pozRepository.GetPozLineTotalAsync<double>(docEn, pozLine.LineId, poz.U_PozNo, false,true)) + (pozLine.U_ToplamHakedis * (poz.U_BirimFiyat ?? 1));

            await _sapOperationsService.SapPatchServiceAsync(new ProgressPaymentCreateUpdateDto()
            {
                U_ToplamHakedis = pozTotal,
                U_SimdikiHakedis = pozTotal - pp.U_OncekiHakedis,
                U_TotHakAmnt = headerTotal,
                U_SimHakAmnt = headerTotal - pp.U_OncHakAmnt,
                ERP_HAKEDISPOZLINECollection = new List<PozLineDto> {
                    pozLine
                },
                ERP_HAKEDISPOZCollection = new List<PozDto> {
                    new PozDto {
                        LineId = poz.LineId,
                        U_ToplamHakedis = lineTotal,
                        U_SimdikiHakedis = lineTotal - (poz.U_OncekiHakedis??0),
                        U_TotHakAmnt = lineTotal * (poz.U_BirimFiyat ?? 1),
                        U_SimHakAmnt = ((lineTotal - (poz.U_OncekiHakedis??0)) * (poz.U_BirimFiyat ?? 1))
                    }
                }
            }, docEn, "AL_ERP_HAKEDIS");
            return new Result(true);
        }

        public async Task<ProgressPaymentCreateUpdateDto> GetPPAfterPozLineTransactionAsync(string docEn,string? U_PozAdi)
        {
            var pp = await _progressPaymentRepository.GetProgressPaymentPreviousTotal(docEn);
            var pozs = (await _pozRepository.GetPozAsync(docEn));
            var pozTotal = (await _pozRepository.GetPozLineTotalAsync<PozDto>(docEn, -1, U_PozAdi, false));

            return new ProgressPaymentCreateUpdateDto()
            {
                U_ToplamHakedis = pp.U_ToplamHakedis,
                U_SimdikiHakedis = pp.U_SimdikiHakedis,
                U_TotHakAmnt = pp.U_TotHakAmnt,
                U_OncHakAmnt = pp.U_OncHakAmnt,
                U_SimHakAmnt = pp.U_SimHakAmnt,
                ERP_HAKEDISPOZCollection = new List<PozDto> {
                    new PozDto {
                        U_ToplamHakedis = pozTotal.U_ToplamHakedis,
                        U_SimdikiHakedis = pozTotal.U_SimdikiHakedis
                    }
                }
            };
        }

        public async Task<IResult> AddUpdatePozAsync(PozDto PozCreateDto)
        {
            ProgressPaymentCreateUpdateDto req = new() { ERP_HAKEDISPOZCollection = new List<PozDto> { PozCreateDto } };
            await _sapOperationsService.SapPatchServiceAsync(req, PozCreateDto.DocEntry.ToString(), "AL_ERP_HAKEDIS");
            var result = await _pozRepository.GetPozLineIdAsync(PozCreateDto.DocEntry.ToString(), PozCreateDto.U_PozNo);
            return new Result(true, result.ToString());
        }

        public async Task<IResult> DeletePozLineAsync(string docEn, string lineId, string? U_PozAdi)
        {
            await _pozRepository.DeletePozLine(docEn, lineId);
            return new Result(true);
        }

        public async Task<IDataResult<ProgressPaymentDto>> DeletePozAsync(string docEn, string lineId)
        {
            var result = await _pozRepository.DeletePoz(docEn, lineId);
            var response = _mapper.Map<ProgressPaymentDto>(result.Data);
            return new DataResult<ProgressPaymentDto>(response,true);
        }

        public async Task<IQueryable<PozDto>> GetPozAsync(string docEn)
        {
            var result = await _pozRepository.GetPozAsync(docEn);
            var response = _mapper.ProjectTo<PozDto>(result);
            return response;
        }

        public async Task<IQueryable<PozLineDto>> GetPozLineAsync(string pozName, string docEn)
        {
            var result = await _pozRepository.GetPozLineAsync(pozName, docEn);
            var response = _mapper.ProjectTo<PozLineDto>(result);
            return response;
        }

        public async Task<IQueryable<GetItemProgressPaymentDto>> GetItemsAsync()
        {
            var result = await _pozRepository.GetItemsForProgressPayment();
            var response = _mapper.ProjectTo<GetItemProgressPaymentDto>(result);
            return response;
        }

        public async Task<IQueryable<GetActivityForProgressPaymentDto>> GetActivityForProgressPaymentAsync(string budgetId)
        {
            var result = await _pozRepository.GetActivityForProgressPayment(budgetId);
            var response = _mapper.ProjectTo<GetActivityForProgressPaymentDto>(result);
            return response;
        }

        public async Task<IQueryable<GetPozCurrencyDto>> GetCurrencysForPozAsync()
        {
            var result = await _pozRepository.GetCurrencysForPozAsync();
            var response = _mapper.ProjectTo<GetPozCurrencyDto>(result);
            return response;
        }

        public async Task<IQueryable<GetVatgroupDto>> GetVatgroupForPozAsync()
        {
            var result = await _pozRepository.GetVatgroupForPozAsync();
            var response = _mapper.ProjectTo<GetVatgroupDto>(result);
            return response;
        }


    }
}
