using System;
using System.IO;
using System.Linq;
using System.Net;
using OpenQA.Selenium;
using Cookie = System.Net.Cookie;

namespace DataDownloader.Selenium
{
    public class SeleniumFileDownloader
    {
        private readonly IWebDriver _driver;
        public string DownloadPath { get; private set; }

        public SeleniumFileDownloader(IWebDriver driverObject, string downloadPath)
        {
            _driver = driverObject;
            DownloadPath = downloadPath;
        }

        private CookieContainer ExtractCookiesFromDriver()
        {
            var seleniumCookies = _driver.Manage().Cookies;
            var cookies = new CookieContainer();
            foreach (var seleniumCookie in seleniumCookies.AllCookies)
            {
                var newC = new Cookie
                {
                    Domain = seleniumCookie.Domain,
                    HttpOnly = seleniumCookie.IsHttpOnly,
                    Secure = seleniumCookie.Secure,
                    Expires = seleniumCookie.Expiry.GetValueOrDefault(),
                    Name = seleniumCookie.Name,
                    Path = seleniumCookie.Path,
                    Value = seleniumCookie.Value
                };
                cookies.Add(newC);
            }
            return cookies;
        }

        public string DownloadFile(IWebElement element, bool fileDatePrefix = true, string fileOtherPrefix = null)
        {
            return Download(element, "href", fileDatePrefix, fileOtherPrefix);
        }

        public string DownloadImage(IWebElement element, bool fileDatePrefix = true, string fileOtherPrefix = null)
        {
            return Download(element, "src", fileDatePrefix, fileOtherPrefix);
        }

        public string DownloadCurrentPageSource(string fileName, bool fileDatePrefix = true,
            string fileOtherPrefix = null)
        {
            return WriteFile(fileName, _driver.PageSource, fileDatePrefix, fileOtherPrefix);
        }

        public string Download(IWebElement element, string attribute, bool fileDatePrefix = true,
            string fileOtherPrefix = null)
        {
            //Assuming that getAttribute does some magic to return a fully qualified URL
            return Download(element.GetAttribute(attribute), fileDatePrefix, fileOtherPrefix);
        }

        public string Download(string url, bool fileDatePrefix = true, string fileOtherPrefix = null)
        {
            if (url.Trim().Equals(string.Empty))
            {
                throw new Exception("The element you have specified does not link to anything!");
            }
            using (var webClient = new WebClientWithCookies(ExtractCookiesFromDriver()))
            {
                //webClient.DownloadFileCompleted += (sender, args) => { };
                var bytes = webClient.DownloadData(url);
                string fileName = null;
                if (!string.IsNullOrEmpty(webClient.ResponseHeaders["Content-Disposition"]))
                {
                    var raw = webClient.ResponseHeaders["Content-Disposition"];
                    fileName = raw.Substring(raw.IndexOf("filename=", StringComparison.Ordinal) + 9);
                    fileName = Path.GetInvalidFileNameChars()
                        .Aggregate(fileName,
                            (current, invalidFileNameChar) =>
                                current.Replace(invalidFileNameChar.ToString(), string.Empty));
                    fileName = fileName.Replace(";", "");
                }
                return WriteFile(fileName, bytes, fileDatePrefix, fileOtherPrefix);
            }
        }

        private string WriteFile(string fileName, byte[] bytes, bool fileDatePrefix = true, string fileOtherPrefix = null)
        {
            fileName = PrepareFileName(fileName, fileDatePrefix, fileOtherPrefix);

            Directory.CreateDirectory(DownloadPath);
            File.WriteAllBytes(Path.Combine(DownloadPath, fileName), bytes);
            return Path.Combine(DownloadPath, fileName);
        }

        private string WriteFile(string fileName, string bytes, bool fileDatePrefix = true, string fileOtherPrefix = null)
        {
            fileName = PrepareFileName(fileName, fileDatePrefix, fileOtherPrefix);

            Directory.CreateDirectory(DownloadPath);
            File.WriteAllText(Path.Combine(DownloadPath, fileName), bytes);
            return Path.Combine(DownloadPath, fileName);
        }

        private string PrepareFileName(string fileName, bool fileDatePrefix = true, string fileOtherPrefix = null)
        {
            return $"{(fileDatePrefix ? DateTime.Now.ToString("yyyy-MM-dd_HHmmss_") : "")}{(fileOtherPrefix != null ? fileOtherPrefix + "_" : "")}{fileName}";
        }
    }

    public class WebClientWithCookies : WebClient
    {
        public WebClientWithCookies(CookieContainer container = null)
        {
            this.CookieContainer = container ?? new CookieContainer();
        }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = CookieContainer;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                CookieContainer.Add(cookies);
            }
        }
    }
}