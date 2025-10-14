# Variable Name Suggestion Feature

## Overview

The Variable Name Suggestion feature helps developers convert Korean text from the clipboard into C#-compliant variable names. This feature suggests 3 appropriate variable names following Microsoft C# naming conventions (camelCase).

## How to Use

1. **Select Korean text** in any application (e.g., a note, document, or comment)
2. **Press the hotkey** `Ctrl+Shift+Alt+V` (default, configurable in Settings)
3. **View suggestions** in the popup window showing 3 variable name recommendations
4. **Copy a suggestion** by clicking "Copy to Clipboard" button

## Example Usage

### Example 1: Simple Description
**Input:** `사용자 이름`
**Output:**
1. userName
2. userFullName
3. accountName

### Example 2: Complex Description
**Input:** `데이터베이스 연결 상태`
**Output:**
1. databaseConnectionStatus
2. dbConnectionState
3. connectionStatus

### Example 3: Action-oriented
**Input:** `파일 다운로드하기`
**Output:**
1. downloadFile
2. fileDownloader
3. downloadAction

## Configuration

You can customize the hotkey in the Settings window:
1. Double-click the system tray icon to open Settings
2. Find "Variable Name Suggestion Hotkey" field
3. Enter your preferred hotkey (e.g., `Ctrl+Shift+Alt+V`)
4. Click Save and restart the application

## Technical Details

### Implementation
- **Model:** `VariableNameSuggestionResult` - Stores original text and suggested names
- **Service:** `AIService.SuggestVariableNamesAsync()` - Uses OpenAI API to generate suggestions
- **Hotkey ID:** `3` (VARIABLE_NAME_SUGGESTION_HOTKEY_ID)
- **Default Hotkey:** `Ctrl+Shift+Alt+V`

### AI Prompt
The feature uses the following prompt structure:
```
다음 한글 텍스트를 C# 변수명 규칙에 맞게 3개의 변수명을 추천해주세요. 
각 변수명은 camelCase 형식이어야 하며, 의미가 명확해야 합니다.

텍스트: {input}

각 변수명을 새 줄로 구분하여 반환하고, 설명이나 번호는 붙이지 마세요. 변수명만 반환하세요.
```

### System Message
```
당신은 C# 프로그래밍 전문가입니다. 한글 텍스트를 의미있는 영어 변수명으로 변환하는 것을 도와줍니다. 
Microsoft C# 명명 규칙을 따르며, camelCase를 사용합니다.
```

## Naming Conventions

The feature follows Microsoft C# Coding Conventions:
- **Format:** camelCase (first letter lowercase, subsequent words capitalized)
- **Length:** Reasonable length (not too short, not too long)
- **Clarity:** Meaningful and descriptive names
- **Consistency:** Uses common programming terminology

## Benefits

1. **Saves Time:** Quickly converts Korean descriptions to proper variable names
2. **Consistent Naming:** Follows C# conventions automatically
3. **Multiple Options:** Provides 3 suggestions to choose from
4. **Context-Aware:** AI understands the meaning and suggests appropriate names
5. **Easy to Use:** Simple hotkey activation, no typing required

## API Usage

Each variable name suggestion request:
- Uses the configured AI model (default: gpt-4o-mini)
- Consumes approximately 50-100 tokens
- Takes 1-3 seconds to complete
- Records usage statistics

## Troubleshooting

### No suggestions appear
- Ensure you have selected Korean text before pressing the hotkey
- Check that your API key is configured in Settings
- Verify your internet connection

### Poor suggestions
- Try providing more context in the input text
- Use descriptive Korean text rather than single words
- Consider using a more powerful model (gpt-4o) in Settings

### Hotkey not working
- Check that the hotkey isn't used by another application
- Verify the hotkey format in Settings (e.g., `Ctrl+Shift+Alt+V`)
- Restart the application after changing hotkey settings

## Future Enhancements (Potential)

- Support for other programming languages (Python, Java, JavaScript)
- Support for PascalCase (class names, properties)
- Support for SCREAMING_SNAKE_CASE (constants)
- Batch conversion of multiple terms
- Custom naming convention templates
- Integration with IDE clipboard watchers
