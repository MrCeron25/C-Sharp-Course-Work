using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageFlights : Page
    {
        private void UpdateFlights()
        {
            string request = $@"EXECUTE GetFlights;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            //Tools.ChangeDateFormatInDataTable(data);
            if (data != null && data.Rows.Count > 0)
            {
                dataGrid.ItemsSource = data.DefaultView;
            }
            else
            {
                dataGrid.ItemsSource = null;
            }
        }
        public PageFlights()
        {
            InitializeComponent();
            UpdateFlights();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }

        private void Row_Click(object sender, MouseButtonEventArgs e)
        {
            if (!change.IsEnabled && dataGrid.SelectedIndex != -1)
            {
                change.IsEnabled = true;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageFlightsAdd());
        }
    }
}
