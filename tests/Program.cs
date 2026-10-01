using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using UrlShortcut;

string path = Path.Combine(Path.GetTempPath(), $"urlshortcut-{Guid.NewGuid():N}.url");

try
{
    CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
    try
    {
        var resources = new ResourceManager("UrlShortcut.Resources", typeof(MainForm).Assembly);
        foreach (var (cultureName, expectedTitle, expectedButton, expectedError) in new[]
        {
            ("ja-JP", "URL ショートカット作成", "保存...", "有効な URL"),
            ("en-US", "URL Shortcut Creator", "Save...", "valid URL"),
            ("fr-FR", "URL Shortcut Creator", "Save...", "valid URL")
        })
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            foreach (string key in new[] { "WindowTitle", "UrlPrompt", "SaveButton", "InvalidUrl", "InvalidUrlTitle",
                "SaveDialogTitle", "SaveDialogFilter", "SaveSuccess", "SuccessTitle", "SaveFailureFormat", "ErrorTitle", "InvalidExtension" })
            {
                if (string.IsNullOrWhiteSpace(resources.GetString(key, CultureInfo.CurrentUICulture)))
                {
                    throw new Exception($"Missing {cultureName} text: {key}");
                }
            }

            using var form = new MainForm();
            var layout = form.Controls.OfType<TableLayoutPanel>().Single();
            var button = layout.Controls.OfType<Button>().Single();
            if (form.Text != expectedTitle || button.Text != expectedButton)
            {
                throw new Exception($"Wrong UI language: {cultureName}");
            }

            foreach (Size size in new[] { form.Size, form.MinimumSize })
            {
                form.Size = size;
                form.CreateControl();
                form.PerformLayout();
                layout.PerformLayout();
                if (button.Bottom > layout.ClientSize.Height - layout.Padding.Bottom)
                {
                    throw new Exception($"Save button is clipped: {cultureName}");
                }
            }

            try
            {
                InternetShortcut.ParseWebUrl("invalid");
                throw new Exception("Invalid URL was accepted.");
            }
            catch (ArgumentException ex)
            {
                if (!ex.Message.Contains(expectedError, StringComparison.Ordinal))
                {
                    throw new Exception($"Wrong error language: {cultureName}: {ex.Message}");
                }
            }
        }
    }
    finally
    {
        CultureInfo.CurrentUICulture = originalUiCulture;
    }

    using (Stream iconStream = typeof(InternetShortcut).Assembly.GetManifestResourceStream("UrlShortcut.assets.UrlShortcut.ico")
        ?? throw new Exception("アイコンが埋め込まれていません。"))
    using (var icon = new Icon(iconStream))
    {
        if (icon.Width == 0 || icon.Height == 0)
        {
            throw new Exception("アイコンを読み取れません。");
        }
    }

    Uri url = InternetShortcut.ParseWebUrl(" https://example.com/日本語?q=1#section ");
    InternetShortcut.Create(path, url);

    string file = File.ReadAllText(path, Encoding.UTF8);
    if (!file.StartsWith("[InternetShortcut]\r\nURL=https://example.com/", StringComparison.Ordinal) ||
        !file.Contains("%E6%97%A5%E6%9C%AC%E8%AA%9E?q=1#section", StringComparison.Ordinal) ||
        !file.EndsWith("\r\n", StringComparison.Ordinal))
    {
        throw new Exception(".url の内容が正しくありません。: " + file);
    }

    Type shellType = Type.GetTypeFromProgID("WScript.Shell")
        ?? throw new Exception("Windows Shell COM を利用できません。");
    dynamic shell = Activator.CreateInstance(shellType)!;
    dynamic shortcut = shell.CreateShortcut(path);
    string target = shortcut.TargetPath;
    if (!string.Equals(target, url.AbsoluteUri, StringComparison.Ordinal))
    {
        throw new Exception("Windows が読み取ったリンク先が異なります: " + target);
    }

    foreach (string invalid in new[] { "", "example.com", "file:///C:/Windows", "https://", "https://example.com/\r\nURL=https://other.example/" })
    {
        try
        {
            InternetShortcut.ParseWebUrl(invalid);
            throw new Exception("不正な URL を受け付けました: " + invalid);
        }
        catch (ArgumentException)
        {
        }
    }

    Console.WriteLine("Smoke tests passed.");
}
finally
{
    if (File.Exists(path))
    {
        File.Delete(path);
    }
}
