using System.Collections.Generic;

namespace Toucan.Controllers
{
    public interface IToucanController
    {
        Dictionary<string, object> Models { get; }  
        
        T GetModelInstance<T>(string name) where T : class; 
    }
}