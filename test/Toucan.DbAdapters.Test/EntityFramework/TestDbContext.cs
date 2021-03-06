using System;
using Microsoft.EntityFrameworkCore;
using Toucan.Core.Data;

namespace Toucan.DbAdapters.EntityFramework.Tests
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() : base(){}
        
        public TestDbContext(DbContextOptions options) : base (options){}
        
        public DbSet<TestModel> TestModels { get; set; }
    }
    
    public class TestModel : DbEntity<int>
    {
        public override int Id { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
    }
}