using System.Data.Odbc;
using System.Data.OleDb;
using TCode.r2rml4net.Mapping.DirectMapping;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using TCode.r2rml4net.TriplesGeneration;

namespace Samples.Excel2010
{
    class Program
    {
        static void Main()
        {
            OleDbConnectionStringBuilder csbuilder = new OleDbConnectionStringBuilder();
            csbuilder.Provider = "Microsoft.ACE.OLEDB.12.0";
            csbuilder.DataSource = @"c:\temp\example.xlsx";
            csbuilder.Add("Extended Properties", "Excel 12.0 Xml;HDR=YES");

            using (var r2RML = new W3CR2RMLProcessor(new OdbcConnection(csbuilder.ConnectionString), new RDFTermGenerator()))
            {
                new R2RMLMappingGenerator(new DatabaseSchemaAdapter(), )
                r2RML.GenerateTriples(Default);
            }
        }
    }
}
