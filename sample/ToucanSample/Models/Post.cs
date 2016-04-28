using System;
using System.ComponentModel.DataAnnotations;

namespace ToucanSample.Models
{
    public class Post
    {
       public  int Id { get; set;}
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime CreatedAt { get; set; }
        [StringLength(450)]public string CreatedById { get; set; } 
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}