using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using SpellingChecker.Models;
using SpellingChecker.Services;
using SpellingChecker.Views;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SpellingChecker
{
    /// <summary>
    /// Main window that runs in the background with system tray icon
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon? _notifyIcon;
        private HotkeyService _hotkeyService;
        private AIService _aiService;
        private ClipboardService _clipboardService;
        private SettingsService _settingsService;

        public MainWindow()
        {
            InitializeComponent();
            
            _settingsService = new SettingsService();
            _aiService = new AIService(_settingsService);
            _clipboardService = new ClipboardService(_settingsService);
            _hotkeyService = new HotkeyService();

            InitializeSystemTray();
            
            // Hide window on startup - app runs in background
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
        }

        private void InitializeSystemTray()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application, // Will be replaced with custom icon
                Visible = true,
                Text = "AI Spelling Checker"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, OnSettingsClick);
            contextMenu.Items.Add("Check for Updates", null, OnCheckForUpdatesClick);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, OnExitClick);
            
            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += OnTrayIconDoubleClick;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var helper = new WindowInteropHelper(this);
            var settings = _settingsService.LoadSettings();
            var success = _hotkeyService.RegisterHotkeys(helper.Handle, settings.CommonQuestionHotkey, settings.SpellingCorrectionHotkey, settings.TranslationHotkey, settings.VariableNameSuggestionHotkey);

            if (!success)
            {
                MessageBox.Show("Failed to register hotkeys. The application may not work correctly.", 
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Show startup notification with hotkey information
                ShowNotification("프로그램 시작", 
                    $"프로그램이 시작되었습니다.\n" +
                    $"보편 질문 답변: {settings.CommonQuestionHotkey}\n" +
                    $"맞춤법 교정: {settings.SpellingCorrectionHotkey}\n" +
                    $"번역: {settings.TranslationHotkey}\n" +
                    $"변수명 추천: {settings.VariableNameSuggestionHotkey}");
            }

            _hotkeyService.CommonQuestionRequested += OnCommonQuestionRequested;
            _hotkeyService.SpellingCorrectionRequested += OnSpellingCorrectionRequested;
            _hotkeyService.TranslationRequested += OnTranslationRequested;
            _hotkeyService.VariableNameSuggestionRequested += OnVariableNameSuggestionRequested;

            // Add hook to process hotkey messages
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(HwndHook);

            // Check if API key is configured, if not, show settings window
            CheckAndShowSettingsIfNeeded(settings);

            // Check for updates on startup
            CheckForUpdatesOnStartup();
        }

        private void CheckAndShowSettingsIfNeeded(AppSettings settings)
        {
            // Check if API key is set for the current provider
            var apiKey = settings.GetApiKeyForProvider(settings.Provider ?? "OpenAI");
            
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                // Show notification that settings are required
                ShowNotification("설정 필요", "API 키가 설정되지 않았습니다. 설정 창을 열어 구성해주세요.");
                
                // Open settings window automatically
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var settingsWindow = new SettingsWindow();
                    settingsWindow.ShowDialog();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                _hotkeyService.ProcessHotkey(wParam.ToInt32());
                handled = true;
            }
            return IntPtr.Zero;
        }

        private async void OnSpellingCorrectionRequested(object? sender, EventArgs e)
        {
            ResultPopupWindow? popup = null;
            try
            {
                var selectedText = _clipboardService.GetSelectedText();
                
                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    ShowNotification("No text selected", "Please select some text to correct.");
                    return;
                }

                // Show debug notification when request is made
                ShowNotification("맞춤법 교정 요청", 
                    $"'{selectedText}' 문장의 맞춤법 교정을 요청했습니다.", true);

                // Create and show popup immediately with progress indicator
                var settings = _settingsService.LoadSettings();
                popup = new ResultPopupWindow("", selectedText, "Spelling Correction", false, settings, enableHighlighting: true);
                popup.ShowProgressIndicator();
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(popup.GetResultText());
                popup.ConvertRequested += async (s, text) => await ReprocessSpellingCorrection(popup, text);
                popup.ToneChangeRequested += async (s, text) => 
                {
                    // Save settings before reprocessing to persist the tone change
                    _settingsService.SaveSettings(settings);
                    await ReprocessSpellingCorrection(popup, text);
                };
                popup.Show();

                // Show progress notification
                ShowNotification("Processing...", "AI is correcting your text. Please wait...", true);

                var result = await _aiService.CorrectSpellingAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("맞춤법 교정 완료", 
                    $"교정 결과는 '{result.CorrectedText}' 입니다.", true);
                
                popup.UpdateResultWithTone(result.CorrectedText, result.AppliedToneName);
                popup.HideProgressIndicator();
                
                // Save settings after processing completes to persist tone selection
                _settingsService.SaveSettings(settings);
            }
            catch (TimeoutException)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async void OnTranslationRequested(object? sender, EventArgs e)
        {
            ResultPopupWindow? popup = null;
            try
            {
                var selectedText = _clipboardService.GetSelectedText();
                
                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    ShowNotification("No text selected", "Please select some text to translate.");
                    return;
                }

                // Show debug notification when request is made
                ShowNotification("번역 요청", 
                    $"'{selectedText}' 문장의 번역을 요청했습니다.", true);

                // Create and show popup immediately with progress indicator
                popup = new ResultPopupWindow("", selectedText, "Translation", true, enableHighlighting: false);
                popup.ShowProgressIndicator();
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(popup.GetResultText());
                popup.ConvertRequested += async (s, text) => await ReprocessTranslation(popup, text);
                popup.Show();

                // Show progress notification
                ShowNotification("Processing...", "AI is translating your text. Please wait...", true);

                var result = await _aiService.TranslateAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("번역 완료", 
                    $"번역 결과는 '{result.TranslatedText}' 입니다.\n" +
                    $"언어: {result.SourceLanguage} → {result.TargetLanguage}", true);
                
                popup.Title = $"Translation ({result.SourceLanguage} → {result.TargetLanguage})";
                popup.UpdateResult(result.TranslatedText);
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async void OnCommonQuestionRequested(object? sender, EventArgs e)
        {
            ResultPopupWindow? popup = null;
            try
            {
                var selectedText = _clipboardService.GetSelectedText();
                
                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    ShowNotification("No text selected", "Please select some text as a question.");
                    return;
                }

                // Show debug notification when request is made
                ShowNotification("질문 답변 요청", 
                    $"'{selectedText}' 질문에 대한 답변을 요청했습니다.", true);

                // Create and show popup immediately with progress indicator
                popup = new ResultPopupWindow("", selectedText, "Common Question Answer - Ctrl+Enter to reconvert", false, enableHighlighting: false);
                popup.ShowProgressIndicator();
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(popup.GetResultText());
                popup.ConvertRequested += async (s, text) => await ReprocessCommonQuestion(popup, text);
                popup.Show();

                // Show progress notification
                ShowNotification("Processing...", "AI is answering your question. Please wait...", true);

                var result = await _aiService.AnswerCommonQuestionAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("질문 답변 완료", 
                    $"질문에 대한 답변이 완료되었습니다.", true);
                
                popup.UpdateResult(result.Answer);
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ReprocessSpellingCorrection(ResultPopupWindow popup, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    popup.HideProgressIndicator();
                    ShowNotification("No text", "Please enter some text to correct.");
                    return;
                }

                ShowNotification("Processing...", "AI is correcting your text. Please wait...", true);

                var result = await _aiService.CorrectSpellingAsync(text);
                
                ShowNotification("맞춤법 교정 완료", 
                    $"교정 결과는 '{result.CorrectedText}' 입니다.", true);
                
                popup.UpdateResultWithTone(result.CorrectedText, result.AppliedToneName);
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ReprocessTranslation(ResultPopupWindow popup, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    popup.HideProgressIndicator();
                    ShowNotification("No text", "Please enter some text to translate.");
                    return;
                }

                popup.ShowProgressIndicator();
                ShowNotification("Processing...", "AI is translating your text. Please wait...", true);

                var result = await _aiService.TranslateAsync(text);
                
                ShowNotification("번역 완료", 
                    $"번역 결과는 '{result.TranslatedText}' 입니다.\n" +
                    $"언어: {result.SourceLanguage} → {result.TargetLanguage}", true);
                
                popup.UpdateResult(result.TranslatedText);
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async void OnVariableNameSuggestionRequested(object? sender, EventArgs e)
        {
            ResultPopupWindow? popup = null;
            try
            {
                var selectedText = _clipboardService.GetSelectedText();
                
                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    ShowNotification("No text selected", "Please select some text to suggest variable names.");
                    return;
                }

                // Show debug notification when request is made
                ShowNotification("변수명 추천 요청", 
                    $"'{selectedText}' 텍스트의 변수명을 추천합니다.", true);

                // Create and show popup immediately with progress indicator
                popup = new ResultPopupWindow("", selectedText, "Variable Name Suggestions (C#) - Ctrl+Enter to reconvert", false, enableHighlighting: false, isVariableNameMode: true);
                popup.ShowProgressIndicator();
                
                // Set copy handler to copy only the first variable name
                popup.CopyRequested += (s, args) => {
                    var firstVariableName = ExtractFirstVariableName(popup.GetResultText());
                    _clipboardService.SetClipboard(firstVariableName);
                };
                popup.ConvertRequested += async (s, text) => await ReprocessVariableNameSuggestion(popup, text);
                popup.ToggleModeRequested += async (s, isFunctionMode) => await ToggleVariableFunctionMode(popup, selectedText, isFunctionMode);
                popup.Show();

                // Show progress notification
                ShowNotification("Processing...", "AI is suggesting variable names. Please wait...", true);

                var result = await _aiService.SuggestVariableNamesAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("변수명 추천 완료", 
                    $"추천된 변수명: {string.Join(", ", result.SuggestedNames)}", true);
                
                popup.UpdateResult(FormatNameSuggestions(result.SuggestedNames));
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup?.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ReprocessVariableNameSuggestion(ResultPopupWindow popup, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    popup.HideProgressIndicator();
                    ShowNotification("No text", "Please enter some text to suggest variable names.");
                    return;
                }

                popup.ShowProgressIndicator();
                ShowNotification("Processing...", "AI is suggesting variable names. Please wait...", true);

                var result = await _aiService.SuggestVariableNamesAsync(text);
                
                ShowNotification("변수명 추천 완료", 
                    $"추천된 변수명: {string.Join(", ", result.SuggestedNames)}", true);
                
                popup.UpdateResult(FormatNameSuggestions(result.SuggestedNames));
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ToggleVariableFunctionMode(ResultPopupWindow popup, string originalText, bool isFunctionMode)
        {
            try
            {
                ShowNotification("Processing...", isFunctionMode ? "AI is suggesting function names. Please wait..." : "AI is suggesting variable names. Please wait...", true);

                VariableNameSuggestionResult result;
                if (isFunctionMode)
                {
                    result = await _aiService.SuggestFunctionNamesAsync(originalText);
                    ShowNotification("함수명 추천 완료", 
                        $"추천된 함수명: {string.Join(", ", result.SuggestedNames)}", true);
                }
                else
                {
                    result = await _aiService.SuggestVariableNamesAsync(originalText);
                    ShowNotification("변수명 추천 완료", 
                        $"추천된 변수명: {string.Join(", ", result.SuggestedNames)}", true);
                }
                
                popup.UpdateResult(FormatNameSuggestions(result.SuggestedNames));
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ReprocessCommonQuestion(ResultPopupWindow popup, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    popup.HideProgressIndicator();
                    ShowNotification("No text", "Please enter a question.");
                    return;
                }

                popup.ShowProgressIndicator();
                ShowNotification("Processing...", "AI is answering your question. Please wait...", true);

                var result = await _aiService.AnswerCommonQuestionAsync(text);
                
                ShowNotification("질문 답변 완료", 
                    $"질문에 대한 답변이 완료되었습니다.", true);
                
                popup.UpdateResult(result.Answer);
                popup.HideProgressIndicator();
            }
            catch (TimeoutException)
            {
                popup.HideProgressIndicator();
                ShowNotification("Request Timeout", "The AI service did not respond in time. Please check your network connection and try again.");
            }
            catch (Exception ex)
            {
                popup.HideProgressIndicator();
                ShowNotification("Error", ex.Message);
            }
        }

        private void OnSettingsClick(object? sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void OnTrayIconDoubleClick(object? sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void OnExitClick(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

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

        /// <summary>
        /// Formats variable/function name suggestions for display.
        /// </summary>
        /// <param name="suggestions">Array of suggested names</param>
        /// <returns>Formatted string with numbered suggestions</returns>
        private string FormatNameSuggestions(string[] suggestions)
        {
            return string.Join("\n", suggestions.Select((name, index) => $"{index + 1}. {name}"));
        }

        /// <summary>
        /// Extracts the first variable name from formatted variable name suggestions.
        /// Expected format: "1. variableName\n2. variableName2\n..."
        /// </summary>
        /// <param name="formattedText">The formatted text containing numbered variable names</param>
        /// <returns>The first variable name without the number prefix</returns>
        private string ExtractFirstVariableName(string formattedText)
        {
            if (string.IsNullOrWhiteSpace(formattedText))
            {
                return string.Empty;
            }

            // Split by newlines and get the first line
            var lines = formattedText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                return formattedText.Trim();
            }

            var firstLine = lines[0].Trim();
            
            // Remove the number prefix (e.g., "1. " from "1. variableName")
            // The format is: "number. variableName"
            var dotIndex = firstLine.IndexOf('.');
            if (dotIndex > 0 && dotIndex < firstLine.Length - 1)
            {
                // Extract everything after the dot and space
                return firstLine.Substring(dotIndex + 1).TrimStart();
            }

            // If no dot found, return the whole line (fallback)
            return firstLine;
        }

        private async void CheckForUpdatesOnStartup()
        {
            try
            {
                // Add a small delay to not interfere with startup
                await Task.Delay(3000);

                var updateInfo = await UpdateService.CheckForUpdatesAsync();
                if (updateInfo.UpdateAvailable)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var updateDialog = new UpdateDialog(updateInfo);
                        updateDialog.Show();
                    });
                }
            }
            catch (Exception)
            {
                // Silently fail - don't interrupt user experience if update check fails
            }
        }

        private async void OnCheckForUpdatesClick(object? sender, EventArgs e)
        {
            try
            {
                ShowNotification("업데이트 확인", "업데이트를 확인하는 중...");

                var updateInfo = await UpdateService.CheckForUpdatesAsync();
                
                if (updateInfo.UpdateAvailable)
                {
                    var updateDialog = new UpdateDialog(updateInfo);
                    updateDialog.Show();
                }
                else
                {
                    ShowNotification("업데이트 확인", "현재 최신 버전을 사용하고 있습니다.");
                }
            }
            catch (Exception ex)
            {
                ShowNotification("업데이트 확인 실패", $"업데이트를 확인하는 동안 오류가 발생했습니다: {ex.Message}");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _hotkeyService?.Dispose();
            _notifyIcon?.Dispose();
            base.OnClosing(e);
        }
    }
}
