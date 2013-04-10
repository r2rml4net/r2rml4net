using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using TCode.r2rml4net.Excel;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace Samples.Excel2010
{
    class Program
    {
        private readonly string _path;

        private Program(string excelPath)
        {
            _path = excelPath;
        }

        static void Main(string[] args)
        {
            Program sample = new Program(args[0] ?? "Data\\SampleData.xlsx");

            var excelSchemaProvider = new ExcelSchemaProvider(sample._path, ExcelFormat.OpenXML);
            IR2RML mappings = sample.GetDefaultMappingForExcel(excelSchemaProvider);

            ITripleStore generatedTriples = sample.GetTriples(mappings, excelSchemaProvider.Connection);

            Console.WriteLine("Extracted {0} triples", generatedTriples.Triples.Count());
            Console.WriteLine();
            foreach (var triple in generatedTriples.Triples)
            {
                Console.WriteLine(triple);
            }
        }

        private ITripleStore GetTriples(IR2RML mappings, IDbConnection excelConnection)
        {
            using (var r2RML = new W3CR2RMLProcessor(excelConnection))
            {
                return r2RML.GenerateTriples(mappings);
            }
        }

        private IR2RML GetDefaultMappingForExcel(IDatabaseMetadata excelSchemaProvider)
        {
            return new DirectR2RMLMapping(excelSchemaProvider);
        }
    }
}
