using AutoMapper;
using Eropa.Application.Contracts.Activities;
using Eropa.Domain.Activities;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using ExcelDataReader;
using System.Data;

namespace Eropa.Application.Activities
{
    public class ActivityAppService : IActivityAppService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ISapOperationsService _sapOperationsService;
        private readonly IMapper _mapper;
        public ActivityAppService(IActivityRepository activityRepository, IMapper mapper, ISapOperationsService sapOperationsService)
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
            _sapOperationsService = sapOperationsService;
        }

        public async Task<IQueryable<ActivityDto>> GetActivities()
        {
            var result = await _activityRepository.GetActivitiesAsync();
            var response = _mapper.ProjectTo<ActivityDto>(result);
            return response;
        }

        public async Task<IResult> AddActivityAsync(ActivityDto activityCreateDto)
        {
            if (!await _activityRepository.GetActivityByCode(activityCreateDto.U_AktiviteKodu))
                return new Result(false, $"'{activityCreateDto.U_AktiviteKodu}' Aktivite Kodu Zaten Mevcut");
            ActivityCreateDto activityCreate = _mapper.Map<ActivityCreateDto>(activityCreateDto);
            activityCreate.Code = activityCreateDto.U_AktiviteKodu;
            var ss = await _sapOperationsService.SapPostServiceAsync(activityCreate, "AL_ERP_PROJ_ACT");
            return new Result(true);
        }

        public async Task<IResult> UpdateActivityAsync(ActivityDto activityUpdateDto, string key)
        {
            ActivityUpdateDto activityUpdate = _mapper.Map<ActivityUpdateDto>(activityUpdateDto);
            await _sapOperationsService.SapPatchServiceAsync(activityUpdate, $"'{key}'", "AL_ERP_PROJ_ACT");
            return new Result(true);
        }

        public async Task<IResult> DeleteActivityAsync(string key)
        {
            await _sapOperationsService.SapDeleteServiceAsync($"'{key}'", "AL_ERP_PROJ_ACT");
            return new Result(true);
        }

        public async Task<IResult> ActivityExcelImport(string excelBase64)
        {
            try
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(excelBase64.Substring(78));
                MemoryStream stream = new MemoryStream(base64EncodedBytes, 0, base64EncodedBytes.Length);

                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);

                DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                DataTable table = dataSet.Tables[0];

                var reqList = table.AsEnumerable().Select(m => new ActivityCreateDto
                {
                    Code = m.Field<string>("Aktivite Kodu"),
                    U_AktiviteKodu = m.Field<string>("Aktivite Kodu"),
                    U_AktiviteTanimi = m.Field<string>("Aktivite Tanımı"),
                    U_UstAktiviteKodu = m.Field<string>("Üst Aktivite Kodu"),
                    U_Durum = m.Field<string>("Durum"),
                    U_Seviye = m.Field<double>("Seviye").ToString(),
                }).ToList();


                await Task.WhenAll(reqList.Select(req => _sapOperationsService.SapPostServiceAsync(req, "AL_ERP_PROJ_ACT")));
                return new Result(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Excel içeri aktarılırken bir hata oluştu: " + ex.Message);
            }
        }

    }
}
