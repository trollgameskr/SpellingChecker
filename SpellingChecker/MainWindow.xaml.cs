using System;
using System.ComponentModel;
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
            var success = _hotkeyService.RegisterHotkeys(helper.Handle);

            if (!success)
            {
                MessageBox.Show("Failed to register hotkeys. The application may not work correctly.", 
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            _hotkeyService.SpellingCorrectionRequested += OnSpellingCorrectionRequested;
            _hotkeyService.TranslationRequested += OnTranslationRequested;

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

                var result = await _aiService.CorrectSpellingAsync(selectedText);
                
                var popup = new ResultPopupWindow(result.CorrectedText, selectedText, "Spelling Correction");
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(result.CorrectedText);
                popup.ReplaceRequested += (s, args) => _clipboardService.ReplaceSelectedText(result.CorrectedText);
                popup.ShowDialog();
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

                var result = await _aiService.TranslateAsync(selectedText);
                
                var popup = new ResultPopupWindow(
                    result.TranslatedText, 
                    selectedText, 
                    $"Translation ({result.SourceLanguage} → {result.TargetLanguage})"
                );
                popup.CopyRequested += (s, args) => _clipboardService.SetClipboard(result.TranslatedText);
                popup.ReplaceRequested += (s, args) => _clipboardService.ReplaceSelectedText(result.TranslatedText);
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

        private void ShowNotification(string title, string message)
        {
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
