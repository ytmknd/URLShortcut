using System.Globalization;
using System.Resources;

namespace UrlShortcut;

internal static class Strings
{
    private static readonly ResourceManager Resources = new("UrlShortcut.Resources", typeof(Strings).Assembly);

    private static string Get(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture)
        ?? throw new MissingManifestResourceException($"Missing text resource: {key}");

    public static string WindowTitle => Get(nameof(WindowTitle));
    public static string UrlPrompt => Get(nameof(UrlPrompt));
    public static string SaveButton => Get(nameof(SaveButton));
    public static string InvalidUrl => Get(nameof(InvalidUrl));
    public static string InvalidUrlTitle => Get(nameof(InvalidUrlTitle));
    public static string SaveDialogTitle => Get(nameof(SaveDialogTitle));
    public static string SaveDialogFilter => Get(nameof(SaveDialogFilter));
    public static string SaveSuccess => Get(nameof(SaveSuccess));
    public static string SuccessTitle => Get(nameof(SuccessTitle));
    public static string SaveFailureFormat => Get(nameof(SaveFailureFormat));
    public static string ErrorTitle => Get(nameof(ErrorTitle));
    public static string InvalidExtension => Get(nameof(InvalidExtension));
}
