using Scalar.AspNetCore;
using Kip.Report.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Kip.Report.Api.Endpoints;
using Kip.Report.Application;
using Kip.Report.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly, typeof(ApplicationDbContext).Assembly);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<QueryOptions>(
    builder.Configuration.GetSection("Query")
);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddSingleton<BackgroundReportQueue>();
builder.Services.AddHostedService<ReportBackgroundWorker>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await applicationDbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapReportEndpoints();

app.Run();
