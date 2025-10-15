# Deployment Guide

This document describes how to create and publish releases of SpellingChecker.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Building a Release Locally](#building-a-release-locally)
- [Creating a Release on GitHub](#creating-a-release-on-github)
- [Manual Deployment Steps](#manual-deployment-steps)
- [Troubleshooting](#troubleshooting)

## Prerequisites

### For Local Builds

1. **Windows 10 or later**
2. **.NET 9.0 SDK** - Download from https://dotnet.microsoft.com/download
3. **Inno Setup 6.x or later** (optional, for creating installer)
   - Download from https://jrsoftware.org/isinfo.php
   - Install to default location: `C:\Program Files (x86)\Inno Setup 6\`

### For GitHub Actions Builds

- Repository maintainer access
- Proper GitHub permissions to create releases

## Building a Release Locally

### Using the Build Scripts

We provide two scripts for building releases:

#### PowerShell Script (Recommended)

```powershell
.\build-release.ps1 -Version "1.0.0"
```

#### Batch Script

```cmd
build-release.bat 1.0.0
```

Both scripts will:
1. Clean previous builds
2. Restore dependencies
3. Build the solution in Release configuration
4. Publish a self-contained executable
5. Create an installer (if Inno Setup is installed)

### Manual Build Steps

If you prefer to build manually:

#### 1. Restore Dependencies

```bash
dotnet restore SpellingChecker.sln
```

#### 2. Build in Release Mode

```bash
dotnet build SpellingChecker.sln --configuration Release --no-restore
```

#### 3. Publish Self-Contained Executable

```bash
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

The executable will be created at:
```
SpellingChecker/bin/Release/net9.0-windows/win-x64/publish/SpellingChecker.exe
```

#### 4. Create Installer (Optional)

If you have Inno Setup installed:

```powershell
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer.iss
```

The installer will be created at:
```
installer/SpellingCheckerSetup_v1.0.0.exe
```

## Creating a Release on GitHub

### Automated Release (Recommended)

The repository includes a GitHub Actions workflow that automatically builds and creates releases.

#### Method 1: Create a Git Tag

1. Tag your commit with a version number:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The workflow will automatically:
   - Build the application
   - Create the installer
   - Create a GitHub Release
   - Upload all artifacts

#### Method 2: Manual Workflow Dispatch

1. Go to the repository on GitHub
2. Click on "Actions" tab
3. Select "Release" workflow
4. Click "Run workflow"
5. Enter the version number (e.g., 1.0.0)
6. Click "Run workflow"

The workflow will create a release with:
- Windows Installer (`.exe`)
- Standalone executable
- README documentation

### Release Artifacts

Each release includes:

1. **SpellingCheckerSetup_vX.X.X.exe** - Windows installer
   - Includes all dependencies
   - Creates Start Menu shortcuts
   - Optional desktop icon
   - Optional Windows startup
   - Approximately 60-80 MB

2. **SpellingChecker.exe** - Portable executable
   - Self-contained, no installation required
   - Can run from any folder
   - Approximately 60-80 MB

3. **README.md** - Documentation

## Manual Deployment Steps

If you need to deploy manually without GitHub Actions:

### 1. Build the Release Locally

Follow the steps in [Building a Release Locally](#building-a-release-locally).

### 2. Create a GitHub Release

1. Go to your repository on GitHub
2. Click on "Releases" on the right sidebar
3. Click "Draft a new release"
4. Fill in the release information:
   - **Tag version**: v1.0.0
   - **Release title**: Release v1.0.0
   - **Description**: See [Release Notes Template](#release-notes-template)
5. Upload the following files:
   - `installer/SpellingCheckerSetup_v1.0.0.exe`
   - `SpellingChecker/bin/Release/net9.0-windows/win-x64/publish/SpellingChecker.exe`
   - `README.md`
6. Click "Publish release"

### Release Notes Template

```markdown
## AI Spelling Checker - Release vX.X.X

### 📦 Installation

Download the installer below and run it:
- `SpellingCheckerSetup_vX.X.X.exe` - Windows Installer

Or download the standalone executable:
- `SpellingChecker.exe` - Portable version (no installation required)

### 🚀 Quick Start

1. Run the installer or executable
2. Get an API key from [OpenAI](https://platform.openai.com/api-keys), [Anthropic](https://console.anthropic.com/), or [Google Gemini](https://makersuite.google.com/app/apikey)
3. Right-click the system tray icon and select "Settings"
4. Enter your API key and select your preferred model
5. Use Ctrl+Shift+C to correct text or Ctrl+Shift+T to translate

### 📖 Documentation

- [Quick Start Guide](https://github.com/trollgameskr/SpellingChecker/blob/main/QUICKSTART.md)
- [Configuration Guide](https://github.com/trollgameskr/SpellingChecker/blob/main/CONFIG.md)
- [Full Documentation](https://github.com/trollgameskr/SpellingChecker/blob/main/README.md)

### ✨ What's New

- [List new features and improvements]

### 🐛 Bug Fixes

- [List bug fixes]

### 🐛 Known Issues

- [List any known issues]

Please report any issues on our [GitHub Issues](https://github.com/trollgameskr/SpellingChecker/issues) page.
```

## Version Numbering

We follow [Semantic Versioning](https://semver.org/):

- **MAJOR.MINOR.PATCH** (e.g., 1.2.3)
  - **MAJOR**: Incompatible API changes or major features
  - **MINOR**: New features, backward compatible
  - **PATCH**: Bug fixes, backward compatible

Examples:
- `1.0.0` - Initial release
- `1.1.0` - Added new features
- `1.1.1` - Bug fixes
- `2.0.0` - Breaking changes

## Testing Before Release

Before creating a release, ensure:

### ✅ Pre-Release Checklist

- [ ] Code builds without errors
- [ ] All features work as expected
- [ ] Settings can be saved and loaded
- [ ] Hotkeys work correctly
- [ ] API integration works (test with real API keys)
- [ ] System tray icon and menu work
- [ ] Installer creates proper shortcuts
- [ ] Installer uninstalls cleanly
- [ ] Test on clean Windows installation
- [ ] Update version numbers in:
  - [ ] `SpellingChecker/SpellingChecker.csproj`
  - [ ] `installer.iss`
  - [ ] Release notes
- [ ] Update CHANGELOG.md
- [ ] Update README.md if needed

### Testing the Installer

1. **Install Test**
   - Run the installer on a clean Windows machine
   - Verify shortcuts are created
   - Test "Start with Windows" option
   - Verify the application launches

2. **Functionality Test**
   - Configure API key
   - Test correction hotkey (Ctrl+Shift+C)
   - Test translation hotkey (Ctrl+Shift+T)
   - Test different AI models
   - Test copy functionality
   - Test replace functionality

3. **Uninstall Test**
   - Uninstall the application
   - Verify all files are removed
   - Verify shortcuts are removed
   - Check that settings are cleaned up

## Troubleshooting

### Build Fails

**Problem**: `dotnet build` fails with errors

**Solutions**:
1. Ensure .NET 9.0 SDK is installed: `dotnet --version`
2. Clean and rebuild: `dotnet clean && dotnet build`
3. Restore packages: `dotnet restore`
4. Check for syntax errors in code

### Publish Fails

**Problem**: `dotnet publish` fails

**Solutions**:
1. Ensure you have enough disk space (need ~500MB)
2. Close any running instances of SpellingChecker.exe
3. Try publishing to a different folder
4. Check for locked files

### Inno Setup Not Found

**Problem**: Script can't find Inno Setup

**Solutions**:
1. Install Inno Setup from https://jrsoftware.org/isinfo.php
2. Verify installation path: `C:\Program Files (x86)\Inno Setup 6\`
3. Manually compile: Open `installer.iss` in Inno Setup Compiler

### Installer Build Fails

**Problem**: Inno Setup compilation fails

**Solutions**:
1. Ensure the published executable exists:
   ```
   SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
   ```
2. Check that required files exist:
   - `README.md`
   - `LICENSE`
   - `CONFIG.md`
   - `SpellingChecker\app.ico`
3. Review Inno Setup compiler output for specific errors

### GitHub Actions Workflow Fails

**Problem**: Release workflow fails on GitHub

**Solutions**:
1. Check the Actions log for specific errors
2. Verify the tag format is correct (e.g., `v1.0.0`)
3. Ensure you have proper repository permissions
4. Check that all referenced files exist in the repository

### Large File Size

**Problem**: Executable is very large (>100MB)

**Explanation**: This is expected for self-contained .NET applications
- Includes .NET runtime (~60MB)
- Includes all dependencies
- Self-contained = no .NET installation required

**To reduce size** (not recommended for beginners):
- Use framework-dependent deployment (requires .NET installed on user's machine)
- Use ReadyToRun compilation
- Use assembly trimming (may break some features)

## Distribution

Once you have created a release:

### GitHub Releases (Primary)

Users can download from:
```
https://github.com/trollgameskr/SpellingChecker/releases
```

### Alternative Distribution

If needed, you can also distribute via:
- Company file server
- Cloud storage (OneDrive, Google Drive, etc.)
- Installer hosting services

**Important**: Always provide checksums (SHA256) when distributing outside GitHub.

## Code Signing (Optional)

For production releases, consider code signing:

### Why Sign?

- Prevents Windows SmartScreen warnings
- Increases user trust
- Verifies publisher identity

### How to Sign

1. Obtain a code signing certificate from a trusted CA
2. Sign the executable:
   ```powershell
   signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com SpellingChecker.exe
   ```
3. Sign the installer:
   ```powershell
   signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com SpellingCheckerSetup_v1.0.0.exe
   ```

**Note**: Code signing certificates typically cost $100-300/year.

## Support

For deployment questions:
- Check this guide first
- Review [BUILD.md](BUILD.md) for build details
- Open an issue on GitHub: https://github.com/trollgameskr/SpellingChecker/issues

---

**Last Updated**: 2025-10-15
