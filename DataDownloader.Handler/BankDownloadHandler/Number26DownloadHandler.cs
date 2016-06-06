using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using CsvHelper;
using CsvHelper.Configuration;
using DataDownloader.Common.Properties;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.Model;
using DataDownloader.Handler.Selenium;
using KeePass;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;

namespace DataDownloader.Handler.BankDownloadHandler
{
    public class Number26DownloadHandler : BankDownloadHandlerBase
    {
        public Number26DownloadHandler(string password)
            : base(password, "https://my.number26.de/", Path.Combine(SettingHandler.Default.DataDownloaderPath,
                SettingHandler.Default.DataDownloaderSubfolderNumber26))
        {
        }

        public Number26DownloadHandler(SecureString password) : base(password, "https://my.number26.de/", Path.Combine(SettingHandler.Default.DataDownloaderPath,
                SettingHandler.Default.DataDownloaderSubfolderNumber26))
        {
        }

        protected override void Login()
        {
            Browser.WaitForJavaScript();
            Browser.FindElement(By.XPath("//input[@type='email']"))
                .SendKeys(KeePass.GetEntryByUuid(SettingHandler.Default.KeePassEntryUuidNumber26).GetUserName());
            Browser.FindElement(By.XPath("//input[@type='password']"))
                .SendKeys(KeePass.GetEntryByUuid(SettingHandler.Default.KeePassEntryUuidNumber26).GetPassword());

            Browser.FindElement(new ByAll(By.TagName("a"), By.ClassName("login"))).Click();
        }

        protected override void Logout()
        {
            Browser.WaitForJavaScript();
            Browser.FindElement(By.ClassName("logout")).Click();

            Browser.WaitForJavaScript();
            Browser.FindElement(By.CssSelector(".btn.ok")).Click();
        }

        protected override void NavigateHome()
        {
        }

        protected override void Download()
        {
            Browser.WaitForJavaScript(5000);

            DownloadTransactions();

            DownloadPdfs();
        }

        private void DownloadTransactions()
        {
            Browser.FindElement(new ByAll(By.TagName("button"), By.ClassName("activities"))).Click();
            Browser.WaitForJavaScript();

            var entries =
                Browser.ExecuteJavaScript<object>(
                    "var array_values = new Array();for (var key in Backbone.activities_.attributes) { array_values.push(Backbone.activities_.attributes[key]); } return array_values;");
            var values =
                ((IEnumerable)entries).Cast<Dictionary<string, object>>()
                    .Where(objects => objects.Count > 0)
                    .Select(objects => new N26TransactionEntry(objects))
                    .OrderBy(entry => entry.Valuta)
                    .ThenBy(entry => entry.Confirmed)
                    .ThenBy(entry => entry.VisibleTS)
                    .ToList();
            using (var stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(stringWriter, new CsvConfiguration() { Delimiter = ";" }))
                {
                    csvWriter.WriteRecords(values);
                    FileDownloader.WriteFile("transactions.csv", stringWriter.ToString());
                }
            }
        }

        private List<IWebElement> GetSeparators()
        {
            return Browser.FindElements(new ByChained(By.CssSelector(".holder.activities"), By.CssSelector(".node.delim"))).ToList();
        }

        private List<IWebElement> GetTransactions()
        {
            return Browser.FindElements(new ByChained(By.CssSelector(".holder.activities"), By.CssSelector(".node.activity"))).ToList();
        }

        private void ScrollDown()
        {
            Browser.FindElementOnPage(By.CssSelector(".holder.activities"));
            Browser.ExecuteJavaScript("scroll(0, 500);");
            Browser.WaitForJavaScript();
        }

        private void DownloadPdfs()
        {
            Browser.FindElement(new ByAll(By.TagName("button"), By.ClassName("balancestatements"))).Click();
            Browser.WaitForJavaScript();

            for (int i = 1; i < GetBalanceStatementLinks().Count; i++)
            {
                GetBalanceStatementLinks()[i].Click();
                Browser.WaitForJavaScript(10000);
            }

            foreach (var file in Directory.GetFiles(DownloadPath, "*).pdf"))
            {
                File.Delete(file);
            }

            foreach (var file in Directory.GetFiles(DownloadPath, "*.pdf"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var dateTime = DateTime.ParseExact(fileName, "yyyy-M", CultureInfo.InvariantCulture);
                var newFileName = $"{dateTime.ToString("yyyy-MM")}.pdf";
                var newPath = Path.Combine(DownloadPath, newFileName);
                if (File.Exists(newPath))
                {
                    File.Delete(file);
                }
                else
                {
                    File.Move(file, newPath);
                }
            }
        }

        private List<IWebElement> GetBalanceStatementLinks()
        {
            return Browser.FindElements(new ByChained(By.CssSelector(".node.balancestatement"), By.TagName("a"))).ToList();
        }
    }
}