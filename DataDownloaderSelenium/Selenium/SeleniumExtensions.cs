using OpenQA.Selenium;

namespace DataDownloader.Selenium
{
    public static class SeleniumExtensions
    {
        public static By Or(this By by, By otherBy)
        {
            return new ByAllDisjunctive(by, otherBy);
        }
    }
}