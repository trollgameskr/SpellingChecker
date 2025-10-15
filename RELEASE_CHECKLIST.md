# Release Checklist

릴리스를 만들기 전에 이 체크리스트를 사용하세요.

## 릴리스 전 체크리스트 / Pre-Release Checklist

### 코드 품질 / Code Quality
- [ ] 모든 변경사항이 커밋되었는가?
- [ ] 코드가 에러 없이 빌드되는가?
- [ ] 모든 기능이 정상 작동하는가?
- [ ] 설정을 저장하고 불러올 수 있는가?
- [ ] 단축키가 올바르게 작동하는가?

### 버전 업데이트 / Version Updates
- [ ] `SpellingChecker/SpellingChecker.csproj`의 버전 번호 업데이트
- [ ] `installer.iss`의 AppVersion 업데이트
- [ ] `CHANGELOG.md` 업데이트
- [ ] 릴리스 노트 작성

### 테스트 / Testing
- [ ] API 통합 테스트 (실제 API 키로)
  - [ ] OpenAI API 테스트
  - [ ] Anthropic API 테스트 (선택사항)
  - [ ] Google Gemini API 테스트 (선택사항)
- [ ] 교정 기능 테스트 (Ctrl+Shift+C)
- [ ] 번역 기능 테스트 (Ctrl+Shift+T)
- [ ] 시스템 트레이 아이콘 및 메뉴 테스트
- [ ] 설정 창 테스트
- [ ] 통계 기능 테스트

### 빌드 테스트 / Build Testing
- [ ] Release 빌드 성공
- [ ] Publish 빌드 성공
- [ ] 실행 파일 생성 확인
- [ ] 실행 파일 실행 테스트
- [ ] 설치 프로그램 생성 성공

### 설치 프로그램 테스트 / Installer Testing
- [ ] 깨끗한 Windows 환경에 설치 테스트
- [ ] 시작 메뉴 바로가기 생성 확인
- [ ] "Windows와 함께 시작" 옵션 테스트
- [ ] 바탕화면 아이콘 생성 옵션 테스트
- [ ] 애플리케이션 실행 테스트
- [ ] 제거 테스트
  - [ ] 모든 파일이 삭제되는가?
  - [ ] 바로가기가 제거되는가?
  - [ ] 설정 폴더가 정리되는가?

### 문서 / Documentation
- [ ] README.md가 최신 상태인가?
- [ ] CHANGELOG.md에 새 버전이 추가되었는가?
- [ ] 릴리스 노트가 준비되었는가?
- [ ] 필요한 경우 문서 업데이트

### 릴리스 / Release
- [ ] Git 태그 생성 (`git tag v1.0.0`)
- [ ] 태그를 GitHub에 푸시 (`git push origin v1.0.0`)
- [ ] GitHub Actions 워크플로우 성공 확인
- [ ] GitHub 릴리스 페이지 확인
- [ ] 다운로드 링크 테스트
- [ ] 릴리스 공지 (필요한 경우)

## 빠른 릴리스 가이드 / Quick Release Guide

### 자동 릴리스 (권장) / Automated Release (Recommended)

```bash
# 1. 버전 번호 업데이트 (SpellingChecker.csproj, installer.iss)
# 2. 변경사항 커밋
git add .
git commit -m "Release v1.0.0"

# 3. 태그 생성 및 푸시
git tag v1.0.0
git push origin main
git push origin v1.0.0

# 4. GitHub Actions가 자동으로 릴리스 생성
```

### 수동 릴리스 / Manual Release

```bash
# 1. 빌드 스크립트 실행
.\build-release.ps1 -Version "1.0.0"

# 2. GitHub 릴리스 수동 생성
# - 웹 UI에서 릴리스 생성
# - 생성된 파일 업로드:
#   - installer/SpellingCheckerSetup_v1.0.0.exe
#   - SpellingChecker/bin/Release/net9.0-windows/win-x64/publish/SpellingChecker.exe
#   - README.md
```

## 릴리스 후 / Post-Release

- [ ] 릴리스 공지 작성 (SNS, 커뮤니티 등)
- [ ] 사용자 피드백 모니터링
- [ ] 이슈 트래커 확인
- [ ] 다음 버전 계획

## 긴급 패치 릴리스 / Hotfix Release

긴급 버그 수정이 필요한 경우:

1. 핫픽스 브랜치 생성: `git checkout -b hotfix/v1.0.1`
2. 버그 수정
3. 버전 번호를 패치 버전으로 업데이트 (예: 1.0.0 → 1.0.1)
4. 테스트
5. 메인 브랜치에 병합
6. 새 태그 생성 및 릴리스

## 롤백 절차 / Rollback Procedure

릴리스에 심각한 문제가 발견된 경우:

1. GitHub 릴리스 페이지에서 해당 릴리스를 "Pre-release"로 표시
2. 사용자에게 이전 버전 사용 권장 공지
3. 문제 수정 후 새 릴리스 배포

## 버전 번호 규칙 / Version Numbering

[Semantic Versioning](https://semver.org/) 사용:

- **MAJOR.MINOR.PATCH** (예: 1.2.3)
  - **MAJOR**: 호환되지 않는 API 변경 또는 주요 기능 추가
  - **MINOR**: 새로운 기능 추가, 하위 호환성 유지
  - **PATCH**: 버그 수정, 하위 호환성 유지

예시:
- `1.0.0` - 첫 릴리스
- `1.1.0` - 새 기능 추가
- `1.1.1` - 버그 수정
- `2.0.0` - 호환성 깨지는 변경

## 참고 문서 / References

- [DEPLOYMENT.md](DEPLOYMENT.md) - 전체 배포 가이드
- [BUILD.md](BUILD.md) - 빌드 가이드
- [CHANGELOG.md](CHANGELOG.md) - 버전 기록

---

**Note**: 이 체크리스트를 릴리스마다 복사하여 사용하세요.
