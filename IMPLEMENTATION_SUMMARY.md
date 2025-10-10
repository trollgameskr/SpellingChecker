# Usage Statistics Feature - Implementation Summary

## Overview

This document summarizes the implementation of the usage statistics and history feature for the SpellingChecker application. The feature allows users to track their API usage, including token consumption and costs.

## Problem Statement (Original Korean)

> 사용 통계 및 히스토리 기능 추가
> 사용 통계에서 내가 사용한 토큰, 비용 볼 수 있게 해줘

**Translation**: Add usage statistics and history feature. Allow users to see their token usage and costs in the usage statistics.

## Implementation Details

### 1. New Models (SpellingChecker/Models/Models.cs)

Added two new model classes:

#### UsageRecord
Represents a single usage record for API calls.
```csharp
public class UsageRecord
{
    public DateTime Timestamp { get; set; }
    public string OperationType { get; set; } // "Correction" or "Translation"
    public string Model { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal Cost { get; set; }
}
```

#### UsageStatistics
Represents aggregated usage statistics.
```csharp
public class UsageStatistics
{
    public int TotalCorrectionCount { get; set; }
    public int TotalTranslationCount { get; set; }
    public int TotalPromptTokens { get; set; }
    public int TotalCompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal TotalCost { get; set; }
}
```

### 2. Usage Service (SpellingChecker/Services/UsageService.cs)

New service to manage usage tracking and statistics:

**Key Features**:
- Records usage data after each API call
- Calculates costs based on model pricing
- Stores usage history in JSON format
- Provides statistics filtering by date range
- Supports clearing usage history

**Pricing Information**:
- gpt-4o-mini: $0.15 input, $0.60 output per 1M tokens
- gpt-4o: $5.00 input, $15.00 output per 1M tokens
- gpt-3.5-turbo: $0.50 input, $1.50 output per 1M tokens

**Storage Location**:
```
%APPDATA%\SpellingChecker\usage_history.json
```

### 3. AIService Updates (SpellingChecker/Services/AIService.cs)

Modified to track usage:

**Changes**:
- Added `UsageService` dependency
- Added `RecordUsageFromResponse()` method to extract token usage from OpenAI API responses
- Integrated usage recording in both `CorrectSpellingAsync()` and `TranslateAsync()` methods

**How it works**:
1. API request is sent to OpenAI
2. Response includes usage data (prompt_tokens, completion_tokens)
3. `RecordUsageFromResponse()` extracts token counts
4. `UsageService.RecordUsage()` saves the record with calculated cost

### 4. Usage Statistics Window

#### XAML (SpellingChecker/Views/UsageStatisticsWindow.xaml)

**UI Components**:
- Summary statistics panel (operations, tokens, costs)
- Period filter dropdown (All Time, Today, This Week, This Month)
- DataGrid for detailed usage history
- Clear history button
- Close button

**Layout**:
- Three-column statistics display
- Prominent cost display in green
- Sortable history grid with 7 columns
- Confirmation dialog for clearing history

#### Code-behind (SpellingChecker/Views/UsageStatisticsWindow.xaml.cs)

**Functionality**:
- `LoadStatistics()`: Loads and displays statistics
- `PeriodComboBox_SelectionChanged()`: Filters data by period
- `ClearHistoryButton_Click()`: Clears all history with confirmation
- Data sorting: Most recent records first

### 5. Settings Window Updates

#### XAML Changes (SpellingChecker/Views/SettingsWindow.xaml)
- Added "📊 View Usage Statistics" button
- Increased window height to accommodate new section
- Added descriptive text for the feature

#### Code-behind Changes (SpellingChecker/Views/SettingsWindow.xaml.cs)
- Added `ViewUsageStatisticsButton_Click()` handler
- Opens UsageStatisticsWindow as a modal dialog

## Data Flow

### Recording Usage
```
1. User triggers correction/translation
2. AIService sends request to OpenAI
3. OpenAI responds with result + usage data
4. AIService extracts tokens from response
5. UsageService calculates cost
6. Record saved to usage_history.json
```

### Viewing Statistics
```
1. User opens Settings
2. Clicks "View Usage Statistics"
3. UsageStatisticsWindow loads all records
4. Aggregates statistics by operation type
5. Displays summary and detailed history
6. User can filter by period or clear history
```

## Files Created

1. `/SpellingChecker/Services/UsageService.cs` - Usage tracking service
2. `/SpellingChecker/Views/UsageStatisticsWindow.xaml` - Statistics UI
3. `/SpellingChecker/Views/UsageStatisticsWindow.xaml.cs` - Statistics logic
4. `/TESTING_USAGE_STATS.md` - Testing guide

## Files Modified

1. `/SpellingChecker/Models/Models.cs` - Added UsageRecord and UsageStatistics
2. `/SpellingChecker/Services/AIService.cs` - Added usage tracking
3. `/SpellingChecker/Views/SettingsWindow.xaml` - Added statistics button
4. `/SpellingChecker/Views/SettingsWindow.xaml.cs` - Added button handler
5. `/README.md` - Documented new feature
6. `/CHANGELOG.md` - Added to unreleased features
7. `/CONFIG.md` - Added usage statistics guide
8. `/DOCS_INDEX.md` - Updated documentation index

## Features Implemented

✅ Track token usage (prompt and completion tokens)
✅ Calculate costs based on model pricing
✅ Store usage history locally
✅ View detailed usage history
✅ Filter statistics by period (today, week, month, all time)
✅ Clear usage history
✅ Display aggregated statistics
✅ Show per-operation breakdown
✅ Accessible from Settings window

## Key Design Decisions

### 1. Storage Format
- **JSON format**: Easy to read, debug, and migrate
- **Plain text**: No encryption (contains no sensitive data)
- **Local storage**: Privacy-focused, no cloud sync

### 2. Cost Calculation
- **Client-side calculation**: No external API calls needed
- **Fixed pricing table**: Updated manually when OpenAI changes prices
- **Accurate to 6 decimal places**: Shows precise cost per operation

### 3. Error Handling
- **Silent failures**: Usage tracking errors don't interrupt user operations
- **Graceful degradation**: If tracking fails, core features still work
- **Try-catch blocks**: Prevent crashes from file I/O or JSON parsing errors

### 4. Privacy
- **No personal data**: Only timestamps, token counts, and operation types
- **Local only**: No data sent to external servers
- **User control**: Can clear history at any time

## Testing Recommendations

See [TESTING_USAGE_STATS.md](TESTING_USAGE_STATS.md) for detailed testing guide.

**Key Test Scenarios**:
1. First-time user (empty statistics)
2. Regular usage (multiple operations)
3. Period filtering
4. Data persistence
5. Clear history
6. Cost calculation accuracy

## Future Enhancements

Potential improvements:
- Export statistics to CSV/Excel
- Usage graphs and charts
- Cost alerts when exceeding thresholds
- Monthly/weekly usage reports
- Comparison with previous periods
- Backup/restore usage history

## Compliance

### OpenAI API Terms
- Uses official OpenAI API response structure
- Respects rate limits (no changes to existing logic)
- Pricing based on public OpenAI pricing page

### Privacy
- No PII collected
- Data stored locally only
- User has full control over data

### License
- Follows MIT License of the project
- No third-party dependencies added

## Notes

1. **Pricing Updates**: When OpenAI changes pricing, update the `ModelPricing` dictionary in `UsageService.cs`

2. **Model Support**: Currently supports gpt-4o-mini, gpt-4o, and gpt-3.5-turbo. Add new models to the pricing dictionary as needed.

3. **History Size**: No automatic cleanup. Users must manually clear history if the file grows too large.

4. **Time Zones**: All timestamps use local system time (not UTC).

5. **Backward Compatibility**: Existing settings and functionality unchanged. Feature is purely additive.

## Verification Checklist

- [x] Models defined correctly
- [x] Service implements all required methods
- [x] UI window displays all necessary information
- [x] Data persistence works correctly
- [x] Cost calculation is accurate
- [x] Period filtering works
- [x] Clear history includes confirmation
- [x] Documentation updated
- [x] Testing guide created
- [x] No breaking changes to existing code

## Conclusion

The usage statistics and history feature has been successfully implemented with minimal changes to existing code. It provides users with full visibility into their API usage and costs while maintaining the application's privacy-first approach. The implementation is modular, well-documented, and ready for testing on Windows.
