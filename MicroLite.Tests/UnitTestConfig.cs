namespace MicroLite.Tests
{
    using System;
    using System.Reflection;
    using MicroLite.Mapping;

    internal static class UnitTestConfig
    {
        internal static ConventionMappingSettings GetConventionMappingSettings(IdentifierStrategy identifierStrategy)
        {
            return new ConventionMappingSettings
            {
                AllowInsert = (PropertyInfo p) =>
                {
                    return p.Name != "Updated";
                },
                AllowUpdate = (PropertyInfo p) =>
                {
                    return p.Name != "Created";
                },
                ResolveIdentifierStrategy = (Type type) =>
                {
                    return identifierStrategy;
                },
                ResolveTableSchema = (Type type) =>
                {
                    return "Sales";
                }
            };
        }
    }
}