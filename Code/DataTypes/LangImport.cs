namespace Thingus;

public class LangImport
{
    public Dictionary<string, string> Keys = new Dictionary<string, string>() { };

    public LangImport(string file)
    {
        string[] lines = File.ReadAllLines(file);
        foreach (string line in lines)
        {
            string[] words = line.Split("=");
            string key = words[0].Trim();
            string content = words[1].Trim();
            Keys[key] = content;
        }
    }
}