; ==============================================================================
; SpellingChecker - AutoHotkey Version
; AI 기반 맞춤법 교정 및 영한 번역 윈도우 프로그램
; ==============================================================================

#Requires AutoHotkey v2.0
#SingleInstance Force

; ==============================================================================
; Global Variables
; ==============================================================================
global g_ApiKey := ""
global g_ApiEndpoint := "https://api.openai.com/v1"
global g_Model := "gpt-4o-mini"
global g_SpellingHotkey := "^+!y"  ; Ctrl+Shift+Alt+Y
global g_TranslationHotkey := "^+!t"  ; Ctrl+Shift+Alt+T
global g_VariableNameHotkey := "^+!v"  ; Ctrl+Shift+Alt+V
global g_SelectedTonePresetId := "default-none"
global g_TonePresets := Map()
global g_ShowProgressNotifications := false
global g_SettingsFile := A_AppData "\SpellingChecker\settings.ini"
global g_UsageFile := A_AppData "\SpellingChecker\usage.json"
global g_MainGui := ""
global g_TrayMenu := ""

; ==============================================================================
; Initialization
; ==============================================================================
InitializeApp()

InitializeApp() {
    ; Create AppData directory if it doesn't exist
    if !DirExist(A_AppData "\SpellingChecker") {
        DirCreate(A_AppData "\SpellingChecker")
    }
    
    ; Initialize tone presets
    InitializeTonePresets()
    
    ; Load settings
    LoadSettings()
    
    ; Setup system tray
    SetupTrayIcon()
    
    ; Register hotkeys
    RegisterHotkeys()
    
    ; Show startup notification
    TrayTip("프로그램 시작", 
        "프로그램이 시작되었습니다.`n" .
        "맞춤법 교정: " . ConvertHotkeyToDisplay(g_SpellingHotkey) . "`n" .
        "번역: " . ConvertHotkeyToDisplay(g_TranslationHotkey) . "`n" .
        "변수명 추천: " . ConvertHotkeyToDisplay(g_VariableNameHotkey))
}

; ==============================================================================
; Tone Preset Management
; ==============================================================================
InitializeTonePresets() {
    global g_TonePresets
    
    ; Default tone presets
    g_TonePresets["default-none"] := {
        Id: "default-none",
        Name: "톤 없음",
        Description: "원문의 톤을 유지하며 맞춤법만 교정합니다",
        IsDefault: true
    }
    
    g_TonePresets["default-serious-manager"] := {
        Id: "default-serious-manager",
        Name: "근엄한 팀장님 톤",
        Description: "격식 있고 권위적이며 전문적인 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-eager-newbie"] := {
        Id: "default-eager-newbie",
        Name: "싹싹한 신입 사원 톤",
        Description: "예의 바르고 열정적이며 공손한 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-mz"] := {
        Id: "default-mz",
        Name: "MZ세대 톤",
        Description: "캐주얼하고 친근하며 신조어를 사용하는 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-indifferent-parttime"] := {
        Id: "default-indifferent-parttime",
        Name: "심드렁한 알바생 톤",
        Description: "무덤덤하고 간결하며 최소한의 예의를 갖춘 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-polite-guard"] := {
        Id: "default-polite-guard",
        Name: "유난히 예의 바른 경비원 톤",
        Description: "극도로 공손하고 정중하며 존댓말을 사용하는 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-enthusiastic-host"] := {
        Id: "default-enthusiastic-host",
        Name: "오버하는 홈쇼핑 쇼호스트 톤",
        Description: "과장되고 흥분되며 감정이 풍부한 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-trendy-entertainer"] := {
        Id: "default-trendy-entertainer",
        Name: "유행어 난발하는 예능인 톤",
        Description: "유행어와 재미있는 표현을 많이 사용하는 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-old-grandma"] := {
        Id: "default-old-grandma",
        Name: "100년 된 할머니 톤",
        Description: "옛날식 표현과 친근한 할머니 같은 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-drama-chaebol"] := {
        Id: "default-drama-chaebol",
        Name: "드라마 재벌 2세 톤",
        Description: "거만하고 자신감 넘치며 명령조의 말투",
        IsDefault: true
    }
    
    g_TonePresets["default-foreign-learner"] := {
        Id: "default-foreign-learner",
        Name: "외국인 한국어 학습자 톤",
        Description: "어색하지만 열심히 노력하는 한국어 학습자의 말투",
        IsDefault: true
    }
}

; ==============================================================================
; Settings Management
; ==============================================================================
LoadSettings() {
    global g_ApiKey, g_ApiEndpoint, g_Model
    global g_SpellingHotkey, g_TranslationHotkey, g_VariableNameHotkey
    global g_SelectedTonePresetId, g_ShowProgressNotifications
    global g_SettingsFile
    
    if FileExist(g_SettingsFile) {
        g_ApiKey := IniRead(g_SettingsFile, "API", "ApiKey", "")
        g_ApiEndpoint := IniRead(g_SettingsFile, "API", "ApiEndpoint", "https://api.openai.com/v1")
        g_Model := IniRead(g_SettingsFile, "API", "Model", "gpt-4o-mini")
        g_SpellingHotkey := IniRead(g_SettingsFile, "Hotkeys", "SpellingCorrection", "^+!y")
        g_TranslationHotkey := IniRead(g_SettingsFile, "Hotkeys", "Translation", "^+!t")
        g_VariableNameHotkey := IniRead(g_SettingsFile, "Hotkeys", "VariableName", "^+!v")
        g_SelectedTonePresetId := IniRead(g_SettingsFile, "General", "SelectedTonePresetId", "default-none")
        g_ShowProgressNotifications := IniRead(g_SettingsFile, "General", "ShowProgressNotifications", "0")
        
        ; Load custom tone presets
        LoadCustomTonePresets()
    }
}

SaveSettings() {
    global g_ApiKey, g_ApiEndpoint, g_Model
    global g_SpellingHotkey, g_TranslationHotkey, g_VariableNameHotkey
    global g_SelectedTonePresetId, g_ShowProgressNotifications
    global g_SettingsFile
    
    IniWrite(g_ApiKey, g_SettingsFile, "API", "ApiKey")
    IniWrite(g_ApiEndpoint, g_SettingsFile, "API", "ApiEndpoint")
    IniWrite(g_Model, g_SettingsFile, "API", "Model")
    IniWrite(g_SpellingHotkey, g_SettingsFile, "Hotkeys", "SpellingCorrection")
    IniWrite(g_TranslationHotkey, g_SettingsFile, "Hotkeys", "Translation")
    IniWrite(g_VariableNameHotkey, g_SettingsFile, "Hotkeys", "VariableName")
    IniWrite(g_SelectedTonePresetId, g_SettingsFile, "General", "SelectedTonePresetId")
    IniWrite(g_ShowProgressNotifications, g_SettingsFile, "General", "ShowProgressNotifications")
    
    ; Save custom tone presets
    SaveCustomTonePresets()
}

LoadCustomTonePresets() {
    global g_TonePresets, g_SettingsFile
    
    customPresetsCount := IniRead(g_SettingsFile, "TonePresets", "Count", "0")
    
    Loop customPresetsCount {
        id := "custom-" . A_Index
        name := IniRead(g_SettingsFile, "TonePreset-" . id, "Name", "")
        description := IniRead(g_SettingsFile, "TonePreset-" . id, "Description", "")
        
        if (name != "") {
            g_TonePresets[id] := {
                Id: id,
                Name: name,
                Description: description,
                IsDefault: false
            }
        }
    }
}

SaveCustomTonePresets() {
    global g_TonePresets, g_SettingsFile
    
    ; Count custom presets
    customCount := 0
    for id, preset in g_TonePresets {
        if (!preset.IsDefault) {
            customCount++
        }
    }
    
    IniWrite(customCount, g_SettingsFile, "TonePresets", "Count")
    
    ; Save each custom preset
    index := 1
    for id, preset in g_TonePresets {
        if (!preset.IsDefault) {
            IniWrite(preset.Name, g_SettingsFile, "TonePreset-" . preset.Id, "Name")
            IniWrite(preset.Description, g_SettingsFile, "TonePreset-" . preset.Id, "Description")
            index++
        }
    }
}

; ==============================================================================
; Tray Icon Setup
; ==============================================================================
SetupTrayIcon() {
    global g_TrayMenu
    
    A_IconTip := "AI Spelling Checker"
    
    g_TrayMenu := A_TrayMenu
    g_TrayMenu.Delete()
    g_TrayMenu.Add("Settings", MenuHandler_Settings)
    g_TrayMenu.Add()
    g_TrayMenu.Add("Exit", MenuHandler_Exit)
    g_TrayMenu.Default := "Settings"
}

MenuHandler_Settings(*) {
    ShowSettingsWindow()
}

MenuHandler_Exit(*) {
    ExitApp()
}

; ==============================================================================
; Hotkey Registration
; ==============================================================================
RegisterHotkeys() {
    global g_SpellingHotkey, g_TranslationHotkey, g_VariableNameHotkey
    
    ; Register spelling correction hotkey
    try {
        Hotkey(g_SpellingHotkey, OnSpellingCorrectionRequested)
    } catch {
        MsgBox("Failed to register spelling correction hotkey: " . g_SpellingHotkey)
    }
    
    ; Register translation hotkey
    try {
        Hotkey(g_TranslationHotkey, OnTranslationRequested)
    } catch {
        MsgBox("Failed to register translation hotkey: " . g_TranslationHotkey)
    }
    
    ; Register variable name suggestion hotkey
    try {
        Hotkey(g_VariableNameHotkey, OnVariableNameSuggestionRequested)
    } catch {
        MsgBox("Failed to register variable name suggestion hotkey: " . g_VariableNameHotkey)
    }
}

; ==============================================================================
; Hotkey Handlers
; ==============================================================================
OnSpellingCorrectionRequested(*) {
    selectedText := GetSelectedText()
    
    if (selectedText == "") {
        ShowNotification("No text selected", "Please select some text to correct.")
        return
    }
    
    ShowNotification("맞춤법 교정 요청", "'" . selectedText . "' 문장의 맞춤법 교정을 요청했습니다.", true)
    ShowNotification("Processing...", "AI is correcting your text. Please wait...", true)
    
    correctedText := CorrectSpellingAsync(selectedText)
    
    if (correctedText != "") {
        ShowNotification("맞춤법 교정 완료", "교정 결과는 '" . correctedText . "' 입니다.", true)
        ShowResultPopup(correctedText, selectedText, "Spelling Correction", false)
    }
}

OnTranslationRequested(*) {
    selectedText := GetSelectedText()
    
    if (selectedText == "") {
        ShowNotification("No text selected", "Please select some text to translate.")
        return
    }
    
    ShowNotification("번역 요청", "'" . selectedText . "' 문장의 번역을 요청했습니다.", true)
    ShowNotification("Processing...", "AI is translating your text. Please wait...", true)
    
    result := TranslateAsync(selectedText)
    
    if (result.translatedText != "") {
        ShowNotification("번역 완료", 
            "번역 결과는 '" . result.translatedText . "' 입니다.`n" .
            "언어: " . result.sourceLanguage . " → " . result.targetLanguage, true)
        ShowResultPopup(result.translatedText, selectedText, 
            "Translation (" . result.sourceLanguage . " → " . result.targetLanguage . ")", true)
    }
}

OnVariableNameSuggestionRequested(*) {
    selectedText := GetSelectedText()
    
    if (selectedText == "") {
        ShowNotification("No text selected", "Please select some text to suggest variable names.")
        return
    }
    
    ShowNotification("변수명 추천 요청", "'" . selectedText . "' 텍스트의 변수명을 추천합니다.", true)
    ShowNotification("Processing...", "AI is suggesting variable names. Please wait...", true)
    
    suggestions := SuggestVariableNamesAsync(selectedText)
    
    if (suggestions.Length > 0) {
        formattedSuggestions := ""
        for index, name in suggestions {
            formattedSuggestions .= index . ". " . name . "`n"
        }
        
        ShowNotification("변수명 추천 완료", "추천된 변수명: " . StrJoin(suggestions, ", "), true)
        ShowResultPopup(formattedSuggestions, selectedText, 
            "Variable Name Suggestions (C#)", false)
    }
}

; ==============================================================================
; Clipboard Service
; ==============================================================================
GetSelectedText() {
    ; Save current clipboard
    clipboardBackup := ClipboardAll()
    A_Clipboard := ""
    
    ; Send Ctrl+C to copy selected text
    Send("^c")
    
    ; Wait for clipboard to contain data
    if !ClipWait(0.5) {
        A_Clipboard := clipboardBackup
        return ""
    }
    
    selectedText := A_Clipboard
    
    ; Restore clipboard
    A_Clipboard := clipboardBackup
    
    return selectedText
}

SetClipboard(text) {
    A_Clipboard := text
}

; ==============================================================================
; AI Service - OpenAI API Integration
; ==============================================================================
CorrectSpellingAsync(text) {
    global g_ApiKey, g_ApiEndpoint, g_Model, g_SelectedTonePresetId, g_TonePresets
    
    if (g_ApiKey == "") {
        MsgBox("API Key is not configured. Please set your OpenAI API key in settings.")
        return ""
    }
    
    ; Get selected tone preset
    tonePreset := g_TonePresets.Has(g_SelectedTonePresetId) ? g_TonePresets[g_SelectedTonePresetId] : ""
    
    ; Build prompt based on tone selection
    systemMessage := "당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하되, 원문의 의미와 어조를 최대한 유지하세요."
    prompt := ""
    
    if (tonePreset && tonePreset.Id != "default-none") {
        prompt := "맞춤법과 문법을 교정하고, 원문의 톤은 완전히 무시한 채 오직 내용만 유지하여 다음 톤으로 변환해주세요.`n`n톤: " . tonePreset.Name . "`n설명: " . tonePreset.Description . "`n`n교정 및 톤 변환된 텍스트만 반환하고 설명은 하지 마세요:`n`n" . text
        systemMessage := "당신은 한국어와 영어 맞춤법 및 문법 교정 전문가입니다. 오류를 정확하게 교정하고, 원문의 톤이나 말투는 완전히 무시한 채 오직 의미만 유지하면서 지정된 톤(" . tonePreset.Description . ")으로 완전히 새롭게 변환하세요."
    } else {
        prompt := "맞춤법과 문법을 교정해주세요. 교정된 텍스트만 반환하고 설명은 하지 마세요:`n`n" . text
    }
    
    ; Build request body
    requestBody := '{"model":"' . g_Model . '","messages":[{"role":"system","content":"' . EscapeJson(systemMessage) . '"},{"role":"user","content":"' . EscapeJson(prompt) . '"}],"temperature":0.3,"max_tokens":2000}'
    
    ; Send request to OpenAI API
    response := SendOpenAIRequest(requestBody)
    
    if (response) {
        correctedText := ExtractContentFromResponse(response)
        RecordUsage(response, "Correction")
        return correctedText
    }
    
    return ""
}

TranslateAsync(text) {
    global g_ApiKey, g_ApiEndpoint, g_Model
    
    if (g_ApiKey == "") {
        MsgBox("API Key is not configured. Please set your OpenAI API key in settings.")
        return {translatedText: "", sourceLanguage: "", targetLanguage: ""}
    }
    
    ; Detect language
    sourceLanguage := DetectLanguage(text)
    targetLanguage := (sourceLanguage == "Korean") ? "English" : "Korean"
    
    ; Build prompt
    prompt := ""
    if (sourceLanguage == "Korean") {
        prompt := "다음 한국어 텍스트를 영어로 번역해주세요. 번역 결과만 반환하고 설명은 하지 마세요:`n`n" . text
    } else {
        prompt := "Translate the following English text to Korean. Return only the translation without explanations:`n`n" . text
    }
    
    ; Build request body
    requestBody := '{"model":"' . g_Model . '","messages":[{"role":"system","content":"당신은 전문 번역가입니다. 정확하고 자연스러운 번역을 제공하세요."},{"role":"user","content":"' . EscapeJson(prompt) . '"}],"temperature":0.3,"max_tokens":2000}'
    
    ; Send request to OpenAI API
    response := SendOpenAIRequest(requestBody)
    
    if (response) {
        translatedText := ExtractContentFromResponse(response)
        RecordUsage(response, "Translation")
        return {translatedText: translatedText, sourceLanguage: sourceLanguage, targetLanguage: targetLanguage}
    }
    
    return {translatedText: "", sourceLanguage: "", targetLanguage: ""}
}

SuggestVariableNamesAsync(text) {
    global g_ApiKey, g_ApiEndpoint, g_Model
    
    if (g_ApiKey == "") {
        MsgBox("API Key is not configured. Please set your OpenAI API key in settings.")
        return []
    }
    
    ; Build prompt
    prompt := "다음 한글 텍스트를 C# 변수명 규칙에 맞게 3개의 변수명을 추천해주세요. 각 변수명은 camelCase 형식이어야 하며, 의미가 명확해야 합니다.`n`n텍스트: " . text . "`n`n각 변수명을 새 줄로 구분하여 반환하고, 설명이나 번호는 붙이지 마세요. 변수명만 반환하세요."
    
    ; Build request body
    requestBody := '{"model":"' . g_Model . '","messages":[{"role":"system","content":"당신은 C# 프로그래밍 전문가입니다. 한글 텍스트를 의미있는 영어 변수명으로 변환하는 것을 도와줍니다. Microsoft C# 명명 규칙을 따르며, camelCase를 사용합니다."},{"role":"user","content":"' . EscapeJson(prompt) . '"}],"temperature":0.5,"max_tokens":200}'
    
    ; Send request to OpenAI API
    response := SendOpenAIRequest(requestBody)
    
    if (response) {
        suggestionsText := ExtractContentFromResponse(response)
        RecordUsage(response, "VariableNameSuggestion")
        
        ; Parse suggestions
        suggestions := StrSplit(suggestionsText, "`n")
        result := []
        
        for index, line in suggestions {
            trimmed := Trim(line)
            if (trimmed != "" && result.Length < 3) {
                result.Push(trimmed)
            }
        }
        
        return result
    }
    
    return []
}

SendOpenAIRequest(requestBody) {
    global g_ApiKey, g_ApiEndpoint
    
    try {
        http := ComObject("WinHttp.WinHttpRequest.5.1")
        http.Open("POST", g_ApiEndpoint . "/chat/completions", false)
        http.SetRequestHeader("Content-Type", "application/json")
        http.SetRequestHeader("Authorization", "Bearer " . g_ApiKey)
        http.Send(requestBody)
        
        if (http.Status == 200) {
            return http.ResponseText
        } else {
            MsgBox("API request failed: " . http.Status . " - " . http.ResponseText)
            return ""
        }
    } catch as err {
        MsgBox("Error sending API request: " . err.Message)
        return ""
    }
}

ExtractContentFromResponse(response) {
    try {
        ; Parse JSON response manually (simple extraction)
        ; Look for "content":"..." pattern
        contentStart := InStr(response, '"content":"')
        if (contentStart == 0) {
            return ""
        }
        
        contentStart += StrLen('"content":"')
        contentEnd := InStr(response, '"', false, contentStart)
        
        if (contentEnd == 0) {
            return ""
        }
        
        content := SubStr(response, contentStart, contentEnd - contentStart)
        
        ; Unescape JSON string
        content := StrReplace(content, '\n', '`n')
        content := StrReplace(content, '\r', '`r')
        content := StrReplace(content, '\t', '`t')
        content := StrReplace(content, '\"', '"')
        content := StrReplace(content, '\\', '\')
        
        return content
    } catch {
        return ""
    }
}

DetectLanguage(text) {
    ; Simple language detection based on character ranges
    for index, char in StrSplit(text) {
        charCode := Ord(char)
        ; Check for Hangul syllables (0xAC00 - 0xD7A3)
        if (charCode >= 0xAC00 && charCode <= 0xD7A3) {
            return "Korean"
        }
    }
    return "English"
}

; ==============================================================================
; Usage Tracking
; ==============================================================================
RecordUsage(response, operationType) {
    global g_UsageFile
    
    try {
        ; Extract usage information from response
        ; This is a simplified version - in production you'd parse JSON properly
        usageRecord := {
            Timestamp: FormatTime(, "yyyy-MM-dd HH:mm:ss"),
            OperationType: operationType,
            Model: g_Model
        }
        
        ; Append to usage file (simplified - in production use proper JSON)
        FileAppend(usageRecord.Timestamp . "," . usageRecord.OperationType . "," . usageRecord.Model . "`n", g_UsageFile)
    } catch {
        ; Silently fail - don't interrupt user operations
    }
}

; ==============================================================================
; Result Popup Window
; ==============================================================================
ShowResultPopup(resultText, originalText, title, isTranslation) {
    resultGui := Gui("+AlwaysOnTop", title)
    resultGui.SetFont("s10", "Segoe UI")
    
    ; Original text
    resultGui.Add("Text", "w500", "Original Text:")
    resultGui.Add("Edit", "w500 h100 ReadOnly vOriginalEdit", originalText)
    
    ; Result text
    resultGui.Add("Text", "w500", "Result:")
    resultGui.Add("Edit", "w500 h100 ReadOnly vResultEdit", resultText)
    
    ; Buttons
    copyBtn := resultGui.Add("Button", "w150 Default", "Copy to Clipboard")
    copyBtn.OnEvent("Click", (*) => CopyResultToClipboard(resultText, resultGui))
    
    replaceBtn := resultGui.Add("Button", "x+10 w150", "Replace")
    replaceBtn.OnEvent("Click", (*) => ReplaceOriginalText(resultText, resultGui))
    
    closeBtn := resultGui.Add("Button", "x+10 w150", "Close")
    closeBtn.OnEvent("Click", (*) => resultGui.Destroy())
    
    resultGui.Show()
}

CopyResultToClipboard(text, gui) {
    SetClipboard(text)
    TrayTip("Copied", "Result copied to clipboard")
    gui.Destroy()
}

ReplaceOriginalText(text, gui) {
    SetClipboard(text)
    Send("^v")
    TrayTip("Replaced", "Original text replaced")
    gui.Destroy()
}

; ==============================================================================
; Settings Window
; ==============================================================================
ShowSettingsWindow() {
    global g_ApiKey, g_ApiEndpoint, g_Model
    global g_SpellingHotkey, g_TranslationHotkey, g_VariableNameHotkey
    global g_SelectedTonePresetId, g_ShowProgressNotifications
    
    settingsGui := Gui("+AlwaysOnTop", "Settings - SpellingChecker")
    settingsGui.SetFont("s10", "Segoe UI")
    
    ; API Settings
    settingsGui.Add("GroupBox", "w500 h150", "API Settings")
    settingsGui.Add("Text", "xp+10 yp+25", "API Key:")
    apiKeyEdit := settingsGui.Add("Edit", "w480 Password vApiKey", g_ApiKey)
    
    settingsGui.Add("Text", "xp yp+35", "API Endpoint:")
    apiEndpointEdit := settingsGui.Add("Edit", "w480 vApiEndpoint", g_ApiEndpoint)
    
    settingsGui.Add("Text", "xp yp+35", "Model:")
    modelEdit := settingsGui.Add("Edit", "w480 vModel", g_Model)
    
    ; Hotkey Settings
    settingsGui.Add("GroupBox", "xm y+20 w500 h120", "Hotkeys")
    settingsGui.Add("Text", "xp+10 yp+25", "Spelling Correction:")
    spellingEdit := settingsGui.Add("Edit", "w380 vSpellingHotkey", ConvertHotkeyToDisplay(g_SpellingHotkey))
    
    settingsGui.Add("Text", "xp yp+35", "Translation:")
    translationEdit := settingsGui.Add("Edit", "w380 vTranslationHotkey", ConvertHotkeyToDisplay(g_TranslationHotkey))
    
    settingsGui.Add("Text", "xp yp+35", "Variable Name Suggestion:")
    variableEdit := settingsGui.Add("Edit", "w380 vVariableNameHotkey", ConvertHotkeyToDisplay(g_VariableNameHotkey))
    
    ; Tone Preset Settings
    settingsGui.Add("GroupBox", "xm y+20 w500 h80", "Tone Preset")
    settingsGui.Add("Text", "xp+10 yp+25", "Selected Tone:")
    
    ; Build tone preset list
    toneList := []
    for id, preset in g_TonePresets {
        toneList.Push(preset.Name)
    }
    
    toneDropdown := settingsGui.Add("DropDownList", "w380 vSelectedTone", toneList)
    
    ; Select current tone
    currentIndex := 1
    index := 1
    for id, preset in g_TonePresets {
        if (id == g_SelectedTonePresetId) {
            currentIndex := index
            break
        }
        index++
    }
    toneDropdown.Choose(currentIndex)
    
    ; Other Settings
    settingsGui.Add("GroupBox", "xm y+20 w500 h60", "Notifications")
    progressCheck := settingsGui.Add("Checkbox", "xp+10 yp+25 vShowProgress", "Show progress notifications")
    progressCheck.Value := g_ShowProgressNotifications
    
    ; Buttons
    saveBtn := settingsGui.Add("Button", "xm y+20 w150 Default", "Save")
    saveBtn.OnEvent("Click", (*) => SaveSettingsFromGui(settingsGui))
    
    cancelBtn := settingsGui.Add("Button", "x+10 w150", "Cancel")
    cancelBtn.OnEvent("Click", (*) => settingsGui.Destroy())
    
    usageBtn := settingsGui.Add("Button", "x+10 w150", "📊 Usage Statistics")
    usageBtn.OnEvent("Click", (*) => ShowUsageStatisticsWindow())
    
    settingsGui.Show()
}

SaveSettingsFromGui(gui) {
    global g_ApiKey, g_ApiEndpoint, g_Model
    global g_SpellingHotkey, g_TranslationHotkey, g_VariableNameHotkey
    global g_SelectedTonePresetId, g_ShowProgressNotifications
    
    saved := gui.Submit()
    
    g_ApiKey := saved.ApiKey
    g_ApiEndpoint := saved.ApiEndpoint
    g_Model := saved.Model
    g_ShowProgressNotifications := saved.ShowProgress
    
    ; Convert hotkeys back to AHK format
    g_SpellingHotkey := ConvertHotkeyFromDisplay(saved.SpellingHotkey)
    g_TranslationHotkey := ConvertHotkeyFromDisplay(saved.TranslationHotkey)
    g_VariableNameHotkey := ConvertHotkeyFromDisplay(saved.VariableNameHotkey)
    
    ; Get selected tone preset ID
    selectedToneName := saved.SelectedTone
    for id, preset in g_TonePresets {
        if (preset.Name == selectedToneName) {
            g_SelectedTonePresetId := id
            break
        }
    }
    
    SaveSettings()
    
    ; Re-register hotkeys
    try {
        Hotkey(g_SpellingHotkey, OnSpellingCorrectionRequested)
        Hotkey(g_TranslationHotkey, OnTranslationRequested)
        Hotkey(g_VariableNameHotkey, OnVariableNameSuggestionRequested)
    } catch {
        MsgBox("Failed to register hotkeys. Please check your hotkey settings.")
    }
    
    TrayTip("Settings Saved", "Settings have been saved successfully!")
    gui.Destroy()
}

; ==============================================================================
; Usage Statistics Window
; ==============================================================================
ShowUsageStatisticsWindow() {
    global g_UsageFile
    
    statsGui := Gui("+AlwaysOnTop", "Usage Statistics")
    statsGui.SetFont("s10", "Segoe UI")
    
    ; Read usage data
    usageData := ""
    if FileExist(g_UsageFile) {
        usageData := FileRead(g_UsageFile)
    }
    
    ; Count operations
    correctionCount := 0
    translationCount := 0
    variableNameCount := 0
    
    Loop Parse, usageData, "`n" {
        if (InStr(A_LoopField, "Correction")) {
            correctionCount++
        } else if (InStr(A_LoopField, "Translation")) {
            translationCount++
        } else if (InStr(A_LoopField, "VariableNameSuggestion")) {
            variableNameCount++
        }
    }
    
    ; Display statistics
    statsGui.Add("Text", "w400", "Spelling Corrections: " . correctionCount)
    statsGui.Add("Text", "w400", "Translations: " . translationCount)
    statsGui.Add("Text", "w400", "Variable Name Suggestions: " . variableNameCount)
    statsGui.Add("Text", "w400", "Total Operations: " . (correctionCount + translationCount + variableNameCount))
    
    ; Close button
    closeBtn := statsGui.Add("Button", "w150 Default", "Close")
    closeBtn.OnEvent("Click", (*) => statsGui.Destroy())
    
    statsGui.Show()
}

; ==============================================================================
; Utility Functions
; ==============================================================================
ShowNotification(title, message, isProgress := false) {
    global g_ShowProgressNotifications
    
    if (isProgress && !g_ShowProgressNotifications) {
        return
    }
    
    TrayTip(title, message)
}

ConvertHotkeyToDisplay(ahkHotkey) {
    ; Convert AutoHotkey format to display format
    ; ^ = Ctrl, + = Shift, ! = Alt, # = Win
    displayHotkey := ahkHotkey
    displayHotkey := StrReplace(displayHotkey, "^", "Ctrl+")
    displayHotkey := StrReplace(displayHotkey, "+", "Shift+")
    displayHotkey := StrReplace(displayHotkey, "!", "Alt+")
    displayHotkey := StrReplace(displayHotkey, "#", "Win+")
    return displayHotkey
}

ConvertHotkeyFromDisplay(displayHotkey) {
    ; Convert display format to AutoHotkey format
    ahkHotkey := displayHotkey
    ahkHotkey := StrReplace(ahkHotkey, "Ctrl+", "^")
    ahkHotkey := StrReplace(ahkHotkey, "Shift+", "+")
    ahkHotkey := StrReplace(ahkHotkey, "Alt+", "!")
    ahkHotkey := StrReplace(ahkHotkey, "Win+", "#")
    return ahkHotkey
}

EscapeJson(text) {
    ; Escape special characters for JSON
    escaped := text
    escaped := StrReplace(escaped, '\', '\\')
    escaped := StrReplace(escaped, '"', '\"')
    escaped := StrReplace(escaped, "`n", '\n')
    escaped := StrReplace(escaped, "`r", '\r')
    escaped := StrReplace(escaped, "`t", '\t')
    return escaped
}

StrJoin(arr, separator) {
    result := ""
    for index, item in arr {
        if (index > 1) {
            result .= separator
        }
        result .= item
    }
    return result
}
