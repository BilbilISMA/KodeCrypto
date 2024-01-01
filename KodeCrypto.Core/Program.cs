using Hangfire;
using KodeCrypto.Application;
using KodeCrypto.Core;
using KodeCrypto.Domain.Entities.Identity;
using KodeCrypto.Infrastructure.Data;
using KodeCrypto.Infrastructure.Jobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KodeCrypto", Version = "v1" });

    // Configure Swagger to use JWT authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services
           .AddIdentityCore<User>()
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddWebServices();
builder.Services.AddJWTConfiguration(builder.Configuration);
builder.Services.AddSession();
builder.Services.AddAuthentication();
builder.Services.AddScoped<JobExecuter>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
    app.UseHangfireServer();

    //Job for syncing with 3rd party APIs   
    RecurringJob.AddOrUpdate<JobExecuter>(x => x.RunSync(), "0 * * * *");
}
Host.CreateDefaultBuilder(args)
.ConfigureLogging(logging =>
{
    logging.ClearProviders(); 
    logging.AddConsole(); 
});           
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSession();
app.Run();


