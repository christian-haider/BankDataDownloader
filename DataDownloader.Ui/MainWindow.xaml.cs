using System.Windows;

namespace DataDownloader.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingWindow().ShowDialog();
        }

        private void ButtonStartDownload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
