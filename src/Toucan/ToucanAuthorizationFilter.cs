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
                if(context.RouteData.Values.ContainsKey("id"))
                {
                    var prop = GetDBSetProperty(modelType, serviceContext.DbContext);
                    var model = prop.FirstOrDefault(m =>
                        (int)(Convert.ChangeType(m,modelType).GetType().GetProperty("Id").GetGetMethod().Invoke(m, new object[0])) == Int32.Parse(context.RouteData.Values["id"].ToString())
                     );
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
                else
                {
                    Console.WriteLine("Initializing new model entity");
                    Activator.CreateInstance(modelType);             
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
