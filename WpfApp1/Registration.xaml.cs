using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        private string request;
        public Registration()
        {
            InitializeComponent();
        }

        private static bool CheckPassword(string password)
        {
            //min 6 chars
            if (password.Length < 8)
                return false;
            //At least 1 upper case letter
            if (!password.Any(char.IsUpper))
                return false;
            //At least 1 lower case letter
            if (!password.Any(char.IsLower))
                return false;
            //At least 1 special char
            string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();
            foreach (char c in specialCharactersArray)
            {
                if (password.Contains(c))
                {
                    return true;
                }
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                             name.Text,
                             surname.Text,
                             date_of_birth.Text,
                             passport_id.Text,
                             passport_series.Text,
                             login.Text,
                             password.Password))
            {
                if (CheckPassword(password.Password))
                {
                    // проверка логина в бд
                    MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Login", SqlDbType.NVarChar).Value = login.Text;
                    request = $@"select [login] from [system]
                                where [login] = @Login;";
                    DataTable tableWithData = MySingleton.Instance.SqlServer.Select(request);
                    if (tableWithData.Rows.Count > 0)
                    {
                        MessageBox.Show("Логин уже занят.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name.Text;
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Surname", SqlDbType.NVarChar).Value = surname.Text;
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Sex", SqlDbType.NVarChar).Value = sex.Text;
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Date_of_birth", SqlDbType.Date).Value = date_of_birth.Text;
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Passport_id", SqlDbType.Int).Value = passport_id.Text;
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Passport_series", SqlDbType.Int).Value = passport_series.Text;
                        MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password.Password;
                        request = $@"EXEC UserRegistration @Name, @Surname, @Sex, @Date_of_birth, @Passport_id, 
                                                   @Passport_series, @Login, @Password;";
                        tableWithData = MySingleton.Instance.SqlServer.Select(request);
                        if (tableWithData.Rows[0].ItemArray[0].ToString() == "1")
                        {
                            MySingleton.Instance.MainWindow.main.Navigate(new MainPage());
                            MessageBox.Show("Вы успешно зарегистрировались.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка запроса.\n{tableWithData.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
                }
                else
                {
                    MessageBox.Show("Слишком лёгкий пароль.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MySingleton.Instance.MainWindow.main.CanGoBack)
            {
                MySingleton.Instance.MainWindow.main.GoBack();
            }
        }

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.TextBoxChangeColors(login, new BrushConverter());
        }

        private void passport_id_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.TextBoxChangeColors(passport_id, new BrushConverter());
        }

        private void passport_series_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.TextBoxChangeColors(passport_series, new BrushConverter());
        }

        private void name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.TextBoxChangeColors(name, new BrushConverter());
        }

        private void surname_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.TextBoxChangeColors(surname, new BrushConverter());
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs args)
        {
            Tools.PasswordBoxChangeColors(password, new BrushConverter());
        }

        private void date_of_birth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(date_of_birth.Text))
            {
                date_of_birth.Background = (Brush)new BrushConverter().ConvertFromString("#FFFF0000");
            }
            else
            {
                date_of_birth.Background = (Brush)new BrushConverter().ConvertFromString("#00FFFFFF");
            }
        }
    }
}
