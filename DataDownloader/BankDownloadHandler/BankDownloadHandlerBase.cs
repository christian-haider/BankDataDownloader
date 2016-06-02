using System;
using DataDownloader.Properties;
using DataDownloader.Selenium;
using KeePass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DataDownloader.BankDownloadHandler
{
    public abstract class BankDownloadHandlerBase
    {
        protected IWebDriver Browser;
        protected KeePassWrapper KeePass;
        protected SeleniumFileDownloader FileDownloader;
        protected string Url;
        protected string DownloadPath;

        internal BankDownloadHandlerBase(string url, string downloadPath)
        {
            Url = url;
            DownloadPath = downloadPath;
            FileDownloader = new SeleniumFileDownloader(Browser, downloadPath);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            KeePass = KeePassWrapper.OpenWithPassword(Settings.Default.KeePass_Path, Settings.Default.KeePass_MasterPassword);

            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", DownloadPath);
            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);

            Browser = new ChromeDriver(@"webdriver", options);

            Browser.Manage().Window.Maximize();
            Browser.Navigate().GoToUrl(Url);
            Login();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Logout();

            Browser.Dispose();
        }

        protected abstract void Login();
        protected abstract void Logout();
        protected abstract void NavigateHome();

        public abstract void DownloadAllData();
    }
}