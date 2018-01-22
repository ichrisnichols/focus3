using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Focus3.CsvTransform.CS.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var properties = source.GetType().GetProperties(bindingAttr);

            var dictionary = properties.ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

            return dictionary;

            //return source.GetType().GetProperties(bindingAttr).ToDictionary
            //(
            //    propInfo => propInfo.Name,
            //    propInfo => propInfo.GetValue(source, null)
            //);

        }
    }
}
