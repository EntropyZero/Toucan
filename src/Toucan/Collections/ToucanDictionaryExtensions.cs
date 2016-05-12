using System;
using System.Collections.Generic;
using System.Reflection;

namespace Toucan.Collections
{
    public static class ToucanDictionaryExtensions
    {
        public static object FindModelKey(this IDictionary<string, object> routeData, Type modelType, Type keyType)
        {
            string candidateKey = string.Format("{0}_id", modelType.Name.ToLowerInvariant());
            if(routeData.Keys.Contains(candidateKey))
            {
                return GetKeyValue(routeData[candidateKey], keyType);               
            }
            if(routeData.Keys.Contains("id"))
            {
                return GetKeyValue(routeData["id"], keyType);  
            }
            
            return null;                      
        }
        
        private static object GetKeyValue(object keyValue, Type keyType)
        {
            object key;
            if(keyType != keyValue.GetType() && keyValue is string)
            {   
                key = keyType.GetMethod("Parse", new[]{typeof(string)} ).Invoke(null, new []{keyValue});
            } 
            else
            {
                key = keyValue;
            }
            return key;    
        }
    }     
}