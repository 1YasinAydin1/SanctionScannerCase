using Autofac;
using Eropa.Application.Activities;
using Eropa.Application.Auth;
using Eropa.Application.Budgets;
using Eropa.Application.Contracts.Activities;
using Eropa.Application.Contracts.Auth;
using Eropa.Application.Contracts.Budgets;
using Eropa.Application.Contracts.Interruption;
using Eropa.Application.Contracts.ProgressPayments;
using Eropa.Application.Interruptions;
using Eropa.Application.ProgressPayments;

namespace Eropa.Application.DependecyResolvers
{
    public class ApplicationConfigurationModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActivityAppService>().As<IActivityAppService>();
            builder.RegisterType<AuthAppService>().As<IAuthAppService>();
            builder.RegisterType<BudgetAppService>().As<IBudgetAppService>();
            builder.RegisterType<BudgetDetailAppService>().As<IBudgetDetailAppService>();
            builder.RegisterType<BudgetReportAppService>().As<IBudgetReportAppService>();
            builder.RegisterType<InterruptionAppService>().As<IInterruptionAppService>();
            builder.RegisterType<ProgressPaymentAppService>().As<IProgressPaymentAppService>();
            builder.RegisterType<PozAppService>().As<IPozAppService>();
            builder.RegisterType<TutanakAppService>().As<ITutanakAppService>();
        }
    }
}
