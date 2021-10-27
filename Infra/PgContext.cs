using Core;
using Microsoft.EntityFrameworkCore;

namespace Infra
{
    public class PgContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseNpgsql("Host=localhost;Database=wip_cdc19;Username=;Password=")
                .UseSnakeCaseNamingConvention();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(EntityConfigurations).Assembly);

#nullable disable
        public DbSet<CovidCase> CovidCase { get; set; }
        public DbSet<AgeGroup> AgeGroup { get; set; }
        public DbSet<CurrentStatus> CurrentStatus { get; set; }
        public DbSet<Ethnicity> Ethnicity { get; set; }
        public DbSet<Process> Process { get; set; }
        public DbSet<Race> Race { get; set; }
        public DbSet<Sex> Sex { get; set; }
        public DbSet<SymptomStatus> SymptomStatus { get; set; }
        public DbSet<Yn> Yn { get; set; }
#nullable enable
    }
}
