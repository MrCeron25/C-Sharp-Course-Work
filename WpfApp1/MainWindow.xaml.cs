using System.Windows;

namespace WpfApp1
{
    public class Singleton
    {
        private static Singleton _INSTANCE;
        public static Singleton Instance
        {
            get
            {
                if (_INSTANCE == null)
                {
                    _INSTANCE = new Singleton();
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
            Singleton.Instance.SqlServer = new SqlServer(new SqlConfig(@".\SQLEXPRESS", "course_work", true));
            Singleton.Instance.MainWindow = this;
            Singleton.Instance.MainWindow.main.Navigate(new MainPage());
        }
    }
}
