// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NetExtender.Types.Exceptions;
using NetExtender.Utilities.Types;

namespace NetExtender.Utilities.Core
{
    [Flags]
    public enum PrimitiveType : Byte
    {
        Default = 0,
        String = 1,
        Decimal = 2,
        Complex = 4,
        TimeSpan = 8,
        DateTime = 16,
        DateTimeOffset = 32,
        All = String | Decimal | Complex | TimeSpan | DateTime | DateTimeOffset
    }

    [Flags]
    public enum AssemblyNameType : Byte
    {
        Default = 0,
        Version = 1,
        Culture = 2,
        PublicKeyToken = 4,
        All = Version | Culture | PublicKeyToken
    }

    public enum MethodVisibilityType : Byte
    {
        Unavailable,
        Private,
        Public
    }

    public enum ReflectionEqualityType : Byte
    {
        NotEquals,
        NameNotEquals,
        TypeNotEquals,
        AttributeNotEquals,
        SignatureNotEquals,
        ReturnTypeNotEquals,
        AccessorNotEquals,
        Equals
    }

    [Flags]
    public enum ConstructorDifferenceStrictMode : Byte
    {
        None = 0,
        Name = 1,
        Access = 2,
        Attribute = 4 | Access,
        CallingConvention = 8,
        Strict = Name | Access | CallingConvention,
        NotStrict = Name | CallingConvention,
        All = Name | Attribute | CallingConvention
    }

    [Flags]
    public enum FieldDifferenceStrictMode : Byte
    {
        None = 0,
        Name = 1,
        Access = 2,
        InitOnly = 4,
        Attribute = 8 | Access | InitOnly,
        Strict = Name | Access | InitOnly,
        NotStrict = Name | InitOnly,
        All = Name | Attribute
    }

    [Flags]
    public enum PropertyDifferenceStrictMode : Byte
    {
        None = 0,
        Name = 1,
        Access = 2,
        Attribute = 8 | Access,
        Strict = Name | Access,
        NotStrict = Name,
        All = Name | Attribute
    }

    [Flags]
    public enum MethodDifferenceStrictMode : Byte
    {
        None = 0,
        Name = 1,
        Access = 2,
        Attribute = 4 | Access,
        CallingConvention = 8,
        Strict = Name | Access | CallingConvention,
        NotStrict = Name | CallingConvention,
        All = Name | Attribute | CallingConvention
    }

    [Flags]
    public enum EventDifferenceStrictMode : Byte
    {
        None = 0,
        Name = 1,
        Multicast = 2,
        Attribute = 4 | Multicast,
        Strict = Name | Attribute,
        NotStrict = Name | Multicast,
        All = Name | Attribute
    }

    [Flags]
    public enum ParameterDifferenceStrictMode : Byte
    {
        None = 0,
        Name = 1,
        In = 2,
        Out = 4,
        Retval = 8,
        Attribute = 16 | In | Out | Retval,
        Optional = 32,
        DefaultValue = 64,
        DefaultValueEquals = 128 | DefaultValue,
        Strict = Name | Attribute | DefaultValueEquals,
        NotStrict = Name | In | Out,
        All = Name | Attribute | Optional | DefaultValueEquals
    }

    public static class ReflectionUtilities
    {
        public static Boolean HasInterface<T>(this Type type) where T : class
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.HasInterface(typeof(T));
        }

        public static Boolean HasInterface<T>(this TypeInfo type) where T : class
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.HasInterface(typeof(T));
        }

        public static Boolean HasInterface(this Type type, Type @interface)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (@interface is null)
            {
                throw new ArgumentNullException(nameof(@interface));
            }

            return type.GetInterfaces().Contains(@interface);
        }

        public static Boolean HasInterface(this TypeInfo type, Type @interface)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (@interface is null)
            {
                throw new ArgumentNullException(nameof(@interface));
            }

            return type.ImplementedInterfaces.Contains(@interface);
        }
        
        public static Boolean HasInterface(this Type type, params Type?[]? interfaces)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (interfaces is null)
            {
                return true;
            }
            
            return interfaces.Length switch
            {
                <= 0 => true,
                1 => interfaces[0] is null || type.GetInterfaces().Contains(interfaces[0]),
                _ => interfaces.WhereNotNull().All(new HashSet<Type>(type.GetInterfaces()).Contains)
            };
        }

        public static Boolean HasInterface(this TypeInfo type, params Type?[]? interfaces)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (interfaces is null)
            {
                return true;
            }

            return interfaces.Length switch
            {
                <= 0 => true,
                1 => interfaces[0] is null || type.ImplementedInterfaces.Contains(interfaces[0]),
                _ => interfaces.WhereNotNull().All(new HashSet<Type>(type.ImplementedInterfaces).Contains)
            };
        }
        
        public static Boolean HasAnyInterface(this Type type, params Type?[]? interfaces)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (interfaces is null)
            {
                return false;
            }
            
            return interfaces.Length switch
            {
                <= 0 => false,
                1 => interfaces[0] is not null && type.GetInterfaces().Contains(interfaces[0]),
                _ => interfaces.WhereNotNull().Any(new HashSet<Type>(type.GetInterfaces()).Contains)
            };
        }

        public static Boolean HasAnyInterface(this TypeInfo type, params Type?[]? interfaces)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (interfaces is null)
            {
                return false;
            }

            return interfaces.Length switch
            {
                <= 0 => false,
                1 => interfaces[0] is not null && type.ImplementedInterfaces.Contains(interfaces[0]),
                _ => interfaces.WhereNotNull().Any(new HashSet<Type>(type.ImplementedInterfaces).Contains)
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type TryGetGenericTypeDefinition(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] TryGetGenericTypeDefinitionInterfaces(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type[] interfaces = type.GetInterfaces();
            interfaces.InnerChange(TryGetGenericTypeDefinition);

            return interfaces;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo TryGetGenericMethodDefinition(this MethodInfo method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[]? TryGetGenericArguments(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType ? type.GetGenericArguments() : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[]? TryGetGenericArguments(this MethodBase method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return method.IsGenericMethod ? method.GetGenericArguments() : null;
        }
        
        public static Type[]? GetGenericArguments(this Type generic, Type @base)
        {
            if (generic is null)
            {
                throw new ArgumentNullException(nameof(generic));
            }

            if (@base is null)
            {
                throw new ArgumentNullException(nameof(@base));
            }

            Type? type = generic;
            while (type is not null && type != typeof(Object))
            {
                if (@base == type.TryGetGenericTypeDefinition())
                {
                    return type.GetGenericArguments();
                }

                type = type.BaseType;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type? TryGetGenericInterface(this Type type, Type @interface)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = type.TryGetGenericTypeDefinition();
            @interface = @interface.TryGetGenericTypeDefinition();
            return type.TryGetGenericTypeDefinitionInterfaces().FirstOrDefault(item => item == @interface);
        }
        
        /// <summary>
        /// Gets the method that called the current method where this method is used. This does not work when used in async methods.
        /// <para>
        /// Note that because of compiler optimization, you should add <see cref="MethodImplAttribute"/> to the method where this method is used and use the <see cref="MethodImplOptions.NoInlining"/> value.
        /// </para>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static MethodBase? GetCallingMethod()
        {
            return new StackFrame(2, false).GetMethod();
        }

        /// <summary>
        /// Gets the type that called the current method where this method is used.
        /// <para>
        /// Note that because of compiler optimization, you should add <see cref="MethodImplAttribute"/> to the method where this method is used and use the <see cref="MethodImplOptions.NoInlining"/> value.
        /// </para>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type? GetCallingType()
        {
            return new StackFrame(2, false).GetMethod()?.DeclaringType;
        }

        /// <summary>
        /// Returns all the public properties of this object whose property type is <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the properties.</typeparam>
        /// <param name="obj">The object.</param>
        public static IEnumerable<PropertyInfo> GetAllPropertiesOfType<T>(this Object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return GetAllPropertiesOfType<T>(obj.GetType());
        }

        /// <summary>
        /// Returns all the public properties of this type whose property type is <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the properties.</typeparam>
        /// <param name="type">The type of which to get the properties.</param>
        public static IEnumerable<PropertyInfo> GetAllPropertiesOfType<T>(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetProperties().Where(info => info.PropertyType == typeof(T));
        }

        /// <summary>
        /// Gets the value of the property or field with the specified name in this object or type.
        /// </summary>
        /// <param name="obj">The object or type that has the property or field.</param>
        /// <param name="name">The name of the property or field.</param>
        public static Object? GetPropertyValue(this Object obj, String name)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (GetPropertyValue(obj, name, out Object? result))
            {
                return result;
            }

            throw new Exception($"'{name}' is neither a property or a field of type '{result}'.");
        }

        /// <summary>
        /// Gets the value of the property or field with the specified name in this object or type.
        /// </summary>
        /// <param name="obj">The object or type that has the property or field.</param>
        /// <param name="name">The name of the property or field.</param>
        /// <param name="result">Object if successful else <see cref="obj"/> <see cref="Type"/></param>
        public static Boolean GetPropertyValue(this Object obj, String name, out Object? result)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (obj is not Type type)
            {
                type = obj.GetType();
            }

            const BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            PropertyInfo? property = type.GetProperty(name, binding);
            if (property is not null)
            {
                result = property.GetValue(obj);
                return true;
            }

            FieldInfo? field = type.GetField(name, binding);
            if (field is not null)
            {
                result = field.GetValue(obj);
                return true;
            }

            result = type;
            return false;
        }

        /// <summary>
        /// Gets the type of the specified property in the type.
        /// <para>
        /// If the type is nullable, this function gets its generic definition."/>.
        /// </para>
        /// </summary>
        /// <param name="type">The type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        public static Type GetPropertyType(this Type type, String name)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type property = GetPropertyTypeReal(type, name);

            // get the generic type of nullable, not THE nullable
            if (property.IsGenericType && property.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                property = FirstGenericArgument(property);
            }

            return property;
        }

        /// <summary>
        /// Gets the type of the specified property in the type.
        /// <para>
        /// If the type is nullable, this function gets its generic definition."/>.
        /// </para>
        /// </summary>
        /// <param name="type">The type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="binding">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted. -or- Zero, to return null.</param>
        public static Type GetPropertyType(this Type type, String name, BindingFlags binding)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type property = GetPropertyTypeReal(type, name, binding);

            // get the generic type of nullable, not THE nullable
            if (property.IsGenericType && property.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                property = FirstGenericArgument(property);
            }

            return property;
        }

        /// <summary>
        /// Gets the type of the specified property in the type.
        /// </summary>
        /// <param name="type">The type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        public static Type GetPropertyTypeReal(this Type type, String name)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            PropertyInfo property = GetPropertyInfo(type, name);
            return property.PropertyType;
        }

        /// <summary>
        /// Gets the type of the specified property in the type.
        /// </summary>
        /// <param name="type">The type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="binding">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted. -or- Zero, to return null.</param>
        public static Type GetPropertyTypeReal(this Type type, String name, BindingFlags binding)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            PropertyInfo property = GetPropertyInfo(type, name, binding);
            return property.PropertyType;
        }

        /// <summary>
        /// Gets the property information by name for the type of the object.
        /// </summary>
        /// <param name="obj">Object with a type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        public static PropertyInfo GetPropertyInfo(this Object obj, String name)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return GetPropertyInfo(obj.GetType(), name);
        }

        /// <summary>
        /// Gets the property information by name for the type of the object.
        /// </summary>
        /// <param name="obj">Object with a type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="binding">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted. -or- Zero, to return null.</param>
        public static PropertyInfo GetPropertyInfo(this Object obj, String name, BindingFlags binding)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return GetPropertyInfo(obj.GetType(), name, binding);
        }

        /// <summary>
        /// Gets the property information by name for the type.
        /// </summary>
        /// <param name="type">Type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        public static PropertyInfo GetPropertyInfo(this Type type, String name)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            PropertyInfo? property = type.GetProperty(name);

            if (property is null)
            {
                throw new Exception($"The provided property name ({name}) does not exist in type '{type}'.");
            }

            return property;
        }

        /// <summary>
        /// Gets the property information by name for the type.
        /// </summary>
        /// <param name="type">Type that has the specified property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="binding">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted. -or- Zero, to return null.</param>
        public static PropertyInfo GetPropertyInfo(this Type type, String name, BindingFlags binding)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            PropertyInfo? property = type.GetProperty(name, binding);

            if (property is null)
            {
                throw new Exception($"The provided property name ({name}) does not exist in type '{type}'.");
            }

            return property;
        }

        public static Boolean IsIndexer(this PropertyInfo info)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            return info.GetIndexParameters().Length > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsVoid(this Type? type)
        {
            return type == typeof(void);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsVoid(this MethodInfo method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return method.ReturnType.IsVoid();
        }

        public static FieldAttributes Access(this FieldAttributes attributes)
        {
            return attributes & FieldAttributes.FieldAccessMask;
        }

        public static MethodAttributes Access(this MethodAttributes attributes)
        {
            return attributes & MethodAttributes.MemberAccessMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodVisibilityType IsRead(this PropertyInfo info)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (!info.CanRead)
            {
                return MethodVisibilityType.Unavailable;
            }

            return info.GetMethod?.IsPublic switch
            {
                null => MethodVisibilityType.Unavailable,
                false => MethodVisibilityType.Private,
                true => MethodVisibilityType.Public
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodVisibilityType IsWrite(this PropertyInfo info)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (!info.CanWrite)
            {
                return MethodVisibilityType.Unavailable;
            }

            return info.SetMethod?.IsPublic switch
            {
                null => MethodVisibilityType.Unavailable,
                false => MethodVisibilityType.Private,
                true => MethodVisibilityType.Public
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean? ToBoolean(this MethodVisibilityType type)
        {
            return type switch
            {
                MethodVisibilityType.Unavailable => null,
                MethodVisibilityType.Private => false,
                MethodVisibilityType.Public => true,
                _ => throw new EnumUndefinedOrNotSupportedException<MethodVisibilityType>(type, nameof(type), null)
            };
        }

        /// <summary>
        /// Gets the field information by name for the type of the object.
        /// </summary>
        /// <param name="obj">Object with a type that has the specified field.</param>
        /// <param name="name">The name of the field.</param>
        public static FieldInfo GetFieldInfo(this Object obj, String name)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return GetFieldInfo(obj.GetType(), name);
        }

        /// <summary>
        /// Gets the field information by name for the type.
        /// </summary>
        /// <param name="type">Type that has the specified field.</param>
        /// <param name="name">The name of the field.</param>
        public static FieldInfo GetFieldInfo(this Type type, String name)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field is null)
            {
                throw new Exception($"The provided property name ({name}) does not exist in type '{type}'.");
            }

            return field;
        }

        /// <summary>
        /// Returns the first definition of generic type of this generic type.
        /// </summary>
        /// <param name="type">The type from which to get the generic type.</param>
        public static Type FirstGenericArgument(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type[] arguments = type.GetGenericArguments();

            if (arguments.Length <= 0)
            {
                throw new ArgumentException($"The provided type ({type}) is not a generic type.", nameof(type));
            }

            return arguments[0];
        }

        /// <summary>
        /// Gets the constants defined in this type.
        /// </summary>
        /// <param name="type">The type from which to get the constants.</param>
        public static IEnumerable<FieldInfo> GetConstants(this Type type)
        {
            return GetConstants(type, true);
        }

        /// <summary>
        /// Gets the constants defined in this type.
        /// </summary>
        /// <param name="type">The type from which to get the constants.</param>
        /// <param name="inherited">Determines whether or not to include inherited constants.</param>
        public static IEnumerable<FieldInfo> GetConstants(this Type type, Boolean inherited)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            BindingFlags binding = BindingFlags.Public | BindingFlags.Static | (inherited ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            return type.GetFields(binding).Where(field => field.IsLiteral && !field.IsInitOnly);
        }

        public static IEnumerable<FieldInfo> GetAccessibleFields(this Type type)
        {
            return GetAccessibleMembers(type, RuntimeReflectionExtensions.GetRuntimeFields);
        }

        public static IEnumerable<PropertyInfo> GetAccessibleProperties(this Type type)
        {
            return GetAccessibleMembers(type, RuntimeReflectionExtensions.GetRuntimeProperties);
        }

        public static IEnumerable<EventInfo> GetAccessibleEvents(this Type type)
        {
            return GetAccessibleMembers(type, RuntimeReflectionExtensions.GetRuntimeEvents);
        }

        public static IEnumerable<MethodInfo> GetAccessibleMethods(this Type type)
        {
            return GetAccessibleMembers(type, RuntimeReflectionExtensions.GetRuntimeMethods);
        }

        private static IEnumerable<T> GetAccessibleMembers<T>(this Type type, Func<Type, IEnumerable<T>> finder) where T : MemberInfo
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }

            List<T> result = new List<T>(finder(type));

            if (!type.IsInterface)
            {
                return result.ToArray();
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsVisible)
                {
                    result.AddRange(finder.Invoke(@interface));
                }
            }

            result.AddRange(finder.Invoke(typeof(Object)));
            return result.ToArray();
        }

        /// <summary>
        /// Sets the value of the property or field with the specified name in this object or type.
        /// </summary>
        /// <param name="obj">The objector type that has the property or field.</param>
        /// <param name="name">The name of the property or field.</param>
        /// <param name="value">The value to set.</param>
        public static T SetValue<T>(this T obj, String name, Object? value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (obj is not Type type)
            {
                type = obj.GetType();
            }

            PropertyInfo? property = type.GetProperty(name);
            if (property is not null)
            {
                property.SetValue(obj, value);
                return obj;
            }

            FieldInfo? field = type.GetField(name);

            if (field is null)
            {
                throw new Exception($"'{name}' is neither a property or a field of type '{type}'.");
            }

            field.SetValue(obj, value);
            return obj;
        }

        /// <summary>
        /// Sets the specified field to the provided value in the object.
        /// </summary>
        /// <param name="obj">The object with the field.</param>
        /// <param name="name">The name of the field to set.</param>
        /// <param name="value">The value to set the field to.</param>
        public static T SetField<T>(this T obj, String name, Object value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            FieldInfo field = GetFieldInfo(obj, name);
            field.SetValue(obj, value);
            return obj;
        }

        /// <summary>
        /// Sets the specified property to the provided value in the object.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static T SetProperty<T>(this T obj, String name, Object value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            PropertyInfo property = GetPropertyInfo(obj, name);
            property.SetValue(obj, value);
            return obj;
        }

        /// <summary>
        /// Sets the specified property to the provided value in the object.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="binding">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted. -or- Zero, to return null.</param>
        public static T SetProperty<T>(this T obj, String name, Object value, BindingFlags binding)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            PropertyInfo property = GetPropertyInfo(obj, name, binding);
            property.SetValue(obj, value);
            return obj;
        }

        /// <summary>
        /// Sets the specified property to a value that will be extracted from the provided string value using the <see cref="TypeDescriptor.GetConverter(Type)"/> and <see cref="TypeConverter.ConvertFromString(string)"/>.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The string representation of the value to set to the property.</param>
        public static T SetPropertyFromString<T>(this T obj, String name, String value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            PropertyInfo property = GetPropertyInfo(obj, name);
            TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
            Object? result = converter.ConvertFromString(value);
            property.SetValue(obj, result);
            return obj;
        }

        /// <summary>
        /// Sets the specified property to a value that will be extracted from the provided string value using the <see cref="TypeDescriptor.GetConverter(Type)"/> and <see cref="TypeConverter.ConvertFromString(string)"/>.
        /// </summary>
        /// <param name="obj">The object with the property.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The string representation of the value to set to the property.</param>
        /// <param name="binding">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted. -or- Zero, to return null.</param>
        public static T SetPropertyFromString<T>(this T obj, String name, String value, BindingFlags binding)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            PropertyInfo property = GetPropertyInfo(obj, name, binding);
            TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
            Object? result = converter.ConvertFromString(value);
            property.SetValue(obj, result);
            return obj;
        }

        /// <summary>
        /// Sets all fields and properties of the specified type in the provided object to the specified value.
        /// <para>Internal, protected and private fields are included, static are not.</para>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="TValue">The type of the properties.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value to set the properties to.</param>
        public static T SetAllPropertiesOfType<T, TValue>(this T obj, TValue value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(TValue))
                {
                    field.SetValue(obj, value);
                }
            }

            return obj;
        }

        /// <summary>
        /// Determines whether or not this object has a property with the specified name.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name of the property.</param>
        public static Boolean HasProperty(this Object obj, String name)
        {
            return HasProperty(obj, name, true);
        }

        /// <summary>
        /// Determines whether or not this object has a property with the specified name.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="inherited">Determines whether of not to include inherited properties.</param>
        public static Boolean HasProperty(this Object obj, String name, Boolean inherited)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Type type = obj.GetType();
            return HasProperty(type, name, inherited);
        }

        /// <summary>
        /// Determines whether or not this type has a property with the specified name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the property.</param>
        public static Boolean HasProperty(this Type type, String name)
        {
            return HasProperty(type, name, true);
        }

        /// <summary>
        /// Determines whether or not this type has a property with the specified name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="inherited">Determines whether of not to include inherited properties.</param>
        public static Boolean HasProperty(this Type type, String name, Boolean inherited)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            BindingFlags binding = BindingFlags.Public | BindingFlags.Instance | (inherited ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);

            PropertyInfo? property = type.GetProperty(name, binding);
            return property is not null;
        }

        private static Boolean IsMulticastDelegateFieldType(this Type? type)
        {
            return type is not null && (type == typeof(MulticastDelegate) || type.IsSubclassOf(typeof(MulticastDelegate)));
        }

        public static FieldInfo? GetEventField(this Type type, String name)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Type? current = type;
            FieldInfo? field = null;
            while (current is not null)
            {
                const BindingFlags binding = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
                field = current.GetField(name, binding);

                if (field is not null && field.FieldType.IsMulticastDelegateFieldType())
                {
                    return field;
                }

                field = current.GetField("EVENT_" + name.ToUpper(), binding);

                if (field is not null && field.FieldType.IsMulticastDelegateFieldType())
                {
                    return field;
                }

                current = current.BaseType;
            }

            return field;
        }

        public static FieldInfo? GetEventField<T>(this Type type) where T : Delegate
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            Type? current = type;
            while (current is not null)
            {
                const BindingFlags binding = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo? field = current.GetFields(binding).FirstOrDefault(field => field.FieldType.IsMulticastDelegateFieldType() && field.FieldType.IsAssignableFrom(typeof(T)));
                
                if (field is not null)
                {
                    return field;
                }

                current = current.BaseType;
            }

            return null;
        }

        public static FieldInfo[] GetEventFields(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type? current = type;
            List<FieldInfo> fields = new List<FieldInfo>(8);
            while (current is not null)
            {
                const BindingFlags binding = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
                fields.AddRange(current.GetFields(binding).Where(field => field.FieldType.IsMulticastDelegateFieldType()));
                current = current.BaseType;
            }

            return fields.ToArray();
        }
        
        public static FieldInfo[] GetEventFields<T>(this Type type) where T : Delegate
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type? current = type;
            List<FieldInfo> fields = new List<FieldInfo>(8);
            while (current is not null)
            {
                const BindingFlags binding = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
                fields.AddRange(current.GetFields(binding).Where(field => field.FieldType.IsMulticastDelegateFieldType() && field.FieldType.IsAssignableFrom(typeof(T))));
                current = current.BaseType;
            }

            return fields.ToArray();
        }
        
        public static MethodInfo? GetEventRaiseMethod(this Type type, String name)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return type.GetEventField(name)?.FieldType.GetMethod("Invoke");
        }
        
        public static MethodInfo? GetEventRaiseMethod<T>(this Type type) where T : Delegate
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetEventField<T>()?.FieldType.GetMethod("Invoke");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MulticastDelegate? GetEventDelegate(this Type type, String name, Object? @object)
        {
            return GetEventDelegate(type, name, @object, out _);
        }

        public static MulticastDelegate? GetEventDelegate(this Type type, String name, Object? @object, out FieldInfo? field)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            field = type.GetEventField(name);
            return (MulticastDelegate?) field?.GetValue(@object);
        }

        public static T? GetEventDelegate<T>(this Type type, Object? @object) where T : Delegate
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            KeyValuePair<FieldInfo, T>[] delegates = GetEventDelegates<T>(type, @object);

            return delegates.Length switch
            {
                <= 0 => null,
                1 => delegates[0].Value,
                _ => throw new AmbiguousMatchException()
            };
        }
        
        public static MethodInfo? GetMethod(this Type type, String name, BindingFlags bindingAttr, Type[] types)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            return type.GetMethod(name, bindingAttr, null, types, null);
        }

        public static T? GetEventDelegate<T>(this Type type, Object? @object, out FieldInfo? field) where T : Delegate
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            KeyValuePair<FieldInfo, T>[] delegates = GetEventDelegates<T>(type, @object);

            switch (delegates.Length)
            {
                case <= 0:
                    field = null;
                    return null;
                case 1:
                    field = delegates[0].Key;
                    return delegates[0].Value;
                default:
                    throw new AmbiguousMatchException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetEventDelegate<T>(this Type type, String name, Object? @object) where T : Delegate
        {
            return GetEventDelegate(type, name, @object) as T;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetEventDelegate<T>(this Type type, String name, Object? @object, out FieldInfo? field) where T : Delegate
        {
            return GetEventDelegate(type, name, @object, out field) as T;
        }
        
        public static KeyValuePair<FieldInfo, T>[] GetEventDelegates<T>(this Type type, Object? @object) where T : Delegate
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetEventFields<T>().Select(field => new KeyValuePair<FieldInfo, T?>(field, field.GetValue(@object) as T)).Where(pair => pair.Value is not null).ToArray();
        }

        public static Boolean ClearEventInvocations(Object value, String name)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            try
            {
                FieldInfo? info = value.GetType().GetEventField(name);

                if (info is null)
                {
                    return false;
                }

                info.SetValue(value, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Boolean Implements<TValue>(Object? value)
        {
            return Implements(value, typeof(TValue));
        }

        public static Boolean Implements(Object? value, Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return value is not null && Implements(value.GetType(), type);
        }

        public static Boolean Implements<TValue>(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Implements(type, typeof(TValue));
        }

        public static Boolean Implements(this Type type, Type? other)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (other is null)
            {
                return false;
            }

            return type.IsGenericTypeDefinition ? type.ImplementsGeneric(type) : type.IsAssignableFrom(type);
        }

        public static Boolean ImplementsGeneric(this Type type, Type @interface)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (@interface is null)
            {
                throw new ArgumentNullException(nameof(@interface));
            }

            return type.ImplementsGeneric(@interface, out _);
        }

        public static Boolean ImplementsGeneric(this Type type, Type @interface, out Type? result)
        {
            result = null;

            if (@interface.IsInterface)
            {
                result = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == @interface);

                if (result is not null)
                {
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == @interface)
            {
                result = type;
                return true;
            }

            Type? baseType = type.BaseType;
            return baseType is not null && baseType.ImplementsGeneric(@interface, out result);
        }

        /// <summary>
        /// Determines whether the specified type to check derives from this generic type.
        /// </summary>
        /// <param name="generic">The parent generic type.</param>
        /// <param name="type">The type to check if it derives from the specified generic type.</param>
        public static Boolean IsSubclassOfRawGeneric(this Type generic, Type? type)
        {
            if (generic is null)
            {
                throw new ArgumentNullException(nameof(generic));
            }

            while (type is not null && type != typeof(Object))
            {
                if (generic == type.TryGetGenericTypeDefinition())
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean IsSuperclassOfRawGeneric(this Type type, Type? generic)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return generic is not null && IsSubclassOfRawGeneric(generic, type);
        }
        
        /// <inheritdoc cref="IsPrimitive(System.Type,PrimitiveType)"/>
        public static Boolean IsPrimitive(this Type type)
        {
            return IsPrimitive(type, PrimitiveType.All);
        }

        /// <summary>
        /// Determines whether this type is a primitive.
        /// <para><see cref="string"/> is considered a primitive.</para>
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="primitive">What type is primitive</param>
        public static Boolean IsPrimitive(this Type type, PrimitiveType primitive)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsPrimitive ||
                   primitive.HasFlag(PrimitiveType.String) && type == typeof(String) ||
                   primitive.HasFlag(PrimitiveType.Decimal) && type == typeof(Decimal) ||
                   primitive.HasFlag(PrimitiveType.Complex) && type == typeof(Complex) ||
                   primitive.HasFlag(PrimitiveType.TimeSpan) && type == typeof(TimeSpan) ||
                   primitive.HasFlag(PrimitiveType.DateTime) && type == typeof(DateTime) ||
                   primitive.HasFlag(PrimitiveType.DateTimeOffset) && type == typeof(DateTimeOffset);
        }

        public static Boolean IsCompilerGenerated(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsDefined(typeof(CompilerGeneratedAttribute));
        }

        private static class SizeCache<T> where T : struct
        {
            // ReSharper disable once StaticMemberInGenericType
            public static Int32 Size { get; }

            static SizeCache()
            {
                Size = GetSize(typeof(T));
            }
        }

        private static class SizeCache
        {
            private static ConcurrentDictionary<Type, Int32> Cache { get; } = new ConcurrentDictionary<Type, Int32>();

            public static Int32 GetTypeSize(Type type)
            {
                if (Cache.TryGetValue(type, out Int32 size))
                {
                    return size;
                }

                if (!type.IsValueType)
                {
                    throw new ArgumentException(@"Is not value type", nameof(type));
                }

                DynamicMethod method = new DynamicMethod("SizeOfType", typeof(Int32), Type.EmptyTypes);
                ILGenerator il = method.GetILGenerator();
                il.Emit(OpCodes.Sizeof, type);
                il.Emit(OpCodes.Ret);

                Object? value = method.Invoke(null, null);

                if (value is null)
                {
                    return 0;
                }

                size = (Int32) value;
                Cache.TryAdd(type, size);
                return size;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 GetSize<T>(this T _) where T : struct
        {
            return GetSize<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 GetSize<T>(this T[] array) where T : struct
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return SizeCache<T>.Size * array.Length;
        }

        public static Boolean GetSize<T>(this T[] array, out Int64 size) where T : struct
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            try
            {
                size = SizeCache<T>.Size * array.LongLength;
                return true;
            }
            catch (OverflowException)
            {
                size = default;
                return false;
            }
        }

        public static Boolean GetSize<T>(this T[] array, out BigInteger size) where T : struct
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            try
            {
                size = SizeCache<T>.Size * (BigInteger) array.LongLength;
                return true;
            }
            catch (OverflowException)
            {
                size = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 GetSize<T>() where T : struct
        {
            return SizeCache<T>.Size;
        }

        public static Int32 GetSize(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsValueType)
            {
                throw new ArgumentException(@"Is not value type", nameof(type));
            }

            return SizeCache.GetTypeSize(type);
        }

        private static class GenericDefaultCache
        {
            private static ConcurrentDictionary<Type, ValueType> Values { get; } = new ConcurrentDictionary<Type, ValueType>();

            public static Object? Default(Type type)
            {
                if (type is null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                static ValueType Create(Type type)
                {
                    return (ValueType) Activator.CreateInstance(type)!;
                }

                return type.IsValueType ? Values.GetOrAdd(type, Create) : null;
            }
        }

        public static IEnumerator<StackFrame> GetEnumerator(this StackTrace stack)
        {
            if (stack is null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            Int32 i = 0;
            while (i < stack.FrameCount && stack.GetFrame(i++) is { } frame)
            {
                yield return frame;
            }
        }

        public static IEnumerable<Type> GetStackTypes(this StackTrace stack)
        {
            if (stack is null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            foreach (StackFrame frame in stack)
            {
                if (frame.GetMethod()?.DeclaringType is { } type)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetStackTypesUnique(this StackTrace stack)
        {
            if (stack is null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            return stack.GetStackTypes().Distinct();
        }

        /// <summary>
        /// Gets the default value of this type.
        /// </summary>
        /// <param name="type">The type for which to get the default value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Object? Default(Type type)
        {
            return GenericDefaultCache.Default(type);
        }

        /// <summary>
        /// Gets the default value of the specified type.
        /// </summary>
        /// <typeparam name="T">The type for which to get the default value.</typeparam>
        public static T? Default<T>()
        {
            return default;
        }
    }
}