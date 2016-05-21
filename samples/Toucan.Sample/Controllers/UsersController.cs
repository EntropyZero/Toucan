using Microsoft.AspNetCore.Mvc;
using Toucan.Controllers;
using Toucan.Sample.Models;

namespace Toucan.Sample.Controllers
{
    [LoadAndAuthorizeResourceAttribute(typeof(ApplicationUser))]
    public class UsersController : ToucanController  
    {
        public IActionResult Details()
        {
            return View(this.GetModelInstance<ApplicationUser>());
        }
    }  
}