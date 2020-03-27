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
            this.ResetMicroLiteInternalState();
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
            this.ResetMicroLiteInternalState();

            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        private void ResetMicroLiteInternalState()
        {
            Configure.OnSessionFactoryCreated = null;
            SqlCharacters.Current = null;
            LogManager.GetLogger = null;
            Configure.SessionFactories.Clear();

            // Reset the internal state of ObjectInfo for the next set of tests so that we can easily
            // Test different mapping conventions etc.
            // Use reflection to assasinate the currently chosen mapping convention and objectinfos.
            var objectInfoType = typeof(ObjectInfo);

            var mappingConventionField = objectInfoType.GetField("s_mappingConvention", BindingFlags.Static | BindingFlags.NonPublic);
            mappingConventionField.SetValue(null, null);

            var objectInfosField = objectInfoType.GetField("s_objectInfos", BindingFlags.Static | BindingFlags.NonPublic);
            var getObjectInfosMethod = objectInfoType.GetMethod("GetObjectInfos", BindingFlags.Static | BindingFlags.NonPublic);

            var objectInfos = getObjectInfosMethod.Invoke(null, null);
            objectInfosField.SetValue(null, objectInfos);
        }
    }
}
