using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;

namespace HMS.DataAccess.Entity
{
    public class MrdDetail
    {
        public long DetailId { get; set; }
        public long YojanaId { get; set; }
        public string IpdNo { get; set; }
        public string OpdNo { get; set; }
        public string IpdOpdNo { get; set; }
        public string CaseType { get; set; }
        public string DockNo { get; set; }
        public PaceDetail Pace { get; set; }
        [ColumnName("CategoryId", "Name")]
        public ListOfItem Category { get; set; }
        public List<DocumentDetail> Documents { get; set; }
        public string Folder { get; set; }
        public DateTime SYSDate { get; set; }
        public string UserName { get; set; }

    }
    public class PaceDetail
    {
        public string PatientName { get; set; }
        public string UHIDNo { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string MLCNo { get; set; }
    }
    public class MrdDcoument
    {
        public long DetailId { get; set; }
        public int DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string Description { get; set; }
        public string Prefix { get; set; }
        public string Flag { get; set; }
        public string IsRequired { get; set; }
        public string DocumentType { get; set; }
        public int Sort { get; set; }
        public int GroupSort { get; set; }
        public DataRowState RowState { get; set; } = DataRowState.Unchanged;
        public int DocType { get; set; }
        public Document Document
        {
            get { return DocType == 200 ? Document.Additional : Document.Predefined; }
            set { DocType = value == Document.Predefined ? 100 : 200; }
        }
    }
    public class PatientSearchParam
    {
        public string Name { get; set; }
        public ListOfItem IdType { get; set; }
        public string IdNo { get; set; }
        public string MobileNo { get; set; }
        public string UHIDNo { get; set; }
        public string IPD { get; set; }
        public string MLCNo { get; set; }
        public DateTime? AdmissionFromDate { get; set; }
        public DateTime? AdmissionToDate { get; set; }
        public DateTime? DischhargeFromDate { get; set; }
        public DateTime? DischhargeToDate { get; set; }
        public int? FromAge { get; set; }
        public int? ToAge { get; set; }
        public ListOfItem ConsultantDoctor { get; set; }
        public ListOfItem ReferenceDoctor { get; set; }
        public string Diagnosis { get; set; }

    }
    public class IPDDetails
    {
        public string UHID { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string IPDNo { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string ConsultantDoctor { get; set; }
        public string ReferenceDoctor { get; set; }
        public string Organization { get; set; }
        public string CardNo { get; set; }
        public string CaseNo { get; set; }
        public string MLCNo { get; set; }
        public string Mediclaim { get; set; }
        public string Witness { get; set; }
        public string Diagnosis { get; set; }
        public string Url
        {
            get
            {
                return $@"/Home/MRD?IpdNo={IPDNo}";
            }
        }
        public string AdmissionDateDisplay
        {
            get
            {
                return AdmissionDate?.ToShortDateString() ?? null;
            }
        }
        public string DischargeDateDisplay
        {
            get
            {
                return DischargeDate?.ToShortDateString() ?? null;
            }
        }
    }
    public class PendingReport
    {
        public string IpdNo { get; set; }
        public IEnumerable<int> BatchNo { get; set; }

    }

    public class DocMaster
    {
        public int DocMstId { get; set; }
        public string IPDNo { get; set; }
        public string OPDNo { get; set; }
        public string Category { get; set; }
        public string DocType { get; set; }
        public string UserName { get; set; }
        public DateTime? SysDate { get; set; }
        public string IP { get; set; }
        public List<DocDetail> Detail { get; set; }
        public DataRowState RowState { get; set; } = DataRowState.Unchanged;
        public string Action
        {
            get
            {
                switch (RowState)
                {
                    case DataRowState.Added:
                        return "INSERT";
                    case DataRowState.Deleted:
                        return "DELETE";
                    case DataRowState.Modified:
                        return "UPDATE";
                    default:
                        return string.Empty;
                }
            }
        }
        public string GridName { get; set; } = "DocMaster";
        public bool IsValid { get; set; } = true;
    }

    public class DocDetail
    {
        public int DocDetailId { get; set; }
        public int DocMstId { get; set; }
        public string DocPath { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }
        public DateTime? SysDate { get; set; }
        public string IP { get; set; }
        public DataRowState RowState { get; set; } = DataRowState.Unchanged;
        public string Action
        {
            get
            {
                switch (RowState)
                {
                    case DataRowState.Added:
                        return "INSERT";
                    case DataRowState.Deleted:
                        return "DELETE";
                    case DataRowState.Modified:
                        return "UPDATE";
                    default:
                        return string.Empty;
                }
            }
        }
        public string GridName { get; set; } = "DocDetail";
        public bool IsValid { get; set; } = true;
    }
}
