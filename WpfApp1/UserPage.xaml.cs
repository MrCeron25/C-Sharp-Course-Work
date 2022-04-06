using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Collections.Generic;

namespace WpfApp1
{
    public partial class UserPage : Page
    {
        private readonly uint passengerId;
        private uint numberOfSeat;
        private string request;
        private uint flightId;
        private DataTable data;
        private bool IsPurchasedTickets()
        {
            bool res = false; //нет билетов
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            request = $@"select * from get_user_tickets(@IdPassenger);";
            DataTable data = Singleton.Instance.SqlServer.Select(request);
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();
            if (data != null && data.Rows.Count > 0)
            {
                //есть билеты
                res = true;
            }
            return res;
        }
        public UserPage(uint PassengerId, string userName)
        {
            InitializeComponent();
            name.Content = userName;
            passengerId = PassengerId;

            // если у пользователя нет купленных билетов
            if (!IsPurchasedTickets())
            {
                myTicket.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Singleton.Instance.MainWindow.main.CanGoBack)
            {
                Singleton.Instance.MainWindow.main.GoBack();
            }
        }

        private void Picture_MouseDown(object sender, MouseEventArgs e)
        {
            string buf = to.Text;
            to.Text = from.Text;
            from.Text = buf;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                                   from.Text,
                                   to.Text))
            {
                if (!string.IsNullOrEmpty(start.Text))
                {
                    Singleton.Instance.SqlServer.cmd.Parameters.Add("@DepartureCity", SqlDbType.NVarChar).Value = from.Text;
                    Singleton.Instance.SqlServer.cmd.Parameters.Add("@ArrivalCity", SqlDbType.NVarChar).Value = to.Text;
                    Singleton.Instance.SqlServer.cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = start.SelectedDate;
                    if (!string.IsNullOrEmpty(end.Text))
                    {
                        //start: NOT empty end: NOT empty
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end.SelectedDate;
                        request = $@"select * from dbo.get_list_of_flights_with_dep_and_arr_date(@DepartureCity, @ArrivalCity, @StartDate, @EndDate);";
                    }
                    else
                    {
                        //start: NOT empty end: empty
                        request = $@"select * from dbo.get_list_of_flights_with_dep__date(@DepartureCity, @ArrivalCity, @StartDate);";
                    }
                    DataTable data = Singleton.Instance.SqlServer.Select(request);


                    if (data != null && data.Rows.Count > 0)
                    {
                        dataGrid.ItemsSource = data.DefaultView;
                    }
                    else
                    {
                        dataGrid.ItemsSource = null;
                    }
                    Singleton.Instance.SqlServer.cmd.Parameters.Clear();
                    dataGrid.SelectedIndex = -1;
                    buyTicket.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Выберите дату отправления.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Заполните города вылета и прилёта.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Row_Click(object sender, MouseButtonEventArgs e)
        {
            DataRowView rowview = dataGrid.SelectedItem as DataRowView;
            flightId = uint.Parse(rowview.Row[0].ToString());
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@flightId", SqlDbType.BigInt).Value = flightId;
            request = $@"select * from dbo.get_occupied_seats(@flightId);";
            data = Singleton.Instance.SqlServer.Select(request);

            List<uint> occupiedPlaces = new List<uint>();
            foreach (DataRow item in data.Rows)
            {
                occupiedPlaces.Add(uint.Parse(item[0].ToString()));
            }

            request = $@"select dbo.get_number_of_seats(@flightId);";
            data = Singleton.Instance.SqlServer.Select(request);
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();

            numberOfSeat = uint.Parse(data.Rows[0].ItemArray[0].ToString());
            combobox.ItemsSource = GetFreeSeats(occupiedPlaces, numberOfSeat);
            if (combobox.Items.Count > 0)
            {
                combobox.IsEnabled = true;
            }

            buyTicket.IsEnabled = false;
            combobox.SelectedItem = null;
        }

        private List<uint> GetFreeSeats(List<uint> occupiedPlaces, uint numberOfSeat)
        {
            List<uint> res = new List<uint>();
            for (uint i = 1; i <= numberOfSeat; i++)
            {
                if (!occupiedPlaces.Contains(i))
                {
                    res.Add(i);
                }
            }
            return res;
        }

        private void buyTicket_Click(object sender, RoutedEventArgs e)
        {
            uint seat_number = uint.Parse(combobox.SelectedItem.ToString());
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@flightId", SqlDbType.BigInt).Value = flightId;
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@SeatNumber", SqlDbType.BigInt).Value = seat_number;
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            request = $@"insert into tickets(flight_id, seat_number,id_passenger) 
                             values (@flightId, @SeatNumber, @IdPassenger);";
            int result = Singleton.Instance.SqlServer.ExecuteRequest(request);
            if (result > 0)
            {
                MessageBox.Show("Вы успешно забронировали билет.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();
            dataGrid.ItemsSource = null;
            buyTicket.IsEnabled = false;
            combobox.IsEnabled = false;
            combobox.ItemsSource = null;
            Button_Click_1(null, null); // обновление таблицы
        }

        private void myTicket_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.MainWindow.main.Navigate(new MyTicketsPage(passengerId));
        }

        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!buyTicket.IsEnabled && dataGrid.SelectedIndex != -1 && combobox.SelectedIndex != -1)
            {
                buyTicket.IsEnabled = true;
            }
        }
    }
}
