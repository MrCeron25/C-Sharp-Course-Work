using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class PageCountries : Page
    {
        private void UpdateTable()
        {
            string req = $@"select * from country;";
            DataTable data = Singleton.Instance.SqlServer.Select(req);
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
            Singleton.Instance.SqlServer.cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
            string req = $@"delete from country where id=@Id";
            int res = Singleton.Instance.SqlServer.ExecuteRequest(req);
            if (res != 0)
            {
                MessageBox.Show("Город успешно удалён.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Singleton.Instance.SqlServer.cmd.Parameters.Clear();
            UpdateTable();
            delete.IsEnabled = false;
            change.IsEnabled = false;
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
