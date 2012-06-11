namespace TCode.r2rml4net.Mapping.Fluent
{
    class UrisHelper
    {
        public static string LiterlUriNode(System.Data.DbType columnType)
        {
            switch (columnType)
            {
                case System.Data.DbType.Int16:
                case System.Data.DbType.Int32:
                case System.Data.DbType.Int64:
                case System.Data.DbType.UInt16:
                case System.Data.DbType.UInt32:
                case System.Data.DbType.UInt64:
                    return "xsd:integer";
                case System.Data.DbType.Boolean:
                    return "xsd:boolean";
                case System.Data.DbType.Byte:
                    return "xsd:unsignedByte";
                case System.Data.DbType.Date:
                    return "xsd:date";
                case System.Data.DbType.DateTime:
                case System.Data.DbType.DateTime2:
                case System.Data.DbType.DateTimeOffset:
                    return "xsd:datetime";
                case System.Data.DbType.Currency:
                case System.Data.DbType.Decimal:
                    return "xsd:decimal";
                case System.Data.DbType.Double:
                case System.Data.DbType.Single:
                    return "xsd:double";
                case System.Data.DbType.SByte:
                    return "xsd:byte";
                case System.Data.DbType.Time:
                    return "xsd:time";
                default:
                    return null;
            }
        } 
    }
}