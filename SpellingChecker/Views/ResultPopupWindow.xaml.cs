using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

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
        private Services.AIService? _aiService;

        public ResultPopupWindow(string result, string original, string title, bool isTranslationMode = false, Models.AppSettings? settings = null, bool enableHighlighting = false)
        {
            InitializeComponent();
            
            Title = title;
            OriginalTextBox.Text = original;
            _isTranslationMode = isTranslationMode;
            _enableHighlighting = enableHighlighting;
            _settings = settings;
            _originalText = original;

            // Initialize AI service for word definition lookup
            var settingsService = new Services.SettingsService();
            _aiService = new Services.AIService(settingsService);

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

        private async void ResultRichTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_aiService == null)
                return;

            // Get the position of the click
            var position = e.GetPosition(ResultRichTextBox);
            var textPointer = ResultRichTextBox.GetPositionFromPoint(position, true);
            
            if (textPointer == null)
                return;

            // Get the word at the clicked position
            var word = GetWordAtPosition(textPointer);
            
            if (string.IsNullOrWhiteSpace(word))
                return;

            // Show the popup with loading state
            DefinitionWordTextBlock.Text = word;
            DefinitionTextBlock.Text = "정의를 불러오는 중...";
            DefinitionProgressBar.Visibility = Visibility.Visible;
            WordDefinitionPopup.IsOpen = true;

            try
            {
                // Get the word definition from AI service
                var result = await _aiService.GetWordDefinitionAsync(word);
                
                // Update the popup with the definition
                DefinitionTextBlock.Text = result.Definition;
                DefinitionProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                DefinitionTextBlock.Text = $"오류: {ex.Message}";
                DefinitionProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private string GetWordAtPosition(TextPointer position)
        {
            if (position == null)
                return string.Empty;

            // Get the text run at the position
            var textRun = position.Parent as Run;
            if (textRun == null)
            {
                // Try to find the nearest run
                var contextBefore = position.GetTextInRun(LogicalDirection.Backward);
                var contextAfter = position.GetTextInRun(LogicalDirection.Forward);
                
                if (!string.IsNullOrEmpty(contextBefore) || !string.IsNullOrEmpty(contextAfter))
                {
                    var fullContext = contextBefore + contextAfter;
                    return ExtractWordFromContext(fullContext, contextBefore.Length);
                }
                return string.Empty;
            }

            // Get the text of the run
            var text = textRun.Text;
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Find the character offset within the run
            var runStart = textRun.ContentStart;
            var offset = runStart.GetOffsetToPosition(position);
            
            return ExtractWordFromContext(text, offset);
        }

        private string ExtractWordFromContext(string text, int clickPosition)
        {
            if (string.IsNullOrWhiteSpace(text) || clickPosition < 0 || clickPosition > text.Length)
                return string.Empty;

            // Find the word boundaries around the click position
            int start = clickPosition;
            int end = clickPosition;

            // Move start backwards to find the beginning of the word
            while (start > 0 && IsWordCharacter(text[start - 1]))
            {
                start--;
            }

            // Move end forwards to find the end of the word
            while (end < text.Length && IsWordCharacter(text[end]))
            {
                end++;
            }

            if (start >= end)
                return string.Empty;

            return text.Substring(start, end - start).Trim();
        }

        private bool IsWordCharacter(char c)
        {
            // Consider letters, numbers, and Korean characters as word characters
            return char.IsLetterOrDigit(c) || (c >= 0xAC00 && c <= 0xD7A3) || c == '\'' || c == '-';
        }
    }
}
