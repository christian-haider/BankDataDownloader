namespace DataDownloader.Common.Properties
{
    public static class SettingsHandler
    {
        private static ISettings _instance;

        public static ISettings Instance => _instance ?? Settings.Default;

        public static void RegisterSettingHandler(ISettings settings)
        {
            _instance = settings;
        }
    }
}