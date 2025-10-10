# Usage Statistics Feature - Testing Guide

This document provides guidance for testing the new usage statistics and history feature.

## Features to Test

### 1. Usage Recording
- [ ] Perform a spelling correction (Ctrl+Shift+Alt+Y)
- [ ] Perform a translation (Ctrl+Shift+Alt+T)
- [ ] Check that usage is recorded (open Usage Statistics window)

### 2. Usage Statistics Window
- [ ] Open Settings window (double-click tray icon or right-click → Settings)
- [ ] Click "📊 View Usage Statistics" button
- [ ] Verify window opens successfully

### 3. Statistics Display
- [ ] Verify total operations count is correct
- [ ] Verify corrections count is correct
- [ ] Verify translations count is correct
- [ ] Verify token counts are displayed
- [ ] Verify total cost is calculated and displayed

### 4. Period Filtering
- [ ] Select "All Time" - should show all records
- [ ] Select "Today" - should show today's records only
- [ ] Select "This Week" - should show this week's records
- [ ] Select "This Month" - should show this month's records

### 5. History Grid
- [ ] Verify records are displayed in the DataGrid
- [ ] Verify records are sorted by timestamp (most recent first)
- [ ] Verify all columns are visible:
  - Date/Time
  - Type (Correction/Translation)
  - Model (e.g., gpt-4o-mini)
  - Prompt Tokens
  - Completion Tokens
  - Total Tokens
  - Cost (USD)

### 6. Clear History
- [ ] Click "Clear History" button
- [ ] Verify confirmation dialog appears
- [ ] Click "Yes" to confirm
- [ ] Verify all records are cleared
- [ ] Verify statistics are reset to zero

### 7. Cost Calculation
- [ ] Verify costs are calculated correctly for different models
- [ ] Test with gpt-4o-mini (default)
- [ ] If available, test with gpt-4o
- [ ] Verify cost format shows 6 decimal places

### 8. Data Persistence
- [ ] Perform several operations
- [ ] Close the application
- [ ] Reopen the application
- [ ] Verify usage history is retained
- [ ] Check file at `%APPDATA%\SpellingChecker\usage_history.json`

## Expected Behavior

### Cost Calculation Formula
```
Input Cost = (Prompt Tokens / 1,000,000) × Input Price
Output Cost = (Completion Tokens / 1,000,000) × Output Price
Total Cost = Input Cost + Output Cost
```

### Model Pricing (as of implementation)
- gpt-4o-mini: $0.15 input, $0.60 output per 1M tokens
- gpt-4o: $5.00 input, $15.00 output per 1M tokens
- gpt-3.5-turbo: $0.50 input, $1.50 output per 1M tokens

## Test Scenarios

### Scenario 1: First-Time User
1. Fresh installation
2. No usage history
3. Open Usage Statistics window
4. Should show all zeros
5. Empty history grid

### Scenario 2: Regular Usage
1. Perform 5 corrections
2. Perform 3 translations
3. Open Usage Statistics
4. Should show:
   - Total Operations: 8
   - Corrections: 5
   - Translations: 3
   - Appropriate token counts
   - Calculated costs

### Scenario 3: Period Filtering
1. Perform operations on different days (if possible)
2. Filter by "Today"
3. Should only show today's operations
4. Switch to "All Time"
5. Should show all operations

### Scenario 4: Data Persistence
1. Perform some operations
2. Note the statistics
3. Close application completely
4. Reopen application
5. Open Usage Statistics
6. Verify statistics match previous values

## Files Modified/Added

### New Files
- `SpellingChecker/Services/UsageService.cs`
- `SpellingChecker/Views/UsageStatisticsWindow.xaml`
- `SpellingChecker/Views/UsageStatisticsWindow.xaml.cs`

### Modified Files
- `SpellingChecker/Models/Models.cs` - Added UsageRecord and UsageStatistics classes
- `SpellingChecker/Services/AIService.cs` - Added usage tracking
- `SpellingChecker/Views/SettingsWindow.xaml` - Added statistics button
- `SpellingChecker/Views/SettingsWindow.xaml.cs` - Added button handler

### Documentation Updates
- `README.md` - Added usage statistics feature description
- `CHANGELOG.md` - Added to unreleased features
- `CONFIG.md` - Added usage statistics configuration guide

## Known Limitations

1. Usage tracking starts from the time this feature is deployed
2. Historical data before this update is not available
3. Costs are estimates based on current pricing (subject to change by OpenAI)
4. Usage data is stored locally and not backed up automatically

## Troubleshooting

### Issue: Statistics window doesn't open
- Check for exceptions in error logs
- Verify all XAML files are properly formatted

### Issue: Usage not being recorded
- Check if `%APPDATA%\SpellingChecker\usage_history.json` is being created
- Verify file permissions
- Check if UsageService is being instantiated correctly

### Issue: Incorrect cost calculations
- Verify pricing constants in UsageService.cs match OpenAI's current rates
- Check if the model name matches the pricing dictionary keys

### Issue: History cleared unexpectedly
- Check if usage_history.json file still exists
- Verify file is not being deleted by antivirus or cleanup tools
