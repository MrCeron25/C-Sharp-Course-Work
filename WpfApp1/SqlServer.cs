using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace WpfApp1
{
    public class SqlTable
    {
        public int CountRows => Data.Count;
        public List<string> ColumnNames;
        public List<List<string>> Data;

        public SqlTable()
        {
            ColumnNames = new List<string>();
            Data = new List<List<string>>();
        }

        private List<int> MaxLenColumns()
        {
            List<int> res = new List<int>();
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                res.Add(ColumnNames[i].Length);
            }
            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = 0; j < Data[i].Count; j++)
                {
                    if (Data[i][j].Length > res[j])
                    {
                        res[j] = Data[i][j].Length;
                    }
                }
            }
            return res;
        }

        private string GetHeader(List<int> MaxLenColumns)
        {
            string res = "|";
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                res += $"{Tools.GetSym(Convert.ToUInt32(MaxLenColumns[i] - ColumnNames[i].Length), ' ')}{ColumnNames[i]}|";
            }
            return res;
        }

        public override string ToString()
        {
            string res = "";
            List<int> lens = MaxLenColumns();
            int lenRow = 0;
            for (int i = 0; i < lens.Count; i++)
            {
                lenRow += lens[i];
            }
            lenRow += ColumnNames.Count + 1;

            res += $"{Tools.GetSym(Convert.ToUInt32(lenRow), '=')}\n";
            res += $"{GetHeader(lens)}\n";
            res += $"{Tools.GetSym(Convert.ToUInt32(lenRow), '=')}\n";
            for (int i = 0; i < Data.Count; i++)
            {
                res += "|";
                for (int j = 0; j < Data[i].Count; j++)
                {
                    res += $"{Tools.GetSym(Convert.ToUInt32(lens[j] - Data[i][j].Length), ' ')}{Data[i][j]}|";
                }
                res += "\n";
            }
            res += $"{Tools.GetSym(Convert.ToUInt32(lenRow), '=')}\n";
            return res;
        }
    }

    public class SqlServer
    {
        private static SqlConfig sqlConfig;
        private static SqlConnection connection;
        public SqlCommand cmd;

        public SqlServer(SqlConfig config)
        {
            sqlConfig = config;
        }

        //public static List<OValue> RemapList<TValue, OValue>(List<TValue> inputList, Func<TValue, OValue> mapper)
        //{
        //    var list = new List<OValue> { };

        //    inputList.ForEach(input => { list.Add(mapper.Invoke(input)); });

        //    return list;
        //}
        //RemapList(new List<string> { }, str => int.Parse(str));

        private static void ShowError(string error)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //public List<string> GetColumnsName(string tableName)
        //{
        //    List<string> listColumnName = new List<string>();
        //    try
        //    {
        //        string req = $"select * from {tableName}";
        //        cmd.CommandText = req;
        //        Reader = cmd.ExecuteReader();
        //        for (int i = 0; i < Reader.FieldCount; i++)
        //        {
        //            listColumnName.Add(Reader.GetName(i));
        //            //Console.WriteLine(Reader.GetName(i));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError($"{ex.Message}");
        //    }
        //    return listColumnName;
        //}

        public DataTable GetDataTable(string sqlExpression)
        {
            DataTable Table = new DataTable();
            try
            {
                cmd.CommandText = sqlExpression;
                SqlDataAdapter DataAdapter = new SqlDataAdapter(cmd);
                DataAdapter.Fill(Table);
            }
            catch (Exception ex)
            {
                ShowError($"{ex.Message}");
            }
            return Table;
        }

        //public SqlTable GetDataFromExecute(string sqlExpression)
        //{
        //    SqlTable Table = new SqlTable();
        //    try
        //    {
        //        cmd.CommandText = sqlExpression;
        //        SqlDataReader Reader = cmd.ExecuteReader();
        //        for (int i = 0; i < Reader.FieldCount; i++)
        //        {
        //            Table.ColumnNames.Add(Reader.GetName(i));
        //        }
        //        while (Reader.Read())
        //        {
        //            List<string> row = new List<string>();
        //            for (int i = 0; i < Reader.FieldCount; i++)
        //            {
        //                row.Add(Reader[i].ToString());
        //            }
        //            Table.Data.Add(row);
        //        }
        //        Reader.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError($"{ex.Message}");
        //    }
        //    return Table;
        //}

        //public DataTable GetDataTable(SqlTable tableWithData)
        //{
        //    DataTable dataTable = new DataTable();
        //    for (int i = 0; i < tableWithData.ColumnNames.Count; i++)
        //    {
        //        dataTable.Columns.Add(new DataColumn(tableWithData.ColumnNames[i], typeof(string)));
        //    }
        //    for (int i = 0; i < tableWithData.Data.Count; i++)
        //    {
        //        DataRow newrow = dataTable.NewRow();
        //        for (int j = 0; j < tableWithData.Data[i].Count; j++)
        //        {
        //            newrow[j] = tableWithData.Data[i][j];
        //        }
        //        dataTable.Rows.Add(newrow);
        //    }
        //    return dataTable;
        //}

        public int ExecuteRequest(string request)
        {
            int affectedRows = 0;
            try
            {
                cmd.CommandText = request;
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            return affectedRows;
        }

        public void OpenConnection()
        {
            try
            {
                string connectionString = $@"Data Source={sqlConfig.DataSource};
                                             Initial Catalog={sqlConfig.DataBaseName};
                                             Integrated Security={(sqlConfig.IntegratedSecurity ? "True" : "False")};";
                connection = new SqlConnection(connectionString);
                cmd = new SqlCommand();
                cmd.Connection = connection;
                // Открываем подключение
                connection.Open();
            }
            catch (Exception ex)
            {
                ShowError($"{ex.Message}");
            }
        }

        public void CloseConnection()
        {
            try
            {
                // закрываем подключение
                connection.Close();
                connection.Dispose();
                connection = null;
            }
            catch (Exception ex)
            {
                ShowError($"{ex.Message}");
            }
        }
    }
}
