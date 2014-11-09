#region Licence
// Copyright (C) 2012-2014 Tomasz Pluskiewicz
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
using System.Text;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Direct
{
    /// <summary>
    /// Default implementation of <see cref="IForeignKeyMappingStrategy"/>, which creates mapping graph
    /// consistent with the official <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
    /// </summary>
    public class ForeignKeyMappingStrategy : MappingStrategyBase, IForeignKeyMappingStrategy
    {
        private IPrimaryKeyMappingStrategy _primaryKeyMappingStrategy;

        /// <summary>
        /// Gets or sets mapping strategy for primary keys
        /// </summary>
        public IPrimaryKeyMappingStrategy PrimaryKeyMappingStrategy
        {
            get
            {
                if (_primaryKeyMappingStrategy == null)
                {
                    _primaryKeyMappingStrategy = new PrimaryKeyMappingStrategy();
                }

                return _primaryKeyMappingStrategy;
            }

            set
            {
                _primaryKeyMappingStrategy = value;
            }
        }

        /// <summary>
        /// Creates a predicate URI for foreign Key according to <a href="www.w3.org/TR/rdb-direct-mapping/">Direct Mapping specfication</a>
        /// </summary>
        /// <example>For referenced table "Student", foreign key columns "Last Name" and "SSN" and base URI "http://www.exmample.com/" it creates a 
        /// URI "http://www.exmample.com/Student#ref-{\"Last Name\"};{\"SSN\"}"</example>
        public virtual Uri CreateReferencePredicateUri(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            if (string.IsNullOrWhiteSpace(foreignKey.TableName))
            {
                throw new ArgumentException("Invalid referencing table's name");
            }

            if (!foreignKey.ForeignKeyColumns.Any())
            {
                throw new ArgumentException("Empty foreign key", "foreignKey");
            }

            string uri = string.Format(
                "{0}{1}#ref-{2}", 
                baseUri, 
                MappingHelper.UrlEncode(foreignKey.TableName), 
                string.Join(";", foreignKey.ForeignKeyColumns.Select(MappingHelper.UrlEncode)));

            return new Uri(uri);
        }

        /// <summary>
        /// Creates an object template for a foreign key reference
        /// </summary>
        /// <remarks>The template contains both referenced and referencing columns. Different columns are used
        /// if the referenced table has or hasn't got a primary key producing different templates</remarks>
        public virtual string CreateReferenceObjectTemplate(Uri baseUri, ForeignKeyMetadata foreignKey)
        {
            if (!foreignKey.ForeignKeyColumns.Any())
            {
                throw new ArgumentException("Empty foreign key", "foreignKey");
            }

            if (foreignKey.ForeignKeyColumns.Length != foreignKey.ReferencedColumns.Length)
            {
                throw new ArgumentException(string.Format("Foreign key columns count mismatch in table {0}", foreignKey.TableName), "foreignKey");
            }

            if (foreignKey.IsCandidateKeyReference && (!foreignKey.ReferencedTableHasPrimaryKey || !foreignKey.ReferencedTable.PrimaryKey.Any()))
            {
                throw new ArgumentException(
                    string.Format(
                        "Canditate key reference between tables {0} and {1} but table {1} has no primary key",
                        foreignKey.TableName, 
                        foreignKey.ReferencedTable.Name));
            }

            string[] referencedColumns = foreignKey.IsCandidateKeyReference && foreignKey.ReferencedTableHasPrimaryKey
                ? foreignKey.ReferencedTable.PrimaryKey.ToArray()
                : foreignKey.ReferencedColumns;
            string[] foreignKeyColumns = foreignKey.IsCandidateKeyReference && foreignKey.ReferencedTableHasPrimaryKey
                ? foreignKey.ReferencedTable.PrimaryKey.Select(c => string.Format("{0}{1}", foreignKey.ReferencedTable.Name, c)).ToArray()
                : foreignKey.ForeignKeyColumns;

            StringBuilder template = new StringBuilder(PrimaryKeyMappingStrategy.CreateSubjectClassUri(baseUri, foreignKey.ReferencedTable.Name) + "/");
            template.AppendFormat("{0}={1}", MappingHelper.UrlEncode(referencedColumns[0]), MappingHelper.EncloseColumnName(foreignKeyColumns[0]));
            for (int i = 1; i < foreignKeyColumns.Length; i++)
            {
                template.AppendFormat(";{0}={1}", MappingHelper.UrlEncode(referencedColumns[i]), MappingHelper.EncloseColumnName(foreignKeyColumns[i]));
            }

            return template.ToString();
        }

        /// <summary>
        /// Creates a blank node object identifier template for foreign key, which references a candidate key.
        /// See <see cref="MappingStrategyBase.CreateBlankNodeTemplate"/> for details on implementation
        /// </summary>
        public string CreateObjectTemplateForCandidateKeyReference(ForeignKeyMetadata foreignKey)
        {
            if (!foreignKey.IsCandidateKeyReference)
            {
                throw new ArgumentException(
                    string.Format(
                        "Canditate key reference expected but was primary key reference between tables {0} and {1}",
                        foreignKey.TableName, 
                        foreignKey.ReferencedTable.Name));
            }

            return CreateBlankNodeTemplate(foreignKey.ReferencedTable.Name, foreignKey.ForeignKeyColumns);
        }
    }
}