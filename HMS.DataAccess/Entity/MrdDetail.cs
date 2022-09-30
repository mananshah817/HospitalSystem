using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace HMS.DataAccess.Entity
{
    public class MrdDetail
    {
        
    }

    public class DocMaster
    {
        public int DocMstId { get; set; }
        public string IPDNo { get; set; }
        public string OPDNo { get; set; }
        public ListOfItem Category { get; set; }
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
        private string CategoryName { get; set; }
        private int CategoryId { get; set; }
        public void FormateData()
        {
            Category = new ListOfItem
            {
                Name = CategoryName
            };
        }

        public Guid GUID { get; set; } = Guid.NewGuid();
        public long DocumentTranId { get; set; }
        public long DocumentExtTranId { get; set; }
        public long DetailId { get; set; }
        public long DocumentId { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
        public string Path { get; set; }
        public string Prefix { get; set; }
        public string IsRequired { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public bool IsFileAdded { get; set; } = false;
        public int DocuType { get; set; }
        public Document Document
        {
            get { return DocuType == 200 ? Document.Additional : Document.Predefined; }
            set { DocuType = value == Document.Predefined ? 100 : 200; }
        }
        public void PopulateFileName()
        {
            FileName = string.IsNullOrEmpty(Path) ? null : new FileInfo(Path).Name;
        }
    }

    public class DocDetail
    {
        public int DocDetailId { get; set; }
        public int DocMstId { get; set; }
        public string DocPath { get; set; }
        public string FileName { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }
        public DateTime? SysDate { get; set; }
        public string IP { get; set; }
        public Guid GUID { get; set; } = Guid.NewGuid();
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

        public long DocumentTranId { get; set; }
        public long DocumentExtTranId { get; set; }
        public long DetailId { get; set; }
        public long DocumentId { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
        public string Path { get; set; }
        public string Prefix { get; set; }
        public string IsRequired { get; set; }
        public string DocumentType { get; set; }
        public bool IsFileAdded { get; set; } = false;
        public int DocType { get; set; }
        public Document Document
        {
            get { return DocType == 200 ? Document.Additional : Document.Predefined; }
            set { DocType = value == Document.Predefined ? 100 : 200; }
        }
        public void PopulateFileName()
        {
            FileName = string.IsNullOrEmpty(Path) ? null : new FileInfo(Path).Name;
        }
    }
}
