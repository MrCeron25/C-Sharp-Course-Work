using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private readonly uint passengerId;
        private string req;
        private bool IsPurchasedTickets()
        {
            bool res = false; //нет билетов
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            string req = $@"select * from get_user_tickets(@IdPassenger);";
            DataTable data = Singleton.Instance.SqlServer.Select(req);
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
                        req = $@"select * from dbo.get_list_of_flights_with_dep_and_arr_date(@DepartureCity, @ArrivalCity, @StartDate, @EndDate);";
                    }
                    else
                    {
                        //start: NOT empty end: empty
                        req = $@"select * from dbo.get_list_of_flights_with_dep__date(@DepartureCity, @ArrivalCity, @StartDate);";
                    }
                    DataTable data = Singleton.Instance.SqlServer.Select(req);

                    //ComboBox comboBox = new ComboBox();
                    //data.Columns.Add(new DataColumn("col", typeof(ComboBox)));

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
            //selectedRow = dataGrid.SelectedIndex;
            if (!buyTicket.IsEnabled && dataGrid.SelectedIndex != -1)
            {
                buyTicket.IsEnabled = true;
            }
        }

        private void buyTicket_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = dataGrid.SelectedItem as DataRowView;
            uint id = uint.Parse(rowview.Row[0].ToString());
            Singleton.Instance.MainWindow.main.Navigate(new BuyTicket(id, passengerId));
            dataGrid.ItemsSource = null;
            buyTicket.IsEnabled = false;
        }

        private void myTicket_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.MainWindow.main.Navigate(new MyTicketsPage(passengerId));
        }
    }
}
