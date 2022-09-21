using Dapper;
using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HMS.DataAccess.Repository
{
    public class MRDRepository
    {
        readonly ConnectionFactory _connectionFactory;
        private readonly SqlConnection cnn = new SqlConnection();
        private SqlCommand cmd = new SqlCommand();
        private SqlDataAdapter adp = new SqlDataAdapter();
        User _User;
        DataTable dt;
        private SqlTransaction _Transaction;
        public MRDRepository(ConnectionFactory connectionFactory, User user)
        {
            _connectionFactory = connectionFactory;
            _User = user;
        }

        #region Common Code
        public void BeginTransaction()
        {
            _Transaction = _connectionFactory.GetConnection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _Transaction.Commit();
        }
        public void RollbackTransaction()
        {
            _Transaction.Rollback();
        }
        #endregion

        #region Medical Record Repository
        public IEnumerable<ListOfItem> GetDocDD(string P_TYPE, string P_VAL_1)
        {
            return GetQuery(P_TYPE, P_VAL_1, _User.UserName, string.Empty, string.Empty, string.Empty, string.Empty).ToList<ListOfItem>();
        }

        private DataTable GetQuery(string Type, string Pval1, string Pval2, string Pval3, string Pval4, string Pval5, string Pval6)
        {
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "WEBMASTER.dbo.GetMRDQuery";
                cmd.Connection = _connectionFactory.GetConnection;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("@P_TYPE", SqlDbType.VarChar).Value = Type;
                cmd.Parameters.Add("@P_VAL_1", SqlDbType.VarChar).Value = CheckNull(Pval1);
                cmd.Parameters.Add("@P_VAL_2", SqlDbType.VarChar).Value = CheckNull(Pval2);
                cmd.Parameters.Add("@P_VAL_3", SqlDbType.VarChar).Value = CheckNull(Pval3);
                cmd.Parameters.Add("@P_VAL_4", SqlDbType.VarChar).Value = CheckNull(Pval4);
                cmd.Parameters.Add("@P_VAL_5", SqlDbType.VarChar).Value = CheckNull(Pval5);
                cmd.Parameters.Add("@P_VAL_6", SqlDbType.VarChar).Value = CheckNull(Pval6);

                adp.SelectCommand = cmd;
                dt = new DataTable();
                adp.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object CheckNull(string pstrVal)
        {
            if (string.IsNullOrEmpty(pstrVal)) return DBNull.Value;
            return pstrVal;
        }

        public IEnumerable<DocMaster> GetDocMaster(string P_IPDNo, string P_OPDNo)
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                var List = Connection.Query<DocMaster>("select * from WEBMASTER.dbo.GetDocMaster(@p_ipdno, @p_opdno)", new { P_IPDNo, P_OPDNo }, commandType: CommandType.Text);
                var Detail = GetDocDetail();
                foreach (var item in List)
                {
                    item.Detail = Detail.Where(x => x.DocMstId == item.DocMstId).ToList();
                }
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DocDetail> GetDocDetail()
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                return Connection.Query<DocDetail>("select * from WEBMASTER.dbo.GetDocDetail()", commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DocMaster PopDocMaster(DocMaster Entity)
        {
            try
            {
                var dp = new DynamicParameters();
                
                dp.Add("@Action", Entity.Action, DbType.String, ParameterDirection.Input);
                dp.Add("@IPDNo", Entity?.IPDNo, DbType.String, ParameterDirection.Input);
                dp.Add("@OPDNo", Entity?.OPDNo, DbType.String, ParameterDirection.Input);
                dp.Add("@Category", Entity.Category.Name, DbType.String, ParameterDirection.Input);

                dp.Add("@DocType", Entity.DocType, DbType.String, ParameterDirection.Input);
                dp.Add("@UserName", _User.UserName.ToUpper(), DbType.String, ParameterDirection.Input);
                dp.Add("@IP", Environment.MachineName.ToUpper(), DbType.String, ParameterDirection.Input);
                dp.Add("@DocMstId", Entity.DocMstId, DbType.Int32, ParameterDirection.InputOutput);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("WEBMASTER.dbo.PopDocMaster", dp, _Transaction, commandType: CommandType.StoredProcedure);
                Entity.DocMstId = dp.Get<int>("DocMstId");
                return Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DocDetail PopDocDetail(DocDetail Entity)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@Action", Entity.Action, DbType.String, ParameterDirection.Input);
                dp.Add("@DocMstId", Entity?.DocMstId, DbType.Int32, ParameterDirection.Input);
                dp.Add("@DocPath", Entity?.DocPath, DbType.String, ParameterDirection.Input);
                dp.Add("@Remark", Entity?.Remark, DbType.String, ParameterDirection.Input);

                dp.Add("@UserName", _User.UserName.ToUpper(), DbType.String, ParameterDirection.Input);
                dp.Add("@IP", Environment.MachineName.ToUpper(), DbType.String, ParameterDirection.Input);
                dp.Add("@DocDetailId", Entity.DocDetailId, DbType.Int32, ParameterDirection.InputOutput);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("WEBMASTER.dbo.PopDocDetail", dp, _Transaction, commandType: CommandType.StoredProcedure);
                Entity.DocDetailId = dp.Get<int>("DocDetailId");
                return Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
