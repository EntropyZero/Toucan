using System;

namespace Toucan.Adapters
{
    public interface IDbAdapter<TKey>
    {   
        object GetModel(TKey key, Type modelType);
        Type KeyType { get; }
    }
    
    public interface IDbEntity<TKeyType>
    {
         TKeyType Id { get; set; }
    }

    public abstract class DbAdapter<TStoreType, TKey> : IDbAdapter<TKey> where TStoreType : class
    {
        protected TStoreType DataAccess{ get; set; }        
       
        public abstract object GetModel(TKey key, Type modelType);
        
        public Type KeyType 
        {
            get { return typeof(TKey); }
        }
    }
}