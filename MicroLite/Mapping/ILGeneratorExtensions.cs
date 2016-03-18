// -----------------------------------------------------------------------
// <copyright file="ILGeneratorExtensions.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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

    /// <summary>
    /// Extension methods for the ILGenerator class.
    /// </summary>
    internal static class ILGeneratorExtensions
    {
        /// <summary>
        /// Emits the box <see cref="OpCode"/> if the type is a value type.
        /// </summary>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="type">The type.</param>
        internal static void EmitBoxIfValueType(this ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// Emits the load integer <see cref="OpCode"/> using the most efficient method.
        /// </summary>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="value">The value.</param>
        internal static void EmitEfficientInt(this ILGenerator ilGenerator, int value)
        {
            switch (value)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    return;

                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    return;

                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    return;

                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    return;

                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    return;

                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    return;

                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    return;

                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    return;

                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    return;

                default:
                    ilGenerator.Emit(OpCodes.Ldc_I4, value);
                    return;
            }
        }

        /// <summary>
        /// Emits the unbox <see cref="OpCode"/> if the type is a value type.
        /// </summary>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="type">The type.</param>
        internal static void EmitUnboxIfValueType(this ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
        }

        /// <summary>
        /// Emits either an unbox <see cref="OpCode"/> if the type is a value type or a cast to the type.
        /// </summary>
        /// <param name="ilGenerator">The il generator.</param>
        /// <param name="type">The type.</param>
        internal static void EmitUnboxOrCast(this ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }
    }
}