using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Adapters;
using Toucan.Infrastructure;
using Toucan.Services;

namespace Toucan
{
    public static class ToucanServiceCollectionExtensions
    {
        public static IServiceCollection AddToucan<T>(this IServiceCollection services, Action<PermissionStore> setupAction) where T : class, IDbAdapter
        {
            PermissionStore store = new PermissionStore();
            services.AddSingleton<PermissionStore>(store);
            services.AddSingleton<T, T>();    
            services.AddSingleton<IServiceContext, ServiceContext<T>>();
            services.AddScoped<IAuthorizationHandler, ToucanAuthorizationHandler>();
            
            setupAction.Invoke(store);
            
            return services;
        }
    }
}