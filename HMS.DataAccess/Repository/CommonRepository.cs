using Dapper;
using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HMS.DataAccess.Repository
{
    public class CommonRepository
    {
        readonly ConnectionFactory _connectionFactory;
        private readonly SqlConnection cnn = new SqlConnection();
        //SqlTransaction _Transaction;
        User _user;
        public CommonRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public CommonRepository(ConnectionFactory connectionFactory, User user)
        {
            _connectionFactory = connectionFactory;
            _user = user;
        }

        public string GetTemplateId(string TemplateName)
        {
            try
            {
                var Query = $"select TemplateId from VENUSHOSP.dbo.SMSTemplate where Template='{TemplateName}'";
                var Conn = _connectionFactory.GetConnection;
                return Conn.QueryFirst<string>(Query, commandType: CommandType.Text);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void SendMail(Mailing Entity)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@p_from", Mailing.From, DbType.String, ParameterDirection.Input);
                dp.Add("@p_to_list", string.Join(";", Entity.To), DbType.String, ParameterDirection.Input);
                dp.Add("@p_to_grp_list", string.Join(";", Entity?.ToGroup), DbType.String, ParameterDirection.Input);
                dp.Add("@p_cc_list", string.Join(";", Entity?.CC), DbType.String, ParameterDirection.Input);
                dp.Add("@p_cc_grp_list", string.Join(";", Entity?.CCGroup), DbType.String, ParameterDirection.Input);
                dp.Add("@p_subject", Entity.Subject, DbType.String, ParameterDirection.Input);
                //dp.Add("@p_subject",  $"{Entity.Subject} [{Environment.MachineName}]", DbType.String, ParameterDirection.Input);
                dp.Add("@p_body", Entity.Body, DbType.String, ParameterDirection.Input);
                dp.Add("@p_attachment", Entity.Attachment, DbType.String, ParameterDirection.Input);
                dp.Add("@p_mail_typ", Entity.Type == MailType.Text ? "TEXT" : "HTML", DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("VENUSHOSP.dbo.send_mail", dp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetQuery<T>(string Type)
        {
            try
            {
                var Dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", null, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<T>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetQuery<T>(string Type, string Pval1)
        {
            try
            {
                var Dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", Pval1, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", null, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<T>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetQuery<T>(string Type, string Pval1, string Pval2, string Pval3)
        {
            try
            {
                var Dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", Pval1, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", Pval2, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", Pval3, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<T>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<ListOfItem> GetMasterGroupData(string Type, string Search)
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                var List = Connection.Query<ListOfItem>("select * from VENUSHOSP.dbo.GetMasterGroupData(@Type,@Search)", new { Type, Search }, commandType: CommandType.Text);
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<MenuDetail> GetMenu(string formName = null)
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                var List = Connection.Query<MenuDetail>("select * from dbo.GetMenu(@user,@FormName)", new { User = _user.UserName, formName }, commandType: CommandType.Text);
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> WEBPACEDATA2019_Get_Query<T>(string Type, string Pval1, string Pval2, string Pval3, string Pval4, string Pval5, string Pval6)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", CheckNull(Pval1), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", CheckNull(Pval2), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", CheckNull(Pval3), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_4", CheckNull(Pval4), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_5", CheckNull(Pval5), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_6", CheckNull(Pval6), DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                return Conn.Query<T>("WEBPACEDATA2019.dbo.Get_Query", dp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable WEBPACEDATA2019_Get_Query(string p_type, string p_val_1, string p_val_2, string p_val_3, string p_val_4, string p_val_5, string p_val_6)
        {
            try
            {
                var dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", p_type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", CheckNull(p_val_1), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", CheckNull(p_val_2), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", CheckNull(p_val_3), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_4", CheckNull(p_val_4), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_5", CheckNull(p_val_5), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_6", CheckNull(p_val_6), DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                var result = Conn.ExecuteReader("WEBPACEDATA2019.dbo.Get_Query", dp, commandType: CommandType.StoredProcedure);

                dt.Load(result);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cnn != null) cnn.Close();
            }
        }

        public IEnumerable<T> VHPHARMA_Get_Query<T>(string Type, string Pval1, string Pval2, string Pval3, string Pval4, string Pval5, string Pval6)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", CheckNull(Pval1), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", CheckNull(Pval2), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", CheckNull(Pval3), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_4", CheckNull(Pval4), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_5", CheckNull(Pval5), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_6", CheckNull(Pval6), DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                return Conn.Query<T>("VHPHARMA.dbo.Get_Query", dp, commandType: CommandType.StoredProcedure);
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

        public IEnumerable<T> VENUSHOSP_GetQuery<T>(string Type, string Pval1, string Pval2, string Pval3)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", CheckNull(Pval1), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", CheckNull(Pval2), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", CheckNull(Pval3), DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                return Conn.Query<T>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable VENUSHOSP_GetQuery(string p_type, string p_val_1, string p_val_2, string p_val_3)
        {
            try
            {
                var dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", p_type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", CheckNull(p_val_1), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", CheckNull(p_val_2), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", CheckNull(p_val_3), DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                var result = Conn.ExecuteReader("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);

                dt.Load(result);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cnn != null) cnn.Close();
            }
        }
        public DataTable WEBPACEPAYROLL_GetQuery(string p_type, string p_val_1, string p_val_2, string p_val_3)
        {
            try
            {
                var dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", p_type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", CheckNull(p_val_1), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", CheckNull(p_val_2), DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", CheckNull(p_val_3), DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                var result = Conn.ExecuteReader("WEBPACEPAYROLL.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);

                dt.Load(result);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cnn != null) cnn.Close();
            }
        }
    }
}
