using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public class StatusList : List<string>
    {
        public StatusList(List<string> data)
        {
            AddRange(data);
        }
    }


    public partial class PageCities : Page
    {
        public List<string> countries;
        private void UpdateCities()
        {
            string request = $"EXECUTE GetCountries;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            foreach (DataRow item in data.Rows)
            {
                countries.Add(item["name"].ToString());
            }

            request = $"select ci.name [Город] from cities ci;";
            command = SqlServer.Instance.CreateSqlCommand(request);

            data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0)
            {
                //DataGridComboBoxColumn dataGridComboBox = new DataGridComboBoxColumn
                //{
                //    Header = "Страна",
                //    Width = 100
                //};
                //List<string> list = new List<string>
                //{
                //    "Item 1",
                //    "Item 2"
                //};

                //dataGridComboBox.ItemsSource = list;
                //dataGrid.Columns.Add(dataGridComboBox);

                dataGrid.ItemsSource = data.DefaultView;
                //data.Columns.Add(dataGridComboBox);
                //dataGrid.DataContext ;
            }
            else
            {
                dataGrid.ItemsSource = null;
            }
        }

        public PageCities()
        {
            InitializeComponent();
            countries = new List<string>();
            UpdateCities();
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubWindowCities window = new SubWindowCities();
            window.label.Content = "Название города :";
            window.Title = "Окно добавления";
            window.action.Content = "Добавить";
            if ((bool)window.ShowDialog())
            {
                string request = $"insert into country(name) values (@CountryName);";
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
                UpdateCities();
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
