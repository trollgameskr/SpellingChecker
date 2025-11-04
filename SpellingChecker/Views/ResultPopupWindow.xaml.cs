using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SpellingChecker.Views
{
    /// <summary>
    /// Popup window to display correction/translation results
    /// </summary>
    public partial class ResultPopupWindow : Window
    {
        public event EventHandler? CopyRequested;
        public event EventHandler<string>? ConvertRequested;
        public event EventHandler<string>? ToneChangeRequested;

        private readonly bool _isTranslationMode;
        private readonly bool _enableHighlighting;
        private readonly Models.AppSettings? _settings;
        private readonly string _originalText;
        private bool _isInitializing = true;
        private string? _appliedToneName = null;
        private bool _isVariableMode = true; // true = variable mode (camelCase), false = function mode (PascalCase)
        private readonly bool _isVariableNameMode;

        public ResultPopupWindow(string result, string original, string title, bool isTranslationMode = false, Models.AppSettings? settings = null, bool enableHighlighting = false, bool isVariableNameMode = false)
        {
            InitializeComponent();
            
            Title = title;
            OriginalTextBox.Text = original;
            _isTranslationMode = isTranslationMode;
            _enableHighlighting = enableHighlighting;
            _settings = settings;
            _originalText = original;
            _isVariableNameMode = isVariableNameMode;

            // Show toggle button only in variable name mode
            if (_isVariableNameMode)
            {
                ToggleVariableFunctionButton.Visibility = Visibility.Visible;
            }

            // Set result text with highlighting if highlighting is enabled
            if (_enableHighlighting)
            {
                SetResultWithHighlighting(result, original);
                
                // Show tone preset UI only in spelling correction mode
                if (settings != null)
                {
                    TonePresetPanel.Visibility = Visibility.Visible;
                    LoadTonePresets();
                }
            }
            else
            {
                SetResultTextPlain(result);
            }

            // Position window near cursor
            var point = System.Windows.Forms.Cursor.Position;
            Left = point.X - Width / 2;
            Top = point.Y - Height - 20;

            // Ensure window is on screen
            if (Left < 0) Left = 0;
            if (Top < 0) Top = 0;
            if (Left + Width > SystemParameters.PrimaryScreenWidth)
                Left = SystemParameters.PrimaryScreenWidth - Width;
        }

        public string GetResultText()
        {
            return new TextRange(ResultRichTextBox.Document.ContentStart, ResultRichTextBox.Document.ContentEnd).Text.TrimEnd('\r', '\n');
        }

        public void UpdateResult(string newResult)
        {
            if (_enableHighlighting)
            {
                SetResultWithHighlighting(newResult, OriginalTextBox.Text);
            }
            else
            {
                SetResultTextPlain(newResult);
            }
        }

        public void UpdateResultWithTone(string newResult, string? appliedToneName)
        {
            _appliedToneName = appliedToneName;
            UpdateResult(newResult);
            UpdateAppliedToneDisplay();
        }

        public void ShowProgressIndicator()
        {
            ProgressOverlay.Visibility = Visibility.Visible;
        }

        public void HideProgressIndicator()
        {
            ProgressOverlay.Visibility = Visibility.Collapsed;
        }

        public void SetProgressText(string text)
        {
            ProgressText.Text = text;
        }

        private void SetResultTextPlain(string text)
        {
            ResultRichTextBox.Document.Blocks.Clear();
            var paragraph = new Paragraph(new Run(text));
            ResultRichTextBox.Document.Blocks.Add(paragraph);
        }

        private void SetResultWithHighlighting(string corrected, string original)
        {
            ResultRichTextBox.Document.Blocks.Clear();
            
            var paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0);
            
            var changes = FindChanges(original, corrected);
            
            if (changes.Count == 0)
            {
                // No changes, display plain text
                paragraph.Inlines.Add(new Run(corrected));
            }
            else
            {
                // Highlight changes
                int lastIndex = 0;
                foreach (var change in changes.OrderBy(c => c.CorrectedStart))
                {
                    // Add unchanged text before this change
                    if (change.CorrectedStart > lastIndex)
                    {
                        paragraph.Inlines.Add(new Run(corrected.Substring(lastIndex, change.CorrectedStart - lastIndex)));
                    }
                    
                    // Add highlighted changed text
                    var highlightedRun = new Run(change.CorrectedText)
                    {
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 0)), // Yellow highlight
                        Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)) // Black text
                    };
                    paragraph.Inlines.Add(highlightedRun);
                    
                    lastIndex = change.CorrectedStart + change.CorrectedText.Length;
                }
                
                // Add any remaining text after the last change
                if (lastIndex < corrected.Length)
                {
                    paragraph.Inlines.Add(new Run(corrected.Substring(lastIndex)));
                }
            }
            
            ResultRichTextBox.Document.Blocks.Add(paragraph);
        }

        private List<TextChange> FindChanges(string original, string corrected)
        {
            var changes = new List<TextChange>();
            
            // Simple word-by-word comparison
            var originalWords = SplitIntoWords(original);
            var correctedWords = SplitIntoWords(corrected);
            
            int correctedPos = 0;
            
            for (int i = 0; i < Math.Max(originalWords.Count, correctedWords.Count); i++)
            {
                string? origWord = i < originalWords.Count ? originalWords[i].Word : null;
                string? corrWord = i < correctedWords.Count ? correctedWords[i].Word : null;
                
                if (origWord != corrWord)
                {
                    if (corrWord != null)
                    {
                        changes.Add(new TextChange
                        {
                            OriginalText = origWord ?? "",
                            CorrectedText = corrWord,
                            CorrectedStart = correctedPos
                        });
                    }
                }
                
                if (corrWord != null)
                {
                    correctedPos += corrWord.Length;
                }
            }
            
            return changes;
        }

        private List<WordInfo> SplitIntoWords(string text)
        {
            var words = new List<WordInfo>();
            var currentWord = "";
            int startPos = 0;
            
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
                {
                    if (currentWord.Length > 0)
                    {
                        words.Add(new WordInfo { Word = currentWord, Start = startPos });
                        currentWord = "";
                    }
                    words.Add(new WordInfo { Word = c.ToString(), Start = i });
                    startPos = i + 1;
                }
                else
                {
                    if (currentWord.Length == 0)
                    {
                        startPos = i;
                    }
                    currentWord += c;
                }
            }
            
            if (currentWord.Length > 0)
            {
                words.Add(new WordInfo { Word = currentWord, Start = startPos });
            }
            
            return words;
        }

        private class TextChange
        {
            public string OriginalText { get; set; } = "";
            public string CorrectedText { get; set; } = "";
            public int CorrectedStart { get; set; }
        }

        private class WordInfo
        {
            public string Word { get; set; } = "";
            public int Start { get; set; }
        }


        private void LoadTonePresets()
        {
            if (_settings == null || _settings.TonePresets == null)
                return;

            TonePresetComboBox.ItemsSource = _settings.TonePresets;
            
            // Select the current tone preset
            if (!string.IsNullOrEmpty(_settings.SelectedTonePresetId))
            {
                var selectedPreset = _settings.TonePresets.FirstOrDefault(p => p.Id == _settings.SelectedTonePresetId);
                if (selectedPreset != null)
                {
                    TonePresetComboBox.SelectedItem = selectedPreset;
                    TonePresetDescriptionTextBlock.Text = selectedPreset.Description;
                }
            }
            else if (_settings.TonePresets.Count > 0)
            {
                TonePresetComboBox.SelectedIndex = 0;
                TonePresetDescriptionTextBlock.Text = _settings.TonePresets[0].Description;
            }
            
            // Initialization complete
            _isInitializing = false;
            
            // Update the applied tone display
            UpdateAppliedToneDisplay();
        }

        private void UpdateAppliedToneDisplay()
        {
            if (AppliedToneTextBlock == null)
                return;

            if (!string.IsNullOrEmpty(_appliedToneName))
            {
                AppliedToneTextBlock.Text = $"적용된 톤: {_appliedToneName}";
                AppliedToneTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                AppliedToneTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void TonePresetComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Skip if this is triggered during initialization
            if (_isInitializing)
                return;
            
            if (TonePresetComboBox.SelectedItem is Models.TonePreset selectedPreset)
            {
                TonePresetDescriptionTextBlock.Text = selectedPreset.Description;
                
                // Update the selected tone preset in settings
                if (_settings != null)
                {
                    _settings.SelectedTonePresetId = selectedPreset.Id;
                }
                
                // Update the applied tone name for the upcoming conversion
                // Don't show indicator for "톤 없음" (default-none)
                _appliedToneName = (selectedPreset.Id != "default-none") ? selectedPreset.Name : null;
                
                // Show progress indicator with tone change message
                SetProgressText("대화톤 변경 중...");
                ShowProgressIndicator();
                
                // Request re-correction with the new tone using current text from Original TextBox
                // (user may have edited it)
                ToneChangeRequested?.Invoke(this, OriginalTextBox.Text);
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            CopyRequested?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the text from the Original TextBox and request re-conversion
            string textToConvert = OriginalTextBox.Text;
            ConvertRequested?.Invoke(this, textToConvert);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OriginalTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Trigger conversion when Ctrl+Enter is pressed
            if (e.Key == System.Windows.Input.Key.Enter && (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl)))
            {
                e.Handled = true;
                ConvertButton_Click(sender, new RoutedEventArgs());
            }
        }

        private void ToggleVariableFunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isVariableNameMode)
                return;

            // Toggle mode
            _isVariableMode = !_isVariableMode;

            // Update button text
            ToggleVariableFunctionButton.Content = _isVariableMode ? "변수 → 함수" : "함수 → 변수";

            // Get current result text
            var currentResult = GetResultText();

            // Convert the names
            var convertedResult = ConvertNamingStyle(currentResult, _isVariableMode);

            // Update the display
            SetResultTextPlain(convertedResult);
        }

        private string ConvertNamingStyle(string text, bool toVariableMode)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var convertedLines = new List<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Extract the number prefix if exists (e.g., "1. ", "2. ", etc.)
                var dotIndex = trimmedLine.IndexOf('.');
                string prefix = "";
                string name = trimmedLine;
                
                if (dotIndex > 0 && dotIndex < trimmedLine.Length - 1)
                {
                    // Check if the part before dot is a number
                    var beforeDot = trimmedLine.Substring(0, dotIndex).Trim();
                    if (int.TryParse(beforeDot, out _))
                    {
                        prefix = trimmedLine.Substring(0, dotIndex + 1) + " ";
                        name = trimmedLine.Substring(dotIndex + 1).TrimStart();
                    }
                }

                // Remove trailing parentheses if present (for function mode)
                name = name.TrimEnd('(', ')', ' ');

                // Convert naming style
                string convertedName;
                if (toVariableMode)
                {
                    // Convert to camelCase (variable style)
                    convertedName = ToCamelCase(name);
                }
                else
                {
                    // Convert to PascalCase with parentheses (function style)
                    convertedName = ToPascalCase(name) + "()";
                }

                convertedLines.Add(prefix + convertedName);
            }

            return string.Join("\n", convertedLines);
        }

        private string ToCamelCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // If already in camelCase, return as is
            if (char.IsLower(text[0]))
                return text;

            // Convert first character to lowercase
            return char.ToLower(text[0]) + text.Substring(1);
        }

        private string ToPascalCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // If already in PascalCase, return as is
            if (char.IsUpper(text[0]))
                return text;

            // Convert first character to uppercase
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Close window when ESC is pressed
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                Close();
            }
        }
    }
}
