using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eropa.Application.Validation
{
    public static class FluentValidationService
    {
        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BudgetDetailValidation>());
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BudgetDetailCreateOrUpdateValidation>());
            return services;
        }
    }
}
