# Tone Preset Feature - UI Visual Guide

## Settings Window Layout

### Window Dimensions
- **Previous**: 700px (height) × 600px (width)
- **Updated**: 800px (height) × 650px (width)
- **Reason**: Accommodate new tone preset section

---

## New UI Section: 문장 톤 프리셋

### Location
- **Grid Row**: 6 (inserted between Hotkeys and Usage Statistics sections)
- **Margin**: 0,0,0,15 (consistent with other sections)

### Components

#### 1. Section Header
```
문장 톤 프리셋:
[FontWeight: SemiBold]
```

#### 2. Tone Preset ComboBox
```
┌─────────────────────────────────────────────────────┐
│ 톤 없음                                        ▼    │
└─────────────────────────────────────────────────────┘
```
- **DisplayMemberPath**: Name
- **SelectedValuePath**: Id
- **Height**: 35px
- **Padding**: 5px

#### 3. Description TextBlock
```
원문의 톤을 그대로 유지합니다.
[FontSize: 10, Foreground: Gray, TextWrapping: Wrap]
```
- Updates dynamically based on selected tone
- Shows full tone description

#### 4. Management Buttons (Horizontal Stack)
```
┌─────────┐  ┌─────────┐  ┌─────────┐
│➕ 추가 │  │✏️ 수정 │  │🗑️ 삭제│
└─────────┘  └─────────┘  └─────────┘
```
- **Add Button** (➕ 추가):
  - Background: #4CAF50 (Green)
  - Foreground: White
  - Always enabled
  
- **Edit Button** (✏️ 수정):
  - Background: #2196F3 (Blue)
  - Foreground: White
  - Enabled only for custom presets
  
- **Delete Button** (🗑️ 삭제):
  - Background: #F44336 (Red)
  - Foreground: White
  - Enabled only for custom presets

---

## Tone Preset Dialog

### Window Properties
- **Title**: "톤 프리셋 관리" (changes to "새 톤 프리셋 추가" or "톤 프리셋 수정")
- **Size**: 300px (height) × 500px (width)
- **WindowStartupLocation**: CenterOwner
- **ResizeMode**: NoResize

### Dialog Layout

```
┌──────────────────────────────────────────────────┐
│  새 톤 프리셋 추가                               │
│                                                  │
│  프리셋 이름:                                    │
│  ┌────────────────────────────────────────────┐ │
│  │                                            │ │
│  └────────────────────────────────────────────┘ │
│                                                  │
│  톤 설명:                                        │
│  ┌────────────────────────────────────────────┐ │
│  │                                            │ │
│  │                                            │ │
│  │                                            │ │
│  └────────────────────────────────────────────┘ │
│  예: 권위 있고 엄격한 말투, 지시와 조언이 섞인 │
│  느낌                                            │
│                                                  │
│                                                  │
│                       ┌────────┐  ┌────────┐    │
│                       │  확인  │  │  취소  │    │
│                       └────────┘  └────────┘    │
└──────────────────────────────────────────────────┘
```

#### Components:
1. **Title TextBlock** (DialogTitle)
   - FontSize: 18
   - FontWeight: Bold
   - Margin: 0,0,0,20

2. **Preset Name TextBox**
   - Height: 30px
   - Padding: 5px
   - Single line input

3. **Preset Description TextBox**
   - Height: 80px
   - Padding: 5px
   - TextWrapping: Wrap
   - AcceptsReturn: True
   - VerticalScrollBarVisibility: Auto

4. **Helper Text**
   - FontSize: 10
   - Foreground: Gray
   - Shows example format

5. **OK Button** (확인)
   - Background: #4CAF50 (Green)
   - Foreground: White
   - Padding: 30,10

6. **Cancel Button** (취소)
   - Background: #9E9E9E (Gray)
   - Foreground: White
   - Padding: 30,10

---

## Settings Window - Complete Layout

```
┌────────────────────────────────────────────────────────────┐
│  Application Settings                                      │
│                                                            │
│  OpenAI API Key:                                          │
│  ┌──────────────────────────────────────────────────────┐ │
│  │ ••••••••••••••••••••                                 │ │
│  └──────────────────────────────────────────────────────┘ │
│  Required for AI-powered spelling correction              │
│                                                            │
│  API Endpoint:                                            │
│  ┌──────────────────────────────────────────────────────┐ │
│  │ https://api.openai.com/v1                            │ │
│  └──────────────────────────────────────────────────────┘ │
│  Default: https://api.openai.com/v1                       │
│                                                            │
│  AI Model:                                                │
│  ┌──────────────────────────────────────────────────────┐ │
│  │ gpt-4o-mini                                          │ │
│  └──────────────────────────────────────────────────────┘ │
│  Default: gpt-4o-mini (recommended)                       │
│                                                            │
│  ☑ Start with Windows                                     │
│  Launch the application automatically when Windows starts │
│                                                            │
│  Hotkeys:                                                 │
│                                                            │
│    Spelling Correction Hotkey:                           │
│    ┌────────────────────────────────────────────────────┐ │
│    │ Ctrl+Shift+Alt+Y                                   │ │
│    └────────────────────────────────────────────────────┘ │
│    Example: Ctrl+Shift+Alt+Y                              │
│                                                            │
│    Translation Hotkey:                                    │
│    ┌────────────────────────────────────────────────────┐ │
│    │ Ctrl+Shift+Alt+T                                   │ │
│    └────────────────────────────────────────────────────┘ │
│    Example: Ctrl+Shift+Alt+T                              │
│                                                            │
│  문장 톤 프리셋:                          ← NEW SECTION  │
│  ┌──────────────────────────────────────────────────────┐ │
│  │ 톤 없음                                         ▼   │ │
│  └──────────────────────────────────────────────────────┘ │
│  원문의 톤을 그대로 유지합니다.                           │
│                                                            │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐                  │
│  │➕ 추가 │  │✏️ 수정 │  │🗑️ 삭제│  ← NEW BUTTONS   │
│  └─────────┘  └─────────┘  └─────────┘                  │
│                                                            │
│  ┌──────────────────────────────────┐                    │
│  │  📊 View Usage Statistics        │                    │
│  └──────────────────────────────────┘                    │
│  Track your API usage, token consumption, and costs       │
│                                                            │
│                                   ┌──────┐  ┌──────────┐ │
│                                   │ Save │  │  Cancel  │ │
│                                   └──────┘  └──────────┘ │
└────────────────────────────────────────────────────────────┘
```

---

## ComboBox Dropdown - Tone Presets List

When the user clicks the dropdown arrow:

```
┌──────────────────────────────────────────────────────┐
│ ✓ 톤 없음                                            │
├──────────────────────────────────────────────────────┤
│   근엄한 팀장님 톤                                   │
├──────────────────────────────────────────────────────┤
│   싹싹한 신입 사원 톤                                │
├──────────────────────────────────────────────────────┤
│   MZ세대 톤                                          │
├──────────────────────────────────────────────────────┤
│   심드렁한 알바생 톤                                 │
├──────────────────────────────────────────────────────┤
│   유난히 예의 바른 경비원 톤                         │
├──────────────────────────────────────────────────────┤
│   오버하는 홈쇼핑 쇼호스트 톤                        │
├──────────────────────────────────────────────────────┤
│   유행어 난발하는 예능인 톤                          │
├──────────────────────────────────────────────────────┤
│   100년 된 할머니 톤                                 │
├──────────────────────────────────────────────────────┤
│   드라마 재벌 2세 톤                                 │
├──────────────────────────────────────────────────────┤
│   외국인 한국어 학습자 톤                            │
├──────────────────────────────────────────────────────┤
│   [User Custom Tones appear here if added]           │
└──────────────────────────────────────────────────────┘
```

---

## UI State Changes

### State 1: Default Tone Selected
- **ComboBox**: Shows "톤 없음"
- **Description**: "원문의 톤을 그대로 유지합니다."
- **Edit Button**: Disabled (grayed out)
- **Delete Button**: Disabled (grayed out)
- **Add Button**: Enabled (green)

### State 2: Default Tone Selected (Non-"톤 없음")
- **ComboBox**: Shows selected default tone name
- **Description**: Shows tone-specific description
- **Edit Button**: Disabled (grayed out)
- **Delete Button**: Disabled (grayed out)
- **Add Button**: Enabled (green)

### State 3: Custom Tone Selected
- **ComboBox**: Shows custom tone name
- **Description**: Shows custom tone description
- **Edit Button**: Enabled (blue)
- **Delete Button**: Enabled (red)
- **Add Button**: Enabled (green)

---

## User Flow Diagrams

### Flow 1: Adding a Custom Tone

```
┌─────────────┐
│   Settings  │
│   Window    │
└──────┬──────┘
       │
       │ Click "➕ 추가"
       ▼
┌─────────────────┐
│ TonePresetDialog│
│  (Add Mode)     │
└──────┬──────────┘
       │
       │ Enter name & description
       │ Click "확인"
       ▼
┌─────────────┐
│   Dialog    │
│   Validates │
└──────┬──────┘
       │
       ▼
   Valid?
   /    \
 Yes     No
  │       │
  │       └─→ Show error message
  │           Stay on dialog
  │
  ▼
┌─────────────────┐
│ TonePresetService│
│ .AddTonePreset()│
└──────┬──────────┘
       │
       ▼
┌─────────────┐
│  Settings   │
│  Window     │
│  Refreshes  │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ New preset  │
│ is selected │
└─────────────┘
```

### Flow 2: Spelling Correction with Tone

```
┌──────────────┐
│ User selects │
│   text in    │
│   any app    │
└──────┬───────┘
       │
       │ Press Ctrl+Shift+Alt+Y
       ▼
┌──────────────┐
│  AIService   │
│.CorrectSpell │
│   ingAsync() │
└──────┬───────┘
       │
       │ Get selected tone preset
       ▼
┌────────────────┐
│TonePresetService│
│.GetSelectedTone│
│   Preset()     │
└──────┬─────────┘
       │
       ▼
   Tone selected?
   /         \
 Yes          No
  │            │
  │            └─→ Use basic correction prompt
  │
  ▼
┌─────────────────┐
│ Build prompt    │
│ with tone       │
│ instructions    │
└──────┬──────────┘
       │
       ▼
┌─────────────┐
│  Send to    │
│  OpenAI API │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Display   │
│   result    │
└─────────────┘
```

---

## Color Scheme

### Primary Colors
- **Green (#4CAF50)**: Add button, OK button
- **Blue (#2196F3)**: Edit button, Usage Statistics button
- **Red (#F44336)**: Delete button
- **Gray (#9E9E9E)**: Cancel button
- **Light Gray**: Disabled button state

### Text Colors
- **Black**: Primary text, labels
- **Gray**: Helper text, descriptions
- **White**: Button text

---

## Responsive Behavior

### When Custom Preset is Selected
```
Before:                          After:
┌─────────┐  ┌─────────┐        ┌─────────┐  ┌─────────┐
│✏️ 수정 │  │🗑️ 삭제│        │✏️ 수정 │  │🗑️ 삭제│
│(disabled)│  │(disabled)│        │(enabled)│  │(enabled)│
└─────────┘  └─────────┘        └─────────┘  └─────────┘
   Gray          Gray              Blue          Red
```

### When Description Updates
```
Tone Selection Changed
         │
         ▼
┌────────────────────────────────────────────┐
│ Description TextBlock immediately updates  │
│ with new tone's description                │
└────────────────────────────────────────────┘
```

---

## Accessibility Features

### Keyboard Navigation
- Tab order: API Key → Endpoint → Model → Auto Start → Spelling Hotkey → Translation Hotkey → **Tone Preset ComboBox** → **Add Button** → **Edit Button** → **Delete Button** → Usage Stats → Save → Cancel
- Enter key: Activates focused button
- Space key: Opens ComboBox dropdown
- Arrow keys: Navigate ComboBox items

### Screen Reader Support
- All buttons have descriptive text
- ComboBox items are read correctly
- Description updates are announced

---

## Example Screenshots (Text-based)

### Example 1: Initial View
```
Settings Window (Initial State)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
문장 톤 프리셋: [톤 없음 ▼]
원문의 톤을 그대로 유지합니다.

[➕ 추가] [✏️ 수정] [🗑️ 삭제]
         (disabled)(disabled)
```

### Example 2: After Adding Custom Tone
```
Settings Window (Custom Tone Added)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
문장 톤 프리셋: [공식 이메일 톤 ▼]
정중하고 격식 있는 비즈니스 이메일 말투

[➕ 추가] [✏️ 수정] [🗑️ 삭제]
         (enabled) (enabled)
```

### Example 3: Add Dialog
```
┌─────────────────────────────────────┐
│ 새 톤 프리셋 추가                   │
│                                     │
│ 프리셋 이름:                        │
│ [공식 보고서 톤________________]    │
│                                     │
│ 톤 설명:                            │
│ [격식 있고 전문적인 말투,___]       │
│ [보고서 작성에 적합_________]       │
│ [__________________________]       │
│                                     │
│ 예: 권위 있고 엄격한 말투...        │
│                                     │
│              [확인]    [취소]       │
└─────────────────────────────────────┘
```

---

## Implementation Notes

### XAML Structure
```xml
<StackPanel Grid.Row="6" Margin="0,0,0,15">
    <TextBlock Text="문장 톤 프리셋:" FontWeight="SemiBold"/>
    
    <ComboBox x:Name="TonePresetComboBox"
              DisplayMemberPath="Name"
              SelectedValuePath="Id"
              SelectionChanged="TonePresetComboBox_SelectionChanged"/>
    
    <TextBlock x:Name="TonePresetDescriptionTextBlock"
               FontSize="10"
               Foreground="Gray"
               TextWrapping="Wrap"/>
    
    <StackPanel Orientation="Horizontal">
        <Button Content="➕ 추가" Click="AddTonePresetButton_Click"/>
        <Button x:Name="EditTonePresetButton" 
                Content="✏️ 수정"
                Click="EditTonePresetButton_Click"
                IsEnabled="False"/>
        <Button x:Name="DeleteTonePresetButton"
                Content="🗑️ 삭제"
                Click="DeleteTonePresetButton_Click"
                IsEnabled="False"/>
    </StackPanel>
</StackPanel>
```

### Event Handlers
```csharp
private void TonePresetComboBox_SelectionChanged(...)
{
    // Update description
    // Enable/disable edit and delete buttons
}

private void AddTonePresetButton_Click(...)
{
    // Show TonePresetDialog in Add mode
    // Add new preset if dialog returns true
}

private void EditTonePresetButton_Click(...)
{
    // Show TonePresetDialog in Edit mode
    // Update preset if dialog returns true
}

private void DeleteTonePresetButton_Click(...)
{
    // Show confirmation dialog
    // Delete preset if confirmed
}
```

---

## Conclusion

This visual guide provides a complete overview of the UI changes for the tone preset feature. All new components follow the existing design language and integrate seamlessly with the current settings window layout.
