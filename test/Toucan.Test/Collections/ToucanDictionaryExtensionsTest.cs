using System.Collections.Generic;
using Toucan.Collections;
using Xunit;

namespace Toucan.Tests.Collections
{
    public class ToucanDictionaryExtensionsTest
    {
        [Fact]
        public void ShouldReturnKeyByLowerCaseModelNameIfPresent()
        {
            var routeDataValues = new Dictionary<string, object>();   
            routeDataValues.Add("id", 5);
            routeDataValues.Add("testmodel_id", 10); 
            
            Assert.Equal(10, routeDataValues.FindModelKey(typeof(TestModel)));            
        }
        
        [Fact]
        public void ShouldReturnKeyByIdIfIdPresentAndModelNameIsNot()
        {
            var routeDataValues = new Dictionary<string, object>();   
            routeDataValues.Add("id", 5);
            
            Assert.Equal(5, routeDataValues.FindModelKey(typeof(TestModel)));            
        }
              
        [Fact]
        public void ShouldReturnNullIfIdValuesAreNotPresent()
        {
            var routeDataValues = new Dictionary<string, object>();   
            routeDataValues.Add("action", "Details");
            
            Assert.Null(routeDataValues.FindModelKey(typeof(TestModel)));            
        }
    }
    
    internal class TestModel
    {
        public int Id {get; set;}
    }
}