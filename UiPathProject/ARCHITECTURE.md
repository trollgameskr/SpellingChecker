# UiPath SpellingChecker Architecture

This document explains the architecture and workflow design of the UiPath RPA implementation.

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         User                                 │
│                           │                                  │
│                           ▼                                  │
│  ┌───────────────────────────────────────────────────┐     │
│  │        Any Application (Notepad, Word, etc.)       │     │
│  │              [Selected Text]                       │     │
│  └───────────────────────────────────────────────────┘     │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ User runs workflow
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    UiPath Robot                              │
│                                                               │
│  ┌────────────────────────────────────────────────────┐    │
│  │              Main.xaml                              │    │
│  │         [Entry Point Workflow]                      │    │
│  │                                                      │    │
│  │  - Shows menu dialog                                │    │
│  │  - Routes to sub-workflows                          │    │
│  │  - Manages configuration                            │    │
│  └──────────┬─────────────────────┬────────────────────┘    │
│             │                     │                          │
│             ▼                     ▼                          │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ Spelling         │  │ Translation      │                │
│  │ Correction.xaml  │  │ .xaml            │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
│           │                     │                            │
│           └─────────┬───────────┘                            │
│                     │                                        │
│                     ▼                                        │
│  ┌────────────────────────────────────────┐                │
│  │     Text Capture Sequence              │                │
│  │  1. Send Ctrl+C                        │                │
│  │  2. Wait 500ms                         │                │
│  │  3. Get Clipboard Text                 │                │
│  └────────────────────────────────────────┘                │
│                     │                                        │
│                     ▼                                        │
│  ┌────────────────────────────────────────┐                │
│  │     CallOpenAIAPI.xaml                 │                │
│  │  - HTTP POST request                   │                │
│  │  - Authorization header                │                │
│  │  - JSON request/response               │                │
│  └────────────────────────────────────────┘                │
│                     │                                        │
└─────────────────────┼────────────────────────────────────────┘
                      │ HTTPS
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                   OpenAI API                                 │
│               (api.openai.com)                               │
│                                                               │
│  - GPT-4o-mini model                                         │
│  - Spelling correction                                       │
│  - Translation (Korean ↔ English)                           │
└─────────────────────────────────────────────────────────────┘
```

## Workflow Hierarchy

```
Main.xaml (Root)
│
├─→ SpellingCorrection.xaml
│   │
│   ├─→ Text Capture Sequence
│   │   ├─ SendHotkey (Ctrl+C)
│   │   ├─ Delay (500ms)
│   │   └─ GetClipboardText
│   │
│   ├─→ CallOpenAIAPI.xaml
│   │   └─ HTTP Request Activity
│   │
│   └─→ Text Action Sequence
│       ├─ Option 1: SetClipboardText
│       └─ Option 2: SetClipboardText + SendHotkey (Ctrl+V)
│
├─→ Translation.xaml
│   │
│   ├─→ Text Capture Sequence (same as above)
│   │
│   ├─→ CallOpenAIAPI.xaml (reused)
│   │
│   └─→ Text Action Sequence (same as above)
│
└─→ Settings (Future)
```

## Data Flow

### Spelling Correction Flow

```
┌─────────────────────────────────────────────────────────────┐
│ 1. User Action                                               │
│    - User selects text in application                        │
│    - User runs Main.xaml                                     │
│    - User clicks "Spelling Correction"                       │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 2. Text Capture (SpellingCorrection.xaml)                   │
│    Input:  None                                              │
│    Process:                                                  │
│      - SendHotkey: Ctrl+C                                    │
│      - Delay: 500ms                                          │
│      - GetClipboardText → SelectedText                       │
│    Output: SelectedText (String)                             │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 3. Validation                                                │
│    - If SelectedText is empty → Show error, exit             │
│    - If SelectedText has content → Continue                  │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 4. API Request Preparation                                   │
│    - Build JSON request body:                                │
│      {                                                        │
│        "model": "gpt-4o-mini",                               │
│        "messages": [                                         │
│          {                                                    │
│            "role": "system",                                 │
│            "content": "You are a spelling corrector..."      │
│          },                                                   │
│          {                                                    │
│            "role": "user",                                   │
│            "content": <SelectedText>                         │
│          }                                                    │
│        ],                                                     │
│        "temperature": 0.3                                    │
│      }                                                        │
│    Output: RequestBody (String)                              │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 5. API Call (CallOpenAIAPI.xaml)                            │
│    Input:  in_APIKey, in_APIEndpoint, in_RequestBody        │
│    Process:                                                  │
│      - HTTP POST to endpoint                                 │
│      - Headers: Authorization, Content-Type                  │
│      - Body: RequestBody                                     │
│      - Timeout: 30 seconds                                   │
│    Output: out_Response (JSON String)                        │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 6. Response Parsing                                          │
│    - Parse JSON response                                     │
│    - Extract: response.choices[0].message.content            │
│    - Trim whitespace                                         │
│    Output: CorrectedText (String)                            │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 7. Result Display                                            │
│    - MessageBox showing:                                     │
│      Original: <SelectedText>                                │
│      Corrected: <CorrectedText>                              │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 8. User Action Selection                                     │
│    - InputDialog with options:                               │
│      1. Copy to Clipboard                                    │
│      2. Replace Selected Text                                │
│    Output: UserAction (String)                               │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│ 9. Execute Action                                            │
│    If UserAction = "Copy to Clipboard":                      │
│      - SetClipboardText: CorrectedText                       │
│                                                               │
│    If UserAction = "Replace Selected Text":                  │
│      - SetClipboardText: CorrectedText                       │
│      - SendHotkey: Ctrl+V                                    │
│                                                               │
│    Result: Text copied or replaced                           │
└─────────────────────────────────────────────────────────────┘
```

### Translation Flow

Translation follows the same pattern but with different API prompt:

```
System Message: "You are a translator. If text is Korean, translate to 
                 English. If text is English, translate to Korean."
```

## Component Details

### Main.xaml

**Purpose**: Application entry point and router

**Variables**:
- `UserMenuSelection` (String): User's menu choice

**Arguments**:
- `in_APIKey` (String, In): OpenAI API key
- `in_APIEndpoint` (String, In): API endpoint URL

**Activities**:
1. `LogMessage`: Log application start
2. `InputDialog`: Show menu to user
3. `Switch`: Route based on selection
   - Case "1. Spelling Correction" → Invoke SpellingCorrection.xaml
   - Case "2. Translation" → Invoke Translation.xaml
   - Case "3. Settings" → Show "Coming soon" message
   - Case "4. Exit" → Show exit message

**Error Handling**: None (errors handled in sub-workflows)

### SpellingCorrection.xaml

**Purpose**: Capture text, correct spelling, display results

**Variables**:
- `SelectedText` (String): Text captured from clipboard
- `CorrectedText` (String): AI-corrected text
- `APIResponse` (String): Raw API response
- `RequestBody` (String): JSON request to API
- `UserAction` (String): Copy or Replace choice

**Arguments**:
- `in_APIKey` (String, In): OpenAI API key
- `in_APIEndpoint` (String, In): API endpoint URL

**Sequences**:

1. **Text Capture Sequence**
   - `MessageBox`: Show instructions
   - `SendHotkey`: Simulate Ctrl+C
   - `Delay`: Wait 500ms
   - `GetClipboardText`: Read clipboard → SelectedText
   - `LogMessage`: Log captured text

2. **Validation Sequence**
   - `If`: Check if SelectedText is empty
     - Then: Show error and exit
     - Else: Continue

3. **API Processing Sequence**
   - `Assign`: Build RequestBody JSON
   - `InvokeWorkflowFile`: Call CallOpenAIAPI.xaml
   - `Assign`: Parse response → CorrectedText
   - `MessageBox`: Show original vs corrected

4. **Action Execution Sequence**
   - `InputDialog`: Ask user action
   - `If`: Check UserAction
     - Replace: `SetClipboardText` + `SendHotkey` (Ctrl+V)
     - Copy: `SetClipboardText` only

**Error Handling**: Try-Catch in CallOpenAIAPI.xaml

### Translation.xaml

**Purpose**: Capture text, translate, display results

**Structure**: Identical to SpellingCorrection.xaml except:
- Different system prompt in RequestBody
- Variables: `TranslatedText` instead of `CorrectedText`
- Display: Shows translation instead of correction

### CallOpenAIAPI.xaml

**Purpose**: Reusable HTTP request handler for OpenAI API

**Variables**:
- `ResponseContent` (String): API response body
- `StatusCode` (Int32): HTTP status code

**Arguments**:
- `in_APIKey` (String, In): API key
- `in_APIEndpoint` (String, In): Endpoint URL
- `in_RequestBody` (String, In): JSON request body
- `out_Response` (String, Out): JSON response

**Activities**:
1. `LogMessage`: Log API call
2. `Assign`: Set default endpoint if empty
3. `TryCatch`:
   - Try:
     - `HttpClient`: Make POST request
       - Endpoint: in_APIEndpoint
       - Method: POST
       - Headers:
         - Authorization: Bearer {in_APIKey}
         - Content-Type: application/json
       - Body: in_RequestBody
       - Timeout: 30000ms
   - Catch (Exception):
     - `LogMessage`: Log error
     - `MessageBox`: Show error to user
4. `Assign`: Set out_Response from ResponseContent

**Error Handling**: Catches all exceptions, logs them, shows to user

## Activity Types Used

| Activity | Purpose | Package |
|----------|---------|---------|
| `Sequence` | Group activities | System.Activities |
| `Assign` | Set variable values | System.Activities |
| `If` | Conditional branching | System.Activities |
| `Switch` | Multi-way branching | System.Activities |
| `TryCatch` | Error handling | System.Activities |
| `LogMessage` | Log to Output panel | UiPath.System.Activities |
| `MessageBox` | Show messages to user | UiPath.System.Activities |
| `InputDialog` | Get user selection | UiPath.System.Activities |
| `GetClipboardText` | Read from clipboard | UiPath.System.Activities |
| `SetClipboardText` | Write to clipboard | UiPath.System.Activities |
| `SendHotkey` | Simulate key press | UiPath.UIAutomation.Activities |
| `Delay` | Wait for duration | UiPath.System.Activities |
| `InvokeWorkflowFile` | Call sub-workflow | UiPath.System.Activities |
| `HttpClient` | Make HTTP request | UiPath.WebAPI.Activities |
| `Comment` | Add documentation | UiPath.System.Activities |

## Variable Scope

```
Main.xaml
├─ UserMenuSelection (Main scope)
│
├─→ SpellingCorrection.xaml
│   ├─ SelectedText (SpellingCorrection scope)
│   ├─ CorrectedText (SpellingCorrection scope)
│   ├─ APIResponse (SpellingCorrection scope)
│   ├─ RequestBody (SpellingCorrection scope)
│   └─ UserAction (SpellingCorrection scope)
│
├─→ Translation.xaml
│   ├─ SelectedText (Translation scope)
│   ├─ TranslatedText (Translation scope)
│   ├─ APIResponse (Translation scope)
│   ├─ RequestBody (Translation scope)
│   └─ UserAction (Translation scope)
│
└─→ CallOpenAIAPI.xaml
    ├─ ResponseContent (CallOpenAIAPI scope)
    └─ StatusCode (CallOpenAIAPI scope)
```

## Argument Flow

```
User Input
    │
    ▼
in_APIKey ────────────┐
in_APIEndpoint ───────┤
                      │
                      ▼
                 Main.xaml
                      │
                      ├─────────────────┐
                      │                 │
                      ▼                 ▼
         SpellingCorrection.xaml  Translation.xaml
                      │                 │
         in_APIKey    │    in_APIKey    │
         in_APIEndpoint│   in_APIEndpoint│
         in_RequestBody│   in_RequestBody│
                      │                 │
                      └─────────┬───────┘
                                │
                                ▼
                      CallOpenAIAPI.xaml
                                │
                       out_Response
                                │
                                ▼
                      Parse & Display
```

## Error Handling Strategy

### Level 1: Activity Level
```
Try:
  HTTP Request
Catch:
  Log error
  Show MessageBox
  Continue workflow
```

### Level 2: Workflow Level
```
Validate Input:
  If empty → Show error, exit
  If valid → Continue

Validate Output:
  If API error → Already caught
  If parsing error → Show error
```

### Level 3: User Guidance
- Clear instruction dialogs before actions
- Informative error messages
- Log all operations for debugging

## Performance Characteristics

| Operation | Duration | Notes |
|-----------|----------|-------|
| Workflow startup | 2-3s | UiPath initialization |
| Text capture | 600ms | Ctrl+C + 500ms delay |
| API request | 1-5s | Network dependent |
| Response parsing | <100ms | JSON parsing |
| Text replacement | 200ms | Ctrl+V simulation |
| **Total** | **3-9s** | End-to-end |

## Scalability Considerations

### Current Design
- ✅ Single-threaded (one workflow at a time)
- ✅ Synchronous API calls
- ✅ Attended automation (requires user)

### Future Enhancements
- 🔄 Parallel processing (multiple texts)
- 🔄 Async API calls with callbacks
- 🔄 Unattended automation (scheduled)
- 🔄 Queue-based processing (Orchestrator)

## Security Model

### API Key Handling
```
Storage Options:
1. Workflow Arguments (Development)
   - Visible in Studio
   - Not encrypted
   - Use only for testing

2. Config File (Testing)
   - Encrypted file recommended
   - .gitignore required
   - Per-machine storage

3. Orchestrator Assets (Production)
   - Encrypted at rest
   - Centralized management
   - Audited access
```

### Data Flow Security
```
Selected Text ──→ Clipboard ──→ UiPath Variable ──→ HTTPS ──→ OpenAI
                                                    (TLS 1.2+)
    ↑                                                  ↓
    │                                              Response
    │                                                  ↓
    └──────────────────── Paste ←─────────────── Parse
```

## Monitoring & Logging

### Log Levels
- **Info**: Normal operations (start, API call, success)
- **Warn**: Recoverable issues (empty selection, retry)
- **Error**: Failures (API error, parsing error)

### Log Locations
1. **UiPath Studio**: Output panel (bottom)
2. **Robot Logs**: `%LocalAppData%\UiPath\Logs`
3. **Orchestrator**: Centralized logging (if deployed)

### Log Examples
```
[Info] Starting Spelling Correction Workflow
[Info] Attempting to get selected text via clipboard (Ctrl+C)
[Info] Selected text: This is a test
[Info] Sending request to OpenAI API...
[Info] Received response from OpenAI API
[Error] API call failed: 401 Unauthorized
```

## Maintenance & Updates

### Updating Workflows
1. Open project in UiPath Studio
2. Modify workflow xaml files
3. Test in Debug mode
4. Publish new version
5. Deploy to Orchestrator (if applicable)

### Updating Dependencies
1. Click Manage Packages (Ctrl+P)
2. Check for updates
3. Update packages
4. Test workflows
5. Commit updated project.json

### Version Control
- Use Git for workflow versions
- Tag releases (v1.0.0, v1.1.0, etc.)
- Keep migration notes in commit messages

## Conclusion

This architecture provides:
- ✅ Clear separation of concerns
- ✅ Reusable components (CallOpenAIAPI)
- ✅ Extensible design (easy to add new workflows)
- ✅ Robust error handling
- ✅ User-friendly interactions

The design balances simplicity for users with flexibility for developers.
