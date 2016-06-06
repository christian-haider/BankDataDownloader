using System.IO;
using System.Text;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.BankDownloadHandler;
using DataDownloader.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DataDownloader.Test.DownloadHandlerTests
{
    [TestClass]
    public class RaiffeisenDownloadHandlerTest
    {
        private RaiffeisenDownloadHandler _downloadHandler;
        private TestSettingHandler _testSettingHandler;

        [TestInitialize]
        public void TestInit()
        {
            var name = "DataDownloader.Test.testsettings.json";
            using (var stream = typeof(TestSettingHandler).Assembly.GetManifestResourceStream(name))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream, Encoding.Default))
                    {
                        _testSettingHandler = JsonConvert.DeserializeObject<TestSettingHandler>(sr.ReadToEnd());
                        SettingHandler.MockSettingHandler(_testSettingHandler);
                    }
                }
            }

            _downloadHandler = new RaiffeisenDownloadHandler(_testSettingHandler.KeePassPassword);
        }

        [TestMethod]
        public void TestMethod1()
        {
            _downloadHandler.DownloadAllData();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _downloadHandler.Dispose();
        }
    }
}
