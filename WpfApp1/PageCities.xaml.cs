using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageCities : Page
    {
        private List<string> GetUpdatedCountries()
        {
            List<string> result = new List<string>();
            string request = $"EXECUTE GetCountries;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            foreach (DataRow item in data.Rows)
            {
                result.Add(item["name"].ToString());
            }
            return result;
        }
        private void UpdateCities()
        {
            string request = $@"select co.name [Страна], ci.name [Город] from cities ci
                                join country co on co.id = ci.country_id";
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

        public PageCities()
        {
            InitializeComponent();
            UpdateCities();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SubWindowCitiesAdd window = new SubWindowCitiesAdd();
            window.ComboBox.ItemsSource = GetUpdatedCountries();
            if ((bool)window.ShowDialog())
            {
                string request = $"EXECUTE AddCities @CityName, @CountryName;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = window.textBox.Text;
                command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = window.ComboBox.SelectedItem.ToString();
                int updatedRows = SqlServer.Instance.ExecuteRequest(command);
                if (updatedRows > 0)
                {
                    //MessageBox.Show("Город успешно добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateCities();
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView rowView = dataGrid.SelectedItem as DataRowView;
                string CityName = rowView.Row.ItemArray[1].ToString();
                string CountyName = rowView.Row.ItemArray[0].ToString();
                SubWindowCitiesChange window = new SubWindowCitiesChange();
                window.ComboBox.ItemsSource = GetUpdatedCountries();
                window.ComboBox.SelectedItem = CountyName;
                window.textBox.Text = CityName;
                if ((bool)window.ShowDialog() && (
                    CountyName != window.ComboBox.SelectedItem.ToString() ||
                    CityName != window.textBox.Text))
                {
                    string request = $"EXECUTE ChangeCities @CityName, @NewCityName, @CountryName, @NewCountryName;";
                    SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                    command.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = CityName;
                    command.Parameters.Add("@NewCityName", SqlDbType.NVarChar).Value = window.textBox.Text;
                    command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountyName;
                    command.Parameters.Add("@NewCountryName", SqlDbType.NVarChar).Value = window.ComboBox.SelectedItem.ToString();
                    DataTable data = SqlServer.Instance.Select(command);
                    if (data.Rows[0].ItemArray[0].ToString() == "1")
                    {
                        //MessageBox.Show("Изменения сохранены.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    UpdateCities();
                }
            }
        }
    }
}
