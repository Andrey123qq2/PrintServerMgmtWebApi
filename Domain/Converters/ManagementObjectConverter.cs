using Domain.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace Domain.Converters
{
    public static class ManagementObjectConverter
    {

        public static T Convert<T>(ManagementObject managementObject)
        {
            Type modelType = typeof(T);
            T model = (T)Activator.CreateInstance(modelType);
            modelType.GetProperties()
                .ToList()
                .ForEach(p =>
                {
                    string propName = p.Name;
                    string propValue = managementObject.Properties[propName].Value?.ToString();
                    modelType.GetProperty(propName).SetValue(model, propValue);
                });
            return model;
        }
    }
}