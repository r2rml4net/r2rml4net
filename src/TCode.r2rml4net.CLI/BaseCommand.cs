#region Licence

// Copyright (C) 2012-2020 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@t-code.pl
//
// ------------------------------------------------------------------------
//
// This file is part of r2rml4net.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.
//
// ------------------------------------------------------------------------
//
// r2rml4net may alternatively be used under the LGPL licence
//
// http://www.gnu.org/licenses/lgpl.html
//
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.

#endregion

using CommandLine;
using NLog;
using VDS.RDF;
using VDS.RDF.Writing;

namespace TCode.r2rml4net.CLI
{
    public abstract class BaseCommand
    {
        [Option('c', "connection-string", Required = true)]
        public string ConnectionString { get; set; }

        [Option('b', "base-uri", Default = "http://r2rml.net/base/")]
        public string BaseUri { get; set; }

        [Option('v')]
        public bool Verbose { get; set; }

        [Option("preserve-duplicate-rows", Default = false)]
        public bool PreserveDuplicateRows { get; set; }

        public MappingOptions MappingOptions => new MappingOptions()
            .WithBaseUri(this.BaseUri)
            .WithDuplicateRowsPreserved(this.PreserveDuplicateRows);

        public virtual void Prepare()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole")
            {
                Layout = "${level} ${message}"
            };

            var minLevel = this.Verbose ? LogLevel.Debug : LogLevel.Info;
            config.AddRule(minLevel, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
        }

        public abstract void Run();
    }
}