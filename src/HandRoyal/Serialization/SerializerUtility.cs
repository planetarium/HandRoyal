using System.Collections;

namespace HandRoyal.Serialization;

public static class SerializerUtility
{
    public static int GetHashCode(object obj)
    {
        var properties = Serializer.GetProperties(obj.GetType());
        HashCode hash = default;
        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            hash.Add(value);
        }

        return hash.ToHashCode();
    }

    public static bool Equals<T>(T left, T? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (right is null)
        {
            return false;
        }

        var properties = Serializer.GetProperties(typeof(T));
        foreach (var property in properties)
        {
            var leftValue = property.GetValue(left);
            var rightValue = property.GetValue(right);
            if (leftValue is IList leftList
                && rightValue is IList rightList
                && typeof(IList).IsAssignableFrom(property.PropertyType))
            {
                if (leftList.Count != rightList.Count)
                {
                    return false;
                }

                for (var i = 0; i < leftList.Count; i++)
                {
                    if (!object.Equals(leftList[i], rightList[i]))
                    {
                        return false;
                    }
                }
            }
            else if (!object.Equals(leftValue, rightValue))
            {
                return false;
            }
        }

        return true;
    }
}
