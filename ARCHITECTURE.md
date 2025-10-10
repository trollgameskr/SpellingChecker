# Architecture Documentation

## Overview

SpellingChecker is a Windows desktop application built with WPF (Windows Presentation Foundation) that provides AI-powered spelling correction and translation services. The application runs in the background and responds to global hotkeys.

## Technology Stack

- **Framework**: .NET 9.0
- **UI**: WPF (Windows Presentation Foundation)
- **Language**: C# 12
- **AI Provider**: OpenAI GPT API
- **Security**: Windows Data Protection API (DPAPI)
- **JSON Processing**: Newtonsoft.Json

## Architecture Pattern

The application follows a **Service-Oriented Architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌───────────┬──────────────┬────────────────────────┐ │
│  │ MainWindow│ SettingsWindow│ ResultPopupWindow      │ │
│  │ (Hidden)  │              │                        │ │
│  └───────────┴──────────────┴────────────────────────┘ │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────┴────────────────────────────────────┐
│                    Service Layer                         │
│  ┌──────────┬────────────┬──────────────┬────────────┐ │
│  │ Hotkey   │ Clipboard  │ AI Service   │ Settings   │ │
│  │ Service  │ Service    │              │ Service    │ │
│  └──────────┴────────────┴──────────────┴────────────┘ │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────┴────────────────────────────────────┐
│                     Data Layer                          │
│  ┌──────────┬────────────┬──────────────────────────┐ │
│  │ Models   │ Settings   │ External APIs            │ │
│  │          │ Storage    │ (OpenAI)                 │ │
│  └──────────┴────────────┴──────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

## Component Design

### 1. Presentation Layer

#### MainWindow
- **Purpose**: Background window that hosts the application
- **Responsibilities**:
  - Initialize system tray icon
  - Register global hotkeys
  - Coordinate between services
  - Handle hotkey events
- **Visibility**: Hidden (runs in background)

#### SettingsWindow
- **Purpose**: User interface for configuration
- **Features**:
  - API key input (password field)
  - API endpoint configuration
  - Model selection
  - Auto-start toggle

#### ResultPopupWindow
- **Purpose**: Display correction/translation results
- **Features**:
  - Side-by-side comparison (original vs result)
  - Copy to clipboard button
  - Replace text button
  - Auto-positioning near cursor

### 2. Service Layer

#### HotkeyService
```csharp
// Registers and manages global Windows hotkeys
- RegisterHotkeys(IntPtr windowHandle): bool
- UnregisterHotkeys(): void
- ProcessHotkey(int hotkeyId): void
- Events: SpellingCorrectionRequested, TranslationRequested
```

**Implementation Details**:
- Uses Windows API (`user32.dll`)
- Registers `MOD_CONTROL | MOD_SHIFT` combinations
- Handles WM_HOTKEY messages
- Thread-safe event dispatching

#### ClipboardService
```csharp
// Handles clipboard operations and text selection
- GetSelectedText(): string
- SetClipboard(string text): void
- ReplaceSelectedText(string text): void
```

**Implementation Details**:
- Simulates Ctrl+C to capture selected text
- Preserves previous clipboard content when possible
- Uses keyboard events via `user32.dll`
- Implements delays for clipboard synchronization

#### AIService
```csharp
// Communicates with OpenAI API
- CorrectSpellingAsync(string text): Task<CorrectionResult>
- TranslateAsync(string text): Task<TranslationResult>
- DetectLanguage(string text): string
```

**Implementation Details**:
- Uses HttpClient for API communication
- Implements retry logic (future enhancement)
- Handles API errors gracefully
- Configurable model and temperature
- Language detection via Unicode ranges

#### SettingsService
```csharp
// Manages application settings with encryption
- LoadSettings(): AppSettings
- SaveSettings(AppSettings settings): void
```

**Implementation Details**:
- Uses Windows DPAPI for encryption
- Stores settings in `%APPDATA%\SpellingChecker\`
- Settings encrypted per-user
- JSON serialization with Newtonsoft.Json

### 3. Data Layer

#### Models

**CorrectionResult**
```csharp
{
    string OriginalText
    string CorrectedText
    string[] Changes
    bool HasChanges
}
```

**TranslationResult**
```csharp
{
    string OriginalText
    string TranslatedText
    string SourceLanguage
    string TargetLanguage
}
```

**AppSettings**
```csharp
{
    string ApiKey
    string ApiEndpoint
    string SpellingCorrectionHotkey
    string TranslationHotkey
    bool AutoStartWithWindows
    string Model
}
```

## Data Flow

### Spelling Correction Flow

```
1. User selects text in any application
2. User presses Ctrl+Shift+Y
3. HotkeyService detects hotkey → fires event
4. MainWindow receives event
5. ClipboardService captures selected text (Ctrl+C simulation)
6. AIService sends text to OpenAI API
7. API returns corrected text
8. ResultPopupWindow displays results
9. User clicks "Copy" or "Replace"
10. ClipboardService executes action
```

### Translation Flow

```
1. User selects text in any application
2. User presses Ctrl+Shift+T
3. HotkeyService detects hotkey → fires event
4. MainWindow receives event
5. ClipboardService captures selected text
6. AIService detects language (Korean/English)
7. AIService sends translation request to OpenAI
8. API returns translated text
9. ResultPopupWindow displays results
10. User clicks "Copy" or "Replace"
11. ClipboardService executes action
```

### Settings Flow

```
1. User opens Settings (tray icon)
2. SettingsWindow loads from SettingsService
3. SettingsService decrypts settings file
4. User modifies settings
5. User clicks Save
6. SettingsService encrypts and saves
7. Services reload settings on next use
```

## Security Architecture

### API Key Protection

1. **Input**: Plain text API key from user
2. **Process**:
   ```
   Plain Text → UTF-8 Bytes → DPAPI Encrypt → File Write
   ```
3. **Storage**: Encrypted binary in `%APPDATA%\SpellingChecker\settings.json`
4. **Retrieval**:
   ```
   File Read → DPAPI Decrypt → UTF-8 String → Use
   ```

### Security Measures

- **DPAPI Encryption**: Windows-integrated, per-user encryption
- **Entropy**: Additional salt for encryption strength
- **Scope**: CurrentUser (not machine-wide)
- **No Plain Text**: API key never stored unencrypted
- **Memory Safety**: Sensitive strings not logged

## Threading Model

### Main Thread (UI Thread)
- Window management
- Event handling
- UI updates

### Background Operations
- API calls (async/await)
- File I/O (settings load/save)
- Clipboard operations

### Synchronization
- All UI updates on UI thread (Dispatcher)
- Async operations with cancellation support (future)
- Thread-safe service access

## Error Handling

### Levels

1. **Service Level**
   - Catch and wrap exceptions
   - Provide meaningful error messages
   - Log errors (future enhancement)

2. **UI Level**
   - Display user-friendly error dialogs
   - Show balloon notifications
   - Graceful degradation

3. **Global Level**
   - Unhandled exception handler (future)
   - Crash reporting (future)

### Common Errors

| Error | Cause | Handling |
|-------|-------|----------|
| API key missing | User hasn't configured | Prompt to open settings |
| API call failure | Network/auth issues | Show error, suggest fixes |
| Hotkey registration fails | Conflict with other app | Warn user, continue running |
| Clipboard access denied | Security restrictions | Fallback to clipboard-only mode |

## Performance Considerations

### Optimization Strategies

1. **API Calls**
   - 30-second timeout
   - Async/await pattern
   - Single HttpClient instance (connection pooling)

2. **Memory**
   - Dispose services properly
   - Clear clipboard caches
   - Minimal background resource use

3. **Startup**
   - Lazy initialization where possible
   - Asynchronous settings load
   - Minimal UI (hidden window)

4. **UI Responsiveness**
   - All long operations async
   - UI never blocks on I/O
   - Popup windows separate from main thread

### Resource Usage

- **Memory**: ~50-100 MB (typical)
- **CPU**: <1% idle, 5-10% during processing
- **Network**: Burst usage during API calls
- **Disk**: <1 MB settings storage

## Extensibility Points

### Future Enhancements

1. **Plugin System**
   - Custom AI providers
   - Additional language processors
   - Custom correction rules

2. **Configuration**
   - Customizable hotkeys
   - Multiple API provider support
   - Theme customization

3. **Features**
   - Offline mode with local models
   - History/statistics tracking
   - Custom dictionaries
   - Browser extensions

### Design for Extension

- Services are interface-based (future)
- Dependency injection ready
- Loosely coupled components
- Event-driven architecture

## Deployment Architecture

### Installation

```
Installer (Inno Setup)
├── Application Files
│   ├── SpellingChecker.exe
│   ├── Dependencies (bundled in single file)
│   └── Documentation (README, LICENSE, etc.)
├── Registry Keys
│   └── Auto-start (if enabled)
└── Application Data
    └── %APPDATA%\SpellingChecker\
        └── settings.json (created on first run)
```

### Update Mechanism (Future)

```
1. Check for updates (GitHub Releases API)
2. Download new installer
3. Verify signature
4. Close application
5. Run installer
6. Restart application
```

## Testing Strategy

### Manual Testing
- End-to-end workflows
- Hotkey functionality
- UI interactions
- Settings persistence

### Automated Testing (Future)
- Unit tests for services
- Integration tests for API
- UI automation tests
- Performance benchmarks

## Monitoring and Diagnostics (Future)

### Logging
```
%APPDATA%\SpellingChecker\logs\
├── application.log
├── api-requests.log
└── errors.log
```

### Metrics
- API call count
- Response times
- Error rates
- User actions

## Compliance and Licensing

- **MIT License**: Open source, permissive
- **OpenAI Terms**: User must comply with OpenAI's terms
- **GDPR**: No personal data collected by app
- **Privacy**: User data only sent to OpenAI per their privacy policy

## Diagrams

### Component Interaction Diagram

```
┌─────────────┐
│   User      │
└──────┬──────┘
       │ (1) Selects text & presses hotkey
       ↓
┌──────────────────────────────────────────┐
│         Windows OS                       │
│  ┌────────────────────────────────────┐ │
│  │  Global Hotkey Message             │ │
│  └─────────────┬──────────────────────┘ │
└────────────────┼──────────────────────────┘
                 │ (2) WM_HOTKEY
                 ↓
┌──────────────────────────────────────────┐
│         MainWindow                       │
│  ┌────────────────────────────────────┐ │
│  │  HotkeyService.ProcessHotkey()     │ │
│  └─────────────┬──────────────────────┘ │
└────────────────┼──────────────────────────┘
                 │ (3) Event
                 ↓
┌──────────────────────────────────────────┐
│    OnSpellingCorrectionRequested()       │
└─────────────┬────────────────────────────┘
              │ (4) GetSelectedText()
              ↓
┌──────────────────────────────────────────┐
│      ClipboardService                    │
│  ┌────────────────────────────────────┐ │
│  │  Simulates Ctrl+C                  │ │
│  │  Captures clipboard                │ │
│  └─────────────┬──────────────────────┘ │
└────────────────┼──────────────────────────┘
                 │ (5) Returns text
                 ↓
┌──────────────────────────────────────────┐
│         AIService                        │
│  ┌────────────────────────────────────┐ │
│  │  CorrectSpellingAsync()            │ │
│  │  → OpenAI API Call                 │ │
│  └─────────────┬──────────────────────┘ │
└────────────────┼──────────────────────────┘
                 │ (6) Returns result
                 ↓
┌──────────────────────────────────────────┐
│      ResultPopupWindow                   │
│  ┌────────────────────────────────────┐ │
│  │  Display original & corrected      │ │
│  │  [Copy] [Replace] [Close]          │ │
│  └─────────────┬──────────────────────┘ │
└────────────────┼──────────────────────────┘
                 │ (7) User clicks action
                 ↓
         Back to ClipboardService
```

## Conclusion

This architecture provides:
- ✅ Clear separation of concerns
- ✅ Maintainable and testable code
- ✅ Extensible design
- ✅ Secure credential management
- ✅ Responsive user experience
- ✅ Production-ready foundation

For implementation details, see the source code in `SpellingChecker/` directory.
