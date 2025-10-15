using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Collections.Generic;
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

        // Model definitions for different providers
        private static readonly Dictionary<AIProvider, List<ModelInfo>> ProviderModels = new()
        {
            {
                AIProvider.OpenAI, new List<ModelInfo>
                {
                    new ModelInfo { Name = "gpt-4o-mini", Description = "Fast, cost-effective, good quality (recommended)" },
                    new ModelInfo { Name = "gpt-4o", Description = "Slower, more expensive, highest quality" },
                    new ModelInfo { Name = "gpt-3.5-turbo", Description = "Faster, less expensive, lower quality" }
                }
            },
            {
                AIProvider.AzureOpenAI, new List<ModelInfo>
                {
                    new ModelInfo { Name = "gpt-4o-mini", Description = "Fast, cost-effective, good quality (recommended)" },
                    new ModelInfo { Name = "gpt-4o", Description = "Slower, more expensive, highest quality" },
                    new ModelInfo { Name = "gpt-35-turbo", Description = "Faster, less expensive, lower quality" }
                }
            },
            {
                AIProvider.Custom, new List<ModelInfo>
                {
                    new ModelInfo { Name = "gpt-4o-mini", Description = "Enter custom model name" }
                }
            }
        };

        private class ModelInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

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
            
            // Load providers
            ProviderComboBox.ItemsSource = Enum.GetValues(typeof(AIProvider));
            ProviderComboBox.SelectedItem = _settings.Provider;
            
            // Load models for the selected provider
            LoadModelsForProvider(_settings.Provider);
            ModelComboBox.Text = _settings.Model;
            
            // Load tone presets
            LoadTonePresets();
        }

        private void LoadModelsForProvider(AIProvider provider)
        {
            if (ProviderModels.TryGetValue(provider, out var models))
            {
                ModelComboBox.ItemsSource = models.Select(m => m.Name).ToList();
                
                // Update the description when a model is selected
                if (ModelComboBox.SelectedItem != null)
                {
                    var selectedModel = models.FirstOrDefault(m => m.Name == ModelComboBox.SelectedItem.ToString());
                    if (selectedModel != null)
                    {
                        ModelDescriptionTextBlock.Text = selectedModel.Description;
                    }
                }
            }
        }

        private void ProviderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProviderComboBox.SelectedItem is AIProvider selectedProvider)
            {
                LoadModelsForProvider(selectedProvider);
                
                // Update default endpoint based on provider
                switch (selectedProvider)
                {
                    case AIProvider.OpenAI:
                        if (ApiEndpointTextBox.Text == "https://your-resource.openai.azure.com/" || 
                            string.IsNullOrWhiteSpace(ApiEndpointTextBox.Text))
                        {
                            ApiEndpointTextBox.Text = "https://api.openai.com/v1";
                        }
                        break;
                    case AIProvider.AzureOpenAI:
                        if (ApiEndpointTextBox.Text == "https://api.openai.com/v1" || 
                            string.IsNullOrWhiteSpace(ApiEndpointTextBox.Text))
                        {
                            ApiEndpointTextBox.Text = "https://your-resource.openai.azure.com/";
                        }
                        break;
                    case AIProvider.Custom:
                        // Don't change endpoint for custom provider
                        break;
                }
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
                _settings.Model = ModelComboBox.Text;
                _settings.Provider = (AIProvider)(ProviderComboBox.SelectedItem ?? AIProvider.OpenAI);
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
