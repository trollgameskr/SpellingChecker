using System.Windows;

namespace SpellingChecker.Views
{
    /// <summary>
    /// Dialog for adding or editing tone presets
    /// </summary>
    public partial class TonePresetDialog : Window
    {
        public string PresetName { get; private set; } = string.Empty;
        public string PresetDescription { get; private set; } = string.Empty;

        public TonePresetDialog()
        {
            InitializeComponent();
            DialogTitle.Text = "새 톤 프리셋 추가";
        }

        public TonePresetDialog(string name, string description) : this()
        {
            DialogTitle.Text = "톤 프리셋 수정";
            PresetNameTextBox.Text = name;
            PresetDescriptionTextBox.Text = description;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PresetNameTextBox.Text))
            {
                MessageBox.Show("프리셋 이름을 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PresetDescriptionTextBox.Text))
            {
                MessageBox.Show("톤 설명을 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PresetName = PresetNameTextBox.Text.Trim();
            PresetDescription = PresetDescriptionTextBox.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
