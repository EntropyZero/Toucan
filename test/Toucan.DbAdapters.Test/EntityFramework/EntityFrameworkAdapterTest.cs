using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using Toucan.Core.Data;
using Toucan.Adapters;

namespace Toucan.Tests.Adapters
{
    public class EntityFrameworkAdapterTest : IDisposable
    {
        private TestDbContext _testDbContext;
        private TestModel _testModel;
        
        public EntityFrameworkAdapterTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _testDbContext = new TestDbContext(optionsBuilder.Options);
            _testModel = new TestModel{
                    Id = 1,
                    Description = "Test Description",
                    Created = DateTime.Now
                }; 
            _testDbContext.TestModels.Add(_testModel);
            _testDbContext.SaveChanges();  
        }
        
        [Fact]
        public void ShouldReturnModelFromDatabase()
        { 
            IDbAdapter adapter = new EntityFrameworkAdapter<TestDbContext, int>(_testDbContext);
            var model = adapter.GetModel(1, typeof(TestModel));
            
            Assert.IsType<TestModel>(model);
            Assert.Equal(1, ((TestModel)model).Id);
            Assert.Equal("Test Description", ((TestModel)model).Description);
        }
        
        [Fact]
        public void ShouldReturnNullIfModelNotFound()
        {            
            IDbAdapter adapter = new EntityFrameworkAdapter<TestDbContext, int>(_testDbContext);
            var model = adapter.GetModel(2, typeof(TestModel));
            
            Assert.Null(model); 
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _testDbContext.TestModels.Remove(_testModel);
                    _testDbContext.SaveChanges();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}