# Sample Responses Feature - Visual Example

## Before (기존)

```
┌────────────────────────────────────────────────┐
│ Common Question Answer                         │
├────────────────────────────────────────────────┤
│ Original:                                       │
│ ┌────────────────────────────────────────────┐ │
│ │ Python과 JavaScript 중 어떤 것을 배워야    │ │
│ │ 할까요?                                     │ │
│ └────────────────────────────────────────────┘ │
│                                                 │
│ Result:                                         │
│ ┌────────────────────────────────────────────┐ │
│ │ Python과 JavaScript는 각각 장단점이 있습   │ │
│ │ 니다. Python은 데이터 과학과 머신러닝에     │ │
│ │ 강점이 있고, JavaScript는 웹 개발에        │ │
│ │ 필수적입니다.                               │ │
│ │ 어떤 분야에 관심이 있으신가요?             │ │
│ └────────────────────────────────────────────┘ │
│                                                 │
│  [Copy to Clipboard] [변환] [Close]            │
└────────────────────────────────────────────────┘
```

## After (개선 후)

```
┌────────────────────────────────────────────────┐
│ Common Question Answer                         │
├────────────────────────────────────────────────┤
│ Original:                                       │
│ ┌────────────────────────────────────────────┐ │
│ │ Python과 JavaScript 중 어떤 것을 배워야    │ │
│ │ 할까요?                                     │ │
│ └────────────────────────────────────────────┘ │
│                                                 │
│ Result:                                         │
│ ┌────────────────────────────────────────────┐ │
│ │ Python과 JavaScript는 각각 장단점이 있습   │ │
│ │ 니다. Python은 데이터 과학과 머신러닝에     │ │
│ │ 강점이 있고, JavaScript는 웹 개발에        │ │
│ │ 필수적입니다.                               │ │
│ │ 어떤 분야에 관심이 있으신가요?             │ │
│ └────────────────────────────────────────────┘ │
│                                                 │
│ 💡 응답 샘플 (클릭하여 사용):                  │
│ ┌────────────────────────────────────────────┐ │
│ │ 저는 웹 개발에 관심이 있어서 프론트엔드와  │ │ ← Clickable
│ │ 백엔드 모두 다룰 수 있는 언어를 배우고     │ │   (hover effect)
│ │ 싶습니다.                                   │ │
│ └────────────────────────────────────────────┘ │
│ ┌────────────────────────────────────────────┐ │
│ │ 데이터 분석과 인공지능 분야에 관심이       │ │ ← Clickable
│ │ 있어서 Python을 먼저 배우고 싶습니다.      │ │   (hover effect)
│ └────────────────────────────────────────────┘ │
│                                                 │
│  [Copy to Clipboard] [변환] [Close]            │
└────────────────────────────────────────────────┘
```

## User Interaction Flow

1. **User asks a question** → AI answers with a follow-up question
2. **System detects** the question in AI's answer
3. **AI generates** 2 contextual sample responses
4. **User clicks** one of the sample responses
5. **System automatically**:
   - Fills the sample text into Original textbox
   - Triggers re-conversion (as if user pressed Ctrl+Enter)
6. **AI responds** to the selected sample, continuing the conversation

## Visual Design

- **Header**: 💡 emoji + "응답 샘플 (클릭하여 사용):"
- **Sample boxes**:
  - Light blue background (#E3F2FD)
  - Blue border (#2196F3)
  - Rounded corners (4px)
  - Padding: 10px
  - Cursor changes to hand pointer
  - Hover effect: Background becomes lighter (#BBDEFB)
  - Text color: Dark blue (#1976D2)
