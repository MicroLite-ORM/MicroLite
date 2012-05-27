namespace MicroLite.Core
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The implementation of <see cref="IListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.DbGenerated is used.
    /// </summary>
    internal sealed class DbGeneratedListener : Listener
    {
        public override void AfterInsert(object instance, object executeScalarResult)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                var propertyInfo = objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

                var identifierValue = Convert.ChangeType(executeScalarResult, propertyInfo.PropertyType, CultureInfo.InvariantCulture);

                propertyInfo.SetValue(instance, identifierValue, null);
            }
        }

        public override void BeforeInsert(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (!objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.DbGenerated_IdentifierSetForInsert);
                }
            }
        }

        public override void BeforeInsert(Type forType, SqlQuery sqlQuery)
        {
            var objectInfo = ObjectInfo.For(forType);

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                sqlQuery.InnerSql.Append(";SELECT SCOPE_IDENTITY()");
            }
        }

        public override void BeforeUpdate(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.DbGenerated_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}