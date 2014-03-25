namespace MicroLite.Tests
{
    using System;
    using System.Reflection;
    using MicroLite.Configuration;
    using MicroLite.Mapping;

    public abstract class UnitTest : IDisposable
    {
        protected UnitTest()
        {
            Configure.SessionFactories.Clear();
            ObjectInfo.Reset();
            SqlCharacters.Current = null;
        }

        public static ConventionMappingSettings GetConventionMappingSettings(IdentifierStrategy identifierStrategy)
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

        public void Dispose()
        {
            Configure.SessionFactories.Clear();
            ObjectInfo.Reset();
            SqlCharacters.Current = null;

            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}