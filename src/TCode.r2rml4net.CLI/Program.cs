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
        private readonly DirectMappingOptions _options;
        private readonly ITripleStore _output;
        private readonly IStoreWriter _writer;

        private Program(DirectMappingOptions options)
        {
            _options = options;
            _output = new TripleStore();
            _writer = new NQuadsWriter();

            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
            {
                Layout = "${level} ${message}"
            };

            var minLevel = options.Verbose ? LogLevel.Debug : LogLevel.Info;
            config.AddRule(minLevel, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<DirectMappingOptions, R2rmlOptions, GenerateDirectOptions>(args)
                .WithParsed<DirectMappingOptions>(options => new Program(options).RunDirect())
                .WithParsed<R2rmlOptions>(options => new Program(options).RunMapping(options.MappingPath))
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
            var rml = GenerateDirectMapping(this._options.ConnectionString);

            using (DbConnection connection = new SqlConnection(this._options.ConnectionString))
            {
                var processor = new W3CR2RMLProcessor(connection);

                this.Run(processor, rml);
            }
        }

        private void RunMapping(string mappingPath)
        {
            using (IDbConnection connection = new SqlConnection(this._options.ConnectionString))
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

            Directory.CreateDirectory(Path.GetDirectoryName(this._options.Output));
            this._writer.Save(this._output, this._options.Output);
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
        class R2rmlOptions : DirectMappingOptions
        {
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