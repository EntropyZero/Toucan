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
        public static IServiceCollection AddToucan<TAdapter, TAdapterKey>(this IServiceCollection services, Action<PermissionStore> setupAction) where TAdapter : class, IDbAdapter<TAdapterKey>
        {
            PermissionStore store = new PermissionStore();
            services.AddSingleton<PermissionStore>(store);
            services.AddSingleton<TAdapter, TAdapter>();    
            services.AddSingleton<IServiceContext<TAdapterKey>, ServiceContext<TAdapter, TAdapterKey>>();
            services.AddScoped<IAuthorizationHandler, ToucanAuthorizationHandler>();
            
            setupAction.Invoke(store);
            
            return services;
        }
    }
}