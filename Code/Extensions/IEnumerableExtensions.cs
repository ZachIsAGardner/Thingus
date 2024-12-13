namespace Thingus;

public static class IEnumerableExtensions
{
    public static T Random<T>(this IEnumerable<T> list)
    {
        if (list == null || list.Count() <= 0)
        {
            return default(T);
        }

        int index = Chance.Range(0, list.Count());
        return list.ToList()[index];
    }
}