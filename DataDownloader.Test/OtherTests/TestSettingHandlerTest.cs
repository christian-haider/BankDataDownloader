using System;
using System.IO;
using System.Text;
using DataDownloader.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DataDownloader.Test.OtherTests
{
    [TestClass]
    public class TestSettingHandlerTest
    {
        [TestMethod]
        public void WriteTestSettingSkeleton()
        {
            Console.WriteLine(JsonConvert.SerializeObject(new TestSettings(), Formatting.Indented));
        }
    }
}
