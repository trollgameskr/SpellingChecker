@echo off
REM Build MSI Installer Script for SpellingChecker
REM This script builds the application and creates a WiX-based MSI installer
REM Requires: .NET 9.0 SDK, WiX Toolset 3.11 or later

setlocal
set VERSION=1.0.0
set CONFIGURATION=Release

echo ============================================
echo SpellingChecker Installer Build Script
echo ============================================
echo.

REM Step 1: Clean previous builds
echo [1/4] Cleaning previous builds...
if exist "SpellingChecker\bin" rmdir /s /q "SpellingChecker\bin"
if exist "SpellingChecker\obj" rmdir /s /q "SpellingChecker\obj"
if exist "installer" rmdir /s /q "installer"
if exist "obj" rmdir /s /q "obj"
echo   + Cleaned build folders
echo.

REM Step 2: Restore dependencies
echo [2/4] Restoring dependencies...
dotnet restore SpellingChecker.sln
if errorlevel 1 (
    echo   X Failed to restore dependencies
    exit /b 1
)
echo   + Dependencies restored
echo.

REM Step 3: Build and publish the application
echo [3/4] Building and publishing application...
dotnet publish SpellingChecker/SpellingChecker.csproj -c %CONFIGURATION% -r win-x64 --self-contained -p:PublishSingleFile=true -p:Version=%VERSION%
if errorlevel 1 (
    echo   X Failed to build application
    exit /b 1
)
echo   + Application built successfully
echo.

REM Step 4: Build the installer
echo [4/4] Building WiX installer...

REM Check if WiX is installed
if not defined WIX (
    echo   X WiX Toolset not found!
    echo.
    echo Please install WiX Toolset from: https://wixtoolset.org/releases/
    echo.
    echo Alternative: Use the pre-built executable from:
    echo   SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
    pause
    exit /b 1
)

set WIXPATH=%WIX%bin

REM Create output directories
mkdir obj 2>nul
mkdir installer 2>nul

REM Build with candle (WiX compiler)
echo   Compiling WiX source...
"%WIXPATH%\candle.exe" -arch x64 -out "obj\Product.wixobj" "Product.wxs" -ext WixUIExtension -ext WixUtilExtension
if errorlevel 1 (
    echo   X Failed to compile WiX source
    pause
    exit /b 1
)

REM Link with light (WiX linker)
echo   Linking WiX installer...
"%WIXPATH%\light.exe" -out "installer\SpellingCheckerSetup_v%VERSION%.msi" "obj\Product.wixobj" -ext WixUIExtension -ext WixUtilExtension -cultures:en-us -sval
if errorlevel 1 (
    echo   X Failed to link WiX installer
    pause
    exit /b 1
)

echo   + Installer created successfully
echo.

REM Summary
echo ============================================
echo Build Complete!
echo ============================================
echo.
echo Installer Location:
echo   installer\SpellingCheckerSetup_v%VERSION%.msi
echo.
echo Standalone Executable:
echo   SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
echo.
pause
