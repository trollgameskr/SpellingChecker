# 배포 파일 요약 (Deployment Files Summary)

이 문서는 SpellingChecker 프로젝트에 추가된 배포 관련 파일들의 요약입니다.

## 📦 생성된 파일들 (Created Files)

### 1. GitHub Actions 워크플로우
**파일**: `.github/workflows/release.yml`

- **목적**: 자동화된 릴리스 빌드 및 GitHub Release 생성
- **트리거**: 
  - `v*.*.*` 형식의 Git 태그 푸시 시 자동 실행
  - 수동 실행 가능 (workflow_dispatch)
- **수행 작업**:
  1. .NET 9.0 환경 설정
  2. 프로젝트 빌드
  3. 자체 포함 실행 파일 게시
  4. Inno Setup 설치 및 설치 프로그램 컴파일
  5. GitHub Release 생성
  6. 빌드 산출물 업로드 (설치 프로그램, 실행 파일, README)

### 2. 로컬 빌드 스크립트

#### build-release.ps1 (PowerShell)
- **목적**: Windows에서 로컬 릴리스 빌드
- **사용법**: `.\build-release.ps1 -Version "1.0.0"`
- **장점**: 
  - 컬러 출력
  - 자세한 에러 처리
  - 크로스 플랫폼 지원 가능

#### build-release.bat (배치 파일)
- **목적**: Windows 명령 프롬프트용 빌드 스크립트
- **사용법**: `build-release.bat 1.0.0`
- **장점**: 
  - .NET이나 PowerShell 없이도 실행 가능
  - 간단한 구조

두 스크립트 모두 다음을 수행:
- 이전 빌드 정리
- 의존성 복원
- Release 빌드
- 실행 파일 게시
- Inno Setup 설치 프로그램 생성 (선택사항)

### 3. 문서

#### DEPLOYMENT.md
**종합 배포 가이드**
- 로컬 빌드 방법
- GitHub Actions를 통한 자동 릴리스
- 수동 배포 단계
- 버전 관리 규칙
- 테스트 체크리스트
- 문제 해결 가이드
- 코드 서명 정보

#### RELEASE_CHECKLIST.md
**릴리스 체크리스트**
- 릴리스 전 확인사항
- 버전 업데이트 항목
- 테스트 절차
- 설치 프로그램 테스트
- 릴리스 후 작업
- 긴급 패치 절차
- 롤백 절차

#### RELEASE_SCRIPTS_README.md
**빌드 스크립트 사용 가이드**
- 스크립트 사용법 (한국어/영어)
- 요구사항
- 출력 파일 위치
- 문제 해결

## 🚀 사용 방법 (How to Use)

### 자동 릴리스 (권장)

1. 버전 번호 업데이트:
   - `SpellingChecker/SpellingChecker.csproj` - Version 태그
   - `installer.iss` - AppVersion

2. 변경사항 커밋:
   ```bash
   git add .
   git commit -m "Release v1.0.0"
   git push origin main
   ```

3. 태그 생성 및 푸시:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

4. GitHub Actions가 자동으로:
   - 빌드 수행
   - 설치 프로그램 생성
   - GitHub Release 생성
   - 파일 업로드

### 로컬 빌드

PowerShell 사용 (권장):
```powershell
.\build-release.ps1 -Version "1.0.0"
```

또는 배치 파일:
```cmd
build-release.bat 1.0.0
```

### 수동 GitHub UI에서 릴리스

1. GitHub 리포지토리의 "Actions" 탭으로 이동
2. "Release" 워크플로우 선택
3. "Run workflow" 클릭
4. 버전 번호 입력 (예: 1.0.0)
5. "Run workflow" 클릭하여 실행

## 📋 릴리스 체크리스트

릴리스를 만들기 전에:

1. ✅ 코드가 에러 없이 빌드되는가?
2. ✅ 모든 기능이 작동하는가?
3. ✅ 버전 번호가 업데이트되었는가?
4. ✅ CHANGELOG.md가 업데이트되었는가?
5. ✅ 깨끗한 Windows 환경에서 테스트했는가?

자세한 체크리스트는 [RELEASE_CHECKLIST.md](RELEASE_CHECKLIST.md)를 참조하세요.

## 📂 출력 파일 (Output Files)

릴리스 빌드 후 다음 파일이 생성됩니다:

1. **실행 파일**: 
   - 경로: `SpellingChecker/bin/Release/net9.0-windows/win-x64/publish/SpellingChecker.exe`
   - 크기: ~156MB (자체 포함, .NET 런타임 포함)
   - 설치 불필요, 실행만 하면 됨

2. **설치 프로그램**: 
   - 경로: `installer/SpellingCheckerSetup_v{버전}.exe`
   - 크기: ~60-80MB
   - 시작 메뉴 바로가기, Windows 자동 시작 등 포함

3. **GitHub Release에 포함**:
   - SpellingCheckerSetup_v{버전}.exe
   - SpellingChecker.exe (포터블 버전)
   - README.md

## 🔧 요구사항 (Requirements)

### 로컬 빌드용
- Windows 10 이상
- .NET 9.0 SDK
- Inno Setup 6.x (설치 프로그램 생성 시)

### GitHub Actions용
- GitHub 리포지토리 관리자 권한
- 릴리스 생성 권한

## 🆘 문제 해결 (Troubleshooting)

### 빌드 실패
- .NET 9.0 SDK 설치 확인: `dotnet --version`
- NuGet 패키지 복원: `dotnet restore`
- 빌드 폴더 정리 후 재시도

### Inno Setup 오류
- Inno Setup 설치 확인: `C:\Program Files (x86)\Inno Setup 6\`
- 필수 파일 존재 확인: README.md, LICENSE, CONFIG.md

### GitHub Actions 실패
- Actions 로그 확인
- 태그 형식 확인 (v1.0.0)
- 권한 확인

자세한 내용은 [DEPLOYMENT.md](DEPLOYMENT.md)의 문제 해결 섹션을 참조하세요.

## 📚 관련 문서 (Related Documentation)

- **[DEPLOYMENT.md](DEPLOYMENT.md)** - 전체 배포 가이드
- **[BUILD.md](BUILD.md)** - 빌드 및 개발 가이드
- **[RELEASE_CHECKLIST.md](RELEASE_CHECKLIST.md)** - 릴리스 체크리스트
- **[RELEASE_SCRIPTS_README.md](RELEASE_SCRIPTS_README.md)** - 스크립트 사용 가이드
- **[CHANGELOG.md](CHANGELOG.md)** - 버전 변경 기록

## 🎯 다음 단계 (Next Steps)

1. 첫 릴리스 생성 전에 [RELEASE_CHECKLIST.md](RELEASE_CHECKLIST.md) 확인
2. 버전 번호 업데이트
3. 태그 생성 및 푸시로 자동 릴리스 트리거
4. 생성된 릴리스 확인 및 테스트

## 💡 팁 (Tips)

- **자주 릴리스하기**: 작은 변경사항도 자주 릴리스하면 사용자에게 더 나은 경험 제공
- **버전 관리 규칙 따르기**: Semantic Versioning (MAJOR.MINOR.PATCH)
- **체크리스트 사용**: 매번 릴리스 전에 체크리스트 확인
- **테스트 철저히**: 특히 깨끗한 Windows 환경에서 설치 테스트

---

**작성일**: 2025-10-15

**작성자**: GitHub Copilot

이 파일들은 SpellingChecker 프로젝트의 배포 프로세스를 자동화하고 표준화하기 위해 생성되었습니다.
