using System;
using System.IO;
using System.Security;
using DataDownloader.Common.Helper;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.Properties;
using DataDownloader.Handler.Selenium;
using KeePass;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DataDownloader.Handler.BankDownloadHandler
{
    public abstract class BankDownloadHandlerBase : IDisposable
    {
        protected string Url;
        protected string DownloadPath;

        protected IWebDriver Browser;
        protected KeePassWrapper KeePass;
        protected SeleniumFileDownloader FileDownloader;

        private readonly string _keePassMasterPasswordString;
        private readonly SecureString _keePassMasterPasswordSecureString;

        private string KeePassMasterPassword => string.IsNullOrEmpty(_keePassMasterPasswordString)
            ? _keePassMasterPasswordSecureString.ConvertToUnsecureString()
            : _keePassMasterPasswordString;


        internal BankDownloadHandlerBase(string masterPassword, string url, string downloadPath)
        {
            Url = url;
            DownloadPath = downloadPath;

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            _keePassMasterPasswordSecureString = null;
            _keePassMasterPasswordString = masterPassword;
        }

        internal BankDownloadHandlerBase(SecureString masterPassword, string url, string downloadPath)
        {
            Url = url;
            DownloadPath = downloadPath;

            _keePassMasterPasswordSecureString = masterPassword;
            _keePassMasterPasswordString = null;
        }

        private void Initialize()
        {
            KeePass = KeePassWrapper.OpenWithPassword(SettingHandler.Default.KeePassPath, KeePassMasterPassword);

            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", DownloadPath);
            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);

            //extract chromedriver from resources to 
            var driverDirPath = Path.Combine(Path.GetTempPath(), "Selenium");
            var driverPath = Path.Combine(driverDirPath, "chromedriver.exe");
            if (!File.Exists(driverPath))
            {
                if (!Directory.Exists(driverDirPath))
                {
                    Directory.CreateDirectory(driverDirPath);
                }
                File.WriteAllBytes(driverPath, Resources.chromedriver);
            }

            Browser = new ChromeDriver(driverDirPath, options);
            Browser.Manage().Window.Maximize();
            Browser.Navigate().GoToUrl(Url);

            FileDownloader = new SeleniumFileDownloader(Browser, DownloadPath);

            Login();
        }

        public void DownloadAllData()
        {
            Initialize();

            Download();
        }

        public void Dispose()
        {
            try
            {
                Logout();

                Browser.Dispose();
            }
            catch (Exception)
            {

            }
        }

        protected abstract void Login();
        protected abstract void Logout();
        protected abstract void NavigateHome();
        protected abstract void Download();
    }
}