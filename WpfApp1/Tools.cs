using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public class Tools
    {
        public static bool CheckStrings(Func<string, bool> func, params string[] arr)
        {
            HashSet<bool> bools = new HashSet<bool>();
            foreach (string item in arr)
            {
                bools.Add(func.Invoke(item));
            }
            return bools.All(it => it);
        }

        public static void TextBoxChangeColors(TextBox textBox, BrushConverter converter)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.BorderBrush = (Brush)converter.ConvertFromString("#FFFF0000");
                textBox.SelectionBrush = (Brush)converter.ConvertFromString("#FFFF0000");
            }
            else
            {
                textBox.BorderBrush = (Brush)converter.ConvertFromString("#FFABADB3");
                textBox.SelectionBrush = (Brush)converter.ConvertFromString("#FF0078D7");
            }
        }

        public static void PasswordBoxChangeColors(PasswordBox passwordBox, BrushConverter converter)
        {
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                passwordBox.BorderBrush = (Brush)converter.ConvertFromString("#FFFF0000");
                passwordBox.SelectionBrush = (Brush)converter.ConvertFromString("#FFFF0000");
            }
            else
            {
                passwordBox.BorderBrush = (Brush)converter.ConvertFromString("#FFABADB3");
                passwordBox.SelectionBrush = (Brush)converter.ConvertFromString("#FF0078D7");
            }
        }
    }
}
