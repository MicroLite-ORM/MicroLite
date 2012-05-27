namespace MicroLite.Core
{
    /// <summary>
    /// The implementation of <see cref="IListener"/> for checking the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.Assigned is used.
    /// </summary>
    internal sealed class AssignedListener : Listener
    {
        public override void BeforeInsert(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Assigned)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.Assigned_IdentifierNotSetForInsert);
                }
            }
        }

        public override void BeforeUpdate(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Assigned)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.Assigned_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}