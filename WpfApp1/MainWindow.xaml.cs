using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Data.SqlClient;

namespace WpfApp1
{
    public class MySingleton
    {
        private static MySingleton _INSTANCE;
        public static MySingleton Instance
        {
            get
            {
                if (_INSTANCE == null)
                {
                    _INSTANCE = new MySingleton();
                }
                return _INSTANCE;
            }
        }

        public SqlServer SqlServer { get; set; }
        public MainWindow MainWindow { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SqlServer sqlServer;
        public MainWindow()
        {
            InitializeComponent();
            MySingleton.Instance.SqlServer = new SqlServer(new SqlConfig(@".\SQLEXPRESS", "course_work", true));
            MySingleton.Instance.MainWindow = this;
            MySingleton.Instance.MainWindow.main.Navigate(new MainPage());
        }
    }
    // план салона визуализировать
}

