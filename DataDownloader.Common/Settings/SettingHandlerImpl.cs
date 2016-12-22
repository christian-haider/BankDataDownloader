namespace DataDownloader.Common.Settings
{
    public class SettingHandlerImpl : ISettingHandler
    {
        private readonly Properties.Settings _baseSettingHandler = Properties.Settings.Default;

        internal SettingHandlerImpl() : base()
        {
        }

        public string KeePassPath
        {
            get { return _baseSettingHandler.KeePass_Path; }
            set { _baseSettingHandler.KeePass_Path = value; }
        }

        public string KeePassEntryUuidRaiffeisen
        {
            get { return _baseSettingHandler.KeePass_Entry_Uuid_Raiffeisen; }
            set { _baseSettingHandler.KeePass_Entry_Uuid_Raiffeisen = value; }
        }

        public string DataDownloaderPath
        {
            get { return _baseSettingHandler.DataDownloader_Path; }
            set { _baseSettingHandler.DataDownloader_Path = value; }
        }

        public string DataDownloaderSubfolderRaiffeisen
        {
            get { return _baseSettingHandler.DataDownloader_Subfolder_Raiffeisen; }
            set { _baseSettingHandler.DataDownloader_Subfolder_Raiffeisen = value; }
        }

        public string DataDownloaderSubfolderSantander
        {
            get { return _baseSettingHandler.DataDownloader_Subfolder_Santander; }
            set { _baseSettingHandler.DataDownloader_Subfolder_Santander = value; }
        }

        public string KeePassEntryUuidSantander
        {
            get { return _baseSettingHandler.KeePass_Entry_Uuid_Santander; }
            set { _baseSettingHandler.KeePass_Entry_Uuid_Santander = value; }
        }

        public string DataDownloaderSubfolderDkb
        {
            get { return _baseSettingHandler.DataDownloader_Subfolder_Dkb; }
            set { _baseSettingHandler.DataDownloader_Subfolder_Dkb = value; }
        }

        public string KeePassEntryUuidDkb
        {
            get { return _baseSettingHandler.KeePass_Entry_Uuid_Dkb; }
            set { _baseSettingHandler.KeePass_Entry_Uuid_Dkb = value; }
        }

        public string DataDownloaderSubfolderNumber26
        {
            get { return _baseSettingHandler.DataDownloader_Subfolder_Number26; }
            set { _baseSettingHandler.DataDownloader_Subfolder_Number26 = value; }
        }

        public string KeePassEntryUuidRci
        {
            get { return _baseSettingHandler.KeePass_Entry_Uuid_Rci; }
            set { _baseSettingHandler.KeePass_Entry_Uuid_Rci = value; }
        }
        public string DataDownloaderSubfolderRci
        {
            get { return _baseSettingHandler.DataDownloader_Subfolder_Rci; }
            set { _baseSettingHandler.DataDownloader_Subfolder_Rci = value; }
        }

        public string KeePassEntryUuidNumber26
        {
            get { return _baseSettingHandler.KeePass_Entry_Uuid_Number26; }
            set { _baseSettingHandler.KeePass_Entry_Uuid_Number26 = value; }
        }

        public string KeePassFieldPinRaiffeisen
        {
            get { return _baseSettingHandler.KeePass_Field_Pin_Raiffeisen; }
            set { _baseSettingHandler.KeePass_Field_Pin_Raiffeisen = value; }
        }

        public string KeePassFieldBirthdaySantander
        {
            get { return _baseSettingHandler.KeePass_Field_Birthday_Santander; }
            set { _baseSettingHandler.KeePass_Field_Birthday_Santander = value; }
        }

        public void Save()
        {
            _baseSettingHandler.Save();
        }

        public void Reload()
        {
            _baseSettingHandler.Reload();
        }
    }
}