using Dapper;
using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace HMS.DataAccess.Repository
{
    public class UserRepository
    {
        readonly ConnectionFactory _connectionFactory;
        private readonly SqlConnection cnn = new SqlConnection();
        private readonly SqlCommand cmd = new SqlCommand();
        private readonly SqlDataAdapter adp = new SqlDataAdapter();
        readonly User _User;
        readonly CommonRepository Common;
        public UserRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public UserRepository(ConnectionFactory connectionFactory, User user)
        {
            _connectionFactory = connectionFactory;
            _User = user;
            Common = new CommonRepository(_connectionFactory, user);
        }
        public bool IsValidUser(User entity)
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                var result = Connection.QueryFirst<string>("select WEBMASTER.dbo.IsValidUser(@UserName,@Password)", new { entity.UserName, entity.Password }, commandType: CommandType.Text);
                return result == "Y";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConvertingstringToHexa(string input, Encoding encoding)
        {
            byte[] stringbytes = encoding.GetBytes(input);
            StringBuilder sbbytes = new StringBuilder(stringbytes.Length * 2);
            foreach (byte b in stringbytes)
            {
                sbbytes.AppendFormat("{0:X2}", b);
            }
            return sbbytes.ToString();
        }

        public bool IsMenuAllowed(User entity, string Menu)
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                var result = Connection.QueryFirst<string>("select venushosp.dbo.IsMenuAllowed(@User,@Menu)", new { User = entity.UserName, Menu }, commandType: CommandType.Text);
                return result == "Y";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
