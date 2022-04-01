using System.Windows;

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

        public SqlServer SqlServer;
        public MainWindow MainWindow;
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MySingleton.Instance.SqlServer = new SqlServer(new SqlConfig(@".\SQLEXPRESS", "course_work", true));
            MySingleton.Instance.MainWindow = this;
            MySingleton.Instance.MainWindow.main.Navigate(new MainPage());
        }
    }
}
