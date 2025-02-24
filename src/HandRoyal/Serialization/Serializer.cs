using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bencodex;
using Bencodex.Types;
using static HandRoyal.Serialization.ArrayUtility;

namespace HandRoyal.Serialization;

public static class Serializer
{
    public static readonly Type[] SupportedBaseTypes =
    [
        typeof(int),
        typeof(long),
        typeof(string),
        typeof(bool),
        typeof(BigInteger),
        typeof(byte[]),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
    ];

    private const int ObjectValue = 0x_0000_0001;
    private const int ArrayValue = 0x_0000_0002;
    private const int ImmutableArrayValue = 0x_0000_0003;

    private static readonly ConcurrentDictionary<Type, ImmutableArray<PropertyInfo>>
        _propertiesByType = new();

    private static readonly ConcurrentDictionary<string, Type> _typeByName = new();

    static Serializer()
    {
        var assembly = typeof(Serializer).Assembly;
        var types = GetSerializableTypes(assembly);
        foreach (var type in types)
        {
            var typeName = type.FullName
                ?? throw new UnreachableException("Type does not have FullName");
            _typeByName[typeName] = type;
        }
    }

    public static ImmutableArray<PropertyInfo> GetProperties(Type type)
    {
        if (!type.IsDefined(typeof(ModelAttribute)))
        {
            throw new ArgumentException(
                $"Type {type} does not have {nameof(ModelAttribute)}", nameof(type));
        }

        if (!_propertiesByType.TryGetValue(type, out var properties))
        {
            properties = CreateProperties(type);
            _propertiesByType[type] = properties;
        }

        return properties;
    }

    public static Type GetType(string typeName)
    {
        if (!_typeByName.TryGetValue(typeName, out var type))
        {
            type = Type.GetType(typeName)
                ?? throw new ArgumentException($"Type {typeName} is not found", nameof(typeName));
            _typeByName[typeName] = type;
        }

        return type;
    }

    public static bool TryGetType(string typeName, [MaybeNullWhen(false)] out Type type)
    {
        if (!_typeByName.TryGetValue(typeName, out type))
        {
            type = Type.GetType(typeName);
            if (type is not null)
            {
                _typeByName[typeName] = type;
            }
        }

        return type is not null;
    }

    public static bool TryGetType(IValue value, [MaybeNullWhen(false)] out Type type)
    {
        if (SerializationData.TryGetObject(value, out var data))
        {
            var header = data.Header;
            if (header.TypeValue == ObjectValue)
            {
                return TryGetType(header.TypeName, out type);
            }
            else if (header.TypeValue == ArrayValue)
            {
                var elementType = GetType(header.TypeName);
                type = GetArrayType(elementType);
                return true;
            }
            else if (header.TypeValue == ImmutableArrayValue)
            {
                var elementType = GetType(header.TypeName);
                type = GetImmutableArrayType(elementType);
                return true;
            }
        }

        type = null;
        return false;
    }

    public static bool CanSupportType(Type type)
    {
        if (SupportedBaseTypes.Contains(type)
            || type.IsEnum
            || IsBencodableType(type)
            || IsBencodexType(type))
        {
            return true;
        }

        if (type.IsDefined(typeof(ModelAttribute)))
        {
            return true;
        }

        if (IsSupportedArrayType(type, out var elementType))
        {
            return CanSupportType(elementType);
        }

        return false;
    }

    public static IValue Serialize(object? obj)
    {
        if (obj is null)
        {
            return Null.Value;
        }

        var type = obj.GetType();
        if (SupportedBaseTypes.Contains(type)
            || type.IsEnum
            || IsBencodableType(type)
            || IsBencodexType(type)
            || IsSupportedArrayType(type))
        {
            return SerializeValue(obj, type);
        }

        var typeName = GetTypeName(type);
        var version = GetVersion(type);
        var propertyInfos = GetProperties(type);
        var valueList = new List<IValue>(propertyInfos.Length);
        foreach (var propInfo in propertyInfos)
        {
            var value = propInfo.GetValue(obj);
            var serialized = SerializeValue(value, propInfo.PropertyType);
            valueList.Add(serialized);
        }

        var data = new SerializationData
        {
            Header = new SerializationHeader
            {
                TypeValue = ObjectValue,
                TypeName = typeName,
                Version = version,
            },
            Value = new List(valueList),
        };
        return data.Bencoded;
    }

    public static object? Deserialize(IValue value, Type type)
    {
        if (SupportedBaseTypes.Contains(type)
            || type.IsEnum
            || IsBencodableType(type)
            || IsBencodexType(type)
            || IsSupportedArrayType(type))
        {
            return DeserializeValue(type, value);
        }

        var data = SerializationData.GetObject(value);
        var header = data.Header;

        if (header.TypeValue != ObjectValue)
        {
            throw new ArgumentException(
                $"Given magic value {header.TypeValue} is not {ObjectValue}", nameof(value));
        }

        var typeName = GetTypeName(type);
        if (header.TypeName != typeName)
        {
            throw new ArgumentException(
                $"Given type name {header.TypeName} is not {typeName}", nameof(value));
        }

        var version = GetVersion(type);
        if (header.Version != version)
        {
            throw new ArgumentException(
                $"Given version {header.Version} is not {version}", nameof(value));
        }

        if (data.Value is not List list)
        {
            throw new ArgumentException(
                $"The value is not a list: {data.Value}", nameof(value));
        }

        var propertyInfos = GetProperties(type);
        var obj = Activator.CreateInstance(type)
            ?? throw new ArgumentException($"Failed to create an instance of {type}", nameof(type));

        for (var i = 0; i < propertyInfos.Length; i++)
        {
            var propertyInfo = propertyInfos[i];
            var propertyAttribute = propertyInfo.GetCustomAttribute<PropertyAttribute>()
                ?? throw new UnreachableException(
                    "Property does not have SerializablePropertyAttribute");
            var propertyIndex = propertyAttribute.Index;
            var propertyType = propertyInfo.PropertyType;
            var propertyValue = list[propertyIndex];
            var deserializedValue = DeserializeValue(propertyType, propertyValue);
            propertyInfo.SetValue(obj, deserializedValue);
        }

        return obj;
    }

    public static T? Deserialize<T>(IValue value)
    {
        if (Deserialize(value, typeof(T)) is T obj)
        {
            return obj;
        }

        return default;
    }

    private static int GetVersion(Type type)
    {
        if (SupportedBaseTypes.Contains(type)
            || type.IsEnum
            || IsBencodableType(type)
            || IsBencodexType(type))
        {
            return 0;
        }

        var attribute = type.GetCustomAttribute<ModelAttribute>()
            ?? throw new UnreachableException("Type does not have SerializableObjectAttribute");
        return attribute.Version;
    }

    private static IValue SerializeValue(object? value, Type propertyType)
    {
        if (value is IBencodable bencodable)
        {
            return bencodable.Bencoded;
        }
        else if (value is IValue bencoded)
        {
            return bencoded;
        }
        else if (value is null)
        {
            return Null.Value;
        }
        else if (value is int @int)
        {
            return new Integer(@int);
        }
        else if (value is long @long)
        {
            return new Integer(@long);
        }
        else if (value is BigInteger bigInteger)
        {
            return new Integer(bigInteger);
        }
        else if (value is string @string)
        {
            return new Text(@string);
        }
        else if (value is bool @bool)
        {
            return new Bencodex.Types.Boolean(@bool);
        }
        else if (value is byte[] bytes)
        {
            return new Binary(bytes);
        }
        else if (value is DateTimeOffset dateTimeOffset)
        {
            return new Integer(dateTimeOffset.UtcTicks);
        }
        else if (value is TimeSpan timeSpan)
        {
            return new Integer(timeSpan.Ticks);
        }
        else if (propertyType.IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(propertyType);
            if (underlyingType == typeof(int))
            {
                return new Integer((int)value);
            }
            else if (underlyingType == typeof(long))
            {
                return new Integer((long)value);
            }
        }
        else if (IsArray(propertyType, out var elementType))
        {
            var items = (IList)value;
            if (items.Count == 0)
            {
                return Null.Value;
            }

            var list = new List<IValue>(items.Count);
            var typeName = GetTypeName(elementType);
            var version = GetVersion(elementType);

            foreach (var item in items)
            {
                list.Add(SerializeValue(item, elementType));
            }

            var data = new SerializationData
            {
                Header = new SerializationHeader
                {
                    TypeValue = ArrayValue,
                    TypeName = typeName,
                    Version = version,
                },
                Value = new List(list),
            };

            return data.Bencoded;
        }
        else if (IsImmutableArray(propertyType, out elementType))
        {
            var items = (IList)value;
            if (items.Count == 0)
            {
                return Null.Value;
            }

            var list = new List<IValue>(items.Count);
            var typeName = GetTypeName(elementType);
            var version = GetVersion(elementType);

            foreach (var item in items)
            {
                list.Add(SerializeValue(item, elementType));
            }

            var data = new SerializationData
            {
                Header = new SerializationHeader
                {
                    TypeValue = ImmutableArrayValue,
                    TypeName = typeName,
                    Version = version,
                },
                Value = new List(list),
            };

            return data.Bencoded;
        }
        else if (propertyType.IsDefined(typeof(ModelAttribute)))
        {
            return Serialize(value);
        }

        throw new ArgumentException($"Unsupported type {value.GetType()}");
    }

    private static object? DeserializeValue(Type propertyType, IValue propertyValue)
    {
        if (IsBencodableType(propertyType))
        {
            var instance = Activator.CreateInstance(propertyType, args: [propertyValue]);
            if (instance is not IBencodable bencodable)
            {
                throw new InvalidOperationException(
                    $"Failed to create an instance of {propertyType}");
            }

            return bencodable;
        }
        else if (IsBencodexType(propertyType))
        {
            if (propertyValue.GetType() == propertyType)
            {
                return propertyValue;
            }
        }
        else if (propertyType.IsEnum)
        {
            if (propertyValue is Integer integer)
            {
                var underlyingType = Enum.GetUnderlyingType(propertyType);
                if (underlyingType == typeof(long))
                {
                    return Enum.ToObject(propertyType, (long)integer.Value);
                }
                else if (underlyingType == typeof(int))
                {
                    return Enum.ToObject(propertyType, (int)integer.Value);
                }
            }
        }
        else if (propertyType == typeof(int))
        {
            if (propertyValue is Integer integer)
            {
                return (int)integer.Value;
            }
        }
        else if (propertyType == typeof(long))
        {
            if (propertyValue is Integer integer)
            {
                return (long)integer.Value;
            }
        }
        else if (propertyType == typeof(BigInteger))
        {
            if (propertyValue is Integer integer)
            {
                return integer.Value;
            }
        }
        else if (propertyType == typeof(string))
        {
            if (propertyValue is Text text)
            {
                return text.Value;
            }
            else if (propertyValue is Null)
            {
                return null;
            }
        }
        else if (propertyType == typeof(bool))
        {
            if (propertyValue is Bencodex.Types.Boolean boolean)
            {
                return boolean.Value;
            }
        }
        else if (propertyType == typeof(byte[]))
        {
            if (propertyValue is Binary binary)
            {
                return binary.ToByteArray();
            }
            else if (propertyValue is Null)
            {
                return null;
            }
        }
        else if (propertyType == typeof(DateTimeOffset))
        {
            if (propertyValue is Integer integer)
            {
                return new DateTimeOffset((long)integer.Value, TimeSpan.Zero);
            }
        }
        else if (propertyType == typeof(TimeSpan))
        {
            if (propertyValue is Integer integer)
            {
                return new TimeSpan((long)integer.Value);
            }
        }
        else if (propertyType.IsDefined(typeof(ModelAttribute)))
        {
            if (propertyValue is Null)
            {
                return null;
            }

            return Deserialize(propertyValue, propertyType);
        }
        else if (IsArray(propertyType, out var elementType))
        {
            if (propertyValue is Null)
            {
                return ToEmptyArray(elementType);
            }

            if (SerializationData.TryGetObject(propertyValue, out var data))
            {
                var header = data.Header;
                if (header.TypeValue != ArrayValue)
                {
                    throw new ArgumentException(
                        $"Given magic value {header.TypeValue} is not {ArrayValue}");
                }

                var typeName = GetTypeName(elementType);
                if (header.TypeName != typeName)
                {
                    throw new ArgumentException(
                        $"Given type name {header.TypeName} is not {typeName}");
                }

                var list = (List)data.Value;
                return ToArray(list, elementType);
            }
        }
        else if (IsImmutableArray(propertyType, out elementType))
        {
            if (propertyValue is Null)
            {
                return ToImmutableEmptyArray(elementType);
            }

            if (SerializationData.TryGetObject(propertyValue, out var data))
            {
                var header = data.Header;
                if (header.TypeValue != ImmutableArrayValue)
                {
                    throw new ArgumentException(
                        $"Given magic value {header.TypeValue} is not {ArrayValue}");
                }

                var typeName = GetTypeName(elementType);
                if (header.TypeName != typeName)
                {
                    throw new ArgumentException(
                        $"Given type name {header.TypeName} is not {typeName}");
                }

                var list = (List)data.Value;
                return ToImmutableArray(list, elementType);
            }
        }
        else if (propertyValue is Null)
        {
            return null;
        }
        else
        {
            var message = $"Unsupported type {propertyType}. Cannot convert value of type " +
                          $"{propertyValue.GetType()} to {propertyType}";
            throw new ArgumentException(message, nameof(propertyValue));
        }

        throw new ArgumentException(
            message: $"Unsupported type {propertyType}. Cannot convert value of type " +
                     $"{propertyValue.GetType()} to {propertyType}",
            paramName: nameof(propertyValue));
    }

    private static Array ToArray(List list, Type elementType)
    {
        var array = Array.CreateInstance(elementType, list.Count);
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            var itemValue = item is null ? null : DeserializeValue(elementType, item);
            array.SetValue(itemValue, i);
        }

        return array;
    }

    private static object ToImmutableArray(List list, Type elementType)
    {
        var listType = typeof(List<>).MakeGenericType(elementType);
        var listInstance = (IList)Activator.CreateInstance(listType)!;
        foreach (var item in list)
        {
            var itemValue = item is null ? null : DeserializeValue(elementType, item);
            listInstance.Add(itemValue);
        }

        var methodName = nameof(ImmutableArray.CreateRange);
        var methodInfo = GetCreateRangeMethod(
            typeof(ImmutableArray), methodName, typeof(IEnumerable<>));
        var genericMethodInfo = methodInfo.MakeGenericMethod(elementType);
        var methodArgs = new object?[] { listInstance };
        return genericMethodInfo.Invoke(null, parameters: methodArgs)!;
    }

    private static MethodInfo GetCreateRangeMethod(Type type, string methodName, Type parameterType)
    {
        var parameterName = parameterType.Name;
        var bindingFlags = BindingFlags.Public | BindingFlags.Static;
        var methodInfos = type.GetMethods(bindingFlags);

        for (var i = 0; i < methodInfos.Length; i++)
        {
            var methodInfo = methodInfos[i];
            var parameters = methodInfo.GetParameters();
            if (methodInfo.Name == methodName &&
                parameters.Length == 1 &&
                parameters[0].ParameterType.Name == parameterName)
            {
                return methodInfo;
            }
        }

        throw new NotSupportedException("The method is not found.");
    }

    private static IEnumerable<Type> GetSerializableTypes(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(IsDefined);
        foreach (var type in types)
        {
            yield return type;
        }

        static bool IsDefined(Type type) => type.IsDefined(typeof(ModelAttribute));
    }

    private static string GetTypeName(Type type)
    {
        var typeName = type.FullName
             ?? throw new UnreachableException("Type does not have FullName");
        var assemblyName = type.Assembly.GetName().Name
             ?? throw new UnreachableException("Assembly does not have Name");
        return $"{typeName}, {assemblyName}";
    }

    private static ImmutableArray<PropertyInfo> CreateProperties(Type type)
    {
        var query = from propertyInfo in type.GetProperties()
                    let propertyAttribute
                        = propertyInfo.GetCustomAttribute<PropertyAttribute>()
                    where propertyAttribute is not null
                    orderby propertyAttribute.Index
                    select (propertyInfo, propertyAttribute);
        var items = query.ToArray();
        var builder = ImmutableArray.CreateBuilder<PropertyInfo>(items.Length);
        foreach (var (propertyInfo, propertyAttribute) in items)
        {
            var index = propertyAttribute.Index;
            if (index != builder.Count)
            {
                throw new NotSupportedException(
                    $"Property {propertyInfo.Name} has an invalid index {index}");
            }

            ValidatorProperty(type, propertyInfo);

            builder.Add(propertyInfo);
        }

        return builder.ToImmutable();
    }

    private static void ValidatorProperty(Type type, PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;
        if (typeof(IList).IsAssignableFrom(propertyType))
        {
            ValidateArrayProperty(type, propertyType);
        }
    }

#pragma warning disable MEN009 // Use the preferred exception type
    private static void ValidateArrayProperty(Type type, Type propertyType)
    {
        if (!IsSupportedArrayType(propertyType))
        {
            throw new NotSupportedException($"Type {propertyType} is not supported.");
        }

        var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var equatableType = typeof(IEquatable<>).MakeGenericType(type);
        if (!equatableType.IsAssignableFrom(type))
        {
            throw new NotSupportedException(
                $"Type {type} does not implement {equatableType}. " +
                "Please implement IEquatable<T> and override GetHashCode and Equals methods.");
        }

        var isRecord = type.GetMethod("<Clone>$") != null;
        var methodParams1 = new[] { type };
        var methodName1 = nameof(IEquatable<object>.Equals);
        var methodInfo1 = type.GetMethod(methodName1, bindingFlags, types: methodParams1);
        if (methodInfo1 is null)
        {
            throw new NotImplementedException(
                $"Method {nameof(IEquatable<object>.Equals)} is not implemented in {type}. " +
                "Please implement IEquatable<T> Equals method.");
        }
        else if (methodInfo1.IsDefined(typeof(CompilerGeneratedAttribute)))
        {
            throw new NotImplementedException(
                $"Method {nameof(IEquatable<object>.Equals)} is not implemented in {type}. " +
                "Please implement IEquatable<T> Equals method.");
        }

        var methodName2 = nameof(GetHashCode);
        var methodInfo2 = type.GetMethod(methodName2, bindingFlags);
        if (methodInfo2 is null)
        {
            throw new NotImplementedException(
                $"Method {nameof(GetHashCode)} is not implemented in {type}. " +
                "Please override GetHashCode method.");
        }
        else if (methodInfo2.IsDefined(typeof(CompilerGeneratedAttribute)))
        {
            throw new NotImplementedException(
                $"Method {nameof(GetHashCode)} is not implemented in {type}. " +
                "Please override GetHashCode method.");
        }

        if (!isRecord)
        {
            var methodParams3 = new[] { typeof(object) };
            var methodName3 = nameof(object.Equals);
            var methodInfo3 = type.GetMethod(methodName3, bindingFlags, types: methodParams3);
            if (methodInfo3 is null)
            {
                throw new NotImplementedException(
                    $"Method {nameof(object.Equals)} is not implemented in {type}. " +
                    "Please override Equals method.");
            }
        }
    }
#pragma warning restore MEN009 // Use the preferred exception type

    private static bool IsBencodableType(Type type)
        => typeof(IBencodable).IsAssignableFrom(type) && !type.IsInterface;

    private static bool IsBencodexType(Type type)
        => typeof(IValue).IsAssignableFrom(type) && !type.IsInterface;
}
