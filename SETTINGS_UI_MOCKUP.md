# Settings Window UI - Progress Notification Control

## Visual Layout

The new "Show progress notifications" checkbox has been added to the Settings window in the following location:

```
┌─────────────────────────────────────────────────────────────┐
│  Settings - AI Spelling Checker v1.0.0              [_][□][X]│
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Application Settings                                        │
│                                                               │
│  OpenAI API Key:                                             │
│  [•••••••••••••••••••••••••••••••••••••••]                  │
│  Required for AI-powered spelling correction and translation│
│                                                               │
│  API Endpoint:                                               │
│  [https://api.openai.com/v1                ]                 │
│  Default: https://api.openai.com/v1                          │
│                                                               │
│  AI Model:                                                   │
│  [gpt-4o-mini                             ]                  │
│  Default: gpt-4o-mini (recommended for balance of speed...)  │
│                                                               │
│  ☑ Start with Windows                                        │
│    Launch the application automatically when Windows starts  │
│                                                               │
│  ☐ Show progress notifications              ← NEW CHECKBOX  │
│    Display desktop notifications for spelling correction     │
│    and translation progress                                  │
│                                                               │
│  Hotkeys:                                                    │
│                                                               │
│  Spelling Correction Hotkey:                                 │
│  [Ctrl+Shift+Alt+Y                        ]                  │
│  Example: Ctrl+Shift+Alt+Y                                   │
│                                                               │
│  Translation Hotkey:                                         │
│  [Ctrl+Shift+Alt+T                        ]                  │
│  Example: Ctrl+Shift+Alt+T                                   │
│                                                               │
│  [📊 View Usage Statistics]                                  │
│  Track your API usage, token consumption, and costs          │
│                                                               │
│                                          [  Save  ] [Cancel] │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## Checkbox Details

**Label**: "Show progress notifications"
- **Font**: SemiBold
- **Default State**: Unchecked (☐)
- **Position**: Below "Start with Windows" checkbox
- **Margin**: 10px top spacing from previous checkbox

**Description Text**: "Display desktop notifications for spelling correction and translation progress"
- **Font Size**: 10px
- **Color**: Gray
- **Position**: Below checkbox, indented 20px from left

## Behavior

### When Unchecked (Default)
- Progress notifications are NOT shown
- Only error and startup notifications appear
- Users work without progress interruptions
- Result popup windows still appear normally

### When Checked
Desktop notifications appear at these points:

**Spelling Correction:**
1. 🔔 "맞춤법 교정 요청" - When user triggers hotkey
2. 🔔 "Processing..." - While AI is processing
3. 🔔 "맞춤법 교정 완료" - When correction is complete

**Translation:**
1. 🔔 "번역 요청" - When user triggers hotkey
2. 🔔 "Processing..." - While AI is processing
3. 🔔 "번역 완료" - When translation is complete

### Always Shown (Regardless of Setting)
- ✅ Startup notification with hotkey information
- ✅ Error notifications ("No text selected", "Error", etc.)

## Implementation Details

The checkbox is bound to the `ShowProgressNotifications` property in `AppSettings`:
- Property Type: `bool`
- Default Value: `false`
- Storage: Encrypted settings file in `%APPDATA%\SpellingChecker\settings.json`

## Testing Checklist

To test this feature on Windows:

1. ☐ Build and run the application
2. ☐ Open Settings window (double-click tray icon)
3. ☐ Verify checkbox appears below "Start with Windows"
4. ☐ Verify checkbox is UNCHECKED by default
5. ☐ Leave unchecked and test spelling correction
   - Should NOT see progress notifications
   - Should see result popup
6. ☐ Check the "Show progress notifications" box
7. ☐ Click Save
8. ☐ Test spelling correction again
   - Should see "맞춤법 교정 요청" notification
   - Should see "Processing..." notification
   - Should see "맞춤법 교정 완료" notification
   - Should see result popup
9. ☐ Test translation with checkbox checked
   - Should see "번역 요청" notification
   - Should see "Processing..." notification
   - Should see "번역 완료" notification
   - Should see result popup
10. ☐ Uncheck the box and save
11. ☐ Test again - no progress notifications should appear
12. ☐ Test error cases - error notifications should always appear

---

**Note**: Since this is a WPF application, it can only be properly tested on Windows. The implementation follows WPF best practices and standard checkbox behavior.
