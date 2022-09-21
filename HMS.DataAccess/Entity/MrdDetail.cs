using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;

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
