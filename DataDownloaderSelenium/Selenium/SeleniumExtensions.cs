using DataDownloaderSelenium.Selenium;
using OpenQA.Selenium;

namespace DataDownloaderSelenium.Selenium
{
    public static class SeleniumExtensions
    {
        public static By Or(this By by, By otherBy)
        {
            return new ByAllDisjunctive(by, otherBy);
        }
    }
}