
using System.IO;
using System.Linq;
using DataDownloaderSelenium.Properties;
using DataDownloaderSelenium.Selenium;
using KeePass;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;


namespace DataDownloaderSelenium
{
    [TestClass]
    public class RaiffeisenDownloader : BankDownloaderBase
    {
        static RaiffeisenDownloader()
        {
            Url = "https://banking.raiffeisen.at";
        }

        protected override void Login()
        {
            var entry = KeePass.GetEntryByUuid(Settings.Default.KeePass_Entry_Uuid_Raiffeisen);

            //change to username login
            Browser.FindElement(By.Id("j_id280:benutzer")).Click();

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

        protected void NavigateGiro()
        {
            Browser.FindElement(
                new ByChained(
                    By.ClassName("kontoTable"),
                    By.TagName("tbody"),
                    By.XPath("tr[1]"),
                    By.XPath("td[1]"),
                    By.TagName("a")
                    )).Click();
        }

        protected void SetMaxDateRange()
        {
            Browser.FindElement(By.Id("kontoauswahlSelectionToggleLink")).Click();

            var month = new SelectElement(Browser.FindElement(
                By.Id("j_id1_kontoinfo_WAR_kontoinfoportlet_INSTANCE_9k3Y_:umsaetzeForm:kontoauswahlDatumVon-month-year")
                    .Or(new ByChained(By.Id("kontoauswahlSelectionExtended"), By.ClassName("float-left"),
                        By.ClassName("cal-month-year")))));
            month.SelectByIndex(0);

            var day = new SelectElement(Browser.FindElement(
                By.Id("j_id1_kontoinfo_WAR_kontoinfoportlet_INSTANCE_9k3Y_:umsaetzeForm:kontoauswahlDatumVon-day")
                    .Or(new ByChained(By.Id("kontoauswahlSelectionExtended"), By.ClassName("float-left"),
                        By.ClassName("cal-day")))));
            day.SelectByIndex(0);

            Browser.FindElement(new ByChained(By.ClassName("boxFormFooter"),
                new ByAll(By.ClassName("button"), By.ClassName("button-colored")))).Click();
        }

        protected void DownloadCsv()
        {
            //*[@id="j_id1_kontoinfo_WAR_kontoinfoportlet_INSTANCE_9k3Y_:umsaetzeForm"]/div[3]/div[1]/div/a
            Browser.FindElement(
                new ByChained(By.ClassName("serviceButtonArea"),
                    new ByAll(By.ClassName("formControlButton"), By.ClassName("print")))).Click();

            var combo = new SelectElement(Browser.FindElement(
                new ByChained(By.ClassName("mainInput"), By.ClassName("inputLarge"))));
            combo.SelectByValue("CSV");

            var link =
                Browser.FindElement(
                    new ByChained(By.ClassName("formFooterRight"),
                    new ByAll(By.ClassName("button"), By.ClassName("button-colored"))));
            var downloader = new SeleniumFileDownloader(Browser, Path.Combine(Settings.Default.DataDownloader_Path, Settings.Default.DataDownloader_Subfolder_Raiffeisen));
            downloader.DownloadFile(link);
        }

        [TestMethod]
        public void CodedUITestMethod1()
        {
            NavigateGiro();
            SetMaxDateRange();
            DownloadCsv();

            //id 
            //class float left 
            //class cal-month-year
        }
    }
}
