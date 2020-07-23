using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Anotar.NLog;
using CommandLine;
using NLog;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using VDS.RDF.Writing;

[assembly: LogMinimalMessage]

namespace TCode.r2rml4net.CLI
{
    class Program
    {
        private readonly Options _options;
        private readonly ITripleStore _output;
        private readonly IStoreWriter _writer;

        private Program(Options options)
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
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => new Program(options).Run());
        }

        private void Run()
        {
            using (IDbConnection connection = new SqlConnection(this._options.ConnectionString))
            {
                var processor = new W3CR2RMLProcessor(connection);
                if ((File.GetAttributes(this._options.MappingPath) & FileAttributes.Directory) != 0)
                {
                    foreach (var path in Directory.GetFiles(this._options.MappingPath))
                    {
                        this.RunMapping(processor, path);
                    }
                }
                else
                {
                    this.RunMapping(processor, this._options.MappingPath);
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(this._options.Output));
            this._writer.Save(this._output, this._options.Output);
        }

        private void RunMapping(IR2RMLProcessor processor, string path)
        {
            LogTo.Info($"Processing {path}");
            var rml = R2RMLLoader.LoadFile(path);

            LogTo.Info("Found {0} triples maps", rml.TriplesMaps.Count());
            processor.GenerateTriples(rml, this._output);
            LogTo.Info("Generated {0} quads in {1} graphs", processor.TriplesGenerated, processor.GraphsGenerated);
        }

        class Options
        {
            [Option('c', "connection-string", Required = true)]
            public string ConnectionString { get; set; }

            [Option('m', "mapping", Required = true)]
            public string MappingPath { get; set; }

            [Option('o', "output")]
            public string Output { get; set; }

            [Option('v')]
            public bool Verbose { get; set; }
        }
    }
}