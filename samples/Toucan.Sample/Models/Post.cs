using System;
using Toucan.Core.Data;

namespace Toucan.Sample.Models
{
    public class Post : DbEntity<int>
    {
        public override int Id { get; set; }
        public string Content { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}