using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Toucan.Core.Permissions;

namespace Toucan.Services
{

    public class ToucanAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, object>
        {
            PermissionStore _permissionStore;
            public ToucanAuthorizationHandler(PermissionStore permissionStore)
            {
                _permissionStore = permissionStore;    
            }
            
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, object resource)
            {
                if(_permissionStore.HasMatchingPermission(context.User, requirement.Name, resource))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return Task.FromResult(0);
            }
        }
    }