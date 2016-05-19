using System;

namespace Toucan.Sample.Models
{
    public class Comment
    {
        public int Id { get; set; } 
        public string Content { get; set; }
        public virtual Post Post { get; set; }
        public int PostId { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}