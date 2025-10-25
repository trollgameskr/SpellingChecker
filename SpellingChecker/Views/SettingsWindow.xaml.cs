using System;
using System.Collections.Generic;
using System.Linq;
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
            // Load API key for current provider
            string currentProvider = _settings.Provider ?? "OpenAI";
            ApiKeyTextBox.Password = _settings.GetApiKeyForProvider(currentProvider);
            
            ApiEndpointTextBox.Text = _settings.ApiEndpoint;
            AutoStartCheckBox.IsChecked = _settings.AutoStartWithWindows;
            ShowProgressNotificationsCheckBox.IsChecked = _settings.ShowProgressNotifications;
            CommonQuestionHotkeyTextBox.Text = _settings.CommonQuestionHotkey;
            SpellingHotkeyTextBox.Text = _settings.SpellingCorrectionHotkey;
            TranslationHotkeyTextBox.Text = _settings.TranslationHotkey;
            VariableNameSuggestionHotkeyTextBox.Text = _settings.VariableNameSuggestionHotkey;
            
            // Load provider options
            LoadProviderOptions();
            
            // Load tone presets
            LoadTonePresets();
        }

        private void LoadProviderOptions()
        {
            // Populate provider combobox
            ProviderComboBox.ItemsSource = AIProviderConfig.Providers;
            
            // Set selected provider
            string selectedProvider = _settings.Provider ?? "OpenAI";
            ProviderComboBox.SelectedItem = selectedProvider;
            
            // Load models for the selected provider
            LoadModelsForProvider(selectedProvider);
        }

        private void LoadModelsForProvider(string provider)
        {
            var models = AIProviderConfig.GetModelsForProvider(provider, _settings.CustomModels);
            ModelComboBox.ItemsSource = models;
            
            // Select current model if it exists in the list, otherwise just set the text
            if (models.Contains(_settings.Model))
            {
                ModelComboBox.SelectedItem = _settings.Model;
            }
            else
            {
                // If model not in list, set it as text (custom model)
                ModelComboBox.Text = _settings.Model;
            }
            
            // Update description based on provider
            UpdateModelDescription(provider);
        }

        private void UpdateModelDescription(string provider)
        {
            if (provider == "OpenAI")
            {
                ModelDescriptionTextBlock.Text = "💡 Tip: Type custom model names like 'gpt-5' or select from: gpt-4o-mini (recommended), gpt-4o, o1, o1-mini";
            }
            else if (provider == "Anthropic")
            {
                ModelDescriptionTextBlock.Text = "💡 Tip: Type custom model names like 'claude-opus-4' or select from: claude-sonnet-4-5 (recommended), claude-3-5-sonnet-latest, claude-3-5-haiku-latest";
            }
            else if (provider == "Gemini")
            {
                ModelDescriptionTextBlock.Text = "💡 Tip: Type custom model names like 'gemini-ultra-2.0' or select from: gemini-2.5-flash-latest (recommended), gemini-2.5-pro-latest, gemini-2.0-flash-exp";
            }
        }

        private void ProviderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProviderComboBox.SelectedItem is string selectedProvider)
            {
                // Save current provider's API key before switching
                if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is string previousProvider)
                {
                    _settings.SetApiKeyForProvider(previousProvider, ApiKeyTextBox.Password);
                }
                
                // Load API key for the new provider
                ApiKeyTextBox.Password = _settings.GetApiKeyForProvider(selectedProvider);
                
                // Update API endpoint based on provider
                ApiEndpointTextBox.Text = AIProviderConfig.GetDefaultEndpoint(selectedProvider);
                
                // Load models for the new provider
                LoadModelsForProvider(selectedProvider);
            }
        }

        private void LoadTonePresets()
        {
            // Clear ItemsSource first to force ComboBox to refresh
            TonePresetComboBox.ItemsSource = null;
            TonePresetComboBox.ItemsSource = _settings.TonePresets;
            
            // Select the current tone preset
            if (!string.IsNullOrEmpty(_settings.SelectedTonePresetId))
            {
                var selectedPreset = _settings.TonePresets?.FirstOrDefault(p => p.Id == _settings.SelectedTonePresetId);
                if (selectedPreset != null)
                {
                    TonePresetComboBox.SelectedItem = selectedPreset;
                }
            }
            else if (_settings.TonePresets?.Count > 0)
            {
                TonePresetComboBox.SelectedIndex = 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate hotkeys before saving
                if (!HotkeyParser.IsValidHotkey(CommonQuestionHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid common question hotkey format. Please use format like 'Ctrl+Alt+1'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!HotkeyParser.IsValidHotkey(SpellingHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid spelling correction hotkey format. Please use format like 'Ctrl+Alt+2'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!HotkeyParser.IsValidHotkey(TranslationHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid translation hotkey format. Please use format like 'Ctrl+Alt+3'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!HotkeyParser.IsValidHotkey(VariableNameSuggestionHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid variable name suggestion hotkey format. Please use format like 'Ctrl+Alt+4'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Save provider-specific API key
                string selectedProvider = ProviderComboBox.SelectedItem?.ToString() ?? "OpenAI";
                _settings.SetApiKeyForProvider(selectedProvider, ApiKeyTextBox.Password);
                
                _settings.ApiEndpoint = ApiEndpointTextBox.Text;
                _settings.Provider = selectedProvider;
                
                // Get model from ComboBox (can be selected item or typed text)
                string modelName = string.IsNullOrWhiteSpace(ModelComboBox.Text) 
                    ? (ModelComboBox.SelectedItem?.ToString() ?? AIProviderConfig.GetDefaultModel(_settings.Provider))
                    : ModelComboBox.Text.Trim();
                
                // If "직접입력" is selected, validate that user has entered a custom model
                if (modelName == "직접입력")
                {
                    MessageBox.Show("모델 이름을 직접 입력해주세요.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                _settings.Model = modelName;
                
                // Save custom model if it's not in the default list
                var defaultModels = AIProviderConfig.ProviderModels.ContainsKey(_settings.Provider) 
                    ? AIProviderConfig.ProviderModels[_settings.Provider] 
                    : Array.Empty<string>();
                
                if (!string.IsNullOrWhiteSpace(modelName) && !defaultModels.Contains(modelName) && modelName != "직접입력")
                {
                    // Initialize CustomModels dictionary if needed
                    if (_settings.CustomModels == null)
                    {
                        _settings.CustomModels = new Dictionary<string, List<string>>();
                    }
                    
                    // Initialize provider list if needed
                    if (!_settings.CustomModels.ContainsKey(_settings.Provider))
                    {
                        _settings.CustomModels[_settings.Provider] = new List<string>();
                    }
                    
                    // Add custom model if not already present
                    if (!_settings.CustomModels[_settings.Provider].Contains(modelName))
                    {
                        _settings.CustomModels[_settings.Provider].Add(modelName);
                    }
                }
                
                _settings.AutoStartWithWindows = AutoStartCheckBox.IsChecked ?? false;
                _settings.ShowProgressNotifications = ShowProgressNotificationsCheckBox.IsChecked ?? false;
                
                _settings.CommonQuestionHotkey = CommonQuestionHotkeyTextBox.Text;
                _settings.SpellingCorrectionHotkey = SpellingHotkeyTextBox.Text;
                _settings.TranslationHotkey = TranslationHotkeyTextBox.Text;
                _settings.VariableNameSuggestionHotkey = VariableNameSuggestionHotkeyTextBox.Text;

                // Save selected tone preset
                if (TonePresetComboBox.SelectedItem is TonePreset selectedPreset)
                {
                    _settings.SelectedTonePresetId = selectedPreset.Id;
                }

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

        private void DeleteAllSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete all saved settings?\n\nThis will:\n- Delete all API keys\n- Reset all settings to defaults\n- Remove custom tone presets\n- Clear usage statistics\n\nThe application will need to be restarted after deletion.",
                "Delete All Settings",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _settingsService.DeleteAllSettings();
                    MessageBox.Show(
                        "All settings have been deleted successfully.\n\nPlease restart the application.",
                        "Settings Deleted",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to delete settings: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void ViewUsageStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var usageWindow = new UsageStatisticsWindow();
            usageWindow.ShowDialog();
        }

        private void TonePresetComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TonePresetComboBox.SelectedItem is TonePreset selectedPreset)
            {
                TonePresetDescriptionTextBlock.Text = selectedPreset.Description;
                
                // Enable edit/delete buttons for all presets
                EditTonePresetButton.IsEnabled = true;
                DeleteTonePresetButton.IsEnabled = true;
            }
        }

        private void AddTonePresetButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TonePresetDialog();
            if (dialog.ShowDialog() == true)
            {
                TonePresetService.AddTonePreset(_settings, dialog.PresetName, dialog.PresetDescription);
                LoadTonePresets();
                
                // Select the newly added preset
                var newPreset = _settings.TonePresets?.LastOrDefault(p => !p.IsDefault);
                if (newPreset != null)
                {
                    TonePresetComboBox.SelectedItem = newPreset;
                }
            }
        }

        private void EditTonePresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (TonePresetComboBox.SelectedItem is TonePreset selectedPreset)
            {
                var dialog = new TonePresetDialog(selectedPreset.Name, selectedPreset.Description);
                if (dialog.ShowDialog() == true)
                {
                    if (TonePresetService.UpdateTonePreset(_settings, selectedPreset.Id, dialog.PresetName, dialog.PresetDescription))
                    {
                        LoadTonePresets();
                        TonePresetComboBox.SelectedItem = selectedPreset;
                    }
                }
            }
        }

        private void DeleteTonePresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (TonePresetComboBox.SelectedItem is TonePreset selectedPreset)
            {
                var result = MessageBox.Show(
                    $"'{selectedPreset.Name}' 톤 프리셋을 삭제하시겠습니까?",
                    "톤 프리셋 삭제",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    TonePresetService.DeleteTonePreset(_settings, selectedPreset.Id);
                    LoadTonePresets();
                }
            }
        }
    }
}
