namespace DataDownloader.Common.Settings
{
    public interface ISettingHandler
    {
        string KeePassPath { get; set; }
        string KeePassEntryUuidRaiffeisen { get; set; }
        string DataDownloaderPath { get; set; }
        string DataDownloaderSubfolderRaiffeisen { get; set; }
        string DataDownloaderSubfolderSantander { get; set; }
        string KeePassEntryUuidSantander { get; set; }
        string DataDownloaderSubfolderDkb { get; set; }
        string KeePassEntryUuidDkb { get; set; }
        string DataDownloaderSubfolderNumber26 { get; set; }
        string KeePassEntryUuidNumber26 { get; set; }
        string KeePassFieldPinRaiffeisen { get; set; }
        string KeePassFieldBirthdaySantander { get; set; }
        void Save();
        void Reload();
    }
}