using System.Data;

namespace TCode.r2rml4net.Mapping.Fluent
{
    class UrisHelper
    {
        public static string GetXsdDataType(DbType columnType)
        {
            const string prefix = "http://www.w3.org/2001/XMLSchema#";
            string datatype;

            switch (columnType)
            {
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    datatype = "integer";
                    break;
                case DbType.Boolean:
                    datatype = "boolean";
                    break;
                case DbType.Byte:
                    datatype = "unsignedByte";
                    break;
                case DbType.Date:
                    datatype = "date";
                    break;
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    datatype = "dateTime";
                    break;
                case DbType.Currency:
                case DbType.Decimal:
                    datatype = "decimal";
                    break;
                case DbType.Double:
                case DbType.Single:
                    datatype = "double";
                    break;
                case DbType.SByte:
                    datatype = "byte";
                    break;
                case DbType.Time:
                    datatype = "time";
                    break;
                case DbType.Binary:
                    datatype = "hexBinary";
                    break;
                default:
                    return null;
            }

            return prefix + datatype;
        }

        internal const string RrTemplateProperty = "rr:template";
        internal const string RdfType = "rdf:type";
        internal const string RrIRI = "rr:IRI";
        internal const string RrConstantProperty = "rr:constant";
        internal const string RrTermTypeProperty = "rr:termType";
        internal const string RrLogicalTableProperty = "rr:logicalTable";
        internal const string RrTableNameProperty = "rr:tableName";
        internal const string RrSqlQueryProperty = "rr:sqlQuery";
        internal const string RrSqlVersionProperty = "rr:sqlVersion";
        internal const string RrTriplesMapClass = "rr:TriplesMap";
        internal const string RrClassProperty = "rr:class";
        internal const string RrSubjectMapProperty = "rr:subjectMap";
        internal const string RrBlankNode = "rr:BlankNode";
        internal const string RrLiteral = "rr:Literal";
        internal const string RrSubjectProperty = "rr:subject";
        internal const string RrPredicatePropety = "rr:predicate";
        internal const string RrObjectProperty = "rr:object";
        internal const string RrGraphPropety = "rr:graph";
        internal const string RrPredicateMapPropety = "rr:predicateMap";
        internal const string RrObjectMapProperty = "rr:objectMap";
        internal const string RrGraphMapPropety = "rr:graphMap";
        internal const string RrColumnProperty = "rr:column";
        internal const string RrPredicateObjectMapPropety = "rr:predicateObjectMap";
        internal const string RrLanguageTagPropety = "rr:languageTag";
        internal const string RrDatatypePropety = "rr:datatype";
    }
}