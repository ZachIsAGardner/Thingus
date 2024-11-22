namespace Thingus;

public static class ListExtensions
{

    public static T Next<T>(this List<T> list, T current) 
    {
        if (list == null || list.Count <= 0 || !list.Contains(current)) return default(T);
        int index = list.IndexOf(current);
        index++;
        if (index >= list.Count) index = 0;
        return list[index];
    }
    public static string Join(this List<string> list, string seperator)
    {
        return String.Join(seperator, list);
    }

    public static string Join(this IEnumerable<string> list, string seperator)
    {
        return String.Join(seperator, list);
    }

    public static T Random<T>(this List<T> list)
    {
        if (list == null || list.Count <= 0)
        {
            return default(T);
        }

        int index = Chance.Range(0, list.Count);
        return list[index];
    }

    public static List<T> Clone<T>(this List<T> list)
    {
        if (list == null) return null;
        List<T> result = new List<T>() { };
        foreach (T item in list) result.Add(item);
        return result;
    }

    public static List<T> Reversed<T>(this List<T> list)
    {
        List<T> result = list.Clone();
        result.Reverse();
        return result;
    }

    public static bool None<T>(this List<T> list, Func<T, bool> predicate)
    {
        return !list.Any(predicate);
    }

    public static bool None<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        return !list.Any(predicate);
    }
}
