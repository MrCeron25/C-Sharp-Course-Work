using System;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace WpfApp1
{
    public class SqlServer
    {

        private static SqlServer _INSTANCE;
        public static SqlServer Instance
        {
            get
            {
                if (_INSTANCE == null)
                {
                    _INSTANCE = new SqlServer(new SqlConfig(@".\SQLEXPRESS", "course_work", true));
                }
                return _INSTANCE;
            }
        }

        private static SqlConfig sqlConfig;
        private static SqlConnection connection;

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
        private void ShowError(string error)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Выборка данных (SELECT/PROC/FUNC)
        /// </summary>
        /// <param name="command">SqlCommand c запросом и подключением</param>
        /// <returns>DataTable с данными</returns>
        public DataTable Select(SqlCommand command)
        {
            DataTable Table = new DataTable();
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
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
        /// <param name="command">SqlCommand c запросом и подключением</param>
        /// <returns>Количество затронутых строк</returns>
        public int ExecuteRequest(SqlCommand command)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            return affectedRows;
        }

        public SqlCommand CreateSqlCommand(string command)
        {
            return new SqlCommand
            {
                Connection = connection,
                CommandText = command
            };
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
