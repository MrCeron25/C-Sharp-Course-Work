using System.Windows;

namespace WpfApp1
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.Instance.MainWindow = this;
            Manager.Instance.MainFrame.Navigate(new MainPage());
        }
    }
}
