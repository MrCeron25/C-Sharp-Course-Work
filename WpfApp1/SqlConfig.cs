namespace WpfApp1
{
    public class SqlConfig
    {
        public string DataSource;

        public string DataBaseName;

        public bool IntegratedSecurity;

        public SqlConfig(string dataSource, string dataBaseName, bool integratedSecurity = true)
        {
            DataSource = dataSource;
            DataBaseName = dataBaseName;
            IntegratedSecurity = integratedSecurity;
        }
    }
}
