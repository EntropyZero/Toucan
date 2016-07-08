using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Moq;
using Toucan.Core.Permissions;
using Toucan.Services;
using Xunit;

namespace Toucan.Tests.Services
{
    public class ToucanAuthorizationHandlerTest
    {
        [Fact]
        public void HandleShouldCallPermissionStoreToCheckPermission()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            var requirement = new OperationAuthorizationRequirement{ Name = "Test"};
            object model = new object();
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(new []{requirement}, user, model);   
            var mockStore = new Mock<PermissionStore>();
            
            ToucanAuthorizationHandler handler = new ToucanAuthorizationHandler(mockStore.Object);
            Task task = handler.HandleAsync(context);
            task.Wait();
            mockStore.Verify(m => m.HasMatchingPermission(user, "Test", model));
        }
        
        [Fact]
        public void HandleShouldSucceedWhenHasMatchingPermissionIsTrue()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            var requirement = new OperationAuthorizationRequirement{ Name = "Test"};
            object model = new object();
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(new []{requirement}, user, model);
            var mockStore = new Mock<PermissionStore>();
            mockStore.Setup(m => m.HasMatchingPermission(user, "Test", model)).Returns(true);
            
            ToucanAuthorizationHandler handler = new ToucanAuthorizationHandler(mockStore.Object);
            Task task = handler.HandleAsync(context);
            task.Wait();
            Assert.True(context.HasSucceeded);
        }
                
        [Fact]
        public void HandleShouldFailWhenHasMatchingPermissionIsFalse()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            var requirement = new OperationAuthorizationRequirement{ Name = "Test"};
            object model = new object();
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(new []{requirement}, user, model);
            var mockStore = new Mock<PermissionStore>();
            mockStore.Setup(m => m.HasMatchingPermission(user, "Test", model)).Returns(false);
            
            ToucanAuthorizationHandler handler = new ToucanAuthorizationHandler(mockStore.Object);
            Task task = handler.HandleAsync(context);
            task.Wait();
            Assert.True(context.HasFailed);
        }
    }
}