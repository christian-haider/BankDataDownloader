using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace DataDownloaderSelenium.Selenium
{
    public class ByAllDisjunctive : By
    {
        private readonly By[] _bys;

        public ByAllDisjunctive(params By[] bys)
        {
            this._bys = bys;
        }

        public override IWebElement FindElement(ISearchContext context)
        {
            ReadOnlyCollection<IWebElement> elements = FindElements(context);
            if (elements.Count == 0)
                throw new NoSuchElementException("Cannot locate an element using " + this.ToString());
            return elements[0];
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            var set = new HashSet<IWebElement>();
            foreach (By by in this._bys)
            {
                foreach (var findElement in by.FindElements(context))
                {
                    set.Add(findElement);
                }
            }
            return set.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (By by in this._bys)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(",");
                stringBuilder.Append((object)by);
            }
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "By.ByAllDisjunctive([{0}])", new object[1]
            {
        (object) stringBuilder.ToString()
            });
        }
    }
}
