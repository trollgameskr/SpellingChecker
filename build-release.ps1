#!/usr/bin/env pwsh
# Build Release Script for SpellingChecker
# This script builds the application and creates the installer

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SpellingChecker Release Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Version: $Version" -ForegroundColor Yellow
Write-Host ""

# Step 1: Clean previous builds
Write-Host "[1/5] Cleaning previous builds..." -ForegroundColor Green
if (Test-Path "SpellingChecker\bin\Release") {
    Remove-Item -Recurse -Force "SpellingChecker\bin\Release"
}
if (Test-Path "installer") {
    Remove-Item -Recurse -Force "installer"
}
Write-Host "Done." -ForegroundColor Green
Write-Host ""

# Step 2: Restore dependencies
Write-Host "[2/5] Restoring dependencies..." -ForegroundColor Green
dotnet restore SpellingChecker.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error restoring dependencies!" -ForegroundColor Red
    exit 1
}
Write-Host "Done." -ForegroundColor Green
Write-Host ""

# Step 3: Build solution
Write-Host "[3/5] Building solution..." -ForegroundColor Green
dotnet build SpellingChecker.sln --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error building solution!" -ForegroundColor Red
    exit 1
}
Write-Host "Done." -ForegroundColor Green
Write-Host ""

# Step 4: Publish executable
Write-Host "[4/5] Publishing executable..." -ForegroundColor Green
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error publishing executable!" -ForegroundColor Red
    exit 1
}
Write-Host "Done." -ForegroundColor Green
Write-Host ""

# Step 5: Create installer with Inno Setup
Write-Host "[5/5] Creating installer with Inno Setup..." -ForegroundColor Green
Write-Host ""
Write-Host "NOTE: This step requires Inno Setup to be installed." -ForegroundColor Yellow
Write-Host "Download from: https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
Write-Host ""

$InnoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $InnoPath)) {
    Write-Host "Inno Setup not found at $InnoPath" -ForegroundColor Yellow
    Write-Host "Please install Inno Setup and try again." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "However, the executable has been built successfully at:" -ForegroundColor Green
    Write-Host "SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -ForegroundColor Cyan
    exit 0
}

# Compile the installer
& $InnoPath installer.iss
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error creating installer!" -ForegroundColor Red
    exit 1
}
Write-Host "Done." -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build completed successfully!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Output files:" -ForegroundColor Green
Write-Host "- Executable: SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe" -ForegroundColor Cyan
Write-Host "- Installer: installer\SpellingCheckerSetup_v$Version.exe" -ForegroundColor Cyan
Write-Host ""
Write-Host "You can now distribute these files." -ForegroundColor Green
Write-Host ""

exit 0
