using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

namespace Toucan.Controllers
{
    public abstract class ToucanController : Controller, IToucanController
    {
        protected readonly Dictionary<string, object> _models = new Dictionary<string, object>();
        
        public Dictionary<string, object> Models
        {
            get
            {
                return _models;
            }
        }

        public T GetModelInstance<T>(string name) where T : class
        {
            if(_models.ContainsKey(name))
            {
                return _models[name] as T;
            }
            
            return null;
        }
        
        
    }
}