using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization.Infrastructure;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Controllers;
using Toucan.Services;

namespace Toucan
{
    public class ToucanAuthorizationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {         
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            IServiceContext serviceContext = context.HttpContext.ApplicationServices.GetRequiredService<IServiceContext>();
            TypeInfo t = context.Controller.GetType().GetTypeInfo(); 
            var attributes = t.GetCustomAttributes<LoadAndAuthorizeResourceAttribute>();
            
            if(!attributes.Any())
            {
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
                    model = serviceContext.DbContext.GetModel(context.RouteData.Values["id"], modelType);
                }
                else
                {
                    model = Activator.CreateInstance(modelType);             
                }   

                Task<bool> authTask = serviceContext.AuthorizationService.AuthorizeAsync(context.HttpContext.User, model, new[]{new OperationAuthorizationRequirement{ Name = context.RouteData.Values["action"].ToString()}}); 
                authTask.Wait();
                if(authTask.Result == true)
                {
                    (context.Controller as IToucanController).Models.Add(modelType.Name, model);
                }
                else
                {
                    context.Result = new ChallengeResult();
                    return;
                }               
            }   
        }
        
        private IEnumerable<object> GetDBSetProperty(Type modelType, object context)
        {
            var contextType = (context as DbContext).GetType();
            var members = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var genericMembers = members.Where( m => m.PropertyType.IsConstructedGenericType);
            var property = genericMembers.FirstOrDefault(m => m.PropertyType.GenericTypeArguments.First().Name == modelType.Name);

            return property.GetGetMethod().Invoke(context, new object[0]) as IEnumerable<object>;
        }
    }
}
