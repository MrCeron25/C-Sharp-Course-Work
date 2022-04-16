using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class SubWindowCitiesAdd : Window
    {
        public SubWindowCitiesAdd()
        {
            InitializeComponent();
        }

        private void CheckEnabledButton()
        {
            if (ComboBox.SelectedItem != null)
            {
                if (Tools.CheckStrings(it => !string.IsNullOrEmpty(it),
                                       textBox.Text,
                                       ComboBox.SelectedItem.ToString()))
                {
                    Add.IsEnabled = true;
                }
                else
                {
                    Add.IsEnabled = false;
                }
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckEnabledButton();
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckEnabledButton();
        }
    }
}
