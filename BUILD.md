# Build and Development Guide

## Prerequisites

- Windows 10 or later
- .NET 9.0 SDK
- Visual Studio 2022 (recommended) or Visual Studio Code with C# extension
- OpenAI API Key

## Development Environment Setup

### Option 1: Visual Studio 2022 (Recommended)

1. Install Visual Studio 2022 with the following workloads:
   - .NET desktop development
   - Windows application development

2. Clone the repository:
```bash
git clone https://github.com/shinepcs/SpellingChecker.git
```

3. Open `SpellingChecker.sln` in Visual Studio

4. Restore NuGet packages (automatic in VS2022)

5. Build the solution: `Ctrl+Shift+B`

6. Run: `F5`

### Option 2: Command Line

1. Install .NET 9.0 SDK from https://dotnet.microsoft.com/download

2. Clone the repository:
```bash
git clone https://github.com/shinepcs/SpellingChecker.git
cd SpellingChecker
```

3. Restore dependencies:
```bash
dotnet restore
```

4. Build:
```bash
dotnet build
```

5. Run:
```bash
dotnet run --project SpellingChecker/SpellingChecker.csproj
```

## Building for Release

### Create Publish Build

```bash
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

The executable will be created in:
```
SpellingChecker/bin/Release/net9.0-windows/win-x64/publish/SpellingChecker.exe
```

### Build Configurations

- **Debug**: For development, includes debugging symbols
- **Release**: Optimized for deployment

## Creating Distribution Files

### Option 1: Portable ZIP Distribution (Recommended)

**Perfect for users who prefer portable applications**.

1. **Build using PowerShell script**:
   ```powershell
   .\build-portable.ps1
   ```

2. **Output**: `dist\SpellingChecker-v1.0.0-portable-win-x64.zip`

**Benefits of portable version**:
- No installation required
- Can run from USB drive
- No registry changes
- Easy to delete (just remove folder)
- Includes all documentation

Users extract the ZIP and run `SpellingChecker.exe` directly.

### Option 2: Standalone Executable (Simplest)

Just distribute the published executable:

```bash
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

The executable is located at:
```
SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
```

Users can run this file directly without any installation.

## Testing

### Manual Testing Checklist

1. **Installation**
   - [ ] Application installs correctly
   - [ ] System tray icon appears
   - [ ] Settings can be opened

2. **Settings**
   - [ ] API key can be saved
   - [ ] Settings are encrypted and persisted
   - [ ] Settings load correctly on restart

3. **Spelling Correction (Ctrl+Shift+Alt+Y)**
   - [ ] Hotkey works globally
   - [ ] Selected text is captured
   - [ ] AI corrects spelling errors
   - [ ] Results popup appears near cursor
   - [ ] Copy to clipboard works
   - [ ] Replace works

4. **Translation (Ctrl+Shift+Alt+T)**
   - [ ] Hotkey works globally
   - [ ] Language detection works (Korean/English)
   - [ ] Translation is accurate
   - [ ] Results popup appears
   - [ ] Copy and replace work

5. **System Integration**
   - [ ] Runs in background
   - [ ] System tray icon functions
   - [ ] Can exit from tray menu
   - [ ] Auto-start with Windows (if enabled)

### Automated Testing

Currently, the project focuses on core functionality. Future improvements will include:
- Unit tests for services
- Integration tests for API communication
- UI automation tests

## Debugging Tips

### Common Issues

1. **Hotkeys not working**
   - Check if another application is using the same hotkeys
   - Run as administrator
   - Check Windows event log

2. **API errors**
   - Verify API key is correct
   - Check internet connection
   - Verify API endpoint URL
   - Check OpenAI API status

3. **Text selection not working**
   - Some applications may block clipboard access
   - Try running as administrator
   - Check Windows security settings

### Logging

To enable detailed logging, you can modify the services to add log output to:
```
%APPDATA%\SpellingChecker\logs\
```

## Performance Optimization

1. **API Response Time**
   - Use gpt-4o-mini for faster responses
   - Consider implementing request caching
   - Add timeout handling

2. **Memory Usage**
   - Monitor for memory leaks in long-running sessions
   - Dispose resources properly

3. **Startup Time**
   - Keep initialization minimal
   - Load settings asynchronously

## Security Considerations

1. **API Key Storage**
   - Uses Windows DPAPI for encryption
   - Keys are user-specific
   - Not accessible by other users

2. **Data Privacy**
   - Text is sent to OpenAI API
   - No data is stored locally (except settings)
   - Review OpenAI's privacy policy

3. **Code Signing**
   - For production releases, sign the executable
   - Prevents Windows SmartScreen warnings
   - Use a code signing certificate

## Deployment Checklist

- [ ] Build in Release configuration
- [ ] Test all features
- [ ] Create installer
- [ ] Sign executable (if available)
- [ ] Test installer on clean Windows installation
- [ ] Create release notes
- [ ] Tag release in Git
- [ ] Upload to distribution platform

## Continuous Integration

For automated builds, you can use GitHub Actions. Example workflow:

```yaml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '9.0.x'
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore --configuration Release
    - name: Publish
      run: dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## Support

For issues and questions:
- GitHub Issues: https://github.com/shinepcs/SpellingChecker/issues
- Email: [Your support email]
