namespace MicroLite.Core
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal static class PropertyInfoExtensions
    {
        internal static object GetValue(this PropertyInfo propertyInfo, object instance)
        {
            var value = propertyInfo.GetValue(instance, null);

            if (propertyInfo.PropertyType.IsEnum)
            {
                return (int)value;
            }
            else
            {
                return value;
            }
        }

        internal static void SetValue<T>(this PropertyInfo propertyInfo, T instance, object value)
        {
            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(instance, value, null);
            }
            else
            {
                var converted = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                propertyInfo.SetValue(instance, converted, null);
            }
        }
    }
}