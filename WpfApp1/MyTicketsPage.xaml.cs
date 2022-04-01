using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MyTicketsPage.xaml
    /// </summary>
    public partial class MyTicketsPage : Page
    {
        private uint passengerId;
        public MyTicketsPage(uint PassengerId)
        {
            InitializeComponent();
            passengerId = PassengerId;
            MySingleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            string req = $@"select * from get_user_tickets(@IdPassenger);";
            DataTable data = MySingleton.Instance.SqlServer.GetDataTable(req);
            MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
            if (data != null && data.Rows.Count > 0)
            {
                //есть билеты
                dataGrid.ItemsSource = data.DefaultView;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MySingleton.Instance.MainWindow.main.CanGoBack)
            {
                MySingleton.Instance.MainWindow.main.GoBack();
            }
        }

        private void ThreadTask()
        {
            // получение инф о пользователе
            MySingleton.Instance.SqlServer.cmd.Parameters.Add("@IdPassenger", SqlDbType.BigInt).Value = passengerId;
            string req = $@"select name,surname,sex from passengers where id = @IdPassenger;";
            DataTable data = MySingleton.Instance.SqlServer.GetDataTable(req);
            MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
            DataRow row = data.Rows[0];
            string name = row["name"].ToString();
            string surname = row["surname"].ToString();
            string sex = row["sex"].ToString();

            string saveFolder = $@"C:\Users\ARTEM\Desktop\КОРОНОВИРУС\21-22\БД\6 семестр\курсовая\WpfApp1\WpfApp1\Tickets";
            //string saveFolder = Directory.GetCurrentDirectory();
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
                                               sex
                                             );
                NewTicket.SaveTicket(saveFolder);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(ThreadTask);
            thread.Start();
        }
    }
}
