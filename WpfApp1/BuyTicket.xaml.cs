using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для BuyTicket.xaml
    /// </summary>
    public partial class BuyTicket : Page
    {
        private uint passengerId, idFlight, numberOfSeat;

        private List<int> GetFreeSeats(List<int> dontFreeSeats, uint numberOfSeat)
        {
            List<int> res = new List<int>();
            for (int i = 1; i <= numberOfSeat; i++)
            {
                if (!dontFreeSeats.Contains(i))
                {
                    res.Add(i);
                }
            }
            return res;
        }
        public BuyTicket(uint IdFlight, uint PassengerId)
        {
            InitializeComponent();
            passengerId = PassengerId;
            idFlight = IdFlight;

            MySingleton.Instance.SqlServer.cmd.Parameters.Add("@FlightId", SqlDbType.BigInt).Value = idFlight;
            string req = $@"select * from dbo.get_occupied_seats(@FlightId);";
            DataTable data = MySingleton.Instance.SqlServer.GetDataTable(req);
            List<int> dontFreeSeats = new List<int>();
            foreach (DataRow item in data.Rows)
            {
                dontFreeSeats.Add(int.Parse(item[0].ToString()));
            }
            req = $@"select dbo.get_number_of_seats(@FlightId);";
            data = MySingleton.Instance.SqlServer.GetDataTable(req);
            MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
            foreach (DataRow item in data.Rows)
            {
                numberOfSeat = uint.Parse(item[0].ToString());
            }
            combobox.ItemsSource = GetFreeSeats(dontFreeSeats, numberOfSeat);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (combobox.SelectedIndex != -1)
            {
                //Console.WriteLine(combobox.SelectedItem.ToString()); // seat_number
                int seat_number = int.Parse(combobox.SelectedItem.ToString());
                MySingleton.Instance.SqlServer.cmd.Parameters.Add("@FlightId", SqlDbType.BigInt).Value = idFlight;
                MySingleton.Instance.SqlServer.cmd.Parameters.Add("@SeatNumber", SqlDbType.BigInt).Value = seat_number;
                MySingleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
                string req = $@"insert into tickets(flight_id, seat_number,id_passenger) values (@FlightId, @SeatNumber, @IdPassenger);";
                MySingleton.Instance.SqlServer.ExecuteRequest(req);
                MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
                Button_Click(null, null);
                MessageBox.Show("Вы успешно забронировали билет.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Выберите № места.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MySingleton.Instance.MainWindow.main.CanGoBack)
            {
                MySingleton.Instance.MainWindow.main.GoBack();
            }
        }
    }
}
