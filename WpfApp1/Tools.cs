using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public class Tools
    {
        public static void ChangeDateFormatInDataTable(DataTable data)
        {
            foreach (DataColumn column in data.Columns)
            {
                if (column.DataType.FullName == "System.DateTime")
                {
                    //(column as DataView).Binding.StringFormat = "dd/MM/yyyy";
                    Console.WriteLine(column);
                    //(column as ).
                    //Console.WriteLine(column.DataType.FullName);
                }
            }

            //foreach (DataColumn column in data.Columns)
            //{
            //    if (column.DataType.FullName == "System.DateTime")
            //    {
            //        (column as DataRow).
            //    }
            //}

            //.Columns["colName"].DefaultCellStyle.Format = "dd/MM/yyyy";
            //data.Columns["WaitTime"]
        }

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
