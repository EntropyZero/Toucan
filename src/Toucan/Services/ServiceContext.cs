using Microsoft.AspNetCore.Authorization;
using Toucan.Adapters;

namespace Toucan.Services
{
    public interface IServiceContext<TKey>
    {
        IDbAdapter<TKey> DbContext { get; } 
        IAuthorizationService AuthorizationService { get; }
    }
    
    public class ServiceContext<TAdapter, TKey> : IServiceContext<TKey>  where TAdapter : IDbAdapter<TKey>
    {

        public ServiceContext(TAdapter dbContext, IAuthorizationService authzService)
        {
            DbContext = dbContext;
            AuthorizationService = authzService;
        }
        
        public IDbAdapter<TKey> DbContext { get; private set;}

        public IAuthorizationService AuthorizationService { get; private set; }
    }
}
