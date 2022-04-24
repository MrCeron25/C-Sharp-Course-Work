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
            SqlServer.Instance.OpenConnection();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            SqlServer.Instance.CloseConnection();
            Manager.Instance.MainWindow.Close();
        }

        private void Back_Click_1(object sender, RoutedEventArgs e)
        {
            Manager.Instance.MainFrame.Navigate(new Registration());
        }

        private void Back_Click_2(object sender, RoutedEventArgs e)
        {
            if (Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                                   login.Text,
                                   password.Password))
            {
                string request = $@"select [login],[password],[is_admin],id_passenger from [system]
                             where [login] = @Login AND [password] = @Password;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@Login", SqlDbType.NVarChar).Value = login.Text;
                command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password.Password;

                DataTable data = SqlServer.Instance.Select(command);
                if (data.Rows.Count > 0)
                {
                    uint passenger_id = uint.Parse(data.Rows[0].ItemArray[3].ToString());
                    if (data.Rows[0].ItemArray[2].ToString() == "True")
                    {
                        // админ
                        Manager.Instance.MainFrame.Navigate(new AdminPage());
                    }
                    else
                    {
                        // пользователь
                        Manager.Instance.MainFrame.Navigate(new UserPage(passenger_id, login.Text));
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
