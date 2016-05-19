using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Toucan.Adapters
{
    public class EntityFrameworkAdapter<TStoreType, TKey> : DbAdapter<TStoreType, TKey> where TStoreType : DbContext
    {
        public EntityFrameworkAdapter(TStoreType dataAccess)
        {
            DataAccess = dataAccess;   
        }
        
        public override object GetModel<TArg>(TArg key, Type modelType)
        {
            IEnumerable<object> dbSet = GetDBSetProperty(modelType);  
            var model = dbSet.FirstOrDefault(
                m => Equals(
                        ((TArg)(Convert.ChangeType(m, modelType).GetType().GetProperty("Id").GetGetMethod().Invoke(m, new object[0]))), 
                        key
                        ));            
            return model;
        }
        
        private IEnumerable<object> GetDBSetProperty(Type modelType)
        {
            var contextType = DataAccess.GetType();
            var members = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var genericMembers = members.Where( m => m.PropertyType.IsConstructedGenericType);
            var property = genericMembers.FirstOrDefault(m => m.PropertyType.GenericTypeArguments.First().Name == modelType.Name);

            return property.GetGetMethod().Invoke(DataAccess, new object[0]) as IEnumerable<object>;
        }
    }
}