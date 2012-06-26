using System;
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
        public DatabaseSchemaAdapter(DatabaseReader reader)
            : this(reader, new CoreSQL2008ColumTypeMapper())
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="DatabaseSchemaAdapter"/>
        /// </summary>
        /// <param name="reader">A <see cref="DatabaseReader"/> initialized for reading a desired database type</param>
        /// <param name="columnTypeMapper">Implementation of <see cref="IColumnTypeMapper"/> responsible for transforming column type read by <see cref="DatabaseReader"/> to <see cref="R2RMLType"/></param>
        public DatabaseSchemaAdapter(DatabaseReader reader, IColumnTypeMapper columnTypeMapper)
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

                    // todo: default generation from View?
                    //foreach (var view in _schema.Views)
                    //{
                    //    _tables.Add(TableMetadataFromDatabaseView(view));
                    //}
                }

                return _tables;
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

            table.ForeignKeys = dbTable.ForeignKeys.Select(fk =>
            {
                string[] referencedColumns;
                DatabaseTable referencedTable = fk.ReferencedTable(_schema);
                if (referencedTable.PrimaryKey == null)
                {
                    referencedColumns = referencedTable.UniqueKeys.Single(key => key.Name == fk.RefersToConstraint).Columns.ToArray();
                }
                else
                {
                    referencedColumns = fk.ReferencedColumns(_schema).ToArray();
                }

                return new ForeignKeyMetadata
                {
                    TableName = table.Name,
                    ForeignKeyColumns = fk.Columns.ToArray(),
                    ReferencedColumns = referencedColumns,
                    ReferencedTableName = referencedTable.Name,
                    IsCandidateKeyReference = referencedTable.PrimaryKey == null
                };
            }).ToArray();

            return table;
        }
    }
}
