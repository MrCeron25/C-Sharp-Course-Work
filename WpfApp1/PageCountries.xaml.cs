using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageCountries : Page
    {
        private string request;
        private DataTable data;
        private void UpdateTable()
        {
            request = $@"select * from country;";
            data = Singleton.Instance.SqlServer.Select(request);
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
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@countryId", SqlDbType.BigInt).Value = id;
            request = $@"EXECUTE DeleteCountry @countryId;";
            data = Singleton.Instance.SqlServer.Select(request);
            if (data.Rows[0].ItemArray[0].ToString() == "1")
            {
                //MessageBox.Show("Город успешно удалён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();
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
                Singleton.Instance.SqlServer.cmd.Parameters.Add("@CountryId", SqlDbType.BigInt).Value = id;
                Singleton.Instance.SqlServer.cmd.Parameters.Add("@NewName", SqlDbType.NVarChar).Value = window.textBox.Text;
                request = $@"EXECUTE UpdateCountry @CountryId, @NewName;";
                //request = $@"exec UpdateCountry @CountryId, @NewName;";
                data = Singleton.Instance.SqlServer.Select(request);
                if (data.Rows.Count > 0 && data.Rows[0].ItemArray[0].ToString() == "1")
                {
                    //MessageBox.Show("Город успешно изменён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Singleton.Instance.SqlServer.cmd.Parameters.Clear();
                UpdateTable();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubWindow window = new SubWindow();
            window.label.Content = "Название страны :";
            window.Title = "Окно изменения";
            window.action.Content = "Добавить";
            if ((bool)window.ShowDialog())
            {
                Singleton.Instance.SqlServer.cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = window.textBox.Text;
                request = $@"insert into country(name) values (@CountryName);";
                int updatedRows = Singleton.Instance.SqlServer.ExecuteRequest(request);
                if (updatedRows > 0)
                {
                    //MessageBox.Show("Город успешно добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Singleton.Instance.SqlServer.cmd.Parameters.Clear();
                UpdateTable();
            }
        }
    }
}
