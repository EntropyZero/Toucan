using Microsoft.AspNetCore.Mvc;
using Toucan.Controllers;
using Toucan.Sample.Models;

namespace Toucan.Sample.Controllers
{
    [LoadAndAuthorizeResourceAttribute(Type=typeof(Post), Only=new[]{"Details"})]
    public class PostsController : ToucanController
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "This area is for posts.";
            return View();
        }
        
        public IActionResult Details()
        {
            return View(GetModelInstance<Post>());
        }
    }
}