using System.Data;

namespace TCode.r2rml4net.Mapping.Fluent
{
    class UrisHelper
    {
        public static string LiterlUriNode(DbType columnType)
        {
            switch (columnType)
            {
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return "xsd:integer";
                case DbType.Boolean:
                    return "xsd:boolean";
                case DbType.Byte:
                    return "xsd:unsignedByte";
                case DbType.Date:
                    return "xsd:date";
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return "xsd:datetime";
                case DbType.Currency:
                case DbType.Decimal:
                    return "xsd:decimal";
                case DbType.Double:
                case DbType.Single:
                    return "xsd:double";
                case DbType.SByte:
                    return "xsd:byte";
                case DbType.Time:
                    return "xsd:time";
                default:
                    return null;
            }
        }

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