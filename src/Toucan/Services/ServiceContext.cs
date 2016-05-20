using Microsoft.AspNetCore.Authorization;
using Toucan.Adapters;

namespace Toucan.Services
{
    public interface IServiceContext
    {
        IDbAdapter DbContext { get; } 
        IAuthorizationService AuthorizationService { get; }
    }
    
    public class ServiceContext<TAdapter> : IServiceContext  where TAdapter : IDbAdapter
    {

        public ServiceContext(TAdapter dbContext, IAuthorizationService authzService)
        {
            DbContext = dbContext;
            AuthorizationService = authzService;
        }
        
        public IDbAdapter DbContext { get; private set;}

        public IAuthorizationService AuthorizationService { get; private set; }
    }
}
