using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class SoldTickets : Page
    {
        private void UpdateStatistics()
        {
            string request = $@"EXECUTE GetStatisticsOnSoldTickets;";
            SqlCommand command = SqlServer.Instance.CreateSqlCommand(request);

            DataTable data = SqlServer.Instance.Select(command);
            if (data != null && data.Rows.Count > 0)
            {
                dataGrid.ItemsSource = data.DefaultView;
                save.IsEnabled = true;
            }
            else
            {
                save.IsEnabled = false;
            }
        }

        public SoldTickets()
        {
            InitializeComponent();
            UpdateStatistics();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.Instance.MainFrame.CanGoBack)
            {
                Manager.Instance.MainFrame.GoBack();
            }
        }
        
        private void SaveStatistics()
        {
            string homeDirectory = "C:\\Users\\ARTEM\\Desktop\\КОРОНОВИРУС\\21-22\\БД\\6 семестр\\курсовая\\WpfApp1\\WpfApp1\\";
            string folderName = homeDirectory + "Statistics";
            try
            {
                string data = "";
                DataView view = (DataView)dataGrid.ItemsSource;
                foreach (DataRowView rowView in view)
                {
                    //Console.WriteLine(rowView.Row.ItemArray);
                    foreach (object item in rowView.Row.ItemArray)
                    {
                        data += $@"{item, 25} ";
                    }
                    data += "\n";
                    Saver.Save(folderName,
                               "Statistics",
                               data);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка :\n{e.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Thread newThread = new Thread(SaveStatistics);
            newThread.Start();
        }
    }
}
