using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using TCode.r2rml4net.TriplesGeneration;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Default implementation of <see cref="ISQLValuesMappingStrategy"/>, which conforms to the
    /// <a href="http://www.w3.org/TR/r2rml/#datatype-conversions">Datatype Conversions section of R2RML specification</a>
    /// </summary>
    public class DefaultSQLValuesMappingStrategy : ISQLValuesMappingStrategy
    {
        private readonly IDictionary<Type, string> _datatypeMappings = new Dictionary<Type, string>();

        /// <summary>
        /// Creates an instance of <see cref="DefaultSQLValuesMappingStrategy"/>
        /// </summary>
        public DefaultSQLValuesMappingStrategy()
        {
            FillDefaultDatatypeMappings();
        }

        #region Implementation of ISQLValuesMappingStrategy

        /// <summary>
        /// Gets the column value's lexical form and it's RDF datatype URI
        /// </summary>
        public string GetLexicalForm(int columnIndex, IDataRecord logicalRow, out Uri naturalRdfDatatype)
        {
            Type type = logicalRow.GetFieldType(columnIndex);
            naturalRdfDatatype = GetXsdUriForType(type, logicalRow.GetDataTypeName(columnIndex));

            if (logicalRow.IsDBNull(columnIndex))
                return null;

            return GetMappedValue(columnIndex, logicalRow, naturalRdfDatatype);
        }

        #endregion

        /// <summary>
        /// Gets a lexical form for column value and the given <paramref name="dataType"/>
        /// </summary>
        protected internal virtual string GetMappedValue(int columnIndex, IDataRecord logicalRow, Uri dataType)
        {
            if (dataType != null)
            {
                string uriString = dataType.AbsoluteUri;

                switch (uriString)
                {
                    case XsdDatatypes.Boolean:
                        return logicalRow.GetValue(columnIndex).ToString().ToLower();
                    case XsdDatatypes.Decimal:
                        return logicalRow.GetDecimal(columnIndex).ToString(CultureInfo.InvariantCulture);
                    case XsdDatatypes.Double:
                        var format = "0.0" + new string('#', 29) + "E-0";

                        return logicalRow.GetFieldType(columnIndex) == typeof(float) 
                            ? logicalRow.GetFloat(columnIndex).ToString(format, CultureInfo.InvariantCulture) 
                            : logicalRow.GetDouble(columnIndex).ToString(format, CultureInfo.InvariantCulture);

                    case XsdDatatypes.DateTime:
                        return GetUtcTime(columnIndex, logicalRow).ToString("u").Replace(' ', 'T').TrimEnd('Z');
                    case XsdDatatypes.Time:
                        return GetUtcTime(columnIndex, logicalRow).ToString("u").Split(' ')[1];
                    case XsdDatatypes.Date:
                        return GetUtcTime(columnIndex, logicalRow).ToString("u").Split(' ')[0];
                    case XsdDatatypes.Binary:
                        return ByteArrayToString((byte[]) logicalRow.GetValue(columnIndex));
                }
            }

            return logicalRow.GetValue(columnIndex).ToString();
        }

        /// <summary>
        /// Gets XSD datatype URI for the given .NET <see cref="Type"/> and <paramref name="sqlTypeName"/>
        /// </summary>
        protected virtual Uri GetXsdUriForType(Type type, string sqlTypeName)
        {
            if (type == typeof(DateTime) && !string.IsNullOrWhiteSpace(sqlTypeName))
            {
                var typeNamelowered = sqlTypeName.ToLower();
                if (typeNamelowered == "time")
                {
                    return new Uri(XsdDatatypes.Time);
                }
                if (typeNamelowered == "date")
                {
                    return new Uri(XsdDatatypes.Date);
                }
            }
            if (_datatypeMappings.ContainsKey(type))
            {
                return new Uri(_datatypeMappings[type]);
            }

            return null;
        }

        private void FillDefaultDatatypeMappings()
        {
            _datatypeMappings.Add(typeof(int), XsdDatatypes.Integer);
            _datatypeMappings.Add(typeof(short), XsdDatatypes.Integer);
            _datatypeMappings.Add(typeof(long), XsdDatatypes.Integer);
            _datatypeMappings.Add(typeof(byte[]), XsdDatatypes.Binary);
            _datatypeMappings.Add(typeof(decimal), XsdDatatypes.Decimal);
            _datatypeMappings.Add(typeof(bool), XsdDatatypes.Boolean);
            _datatypeMappings.Add(typeof(float), XsdDatatypes.Double);
            _datatypeMappings.Add(typeof(double), XsdDatatypes.Double);
            _datatypeMappings.Add(typeof(DateTime), XsdDatatypes.DateTime);
            _datatypeMappings.Add(typeof(TimeSpan), XsdDatatypes.DateTime);
        }

        static DateTime GetUtcTime(int columnIndex, IDataRecord logicalRow)
        {
            return TimeZoneInfo.ConvertTimeToUtc(logicalRow.GetDateTime(columnIndex), TimeZoneInfo.Utc);
        }

        static string ByteArrayToString(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = ((byte)(bytes[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(bytes[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }
    }
}