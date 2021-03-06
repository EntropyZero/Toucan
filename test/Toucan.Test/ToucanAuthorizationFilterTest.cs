using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Toucan.Core.Data;
using Toucan.Controllers;
using Toucan.Services;
using Xunit;

namespace Toucan.Tests
{
    public class ToucanAuthorizationFilterTest
    {
        private static readonly ActionExecutionDelegate EmptyNext = async () => {
            return null;
        };
        
       [Fact]
       public async Task ShouldCheckForPresenceOfLoadAndAutthorizeAttributeAndReturnIfNotFound()
       {
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerNoAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;                      
            
            ActionContext test = new ActionContext(stubHttpContext, new RouteData(), new ActionDescriptor());           
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);  
            
            mockServiceContext.Verify(m => m.DbContext, Times.Never);
            mockServiceContext.Verify(m => m.AuthorizationService, Times.Never);   
       } 
       
       [Fact]
       public async Task ShouldThrowExceptionIfControllerIsNotToucanController()
       {
            var stubHttpContext = new StubHttpContext();
            var mockController = new NotToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;                      
            
            ActionContext test = new ActionContext(stubHttpContext, new RouteData(), new ActionDescriptor());           
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext));  
            
            mockServiceContext.Verify(m => m.DbContext, Times.Never);
            mockServiceContext.Verify(m => m.AuthorizationService, Times.Never);   
       } 
       
       [Fact]
       public async Task ShouldLoadModelFromDbAdapterIfIdIsPresentInRoute()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            mockDbContext.Setup(m => m.GetModel(1, typeof(TestModel))).Returns(new TestModel());
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);

            mockDbContext.Verify(m => m.GetModel(1, typeof(TestModel)));
       }
       
       [Fact]
       public async Task ShouldLoadModelFromDbAdapterIfNestedIdIsPresentInRoute()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            mockDbContext.Setup(m => m.GetModel(1, typeof(TestModel))).Returns(new object());
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("testmodel_id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);

            mockDbContext.Verify(m => m.GetModel(1, typeof(TestModel)));
       }
       
       [Fact]
       public async Task ShouldGetNewInstanceIfIdIsNotPresentInRoute()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);
           
            mockDbContext.Verify(m => m.GetModel(1, typeof(TestModel)), Times.Never);
       }
       
       [Fact]
       public async Task ShouldCallAuthorizationServiceWithUserActionRequirementAndModel()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var mockUser = new Mock<ClaimsPrincipal>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            stubHttpContext.User = mockUser.Object;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);
           
            mockAuthorizationService.Verify(m => m.AuthorizeAsync(
                mockUser.Object, 
                It.IsNotNull<TestModel>(), 
                It.Is<IEnumerable<OperationAuthorizationRequirement>>(r => r.Any(o => o.Name == "test"))));    
       }
       
       [Fact]
       public async Task ShouldSetControllerModelOnSucccessfulAuthorization()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            TestModel model = new TestModel();
            mockDbContext.Setup(m => m.GetModel(1, typeof(TestModel))).Returns(model);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);
           
            Assert.Equal(model, mockController.GetModelInstance<TestModel>());
       }
       
       [Fact] 
       public async Task ShouldNotSetcontrollerModelOnFailedAuthorization()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  false));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            TestModel model = new TestModel();
            mockDbContext.Setup(m => m.GetModel(1, typeof(TestModel))).Returns(model);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);
           
            Assert.Equal(null, mockController.GetModelInstance<TestModel>());
       }
              
       [Fact]
       public async Task ShouldRespondWithChallengeResponseWhenAuthorizationFails()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  false));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<TestModel>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            TestModel model = new TestModel();
            mockDbContext.Setup(m => m.GetModel(1, typeof(TestModel))).Returns(model);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext);
            
            Assert.IsType<ChallengeResult>(actionContext.Result);
       }
       
       [Fact]
       public async Task ShouldThrowExceptionIfLookupModelIsNull()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.RequestServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockDbContext.SetupGet(m => m.KeyType).Returns(typeof(int));
            mockDbContext.Setup(m => m.GetModel(1, typeof(TestModel))).Returns(null);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await new ToucanAuthorizationFilter().OnActionExecutionAsync(actionContext, EmptyNext));
            
            Assert.Equal(null, mockController.GetModelInstance<TestModel>());
            mockAuthorizationService.Verify
                (m => m.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(), 
                    It.IsAny<TestModel>(), 
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()), 
                Times.Never);
       }
    }

    internal class StubServiceProvider : IServiceProvider
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        internal Dictionary<Type, object> Services 
        { 
            get { return _services; }
        }
        
        public object GetService(Type serviceType)
        {
            return _services[serviceType];
        }
        
        public T GetRequiredService<T>() where T : class
        {
            Type theType = typeof(T);
            return _services[theType] as T;
        }
    }

    internal class StubHttpContext : HttpContext
    {
        IServiceProvider _serviceProvider;
        ClaimsPrincipal _user = null;
        public override IServiceProvider RequestServices
        {
            get
            {
                return _serviceProvider;
            }

            set
            {
                _serviceProvider = value;
            }
        }

        public override AuthenticationManager Authentication
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ConnectionInfo Connection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IFeatureCollection Features
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IDictionary<object, object> Items
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override HttpRequest Request
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override CancellationToken RequestAborted
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        
        public override HttpResponse Response
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ISession Session
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string TraceIdentifier
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override ClaimsPrincipal User
        {
            get
            {
                if(_user == null)
                {
                    _user = new Mock<ClaimsPrincipal>().Object;
                }
                return _user;
            }

            set
            {
                _user = value;
            }
        }

        public override WebSocketManager WebSockets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
    
    public class ToucanControllerNoAttributes : ToucanController 
    {
        public Type ReturnType { get; set; }
        
        public new Type GetType()
        {
            return ReturnType;
        }
    }
    
    [LoadAndAuthorizeResourceAttribute(typeof(TestModel))]
    public class NotToucanControllerWithAttributes : Controller
    {}
    
    [LoadAndAuthorizeResourceAttribute(typeof(TestModel))]
    public class ToucanControllerWithAttributes : ToucanController
    {}
    
    public class TestModel : DbEntity<int>
    {
        public override int Id { get; set; }
    }
}