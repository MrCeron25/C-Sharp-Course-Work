﻿using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class SubWindowAirplaneChange : Window
    {
        public SubWindowAirplaneChange()
        {
            InitializeComponent();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                action.IsEnabled = false;
            }
            else if (action != null && !action.IsEnabled)
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
