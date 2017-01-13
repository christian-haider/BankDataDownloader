using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataDownloader.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for IconButton.xaml
    /// </summary>
    public partial class TabHeader : StackPanel
    {
        public static readonly DependencyProperty IconSourcedDependencyProperty =
            DependencyProperty.Register("IconSource", typeof (ImageSource), typeof (TabHeader));

        public static readonly DependencyProperty TextDependencyProperty = DependencyProperty.Register("Text",
            typeof (string), typeof (TabHeader));

        public TabHeader()
        {
            InitializeComponent();
        }

        public ImageSource IconSource
        {
            get { return GetValue(IconSourcedDependencyProperty) as ImageSource; }
            set { SetValue(IconSourcedDependencyProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextDependencyProperty) as string; }
            set { SetValue(TextDependencyProperty, value); }
        }
    }
}