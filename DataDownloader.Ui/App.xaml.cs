using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using DataDownloader.Common.Properties;

namespace DataDownloader.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ChangeUiLanguage(SettingsHandler.Instance.LanguageIso639_1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iso639_1">the 2 letter language code</param>
        public void ChangeUiLanguage(string iso639_1)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(SettingsHandler.Instance.LanguageIso639_1);

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
             new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));
        }
    }
}
