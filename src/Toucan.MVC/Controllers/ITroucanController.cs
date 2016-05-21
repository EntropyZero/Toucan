using System.Collections.Generic;

namespace Toucan.Controllers
{
    public interface IToucanController
    {
        Dictionary<string, object> Models { get; }  
        
        TModel GetModelInstance<TModel>() where TModel : class; 
    }
}