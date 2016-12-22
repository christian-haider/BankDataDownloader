using System;
using System.IO;
using System.Linq;
using System.Security;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.Selenium;
using KeePass;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace DataDownloader.Handler.BankDownloadHandler
{
    public class RciDownloadHandler : BankDownloadHandlerBase
    {
        public static readonly string RciUrl = "https://ebanking.renault-bank-direkt.at";
        public static readonly string RciDownloadPath = Path.Combine(SettingHandler.Default.DataDownloaderPath,
            SettingHandler.Default.DataDownloaderSubfolderRci);

        public RciDownloadHandler(string masterPassword) : base(masterPassword, RciUrl, RciDownloadPath)
        {
        }

        public RciDownloadHandler(SecureString masterPassword) : base(masterPassword, RciUrl, RciDownloadPath)
        {
        }

        protected override void Login()
        {
            Browser.FindElement(By.Id("username")).SendKeys(KeePass.GetEntryByUuid(SettingHandler.Default.KeePassEntryUuidRci).GetUserName());
            Browser.FindElement(new ByChained(By.Id("login"), By.XPath("//input[@type='password']"))).SendKeys(KeePass.GetEntryByUuid(SettingHandler.Default.KeePassEntryUuidRci).GetPassword());
            Browser.FindElement(By.Id("submitButton")).Click();
        }

        protected override void Logout()
        {
            Browser.FindElement(By.ClassName("kontoLogout")).Click();
        }

        protected override void NavigateHome()
        {
            //Browser.FindElement(By.XPath("//*[@id='mainMenu']/ul/li[1]/a")).Click();
            Browser.FindElement(By.PartialLinkText("KONTEN & ZAHLUNGSVERKEHR")).Click();
        }

        private void DownloadTransactions()
        {
            NavigateHome();
            Browser.FindElement(By.XPath("//*[@id='subSubMenu']/li[2]/a")).Click();
            //Browser.FindElement(By.PartialLinkText("Kontoübersicht")).Click();

            //*[@id="submitButton"]
            Browser.FindElement(By.Name("trigger:BUTTON2::")).Click();
            //From date input
            var fromDate = Browser.FindElement(new ByChained(By.CssSelector(".field.cf.west"), By.TagName("input")));
            fromDate.Clear();
            fromDate.SendKeys(DateTime.Today.AddYears(-1).ToString("dd.MM.yyyy"));
            //Checkbox details
            var checkboxDivs = Browser.FindElements(By.CssSelector(".krcheck.checkbox"));
            var detailsCheckbox = checkboxDivs.Last();
            if (!detailsCheckbox.Selected)
            {
                detailsCheckbox.Click();
            }
            //Submit
            Browser.FindElement(By.Id("default")).Click();

            //Screenshot
            Screenshot ss = ((ITakesScreenshot)Browser).GetScreenshot();
            ss.SaveAsFile(Path.Combine(DownloadPath, "screenshot.png"), System.Drawing.Imaging.ImageFormat.Png);

            //Download
            Browser.FindElement(By.XPath("//a[@title='Download']")).Click();
        }

        protected override void Download()
        {
            Browser.WaitForJavaScript(5000);
            DownloadTransactions();
            Browser.WaitForJavaScript(2000);
        }
    }
}