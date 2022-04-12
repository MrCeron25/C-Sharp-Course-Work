using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageAirplanes : Page
    {
        public void UpdateAirplanes()
        {
            string request = $@"select [model] [Модель], 
                                       [number_of_seats] [Количество мест]
                                from airplane;";
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

        public PageAirplanes()
        {
            InitializeComponent();
            UpdateAirplanes();
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
            SubWindowAirplane window = new SubWindowAirplane();
            window.labelModel.Content = "Модель :";
            window.labelNumberSeats.Content = "Количество мест :";
            window.Title = "Окно добавления";
            window.action.Content = "Добавить";
            if ((bool)window.ShowDialog())
            {
                string request = $"insert into airplane([model], number_of_seats) values (@Model,@NumberOfSeats);";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = window.textBoxModel.Text;
                command.Parameters.Add("@NumberOfSeats", SqlDbType.NVarChar).Value = window.textBoxNumberSeats.Text;

                int updatedRows = SqlServer.Instance.ExecuteRequest(command);
                if (updatedRows > 0)
                {
                    //MessageBox.Show("Самолёт успешно добавлен.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateAirplanes();
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowview = dataGrid.SelectedItem as DataRowView;
            string modelName = rowview.Row[0].ToString();

            SubWindowCities window = new SubWindowCities();
            window.label.Content = "Модель :";
            window.textBox.Text = modelName;
            window.Title = "Окно изменения";
            window.action.Content = "Сохранить";
            if ((bool)window.ShowDialog() && window.textBox.Text != modelName)
            {
                string request = $"EXECUTE UpdateAirplane @AirplaneName, @NewAirplaneName;";
                SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);
                command.Parameters.Add("@NewAirplaneName", SqlDbType.NVarChar).Value = window.textBox.Text;
                command.Parameters.Add("@AirplaneName", SqlDbType.NVarChar).Value = modelName;

                DataTable data = SqlServer.Instance.Select(command);
                if (data.Rows.Count > 0 && data.Rows[0].ItemArray[0].ToString() == "1")
                {
                    //MessageBox.Show("Модель успешно изменена.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка запроса.\n{data.Rows[0].ItemArray[1]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateAirplanes();
            }
        }
    }
}
