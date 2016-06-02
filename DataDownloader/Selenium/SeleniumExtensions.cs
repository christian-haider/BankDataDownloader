using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace DataDownloader.Selenium
{
    public static class SeleniumExtensions
    {
        public static By Or(this By by, By otherBy)
        {
            return new ByAllDisjunctive(by, otherBy);
        }

        public static IWebElement GetParent(this IWebElement node)
        {
            return node.FindElement(By.XPath(".."));
        }

        public static IWebElement FindElementOnPage(this IWebDriver webDriver, By by)
        {
            RemoteWebElement element = (RemoteWebElement)webDriver.FindElement(by);
            var hack = element.LocationOnScreenOnceScrolledIntoView;
            return element;
        }

        public static void WaitForJavaScript(this IWebDriver webDriver, int timeout = 1500)
        {
            //var wait = new WebDriverWait(webDriver, new TimeSpan(1000));
            //wait.Until(driver => driver.ExecuteJavaScript<string>("return document.readyState").Equals("complete"));
            Thread.Sleep(timeout);
        }

        public static Dictionary<string, object> GetAllAttributes(this IWebDriver webDriver, IWebElement webElement)
        {
            return webDriver.ExecuteJavaScript<Dictionary<string, object>>(
                "var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;",
                webElement);
        }
    }
}