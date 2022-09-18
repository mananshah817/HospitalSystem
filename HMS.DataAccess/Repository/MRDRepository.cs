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

        private DataTable GetQuery(string Type, string Pval1, string Pval2, string Pval3)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", Pval1, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", Pval2, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", Pval3, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.ExecuteReader("WEBMASTER.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);

                dt = new DataTable();
                dt.Load(Result);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region MRD Repository
        public PaceDetail GetPaceDetail(string IpdOpdNo, string Flag)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@IpdOpdNo", IpdOpdNo, DbType.String, ParameterDirection.Input);
                dp.Add("@Flag", Flag, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                var List = Conn.Query<PaceDetail>("WEBMASTER.dbo.GetPatientDetailFromPace", dp, commandType: CommandType.StoredProcedure);
                return List.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                    throw new Exception($"No Patient for {IpdOpdNo} found.");
            }
            return null;
        }

        public IEnumerable<DocumentDetail> GetDocuments()
        {
            return GetQuery("GET_MRD_DOCS", string.Empty, string.Empty, string.Empty).ToList<DocumentDetail>();
        }

        public IEnumerable<ListOfItem> GetPatientCategory()
        {
            return GetQuery("PATIENT_CATEGORY", string.Empty, string.Empty, string.Empty).ToList<ListOfItem>();
        }

        public IEnumerable<MrdDetail> GetMRDList(string Limit, string IpdNo)
        {
            var List = GetQuery("GET_MRD_LIST", Limit, IpdNo, null).ToList<MrdDetail>();
            List.ForEach(x =>
            {
                var Flag = string.IsNullOrEmpty(x.IpdNo) ? "OPDNO" : "IPDNO";
                x.Pace = GetPaceDetail(x.IpdOpdNo, Flag);
            });
            return List;
        }

        public MrdDetail AddMrdDetail(MrdDetail Entity, User User)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@DetailId", Entity.DetailId, DbType.Int64, ParameterDirection.Input);
                dp.Add("@IpdNo", Entity.IpdNo, DbType.String, ParameterDirection.Input);
                dp.Add("@OpdNo", Entity.OpdNo, DbType.String, ParameterDirection.Input);

                dp.Add("@CaseType", Entity.CaseType, DbType.String, ParameterDirection.Input);
                dp.Add("@DockNo", Entity.DockNo, DbType.String, ParameterDirection.Input);

                dp.Add("@CategoryId", Entity.Category?.Id, DbType.Int32, ParameterDirection.Input);
                dp.Add("@UserName", User.UserName, DbType.String, ParameterDirection.Input);
                dp.Add("@O_DetailId", null, DbType.Int64, ParameterDirection.Output);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("dbo.PopMrd", dp, commandType: CommandType.StoredProcedure);
                Entity.DetailId = dp.Get<long>("@O_DetailId");

                return Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AutoMrd(string IpdNo, string Description, string Path)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@IpdNo", IpdNo, DbType.String, ParameterDirection.Input);
                dp.Add("@Description", Description, DbType.String, ParameterDirection.Input);
                dp.Add("@Path", Path, DbType.String, ParameterDirection.Input);
                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("VENUSHOSP.dbo.AutoMRD", dp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<string> PendingIpd()
        {
            try
            {
                var Dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", "PENDING_MRD", DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", null, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<string>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure, commandTimeout: 0);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<int> GetBatchNo(string IpdNo)
        {
            try
            {
                var Dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", "GET_BATCHNO_FROM_IPD", DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", IpdNo, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", null, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<int>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure, commandTimeout: 0);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<PendingReport> ListPendingReport()
        {

            var IpdList = PendingIpd().Select(x => new PendingReport { IpdNo = x }).ToList();
            foreach (var item in IpdList)
            {
                item.BatchNo = GetBatchNo(item.IpdNo);
                yield return item;
            }
        }

        public IEnumerable<int> PendingCovidReport()
        {
            try
            {
                var Dt = new DataTable();
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", "COVID_OPD_LABREORT", DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", null, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", null, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<int>("VENUSHOSP.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<IPDDetails> SearchPatient(PatientSearchParam Param)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@patientName", Param.Name, DbType.String, ParameterDirection.Input);
                dp.Add("@IdType", (Param.IdType?.Sort != 0) ? Param.IdType?.Name : null, DbType.String, ParameterDirection.Input);
                dp.Add("@IdNo", Param.IdNo, DbType.String, ParameterDirection.Input);
                dp.Add("@MobileNo", Param.MobileNo, DbType.String, ParameterDirection.Input);
                dp.Add("@UhidNo", Param.UHIDNo, DbType.String, ParameterDirection.Input);
                dp.Add("@Ipd", Param.IPD, DbType.String, ParameterDirection.Input);
                dp.Add("@MLC", Param.MLCNo, DbType.String, ParameterDirection.Input);
                dp.Add("@AdmissionFromDate", Param.AdmissionFromDate, DbType.Date, ParameterDirection.Input);
                dp.Add("@AdmissionToDate", Param.AdmissionToDate, DbType.Date, ParameterDirection.Input);
                dp.Add("@DischargeFromDate", Param.DischhargeFromDate, DbType.Date, ParameterDirection.Input);
                dp.Add("@DischargeToDate", Param.DischhargeToDate, DbType.Date, ParameterDirection.Input);
                dp.Add("@FromAge", Param.FromAge, DbType.Int32, ParameterDirection.Input);
                dp.Add("@ToAge", Param.ToAge, DbType.Int32, ParameterDirection.Input);
                dp.Add("@RefDoctorId", Param.ReferenceDoctor?.Id, DbType.Int32, ParameterDirection.Input);
                dp.Add("@ConsultantDoc", Param.ConsultantDoctor?.Id, DbType.Int32, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;

                var Result = Conn.Query<IPDDetails>("VENUSHOSP.dbo.SearchPatient", dp, commandType: CommandType.StoredProcedure);


                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckifDiagnosisMatches(string IpdNo, string SearchText)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@IpdNo", IpdNo, DbType.String, ParameterDirection.Input);
                dp.Add("@searchtext", SearchText, DbType.String, ParameterDirection.Input);
                dp.Add("@result", null, DbType.Int32, ParameterDirection.Output);
                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("WEBPACEDATA2019.dbo.CheckifDiagnosisMatches", dp, commandType: CommandType.StoredProcedure);
                return dp.Get<int>("@result") == 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<ListOfItem> GetTypeOf(string Type, string Filter)
        {
            try
            {
                return GetQuery(Type, Filter, null, null).ToList<ListOfItem>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DocumentDetail AddYojnaDocument(DocumentDetail Entity, User User)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@DocumentTranId", Entity.DocumentTranId, DbType.Int64, ParameterDirection.Input);
                dp.Add("@DetailId", Entity.DetailId, DbType.Int64, ParameterDirection.Input);
                dp.Add("@DocumentId", Entity.DocumentId, DbType.Int64, ParameterDirection.Input);

                dp.Add("@Path", Entity.Path, DbType.String, ParameterDirection.Input);
                dp.Add("@Remark", Entity.Remark, DbType.String, ParameterDirection.Input);
                if (Entity.CheckList != null)
                    dp.Add("@IsCheckList", Entity.CheckList, DbType.String, ParameterDirection.Input);
                else
                    dp.Add("@IsCheckList", Entity.IsChecked ? "Y" : "N", DbType.String, ParameterDirection.Input);

                dp.Add("@UserName", User.UserName, DbType.String, ParameterDirection.Input);
                dp.Add("@O_DocumentTranId", null, DbType.Int64, ParameterDirection.Output);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("dbo.PopYojanaDocument", dp, commandType: CommandType.StoredProcedure);
                Entity.DocumentTranId = dp.Get<long>("@O_DocumentTranId");

                return Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DocumentDetail AddYojnaDocumentExt(DocumentDetail Entity, User User)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@DocumentExtTranId", Entity.DocumentExtTranId, DbType.Int64, ParameterDirection.Input);
                dp.Add("@DetailId", Entity.DetailId, DbType.Int64, ParameterDirection.Input);
                dp.Add("@Description", Entity.Description, DbType.String, ParameterDirection.Input);
                dp.Add("@Flag", Entity.Flag, DbType.String, ParameterDirection.Input);

                dp.Add("@Path", Entity.Path, DbType.String, ParameterDirection.Input);
                dp.Add("@Remark", Entity.Remark, DbType.String, ParameterDirection.Input);

                dp.Add("@UserName", User.UserName, DbType.String, ParameterDirection.Input);
                dp.Add("@O_DocumentExtTranId", null, DbType.Int64, ParameterDirection.Output);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("dbo.PopYojanaDocumentExt", dp, commandType: CommandType.StoredProcedure);
                Entity.DocumentExtTranId = dp.Get<long>("@O_DocumentExtTranId");

                return Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletePatient(YojanaTransaction Entity, User user)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@DetailId", Entity.DetailId, DbType.Int64, ParameterDirection.Input);
                dp.Add("@UserName", user.UserName, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                Conn.Execute("dbo.DeletePatientDetail", dp, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsDeleteAllowed(User entity)
        {
            try
            {
                var Connection = _connectionFactory.GetConnection;
                var result = Connection.QueryFirst<string>("select dbo.IsDeleteAllowed(@UserName)", new { entity.UserName }, commandType: CommandType.Text);
                return result == "Y";
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<DocumentDetail> GetSurgeryDocuments(string SurgeryId, string Type)
        {
            return GetQuery("GET_SRGY_DTL_LST", SurgeryId, Type, string.Empty).ToList<DocumentDetail>();
        }

        public IEnumerable<DocumentDetail> GetDocuments(string TransactionId, string Flag)
        {
            var List = GetQuery("GET_DOCUMENT_LIST", TransactionId, Flag, string.Empty).ToList<DocumentDetail>();
            List.ForEach(item => {
                item.PopulateFileName();
            });
            return List;
        }

        #endregion

        #region Medical Record Repository
        private DataTable GetQuery(string Type, string Pval1, string Pval2, string Pval3, string Pval4, string Pval5, string Pval6)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("@P_TYPE", Type, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_1", Pval1, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_2", Pval2, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", Pval3, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", Pval4, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", Pval5, DbType.String, ParameterDirection.Input);
                dp.Add("@P_VAL_3", Pval6, DbType.String, ParameterDirection.Input);

                var Conn = _connectionFactory.GetConnection;
                var Result = Conn.ExecuteReader("WEBMASTER.dbo.GetQuery", dp, commandType: CommandType.StoredProcedure);
                dt = new DataTable();
                dt.Load(Result);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                dp.Add("@Category", Entity.Category, DbType.String, ParameterDirection.Input);

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
