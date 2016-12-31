using System;
using System.IO;
using System.Net.Mime;
using System.Security;
using DataDownloader.Common.Helper;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.Selenium;
using KeePass;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DataDownloader.Handler.BankDownloadHandler
{
    public abstract class BankDownloadHandlerBase : IDisposable
    {
        public readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string Url { get; }
        public string DownloadPath { get; }

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
            if (!Directory.Exists(DownloadPath))
            {
                Directory.CreateDirectory(DownloadPath);
            }

            KeePass = KeePassWrapper.OpenWithPassword(SettingHandler.Default.KeePassPath, KeePassMasterPassword);

            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", DownloadPath);
            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);

            Browser = new ChromeDriver(options);
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
                // ignored
            }
        }

        protected abstract void Login();
        protected abstract void Logout();
        protected abstract void NavigateHome();
        protected abstract void Download();
    }
}