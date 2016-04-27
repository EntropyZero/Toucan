using System;

namespace Toucan.Adapters
{
    public interface IDbAdapter
    {   
        object GetModel<T>(T key, Type modelType);
        Type KeyType { get; }
    }

    public abstract class DbAdapter<StoreType, T> : IDbAdapter where StoreType : class
    {
        protected StoreType DataAccess{ get; set; }        
       
        public abstract object GetModel<K>(K key, Type modelType);
        
        public Type KeyType 
        {
            get { return typeof(T); }
        }
    }
}