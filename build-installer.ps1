# Build Installer Script for SpellingChecker
# This script builds the application and creates a WiX-based MSI installer
# Requires: .NET 9.0 SDK, WiX Toolset 3.11 or later

param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "SpellingChecker Installer Build Script" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Clean previous builds
Write-Host "[1/4] Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path ".\SpellingChecker\bin") {
    Remove-Item ".\SpellingChecker\bin" -Recurse -Force
    Write-Host "  ✓ Cleaned bin folder" -ForegroundColor Green
}
if (Test-Path ".\SpellingChecker\obj") {
    Remove-Item ".\SpellingChecker\obj" -Recurse -Force
    Write-Host "  ✓ Cleaned obj folder" -ForegroundColor Green
}
if (Test-Path ".\installer") {
    Remove-Item ".\installer" -Recurse -Force
    Write-Host "  ✓ Cleaned installer folder" -ForegroundColor Green
}
if (Test-Path ".\obj") {
    Remove-Item ".\obj" -Recurse -Force
    Write-Host "  ✓ Cleaned build artifacts" -ForegroundColor Green
}
Write-Host ""

# Step 2: Restore dependencies
Write-Host "[2/4] Restoring dependencies..." -ForegroundColor Yellow
dotnet restore SpellingChecker.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Failed to restore dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Dependencies restored" -ForegroundColor Green
Write-Host ""

# Step 3: Build and publish the application
Write-Host "[3/4] Building and publishing application..." -ForegroundColor Yellow
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

# Step 4: Build the installer
Write-Host "[4/4] Building WiX installer..." -ForegroundColor Yellow

# Check if WiX is installed
$wixPath = "${env:WIX}bin"
if (-not (Test-Path $wixPath)) {
    Write-Host "  ✗ WiX Toolset not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install WiX Toolset from: https://wixtoolset.org/releases/" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternative: Use the pre-built executable from:" -ForegroundColor Yellow
    Write-Host "  SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -ForegroundColor Cyan
    exit 1
}

# Create output directory
New-Item -ItemType Directory -Force -Path ".\installer" | Out-Null

# Build with candle (WiX compiler)
& "$wixPath\candle.exe" -arch x64 -out ".\obj\Product.wixobj" "Product.wxs" -ext WixUIExtension -ext WixUtilExtension
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Failed to compile WiX source" -ForegroundColor Red
    exit 1
}

# Link with light (WiX linker)
& "$wixPath\light.exe" -out ".\installer\SpellingCheckerSetup_v$Version.msi" ".\obj\Product.wixobj" -ext WixUIExtension -ext WixUtilExtension -cultures:en-us
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Failed to link WiX installer" -ForegroundColor Red
    exit 1
}

Write-Host "  ✓ Installer created successfully" -ForegroundColor Green
Write-Host ""

# Summary
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Installer Location:" -ForegroundColor Yellow
Write-Host "  .\installer\SpellingCheckerSetup_v$Version.msi" -ForegroundColor Cyan
Write-Host ""
Write-Host "Standalone Executable:" -ForegroundColor Yellow
Write-Host "  .\SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -ForegroundColor Cyan
Write-Host ""
