# 🚀 배포 파일 생성 방법 / How to Create Deployment Files

이 저장소는 **무료 오픈소스 도구만** 사용하여 배포 파일을 생성합니다.

This repository uses **only free and open-source tools** to create deployment files.

---

## 한국어 / Korean

### 📦 사용 가능한 배포 옵션

#### 1. 포터블 ZIP (추천)
설치 불필요, 압축 해제 후 바로 사용

```powershell
# PowerShell 사용
.\build-portable.ps1

# 또는 배치 파일 사용
build-portable.bat
```

**결과물**: `dist\SpellingChecker-v1.0.0-portable-win-x64.zip`

**필요한 것**: 없음 (PowerShell만 있으면 됨)

#### 2. 단독 실행 파일
가장 간단한 방법

```bash
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

**결과물**: `SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe`

### 📚 자세한 문서

- **[배포가이드.md](배포가이드.md)** - 한국어 빠른 가이드
- **[배포워크플로우.md](배포워크플로우.md)** - 시각적 다이어그램
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - 완전한 가이드 (영문)

### ❓ 왜 변경되었나요?

**이전**: Inno Setup 사용 → 2025년 10월부터 유료 💰

**현재**: PowerShell (무료) → 완전 무료! ✅

---

## English

### 📦 Available Deployment Options

#### 1. Portable ZIP (Recommended)
No installation required

```powershell
# Using PowerShell
.\build-portable.ps1

# Or using batch file
build-portable.bat
```

**Output**: `dist\SpellingChecker-v1.0.0-portable-win-x64.zip`

**Requires**: Nothing (PowerShell is built into Windows)

#### 2. Standalone Executable
Simplest method

```bash
dotnet publish SpellingChecker/SpellingChecker.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

**Output**: `SpellingChecker\bin\Release\net9.0-windows\win-x64\publish\SpellingChecker.exe`

### 📚 Detailed Documentation

- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Complete deployment guide
- **[DEPLOYMENT-MIGRATION.md](DEPLOYMENT-MIGRATION.md)** - Migration summary
- **[BUILD.md](BUILD.md)** - Build guide

### ❓ Why the Change?

**Before**: Inno Setup → Became paid software in October 2025 💰

**Now**: PowerShell (free) → Completely free! ✅

---

## 🔧 Quick Start

### Prerequisites
- Windows 10 or later
- .NET 9.0 SDK

### Build Portable ZIP

```powershell
# Build portable ZIP (easiest)
.\build-portable.ps1
```

### Automated Builds

GitHub Actions automatically builds all formats on every push!

Download from: **Actions** tab → Select workflow run → **Artifacts**

---

## 🆓 All Tools Are Free!

| Tool | License | Purpose |
|------|---------|---------|
| .NET SDK | Free (Microsoft) | Build application |
| PowerShell | Built into Windows | Build scripts |

**No paid tools required!** 🎉

---

## 📖 Full Documentation Index

- [README.md](README.md) - Main documentation
- [DEPLOYMENT.md](DEPLOYMENT.md) - Deployment guide (English)
- [배포가이드.md](배포가이드.md) - Deployment guide (Korean)
- [배포워크플로우.md](배포워크플로우.md) - Visual workflow diagrams (Korean)
- [DEPLOYMENT-MIGRATION.md](DEPLOYMENT-MIGRATION.md) - Migration summary
- [BUILD.md](BUILD.md) - Build and development guide
- [DOCS_INDEX.md](DOCS_INDEX.md) - Complete documentation index

---

## 💬 Support

- **Issues**: https://github.com/trollgameskr/SpellingChecker/issues
- **Discussions**: GitHub Discussions
- **Documentation**: See files above

---

**Made with ❤️ using only free and open-source tools**
