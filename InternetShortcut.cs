using System.Text;

namespace UrlShortcut;

public static class InternetShortcut
{
    public static Uri ParseWebUrl(string? input)
    {
        string value = input?.Trim() ?? string.Empty;
        if (value.Length == 0 || value.Any(char.IsControl) ||
            !Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new ArgumentException(Strings.InvalidUrl, nameof(input));
        }

        return uri;
    }

    public static void Create(string path, Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!path.EndsWith(".url", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(Strings.InvalidExtension, nameof(path));
        }

        string content = "[InternetShortcut]\r\nURL=" + url.AbsoluteUri + "\r\n";
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }
}
