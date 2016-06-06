namespace DataDownloader.Common.Settings
{
    public static class SettingHandler
    {
        private static ISettingHandler _instance;

        public static ISettingHandler Default => _instance ?? (_instance = new SettingHandlerImpl());

        public static void MockSettingHandler(ISettingHandler settingHandler)
        {
            _instance = settingHandler;
        }
    }
}