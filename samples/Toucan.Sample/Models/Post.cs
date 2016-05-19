using System;
using Toucan.Adapters;

namespace Toucan.Sample.Models
{
    public class Post : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}