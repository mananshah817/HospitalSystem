using HMS.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace HMS.DataAccess.Entity
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Terminal { get; set; }
        public string HPass { get; set; }
        public string Connection { get; set; }
    }

    public class ListOfItem
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? Sort { get; set; }
        public string Txt { get; set; }
        public long ParentId { get; set; }
        public string sup_name { get; set; }
        public DateTime? DateValue { get; set; }
    }

    public class Mailing
    {
        public Mailing()
        {
            From = ConfigurationManager.AppSettings["EmailFrom"];
        }
        public static string From { get; set; }
        public string[] To { get; set; } = new string[] { };
        public string[] ToGroup { get; set; } = new string[] { };
        public string[] CC { get; set; } = new string[] { };
        public string[] CCGroup { get; set; } = new string[] { };
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachment { get; set; }
        public MailType Type { get; set; } = MailType.Text;
    }

    public class MenuDetail
    {
        public int MenuId { get; set; }
        public string Menu { get; set; }
        public int FormId { get; set; }
        public string Form { get; set; }
        public string FormDisplayName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int MenuSort { get; set; }
        public int FormSort { get; set; }
    }

    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public List<Form> FormList { get; set; } = new List<Form>();
    }

    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int Sort { get; set; }
    }

    public class DocumentDetail
    {
        public string IpdNo { get; set; }
        //[ColumnName("YOJNA_DOCU_D_ID")]
        public long DocumentTranId { get; set; }
        //[ColumnName("YOJNA_DOCU_EXT_T_ID")]
        public long DocumentExtTranId { get; set; }
        public long DetailId { get; set; }
        //[ColumnName("SRGY_D_ID")]
        public long DocumentId { get; set; }
        //[ColumnName("DOC_NM")]
        public string Description { get; set; }
        public string Flag { get; set; }
        //[ColumnName("DOC_PTH")]
        public string Path { get; set; }
        //[ColumnName("DOC_PRFX")]
        public string Prefix { get; set; }
        //[ColumnName("IS_RQRD")]
        public string IsRequired { get; set; }
        public string DocumentType { get; set; }
        //[ColumnName("SRT")]
        public int Sort { get; set; }
        //[ColumnName("GRP_SRT")]
        public int GroupSort { get; set; }
        //[ColumnName("RMK")]
        public string Remark { get; set; }
        public string IsCheckList { get; set; }
        public bool IsChecked
        {
            get { return IsCheckList == "Y"; }
            set { IsCheckList = (value) ? "Y" : "N"; }
        }
        public bool IsFileAdded { get; set; } = false;
        public string FileName { get; set; }
        /*{
            get { return !string.IsNullOrEmpty(FileName) ? FileName : string.IsNullOrEmpty(Path) ? null : new FileInfo(Path).Name; }
            set { FileName = value; }
        }*/
        public int DocType { get; set; }
        public Document Document
        {
            get { return DocType == 200 ? Document.Additional : Document.Predefined; }
            set { DocType = value == Document.Predefined ? 100 : 200; }
        }
        public DataRowState RowState { get; set; } = DataRowState.Unchanged;
        public bool IsValid { get; set; } = true;
        [ColumnName("IsCheckList")]
        public string CheckList { get; set; }
        public string Yes
        {
            get { return $"{DocumentId}_YES"; }
        }
        public string No
        {
            get { return $"{DocumentId}_NO"; }
        }
        public string NA
        {
            get { return $"{DocumentId}_NA"; }
        }
        public void PopulateFileName()
        {
            FileName = string.IsNullOrEmpty(Path) ? null : new FileInfo(Path).Name;
        }
        public DateTime SYSDate { get; set; }
        public string UserName { get; set; }
    }

    public class YojanaTransaction
    {
        public long DetailId { get; set; }
        [ColumnName("YojanaId", "YojanaName")]
        public ListOfItem Yojana { get; set; }
        public string CardNo { get; set; }
        public string Name { get; set; }
        [ColumnName("DocumentTypeId", "DocName")]
        public ListOfItem Surgery { get; set; }
        public string CaseNo { get; set; }
        public string IpdNo { get; set; }
        public string OpdNo { get; set; }
        public string Type { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public List<DocumentDetail> Documents { get; set; }
        public string Folder { get; set; }
    }

    public enum Document
    {
        Predefined = 100,
        Additional = 200

    }

    public enum MailType
    {
        Text,
        HTML
    }
}
