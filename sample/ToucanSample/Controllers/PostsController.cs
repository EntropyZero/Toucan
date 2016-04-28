using Microsoft.AspNet.Mvc;
using Toucan;
using Toucan.Controllers;
using ToucanSample.Models;

namespace ToucanSample.Controllers
{
    [LoadAndAuthorizeResource(Type=typeof(Post), Except=new[]{"Index"})]
    public class PostsController : ToucanController
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Your posts page";
            return View();
        }
        
        public IActionResult Details(int id)
        {
            Post post = this.GetModelInstance<Post>("Post");
            ViewData["Message"] = post.Title;
            return View();
        }
    }
}