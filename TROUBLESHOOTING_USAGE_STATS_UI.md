# Troubleshooting Guide: Usage Statistics UI Not Visible

## Problem
"설정창에서 usage-statistics 관련 UI를 찾을 수 없다" (Can't find usage-statistics UI in settings window)

## Quick Answer
The feature **IS in main branch** and should be visible. If you can't see it, follow these steps:

## Step 1: Verify You Have the Latest Code

```bash
# Check your current commit
git log --oneline -1

# Expected output should be:
# 3d6e54e (or later) Merge pull request #4...
```

If your commit is older than `2f524ed`, you need to pull the latest code:

```bash
git checkout main
git pull origin main
```

## Step 2: Clean Build

```bash
# Clean the solution
dotnet clean

# Delete build artifacts
rm -rf SpellingChecker/bin
rm -rf SpellingChecker/obj

# Rebuild
dotnet build
```

## Step 3: Verify Files Exist

Check that these files are present in your repository:

```bash
# Check all required files exist
ls -la SpellingChecker/Views/UsageStatisticsWindow.xaml
ls -la SpellingChecker/Views/UsageStatisticsWindow.xaml.cs
ls -la SpellingChecker/Services/UsageService.cs

# Check the button is in SettingsWindow.xaml
grep "View Usage Statistics" SpellingChecker/Views/SettingsWindow.xaml
```

Expected output:
```
            <Button Content="📊 View Usage Statistics"
```

## Step 4: Check SettingsWindow.xaml

Open `SpellingChecker/Views/SettingsWindow.xaml` and verify the button exists around line 106:

```xml
<!-- Usage Statistics Button -->
<StackPanel Grid.Row="6" Margin="0,0,0,15">
    <Button Content="📊 View Usage Statistics" 
            Click="ViewUsageStatisticsButton_Click"
            Padding="15,10"
            Background="#2196F3"
            Foreground="White"
            BorderThickness="0"
            Cursor="Hand"
            FontWeight="SemiBold"
            HorizontalAlignment="Left"/>
    <TextBlock Text="Track your API usage, token consumption, and costs" 
               FontSize="10" 
               Foreground="Gray" 
               Margin="0,5,0,0"/>
</StackPanel>
```

## Step 5: Check Event Handler

Open `SpellingChecker/Views/SettingsWindow.xaml.cs` and verify the click handler exists around line 85:

```csharp
private void ViewUsageStatisticsButton_Click(object sender, RoutedEventArgs e)
{
    var usageWindow = new UsageStatisticsWindow();
    usageWindow.ShowDialog();
}
```

## Step 6: Run the Application

1. Build and run the application
2. Right-click the tray icon → Settings (or open Settings from main window)
3. **Scroll down** in the Settings window
4. The button should appear **after the Hotkeys section** and **before the Save/Cancel buttons**

### Visual Location

```
Settings Window:
┌─────────────────────────────────┐
│ Application Settings            │
├─────────────────────────────────┤
│ OpenAI API Key: [___________]  │
│ API Endpoint:   [___________]  │
│ AI Model:       [___________]  │
│ ☑ Start with Windows            │
│                                 │
│ Hotkeys:                        │
│   Spelling:    [___________]   │
│   Translation: [___________]   │
│                                 │
│ ┌─────────────────────────────┐ │
│ │ 📊 View Usage Statistics    │ │ ← HERE!
│ └─────────────────────────────┘ │
│ Track your API usage, costs...  │
│                                 │
│          [Save] [Cancel]        │
└─────────────────────────────────┘
```

## Step 7: Check for Build Errors

If the application builds but the button is not visible, check for compilation errors:

```bash
# Build with detailed output
dotnet build -v detailed 2>&1 | grep -i error

# Check for XAML build errors
dotnet build 2>&1 | grep -i xaml
```

## Common Issues and Solutions

### Issue 1: Old Build Cached
**Solution**: Delete bin/obj folders and rebuild
```bash
rm -rf SpellingChecker/bin SpellingChecker/obj
dotnet build
```

### Issue 2: Visual Studio Cache
**Solution**: If using Visual Studio, clean solution from IDE:
- Build → Clean Solution
- Close Visual Studio
- Delete .vs folder
- Reopen and rebuild

### Issue 3: Window Height Too Small
**Solution**: Check the window height in XAML
```xml
<Window ... Height="620" Width="600" ...>
```
The button is at Grid.Row="6" which requires scrolling if window is too small.

### Issue 4: Grid Row Misconfiguration
**Solution**: Verify Grid.RowDefinitions has 8 rows (0-7):
```bash
grep -A10 "Grid.RowDefinitions" SpellingChecker/Views/SettingsWindow.xaml
```

Should show 8 `<RowDefinition>` elements.

## Verification Checklist

- [ ] Git commit is 2f524ed or later
- [ ] All required files exist
- [ ] Button exists in SettingsWindow.xaml at line ~106
- [ ] Event handler exists in SettingsWindow.xaml.cs at line ~85
- [ ] Clean build completed successfully
- [ ] No XAML compilation errors
- [ ] Application runs without crashes
- [ ] Settings window opens successfully
- [ ] Button is visible in Settings window (scroll down if needed)

## Still Not Working?

If you've followed all steps and still can't see the button:

1. **Check the exact commit you're on**:
   ```bash
   git log --oneline --graph -5
   ```

2. **Verify the merge happened**:
   ```bash
   git log --all --grep="usage-statistics"
   ```

3. **Compare your file with the expected version**:
   ```bash
   git diff 2f524ed SpellingChecker/Views/SettingsWindow.xaml
   ```
   (Should show no differences if you're on main)

4. **Check for manual modifications**:
   ```bash
   git status
   git diff
   ```

## For Developers

If you're investigating why the UI isn't showing:

1. Add debug output in SettingsWindow constructor:
   ```csharp
   public SettingsWindow()
   {
       InitializeComponent();
       System.Diagnostics.Debug.WriteLine("SettingsWindow initialized");
       // ... rest of code
   }
   ```

2. Add debug output in button click handler:
   ```csharp
   private void ViewUsageStatisticsButton_Click(object sender, RoutedEventArgs e)
   {
       System.Diagnostics.Debug.WriteLine("Usage Statistics button clicked");
       var usageWindow = new UsageStatisticsWindow();
       usageWindow.ShowDialog();
   }
   ```

3. Run in Debug mode and check Output window for messages.

## Reference

See `USAGE_STATISTICS_VERIFICATION.md` for detailed information about the merge and code verification.

---

**Last Updated**: October 13, 2025  
**Feature Status**: ✅ Merged in PR #3 (commit 2f524ed)  
**Current Main**: commit 3d6e54e
