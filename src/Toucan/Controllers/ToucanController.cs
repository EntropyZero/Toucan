using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

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

        public TModel GetModelInstance<TModel>() where TModel : class
        {
            string name = typeof(TModel).Name;
            if(_models.ContainsKey(name))
            {
                return _models[name] as TModel;
            }
            
            return null;
        }
        
        
    }
}