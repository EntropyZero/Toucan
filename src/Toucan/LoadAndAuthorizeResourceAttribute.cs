using System;

namespace Toucan {
    
    [AttributeUsage(AttributeTargets.Class)]
    public class LoadAndAuthorizeResourceAttribute : Attribute
    {
        
        public LoadAndAuthorizeResourceAttribute()
        {
        }
        
        public LoadAndAuthorizeResourceAttribute(Type modelType) : this(modelType, null)
        {
        }
        
        public LoadAndAuthorizeResourceAttribute(Type modelType, string[] only) : this(modelType, only, null)
        {
        }
        
        public LoadAndAuthorizeResourceAttribute(Type modelType, string[] only, string[] except)
        {
            this.Type = modelType;
            this.Only = only;  
            this.Except = except;
        }
        
        public Type Type { get; set; }
        
        public string[] Only { get; set; }
        
        public string[] Except { get; set; }
    }
}