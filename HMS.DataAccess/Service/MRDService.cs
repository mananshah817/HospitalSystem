using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using HMS.DataAccess.UnitOfwork;

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

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
