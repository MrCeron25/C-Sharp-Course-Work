using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class PageFlightsAdd : Page
    {
        private List<string> GetCitiesExept(List<string> data, string exept)
        {
            if (data.Contains(exept))
            {
                data.Remove(exept);
            }
            return data;
        }

        private List<string> GetCities()
        {
            List<string> res = new List<string>();
            string request = $@"select name from cities;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow item in data.Rows)
                {
                    res.Add(item.ItemArray[0].ToString());
                }
            }
            return res;
        }

        private List<string> GetAirplane()
        {
            List<string> res = new List<string>();
            string request = $@"select model from airplane;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow item in data.Rows)
                {
                    res.Add(item.ItemArray[0].ToString());
                }
            }
            return res;
        }

        public PageFlightsAdd()
        {
            InitializeComponent();
            airplanes.ItemsSource = GetAirplane();
            departureCity.ItemsSource = GetCities();
            departureDate.DisplayDateStart = DateTime.Now;
            departureDate.SelectedDate = DateTime.Now;
        }

        private void CheckAddButton()
        {
            if (departureCity.SelectedItem != null &&
                arrivalCity.SelectedItem != null &&
                airplanes.SelectedItem != null &&
                travelTime.Text != "00:00" &&
                Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                                   flightName.Text,
                                   departureCity.SelectedItem.ToString(),
                                   arrivalCity.SelectedItem.ToString(),
                                   airplanes.SelectedItem.ToString(),
                                   departureDate.Text,
                                   price.Text))
            {
                Add.IsEnabled = true;
            }
            else
            {
                Add.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }

        private void flightName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckAddButton();
        }

        private void departureCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            arrivalCity.IsEnabled = true;
            arrivalCity.SelectedItem = null;
            arrivalCity.ItemsSource = GetCitiesExept(GetCities(), departureCity.SelectedItem.ToString());
        }

        private void arrivalCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckAddButton();
        }

        private void departureDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckAddButton();
        }

        private void travelTime_LostFocus(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex($@"00:00");
            MatchCollection matches = regex.Matches(travelTime.Text);
            if (matches.Count == 0 && travelTime.Text.Length == 5)
            {
                regex = new Regex($@"\d{2}:\d{2}");
                matches = regex.Matches(travelTime.Text);
                if (matches.Count == 0 &&
                    byte.Parse(travelTime.Text.Substring(0, 2)) < 24 &&
                    byte.Parse(travelTime.Text.Substring(3, 2)) < 60)
                {
                    CheckAddButton();
                }
                else
                {
                    travelTime.Text = $@"00:00";
                }
            }
            else
            {
                travelTime.Text = $@"00:00";
            }
        }

        private void price_TextChanged(object sender, TextChangedEventArgs e)
        {
            uint res;
            if (uint.TryParse(price.Text, out res))
            {
                CheckAddButton();
            }
            else
            {
                price.Text = "";
            }
        }

        private void airplanes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckAddButton();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string request = $@"EXECUTE AddFlight @DepartureCityName, 
                                                  @ArrivalCityName,
                                                  @AirplaneName,
                                                  @FlightName,
                                                  @TraveTime,
                                                  @Price,
                                                  @DepartureDate;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
            command.Parameters.Add("@DepartureCityName", SqlDbType.NVarChar).Value = departureCity.SelectedItem.ToString();
            command.Parameters.Add("@ArrivalCityName", SqlDbType.NVarChar).Value = arrivalCity.SelectedItem.ToString();
            command.Parameters.Add("@AirplaneName", SqlDbType.NVarChar).Value = airplanes.SelectedItem.ToString();
            command.Parameters.Add("@FlightName", SqlDbType.NVarChar).Value = flightName.Text.ToString();
            command.Parameters.Add("@TraveTime", SqlDbType.Time).Value = DateTime.Parse(travelTime.Text).ToShortTimeString();
            command.Parameters.Add("@Price", SqlDbType.Float).Value = uint.Parse(price.Text);
            command.Parameters.Add("@DepartureDate", SqlDbType.DateTime).Value = departureDate.SelectedDate;
            DataTable data = SqlServer.Instance.Select(command);
            if (data.Rows[0].ItemArray[0].ToString() == "1")
            {
                MessageBox.Show("Рейс добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Button_Click(null, null);
        }
    }
}
