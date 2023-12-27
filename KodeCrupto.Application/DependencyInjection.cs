using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using KodeCrypto.Application.Common.Behaviours;
using KodeCrypto.Application.UseCases.Identity.Commands;
using KodeCrypto.Application.UseCases.Identity.DTO;
using KodeCrypto.Application.Generic;
using AutoMapper;
namespace KodeCrypto.Application
{
    public static class DependencyInjection
	{
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            });

            // Register your command handlers
            services.AddScoped<IRequestHandler<LoginCommand, JsonWebTokenDTO>, LoginHandler>();
            services.AddScoped<IRequestHandler<SignUpCommand, bool>, SignUpHandler>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<BaseHandlerServices>();

            return services;
        }
    }
}

