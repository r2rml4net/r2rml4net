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
using System.Collections.Generic;
using TCode.r2rml4net.Mapping;
using TCode.r2rml4net.Mapping.Direct;
using TCode.r2rml4net.RDB;
using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net
{
    /// <summary>
    /// Provides default mapping by wrapping the <see cref="R2RMLMappingGenerator" />
    /// </summary>
    public class DirectR2RMLMapping : IR2RML
    {
        private readonly R2RMLMappingGenerator _generator;
        private IR2RML _generatedMappings;

        /// <summary>
        /// Creates a default mapping with no base URI
        /// </summary>
        public DirectR2RMLMapping(IDatabaseMetadata provider)
        {
            _generator = new R2RMLMappingGenerator(provider, new FluentR2RML(new MappingOptions()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectR2RMLMapping"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public DirectR2RMLMapping(IDatabaseMetadata provider, MappingOptions options)
        {
            _generator = new R2RMLMappingGenerator(provider, new FluentR2RML(options));
        }

        /// <inheritdoc/>
        public ISqlVersionValidator SqlVersionValidator
        {
            get { return GeneratedMappings.SqlVersionValidator; }
            set { GeneratedMappings.SqlVersionValidator = value; }
        }

        /// <inheritdoc/>
        public ISqlQueryBuilder SqlQueryBuilder
        {
            get { return GeneratedMappings.SqlQueryBuilder; }
            set { GeneratedMappings.SqlQueryBuilder = value; }
        }

        public IGraph MappingsGraph => this.GeneratedMappings.MappingsGraph;

        /// <inheritdoc/>
        public IEnumerable<ITriplesMap> TriplesMaps
        {
            get { return GeneratedMappings.TriplesMaps; }
        }

        private IR2RML GeneratedMappings
        {
            get
            {
                if (_generatedMappings == null)
                {
                    _generatedMappings = _generator.GenerateMappings();
                }

                return _generatedMappings;
            }
        }
    }
}