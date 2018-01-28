using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Focus3.CsvTransform.CS.Models;

namespace Focus3.CsvTransform.CS.Extensions
{
    public static class EnrolleeExtensions
    {
        public static IDictionary<string, object> AsDictionary(this Enrollee source, 
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            var properties = source.GetType().GetProperties(bindingAttr);

            var dictionary = properties.ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetPropValue(source)
            );

            // add complex property values to dictionary...
            // company
            if (source.Company == null)
            {
                throw new ArgumentException($"Company data is missing for enrollee with ID [{source.EmployeeId}]");
            }
            source.Company.GetType().GetProperties(bindingAttr)
                .ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(source.Company))
                .ToList()
                .ForEach(d => dictionary.Add(d.Key, d.Value));

            // address
            if (source.Address == null)
            {
                throw new ArgumentException($"Address data is missing for enrollee with ID [{source.EmployeeId}]");
            }
            source.Address.GetType().GetProperties(bindingAttr)
                .ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(source.Address))
                .ToList()
                .ForEach(d => dictionary.Add(d.Key, d.Value));

            // common personal data
            if (source.CommonPersonalData == null)
            {
                throw new ArgumentException($"Personal data is missing for enrollee with ID [{source.EmployeeId}]");
            }
            source.CommonPersonalData.GetType().GetProperties(bindingAttr)
                .ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(source.CommonPersonalData))
                .ToList()
                .ForEach(d => dictionary.Add(d.Key, d.Value));

            // enrollment data
            if (source is Employee employee)
            {
                if (employee.Enrollment == null)
                {
                    throw new ArgumentException($"Enrollment data is missing for enrollee with ID [{source.EmployeeId}]");
                }
                employee.Enrollment.GetType().GetProperties(bindingAttr)
                    .ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(employee.Enrollment))
                    .ToList()
                    .ForEach(d => dictionary.Add(d.Key, d.Value)); 
            }

            return dictionary;
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
