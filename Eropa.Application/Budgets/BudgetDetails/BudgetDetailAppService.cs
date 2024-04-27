using Autofac.Features.Metadata;
using AutoMapper;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Validation;
using Eropa.Domain.Activities;
using Eropa.Domain.Budgets;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eropa.Application.Budgets
{
    public class BudgetDetailAppService : IBudgetDetailAppService
    {
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IBudgetDetailRepository _budgetDetailRepository;
        private readonly IBudgetAppService _budgetAppService;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;

        public BudgetDetailAppService(IMapper mapper, IBudgetDetailRepository budgetDetailRepository, ISapOperationsService sapOperationsService, IBudgetAppService budgetAppService, IActivityRepository activityRepository)
        {
            _mapper = mapper;
            _budgetDetailRepository = budgetDetailRepository;
            _sapOperationsService = sapOperationsService;
            _budgetAppService = budgetAppService;
            _activityRepository = activityRepository;
        }

        public async Task<IResult> AddBudgetDetailAsync(BudgetDetailDto budgetCreateDto, string docEn)
        {
            var request = _mapper.Map<BudgetDetailCreateOrUpdateDto>(budgetCreateDto);
            if (budgetCreateDto.U_AktiviteKodu.Length != 6)
            {
                BudgetCreateDto budgetCreate = new BudgetCreateDto { ERP_PRJ_BUTCE_LINECollection = new List<BudgetDetailCreateOrUpdateDto> { request } };
                await _sapOperationsService.SapPatchServiceAsync(budgetCreate, docEn, "AL_ERP_PRJ_BUTCE");
            }
            else
            {

                var budgetHeader = (await _budgetAppService.GetBadgetAsync(docEn)).FirstOrDefault();

                var budgetDetailLevelTwo = await NoExistParentForAdd(budgetCreateDto.U_AktiviteKodu.Substring(0, 4), docEn, budgetCreateDto.U_kisim);
                var budgetDetailLevelOne = await NoExistParentForAdd(budgetCreateDto.U_AktiviteKodu.Substring(0, 2), docEn);

                var updateBudgetDetailLevelTwo = _mapper.Map<BudgetDetailCreateOrUpdateDto>(budgetDetailLevelTwo);
                var updateBudgetDetailLevelOne = _mapper.Map<BudgetDetailCreateOrUpdateDto>(budgetDetailLevelOne);
                InitializeDetail(ref request, null, budgetHeader.U_UsdKur, budgetHeader.U_EURKur);
                request.U_TemelToplamTutar_TRY = request.U_ToplamTutar_TRY;
                request.U_TemelToplamTutar_USD = request.U_ToplamTutar_USD;
                request.U_TemelToplamTutar_EUR = request.U_ToplamTutar_EUR;

                BudgetCreateDto budgetCreate = new BudgetCreateDto { ERP_PRJ_BUTCE_LINECollection = new List<BudgetDetailCreateOrUpdateDto> { request } };
                await _sapOperationsService.SapPatchServiceAsync(budgetCreate, docEn, "AL_ERP_PRJ_BUTCE");

                await addOrUpdateDetailRow(updateBudgetDetailLevelTwo, true, request.U_ToplamTutar_TRY, request.U_ToplamTutar_USD, request.U_ToplamTutar_EUR, request, docEn);

                await addOrUpdateDetailRow(updateBudgetDetailLevelOne, true, request.U_ToplamTutar_TRY, request.U_ToplamTutar_USD, request.U_ToplamTutar_EUR, request, docEn);

                BudgetUpdateDto budgetDetailCreateOrUpdateDto = _mapper.Map<BudgetUpdateDto>(budgetHeader);
                budgetDetailCreateOrUpdateDto.U_ToplamTutar_TRY += request.U_ToplamTutar_TRY;
                budgetDetailCreateOrUpdateDto.U_ToplamTutar_USD += request.U_ToplamTutar_USD;
                budgetDetailCreateOrUpdateDto.U_ToplamTutar_EUR += request.U_ToplamTutar_EUR;
                budgetDetailCreateOrUpdateDto.U_TemelToplamTutar_TRY += request.U_TemelToplamTutar_TRY;
                budgetDetailCreateOrUpdateDto.U_TemelToplamTutar_USD += request.U_TemelToplamTutar_USD;
                budgetDetailCreateOrUpdateDto.U_TemelToplamTutar_EUR += request.U_TemelToplamTutar_EUR;
                await _budgetAppService.UpdateBudgetAsync(budgetDetailCreateOrUpdateDto);
            }

            return new Result(true);
        }

        private async Task<BudgetDetail> NoExistParentForAdd(string activityCode, string docEn, string part = "")
        {
            var budgetDetailParent = (await _budgetDetailRepository.GetSingleBudgetDetailByActivityCode(docEn, activityCode, part)).FirstOrDefault();
            if (budgetDetailParent == null)
            {
                var activity = await _activityRepository.GetActivitiesAsync(activityCode);
                if (activity == null)
                    throw new Exception($"{activityCode} Aktivite Kodu Bulunamadı!");
                BudgetDetailCreateOrUpdateDto detailParent = new BudgetDetailCreateOrUpdateDto
                {
                    U_AktiviteKodu = activityCode,
                    U_AktiviteAdi = activity.Select(i => i.U_AktiviteTanimi).First()
                };
                BudgetCreateDto budgetCreateParent = new BudgetCreateDto { ERP_PRJ_BUTCE_LINECollection = new List<BudgetDetailCreateOrUpdateDto> { detailParent } };
                await _sapOperationsService.SapPatchServiceAsync(budgetCreateParent, docEn, "AL_ERP_PRJ_BUTCE");
                budgetDetailParent = (await _budgetDetailRepository.GetSingleBudgetDetailByActivityCode(docEn, activityCode, part)).FirstOrDefault();
            }
            return budgetDetailParent;
        }
        public async Task<IResult> UpdateBudgetDetailAsync(BudgetDetailDto budgetUpdateDto, string key, string docEn)
        {
            try
            {
                string lineId = await _budgetDetailRepository.GetLineIdByRowNum(docEn, key);
                var budgetDetail = (await _budgetDetailRepository.GetSingleBudgetDetail(docEn, lineId)).FirstOrDefault();
                if (budgetDetail == null)
                    return new Result(false, $"Güncellenecek Satır Bulunamadı! (Silinmiş veya Değiştirilmiş Olabilir. Sayfayı yenileyiniz.)");

                var dbDetail = JObject.Parse(JsonConvert.SerializeObject(budgetDetail, Newtonsoft.Json.Formatting.None,
                                                         new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                var requestDetail = JObject.Parse(JsonConvert.SerializeObject(_mapper.Map<BudgetDetailCreateOrUpdateDto>(budgetUpdateDto)));

                MergeInstances(dbDetail, requestDetail);
                var request = requestDetail.ToObject<BudgetDetailCreateOrUpdateDto>();
                var validationResult = (new BudgetDetailCreateOrUpdateValidation()).Validate(request);
                if (!validationResult.IsValid)
                    return new Result(false, validationResult.Errors.First().ErrorMessage);

                if (request.U_AktiviteKodu.Length != 6)
                {
                    BudgetCreateDto budgetCreate = new BudgetCreateDto { ERP_PRJ_BUTCE_LINECollection = new List<BudgetDetailCreateOrUpdateDto> { request } };
                    budgetCreate.ERP_PRJ_BUTCE_LINECollection[0].LineId = int.Parse(lineId);
                    await _sapOperationsService.SapPatchServiceAsync(budgetCreate, docEn, "AL_ERP_PRJ_BUTCE");
                }
                else
                {
                    var budgetHeader = (await _budgetAppService.GetBadgetAsync(docEn)).FirstOrDefault();
                    var budgetDetailLevelTwo = (await _budgetDetailRepository.GetSingleBudgetDetailByActivityCode(docEn, budgetDetail.U_AktiviteKodu.Substring(0, 4), budgetDetail.U_kisim)).FirstOrDefault();
                    var budgetDetailLevelOne = (await _budgetDetailRepository.GetSingleBudgetDetailByActivityCode(docEn, budgetDetail.U_AktiviteKodu.Substring(0, 2))).FirstOrDefault();
                    var updateBudgetDetailLevelTwo = _mapper.Map<BudgetDetailCreateOrUpdateDto>(budgetDetailLevelTwo);
                    var updateBudgetDetailLevelOne = _mapper.Map<BudgetDetailCreateOrUpdateDto>(budgetDetailLevelOne);

                    InitializeDetail(ref request, budgetDetail, budgetHeader.U_UsdKur, budgetHeader.U_EURKur);

                    double? TransferedTotalTRY = request.U_ToplamTutar_TRY - (budgetDetail.U_ToplamTutar_TRY ?? 0);
                    double? TransferedTotalUSD = request.U_ToplamTutar_USD - (budgetDetail.U_ToplamTutar_USD ?? 0);
                    double? TransferedTotalEUR = request.U_ToplamTutar_EUR - (budgetDetail.U_ToplamTutar_EUR ?? 0);

                    if (budgetDetail.U_TemelToplamTutar_TRY == 0 || budgetDetail.U_TemelToplamTutar_TRY == null)
                    {
                        request.U_TemelToplamTutar_TRY = request.U_ToplamTutar_TRY;
                        request.U_TemelToplamTutar_USD = request.U_ToplamTutar_USD;
                        request.U_TemelToplamTutar_EUR = request.U_ToplamTutar_EUR;
                    }
                    BudgetCreateDto budgetCreate = new BudgetCreateDto { ERP_PRJ_BUTCE_LINECollection = new List<BudgetDetailCreateOrUpdateDto> { request } };
                    budgetCreate.ERP_PRJ_BUTCE_LINECollection[0].LineId = int.Parse(lineId);
                    await _sapOperationsService.SapPatchServiceAsync(budgetCreate, docEn, "AL_ERP_PRJ_BUTCE");

                    await addOrUpdateDetailRow(updateBudgetDetailLevelTwo, budgetDetail.U_TemelToplamTutar_TRY == 0 || budgetDetail.U_TemelToplamTutar_TRY == null
                                        , TransferedTotalTRY, TransferedTotalUSD, TransferedTotalEUR, request, docEn);

                    await addOrUpdateDetailRow(updateBudgetDetailLevelOne, budgetDetail.U_TemelToplamTutar_TRY == 0 || budgetDetail.U_TemelToplamTutar_TRY == null
                                        , TransferedTotalTRY, TransferedTotalUSD, TransferedTotalEUR, request, docEn);

                    BudgetUpdateDto budgetDetailCreateOrUpdateDto = _mapper.Map<BudgetUpdateDto>(budgetHeader);
                    budgetDetailCreateOrUpdateDto.U_ToplamTutar_TRY += TransferedTotalTRY;
                    budgetDetailCreateOrUpdateDto.U_ToplamTutar_USD += TransferedTotalUSD;
                    budgetDetailCreateOrUpdateDto.U_ToplamTutar_EUR += TransferedTotalEUR;
                    if (budgetDetail.U_TemelToplamTutar_TRY == 0 || budgetDetail.U_TemelToplamTutar_TRY == null)
                    {
                        budgetDetailCreateOrUpdateDto.U_TemelToplamTutar_TRY += request.U_TemelToplamTutar_TRY;
                        budgetDetailCreateOrUpdateDto.U_TemelToplamTutar_USD += request.U_TemelToplamTutar_USD;
                        budgetDetailCreateOrUpdateDto.U_TemelToplamTutar_EUR += request.U_TemelToplamTutar_EUR;
                    }
                    await _budgetAppService.UpdateBudgetAsync(budgetDetailCreateOrUpdateDto);
                }
            }
            catch (Exception ex)
            {
                return new Result(false, $"Hata Oluştu : {ex.Message}");
            }

            return new Result(true);
        }

        private async Task addOrUpdateDetailRow(BudgetDetailCreateOrUpdateDto req, bool condition, double? TransferedTotalTRY, double? TransferedTotalUSD, double? TransferedTotalEUR
            , BudgetDetailCreateOrUpdateDto request, string docEn)
        {
            req.U_ToplamTutar_TRY = (req.U_ToplamTutar_TRY ?? 0) + TransferedTotalTRY;
            req.U_ToplamTutar_USD = (req.U_ToplamTutar_USD ?? 0) + TransferedTotalUSD;
            req.U_ToplamTutar_EUR = (req.U_ToplamTutar_EUR ?? 0) + TransferedTotalEUR;
            if (condition)
            {
                req.U_TemelToplamTutar_TRY = (req.U_TemelToplamTutar_TRY ?? 0) + request.U_TemelToplamTutar_TRY;
                req.U_TemelToplamTutar_USD = (req.U_TemelToplamTutar_USD ?? 0) + request.U_TemelToplamTutar_USD;
                req.U_TemelToplamTutar_EUR = (req.U_TemelToplamTutar_EUR ?? 0) + request.U_TemelToplamTutar_EUR;
            }
            if (req.U_planbaslangic > request.U_planbaslangic || req.U_planbaslangic == null)
                req.U_planbaslangic = request.U_planbaslangic;
            if (req.U_planbitis < request.U_planbitis || req.U_planbitis == null)
                req.U_planbitis = request.U_planbitis;
            if (req.U_baslangic > request.U_baslangic || req.U_baslangic == null)
                req.U_baslangic = request.U_baslangic;
            if (req.U_bitis < request.U_bitis || req.U_bitis == null)
                req.U_bitis = request.U_bitis;
            if (req.U_fark < request.U_fark || req.U_fark == 0)
                req.U_fark = request.U_fark;

            BudgetCreateDto updateBudgetLevelOne = new BudgetCreateDto { ERP_PRJ_BUTCE_LINECollection = new List<BudgetDetailCreateOrUpdateDto> { req } };
            updateBudgetLevelOne.ERP_PRJ_BUTCE_LINECollection[0].LineId = req.LineId;
            await _sapOperationsService.SapPatchServiceAsync(updateBudgetLevelOne, docEn, "AL_ERP_PRJ_BUTCE");
        }

        private static void MergeInstances(JObject instance1, JObject instance2)
        {
            foreach (var property in instance2.Properties())
                if (property.Value.Type == JTokenType.Null && instance1[property.Name] != null)
                    instance2[property.Name] = instance1[property.Name];
        }
        public void InitializeDetail(ref BudgetDetailCreateOrUpdateDto request, BudgetDetail budgetDetail, double? usdCurr, double? eurCurr)
        {
            request.U_ToplamTutar = request!.U_BirimFiyat * request.U_Miktar;
            double? TRYAmount = request.U_ToplamTutar * request.U_Kur;
            request.U_ToplamTutar_EUR = TRYAmount / eurCurr; request.U_ToplamTutar_USD = TRYAmount / usdCurr; request.U_ToplamTutar_TRY = TRYAmount;
            request.U_TemelMiktar = AssignValueForDetail(budgetDetail?.U_TemelMiktar, request.U_Miktar);
            request.U_TemelBirimFiyat = AssignValueForDetail(budgetDetail?.U_TemelBirimFiyat, request.U_BirimFiyat);
            request.U_fark = request.U_ToplamTutar_TRY - (request.U_TemelToplamTutar_TRY ?? 0);
        }
        private double? AssignValueForDetail(double? value, double? value1)
        {
            return value == 0 ? value1 : value;
        }

        public async Task<IQueryable<BudgetDetailDto>> GetBadgetDetailAsync(string docEn)
        {
            var result = await _budgetDetailRepository.GetBudgetDetail(docEn);
            var response = _mapper.ProjectTo<BudgetDetailDto>(result);
            return response;
        }

        public async Task<IResult> DeleteBudgetDetailAsync(string docEn, string key)
        {
            string lineId = await _budgetDetailRepository.GetLineIdByRowNum(docEn, key);
            return await _budgetDetailRepository.DeleteBudgetDetail(docEn, lineId);
        }


        public async Task<IQueryable<GetItemDto>> GetItemsForBudgetDetail()
        {
            var result = await _budgetDetailRepository.GetItemsForBudgetDetail();
            var response = _mapper.ProjectTo<GetItemDto>(result);
            return response;
        }

        public async Task<IQueryable<GetMeasurementUnitDto>> GetMeasurementUnitsForBudgetDetail()
        {
            var result = await _budgetDetailRepository.GetMeasurementUnitsForBudgetDetail();
            var response = _mapper.ProjectTo<GetMeasurementUnitDto>(result);
            return response;
        }

        public async Task<IQueryable<GetCurrencyDto>> GetCurrencyForBudgetDetail()
        {
            var result = await _budgetDetailRepository.GetCurrencysForBudgetDetail();
            var response = _mapper.ProjectTo<GetCurrencyDto>(result);
            return response;
        }
    }
}
