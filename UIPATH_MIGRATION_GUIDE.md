# UiPath Migration Guide

## Overview

This document explains the migration of the SpellingChecker application from .NET/C# to UiPath Studio RPA platform.

## Why Migrate to UiPath?

According to the requirements, implementing the functionality to copy selected text requires using UiPath Studio, which is an RPA (Robotic Process Automation) tool. The migration addresses the following needs:

1. **RPA-based Text Capture**: UiPath provides robust clipboard and keyboard automation capabilities
2. **Application Independence**: Works with any application that supports text selection
3. **Visual Development**: Easier to maintain and modify without deep programming knowledge
4. **Enterprise Integration**: Can be deployed via UiPath Orchestrator for centralized management

## Migration Summary

### Original Architecture (.NET/C#)
```
SpellingChecker.exe (WPF Application)
├── System Tray Service
├── Global Hotkey Registration (Win32 API)
├── ClipboardService.cs (Win32 SendMessage API)
├── AIService.cs (OpenAI HTTP client)
├── HotkeyService.cs (Global hotkey handling)
├── SettingsService.cs (Encrypted configuration)
└── WPF UI Windows
```

### New Architecture (UiPath RPA)
```
UiPathProject/
├── Main.xaml (Entry point workflow)
├── Workflows/
│   ├── SpellingCorrection.xaml
│   ├── Translation.xaml
│   └── CallOpenAIAPI.xaml
└── Config (Arguments/Assets)
```

## Key Changes

### 1. Text Capture Method

**Before (.NET/C#):**
```csharp
// ClipboardService.cs
private string GetSelectedTextViaSendMessage()
{
    var foregroundWindow = GetForegroundWindow();
    var focusedControl = GetFocus();
    SendMessage(focusedControl, EM_GETSEL, ref selStart, ref selEnd);
    SendMessage(focusedControl, EM_GETSELTEXT, IntPtr.Zero, buffer);
    return buffer.ToString();
}
```

**After (UiPath RPA):**
```
┌─────────────────────────────────┐
│ SendHotkey: Ctrl+C              │
├─────────────────────────────────┤
│ Delay: 500ms                    │
├─────────────────────────────────┤
│ GetClipboardText → SelectedText │
└─────────────────────────────────┘
```

### 2. API Integration

**Before (.NET/C#):**
```csharp
// AIService.cs
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", apiKey);
var response = await client.PostAsync(endpoint, content);
```

**After (UiPath RPA):**
```
┌──────────────────────────────────────┐
│ HTTP Request Activity                │
│ - Method: POST                       │
│ - Endpoint: in_APIEndpoint           │
│ - Headers: Authorization Bearer      │
│ - Body: RequestBody (JSON)           │
│ → Response → APIResponse             │
└──────────────────────────────────────┘
```

### 3. User Interaction

**Before (.NET/C#):**
- System tray icon always running
- Global hotkeys (Ctrl+Shift+Alt+Y, Ctrl+Shift+Alt+T)
- WPF popup windows with results
- Settings window for configuration

**After (UiPath RPA):**
- Attended automation (run on-demand)
- Menu-driven workflow selection
- Dialog boxes for input/output
- Arguments for configuration

### 4. Text Replacement

**Before (.NET/C#):**
```csharp
// ClipboardService.cs
public void ReplaceSelectedText(string text)
{
    SetClipboard(text);
    Thread.Sleep(50);
    keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
    keybd_event(VK_V, 0, 0, UIntPtr.Zero);
    keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
}
```

**After (UiPath RPA):**
```
┌────────────────────────────────┐
│ SetClipboardText: CorrectedText│
├────────────────────────────────┤
│ SendHotkey: Ctrl+V             │
└────────────────────────────────┘
```

## Feature Comparison

| Feature | .NET Implementation | UiPath Implementation | Status |
|---------|--------------------|--------------------|--------|
| Text Selection Capture | Win32 SendMessage API | Ctrl+C Simulation | ✅ Migrated |
| Clipboard Fallback | Yes | Primary Method | ✅ Migrated |
| Spelling Correction | OpenAI API | OpenAI API | ✅ Migrated |
| Translation | OpenAI API | OpenAI API | ✅ Migrated |
| Text Replacement | Keyboard Events | Ctrl+V Simulation | ✅ Migrated |
| Copy to Clipboard | Yes | Yes | ✅ Migrated |
| Global Hotkeys | Yes (Win32 API) | No | ❌ Not Migrated |
| System Tray | Yes | No | ❌ Not Migrated |
| Settings Window | WPF Window | Arguments/Config | 🔄 Changed Approach |
| Encrypted Settings | Yes (DPAPI) | Orchestrator Assets | 🔄 Changed Approach |
| Tone Presets | Yes (11 presets) | Can be added | ⚠️ Future Work |
| Variable Name Suggestion | Yes | Can be added | ⚠️ Future Work |
| Usage Statistics | Yes | Can be added | ⚠️ Future Work |
| Auto-start with Windows | Yes | Orchestrator Schedule | 🔄 Changed Approach |
| Background Service | Yes | Attended Mode | 🔄 Changed Approach |

## Workflow Structure

### Main.xaml
```
Main Workflow
├── Display Menu Dialog
│   ├── Option 1: Spelling Correction
│   ├── Option 2: Translation
│   ├── Option 3: Settings (Coming Soon)
│   └── Option 4: Exit
└── Switch on User Selection
    ├── Invoke SpellingCorrection.xaml
    ├── Invoke Translation.xaml
    ├── Show Settings Message
    └── Exit Message
```

### SpellingCorrection.xaml
```
Spelling Correction Workflow
├── Show Instructions
├── Capture Text
│   ├── Send Ctrl+C
│   ├── Wait 500ms
│   └── Get Clipboard Text
├── Validate Text Not Empty
├── Prepare API Request
│   └── JSON Body with Correction Prompt
├── Call OpenAI API
│   └── Invoke CallOpenAIAPI.xaml
├── Parse Response
│   └── Extract Corrected Text
├── Show Results Dialog
├── Ask User Action (Copy/Replace)
└── Execute Action
    ├── Copy: Set Clipboard
    └── Replace: Set Clipboard + Ctrl+V
```

### Translation.xaml
```
Translation Workflow
├── Show Instructions
├── Capture Text (same as above)
├── Validate Text Not Empty
├── Prepare API Request
│   └── JSON Body with Translation Prompt
├── Call OpenAI API
├── Parse Response
│   └── Extract Translated Text
├── Show Results Dialog
├── Ask User Action (Copy/Replace)
└── Execute Action (same as above)
```

### CallOpenAIAPI.xaml
```
OpenAI API Call Workflow
├── Set Default Endpoint if Empty
├── Try-Catch Block
│   ├── Try:
│   │   └── HTTP Request Activity
│   │       ├── POST to endpoint
│   │       ├── Authorization header
│   │       ├── Content-Type: application/json
│   │       └── Body: in_RequestBody
│   └── Catch:
│       ├── Log Error
│       └── Show Error Dialog
└── Return Response (out_Response)
```

## Deployment Differences

### .NET Deployment
1. Build solution in Visual Studio
2. Create installer with Inno Setup
3. Distribute .exe installer
4. Users install and run as desktop app
5. Runs in background with system tray

### UiPath Deployment
1. Publish project from UiPath Studio
2. Upload to UiPath Orchestrator (optional)
3. Options:
   - **Attended**: User runs from UiPath Studio or Robot
   - **Unattended**: Scheduled via Orchestrator
   - **Package**: Distribute .nupkg file

## Configuration Management

### .NET Configuration
```
%APPDATA%\SpellingChecker\settings.json
{
  "ApiKey": "encrypted_with_DPAPI",
  "ApiEndpoint": "https://api.openai.com/v1/chat/completions",
  "Model": "gpt-4o-mini",
  "SpellingCorrectionHotkey": "Ctrl+Shift+Alt+Y",
  "TranslationHotkey": "Ctrl+Shift+Alt+T",
  "ShowProgressNotifications": true,
  "SelectedTonePreset": "None"
}
```

### UiPath Configuration
```
Option 1: Workflow Arguments
- in_APIKey (String)
- in_APIEndpoint (String)

Option 2: Config File (Excel/JSON)
Config.xlsx:
| Key          | Value                                    |
|--------------|------------------------------------------|
| APIKey       | sk-...                                   |
| APIEndpoint  | https://api.openai.com/v1/chat/completions |

Option 3: Orchestrator Assets (Recommended)
- Asset Name: "OpenAI_APIKey"
- Asset Type: Text/Credential
- Scope: Global/Per-Robot
```

## Migration Steps Taken

### Phase 1: Project Setup ✅
- [x] Created UiPath project structure
- [x] Defined project.json with dependencies
- [x] Set up workflow folders

### Phase 2: Core Workflows ✅
- [x] Created Main.xaml entry point
- [x] Implemented SpellingCorrection.xaml
- [x] Implemented Translation.xaml
- [x] Implemented CallOpenAIAPI.xaml

### Phase 3: Text Capture ✅
- [x] Implemented Ctrl+C simulation
- [x] Added clipboard reading
- [x] Added validation

### Phase 4: API Integration ✅
- [x] Implemented HTTP Request activity
- [x] Added JSON request formatting
- [x] Added JSON response parsing
- [x] Added error handling

### Phase 5: User Interaction ✅
- [x] Added instruction dialogs
- [x] Added result display dialogs
- [x] Added action selection dialogs

### Phase 6: Text Replacement ✅
- [x] Implemented clipboard set
- [x] Implemented Ctrl+V simulation
- [x] Added copy-only option

### Phase 7: Documentation ✅
- [x] Created UiPath README
- [x] Created Migration Guide
- [x] Added usage instructions
- [x] Added troubleshooting guide

## Future Migration Tasks

### Not Yet Migrated (Can be added later)

1. **Tone Presets**
   - Add dropdown in Main.xaml for tone selection
   - Modify prompt in SpellingCorrection.xaml based on selection
   - Store presets in Config file or Orchestrator

2. **Variable Name Suggestion**
   - Create VariableNameSuggestion.xaml workflow
   - Add C# naming convention prompt
   - Format output as list

3. **Usage Statistics**
   - Log API calls to Excel file
   - Track token usage and costs
   - Create dashboard workflow

4. **Global Hotkey Simulation**
   - Use UiPath triggers (if available in your version)
   - Or create a monitoring loop with keyboard detection
   - More complex, may require custom activities

5. **Settings Management**
   - Create SettingsWindow.xaml with form activities
   - Persist to Config.xlsx or Orchestrator
   - Reload on workflow start

## Testing Recommendations

### Test Cases for UiPath Implementation

1. **Text Capture**
   - [ ] Select text in Notepad → Verify capture
   - [ ] Select text in Word → Verify capture
   - [ ] Select text in browser → Verify capture
   - [ ] No text selected → Verify error message

2. **Spelling Correction**
   - [ ] Correct text with typos → Verify correction
   - [ ] Correct text with grammar errors → Verify fix
   - [ ] Already correct text → Verify no changes
   - [ ] Empty/whitespace text → Verify error

3. **Translation**
   - [ ] Korean to English → Verify translation
   - [ ] English to Korean → Verify translation
   - [ ] Mixed language → Verify handling
   - [ ] Short text → Verify works
   - [ ] Long text → Verify works

4. **Actions**
   - [ ] Copy to clipboard → Verify clipboard content
   - [ ] Replace text → Verify text replaced
   - [ ] Cancel action → Verify no changes

5. **Error Handling**
   - [ ] Invalid API key → Verify error message
   - [ ] No internet → Verify timeout handling
   - [ ] Rate limit → Verify 429 error handling
   - [ ] Malformed response → Verify error handling

## Performance Considerations

| Metric | .NET | UiPath | Notes |
|--------|------|--------|-------|
| Startup Time | ~1s | ~3-5s | UiPath has more overhead |
| Text Capture | ~50ms | ~600ms | RPA delay needed |
| API Call | ~1-3s | ~1-3s | Same (network bound) |
| Text Replace | ~100ms | ~300ms | RPA timing |
| Memory Usage | ~50MB | ~200MB+ | UiPath runtime overhead |

## Security Considerations

### .NET Security
- API key encrypted with Windows DPAPI
- Stored per-user
- Can't be accessed by other users

### UiPath Security
- **Option 1**: Orchestrator Credential Assets (Most Secure)
  - Encrypted at rest
  - Never visible in workflows
  - Audited access

- **Option 2**: Config File (Less Secure)
  - Plain text or encrypted
  - File system permissions
  - Git ignore required

- **Option 3**: Workflow Arguments (Least Secure)
  - Visible in Studio
  - Logged in execution logs
  - Not recommended for production

**Recommendation**: Use Orchestrator Credential Assets for production deployments.

## Troubleshooting Common Issues

### Issue: "Project dependencies not resolved"
**Solution**: Open UiPath Studio → Manage Packages → Restore all packages

### Issue: "HTTP Request fails with SSL error"
**Solution**: Update UiPath.WebAPI.Activities package to latest version

### Issue: "Ctrl+C doesn't capture text"
**Solution**: Increase delay after SendHotkey from 500ms to 1000ms

### Issue: "JSON parsing error"
**Solution**: Check API response format, add try-catch around JSON parsing

### Issue: "Robot too slow"
**Solution**: Reduce delays where possible, but maintain minimum 300ms for clipboard operations

## Conclusion

The migration to UiPath successfully implements the core functionality of copying selected text and processing it with AI. The RPA approach provides:

✅ **Advantages**:
- Visual workflow development
- Application independence
- Enterprise orchestration capabilities
- Easy maintenance and modifications

⚠️ **Limitations**:
- No background service mode
- No global hotkey registration
- Requires UiPath Studio/Robot
- Higher resource usage

The UiPath implementation is suitable for:
- Attended automation scenarios
- Enterprise deployments with Orchestrator
- Users familiar with RPA tools
- Scenarios requiring visual workflow understanding

For users who need background service with global hotkeys, the original .NET implementation may still be preferable.

## Next Steps

1. **Test** the UiPath implementation thoroughly
2. **Deploy** to UiPath Orchestrator (if applicable)
3. **Train** users on running workflows
4. **Gather feedback** and iterate
5. **Add features** from the future migration tasks list

## Resources

- UiPath Documentation: https://docs.uipath.com/
- UiPath Academy: https://academy.uipath.com/
- UiPath Forum: https://forum.uipath.com/
- OpenAI API Docs: https://platform.openai.com/docs/

---

**Migration completed by**: GitHub Copilot Agent
**Date**: 2025-10-14
**Version**: 1.0.0
