#region Licence
// Copyright (C) 2012 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
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

namespace TCode.r2rml4net
{
    // todo: elaborate in comments below

    /// <summary>
    /// Represents a set of options for <a href="http://www.w3.org/TR/r2rml/#default-mappings">default mapping generation</a> 
    /// and <a href="http://www.w3.org/TR/r2rml/#generated-rdf">triples generation</a>
    /// </summary>
    public sealed class MappingOptions
    {
        private static readonly object Locker = new object();
        private static MappingOptions _defaultInstance;

        private string _blankNodeTemplateSeparator;
        private const string DefaultTemplateSeparator = "_";
        private const char DefaultIdentifierDelimiter = '\"';

        public static MappingOptions Default
        {
            get
            {
                lock (Locker)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new MappingOptions();
                    }
                }

                return _defaultInstance;
            }
        }

        public static MappingOptions Current
        {
            get { return Scope<MappingOptions>.Current ?? Default; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="MappingOptions"/> with default options' values
        /// </summary>
        public MappingOptions()
        {
            BlankNodeTemplateSeparator = DefaultTemplateSeparator;
            SqlIdentifierRightDelimiter = DefaultIdentifierDelimiter;
            SqlIdentifierLeftDelimiter = DefaultIdentifierDelimiter;
            UseDelimitedIdentifiers = true;
            ValidateSqlVersion = true;
            IgnoreMappingErrors = true;
            IgnoreDataErrors = true;
            PreserveDuplicateRows = false;
            AllowAutomaticBlankNodeSubjects = false;
        }

        /// <summary>
        /// Gets or sets the string which will be used for building blank node <a href="http://www.w3.org/TR/r2rml/#from-template">templates</a>.
        /// Default value is "_"
        /// </summary>
        public string BlankNodeTemplateSeparator
        {
            get { return _blankNodeTemplateSeparator; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _blankNodeTemplateSeparator = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether delimited SQL identifiers should be used in mapping graphs
        /// </summary>
        public bool UseDelimitedIdentifiers { get; set; }

        /// <summary>
        /// Gets the right SQL identifier delimiter. Default value is '\"'
        /// </summary>
        public char SqlIdentifierRightDelimiter { get; private set; }

        /// <summary>
        /// Gets the left SQL identifier delimiter. Default value is '\"'
        /// </summary>
        public char SqlIdentifierLeftDelimiter { get; private set; }

        /// <summary>
        /// Gets or sets value indicating whether <a href="http://www.w3.org/TR/r2rml/#dfn-sql-version-identifier">SQL version identifier</a> should be validated.
        /// Default value is true
        /// </summary>
        public bool ValidateSqlVersion { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether mapping errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreMappingErrors { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether data errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreDataErrors { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether duplicate rows in tables without primary key <a href="http://www.w3.org/TR/r2rml/#default-mappings">should be preserved</a>.
        /// Default value is false
        /// </summary>
        public bool PreserveDuplicateRows { get; set; }

        /// <summary>
        /// Gets or set value indicating whether <a href="http://www.w3.org/TR/r2rml/#dfn-subject-map">subject maps</a> can be neither
        /// a <a href="http://www.w3.org/TR/r2rml/#dfn-template-valued-term-map">template-valued</a> nor 
        /// a <a href="http://www.w3.org/TR/r2rml/#dfn-constant-valued-term-map">constant-valued</a> nor 
        /// a <a href="http://www.w3.org/TR/r2rml/#dfn-column-valued-term-map">column-valued</a>. In such case a distinct automatic blank node
        /// will be created for each logical row.
        /// </summary>
        public bool AllowAutomaticBlankNodeSubjects { get; set; }

        /// <summary>
        /// Sets the SQL identifier delimiters
        /// </summary>
        public void SetSqlIdentifierDelimiters(char newLeftDelimiter, char newRightDelimiter)
        {
            SqlIdentifierLeftDelimiter = newLeftDelimiter;
            SqlIdentifierRightDelimiter = newRightDelimiter;
        }


    }
}