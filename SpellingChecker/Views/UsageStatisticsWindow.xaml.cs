using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SpellingChecker.Services;

namespace SpellingChecker.Views
{
    /// <summary>
    /// Window for displaying usage statistics and history
    /// </summary>
    public partial class UsageStatisticsWindow : Window
    {
        private readonly UsageService _usageService;

        public UsageStatisticsWindow()
        {
            InitializeComponent();
            _usageService = new UsageService();
            LoadStatistics();
        }

        private void LoadStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var statistics = _usageService.GetStatistics(startDate, endDate);
            var records = _usageService.LoadAllRecords();

            if (startDate.HasValue)
            {
                records = records.Where(r => r.Timestamp >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                records = records.Where(r => r.Timestamp <= endDate.Value).ToList();
            }

            // Update summary statistics
            TotalOperationsText.Text = (statistics.TotalCorrectionCount + statistics.TotalTranslationCount).ToString();
            TotalCorrectionsText.Text = statistics.TotalCorrectionCount.ToString();
            TotalTranslationsText.Text = statistics.TotalTranslationCount.ToString();
            PromptTokensText.Text = statistics.TotalPromptTokens.ToString("N0");
            CompletionTokensText.Text = statistics.TotalCompletionTokens.ToString("N0");
            TotalTokensText.Text = statistics.TotalTokens.ToString("N0");
            TotalCostText.Text = statistics.TotalCost.ToString("C6");

            // Update history grid (sorted by most recent first)
            HistoryDataGrid.ItemsSource = records.OrderByDescending(r => r.Timestamp).ToList();
        }

        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Prevent execution during initialization before controls are ready
            if (TotalOperationsText == null)
                return;

            if (PeriodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var now = DateTime.Now;
                DateTime? startDate = null;
                DateTime? endDate = null;

                switch (selectedItem.Content.ToString())
                {
                    case "Today":
                        startDate = now.Date;
                        endDate = now.Date.AddDays(1).AddSeconds(-1);
                        break;
                    case "This Week":
                        var daysSinceMonday = ((int)now.DayOfWeek - 1 + 7) % 7;
                        startDate = now.Date.AddDays(-daysSinceMonday);
                        endDate = now.Date.AddDays(7 - daysSinceMonday).AddSeconds(-1);
                        break;
                    case "This Month":
                        startDate = new DateTime(now.Year, now.Month, 1);
                        endDate = startDate.Value.AddMonths(1).AddSeconds(-1);
                        break;
                    case "All Time":
                    default:
                        break;
                }

                LoadStatistics(startDate, endDate);
            }
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all usage history? This action cannot be undone.",
                "Clear History",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _usageService.ClearHistory();
                LoadStatistics();
                MessageBox.Show(
                    "Usage history has been cleared successfully.",
                    "History Cleared",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
