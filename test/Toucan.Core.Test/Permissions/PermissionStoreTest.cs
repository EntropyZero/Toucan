using System.Security.Claims;
using System.Collections.Generic;
using Toucan.Core.Permissions;
using Xunit;
using Moq;

namespace Toucan.Tests.Infrastructure
{
    public class PermissionStoreTest
    {
        [Fact]
        public void AddPermissionShouldReturnNewPermissionWithNoSetup()
        {
            //Arrange
            var permissionStore = new PermissionStore();
            
            //Act
            var permission1 = permissionStore.AddPermission();
            var permission2 = permissionStore.AddPermission();
            
            //Assert
            Assert.NotSame(permission1, permission2);
            Assert.Null(permission1.Action);
            Assert.Null(permission1.Callback);
            Assert.Null(permission1.Model);
            Assert.Null(permission1.Role);
        }
        
        [Fact]
        public void HasMatchingPermissionShouldReturnTrueForMatchingRole_Model_And_Action()
        {
            // Arrange
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(m => m.Claims).Returns(claims);
            
            PermissionStore store = new PermissionStore();
            PermissionMapping perm = store.AddPermission();
            perm.OnModel("TestModel").ForRole("TestRole").WithAction("TestAction");
            
            // Act and Assert
            Assert.True(store.HasMatchingPermission(mockPrincipal.Object, "TestAction", new TestModel()));
        }
        
        [Fact]
        public void HasMatchingPermissionShouldReturnFalsForMissingRole()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(m => m.Claims).Returns(claims);
            
            PermissionStore store = new PermissionStore();
            PermissionMapping perm = store.AddPermission();
            perm.OnModel("TestModel").ForRole("TestRole2").WithAction("TestAction");
            
            Assert.False(store.HasMatchingPermission(mockPrincipal.Object, "TestAction", new TestModel()));
        }
        
        [Fact]
        public void HasMatchingPermissionShouldReturnFalseForIncorrectAction()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(m => m.Claims).Returns(claims);
            
            PermissionStore store = new PermissionStore();
            PermissionMapping perm = store.AddPermission();
            perm.OnModel("TestModel").ForRole("TestRole").WithAction("TestAction");
            
            Assert.False(store.HasMatchingPermission(mockPrincipal.Object, "TestAction2", new TestModel()));
        }
                
        [Fact]
        public void HasMatchingPermissionShouldReturnFalseForDifferentModel()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(m => m.Claims).Returns(claims);
            
            PermissionStore store = new PermissionStore();
            PermissionMapping perm = store.AddPermission();
            perm.OnModel("TestModel").ForRole("TestRole").WithAction("TestAction");
            
            Assert.False(store.HasMatchingPermission(mockPrincipal.Object, "TestAction", new object()));
        }
        
        [Fact]
        public void HasMatchingPermissionShouldReturnTrueWhenCallBackIsTrue()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(m => m.Claims).Returns(claims);
            
            PermissionStore store = new PermissionStore();
            PermissionMapping perm = store.AddPermission();
            perm.OnModel("TestModel")
            .ForRole("TestRole")
            .WithAction("TestAction")
            .Where((user, model) => ((TestModel)model).Id == 1);
            
            Assert.True(store.HasMatchingPermission(mockPrincipal.Object, "TestAction", new TestModel()));
        } 
        
                
        [Fact]
        public void HasMatchingPermissionShouldReturnFalseWhenCallBackIsFalse()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(m => m.Claims).Returns(claims);
            
            PermissionStore store = new PermissionStore();
            PermissionMapping perm = store.AddPermission();
            perm.OnModel("TestModel")
            .ForRole("TestRole")
            .WithAction("TestAction")
            .Where((user, model) => ((TestModel)model).Id == 2);
            
            Assert.False(store.HasMatchingPermission(mockPrincipal.Object, "TestAction", new TestModel()));
        } 
    }  
    
    class TestModel{
        public int Id {
            get { return 1; }
        }
    } 
}