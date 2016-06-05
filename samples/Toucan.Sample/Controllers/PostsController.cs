using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Toucan.Controllers;
using Toucan.Sample.Data;
using Toucan.Sample.Models;

namespace Toucan.Sample.Controllers
{
    [LoadAndAuthorizeResourceAttribute(Type=typeof(Post), Only=new[]{"Details"})]
    public class PostsController : ToucanController
    {
        ApplicationDbContext _dbContext;
        
        public PostsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;    
        }
        
        public IActionResult Index()
        {
            Post post = _dbContext.Posts.First(p => p.Id == 1);
            ViewData["Message"] = "This area is for posts.";
            return View(post);
        }
        
        public IActionResult Details()
        {   
            return View(GetModelInstance<Post>());
        }
    }
}