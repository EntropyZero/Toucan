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
        public static IServiceCollection AddToucan<TAdapter>(this IServiceCollection services, Action<PermissionStore> setupAction) where TAdapter : class, IDbAdapter
        {
            PermissionStore store = new PermissionStore();
            services.AddSingleton<PermissionStore>(store);
            services.AddSingleton<TAdapter, TAdapter>();    
            services.AddSingleton<IServiceContext, ServiceContext<TAdapter>>();
            services.AddScoped<IAuthorizationHandler, ToucanAuthorizationHandler>();
            
            setupAction.Invoke(store);
            
            return services;
        }
    }
}