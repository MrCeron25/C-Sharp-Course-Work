using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class PageFlightsChange : Page
    {
        private Flight flight { get; set; }
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

        public PageFlightsChange(Flight flight)
        {
            InitializeComponent();
            flightName.Text = flight.FlightName;

            departureCity.ItemsSource = GetCities();
            departureCity.SelectedItem = flight.DepartureCity;

            arrivalCity.ItemsSource = GetCitiesExept(GetCities(), flight.DepartureCity);
            arrivalCity.SelectedItem = flight.ArrivalCity;

            departureDate.DisplayDateStart = flight.DepartureDate;
            departureDate.SelectedDate = flight.DepartureDate;

            travelTime.Text = flight.TravelTime.ToShortTimeString();

            price.Text = flight.Price.ToString();

            airplanes.ItemsSource = GetAirplane();
            airplanes.SelectedItem = flight.Airplane;

            this.flight = flight;
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
                Change.IsEnabled = true;
            }
            else
            {
                Change.IsEnabled = false;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
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

        private void travelTime_TextChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CheckAddButton();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Flight newFlight = new Flight(flightName.Text,
                                          departureCity.Text,
                                          arrivalCity.Text,
                                          DateTime.Parse(departureDate.SelectedDate.ToString()),
                                          DateTime.Parse(travelTime.Text),
                                          double.Parse(price.Text),
                                          airplanes.Text);
            if (!flight.Equals(newFlight))
            {
                string request = $@"EXECUTE ChangeFlight @FlightName,
	                                                     @DepartureCity,
	                                                     @ArrivalCity,
	                                                     @DepartureDate,
	                                                     @TravelTime,
	                                                     @Price,
	                                                     @Airplane,
	
	                                                     @NewFlightName,
	                                                     @NewDepartureCity,
	                                                     @NewArrivalCity,
	                                                     @NewDepartureDate,
	                                                     @NewTravelTime,
	                                                     @NewPrice,
	                                                     @NewAirplane;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@FlightName", SqlDbType.NVarChar).Value = flight.FlightName;
                command.Parameters.Add("@DepartureCity", SqlDbType.NVarChar).Value = flight.DepartureCity;
                command.Parameters.Add("@ArrivalCity", SqlDbType.NVarChar).Value = flight.ArrivalCity;
                command.Parameters.Add("@DepartureDate", SqlDbType.DateTime).Value = flight.DepartureDate;
                command.Parameters.Add("@TravelTime", SqlDbType.Time).Value = flight.TravelTime.ToShortTimeString();
                command.Parameters.Add("@Price", SqlDbType.Float).Value = flight.Price;
                command.Parameters.Add("@Airplane", SqlDbType.NVarChar).Value = flight.Airplane;

                command.Parameters.Add("@NewFlightName", SqlDbType.NVarChar).Value = newFlight.FlightName;
                command.Parameters.Add("@NewDepartureCity", SqlDbType.NVarChar).Value = newFlight.DepartureCity;
                command.Parameters.Add("@NewArrivalCity", SqlDbType.NVarChar).Value = newFlight.ArrivalCity;
                command.Parameters.Add("@NewDepartureDate", SqlDbType.DateTime).Value = newFlight.DepartureDate;
                command.Parameters.Add("@NewTravelTime", SqlDbType.Time).Value = newFlight.TravelTime.ToShortTimeString();
                command.Parameters.Add("@NewPrice", SqlDbType.Float).Value = newFlight.Price;
                command.Parameters.Add("@NewAirplane", SqlDbType.NVarChar).Value = newFlight.Airplane;

                DataTable data = SqlServer.Instance.Select(command);
                if (data.Rows[0].ItemArray[0].ToString() == "1")
                {
                    MessageBox.Show("Рейс сохранён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Manager.Instance.MainFrame.Navigate(new PageFlights());
            }
        }
    }
}
