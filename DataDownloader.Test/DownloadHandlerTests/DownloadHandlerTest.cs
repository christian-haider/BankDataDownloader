using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DataDownloader.Common.Settings;
using DataDownloader.Handler.BankDownloadHandler;
using DataDownloader.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DataDownloader.Test.DownloadHandlerTests
{
    public abstract class DownloadHandlerTestBase<TDownloadHandler> where TDownloadHandler : BankDownloadHandlerBase
    {
        protected TDownloadHandler DownloadHandler;
        protected TestSettingHandler TestSettingHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            var name = "DataDownloader.Test.testsettings.json";
            using (var stream = typeof(TestSettingHandler).Assembly.GetManifestResourceStream(name))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream, Encoding.Default))
                    {
                        TestSettingHandler = JsonConvert.DeserializeObject<TestSettingHandler>(sr.ReadToEnd());
                        SettingHandler.MockSettingHandler(TestSettingHandler);
                    }
                }
            }
            var constructors = typeof(TDownloadHandler).GetConstructors();
            var constructor = constructors.Single(info => info.GetParameters().First().ParameterType == typeof(string));
            DownloadHandler = (TDownloadHandler)constructor.Invoke(new object[] { TestSettingHandler.KeePassPassword });
        }

        [TestMethod]
        public void TestDownloadAllData()
        {
            DownloadHandler.DownloadAllData();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DownloadHandler.Dispose();
        }
    }

    [TestClass]
    public class DkbDownloadHandlerTest : DownloadHandlerTestBase<DkbDownloadHandler>
    {
    }

    [TestClass]
    public class Number26DownloadHandlerTest : DownloadHandlerTestBase<Number26DownloadHandler>
    {
    }

    [TestClass]
    public class RaiffeisenDownloadHandlerTest : DownloadHandlerTestBase<RaiffeisenDownloadHandler>
    {
    }

    [TestClass]
    public class SantanderDownloadHandlerTest : DownloadHandlerTestBase<SantanderDownloadHandler>
    {
    }
}
