using System.Windows;
using System.Windows.Forms;
using DataDownloader.Common.Properties;

namespace DataDownloader.Ui
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            InitWindowWithSettings();
        }

        private void ButtonApplySettings_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void ButtonDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                AddExtension = true,
                Filter = "KeePass Files|*.kdbx",
                FileName = TextBoxDatabasePath.Text
            };

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxDatabasePath.Text = fileDialog.FileName;
            }
        }

        private void ButtonDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new FolderBrowserDialog { SelectedPath = TextBoxDownloadPath.Text };

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

        private void InitWindowWithSettings()
        {
            var settings = SettingsHandler.Instance;

            settings.Reload();

            TextBoxDownloadPath.Text = settings.DataDownloaderPath;

            CheckBoxDkbRunDefault.IsChecked = settings.DataDownloaderRunDefaultDkb;
            CheckBoxNumber26RunDefault.IsChecked = settings.DataDownloaderRunDefaultNumber26;
            CheckBoxRaiffeisenRunDefault.IsChecked = settings.DataDownloaderRunDefaultRaiffeisen;
            CheckBoxSantanderRunDefault.IsChecked = settings.DataDownloaderRunDefaultSantander;
            CheckBoxRciRunDefault.IsChecked = settings.DataDownloaderRunDefaultRci;

            TextBoxDkbSubfolder.Text = settings.DataDownloaderSubfolderDkb;
            TextBoxNumber26Subfolder.Text = settings.DataDownloaderSubfolderNumber26;
            TextBoxRaiffeisenSubfolder.Text = settings.DataDownloaderSubfolderRaiffeisen;
            TextBoxSantanderSubfolder.Text = settings.DataDownloaderSubfolderSantander;
            TextBoxRciSubfolder.Text = settings.DataDownloaderSubfolderRci;

            TextBoxDatabasePath.Text = settings.KeePassPath;

            TextBoxDkbUuid.Text = settings.KeePassEntryUuidDkb;
            TextBoxNumber26Uuid.Text = settings.KeePassEntryUuidNumber26;
            TextBoxRaiffeisenUuid.Text = settings.KeePassEntryUuidRaiffeisen;
            TextBoxSantanderUuid.Text = settings.KeePassEntryUuidSantander;
            TextBoxRciUuid.Text = settings.KeePassEntryUuidRci;

            TextBoxSantanderBirthday.Text = settings.KeePassFieldBirthdaySantander;
            TextBoxRaiffeisenPin.Text = settings.KeePassFieldPinRaiffeisen;

            ComboBoxLanguage.SelectedValue = settings.LanguageIso639_1;
        }

        private void SaveSettings()
        {
            var settings = SettingsHandler.Instance;
            settings.DataDownloaderPath = TextBoxDownloadPath.Text;

            settings.DataDownloaderRunDefaultDkb = CheckBoxDkbRunDefault.IsChecked ?? false;
            settings.DataDownloaderRunDefaultNumber26 = CheckBoxNumber26RunDefault.IsChecked ?? false;
            settings.DataDownloaderRunDefaultRaiffeisen = CheckBoxRaiffeisenRunDefault.IsChecked ?? false;
            settings.DataDownloaderRunDefaultSantander = CheckBoxSantanderRunDefault.IsChecked ?? false;
            settings.DataDownloaderRunDefaultRci = CheckBoxRciRunDefault.IsChecked ?? false;

            settings.DataDownloaderSubfolderDkb = TextBoxDkbSubfolder.Text;
            settings.DataDownloaderSubfolderNumber26 = TextBoxNumber26Subfolder.Text;
            settings.DataDownloaderSubfolderRaiffeisen = TextBoxRaiffeisenSubfolder.Text;
            settings.DataDownloaderSubfolderSantander = TextBoxSantanderSubfolder.Text;
            settings.DataDownloaderSubfolderRci = TextBoxRciSubfolder.Text;

            settings.KeePassPath = TextBoxDatabasePath.Text;

            settings.KeePassEntryUuidDkb = TextBoxDkbUuid.Text;
            settings.KeePassEntryUuidNumber26 = TextBoxNumber26Uuid.Text;
            settings.KeePassEntryUuidRaiffeisen = TextBoxRaiffeisenUuid.Text;
            settings.KeePassEntryUuidSantander = TextBoxSantanderUuid.Text;
            settings.KeePassEntryUuidRci = TextBoxRciUuid.Text;

            settings.KeePassFieldBirthdaySantander = TextBoxSantanderBirthday.Text;
            settings.KeePassFieldPinRaiffeisen = TextBoxRaiffeisenPin.Text;

            settings.LanguageIso639_1 = ComboBoxLanguage.SelectedValue.ToString();

            settings.Save();
        }
    }
}