using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Anotar.NLog;
using CommandLine;
using DatabaseSchemaReader;
using NLog;
using TCode.r2rml4net.Mapping.Fluent;
using TCode.r2rml4net.RDB.DatabaseSchemaReader;
using VDS.RDF;
using VDS.RDF.Writing;

[assembly: LogMinimalMessage]

namespace TCode.r2rml4net.CLI
{
    class Program
    {
        private readonly ITripleStore _output;
        private readonly IStoreWriter _writer;
        private readonly string _connectionString;
        private readonly string _outputPath;

        private Program(string connectionString, string outputPath, bool verbose)
        {
            _connectionString = connectionString;
            _outputPath = outputPath;

            _output = new TripleStore();
            _writer = new NQuadsWriter();

            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
            {
                Layout = "${level} ${message}"
            };

            var minLevel = verbose ? LogLevel.Debug : LogLevel.Info;
            config.AddRule(minLevel, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<DirectMappingOptions, R2rmlOptions, GenerateDirectOptions>(args)
                .WithParsed<DirectMappingOptions>(options => new Program(options.ConnectionString, options.Output, options.Verbose).RunDirect())
                .WithParsed<R2rmlOptions>(options => new Program(options.ConnectionString, options.Output, options.Verbose).RunMapping(options.MappingPath))
                .WithParsed<GenerateDirectOptions>(options =>
                {
                    var rml = GenerateDirectMapping(options.ConnectionString);
                    new CompressingTurtleWriter().Save(rml.MappingsGraph, Console.Out);
                });
        }

        static DirectR2RMLMapping GenerateDirectMapping(string connectionString)
        {
            using (DbConnection connection = new SqlConnection(connectionString))
            {
                var dbSchema = new DatabaseSchemaAdapter(
                    new DatabaseReader(connection),
                    new MSSQLServerColumTypeMapper());

                return new DirectR2RMLMapping(dbSchema);
            }
        }

        private void RunDirect()
        {
            var rml = GenerateDirectMapping(this._connectionString);

            using (DbConnection connection = new SqlConnection(this._connectionString))
            {
                var processor = new W3CR2RMLProcessor(connection);

                this.Run(processor, rml);
            }
        }

        private void RunMapping(string mappingPath)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                var processor = new W3CR2RMLProcessor(connection);
                if ((File.GetAttributes(mappingPath) & FileAttributes.Directory) != 0)
                {
                    foreach (var path in Directory.GetFiles(mappingPath))
                    {
                        this.RunMapping(processor, path);
                    }
                }
                else
                {
                    this.RunMapping(processor, mappingPath);
                }
            }

            var dirPath = Path.GetDirectoryName(this._outputPath);
            if (string.IsNullOrWhiteSpace(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
            this._writer.Save(this._output, this._outputPath);
        }

        private void RunMapping(IR2RMLProcessor processor, string path)
        {
            LogTo.Info($"Processing {path}");
            var rml = R2RMLLoader.LoadFile(path);

          this.Run(processor, rml);
        }

        private void Run(IR2RMLProcessor processor, IR2RML rml)
        {
            LogTo.Info("Found {0} triples maps", rml.TriplesMaps.Count());
            processor.GenerateTriples(rml, this._output);
            LogTo.Info("Generated {0} quads in {1} graphs", processor.TriplesGenerated, processor.GraphsGenerated);
        }

        [Verb("rml")]
        class R2rmlOptions
        {
            [Option('c', "connection-string", Required = true)]
            public string ConnectionString { get; set; }

            [Option('o', "output")]
            public string Output { get; set; }

            [Option('v')]
            public bool Verbose { get; set; }

            [Option('m', "mapping", Required = true)]
            public string MappingPath { get; set; }
        }

        [Verb("generate-direct")]
        class GenerateDirectOptions
        {
            [Option('c', "connection-string", Required = true)]
            public string ConnectionString { get; set; }
        }

        [Verb("direct")]
        class DirectMappingOptions
        {
            [Option('c', "connection-string", Required = true)]
            public string ConnectionString { get; set; }

            [Option('o', "output")]
            public string Output { get; set; }

            [Option('v')]
            public bool Verbose { get; set; }
        }
    }
}