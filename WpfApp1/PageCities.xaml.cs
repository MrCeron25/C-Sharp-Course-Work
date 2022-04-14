using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                //DataGridComboBoxColumn dataGridComboBox = new DataGridComboBoxColumn();
                //dataGridComboBox.HeaderText = "Name";
                //dataGridComboBox.Items.Add("Ghanashyam");
                //dataGridComboBox.Items.Add("Jignesh");
                //dataGridComboBox.Items.Add("Ishver");
                //dataGridComboBox..Add("Anand");
                //dataGridComboBox.Name = "cmbName";
                //dataGrid.Columns.Add(dgvCmb);

                //foreach (DataRow item in data.Rows)
                //{
                //    System.Console.WriteLine(item[0].ToString());
                //}
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

                //dataGrid.DataContext = ;

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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SubWindowCitiesAdd window = new SubWindowCitiesAdd();
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
            SubWindowCitiesChange window = new SubWindowCitiesChange();
            window.LabelTextBox.Content = "Название города :";
            window.LabelComboBox.Content = "Страна :";
            window.ComboBox.ItemsSource = GetUpdatedCountries();
            window.Title = "Окно изменения";
            window.action.Content = "Изменить";
            if ((bool)window.ShowDialog())
            {
                //string request = $"insert into country(name) values (@CountryName);";
                //SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                //command.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = window.textBox.Text;
                //int updatedRows = SqlServer.Instance.ExecuteRequest(command);
                //if (updatedRows > 0)
                //{
                //    //MessageBox.Show("Город успешно добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                //else
                //{
                //    MessageBox.Show($"Ошибка запроса.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
                UpdateCities();
            }
        }
    }
}
