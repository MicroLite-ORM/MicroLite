namespace MicroLite.FrameworkExtensions
{
    using System;
    using System.Reflection;

    internal static class MemberInfoExtensions
    {
        internal static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit)
            where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit) as T[];

            return attributes != null && attributes.Length == 1 ? attributes[0] : null;
        }
    }
}