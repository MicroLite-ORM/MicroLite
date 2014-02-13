// -----------------------------------------------------------------------
// <copyright file="DelegateFactory.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Mapping
{
    using System;
    using System.Reflection.Emit;

    internal static class DelegateFactory
    {
        internal static Func<object> CreateInstanceFactory(IObjectInfo objectInfo)
        {
            var dynamicMethod = new DynamicMethod(
                name: "MicroLite" + objectInfo.ForType.Name + "Factory",
                returnType: objectInfo.ForType,
                parameterTypes: null,
                owner: objectInfo.ForType);

            var ilGenerator = dynamicMethod.GetILGenerator();

            // var entity = new T();
            ilGenerator.Emit(OpCodes.Newobj, objectInfo.ForType.GetConstructor(Type.EmptyTypes));

            // return entity;
            ilGenerator.Emit(OpCodes.Ret);

            var instanceFactory = (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));

            return instanceFactory;
        }
    }
}