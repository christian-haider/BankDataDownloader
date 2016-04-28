using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

namespace DataDownloader
{
    public static class HtmlControlBuilderExtensions
    {
        public static T Parent<T>(this T control, HtmlControl parent) where T : HtmlControl
        {
            control.Container = parent;
            return control;
        }

        public static T ById<T>(this T control, string id) where T : HtmlControl
        {
            control.SearchProperties.Add(HtmlControl.PropertyNames.Id, id);
            return control;
        }

        public static T ByClass<T>(this T control, string @class) where T : HtmlControl
        {
            control.SearchProperties.Add(HtmlControl.PropertyNames.Class, @class,PropertyExpressionOperator.Contains);
            return control;
        }

        public static T ByTagName<T>(this T control, string tagName) where T : HtmlControl
        {
            control.SearchProperties.Add(HtmlControl.PropertyNames.TagName, tagName);
            return control;
        }

        public static T Click<T>(this T control) where T : HtmlControl
        {
            Mouse.Click(control);
            return control;
        }
    }
}