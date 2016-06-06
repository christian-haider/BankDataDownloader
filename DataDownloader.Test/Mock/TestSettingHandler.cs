using DataDownloader.Common.Settings;

namespace DataDownloader.Test.Mock
{
    public class TestSettingHandler : ISettingHandler
    {
        public string KeePassPath { get; set; }

        public string KeePassEntryUuidRaiffeisen { get; set; }

        public string DataDownloaderPath { get; set; }

        public string DataDownloaderSubfolderRaiffeisen { get; set; }

        public string DataDownloaderSubfolderSantander { get; set; }

        public string KeePassEntryUuidSantander { get; set; }

        public string DataDownloaderSubfolderDkb { get; set; }

        public string KeePassEntryUuidDkb { get; set; }

        public string DataDownloaderSubfolderNumber26 { get; set; }

        public string KeePassEntryUuidNumber26 { get; set; }

        public string KeePassFieldPinRaiffeisen { get; set; }

        public string KeePassFieldBirthdaySantander { get; set; }

        public void Save()
        {
        }

        public void Reload()
        {
        }

        public string KeePassPassword { get; set; }
    }
}