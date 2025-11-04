using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SpellingChecker.Services;
using SpellingChecker.Models;

namespace SpellingChecker.Views
{
    /// <summary>
    /// Window for displaying usage statistics and history
    /// </summary>
    public partial class UsageStatisticsWindow : Window
    {
        private readonly UsageService _usageService;
        private const decimal USD_TO_KRW_RATE = 1350m; // Approximate exchange rate

        public UsageStatisticsWindow()
        {
            _usageService = new UsageService();
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var statistics = _usageService.GetStatistics(startDate, endDate);

            // Update summary statistics
            TotalOperationsText.Text = (statistics.TotalCorrectionCount + statistics.TotalTranslationCount).ToString();
            TotalCorrectionsText.Text = statistics.TotalCorrectionCount.ToString();
            TotalTranslationsText.Text = statistics.TotalTranslationCount.ToString();
            PromptTokensText.Text = statistics.TotalPromptTokens.ToString("N0");
            CompletionTokensText.Text = statistics.TotalCompletionTokens.ToString("N0");
            TotalTokensText.Text = statistics.TotalTokens.ToString("N0");
            TotalCostText.Text = statistics.TotalCost.ToString("C6");
            TotalCostKRWText.Text = $"₩{(statistics.TotalCost * USD_TO_KRW_RATE):N0}";

            // Load data based on current view mode
            LoadDataGridContent(startDate, endDate);
        }

        private void LoadDataGridContent(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (ViewModeComboBox?.SelectedItem is not ComboBoxItem selectedViewMode)
                return;

            var viewMode = selectedViewMode.Content.ToString();

            // Clear existing columns
            HistoryDataGrid.Columns.Clear();

            switch (viewMode)
            {
                case "Daily Breakdown":
                    LoadDailyBreakdown(startDate, endDate);
                    break;
                case "Monthly Breakdown":
                    LoadMonthlyBreakdown(startDate, endDate);
                    break;
                default: // Summary
                    LoadSummaryView(startDate, endDate);
                    break;
            }
        }

        private void LoadSummaryView(DateTime? startDate = null, DateTime? endDate = null)
        {
            var records = _usageService.LoadAllRecords();

            if (startDate.HasValue)
            {
                records = records.Where(r => r.Timestamp >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                records = records.Where(r => r.Timestamp <= endDate.Value).ToList();
            }

            // Set up columns for detailed view
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Date/Time",
                Binding = new System.Windows.Data.Binding("Timestamp") { StringFormat = "{0:yyyy-MM-dd HH:mm:ss}" },
                Width = new DataGridLength(150)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Type",
                Binding = new System.Windows.Data.Binding("OperationType"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Model",
                Binding = new System.Windows.Data.Binding("Model"),
                Width = new DataGridLength(120)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Prompt Tokens",
                Binding = new System.Windows.Data.Binding("PromptTokens"),
                Width = new DataGridLength(110)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Completion Tokens",
                Binding = new System.Windows.Data.Binding("CompletionTokens"),
                Width = new DataGridLength(140)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Total Tokens",
                Binding = new System.Windows.Data.Binding("TotalTokens"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Cost (USD)",
                Binding = new System.Windows.Data.Binding("Cost") { StringFormat = "${0:F6}" },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            HistoryDataGrid.ItemsSource = records.OrderByDescending(r => r.Timestamp).ToList();
        }

        private void LoadDailyBreakdown(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dailyStats = _usageService.GetDailyStatistics(startDate, endDate);

            // Set up columns for daily breakdown
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Date",
                Binding = new System.Windows.Data.Binding("Date") { StringFormat = "{0:yyyy-MM-dd}" },
                Width = new DataGridLength(120)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Operations",
                Binding = new System.Windows.Data.Binding("OperationCount"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Corrections",
                Binding = new System.Windows.Data.Binding("CorrectionCount"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Translations",
                Binding = new System.Windows.Data.Binding("TranslationCount"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Total Tokens",
                Binding = new System.Windows.Data.Binding("TotalTokens") { StringFormat = "{0:N0}" },
                Width = new DataGridLength(120)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Cost (USD)",
                Binding = new System.Windows.Data.Binding("Cost") { StringFormat = "${0:F6}" },
                Width = new DataGridLength(120)
            });
            
            // Add KRW column
            var krwColumn = new DataGridTextColumn
            {
                Header = "Cost (KRW)",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            };
            krwColumn.Binding = new System.Windows.Data.Binding("Cost")
            {
                Converter = new CostToKRWConverter()
            };
            HistoryDataGrid.Columns.Add(krwColumn);

            HistoryDataGrid.ItemsSource = dailyStats;
        }

        private void LoadMonthlyBreakdown(DateTime? startDate = null, DateTime? endDate = null)
        {
            var monthlyStats = _usageService.GetMonthlyStatistics(startDate, endDate);

            // Set up columns for monthly breakdown
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Month",
                Binding = new System.Windows.Data.Binding("MonthName"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Operations",
                Binding = new System.Windows.Data.Binding("OperationCount"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Corrections",
                Binding = new System.Windows.Data.Binding("CorrectionCount"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Translations",
                Binding = new System.Windows.Data.Binding("TranslationCount"),
                Width = new DataGridLength(100)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Total Tokens",
                Binding = new System.Windows.Data.Binding("TotalTokens") { StringFormat = "{0:N0}" },
                Width = new DataGridLength(120)
            });
            HistoryDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Cost (USD)",
                Binding = new System.Windows.Data.Binding("Cost") { StringFormat = "${0:F6}" },
                Width = new DataGridLength(120)
            });
            
            // Add KRW column
            var krwColumn = new DataGridTextColumn
            {
                Header = "Cost (KRW)",
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            };
            krwColumn.Binding = new System.Windows.Data.Binding("Cost")
            {
                Converter = new CostToKRWConverter()
            };
            HistoryDataGrid.Columns.Add(krwColumn);

            HistoryDataGrid.ItemsSource = monthlyStats;
        }

        private void ViewModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Prevent execution during initialization before controls are ready
            if (TotalOperationsText == null)
                return;

            // Reload data with current filters
            GetCurrentDateRange(out DateTime? startDate, out DateTime? endDate);
            LoadDataGridContent(startDate, endDate);
        }

        private void GetCurrentDateRange(out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            if (PeriodComboBox?.SelectedItem is not ComboBoxItem selectedItem)
                return;

            var now = DateTime.Now;

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
        }

        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Prevent execution during initialization before controls are ready
            if (TotalOperationsText == null)
                return;

            GetCurrentDateRange(out DateTime? startDate, out DateTime? endDate);
            LoadStatistics(startDate, endDate);
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

    /// <summary>
    /// Converter for USD to KRW conversion
    /// </summary>
    public class CostToKRWConverter : System.Windows.Data.IValueConverter
    {
        private const decimal USD_TO_KRW_RATE = 1350m;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal cost)
            {
                var krw = cost * USD_TO_KRW_RATE;
                return $"₩{krw:N0}";
            }
            return "₩0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
