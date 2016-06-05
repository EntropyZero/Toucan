using System;
using System.Security.Claims;

namespace Toucan.Core.Permissions
{
    public class PermissionMapping
    {        
        public string Role { get; internal set; }
        public string Model {get; internal set; }
        public string Action { get; internal set; }
        public Func<ClaimsPrincipal, object, bool> Callback { get; internal set; }
        
        public PermissionMapping OnModel(string modelName)
        {
            Model = modelName;
            return this;
        }
         
        public PermissionMapping ForRole(string roleName)
        {
            Role = roleName;
            return this;
        }

        public PermissionMapping WithAction(string action)
        {
            Action = action;
            return this;
        }
        public PermissionMapping Where(Func<ClaimsPrincipal, object, bool> callback)
        {
            Callback = callback;
            return this;
        }
    }
}