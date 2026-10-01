using System.Drawing;

namespace UrlShortcut;

public sealed class MainForm : Form
{
    private readonly TextBox urlBox = new()
    {
        Dock = DockStyle.Fill,
        PlaceholderText = "https://example.com/",
        AccessibleName = "URL"
    };

    public MainForm()
    {
        using (Stream iconStream = typeof(MainForm).Assembly.GetManifestResourceStream("UrlShortcut.assets.UrlShortcut.ico")!)
        {
            Icon = new Icon(iconStream);
        }

        Text = Strings.WindowTitle;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(480, 220);
        Size = new Size(620, 220);
        Font = new Font("Yu Gothic UI", 10);
        AutoScaleMode = AutoScaleMode.Dpi;

        var saveButton = new Button
        {
            Text = Strings.SaveButton,
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            Margin = new Padding(0, 12, 0, 0)
        };
        saveButton.Click += SaveButton_Click;
        AcceptButton = saveButton;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(18),
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label
        {
            Text = Strings.UrlPrompt,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        }, 0, 0);
        layout.Controls.Add(urlBox, 0, 1);

        layout.Controls.Add(saveButton, 0, 2);

        Controls.Add(layout);
        Shown += (_, _) => urlBox.Focus();
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        Uri url;
        try
        {
            url = InternetShortcut.ParseWebUrl(urlBox.Text);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(this, ex.Message, Strings.InvalidUrlTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            urlBox.Focus();
            urlBox.SelectAll();
            return;
        }

        string suggestedName = string.Concat(url.IdnHost.Select(c =>
            Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));

        using var dialog = new SaveFileDialog
        {
            Title = Strings.SaveDialogTitle,
            Filter = Strings.SaveDialogFilter,
            DefaultExt = "url",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = suggestedName + ".url"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            InternetShortcut.Create(dialog.FileName, url);
            MessageBox.Show(this, Strings.SaveSuccess, Strings.SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            MessageBox.Show(this, string.Format(Strings.SaveFailureFormat, ex.Message), Strings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
