using Kip.Report.Domain.Repositories;
using Kip.Report.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kip.Report.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connStr));

        services.AddScoped<IQueryRepository, QueryRepository>();

        return services;
    }
}
