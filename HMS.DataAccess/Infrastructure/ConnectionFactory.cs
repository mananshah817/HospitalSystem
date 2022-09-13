using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HMS.DataAccess.Infrastructure
{
    public class ConnectionFactory : IDisposable
    {
        public bool IsProduction
        {
            get
            {
                return ConnectionString.Contains("192.168.1.4");
            }
        }
        private readonly string ConnectionString = string.Empty;
        private SqlConnection _connection;
        public ConnectionFactory()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["VEN"].ConnectionString;
        }
        public ConnectionFactory(Database db)
        {
            var DatabaseConnection = string.Empty;
            switch (db)
            {
                case Database.VEN:
                    DatabaseConnection = ConfigurationManager.ConnectionStrings["VEN"].ConnectionString;
                    break;
                default:
                    DatabaseConnection = ConfigurationManager.ConnectionStrings["VEN"].ConnectionString;
                    break;
            }
            ConnectionString = DatabaseConnection;
        }
        public SqlConnection GetConnection
        {
            get
            {
                try
                {
                    if (_connection?.State == ConnectionState.Open)
                        return _connection;
                    else
                    {
                        _connection = new SqlConnection(ConnectionString);
                        _connection.Open();
                        return _connection;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && _connection != null && _connection.State != System.Data.ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                    SqlConnection.ClearPool(_connection);
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public enum Database
    {
        VEN
    }
}
