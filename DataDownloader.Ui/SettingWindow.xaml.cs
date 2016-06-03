using System.Windows;
using System.Windows.Forms;
using DataDownloader.Common.Properties;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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
            var settings = Settings.Default;

            settings.DataDownloader_Path = TextBoxDownloadPath.Text;

            settings.DataDownloader_Subfolder_Dkb = TextBoxDkbSubfolder.Text;
            settings.DataDownloader_Subfolder_Number26 = TextBoxNumber26Subfolder.Text;
            settings.DataDownloader_Subfolder_Raiffeisen = TextBoxRaiffeisenSubfolder.Text;
            settings.DataDownloader_Subfolder_Santander = TextBoxSantanderSubfolder.Text;

            settings.KeePass_Path = TextBoxDatabasePath.Text;

            settings.KeePass_Entry_Uuid_Dkb = TextBoxDkbUuid.Text;
            settings.KeePass_Entry_Uuid_Number26 = TextBoxNumber26Uuid.Text;
            settings.KeePass_Entry_Uuid_Raiffeisen = TextBoxRaiffeisenUuid.Text;
            settings.KeePass_Entry_Uuid_Santander = TextBoxSantanderUuid.Text;

            settings.KeePass_Field_Birthday_Santander = TextBoxSantanderBirtday.Text;
            settings.KeePass_Field_Pin_Raiffeisen = TextBoxRaiffeisenPin.Text;

            settings.Save();
            }

        private void InitDialogWithSettings()
        {
            var settings = Settings.Default;

            settings.Reload();

            TextBoxDownloadPath.Text = settings.DataDownloader_Path;

            TextBoxDkbSubfolder.Text = settings.DataDownloader_Subfolder_Dkb;
            TextBoxNumber26Subfolder.Text = settings.DataDownloader_Subfolder_Number26;
            TextBoxRaiffeisenSubfolder.Text = settings.DataDownloader_Subfolder_Raiffeisen;
            TextBoxSantanderSubfolder.Text = settings.DataDownloader_Subfolder_Santander;

            TextBoxDatabasePath.Text = settings.KeePass_Path;

            TextBoxDkbUuid.Text = settings.KeePass_Entry_Uuid_Dkb;
            TextBoxNumber26Uuid.Text = settings.KeePass_Entry_Uuid_Number26;
            TextBoxRaiffeisenUuid.Text = settings.KeePass_Entry_Uuid_Raiffeisen;
            TextBoxSantanderUuid.Text = settings.KeePass_Entry_Uuid_Santander;

            TextBoxSantanderBirtday.Text = settings.KeePass_Field_Birthday_Santander;
            TextBoxRaiffeisenPin.Text = settings.KeePass_Field_Pin_Raiffeisen;
        }
    }
}
