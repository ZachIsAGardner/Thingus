namespace Thingus;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> set, List<T> items)
    {
        foreach (T item in items)
        {
            set.Add(item);
        }
    }
}
