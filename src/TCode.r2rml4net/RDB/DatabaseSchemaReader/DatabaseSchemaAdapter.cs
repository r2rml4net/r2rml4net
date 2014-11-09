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
using System.Collections.Generic;
using System.Linq;
using DatabaseSchemaReader.DataSchema;
using DatabaseSchemaReader;

namespace TCode.r2rml4net.RDB.DatabaseSchemaReader
{
    /// <summary>
    /// Implementation of <see cref="IDatabaseMetadata"/> using <a href="http://dbschemareader.codeplex.com/">Database Schema Reader project</a>
    /// </summary>
    public class DatabaseSchemaAdapter : IDatabaseMetadata
    {
        readonly DatabaseSchema _schema;
        TableCollection _tables;

        internal IColumnTypeMapper ColumnTypeMapper { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="DatabaseSchemaAdapter"/>
        /// </summary>
        /// <param name="reader">A <see cref="DatabaseReader"/> initialized for reading a desired database type</param>
        /// <param name="columnTypeMapper">Implementation of <see cref="IColumnTypeMapper"/> responsible for transforming column type read by <see cref="DatabaseReader"/> to <see cref="R2RMLType"/></param>
        public DatabaseSchemaAdapter(IDatabaseReader reader, IColumnTypeMapper columnTypeMapper)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (columnTypeMapper == null)
                throw new ArgumentNullException("columnTypeMapper");

            ColumnTypeMapper = columnTypeMapper;
            reader.ReadAll();
            _schema = reader.DatabaseSchema;
        }

        /// <summary>
        /// Reads database schema if required and gets all tables
        /// </summary>
        public TableCollection Tables
        {
            get
            {
                if (_tables == null)
                {
                    _tables = new TableCollection();

                    foreach (var dbTable in _schema.Tables)
                    {
                        _tables.Add(TableMetadataFromDatabaseTable(dbTable));
                    }
                    foreach (var dbTable in _schema.Tables)
                    {
                        ReadKeysMetadata(dbTable);
                    }

                    MarkReferencedUniqueKeys();

                    // todo: default generation from View?
                    //foreach (var view in _schema.Views)
                    //{
                    //    _tables.Add(TableMetadataFromDatabaseView(view));
                    //}
                }

                return _tables;
            }
        }

        private void ReadKeysMetadata(DatabaseTable dbTable)
        {
            var table = Tables[dbTable.Name];
            table.ForeignKeys = GetForeignKeys(dbTable).ToArray();

            foreach (var uniqueKey in dbTable.UniqueKeys)
            {
                var keyMetadata = new UniqueKeyMetadata();

                foreach (var column in uniqueKey.Columns)
                {
                    keyMetadata.Add(table[column]);
                }

                table.UniqueKeys.Add(keyMetadata);
            }
        }

        private void MarkReferencedUniqueKeys()
        {
            foreach (var table in _tables.Where(t => t.ForeignKeys.Any()))
            {
                foreach (var foreignKey in table.ForeignKeys.Where(fk => fk.IsCandidateKeyReference))
                {
                    foreach (var uniqueKey in foreignKey.ReferencedTable.UniqueKeys)
                    {
                        if (foreignKey.ReferencedColumns.Except(uniqueKey.Select(c => c.Name)).Any()) continue;
                        uniqueKey.IsReferenced = true;
                    }
                }
            }
        }

        private TableMetadata TableMetadataFromDatabaseTable(DatabaseTable dbTable)
        {
            TableMetadata table = new TableMetadata { Name = dbTable.Name };

            foreach (var column in dbTable.Columns)
                table.Add(new ColumnMetadata
                {
                    Name = column.Name,
                    Table = table,
                    IsPrimaryKey = column.IsPrimaryKey,
                    Type = ColumnTypeMapper.GetColumnTypeFromColumn(column.DataType)
                });

            return table;
        }

        private IEnumerable<ForeignKeyMetadata> GetForeignKeys(DatabaseTable table)
        {
            foreach (var fk in table.ForeignKeys)
            {
                bool isCandidateKey;
                string[] referencedColumns;
                DatabaseTable referencedTable = fk.ReferencedTable(_schema);
                if (referencedTable.PrimaryKey == null
                    || referencedTable.PrimaryKey.Name != fk.RefersToConstraint)
                {
                    isCandidateKey = true;
                    referencedColumns =
                        referencedTable.UniqueKeys.Single(key => key.Name == fk.RefersToConstraint).Columns.ToArray();
                }
                else
                {
                    isCandidateKey = false;
                    referencedColumns = fk.ReferencedColumns(_schema).ToArray();
                }
                
                var referencedTableMeta = Tables[referencedTable.Name];
                yield return new ForeignKeyMetadata
                    {
                        TableName = table.Name,
                        ForeignKeyColumns = fk.Columns.ToArray(),
                        ReferencedColumns = referencedColumns,
                        ReferencedTable = referencedTableMeta,
                        IsCandidateKeyReference = isCandidateKey,
                        ReferencedTableHasPrimaryKey = referencedTableMeta.PrimaryKey.Any()
                    };
            }
        }
    }
}
