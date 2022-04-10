using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class SubWindowAirplane : Window
    {
        public SubWindowAirplane()
        {
            InitializeComponent();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            uint number = 0;
            if (string.IsNullOrEmpty(textBoxModel.Text) || string.IsNullOrEmpty(textBoxNumberSeats.Text))
            {
                action.IsEnabled = false;
            }
            else if (action != null && !action.IsEnabled && uint.TryParse(textBoxNumberSeats.Text, out number))
            {
                action.IsEnabled = true;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void action_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
