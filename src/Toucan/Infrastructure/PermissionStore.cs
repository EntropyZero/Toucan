using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Toucan.Infrastructure
{
    public class PermissionStore
    {
        private List<PermissionMapping> _mappings;
        
        public PermissionStore()
        {
            _mappings = new List<PermissionMapping>();
        }
        
        public PermissionMapping AddPermission()
        {
            var mapping = new PermissionMapping();
            _mappings.Add(mapping);
            return mapping;
        }
        
        public virtual bool HasMatchingPermission(ClaimsPrincipal user, string action, object model)
        {
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role);
            foreach(Claim role in roles)
            {
                IEnumerable<PermissionMapping> candidates = _mappings.Where(m => m.Role == role.Value.ToString() && m.Model == model.GetType().Name && m.Action == action);
                IEnumerable<PermissionMapping> candidatesWithCallbacks = _mappings.Where(m => m.Callback != null);
                if(candidates.Any() && !candidatesWithCallbacks.Any())
                {
                    return true;
                }
                if(candidatesWithCallbacks.Any() && candidatesWithCallbacks.First().Callback.Invoke(user, model))
                {
                    return true;
                }
            }
            return false;
        }
    }
}