using Microsoft.Extensions.DependencyInjection;

namespace Kip.Report.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<QueryService>();

        return services;
    }
}
