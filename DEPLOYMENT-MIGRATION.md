# Deployment Files Migration Summary

## Overview

This migration replaces **Inno Setup** (now paid software as of October 2025) with **free and open-source alternatives** for creating SpellingChecker deployment files.

## What Changed

### Removed (Deprecated)
- ❌ `installer.iss` - Inno Setup script (moved to `installer.iss.deprecated`)
- ❌ All references to Inno Setup in documentation

### Added (New Free Alternatives)

#### 1. WiX Toolset Support (MSI Installer)
- ✅ `Product.wxs` - WiX installer configuration
- ✅ `Installer.wixproj` - WiX project file
- ✅ `build-installer.ps1` - PowerShell build script
- ✅ `build-installer.bat` - Windows batch build script

#### 2. Portable ZIP Distribution
- ✅ `build-portable.ps1` - PowerShell build script
- ✅ `build-portable.bat` - Windows batch build script

#### 3. Documentation
- ✅ `DEPLOYMENT.md` - Complete deployment guide (English)
- ✅ `배포가이드.md` - Deployment guide (Korean)
- ✅ `INSTALLER-ISS-DEPRECATED.md` - Migration notice

#### 4. Updated Files
- ✅ `BUILD.md` - Updated with free deployment options
- ✅ `README.md` - Added installation options
- ✅ `DOCS_INDEX.md` - Added DEPLOYMENT.md reference
- ✅ `.github/workflows/build.yml` - Automated builds for all formats
- ✅ `.gitignore` - Exclude build artifacts

## Deployment Options

### Option 1: MSI Installer (Recommended)
**Tool**: WiX Toolset (Free, Open-Source)

**Features**:
- Professional Windows installer
- Add/Remove Programs integration
- Start Menu shortcuts
- Optional desktop and startup shortcuts
- Clean uninstallation

**Build Command**:
```powershell
.\build-installer.ps1
```
or
```cmd
build-installer.bat
```

**Output**: `installer\SpellingCheckerSetup_v1.0.0.msi`

### Option 2: Portable ZIP
**Tool**: PowerShell (Built into Windows)

**Features**:
- No installation required
- Run from anywhere (USB drive, network share)
- No registry changes
- Easy cleanup (delete folder)

**Build Command**:
```powershell
.\build-portable.ps1
```
or
```cmd
build-portable.bat
```

**Output**: `dist\SpellingChecker-v1.0.0-portable-win-x64.zip`

### Option 3: Standalone Executable
**Tool**: .NET SDK (Free)

**Features**:
- Single EXE file
- Self-contained (includes .NET runtime)
- Simplest distribution

**Build Command**:
```bash
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

**Output**: `SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe`

## GitHub Actions Integration

The workflow now automatically builds:

### On Every Push
- Standalone executable
- Portable ZIP

### On Releases
- MSI installer (if WiX is available)
- All other formats

All artifacts are uploaded and available for download from the Actions tab.

## Benefits of New Approach

### Cost
- **Old**: Inno Setup (now paid)
- **New**: All tools are FREE
  - WiX Toolset: Free, open-source (MS-PL)
  - PowerShell: Built into Windows
  - .NET SDK: Free (Microsoft)

### Professionalism
- **MSI format**: Industry standard for Windows installers
- **Better integration**: Windows Add/Remove Programs
- **Enterprise ready**: MSI supports Group Policy deployment

### Flexibility
- **3 options**: Choose based on use case
- **Automated builds**: GitHub Actions integration
- **Cross-platform scripts**: PowerShell + Batch

### User Experience
- **MSI**: Professional installation wizard
- **Portable**: No installation required
- **Standalone**: Single file simplicity

## Migration Guide (for Developers)

### From Inno Setup to WiX

1. **Install WiX Toolset**
   - Download from https://wixtoolset.org/releases/
   - Free and open-source

2. **Use new build scripts**
   - Old: Compile `installer.iss` with Inno Setup
   - New: Run `.\build-installer.ps1`

3. **Output format**
   - Old: `.exe` installer
   - New: `.msi` installer (better for enterprise)

4. **Features mapping**
   - Start menu shortcuts: ✅ Both support
   - Desktop shortcuts: ✅ Both support  
   - Startup shortcuts: ✅ Both support
   - Custom graphics: ❌ WiX uses Windows standard
   - Uninstaller: ✅ WiX uses Windows Settings

### For End Users

No changes needed! Users can:
- Install using MSI (double-click, follow wizard)
- Use portable ZIP (extract and run)
- Run standalone EXE (no installation)

All methods work the same after installation/extraction.

## Testing

### Automated Testing
- GitHub Actions builds on every commit
- Artifacts available in Actions tab

### Manual Testing Checklist
- [ ] MSI installer builds successfully
- [ ] MSI installs correctly
- [ ] Start menu shortcut works
- [ ] Application runs after install
- [ ] Uninstall works correctly
- [ ] Portable ZIP extracts correctly
- [ ] Portable version runs without installation
- [ ] Standalone EXE runs

## Documentation

All deployment information is available in:

1. **DEPLOYMENT.md** - Complete guide (English)
2. **배포가이드.md** - Quick guide (Korean)
3. **BUILD.md** - Developer build guide
4. **README.md** - Installation options

## Support

For issues related to deployment:

1. Check documentation first
2. GitHub Issues: https://github.com/shinepcs/SpellingChecker/issues
3. Include:
   - Which deployment method you're using
   - Error messages (if any)
   - Windows version
   - Build environment details

## Future Enhancements

Possible improvements:

- [ ] Code signing support
- [ ] Auto-update functionality
- [ ] Chocolatey package
- [ ] winget package
- [ ] Multi-language installer UI

## License

All deployment tools used are free and open-source:

- **WiX Toolset**: MS-PL License
- **PowerShell**: MIT License
- **.NET SDK**: MIT License
- **SpellingChecker**: MIT License

---

**Migration completed successfully!** 🎉

All deployment files now use **100% free tools**.
