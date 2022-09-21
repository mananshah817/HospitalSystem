using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using HMS.DataAccess.UnitOfwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HMS.DataAccess.Service
{
    public class CommonService : IDisposable
    {
        User User { get; }
        UnitOfWork DbContext { get; }
        public CommonService()
        {
            DbContext = new UnitOfWork(new ConnectionFactory());
        }
        public CommonService(Database database)
        {
            DbContext = new UnitOfWork(new ConnectionFactory(database));
        }
        public CommonService(User user)
        {
            DbContext = new UnitOfWork(new ConnectionFactory(Database.VEN), user);
        }

        public void SendMail(Mailing Entity)
        {
            try
            {
                DbContext.Common.SendMail(Entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}
