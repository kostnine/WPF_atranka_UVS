using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ThreadGeneratorApp
{
    public partial class MainWindow : Window
    {
        private ThreadManager _threadManager;
        private ObservableCollection<ThreadResult> _results;

        public MainWindow()
        {
            InitializeComponent();
            _results = new ObservableCollection<ThreadResult>();
            ResultsListView.ItemsSource = _results;
            _threadManager = new ThreadManager();
            _threadManager.DataGenerated += OnDataGenerated;
        }

        private void OnDataGenerated(object sender, ThreadResult result)
        {
            Dispatcher.Invoke(() =>
            {
                _results.Insert(0, result);
                
                // Keep only last 20 results
                while (_results.Count > 20)
                {
                    _results.RemoveAt(_results.Count - 1);
                }
            });
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ThreadCountTextBox.Text, out int threadCount) || 
                threadCount < 2 || threadCount > 15)
            {
                MessageBox.Show("Thread kiekis turi būti skaičius nuo 2 iki 15", "Klaida", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            ThreadCountTextBox.IsEnabled = false;
            StatusTextBlock.Text = $"Veikia {threadCount} thread'ai...";

            try
            {
                await _threadManager.StartAsync(threadCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Klaida: {ex.Message}", "Klaida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _threadManager.Stop();
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ThreadCountTextBox.IsEnabled = true;
            StatusTextBlock.Text = "Sustabdyta";
        }

        protected override void OnClosed(EventArgs e)
        {
            _threadManager?.Stop();
            base.OnClosed(e);
        }
    }

    public class ThreadResult
    {
        public int ThreadId { get; set; }
        public string GeneratedString { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
