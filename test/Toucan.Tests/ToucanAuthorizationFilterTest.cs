using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authorization.Infrastructure;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Routing;
using Moq;
using Toucan.Adapters;
using Toucan.Controllers;
using Toucan.Services;
using Xunit;

namespace Toucan.Tests
{
    public class ToucanAuthorizationFilterTest
    {
       [Fact]
       public void ShouldCheckForPresenceOfLoadAndAutthorizeAttributeAndReturnIfNotFound()
       {
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerNoAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;                      
            
            ActionContext test = new ActionContext(stubHttpContext, new RouteData(), new ActionDescriptor());           
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
                       
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);  
            
            mockServiceContext.Verify(m => m.DbContext, Times.Never);
            mockServiceContext.Verify(m => m.AuthorizationService, Times.Never);   
       } 
       
       [Fact]
       public void ShouldThrowExceptionIfControllerIsNotToucanController()
       {
            var stubHttpContext = new StubHttpContext();
            var mockController = new NotToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;                      
            
            ActionContext test = new ActionContext(stubHttpContext, new RouteData(), new ActionDescriptor());           
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            Assert.Throws<InvalidOperationException>(() => new ToucanAuthorizationFilter().OnActionExecuting(actionContext));  
            
            mockServiceContext.Verify(m => m.DbContext, Times.Never);
            mockServiceContext.Verify(m => m.AuthorizationService, Times.Never);   
       } 
       
       [Fact]
       public void ShouldLoadModelFromDbAdapterIfIdIsPresentInRoute()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            mockDbContext.SetupGet(m => m.KeyType).Returns(typeof(int));
            mockDbContext.Setup(m => m.GetModel<object>(1, typeof(object))).Returns(new object());
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);
           
            mockDbContext.VerifyGet(m => m.KeyType);
            mockDbContext.Verify(m => m.GetModel<object>(1, typeof(object)));
       }
       
       [Fact]
       public void ShouldGetNewInstanceIfIdIsNotPresentInRoute()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);
           
            mockDbContext.Verify(m => m.GetModel<object>(1, typeof(object)), Times.Never);
       }
       
       [Fact]
       public void ShouldCallAuthorizationServiceWithUserActionRequirementAndModel()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var mockUser = new Mock<ClaimsPrincipal>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            stubHttpContext.User = mockUser.Object;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);
           
            mockAuthorizationService.Verify(m => m.AuthorizeAsync(
                mockUser.Object, 
                It.IsNotNull<object>(), 
                It.Is<IEnumerable<OperationAuthorizationRequirement>>(r => r.Any(o => o.Name == "test"))));    
       }
       
       [Fact]
       public void ShouldSetControllerModelOnSucccessfulAuthorization()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            mockDbContext.SetupGet(m => m.KeyType).Returns(typeof(int));
            object model = new object();
            mockDbContext.Setup(m => m.GetModel<object>(1, typeof(object))).Returns(model);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);
           
            Assert.Equal(model, mockController.GetModelInstance<object>());
       }
       
       [Fact] 
       void ShouldNotSetcontrollerModelOnFailedAuthorization()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  false));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            mockDbContext.SetupGet(m => m.KeyType).Returns(typeof(int));
            object model = new object();
            mockDbContext.Setup(m => m.GetModel<object>(1, typeof(object))).Returns(model);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);
           
            Assert.Equal(null, mockController.GetModelInstance<object>());
       }
       
              
       [Fact]
       public void ShouldResponseWithChallengeResponseWhenAuthorizationFails()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  false));
            mockAuthorizationService.Setup(m => m.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(task);
            mockDbContext.SetupGet(m => m.KeyType).Returns(typeof(int));
            object model = new object();
            mockDbContext.Setup(m => m.GetModel<object>(1, typeof(object))).Returns(model);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);
            
            task.Start();
            new ToucanAuthorizationFilter().OnActionExecuting(actionContext);
            
            Assert.IsType<ChallengeResult>(actionContext.Result);
       }
       
       [Fact]
       public void ShouldThrowExceptionIfLookupModelIsNull()
       {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockDbContext = new Mock<IDbAdapter>();
            var stubHttpContext = new StubHttpContext();
            var mockController = new ToucanControllerWithAttributes();
            var mockServiceContext =  new Mock<IServiceContext>();
            var serviceProvider = new StubServiceProvider();
            serviceProvider.Services.Add(typeof(IServiceContext), mockServiceContext.Object);
            stubHttpContext.ApplicationServices = serviceProvider;
            mockServiceContext.SetupGet(m => m.DbContext).Returns(mockDbContext.Object);
            mockServiceContext.SetupGet(m => m.AuthorizationService).Returns(mockAuthorizationService.Object);
            
            Task<bool> task = new Task<bool>(new Func<bool>(() =>  true));
            mockDbContext.SetupGet(m => m.KeyType).Returns(typeof(int));
            mockDbContext.Setup(m => m.GetModel<object>(1, typeof(object))).Returns(null);
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("id", "1");
            routeData.Values.Add("action", "test");
            
            ActionContext test = new ActionContext(stubHttpContext, routeData, new ActionDescriptor());
            ActionExecutingContext actionContext = new ActionExecutingContext(test, new List<IFilterMetadata>(), new Dictionary<string, object>(), mockController);

            Assert.Throws<InvalidOperationException>(() => new ToucanAuthorizationFilter().OnActionExecuting(actionContext));
            Assert.Equal(null, mockController.GetModelInstance<object>());
            mockAuthorizationService.Verify
                (m => m.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(), 
                    It.IsAny<object>(), 
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
        public override IServiceProvider ApplicationServices
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

        public override IServiceProvider RequestServices
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
    
    [LoadAndAuthorizeResourceAttribute(typeof(object))]
    public class NotToucanControllerWithAttributes : Controller
    {
        
    }
    
    [LoadAndAuthorizeResourceAttribute(typeof(object))]
    public class ToucanControllerWithAttributes : ToucanController
    {}
}