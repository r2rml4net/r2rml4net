using System;
using Anotar.NLog;
using CommandLine;

[assembly: LogMinimalMessage]

namespace TCode.r2rml4net.CLI
{
    static class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<DirectMappingCommand, R2RMLCommand, GenerateDefaultMappingCommand>(args)
                .WithParsed<DirectMappingCommand>(Run)
                .WithParsed<R2RMLCommand>(Run)
                .WithParsed<GenerateDefaultMappingCommand>(Run)
                .WithNotParsed(_ => Environment.Exit(1));
        }

        private static void Run(BaseCommand command)
        {
            command.Prepare();
            if (command.Run())
            {
                command.SaveOutput();
                Environment.Exit(0);
            }
            else
            {
                LogTo.Info("Errors occurred running command. Skipping output");
                Environment.Exit(1);
            }
        }
    }
}