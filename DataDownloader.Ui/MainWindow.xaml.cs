using System;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.BankDownloadHandler;
using KeePass;

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
            Run();
        }

        private void PasswordBoxKeePassMasterPassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Run();
            }
        }

        #region helper

        private void Run()
        {
            var password = PasswordBoxKeePassMasterPassword.SecurePassword;
            try
            {
                using (KeePassWrapper.OpenWithPassword(SettingHandler.Default.KeePassPath, password))
                {
                }

                ResetProgress();
                if (CheckBoxRaiffeisen.IsChecked.HasValue && CheckBoxRaiffeisen.IsChecked.Value)
                {
                    RunningHandler++;
                    RunBankDownloadHanlder(new RaiffeisenDownloadHandler(password));
                }
                if (CheckBoxDkb.IsChecked.HasValue && CheckBoxDkb.IsChecked.Value)
                {
                    RunningHandler++;
                    RunBankDownloadHanlder(new DkbDownloadHandler(password));
                }
                if (CheckBoxSantander.IsChecked.HasValue && CheckBoxSantander.IsChecked.Value)
                {
                    RunningHandler++;
                    RunBankDownloadHanlder(new SantanderDownloadHandler(password));
                }
                if (CheckBoxNumber26.IsChecked.HasValue && CheckBoxNumber26.IsChecked.Value)
                {
                    RunningHandler++;
                    RunBankDownloadHanlder(new Number26DownloadHandler(password));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Check password:\n{ex.Message}", "KeePass error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetProgress()
        {
            RunningHandler = 0;
            ProgressBar.IsIndeterminate = true;
        }

        private void ReportProgress(bool isError = false)
        {
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = RunningHandler;
            ProgressBar.Value++;
        }

        private void RunBankDownloadHanlder(BankDownloadHandlerBase downloadHandler)
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();

            var task = new Task(() =>
            {
                using (downloadHandler)
                {
                    downloadHandler.DownloadAllData();
                }
            });
            task.ContinueWith(t =>
            {
                // Update UI (and UI-related data) here: success status.
                ReportProgress();
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, context);
            task.ContinueWith(t =>
            {
                AggregateException aggregateException = t.Exception;
                aggregateException?.Handle(exception => true);
                var msg =
                    $"Error occured while downloading data via {downloadHandler.GetType().Name}: {aggregateException?.InnerExceptions.Select(exception => exception.Message).Aggregate("", (head, current) => $"{head}\n{current}")}";

                MessageBox.Show(msg, $"Download error {downloadHandler.GetType().Name}", MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Update UI (and UI-related data) here: failed status.
                ReportProgress(true);
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, context);

            task.Start();
        }
        #endregion
    }
}
