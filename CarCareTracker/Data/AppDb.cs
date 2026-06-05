using System.Configuration;
using System.Data.SqlClient;

namespace CarCareTracker.Data
{
    /// <summary>
    /// Central helper for opening SQL Server connections.
    /// Data Access Layer foundation (3-Tier Architecture).
    /// Uses SQL Server (System.Data.SqlClient).
    /// </summary>
    public static class AppDb
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
