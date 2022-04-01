using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для PageCountries.xaml
    /// </summary>
    public partial class PageCountries : Page
    {
        private void UpdateTable()
        {
            string req = $@"select * from country;";
            DataTable data = MySingleton.Instance.SqlServer.GetDataTable(req);
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
            if (MySingleton.Instance.MainWindow.main.CanGoBack)
            {
                MySingleton.Instance.MainWindow.main.GoBack();
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
            MySingleton.Instance.SqlServer.cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
            string req = $@"delete from country where id=@Id";
            int res = MySingleton.Instance.SqlServer.ExecuteRequest(req);
            if (res != 0)
            {
                MessageBox.Show("Город успешно удалён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            MySingleton.Instance.SqlServer.cmd.Parameters.Clear();
            UpdateTable();
            delete.IsEnabled = false;
            change.IsEnabled = false;
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
