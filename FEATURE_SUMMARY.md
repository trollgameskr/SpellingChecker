# 변수명 추천 기능 구현 완료

## 개요
한글 텍스트를 C# 변수명으로 자동 변환하는 기능이 추가되었습니다.

## 기능 설명
- **단축키**: `Ctrl+Shift+Alt+V` (설정에서 변경 가능)
- **입력**: 클립보드에서 선택한 한글 텍스트
- **출력**: 3개의 C# 변수명 추천 (camelCase 형식)
- **AI 기반**: OpenAI API를 사용한 지능형 변수명 생성

## 사용 예시

### 예시 1: 간단한 설명
```
입력: 사용자 이름
출력:
1. userName
2. userFullName
3. accountName
```

### 예시 2: 복잡한 설명
```
입력: 데이터베이스 연결 상태
출력:
1. databaseConnectionStatus
2. dbConnectionState
3. connectionStatus
```

### 예시 3: 액션 중심
```
입력: 파일 다운로드하기
출력:
1. downloadFile
2. fileDownloader
3. downloadAction
```

## 사용 방법

### 1단계: 텍스트 선택
아무 프로그램에서나 한글 텍스트를 선택합니다.
- 메모장
- Visual Studio Code
- 브라우저
- Word 등

### 2단계: 단축키 누르기
`Ctrl+Shift+Alt+V`를 누릅니다.

### 3단계: 결과 확인
팝업 창에 3개의 변수명 추천이 표시됩니다.

### 4단계: 복사
"Copy to Clipboard" 버튼을 클릭하여 추천 목록을 복사합니다.

## 기술적 특징

### Microsoft C# 명명 규칙 준수
- ✅ camelCase 형식
- ✅ 의미있는 이름
- ✅ 적절한 길이
- ✅ 프로그래밍 용어 사용

### AI 프롬프트
```
시스템 메시지:
당신은 C# 프로그래밍 전문가입니다. 
한글 텍스트를 의미있는 영어 변수명으로 변환하는 것을 도와줍니다. 
Microsoft C# 명명 규칙을 따르며, camelCase를 사용합니다.

사용자 프롬프트:
다음 한글 텍스트를 C# 변수명 규칙에 맞게 3개의 변수명을 추천해주세요. 
각 변수명은 camelCase 형식이어야 하며, 의미가 명확해야 합니다.

텍스트: {입력한 텍스트}

각 변수명을 새 줄로 구분하여 반환하고, 설명이나 번호는 붙이지 마세요. 
변수명만 반환하세요.
```

### API 사용량
- **모델**: gpt-4o-mini (기본값)
- **토큰 사용량**: 요청당 약 50-150 토큰
- **비용**: 요청당 약 $0.0001-0.0005
- **응답 시간**: 1-3초

## 설정

### 단축키 변경하기
1. 시스템 트레이 아이콘 더블클릭
2. "Variable Name Suggestion Hotkey" 필드 찾기
3. 원하는 단축키 입력 (예: `Ctrl+Alt+N`)
4. "Save" 버튼 클릭
5. 프로그램 재시작

## 프로그램 시작 시 안내

프로그램을 시작하면 다음과 같은 알림이 표시됩니다:

```
프로그램이 시작되었습니다.
맞춤법 교정: Ctrl+Shift+Alt+Y
번역: Ctrl+Shift+Alt+T
변수명 추천: Ctrl+Shift+Alt+V  ← 새로 추가됨!
```

## 파일 변경 사항

### 코드 파일 (6개)
1. `SpellingChecker/Models/Models.cs` - 새 모델 추가
2. `SpellingChecker/Services/AIService.cs` - AI 서비스 메서드 추가
3. `SpellingChecker/Services/HotkeyService.cs` - 단축키 지원 추가
4. `SpellingChecker/MainWindow.xaml.cs` - 이벤트 핸들러 추가
5. `SpellingChecker/Views/SettingsWindow.xaml` - UI 컨트롤 추가
6. `SpellingChecker/Views/SettingsWindow.xaml.cs` - 설정 로직 추가

### 문서 파일 (4개)
1. `VARIABLE_NAME_SUGGESTION_FEATURE.md` - 기능 상세 문서
2. `VARIABLE_NAME_SUGGESTION_TEST_GUIDE.md` - 테스트 가이드 (15개 테스트 케이스)
3. `IMPLEMENTATION_SUMMARY_VARIABLE_NAME_SUGGESTION.md` - 구현 요약
4. `README.md` - 업데이트된 메인 문서

## 빌드 상태
✅ **빌드 성공**
- 컴파일 에러 없음
- 경고 없음
- .NET 9.0 호환

## 장점

### 1. 시간 절약
한글 설명을 직접 영어로 번역하고 변수명 규칙에 맞게 변환하는 시간을 절약합니다.

### 2. 일관된 명명
Microsoft C# 규칙을 자동으로 따르므로 코드 스타일이 일관됩니다.

### 3. 다양한 선택지
3가지 옵션을 제공하여 상황에 맞는 변수명을 선택할 수 있습니다.

### 4. 컨텍스트 이해
AI가 의미를 이해하고 적절한 변수명을 제안합니다.

### 5. 간편한 사용
단축키 하나로 즉시 사용할 수 있습니다.

## 향후 개선 가능 사항

### 단기 (v1.1)
- [ ] 다른 케이스 형식 지원 (PascalCase, snake_case)
- [ ] 클래스명, 메서드명 등 다양한 식별자 유형 지원

### 중기 (v1.2)
- [ ] 다른 프로그래밍 언어 지원 (Python, Java, JavaScript)
- [ ] 배치 처리 (여러 항목 동시 변환)
- [ ] 이전 제안 히스토리

### 장기 (v2.0)
- [ ] IDE 통합 (VS Code 확장, Visual Studio 플러그인)
- [ ] 코드 컨텍스트 인식 (주변 코드 분석)
- [ ] 팀 코드 스타일 규칙 적용
- [ ] 자주 사용하는 변수명 자동 완성

## 테스트

### 수동 테스트 필요
이 기능은 다음과 같은 이유로 수동 테스트가 필요합니다:
- Windows GUI 애플리케이션
- 글로벌 단축키 사용
- 클립보드 접근
- OpenAI API 실제 호출

### 테스트 가이드
자세한 테스트 절차는 `VARIABLE_NAME_SUGGESTION_TEST_GUIDE.md`를 참조하세요.

## 문제 해결

### 추천이 나타나지 않음
- OpenAI API 키가 설정되어 있는지 확인
- 인터넷 연결 확인
- 한글 텍스트를 선택했는지 확인

### 단축키가 작동하지 않음
- 다른 프로그램이 같은 단축키를 사용하는지 확인
- 설정에서 단축키 형식 확인
- 프로그램 재시작

### 추천이 마음에 들지 않음
- 더 구체적인 한글 텍스트 입력
- 설정에서 더 강력한 모델 사용 (gpt-4o)

## 지원

자세한 정보는 다음 문서를 참조하세요:
- [기능 상세 문서](VARIABLE_NAME_SUGGESTION_FEATURE.md)
- [테스트 가이드](VARIABLE_NAME_SUGGESTION_TEST_GUIDE.md)
- [구현 요약](IMPLEMENTATION_SUMMARY_VARIABLE_NAME_SUGGESTION.md)
- [메인 README](README.md)

## 라이선스
이 기능은 SpellingChecker 프로젝트의 라이선스를 따릅니다.

---

**구현 완료 날짜**: 2025년 10월 13일
**버전**: 1.0
**작성자**: GitHub Copilot
