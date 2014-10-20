namespace MicroLite.Tests
{
    using System;
    using System.Reflection;
    using MicroLite.Characters;
    using MicroLite.Configuration;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    public abstract class UnitTest : IDisposable
    {
        protected UnitTest()
        {
            Configure.OnSessionFactoryCreated = null;
            Configure.SessionFactories.Clear();
            ObjectInfo.Reset();
            SqlCharacters.Current = null;
            LogManager.GetLogger = null;
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
                ResolveSequenceName = (PropertyInfo propertyInfo) =>
                {
                    return identifierStrategy == IdentifierStrategy.Sequence
                        ? propertyInfo.DeclaringType.Name + "_" + propertyInfo.Name + "_Sequence"
                        : null;
                },
                ResolveTableSchema = (Type type) =>
                {
                    return "Sales";
                }
            };
        }

        public void Dispose()
        {
            Configure.OnSessionFactoryCreated = null;
            Configure.SessionFactories.Clear();
            ObjectInfo.Reset();
            SqlCharacters.Current = null;
            LogManager.GetLogger = null;

            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}