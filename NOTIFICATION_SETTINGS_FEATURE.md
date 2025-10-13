# Progress Notification Control Feature

## Overview

This feature allows users to control whether desktop notifications are shown during spelling correction and translation operations.

## Changes Made

### 1. Settings Model (`Models/Models.cs`)
Added a new boolean property to `AppSettings`:
```csharp
public bool ShowProgressNotifications { get; set; } = false;
```
- **Default Value**: `false` (notifications disabled by default)

### 2. Settings UI (`Views/SettingsWindow.xaml`)
Added a new checkbox control in the settings window:
```xml
<CheckBox x:Name="ShowProgressNotificationsCheckBox" 
          Content="Show progress notifications" 
          FontWeight="SemiBold"
          Margin="0,10,0,0"/>
<TextBlock Text="Display desktop notifications for spelling correction and translation progress" 
           FontSize="10" 
           Foreground="Gray" 
           Margin="20,3,0,0"/>
```

### 3. Settings Code-Behind (`Views/SettingsWindow.xaml.cs`)
- Updated `LoadSettings()` to load the checkbox state from settings
- Updated `SaveButton_Click()` to save the checkbox state to settings

### 4. Main Window (`MainWindow.xaml.cs`)
Modified the `ShowNotification` method to accept an optional `isProgress` parameter:
```csharp
private void ShowNotification(string title, string message, bool isProgress = false)
{
    // Only show progress notifications if enabled in settings
    if (isProgress)
    {
        var settings = _settingsService.LoadSettings();
        if (!settings.ShowProgressNotifications)
        {
            return;
        }
    }
    
    _notifyIcon?.ShowBalloonTip(3000, title, message, ToolTipIcon.Info);
}
```

## Notification Types

### Progress Notifications (Controlled by Setting)
These notifications are only shown when `ShowProgressNotifications` is enabled:
- "맞춤법 교정 요청" - Spelling correction requested
- "Processing..." - AI processing in progress
- "맞춤법 교정 완료" - Spelling correction completed
- "번역 요청" - Translation requested
- "번역 완료" - Translation completed

### Always-Visible Notifications (Not Controlled)
These notifications are always shown regardless of the setting:
- "프로그램 시작" - Application startup notification with hotkey info
- "No text selected" - Error when no text is selected
- "Error" - Any error notifications

## User Experience

### Default Behavior (ShowProgressNotifications = false)
- No progress notifications are displayed
- Only errors and startup notification are shown
- Users can work without interruption from progress updates
- The result popup window still appears with the corrected/translated text

### Enabled Behavior (ShowProgressNotifications = true)
- All progress notifications are displayed
- Users can track the progress of spelling correction and translation operations
- Desktop notifications provide feedback for each step:
  1. Request received
  2. Processing in progress
  3. Operation completed

## How to Use

1. Open the Settings window (double-click system tray icon or right-click > Settings)
2. Find the "Show progress notifications" checkbox under "Start with Windows"
3. Check the box to enable progress notifications
4. Click "Save" to apply the changes
5. The setting takes effect immediately for new operations

## Korean Translation

### 기능 개요
이 기능을 통해 사용자는 맞춤법 교정 및 번역 작업 중 데스크톱 알림 표시 여부를 제어할 수 있습니다.

### 기본 설정
- **진행 상황 알림**: 기본적으로 비활성화됨
- **오류 알림**: 항상 표시됨

### 사용 방법
1. 설정 창 열기 (시스템 트레이 아이콘 더블클릭 또는 우클릭 > 설정)
2. "Start with Windows" 아래의 "Show progress notifications" 체크박스 찾기
3. 진행 상황 알림을 활성화하려면 체크박스 선택
4. "저장"을 클릭하여 변경사항 적용
5. 설정은 새로운 작업에 즉시 적용됨

---

**구현 날짜**: 2025-10-13  
**기본값**: 진행 상황 알림 비활성화
