using System.Windows;
using System.Windows.Media;
using System.Data;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            MySingleton.Instance.SqlServer.OpenConnection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MySingleton.Instance.SqlServer.CloseConnection();
            MySingleton.Instance.MainWindow.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MySingleton.Instance.MainWindow.main.Navigate(new Registration());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                                   login.Text,
                                   password.Password))
            {
                MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Login", SqlDbType.NVarChar).Value = login.Text;
                MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password.Password;
                string req = $@"select [login],[password],[is_admin],id_passenger from [system]
                                where [login] = @Login AND [password] = @Password;";
                //SqlTable tableWithData = MySingleton.Instance.SqlServer.GetDataFromExecute(req);
                //if (tableWithData.CountRows > 0)
                //{
                //    uint passenger_id = uint.Parse(tableWithData.Data[0][3]);
                //    if (tableWithData.Data[0][2] == "True")
                //    {
                //        // админ
                //        MySingleton.Instance.MainWindow.main.Navigate(new AdminPage());
                //    }
                //    else
                //    {
                //        // пользователь
                //        MySingleton.Instance.MainWindow.main.Navigate(new UserPage(passenger_id, login.Text));
                //    }
                //}
                //else
                //{
                //    password.Password = "";
                //    MessageBox.Show("Неверный логин или пароль.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
                //MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
            }
            else
            {
                MessageBox.Show("Заполните все поля.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.TextBoxChangeColors(login, new BrushConverter());
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs args)
        {
            Tools.PasswordBoxChangeColors(password, new BrushConverter());
        }
    }
}

/*
 Система должна выдавать информацию:
•	количество свободных мест на заданный рейс
•	список пассажиров на заданный рейс с возможностью выдачи отчёта.
•	список рейсов на заданную дату
•	Сумма, полученная от продажи авиабилетов в каждом месяце заданного года с возможностью выдачи отчёта.
Система должна позволять изменять дату и время вылета, тип самолета, пункт отправления, пункт назначения
Система должна реализовывать:
•	куплю-продажу билетов
•	удаление в архив информации о выполненном рейсе и ввод данных о рейсе с таким же номером на очередную дату 
 */