using System.Windows;
using System.Windows.Media;
using System.Data;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace WpfApp1
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Singleton.Instance.SqlServer.OpenConnection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.SqlServer.CloseConnection();
            Singleton.Instance.MainWindow.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.MainWindow.main.Navigate(new Registration());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                                   login.Text,
                                   password.Password))
            {
                string request = $@"select [login],[password],[is_admin],id_passenger from [system]
                             where [login] = @Login AND [password] = @Password;";
                SqlCommand command = Singleton.Instance.SqlServer.CreateSqlCommand(request);
                command.Parameters.Add("@Login", SqlDbType.NVarChar).Value = login.Text;
                command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password.Password;

                DataTable data = Singleton.Instance.SqlServer.Select(command);
                if (data.Rows.Count > 0)
                {
                    uint passenger_id = uint.Parse(data.Rows[0].ItemArray[3].ToString());
                    if (data.Rows[0].ItemArray[2].ToString() == "True")
                    {
                        // админ
                        Singleton.Instance.MainWindow.main.Navigate(new AdminPage());
                    }
                    else
                    {
                        // пользователь
                        Singleton.Instance.MainWindow.main.Navigate(new UserPage(passenger_id, login.Text));
                    }
                }
                else
                {
                    password.Password = "";
                    MessageBox.Show("Неверный логин или пароль.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
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
