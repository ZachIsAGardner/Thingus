namespace Thingus;

public static class Paths
{
    /// <summary>The folder path to store stuff like save data.</summary>
    private static readonly string UserStorageRelativePath = $"zagawee/{CONSTANTS.SAVE_FOLDER}";

    /// <summary>Path to use when storing stuff like save data.</summary>
    public static string UserStorageBasePath()
    {
        string result = "";

        if (Game.Platform == PlatformType.Windows)
        {
            result = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/{UserStorageRelativePath}";
        }
        else if (Game.Platform == PlatformType.Linux)
        {
            result = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/{UserStorageRelativePath}";
        }
        else if (Game.Platform == PlatformType.Mac)
        {
            result = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Application Support/{UserStorageRelativePath}";
        }
        else
        {
            result = "";
        }

        return result.Replace("\\", "/");
    }
}