namespace DataDownloader.Common.Properties
{
    public interface ISettings
    {
        string DataDownloaderPath { get; set; }

        bool DataDownloaderRunDefaultDkb { get; set; }
        bool DataDownloaderRunDefaultNumber26 { get; set; }
        bool DataDownloaderRunDefaultRaiffeisen { get; set; }
        bool DataDownloaderRunDefaultRci { get; set; }
        bool DataDownloaderRunDefaultSantander { get; set; }

        string DataDownloaderSubfolderDkb { get; set; }
        string DataDownloaderSubfolderNumber26 { get; set; }
        string DataDownloaderSubfolderRaiffeisen { get; set; }
        string DataDownloaderSubfolderRci { get; set; }
        string DataDownloaderSubfolderSantander { get; set; }

        string KeePassEntryUuidDkb { get; set; }
        string KeePassEntryUuidNumber26 { get; set; }
        string KeePassEntryUuidRaiffeisen { get; set; }
        string KeePassEntryUuidRci { get; set; }
        string KeePassEntryUuidSantander { get; set; }

        string KeePassFieldBirthdaySantander { get; set; }

        string KeePassFieldPinRaiffeisen { get; set; }

        string KeePassPath { get; set; }

        string LanguageIso639_1 { get; set; }

        void Reload();
        void Save();
    }
}