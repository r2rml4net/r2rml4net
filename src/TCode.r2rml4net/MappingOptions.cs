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

namespace TCode.r2rml4net
{
    // todo: elaborate in comments below

    /// <summary>
    /// Represents a set of options for <a href="http://www.w3.org/TR/r2rml/#default-mappings">default mapping generation</a>
    /// and <a href="http://www.w3.org/TR/r2rml/#generated-rdf">triples generation</a>
    /// </summary>
    public sealed class MappingOptions
    {
        private const string DefaultTemplateSeparator = "_";
        private const char DefaultIdentifierDelimiter = '\"';
        private static readonly Lazy<MappingOptions> DefaultInstance = new Lazy<MappingOptions>(CreateDefaultInstance);

        /// <summary>
        /// Initializes a new instance of <see cref="MappingOptions"/> with default options' values
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
        /// Gets a singleton instance with default values of <see cref="MappingOptions" />
        /// </summary>
        public static MappingOptions Default
        {
            get { return DefaultInstance.Value; }
        }

        /// <summary>
        /// Gets the <see cref="MappingOptions"/> from the current <see cref="MappingScope"/> or the <see cref="Default"/> options
        /// </summary>
        public static MappingOptions Current
        {
            get { return MappingScope.Current ?? Default; }
        }

        /// <summary>
        /// Gets the string which will be used for building blank node <a href="http://www.w3.org/TR/r2rml/#from-template">templates</a>.
        /// Default value is "_"
        /// </summary>
        public string BlankNodeTemplateSeparator { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether delimited SQL identifiers should be used in mapping graphs
        /// </summary>
        public bool UseDelimitedIdentifiers { get; internal set; }

        /// <summary>
        /// Gets the right SQL identifier delimiter. Default value is '\"'
        /// </summary>
        public char SqlIdentifierRightDelimiter { get; internal set; }

        /// <summary>
        /// Gets the left SQL identifier delimiter. Default value is '\"'
        /// </summary>
        public char SqlIdentifierLeftDelimiter { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether <a href="http://www.w3.org/TR/r2rml/#dfn-sql-version-identifier">SQL version identifier</a> should be validated.
        /// Default value is true
        /// </summary>
        public bool ValidateSqlVersion { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether mapping errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreMappingErrors { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether data errors should be ignored.
        /// Default value is true
        /// </summary>
        public bool IgnoreDataErrors { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether duplicate rows in tables without primary key <a href="http://www.w3.org/TR/r2rml/#default-mappings">should be preserved</a>.
        /// Default value is false
        /// </summary>
        public bool PreserveDuplicateRows { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether <a href="http://www.w3.org/TR/r2rml/#dfn-subject-map">subject maps</a> can be neither
        /// a <a href="http://www.w3.org/TR/r2rml/#dfn-template-valued-term-map">template-valued</a> nor
        /// a <a href="http://www.w3.org/TR/r2rml/#dfn-constant-valued-term-map">constant-valued</a> nor
        /// a <a href="http://www.w3.org/TR/r2rml/#dfn-column-valued-term-map">column-valued</a>. In such case a distinct automatic blank node
        /// will be created for each logical row.
        /// </summary>
        public bool AllowAutomaticBlankNodeSubjects { get; internal set; }

        /// <summary>
        /// Sets the SQL identifier delimiters
        /// </summary>
        public MappingOptions WithSqlIdentifierDelimiters(char newLeftDelimiter, char newRightDelimiter)
        {
            SqlIdentifierLeftDelimiter = newLeftDelimiter;
            SqlIdentifierRightDelimiter = newRightDelimiter;

            return this;
        }

        /// <summary>
        /// Sets the SQL identifier delimiters
        /// </summary>
        public MappingOptions WithSqlIdentifierDelimiters(char newDelimiter)
        {
            SqlIdentifierLeftDelimiter = newDelimiter;
            SqlIdentifierRightDelimiter = newDelimiter;

            return this;
        }

        /// <summary>
        /// Sets the <see cref="ValidateSqlVersion"/> setting
        /// </summary>
        public MappingOptions WithSqlVersionValidation(bool validateSqlVersion)
        {
            ValidateSqlVersion = validateSqlVersion;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="UseDelimitedIdentifiers"/> setting
        /// </summary>
        public MappingOptions UsingDelimitedIdentifiers(bool useDelimitedIdentifiers)
        {
            UseDelimitedIdentifiers = useDelimitedIdentifiers;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="BlankNodeTemplateSeparator"/> setting
        /// </summary>
        public MappingOptions WithBlankNodeTemplateSeparator(string blankNodeTemplateSeparator)
        {
            BlankNodeTemplateSeparator = blankNodeTemplateSeparator;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IgnoreDataErrors"/> setting
        /// </summary>
        public MappingOptions IgnoringDataErrors(bool ignoreDataErrors)
        {
            IgnoreDataErrors = ignoreDataErrors;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="AllowAutomaticBlankNodeSubjects"/> setting
        /// </summary>
        public MappingOptions WithAutomaticBlankNodeSubjects(bool allowAutomaticBlankNodeSubjects)
        {
            AllowAutomaticBlankNodeSubjects = allowAutomaticBlankNodeSubjects;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="PreserveDuplicateRows"/> setting
        /// </summary>
        public MappingOptions WithDuplicateRowsPreserved(bool preserveDulicateRows)
        {
            PreserveDuplicateRows = preserveDulicateRows;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IgnoreMappingErrors"/> setting
        /// </summary>
        public MappingOptions IgnoringMappingErrors(bool ignoreMappingErrors)
        {
            IgnoreMappingErrors = ignoreMappingErrors;
            return this;
        }

        private static MappingOptions CreateDefaultInstance()
        {
            return new MappingOptions();
        }
    }
}