# Usage Statistics Window - UI Overview

## Window Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ Usage Statistics - AI Spelling Checker                    [×]   │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Usage Statistics and History                                   │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐ │
│  │ Total Statistics                                          │ │
│  ├─────────────────┬─────────────────┬─────────────────────┤ │
│  │ Total Operations│ Prompt Tokens   │ Total Cost (USD)    │ │
│  │ 23              │ 1,234           │ $0.123456          │ │
│  │                 │                 │                     │ │
│  │ Corrections     │ Completion Tkns │                     │ │
│  │ 15              │ 5,678           │                     │ │
│  │                 │                 │                     │ │
│  │ Translations    │ Total Tokens    │                     │ │
│  │ 8               │ 6,912           │                     │ │
│  └─────────────────┴─────────────────┴─────────────────────┘ │
│                                                                 │
│  Filter by Period: [All Time ▼]  [Clear History]              │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐ │
│  │ Date/Time         │Type      │Model      │Prompt│Comp│... │ │
│  ├───────────────────┼──────────┼───────────┼──────┼────┼───┤ │
│  │2025-10-10 15:30:22│Correction│gpt-4o-mini│ 123  │456 │... │ │
│  │2025-10-10 14:15:10│Translation│gpt-4o-mini│ 234  │567 │... │ │
│  │2025-10-10 12:05:33│Correction│gpt-4o-mini│ 345  │678 │... │ │
│  │2025-10-10 11:20:45│Translation│gpt-4o-mini│ 456  │789 │... │ │
│  │2025-10-09 16:45:12│Correction│gpt-4o-mini│ 567  │890 │... │ │
│  │                   │          │           │      │    │    │ │
│  │                   │          │           │      │    │    │ │
│  └───────────────────────────────────────────────────────────┘ │
│                                                                 │
│                                              [Close]            │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## UI Components Description

### 1. Title Bar
- Window title: "Usage Statistics - AI Spelling Checker"
- Standard window controls (minimize, maximize, close)

### 2. Summary Statistics Panel (Top)
Three-column layout showing aggregated data:

**Column 1: Operations**
- Total Operations count
- Corrections count
- Translations count

**Column 2: Token Usage**
- Prompt Tokens (formatted with thousands separator)
- Completion Tokens (formatted with thousands separator)
- Total Tokens (bold, formatted)

**Column 3: Cost**
- Large display of total cost in USD
- Green color (#4CAF50)
- 6 decimal places precision

### 3. Filter Controls
- Period dropdown with options:
  - All Time (default)
  - Today
  - This Week
  - This Month
- Clear History button (red, #F44336)

### 4. History DataGrid
Displays detailed usage records with columns:

| Column | Description | Format | Width |
|--------|-------------|--------|-------|
| Date/Time | Timestamp of operation | yyyy-MM-dd HH:mm:ss | 150px |
| Type | Correction or Translation | Text | 100px |
| Model | AI model used | Text | 120px |
| Prompt Tokens | Input tokens | Number | 110px |
| Completion Tokens | Output tokens | Number | 140px |
| Total Tokens | Sum of tokens | Number | 100px |
| Cost (USD) | Calculated cost | $0.000000 | Auto |

**Features**:
- Sortable columns
- Alternating row background (#F5F5F5)
- Horizontal grid lines
- Read-only (no editing)
- Scrollable for many records
- Auto-sized last column

### 5. Close Button
- Blue background (#2196F3)
- White text
- Located at bottom right
- 30px vertical padding, 10px horizontal

## Color Scheme

| Element | Color | Usage |
|---------|-------|-------|
| Primary | #2196F3 | Close button, links |
| Success | #4CAF50 | Save button, cost display |
| Danger | #F44336 | Clear History button |
| Border | #E0E0E0 | Borders, separators |
| Alt Row | #F5F5F5 | Alternating grid rows |
| Text | Black | Primary text |
| Label | Gray | Helper text |

## Window Properties

- **Size**: 800×600 pixels
- **Resizable**: Yes
- **Position**: Center screen
- **Modal**: Yes (when opened from Settings)
- **Background**: White

## User Interactions

### Viewing Statistics
1. User clicks "📊 View Usage Statistics" in Settings
2. Window opens with current data loaded
3. Summary shows aggregated stats
4. Grid shows detailed records (newest first)

### Filtering by Period
1. User selects period from dropdown
2. Data updates immediately
3. Summary recalculates for selected period
4. Grid filters to show only matching records

### Clearing History
1. User clicks "Clear History"
2. Confirmation dialog appears
3. If confirmed:
   - All records deleted
   - Statistics reset to zero
   - Success message shown
4. If cancelled:
   - No changes made

### Closing Window
1. User clicks "Close" or window's X button
2. Window closes
3. Returns to Settings window

## Accessibility Features

- High contrast text
- Large clickable buttons
- Clear visual hierarchy
- Readable fonts (system default)
- Keyboard navigation support
- Screen reader compatible (XAML standard)

## Empty State

When no usage data exists:
- Summary shows all zeros
- Grid is empty
- Message could be added (future enhancement)

## Large Dataset Handling

For users with many records:
- DataGrid has built-in virtualization
- Scrollbar appears automatically
- Performance optimized by WPF
- Consider pagination in future versions

## Responsive Design

Current implementation:
- Fixed minimum size
- Resizable window
- Last column auto-expands
- Scrollbars when needed

Future enhancements:
- Better mobile/tablet support
- Collapsible sections
- Responsive column hiding

## Integration with Main App

Access path:
```
System Tray Icon
  → Right-click → Settings
    → Settings Window
      → Click "📊 View Usage Statistics"
        → Usage Statistics Window opens
```

## Data Refresh

- Data loads on window open
- Updates when period filter changes
- Refreshes after clearing history
- No auto-refresh (static snapshot)

## Error Handling

Graceful degradation:
- If file missing: Show empty state
- If file corrupt: Show zero stats
- If calculation fails: Show $0.00
- Silent failures logged (future)

## Future Enhancements

Potential improvements:
- Export to CSV button
- Chart visualization
- Refresh button
- Real-time updates
- Tooltips with more info
- Right-click context menu
- Column customization
- Dark mode support
