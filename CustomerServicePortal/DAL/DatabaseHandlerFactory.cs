using System.Configuration;

namespace CustomerServicePortal.DAL
{
    public class DatabaseHandlerFactory
    {
        private ConnectionStringSettings connectionStringSettings;

        public DatabaseHandlerFactory(string connectionStringName)
        {
            //ConfigurationManager.ConnectionStrings["CustomerServicePortal"].ConnectionString;
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        public IDatabaseHandler CreateDatabase()
        {
            IDatabaseHandler database = null;

            database = new SqlDataAccess(connectionStringSettings.ConnectionString);

            return database;
        }

        public string GetProviderName()
        {
            return connectionStringSettings.ProviderName;
        }
    }
}