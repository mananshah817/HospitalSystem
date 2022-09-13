using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace HMS.DataAccess.Infrastructure
{
    public static class Extension
    {
        #region DB Helper
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            try
            {
                var List = new List<T>();
                if (dt.Rows.Count > 0)
                {
                    foreach (var row in dt.AsEnumerable())
                    {
                        var obj = ToEntity<T>(row);
                        List.Add(obj);
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<T> ToList<T>(this EnumerableRowCollection<DataRow> Rows) where T : class, new()
        {
            try
            {
                var List = new List<T>();
                if (Rows.Count() > 0)
                {
                    foreach (var row in Rows.AsEnumerable())
                    {
                        var obj = ToEntity<T>(row);
                        List.Add(obj);
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<T> ToList<T>(this EnumerableRowCollection<DataRow> Rows, DataRowState RowState) where T : class, new()
        {
            try
            {
                var List = new List<T>();
                if (Rows.Count() > 0)
                {
                    foreach (var row in Rows.AsEnumerable())
                    {
                        T obj = new T();
                        foreach (var item in obj.GetType().GetProperties())
                        {
                            var property = obj.GetType().GetProperty(item.Name);
                            if (row.Table.Columns.Contains(property.Name))
                            {
                                if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Int32) && row[property.Name, DataRowVersion.Original].GetType() != typeof(DBNull))
                                {
                                    if (RowState == DataRowState.Deleted)
                                        property.SetValue(obj, Convert.ToInt32(row[property.Name, DataRowVersion.Original]), null);
                                    else
                                        property.SetValue(obj, Convert.ToInt32(row[property.Name]), null);
                                    continue;
                                }
                                if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Int64) && row[property.Name, DataRowVersion.Original].GetType() != typeof(DBNull))
                                {
                                    if (RowState == DataRowState.Deleted)
                                        property.SetValue(obj, Convert.ToInt64(row[property.Name, DataRowVersion.Original]), null);
                                    else
                                        property.SetValue(obj, Convert.ToInt64(row[property.Name]), null);
                                    continue;
                                }
                                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(decimal) && row[property.Name, DataRowVersion.Original].GetType() != typeof(DBNull))
                                {
                                    if (RowState == DataRowState.Deleted)
                                        property.SetValue(obj, Convert.ToDecimal(row[property.Name, DataRowVersion.Original]), null);
                                    else
                                        property.SetValue(obj, Convert.ToDecimal(row[property.Name]), null);
                                    continue;
                                }
                                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime) && row[property.Name, DataRowVersion.Original].GetType() != typeof(DBNull))
                                {
                                    if (RowState == DataRowState.Deleted)
                                        property.SetValue(obj, Convert.ToDateTime(row[property.Name, DataRowVersion.Original]), null);
                                    else
                                        property.SetValue(obj, Convert.ToDateTime(row[property.Name]), null);
                                    continue;
                                }
                                else
                                {
                                    if (!row[property.Name, DataRowVersion.Original].GetType().Name.Equals("DBNull") && !row[property.Name, DataRowVersion.Original].GetType().Name.Equals("DateTime"))
                                    {
                                        if (RowState == DataRowState.Deleted)
                                            property.SetValue(obj, Convert.ChangeType(row[property.Name, DataRowVersion.Original], property.PropertyType), null);
                                        else
                                            property.SetValue(obj, Convert.ChangeType(row[property.Name], property.PropertyType), null);
                                    }
                                    else if (row[property.Name, DataRowVersion.Original].GetType().Name.Equals("DateTime"))
                                    {
                                        if (RowState == DataRowState.Deleted)
                                            property.SetValue(obj, Convert.ChangeType(row[property.Name, DataRowVersion.Original], TypeCode.DateTime), null);
                                        else
                                            property.SetValue(obj, Convert.ChangeType(row[property.Name], TypeCode.DateTime), null);
                                    }
                                }
                            }
                        }
                        List.Add(obj);
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<T> GetRows<T>(this DataTable Source, DataRowState RowState) where T : class, new()
        {
            if (Source.AsEnumerable().Any(x => x.RowState == RowState))
            {
                if (RowState == DataRowState.Deleted)
                    return Source.AsEnumerable().Where(x => x.RowState == RowState)?.ToList<T>(RowState);
                else
                    return Source.AsEnumerable().Where(x => x.RowState == RowState)?.CopyToDataTable()?.ToList<T>();
            }
            else
                return new List<T>();
        }
        public static T[] ToArray<T>(this DataTable source, string ColumnName)
        {
            if (source == null)
                throw new ArgumentNullException("Source");
            return source.AsEnumerable().Select(x => x.Field<T>(ColumnName)).ToArray();
        }
        public static T[] ToArray<T>(this IEnumerable<DataRow> source, string ColumnName)
        {
            if (source == null)
                throw new ArgumentNullException("Source");
            return source.Select(x => x.Field<T>(ColumnName)).ToArray();
        }
        static T ToEntity<T>(this DataRow row) where T : class, new()
        {
            T obj = new T();
            foreach (var item in obj.GetType().GetProperties())
            {
                var attribute = item.GetCustomAttributes(false);
                ColumnName columnName = (ColumnName)attribute.FirstOrDefault(x => x.GetType() == typeof(ColumnName));
                var property = obj.GetType().GetProperty(item.Name);
                if (row.Table.Columns.Contains(property.Name) && columnName == null)
                {
                    Object Val = new object();
                    //if (row[property.Name].GetType() == typeof(string) && Convert.ToString(row[property.Name]).Trim().Length==0)
                    //{
                    //    Val = null;
                    //}
                    //else
                    Val = row[property.Name];

                    if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Int16) && row[property.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToInt16(Val), null);
                        continue;
                    }
                    if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Int32) && row[property.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToInt32(Val), null);
                        continue;
                    }
                    if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Int64) && row[property.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToInt64(Val), null);
                        continue;
                    }
                    else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(decimal) && row[property.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToDecimal(Val), null);
                        continue;
                    }
                    else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime) && row[property.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToDateTime(Val), null);
                        continue;
                    }
                    else
                    {
                        if (!row[property.Name].GetType().Name.Equals("DBNull") && !row[property.Name].GetType().Name.Equals("DateTime"))
                            property.SetValue(obj, Convert.ChangeType(Val, property.PropertyType), null);
                        else if (row[property.Name].GetType().Name.Equals("DateTime"))
                            property.SetValue(obj, Convert.ChangeType(Val, TypeCode.DateTime), null);
                    }
                }
                else if (columnName?.isNested ?? false)
                {
                    var index = 0;
                    var nestedObj = Activator.CreateInstance(property.PropertyType.UnderlyingSystemType);
                    foreach (var nestedProperty in property.PropertyType.GetProperties())
                    {
                        if (columnName.Names.Length > index)
                        {
                            var col = columnName.Names[index];
                            if (row.Table.Columns.Contains(col))
                            {
                                if (Nullable.GetUnderlyingType(nestedProperty.PropertyType) == typeof(decimal) && row[col].GetType() != typeof(DBNull))
                                {
                                    nestedProperty.SetValue(nestedObj, Convert.ToDecimal(row[col]), null);
                                    //continue;
                                }
                                else if (Nullable.GetUnderlyingType(nestedProperty.PropertyType) == typeof(long) && row[col].GetType() != typeof(DBNull))
                                {
                                    nestedProperty.SetValue(nestedObj, Convert.ToInt64(row[col]), null);
                                    //continue;
                                }
                                else if (!row[col].GetType().Name.Equals("DBNull", StringComparison.OrdinalIgnoreCase) && !row[col].GetType().Name.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                                    nestedProperty.SetValue(nestedObj, Convert.ChangeType(row[col], nestedProperty.PropertyType), null);
                                else if (row[col].GetType().Name.Equals("DateTime"))
                                    nestedProperty.SetValue(nestedObj, Convert.ChangeType(row[col], TypeCode.DateTime), null);

                                index++;
                            }
                        }

                    }
                    property.SetValue(obj, nestedObj);
                }
                else if (columnName != null && row.Table.Columns.Contains(columnName.Name))
                {
                    if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(decimal) && row[columnName.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToDecimal(row[columnName.Name]), null);
                        continue;
                    }
                    else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(long) && row[columnName.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToInt64(row[columnName.Name]), null);
                        continue;
                    }
                    else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(short) && row[columnName.Name].GetType() != typeof(DBNull))
                    {
                        property.SetValue(obj, Convert.ToInt16(row[columnName.Name]), null);
                        continue;
                    }
                    else if (!row[columnName.Name].GetType().Name.Equals("DBNull", StringComparison.OrdinalIgnoreCase) && !row[columnName.Name].GetType().Name.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                        property.SetValue(obj, Convert.ChangeType(row[columnName.Name], property.PropertyType), null);
                    else if (row[columnName.Name].GetType().Name.Equals("DateTime"))
                        property.SetValue(obj, Convert.ChangeType(row[columnName.Name], TypeCode.DateTime), null);
                }
            }
            return obj;
        }
        #endregion
        public static bool In<T>(this T source, params T[] list)
        {
            if (source == null)
                throw new ArgumentNullException("Source");

            return list.Contains(source);
        }
        public static bool In<T>(this T source, List<T> list)
        {
            if (source == null)
                throw new ArgumentNullException("Source");

            return list.Contains(source);
        }
        public static bool In<T>(this List<T> source, List<T> list)
        {
            if (source == null)
                throw new ArgumentNullException("Source");

            return list.Intersect(source).Any();
        }
        public static bool NotIn<T>(this T source, params T[] list)
        {
            if (source == null)
                throw new ArgumentNullException("Source");

            return !list.Contains(source);
        }
        //public static string GetLocationIPAddress()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (var item in host.AddressList)
        //    {
        //        if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        //            return item.ToString();
        //    }
        //    throw new Exception("no network adapter found.");
        //}
        public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.GroupBy(selector).Select(x => x.First());
        }
        public static List<List<T>> Batch<T>(this IEnumerable<T> Source, int BatchSize)
        {
            return Source.Select((x, i) => new { Index = i, Value = x })
                         .GroupBy(x => x.Index / BatchSize)
                         .Select(x => x.Select(y => y.Value).ToList()).ToList();
        }
        public static ServiceResponse GetTrace(this Exception ex)
        {
            var trace = new List<Trace>();
            var Message = ex.Message;
            var StackTrace = new StackTrace(ex, true);

            foreach (var item in StackTrace.GetFrames())
            {
                trace.Add(new Trace
                {
                    FileName = item.GetFileName(),
                    MethodName = item.GetMethod().Name,
                    LineNumber = item.GetFileLineNumber()
                });
            }
            var ErrorType = (ex is SqlException ? Type.SQL : Type.DotNet);
            var response = new ServiceResponse
            {
                httpStatus = HttpStatusCode.InternalServerError,
                type = "error",
                code = ErrorType,
                body = (ErrorType == Type.SQL) ? Message.Substring(0, Message.IndexOf("Process", StringComparison.OrdinalIgnoreCase)) : Message,
                stackTrace = trace
            };
            return response;
        }
        public static ServiceResponse<T> GetTrace<T>(this Exception ex, T entity)
        {
            var trace = new List<Trace>();
            var Message = ex.Message;
            var StackTrace = new StackTrace(ex, true);

            foreach (var item in StackTrace.GetFrames())
            {
                trace.Add(new Trace
                {
                    FileName = item.GetFileName(),
                    MethodName = item.GetMethod().Name,
                    LineNumber = item.GetFileLineNumber()
                });
            }
            var ErrorType = (ex is SqlException ? Type.SQL : Type.DotNet);
            var response = new ServiceResponse<T>
            {
                httpStatus = HttpStatusCode.InternalServerError,
                type = "error",
                code = ErrorType,
                body = Message,
                stackTrace = trace,
                Data = entity
            };
            return response;
        }
        public static string ToCsv(this DataTable dt, string Seperator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn item in dt.Columns)
            {
                sb.Append($"\"{item.ColumnName}\"");
                sb.Append(Seperator);
            }
            sb.AppendLine();
            foreach (DataRow item in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append($"\"{Convert.ToString(item[i])}\"");
                    if (i < dt.Columns.Count - 1)
                        sb.Append(Seperator);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        public static string Base64Encode(this string value)
        {
            var EncodedBytes = Encoding.ASCII.GetBytes(value);

            var result = Convert.ToBase64String(EncodedBytes);

            return result;
        }
        public static string Base64Decode(this string value)
        {
            var DecodedBytes = Convert.FromBase64String(value);

            var result = Encoding.ASCII.GetString(DecodedBytes);

            return result;
        }
        public static string ToCapitalCase(this string value)
        {
            value = value.ToLower();
            if (value?.Length > 0)
                return $"{char.ToUpper(value[0])}{value.Substring(1)}";
            else
                return null;
        }
        public static string ParseSqlException(this SqlException ex)
        {
            if (ex.Number == 50000)
            {
                var Message = ex.Message.Split(new char[] { '\r' });
                if (Message.Length > 1)
                {
                    return Message[0];
                }
            }
            return ex.Message;
        }
        public static List<DateTime> GetDateRange(this DateTime FromDate, DateTime ToDate)
        {
            return Enumerable.Range(0, int.MaxValue)
                             .Select(x => new DateTime(FromDate.AddDays(x).Ticks))
                             .TakeWhile(date => date <= ToDate).ToList();
        }
        public static DateTime? ToIst(this DateTime? Date)
        {
            if (Date == null)
                return null;

            if (Date?.Kind != DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(Date), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            else
                return Date;
        }
        public static DateTime ToIst(this DateTime Date)
        {
            if (Date.Kind != DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeFromUtc(Date, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            else
                return Date;
        }

    }
    public class ColumnName : Attribute
    {
        public string Name { get; }
        public string[] Names { get; set; }
        public Dictionary<string, string> NameDictionary { get; set; }
        public bool isNested { get; set; }
        public ColumnName(string name)
        {
            Name = name;
            isNested = false;
        }
        public ColumnName(params string[] names)
        {
            Names = names;
            isNested = true;
        }
        /*public ColumnName(Dictionary<string,string> nameDictionary)
        {
            NameDictionary = nameDictionary;
            isNested = true;
        }*/
    }
    public enum Property
    {
        Text,
        DataSource
    }
    public enum Type
    {
        DotNet = 101,
        SQL = 102,
        Success = 103,
        Warning = 104
    }
    public class ServiceResponse
    {
        public HttpStatusCode httpStatus { get; set; }
        public string type { get; set; } = "info";

        public string title
        {
            get
            {
                switch (code)
                {
                    case Type.DotNet:
                        return "Error";
                    case Type.SQL:
                        return "Error: SQL";
                    case Type.Success:
                        return "Success";
                    case Type.Warning:
                        return "Warning";
                    default:
                        return null;
                }
            }
        }
        public int timeout
        {
            get
            {
                switch (code)
                {
                    case Type.DotNet:
                    case Type.SQL:
                    case Type.Warning:
                        return 0;
                    case Type.Success:
                        return 3000;
                    default:
                        return 2;
                }
            }
        }
        public Type code { get; set; }
        public string body { get; set; }
        public List<Trace> stackTrace { get; set; }
    }
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public HttpStatusCode httpStatus { get; set; }
        public string type { get; set; } = "info";

        public string title
        {
            get
            {
                switch (code)
                {
                    case Type.DotNet:
                        return "Error";
                    case Type.SQL:
                        return "Error: SQL";
                    case Type.Success:
                        return "Success";
                    case Type.Warning:
                        return "Warning";
                    default:
                        return null;
                }
            }
        }
        public int timeout
        {
            get
            {
                switch (code)
                {
                    case Type.DotNet:
                    case Type.SQL:
                        return 0;
                    case Type.Success:
                        return 3000;
                    default:
                        return 2;
                }
            }
        }
        public Type code { get; set; }
        public string body { get; set; }
        public List<Trace> stackTrace { get; set; }
    }
    public class Trace
    {
        public string MethodName { get; set; }
        public string FileName { get; set; }
        public int LineNumber { get; set; }

    }
    public static class ISOWeek
    {
        private const int WeeksInLongYear = 53;
        private const int WeeksInShortYear = 52;

        private const int MinWeek = 1;
        private const int MaxWeek = WeeksInLongYear;

        public static int GetWeekOfYear(DateTime date)
        {
            int week = GetWeekNumber(date);

            if (week < MinWeek)
            {
                // If the week number obtained equals 0, it means that the
                // given date belongs to the preceding (week-based) year.
                return GetWeeksInYear(date.Year - 1);
            }

            if (week > GetWeeksInYear(date.Year))
            {
                // If a week number of 53 is obtained, one must check that
                // the date is not actually in week 1 of the following year.
                return MinWeek;
            }

            return week;
        }

        public static int GetYear(DateTime date)
        {
            int week = GetWeekNumber(date);

            if (week < MinWeek)
            {
                // If the week number obtained equals 0, it means that the
                // given date belongs to the preceding (week-based) year.
                return date.Year - 1;
            }

            if (week > GetWeeksInYear(date.Year))
            {
                // If a week number of 53 is obtained, one must check that
                // the date is not actually in week 1 of the following year.
                return date.Year + 1;
            }

            return date.Year;
        }

        // The year parameter represents an ISO week-numbering year (also called ISO year informally).
        // Each week's year is the Gregorian year in which the Thursday falls.
        // The first week of the year, hence, always contains 4 January.
        // ISO week year numbering therefore slightly deviates from the Gregorian for some days close to 1 January.
        public static DateTime GetYearStart(int year)
        {
            return ToDateTime(year, MinWeek, DayOfWeek.Monday);
        }

        // The year parameter represents an ISO week-numbering year (also called ISO year informally).
        // Each week's year is the Gregorian year in which the Thursday falls.
        // The first week of the year, hence, always contains 4 January.
        // ISO week year numbering therefore slightly deviates from the Gregorian for some days close to 1 January.
        public static DateTime GetYearEnd(int year)
        {
            return ToDateTime(year, GetWeeksInYear(year), DayOfWeek.Sunday);
        }

        // From https://en.wikipedia.org/wiki/ISO_week_date#Weeks_per_year:
        //
        // The long years, with 53 weeks in them, can be described by any of the following equivalent definitions:
        //
        // - Any year starting on Thursday and any leap year starting on Wednesday.
        // - Any year ending on Thursday and any leap year ending on Friday.
        // - Years in which 1 January and 31 December (in common years) or either (in leap years) are Thursdays.
        //
        // All other week-numbering years are short years and have 52 weeks.
        public static int GetWeeksInYear(int year)
        {
            //if (year < MinYear || year > MaxYear)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(year), SR.ArgumentOutOfRange_Year);
            //}

            int P(int y) => (y + (y / 4) - (y / 100) + (y / 400)) % 7;

            if (P(year) == 4 || P(year - 1) == 3)
            {
                return WeeksInLongYear;
            }

            return WeeksInShortYear;
        }

        // From https://en.wikipedia.org/wiki/ISO_week_date#Calculating_a_date_given_the_year,_week_number_and_weekday:
        //
        // This method requires that one know the weekday of 4 January of the year in question.
        // Add 3 to the number of this weekday, giving a correction to be used for dates within this year.
        //
        // Multiply the week number by 7, then add the weekday. From this sum subtract the correction for the year.
        // The result is the ordinal date, which can be converted into a calendar date.
        //
        // If the ordinal date thus obtained is zero or negative, the date belongs to the previous calendar year.
        // If greater than the number of days in the year, to the following year.
        public static DateTime ToDateTime(int year, int week, DayOfWeek dayOfWeek)
        {
            //if (year < MinYear || year > MaxYear)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(year), SR.ArgumentOutOfRange_Year);
            //}

            //if (week < MinWeek || week > MaxWeek)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(week), SR.ArgumentOutOfRange_Week_ISO);
            //}

            // We allow 7 for convenience in cases where a user already has a valid ISO
            // day of week value for Sunday. This means that both 0 and 7 will map to Sunday.
            // The GetWeekday method will normalize this into the 1-7 range required by ISO.
            //if ((int)dayOfWeek < 0 || (int)dayOfWeek > 7)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(dayOfWeek), SR.ArgumentOutOfRange_DayOfWeek);
            //}

            var jan4 = new DateTime(year, month: 1, day: 4);

            int correction = GetWeekday(jan4.DayOfWeek) + 3;

            int ordinal = (week * 7) + GetWeekday(dayOfWeek) - correction;

            return new DateTime(year, month: 1, day: 1).AddDays(ordinal - 1);
        }

        // From https://en.wikipedia.org/wiki/ISO_week_date#Calculating_the_week_number_of_a_given_date:
        //
        // Using ISO weekday numbers (running from 1 for Monday to 7 for Sunday),
        // subtract the weekday from the ordinal date, then add 10. Divide the result by 7.
        // Ignore the remainder; the quotient equals the week number.
        //
        // If the week number thus obtained equals 0, it means that the given date belongs to the preceding (week-based) year.
        // If a week number of 53 is obtained, one must check that the date is not actually in week 1 of the following year.
        private static int GetWeekNumber(DateTime date)
        {
            return (date.DayOfYear - GetWeekday(date.DayOfWeek) + 10) / 7;
        }

        // Day of week in ISO is represented by an integer from 1 through 7, beginning with Monday and ending with Sunday.
        // This matches the underlying values of the DayOfWeek enum, except for Sunday, which needs to be converted.
        private static int GetWeekday(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }
    }
}
