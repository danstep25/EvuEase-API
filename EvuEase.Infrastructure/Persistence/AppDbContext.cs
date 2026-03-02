using EvuEase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EvuEase.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Cache> Cache { get; set; }
    public DbSet<CacheLock> CacheLocks { get; set; }
    public DbSet<FailedJob> FailedJobs { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobBatch> JobBatches { get; set; }
    public DbSet<Migration> Migrations { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }
    
    public DbSet<ClassRoster> ClassRosters { get; set; }
    public DbSet<Curricula> Curricula { get; set; }
    public DbSet<DpPercentage> DpPercentages { get; set; }
    public DbSet<GradeRoster> GradeRosters { get; set; }
    public DbSet<MiscellaneousFee> MiscellaneousFees { get; set; }
    public DbSet<OtherSchoolFee> OtherSchoolFees { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<TuitionFee> TuitionFees { get; set; }
    
    public DbSet<Program> Programs { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<SyTerm> SyTerms { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

