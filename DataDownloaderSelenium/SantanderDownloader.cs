using System.IO;
using DataDownloader.Properties;
using DataDownloader.Selenium;
using KeePass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace DataDownloader
{
    [TestClass]
    public class SantanderDownloader : BankDownloaderBase
    {
        static SantanderDownloader()
        {
            Url = "https://service.santanderconsumer.at/eva/";
        }
        protected override void Login()
        {
            var entry = KeePass.GetEntryByUuid(Settings.Default.KeePass_Entry_Uuid_Santander);

            Browser.FindElement(new ByChained(By.Id("eserviceLogin"), new ByIdOrName("disposerId"))).SendKeys(entry.GetUserName());
            Browser.FindElement(new ByChained(By.Id("eserviceLogin"), new ByIdOrName("birthdate"))).SendKeys(entry.GetString("birthday"));
            Browser.FindElement(new ByChained(By.Id("eserviceLogin"), new ByIdOrName("password"))).SendKeys(entry.GetPassword());

            Browser.FindElement(new ByChained(By.Id("eserviceLogin"), new ByIdOrName("submitButton"))).Click();
        }

        protected override void Logout()
        {
            Browser.FindElement(new ByChained(By.Id("login"), By.TagName("a"))).Click();
        }

        protected void NavigateHome()
        {
            Browser.FindElement(new ByChained(By.Id("header"), By.TagName("div"), By.TagName("a"))).Click();
        }

        protected void SetMaxDateRange()
        {
            var month = new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_month"))));
            month.SelectByIndex(0);
            var year = new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_year"))));
            year.SelectByIndex(0);
            Browser.FindElement(new ByChained(By.Id("showBookings"), By.TagName("table"), By.TagName("tbody"),
                By.TagName("tr"), By.XPath("td[5]"), By.TagName("input"))).Click();
        }

        protected void DownloadTransactions(string filePrefix = null)
        {
            Browser.FindElement(By.XPath("//*[@id=\"collapseTwo\"]/table/tbody/tr/td[7]/a")).Click();
            SetMaxDateRange();
            Browser.FindElement(By.XPath("//*[@id=\"showPrint\"]/input")).Click();

            var downloader = new SeleniumFileDownloader(Browser, Path.Combine(Settings.Default.DataDownloader_Path, Settings.Default.DataDownloader_Subfolder_Santander));
            downloader.DownloadCurrentPageSource("account.html",fileOtherPrefix:filePrefix);

            Browser.FindElement(By.XPath("//*[@id=\"printBookings\"]/input")).Click();
        }

        protected void DownloadPdfs(string filePrefix = null)
        {
            

            var downloader = new SeleniumFileDownloader(Browser, Path.Combine(Settings.Default.DataDownloader_Path, Settings.Default.DataDownloader_Subfolder_Santander));
            
            var account = GetAccountSelect();
            account.SelectByIndex(0);

            var year = GetYearSelect();
            for (int i = 0; i < year.Options.Count; i++)
            {
                var month = GetMonthSelect();
                for (int j = 0; j < month.Options.Count; j++)
                {
                    
                    GetAccountSelect().SelectByIndex(0);
                    GetYearSelect().SelectByIndex(i);
                    GetMonthSelect().SelectByIndex(j);
                    //TODO downloader.Download()
                }
            }
        }

        private SelectElement GetAccountSelect()
        {
            return new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_month"))));
        }

        private SelectElement GetYearSelect()
        {
            return new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_month"))));
        }

        private SelectElement GetMonthSelect()
        {
            return new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_month"))));
        }

        [TestMethod]
        public void DownloadMaxDateRangeCsvForAllAccounts()
        {
            Browser.FindElement(By.XPath("//*[@id=\"collapseTwo\"]/table/tbody/tr/td[6]/a")).Click();
            var accountNumber = Browser.FindElement(By.XPath("//*[@id=\"accordion2\"]/div/table[1]/tbody/tr[2]/td[2]")).Text;

            //NavigateHome();
            //DownloadTransactions(accountNumber);

            NavigateHome();
            DownloadPdfs(accountNumber);
        }
    }
}