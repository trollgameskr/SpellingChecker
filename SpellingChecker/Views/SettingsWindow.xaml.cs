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
            ApiKeyTextBox.Password = _settings.ApiKey;
            ApiEndpointTextBox.Text = _settings.ApiEndpoint;
            AutoStartCheckBox.IsChecked = _settings.AutoStartWithWindows;
            ShowProgressNotificationsCheckBox.IsChecked = _settings.ShowProgressNotifications;
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
                ModelDescriptionTextBlock.Text = "gpt-4o-mini: Fast, cost-effective, good quality (recommended)\ngpt-4o: Slower, more expensive, highest quality\no1: Advanced reasoning model\no1-mini: Faster reasoning model";
            }
            else if (provider == "Anthropic")
            {
                ModelDescriptionTextBlock.Text = "claude-sonnet-4-5: Best for coding and agents (recommended)\nclaude-3-5-sonnet-latest: Balanced performance\nclaude-3-5-haiku-latest: Fast and efficient";
            }
            else if (provider == "Gemini")
            {
                ModelDescriptionTextBlock.Text = "gemini-2.5-pro-latest: Complex reasoning and analysis\ngemini-2.5-flash-latest: Fast with good performance (recommended)\ngemini-2.0-flash-exp: Experimental fast model";
            }
        }

        private void ProviderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProviderComboBox.SelectedItem is string selectedProvider)
            {
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

                if (!HotkeyParser.IsValidHotkey(VariableNameSuggestionHotkeyTextBox.Text))
                {
                    MessageBox.Show("Invalid variable name suggestion hotkey format. Please use format like 'Ctrl+Shift+Alt+V'", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _settings.ApiKey = ApiKeyTextBox.Password;
                _settings.ApiEndpoint = ApiEndpointTextBox.Text;
                _settings.Provider = ProviderComboBox.SelectedItem?.ToString() ?? "OpenAI";
                
                // Get model from ComboBox (can be selected item or typed text)
                string modelName = string.IsNullOrWhiteSpace(ModelComboBox.Text) 
                    ? (ModelComboBox.SelectedItem?.ToString() ?? AIProviderConfig.GetDefaultModel(_settings.Provider))
                    : ModelComboBox.Text.Trim();
                
                _settings.Model = modelName;
                
                // Save custom model if it's not in the default list
                var defaultModels = AIProviderConfig.ProviderModels.ContainsKey(_settings.Provider) 
                    ? AIProviderConfig.ProviderModels[_settings.Provider] 
                    : Array.Empty<string>();
                
                if (!defaultModels.Contains(modelName) && !string.IsNullOrWhiteSpace(modelName))
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
