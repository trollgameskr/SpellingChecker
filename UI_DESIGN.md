# UI Design and User Experience

## Visual Design Overview

The application uses a modern, clean design with minimal UI footprint. It runs entirely in the background with only popups and the system tray icon visible to users.

## UI Components

### 1. System Tray Icon

**Appearance:**
```
┌─────────────────────────────────────┐
│  Windows Taskbar                    │
│  [... other icons ...] [📝] [🔔]   │  ← SpellingChecker icon here
└─────────────────────────────────────┘
```

**Context Menu (Right-click):**
```
┌─────────────────────┐
│  Settings          │
│  ─────────────────  │
│  Exit              │
└─────────────────────┘
```

**Behavior:**
- Always visible when app is running
- Double-click: Opens Settings
- Right-click: Shows context menu
- Balloon notifications for errors

---

### 2. Settings Window

**Dimensions:** 600x520 pixels
**Position:** Center screen
**Style:** Modern, clean, white background

**Layout:**
```
┌─────────────────────────────────────────────────────────────┐
│  Settings - AI Spelling Checker                     [_][□][X]│
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Application Settings                                        │
│  ════════════════                                            │
│                                                               │
│  OpenAI API Key: *                                           │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ ••••••••••••••••••••••••••••••••••••••••••••••••••  │   │
│  └─────────────────────────────────────────────────────┘   │
│  Required for AI-powered spelling correction and translation │
│                                                               │
│  API Endpoint: *                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ https://api.openai.com/v1                           │   │
│  └─────────────────────────────────────────────────────┘   │
│  Default: https://api.openai.com/v1                         │
│                                                               │
│  AI Model: *                                                 │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ gpt-4o-mini                                          │   │
│  └─────────────────────────────────────────────────────┘   │
│  Default: gpt-4o-mini (recommended for balance)             │
│                                                               │
│  ☑ Start with Windows                                       │
│  Launch the application automatically when Windows starts    │
│                                                               │
│  Hotkeys:                                                    │
│  • Ctrl+Shift+Y: Spelling Correction                        │
│  • Ctrl+Shift+T: Translation                                │
│                                                               │
│                                                               │
│                                          ┌──────┐ ┌────────┐ │
│                                          │ Save │ │ Cancel │ │
│                                          └──────┘ └────────┘ │
└─────────────────────────────────────────────────────────────┘
```

**Color Scheme:**
- Background: White (#FFFFFF)
- Primary Button: Green (#4CAF50)
- Secondary Button: Gray (#9E9E9E)
- Text: Dark Gray (#333333)
- Hints: Light Gray (#888888)

---

### 3. Result Popup Window

**Dimensions:** 600x400 pixels (resizable)
**Position:** Near cursor, auto-adjusted to stay on screen
**Style:** Modern, with colored borders to distinguish sections

**Spelling Correction Layout:**
```
┌─────────────────────────────────────────────────────────────┐
│  Spelling Correction                            [_][□][X]   │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Original:                                                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Ths is a tst sentance with some erors in it.       │   │
│  │                                                      │   │
│  │                                                      │   │
│  │                                                      │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  Result:                                                     │
│  ┌═════════════════════════════════════════════════════┐   │
│  ║ This is a test sentence with some errors in it.    ║   │
│  ║                                                      ║   │
│  ║                                                      ║   │
│  ║                                                      ║   │
│  └═════════════════════════════════════════════════════┘   │
│                                                               │
│                   ┌──────────────┐ ┌─────────┐ ┌───────┐   │
│                   │Copy to       │ │ Replace │ │ Close │   │
│                   │Clipboard     │ │         │ │       │   │
│                   └──────────────┘ └─────────┘ └───────┘   │
└─────────────────────────────────────────────────────────────┘
```

**Translation Layout:**
```
┌─────────────────────────────────────────────────────────────┐
│  Translation (Korean → English)                 [_][□][X]   │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Original:                                                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ 안녕하세요, 오늘 회의 일정을 변경하고 싶습니다.      │   │
│  │                                                      │   │
│  │                                                      │   │
│  │                                                      │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  Result:                                                     │
│  ┌═════════════════════════════════════════════════════┐   │
│  ║ Hello, I would like to change the meeting           ║   │
│  ║ schedule for today.                                 ║   │
│  ║                                                      ║   │
│  ║                                                      ║   │
│  └═════════════════════════════════════════════════════┘   │
│                                                               │
│                   ┌──────────────┐ ┌─────────┐ ┌───────┐   │
│                   │Copy to       │ │ Replace │ │ Close │   │
│                   │Clipboard     │ │         │ │       │   │
│                   └──────────────┘ └─────────┘ └───────┘   │
└─────────────────────────────────────────────────────────────┘
```

**Color Scheme:**
- Original box: Light gray background (#F5F5F5), gray border
- Result box: Light blue background (#F0F8FF), blue border (#2196F3)
- Copy button: Green (#4CAF50)
- Replace button: Blue (#2196F3)
- Close button: Red (#f44336)

---

## User Interaction Flows

### Flow 1: First-Time Setup

```
1. User installs application
   └─> SpellingChecker appears in system tray
   
2. User double-clicks tray icon
   └─> Settings window opens
   
3. User enters API key
   └─> Types/pastes OpenAI API key
   
4. User clicks "Save"
   └─> Settings encrypted and saved
   └─> Window closes
   └─> Balloon notification: "Settings saved successfully!"
```

### Flow 2: Spelling Correction

```
1. User selects text in any application
   └─> Text highlighted
   
2. User presses Ctrl+Shift+Y
   └─> [Background] Clipboard captures text
   └─> Balloon notification appears: "Processing... - AI is correcting your text. Please wait..."
   └─> [Background] Text sent to AI (1-3 seconds)
   
3. Result popup appears near cursor
   └─> Shows original vs corrected text side-by-side
   
4. User reviews correction
   └─> Sees what changed
   
5. User clicks "Copy to Clipboard"
   └─> Corrected text copied
   └─> Popup shows: "Copied to clipboard!"
   └─> OR user clicks "Replace"
   └─> Original text replaced with correction
   └─> Popup closes
```

### Flow 3: Translation

```
1. User selects Korean or English text
   └─> Text highlighted
   
2. User presses Ctrl+Shift+T
   └─> [Background] Clipboard captures text
   └─> Balloon notification appears: "Processing... - AI is translating your text. Please wait..."
   └─> [Background] Language detected
   └─> [Background] Translation requested from AI (1-3 seconds)
   
3. Result popup appears
   └─> Title shows: "Translation (Korean → English)"
   └─> Shows original and translated text
   
4. User chooses action
   └─> Click "Copy to Clipboard" for reference
   └─> OR click "Replace" to substitute original
   └─> OR click "Close" to dismiss
```

### Flow 4: Error Handling

```
Scenario: API key not configured

1. User presses Ctrl+Shift+Y
   └─> System captures text
   └─> Attempts API call
   └─> Error: "API key not configured"
   
2. Balloon notification appears
   └─> "API Key is not configured"
   └─> "Please set your OpenAI API key in settings."
   
3. User clicks notification
   └─> Settings window opens
   └─> User can enter API key
```

---

## Accessibility Features

### Keyboard Navigation

**In Settings Window:**
- Tab: Move between fields
- Enter: Save
- Esc: Cancel

**In Result Popup:**
- Tab: Cycle through buttons
- Enter: Activate focused button
- Esc: Close popup

### Screen Reader Support

All controls have:
- Descriptive labels
- ARIA attributes (WPF Automation)
- Logical tab order

### High Contrast Mode

The application respects Windows high contrast settings:
- System colors used for text/backgrounds
- Borders remain visible
- Buttons maintain clear visual distinction

---

## Visual Feedback

### Loading States

**During API Call:**
```
Cursor changes to "waiting" (hourglass/spinner)
System tray icon could show activity indicator (future)
```

### Success States

**After successful action:**
```
✓ Settings saved → Balloon notification
✓ Text copied → In-popup notification
✓ Text replaced → Popup closes immediately
```

### Error States

**When errors occur:**
```
✗ API error → Balloon notification with error message
✗ No text selected → Balloon: "Please select text first"
✗ Network error → Balloon: "Check internet connection"
```

---

## Responsive Design

### Multi-Monitor Support

**Popup positioning logic:**
```
1. Get cursor position
2. Calculate popup position (centered on cursor)
3. Check if popup fits on current screen
4. Adjust position if needed:
   - Move left if too far right
   - Move up if too far down
   - Ensure fully visible on primary monitor
```

### Display Scaling

**Supports Windows DPI scaling:**
- 100% (96 DPI) - Default
- 125% (120 DPI) - Recommended
- 150% (144 DPI) - Supported
- 200% (192 DPI) - Supported

All UI elements scale proportionally.

---

## Animation and Transitions

**Current Version (1.0):**
- Minimal animations for performance
- Instant popup appearance
- Standard window fade-out on close

**Future Versions:**
- Smooth popup fade-in
- Button hover effects
- Settings save confirmation animation
- Progress indicator during API calls

---

## Branding

### Application Icon

**Placeholder:** System application icon
**Future:** Custom icon with:
- Pencil/checkmark symbol
- Blue/green color scheme
- Clear at 16x16 (tray icon size)
- Professional at 256x256 (installer)

### Color Palette

**Primary Colors:**
- Success: Green (#4CAF50)
- Action: Blue (#2196F3)
- Warning: Orange (#FF9800)
- Error: Red (#f44336)

**Neutral Colors:**
- White (#FFFFFF)
- Light Gray (#F5F5F5)
- Medium Gray (#9E9E9E)
- Dark Gray (#333333)

---

## Performance UI

### Response Time Targets

**User perception:**
- < 100ms: Instant
- 100-300ms: Slight delay (acceptable)
- 300-1000ms: Noticeable (show loading)
- > 1000ms: Slow (show progress)

**Actual performance:**
- Hotkey detection: < 10ms (instant)
- Clipboard capture: ~50-100ms (instant)
- API call (gpt-4o-mini): 1-3 seconds (loading cursor)
- Popup display: < 50ms (instant)

### Memory Footprint

**Target:** < 100 MB RAM
**Actual (typical):** 50-80 MB

**Strategies:**
- Single HttpClient instance
- Dispose windows when closed
- No caching of results (privacy)
- Lazy loading of services

---

## Error Prevention

### Input Validation

**Settings Window:**
- API key required (non-empty)
- Endpoint must be valid URL
- Model name validated against known models

**Visual indicators:**
```
Valid input:   ┌──────────┐
               │ value    │
               └──────────┘

Invalid input: ┌──────────┐  ⚠️ Required field
               │          │
               └──────────┘
               Red border
```

### Confirmation Dialogs

**Currently:**
- Settings save: Silent save with notification
- Exit: Direct exit (running in background)

**Future:**
- Unsaved changes warning
- Destructive action confirmations

---

## Localization (Future)

### Current: English UI with Korean support

**Planned languages:**
- Korean UI
- Japanese UI
- Simplified Chinese UI

**Localization strategy:**
- Resource files (.resx)
- Runtime language switching
- Respect Windows language settings

---

## Comparison with Competitors

### vs. Built-in Spell Checkers

**SpellingChecker advantages:**
✓ AI-powered (context-aware)
✓ Works everywhere
✓ Translation included
✓ Korean support

**Built-in advantages:**
✓ Free
✓ Offline
✓ Faster

### vs. Online Translation Services

**SpellingChecker advantages:**
✓ Hotkey access (faster)
✓ No context switching
✓ Direct text replacement
✓ Correction + translation combo

**Online services advantages:**
✓ Free (usually)
✓ More languages
✓ Web interface

---

## Future UI Enhancements

### Planned Features

1. **Dark Mode**
   - Automatic based on Windows theme
   - Manual toggle in settings

2. **Customizable Hotkeys**
   - Hotkey configuration in settings
   - Conflict detection
   - Multiple action support

3. **History Panel**
   - Recent corrections/translations
   - Re-use previous results
   - Search history

4. **Inline Suggestions**
   - Show corrections in-context
   - Hover to preview
   - Click to apply

5. **Browser Extension**
   - Web-specific UI
   - Context menu integration
   - Selected text highlighting

---

## Conclusion

The UI design prioritizes:
1. **Minimal footprint** - Runs in background
2. **Fast access** - Global hotkeys
3. **Clear presentation** - Side-by-side comparison
4. **Intuitive actions** - Copy or replace
5. **Professional appearance** - Modern, clean design

This design ensures the application stays out of the user's way until needed, then provides quick, clear results when activated.
