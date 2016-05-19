using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Controllers;
using Toucan.Collections;
using Toucan.Services;

namespace Toucan
{
    public class ToucanAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IServiceContext serviceContext = context.HttpContext.RequestServices.GetRequiredService<IServiceContext>();
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
                var key = context.RouteData.Values.FindModelKey(modelType, serviceContext.DbContext.KeyType);
                if(key != null)
                {                          
                    model = serviceContext.DbContext.GetModel(key, modelType);
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
                    break;
                }
                
            }
            if(context.Result == null)
            {
                await next();
            }
        }
    }
}
