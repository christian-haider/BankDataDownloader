using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using DataDownloader.Common.Properties;
using DataDownloader.Common.Settings;
using KeePass;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace DataDownloader.Handler.BankDownloadHandler
{
    public class DkbDownloadHandler : BankDownloadHandlerBase
    {
        public DkbDownloadHandler(string password) : base(password, "https://www.dkb.de/banking", Path.Combine(SettingHandler.Default.DataDownloaderPath,
                SettingHandler.Default.DataDownloaderSubfolderDkb))
        {
        }

        public DkbDownloadHandler(SecureString password) : base(password, "https://www.dkb.de/banking", Path.Combine(SettingHandler.Default.DataDownloaderPath,
                SettingHandler.Default.DataDownloaderSubfolderDkb))
        {
        }

        protected override void Login()
        {
            Browser.FindElement(By.Id("loginInputSelector")).SendKeys(KeePass.GetEntryByUuid(SettingHandler.Default.KeePassEntryUuidDkb).GetUserName());
            Browser.FindElement(By.Id("pinInputSelector")).SendKeys(KeePass.GetEntryByUuid(SettingHandler.Default.KeePassEntryUuidDkb).GetPassword());

            Browser.FindElement(By.Id("login")).Submit();
        }

        protected override void Logout()
        {
            Browser.FindElement(By.Id("logout")).Click();
        }

        protected override void NavigateHome()
        {
            Browser.FindElement(By.ClassName("dkb_logo_container")).Click();
        }

        protected override void Download()
        {
            DownloadTransactions();

            DownloadPdfs();
        }

        private void DownloadTransactions()
        {
            //bankaccount
            NavigateHome();
            GetAccountTransactions()[0].Click();
            SetMaxDateRange("[id*=transactionDate]", "[id*=toTransactionDate]");
            FileDownloader.DownloadFile(Browser.FindElement(By.ClassName("evt-csvExport")), fileOtherPrefix: "Giro");

            //credit card
            NavigateHome();
            GetAccountTransactions()[1].Click();
            SetMaxDateRange("[id*=postingDate]", "[id*=toPostingDate]");
            FileDownloader.DownloadFile(Browser.FindElement(By.ClassName("evt-csvExport")), fileOtherPrefix: "Visa");
        }

        private List<IWebElement> GetAccountTransactions()
        {
            return Browser.FindElements(new ByChained(By.ClassName("financialStatusTable"), By.ClassName("mainRow"), By.ClassName("evt-paymentTransaction"))).ToList();
        }

        private void SetMaxDateRange(string cssSelectorFromDate, string cssSelectorToDate)
        {
            var dateFormatString = "dd.MM.yyyy";

            var startRange = Browser.FindElement(new ByAll(By.TagName("input"), By.CssSelector(cssSelectorFromDate)));
            startRange.Clear();
            startRange.SendKeys(DateTime.Today.AddYears(-5).ToString(dateFormatString));

            var endRange = Browser.FindElement(new ByAll(By.TagName("input"), By.CssSelector(cssSelectorToDate)));
            endRange.Clear();
            endRange.SendKeys(DateTime.Today.ToString(dateFormatString));

            Browser.FindElement(By.Id("searchbutton")).Click();
            //Date has been adapted now click again
            Browser.FindElement(By.Id("searchbutton")).Click();
        }

        private void DownloadPdfs()
        {
            NavigateHome();

            GetPostboxMenuItem().Click();
            for (int i = 0; i < GetSubPostboxMenuItems().Count; i++)
            {
                GetSubPostboxMenuItems()[i].Click();

                foreach (var fileLink in Browser.FindElements(By.ClassName("iconSpeichern0")))
                {
                    FileDownloader.DownloadFile(fileLink, fileDatePrefix: false);
                }
            }
        }

        private IWebElement GetPostboxMenuItem()
        {
            return Browser.FindElement(new ByChained(By.Id("menu"), By.LinkText("Postfach")));
        }

        private List<IWebElement> GetSubPostboxMenuItems()
        {
            return GetPostboxMenuItem().FindElements(By.XPath("..//ul//a")).ToList();
        }
    }
}