# AutoHotkey 버전 테스트 가이드

## 기본 테스트

AutoHotkey가 설치되어 있지 않은 환경에서는 스크립트 구조와 문법만 검증할 수 있습니다.

### 1. 스크립트 구조 검증

```bash
# 파일 존재 확인
ls -l SpellingChecker.ahk

# 파일 크기 확인 (약 30KB)
wc -c SpellingChecker.ahk

# 줄 수 확인 (약 800+ 줄)
wc -l SpellingChecker.ahk

# AutoHotkey v2.0 지시문 확인
head -10 SpellingChecker.ahk | grep "Requires AutoHotkey v2.0"
```

### 2. 주요 함수 확인

```bash
# 주요 함수 존재 확인
grep -E "^[A-Za-z]+\(" SpellingChecker.ahk | head -20
```

### 3. 설정 파일 예제 확인

```bash
# 설정 파일 예제 존재 확인
cat settings.ini.example
```

## Windows 환경에서 실제 테스트

### 사전 준비
1. AutoHotkey v2.0 설치
2. OpenAI API 키 준비

### 테스트 1: 기본 실행
1. `SpellingChecker.ahk` 더블클릭
2. 시스템 트레이에 아이콘 나타나는지 확인
3. 트레이 아이콘 우클릭 → 메뉴 확인

**예상 결과:**
- ✅ 스크립트가 실행됨
- ✅ 트레이 아이콘 표시
- ✅ Settings, Exit 메뉴 표시

### 테스트 2: 설정 저장
1. 트레이 아이콘 → Settings
2. API 키 입력
3. Save 클릭
4. `%APPDATA%\SpellingChecker\settings.ini` 파일 생성 확인

**예상 결과:**
- ✅ 설정 창 표시
- ✅ 설정 파일 생성
- ✅ API 키 저장됨

### 테스트 3: 맞춤법 교정
1. 메모장 열기
2. "안녕하세요 저는 테스트입니다" 입력
3. 텍스트 선택
4. `Ctrl+Shift+Alt+Y` 누르기
5. 결과 팝업 확인

**예상 결과:**
- ✅ 팝업 창 표시
- ✅ 교정된 텍스트 표시
- ✅ Copy/Replace 버튼 작동

### 테스트 4: 번역
1. 메모장에 "Hello, how are you?" 입력
2. 텍스트 선택
3. `Ctrl+Shift+Alt+T` 누르기
4. 번역 결과 확인

**예상 결과:**
- ✅ 한국어로 번역됨
- ✅ "안녕하세요, 어떻게 지내세요?" 등

### 테스트 5: 변수명 추천
1. 메모장에 "사용자 이름" 입력
2. 텍스트 선택
3. `Ctrl+Shift+Alt+V` 누르기
4. 변수명 추천 확인

**예상 결과:**
- ✅ 3개의 camelCase 변수명 표시
- ✅ userName, userFullName 등

### 테스트 6: 톤 프리셋
1. Settings → Selected Tone → "MZ세대 톤" 선택
2. Save
3. 맞춤법 교정 실행
4. 결과가 MZ세대 톤으로 변환되었는지 확인

**예상 결과:**
- ✅ 톤이 적용된 결과 표시
- ✅ 캐주얼한 표현 사용

### 테스트 7: 사용 통계
1. Settings → Usage Statistics 클릭
2. 작업 기록 확인

**예상 결과:**
- ✅ 통계 창 표시
- ✅ 작업 카운트 표시

## 자동화된 구문 검증 (Linux/WSL)

```bash
#!/bin/bash

echo "=== AutoHotkey Script Validation ==="

# 1. File exists
if [ -f "SpellingChecker.ahk" ]; then
    echo "✓ SpellingChecker.ahk exists"
else
    echo "✗ SpellingChecker.ahk not found"
    exit 1
fi

# 2. Check AutoHotkey version requirement
if grep -q "#Requires AutoHotkey v2.0" SpellingChecker.ahk; then
    echo "✓ AutoHotkey v2.0 requirement found"
else
    echo "✗ Missing AutoHotkey v2.0 requirement"
fi

# 3. Check essential functions
FUNCTIONS=(
    "InitializeApp"
    "LoadSettings"
    "SaveSettings"
    "RegisterHotkeys"
    "OnSpellingCorrectionRequested"
    "OnTranslationRequested"
    "OnVariableNameSuggestionRequested"
    "GetSelectedText"
    "CorrectSpellingAsync"
    "TranslateAsync"
    "SuggestVariableNamesAsync"
    "ShowResultPopup"
    "ShowSettingsWindow"
    "ShowUsageStatisticsWindow"
)

for func in "${FUNCTIONS[@]}"; do
    if grep -q "^${func}(" SpellingChecker.ahk; then
        echo "✓ Function $func found"
    else
        echo "✗ Function $func missing"
    fi
done

# 4. Check global variables
GLOBALS=(
    "g_ApiKey"
    "g_ApiEndpoint"
    "g_Model"
    "g_SpellingHotkey"
    "g_TranslationHotkey"
    "g_VariableNameHotkey"
    "g_TonePresets"
)

for var in "${GLOBALS[@]}"; do
    if grep -q "global $var" SpellingChecker.ahk; then
        echo "✓ Global variable $var found"
    else
        echo "✗ Global variable $var missing"
    fi
done

# 5. Check tone presets
PRESETS=(
    "default-none"
    "default-serious-manager"
    "default-eager-newbie"
    "default-mz"
)

for preset in "${PRESETS[@]}"; do
    if grep -q "$preset" SpellingChecker.ahk; then
        echo "✓ Tone preset $preset found"
    else
        echo "✗ Tone preset $preset missing"
    fi
done

echo ""
echo "=== Validation Complete ==="
```

## 체크리스트

### 스크립트 구조
- [ ] AutoHotkey v2.0 지시문
- [ ] 전역 변수 선언
- [ ] 초기화 함수
- [ ] 설정 관리 함수
- [ ] 단축키 핸들러
- [ ] API 통합 함수
- [ ] GUI 함수

### 핵심 기능
- [ ] 맞춤법 교정 작동
- [ ] 번역 작동
- [ ] 변수명 추천 작동
- [ ] 설정 저장/로드
- [ ] 톤 프리셋 적용
- [ ] 사용 통계 추적

### UI/UX
- [ ] 시스템 트레이 아이콘
- [ ] 설정 창
- [ ] 결과 팝업
- [ ] 통계 창
- [ ] 알림 메시지

### 파일
- [ ] SpellingChecker.ahk
- [ ] README_AUTOHOTKEY.md
- [ ] QUICKSTART_AUTOHOTKEY.md
- [ ] settings.ini.example
- [ ] AUTOHOTKEY_MIGRATION_REPORT.md
- [ ] VERSION_COMPARISON.md

## 알려진 제한사항

1. **JSON 파싱**: 간단한 정규식 기반 파싱 사용
2. **에러 처리**: 기본 수준의 try-catch
3. **API 키 보안**: INI 파일에 평문 저장
4. **UI**: 기본 GUI 컴포넌트 사용

## 문제 해결

### "AutoHotkey가 설치되지 않음"
```
해결: https://www.autohotkey.com/ 에서 v2.0 설치
```

### "API 호출 실패"
```
확인사항:
1. API 키가 올바른가?
2. 인터넷 연결이 되어 있는가?
3. OpenAI 크레딧이 있는가?
```

### "단축키가 작동하지 않음"
```
확인사항:
1. 다른 프로그램과 충돌?
2. Settings에서 다른 조합으로 변경
```

## 성공 기준

✅ 모든 핵심 기능이 C# WPF 버전과 동일하게 작동
✅ 메모리 사용량이 C# 버전보다 적음
✅ 설치와 사용이 간편함
✅ 코드 수정이 쉬움
✅ 문서가 완비됨
