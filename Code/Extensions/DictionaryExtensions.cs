namespace Thingus;

public static class DictionaryExtensions
{
    public static T2 Get<T, T2>(this Dictionary<T, T2> dict, T key)
    {
        if (key == null || !dict.ContainsKey(key)) return default;
        else return dict[key];
    }
}
