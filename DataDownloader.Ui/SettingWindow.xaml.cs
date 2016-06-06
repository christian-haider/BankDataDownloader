using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DataDownloader.Common.Properties;
using DataDownloader.Common.Settings;

namespace DataDownloader.Ui
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            InitDialogWithSettings();
        }

        private void ButtonDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.CheckFileExists = true;
            fileDialog.AddExtension = true;
            fileDialog.Filter = "KeePass Files|*.kdbx";
            fileDialog.FileName = TextBoxDatabasePath.Text;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxDatabasePath.Text = fileDialog.FileName;
            }
        }

        private void ButtonDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new FolderBrowserDialog();
            fileDialog.SelectedPath = TextBoxDownloadPath.Text;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxDownloadPath.Text = fileDialog.SelectedPath;
            }
        }

        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void ButtonApplySettings_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            Task.Run(() =>
            {
                var settings = SettingHandler.Default;
                settings.DataDownloaderPath = TextBoxDownloadPath.Text;

                settings.DataDownloaderSubfolderDkb = TextBoxDkbSubfolder.Text;
                settings.DataDownloaderSubfolderNumber26 = TextBoxNumber26Subfolder.Text;
                settings.DataDownloaderSubfolderRaiffeisen = TextBoxRaiffeisenSubfolder.Text;
                settings.DataDownloaderSubfolderSantander = TextBoxSantanderSubfolder.Text;

                settings.KeePassPath = TextBoxDatabasePath.Text;

                settings.KeePassEntryUuidDkb = TextBoxDkbUuid.Text;
                settings.KeePassEntryUuidNumber26 = TextBoxNumber26Uuid.Text;
                settings.KeePassEntryUuidRaiffeisen = TextBoxRaiffeisenUuid.Text;
                settings.KeePassEntryUuidSantander = TextBoxSantanderUuid.Text;

                settings.KeePassFieldBirthdaySantander = TextBoxSantanderBirtday.Text;
                settings.KeePassFieldPinRaiffeisen = TextBoxRaiffeisenPin.Text;

                settings.Save();
            });
        }

        private void InitDialogWithSettings()
        {
            var settings = SettingHandler.Default;

            settings.Reload();

            TextBoxDownloadPath.Text = settings.DataDownloaderPath;

            TextBoxDkbSubfolder.Text = settings.DataDownloaderSubfolderDkb;
            TextBoxNumber26Subfolder.Text = settings.DataDownloaderSubfolderNumber26;
            TextBoxRaiffeisenSubfolder.Text = settings.DataDownloaderSubfolderRaiffeisen;
            TextBoxSantanderSubfolder.Text = settings.DataDownloaderSubfolderSantander;

            TextBoxDatabasePath.Text = settings.KeePassPath;

            TextBoxDkbUuid.Text = settings.KeePassEntryUuidDkb;
            TextBoxNumber26Uuid.Text = settings.KeePassEntryUuidNumber26;
            TextBoxRaiffeisenUuid.Text = settings.KeePassEntryUuidRaiffeisen;
            TextBoxSantanderUuid.Text = settings.KeePassEntryUuidSantander;

            TextBoxSantanderBirtday.Text = settings.KeePassFieldBirthdaySantander;
            TextBoxRaiffeisenPin.Text = settings.KeePassFieldPinRaiffeisen;
        }
    }
}
