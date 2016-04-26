using Microsoft.AspNet.Authorization;
using Toucan.Adapters;

namespace Toucan.Services
{
    public interface IServiceContext
    {
        IDbAdapter DbContext { get; } 
        IAuthorizationService AuthorizationService { get; }
    }
    
    public class ServiceContext<T> : IServiceContext  where T : IDbAdapter
    {

        public ServiceContext(T dbContext, IAuthorizationService authzService)
        {
            DbContext = dbContext;
            AuthorizationService = authzService;
        }
        
        public IDbAdapter DbContext { get; private set;}

        public IAuthorizationService AuthorizationService { get; private set; }
    }
}
