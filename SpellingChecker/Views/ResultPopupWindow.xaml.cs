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

        private readonly bool _isTranslationMode;

        public ResultPopupWindow(string result, string original, string title, bool isTranslationMode = false)
        {
            InitializeComponent();
            
            Title = title;
            OriginalTextBox.Text = original;
            _isTranslationMode = isTranslationMode;

            // Set result text with highlighting if in spelling correction mode
            if (!isTranslationMode)
            {
                SetResultWithHighlighting(result, original);
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
            if (!_isTranslationMode)
            {
                SetResultWithHighlighting(newResult, OriginalTextBox.Text);
            }
            else
            {
                SetResultTextPlain(newResult);
            }
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
            int originalPos = 0;
            
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
    }
}
