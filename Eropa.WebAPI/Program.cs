using Autofac;
using Autofac.Extensions.DependencyInjection;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Auth;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Contracts.Interruption;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Application.DependecyResolvers;
using Eropa.Application.Map;
using Eropa.Application.Validation;
using Eropa.Domain.ProgressPayments;
using Eropa.Eropa.Persistence.DependecyResolvers;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<ActivityDto>("Activities");
    builder.EntitySet<DbInfosDto>("Auth");




    var progressPayment = builder.EntitySet<ProgressPaymentDto>("ProgressPayment");

    var getContractNumber = progressPayment.EntityType.Collection.Function("GetContractNumber");
    getContractNumber.ReturnsCollectionFromEntitySet<ContractNumberDto>("GetContractNumber");

    var getEmployeeMasterData = progressPayment.EntityType.Collection.Function("GetEmployeeMasterData");
    getEmployeeMasterData.ReturnsCollectionFromEntitySet<EmployeeMasterDataDto>("GetEmployeeMasterData");

    var getWithholdingTax = progressPayment.EntityType.Collection.Function("GetWithholdingTax");
    getWithholdingTax.ReturnsCollectionFromEntitySet<WithholdingTaxDto>("GetWithholdingTax");

    var getActivityForProgressPayment = progressPayment.EntityType.Collection.Function("GetActivityForProgressPayment");
    getActivityForProgressPayment.ReturnsCollectionFromEntitySet<GetActivityForProgressPaymentDto>("GetActivityForProgressPayment");



    var poz = builder.EntitySet<PozLineDto>("Poz");

    var getPoz = poz.EntityType.Collection.Function("GetPozs");
    getPoz.ReturnsCollectionFromEntitySet<PozDto>("GetPozs");

    var getSelectPoz = poz.EntityType.Collection.Function("GetSelectPoz");
    getSelectPoz.ReturnsCollectionFromEntitySet<SelectPozDto>("GetSelectPoz");

    var getItems = poz.EntityType.Collection.Function("GetItemsPoz");
    getItems.ReturnsCollectionFromEntitySet<GetItemProgressPaymentDto>("GetItemsPoz");

    var getActivityPoz = poz.EntityType.Collection.Function("GetActivityPoz");
    getActivityPoz.ReturnsCollectionFromEntitySet<GetActivityForProgressPaymentDto>("GetActivityPoz");

    var getPozCurrencies = poz.EntityType.Collection.Function("GetPozCurrencies");
    getPozCurrencies.ReturnsCollectionFromEntitySet<GetPozCurrencyDto>("GetPozCurrencies");

    var getPozVatGroups = poz.EntityType.Collection.Function("GetPozVatGroups");
    getPozVatGroups.ReturnsCollectionFromEntitySet<GetVatgroupDto>("GetPozVatGroups");






    var interruption = builder.EntitySet<InterruptionDto>("Interruption");

    var getAccountCodes = interruption.EntityType.Collection.Function("GetAccountCodes");
    getAccountCodes.ReturnsCollectionFromEntitySet<AccountCodeDto>("GetAccountCodes");



    var tutanak = builder.EntitySet<TutanakDto>("Tutanak");




    var budgets = builder.EntitySet<BudgetDto>("Budget");

    var getProjectFunction = budgets.EntityType.Collection.Function("GetProject");
    getProjectFunction.ReturnsCollectionFromEntitySet<ProjectDto>("GetProject");




    var budgetDetails = builder.EntitySet<BudgetDetailDto>("BudgetDetail");

    var getItemsForBudgetDetailFunction = budgetDetails.EntityType.Collection.Function("GetItems");
    getItemsForBudgetDetailFunction.ReturnsCollectionFromEntitySet<GetItemDto>("GetItems");

    var getMeasurementUnitsForBudgetDetailFunction = budgetDetails.EntityType.Collection.Function("GetMeasurementUnits");
    getMeasurementUnitsForBudgetDetailFunction.ReturnsCollectionFromEntitySet<GetMeasurementUnitDto>("GetMeasurementUnits");

    var getCurrenciesFunction = budgetDetails.EntityType.Collection.Function("GetCurrencies");
    getCurrenciesFunction.ReturnsCollectionFromEntitySet<GetCurrencyDto>("GetCurrencies");

    return builder.GetEdmModel();
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers()
    .AddOData(opt => opt.AddRouteComponents("oData", GetEdmModel()).EnableQueryFeatures())
    .AddJsonOptions(i => i.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddFluentValidationServices();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(i => { i.RegisterModule(new ApplicationConfigurationModule()); });

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(i => { i.RegisterModule(new PersistenceConfigurationModule()); });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseODataBatching();
app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
      name: "AllowAllOrigins",
      builder =>
      {
          builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
      });
});
