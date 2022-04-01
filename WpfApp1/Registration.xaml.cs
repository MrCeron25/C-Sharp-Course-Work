using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Linq;

namespace WpfApp1
{
    public partial class Registration : Page
    {
        private string request;
        private DataTable data;
        public Registration()
        {
            InitializeComponent();
        }

        private static bool CheckPassword(string password)
        {
            bool res = false;
            char[] specialCharactersArray = "%!@#$%^&*()?/>.<,:;'\\|}]{[_~`+=-\"".ToCharArray();
            //min 6 chars OR At least 1 upper case letter OR At least 1 lower case letter
            if (password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsLower))
            {
                res = false;
            }
            else if (password.Any(specialCharactersArray.Contains)) // At least 1 special char
            {
                res = true;
            }
            return res;
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
                    Singleton.Instance.SqlServer.cmd.Parameters.Add("@Login", SqlDbType.NVarChar).Value = login.Text;
                    request = $@"select [login] from [system]
                                where [login] = @Login;";
                    data = Singleton.Instance.SqlServer.Select(request);
                    if (data.Rows.Count > 0)
                    {
                        MessageBox.Show("Логин уже занят.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name.Text;
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Surname", SqlDbType.NVarChar).Value = surname.Text;
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Sex", SqlDbType.NVarChar).Value = sex.Text;
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Date_of_birth", SqlDbType.Date).Value = date_of_birth.Text;
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Passport_id", SqlDbType.Int).Value = passport_id.Text;
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Passport_series", SqlDbType.Int).Value = passport_series.Text;
                        Singleton.Instance.SqlServer.cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password.Password;
                        request = $@"EXEC UserRegistration @Name, @Surname, @Sex, @Date_of_birth, @Passport_id, 
                                                           @Passport_series, @Login, @Password;";
                        data = Singleton.Instance.SqlServer.Select(request);
                        if (data.Rows[0].ItemArray[0].ToString() == "1")
                        {
                            Singleton.Instance.MainWindow.main.Navigate(new MainPage());
                            MessageBox.Show("Вы успешно зарегистрировались.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    Singleton.Instance.SqlServer.cmd.Parameters.Clear();
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
            if (Singleton.Instance.MainWindow.main.CanGoBack)
            {
                Singleton.Instance.MainWindow.main.GoBack();
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
