using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class BuyTicket : Page
    {
        private readonly uint passengerId;
        private readonly uint idFlight;
        private readonly uint numberOfSeat;
        private string request;
        private DataTable data;

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

        public BuyTicket(uint IdFlight, uint PassengerId)
        {
            InitializeComponent();
            passengerId = PassengerId;
            idFlight = IdFlight;

            Singleton.Instance.SqlServer.cmd.Parameters.Add("@FlightId", SqlDbType.BigInt).Value = idFlight;
            request = $@"select * from dbo.get_occupied_seats(@FlightId);";
            data = Singleton.Instance.SqlServer.Select(request);

            List<uint> occupiedPlaces = new List<uint>();
            foreach (DataRow item in data.Rows)
            {
                occupiedPlaces.Add(uint.Parse(item[0].ToString()));
            }

            request = $@"select dbo.get_number_of_seats(@FlightId);";
            data = Singleton.Instance.SqlServer.Select(request);
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();

            numberOfSeat = uint.Parse(data.Rows[0].ItemArray[0].ToString());
            combobox.ItemsSource = GetFreeSeats(occupiedPlaces, numberOfSeat);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (combobox.SelectedIndex != -1)
            {
                uint seat_number = uint.Parse(combobox.SelectedItem.ToString());
                Singleton.Instance.SqlServer.cmd.Parameters.Add("@FlightId", SqlDbType.BigInt).Value = idFlight;
                Singleton.Instance.SqlServer.cmd.Parameters.Add("@SeatNumber", SqlDbType.BigInt).Value = seat_number;
                Singleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
                request = $@"insert into tickets(flight_id, seat_number,id_passenger) 
                             values (@FlightId, @SeatNumber, @IdPassenger);";
                int result = Singleton.Instance.SqlServer.ExecuteRequest(request);
                if (result > 0)
                {
                    MessageBox.Show("Вы успешно забронировали билет.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Singleton.Instance.SqlServer.cmd.Parameters.Clear();
                Button_Click(null, null); // возврат назад
            }
            else
            {
                MessageBox.Show("Выберите № места.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Singleton.Instance.MainWindow.main.CanGoBack)
            {
                Singleton.Instance.MainWindow.main.GoBack();
            }
        }
    }
}
