using System;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace WpfApp1
{
    public class SqlServer
    {
        private static SqlConfig sqlConfig;
        private static SqlConnection connection;
        private static SqlDataAdapter dataAdapter;
        public SqlCommand cmd;

        /// <summary>
        /// Констроктор
        /// </summary>
        /// <param name="config">Конфиг с настройками</param>
        public SqlServer(SqlConfig config)
        {
            sqlConfig = config;
        }

        /// <summary>
        /// Вызов окна с ошибкой
        /// </summary>
        /// <param name="error">Текст ошибки</param>
        private static void ShowError(string error)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Выборка данных (SELECT/PROC/FUNC)
        /// </summary>
        /// <param name="sqlExpression">Строка запроса</param>
        /// <returns>Таблица с данными</returns>
        public DataTable Select(string sqlExpression)
        {
            DataTable Table = new DataTable();
            try
            {
                cmd.CommandText = sqlExpression;
                dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(Table);
            }
            catch (Exception ex)
            {
                ShowError($"{ex.Message}");
            }
            return Table;
        }

        /// <summary>
        /// Выполнить sql запрос (подходит для Update/Delete/Insert/PROC/FUNC) 
        /// </summary>
        /// <param name="request">Строка запроса</param>
        /// <returns>Возвращяет количесво изменных строк</returns>
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

        /// <summary>
        /// Открытие подключения
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                connection = new SqlConnection($@"
                    Data Source={sqlConfig.DataSource};
                    Initial Catalog={sqlConfig.DataBaseName};
                    Integrated Security={sqlConfig.IntegratedSecurity};");
                cmd = new SqlCommand
                {
                    Connection = connection
                };
                connection.Open(); // Открываем подключение
            }
            catch (Exception ex)
            {
                ShowError($"{ex.Message}");
            }
        }

        /// <summary>
        /// Закрытие подключения
        /// </summary>
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
