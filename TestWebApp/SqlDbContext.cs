using EntityFrameworkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestWebApp
{
    public class SqlDbContext : DbContext
    {
        public DbSet<WeatherForecast> Forecasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecast>().HasAnnotation("Merge", true);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TestDatabase");
            base.OnConfiguring(optionsBuilder);
            if (optionsBuilder == null)
            {
                return;
            }
            optionsBuilder.ReplaceService<IMigrationsModelDiffer, MergeMigrationsModelDiffer>();
            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, SqlServerMergeMigrationSqlGenerator>();
        }
    }
}
