# Sample Responses Feature - Test Scenarios

## Test Environment
- OS: Windows 10+
- Framework: .NET 9.0
- AI Provider: OpenAI, Anthropic, or Gemini

## Test Scenario 1: Basic Question Detection

### Test Case 1.1: Question with Question Mark
**Input Question:**
```
오늘 날씨가 어떤가요?
```

**Expected AI Answer:**
```
오늘 날씨는 맑고 화창합니다. 기온은 20도 정도입니다. 
외출 계획이 있으신가요?
```

**Expected Behavior:**
- ✅ Question detected (contains "?")
- ✅ 2 sample responses generated
- ✅ Sample responses displayed in UI

**Sample Responses Should Be Like:**
```
1. 네, 오후에 친구들과 공원에 가려고 합니다.
2. 아니요, 오늘은 집에서 쉴 예정입니다.
```

### Test Case 1.2: Question with Korean Pattern
**Input Question:**
```
Python 공부법 알려줘
```

**Expected AI Answer:**
```
Python 공부는 다음과 같이 할 수 있습니다:
1. 공식 문서 읽기
2. 프로젝트 만들기
3. 온라인 강의 듣기

어떤 방법이 더 적합하다고 생각하세요?
```

**Expected Behavior:**
- ✅ Question detected (contains "생각하세요")
- ✅ 2 sample responses generated
- ✅ Sample responses displayed in UI

## Test Scenario 2: No Question Detection

### Test Case 2.1: Statement Only
**Input Question:**
```
Python 공부법 알려줘
```

**Expected AI Answer:**
```
Python 공부는 다음과 같이 할 수 있습니다:
1. 공식 문서를 읽으며 기본 문법을 익히세요
2. 작은 프로젝트를 만들어보세요
3. 온라인 강의를 들어보세요
```

**Expected Behavior:**
- ✅ No question detected
- ✅ No sample responses generated
- ✅ Sample responses panel hidden

## Test Scenario 3: User Interaction

### Test Case 3.1: Click Sample Response 1
**Steps:**
1. Ask a question that triggers sample responses
2. Wait for AI answer and sample responses to appear
3. Click on the first sample response box

**Expected Behavior:**
- ✅ Sample text copied to Original textbox
- ✅ Conversion automatically triggered (same as Ctrl+Enter)
- ✅ Progress indicator shows "처리 중..."
- ✅ AI generates new response to the sample answer
- ✅ New response shown in Result box

### Test Case 3.2: Click Sample Response 2
**Steps:**
1. Ask a question that triggers sample responses
2. Wait for AI answer and sample responses to appear
3. Click on the second sample response box

**Expected Behavior:**
- ✅ Sample text copied to Original textbox
- ✅ Conversion automatically triggered
- ✅ AI generates new response to the sample answer

### Test Case 3.3: Hover Effect
**Steps:**
1. Ask a question that triggers sample responses
2. Move mouse over first sample response box
3. Move mouse away from sample response box

**Expected Behavior:**
- ✅ Background color changes to lighter blue (#BBDEFB) on hover
- ✅ Background color returns to original (#E3F2FD) on mouse leave
- ✅ Cursor changes to hand pointer on hover

## Test Scenario 4: Edge Cases

### Test Case 4.1: Empty Question
**Input:**
```
(empty or whitespace only)
```

**Expected Behavior:**
- ✅ No API call made
- ✅ Notification shown: "No text selected"

### Test Case 4.2: API Timeout
**Scenario:** Network issue or slow AI response

**Expected Behavior:**
- ✅ Progress indicator shown
- ✅ Timeout error caught
- ✅ User notified: "Request Timeout"
- ✅ UI remains functional

### Test Case 4.3: Sample Generation Fails
**Scenario:** Sample response generation API call fails

**Expected Behavior:**
- ✅ Main answer still displayed correctly
- ✅ Sample responses panel hidden (empty array)
- ✅ No error shown to user
- ✅ Feature degrades gracefully

## Test Scenario 5: Multiple Languages

### Test Case 5.1: English Question
**Input Question:**
```
How do I learn Python?
```

**Expected AI Answer:**
```
You can learn Python through:
1. Reading official documentation
2. Building projects
3. Taking online courses

Which method do you prefer?
```

**Expected Behavior:**
- ✅ Question detected (contains "?")
- ✅ 2 sample responses generated (should be in English)
- ✅ Sample responses displayed

### Test Case 5.2: Mixed Korean-English
**Input Question:**
```
Python과 Java의 차이점이 뭔가요?
```

**Expected AI Answer:**
```
Python과 Java의 주요 차이점:
1. Python은 동적 타입, Java는 정적 타입
2. Python은 간결한 문법, Java는 verbose
3. Python은 인터프리터, Java는 컴파일

어떤 프로젝트를 진행하실 예정인가요?
```

**Expected Behavior:**
- ✅ Question detected
- ✅ Sample responses in Korean (matching context)

## Test Scenario 6: Integration with Other Features

### Test Case 6.1: After Tone Change
**Steps:**
1. Use spelling correction with tone preset
2. Switch to CommonQuestion mode
3. Ask a question

**Expected Behavior:**
- ✅ Sample responses work independently
- ✅ No interference from tone preset setting
- ✅ Tone preset panel hidden in CommonQuestion mode

### Test Case 6.2: Multiple Reconversions
**Steps:**
1. Ask question → Get answer with samples
2. Click sample 1 → Get new answer
3. Manually edit Original text
4. Press Ctrl+Enter → Get new answer

**Expected Behavior:**
- ✅ Each reconversion works correctly
- ✅ New sample responses generated each time if applicable
- ✅ Previous samples replaced with new ones

## Performance Tests

### Test Case P.1: Response Time
**Metric:** Time from question submission to sample display

**Acceptable Range:**
- Question detection: < 100ms (instant)
- Sample generation: 2-5 seconds (depends on AI provider)
- UI update: < 100ms (instant)

### Test Case P.2: API Token Usage
**Metric:** Token consumption per question with samples

**Expected:**
- Main answer: ~500-1000 tokens
- Sample generation: ~200-300 tokens
- Total: ~700-1300 tokens per question (with samples)

### Test Case P.3: Memory Usage
**Metric:** Memory footprint increase

**Expected:**
- Minimal increase (< 5MB)
- No memory leaks after multiple uses

## Accessibility Tests

### Test Case A.1: Keyboard Navigation
**Test:**
- Can user tab through sample response boxes?
- Can user activate sample with Enter/Space?

**Expected:**
- ✅ Tab navigation works (or clarify it's mouse-only)
- 🔲 Keyboard activation (future enhancement)

### Test Case A.2: Screen Reader
**Test:**
- Are sample responses announced by screen reader?
- Is the purpose of clickable boxes clear?

**Expected:**
- ✅ Text content is readable
- 🔲 ARIA labels (future enhancement)

## Regression Tests

### Test Case R.1: Existing Features Unchanged
**Verify:**
- ✅ Spelling correction still works
- ✅ Translation still works
- ✅ Variable name suggestion still works
- ✅ Tone presets still work
- ✅ Hotkeys still work
- ✅ Copy to clipboard still works
- ✅ Replace text still works

### Test Case R.2: Settings Persistence
**Verify:**
- ✅ API keys still saved/loaded correctly
- ✅ Hotkey settings still saved/loaded
- ✅ Tone preset selections still saved/loaded

## Known Limitations

1. **Language Support**: Korean question patterns are more comprehensive than English
2. **API Costs**: Each question with samples uses ~30% more tokens
3. **Network Dependency**: Sample generation requires additional API call
4. **Response Quality**: Sample quality depends on AI model used

## Future Test Scenarios

- [ ] Test with very long AI answers (>1000 characters)
- [ ] Test with multiple questions in one answer
- [ ] Test with non-question interrogative sentences
- [ ] Test with rhetorical questions
- [ ] Test offline behavior (no API connection)
- [ ] Test with different AI providers (OpenAI, Anthropic, Gemini)
