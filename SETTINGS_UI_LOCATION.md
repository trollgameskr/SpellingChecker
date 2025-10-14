# Settings UI - Variable Name Suggestion Hotkey Location

## Settings Window Layout (설정 창 레이아웃)

```
┌──────────────────────────────────────────────────────┐
│  Settings - AI Spelling Checker                      │
├──────────────────────────────────────────────────────┤
│                                                       │
│  Application Settings                                │
│                                                       │
│  OpenAI API Key:                                     │
│  [****************************************]           │
│  Required for AI-powered spelling correction...      │
│                                                       │
│  API Endpoint:                                       │
│  [https://api.openai.com/v1              ]           │
│  Default: https://api.openai.com/v1                  │
│                                                       │
│  AI Model:                                           │
│  [gpt-4o-mini                            ]           │
│  Default: gpt-4o-mini (recommended...)               │
│                                                       │
│  ☑ Start with Windows                                │
│    Launch the application automatically...           │
│                                                       │
│  ☑ Show progress notifications                       │
│    Display desktop notifications for...              │
│                                                       │
│  Hotkeys:                                            │
│                                                       │
│    Spelling Correction Hotkey:                       │
│    [Ctrl+Shift+Alt+Y                     ]           │
│    Example: Ctrl+Shift+Alt+Y                         │
│                                                       │
│    Translation Hotkey:                               │
│    [Ctrl+Shift+Alt+T                     ]           │
│    Example: Ctrl+Shift+Alt+T                         │
│                                                       │
│  ┌──────────────────────────────────────────────┐    │
│  │  Variable Name Suggestion Hotkey:            │    │  ← 여기! (Here!)
│  │  [Ctrl+Shift+Alt+V                     ]     │    │
│  │  Example: Ctrl+Shift+Alt+V                   │    │
│  └──────────────────────────────────────────────┘    │
│                                                       │
│  문장 톤 프리셋:                                      │
│  [기본 (Default) ▼                       ]           │
│  맞춤법 교정 시 적용할 문장 톤을 선택하세요.         │
│                                                       │
│  [➕ 추가]  [✏️ 수정]  [🗑️ 삭제]                   │
│                                                       │
│  [📊 View Usage Statistics]                          │
│  Track your API usage, token consumption...          │
│                                                       │
│                               [Save]  [Cancel]       │
│                                                       │
└──────────────────────────────────────────────────────┘
```

## Step-by-Step Instructions (단계별 안내)

### 1. Open Settings (설정 열기)
```
System Tray → Right-click SpellingChecker Icon → Settings
시스템 트레이 → SpellingChecker 아이콘 우클릭 → Settings
```

### 2. Scroll to Hotkeys Section (단축키 섹션으로 스크롤)
- Look for "Hotkeys:" heading
- "Hotkeys:" 제목을 찾습니다

### 3. Find Variable Name Suggestion Hotkey (변수명 추천 단축키 찾기)
- Located below "Translation Hotkey"
- Above "문장 톤 프리셋" section
- "Translation Hotkey" 아래에 있습니다
- "문장 톤 프리셋" 섹션 위에 있습니다

### 4. Edit the Hotkey (단축키 편집)
```
Current:  Ctrl+Shift+Alt+V
          ↓ (click and type)
New:      Ctrl+Alt+N  (or any valid combination)
```

### 5. Save Changes (변경사항 저장)
- Click the "Save" button at the bottom
- Restart the application
- 하단의 "Save" 버튼 클릭
- 애플리케이션 재시작

## XAML Code Reference (XAML 코드 참조)

From `SpellingChecker/Views/SettingsWindow.xaml` (Lines 114-124):

```xml
<!-- Variable Name Suggestion Hotkey -->
<StackPanel>
    <TextBlock Text="Variable Name Suggestion Hotkey:" 
               FontSize="12" 
               Margin="0,0,0,3"/>
    <TextBox x:Name="VariableNameSuggestionHotkeyTextBox" 
             Padding="5"
             Height="30"/>
    <TextBlock Text="Example: Ctrl+Shift+Alt+V" 
               FontSize="10" 
               Foreground="Gray" 
               Margin="0,3,0,0"/>
</StackPanel>
```

## C# Code Reference (C# 코드 참조)

From `SpellingChecker/Views/SettingsWindow.xaml.cs`:

### Loading Settings (설정 로드)
```csharp
private void LoadSettings()
{
    // ... other settings ...
    VariableNameSuggestionHotkeyTextBox.Text = _settings.VariableNameSuggestionHotkey;
    // ... other settings ...
}
```

### Saving Settings (설정 저장)
```csharp
private void SaveButton_Click(object sender, RoutedEventArgs e)
{
    // Validate hotkey format
    if (!HotkeyParser.IsValidHotkey(VariableNameSuggestionHotkeyTextBox.Text))
    {
        MessageBox.Show("Invalid variable name suggestion hotkey format. " +
                       "Please use format like 'Ctrl+Shift+Alt+V'", 
                       "Error", 
                       MessageBoxButton.OK, 
                       MessageBoxImage.Error);
        return;
    }
    
    // Save the hotkey
    _settings.VariableNameSuggestionHotkey = VariableNameSuggestionHotkeyTextBox.Text;
    _settingsService.SaveSettings(_settings);
    
    MessageBox.Show("Settings saved successfully! " +
                   "Please restart the application for hotkey changes to take effect.", 
                   "Success", 
                   MessageBoxButton.OK, 
                   MessageBoxImage.Information);
}
```

## Valid Hotkey Examples (유효한 단축키 예시)

| Example | Description |
|---------|-------------|
| `Ctrl+Shift+Alt+V` | Default (기본값) |
| `Ctrl+Alt+N` | Simple combination |
| `Win+Shift+V` | Using Windows key |
| `Ctrl+Shift+F1` | Function key |
| `Ctrl+Alt+Shift+D` | Letter key |

## Feature is Already Implemented! (기능이 이미 구현되어 있습니다!)

✅ **Model**: `AppSettings.VariableNameSuggestionHotkey` property exists  
✅ **UI**: `VariableNameSuggestionHotkeyTextBox` control exists in XAML  
✅ **Logic**: Load/Save/Validation code implemented  
✅ **Service**: HotkeyService registers and handles the hotkey  
✅ **Build**: Project compiles successfully without errors  

**No code changes needed!** (코드 변경 불필요!)

The feature requested is already complete and working.  
요청하신 기능은 이미 완성되어 작동 중입니다.
