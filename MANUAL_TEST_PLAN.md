# Manual Testing Guide for Usage Statistics Fix

## Issue Fixed
**Problem:** Clicking "📊 View Usage Statistics" button caused `NullReferenceException` crash
**Fix:** Added null check in `PeriodComboBox_SelectionChanged` event handler

## Test Environment Setup
- Windows 10 or later
- Build the project in Debug or Release mode
- Ensure you have some usage data (perform a few corrections/translations) OR test with empty state

## Test Cases

### Test Case 1: Open Statistics Window (Primary Fix Verification)
**Objective:** Verify the window opens without crashing

**Steps:**
1. Launch the SpellingChecker application
2. Double-click the system tray icon OR right-click → "Settings"
3. In the Settings window, scroll down if needed
4. Click the "📊 View Usage Statistics" button

**Expected Results:**
- ✅ Usage Statistics window opens successfully
- ✅ No exception or error message appears
- ✅ Window displays with all UI elements visible
- ✅ "All Time" is pre-selected in the period filter dropdown
- ✅ Statistics show correct data (or zeros if no usage yet)

**If Failed:**
- ❌ Window crashes with exception → Fix didn't work
- ❌ Window shows but data is wrong → Different issue

---

### Test Case 2: Default Filter Selection
**Objective:** Verify default "All Time" filter is selected and works

**Steps:**
1. Open Usage Statistics window (as in Test Case 1)
2. Observe the "Filter by Period" dropdown

**Expected Results:**
- ✅ "All Time" is selected by default
- ✅ Statistics show all historical data
- ✅ History grid shows all records

---

### Test Case 3: Filter Changes
**Objective:** Verify period filtering works correctly after initialization

**Steps:**
1. Open Usage Statistics window
2. Click the "Filter by Period" dropdown
3. Select "Today"
4. Verify statistics update
5. Select "This Week"
6. Verify statistics update
7. Select "This Month"
8. Verify statistics update
9. Select "All Time" again
10. Verify statistics revert to showing all data

**Expected Results:**
- ✅ Each filter change updates the statistics immediately
- ✅ "Today" shows only today's operations
- ✅ "This Week" shows this week's operations (Monday-Sunday)
- ✅ "This Month" shows current month's operations
- ✅ "All Time" shows all operations
- ✅ History grid updates with filtered records
- ✅ No exceptions occur during filter changes

---

### Test Case 4: Empty State
**Objective:** Verify behavior with no usage data

**Steps:**
1. Clear all usage history (if any exists):
   - Open Usage Statistics window
   - Click "Clear History" button
   - Confirm deletion
2. Close and reopen Usage Statistics window

**Expected Results:**
- ✅ Window opens without crash
- ✅ All statistics show "0"
- ✅ Total cost shows "$0.000000"
- ✅ History grid is empty
- ✅ Period filter still works (just shows zeros for all periods)

---

### Test Case 5: With Usage Data
**Objective:** Verify behavior with actual usage data

**Prerequisites:**
- Perform some operations before testing:
  - Press Ctrl+Shift+Alt+Y (spelling correction) 2-3 times
  - Press Ctrl+Shift+Alt+T (translation) 2-3 times

**Steps:**
1. Open Usage Statistics window
2. Verify all statistics show correct numbers
3. Verify history grid shows individual records
4. Test period filtering with actual data

**Expected Results:**
- ✅ "Total Operations" = Corrections + Translations
- ✅ "Corrections" count is accurate
- ✅ "Translations" count is accurate
- ✅ Token counts are > 0
- ✅ Total cost is calculated correctly
- ✅ History grid shows individual rows with timestamps
- ✅ Records are sorted by most recent first

---

### Test Case 6: Window Lifecycle
**Objective:** Verify window can be opened, closed, and reopened

**Steps:**
1. Open Usage Statistics window
2. Click "Close" button
3. Reopen Usage Statistics window
4. Close by clicking the X button
5. Reopen Usage Statistics window
6. Close by pressing Alt+F4
7. Reopen Usage Statistics window

**Expected Results:**
- ✅ Window opens successfully each time
- ✅ Data is consistent across reopens
- ✅ No memory leaks or performance degradation
- ✅ All close methods work correctly

---

### Test Case 7: Clear History Function
**Objective:** Verify clearing history works and doesn't crash

**Steps:**
1. Open Usage Statistics window (with some data)
2. Click "Clear History" button
3. Confirm the deletion in the dialog
4. Observe the results

**Expected Results:**
- ✅ Confirmation dialog appears
- ✅ After confirming, all statistics reset to 0
- ✅ History grid becomes empty
- ✅ Success message appears
- ✅ Window remains open and functional
- ✅ Period filter still works (shows zeros)

---

## Regression Testing

### Areas to Check for Regressions
1. **Settings Window**
   - ✅ Opens normally
   - ✅ All other buttons work
   - ✅ Settings can be saved
   
2. **Main Application**
   - ✅ Spelling correction still works (Ctrl+Shift+Alt+Y)
   - ✅ Translation still works (Ctrl+Shift+Alt+T)
   - ✅ Usage is still recorded
   
3. **System Tray**
   - ✅ Icon shows correctly
   - ✅ Menu works
   - ✅ Settings option works

---

## Performance Testing

### Test Opening Statistics Window Multiple Times
**Steps:**
1. Open Usage Statistics window
2. Close it
3. Repeat 10-20 times

**Expected Results:**
- ✅ Each open is fast (< 1 second)
- ✅ No noticeable slowdown
- ✅ Memory usage stays reasonable

---

## Edge Cases

### Test Case: Very Large Dataset
**Setup:** Import or generate hundreds of usage records

**Expected Results:**
- ✅ Window still opens quickly
- ✅ Filtering works on large dataset
- ✅ Grid scrolls smoothly
- ✅ No crashes or hangs

### Test Case: Corrupted Data File
**Setup:** Manually corrupt the usage_history.json file

**Expected Results:**
- ✅ Window opens (doesn't crash)
- ✅ Shows empty state or default values
- ✅ Handles error gracefully

---

## Test Results Template

### Tester Information
- Tester Name: _______________
- Date: _______________
- Build Version: _______________
- OS: Windows _______________

### Test Results Summary

| Test Case | Status | Notes |
|-----------|--------|-------|
| TC1: Open Statistics Window | ⬜ Pass ⬜ Fail | |
| TC2: Default Filter Selection | ⬜ Pass ⬜ Fail | |
| TC3: Filter Changes | ⬜ Pass ⬜ Fail | |
| TC4: Empty State | ⬜ Pass ⬜ Fail | |
| TC5: With Usage Data | ⬜ Pass ⬜ Fail | |
| TC6: Window Lifecycle | ⬜ Pass ⬜ Fail | |
| TC7: Clear History Function | ⬜ Pass ⬜ Fail | |

### Overall Result: ⬜ PASS ⬜ FAIL

### Issues Found:
(List any issues discovered during testing)

---

### Sign-off
- ⬜ All test cases passed
- ⬜ No regressions detected
- ⬜ Ready for release

Tester Signature: _______________ Date: _______________
