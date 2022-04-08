using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageCountries : Page
    {
        private void UpdateTable()
        {
            string request = $@"select * from country;";
            SqlCommand command = Singleton.Instance.SqlServer.CreateSqlCommand(request);

            DataTable data = Singleton.Instance.SqlServer.Select(command);
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
            UpdateTable();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Singleton.Instance.MainWindow.main.CanGoBack)
            {
                Singleton.Instance.MainWindow.main.GoBack();
            }
        }

        private void Row_Click(object sender, MouseButtonEventArgs e)
        {
            if (!delete.IsEnabled && dataGrid.SelectedIndex != -1)
            {
                delete.IsEnabled = true;
            }
            if (!change.IsEnabled && dataGrid.SelectedIndex != -1)
            {
                change.IsEnabled = true;
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = dataGrid.SelectedItem as DataRowView;
            uint id = uint.Parse(rowview.Row[0].ToString());

            string request = $@"EXECUTE DeleteCountry @countryId;";
            SqlCommand command = Singleton.Instance.SqlServer.CreateSqlCommand(request);
            command.Parameters.Add("@countryId", SqlDbType.BigInt).Value = id;

            DataTable data = Singleton.Instance.SqlServer.Select(command);
            if (data.Rows[0].ItemArray[0].ToString() == "1")
            {
                //MessageBox.Show("Город успешно удалён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateTable();
            delete.IsEnabled = false;
            change.IsEnabled = false;
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = dataGrid.SelectedItem as DataRowView;
            string countyName = rowview.Row[1].ToString();
            string id = rowview.Row[0].ToString();

            SubWindow window = new SubWindow();
            window.label.Content = "Страна :";
            window.textBox.Text = countyName;
            window.Title = "Окно изменения";
            window.action.Content = "Сохранить";
            if ((bool)window.ShowDialog() && window.textBox.Text != countyName)
            {
                string request = $@"EXECUTE UpdateCountry @CountryId, @NewName;";
                SqlCommand command = Singleton.Instance.SqlServer.CreateSqlCommand(request);
                command.Parameters.Add("@CountryId", SqlDbType.BigInt).Value = id;
                command.Parameters.Add("@NewName", SqlDbType.NVarChar).Value = window.textBox.Text;

                DataTable data = Singleton.Instance.SqlServer.Select(command);
                if (data.Rows.Count > 0 && data.Rows[0].ItemArray[0].ToString() == "1")
                {
                    //MessageBox.Show("Город успешно изменён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateTable();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubWindow window = new SubWindow();
            window.label.Content = "Название страны :";
            window.Title = "Окно добавления";
            window.action.Content = "Добавить";
            if ((bool)window.ShowDialog())
            {
                string request = $@"insert into country(name) values (@CountryName);";
                SqlCommand command = Singleton.Instance.SqlServer.CreateSqlCommand(request);
                command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = window.textBox.Text;

                int updatedRows = Singleton.Instance.SqlServer.ExecuteRequest(command);
                if (updatedRows > 0)
                {
                    //MessageBox.Show("Город успешно добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdateTable();
            }
        }
    }
}
