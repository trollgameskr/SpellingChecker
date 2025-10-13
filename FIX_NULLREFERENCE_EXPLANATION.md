# Fix for NullReferenceException in Usage Statistics Window

## Issue Description
When clicking the "📊 View Usage Statistics" button in the Settings window, the application crashes with a `NullReferenceException` error.

## Error Stack Trace
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at SpellingChecker.Views.UsageStatisticsWindow.LoadStatistics(Nullable`1 startDate, Nullable`1 endDate) in UsageStatisticsWindow.xaml.cs:line 25
   at SpellingChecker.Views.UsageStatisticsWindow.PeriodComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs e) in UsageStatisticsWindow.xaml.cs:line 79
```

## Root Cause Analysis

### Event Firing During XAML Initialization
The issue occurs due to the order of operations during WPF window initialization:

1. **Window Constructor Called** (line 16-21 in `UsageStatisticsWindow.xaml.cs`)
   ```csharp
   public UsageStatisticsWindow()
   {
       InitializeComponent();  // <- XAML parsing starts here
       _usageService = new UsageService();
       LoadStatistics();
   }
   ```

2. **XAML Parsing** (during `InitializeComponent()`)
   - XAML file is parsed and controls are created
   - At line 93 of `UsageStatisticsWindow.xaml`, the ComboBox has:
     ```xml
     <ComboBoxItem Content="All Time" IsSelected="True"/>
     ```
   - Setting `IsSelected="True"` **immediately fires** the `SelectionChanged` event

3. **Event Handler Called Prematurely**
   - `PeriodComboBox_SelectionChanged` is called at line 79
   - This happens **before** `InitializeComponent()` completes
   - UI controls like `TotalOperationsText` don't exist yet (null)

4. **Null Reference Exception**
   - `LoadStatistics()` tries to access `TotalOperationsText.Text` at line 39
   - Since the control hasn't been created yet, it's null
   - Exception is thrown

### Timeline Diagram
```
Constructor starts
  ├─> InitializeComponent() called
  │     ├─> XAML parsing begins
  │     ├─> PeriodComboBox created
  │     ├─> ComboBoxItem created with IsSelected="True"
  │     ├─> SelectionChanged event fires  ⚠️
  │     │     └─> PeriodComboBox_SelectionChanged() called
  │     │           └─> LoadStatistics() called
  │     │                 └─> TotalOperationsText is NULL ❌
  │     │                       └─> NullReferenceException thrown
  │     │
  │     ├─> TotalOperationsText created (never reached)
  │     └─> Other controls created (never reached)
  │
  └─> InitializeComponent() never completes
```

## Solution

### The Fix
Add a null check at the beginning of `PeriodComboBox_SelectionChanged` to prevent execution during initialization:

```csharp
private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    // Prevent execution during initialization before controls are ready
    if (TotalOperationsText == null)
        return;

    // Rest of the method...
}
```

### Why This Works
1. During XAML initialization, when the event fires, `TotalOperationsText` is still null
2. The null check detects this and returns early
3. XAML initialization completes successfully
4. All controls are now initialized
5. Future selection changes work normally because controls exist

### Alternative Solutions Considered

#### ❌ Option 1: Remove `IsSelected="True"` from XAML
```xml
<!-- Don't set a default selection -->
<ComboBoxItem Content="All Time"/>
```
**Rejected:** This would leave no item selected initially, poor UX.

#### ❌ Option 2: Set selection in code-behind
```csharp
public UsageStatisticsWindow()
{
    InitializeComponent();
    _usageService = new UsageService();
    PeriodComboBox.SelectedIndex = 0; // Set programmatically
    LoadStatistics();
}
```
**Rejected:** This would still trigger the event handler and call `LoadStatistics()` twice.

#### ❌ Option 3: Unsubscribe and resubscribe to event
```csharp
public UsageStatisticsWindow()
{
    InitializeComponent();
    PeriodComboBox.SelectionChanged -= PeriodComboBox_SelectionChanged;
    _usageService = new UsageService();
    LoadStatistics();
    PeriodComboBox.SelectionChanged += PeriodComboBox_SelectionChanged;
}
```
**Rejected:** More complex, harder to maintain, doesn't handle XAML-triggered events.

#### ✅ Option 4: Null check (Selected)
Simple, clear intent, minimal code change, handles all edge cases.

## Testing

### Manual Test Steps
1. Build and run the application
2. Open Settings window (double-click tray icon or right-click → Settings)
3. Click "📊 View Usage Statistics" button
4. **Expected:** Window opens successfully showing statistics
5. **Expected:** "All Time" is pre-selected in the period filter
6. Change the period filter to "Today"
7. **Expected:** Statistics update to show today's data only
8. Change back to "All Time"
9. **Expected:** Statistics show all data again

### Verification Checklist
- [x] Window opens without crashing
- [x] Default "All Time" filter is selected
- [x] All statistics display correctly
- [x] Period filter changes work properly
- [x] No duplicate calls to `LoadStatistics()`

## Impact Analysis

### Changed Files
- `SpellingChecker/Views/UsageStatisticsWindow.xaml.cs` (+4 lines)

### Affected Functionality
- ✅ Opening Usage Statistics window (now works)
- ✅ Viewing statistics (no change)
- ✅ Filtering by period (no change)
- ✅ Clearing history (no change)

### Regression Risk
**Very Low** - The change is defensive (early return) and doesn't affect normal operation after initialization.

## Related Issues
This is a common WPF gotcha: event handlers firing during `InitializeComponent()`. Always verify controls are initialized before using them in event handlers triggered during XAML parsing.

## References
- Error Stack Trace: Original issue report
- WPF Control Lifecycle: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/object-lifetime-events
- Similar Issues: https://stackoverflow.com/questions/tagged/wpf+nullreferenceexception+initializecomponent
