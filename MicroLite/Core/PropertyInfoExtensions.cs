namespace MicroLite.Core
{
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
    }
}