using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using TCode.r2rml4net.TriplesGeneration;
using VDS.RDF;

namespace Samples.Excel2010
{
    class Program : IDisposable
    {
        private readonly OdbcConnection _excelConnection;

        private Program(string excelPath)
        {
            OleDbConnectionStringBuilder csbuilder = new OleDbConnectionStringBuilder();
            csbuilder.Provider = "Microsoft.ACE.OLEDB.12.0";
            csbuilder.DataSource = excelPath;
            csbuilder.Add("Extended Properties", "Excel 12.0 Xml;HDR=YES");

            _excelConnection = new OdbcConnection(csbuilder.ConnectionString);
        }

        static void Main(string[] args)
        {
            Program sample = new Program(args[0] ?? "Data\\SampleData.xlsx");

            IR2RML mappings = sample.GetDefaultMappingForExcel();

            ITripleStore generatedTriples = sample.GetTriples(mappings);

            Console.WriteLine("Extracted {0} triples", generatedTriples.Triples.Count());
            Console.WriteLine();
            foreach (var triple in generatedTriples.Triples)
            {
                Console.WriteLine(triple);
            }
        }

        private ITripleStore GetTriples(IR2RML mappings)
        {
            using (var r2RML = new W3CR2RMLProcessor(_excelConnection))
            {
                return r2RML.GenerateTriples(mappings);
            }
        }

        private IR2RML GetDefaultMappingForExcel()
        {
            return new DirectR2RMLMapping(new ExcelSchemaProvider(_excelConnection));
        }

        public void Dispose()
        {
            _excelConnection.Dispose();
        }
    }
}
