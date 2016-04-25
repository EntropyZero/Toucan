using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Routing;
using Moq;
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
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
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
}