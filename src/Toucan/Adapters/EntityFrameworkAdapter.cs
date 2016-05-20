using System;
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
        
        public override object GetModel(TKey key, Type modelType)
        {
            IQueryable<IDbEntity> dbSet = GetDBSetProperty(modelType);  
            var model = dbSet.FirstOrDefault(m => m.Id.Equals(key));            
            return model;
        }
        
        private IQueryable<IDbEntity> GetDBSetProperty(Type modelType)
        {
            var contextType = DataAccess.GetType();
            var members = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var genericMembers = members.Where( m => m.PropertyType.IsConstructedGenericType);
            var property = genericMembers.FirstOrDefault(m => m.PropertyType.GenericTypeArguments.First().Name == modelType.Name);

            return property.GetGetMethod().Invoke(DataAccess, new object[0]) as IQueryable<IDbEntity>;
        }
    }
}