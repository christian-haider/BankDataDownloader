using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using DataDownloaderSelenium.Properties;
using OpenQA.Selenium;
using Cookie = System.Net.Cookie;

namespace DataDownloaderSelenium.Selenium
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

        public string DownloadFile(IWebElement element)
        {
            return Download(element, "href");
        }

        public string DownloadImage(IWebElement element)
        {
            return Download(element, "src");
        }

        public string Download(IWebElement element, string attribute)
        {
            //Assuming that getAttribute does some magic to return a fully qualified URL
            return Download(element.GetAttribute(attribute));
        }

        public string Download(string url)
        {
            if (url.Trim().Equals(string.Empty))
            {
                throw new Exception("The element you have specified does not link to anything!");
            }
            var webClient = new WebClientWithCookies(ExtractCookiesFromDriver());
            //webClient.DownloadFileCompleted += (sender, args) => { };
            var bytes = webClient.DownloadData(url);
            string fileName = null;
            if (!String.IsNullOrEmpty(webClient.ResponseHeaders["Content-Disposition"]))
            {
                var raw = webClient.ResponseHeaders["Content-Disposition"];
                fileName = raw.Substring(raw.IndexOf("filename=", StringComparison.Ordinal)+9);
                fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, invalidFileNameChar) => current.Replace(invalidFileNameChar.ToString(), string.Empty));
                fileName = fileName.Replace(";", "");
            }
            fileName = fileName ?? DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            Directory.CreateDirectory(DownloadPath);
            File.WriteAllBytes(Path.Combine(DownloadPath, fileName), bytes);
            return Path.Combine(DownloadPath, fileName);
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