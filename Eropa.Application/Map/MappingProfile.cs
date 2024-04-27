using AutoMapper;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Auth;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Contracts.Interruption;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Domain.Activities;
using Eropa.Domain.Auth;
using Eropa.Domain.Budgets;
using Eropa.Domain.Interruptions;
using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;

namespace Eropa.Application.Map
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDto>();
            CreateMap<ActivityDto, ActivityCreateDto>();
            CreateMap<ActivityDto, ActivityUpdateDto>();

            CreateMap<SapLoginDto, SapLogin>();
            CreateMap<DbInfos, DbInfosDto>();

            CreateMap<BudgetReportFilterDto, BudgetReportFilter>();
            CreateMap<BudgetReport, BudgetReportDto>();
            CreateMap<Budget, BudgetDto>();
            CreateMap<BudgetDetail, BudgetDetailDto>();
            CreateMap<BudgetDetailDto, BudgetDetailCreateOrUpdateDto>();
            CreateMap<BudgetDetail, BudgetDetailCreateOrUpdateDto>();
            CreateMap<BudgetDto, BudgetUpdateDto>();
            CreateMap<CurrencyCurr, CurrencyCurrDto>();

            CreateMap<GetCurrency, GetCurrencyDto>();
            CreateMap<Project, ProjectDto>();
            CreateMap<GetItem,GetItemDto>();
            CreateMap<GetMeasurementUnit, GetMeasurementUnitDto>();


            CreateMap<Interruption, InterruptionDto>();
            CreateMap<InterruptionDto, InterruptionCreateDto>();
            CreateMap<InterruptionDto, InterruptionUpdateDto>();
            CreateMap<AccountCode, AccountCodeDto>();

            CreateMap<ProgressPayment, ProgressPaymentDto>();
            CreateMap<ContractNumber, ContractNumberDto>();
            CreateMap<EmployeeMasterData, EmployeeMasterDataDto>();
            CreateMap<WithholdingTax, WithholdingTaxDto>();
            CreateMap<PurchaseInvoiceInfo, PurchaseInvoiceInfoDto>();
            CreateMap<PurchaseInvoiceLineInfo, PurchaseInvoiceLineInfoDto>();

            CreateMap<Poz, PozDto>();
            CreateMap<PozLine, PozLineDto>();
            CreateMap<SelectPoz, SelectPozDto>();
            CreateMap<GetPozCurrency, GetPozCurrencyDto>();
            CreateMap<GetVatgroup, GetVatgroupDto>();
            CreateMap<ApprovalUser, ApprovalUserDto>();

            CreateMap<Tutanak,TutanakDto>();
            CreateMap<TutanakNo, TutanakNoDto>();
            CreateMap<InvoiceInfo, InvoiceInfoDto>();
            CreateMap<InvoiceLineInfo, InvoiceLineInfoDto>();

            CreateMap<GetItemProgressPayment, GetItemProgressPaymentDto>();
            CreateMap<GetActivityForProgressPayment, GetActivityForProgressPaymentDto>();
        }
    }
}
