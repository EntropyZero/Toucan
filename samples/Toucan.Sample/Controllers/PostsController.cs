using Microsoft.AspNetCore.Mvc;
using Toucan.Controllers;

namespace Toucan.Sample.Controllers
{
    public class PostsController : ToucanController
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "This area is for posts.";
            return View();
        }
    }
}