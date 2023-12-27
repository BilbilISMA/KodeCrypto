using System;
using KodeCrypto.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using KodeCrypto.Application.Common.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using KodeCrypto.Domain.Entities.Identity;

namespace KodeCrypto.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>, IApplicationDbContext
    {
        private readonly IConfigurationRoot _configuration;
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfigurationRoot configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Domain.Entities.AccountBalance> AccountBalances => Set<Domain.Entities.AccountBalance>();
        public DbSet<Domain.Entities.TradeHistory> TradeHistories => Set<Domain.Entities.TradeHistory>();
        public DbSet<Domain.Entities.ApiKey> ApiKeys => Set<Domain.Entities.ApiKey>();
        public DbSet<Domain.Entities.Order> Orders => Set<Domain.Entities.Order>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                connectionString = new SqliteConnectionStringBuilder(connectionString)
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                }.ToString();
                optionsBuilder.UseSqlite(connectionString);
            }
#if (DEBUG)
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.EnableDetailedErrors(true);
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.LogTo(message => Debug.WriteLine(message));
#endif
        }
    }
}