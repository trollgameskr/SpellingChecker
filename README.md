# SpellingChecker - AI 문장 맞춤법 교정 및 영한 번역 윈도우 프로그램

AI 기반 맞춤법 교정 및 한영/영한 번역을 제공하는 Windows 데스크탑 애플리케이션입니다.

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
- API 사용량 추적 (토큰 수, 비용 계산)
- 교정 및 번역 작업 기록
- 기간별 통계 조회 (오늘, 이번 주, 이번 달, 전체)
- 사용 내역 상세 조회 및 관리

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
- **커스텀 톤 프리셋 관리**:
  - 사용자 정의 톤 추가
  - 커스텀 톤 수정 및 삭제
  - 원하는 톤 선택하여 맞춤법 교정에 적용

## 시스템 요구사항

- **운영체제**: Windows 10 이상
- **프레임워크**: .NET 9.0
- **API**: 다음 중 하나의 AI 제공자 API 키 필요
  - OpenAI API 키
  - Anthropic API 키
  - Google Gemini API 키

## 설치 및 실행

### 빌드 방법

1. 저장소 클론:
```bash
git clone https://github.com/shinepcs/SpellingChecker.git
cd SpellingChecker
```

2. 프로젝트 빌드:
```bash
cd SpellingChecker
dotnet build
```

3. 실행:
```bash
dotnet run
```

또는 Visual Studio에서 `SpellingChecker.sln` 파일을 열고 빌드/실행합니다.

### 초기 설정

1. 애플리케이션을 실행하면 시스템 트레이에 아이콘이 나타납니다
2. 트레이 아이콘을 더블클릭하거나 우클릭 → "Settings"를 선택
3. AI Provider를 선택합니다 (OpenAI, Anthropic, 또는 Gemini)
4. 선택한 제공자의 API 키를 입력합니다
5. 원하는 AI 모델을 선택합니다 (기본값 권장)
6. 필요시 API 엔드포인트를 변경합니다 (자동 설정됨)
7. "Save" 버튼을 클릭하여 설정을 저장합니다

## 사용 방법

### 맞춤법 교정
1. 교정할 텍스트를 선택합니다 (메모장, 워드, 브라우저 등)
2. `Ctrl+Shift+Alt+Y`를 누릅니다
3. 교정 결과가 팝업 창에 표시됩니다
4. "Copy to Clipboard" 버튼으로 결과를 복사하거나 "Replace" 버튼으로 원본 텍스트를 교체합니다

**문장 톤 변경하기**:
1. 시스템 트레이 아이콘 → "Settings" 선택
2. "문장 톤 프리셋" 섹션에서 원하는 톤 선택
3. 설정을 저장하고 맞춤법 교정 사용 시 선택한 톤이 자동 적용됩니다

**커스텀 톤 추가하기**:
1. Settings → "문장 톤 프리셋" 섹션
2. "➕ 추가" 버튼 클릭
3. 톤 이름과 설명 입력 (예: "공식 보고서 톤", "격식 있고 전문적인 말투")
4. 확인 버튼 클릭하여 저장

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
5. 자세한 사용법은 [VARIABLE_NAME_SUGGESTION_FEATURE.md](VARIABLE_NAME_SUGGESTION_FEATURE.md) 참조

### 사용 통계 확인
1. 시스템 트레이 아이콘 → "Settings" 선택
2. "📊 View Usage Statistics" 버튼 클릭
3. 토큰 사용량, 비용, 작업 내역 확인
4. 기간별 필터링 (오늘, 이번 주, 이번 달, 전체)
5. 필요시 사용 내역 삭제 가능

## 기술 스택

- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C# / .NET 9.0
- **AI API**: 
  - OpenAI GPT (gpt-4o-mini, gpt-4o, gpt-3.5-turbo)
  - Anthropic Claude (claude-3-5-sonnet, claude-3-5-haiku, claude-3-opus)
  - Google Gemini (gemini-2.0-flash-exp, gemini-1.5-pro, gemini-1.5-flash)
- **보안**: Windows Data Protection API를 사용한 API 키 암호화
- **JSON**: Newtonsoft.Json

## 프로젝트 구조

```
SpellingChecker/
├── SpellingChecker/
│   ├── Models/               # 데이터 모델
│   │   └── Models.cs
│   ├── Services/             # 비즈니스 로직
│   │   ├── AIService.cs      # AI API 통신
│   │   ├── ClipboardService.cs  # 클립보드 및 텍스트 선택
│   │   ├── HotkeyService.cs  # 전역 단축키
│   │   ├── SettingsService.cs   # 설정 관리
│   │   ├── TonePresetService.cs # 톤 프리셋 관리
│   │   └── UsageService.cs   # 사용 통계 추적
│   ├── Views/                # UI 창
│   │   ├── ResultPopupWindow.xaml/cs  # 결과 팝업
│   │   ├── SettingsWindow.xaml/cs     # 설정 창
│   │   ├── TonePresetDialog.xaml/cs   # 톤 프리셋 대화상자
│   │   └── UsageStatisticsWindow.xaml/cs  # 사용 통계 창
│   ├── App.xaml/cs           # 애플리케이션 진입점
│   ├── MainWindow.xaml/cs    # 메인 윈도우 (백그라운드)
│   └── SpellingChecker.csproj
└── README.md
```

## 보안

- API 키는 Windows Data Protection API (DPAPI)를 사용하여 암호화되어 저장됩니다
- 사용자별로 암호화되어 다른 사용자는 접근할 수 없습니다
- 설정 파일 위치: `%APPDATA%\SpellingChecker\settings.json`

## 향후 계획

- [ ] 인스톨러 제작 (WiX 또는 Inno Setup)
- [ ] 자동 업데이트 기능
- [ ] 추가 언어 지원 (일본어, 중국어 등)
- [ ] 오프라인 모델 지원
- [ ] 단축키 커스터마이징
- [ ] 변경 사항 하이라이팅
- [ ] 사용 통계 및 히스토리

## 라이선스

MIT License

## 기여

이슈 리포트 및 풀 리퀘스트를 환영합니다!

## 문의

문제가 발생하거나 제안사항이 있으시면 GitHub Issues를 통해 알려주세요.
