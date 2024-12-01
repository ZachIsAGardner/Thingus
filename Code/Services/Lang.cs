namespace Thingus;

public static class Lang
{
    static LangImport import;

    public static void Start()
    {
        import = Library.LangImports["English"];
    } 

    public static string Key(string key)
    {
        return import.Keys[key];
    }
}