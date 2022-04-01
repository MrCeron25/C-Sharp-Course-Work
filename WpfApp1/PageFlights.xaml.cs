using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class PageFlights : Page
    {
        public PageFlights()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Singleton.Instance.MainWindow.main.CanGoBack)
            {
                Singleton.Instance.MainWindow.main.GoBack();
            }
        }
    }
}
