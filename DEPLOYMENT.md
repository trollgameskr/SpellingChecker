# Deployment Guide

This guide explains how to create distribution files for SpellingChecker using **free and open-source tools only**.

## Overview

SpellingChecker offers two deployment options:

1. **Portable ZIP** - No-installation-required package
2. **Standalone Executable** - Single EXE file

All options are built using free tools and don't require any paid software.

---

## Prerequisites

- Windows 10 or later
- .NET 9.0 SDK
- PowerShell 5.1 or later (included in Windows)

---

## Option 1: Portable ZIP (Recommended)

Creates a ZIP file that users can extract and run anywhere.

### Why Portable?

- ✅ No installation required
- ✅ Run from USB drive
- ✅ No registry changes
- ✅ Easy to delete (just remove folder)
- ✅ Perfect for testing or temporary use

### Build Steps

**Using the automated script (easiest)**:

```powershell
.\build-portable.ps1
```

Or using batch file:

```cmd
build-portable.bat
```

**Manual build**:

```powershell
# Step 1: Build the application
dotnet publish SpellingChecker/SpellingChecker.csproj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true

# Step 2: Create distribution folder
$distPath = "dist\SpellingChecker-portable"
New-Item -ItemType Directory -Force -Path $distPath | Out-Null

# Step 3: Copy files
Copy-Item "SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -Destination $distPath
Copy-Item "README.md" -Destination $distPath
Copy-Item "LICENSE" -Destination $distPath
Copy-Item "CONFIG.md" -Destination $distPath
Copy-Item "QUICKSTART.md" -Destination $distPath

# Step 4: Create portable README
$readme = @"
# AI Spelling Checker - Portable Version

## Quick Start
1. Run SpellingChecker.exe
2. Configure your OpenAI API key in Settings (system tray icon)
3. Use hotkeys:
   - Ctrl+Shift+Alt+Y: Spell check
   - Ctrl+Shift+Alt+T: Translate

See README.md for full documentation.
"@
$readme | Out-File -FilePath "$distPath\PORTABLE-README.txt" -Encoding UTF8

# Step 5: Create ZIP
Compress-Archive -Path $distPath -DestinationPath "dist\SpellingChecker-v1.0.0-portable-win-x64.zip" -CompressionLevel Optimal
```

### Output

The portable ZIP will be created at:
```
dist\SpellingChecker-v1.0.0-portable-win-x64.zip
```

Size: ~15-20 MB (self-contained)

### What Users Get

When users extract the ZIP:

```
SpellingChecker-portable/
├── SpellingChecker.exe    (Main application)
├── PORTABLE-README.txt    (Quick start guide)
├── README.md             (Full documentation)
├── CONFIG.md             (Configuration guide)
├── QUICKSTART.md         (Quick start guide)
└── LICENSE               (MIT License)
```

Users simply run `SpellingChecker.exe` - no installation needed!

---

## Option 2: Standalone Executable
- ✅ No registry changes
- ✅ Easy to delete (just remove folder)
- ✅ Perfect for testing or temporary use

### Build Steps

**Using the automated script (easiest)**:

```powershell
.\build-portable.ps1
```

**Manual build**:

```powershell
# Step 1: Build the application
dotnet publish SpellingChecker/SpellingChecker.csproj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true

# Step 2: Create distribution folder
$distPath = "dist\SpellingChecker-portable"
New-Item -ItemType Directory -Force -Path $distPath | Out-Null

# Step 3: Copy files
Copy-Item "SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -Destination $distPath
Copy-Item "README.md" -Destination $distPath
Copy-Item "LICENSE" -Destination $distPath
Copy-Item "CONFIG.md" -Destination $distPath
Copy-Item "QUICKSTART.md" -Destination $distPath

# Step 4: Create portable README
$readme = @"
# AI Spelling Checker - Portable Version

## Quick Start
1. Run SpellingChecker.exe
2. Configure your OpenAI API key in Settings (system tray icon)
3. Use hotkeys:
   - Ctrl+Shift+Alt+Y: Spell check
   - Ctrl+Shift+Alt+T: Translate

## Auto-start (Optional)
1. Press Win+R, type: shell:startup
2. Create shortcut to SpellingChecker.exe in the Startup folder

See README.md for full documentation.
"@
$readme | Out-File -FilePath "$distPath\PORTABLE-README.txt" -Encoding UTF8

# Step 5: Create ZIP
Compress-Archive -Path $distPath -DestinationPath "dist\SpellingChecker-v1.0.0-portable-win-x64.zip" -CompressionLevel Optimal
```

### Output

The portable ZIP will be created at:
```
dist\SpellingChecker-v1.0.0-portable-win-x64.zip
```

Size: ~15-20 MB (self-contained)

### What Users Get

When users extract the ZIP:

```
SpellingChecker-portable/
├── SpellingChecker.exe    (Main application)
├── PORTABLE-README.txt    (Quick start guide)
├── README.md             (Full documentation)
├── CONFIG.md             (Configuration guide)
├── QUICKSTART.md         (Quick start guide)
└── LICENSE               (MIT License)
```

Users simply run `SpellingChecker.exe` - no installation needed!

---

## Option 3: Standalone Executable

Single EXE file - simplest distribution method.

### Why Standalone?

- ✅ Single file - easiest to distribute
- ✅ No additional files needed
- ✅ Perfect for quick testing
- ✅ Can be placed anywhere

### Build Steps

```powershell
dotnet publish SpellingChecker/SpellingChecker.csproj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true
```

### Output

The executable will be created at:
```
SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
```

Size: ~15-20 MB (self-contained, includes .NET runtime)

### What Users Get

Just a single `SpellingChecker.exe` file that runs on any Windows 10+ system.

---

## Comparison

| Feature | Portable ZIP | Standalone EXE |
|---------|--------------|----------------|
| Installation | Not required | Not required |
| File count | Multiple | Single |
| Documentation included | ✅ Yes | ❌ No |
| Size | 15-20 MB | 15-20 MB |
| Convenience | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| Portability | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |

---

## GitHub Actions (Automated Builds)

The repository includes GitHub Actions workflow that automatically builds all distribution options on every commit.

Artifacts are available in the Actions tab:

- `SpellingChecker-Standalone-win-x64` - Standalone executable
- `SpellingChecker-Portable-win-x64` - Portable ZIP

---

## Code Signing (Optional but Recommended)

For production releases, consider signing the executable to avoid Windows SmartScreen warnings.

### Get a Code Signing Certificate

1. Purchase from a Certificate Authority (CA):
   - DigiCert
   - Sectigo (formerly Comodo)
   - GlobalSign
   
2. Or use a free alternative:
   - Self-signed certificate (for testing only)

### Sign the Executable

```powershell
# Using signtool (included with Windows SDK)
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com SpellingChecker.exe
```

---

## Distribution Checklist

Before releasing:

- [ ] Update version number in:
  - [ ] `Product.wxs` (ProductVersion)
  - [ ] `SpellingChecker.csproj` (Version)
  - [ ] `CHANGELOG.md`
- [ ] Test all three distribution methods
- [ ] Test on clean Windows installation
- [ ] Sign executable (if certificate available)
- [ ] Create release notes
- [ ] Tag release in Git
- [ ] Upload to GitHub Releases

---

## Troubleshooting

### Build Fails - .NET SDK Not Found

**Error**: "The specified .NET SDK version is not installed"

**Solution**:
1. Download .NET 9.0 SDK from https://dotnet.microsoft.com/download
2. Install and restart
3. Verify: `dotnet --version` should show 9.0.x

### Portable ZIP - Access Denied

**Error**: "Access denied" when creating ZIP

**Solution**:
- Close any programs that might have files open
- Run PowerShell as Administrator
- Or delete the `dist` folder first

---

## Migration from Inno Setup

If you were using Inno Setup (now paid software), use the portable deployment approach:

1. **Why portable instead of installer?**:
   - No installation required - easier for users
   - No administrative rights needed
   - Runs from anywhere (USB, network drive, etc.)
   - Completely free - no paid tools required

2. **Migration steps**:
   - Remove `installer.iss` (deprecated)
   - Use `build-portable.ps1` or `build-portable.bat`
   - Distribute ZIP file instead of EXE installer

3. **What's different**:
   - Users extract ZIP and run executable directly
   - No Start Menu shortcuts (users can create manually if desired)
   - No uninstaller needed (just delete the folder)
   - No custom graphics/themes (uses Windows standard)

---

## Support

For questions or issues:

- GitHub Issues: https://github.com/shinepcs/SpellingChecker/issues
- Documentation: See BUILD.md, README.md

---

**Free Tools Used**:
- .NET SDK - Free (Microsoft)
- PowerShell - Free (included in Windows)

**No paid tools required!** 🎉
