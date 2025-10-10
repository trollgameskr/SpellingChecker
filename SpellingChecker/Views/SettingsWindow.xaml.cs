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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _settings.ApiKey = ApiKeyTextBox.Password;
                _settings.ApiEndpoint = ApiEndpointTextBox.Text;
                _settings.Model = ModelTextBox.Text;
                _settings.AutoStartWithWindows = AutoStartCheckBox.IsChecked ?? false;

                _settingsService.SaveSettings(_settings);
                
                MessageBox.Show("Settings saved successfully!", "Success", 
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
    }
}
