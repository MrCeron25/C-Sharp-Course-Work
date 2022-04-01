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
        private string request;
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
                request = $@"select [login],[password],[is_admin],id_passenger from [system]
                                where [login] = @Login AND [password] = @Password;";
                DataTable tableWithData = MySingleton.Instance.SqlServer.Select(request);
                if (tableWithData.Rows.Count > 0)
                {
                    uint passenger_id = uint.Parse(tableWithData.Rows[0].ItemArray[3].ToString());
                    if (tableWithData.Rows[0].ItemArray[2].ToString() == "True")
                    {
                        // админ
                        MySingleton.Instance.MainWindow.main.Navigate(new AdminPage());
                    }
                    else
                    {
                        // пользователь
                        MySingleton.Instance.MainWindow.main.Navigate(new UserPage(passenger_id, login.Text));
                    }
                }
                else
                {
                    password.Password = "";
                    MessageBox.Show("Неверный логин или пароль.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
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
