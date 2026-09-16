using Kip.Report.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Kip.Report.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ReportQueryDbo> Reports => Set<ReportQueryDbo>();
}   
