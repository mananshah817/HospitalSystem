using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using HMS.DataAccess.Repository;
using System;
using System.Data.SqlClient;

namespace HMS.DataAccess.UnitOfwork
{
    public class UnitOfWork : IDisposable
    {
        #region Connection 
        public ConnectionFactory _connectionFactory;
        public SqlTransaction _Transaction;
        #endregion

        public UserRepository UserRepo;
        public CommonRepository Common;
        public MRDRepository MRD;
        public bool isProduction;

        public UnitOfWork(ConnectionFactory ConnectionFactory, User user = null)
        {
            _connectionFactory = ConnectionFactory;
            UserRepo = new UserRepository(ConnectionFactory, user);
            Common = new CommonRepository(ConnectionFactory, user);
            MRD = new MRDRepository(ConnectionFactory, user);
            isProduction = ConnectionFactory.IsProduction;
        }
        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _connectionFactory.Dispose();
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
}
