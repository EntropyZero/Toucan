using System;

namespace Toucan.Adapters
{
    public interface IDbAdapter
    {   
        object GetModel<TArg>(TArg key, Type modelType);
        Type KeyType { get; }
    }

    public abstract class DbAdapter<TStoreType, TKey> : IDbAdapter where TStoreType : class
    {
        protected TStoreType DataAccess{ get; set; }        
       
        public abstract object GetModel<TArg>(TArg key, Type modelType);
        
        public Type KeyType 
        {
            get { return typeof(TKey); }
        }
    }
}