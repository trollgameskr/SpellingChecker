# Tone Preset Feature - Testing Guide

## Overview

This guide provides step-by-step instructions for testing the tone preset feature in the SpellingChecker application.

## Prerequisites

- Windows 10 or later
- .NET 9.0 Runtime installed
- SpellingChecker application built and running
- Valid OpenAI API key configured

## Test Scenarios

### Scenario 1: First-Time User Experience

**Objective**: Verify default tone presets are initialized correctly

**Steps**:
1. Delete existing settings file (if any): `%APPDATA%\SpellingChecker\settings.json`
2. Launch the application
3. Double-click the system tray icon to open Settings
4. Navigate to the "문장 톤 프리셋" section

**Expected Results**:
- ✅ ComboBox should show 11 tone presets
- ✅ First preset should be "톤 없음" (selected by default)
- ✅ Description should display: "원문의 톤을 그대로 유지합니다."
- ✅ Edit and Delete buttons should be disabled (default preset selected)

---

### Scenario 2: Tone Selection and Description Update

**Objective**: Verify UI updates when different tones are selected

**Steps**:
1. Open Settings → "문장 톤 프리셋" section
2. Click on the ComboBox dropdown
3. Select "근엄한 팀장님 톤"
4. Observe the description text
5. Select "MZ세대 톤"
6. Observe the description text

**Expected Results**:
- ✅ Selecting "근엄한 팀장님 톤" displays: "권위 있고 엄격한 말투, 지시와 조언이 섞인 느낌."
- ✅ Selecting "MZ세대 톤" displays: "최신 유행어와 인터넷 밈을 섞은 가벼운 말투."
- ✅ Edit and Delete buttons remain disabled (default presets)

---

### Scenario 3: Spelling Correction with No Tone

**Objective**: Verify spelling correction works without tone (baseline)

**Test Input**: "안녕하세요 오늘 회의 자료 준비했어요"

**Steps**:
1. Open Settings → Select "톤 없음" → Save
2. Open any text editor (Notepad, Word, etc.)
3. Type the test input
4. Select the text
5. Press `Ctrl+Shift+Alt+Y`

**Expected Results**:
- ✅ Popup window appears with corrected text
- ✅ Text should be corrected for spelling/grammar only
- ✅ Original tone and style should be preserved
- ✅ Example result: "안녕하세요. 오늘 회의 자료를 준비했어요."

---

### Scenario 4: Spelling Correction with "근엄한 팀장님 톤"

**Objective**: Verify tone is applied during spelling correction

**Test Input**: "안녕하세요 오늘 회의 자료 준비했어요"

**Steps**:
1. Open Settings → Select "근엄한 팀장님 톤" → Save
2. Open any text editor
3. Type the test input
4. Select the text
5. Press `Ctrl+Shift+Alt+Y`

**Expected Results**:
- ✅ Popup window appears with corrected and tone-converted text
- ✅ Text should be more formal and authoritative
- ✅ Example result: "안녕하십니까. 오늘 회의 자료를 준비했습니다. 검토 바랍니다."

---

### Scenario 5: Spelling Correction with "MZ세대 톤"

**Objective**: Verify different tone produces different style

**Test Input**: "안녕하세요 오늘 회의 자료 준비했어요"

**Steps**:
1. Open Settings → Select "MZ세대 톤" → Save
2. Open any text editor
3. Type the test input
4. Select the text
5. Press `Ctrl+Shift+Alt+Y`

**Expected Results**:
- ✅ Popup window appears with corrected and tone-converted text
- ✅ Text should be casual with trendy expressions
- ✅ Example result: "안뇽! 오늘 회의 자료 준비 완료~ㅋㅋ 확인 ㄱㄱ"

---

### Scenario 6: Add Custom Tone Preset

**Objective**: Verify users can add custom tone presets

**Steps**:
1. Open Settings → "문장 톤 프리셋" section
2. Click "➕ 추가" button
3. Enter Name: "공식 이메일 톤"
4. Enter Description: "정중하고 격식 있는 비즈니스 이메일 말투"
5. Click "확인"

**Expected Results**:
- ✅ Dialog closes
- ✅ New preset appears in ComboBox
- ✅ New preset is automatically selected
- ✅ Description displays the entered text
- ✅ Edit and Delete buttons are now enabled (custom preset)

---

### Scenario 7: Edit Custom Tone Preset

**Objective**: Verify users can edit custom tone presets

**Steps**:
1. Ensure a custom preset exists (from Scenario 6)
2. Select the custom preset "공식 이메일 톤"
3. Click "✏️ 수정" button
4. Change Description to: "정중하고 격식 있는 비즈니스 이메일 말투, 존댓말 필수"
5. Click "확인"

**Expected Results**:
- ✅ Dialog closes
- ✅ Description updates to new text
- ✅ Changes are visible immediately
- ✅ Preset remains selected

---

### Scenario 8: Delete Custom Tone Preset

**Objective**: Verify users can delete custom tone presets

**Steps**:
1. Ensure a custom preset exists
2. Select the custom preset
3. Click "🗑️ 삭제" button
4. In the confirmation dialog, click "Yes"

**Expected Results**:
- ✅ Confirmation dialog appears
- ✅ Preset is removed from ComboBox
- ✅ Selection automatically changes to "톤 없음"
- ✅ Description updates accordingly

---

### Scenario 9: Cannot Edit Default Preset

**Objective**: Verify default presets are protected from editing

**Steps**:
1. Open Settings → "문장 톤 프리셋" section
2. Select any default preset (e.g., "근엄한 팀장님 톤")
3. Observe Edit and Delete buttons

**Expected Results**:
- ✅ Edit button is disabled
- ✅ Delete button is disabled
- ✅ Cannot modify default presets

---

### Scenario 10: Validation - Empty Name

**Objective**: Verify validation when adding preset with empty name

**Steps**:
1. Open Settings → Click "➕ 추가"
2. Leave Name field empty
3. Enter Description: "Test description"
4. Click "확인"

**Expected Results**:
- ✅ Warning dialog appears: "프리셋 이름을 입력해주세요."
- ✅ Dialog does not close
- ✅ No preset is created

---

### Scenario 11: Validation - Empty Description

**Objective**: Verify validation when adding preset with empty description

**Steps**:
1. Open Settings → Click "➕ 추가"
2. Enter Name: "Test Tone"
3. Leave Description field empty
4. Click "확인"

**Expected Results**:
- ✅ Warning dialog appears: "톤 설명을 입력해주세요."
- ✅ Dialog does not close
- ✅ No preset is created

---

### Scenario 12: Tone Persistence After Restart

**Objective**: Verify selected tone persists after application restart

**Steps**:
1. Open Settings → Select "싹싹한 신입 사원 톤" → Save
2. Close Settings window
3. Exit the application (right-click tray icon → Exit)
4. Restart the application
5. Open Settings → Check "문장 톤 프리셋" section

**Expected Results**:
- ✅ "싹싹한 신입 사원 톤" is still selected
- ✅ Description matches the selected tone
- ✅ Settings were persisted correctly

---

### Scenario 13: Custom Tone Persistence

**Objective**: Verify custom presets persist after restart

**Steps**:
1. Add a custom preset (e.g., "테스트 톤")
2. Save settings
3. Exit and restart the application
4. Open Settings → Check "문장 톤 프리셋" section

**Expected Results**:
- ✅ Custom preset "테스트 톤" is still in the list
- ✅ Can select and use the custom preset
- ✅ All custom presets are preserved

---

### Scenario 14: Long Text with Tone

**Objective**: Verify tone application works with longer text

**Test Input**: 
```
안녕하세요. 이번 프로젝트의 진행 상황을 보고드립니다.
현재까지 개발 진척도는 75% 정도이며, 예정된 일정대로 진행되고 있습니다.
다음 주까지 베타 테스트를 완료할 예정입니다.
```

**Steps**:
1. Open Settings → Select "드라마 재벌 2세 톤" → Save
2. Paste the test input in a text editor
3. Select the text
4. Press `Ctrl+Shift+Alt+Y`

**Expected Results**:
- ✅ Entire text is corrected and tone-converted
- ✅ Tone is applied consistently throughout
- ✅ Result should sound arrogant and luxurious

---

### Scenario 15: English Text with Tone

**Objective**: Verify tone application works with English text

**Test Input**: "Hello, I prepared the meeting materials today"

**Steps**:
1. Open Settings → Select "유행어 난발하는 예능인 톤" → Save
2. Type the test input in a text editor
3. Select the text
4. Press `Ctrl+Shift+Alt+Y`

**Expected Results**:
- ✅ English text is corrected
- ✅ Tone may or may not apply to English (depending on AI interpretation)
- ✅ No errors occur

---

### Scenario 16: Delete Selected Tone

**Objective**: Verify behavior when deleting the currently selected tone

**Steps**:
1. Add a custom preset "임시 톤"
2. Save settings (preset is now selected)
3. Click "🗑️ 삭제" and confirm
4. Observe the ComboBox selection

**Expected Results**:
- ✅ Preset is deleted
- ✅ Selection automatically changes to "톤 없음"
- ✅ No errors occur
- ✅ Spelling correction still works with default tone

---

### Scenario 17: Multiple Custom Presets

**Objective**: Verify multiple custom presets can coexist

**Steps**:
1. Add custom preset "톤1" with description "설명1"
2. Add custom preset "톤2" with description "설명2"
3. Add custom preset "톤3" with description "설명3"
4. Switch between them

**Expected Results**:
- ✅ All three custom presets appear in ComboBox
- ✅ Descriptions update correctly when switching
- ✅ All 11 default presets are still present
- ✅ Total presets = 11 (default) + 3 (custom) = 14

---

### Scenario 18: Cancel Add Dialog

**Objective**: Verify cancel button works in add dialog

**Steps**:
1. Open Settings → Click "➕ 추가"
2. Enter some name and description
3. Click "취소" button

**Expected Results**:
- ✅ Dialog closes
- ✅ No new preset is created
- ✅ ComboBox remains unchanged

---

### Scenario 19: Cancel Edit Dialog

**Objective**: Verify cancel button works in edit dialog

**Steps**:
1. Create a custom preset
2. Click "✏️ 수정"
3. Change the description
4. Click "취소" button

**Expected Results**:
- ✅ Dialog closes
- ✅ Changes are not saved
- ✅ Original description is preserved

---

### Scenario 20: Settings Window Size

**Objective**: Verify settings window displays all content properly

**Steps**:
1. Open Settings window
2. Observe all UI elements
3. Check if scrolling is needed

**Expected Results**:
- ✅ Window height: 800px (increased from 700px)
- ✅ Window width: 650px (increased from 600px)
- ✅ All tone preset UI elements are visible
- ✅ No overlapping elements
- ✅ No content is cut off

---

## Performance Tests

### Test P1: Tone Preset Load Time

**Steps**:
1. Create 20 custom tone presets
2. Restart the application
3. Open Settings window

**Expected Results**:
- ✅ Settings window opens in < 1 second
- ✅ All 31 presets (11 default + 20 custom) load quickly
- ✅ No lag when switching between presets

### Test P2: Spelling Correction Response Time

**Steps**:
1. Select any tone preset
2. Perform spelling correction on medium-length text (~100 words)
3. Measure time from hotkey press to result display

**Expected Results**:
- ✅ Response time similar to without tone (within 1-2 seconds difference)
- ✅ Tone application doesn't significantly slow down the process

---

## Error Handling Tests

### Test E1: Corrupted Settings File

**Steps**:
1. Locate `%APPDATA%\SpellingChecker\settings.json`
2. Open in text editor and corrupt the JSON (invalid syntax)
3. Save and restart the application

**Expected Results**:
- ✅ Application handles error gracefully
- ✅ Default tone presets are initialized
- ✅ No crash or error dialog

### Test E2: AI API Error with Tone

**Steps**:
1. Select a tone preset
2. Use invalid API key
3. Attempt spelling correction

**Expected Results**:
- ✅ Error is displayed to user
- ✅ Error message indicates API issue
- ✅ Application doesn't crash

---

## Regression Tests

### Test R1: Spelling Correction without Tone Still Works

**Objective**: Ensure original functionality is preserved

**Steps**:
1. Select "톤 없음"
2. Test spelling correction on various texts

**Expected Results**:
- ✅ Spelling correction works as before
- ✅ No tone is applied
- ✅ Original behavior is preserved

### Test R2: Translation Feature Unaffected

**Objective**: Ensure translation feature works independently

**Steps**:
1. Select any tone preset
2. Test translation (Ctrl+Shift+Alt+T)

**Expected Results**:
- ✅ Translation works normally
- ✅ Tone preset doesn't affect translation
- ✅ Translation is accurate

### Test R3: Hotkey Customization Still Works

**Objective**: Ensure hotkey settings are independent

**Steps**:
1. Change spelling correction hotkey
2. Select a tone preset
3. Test with new hotkey

**Expected Results**:
- ✅ New hotkey works
- ✅ Tone is applied correctly
- ✅ No conflicts

---

## Security Tests

### Test S1: Tone Preset Data Encryption

**Objective**: Verify tone presets are encrypted with settings

**Steps**:
1. Add custom tone presets
2. Save settings
3. Open `%APPDATA%\SpellingChecker\settings.json` in text editor

**Expected Results**:
- ✅ File content is encrypted (binary/unreadable)
- ✅ Cannot read tone preset data in plain text
- ✅ DPAPI encryption is working

---

## Test Summary Checklist

### Core Functionality
- [ ] Default 11 tone presets load correctly
- [ ] Tone selection updates description
- [ ] Spelling correction applies selected tone
- [ ] "톤 없음" preserves original tone
- [ ] Different tones produce different styles

### Custom Preset Management
- [ ] Can add custom tone presets
- [ ] Can edit custom tone presets
- [ ] Can delete custom tone presets
- [ ] Cannot edit default presets
- [ ] Cannot delete default presets

### Validation
- [ ] Empty name validation works
- [ ] Empty description validation works
- [ ] Cancel buttons work correctly

### Persistence
- [ ] Selected tone persists after restart
- [ ] Custom presets persist after restart
- [ ] Settings encryption includes tone data

### UI/UX
- [ ] Edit/Delete buttons enable/disable correctly
- [ ] ComboBox displays all presets
- [ ] Window size accommodates new UI elements
- [ ] No UI glitches or overlaps

### Integration
- [ ] Translation feature unaffected
- [ ] Hotkey customization works
- [ ] Usage statistics still track correctly
- [ ] No breaking changes to existing features

### Error Handling
- [ ] Corrupted settings handled gracefully
- [ ] API errors don't crash application
- [ ] Missing data initializes to defaults

---

## Test Results Template

```
Test Date: ___________
Tester: ___________
Build Version: ___________

| Scenario | Status | Notes |
|----------|--------|-------|
| Scenario 1 | ☐ Pass ☐ Fail | |
| Scenario 2 | ☐ Pass ☐ Fail | |
| Scenario 3 | ☐ Pass ☐ Fail | |
| ... | ... | |

Issues Found:
1. 
2. 
3. 

Overall Assessment: ☐ Ready for Release ☐ Needs Fixes
```

---

## Conclusion

This comprehensive test guide covers all aspects of the tone preset feature. Follow each scenario carefully and document any issues found. The feature should be tested on multiple Windows versions and configurations before release.
