using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using CommandLine;
using TCode.r2rml4net;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Storage;

namespace TCode.r2rml4net.CLI
{
    class Program
    {
        private readonly Options _options;
        private readonly ITripleStore _output;

        private Program(Options options)
        {
            _options = options;
            _output = new TripleStore();
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => new Program(options).Run());
        }

        private void Run()
        {
            using (IDbConnection connection = new SqlConnection(this._options.ConnectionString))
            {
                if ((File.GetAttributes(this._options.MappingPath) & FileAttributes.Directory) != 0)
                {
                    foreach (var path in Directory.GetFiles(this._options.MappingPath))
                    {
                        this.RunMapping(connection, path);
                    }
                }
                else
                {
                    this.RunMapping(connection, this._options.MappingPath);
                }
            }

            new VDS.RDF.Writing.NQuadsWriter().Save(this._output, this._options.Output);
        }

        private void RunMapping(IDbConnection connection, string path)
        {
            Console.WriteLine($"Processing {path}");
            var rml = R2RMLLoader.LoadFile(path);

            new W3CR2RMLProcessor(connection).GenerateTriples(rml, this._output);
        }

        class Options
        {
            [Option('c', "connection-string", Required = true)]
            public string ConnectionString { get; set; }

            [Option('m', "mapping", Required = true)]
            public string MappingPath { get; set; }

            [Option('o', "output")]
            public string Output { get; set; }
        }
    }
}