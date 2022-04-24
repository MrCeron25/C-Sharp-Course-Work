using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MyTicketsPage : Page
    {
        private readonly uint passengerId;

        public MyTicketsPage(uint PassengerId)
        {
            InitializeComponent();
            passengerId = PassengerId;

            string request = $"select * from get_user_tickets(@IdPassenger);";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
            command.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            
            DataTable data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0) //есть билеты
            {
                dataGrid.ItemsSource = data.DefaultView;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }

        private void SaveAllTickets()
        {
            // получение информации о пользователе
            string request = $"select name,surname,sex from passengers where id = @IdPassenger;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
            command.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;

            DataTable data = SqlServer.Instance.Select(command);
            DataRow row = data.Rows[0];
            string name = row["name"].ToString();
            string surname = row["surname"].ToString();
            string sex = row["sex"].ToString();

            string saveFolder = $@"C:\Users\ARTEM\Desktop\КОРОНОВИРУС\21-22\БД\6 семестр\курсовая\WpfApp1\WpfApp1\Tickets\";
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
                Saver.Save(saveFolder, 
                           $"{NewTicket.ticketId} {NewTicket.name} {NewTicket.surname} {NewTicket.flightName}", 
                           NewTicket.ToString());
            }
        }
        private void Back_Click_1(object sender, RoutedEventArgs e)
        {
            Thread newThread = new Thread(SaveAllTickets);
            newThread.Start();
        }
    }
}
