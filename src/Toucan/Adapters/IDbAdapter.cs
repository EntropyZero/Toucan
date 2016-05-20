using System;

namespace Toucan.Adapters
{
    public interface IDbAdapter
    {   
        object GetModel(object key, Type modelType);
        Type KeyType { get; }
    }
    
    public interface IDbEntity
    {
         object Id { get; }
    }

    public abstract class DbAdapter<TStoreType, TKeyType> : IDbAdapter where TStoreType : class
    {
        protected TStoreType DataAccess{ get; set; }        
       
        public abstract object GetModel(TKeyType key, Type modelType);

        object IDbAdapter.GetModel(object key, Type modelType)
        {
            return GetModel((TKeyType)key, modelType);
        }

        public Type KeyType 
        {
            get { return typeof(TKeyType); }
        }

        Type IDbAdapter.KeyType
        {
            get
            {
                return typeof(TKeyType);
            }
        }
    }
    
    public abstract class DbEntity<TKeyType> : IDbEntity
    {
        public abstract TKeyType Id { get; set; }

        object IDbEntity.Id
        {
            get
            {
                return Id;
            }
        }
    }
}