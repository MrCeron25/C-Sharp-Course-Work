using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }

        private void Back_Click_1(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageCountries());
        }

        private void Back_Click_2(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageCities());
        }

        private void Back_Click_3(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageAirplanes());
        }

        private void Back_Click_4(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageFlights());
        }

        private void Back_Click_5(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PassengerList());
        }

        private void Back_Click_6(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new SoldTickets());
        }
    }
}
