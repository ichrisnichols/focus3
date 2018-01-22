using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Focus3.CsvTransform.CS.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AsDictionary(this object source, 
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            var properties = source.GetType().GetProperties(bindingAttr);

            var dictionary = properties.ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetPropValue(source)
            );

            // todo: map class data...

            return dictionary;

            //return source.GetType().GetProperties(bindingAttr).ToDictionary
            //(
            //    propInfo => propInfo.Name,
            //    propInfo => propInfo.GetValue(source, null)
            //);

        }

        public static object GetPropValue(this PropertyInfo propInfo, object source,
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            var propType = propInfo.PropertyType;
            if (!propType.IsClass || propType.Name == "String")
            {
                return propInfo.GetValue(source, null);
            }
            return null;
        }
    }
}
