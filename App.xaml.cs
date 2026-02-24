using System;
using System.Threading.Tasks;
using System.Windows;

namespace ThreadGeneratorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();

            _ = Task.Run(async () =>
            {
                try
                {
                    var dbService = new DatabaseService();
                    await dbService.InitializeDatabaseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database initialization failed: {ex.Message}");
                }
            });
        }
    }
}
