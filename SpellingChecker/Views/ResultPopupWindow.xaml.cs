using System;
using System.Windows;

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
            ResultTextBox.Text = result;
            OriginalTextBox.Text = original;
            _isTranslationMode = isTranslationMode;

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
            return ResultTextBox.Text;
        }

        public void UpdateResult(string newResult)
        {
            ResultTextBox.Text = newResult;
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
