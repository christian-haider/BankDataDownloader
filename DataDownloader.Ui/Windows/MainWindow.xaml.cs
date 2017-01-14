using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataDownloader.Common.Properties;
using DataDownloader.Common.Resources;
using DataDownloader.Handler.BankDownloadHandler;
using KeePass;
using NLog;

namespace DataDownloader.Ui.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly Logger Log = LogManager.GetCurrentClassLogger();

        public int RunningHandler;

        public MainWindow()
        {
            InitializeComponent();
            InitWindowWithSettings();
        }

        private void ButtonStartDownload_Click(object sender, RoutedEventArgs e)
        {
            Run();
        }

        private void InitWindowWithSettings()
        {
            CheckBoxRci.IsChecked = SettingsHandler.Instance.DataDownloaderRunDefaultRci;
            CheckBoxDkb.IsChecked = SettingsHandler.Instance.DataDownloaderRunDefaultDkb;
            CheckBoxNumber26.IsChecked = SettingsHandler.Instance.DataDownloaderRunDefaultNumber26;
            CheckBoxRaiffeisen.IsChecked = SettingsHandler.Instance.DataDownloaderRunDefaultRaiffeisen;
            CheckBoxSantander.IsChecked = SettingsHandler.Instance.DataDownloaderRunDefaultSantander;
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
            InitWindowWithSettings();
        }

        private void PasswordBoxKeePassMasterPassword_KeyUp(object sender, KeyEventArgs e)
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
            var errorOccured = false;
            try
            {
                var downloadPath = SettingsHandler.Instance.DataDownloaderPath;
                if (!Directory.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Messages.DesinationNotAccessible_Text, SettingsHandler.Instance.DataDownloaderPath), Messages.DesinationNotAccessible_Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Log.Warn(ex, "Download path seems wrong when accessing \"{0}\"", SettingsHandler.Instance.DataDownloaderPath);
                errorOccured = true;
            }
            try
            {
                using (KeePassWrapper.OpenWithPassword(SettingsHandler.Instance.KeePassPath, password))
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Messages.KeePassError_Text, SettingsHandler.Instance.KeePassPath), Messages.KeePassError_Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Log.Warn(ex, "KeePass password or path seems wrong when accessing \"{0}\"", SettingsHandler.Instance.KeePassPath);
                errorOccured = true;
            }
            if (errorOccured)
            {
                return;
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
            if (CheckBoxRci.IsChecked.HasValue && CheckBoxRci.IsChecked.Value)
            {
                RunningHandler++;
                RunBankDownloadHanlder(new RciDownloadHandler(password));
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
                    Log.Info("Start downloading data from {0}", downloadHandler.Url);
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
                var aggregateException = t.Exception;
                aggregateException?.Handle(exception => true);
                var msg = string.Format(Messages.DownloadHandlerError_Text,
                    downloadHandler.GetType().Name);

                Log.Error(t.Exception, "Error occured while downloading data via {0}\n{1}",
                    downloadHandler.GetType().Name,
                    aggregateException?.InnerExceptions.Select(exception => exception.Message)
                        .Aggregate("", (head, current) => $"{head}\n{current}"));
                MessageBox.Show(msg, string.Format(Messages.DownloadHandlerError_Title, downloadHandler.GetType().Name), MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Update UI (and UI-related data) here: failed status.
                ReportProgress(true);
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, context);

            task.Start();
        }

        #endregion

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void MenuItemLogs_Click(object sender, RoutedEventArgs e)
        {
            var logPath = NLog.Layouts.SimpleLayout.Evaluate(LogManager.Configuration.Variables["logPath"].Text, null);
            Process.Start(new ProcessStartInfo(logPath));
        }
    }
}