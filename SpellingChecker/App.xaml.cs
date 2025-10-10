using System;
using System.Windows;

namespace SpellingChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Create and show main window (hidden, runs in background)
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
        }
    }
}
