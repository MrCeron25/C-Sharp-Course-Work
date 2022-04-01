namespace WpfApp1
{
    public class SqlConfig
    {
        public string DataSource { get; set; }

        public string DataBaseName { get; set; }

        public bool IntegratedSecurity { get; set; }

        public SqlConfig(string dataSource, string dataBaseName, bool integratedSecurity = true)
        {
            DataSource = dataSource;
            DataBaseName = dataBaseName;
            IntegratedSecurity = integratedSecurity;
        }
    }
}
