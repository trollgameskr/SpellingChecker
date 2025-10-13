using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
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
            _clipboardService = new ClipboardService();
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
            var success = _hotkeyService.RegisterHotkeys(helper.Handle, settings.SpellingCorrectionHotkey, settings.TranslationHotkey, settings.VariableNameSuggestionHotkey);

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
                    $"맞춤법 교정: {settings.SpellingCorrectionHotkey}\n" +
                    $"번역: {settings.TranslationHotkey}\n" +
                    $"변수명 추천: {settings.VariableNameSuggestionHotkey}");
            }

            _hotkeyService.SpellingCorrectionRequested += OnSpellingCorrectionRequested;
            _hotkeyService.TranslationRequested += OnTranslationRequested;
            _hotkeyService.VariableNameSuggestionRequested += OnVariableNameSuggestionRequested;

            // Add hook to process hotkey messages
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(HwndHook);
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

                // Show progress notification
                ShowNotification("Processing...", "AI is correcting your text. Please wait...", true);

                var result = await _aiService.CorrectSpellingAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("맞춤법 교정 완료", 
                    $"교정 결과는 '{result.CorrectedText}' 입니다.", true);
                
                var settings = _settingsService.LoadSettings();
                var popup = new ResultPopupWindow(result.CorrectedText, selectedText, "Spelling Correction", false, settings);
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(popup.GetResultText());
                popup.ConvertRequested += async (s, text) => await ReprocessSpellingCorrection(popup, text);
                popup.ToneChangeRequested += async (s, text) => await ReprocessSpellingCorrection(popup, text);
                popup.ShowDialog();
                
                // Save settings after popup closes to persist tone selection
                _settingsService.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                ShowNotification("Error", ex.Message);
            }
        }

        private async void OnTranslationRequested(object? sender, EventArgs e)
        {
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

                // Show progress notification
                ShowNotification("Processing...", "AI is translating your text. Please wait...", true);

                var result = await _aiService.TranslateAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("번역 완료", 
                    $"번역 결과는 '{result.TranslatedText}' 입니다.\n" +
                    $"언어: {result.SourceLanguage} → {result.TargetLanguage}", true);
                
                var popup = new ResultPopupWindow(
                    result.TranslatedText, 
                    selectedText, 
                    $"Translation ({result.SourceLanguage} → {result.TargetLanguage})",
                    true
                );
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(popup.GetResultText());
                popup.ConvertRequested += async (s, text) => await ReprocessTranslation(popup, text);
                popup.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ReprocessSpellingCorrection(ResultPopupWindow popup, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    ShowNotification("No text", "Please enter some text to correct.");
                    return;
                }

                ShowNotification("Processing...", "AI is correcting your text. Please wait...", true);

                var result = await _aiService.CorrectSpellingAsync(text);
                
                ShowNotification("맞춤법 교정 완료", 
                    $"교정 결과는 '{result.CorrectedText}' 입니다.", true);
                
                popup.UpdateResult(result.CorrectedText);
            }
            catch (Exception ex)
            {
                ShowNotification("Error", ex.Message);
            }
        }

        private async Task ReprocessTranslation(ResultPopupWindow popup, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    ShowNotification("No text", "Please enter some text to translate.");
                    return;
                }

                ShowNotification("Processing...", "AI is translating your text. Please wait...", true);

                var result = await _aiService.TranslateAsync(text);
                
                ShowNotification("번역 완료", 
                    $"번역 결과는 '{result.TranslatedText}' 입니다.\n" +
                    $"언어: {result.SourceLanguage} → {result.TargetLanguage}", true);
                
                popup.UpdateResult(result.TranslatedText);
            }
            catch (Exception ex)
            {
                ShowNotification("Error", ex.Message);
            }
        }

        private async void OnVariableNameSuggestionRequested(object? sender, EventArgs e)
        {
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

                // Show progress notification
                ShowNotification("Processing...", "AI is suggesting variable names. Please wait...", true);

                var result = await _aiService.SuggestVariableNamesAsync(selectedText);
                
                // Show debug notification with result
                ShowNotification("변수명 추천 완료", 
                    $"추천된 변수명: {string.Join(", ", result.SuggestedNames)}", true);
                
                // Format the suggestions for display
                var formattedSuggestions = string.Join("\n", result.SuggestedNames.Select((name, index) => $"{index + 1}. {name}"));
                
                var popup = new ResultPopupWindow(
                    formattedSuggestions, 
                    selectedText, 
                    "Variable Name Suggestions (C#)",
                    false
                );
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(popup.GetResultText());
                popup.ShowDialog();
            }
            catch (Exception ex)
            {
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

        protected override void OnClosing(CancelEventArgs e)
        {
            _hotkeyService?.Dispose();
            _notifyIcon?.Dispose();
            base.OnClosing(e);
        }
    }
}
