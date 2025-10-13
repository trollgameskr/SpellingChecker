using System;
using System.Reflection;
using System.Windows;
using SpellingChecker.Models;
using SpellingChecker.Services;

namespace SpellingChecker.Views
{
    /// <summary>
    /// Settings window for configuring the application
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;
        private AppSettings _settings;

        public SettingsWindow()
        {
            InitializeComponent();
            
            // Set window title with app version
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"Settings - AI Spelling Checker v{version?.Major}.{version?.Minor}.{version?.Build}";
            
            _settingsService = new SettingsService();
            _settings = _settingsService.LoadSettings();
            
            LoadSettings();
        }

        private void LoadSettings()
        {
            ApiKeyTextBox.Password = _settings.ApiKey;
            ApiEndpointTextBox.Text = _settings.ApiEndpoint;
            ModelTextBox.Text = _settings.Model;
            AutoStartCheckBox.IsChecked = _settings.AutoStartWithWindows;
            ShowProgressNotificationsCheckBox.IsChecked = _settings.ShowProgressNotifications;
            SpellingHotkeyTextBox.Text = _settings.SpellingCorrectionHotkey;
            TranslationHotkeyTextBox.Text = _settings.TranslationHotkey;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate hotkeys before saving
                if (!HotkeyParser.IsValidHotkey(SpellingHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid spelling correction hotkey format. Please use format like 'Ctrl+Shift+Alt+Y'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!HotkeyParser.IsValidHotkey(TranslationHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid translation hotkey format. Please use format like 'Ctrl+Shift+Alt+T'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _settings.ApiKey = ApiKeyTextBox.Password;
                _settings.ApiEndpoint = ApiEndpointTextBox.Text;
                _settings.Model = ModelTextBox.Text;
                _settings.AutoStartWithWindows = AutoStartCheckBox.IsChecked ?? false;
                _settings.ShowProgressNotifications = ShowProgressNotificationsCheckBox.IsChecked ?? false;
                _settings.SpellingCorrectionHotkey = SpellingHotkeyTextBox.Text;
                _settings.TranslationHotkey = TranslationHotkeyTextBox.Text;

                _settingsService.SaveSettings(_settings);
                
                MessageBox.Show("Settings saved successfully! Please restart the application for hotkey changes to take effect.", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ViewUsageStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var usageWindow = new UsageStatisticsWindow();
            usageWindow.ShowDialog();
        }
    }
}
