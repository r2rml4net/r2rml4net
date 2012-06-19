using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseSchemaReader.DataSchema;

namespace TCode.r2rml4net.RDB.DatabaseSchemaReader
{
    public class DatabaseSchemaAdapter : IDatabaseMetadata
    {
        readonly DatabaseSchema _schema;
        TableCollection _tables;

        public DatabaseSchemaAdapter(DatabaseSchema schema)
        {
            _schema = schema;
        }

        public void ReadMetadata() { }

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
                    Type = System.Data.DbType.Object // todo
                });

            table.ForeignKeys = dbTable.ForeignKeys.Select(fk => new ForeignKeyMetadata
            {
                TableName = table.Name,
                ForeignKeyColumns = fk.Columns.ToArray(),
                ReferencedColumns = fk.ReferencedColumns(_schema).ToArray(),
                ReferencedTableName = fk.ReferencedTable(_schema).Name,
                IsCandidateKeyReference = string.IsNullOrEmpty(fk.RefersToConstraint)
            }).ToArray();

            return table;
        }
    }
}
