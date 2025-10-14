# Variable Name Suggestion Hotkey Configuration Guide

## 개요 (Overview)

변수명 추천 기능의 단축키는 이미 설정에서 변경할 수 있도록 구현되어 있습니다.
The variable name suggestion feature hotkey is already configurable in the application settings.

## 단축키 변경 방법 (How to Change the Hotkey)

### 1단계: 설정 창 열기 (Step 1: Open Settings)
- 시스템 트레이에서 애플리케이션 아이콘을 찾습니다
- 아이콘을 마우스 오른쪽 버튼으로 클릭합니다
- "Settings" 메뉴를 클릭합니다

### 2단계: 변수명 추천 단축키 찾기 (Step 2: Locate Variable Name Suggestion Hotkey)
- 설정 창에서 "Hotkeys:" 섹션을 찾습니다
- "Variable Name Suggestion Hotkey:" 필드가 있습니다
- 현재 설정된 단축키가 표시됩니다 (기본값: `Ctrl+Shift+Alt+V`)

### 3단계: 단축키 변경 (Step 3: Change the Hotkey)
- "Variable Name Suggestion Hotkey:" 텍스트 박스를 클릭합니다
- 원하는 단축키 조합을 입력합니다
  - 예시: `Ctrl+Shift+Alt+V`, `Ctrl+Alt+N`, `Ctrl+Shift+V` 등
  - 형식: 수정자(Ctrl, Shift, Alt, Win) + 키
  - 최소 하나 이상의 수정자 키가 필요합니다

### 4단계: 저장 및 재시작 (Step 4: Save and Restart)
- "Save" 버튼을 클릭합니다
- 애플리케이션을 재시작합니다 (변경사항 적용을 위해)
- 새로운 단축키가 적용됩니다

## 단축키 형식 예시 (Hotkey Format Examples)

✅ **올바른 형식 (Valid formats):**
- `Ctrl+Shift+Alt+V`
- `Ctrl+Alt+N`
- `Ctrl+Shift+V`
- `Win+Shift+V`
- `Ctrl+Shift+Alt+F1`

❌ **잘못된 형식 (Invalid formats):**
- `V` (수정자 키 없음 / no modifier key)
- `Ctrl+V` (전역 단축키로는 너무 일반적 / too common for global hotkey)
- `InvalidKey` (유효하지 않은 키 / invalid key name)

## 기능 검증 (Feature Verification)

변수명 추천 기능이 제대로 작동하는지 확인하려면:

1. 텍스트 편집기에서 한글 텍스트를 선택합니다
   예: "사용자 이름"

2. 설정한 단축키를 누릅니다
   기본값: `Ctrl+Shift+Alt+V`

3. 팝업 창에 3개의 변수명 제안이 표시됩니다
   예:
   ```
   1. userName
   2. userId
   3. userIdentifier
   ```

4. 제안을 클립보드에 복사하거나 선택할 수 있습니다

## 구현 세부사항 (Implementation Details)

### 파일 위치 (File Locations)
- **모델**: `SpellingChecker/Models/Models.cs` - `AppSettings.VariableNameSuggestionHotkey`
- **UI**: `SpellingChecker/Views/SettingsWindow.xaml` - `VariableNameSuggestionHotkeyTextBox`
- **설정 로직**: `SpellingChecker/Views/SettingsWindow.xaml.cs` - `LoadSettings()`, `SaveButton_Click()`
- **단축키 등록**: `SpellingChecker/Services/HotkeyService.cs` - `RegisterHotkeys()`

### 기본 설정 (Default Configuration)
```csharp
public string VariableNameSuggestionHotkey { get; set; } = "Ctrl+Shift+Alt+V";
```

### 검증 로직 (Validation Logic)
```csharp
if (!HotkeyParser.IsValidHotkey(VariableNameSuggestionHotkeyTextBox.Text))
{
    MessageBox.Show("Invalid variable name suggestion hotkey format. Please use format like 'Ctrl+Shift+Alt+V'", 
        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    return;
}
```

## 문제 해결 (Troubleshooting)

### 단축키가 작동하지 않는 경우 (If the hotkey doesn't work)

1. **애플리케이션 재시작 확인**
   - 단축키 변경 후 반드시 애플리케이션을 재시작해야 합니다

2. **다른 애플리케이션과 충돌 확인**
   - 다른 프로그램이 동일한 단축키를 사용하고 있을 수 있습니다
   - 다른 단축키 조합을 시도해보세요

3. **단축키 형식 확인**
   - 올바른 형식으로 입력되었는지 확인하세요
   - 최소 하나 이상의 수정자 키(Ctrl, Shift, Alt, Win)가 필요합니다

4. **API 키 확인**
   - OpenAI API 키가 올바르게 설정되어 있는지 확인하세요
   - Settings에서 "OpenAI API Key" 필드를 확인하세요

## 추가 정보 (Additional Information)

- 자세한 기능 설명: [VARIABLE_NAME_SUGGESTION_FEATURE.md](VARIABLE_NAME_SUGGESTION_FEATURE.md)
- 테스트 가이드: [VARIABLE_NAME_SUGGESTION_TEST_GUIDE.md](VARIABLE_NAME_SUGGESTION_TEST_GUIDE.md)
- 구현 요약: [IMPLEMENTATION_SUMMARY_VARIABLE_NAME_SUGGESTION.md](IMPLEMENTATION_SUMMARY_VARIABLE_NAME_SUGGESTION.md)

## 결론 (Conclusion)

변수명 추천 기능의 단축키는 이미 완전히 구현되어 있으며, 설정 UI를 통해 쉽게 변경할 수 있습니다. 
추가 코드 변경 없이 바로 사용 가능합니다.

The variable name suggestion feature hotkey is already fully implemented and can be easily changed through the settings UI. 
It is ready to use without any additional code changes.
