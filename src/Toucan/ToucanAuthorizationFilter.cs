using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization.Infrastructure;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Controllers;
using Toucan.Services;

namespace Toucan
{
    public class ToucanAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IServiceContext serviceContext = context.HttpContext.ApplicationServices.GetRequiredService<IServiceContext>();
            TypeInfo t = context.Controller.GetType().GetTypeInfo(); 
            var attributes = t.GetCustomAttributes<LoadAndAuthorizeResourceAttribute>();
            
            if(!attributes.Any())
            {
                    await next();
                    return;
            }
            if(!(context.Controller is IToucanController))
            {
                throw new InvalidOperationException("Unable to load resources for controllers that do not implement IToucanController");
            }
            foreach(var attribute in attributes)
            {
                if(attribute.Only != null && !attribute.Only.Contains(context.RouteData.Values["action"]))
                {
                    continue;
                }
                if(attribute.Except != null && attribute.Except.Contains(context.RouteData.Values["action"]))
                {
                    continue;
                }
                Type modelType = attribute.Type;
                object model;
                if(context.RouteData.Values.ContainsKey("id"))
                {  
                    var key = context.RouteData.Values["id"];
                    object newkey;
                    if(serviceContext.DbContext.KeyType != key.GetType() && key is string)
                    {   
                        newkey = serviceContext.DbContext.KeyType.GetMethod("Parse", new[]{typeof(string)} ).Invoke(null, new []{key});
                    } 
                    else
                    {
                        newkey = key;
                    }                              
                    model = serviceContext.DbContext.GetModel(newkey, modelType);
                }
                else
                {
                    model = Activator.CreateInstance(modelType);             
                }   
                if(model == null)
                {
                    throw new InvalidOperationException(string.Format("Requested model of type {0} not found", modelType.FullName));
                }
                bool authResult = await serviceContext.AuthorizationService.AuthorizeAsync(context.HttpContext.User, model, new[]{new OperationAuthorizationRequirement{ Name = context.RouteData.Values["action"].ToString()}}); 
                if(authResult == true)
                {
                    (context.Controller as IToucanController).Models.Add(modelType.Name, model);
                }
                else
                {
                    context.Result = new ChallengeResult();
                }
                await next();
            }   
        }
    }
}
