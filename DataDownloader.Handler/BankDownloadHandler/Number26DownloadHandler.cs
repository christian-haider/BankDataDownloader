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
using DataDownloader.Common.Model;
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
            : base(password, "https://my.number26.de/", Path.Combine(SettingsHandler.Instance.DataDownloaderPath,
                SettingsHandler.Instance.DataDownloaderSubfolderNumber26))
        {
        }

        public Number26DownloadHandler(SecureString password) : base(password, "https://my.number26.de/", Path.Combine(SettingsHandler.Instance.DataDownloaderPath,
                SettingsHandler.Instance.DataDownloaderSubfolderNumber26))
        {
        }

        protected override void Login()
        {
            Browser.WaitForJavaScript();
            Browser.FindElement(By.Name("email"))
                .SendKeys(KeePass.GetEntryByUuid(SettingsHandler.Instance.KeePassEntryUuidNumber26).GetUserName());
            Browser.FindElement(By.Name("password"))
                .SendKeys(KeePass.GetEntryByUuid(SettingsHandler.Instance.KeePassEntryUuidNumber26).GetPassword());

            Browser.FindElement(new ByAll(By.TagName("a"), By.ClassName("login"))).Click();
        }

        protected override void Logout()
        {
            Browser.WaitForJavaScript();
            Browser.FindElement(By.ClassName("UIHeader__logout-button")).Click();

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
            Screenshot ss = ((ITakesScreenshot)Browser).GetScreenshot();
            ss.SaveAsFile(Path.Combine(DownloadPath, "transactions.png"), System.Drawing.Imaging.ImageFormat.Png);

            //Click download button
            Browser.FindElement(By.ClassName("csv")).Click();
            Browser.WaitForJavaScript();
            //Click previous a few times
            Browser.FindElement(By.ClassName("ui-datepicker-today")).Click();
            Browser.WaitForJavaScript(100);
            for (int i = 0; i < 12; i++)
            {
                Browser.FindElement(By.ClassName("ui-datepicker-prev")).Click();
                Browser.WaitForJavaScript(100);
            }
            //Click first day of month
            Browser.FindElement(new ByChained(By.ClassName("ui-datepicker-calendar"), By.XPath("//*[@data-handler='selectDay']"))).Click();
            //Click download
            Browser.FindElement(By.ClassName("ok")).Click();
            Browser.WaitForJavaScript();
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
            Browser.ExecuteJavaScript<object>("scroll(0, 500);");
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
                try
                {
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
                catch (FormatException e)
                {
                    Log.Warn(e, "Couldn't rename balance statement pdf {0}", file);
                }
            }
        }

        private List<IWebElement> GetBalanceStatementLinks()
        {
            return Browser.FindElements(new ByChained(By.CssSelector(".node.balancestatement"), By.TagName("a"))).ToList();
        }
    }
}