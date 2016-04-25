using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;

namespace Toucan.Services
{
    public interface IServiceContext
    {
        object DbContext { get; } 
        IAuthorizationService AuthorizationService { get; }
    }
    
    public class ServiceContext<T> : IServiceContext
    {

        public ServiceContext(T dbContext, IAuthorizationService authzService)
        {
            DbContext = dbContext;
            AuthorizationService = authzService;
        }
        
        public object DbContext { get; private set;}

        public IAuthorizationService AuthorizationService { get; private set; }
    }
}
