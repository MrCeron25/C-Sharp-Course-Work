using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageCountries());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageCities());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageAirplanes());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageFlights());
        }
    }
}
