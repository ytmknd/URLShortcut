# URL Shortcut Creator

A C# Windows Forms app for Windows 11 that creates a `.url` shortcut to a website.
The app uses Japanese text when the Windows display language is Japanese and English text for other display languages.

## Run

With the .NET 10 SDK installed on Windows, run this command from the project directory:

```powershell
dotnet run --project .\UrlShortcut.csproj
```

Enter an address beginning with `https://` or `http://`, click the Save button, and choose where to save the shortcut. Double-click the resulting `.url` file to open the address in your default browser.

To build a standalone executable for distribution:

```powershell
dotnet publish .\UrlShortcut.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable is written to `bin\Release\net10.0-windows\win-x64\publish\`. Close any running copy of the app before publishing again.

## Verify

```powershell
dotnet run --project .\tests\UrlShortcut.Smoke.csproj -c Release
```

This test checks both UI languages, creates a `.url` file in a temporary directory, and confirms that Windows reads its target address correctly.
