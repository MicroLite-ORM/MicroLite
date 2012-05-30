namespace MicroLite.Core
{
    using System;
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="IObjectBuilder"/>.
    /// </summary>
    internal sealed class ObjectBuilder : IObjectBuilder
    {
        private static readonly ILog log = LogManager.GetLogInstance("MicroLite.ObjectBuilder");

        public T BuildNewInstance<T>(IDataReader reader)
            where T : class, new()
        {
            var objectInfo = ObjectInfo.For(typeof(T));

            log.TryLogDebug(LogMessages.ObjectBuilder_CreatingInstance, objectInfo.ForType.FullName);
            var instance = new T();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);

                var propertyInfo = objectInfo.GetPropertyInfoForColumn(columnName);

                if (propertyInfo != null)
                {
                    try
                    {
                        log.TryLogDebug(LogMessages.ObjectBuilder_SettingPropertyValue, objectInfo.ForType.Name, columnName);
                        propertyInfo.SetValue(instance, reader[i]);
                    }
                    catch (Exception e)
                    {
                        log.TryLogFatal(e.Message, e);
                        throw new MicroLiteException(e.Message, e);
                    }
                }
                else
                {
                    log.TryLogWarn(LogMessages.ObjectBuilder_UnknownProperty, objectInfo.ForType.Name, columnName);
                }
            }

            return instance;
        }
    }
}