# AutoHotkey 버전 빠른 시작 가이드

## 1단계: AutoHotkey v2.0 설치

### 다운로드
1. https://www.autohotkey.com/ 접속
2. "Download" 버튼 클릭
3. **AutoHotkey v2.0** 다운로드 (v1.1이 아님!)

### 설치
1. 다운로드한 설치 파일 실행
2. 설치 마법사 따라 진행
3. 기본 설정으로 설치 완료

## 2단계: 스크립트 실행

### 방법 1: 직접 실행
1. `SpellingChecker.ahk` 파일 찾기
2. 파일을 더블클릭하여 실행
3. 시스템 트레이에 AutoHotkey 아이콘 나타남

### 방법 2: 바탕화면 바로가기 생성
1. `SpellingChecker.ahk` 파일 우클릭
2. "바로가기 만들기" 선택
3. 바로가기를 바탕화면으로 이동
4. 바로가기를 더블클릭하여 실행

## 3단계: API 키 설정

1. 시스템 트레이의 AutoHotkey 아이콘 우클릭
2. "Settings" 메뉴 선택
3. "API Key" 필드에 OpenAI API 키 입력
4. "Save" 버튼 클릭

## 4단계: 사용 시작!

### 맞춤법 교정
1. 아무 텍스트나 선택 (메모장, 워드, 브라우저 등)
2. `Ctrl+Shift+Alt+Y` 누르기
3. 팝업 창에서 교정 결과 확인
4. "Copy to Clipboard" 또는 "Replace" 선택

### 번역
1. 텍스트 선택
2. `Ctrl+Shift+Alt+T` 누르기
3. 번역 결과 확인

### 변수명 추천
1. 한글 텍스트 선택 (예: "사용자 이름")
2. `Ctrl+Shift+Alt+V` 누르기
3. C# 변수명 추천 확인

## Windows 시작 시 자동 실행 설정

1. `Win+R` 누르기
2. `shell:startup` 입력하고 Enter
3. 열린 폴더에 `SpellingChecker.ahk` 바로가기 복사
4. 다음 부팅부터 자동으로 실행됨

## 문제 해결

### "AutoHotkey v2.0이 설치되지 않음" 오류
- AutoHotkey v2.0을 설치했는지 확인
- v1.1이 아닌 **v2.0**을 설치해야 함

### 스크립트 실행 시 오류 발생
- `.ahk` 파일 연결 프로그램 확인
- 우클릭 → "연결 프로그램" → AutoHotkey v2.0 선택

### 단축키가 작동하지 않음
- 다른 프로그램과 단축키 충돌 확인
- Settings에서 다른 단축키로 변경

### API 키 오류
- OpenAI API 키가 올바른지 확인
- API 키 앞뒤 공백 확인
- OpenAI 계정에 충분한 크레딧이 있는지 확인

## 스크립트 종료

### 방법 1: 트레이 메뉴
1. 시스템 트레이의 AutoHotkey 아이콘 우클릭
2. "Exit" 선택

### 방법 2: 작업 관리자
1. `Ctrl+Shift+Esc` 눌러 작업 관리자 열기
2. "AutoHotkey" 프로세스 찾기
3. 우클릭 → "작업 끝내기"

## 추가 정보

- 자세한 사용 방법은 `README_AUTOHOTKEY.md` 참조
- C# WPF 버전 정보는 `README.md` 참조
- 문제 발생 시 GitHub Issues에 문의

## OpenAI API 키 발급 방법

1. https://platform.openai.com/ 접속
2. 계정 생성 또는 로그인
3. "API Keys" 메뉴 선택
4. "Create new secret key" 클릭
5. 생성된 키 복사 (한 번만 표시됨!)
6. SpellingChecker Settings에 붙여넣기

**주의**: API 사용은 유료입니다. 요금제 확인 필수!

## 유용한 팁

### 단축키 변경
Settings 창에서 단축키를 원하는 조합으로 변경 가능:
- `Ctrl+Alt+S` (맞춤법)
- `Ctrl+Alt+T` (번역)
- `Ctrl+Alt+V` (변수명)

### 톤 프리셋 활용
Settings의 "Selected Tone"에서 다양한 말투 선택:
- 근엄한 팀장님 톤: 공식 문서용
- MZ세대 톤: 친구들과 대화용
- 드라마 재벌 2세 톤: 재미있는 글쓰기용

### 빠른 실행
1. 바탕화면에 바로가기 생성
2. 바로가기 우클릭 → 속성
3. "바로가기 키" 설정 (예: `Ctrl+Alt+S`)
4. 이제 어디서든 단축키로 스크립트 실행/중지 가능

## C# 버전과 비교

| 기능 | AutoHotkey | C# WPF |
|------|-----------|--------|
| 설치 | ⭐⭐⭐⭐⭐ 쉬움 | ⭐⭐⭐ 보통 |
| 메모리 사용 | ⭐⭐⭐⭐⭐ 적음 | ⭐⭐⭐ 보통 |
| 보안 | ⭐⭐⭐ 보통 | ⭐⭐⭐⭐⭐ 높음 |
| UI 품질 | ⭐⭐⭐ 기본 | ⭐⭐⭐⭐⭐ 우수 |
| 커스터마이징 | ⭐⭐⭐⭐⭐ 쉬움 | ⭐⭐⭐ 보통 |

**추천**:
- 빠르고 간편하게 사용: AutoHotkey 버전
- 보안과 UI가 중요: C# WPF 버전
