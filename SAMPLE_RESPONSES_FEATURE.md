# 응답 샘플 제공 기능 (Sample Response Feature)

## 개요 (Overview)

CommonQuestion 기능 사용 시, AI의 답변에 사용자에게 질문이 포함되어 있을 경우, 자동으로 2가지 응답 샘플을 생성하여 제공합니다.

When using the CommonQuestion feature, if the AI's answer contains a question for the user, the system automatically generates and provides 2 sample responses.

## 기능 설명 (Features)

### 1. 자동 질문 감지 (Automatic Question Detection)

AI의 답변에서 다음과 같은 패턴을 감지합니다:
- 물음표 (`?` 또는 `?`)
- 한국어 질문 패턴:
  - "어떻게 생각하시나요"
  - "어떻게 생각하세요"
  - "원하시나요"
  - "원하세요"
  - "궁금하신가요"
  - "어떤가요"
  - "무엇인가요"
  - 기타 질문 패턴

### 2. 응답 샘플 생성 (Sample Response Generation)

질문이 감지되면:
- 컨텍스트에 맞는 2가지 적절한 응답을 자동 생성
- 다양한 관점의 응답 제공
- 자연스러운 한국어 응답

### 3. 사용자 인터페이스 (User Interface)

응답 샘플은 결과 창 하단에 표시됩니다:
- 💡 아이콘과 함께 "응답 샘플 (클릭하여 사용)" 표시
- 2개의 클릭 가능한 파란색 박스로 표시
- 마우스 호버 시 색상 변경 (시각적 피드백)

### 4. 샘플 사용 방법 (How to Use Samples)

1. 원하는 응답 샘플을 클릭
2. 자동으로 Original 텍스트 박스에 샘플 텍스트가 입력됨
3. 자동으로 재변환 실행 (Ctrl+Enter와 동일한 효과)
4. AI가 선택한 응답에 대해 새로운 답변 제공

## 사용 예시 (Usage Example)

### 시나리오 (Scenario)

**사용자 질문:**
```
Python과 JavaScript 중 어떤 것을 배워야 할까요?
```

**AI 답변:**
```
Python과 JavaScript는 각각 장단점이 있습니다. 
Python은 데이터 과학과 머신러닝에 강점이 있고, 
JavaScript는 웹 개발에 필수적입니다. 
어떤 분야에 관심이 있으신가요?
```

**제공되는 응답 샘플:**
```
1. 저는 웹 개발에 관심이 있어서 프론트엔드와 백엔드 모두 다룰 수 있는 언어를 배우고 싶습니다.

2. 데이터 분석과 인공지능 분야에 관심이 있어서 Python을 먼저 배우고 싶습니다.
```

사용자는 원하는 응답을 클릭하여 계속 대화를 이어갈 수 있습니다.

## 기술적 구현 (Technical Implementation)

### 1. 모델 변경 (Model Changes)
- `CommonQuestionResult`에 `SampleResponses` 속성 추가

### 2. 서비스 로직 (Service Logic)
- `AIService.AnswerCommonQuestionAsync`: 질문 감지 및 샘플 생성
- `AIService.ContainsQuestionForUser`: 질문 패턴 감지
- `AIService.GenerateSampleResponsesAsync`: AI를 사용한 샘플 응답 생성

### 3. UI 업데이트 (UI Updates)
- `ResultPopupWindow.xaml`: 샘플 응답 표시 UI 추가
- `ResultPopupWindow.xaml.cs`: 샘플 응답 표시 및 클릭 처리
- `MainWindow.xaml.cs`: CommonQuestion 처리 시 샘플 응답 전달

## 주의사항 (Notes)

1. **API 사용량**: 샘플 응답 생성을 위해 추가 API 호출이 발생합니다.
2. **오류 처리**: 샘플 생성 실패 시에도 메인 기능은 정상 작동합니다.
3. **언어 지원**: 현재 한국어 질문 패턴에 최적화되어 있습니다.
4. **비동기 처리**: 샘플 생성은 비동기로 처리되어 UX를 해치지 않습니다.

## 향후 개선 사항 (Future Improvements)

- [ ] 영어 질문 패턴 추가 지원
- [ ] 샘플 응답 개수 사용자 설정 가능
- [ ] 샘플 응답 캐싱으로 API 사용량 최적화
- [ ] 사용자 정의 응답 템플릿 지원
