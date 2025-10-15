# SpellingChecker - AutoHotkey Version

AI 기반 맞춤법 교정 및 영한 번역을 제공하는 AutoHotkey 스크립트입니다.

## 주요 기능

### 1. 맞춤법 교정 (Ctrl+Shift+Alt+Y)
- 선택한 텍스트를 AI 기반으로 맞춤법 및 문법 교정
- **문장 톤 프리셋 적용**: 교정과 동시에 다양한 말투로 변환
- 교정된 결과를 팝업 창으로 표시
- 클립보드에 복사하거나 원본 텍스트 교체 가능

### 2. 영한/한영 번역 (Ctrl+Shift+Alt+T)
- 선택한 텍스트의 언어를 자동으로 감지
- 한국어 → 영어 또는 영어 → 한국어로 자동 번역
- 번역 결과를 팝업 창으로 표시
- 클립보드에 복사하거나 원본 텍스트 교체 가능

### 3. 변수명 추천 (Ctrl+Shift+Alt+V)
- 한글로 입력된 클립보드 내용을 C# 변수명으로 변환
- 3개의 변수명을 추천
- camelCase 형식의 C# 명명 규칙 준수
- Microsoft C# Coding Conventions 준수

### 4. 시스템 트레이 통합
- 백그라운드에서 실행
- 시스템 트레이 아이콘으로 접근
- 설정 창을 통한 API 키 및 환경 설정

### 5. 사용 통계 및 히스토리
- API 사용량 추적
- 교정 및 번역 작업 기록
- 사용 내역 상세 조회

### 6. 문장 톤 프리셋
- **11가지 기본 톤 프리셋 제공**:
  - 톤 없음 (원문 유지)
  - 근엄한 팀장님 톤
  - 싹싹한 신입 사원 톤
  - MZ세대 톤
  - 심드렁한 알바생 톤
  - 유난히 예의 바른 경비원 톤
  - 오버하는 홈쇼핑 쇼호스트 톤
  - 유행어 난발하는 예능인 톤
  - 100년 된 할머니 톤
  - 드라마 재벌 2세 톤
  - 외국인 한국어 학습자 톤

## 시스템 요구사항

- **운영체제**: Windows 10 이상
- **AutoHotkey**: AutoHotkey v2.0 이상
- **API**: OpenAI API 키 필요

## 설치 및 실행

### 1. AutoHotkey v2.0 설치

1. [AutoHotkey v2.0 다운로드](https://www.autohotkey.com/)
2. 설치 프로그램 실행하여 AutoHotkey v2.0 설치

### 2. 스크립트 실행

1. `SpellingChecker.ahk` 파일을 더블클릭하여 실행
2. 시스템 트레이에 아이콘이 나타납니다

### 3. 초기 설정

1. 시스템 트레이 아이콘을 우클릭 → "Settings" 선택
2. OpenAI API 키를 입력합니다
3. 필요시 API 엔드포인트 및 모델을 변경합니다
4. "Save" 버튼을 클릭하여 설정을 저장합니다

### 4. 자동 시작 설정 (선택사항)

Windows 시작 시 자동으로 실행되도록 설정:

1. `Win+R` 키를 눌러 실행 창 열기
2. `shell:startup` 입력하고 Enter
3. `SpellingChecker.ahk` 파일의 바로가기를 해당 폴더에 복사

## 사용 방법

### 맞춤법 교정
1. 교정할 텍스트를 선택합니다 (메모장, 워드, 브라우저 등)
2. `Ctrl+Shift+Alt+Y`를 누릅니다
3. 교정 결과가 팝업 창에 표시됩니다
4. "Copy to Clipboard" 버튼으로 결과를 복사하거나 "Replace" 버튼으로 원본 텍스트를 교체합니다

**문장 톤 변경하기**:
1. 시스템 트레이 아이콘 → "Settings" 선택
2. "Selected Tone" 드롭다운에서 원하는 톤 선택
3. "Save" 버튼 클릭
4. 맞춤법 교정 사용 시 선택한 톤이 자동 적용됩니다

### 번역
1. 번역할 텍스트를 선택합니다
2. `Ctrl+Shift+Alt+T`를 누릅니다
3. 번역 결과가 팝업 창에 표시됩니다
4. "Copy to Clipboard" 버튼으로 결과를 복사하거나 "Replace" 버튼으로 원본 텍스트를 교체합니다

### 변수명 추천
1. 변수명으로 변환할 한글 텍스트를 선택합니다 (예: "사용자 이름")
2. `Ctrl+Shift+Alt+V`를 누릅니다
3. 3개의 C# 변수명 추천이 팝업 창에 표시됩니다 (예: userName, userFullName, accountName)
4. "Copy to Clipboard" 버튼으로 추천 목록을 복사합니다

### 사용 통계 확인
1. 시스템 트레이 아이콘 → "Settings" 선택
2. "📊 Usage Statistics" 버튼 클릭
3. 사용 내역 확인

## 설정 파일

설정은 다음 위치에 저장됩니다:
- 설정 파일: `%APPDATA%\SpellingChecker\settings.ini`
- 사용 통계: `%APPDATA%\SpellingChecker\usage.json`

## 단축키 커스터마이징

Settings 창에서 단축키를 변경할 수 있습니다:

**유효한 단축키 예시:**
- `Ctrl+Shift+Alt+Y` (기본값)
- `Ctrl+Alt+N`
- `Win+Shift+V`
- `Ctrl+Shift+F1`

## C# WPF 버전과의 차이점

### AutoHotkey 버전의 장점:
- ✅ 설치가 간단함 (단일 스크립트 파일)
- ✅ 메모리 사용량이 적음
- ✅ 코드 수정이 쉬움 (텍스트 편집기로 직접 수정 가능)
- ✅ .NET 프레임워크 불필요
- ✅ 빠른 실행 속도

### AutoHotkey 버전의 제한사항:
- ⚠️ API 키가 INI 파일에 평문으로 저장됨 (C# 버전은 DPAPI로 암호화)
- ⚠️ UI가 상대적으로 단순함
- ⚠️ JSON 파싱이 제한적 (간단한 수동 파싱)

## 보안 주의사항

AutoHotkey 버전은 API 키를 INI 파일에 평문으로 저장합니다. 보안이 중요한 환경에서는 C# WPF 버전 사용을 권장합니다.

설정 파일 권한 설정:
1. `%APPDATA%\SpellingChecker` 폴더로 이동
2. `settings.ini` 파일 우클릭 → 속성
3. 보안 탭에서 불필요한 사용자 권한 제거

## 문제 해결

### 스크립트가 실행되지 않는 경우
- AutoHotkey v2.0이 설치되어 있는지 확인
- `.ahk` 파일 연결 프로그램이 AutoHotkey v2.0인지 확인

### 단축키가 작동하지 않는 경우
- 다른 프로그램과 단축키 충돌 확인
- 관리자 권한으로 실행 시도

### API 호출 실패
- API 키가 올바른지 확인
- 인터넷 연결 상태 확인
- OpenAI API 할당량 확인

## 스크립트 구조

```
SpellingChecker.ahk
├── Global Variables          # 전역 변수 선언
├── Initialization           # 초기화 및 설정 로드
├── Tone Preset Management   # 톤 프리셋 관리
├── Settings Management      # 설정 파일 읽기/쓰기
├── Tray Icon Setup         # 시스템 트레이 아이콘
├── Hotkey Registration     # 전역 단축키 등록
├── Hotkey Handlers         # 단축키 이벤트 처리
├── Clipboard Service       # 클립보드 및 텍스트 캡처
├── AI Service              # OpenAI API 통합
├── Usage Tracking          # 사용량 추적
├── Result Popup Window     # 결과 팝업 창
├── Settings Window         # 설정 창
├── Usage Statistics Window # 사용 통계 창
└── Utility Functions       # 유틸리티 함수
```

## 코드 수정

스크립트를 텍스트 편집기로 열어 직접 수정할 수 있습니다:

### 기본 단축키 변경
```autohotkey
global g_SpellingHotkey := "^+!y"  ; Ctrl+Shift+Alt+Y
global g_TranslationHotkey := "^+!t"  ; Ctrl+Shift+Alt+T
global g_VariableNameHotkey := "^+!v"  ; Ctrl+Shift+Alt+V
```

### 기본 모델 변경
```autohotkey
global g_Model := "gpt-4o-mini"
```

### 사용자 정의 톤 프리셋 추가
```autohotkey
g_TonePresets["custom-my-tone"] := {
    Id: "custom-my-tone",
    Name: "나만의 톤",
    Description: "내가 원하는 말투 설명",
    IsDefault: false
}
```

## 라이선스

MIT License

## 기여

이슈 리포트 및 풀 리퀘스트를 환영합니다!

## 관련 파일

- `SpellingChecker.ahk` - 메인 스크립트
- `README_AUTOHOTKEY.md` - 본 문서
- C# WPF 버전은 `SpellingChecker/` 폴더 참조

## 문의

문제가 발생하거나 제안사항이 있으시면 GitHub Issues를 통해 알려주세요.
