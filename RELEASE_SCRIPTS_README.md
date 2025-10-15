# Release Build Scripts

이 폴더에는 SpellingChecker 릴리스를 빌드하기 위한 스크립트가 포함되어 있습니다.

## 사용 가능한 스크립트

### build-release.ps1 (PowerShell - 권장)
PowerShell 스크립트로 크로스 플랫폼 지원이 더 좋습니다.

**사용법:**
```powershell
.\build-release.ps1 -Version "1.0.0"
```

### build-release.bat (배치 파일)
Windows 명령 프롬프트용 배치 스크립트입니다.

**사용법:**
```cmd
build-release.bat 1.0.0
```

## 스크립트 실행 내용

두 스크립트 모두 다음 작업을 수행합니다:

1. **이전 빌드 정리** - 기존 Release 폴더 삭제
2. **의존성 복원** - NuGet 패키지 복원
3. **솔루션 빌드** - Release 구성으로 빌드
4. **실행 파일 게시** - 자체 포함 단일 파일 생성
5. **설치 프로그램 생성** - Inno Setup으로 설치 프로그램 컴파일 (선택사항)

## 요구 사항

- Windows 10 이상
- .NET 9.0 SDK
- Inno Setup 6.x (설치 프로그램 생성용, 선택사항)

## 출력 파일

스크립트 실행 후 다음 파일이 생성됩니다:

- **실행 파일**: `SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe`
- **설치 프로그램**: `installer\SpellingCheckerSetup_v{버전}.exe` (Inno Setup이 설치된 경우)

## 문제 해결

스크립트 실행 시 문제가 발생하면:

1. .NET 9.0 SDK가 설치되어 있는지 확인: `dotnet --version`
2. Inno Setup 설치 경로 확인: `C:\Program Files (x86)\Inno Setup 6\`
3. 관리자 권한으로 실행해 보세요
4. 자세한 내용은 [DEPLOYMENT.md](DEPLOYMENT.md)를 참조하세요

## 추가 정보

더 자세한 배포 가이드는 다음을 참조하세요:
- [DEPLOYMENT.md](DEPLOYMENT.md) - 전체 배포 가이드
- [BUILD.md](BUILD.md) - 빌드 및 개발 가이드

---

# Release Build Scripts (English)

This folder contains scripts for building SpellingChecker releases.

## Available Scripts

### build-release.ps1 (PowerShell - Recommended)
PowerShell script with better cross-platform support.

**Usage:**
```powershell
.\build-release.ps1 -Version "1.0.0"
```

### build-release.bat (Batch File)
Batch script for Windows Command Prompt.

**Usage:**
```cmd
build-release.bat 1.0.0
```

## What the Scripts Do

Both scripts perform the following tasks:

1. **Clean previous builds** - Remove existing Release folders
2. **Restore dependencies** - Restore NuGet packages
3. **Build solution** - Build in Release configuration
4. **Publish executable** - Create self-contained single file
5. **Create installer** - Compile installer with Inno Setup (optional)

## Requirements

- Windows 10 or later
- .NET 9.0 SDK
- Inno Setup 6.x (for installer creation, optional)

## Output Files

After running the script, the following files will be created:

- **Executable**: `SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe`
- **Installer**: `installer\SpellingCheckerSetup_v{version}.exe` (if Inno Setup is installed)

## Troubleshooting

If you encounter issues running the script:

1. Verify .NET 9.0 SDK is installed: `dotnet --version`
2. Check Inno Setup installation path: `C:\Program Files (x86)\Inno Setup 6\`
3. Try running with administrator privileges
4. See [DEPLOYMENT.md](DEPLOYMENT.md) for more details

## More Information

For detailed deployment guide, see:
- [DEPLOYMENT.md](DEPLOYMENT.md) - Complete deployment guide
- [BUILD.md](BUILD.md) - Build and development guide
