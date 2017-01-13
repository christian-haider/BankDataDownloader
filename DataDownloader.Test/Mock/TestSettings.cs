using DataDownloader.Common.Properties;

namespace DataDownloader.Test.Mock
{
    public class TestSettings : ISettings
    {
        public string DataDownloaderPath { get; set; }

        public bool DataDownloaderRunDefaultDkb { get; set; }
        public bool DataDownloaderRunDefaultNumber26 { get; set; }
        public bool DataDownloaderRunDefaultRaiffeisen { get; set; }
        public bool DataDownloaderRunDefaultRci { get; set; }
        public bool DataDownloaderRunDefaultSantander { get; set; }

        public string DataDownloaderSubfolderDkb { get; set; }
        public string DataDownloaderSubfolderNumber26 { get; set; }
        public string DataDownloaderSubfolderRaiffeisen { get; set; }
        public string DataDownloaderSubfolderRci { get; set; }
        public string DataDownloaderSubfolderSantander { get; set; }

        public string KeePassEntryUuidDkb { get; set; }
        public string KeePassEntryUuidNumber26 { get; set; }
        public string KeePassEntryUuidRaiffeisen { get; set; }
        public string KeePassEntryUuidRci { get; set; }
        public string KeePassEntryUuidSantander { get; set; }

        public string KeePassFieldBirthdaySantander { get; set; }

        public string KeePassFieldPinRaiffeisen { get; set; }

        public string KeePassPassword { get; set; }

        public string KeePassPath { get; set; }

        public string LanguageIso639_1 { get; set; }

        public void Reload()
        {

        }

        public void Save()
        {

        }
    }
}