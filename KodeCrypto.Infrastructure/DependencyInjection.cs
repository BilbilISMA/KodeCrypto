using KodeCrypto.Infrastructure.Identity;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Ardalis.GuardClauses;
using KodeCrypto.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KodeCrypto.Domain.Constants;
using KodeCrypto.Infrastructure.Integration.Binance;
using KodeCrypto.Infrastructure.Integration.Kraken;
using KodeCrypto.Domain.Entities.Identity;
using Hangfire;
using Hangfire.SQLite;
using KodeCrypto.Infrastructure.Repositories;
using KodeCrypto.Application.Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KodeCrypto.Infrastructure.Integration.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");
        var sqliteOptions = new SQLiteStorageOptions();

        services.AddHangfire(c => c
                .UseSQLiteStorage("Filename=kodecrypto.db;", sqliteOptions));

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlite(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

       
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddHttpClient<BinanceApiClient>();
        services.AddTransient<IBinanceService, BinanceService>();

        services.AddHttpClient<KrakenApiClient>();
        services.AddTransient<IKrakenService, KrakenService>();

        services.AddTransient<ILocalDataRepository, LocalDataRepository>();
        services.AddTransient<IApiKeyRepository, ApiKeyRepository>();
        services.Configure<KrakenConfig>(configuration.GetSection(KrakenConfig.SettingsSection));
        services.Configure<BinanceConfig>(configuration.GetSection(BinanceConfig.SettingsSection));
        
        // Configure ASP.NET Core Identity
        services.AddIdentity<User, IdentityRole>(o =>
        {
            o.Password.RequireUppercase = false;
            o.User.RequireUniqueEmail = true;
            o.SignIn.RequireConfirmedEmail = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SettingsSection));
        return services;
    }

    #region Injecting JWT Configurations

    /// <summary>
    ///     Configures and injects all the needed stuff for jwt authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddJWTConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //Extension for JWT
        //services.AddJwtAuthentication(HostingEnvironment, builder);

        IConfigurationSection jwtSection = configuration.GetSection(JwtOptions.SettingsSection);
        JwtOptions jwtOptions = new JwtOptions();
        jwtSection.Bind(jwtOptions);
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg => cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        });

        return services;
    }

    #endregion Injecting JWT Configurations
}


