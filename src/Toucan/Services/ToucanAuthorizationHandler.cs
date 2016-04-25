using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authorization.Infrastructure;
using Toucan.Infrastructure;

namespace Toucan.Services
{

    public class ToucanAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, object>
        {
            PermissionStore _permissionStore;
            public ToucanAuthorizationHandler(PermissionStore permissionStore)
            {
                _permissionStore = permissionStore;    
            }
            
            protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, object resource)
            {
                 
                if(_permissionStore.HasMatchingPermission(context.User, requirement.Name, resource))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
        }
    }