@echo off
REM Build Release Script for SpellingChecker
REM This script builds the application and creates the installer

echo ========================================
echo SpellingChecker Release Build Script
echo ========================================
echo.

REM Check if version parameter is provided
set VERSION=%1
if "%VERSION%"=="" (
    echo Usage: build-release.bat [version]
    echo Example: build-release.bat 1.0.0
    exit /b 1
)

echo Version: %VERSION%
echo.

echo [1/5] Cleaning previous builds...
if exist "SpellingChecker\bin\Release" rmdir /s /q "SpellingChecker\bin\Release"
if exist "installer" rmdir /s /q "installer"
echo Done.
echo.

echo [2/5] Restoring dependencies...
dotnet restore SpellingChecker.sln
if errorlevel 1 (
    echo Error restoring dependencies!
    exit /b 1
)
echo Done.
echo.

echo [3/5] Building solution...
dotnet build SpellingChecker.sln --configuration Release --no-restore
if errorlevel 1 (
    echo Error building solution!
    exit /b 1
)
echo Done.
echo.

echo [4/5] Publishing executable...
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
if errorlevel 1 (
    echo Error publishing executable!
    exit /b 1
)
echo Done.
echo.

echo [5/5] Creating installer with Inno Setup...
echo.
echo NOTE: This step requires Inno Setup to be installed.
echo Download from: https://jrsoftware.org/isinfo.php
echo.

REM Check if Inno Setup is installed
set INNO_PATH="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if not exist %INNO_PATH% (
    echo Inno Setup not found at %INNO_PATH%
    echo Please install Inno Setup and try again.
    echo.
    echo However, the executable has been built successfully at:
    echo SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
    exit /b 0
)

REM Compile the installer
%INNO_PATH% installer.iss
if errorlevel 1 (
    echo Error creating installer!
    exit /b 1
)
echo Done.
echo.

echo ========================================
echo Build completed successfully!
echo ========================================
echo.
echo Output files:
echo - Executable: SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe
echo - Installer: installer\SpellingCheckerSetup_v%VERSION%.exe
echo.
echo You can now distribute these files.
echo.

exit /b 0
