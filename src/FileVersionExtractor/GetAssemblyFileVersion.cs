#region Licence
// Copyright (C) 2012-2018 Tomasz Pluskiewicz
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
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace FileVersionExtractor
{
    public class GetAssemblyFileVersion : ITask
    {
        private const string Pattern = @"(?:AssemblyInformationalVersion\("")(?<ver>.+)(?:""\))";

        [Required]
        public string FilePathAssemblyInfo { get; set; }

        [Output]
        public string AssemblyFileVersion { get; set; }

        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            StreamReader streamreaderAssemblyInfo = null;
            AssemblyFileVersion = String.Empty;
            try
            {
                streamreaderAssemblyInfo = new StreamReader(FilePathAssemblyInfo);
                string strLine;
                while ((strLine = streamreaderAssemblyInfo.ReadLine()) != null)
                {
                    Match matchVersion = Regex.Match(
                        strLine,
                        Pattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                    if (!matchVersion.Success)
                    {
                        continue;
                    }

                    var groupVersion = matchVersion.Groups["ver"];
                    if ((!groupVersion.Success) || (String.IsNullOrEmpty(groupVersion.Value)))
                    {
                        continue;
                    }
                    
                    AssemblyFileVersion = groupVersion.Value;
                    break;
                }
            }
            catch (Exception e)
            {
                var args = new BuildMessageEventArgs(e.Message, string.Empty, "GetAssemblyFileVersion", MessageImportance.High);
                BuildEngine.LogMessageEvent(args);
            }
            finally
            {
                if (streamreaderAssemblyInfo != null)
                {
                    streamreaderAssemblyInfo.Close();
                }
            }

            return (true);
        }
    }
}