﻿#region Licence
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NullGuard;
using TCode.r2rml4net.Exceptions;
using TCode.r2rml4net.Extensions;
using TCode.r2rml4net.RDF;
using VDS.RDF;
using VDS.RDF.Query;

namespace TCode.r2rml4net.Mapping.Fluent
{
    /// <summary>
    /// Implementation of fluent configuration interface for <a href="http://www.w3.org/TR/r2rml/#triples-map">Triples Maps</a>
    /// </summary>
    [NullGuard(ValidationFlags.All)]
    internal class TriplesMapConfiguration : BaseConfiguration, ITriplesMapFromR2RMLViewConfiguration
    {
        private readonly MappingOptions _options;
        private static readonly Regex TableNameRegex = new Regex(@"([\p{L}0-9 _]+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly IR2RMLConfiguration _r2RMLConfiguration;
        private readonly IList<PredicateObjectMapConfiguration> _predicateObjectMaps = new List<PredicateObjectMapConfiguration>();
        private SubjectMapConfiguration _subjectMapConfiguration;

        internal TriplesMapConfiguration(TriplesMapConfigurationStub triplesMapConfigurationStub, INode node,
            MappingOptions options)
            : base(triplesMapConfigurationStub.R2RMLMappings, node)
        {
            _options = options;
            _r2RMLConfiguration = triplesMapConfigurationStub.R2RMLConfiguration;
        }

        /// <inheritdoc/>
        public string TableName
        {
            [return: AllowNull]
            get
            {
                var sparqlQuery = new SparqlParameterizedString(
@"PREFIX rr: <http://www.w3.org/ns/r2rml#>

SELECT ?tableName ?triplesMap
WHERE 
{{
    ?triplesMap rr:logicalTable ?lt .
    ?lt rr:tableName ?tableName .
}}");

                var result = ((SparqlResultSet)R2RMLMappings.ExecuteQuery(sparqlQuery))
                    .Where(r => Node.Equals(r["triplesMap"]))
                    .ToList();

                if (result.Count > 1)
                {
                    throw new InvalidMapException("Triples map contains multiple table names", this);
                }

                if (result.Count == 1)
                {
                    return result[0].Value("tableName").ToString();
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public string SqlQuery
        {
            [return: AllowNull]
            get
            {
                var sparqlQuery = new SparqlParameterizedString(
@"PREFIX rr: <http://www.w3.org/ns/r2rml#>

SELECT ?sqlQuery ?triplesMap
WHERE 
{{
    ?triplesMap rr:logicalTable ?lt .
    ?lt rr:sqlQuery ?sqlQuery 
}}");
                var result = ((SparqlResultSet)R2RMLMappings.ExecuteQuery(sparqlQuery))
                    .Where(r => Node.Equals(r["triplesMap"]))
                    .ToList();

                if (result.Count > 1)
                {
                    throw new InvalidMapException("Triples map contains multiple SQL queries", this);
                }

                if (result.Count == 1)
                {
                    return result[0].Value("sqlQuery").ToString();
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public Uri Uri
        {
            get
            {
                if (Node == null || !(Node is IUriNode))
                {
                    return null;
                }

                return (Node as IUriNode).Uri;
            }
        }

        /// <inheritdoc/>
        ISubjectMapConfiguration ITriplesMapConfiguration.SubjectMap
        {
            get
            {
                if (_subjectMapConfiguration == null)
                {
                    _subjectMapConfiguration = new SubjectMapConfiguration(this, R2RMLMappings);
                }

                return _subjectMapConfiguration;
            }
        }

        /// <summary>
        /// Gets the effective sql query based on <see cref="TableName"/> or <see cref="SqlQuery"/>
        /// </summary>
        /// <remarks>See http://www.w3.org/TR/r2rml/#dfn-effective-sql-query, http://www.w3.org/TR/r2rml/#physical-tables and http://www.w3.org/TR/r2rml/#r2rml-views</remarks>
        public string EffectiveSqlQuery
        {
            get
            {
                return R2RMLConfiguration.SqlQueryBuilder.GetEffectiveQueryForTriplesMap(this);
            }
        }

        public IR2RMLConfiguration R2RMLConfiguration
        {
            get { return _r2RMLConfiguration; }
        }

        /// <inheritdoc/>
        public Uri[] SqlVersions
        {
            get
            {
                return (from obj in LogicalTableNode.GetObjects(R2RMLUris.RrSqlVersionProperty)
                        select obj.GetUri()).ToArray();
            }
        }

        public IEnumerable<IPredicateObjectMap> PredicateObjectMaps
        {
            get { return _predicateObjectMaps; }
        }

        public ISubjectMap SubjectMap
        {
            get
            {
                if (_subjectMapConfiguration == null)
                {
                    _subjectMapConfiguration = new SubjectMapConfiguration(this, R2RMLMappings);
                }

                return _subjectMapConfiguration;
            }
        }

        protected internal override ITriplesMapConfiguration TriplesMap
        {
            get { return this; }
        }

        private IBlankNode LogicalTableNode
        {
            get
            {
                var logicalTable = Node.GetObjects(R2RMLUris.RrLogicalTableProperty)
                                       .GetSingleOrDefault(nodes => new InvalidMapException("Triples Map contains multiple logical tables!", this));

                if (logicalTable == null)
                {
                    throw new InvalidMapException("Triples Map contains no logical tables!", this);
                }

                return logicalTable as IBlankNode;
            }
        }

        /// <summary>
        /// <see cref="ITriplesMapConfiguration.CreatePropertyObjectMap"/>
        /// </summary>
        public IPredicateObjectMapConfiguration CreatePropertyObjectMap()
        {
            var propertyObjectMap = new PredicateObjectMapConfiguration(this, R2RMLMappings);
            _predicateObjectMaps.Add(propertyObjectMap);
            return propertyObjectMap;
        }

        /// <summary>
        /// Asserts this Triples Map's SQL query as a query of type defined by <paramref name="uri"/> parmeter
        /// </summary>
        /// <param name="uri">Usually on of the URIs listed on http://www.w3.org/2001/sw/wiki/RDB2RDF/SQL_Version_IRIs </param>
        public ITriplesMapFromR2RMLViewConfiguration SetSqlVersion(Uri uri)
        {
            if (TableName != null)
            {
                throw new InvalidMapException("Cannot set SQL version to a table-based logical table", this);
            }

            if (this._options.ValidateSqlVersion && R2RMLConfiguration.SqlVersionValidator.SqlVersionIsValid(uri) == false)
            {
                throw new InvalidSqlVersionException(uri);
            }

            R2RMLMappings.Assert(LogicalTableNode, R2RMLMappings.CreateUriNode(R2RMLUris.RrSqlVersionProperty), R2RMLMappings.CreateUriNode(uri));

            return this;
        }

        /// <summary>
        /// <see cref="SetSqlVersion(System.Uri)"/>
        /// </summary>
        public ITriplesMapFromR2RMLViewConfiguration SetSqlVersion(string uri)
        {
            return this.SetSqlVersion(new Uri(uri));
        }

        internal static TriplesMapConfiguration FromSqlQuery(TriplesMapConfigurationStub triplesMapConfigurationStub, string sqlQuery, MappingOptions options)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                throw new ArgumentOutOfRangeException("sqlQuery");
            }

            INode node = AssertSqlQueryTriples(triplesMapConfigurationStub.R2RMLMappings, sqlQuery);

            return new TriplesMapConfiguration(triplesMapConfigurationStub, node, options);
        }

        internal static TriplesMapConfiguration FromTable(
            TriplesMapConfigurationStub triplesMapConfigurationStub,
            string tableName,
            MappingOptions options)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentOutOfRangeException("tableName");
            }

            string tablename = TrimTableName(tableName);
            if (tablename == string.Empty)
            {
                throw new ArgumentOutOfRangeException("tableName", "The table name seems invalid");
            }

            INode node = AssertTableNameTriples(triplesMapConfigurationStub.R2RMLMappings, tablename);

            return new TriplesMapConfiguration(triplesMapConfigurationStub, node, options);
        }

        protected override void InitializeSubMapsFromCurrentGraph()
        {
            CreateSubMaps(R2RMLUris.RrPredicateObjectMapPropety, (graph, node) => new PredicateObjectMapConfiguration(this, graph, node), _predicateObjectMaps);

            var subjectMaps = new List<SubjectMapConfiguration>();
            CreateSubMaps(R2RMLUris.RrSubjectMapProperty, (graph, node) => new SubjectMapConfiguration(this, graph, node), subjectMaps);

            if (subjectMaps.Count > 1)
            {
                throw new InvalidMapException("Triples map can only have one subject map");
            }

            _subjectMapConfiguration = subjectMaps.SingleOrDefault();
        }

        private static string TrimTableName(string tablename)
        {
            var regexMatch = TableNameRegex.Match(tablename);

            StringBuilder stringBuilder = new StringBuilder(tablename.Length);

            if (regexMatch.Success)
            {
                stringBuilder.Append(regexMatch.Value);
                regexMatch = regexMatch.NextMatch();

                while (regexMatch.Success)
                {
                    stringBuilder.AppendFormat(".{0}", regexMatch.Value);
                    regexMatch = regexMatch.NextMatch();
                }
            }

            return stringBuilder.ToString();
        }

        private static INode AssertTableNameTriples(IGraph r2RMLMappings, string tablename)
        {
            var node = r2RMLMappings.CreateUriNode(new Uri(string.Format("{0}TriplesMap", tablename), UriKind.Relative));

            IBlankNode tableDefinition;
            AssertTriplesMapsTriples(r2RMLMappings, node, out tableDefinition);

            var tableName = r2RMLMappings.CreateUriNode(R2RMLUris.RrTableNameProperty);
            var tableNameLiteral = r2RMLMappings.CreateLiteralNode(tablename);

            r2RMLMappings.Assert(tableDefinition, tableName, tableNameLiteral);

            return node;
        }

        private static INode AssertSqlQueryTriples(IGraph r2RMLMappings, string sqlQuery)
        {
            var node = r2RMLMappings.CreateBlankNode();

            IBlankNode tableDefinition;
            AssertTriplesMapsTriples(r2RMLMappings, node, out tableDefinition);

            var sqlQueryLiteral = r2RMLMappings.CreateLiteralNode(sqlQuery);
            var sqlQueryProperty = r2RMLMappings.CreateUriNode(R2RMLUris.RrSqlQueryProperty);

            r2RMLMappings.Assert(tableDefinition, sqlQueryProperty, sqlQueryLiteral);

            return node;
        }

        private static void AssertTriplesMapsTriples(IGraph r2RMLMappings, INode treipleMapsNode, out IBlankNode tableDefinition)
        {
            var tripleMapClass = r2RMLMappings.CreateUriNode(R2RMLUris.RrTriplesMapClass);
            var type = r2RMLMappings.CreateUriNode(R2RMLUris.RdfType);
            var logicalTable = r2RMLMappings.CreateUriNode(R2RMLUris.RrLogicalTableProperty);
            tableDefinition = r2RMLMappings.CreateBlankNode();

            r2RMLMappings.Assert(treipleMapsNode, type, tripleMapClass);
            r2RMLMappings.Assert(treipleMapsNode, logicalTable, tableDefinition);
        }
    }
}