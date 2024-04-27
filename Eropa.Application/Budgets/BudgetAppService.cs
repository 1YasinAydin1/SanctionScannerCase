using AutoMapper;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Budgets;
using Eropa.Domain.Budgets;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using ExcelDataReader;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Eropa.Application.Budgets
{
    public class BudgetAppService : IBudgetAppService
    {
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IBudgetDetailRepository _budgetDetailRepository;
        private readonly IActivityAppService _activityAppService;
        private readonly IMapper _mapper;
        public BudgetAppService(IBudgetRepository budgetRepository, IMapper mapper, ISapOperationsService sapOperationsService, IBudgetDetailRepository budgetDetailRepository, IActivityAppService activityAppService)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
            _sapOperationsService = sapOperationsService;
            _budgetDetailRepository = budgetDetailRepository;
            _activityAppService = activityAppService;
        }

        public async Task<IQueryable<BudgetDto>> GetBadgetAsync(string DocEn = "")
        {
            var result = await _budgetRepository.GetBudget(DocEn);
            var response = _mapper.ProjectTo<BudgetDto>(result);
            return response;
        }

        public async Task<IQueryable<CurrencyCurrDto>> GetCurrencyAsync()
        {
            var result = await _budgetRepository.GetCurrency();
            var response = _mapper.ProjectTo<CurrencyCurrDto>(result);
            return response;
        }

        public async Task<IQueryable<ProjectDto>> GetProjectAsync()
        {
            var result = await _budgetRepository.GetProject();
            var response = _mapper.ProjectTo<ProjectDto>(result);
            return response;
        }

        public async Task<IResult> UpdateBudgetAsync(BudgetUpdateDto budgetUpdateDto)
        {
            if (budgetUpdateDto.U_UsdKur != 0 && budgetUpdateDto.U_TemelUSDKur == 0) budgetUpdateDto.U_TemelUSDKur = budgetUpdateDto.U_UsdKur;
            if (budgetUpdateDto.U_EURKur != 0 && budgetUpdateDto.U_TemelEURKur == 0) budgetUpdateDto.U_TemelEURKur = budgetUpdateDto.U_EURKur;

            var result = (await _budgetRepository.GetBudget(budgetUpdateDto.DocEntry.ToString())).ToList();
            if (result[0].U_UsdKur != budgetUpdateDto.U_UsdKur)
            {
                budgetUpdateDto.ERP_PRJ_BUTCE_LINECollection = new();
                BudgetUpdateLineDto dto = new();
                var response = await _sapOperationsService.SapGetApiAsync("AL_ERP_PRJ_BUTCE", $"{budgetUpdateDto.DocEntry}");
                JObject jsonObject = JObject.Parse(response.Data);
                foreach (var item in jsonObject["ERP_PRJ_BUTCE_LINECollection"])
                    budgetUpdateDto.ERP_PRJ_BUTCE_LINECollection.Add(new BudgetUpdateLineDto
                    {
                        LineId = int.Parse(item["LineId"].ToString()),
                        U_AktiviteKodu = item["U_AktiviteKodu"].ToString(),
                        U_ToplamTutar_USD = Math.Round((double.Parse(item["U_ToplamTutar_TRY"].ToString()) / budgetUpdateDto.U_UsdKur) ?? 0, 2)
                    });

                budgetUpdateDto.U_ToplamTutar_USD = budgetUpdateDto.ERP_PRJ_BUTCE_LINECollection.Where(i=>i.U_AktiviteKodu.Length==6).Sum(i => i.U_ToplamTutar_USD);
            }
            if (result[0].U_EURKur != budgetUpdateDto.U_EURKur)
            {
                budgetUpdateDto.ERP_PRJ_BUTCE_LINECollection = new();
                BudgetUpdateLineDto dto = new();
                var response = await _sapOperationsService.SapGetApiAsync("AL_ERP_PRJ_BUTCE", $"{budgetUpdateDto.DocEntry}");
                JObject jsonObject = JObject.Parse(response.Data);
                foreach (var item in jsonObject["ERP_PRJ_BUTCE_LINECollection"])
                    budgetUpdateDto.ERP_PRJ_BUTCE_LINECollection.Add(new BudgetUpdateLineDto
                    {
                        LineId = int.Parse(item["LineId"].ToString()),
                        U_AktiviteKodu = item["U_AktiviteKodu"].ToString(),
                        U_ToplamTutar_EUR = Math.Round((double.Parse(item["U_ToplamTutar_TRY"].ToString()) / budgetUpdateDto.U_EURKur) ?? 0, 2)
                    });

                budgetUpdateDto.U_ToplamTutar_EUR = budgetUpdateDto.ERP_PRJ_BUTCE_LINECollection.Where(i => i.U_AktiviteKodu.Length == 6).Sum(i => i.U_ToplamTutar_EUR);
            }

            budgetUpdateDto.U_BelgeTarihi = DateControl(budgetUpdateDto.U_BelgeTarihi);
            budgetUpdateDto.U_BaslangicTarihi = DateControl(budgetUpdateDto.U_BaslangicTarihi);
            budgetUpdateDto.U_BitisTarihi = DateControl(budgetUpdateDto.U_BitisTarihi);
            budgetUpdateDto.U_SonlandirmaTarihi = DateControl(budgetUpdateDto.U_SonlandirmaTarihi);

            await _sapOperationsService.SapPatchServiceAsync(budgetUpdateDto, $"{budgetUpdateDto.DocEntry}", "AL_ERP_PRJ_BUTCE");
            return new Result(true);
        }

        private DateTime? DateControl(DateTime? date)
        {
            if (date != null)
                if (date.Value.Hour == 21)
                    date = date.Value.AddHours(3);
            return date;
        }

        public async Task<IResult> BudgetEmptyRecordAdd()
        {
            BudgetCreateDto reqBudget = new BudgetCreateDto { U_BelgeNo = await _budgetRepository.GetDocumentNo(), U_Durum = "1" };
            await _sapOperationsService.SapPostServiceAsync(reqBudget, "AL_ERP_PRJ_BUTCE");
            return new Result(true);
        }

        public async Task<IResult> BudgetExcelImportAsync(string excelBase64)
        {
            try
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(excelBase64.Substring(78));
                MemoryStream stream = new MemoryStream(base64EncodedBytes, 0, base64EncodedBytes.Length);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
                DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });

                var table = dataSet.Tables[0].AsEnumerable();

                var header = table.First();
                string documentNo = await _budgetRepository.GetDocumentNo();
                BudgetCreateDto reqBudget = new BudgetCreateDto
                {
                    U_BelgeNo = documentNo,
                    U_BaslangicTarihi = header.Field<DateTime?>("Bütçe Başlangıç Tarihi"),//DateTime.ParseExact(header.Field<string>("Bütçe Başlangıç Tarihi")!, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                    U_BitisTarihi = header.Field<DateTime?>("Bütçe Bitiş Tarihi"),//DateTime.ParseExact(header.Field<string>("Bütçe Bitiş Tarihi")!, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                    U_Durum = header.Field<string>("Durum").Equals("Açık") ? "1" : "2",
                    U_Proje = header.Field<string>("Proje Kodu"),
                    U_UsdKur = header.Field<double>("USD Kuru"),
                    U_EURKur = header.Field<double>("EUR Kuru"),
                    U_Revizyon = header.Field<string>("Revizyon Durumu"),
                    U_Aciklama = header.Field<string>("Açıklama"),
                    U_TemelUSDKur = header.Field<double>("Temel USD Kur"),
                    U_TemelEURKur = header.Field<double>("Temel EUR Kur"),
                };
                var lineList = table.Select(m => new BudgetDetailCreateOrUpdateDto
                {
                    U_Proje = header.Field<string>("Proje Kodu"),
                    U_BelgeNo = documentNo,
                    U_AktiviteKodu = m.Field<string>("Aktivite Kodu"),
                    U_AktiviteAdi = m.Field<string>("Aktivite Adı"),
                    U_KalemKodu = m.Field<string>("Kalem Kodu"),
                    U_Birim = m.Field<string>("Birim"),
                    U_kisim = m.Field<string>("Kısım"),
                    U_BirimFiyatReferans = m.Field<string>("Birim fiyat referans alanı"),
                    U_ParaBirimi = m.Field<string>("Para Birimi"),
                    U_Kur = m.Field<double?>("Kur"),
                    U_BirimFiyat = m.Field<double?>("Birim Fiyat"),
                    U_Miktar = m.Field<double?>("Miktar"),
                    U_planbaslangic = m.Field<DateTime?>("Planlanan Başlangıç"),//!string.IsNullOrEmpty(m.Field<string>("Planlanan Başlangıç")) ? DateTime.ParseExact(m.Field<string>("Planlanan Başlangıç"), "dd.MM.yyyy", CultureInfo.InvariantCulture) : null,
                    U_planbitis = m.Field<DateTime?>("Planlanan Bitiş"),//!string.IsNullOrEmpty(m.Field<string>("Planlanan Bitiş")) ? DateTime.ParseExact(m.Field<string>("Planlanan Bitiş"), "dd.MM.yyyy", CultureInfo.InvariantCulture) : null
                }).ToList();

                List<string> errors = new List<string>();

                reqBudget.ERP_PRJ_BUTCE_LINECollection = await CalculatingBudget(lineList, reqBudget, errors);
                errors.Reverse();
                if (errors.Count > 0)
                    return new Result(false, errors);

                reqBudget.U_ToplamTutar_TRY = HeaderTotalTry;
                reqBudget.U_ToplamTutar_USD = HeaderTotalUsd;
                reqBudget.U_ToplamTutar_EUR = HeaderTotalEur;
                reqBudget.U_TemelToplamTutar_TRY = HeaderTotalTry;
                reqBudget.U_TemelToplamTutar_USD = HeaderTotalUsd;
                reqBudget.U_TemelToplamTutar_EUR = HeaderTotalEur;

                await _sapOperationsService.SapPostServiceAsync(reqBudget, "AL_ERP_PRJ_BUTCE");
                return new Result(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Excel içeri aktarılırken bir hata oluştu: " + ex.Message);
            }
        }
        private double? HeaderTotalTry = 0, HeaderTotalUsd = 0, HeaderTotalEur = 0;
        private double? LevelTwoTotalTry = 0, LevelTwoTotalUsd = 0, LevelTwoTotalEur = 0;
        private double? LevelOneTotalTry = 0, LevelOneTotalUsd = 0, LevelOneTotalEur = 0;
        private DateTime? planbaslangic, planbitis, baslangic, bitis;
        private async Task<List<BudgetDetailCreateOrUpdateDto>> CalculatingBudget(List<BudgetDetailCreateOrUpdateDto> lineList, BudgetCreateDto reqBudget, List<string> errors)
        {
            try
            {
                var projects = await _budgetRepository.GetProject();
                var activities = await _activityAppService.GetActivities();
                var items = await _budgetDetailRepository.GetItemsForBudgetDetail();
                var measurementUnits = await _budgetDetailRepository.GetMeasurementUnitsForBudgetDetail();
                var currencies = await _budgetDetailRepository.GetCurrencysForBudgetDetail();
                bool isError = false;
                for (int i = lineList.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(lineList[i].U_AktiviteKodu)) { errors.Add($"{i + 1}. Satır Hatası : Aktivite Kodu Boş Bırakılamaz."); isError = true; }
                    else if (!string.IsNullOrEmpty(lineList[i].U_AktiviteKodu) && !activities.Any(x => x.U_AktiviteKodu == lineList[i].U_AktiviteKodu)) { errors.Add($"{i + 1}. Satır Hatası : {lineList[i].U_AktiviteKodu} Aktivite  Kodu Bulunamadı."); isError = true; }
                    if (!string.IsNullOrEmpty(lineList[i].U_Proje) && !projects.Any(x => x.PrjCode == lineList[i].U_Proje)) { errors.Add($"{i + 1}. Satır Hatası : {lineList[i].U_Proje} Proje Kodu Bulunamadı."); isError = true; }
                    if (!string.IsNullOrEmpty(lineList[i].U_KalemKodu) && !items.Any(x => x.ItemCode == lineList[i].U_KalemKodu)) { errors.Add($"{i + 1}. Satır Hatası : {lineList[i].U_KalemKodu} Kalem Kodu Kodu Bulunamadı."); isError = true; }
                    if (!string.IsNullOrEmpty(lineList[i].U_Birim) && !measurementUnits.Any(x => x.UomCode == lineList[i].U_Birim)) { errors.Add($"{i + 1}. Satır Hatası : {lineList[i].U_Birim} Birim Kodu Bulunamadı."); isError = true; }
                    if (!string.IsNullOrEmpty(lineList[i].U_ParaBirimi) && !currencies.Any(x => x.CurrCode == lineList[i].U_ParaBirimi)) { errors.Add($"{i + 1}. Satır Hatası : {lineList[i].U_ParaBirimi} Para Birimi Kodu Bulunamadı."); isError = true; }

                    if (isError) { continue; }
                    switch (lineList[i].U_AktiviteKodu.Length)
                    {
                        case 2:
                            InitializeDetailOneOrTwo(lineList[i], planbaslangic, planbitis, baslangic, bitis, LevelOneTotalTry, LevelOneTotalUsd, LevelOneTotalEur);
                            LevelOneTotalTry = 0; LevelOneTotalUsd = 0; LevelOneTotalEur = 0; planbaslangic = null; planbitis = null; baslangic = null; bitis = null;
                            break;
                        case 4:
                            InitializeDetailOneOrTwo(lineList[i], planbaslangic, planbitis, baslangic, bitis, LevelTwoTotalTry, LevelTwoTotalUsd, LevelTwoTotalEur);
                            LevelTwoTotalTry = 0; LevelTwoTotalUsd = 0; LevelTwoTotalEur = 0;
                            break;
                        case 6:
                            InitializeDetailLevelThree(lineList[i], reqBudget.U_UsdKur, reqBudget.U_EURKur);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lineList;
        }

        public BudgetDetailCreateOrUpdateDto InitializeDetailLevelThree(BudgetDetailCreateOrUpdateDto request, double? usdCurr, double? eurCurr)
        {
            request.U_ToplamTutar = request!.U_BirimFiyat * request.U_Miktar;
            double? TRYAmount = request.U_ToplamTutar * request.U_Kur;

            request.U_ToplamTutar_TRY = TRYAmount;
            HeaderTotalTry += request.U_ToplamTutar_TRY;
            LevelTwoTotalTry += request.U_ToplamTutar_TRY;
            LevelOneTotalTry += request.U_ToplamTutar_TRY;

            request.U_ToplamTutar_USD = TRYAmount / usdCurr;
            HeaderTotalUsd += request.U_ToplamTutar_USD;
            LevelTwoTotalUsd += request.U_ToplamTutar_USD;
            LevelOneTotalUsd += request.U_ToplamTutar_USD;

            request.U_ToplamTutar_EUR = TRYAmount / eurCurr;
            HeaderTotalEur += request.U_ToplamTutar_EUR;
            LevelTwoTotalEur += request.U_ToplamTutar_EUR;
            LevelOneTotalEur += request.U_ToplamTutar_EUR;

            request.U_TemelMiktar = request.U_Miktar;
            request.U_TemelBirimFiyat = request.U_BirimFiyat;

            request.U_TemelToplamTutar_TRY = request.U_ToplamTutar_TRY;
            request.U_TemelToplamTutar_USD = request.U_ToplamTutar_USD;
            request.U_TemelToplamTutar_EUR = request.U_ToplamTutar_EUR;

            if (request.U_planbaslangic < planbaslangic || request.U_planbaslangic == null || planbaslangic == null)
                planbaslangic = request.U_planbaslangic;
            if (request.U_planbitis > planbitis || request.U_planbitis == null || planbitis == null)
                planbitis = request.U_planbitis;
            if (request.U_baslangic < baslangic || request.U_baslangic == null || baslangic == null)
                baslangic = request.U_baslangic;
            if (request.U_bitis > bitis || request.U_bitis == null || bitis == null)
                bitis = request.U_bitis;

            return request;
        }

        public BudgetDetailCreateOrUpdateDto InitializeDetailOneOrTwo(BudgetDetailCreateOrUpdateDto request, DateTime? planbaslangic, DateTime? planbitis, DateTime? baslangic, DateTime? bitis, double? TotalTry, double? TotalUsd, double? TotalEur)
        {
            request.U_ToplamTutar_TRY = TotalTry;
            request.U_ToplamTutar_USD = TotalUsd;
            request.U_ToplamTutar_EUR = TotalEur;
            request.U_TemelToplamTutar_TRY = request.U_ToplamTutar_TRY;
            request.U_TemelToplamTutar_USD = request.U_ToplamTutar_USD;
            request.U_TemelToplamTutar_EUR = request.U_ToplamTutar_EUR;
            request.U_planbaslangic = planbaslangic;
            request.U_planbitis = planbitis;
            request.U_baslangic = baslangic;
            request.U_bitis = bitis;
            return request;
        }

        public async Task<IResult> RevisedBudgetAsync(string revisedDocEn)
        {
            string jsonStringBudget = (await _sapOperationsService.SapGetApiAsync("AL_ERP_PRJ_BUTCE", revisedDocEn)).Data;
            JObject jsonObject = JObject.Parse(jsonStringBudget);

            string revised = await _budgetRepository.GetRevised(jsonObject["U_Proje"].ToString());
            if (revised.StartsWith("R"))
                jsonObject["U_Revizyon"] = $"R{int.Parse(revised.Substring(1)) + 1}";
            else jsonObject["U_Revizyon"] = "R1";

            jsonStringBudget = jsonObject.ToString();
            await _sapOperationsService.SapPostServiceAsync<string>(jsonStringBudget, "AL_ERP_PRJ_BUTCE");
            return new Result(true);
        }

        public async Task<IResult> DeleteBudgetAsync(string key)
        {
            await _sapOperationsService.SapDeleteServiceAsync(key, "AL_ERP_PRJ_BUTCE");
            return new Result(true);
        }
    }
}
