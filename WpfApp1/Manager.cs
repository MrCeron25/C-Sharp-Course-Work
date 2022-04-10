using System.Windows.Controls;

namespace WpfApp1
{
    public class Manager
    {
        private static Manager _INSTANCE;

        public static Manager Instance
        {
            get
            {
                if (_INSTANCE == null)
                {
                    _INSTANCE = new Manager();
                }
                return _INSTANCE;
            }
        }

        public Frame MainFrame
        {
            get => MainWindow.main;
            set => MainWindow.main = value;
        }
        public MainWindow MainWindow { get; set; }
    }
}
