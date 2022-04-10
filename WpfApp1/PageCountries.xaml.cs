using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageCountries : Page
    {
        private void UpdateCountries()
        {
            string request = $@"select name [Страна] from country;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0)
            {
                dataGrid.ItemsSource = data.DefaultView;
            }
            else
            {
                dataGrid.ItemsSource = null;
            }
        }

        public PageCountries()
        {
            InitializeComponent();
            UpdateCountries();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }

        private void Row_Click(object sender, MouseButtonEventArgs e)
        {
            if (!change.IsEnabled && dataGrid.SelectedIndex != -1)
            {
                change.IsEnabled = true;
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = dataGrid.SelectedItem as DataRowView;
            string countyName = rowview.Row[0].ToString();

            SubWindowCities window = new SubWindowCities();
            window.label.Content = "Страна :";
            window.textBox.Text = countyName;
            window.Title = "Окно изменения";
            window.action.Content = "Сохранить";
            if ((bool)window.ShowDialog() && window.textBox.Text != countyName)
            {
                string request = $@"EXECUTE UpdateCountry @CountryName, @NewName;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@NewName", SqlDbType.NVarChar).Value = window.textBox.Text;
                command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = countyName;

                DataTable data = SqlServer.Instance.Select(command);
                if (data.Rows.Count > 0 && data.Rows[0].ItemArray[0].ToString() == "1")
                {
                    //MessageBox.Show("Город успешно изменён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateCountries();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubWindowCities window = new SubWindowCities();
            window.label.Content = "Название страны :";
            window.Title = "Окно добавления";
            window.action.Content = "Добавить";
            if ((bool)window.ShowDialog())
            {
                string request = $@"insert into country(name) values (@CountryName);";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = window.textBox.Text;

                int updatedRows = SqlServer.Instance.ExecuteRequest(command);
                if (updatedRows > 0)
                {
                    //MessageBox.Show("Город успешно добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdateCountries();
            }
        }
    }
}
