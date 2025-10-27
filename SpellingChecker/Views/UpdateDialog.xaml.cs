using System;
using System.Windows;
using SpellingChecker.Services;

namespace SpellingChecker.Views
{
    /// <summary>
    /// Update dialog window
    /// </summary>
    public partial class UpdateDialog : Window
    {
        private readonly UpdateService.UpdateInfo _updateInfo;

        public UpdateDialog(UpdateService.UpdateInfo updateInfo)
        {
            InitializeComponent();
            _updateInfo = updateInfo;
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            // Set version information
            CurrentVersionText.Text = $"현재 버전: {_updateInfo.CurrentVersion}";
            LatestVersionText.Text = $"최신 버전: {_updateInfo.LatestVersion}";

            // Set release notes
            if (!string.IsNullOrEmpty(_updateInfo.ReleaseNotes))
            {
                ReleaseNotesText.Text = _updateInfo.ReleaseNotes;
            }
            else
            {
                ReleaseNotesText.Text = "릴리스 노트를 사용할 수 없습니다.";
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_updateInfo.DownloadUrl))
            {
                MessageBox.Show("다운로드 URL을 찾을 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Disable buttons and show progress
            UpdateButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            ProgressPanel.Visibility = Visibility.Visible;

            try
            {
                var progress = new Progress<int>(value =>
                {
                    DownloadProgress.Value = value;
                    ProgressText.Text = $"다운로드 중... {value}%";
                });

                var success = await UpdateService.DownloadAndInstallUpdateAsync(_updateInfo.DownloadUrl, progress);

                if (!success)
                {
                    MessageBox.Show("업데이트를 다운로드하거나 설치하는 동안 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    ProgressPanel.Visibility = Visibility.Collapsed;
                }
                // If successful, the application will exit and restart
            }
            catch (Exception ex)
            {
                MessageBox.Show($"업데이트 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
