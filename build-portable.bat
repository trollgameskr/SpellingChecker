@echo off
REM Build Portable ZIP Distribution
REM This script creates a portable ZIP file that doesn't require installation

setlocal
set VERSION=1.0.0

echo ============================================
echo SpellingChecker Portable ZIP Build Script
echo ============================================
echo.

REM Step 1: Build the application
echo [1/3] Building application...
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:Version=%VERSION%
if errorlevel 1 (
    echo   X Failed to build application
    exit /b 1
)
echo   + Application built successfully
echo.

REM Step 2: Prepare distribution folder
echo [2/3] Preparing distribution folder...
set DIST_PATH=dist\SpellingChecker-v%VERSION%-portable
if exist "%DIST_PATH%" rmdir /s /q "%DIST_PATH%"
mkdir "%DIST_PATH%"

REM Copy files
copy "SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" "%DIST_PATH%\" >nul
copy "README.md" "%DIST_PATH%\" >nul
copy "LICENSE" "%DIST_PATH%\" >nul
copy "CONFIG.md" "%DIST_PATH%\" >nul
copy "QUICKSTART.md" "%DIST_PATH%\" >nul
echo   + Copied files to distribution folder
echo.

REM Create portable README
(
echo # AI Spelling Checker - Portable Version
echo.
echo ## Quick Start
echo.
echo 1. Run SpellingChecker.exe
echo 2. Configure your OpenAI API key in Settings ^(system tray icon^)
echo 3. Use the hotkeys:
echo    - **Ctrl+Shift+Alt+Y**: Spell check selected text
echo    - **Ctrl+Shift+Alt+T**: Translate selected text
echo.
echo ## Auto-start with Windows ^(Optional^)
echo.
echo To make SpellingChecker start automatically with Windows:
echo.
echo 1. Press Win+R to open Run dialog
echo 2. Type: shell:startup and press Enter
echo 3. Create a shortcut to SpellingChecker.exe in the Startup folder
echo.
echo ## Documentation
echo.
echo - README.md - Full documentation
echo - CONFIG.md - Configuration guide
echo - QUICKSTART.md - Quick start guide
echo.
echo ## Support
echo.
echo - GitHub: https://github.com/shinepcs/SpellingChecker
echo - Issues: https://github.com/shinepcs/SpellingChecker/issues
echo.
echo Version: %VERSION%
) > "%DIST_PATH%\PORTABLE-README.txt"
echo   + Created portable README
echo.

REM Step 3: Create ZIP archive
echo [3/3] Creating ZIP archive...
set ZIP_PATH=dist\SpellingChecker-v%VERSION%-portable-win-x64.zip
if exist "%ZIP_PATH%" del /q "%ZIP_PATH%"

REM Use PowerShell to create ZIP (available on all modern Windows)
powershell -Command "Compress-Archive -Path '%DIST_PATH%' -DestinationPath '%ZIP_PATH%' -CompressionLevel Optimal"
if errorlevel 1 (
    echo   X Failed to create ZIP archive
    exit /b 1
)
echo   + ZIP archive created
echo.

REM Summary
echo ============================================
echo Portable Build Complete!
echo ============================================
echo.
echo Portable ZIP Location:
echo   %ZIP_PATH%
echo.
echo Distribution Folder:
echo   %DIST_PATH%
echo.
echo Users can extract the ZIP and run SpellingChecker.exe directly!
echo.
pause
