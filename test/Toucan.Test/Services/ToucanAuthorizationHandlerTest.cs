using System.Security.Claims;
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
            AuthorizationContext context = new AuthorizationContext(new []{requirement}, user, model);   
            var mockStore = new Mock<PermissionStore>();
            
            ToucanAuthorizationHandler handler = new ToucanAuthorizationHandler(mockStore.Object);
            handler.Handle(context);
            
            mockStore.Verify(m => m.HasMatchingPermission(user, "Test", model));
        }
        
        [Fact]
        public void HandleShouldSucceedWhenHasMatchingPermissionIsTrue()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            var requirement = new OperationAuthorizationRequirement{ Name = "Test"};
            object model = new object();
            AuthorizationContext context = new AuthorizationContext(new []{requirement}, user, model);
            var mockStore = new Mock<PermissionStore>();
            mockStore.Setup(m => m.HasMatchingPermission(user, "Test", model)).Returns(true);
            
            ToucanAuthorizationHandler handler = new ToucanAuthorizationHandler(mockStore.Object);
            handler.Handle(context);
            Assert.True(context.HasSucceeded);
        }
                
        [Fact]
        public void HandleShouldFailWhenHasMatchingPermissionIsFalse()
        {
            ClaimsPrincipal user = new ClaimsPrincipal();
            var requirement = new OperationAuthorizationRequirement{ Name = "Test"};
            object model = new object();
            AuthorizationContext context = new AuthorizationContext(new []{requirement}, user, model);
            var mockStore = new Mock<PermissionStore>();
            mockStore.Setup(m => m.HasMatchingPermission(user, "Test", model)).Returns(false);
            
            ToucanAuthorizationHandler handler = new ToucanAuthorizationHandler(mockStore.Object);
            handler.Handle(context);
            Assert.True(context.HasFailed);
        }
    }
}