
using Autofac;
using Eropa.Domain.Activities;
using Eropa.Domain.Auth;
using Eropa.Domain.Budgets;
using Eropa.Domain.Interruptions;
using Eropa.Domain.ProgressPayments;
using Eropa.Domain.SAPConnection;
using Eropa.Eropa.Persistence.SAPConnection;
using Eropa.Persistence.Activities;
using Eropa.Persistence.Auth;
using Eropa.Persistence.Budgets;
using Eropa.Persistence.Interruptions;
using Eropa.Persistence.ProgressPayments;

namespace Eropa.Eropa.Persistence.DependecyResolvers
{
    public class PersistenceConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SapOperationsService>().As<ISapOperationsService>();
            builder.RegisterType<ActivityRepository>().As<IActivityRepository>();
            builder.RegisterType<BudgetRepository>().As<IBudgetRepository>();
            builder.RegisterType<BudgetDetailRepository>().As<IBudgetDetailRepository>();
            builder.RegisterType<BudgetReportRepository>().As<IBudgetReportRepository>();
            builder.RegisterType<InterruptionRepository>().As<IInterruptionRepository>();
            builder.RegisterType<ProgressPaymentRepository>().As<IProgressPaymentRepository>();
            builder.RegisterType<PozRepository>().As<IPozRepository>();
            builder.RegisterType<TutanakRepository>().As<ITutanakRepository>();
            builder.RegisterType<AuthRepository>().As<IAuthRepository>();
        }
    }
}
