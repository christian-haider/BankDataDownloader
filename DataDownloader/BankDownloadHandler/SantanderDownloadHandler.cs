﻿using System.IO;
using System.Threading;
using DataDownloader.Properties;
using DataDownloader.Selenium;
using KeePass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace DataDownloader.BankDownloadHandler
{
    [TestClass]
    public class SantanderDownloadHandler : BankDownloadHandlerBase
    {
        public SantanderDownloadHandler() : base("https://service.santanderconsumer.at/eva/")
        {
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

        protected override void NavigateHome()
        {
            Browser.FindElement(new ByChained(By.Id("header"), By.TagName("div"), By.TagName("a"))).Click();
        }

        private void DownloadTransactions(string filePrefix = null)
        {
            Browser.FindElement(new ByChained(By.Id("collapseTwo"), By.LinkText("Buchungen"))).Click();
            SetMaxDateRange();
            Browser.FindElement(By.Id("showPrint")).Submit();

            var downloader = new SeleniumFileDownloader(Browser, Path.Combine(Settings.Default.DataDownloader_Path, Settings.Default.DataDownloader_Subfolder_Santander));
            downloader.DownloadCurrentPageSource("account.html", fileOtherPrefix: filePrefix);

            Browser.FindElement(By.Id("printBookings")).Submit();
        }

        private void SetMaxDateRange()
        {
            var month = new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_month"))));
            month.SelectByIndex(0);
            var year = new SelectElement(Browser.FindElement(new ByChained(By.Id("showBookings"), new ByIdOrName("dateFrom_year"))));
            year.SelectByIndex(0);
            Browser.FindElement(new ByChained(By.Id("showBookings"), By.TagName("table"), By.TagName("tbody"),
                By.TagName("tr"), By.XPath("td[5]"), By.TagName("input"))).Click();
        }

        private void DownloadPdfs(string filePrefix = null)
        {
            //Go to Nachrichten
            Browser.FindElement(By.XPath("//*[@id=\"main-menu\"]/li[2]/a")).Click();
            //Click on Messages until found correct message
            var foundLink = false;
            for (int i = 1; !foundLink; i++)
            {
                var selector = $"//*[@id=\"collapseTwo\"]/table/tbody/tr[{i}]/td[3]/a";
                Browser.FindElement(By.XPath(selector)).Click();
                try
                {
                    Browser.FindElement(By.LinkText("Kontauszug BestFlex")).Click();
                    foundLink = true;
                }
                catch (NoSuchElementException)
                {
                }
            }

            var downloader = new SeleniumFileDownloader(Browser, Path.Combine(Settings.Default.DataDownloader_Path, Settings.Default.DataDownloader_Subfolder_Santander));

            GetAccountSelect().SelectByIndex(1);
            //Start with idx 1 as first entry is empty
            for (int i = 1; i < GetYearSelect().Options.Count; i++)
            {
                GetYearSelect().SelectByIndex(i);
                //Start with idx 1 as first entry is empty
                for (int j = 1; j < GetMonthSelect().Options.Count; j++)
                {
                    GetAccountSelect().SelectByIndex(1);
                    //little waiting time to make JS execute
                    Thread.Sleep(50);
                    GetYearSelect().SelectByIndex(i);
                    Thread.Sleep(50);
                    GetMonthSelect().SelectByIndex(j);

                    //download button
                    Browser.FindElement(By.Id("eServiceForm")).Submit();
                    var downloadLink = Browser.FindElement(By.LinkText("Kontoauszug downloaden"));
                    downloader.DownloadFile(downloadLink, fileDatePrefix: false, fileOtherPrefix: filePrefix);

                    //return to form
                    Browser.FindElement(By.XPath("//*[@id=\"esNext\"]/input")).Click();
                    GetAccountSelect().SelectByIndex(1);
                    Thread.Sleep(50);
                    GetYearSelect().SelectByIndex(i);
                    Thread.Sleep(50);
                    GetMonthSelect().SelectByIndex(j);
                }
            }
        }

        private SelectElement GetAccountSelect()
        {
            return new SelectElement(Browser.FindElement(By.Id("applyTo")));
        }

        private SelectElement GetYearSelect()
        {
            return new SelectElement(Browser.FindElement(By.Id("years_year")));
        }

        private SelectElement GetMonthSelect()
        {
            return new SelectElement(Browser.FindElement(By.Id("months_month")));
        }

        [TestMethod]
        public override void DownloadAllData()
        {
            Browser.FindElement(By.XPath("//*[@id=\"collapseTwo\"]/table/tbody/tr/td[6]/a")).Click();
            var accountNumber = Browser.FindElement(By.XPath("//*[@id=\"accordion2\"]/div/table[1]/tbody/tr[2]/td[2]")).Text;

            NavigateHome();
            DownloadTransactions(accountNumber);

            NavigateHome();
            DownloadPdfs(accountNumber);
        }
    }
}