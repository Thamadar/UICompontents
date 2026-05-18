using Client.WPF.Views;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Client.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {  
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); 

            var mainWindow = await GetMainWindow();
            mainWindow.Closed += OnMainWindowClosed;

            try
            {
                mainWindow.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии главного окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            mainWindow.Activate();
        }

        /// <summary>
        /// Get main window view.
        /// </summary> 
        private async Task<MainWindowView> GetMainWindow()
        {
            var mainWindow = new MainWindowView();
            var viewModel  = new MainWindowViewModel(); 

            mainWindow.DataContext = viewModel;

            return mainWindow;
        }

        /// <summary>
        /// Ons the main window closed.
        /// </summary> 
        private void OnMainWindowClosed(object? sender, System.EventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    } 
}
