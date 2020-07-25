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
using System.Linq;
using Anotar.NLog;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Direct
{
    /// <summary>
    /// Default implementation of <see cref="IPrimaryKeyMappingStrategy"/>, which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class PrimaryKeyMappingStrategy : MappingStrategyBase, IPrimaryKeyMappingStrategy
    {
        public PrimaryKeyMappingStrategy(MappingOptions options) : base(options)
        {
        }

        /// <summary>
        /// Creates a URI for subject class by joining <paramref name="baseUri"/> and <paramref name="tableName"/>
        /// </summary>
        public Uri CreateSubjectClassUri(Uri baseUri, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Invalid table name");
            }

            return new Uri(baseUri, MappingHelper.UrlEncode(tableName));
        }

        /// <summary>
        /// Creates a blank node identifier subject template by concatenating the referenced table name with the referenced columns
        /// </summary>
        /// <example>For table "Student" and referenced columns "Last Name" and "SSN" it creates a template "Student;{\"Last Name\"};{\"SSN\"}"</example>
        /// <remarks>If the referenced table has multiple unique keys the template will be created for the longest one. <br/>
        /// If the referenced table has no unique key, all columns are used</remarks>
        public virtual string CreateSubjectTemplateForNoPrimaryKey(TableMetadata table)
        {
            var uniqueKeys = table.UniqueKeys.ToArray();
            var referencedUniqueKeys = uniqueKeys.Where(uq => uq.IsReferenced).ToArray();
            if (referencedUniqueKeys.Length > 1)
            {
                LogTo.Info("Multiple Composite Key references to table {0}", table.Name);
            }

            ColumnCollection columnsForTemplate;

            if (uniqueKeys.Any())
            {
                if (referencedUniqueKeys.Length == 1)
                {
                    columnsForTemplate = referencedUniqueKeys.Single();
                }
                else
                {
                    columnsForTemplate = uniqueKeys.OrderBy(c => c.ColumnsCount).First();
                }
            }
            else
            {
                columnsForTemplate = table;
            }

            var columnsArray = columnsForTemplate.Select(c => c.Name).ToArray();
            var name = table.Name;
            if (!columnsArray.Any())
            {
                throw new InvalidMapException(string.Format("No columns for table {0}", name));
            }

            return CreateBlankNodeTemplate(name, columnsArray);
        }

        /// <summary>
        /// Creates a blank node identifier subject template for referenced table with primary key
        /// </summary>
        public virtual string CreateSubjectTemplateForPrimaryKey(Uri baseUri, TableMetadata table)
        {
            if (!table.PrimaryKey.Any())
            {
                throw new ArgumentException(string.Format("Table {0} has no primary key", table.Name));
            }

            string template = CreateSubjectClassUri(baseUri, table.Name).OriginalString;
            template += "/" + string.Join(";", table.PrimaryKey.Select(pk => string.Format("{0}={1}", MappingHelper.UrlEncode(pk), MappingHelper.EncloseColumnName(pk, this.Options))));
            return template;
        }
    }
}