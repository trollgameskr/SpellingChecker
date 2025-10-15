# Create Portable ZIP Distribution
# This script creates a portable ZIP file that doesn't require installation
# Users can extract and run SpellingChecker.exe directly

param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "SpellingChecker Portable ZIP Build Script" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build the application
Write-Host "[1/3] Building application..." -ForegroundColor Yellow
dotnet publish SpellingChecker/SpellingChecker.csproj `
    -c $Configuration `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:Version=$Version

if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Failed to build application" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Application built successfully" -ForegroundColor Green
Write-Host ""

# Step 2: Prepare distribution folder
Write-Host "[2/3] Preparing distribution folder..." -ForegroundColor Yellow
$distPath = ".\dist\SpellingChecker-v$Version-portable"
if (Test-Path $distPath) {
    Remove-Item $distPath -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $distPath | Out-Null

# Copy executable
Copy-Item ".\SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -Destination $distPath
Write-Host "  ✓ Copied executable" -ForegroundColor Green

# Copy documentation
Copy-Item ".\README.md" -Destination $distPath
Copy-Item ".\LICENSE" -Destination $distPath
Copy-Item ".\CONFIG.md" -Destination $distPath
Copy-Item ".\QUICKSTART.md" -Destination $distPath
Write-Host "  ✓ Copied documentation" -ForegroundColor Green

# Create a README for portable version
$portableReadme = @"
# AI Spelling Checker - Portable Version

## Quick Start

1. Run ``SpellingChecker.exe``
2. Configure your OpenAI API key in Settings (system tray icon)
3. Use the hotkeys:
   - **Ctrl+Shift+Alt+Y**: Spell check selected text
   - **Ctrl+Shift+Alt+T**: Translate selected text

## Auto-start with Windows (Optional)

To make SpellingChecker start automatically with Windows:

1. Press ``Win+R`` to open Run dialog
2. Type ``shell:startup`` and press Enter
3. Create a shortcut to ``SpellingChecker.exe`` in the Startup folder

## Documentation

- ``README.md`` - Full documentation
- ``CONFIG.md`` - Configuration guide
- ``QUICKSTART.md`` - Quick start guide

## Support

- GitHub: https://github.com/shinepcs/SpellingChecker
- Issues: https://github.com/shinepcs/SpellingChecker/issues

---

Version: $Version
"@

$portableReadme | Out-File -FilePath "$distPath\PORTABLE-README.txt" -Encoding UTF8
Write-Host "  ✓ Created portable README" -ForegroundColor Green
Write-Host ""

# Step 3: Create ZIP archive
Write-Host "[3/3] Creating ZIP archive..." -ForegroundColor Yellow
$zipPath = ".\dist\SpellingChecker-v$Version-portable-win-x64.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Compress-Archive -Path $distPath -DestinationPath $zipPath -CompressionLevel Optimal
Write-Host "  ✓ ZIP archive created" -ForegroundColor Green
Write-Host ""

# Summary
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Portable Build Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Portable ZIP Location:" -ForegroundColor Yellow
Write-Host "  $zipPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Distribution Folder:" -ForegroundColor Yellow
Write-Host "  $distPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Users can extract the ZIP and run SpellingChecker.exe directly!" -ForegroundColor Green
Write-Host ""
