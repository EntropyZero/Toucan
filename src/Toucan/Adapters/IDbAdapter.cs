using System;

namespace Toucan.Adapters
{
    public interface IDbAdapter
    {
        object GetModel<T>(T key, Type modelType);
    }

    public abstract class DbAdapter<StoreType> : IDbAdapter where StoreType : class
    {
        protected StoreType DataAccess{ get; set; }        
       
        public abstract object GetModel<T>(T key, Type modelType);
    }
}