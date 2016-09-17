// -----------------------------------------------------------------------
// <copyright file="DelegateFactory.cs" company="MicroLite">
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
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;
    using MicroLite.TypeConverters;

    internal static class DelegateFactory
    {
        private static readonly MethodInfo convertFromDbValueMethod = typeof(ITypeConverter).GetMethod("ConvertFromDbValue", new[] { typeof(IDataReader), typeof(int), typeof(Type) });
        private static readonly MethodInfo convertToDbValueMethod = typeof(ITypeConverter).GetMethod("ConvertToDbValue", new[] { typeof(object), typeof(Type) });
        private static readonly MethodInfo dataRecordGetFieldCount = typeof(IDataRecord).GetProperty("FieldCount").GetGetMethod();
        private static readonly MethodInfo dataRecordGetName = typeof(IDataRecord).GetMethod("GetName");
        private static readonly MethodInfo dataRecordIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
        private static readonly ConstructorInfo sqlArgumentConstructor = typeof(SqlArgument).GetConstructor(new[] { typeof(object), typeof(DbType) });
        private static readonly MethodInfo stringEquals = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) });
        private static readonly MethodInfo typeConverterForMethod = typeof(TypeConverter).GetMethod("For", new[] { typeof(Type) });
        private static readonly MethodInfo typeGetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");

        internal static Func<object, object> CreateGetIdentifier(IObjectInfo objectInfo)
        {
            var dynamicMethod = new DynamicMethod(
                name: "MicroLite" + objectInfo.ForType.Name + "GetIdentifier",
                returnType: typeof(object),
                parameterTypes: new[] { typeof(object) });

            var ilGenerator = dynamicMethod.GetILGenerator();

            // var instance = ({ForType})arg_0;
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, objectInfo.ForType);

            // var identifier = instance.Id;
            ilGenerator.Emit(OpCodes.Callvirt, objectInfo.TableInfo.IdentifierColumn.PropertyInfo.GetGetMethod());

            // value = (object)identifier;
            ilGenerator.EmitBoxIfValueType(objectInfo.TableInfo.IdentifierColumn.PropertyInfo.PropertyType);

            // return identifier;
            ilGenerator.Emit(OpCodes.Ret);

            var getIdentifierValue = (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));

            return getIdentifierValue;
        }

        internal static Func<object, SqlArgument[]> CreateGetInsertValues(IObjectInfo objectInfo)
        {
            var dynamicMethod = new DynamicMethod(
                name: "MicroLite" + objectInfo.ForType.Name + "GetInsertValues",
                returnType: typeof(SqlArgument[]),
                parameterTypes: new[] { typeof(object) },
                m: typeof(ObjectInfo).Module);

            var ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.DeclareLocal(objectInfo.ForType);     // loc_0
            ilGenerator.DeclareLocal(typeof(SqlArgument[]));  // loc_1

            // var instance = ({ForType})arg_0;
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, objectInfo.ForType);
            ilGenerator.Emit(OpCodes.Stloc_0);

            // var values = new SqlArgument[count];
            ilGenerator.EmitEfficientInt(objectInfo.TableInfo.InsertColumnCount);
            ilGenerator.Emit(OpCodes.Newarr, typeof(SqlArgument));
            ilGenerator.Emit(OpCodes.Stloc_1);

            EmitGetPropertyValues(ilGenerator, objectInfo, c => c.AllowInsert);

            // return values;
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ret);

            var getInsertValues = (Func<object, SqlArgument[]>)dynamicMethod.CreateDelegate(typeof(Func<object, SqlArgument[]>));

            return getInsertValues;
        }

        internal static Func<object, SqlArgument[]> CreateGetUpdateValues(IObjectInfo objectInfo)
        {
            var dynamicMethod = new DynamicMethod(
                name: "MicroLite" + objectInfo.ForType.Name + "GetUpdateValues",
                returnType: typeof(SqlArgument[]),
                parameterTypes: new[] { typeof(object) },
                m: typeof(ObjectInfo).Module);

            var ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.DeclareLocal(objectInfo.ForType);     // loc_0
            ilGenerator.DeclareLocal(typeof(SqlArgument[]));  // loc_1

            // var instance = ({ForType})arg_0;
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, objectInfo.ForType);
            ilGenerator.Emit(OpCodes.Stloc_0);

            // var values = new SqlArgument[count + 1]; // Add 1 for the identifier
            ilGenerator.EmitEfficientInt(objectInfo.TableInfo.UpdateColumnCount + 1);
            ilGenerator.Emit(OpCodes.Newarr, typeof(SqlArgument));
            ilGenerator.Emit(OpCodes.Stloc_1);

            EmitGetPropertyValues(ilGenerator, objectInfo, c => c.AllowUpdate);

            // values[values.Length - 1] = entity.{Id};
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.EmitEfficientInt(objectInfo.TableInfo.UpdateColumnCount);
            ilGenerator.Emit(OpCodes.Ldelema, typeof(SqlArgument));
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Callvirt, objectInfo.TableInfo.IdentifierColumn.PropertyInfo.GetGetMethod());

            ilGenerator.EmitBoxIfValueType(objectInfo.TableInfo.IdentifierColumn.PropertyInfo.PropertyType);

            ilGenerator.EmitEfficientInt((int)objectInfo.TableInfo.IdentifierColumn.DbType);
            ilGenerator.Emit(OpCodes.Newobj, sqlArgumentConstructor);

            // values[i] = new SqlArgument(value, column.DbType); OR values[i] = new SqlArgument(converted, column.DbType);
            ilGenerator.Emit(OpCodes.Stobj, typeof(SqlArgument));

            // return values;
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ret);

            var getUpdateValues = (Func<object, SqlArgument[]>)dynamicMethod.CreateDelegate(typeof(Func<object, SqlArgument[]>));

            return getUpdateValues;
        }

        internal static Func<IDataReader, object> CreateInstanceFactory(IObjectInfo objectInfo)
        {
            var dynamicMethod = new DynamicMethod(
                name: "MicroLite" + objectInfo.ForType.Name + "Factory",
                returnType: typeof(object),
                parameterTypes: new[] { typeof(IDataReader) },
                m: typeof(ObjectInfo).Module);

            var ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.DeclareLocal(objectInfo.ForType);     // loc_0 - {Type} instance = null;
            ilGenerator.DeclareLocal(typeof(int));            // loc_1 - int i
            ilGenerator.DeclareLocal(typeof(string));         // loc_2 - string columnName

            var isDBNull = ilGenerator.DefineLabel();
            var getColumnName = ilGenerator.DefineLabel();
            var columnLabels = new Label[objectInfo.TableInfo.Columns.Count];
            var incrementIndex = ilGenerator.DefineLabel();
            var getFieldCount = ilGenerator.DefineLabel();
            var returnEntity = ilGenerator.DefineLabel();

            // var entity = new T();
            ilGenerator.Emit(OpCodes.Newobj, objectInfo.ForType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc_0);

            // var i = 0;
            ilGenerator.EmitEfficientInt(0);
            ilGenerator.Emit(OpCodes.Stloc_1);
            ilGenerator.Emit(OpCodes.Br, getFieldCount);

            // if (dataReader.IsDBNull(i)) { continue; }
            ilGenerator.MarkLabel(isDBNull);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.EmitCall(OpCodes.Callvirt, dataRecordIsDBNull, null);
            ilGenerator.Emit(OpCodes.Brtrue, incrementIndex);

            // var columnName = dataReader.GetName(i);
            ilGenerator.MarkLabel(getColumnName);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.EmitCall(OpCodes.Callvirt, dataRecordGetName, null);
            ilGenerator.Emit(OpCodes.Stloc_2);
            ilGenerator.Emit(OpCodes.Ldloc_2);
            ilGenerator.Emit(OpCodes.Brfalse, incrementIndex);

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var column = objectInfo.TableInfo.Columns[i];
                columnLabels[i] = ilGenerator.DefineLabel();

                // case "{PropertyName}"
                ilGenerator.Emit(OpCodes.Ldloc_2);
                ilGenerator.Emit(OpCodes.Ldstr, column.ColumnName);
                ilGenerator.Emit(OpCodes.Call, stringEquals);
                ilGenerator.Emit(OpCodes.Brtrue, columnLabels[i]);
            }

            ilGenerator.Emit(OpCodes.Br, incrementIndex);

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var column = objectInfo.TableInfo.Columns[i];
                var actualPropertyType = TypeConverter.ResolveActualType(column.PropertyInfo.PropertyType);

                // case "{ColumnName}":
                ilGenerator.MarkLabel(columnLabels[i]);

                if (TypeConverter.For(column.PropertyInfo.PropertyType) == null
                    && typeof(IDataRecord).GetMethod("Get" + actualPropertyType.Name) != null)
                {
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    ilGenerator.EmitCall(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("Get" + actualPropertyType.Name), null);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldloc_0);

                    if (TypeConverter.For(column.PropertyInfo.PropertyType) != null)
                    {
                        // typeConverter = TypeConverter.For(propertyType);
                        ilGenerator.Emit(OpCodes.Ldtoken, column.PropertyInfo.PropertyType);
                        ilGenerator.EmitCall(OpCodes.Call, typeGetTypeFromHandleMethod, null);
                        ilGenerator.EmitCall(OpCodes.Call, typeConverterForMethod, null);
                    }
                    else
                    {
                        // typeConverter = TypeConverter.Default;
                        ilGenerator.EmitCall(OpCodes.Call, typeof(TypeConverter).GetProperty("Default").GetGetMethod(), null);
                    }

                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldloc_1);

                    // var converted = typeConverter.ConvertFromDbValue(reader, i, {propertyType});
                    ilGenerator.Emit(OpCodes.Ldtoken, column.PropertyInfo.PropertyType);
                    ilGenerator.EmitCall(OpCodes.Call, typeGetTypeFromHandleMethod, null);
                    ilGenerator.EmitCall(OpCodes.Callvirt, convertFromDbValueMethod, null);

                    // converted = ({PropertyType})converted;
                    ilGenerator.EmitUnboxOrCast(actualPropertyType);

                }
                if (column.PropertyInfo.PropertyType.IsGenericType)
                {
                    // This is for nullable<T> (e.g. Int?)
                    ilGenerator.Emit(OpCodes.Newobj, column.PropertyInfo.PropertyType.GetConstructor(new[] { actualPropertyType }));
                }

                // entity.{Property} = dataReader.Get{PropertyType}(i);
                ilGenerator.EmitCall(OpCodes.Callvirt, column.PropertyInfo.GetSetMethod(), null);

                ilGenerator.Emit(OpCodes.Br, incrementIndex);
            }

            // i++;
            ilGenerator.MarkLabel(incrementIndex);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.EmitEfficientInt(1);
            ilGenerator.Emit(OpCodes.Add);
            ilGenerator.Emit(OpCodes.Stloc_1);

            // if (i < dataReader.FieldCount)
            ilGenerator.MarkLabel(getFieldCount);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Callvirt, dataRecordGetFieldCount, null);
            ilGenerator.Emit(OpCodes.Blt, isDBNull);

            // return entity;
            ilGenerator.MarkLabel(returnEntity);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            var instanceFactory = (Func<IDataReader, object>)dynamicMethod.CreateDelegate(typeof(Func<IDataReader, object>));

            return instanceFactory;
        }

        internal static Action<object, object> CreateSetIdentifier(IObjectInfo objectInfo)
        {
            var dynamicMethod = new DynamicMethod(
                name: "MicroLite" + objectInfo.ForType.Name + "SetIdentifier",
                returnType: null,
                parameterTypes: new[] { typeof(object), typeof(object) });

            var ilGenerator = dynamicMethod.GetILGenerator();

            // var instance = ({ForType})arg_0;
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, objectInfo.ForType);

            // var value = arg_1;
            ilGenerator.Emit(OpCodes.Ldarg_1);

            // value = ({PropertyType})value;
            ilGenerator.EmitUnboxIfValueType(objectInfo.TableInfo.IdentifierColumn.PropertyInfo.PropertyType);

            // instance.Id = value;
            ilGenerator.Emit(OpCodes.Callvirt, objectInfo.TableInfo.IdentifierColumn.PropertyInfo.GetSetMethod());

            ilGenerator.Emit(OpCodes.Ret);

            var setIdentifierValue = (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));

            return setIdentifierValue;
        }

        private static void EmitGetPropertyValues(ILGenerator ilGenerator, IObjectInfo objectInfo, Func<ColumnInfo, bool> includeColumn)
        {
            var index = 0;

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var column = objectInfo.TableInfo.Columns[i];

                if (!includeColumn(column))
                {
                    continue;
                }

                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.EmitEfficientInt(index++);
                ilGenerator.Emit(OpCodes.Ldelema, typeof(SqlArgument));

                var hasTypeConverter = TypeConverter.For(column.PropertyInfo.PropertyType) != null;

                if (hasTypeConverter)
                {
                    // typeConverter = TypeConverter.For(propertyType);
                    ilGenerator.Emit(OpCodes.Ldtoken, column.PropertyInfo.PropertyType);
                    ilGenerator.EmitCall(OpCodes.Call, typeGetTypeFromHandleMethod, null);
                    ilGenerator.EmitCall(OpCodes.Call, typeConverterForMethod, null);
                }

                // var value = entity.{PropertyName};
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.EmitCall(OpCodes.Callvirt, column.PropertyInfo.GetGetMethod(), null);

                // value = (object)value;
                ilGenerator.EmitBoxIfValueType(column.PropertyInfo.PropertyType);

                if (hasTypeConverter)
                {
                    // var converted = typeConverter.ConvertToDbValue(value, propertyType);
                    ilGenerator.Emit(OpCodes.Ldtoken, column.PropertyInfo.PropertyType);
                    ilGenerator.EmitCall(OpCodes.Call, typeGetTypeFromHandleMethod, null);
                    ilGenerator.EmitCall(OpCodes.Call, convertToDbValueMethod, null);
                }

                ilGenerator.EmitEfficientInt((int)column.DbType);
                ilGenerator.Emit(OpCodes.Newobj, sqlArgumentConstructor);

                // values[i] = new SqlArgument(value, column.DbType); OR values[i] = new SqlArgument(converted, column.DbType);
                ilGenerator.Emit(OpCodes.Stobj, typeof(SqlArgument));
            }
        }
    }
}