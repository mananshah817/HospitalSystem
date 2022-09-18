using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using HMS.DataAccess.UnitOfwork;
using System.Linq;
using System.Data;
using System.Net;

namespace HMS.DataAccess.Service
{
    public class MRDService : IDisposable
    {
        User User { get; }
        UnitOfWork DbContext { get; }
        public MRDService(User user)
        {
            User = user;
            DbContext = new UnitOfWork(new ConnectionFactory(Database.VEN), user);
        }
        public IEnumerable<IPDDetails> SearchPatient(PatientSearchParam Param)
        {
            var List = DbContext.MRD.SearchPatient(Param);
            foreach (var item in List)
            {
                if (string.IsNullOrEmpty(Param.Diagnosis))
                    yield return item;
                else if (DbContext.MRD.CheckifDiagnosisMatches(item.IPDNo, Param.Diagnosis))
                    yield return item;
            }
        }
        public IEnumerable<ListOfItem> GetIDType(string Type, string Filter)
        {
            try
            {
                return DbContext.MRD.GetTypeOf(Type, Filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Medical Record Data
        public IEnumerable<DocMaster> GetDocMaster(string IPDNo, string OPDNo)
        {
            try
            {
                var List = DbContext.MRD.GetDocMaster(IPDNo, OPDNo).ToList();
                var Detail = GetDocDetail();
                List.ForEach(item =>
                {
                    //item.FormateData();
                    item.Detail = Detail.Where(x => x.DocMstId == item.DocMstId).ToList();
                });
                return List.OrderBy(x => x.DocType);
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
                var List = DbContext.MRD.GetDocDetail().ToList();
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ServiceResponse<List<DocMaster>> PopDocMaster(List<DocMaster> Entity)
        {
            try
            {
                DbContext.MRD.BeginTransaction();
                Entity.Where(x => x.RowState == DataRowState.Modified && x.DocMstId == 0).ToList().ForEach(x =>
                {
                    x.RowState = DataRowState.Added;
                });
                var UnchangedData = Entity.Where(e => e.RowState.In(DataRowState.Unchanged));
                var LeftData = UnchangedData.Where(e => e.Detail.Any(x => x.RowState.In(DataRowState.Added, DataRowState.Modified, DataRowState.Deleted)))
                                            .SelectMany(x => x.Detail).ToList();//Detail Updated but not master.
                PopDocDetail(LeftData);
                foreach (var item in Entity.Except(UnchangedData))
                {
                    DbContext.MRD.PopDocMaster(item);
                    item.Detail.ForEach(x =>
                    {
                        x.DocMstId = item.DocMstId;
                    });
                    PopDocDetail(item.Detail);
                }
                DbContext.MRD.CommitTransaction();
                return new ServiceResponse<List<DocMaster>>
                {
                    type = "info",
                    code = Infrastructure.Type.Success,
                    body = "Doc Master saved successfuly.",
                    httpStatus = HttpStatusCode.OK,
                    Data = Entity
                };
            }
            catch (Exception ex)
            {
                DbContext.MRD.RollbackTransaction();
                return ex.GetTrace(Entity);
            }
        }
        public ServiceResponse<List<DocDetail>> PopDocDetail(List<DocDetail> Entity)
        {
            try
            {
                Entity.Where(x => x.RowState == DataRowState.Modified && x.DocDetailId == 0).ToList().ForEach(x =>
                {
                    x.RowState = DataRowState.Added;
                });

                var ModifiedData = Entity.Where(e => e.RowState.NotIn(DataRowState.Unchanged));
                foreach (var item in ModifiedData)
                {
                    DbContext.MRD.PopDocDetail(item);
                }
                return new ServiceResponse<List<DocDetail>>
                {
                    type = "info",
                    code = Infrastructure.Type.Success,
                    body = "Doc Details saved successfuly.",
                    httpStatus = HttpStatusCode.OK,
                    Data = Entity
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
