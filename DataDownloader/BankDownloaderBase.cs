using System;
using DataDownloader.Properties;
using KeePass;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDownloader
{
    public abstract class BankDownloaderBase
    {
        protected BrowserWindow Browser;
        protected KeePassWrapper KeePass;
        protected static string Url;

        [TestInitialize]
        public void TestInitialize()
        {
            KeePass = KeePassWrapper.OpenWithPassword(Settings.Default.KeePass_Path, Settings.Default.KeePass_MasterPassword);
            Browser = BrowserWindow.Launch(new Uri(Url));

            Login();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Logout();

            Browser.Close();
        }

        protected HtmlControl Find(UITestControl parent = null)
        {
            var control = new HtmlControl(parent ?? Browser);
            return control;
        }

        protected HtmlEdit Edit(UITestControl parent = null)
        {
            var control = new HtmlEdit(parent ?? Browser);
            return control;
        }

        protected abstract void Login();
        protected abstract void Logout();
    }
}