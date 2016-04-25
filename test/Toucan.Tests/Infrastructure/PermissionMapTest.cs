using System;
using System.Security.Claims;
using Toucan.Infrastructure;
using Xunit;

namespace Toucan.Tests.Infrastructure 
{
    public class PermissionMapTest
    {
        [Fact]
        public void OnModelShouldSetModelPropertyAndReturnReference()
        {
            //Arrange
            var permissionMapping = new PermissionMapping();
            
            //Act
            var returnVar = permissionMapping.OnModel("TestModel");
            
            //Assert
            Assert.Equal("TestModel", permissionMapping.Model);
            Assert.Same(permissionMapping, returnVar);
        }
        
        [Fact]
        public void ForRoleShouldSetModelPropertyAndReturnReference()
        {
            //Arrange
            var permissionMapping = new PermissionMapping();
            
            //Act
            var returnVar = permissionMapping.ForRole("TestRole");
            
            //Assert
            Assert.Equal("TestRole", permissionMapping.Role);
            Assert.Same(permissionMapping, returnVar);
        }
        
        [Fact]
        public void WithActionShouldSetModelPropertyAndReturnReference()
        {
            //Arrange
            var permissionMapping = new PermissionMapping();
            
            //Act
            var returnVar = permissionMapping.WithAction("TestAction");
            
            //Assert
            Assert.Equal("TestAction", permissionMapping.Action);
            Assert.Same(permissionMapping, returnVar);
        }
        
        [Fact]
        public void WhereShouldSetCallbackPropertyAndReturnReference()
        {
            //Arrange
            var permissionMapping = new PermissionMapping();
            Func<ClaimsPrincipal, object, bool>  func = (user, model) => 1 == 1;
            
            //Act
            var returnVar = permissionMapping.Where(func);
            
            //Assert
            Assert.Same(func, permissionMapping.Callback);
            Assert.Same(permissionMapping, returnVar);
        }
    }
}