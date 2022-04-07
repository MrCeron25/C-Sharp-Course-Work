using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageCities : Page
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
        public PageCities()
        {
            InitializeComponent();
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubWindow window = new SubWindow();
            window.label.Content = "Название города :";
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

        private void change_Click(object sender, RoutedEventArgs e)
        {

        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
