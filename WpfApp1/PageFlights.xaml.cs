using System;
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
            //Tools.ChangeDateFormatInDataTable(data);//!!!!!!!!!!
            if (data != null && data.Rows.Count > 0)
            {
                dataGrid.ItemsSource = data.DefaultView;
            }
            else
            {
                dataGrid.ItemsSource = null;
            }
            change.IsEnabled = false;
            inArchive.IsEnabled = false;
        }

        public PageFlights()
        {
            InitializeComponent();
            UpdateFlights();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }

        private void Row_Click(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                if (!change.IsEnabled)
                {
                    change.IsEnabled = true;
                }
                if (!inArchive.IsEnabled)
                {
                    inArchive.IsEnabled = true;
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new PageFlightsAdd());
            UpdateFlights();
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView rowview = dataGrid.SelectedItem as DataRowView;
                Flight flight = new Flight(rowview.Row[0].ToString(),
                                           rowview.Row[1].ToString(),
                                           rowview.Row[2].ToString(),
                                           DateTime.Parse(rowview.Row[3].ToString()),
                                           DateTime.Parse(rowview.Row[4].ToString()),
                                           double.Parse(rowview.Row[6].ToString()),
                                           rowview.Row[7].ToString());
                Manager.Instance.MainFrame.Navigate(new PageFlightsChange(flight));
            }
        }

        private void inArchive_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView rowview = dataGrid.SelectedItem as DataRowView;

                string request = $@"EXECUTE FlightToArchive @DepartureCityName, 
                                                            @ArrivalCityName,
                                                            @AirplaneName,
                                                            @FlightName,
                                                            @TraveTime,
                                                            @Price,
                                                            @DepartureDate;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@FlightName", SqlDbType.NVarChar).Value = rowview.Row[0].ToString();
                command.Parameters.Add("@DepartureCityName", SqlDbType.NVarChar).Value = rowview.Row[1].ToString();
                command.Parameters.Add("@ArrivalCityName", SqlDbType.NVarChar).Value = rowview.Row[2].ToString();
                command.Parameters.Add("@DepartureDate", SqlDbType.DateTime).Value = DateTime.Parse(rowview.Row[3].ToString());
                command.Parameters.Add("@TraveTime", SqlDbType.Time).Value = DateTime.Parse(rowview.Row[4].ToString()).ToShortTimeString();
                command.Parameters.Add("@Price", SqlDbType.Float).Value = double.Parse(rowview.Row[6].ToString());
                command.Parameters.Add("@AirplaneName", SqlDbType.NVarChar).Value = rowview.Row[7].ToString();

                DataTable data = SqlServer.Instance.Select(command);
                if (data.Rows.Count > 0 && data.Rows[0].ItemArray[0].ToString() == "1")
                {
                    //MessageBox.Show("Рейс добавлен в архив.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateFlights();
            }
        }
    }
}
