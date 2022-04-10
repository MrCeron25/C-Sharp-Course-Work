using System.Windows;

namespace WpfApp1
{
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
