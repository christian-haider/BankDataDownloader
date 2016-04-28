using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Security;
using DataDownloader.Properties;
using KeePass;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using PropertyNames = Microsoft.VisualStudio.TestTools.UITesting.HtmlControls.HtmlControl.PropertyNames;


namespace DataDownloader
{
    [CodedUITest]
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
            Find().ById("j_id280:benutzer").Click();

            //type username
            Edit().ById("loginform:LOGINNAME").Text = entry.GetUserName();

            //type password
            Edit().ById("loginform:LOGINPASSWD").Password = Playback.EncryptText(entry.GetPassword());

            //check pass
            Find().ById("loginform:checkPasswort").Click();

            //type pin
            Edit().ById("loginpinform:PIN").Password = Playback.EncryptText(entry.GetString("PIN"));

            //final login
            Find().ById("loginpinform:anmeldenPIN").Click();
        }

        protected override void Logout()
        {
            Find().ByClass("button").ByClass("logoutlink").Click();
        }

        protected void NavigateGiro()
        {
            var table = Find().ByClass("kontoTable");
            var tableBody = Find(table).ByTagName("tbody");
            var tr = Find(tableBody).ByTagName("tr");
            var td = Find(tr).ByTagName("td");
            Find(td).ByTagName("a").Click();
        }

        protected void SetMaxDateRange()
        {
            Find().ById("kontoauswahlSelectionToggleLink").Click();
            var extended = Find().ById("kontoauswahlSelectionExtended");
            var fromDate = Find(extended).ByClass("float-left");
            var combo = new HtmlComboBox(fromDate);
            combo.ByClass("cal-day").SelectedIndex = 0;
            combo = new HtmlComboBox(fromDate);
            combo.ByClass("cal-month-year").SelectedIndex = 0;

            var divFooter = Find().ByClass("boxFormFooter");
            Find(divFooter).ByClass("button").ByClass("button-colored").Click();
        }

        protected void DownloadCsv()
        {
            Find(Find().ByClass("serviceButtonArea")).ByClass("formControlButton").ByClass("print").Click();

            var combo = new HtmlComboBox(Find().ByClass("mainInput"));
            combo.ByClass("inputLarge").SelectedIndex = 2;

            var link = new HtmlHyperlink(Find().ByClass("formFooterRight"));
            link.ByClass("button").ByClass("button-colored");
            var downloadLink = link.AbsolutePath;
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
