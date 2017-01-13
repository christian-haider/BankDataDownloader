using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using DataDownloader.Common.Properties;
using DataDownloader.Handler.Selenium;
using KeePass;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace DataDownloader.Handler.BankDownloadHandler
{

    public class RaiffeisenDownloadHandler : BankDownloadHandlerBase
    {
        public RaiffeisenDownloadHandler(string password) : base(password, "https://banking.raiffeisen.at", Path.Combine(SettingsHandler.Instance.DataDownloaderPath, SettingsHandler.Instance.DataDownloaderSubfolderRaiffeisen))
        {
        }

        public RaiffeisenDownloadHandler(SecureString password) : base(password, "https://banking.raiffeisen.at", Path.Combine(SettingsHandler.Instance.DataDownloaderPath, SettingsHandler.Instance.DataDownloaderSubfolderRaiffeisen))
        {
        }

        protected override void Login()
        {
            var entry = KeePass.GetEntryByUuid(SettingsHandler.Instance.KeePassEntryUuidRaiffeisen);

            //change to username login
            Browser.FindElement(By.Id("tab-benutzer")).Click();

            //type username
            Browser.FindElement(new ByIdOrName("loginform:LOGINNAME")).SendKeys(entry.GetUserName());

            //type password
            Browser.FindElement(new ByIdOrName("loginform:LOGINPASSWD")).SendKeys(entry.GetPassword());

            //check pass
            Browser.FindElement(new ByIdOrName("loginform:checkPasswort")).Click();

            //type pin
            Browser.FindElement(new ByIdOrName("loginpinform:PIN")).SendKeys(entry.GetString("PIN"));

            //final login
            Browser.FindElement(new ByIdOrName("loginpinform:anmeldenPIN")).Click();
        }

        protected override void Logout()
        {
            Browser.FindElement(new ByAll(By.ClassName("button"), By.ClassName("logoutlink"))).Click();
        }

        protected override void NavigateHome()
        {
            Browser.FindElement(new ByChained(By.Id("nav"), By.TagName("ul"), By.TagName("li"), By.TagName("a"))).Click();
        }

        private void NavigateDepots()
        {
            NavigateHome();
            Browser.FindElement(By.LinkText("Depots")).Click();
        }

        protected override void Download()
        {
            for (int i = 0; i < GetAccountLinks().Count; i++)
            {
                var accountNumber = $"konto_{GetAccountLinks()[i].Text}";
                GetAccountLinks()[i].Click();

                Screenshot ss = ((ITakesScreenshot)Browser).GetScreenshot();
                ss.SaveAsFile(Path.Combine(DownloadPath, $"{accountNumber}.png"), System.Drawing.Imaging.ImageFormat.Png);

                SetMaxDateRange();

                Browser.FindElement(
                new ByChained(By.ClassName("serviceButtonArea"),
                    new ByAll(By.ClassName("formControlButton"), By.ClassName("print")))).Click();

                DownloadCsv(accountNumber);
                NavigateHome();
            }

            try
            {
                NavigateDepots();

                for (int i = 0; i < GetAccountLinks().Count; i++)
                {
                    var accountNumber = $"depot_{GetAccountLinks()[i].Text.Split('/')[1].Trim()}";
                    GetAccountLinks()[i].Click();

                    Browser.FindElement(new ByChained(By.ClassName("serviceButtonArea"), By.LinkText("Daten exportieren"))).Click();

                    DownloadCsv(accountNumber);

                    NavigateDepots();
                }
            }
            catch (NoSuchElementException)
            {
            }
        }

        private void DownloadCsv(string filePrefix = null)
        {
            var combo =
                new SelectElement(Browser.FindElement(new ByChained(By.ClassName("mainInput"), By.TagName("select"))));
            combo.SelectByValue("CSV");

            Browser.FindElement(By.LinkText("Datei erstellen")).Click();
        }

        private void SetMaxDateRange()
        {
            Browser.FindElement(By.Id("kontoauswahlSelectionToggleLink")).Click();

            var month = new SelectElement(Browser.FindElement(By.ClassName("cal-month-year")));
            month.SelectByIndex(0);

            var day = new SelectElement(Browser.FindElement(By.ClassName("cal-day")));
            day.SelectByIndex(0);

            Browser.FindElement(new ByChained(By.ClassName("boxFormFooter"),
                new ByAll(By.ClassName("button"), By.ClassName("button-colored")))).Click();
        }

        private List<IWebElement> GetAccountLinks()
        {
            return Browser.FindElements(
                new ByChained(
                    By.ClassName("kontoTable"),
                    By.TagName("tbody"),
                    By.TagName("tr"),
                    By.XPath("td[1]"),
                    By.TagName("a")
                    )).ToList();
        }
    }
}
