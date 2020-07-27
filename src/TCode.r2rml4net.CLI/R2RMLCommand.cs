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

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Anotar.NLog;
using CommandLine;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using VDS.RDF.Writing;

namespace TCode.r2rml4net.CLI
{
    [Verb("rml")]
    public class R2RMLCommand : BaseCommand
    {
        private TripleStore _output;

        [Option('o', "output")]
        public string OutFile { get; set; }

        [Option('m', "mapping", Required = true)]
        public string MappingPath { get; set; }

        public override void Prepare()
        {
            if (this.OutFile != null)
            {
                base.Prepare();
            }

            this._output = new TripleStore();
        }

        public override bool Run()
        {
            using (IDbConnection connection = new SqlConnection(this.ConnectionString))
            {
                var processor = new W3CR2RMLProcessor(connection, this.MappingOptions);
                if ((File.GetAttributes(this.MappingPath) & FileAttributes.Directory) != 0)
                {
                    foreach (var path in Directory.GetFiles(this.MappingPath))
                    {
                        this.RunMapping(processor, path);
                    }
                }
                else
                {
                    this.RunMapping(processor, this.MappingPath);
                }

                return processor.Success;
            }
        }

        public override void SaveOutput()
        {
            if (this.OutFile != null)
            {
                this._output.SaveToFile(this.OutFile);
            }
            else
            {
                new NQuadsWriter().Save(this._output, Console.Out);
            }
        }

        private void RunMapping(IR2RMLProcessor processor, string path)
        {
            LogTo.Info($"Processing {path}");
            var rml = R2RMLLoader.LoadFile(path, this.MappingOptions);

            processor.Run(rml, this._output);
        }
    }
}