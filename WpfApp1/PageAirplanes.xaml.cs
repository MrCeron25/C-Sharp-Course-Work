using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class PageAirplanes : Page
    {
        public PageAirplanes()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }
    }
}
