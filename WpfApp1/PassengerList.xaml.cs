using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class PassengerList : Page
    {
        public void UpdateFlights()
        {
            string request = $"select distinct [flight_name] from flights;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            foreach (DataRow item in data.Rows)
            {
                ComboBoxFlightNames.Items.Add(item["flight_name"].ToString());
            }
            save.IsEnabled = false;
            ComboBoxDate.IsEnabled = false;
            ComboBoxDate.ItemsSource = null;
        }

        public void UpdateDates()
        {
            string request = $@"select [departure_date] from flights
                                where flight_name = @FlName;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
            command.Parameters.Add("@FlName", SqlDbType.NVarChar).Value = ComboBoxFlightNames.SelectedItem.ToString();

            DataTable data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow item in data.Rows)
                {
                    ComboBoxDate.Items.Add(DateTime.Parse(item["departure_date"].ToString()).ToShortDateString());
                }
                ComboBoxDate.IsEnabled = true;
            }
            else
            {
                ComboBoxDate.IsEnabled = false;
            }
        }

        public PassengerList()
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

        private void ComboBoxFlightNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            save.IsEnabled = false;
            dataGrid.ItemsSource = null;
            ComboBoxDate.SelectedItem = null;
            ComboBoxDate.Items.Clear();
            UpdateDates();
        }

        private void ComboBoxDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxDate.SelectedItem != null)
            {
                string request = $@"EXECUTE GetPassengerList @FlightName, @Date;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@FlightName", SqlDbType.NVarChar).Value = ComboBoxFlightNames.SelectedItem.ToString();
                command.Parameters.Add("@Date", SqlDbType.Date).Value = ComboBoxDate.SelectedItem.ToString();


                DataTable data = SqlServer.Instance.Select(command);
                if (data != null && data.Rows.Count > 0)
                {
                    dataGrid.ItemsSource = data.DefaultView;
                    save.IsEnabled = true;
                }
                else
                {
                    dataGrid.ItemsSource = null;
                }
            }
        }

        private void SaveAllPassengers(object obj)
        {
            string homeDirectory = "C:\\Users\\ARTEM\\Desktop\\КОРОНОВИРУС\\21-22\\БД\\6 семестр\\курсовая\\WpfApp1\\WpfApp1\\Flights\\";
            string folderName = homeDirectory + obj.ToString();
            try
            {
                DirectoryInfo directoryInfo;
                if (!Directory.Exists(folderName))
                {
                    directoryInfo = Directory.CreateDirectory(folderName); // создание папки
                }
                DataView view = (DataView)dataGrid.ItemsSource;
                foreach (DataRowView rowView in view)
                {
                    Passenger NewPassenger = new Passenger(rowView.Row.ItemArray[0].ToString(),
                                                           rowView.Row.ItemArray[1].ToString(),
                                                           rowView.Row.ItemArray[2].ToString(),
                                                           DateTime.Parse(rowView.Row.ItemArray[3].ToString()),
                                                           uint.Parse(rowView.Row.ItemArray[4].ToString()),
                                                           uint.Parse(rowView.Row.ItemArray[5].ToString()));
                    string FileName = $"{NewPassenger.Name} {NewPassenger.Surname} {NewPassenger.PassportId} {NewPassenger.PassportSeries}";
                    Saver.Save(folderName, 
                               FileName, 
                               NewPassenger.ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка :\n{e.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Thread newThread = new Thread(new ParameterizedThreadStart(SaveAllPassengers));
            newThread.Start($"{ComboBoxFlightNames.Text} {ComboBoxDate.Text}");
        }
    }
}
