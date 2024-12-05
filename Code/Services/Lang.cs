namespace Thingus;

public static class Lang
{
    static LangImport import;

    public static void Start()
    {
        import = Library.LangImports.Get("English");
    } 

    public static string Key(string key)
    {
        return import.Keys[key];
    }
}