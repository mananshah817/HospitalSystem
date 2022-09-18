using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using HMS.DataAccess.Service;
using HMS.DataAccess.UnitOfwork;
using HMS.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HMS.APIController
{
    public class HomeController : ApiController
    {
        #region MRD End Point : api/Home/MedicalRecord
        [HttpGet, Route("api/Home/MedicalRecord/{QueryType}")]
        public HttpResponseMessage MRDQuery(string QueryType, [FromHeader] string Pval1, 
                                                              [FromHeader] string Pval2, 
                                                              [FromHeader] string Pval3,
                                                              [FromHeader] string Pval4,
                                                              [FromHeader] string Pval5,
                                                              [FromHeader] string Pval6)
        {
            try
            {
                var User = HttpContext.Current.Session["User"] as User;
                using (var Service = new MRDService(User))
                {
                    switch (QueryType)
                    {
                        //case "DataType":
                        //    var DataType = Service.GetDatatype().Select(x => x.Value);
                        //    return Request.CreateResponse(DataType);
                        case "DocMaster":
                            var DocMaster = Service.GetDocMaster(Pval1, Pval2);
                            return Request.CreateResponse(DocMaster);
                        case "DocDetail":
                            var DocDetail = Service.GetDocDetail();
                            return Request.CreateResponse(DocDetail);
                        default:
                            return Request.CreateResponse();
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("api/Home/MedicalRecord/SaveData")]
        public HttpResponseMessage SaveData(List<DocMaster> Entity)
        {
            UnitOfWork dbContext = new UnitOfWork(new ConnectionFactory(Database.VEN));
            try
            {
                var User = HttpContext.Current.Session["User"] as User;
                using (var Service = new MRDService(User))
                {
                    var ServiceStatus = Service.PopDocMaster(Entity);
                    return Request.CreateResponse(ServiceStatus.httpStatus, ServiceStatus);
                }
            }
            catch (Exception ex)
            {
                var Trace = ex.GetTrace();
                return Request.CreateResponse(Trace.httpStatus, Trace);
            }
        }
        #endregion
    }
}