using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using DataDownloader.Handler.BankDownloadHandler;

namespace DataDownloader.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int RunningHandler;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingWindow().ShowDialog();
        }

        private void ButtonStartDownload_Click(object sender, RoutedEventArgs e)
        {
            RunningHandler = 0;
            ProgressBar.IsIndeterminate = true;

            var password = PasswordBoxKeePassMasterPassword.SecurePassword;
            if (CheckBoxRaiffeisen.IsChecked.HasValue && CheckBoxRaiffeisen.IsChecked.Value)
            {
                RunningHandler++;
                Parallel.Invoke(() => RunRaiffeisen(password));
            }
            if (CheckBoxDkb.IsChecked.HasValue && CheckBoxDkb.IsChecked.Value)
            {
                RunningHandler++;
                Parallel.Invoke(() => RunDkb(password));
            }
            if (CheckBoxSantander.IsChecked.HasValue && CheckBoxSantander.IsChecked.Value)
            {
                RunningHandler++;
                Parallel.Invoke(() => RunSantander(password));
            }
            if (CheckBoxNumber26.IsChecked.HasValue && CheckBoxNumber26.IsChecked.Value)
            {
                RunningHandler++;
                Parallel.Invoke(() => RunNumber26(password));
            }
        }

        private void ReportProgress()
        {
            lock (ProgressBar)
            {
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Minimum = 0;
                ProgressBar.Maximum = RunningHandler;
                ProgressBar.Value++;
            }
        }

        private void RunRaiffeisen(SecureString password)
        {
            using (var handler = new RaiffeisenDownloadHandler(password))
            {
                try
                {
                    handler.DownloadAllData();
                    ReportProgress();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error occured while downloading data via {handler.GetType().Name}: {e.Message}",
                        $"Download error {handler.GetType().Name}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RunDkb(SecureString password)
        {
            using (var handler = new DkbDownloadHandler(password))
            {
                try
                {
                    handler.DownloadAllData();
                    ReportProgress();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error occured while downloading data via {handler.GetType().Name}: {e.Message}",
                        $"Download error {handler.GetType().Name}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RunSantander(SecureString password)
        {
            using (var handler = new SantanderDownloadHandler(password))
            {
                try
                {
                    handler.DownloadAllData();
                    ReportProgress();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error occured while downloading data via {handler.GetType().Name}: {e.Message}",
                        $"Download error {handler.GetType().Name}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RunNumber26(SecureString password)
        {
            using (var handler = new Number26DownloadHandler(password))
            {
                try
                {
                    handler.DownloadAllData();
                    ReportProgress();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error occured while downloading data via {handler.GetType().Name}: {e.Message}",
                        $"Download error {handler.GetType().Name}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
