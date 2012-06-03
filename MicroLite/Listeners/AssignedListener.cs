// -----------------------------------------------------------------------
// <copyright file="AssignedListener.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Listeners
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