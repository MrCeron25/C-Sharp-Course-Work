using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MyTicketsPage : Page
    {
        private readonly uint passengerId;
        private string request;
        private DataTable data;

        public MyTicketsPage(uint PassengerId)
        {
            InitializeComponent();
            passengerId = PassengerId;
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            request = $@"select * from get_user_tickets(@IdPassenger);";
            data = Singleton.Instance.SqlServer.Select(request);
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();
            if (data != null && data.Rows.Count > 0) //есть билеты
            {
                dataGrid.ItemsSource = data.DefaultView;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Singleton.Instance.MainWindow.main.CanGoBack)
            {
                Singleton.Instance.MainWindow.main.GoBack();
            }
        }

        private void SaveAllTickets()
        {
            // получение информации о пользователе
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            request = $@"select name,surname,sex from passengers where id = @IdPassenger;";
            data = Singleton.Instance.SqlServer.Select(request);
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();
            DataRow row = data.Rows[0];
            string name = row["name"].ToString();
            string surname = row["surname"].ToString();
            string sex = row["sex"].ToString();

            string saveFolder = $@"C:\Users\ARTEM\Desktop\КОРОНОВИРУС\21-22\БД\6 семестр\курсовая\WpfApp1\WpfApp1\Tickets";
            DataView view = (DataView)dataGrid.ItemsSource;
            foreach (DataRowView rowView in view)
            {
                Ticket NewTicket = new Ticket(uint.Parse(rowView.Row.ItemArray[0].ToString()),
                                              uint.Parse(rowView.Row.ItemArray[1].ToString()),
                                              uint.Parse(rowView.Row.ItemArray[2].ToString()),
                                              DateTime.Parse(rowView.Row.ItemArray[3].ToString()),
                                              DateTime.Parse(rowView.Row.ItemArray[4].ToString()),
                                              rowView.Row.ItemArray[5].ToString(),
                                              DateTime.Parse(rowView.Row.ItemArray[6].ToString()),
                                              uint.Parse(rowView.Row.ItemArray[7].ToString()),
                                              rowView.Row.ItemArray[8].ToString(),
                                              rowView.Row.ItemArray[9].ToString(),
                                              name,
                                              surname,
                                              sex);
                NewTicket.SaveTicket(saveFolder);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread newThread = new Thread(SaveAllTickets);
            newThread.Start();
        }
    }
}
