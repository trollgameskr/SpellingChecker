# Variable Name Suggestion Feature - Test Guide

## Manual Testing Steps

### Prerequisites
1. Application is built and running
2. OpenAI API key is configured in Settings
3. System tray icon shows the application is running

### Test Case 1: Basic Variable Name Suggestion
**Objective:** Verify that Korean text is converted to C# variable names

**Steps:**
1. Open any text editor (Notepad, VS Code, etc.)
2. Type Korean text: `사용자 이름`
3. Select the text
4. Press `Ctrl+Shift+Alt+V`
5. Observe the popup window

**Expected Result:**
- Popup window appears with title "Variable Name Suggestions (C#)"
- Shows 3 variable name suggestions in camelCase format
- Examples: userName, userFullName, accountName, etc.
- Original text shows: `사용자 이름`

**Pass/Fail:** ___________

---

### Test Case 2: Multiple Word Description
**Objective:** Verify complex Korean descriptions are handled correctly

**Steps:**
1. Open any text editor
2. Type Korean text: `데이터베이스 연결 상태 확인`
3. Select the text
4. Press `Ctrl+Shift+Alt+V`
5. Observe the suggestions

**Expected Result:**
- 3 variable names are suggested
- Names are descriptive and follow camelCase
- Examples: databaseConnectionStatus, checkConnectionState, dbStatusChecker, etc.

**Pass/Fail:** ___________

---

### Test Case 3: Copy Functionality
**Objective:** Verify the copy button works correctly

**Steps:**
1. Perform Test Case 1 to get suggestions
2. Click "Copy to Clipboard" button
3. Open text editor
4. Press `Ctrl+V` to paste

**Expected Result:**
- All 3 suggestions are copied with line numbers
- Format: "1. variableName1\n2. variableName2\n3. variableName3"
- Can be pasted successfully

**Pass/Fail:** ___________

---

### Test Case 4: Settings Configuration
**Objective:** Verify hotkey can be configured in Settings

**Steps:**
1. Double-click system tray icon to open Settings
2. Locate "Variable Name Suggestion Hotkey" field
3. Verify default value is `Ctrl+Shift+Alt+V`
4. Change to different hotkey (e.g., `Ctrl+Alt+N`)
5. Click Save
6. Restart application
7. Try the new hotkey

**Expected Result:**
- Hotkey field is visible and editable
- New hotkey is saved and persisted
- New hotkey works after restart
- Old hotkey no longer triggers the feature

**Pass/Fail:** ___________

---

### Test Case 5: No Text Selected
**Objective:** Verify error handling when no text is selected

**Steps:**
1. Ensure no text is selected anywhere
2. Press `Ctrl+Shift+Alt+V`
3. Observe the notification

**Expected Result:**
- Notification appears: "No text selected"
- Message: "Please select some text to suggest variable names."
- No popup window appears

**Pass/Fail:** ___________

---

### Test Case 6: API Error Handling
**Objective:** Verify error handling when API fails

**Steps:**
1. Open Settings
2. Temporarily set an invalid API key or endpoint
3. Save settings
4. Select Korean text
5. Press `Ctrl+Shift+Alt+V`
6. Observe the error notification

**Expected Result:**
- Error notification appears
- Error message is descriptive
- Application doesn't crash

**Pass/Fail:** ___________

---

### Test Case 7: Progress Notifications
**Objective:** Verify progress notifications work correctly (if enabled)

**Steps:**
1. Open Settings
2. Enable "Show progress notifications"
3. Save settings
4. Select Korean text
5. Press `Ctrl+Shift+Alt+V`
6. Observe notifications

**Expected Result:**
- Notification appears: "변수명 추천 요청"
- Notification appears: "Processing..."
- Notification appears: "변수명 추천 완료" with suggestions

**Pass/Fail:** ___________

---

### Test Case 8: Various Korean Input Types
**Objective:** Verify different types of Korean input work correctly

**Test Scenarios:**

#### 8a: Single Word
- Input: `이름`
- Expected: Simple variable names like `name`, `displayName`, `fullName`
- Pass/Fail: ___________

#### 8b: Action Description
- Input: `파일 저장하기`
- Expected: Action-oriented names like `saveFile`, `fileSaver`, `saveAction`
- Pass/Fail: ___________

#### 8c: Status/State
- Input: `로그인 상태`
- Expected: State-oriented names like `loginStatus`, `isLoggedIn`, `authState`
- Pass/Fail: ___________

#### 8d: Counter/Number
- Input: `사용자 수`
- Expected: Count-oriented names like `userCount`, `numberOfUsers`, `totalUsers`
- Pass/Fail: ___________

#### 8e: Technical Term
- Input: `API 응답 시간`
- Expected: Technical names like `apiResponseTime`, `responseLatency`, `apiDuration`
- Pass/Fail: ___________

---

### Test Case 9: UI Display Format
**Objective:** Verify the suggestions are displayed correctly in the popup

**Steps:**
1. Trigger variable name suggestion
2. Observe the popup window layout

**Expected Result:**
- Original text is shown in the "Original:" section
- Result shows 3 suggestions in numbered format:
  ```
  1. variableName1
  2. variableName2
  3. variableName3
  ```
- Buttons: "Copy to Clipboard" and "Close" are visible
- Window is positioned near cursor
- Window stays on screen (not off-screen)

**Pass/Fail:** ___________

---

### Test Case 10: Startup Notification
**Objective:** Verify hotkey is shown in startup notification

**Steps:**
1. Close the application
2. Start the application
3. Observe the startup notification

**Expected Result:**
- Notification shows:
  ```
  프로그램이 시작되었습니다.
  맞춤법 교정: Ctrl+Shift+Alt+Y
  번역: Ctrl+Shift+Alt+T
  변수명 추천: Ctrl+Shift+Alt+V
  ```

**Pass/Fail:** ___________

---

## Performance Testing

### Test Case 11: Response Time
**Objective:** Measure response time for suggestions

**Steps:**
1. Select Korean text
2. Press hotkey
3. Measure time until popup appears

**Expected Result:**
- Response time: 1-5 seconds (depending on model and internet speed)
- Using gpt-4o-mini: ~1-3 seconds
- Using gpt-4o: ~3-5 seconds

**Actual Response Time:** ___________ seconds

**Pass/Fail:** ___________

---

### Test Case 12: Token Usage
**Objective:** Verify token usage is reasonable

**Steps:**
1. Open Usage Statistics window
2. Note current token count
3. Perform 5 variable name suggestions
4. Check Usage Statistics again

**Expected Result:**
- Each suggestion uses approximately 50-150 tokens total
- Cost per suggestion: ~$0.0001-0.0005 (with gpt-4o-mini)
- Usage is recorded with operation type "VariableNameSuggestion"

**Pass/Fail:** ___________

---

## Edge Cases

### Test Case 13: Very Long Input
**Input:** `사용자 데이터베이스 테이블의 고유 식별자 값을 저장하는 변수`

**Expected Result:**
- Suggestions are still reasonable length
- Names are simplified but meaningful
- Examples: `userDatabaseId`, `uniqueIdentifierValue`, `userTableKey`

**Pass/Fail:** ___________

---

### Test Case 14: English Input (Edge Case)
**Input:** `user name`

**Expected Result:**
- Still provides valid suggestions
- AI understands English input
- Examples: `userName`, `userFullName`, `accountName`

**Pass/Fail:** ___________

---

### Test Case 15: Mixed Korean/English
**Input:** `User 이름`

**Expected Result:**
- Handles mixed input gracefully
- Suggestions are valid C# variable names
- Examples: `userName`, `userDisplayName`, `accountName`

**Pass/Fail:** ___________

---

## Summary

**Total Test Cases:** 15
**Passed:** _____
**Failed:** _____
**Skipped:** _____

**Notes:**
_____________________________________________________________________________
_____________________________________________________________________________
_____________________________________________________________________________

**Tester Name:** _________________________
**Date:** _________________________
**Version:** _________________________
