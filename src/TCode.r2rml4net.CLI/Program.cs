using Anotar.NLog;
using CommandLine;

[assembly: LogMinimalMessage]

namespace TCode.r2rml4net.CLI
{
    static class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<DirectMappingCommand, R2RMLCommand, GenerateDirectMappingCommand>(args)
                .WithParsed<DirectMappingCommand>(Run)
                .WithParsed<R2RMLCommand>(Run)
                .WithParsed<GenerateDirectMappingCommand>(Run);
        }

        private static void Run(BaseCommand command)
        {
            command.Prepare();
            command.Run();
        }
    }
}